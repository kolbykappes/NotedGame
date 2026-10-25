#!/usr/bin/env node
// yt_channel.mjs - list a YouTube channel's newest videos and the games they cover.
//
// Usage: node yt_channel.mjs @handle [--desc] [--limit 30] [--json out.json]
//
// Fetches https://www.youtube.com/@handle/videos with a browser user-agent and a consent cookie and parses the
// embedded `ytInitialData` (first page only, about 30 videos): title, views, age, length. With --desc it also
// fetches each watch page and reads `shortDescription` for the game name (the first words) and any Steam links.
// Titles are usually clickbait; the descriptions are what name the game. Then run quick_facts.mjs on the App IDs.

import fs from "fs";

const argv = process.argv.slice(2);
const handle = argv.find((a) => !a.startsWith("--"));
const val = (name, def) => {
  const i = argv.indexOf(`--${name}`);
  return i >= 0 ? argv[i + 1] : def;
};
if (!handle) {
  console.error("usage: node yt_channel.mjs @handle [--desc] [--limit 30] [--json out.json]");
  process.exit(1);
}
const limit = Number(val("limit", 30));
const headers = {
  "user-agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36",
  "accept-language": "en-US,en;q=0.9",
  cookie: "CONSENT=YES+1; SOCS=CAI",
};
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

const html = await (await fetch(`https://www.youtube.com/${handle.startsWith("@") ? handle : "@" + handle}/videos`, { headers })).text();
const m = html.match(/var ytInitialData = (\{.*?\});<\/script>/s);
if (!m) {
  console.error("Could not find ytInitialData (YouTube may have changed the page or blocked the request).");
  process.exit(2);
}
const data = JSON.parse(m[1]);
const out = [];
(function walk(o) {
  if (!o || typeof o !== "object") return;
  if (o.lockupViewModel?.contentId) {
    const l = o.lockupViewModel;
    const md = l.metadata?.lockupMetadataViewModel;
    const rows = (md?.metadata?.contentMetadataViewModel?.metadataRows ?? []).flatMap((r) => (r.metadataParts ?? []).map((p) => p.text?.content)).join(" / ");
    const len = JSON.stringify(l).match(/"text":"(\d+:\d+(?::\d+)?)"/)?.[1];
    out.push({ id: l.contentId, title: md?.title?.content, stats: rows, length: len });
  } else if (o.videoRenderer) {
    const v = o.videoRenderer;
    out.push({ id: v.videoId, title: v.title?.runs?.map((r) => r.text).join(""), stats: `${v.viewCountText?.simpleText} / ${v.publishedTimeText?.simpleText}`, length: v.lengthText?.simpleText });
  }
  for (const k of Object.keys(o)) walk(o[k]);
})(data);
const seen = new Set();
const vids = out.filter((v) => !seen.has(v.id) && seen.add(v.id)).slice(0, limit);

if (argv.includes("--desc")) {
  for (const v of vids) {
    try {
      const w = await (await fetch(`https://www.youtube.com/watch?v=${v.id}`, { headers })).text();
      const d = w.match(/"shortDescription":"((?:[^"\\]|\\.)*)"/);
      const desc = d ? JSON.parse('"' + d[1] + '"') : "";
      v.steam = [...new Set([...desc.matchAll(/store\.steampowered\.com\/app\/(\d+)/g)].map((x) => x[1]))];
      v.desc = desc.replace(/\s+/g, " ").slice(0, 120);
    } catch (e) {
      v.desc = `(error: ${e.message})`;
    }
    await sleep(400);
  }
}
for (const v of vids) console.log(`${v.id} | ${v.stats} | ${v.length ?? ""} | ${v.title}${v.steam ? `\n    steam: ${v.steam.join(",") || "-"} | ${v.desc}` : ""}`);
console.log(`\n${vids.length} videos (first page only). Titles rarely name the game; use --desc to read descriptions.`);
const jf = val("json", null);
if (jf) fs.writeFileSync(jf, JSON.stringify(vids, null, 1));
