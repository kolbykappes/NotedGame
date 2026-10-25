---
name: game-research
description: Research a video game (or a set of games) from public data for the ClickOne competitive-research folder, in two passes, a cheap cursory pass first and a deeper one that reuses it. Pulls Steam store facts, review totals and reviews, patch notes, Steam threads, the Reddit communities (dedicated sub or the likely shared sub), Discord and social links, other platforms (web builds, consoles), store screenshots, and writes the game file, README row and review-tracker block. Use this whenever the owner says "quick look", "cursory", "deep dive", "analyze this game", "research this game", "what are people saying about X", "look into X", "what's on Reddit about X", pastes a Steam, itch.io or YouTube channel link, asks for top games in a tag, what a creator is covering, or comparable games for mechanics, even if they do not say "research". Never plays the game.
---

# Game research

The owner is designing a small incremental game and researches comparable games from public data. This skill keeps that research cheap, consistent and honest. Its two ideas: **declare which pass you are doing**, and **reuse what the game's file already holds** so a deeper pass only fetches the missing layers.

Three standing rules from the owner:
- **Never play, install, download or run a game, demo or web build.** Read store pages, patch notes, reviews, community threads and store screenshots. Ask the owner for their own impressions.
- **Do not volunteer sales or revenue estimates**, even when a developer post gives numbers.
- **Keep the first pass cheap.** If they say "high level" or "cursory", do Pass 1 only and offer Pass 2.

Paths: the research folder is `Docs/competitive/` (game files, `README.md` index, `review-tracker.md`, `PLAYBOOK.md`). Steam helper: `node Docs/competitive/tools/steam.mjs`. This skill's scripts: `.claude/skills/game-research/scripts/`. Put scratch output (raw JSON, saved tool results, downloaded images) in the session scratchpad, never in the repo.

## Step 0: declare the pass (always first)

Run `node .claude/skills/game-research/scripts/pass_check.mjs "<name or App ID>"`. It finds an existing game file, reads its **Coverage table**, and prints a verdict. Then say one line to the owner, for example "Pass 1 (cursory): no file exists" or "Pass 2 (deep): cursory data found in `04-name.md`; fetching only the missing layers."

- No file: **Pass 1**, then offer Pass 2.
- File with a Coverage table and gaps: **Pass 2** on that file, fetching only layers marked not started or cursory-only.
- Everything done: refresh only stale layers (new patches, review totals, new threads) or the layer the owner names.
- Owner asked for a "deep dive" and no file exists: run Pass 1, then Pass 2, in the same sitting.
- A request about several games ("quick looks at these six") uses a combined file; see `references/modes.md`.

## Keep the cost down

A test of this skill (eight agent runs) used about 1.1 million tokens; a "cursory" run took 23 tool calls and about 100K tokens, and the deep runs crawled an entire 253-topic forum to answer one question. The owner pays for this, so:
- **Pass 1 is capped at about 10 tool calls.** Do not read `references/` unless the task needs them (`modes.md` for surveys, channel scans and combined files; `traps.md` for Reddit-heavy or survey work). Do nothing from Pass 2.
- **Scope Pass 2 to the question.** "What do the Steam threads say about the shop" means list thread titles, read only the topics that match, and update only that Coverage row. Do not crawl a whole forum or re-pull all reviews to answer a narrow question.
- **Reading caps** (what gets read into the conversation; scripts may still count thousands of items for free, and a game with fewer items than the cap is read in full):

  | Source | Cap on what is read | Why this number |
  |---|---|---|
  | Steam reviews, Pass 1 | 5 positive, 4 negative (`quick_facts`) | enough to hear both sides |
  | Steam reviews, Pass 2 | counts and medians for **all**; print the **60** most-voted negatives (about 5K tokens) and read **20** of the top positives | the most-voted reviews carry the repeated themes; below about 100 negatives, read them all |
  | Steam forum | scan up to **100** thread titles; open at most **12** threads, **20** replies each, 400 characters per reply (`steam.mjs threads <id> 12 20`) | the board's first page is a sample anyway; most topics are bugs and puzzle help |
  | Reddit search | **100** posts, tallied by script (costs almost nothing) | enough to see which subreddits matter |
  | Reddit comments | **3** threads, the top **30** comments of each, 300 characters (`reddit_flatten` defaults) | the best comments are sorted to the top; the developer's replies are pulled separately with `--author` |
  | News and patch notes | the newest **120** lines (`steam.mjs news <id> \| head -120`); read further back only if the question needs it | patch notes are short; marketing posts are long |

  If a question genuinely needs more (for example the owner asks "read every thread about X"), say what it costs and ask first. 500 items is fine for a script to count and far too many to read.
- **Reuse before fetching.** Read the game's file first. For surveys and channel scans, look for an earlier dated file and update it rather than re-running the 3,000-row listings and the channel parse.
- **Run Steam calls one at a time.** Parallel calls get throttled and cause retries. If the review endpoint returns nothing (unreleased game, or throttling), say so and move on.
- **No subagents, no baseline comparisons**, unless the owner asks.

## Pass 1: cursory (about 10 tool calls, a half-page result)

