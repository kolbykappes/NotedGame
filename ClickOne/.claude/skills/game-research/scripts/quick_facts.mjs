#!/usr/bin/env node
// quick_facts.mjs - compact Steam facts plus the top reviews for one or more games (Pass 1, cheap).
//
// Usage: node quick_facts.mjs "Name:appid" ["Name:appid" ...] [--pos 5] [--neg 4] [--len 330] [--about 600]
//
// For each game prints: developer, publisher, release date, price, all-language review totals, user tags,
// a shortened description, then the most-voted English positive and negative reviews with hours and votes.
// Reviews are the most-helpful ones (Steam's default ordering), not a random sample; say so when you quote them.

const argv = process.argv.slice(2);
const opt = (name, def) => {
  const i = argv.indexOf(`--${name}`);
  return i >= 0 ? Number(argv[i + 1]) : def;
};
const nPos = opt("pos", 5), nNeg = opt("neg", 4), len = opt("len", 330), aboutLen = opt("about", 600);
const games = [];
for (let i = 0; i < argv.length; i++) {
  if (argv[i].startsWith("--")) { i++; continue; }
  const [name, id] = argv[i].split(":");
  if (id) games.push([name, Number(id)]);
}
if (!games.length) {
  console.error('usage: node quick_facts.mjs "Name:appid" ["Name:appid" ...] [--pos 5] [--neg 4]');
  process.exit(1);
}

const UA = { "user-agent": "competitive-research/1.0" };
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
const strip = (h = "") =>
  h.replace(/\[\/?[a-z0-9]+(=[^\]]*)?\]/gi, " ").replace(/<br\s*\/?>/gi, " ").replace(/<[^>]+>/g, " ")
    .replace(/&quot;/g, '"').replace(/&amp;/g, "&").replace(/&#39;/g, "'").replace(/\s+/g, " ").trim();
const getJson = async (url) => {
  // Steam sometimes redirects or returns an empty or HTML body (unreleased games, throttling): return {} instead of crashing.
  try {
    const r = await fetch(url, { headers: UA, redirect: "manual" });
    if (!r.ok) return {};
    const t = await r.text();
    return t ? JSON.parse(t) : {};
  } catch {
    return {};
  }
};

for (const [name, id] of games) {
  try {
    const d = (await getJson(`https://store.steampowered.com/api/appdetails?appids=${id}&l=english&cc=us`))[id]?.data ?? {};
    await sleep(350);
    const html = await (await fetch(`https://store.steampowered.com/app/${id}/?l=english`, {
      headers: { ...UA, cookie: "birthtime=0; wants_mature_content=1" },
    })).text();
    const tags = [...html.matchAll(/class="app_tag"[^>]*>\s*([^<]+?)\s*</g)].map((m) => m[1]).slice(0, 14);
    await sleep(350);
    const sum = (await getJson(`https://store.steampowered.com/appreviews/${id}?json=1&language=all&num_per_page=0&purchase_type=all`)).query_summary
      ?? { total_reviews: "none (unreleased, or the review endpoint returned nothing)", total_positive: 0 };
    const pct = typeof sum.total_reviews === "number" && sum.total_reviews ? Math.round((100 * sum.total_positive) / sum.total_reviews) : null;
    console.log(`\n===== ${name} (${id}) | dev: ${(d.developers ?? []).join(", ")} | pub: ${(d.publishers ?? []).join(", ")}`);
    const po = d.price_overview;
    // A discounted game shows its SALE price as the current price: record the list price too.
    const price = po ? (po.discount_percent ? `${po.final_formatted} (${po.discount_percent}% off ${po.initial_formatted} list)` : po.final_formatted) : "free/none";
    console.log(`release: ${d.release_date?.date} | price: ${price} | reviews (all languages): ${sum.total_reviews}${pct != null ? `, ${pct}% positive` : ""}`);
    console.log("TAGS:", tags.join(", "));
    console.log("ABOUT:", strip(d.about_the_game || d.short_description).slice(0, aboutLen));
    for (const [label, type, n] of [["POS", "positive", nPos], ["NEG", "negative", nNeg]]) {
      if (!n) continue;
      const r = await getJson(
        `https://store.steampowered.com/appreviews/${id}?json=1&language=english&review_type=${type}&filter=all&day_range=9223372036854775807&num_per_page=${n}&purchase_type=all`
      );
      console.log(`${label} (most helpful, English):`);
      for (const v of r.reviews ?? [])
        console.log(` ${label === "POS" ? "+" : "-"} [${Math.round(v.author.playtime_forever / 60)}h, ${v.votes_up} up] ${strip(v.review).slice(0, len)}`);
      await sleep(350);
    }
  } catch (e) {
    console.log(`\n===== ${name} (${id}) FAILED: ${e.message}`);
  }
}
