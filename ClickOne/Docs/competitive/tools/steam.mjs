#!/usr/bin/env node
// steam.mjs - Steam public-endpoint helper for competitive research. Node 18+, no dependencies.
//
// Usage (see PLAYBOOK.md section 7):
//   node steam.mjs search  "term" ["term" ...]            id | name | date | price | reviews | blurb
//   node steam.mjs details <appid> [<appid> ...]          store facts, languages, spec, news titles, screenshot URLs
//   node steam.mjs summary <appid> [<appid> ...]          review totals, all languages and English
//   node steam.mjs reviews <appid> <positive|negative|all> <english|all> [outfile.json]
//                                                         every review of that type (paginated); raw JSON to outfile,
//                                                         else prints the 25 most-upvoted
//   node steam.mjs news    <appid>                        full patch-note text, dates converted from unix seconds
//   node steam.mjs tags    <appid> [<appid> ...]          user tags from the store page HTML (not in the API)
//   node steam.mjs find   <tagId[,tagId...]> [max] [sort] [filter]  games carrying ALL the tags (ids from store.steampowered.com/tagdata/populartags/english);
//                                                         sort = Reviews_DESC | Released_DESC | - (relevance); 4th arg filter = comingsoon for unreleased games (default search hides them); prints id | name | released | % | reviews | price (review counts are ENGLISH only; use `summary` for totals)
//   node steam.mjs threads <appid> [limit] [maxReplies]   community discussion threads with authors and replies (default 40 threads, 20 replies each; text is
//                                                         embedded as JSON in the page; developer replies show their handle)
//
// Dates: ISteamNews and review timestamps are unix SECONDS; this script converts with new Date(sec * 1000).

