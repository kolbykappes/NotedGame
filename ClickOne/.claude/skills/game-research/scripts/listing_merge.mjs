#!/usr/bin/env node
// listing_merge.mjs - merge several `steam.mjs find` listings, filter by release date, rank by review count.
//
// Usage: node listing_merge.mjs --since 2023-10-21 [--top 40] [--review-order reviews.txt] listing1.txt listing2.txt ...
//
// Steam search returns at most 3,000 results per query, so for a tag run `steam.mjs find <tagId> 3000 <sort>`
// with several sorts (Released_DESC, Reviews_DESC, relevance "-") into files, then merge them here.
// Lines look like: `appid | name | Mon D, YYYY | 95% | 1,234 | $4.99`. Review counts are ENGLISH ONLY.
// --review-order names the Reviews_DESC listing: the script reports whether the top 30 appear in it (a coverage check).

import fs from "fs";

const argv = process.argv.slice(2);
const val = (name, def) => {
  const i = argv.indexOf(`--${name}`);
  return i >= 0 ? argv[i + 1] : def;
};
const files = argv.filter((a, i) => !a.startsWith("--") && !(i > 0 && argv[i - 1].startsWith("--")));
const since = new Date(val("since", "1970-01-01"));
const top = Number(val("top", 40));
if (!files.length) {
  console.error("usage: node listing_merge.mjs --since 2023-10-21 [--top 40] [--review-order f.txt] a.txt b.txt ...");
  process.exit(1);
}
const all = new Map();
for (const f of files) {
  for (const line of fs.readFileSync(f, "utf8").split("\n")) {
    const p = line.split(" | ");
    if (p.length < 6) continue;
    const id = p[0].trim();
    if (all.has(id)) continue;
    all.set(id, { id, name: p[1], date: new Date(p[2].trim()), ds: p[2].trim(), pct: p[3].trim(), rev: parseInt(p[4].replace(/,/g, ""), 10) || 0, price: p[5].trim() });
  }
}
const recent = [...all.values()].filter((g) => !isNaN(g.date) && g.date >= since).sort((a, b) => b.rev - a.rev);
console.log(`merged ${all.size} distinct games; released since ${val("since", "any")}: ${recent.length}`);
for (const g of recent.slice(0, top)) console.log(`${g.id} | ${g.name} | ${g.ds} | ${g.pct} | ${g.rev} | ${g.price}`);
const ro = val("review-order", null);
if (ro) {
  const ids = new Set(fs.readFileSync(ro, "utf8").split("\n").map((l) => l.split(" | ")[0].trim()));
  const missing = recent.slice(0, 30).filter((g) => !ids.has(g.id)).map((g) => g.name);
  console.log(missing.length ? `\nCOVERAGE WARNING: top 30 not in the review-ordered listing: ${missing.join(", ")}` : "\ncoverage check: all of the top 30 appear in the review-ordered listing");
}
console.log("\nnote: counts are English-only; Chinese-market games are undercounted. Dates may be 1.0 dates for long Early Access games.");
