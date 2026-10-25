# Research steps (as actually practiced), written out for a future skill

*Revised 2026-10-22. **This workflow is now implemented as a skill: `.claude/skills/game-research/` (SKILL.md, scripts, references).** The skill is the working copy; this file is the long-form explanation. This is the workflow Claude follows in the ClickOne research sessions, with the traps found along the way. It is the source for a skill. It sits beside [PLAYBOOK.md](PLAYBOOK.md), which holds the long-form rules and the per-game template. **Standing rules from the owner: Claude never plays the games, demos or web builds (2026-10-22); do not volunteer sales or revenue estimates (2026-10-21); keep the first pass cheap (2026-10-22).***

## Two passes, and saying which one you are doing
Every research request starts by **declaring the pass** in one line, for example "Pass 1 (cursory): no file exists" or "Pass 2 (deep): cursory data found in `04-name.md`, fetching only the missing layers".

**How to decide**
1. Look for the game's file (`grep` the folder for the name or App ID).
2. **No file** → **Pass 1 (cursory)**. Create the file with a **Coverage table**.
3. **File exists, only cursory layers done, and the owner asks for more** → **Pass 2 (deep)**. Read the file's Coverage table, reuse everything marked done, fetch **only the missing layers**.
4. **File exists with deep layers done** → refresh only layers that are stale (new patches, review totals, new threads) or newly requested.
5. If the owner says "high level", "cursory" or "don't burn tokens" → Pass 1 only, and offer Pass 2 at the end. If the owner says "deep dive" → Pass 2, running Pass 1 first when no file exists.

**The Coverage table** sits near the top of every game file. One row per layer, columns for Pass 1 and Pass 2 with a date or "not done", and a "Where" column pointing at the section. The next pass reads this table first and skips what is done. Data worth keeping so it is never fetched twice (App IDs, links, review totals with the date, the five top and bottom reviews, the news list) lives in the file itself.

## Pass 1: cursory (small, cheap, always first)
Aim for a few tool calls and a half-page result.
1. **Resolve identity.** `node tools/steam.mjs search "name"` for the App ID, plus the demo and DLC IDs; confirm with `details`. Non-Steam: read the itch.io page or app-store listing with WebFetch.
2. **Store facts.** `details` and `tags`: release date, price, developers and publishers, languages, controller and accessibility flags, AI disclosure, short description, screenshot and trailer URLs, last five news titles.
3. **Review totals.** `summary` (all languages and English). Add a block to `review-tracker.md`.
4. **Top reviews.** The 5 most-voted positive and 4 most-voted negative English reviews, quoted with hours and votes.
5. **Reddit map (read-only tools).**
   - Test for a dedicated subreddit by **guessing names with `reddit_subreddit_about`** (for example `BerryBuryBerry`). Do **not** use the community search first: it returns long descriptions of unrelated subreddits.
   - Then run **one exact-name post search at limit 100** and **tally by subreddit with a script** (posts, upvotes, comments). Big results are saved to a file; parse them, never read them raw.
   - Decide **where the real conversation lives**: a dedicated sub is only useful if it has activity (subscribers, posts a month); otherwise the likely shared sub is the genre sub (for incrementals, r/incremental_games), plus creator communities and recommendation threads. Many hits are unrelated threads where the game is only mentioned; do not count them as discussion.
   - List the **top 3 threads by comments** for Pass 2.
6. **Social accounts.** Take every outbound link from the store page (`linkfilter` targets), the developer page and the news posts. Look for Discord, X/Twitter, Bluesky, YouTube, TikTok, a website. One web search for the developer name if nothing turns up. Report "none found" as unknown, not absent.
7. **Other platforms.** From the store text and one web search: web builds (itch.io, Newgrounds, CrazyGames, Game Jolt, Kongregate), consoles, mobile, Mac and Linux. Say what was searched.
8. **Write the one-paragraph verdict** and the Coverage table, then **offer Pass 2**.