const WAIT_MS = 600;
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
const iso = (sec) => (sec ? new Date(sec * 1000).toISOString().slice(0, 10) : "?");
const strip = (html = "") =>
  html
    .replace(/<br\s*\/?>/gi, "\n")
    .replace(/<\/p>/gi, "\n")
    .replace(/<[^>]+>/g, "")
    .replace(/&quot;/g, '"')
    .replace(/&amp;/g, "&")
    .replace(/&#39;|&apos;/g, "'")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/\n{3,}/g, "\n\n")
    .trim();

async function getJson(url, tries = 3) {
  for (let i = 0; i < tries; i++) {
    const res = await fetch(url, { headers: { "user-agent": "competitive-research/1.0" } });
    if (res.ok) {
      const text = await res.text();
      try {
        return JSON.parse(text);
      } catch {
        /* fall through to retry */
      }
    }
    await sleep(WAIT_MS * (i + 2));
  }
  return null;
}

async function search(terms) {
  for (const term of terms) {
    const data = await getJson(
      `https://store.steampowered.com/api/storesearch/?term=${encodeURIComponent(term)}&l=english&cc=us`,
    );
    console.log(`\n== search: ${term} ==`);
    for (const it of data?.items ?? []) {
      const price = it.price ? `$${(it.price.final / 100).toFixed(2)}` : "free/none";
      console.log(`${it.id} | ${it.name} | ${it.type} | ${price}`);
    }
    await sleep(WAIT_MS);
  }
}

async function details(ids) {
  for (const id of ids) {
    const j = await getJson(`https://store.steampowered.com/api/appdetails?appids=${id}&cc=us&l=english`);
    const d = j?.[id]?.data;
    console.log(`\n== details: ${id} ==`);
    if (!d) {
      console.log("no data (wait and retry before concluding there is none)");
      continue;
    }
    console.log(`name: ${d.name} (${d.type})`);
    console.log(`release: ${d.release_date?.date} (coming_soon=${d.release_date?.coming_soon})`);
    console.log(`price: ${d.price_overview?.final_formatted ?? (d.is_free ? "free" : "not listed")}`);
    console.log(`developers: ${(d.developers ?? []).join(", ")} | publishers: ${(d.publishers ?? []).join(", ")}`);
    console.log(`platforms: ${Object.entries(d.platforms ?? {}).filter(([, v]) => v).map(([k]) => k).join(", ")}`);
    console.log(`controller: ${d.controller_support ?? "none"}`);
    console.log(`genres: ${(d.genres ?? []).map((g) => g.description).join(", ")}`);
    console.log(`categories: ${(d.categories ?? []).map((g) => g.description).join(", ")}`);
    console.log(`languages: ${strip(d.supported_languages)}`);
    console.log(`demos: ${(d.demos ?? []).map((x) => x.appid).join(", ") || "none"} | dlc: ${(d.dlc ?? []).join(", ") || "none"}`);
    console.log(`ai disclosure: ${d.ai_content_description || "none on the page"}`);
    console.log(`short: ${d.short_description}`);
    console.log(`\n-- description --\n${strip(d.detailed_description)}`);
    console.log(`\n-- pc min --\n${strip(d.pc_requirements?.minimum)}`);
    console.log(`\n-- pc rec --\n${strip(d.pc_requirements?.recommended)}`);
    console.log(`\n-- screenshots --`);
    (d.screenshots ?? []).forEach((s, i) => console.log(`${i + 1}. ${s.path_full}`));
    console.log(`\n-- movies --`);
    (d.movies ?? []).forEach((m, i) =>
      console.log(`${i + 1}. ${m.name} | ${m.mp4?.max ?? m.mp4?.["480"] ?? m.webm?.max ?? ""}`),
    );
    const news = await getJson(
      `https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=${id}&count=100&maxlength=0&format=json`,
    );
    console.log(`\n-- news titles --`);
    for (const n of news?.appnews?.newsitems ?? []) console.log(`${iso(n.date)} | ${n.title}`);
    await sleep(WAIT_MS);
  }
}

async function summary(ids) {
  for (const id of ids) {
    const all = await getJson(
      `https://store.steampowered.com/appreviews/${id}?json=1&num_per_page=0&language=all&purchase_type=all`,
    );
    await sleep(WAIT_MS);
    const en = await getJson(
      `https://store.steampowered.com/appreviews/${id}?json=1&num_per_page=0&language=english&purchase_type=all`,
    );
    const f = (s) => {
      if (!s) return "no data";
      const t = s.total_reviews;
      const pct = t ? ((s.total_positive / t) * 100).toFixed(1) : "n/a";
      return `${s.review_score_desc || "No reviews"} | total ${t} | ${pct}% positive (${s.total_positive} pos / ${s.total_negative} neg)`;
    };
    console.log(`\n== summary: ${id} ==\nall:     ${f(all?.query_summary)}\nenglish: ${f(en?.query_summary)}`);
    await sleep(WAIT_MS);
  }
}

async function reviews(id, type, lang, outfile) {
  const out = [];
  let cursor = "*";
  for (let page = 0; page < 200; page++) {
    const u =
      `https://store.steampowered.com/appreviews/${id}?json=1&filter=recent&review_type=${type}` +
      `&language=${lang}&purchase_type=all&num_per_page=100&cursor=${encodeURIComponent(cursor)}`;
    const j = await getJson(u);
    const batch = j?.reviews ?? [];
    if (!batch.length) break;
    out.push(...batch);
    if (!j.cursor || j.cursor === cursor) break;
    cursor = j.cursor;
    await sleep(WAIT_MS);
  }
  if (outfile) {
    const { writeFileSync } = await import("node:fs");
    writeFileSync(outfile, JSON.stringify(out, null, 2));
    console.log(`saved ${out.length} ${type} ${lang} reviews to ${outfile}`);
    return;
  }
  out.sort((a, b) => b.votes_up - a.votes_up);
  console.log(`${out.length} ${type} ${lang} reviews; top 25 by votes:`);
  for (const r of out.slice(0, 25)) {
    const hrs = (r.author.playtime_forever / 60).toFixed(1);
    console.log(`\n[${iso(r.timestamp_created)}] ${r.votes_up} votes | ${hrs} h | ${r.voted_up ? "POS" : "NEG"}\n${r.review}`);
  }
}

async function news(id) {
  const j = await getJson(
    `https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=${id}&count=100&maxlength=0&format=json`,
  );
  for (const n of j?.appnews?.newsitems ?? []) {
    console.log(`\n=== ${iso(n.date)} | ${n.title} ===\n${strip(n.contents)}`);
  }
}

async function tags(ids) {
  for (const id of ids) {
    const res = await fetch(`https://store.steampowered.com/app/${id}/`, {
      headers: { cookie: "birthtime=0; wants_mature_content=1", "user-agent": "competitive-research/1.0" },
    });
    const html = await res.text();
    const found = [...html.matchAll(/class="app_tag"[^>]*>\s*([^<]+?)\s*</g)].map((m) => m[1]);
    console.log(`\n== tags: ${id} ==\n${found.filter((t) => t !== "+").join(", ") || "none found"}`);
    await sleep(WAIT_MS);
  }
}

// Community discussion threads (demo feedback, developer replies). The topic list and each thread embed their text as JSON
// in the page source, so no login is needed. Author names are in "author", replies in "comments_raw".
// limit = how many thread pages to open (the first page of the board only); maxReplies = replies printed per thread
// (first page of each thread only); reply text is cut at 400 characters. These caps keep a big forum cheap to read.
async function threads(id, limit = 40, maxReplies = 20) {
  const get = async (u) => (await fetch(u, { headers: { cookie: "birthtime=0", "user-agent": "competitive-research/1.0" } })).text();
  const list = await get(`https://steamcommunity.com/app/${id}/discussions/`);
  const urls = [...new Set([...list.matchAll(new RegExp(`https://steamcommunity\\.com/app/${id}/discussions/0/\\d+/`, "g"))].map((m) => m[0]))].slice(0, limit);
  const balanced = (h, from) => {
    const s = h.indexOf("{", from);
    let d = 0;
    for (let e = s; e < h.length; e++) {
      if (h[e] === "{") d++;
      else if (h[e] === "}" && --d === 0) return h.slice(s, e + 1);
    }
    return null;
  };
  for (const u of urls) {
    const h = await get(u);
    const title = (h.match(/<title>([^<]*)/) ?? [])[1] ?? u;
    console.log(`\n=== ${title.replace(/ :: .*/, "")} | ${u}`);
    const op = h.match(/"\d{10,}", (\{"author":[\s\S]*?\})\s*\);/);
    try {
      const o = JSON.parse(op[1]);
      console.log(`OP [${o.author}]: ${o.text}`);
    } catch { /* thread layout differs */ }
    const i = h.indexOf('"comments_raw":');
    const raw = i > 0 ? balanced(h, i) : null;
    try {
      const replies = Object.values(JSON.parse(raw ?? "{}"));
      for (const r of replies.slice(0, maxReplies)) console.log(`  - [${r.author ?? "?"}] ${(r.text ?? "").replace(/\n+/g, " ").slice(0, 400)}`);
      if (replies.length > maxReplies) console.log(`  (... ${replies.length - maxReplies} more replies on this page not shown)`);
    } catch { /* no replies */ }
    await sleep(WAIT_MS);
  }
}

