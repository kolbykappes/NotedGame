#!/usr/bin/env node
// reddit_flatten.mjs - turn a saved Reddit comment-tree result into short flat text.
//
// Usage: node reddit_flatten.mjs <saved-file> [--max 420] [--author <name>] [--min-score 0] [--limit 200]
//
// The Reddit MCP tool saves big comment trees to a file ("Output has been saved to ..."). They are 50 to 150 KB
// and must never be read raw. This prints the post header, then each comment as `[score] author: body`,
// indented by depth. With --author it prints only that author's comments, each preceded by the parent's opening
// words: use it to pull a developer's own replies (their stated inspirations, build time, design intent).

import fs from "fs";

const argv = process.argv.slice(2);
const file = argv.find((a) => !a.startsWith("--"));
const val = (name, def) => {
  const i = argv.indexOf(`--${name}`);
  return i >= 0 ? argv[i + 1] : def;
};
if (!file) {
  console.error("usage: node reddit_flatten.mjs <saved-file> [--max 420] [--author name] [--min-score 0] [--limit 200]");
  process.exit(1);
}
let j = JSON.parse(fs.readFileSync(file, "utf8"));
if (Array.isArray(j)) j = JSON.parse(j[0].text);

// Caps keep a big thread cheap to read: 30 comments of 300 characters by default (about 3K tokens).
const max = Number(val("max", 300)), minScore = Number(val("min-score", -9999)), limit = Number(val("limit", 30));
const author = val("author", null);
const flat = (t) => (t ?? "").replace(/\s+/g, " ").trim();

const p = j.post ?? {};
console.log(`POST: ${p.title} | r/${p.subreddit} | ${p.upvotes} up, ${p.comments} comments | ${p.created?.slice(0, 10)}`);
if (p.text) console.log(`BODY: ${flat(p.text).slice(0, 500)}`);
if (j.listing_status && j.listing_status !== "complete") console.log(`(note: listing_status=${j.listing_status}; more comments exist than were returned)`);

let printed = 0, matched = 0;
function walk(node, depth, parent) {
  if (!node || node.kind !== "t1") return;
  const c = node.data;
  const mine = !author || (c.author ?? "").toLowerCase() === author.toLowerCase();
  if (mine && (c.score ?? 0) >= minScore) {
    matched++;
    if (printed < limit) {
      if (author && parent) console.log(`  (replying to ${parent.author}: ${flat(parent.body).slice(0, 140)})`);
      console.log(`${author ? "" : "  ".repeat(depth)}- [${c.score}] ${c.author}: ${flat(c.body).slice(0, max)}`);
      printed++;
    }
  }
  for (const r of c.replies?.data?.children ?? []) walk(r, depth + 1, c);
}
for (const c of j.comments ?? []) walk(c, 0, null);
if (!matched) console.log("(no comments matched)");
else if (matched > printed) console.log(`(... ${matched - printed} more matching comments not shown; raise --limit only if the question needs them)`);