## Pass 2: deep (adds layers; reuses Pass 1)
Run only the layers the Coverage table shows as not done.
1. **All reviews.** `reviews <id> negative english <file>` and `positive english <file>`. Run the analysis script: keyword theme counts (approximate), median playtime of positive versus negative reviewers, the top positives by votes. **Read every negative review** when there are fewer than about 300 and group by theme. Never sample 15 and generalize.
2. **Full news and patch notes.** `news <id>` as full text. Build the dated update table. The first patches show the first complaints; developer posts state intent.
3. **Steam community.** `threads <id> 40`; read the titles, then open the few that show mechanics or complaints (bug lists, "is X overpowered", "games like this").
4. **Reddit threads.** Fetch comments for the **top 3 threads from Pass 1**, top-sorted, depth 2, **save and flatten with a script**. Pull out the **developer's own replies**.
5. **Pictures.** Download the store screenshots to scratch, shrink to about 960 px, and view them. Do not save them to the repo unless asked. Owner-supplied screenshots are converted to WebP, numbered in play order and indexed.
6. **Creator coverage.** Names from reviews and Reddit; for a channel scan, parse the channel page and read each description for game links.
7. **Synthesize** the file from the template: mechanics with evidence tags, patch history, what players love and push back on with counts, a comparison with our design, why it matters (borrow, avoid, risks marked as guesses), open questions, the owner's impressions verbatim.
8. **Housekeeping.** README index row, tracker block, memory note if the owner states a new preference.

## Other modes (use the same two-pass logic)
| Mode | Pass 1 | Pass 2 |
|---|---|---|
| Survey (top games in a tag) | merged tag listings, ranked table, short descriptions | per-game cursory cards for the chosen few |
| Channel scan | video list, game names and Steam links | Steam facts per game |
| Mechanics mining | store text plus top reviews per game | all reviews and threads for the games that matter |
| Topic study | outline, 5 to 10 games, one finding each | full evidence per finding |

Survey notes: Steam search caps at 3,000 results per query, so run the tag with several orderings (newest, review order, relevance), merge, de-duplicate, filter by date and sort by English review count; check the top 30 appear in the review-ordered list.

## Guardrails (learned the hard way)
- **Never play, install, download or run a build.** Viewing store pages and store screenshots is fine.
- **Declare the pass, and reuse the file.** Re-fetching what the file already holds wastes tokens.
- **Say what is a guess.** Inference is tagged. A quote cut off in a review sample stays cut off; do not complete it.
- **Check claims that sound official.** A patch note about "getting verified on Steam Deck" is not a verification.
- **Keyword counts are approximate** and mention a topic, not its sentiment.
- **English-only versus all-language counts** differ; use `summary` for quotable numbers.
- **Tool traps:** the Reddit community search and big comment trees return 50 to 150 KB; guess subreddit names, cap limits, and flatten by script. The Steam `find` count is English only.
- **Do not volunteer sales or revenue numbers**, even when a developer post gives them.
- **Fictional companies only** in any design the owner builds from this.
- **Token discipline:** scripts for big JSON, outputs to files, read only what is needed.
- **No subagents** unless the owner asks.

## What a skill would need
- **Trigger phrases:** "quick look", "cursory", "deep dive", "analyze this game", "what are the top ...", "what is this channel covering", a Steam or itch.io link.
- **Inputs:** a game name or URL; the pass is chosen by the rule above and stated first.
- **State:** the game file and its Coverage table (the skill reads it before fetching anything).
- **Bundled scripts** (now in the session scratchpad, to move into `tools/`):
  - `steam.mjs` (already in `tools/`)
  - `quick_facts.mjs`: compact store facts plus top reviews for a list of App IDs
  - `review_analyzer.mjs`: theme counts, median playtimes, top positives, all negatives
  - `reddit_flatten.mjs` and `reddit_tally.mjs`: comment trees to flat text; subreddit tally of a search
  - `social_links.mjs`: outbound links from the store page, developer page and news
  - `listing_merge.mjs`: merge tag listings, filter by date, rank by count
  - `yt_channel.mjs`: channel page to video list to game links
  - `shots.py`: shrink, view and convert screenshots
- **Outputs:** a new numbered file (or a combined quick-look file) with a Coverage table, a README row, a tracker block, and a short chat summary ending in one question.
- **Rules to carry:** the guardrails above, plus the owner's standing preferences from memory.