// Tag-filtered store search. Games only (category1=998), cc=us. Page size is 100.
async function find(tagIds, max = 300, sort = "", filter = "") {
  let start = 0;
  while (start < max) {
    const u = `https://store.steampowered.com/search/results/?query&start=${start}&count=100&tags=${tagIds}&infinite=1&cc=us&l=english&category1=998${sort ? "&sort_by=" + sort : ""}${filter ? "&filter=" + filter : ""}`;
    const j = await getJson(u);
    const rows = (j?.results_html ?? "").split('data-ds-appid="').slice(1);
    if (!rows.length) break;
    for (const r of rows) {
      const id = r.match(/^(\d+)/)[1];
      const name = (r.match(/class="title">([^<]*)/) ?? [])[1] ?? "?";
      const date = (r.match(/search_released[^>]*>\s*([^<]*?)\s*</) ?? [])[1] ?? "";
      const rev = (r.match(/data-tooltip-html="([^"]*)"/) ?? [])[1] ?? "";
      const pct = (rev.match(/(\d+)% of the ([\d,]+)/) ?? []);
      const price = (r.match(/data-price-final="(\d+)"/) ?? [])[1];
      console.log([id, name.replace(/&amp;/g, "&").replace(/&#39;/g, "'"), date || "TBA", pct[1] ? pct[1] + "%" : "-", pct[2] ?? "0", price == null ? "-" : "$" + (price / 100).toFixed(2)].join(" | "));
    }
    start += 100;
    await sleep(WAIT_MS);
  }
}

const [cmd, ...args] = process.argv.slice(2);
switch (cmd) {
  case "search": await search(args); break;
  case "details": await details(args); break;
  case "summary": await summary(args); break;
  case "reviews": await reviews(args[0], args[1] ?? "negative", args[2] ?? "all", args[3]); break;
  case "news": await news(args[0]); break;
  case "tags": await tags(args); break;
  case "find": await find(args[0], Number(args[1] ?? 300), args[2] === "-" ? "" : (args[2] ?? ""), args[3] ?? ""); break;
  case "threads": await threads(args[0], Number(args[1] ?? 40), Number(args[2] ?? 20)); break;
  default:
    console.log("commands: search | details | summary | reviews | news | tags | find | threads (see header comment)");
}
