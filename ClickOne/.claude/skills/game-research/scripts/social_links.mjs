#!/usr/bin/env node
// social_links.mjs - find a game's community and social links without guessing.
//
// Usage: node social_links.mjs <appid> [news.txt ...]
//
// Reads the Steam store page (outbound "linkfilter" targets), the developer and publisher pages, and any saved
// news text (from `steam.mjs news`), then classifies every external link: Discord, X/Twitter, Bluesky, Mastodon,
// YouTube, TikTok, Instagram, Twitch, itch.io, Newgrounds, Game Jolt, Kongregate, CrazyGames, Reddit, a website.
// "None found" means none were found in these places; it does not prove the account does not exist.

import fs from "fs";

const [appid, ...newsFiles] = process.argv.slice(2);
if (!appid) {
  console.error("usage: node social_links.mjs <appid> [news.txt ...]");
  process.exit(1);
}
const UA = { "user-agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36", "accept-language": "en-US,en;q=0.9", cookie: "birthtime=0; wants_mature_content=1" };
const get = async (u) => {
  try { return await (await fetch(u, { headers: UA })).text(); } catch { return ""; }
};

const kinds = [
  ["discord", /discord\.(gg|com)/i], ["x/twitter", /(^|\.)(twitter|x)\.com\//i], ["bluesky", /bsky\.app/i], ["mastodon", /mastodon|\.social\//i],
  ["youtube", /youtube\.com|youtu\.be/i], ["tiktok", /tiktok\.com/i], ["instagram", /instagram\.com/i], ["twitch", /twitch\.tv/i],
  ["itch.io", /itch\.io/i], ["newgrounds", /newgrounds\.com/i], ["gamejolt", /gamejolt\.com/i], ["kongregate", /kongregate\.com/i],
  ["crazygames", /crazygames\.com/i], ["reddit", /reddit\.com\/r\//i], ["facebook", /facebook\.com/i], ["patreon", /patreon\.com/i],
  ["linktree", /linktr\.ee|bio\.link/i],
];
const skipHosts = /steampowered|steamcommunity|steamstatic|akamai|valvesoftware|steamdeck\.com|store\.steam/i;
const found = new Map(); // url -> source

const addUrl = (raw, source) => {
  let u = raw;
  try { u = decodeURIComponent(raw); } catch {}
  u = u.replace(/[)\].,;]+$/, "").replace(/\?(proof|snr)=[^&]*$/, "");
  if (skipHosts.test(u)) return;
  if (!found.has(u)) found.set(u, source);
};

const html = await get(`https://store.steampowered.com/app/${appid}/?l=english`);
for (const m of html.matchAll(/linkfilter\/\?u=([^"'&<>\s]+)/g)) addUrl(m[1], "store page");
for (const m of html.matchAll(/href="(https?:\/\/[^"]+)"/g)) addUrl(m[1], "store page");

const dev = [...html.matchAll(/store\.steampowered\.com\/(developer|publisher)\/([A-Za-z0-9_%.-]+)/g)].map((m) => m[0]);
for (const page of [...new Set(dev)].slice(0, 3)) {
  const t = await get(`https://${page}`);
  for (const m of t.matchAll(/linkfilter\/\?u=([^"'&<>\s]+)/g)) addUrl(m[1], page.split("/").slice(1, 3).join("/") + " page");
}
for (const f of newsFiles) {
  if (!fs.existsSync(f)) continue;
  for (const m of fs.readFileSync(f, "utf8").matchAll(/https?:\/\/[^\s\]"')<>]+/g)) addUrl(m[0], "news posts");
}

const rows = [...found.entries()].map(([u, src]) => {
  const k = kinds.find(([, re]) => re.test(u));
  return { kind: k ? k[0] : "other", u, src };
});
if (!rows.length) {
  console.log("No external links found on the store page, developer pages or news. (Unknown, not proof of absence.)");
} else {
  for (const r of rows.sort((a, b) => a.kind.localeCompare(b.kind))) console.log(`${r.kind.padEnd(11)} ${r.u}   [${r.src}]`);
}
const kindsSeen = new Set(rows.map((r) => r.kind));
console.log("\nnot found among: " + ["discord", "x/twitter", "bluesky", "youtube", "tiktok", "instagram", "itch.io"].filter((k) => !kindsSeen.has(k)).join(", "));
