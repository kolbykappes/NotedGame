#!/usr/bin/env node
// review_analyzer.mjs - theme counts, median playtimes, top positives and all negatives (Pass 2).
//
// Usage: node review_analyzer.mjs --neg neg.json --pos pos.json [--themes themes.json]
//                                 [--max-neg 400] [--neg-len 380] [--top-pos 40 --pos-out top_pos.txt]
//
// Inputs are the JSON files written by `node steam.mjs reviews <appid> negative|positive english <file>`.
// Theme counts are KEYWORD MATCHES (a review that mentions a topic, not necessarily praising or criticizing it):
// always report them as approximate. A themes.json can replace the defaults: { "label": "regex source", ... }.

import fs from "fs";

const argv = process.argv.slice(2);
const val = (name, def) => {
  const i = argv.indexOf(`--${name}`);
  return i >= 0 ? argv[i + 1] : def;
};
const negFile = val("neg"), posFile = val("pos");
if (!negFile && !posFile) {
  console.error("usage: node review_analyzer.mjs --neg neg.json --pos pos.json [--themes themes.json] [--max-neg 400]");
  process.exit(1);
}
const load = (f) => {
  if (!f) return [];
  const j = JSON.parse(fs.readFileSync(f, "utf8"));
  return Array.isArray(j) ? j : j.reviews ?? [];
};
const N = load(negFile), P = load(posFile);

const DEFAULT_THEMES = {
  "short / not enough content": "short|not enough content|runtime|over too|ends too|only \\d+ hours|lack of content",
  "grind / repetitive / boring": "grind|slow|tedious|boring|repetitive|chore|waiting|dead air|monoton",
  "bugs / crashes / saves": "bug|crash|stuck|glitch|soft.?lock|broken|corrupt|lost my save",
  "motion sickness / camera": "motion sick|dizz|nause|headache|fov|camera",
  "controls / janky": "clunky|janky|controls|floaty|slidey|rebind",
  "story / puzzles / secrets": "story|puzzle|secret|ending|mystery|lore",
  "automation / idle / afk": "automat|idle|afk|offline",
  "horror / scary / dark": "horror|creep|scar|unsettl|disturb",
  "humor / charm / cute": "funny|humou?r|charm|cute|silly|personality|hilarious|adorable",
  "satisfying / juicy / addictive": "juic|satisf|squish|addict|dopamine|one more|can't stop|crunch",
  "difficulty / balance": "too hard|too easy|difficult|balanc|unfair|punishing",
  "price / value / refund": "price|worth|refund|cheap|bargain|overpriced|\\$",
  "performance / Steam Deck": "performance|fps|lag|stutter|steam ?deck|optimi",
  "AI / generated": "\\bAI\\b|generated|slop|chatgpt|midjourney",
};
let themes = DEFAULT_THEMES;
const tf = val("themes");
if (tf) themes = JSON.parse(fs.readFileSync(tf, "utf8"));

const hrs = (r) => Math.round(r.author.playtime_forever / 6) / 10;
const hrsAt = (r) => Math.round((r.author.playtime_at_review ?? r.author.playtime_forever) / 6) / 10;
const med = (a) => {
  const s = [...a].sort((x, y) => x - y);
  return s.length ? s[Math.floor(s.length / 2)] : null;
};
const clean = (t) => t.replace(/\[[^\]]*\]/g, " ").replace(/\s+/g, " ").trim();
const pct = (n, d) => (d ? `${Math.round((100 * n) / d)}%` : "-");

console.log(`reviews: negative ${N.length}, positive ${P.length} (English, as supplied)`);
console.log(`median hours at review: negative ${med(N.map(hrsAt))}, positive ${med(P.map(hrsAt))}  |  median total hours now: negative ${med(N.map(hrs))}, positive ${med(P.map(hrs))}`);
console.log("\nTHEMES (keyword matches, approximate)");
console.log("theme".padEnd(34), "neg".padStart(9), "pos".padStart(12));
for (const [label, src] of Object.entries(themes)) {
  const re = new RegExp(src, "i");
  const n = N.filter((r) => re.test(r.review)).length, p = P.filter((r) => re.test(r.review)).length;
  console.log(label.padEnd(34), `${String(n).padStart(4)} ${pct(n, N.length).padStart(4)}`, `${String(p).padStart(6)} ${pct(p, P.length).padStart(4)}`);
}

// Reading cap: the counts above cover every review (free), but only the most-voted negatives are printed
// into the conversation. Default 60 at 300 characters (about 5K tokens); raise --max-neg only for a reason.
const maxNeg = Number(val("max-neg", 60)), negLen = Number(val("neg-len", 300));
if (N.length) {
  const sorted = [...N].sort((a, b) => b.votes_up - a.votes_up);
  const shown = sorted.slice(0, maxNeg);
  console.log(`\n=== ${shown.length === N.length ? "ALL" : `TOP ${shown.length} BY VOTES OF`} ${N.length} NEGATIVE REVIEWS (hours, votes) ===`);
  for (const r of shown) console.log(`[${hrs(r)}h, ${r.votes_up} up] ${clean(r.review).slice(0, negLen)}`);
  if (shown.length < N.length) console.log(`(... ${N.length - shown.length} more negatives counted in the themes above but not printed)`);
}
const topPos = Number(val("top-pos", 0));
if (topPos && P.length) {
  const out = val("pos-out", "top_pos.txt");
  const body = [...P].sort((a, b) => b.votes_up - a.votes_up).slice(0, topPos)
    .map((r) => `[${hrs(r)}h, ${r.votes_up} up] ${clean(r.review).slice(0, 700)}`).join("\n\n");
  fs.writeFileSync(out, body);
  console.log(`\nwrote top ${topPos} positive reviews to ${out}`);
}
