#!/usr/bin/env node
// reddit_tally.mjs - where is a game actually discussed? Tally a saved Reddit post search by subreddit.
//
// Usage: node reddit_tally.mjs <saved-search-file> --name "Berry Bury Berry" [--top 12]
//
// Run one exact-name post search at limit 100 with the Reddit MCP tool; it saves the big result to a file.
// This prints: how many hits mention the name in their title or body (real discussion) versus hits that only
// matched through comments or unrelated threads; a tally by subreddit; and the top threads by comment count
// among the name-mentioning hits. The top 3 become the Pass 2 reading list.

import fs from "fs";

const argv = process.argv.slice(2);
const file = argv.find((a) => !a.startsWith("--"));
const val = (name, def) => {
  const i = argv.indexOf(`--${name}`);
  return i >= 0 ? argv[i + 1] : def;
};
const name = val("name", "");
const top = Number(val("top", 12));
if (!file || !name) {
  console.error('usage: node reddit_tally.mjs <saved-search-file> --name "Game Name" [--top 12]');
  process.exit(1);
}
let j = JSON.parse(fs.readFileSync(file, "utf8"));
if (Array.isArray(j)) j = JSON.parse(j[0].text);
const P = j.posts ?? [];
const needle = name.toLowerCase();
const mentions = (p) => `${p.title} ${p.text ?? ""}`.toLowerCase().includes(needle);

const direct = P.filter(mentions);
console.log(`hits: ${P.length}${j.after ? " (more pages exist)" : ""}; mention "${name}" in title or body: ${direct.length}; matched only via comments or unrelated: ${P.length - direct.length}`);

const tally = (arr) => {
  const by = {};
  for (const p of arr) {
    const k = p.subreddit;
    by[k] = by[k] || { posts: 0, up: 0, comments: 0 };
    by[k].posts++; by[k].up += p.upvotes; by[k].comments += p.comments;
  }
  return Object.entries(by).sort((a, b) => b[1].comments - a[1].comments);
};
console.log("\nBY SUBREDDIT, name-mentioning hits only (posts / upvotes / comments):");
for (const [k, v] of tally(direct).slice(0, top)) console.log(`  ${k.padEnd(24)} ${String(v.posts).padStart(3)} ${String(v.up).padStart(6)} ${String(v.comments).padStart(5)}`);

console.log("\nTOP THREADS BY COMMENTS (name-mentioning):");
for (const p of [...direct].sort((a, b) => b.comments - a.comments).slice(0, top))
  console.log(`  [${p.upvotes} up / ${p.comments}] r/${p.subreddit} | ${p.title.slice(0, 90)} | ${p.created?.slice(0, 10)} | ${p.permalink}`);

const other = P.filter((p) => !mentions(p)).sort((a, b) => b.comments - a.comments).slice(0, 3);
if (other.length) {
  console.log("\nLARGEST NON-MENTIONING HITS (probably unrelated threads; do not count as discussion):");
  for (const p of other) console.log(`  [${p.comments}] r/${p.subreddit} | ${p.title.slice(0, 90)}`);
}
