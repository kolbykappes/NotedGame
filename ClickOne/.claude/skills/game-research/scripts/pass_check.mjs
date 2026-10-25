#!/usr/bin/env node
// pass_check.mjs - decide which research pass to run for a game, before fetching anything.
//
// Usage: node pass_check.mjs "<game name or App ID>" [--dir Docs/competitive]
//
// Looks for an existing game file (NN-name.md or any .md that mentions the name or App ID), reads its
// "Coverage and pass log" table, and prints which layers are done, which are missing, and the next file number.
// Prints a one-line verdict you can quote when you declare the pass.

import fs from "fs";
import path from "path";

const args = process.argv.slice(2);
const query = args.find((a) => !a.startsWith("--"));
const dirIdx = args.indexOf("--dir");
const dir = dirIdx >= 0 ? args[dirIdx + 1] : "Docs/competitive";
if (!query) {
  console.error('usage: node pass_check.mjs "<game name or App ID>" [--dir Docs/competitive]');
  process.exit(1);
}
if (!fs.existsSync(dir)) {
  console.log(`Folder ${dir} not found. Verdict: Pass 1 (cursory), and the research folder must be created first.`);
  process.exit(0);
}

const q = query.toLowerCase();
const files = fs.readdirSync(dir).filter((f) => f.endsWith(".md"));
const numbered = files.filter((f) => /^\d\d-/.test(f));
const next = String(numbered.reduce((m, f) => Math.max(m, parseInt(f.slice(0, 2), 10)), 0) + 1).padStart(2, "0");

const slug = q.replace(/[^a-z0-9]+/g, "-");
const hits = [];
for (const f of files) {
  const text = fs.readFileSync(path.join(dir, f), "utf8");
  const inName = f.toLowerCase().includes(slug);
  const head = text.slice(0, 3000).toLowerCase();
  const inHead = head.includes(q) || head.includes(`app/${q}`) || head.includes(`app id ${q}`) || head.includes(`(${q})`);
  if (inName || inHead) hits.push({ f, text, score: (inName ? 2 : 0) + (/^\d\d-/.test(f) ? 1 : 0) });
}
hits.sort((a, b) => b.score - a.score);

console.log(`next file number: ${next}`);
if (!hits.length) {
  console.log(`No file found for "${query}" in ${dir}.`);
  console.log(`VERDICT: Pass 1 (cursory). Create ${next}-<slug>.md from references/game-file-template.md with a Coverage table.`);
  process.exit(0);
}

const hit = hits[0];
console.log(`Found: ${path.join(dir, hit.f)}${hits.length > 1 ? `  (also mentioned in: ${hits.slice(1, 4).map((h) => h.f).join(", ")})` : ""}`);

const m = hit.text.match(/## Coverage and pass log([\s\S]*?)(\n## |\n$)/);
if (!m) {
  console.log("The file has no Coverage table. Treat the layers as unknown: read the file's sections, add a Coverage table, then fetch only what is missing.");
  console.log("VERDICT: Pass 2 (deep) if the owner asked for depth, otherwise add the Coverage table first.");
  process.exit(0);
}
// Files written on Windows may have CRLF line endings; normalize before matching table rows.
const rows = m[1].split(/\r?\n/).map((l) => l.trim()).filter((l) => l.startsWith("|") && !/^\|[-\s|:]+\|$/.test(l) && !/^\|\s*Layer\s*\|/i.test(l));
const notStarted = [], cursoryOnly = [], deepDone = [];
// "not done" contains the word "done": rule it out first.
const isNot = (c) => /^(not done|not started|pending)\b/i.test(c.trim()) || /^-*$/.test(c.trim());
const isDone = (c) => !isNot(c) && (/^(done|partly|partial|titles|read|\d{4}-)/i.test(c) || /\bdone\b/i.test(c));
const isNA = (c) => /^n\/a/i.test(c);
for (const r of rows) {
  const cells = r.split("|").slice(1, -1).map((c) => c.trim());
  if (cells.length < 3) continue;
  const [layer, p1, p2] = cells;
  let state;
  if (isDone(p2) && !/^partly|partial/i.test(p2)) state = "deep done";
  else if (isNA(p2) && isDone(p1)) state = "done (no deep layer)";
  else if (isDone(p1) || isDone(p2)) state = "cursory only";
  else state = "not started";
  console.log(`  ${state.padEnd(20)} | ${layer} | P1: ${p1 || "-"} | P2: ${p2 || "-"}`);
  if (state === "not started") notStarted.push(layer);
  else if (state === "cursory only") cursoryOnly.push(layer);
  else deepDone.push(layer);
}
if (!notStarted.length && !cursoryOnly.length) {
  console.log("VERDICT: every layer is done. Refresh only stale layers (new patches, review totals, new threads) or add a layer the owner names.");
} else {
  console.log(`VERDICT: Pass 2 (deep) on the existing file. Reuse everything marked done.`);
  if (notStarted.length) console.log(`  not started (fetch): ${notStarted.join("; ")}`);
  if (cursoryOnly.length) console.log(`  cursory only (deepen if asked): ${cursoryOnly.join("; ")}`);
}
