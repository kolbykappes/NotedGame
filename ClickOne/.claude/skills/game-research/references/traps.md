# Traps and quirks learned on this project

## Steam
- **Sale price versus list price.** `steam.mjs find`/`details` and the store API report the *current* price. A game on sale shows its sale price (Berry Bury Berry read $5.59 for days and was $7.99 list; 30% off). Record both; `quick_facts.mjs` prints "X% off Y list".
- **`find` review counts are English only.** The store search tooltip is language-filtered. Use `summary` for all-language totals and quote the date.
- **Search cap.** A store search returns at most 3,000 results; run several orderings and merge (see `modes.md`).
- **Shortlists by tag overlap miss big sellers** (the cozy management incrementals). Before treating a list as "the market", also read the top by review count for the broad tag pair.
- **Name search is fuzzy** (`term=` text search returns unrelated games). Prefer tag IDs; check by reading snippets.
- **Demos have their own App ID and their own (usually empty) review pool.** A demo can be outdated against the full game: say which was seen.
- **Reviews:** `appreviews` returns the most helpful first. A "top 5" is not a sample; say "most-voted". For counts and themes use the full pull.
- **A "verified" or "supported" claim in a patch note is a goal**, not a status.
- **SteamDB returns 403; SteamSpy and paid trackers are estimates.** Do not use them for numbers the owner will quote.

## Reddit (read-only MCP tools)
- **Community search returns long descriptions of unrelated subreddits.** Guess names with `reddit_subreddit_about` first.
- **Big results (50 to 150 KB) are saved to a file**; never read raw. Use `reddit_tally.mjs` (search results) and `reddit_flatten.mjs` (comment trees).
- **Many search hits are unrelated** threads where a commenter mentioned the game. `reddit_tally.mjs --name` separates them. Do not count raw comment numbers as discussion.
- **`listing_status: truncated`** on a comment tree means more comments exist than were returned.
- A dedicated subreddit with ~100 subscribers is a fan and help board. Design discussion lives in the genre sub and in the developer's launch threads. A developer's own replies (`--author`) are the most useful lines.
- **Read-only only:** never post, comment, vote, message or submit with the Reddit tools.

## YouTube
- Fetch the channel `/videos` page with a browser user-agent and a consent cookie; parse `ytInitialData` (new layout uses `lockupViewModel`). Only the first page (about 30 videos) is reachable this way.
- Descriptions, not titles, name the game.

## Honesty
- Keyword theme counts mention a topic; they do not measure sentiment. Say "approximate".
- Do not complete a truncated quote. Do not turn a reviewer's factual claim into fact (mark it unverified).
- State the research date and what was and was not read at the top of every file.
- A review score is not feel: pair it with what the owner actually played.
- Do not volunteer sales numbers. If a developer's post gives some, they are the developer's claim and not needed.