1. **Identity.** `node Docs/competitive/tools/steam.mjs search "name"` for the App ID, demo and DLC IDs. Non-Steam games: read the itch.io or app-store page with WebFetch.
2. **Facts, totals, top reviews in one call:** `node .claude/skills/game-research/scripts/quick_facts.mjs "Name:appid"` (store facts, tags, shortened description, all-language review totals, 5 most-helpful positive and 4 negative English reviews). Then `node Docs/competitive/tools/steam.mjs summary <appid>` for the English split, and add a block to `review-tracker.md`. A **discounted game shows its sale price**, so record the list price too (the script prints both).
3. **News titles:** `steam.mjs news <appid>` and keep the dates and titles (full text is Pass 2).
4. **Reddit map** (Reddit MCP tools; load them with ToolSearch if deferred):
   - Test for a dedicated subreddit by **guessing names with `reddit_subreddit_about`** (for example `BerryBuryBerry`). Do not start with the community search; it returns long descriptions of unrelated subreddits.
   - One `reddit_search` for the exact name in quotes, `sort: top`, `t: all`, `limit: 100`. The big result is saved to a file: run `node .../reddit_tally.mjs <file> --name "Game Name"`. It separates real discussion from unrelated hits and lists the top threads (the Pass 2 reading list).
   - Decide where the conversation lives. A dedicated sub with a handful of subscribers is a fan and help board; the real talk is usually the genre sub (r/incremental_games for incrementals), the developer's own launch thread, creator communities, and recommendation threads.
5. **Social and community links:** `node .../social_links.mjs <appid> [news.txt]` reads the store page, developer and publisher pages and news for Discord, X/Twitter, Bluesky, YouTube and others. "None found" is unknown, not absent. If nothing turns up, one web search for the developer's name.
6. **Other platforms:** from store text, plus one web search: web builds (itch.io, Newgrounds, CrazyGames, Game Jolt, Kongregate), consoles, mobile, Mac and Linux. Say what you searched.
7. **Write the file** (`references/game-file-template.md`): header, facts, Coverage table, a short verdict, top reviews, Reddit map, social, platforms. Add the README row. Then **offer Pass 2**.

## Pass 2: deep (only the layers still missing)

1. **All reviews.** `steam.mjs reviews <appid> negative english neg.json` and `... positive english pos.json` (to scratch), then `node .../review_analyzer.mjs --neg neg.json --pos pos.json --top-pos 40 --pos-out top_pos.txt`. The script counts themes over **every** review and prints only the 60 most-voted negatives; read `top_pos.txt` (use `--top-pos 20`). Say that the quoted reviews are the most-voted ones, not a sample. **Never generalize from 15 reviews; use the full counts.** Theme counts are keyword matches, so label them approximate.
2. **Patch notes:** `steam.mjs news <appid> | head -120` (the newest posts). Build a dated table of changes; the first patches show the first complaints. Pull mechanics and developer statements.
3. **Steam community:** `steam.mjs threads <appid> 12 20` (12 threads, 20 replies each). To pick which threads, first list titles, then read only the ones that bear on the question.
4. **Reddit threads:** fetch comments (`reddit_post_comments`, `sort: top`, `depth: 2`, `limit: 30`) for the top three threads from Pass 1. Flatten with `node .../reddit_flatten.mjs <saved-file>` (prints the top 30 comments), and pull the developer's replies with `--author <name>`.
5. **Screenshots:** `python .../store_shots.py <appid> <scratch-dir> --max 8`, then view each with the Read tool and describe it. Do not copy them into the repo unless asked. Owner-supplied screenshots: convert to WebP, number in play order, index them.
6. **Creators:** names from reviews and Reddit; for a channel scan, see `references/modes.md`.
7. **Synthesize** into the file: mechanics with evidence tags (`[store]`, `[patch notes]`, `[player report]`, `[dev]`, `[screens]`, `[inference]`), what players love and push back on (counts and quotes), a comparison with the owner's design, why it matters (borrow, avoid, risks marked as guesses), open questions. Update the Coverage table and the README row.

## Report back

A short chat summary: what the game is, what players love and hate, what it means for the owner's design, what was **not** done (nothing played; unknowns). End with **one question**, usually "What did you think when you played it?" and, after Pass 1, whether to go deeper. Put the owner's own words in the "impressions" section verbatim.

## Guardrails

- Tag inference as inference. A quote cut off in a sample stays cut off; do not complete it. Do not state an official-sounding claim as fact (a patch note about "getting verified on Steam Deck" is not a verification).
- Counts: all-language totals come from `summary`; `steam.mjs find` counts are English only. Cite the date.
- Use fictional company names in any design material. No subagents unless the owner asks.
- Longer lists of traps (Steam, Reddit and YouTube quirks) are in `references/traps.md`; read it before a survey, a channel scan or a Reddit-heavy pass.

## Bundled scripts (all take `--help`-style usage on no arguments)

| Script | Use |
|---|---|
| `pass_check.mjs` | Decide the pass; read the Coverage table; next file number |
| `quick_facts.mjs` | Facts, totals and top reviews for one or more App IDs (Pass 1) |
| `review_analyzer.mjs` | Theme counts, median playtimes, top positives, all negatives (Pass 2) |
| `reddit_tally.mjs` | Where is the game discussed? Tally a saved Reddit search by subreddit |
| `reddit_flatten.mjs` | Flatten a saved comment tree; pull one author's replies |
| `social_links.mjs` | Discord, X, Bluesky and other links from store, developer, publisher pages and news |
| `listing_merge.mjs` | Merge tag listings, filter by date, rank by reviews (surveys) |
| `yt_channel.mjs` | A YouTube channel's recent videos and the games they cover |
| `store_shots.py` | Download and shrink store screenshots for viewing |
