# Competitive Analysis Playbook

How we research competing games, where the results live, and how to look things up. **Written to move between projects**:
sections 1 to 11 are project-neutral; section 12 is the project-specific part to replace. Keep this file current: see
section 13.

*Last updated 2026-10-21. Current state: 3 per-game files, 1 discovery write-up, 1 topic study, a dated review tracker, one screenshot folder (empty). All data is from Steam's public endpoints; no hands-on notes yet.*

---

## 1. Principles

1. **Public data first, then hands-on.** Steam store data, patch notes and reviews come from the API. The owner's hands-on
   impressions and screenshots are added on top and are labelled as such.
2. **Say what you know and how.** Every file opens with a research date and states what was read, what the owner played and what is
   second-hand. **Claude never plays the games** (owner rule, 2026-10-22). Anything not seen is marked *unverified*, *guess* or *not yet played*. Never present a guess as fact.
3. **Surface before you dig.** Discovery produces a short candidate list for the owner to triage. Deep dives happen only on
   what they pick. (Reason: most newly listed games are near-duplicates; a full file for each wastes the owner's time.)
4. **Read the reviews, don't sample them.** An early pass that sampled 15 reviews per game overstated the AI-complaint share for two
   titles. Paginate through every negative review (they are few) and count; report counts as approximate when they come from
   keyword matching.
5. **The owner's own words are data.** Quote hands-on notes verbatim in a block quote, then interpret separately.
6. **Compare to our design, not in the abstract.** Each file ends with what it means for us: what to borrow, what to avoid.
7. **Keep it cheap to update.** One file per game, one index, one tracker. Small edits, dated.

## 2. Where things live

All under `Docs/competitive/`:

| Path | What it is |
|---|---|
| `README.md` | **The index.** One table row per game (release, reviews, one-line note), the cross-game findings, and the market read. Start here. |
| `NN-game-slug.md` | **One deep-dive file per game**, numbered in the order added. Never renumber. |
| `discovery-YYYY-MM-DD.md` | A discovery pass: method, tag IDs, triage table, what to deep-dive next. |
| `15-refresh-YYYY-MM-DD.md` style files | A dated sweep across all tracked games (patch notes, review pushback, a candidate shortlist). |
| `review-tracker.md` | **Dated log of review counts and % positive per game**, newest block last (before the template comment). Used to see trends. |
| `img/<game-slug>/NN-description.webp` | Screenshots, numbered, with descriptive names. Indexed in the game's file. |
| `tools/steam.mjs` | The helper script for all Steam lookups (section 7). Node 18+, no dependencies. |
| `PLAYBOOK.md` | This file. |
| `<topic>.md` | A topic study that cuts across games (for example scale, camera, legibility). None yet; add them the same way. |

Numbering is by date added, not by release date. A "unreleased" game keeps its number when it ships.

## 3. The workflow

```
discover  ->  triage (owner picks)  ->  deep dive  ->  hands-on notes + screenshots  ->  index + tracker  ->  refresh
```

1. **Discover.** Use tag combinations (`steam.mjs find`, section 7) for the genre and adjacent ideas (setting, mechanic) on released and unreleased games, then enrich a shortlist with descriptions, user tags and review summaries. Name searches only fill gaps. Write the result as `discovery-YYYY-MM-DD.md`. Keep a
   scratch list. Filter against the index so tracked games are not repeated.
2. **Triage.** Show the owner a short table: name, link or App ID, release date or status, reviews if any, and *why it is close
   to our concept*. Ask which deserve a file. Record dismissed ones only if useful ("probably not a competitor: horror").
3. **Deep dive.** Create `NN-slug.md` from the template (section 4). Pull store details, the full news feed and, for released
   games, every negative review. View the store screenshots for visual and UI facts.
4. **Hands-on.** When the owner plays a demo or the game, they send notes and screenshots. Quote the notes, extend the file, copy
   the screenshots into `img/<slug>/` (section 9).
5. **Index and track.** Add a row to `README.md`, add the game to the current block in `review-tracker.md` (or open a new dated block),
   and update cross-game findings if the new data changes them.
6. **Refresh.** Every few weeks (or at a release, patch or sale), re-run summaries for all tracked games, append a tracker block, and
   write a dated refresh file if anything notable changed.

## 4. Per-game file template

```markdown
# Game Name

*One italic paragraph: research date; what was read (store page, news, N reviews); what the OWNER played (and for how long); Claude does not play
and what was not; where screenshots came from; where an interpretation is marked.*

- **Steam:** link (App ID) [+ demo App ID]
- **Release:** date (and "unreleased" / "slipped" notes)
- **Price:** (or "not listed yet")
- **Reviews (YYYY-MM-DD):** score bucket, total, % positive (pos / neg), English subtotal
- **Developer/publisher:** and team size if known
- **Platform:** OS, players, languages, controller support, minimum spec, accessibility flags
- **Store tags:** the user tags from the store page
- **Generative-AI disclosure:** what the page says (or "none on the page")

## Premise and scale
## Mechanics and abilities          (verbs, upgrade ladder as a table: name | effect | price/level)
## What the screenshots show        (numbered; interpretation marked)
## Patch / update history           (table, if there is a news feed)
## What players love / push back on (released games: counts + quoted themes)
## How it differs from / resembles our design   (a short table)
## Why it matters to us             (numbered: borrow, avoid, risks marked as guesses)
## Open questions
## Kolby's / the owner's impressions  (verbatim quotes; "(not yet played)" if empty)
## Screenshot index                 (table: # | file | what it shows)
```

Rules: numbers get a unit and a date; links to the store; no marketing language; every claim traces to the page, a patch note, a
review or a screenshot. A game that has not been played says so in the first paragraph and in the impressions section.

## 5. The review tracker

`review-tracker.md` holds one block per check:

```markdown
## YYYY-MM-DD

| Game | Score bucket | Total reviews | % positive | Δ reviews since last | Δ % since last |
|---|---|---:|---:|---:|---:|
| Name | Very Positive | 1,234 | 94.4% | +50 | -0.1 |
```

- Counts are **all languages, all time**, from the `appreviews` summary, so they can run higher than the store page shows an
  English visitor. Use one method every time.
- Unreleased games get a row like `No reviews (unreleased, Nov 23)` so they are visible when they ship.
- Add the new block above the `<!-- template -->` comment at the bottom. Never edit old blocks.

## 6. The index (`README.md`)

- One table row per game, grouped by when it was added (baseline, then "Added in the YYYY-MM-DD refresh"). Columns: number, linked
  name, release, reviews, one-line note that says what is distinctive (and "Played by <owner>" when true).
- Cross-game findings sit below the tables: what negative reviews reveal, what the market shows, ideas worth stealing. Update the
  counts (for example the AI-mention table) when new reviews change them, and date the change.
- Keep the header banner pointing at the latest refresh.

## 7. Looking things up on Steam (`tools/steam.mjs`)

```bash
cd Docs/competitive/tools
node steam.mjs search  "tidy up" "library" "archivist"     # name search: id | name | date | price | reviews | blurb
node steam.mjs details 5169710 5183750                      # store facts, languages, spec, news titles, screenshot URLs
node steam.mjs summary 2589500                              # review totals, all languages and English
node steam.mjs reviews 2589500 negative english out.json    # every negative English review (paginated), saved raw
node steam.mjs reviews 2589500 negative all                 # prints the 25 most-upvoted if no outfile
node steam.mjs news    5169710                              # full patch-note text, dates converted
node steam.mjs tags    5169710                              # user tags scraped from the store page HTML
node steam.mjs find    560542,1084988 300 Reviews_DESC      # games carrying ALL tag ids; add `- comingsoon` for unreleased
node steam.mjs threads 5169710                              # community discussion threads: authors, replies, developer answers
```

The underlying endpoints (use them directly if the script is unavailable):

| Need | Endpoint |
|---|---|
| Store facts | `https://store.steampowered.com/api/appdetails?appids=<id>&cc=us&l=english` |
| Review totals | `https://store.steampowered.com/appreviews/<id>?json=1&num_per_page=0&language=all&purchase_type=all` |
| Review text | same with `filter=recent&review_type=negative&num_per_page=100&cursor=<cursor>` (paginate with the returned `cursor`, URL-encoded) |
| News and patch notes | `https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=<id>&count=100&maxlength=0&format=json` |
| Name search | `https://store.steampowered.com/api/storesearch/?term=<q>&l=english&cc=us` |
| User tags | not in the API: fetch the store page HTML and read `class="app_tag"` (send cookie `birthtime=0`) |
| Community threads | `https://steamcommunity.com/app/<id>/discussions/`, then each topic page. The text is embedded as JSON in the page source (`InitializeForumTopic`, `comments_raw`), so no login is needed. A developer reply shows as the developer's handle. |
| Tag listing | `https://store.steampowered.com/search/results/?query&tags=<id>,<id>&start=0&count=100&infinite=1&cc=us&l=english&category1=998` returns JSON with `results_html`. Add `&sort_by=Reviews_DESC` or `Released_DESC`, and `&filter=comingsoon` for unreleased games (the default hides them). Tag IDs: `https://store.steampowered.com/tagdata/populartags/english`. `tools/steam.mjs find` wraps this. |
| Other sources | itch.io page and comments (a demo or earlier build is often there), Incremental DB, SteamDB (returns 403 to scripts). Web search finds little on unreleased games. |

**Known traps**
- **Dates.** `ISteamNews` and review timestamps are unix seconds. A web-fetch summariser once converted them to the wrong year. Use
  the script, or `new Date(sec*1000)`.
- **`num_per_page=0` gives a summary only when unfiltered.** With a `review_type` filter, page through the reviews.
- **A demo is its own App ID** (`demos` in `appdetails`) with no reviews of its own; the game page carries the reviews. DLC may share names.
- **Search is by name only.** It cannot filter by date. To find *recent* games, run many name terms and filter the result by release
  date, or use the App ID as a rough age (new IDs are larger). "Announced this week" cannot be dated: the API does not give the
  date a store page went public.
- **Coming-soon pages** often have no price, no reviews and a placeholder date; treat the date as the developer's stated one.
- **Rate limits.** The script waits between calls; if a result is empty, wait and re-run before concluding there is no data.
- **`ai_content_description`** is the store's own AI-disclosure field; it is empty on most pages. Absence is not evidence of "no AI".
- **`find` review counts are English-only.** The search tooltip is language-filtered, so counts and percentages from `steam.mjs find` are lower than the all-language `summary` figures (a game showing 4,893 reviews in `find` had 18,856 in `summary`). Use `summary` for anything quoted.
- **Ranking by tag overlap can miss the biggest sellers.** A weighted-tag shortlist skipped *Shelldiver* (18,856 reviews) because it is a cozy management incremental, not a defence game. Before treating a shortlist as "the market", also read the top 10 by review count for the broad tag pair (for example Incremental + Relaxing).
- **Name search is a poor discovery tool; tag search is better.** Combine tag IDs (AND) instead of guessing names. Steam's `term=` text search is fuzzy ("no lose condition" returns unrelated games) and SteamSpy's tag endpoint returned unrelated titles, so neither is used. To test a design feature such as "no fail state", scan review text and the full store description for phrases and *read the snippets*; absence of a hit is not evidence.
- **Do not play web builds or demos.** Owner rule, 2026-10-22: "you should never play these." Do not open an itch.io or Steam demo to try it, and do not download or run a build. Read-only viewing of store pages, store screenshots and trailers stills is fine. (Before this rule, ShredDead's web build was played for about 10 minutes; see `01-shreddead.md`.)
- Keep scratch output out of the repo (use the session scratchpad); only conclusions and quotes go into the docs.

## 8. Analysing reviews

1. Pull **all negative reviews** for the language(s) that matter (English, plus the top of the largest other language if it is
   big). Save the raw JSON to scratch.
2. Read every one if there are under about 150. Group by theme; quote the sharpest lines with votes and playtime.
3. Count themes with keyword matches (`story|plot|ending`, `short|abrupt`, `bug|crash|stuck`, `AI|generated`, `price|refund`,
   `unpacking`...). **Say "keyword matches, approximate".**
4. Mine a sample of positives (or all, by keyword) for what players praise, and take the top 10 to 15 by votes.
5. Compare the median playtime of negative and positive reviewers; note the review trend by month for a long-tail signal.
6. For AI, record **AI-mentioning negatives / all negatives** per game. The pattern so far: it is the dominant complaint for
   some titles and nearly absent for others. A "no AI" claim is a liability unless every visible asset can back it.
7. Report non-English themes you could read, and flag any single reviewer's factual claim as *unverified*.

### Estimating sales from reviews

Steam publishes **no sales figures**; SteamSpy, SteamDB and the paid trackers all estimate (SteamSpy returned an empty "0 to 20,000 owners" for a game with 2,008
reviews, and Gamalytic and Video Game Insights pages gave no data to a script). When the owner asks "how well did it sell" or "what is a normal review rate":

1. State plainly that it is an estimate and give a range, never one number.
2. Use **units = reviews x multiplier** (the Boxleiter or NB number). Secondary sources: Birkett 2018 (median about 77x), GameDiscoverCo 2020 (lifetime median 58x,
   2020 releases 38x, plan 20x to 60x; 1,000 to 10,000 reviews averaged about 53x), VG Insights 2021 (about 30x). A calculator site uses 30x (20x to 40x) for
   2022 to 2026 and says cheap games run higher; that is its own synthesis. Typical error is about plus or minus 50%.
3. Invert it for "what percentage review": 1 in 20 to 1 in 50, i.e. **2% to 5% of buyers**; about 3% is the planning figure.
4. Say which sales it **excludes** (mobile ports, DLC, bundles, other stores, key sales) and which way engagement signals push the multiplier (cheap price, high
   review count, many languages and long playtime push it up).
5. Revenue is a further guess: realised price after sales and regional pricing, then Valve's 30%, refunds and tax. Give an order of magnitude only.
6. Keep the count per language (the store summary takes `language=<name>`), since the multiplier moves with the language mix.
7. **Calibrate with developer-stated figures** whenever a news post gives copies sold (search the news feed for "copies", "sold", "wishlists"). Weekly review counts come from `https://store.steampowered.com/appreviewhistogram/<appid>?l=all` (`results.rollups`, one row per week). 2026-10-21 anchors: *Bills Must Be Paid* 100,000+ copies in under 72 hours against 3,811 reviews in week 1 (about 26x to 40x); *Shelldiver* 10,000 copies in 24 hours and about 800 reviews at about 3 days (about 30x to 45x). **Working figure for a $3 to $7 incremental at launch: 30x to 40x.** Add new anchors here as they turn up.

## 9. Screenshots

- **Owner-supplied** (hands-on): copy into `img/<slug>/NN-description.webp`, numbered in the order sent, with descriptive file
  names (what is on screen, not "screenshot"). Spot-check two or three files against their names after copying.
- **Store screenshots**: the `details` command lists the URLs. View them in the in-app browser (navigate to the image URL and take
  a screenshot) rather than downloading. Save them to the repo only if the owner asks.
- Owner screenshots often arrive **out of play order**. Work out the order from the numbers on screen (currency, kill counts), name the files in play order, and say so in the index. Convert PNG to WebP with Pillow (`Image.save(path, "WEBP", quality=88)`); Pillow is installed.
- Add a **Screenshot index** table to the game's file: `# | file | what it shows`. Describe what is visible, mark inferences.
- Size is not a concern for the repo. **Commit only `Docs/competitive/` and ask the owner before each commit.**

## 10. Hands-on notes

When the owner sends play notes and screenshots:
1. Put their notes verbatim in a `>` block under "notes from the demo".
2. Interpret separately: what each screenshot shows, what it implies for our design, where numbers disagree (for example a counter
   versus an estimate) and say so.
3. Fix anything earlier in the file the new evidence contradicts, and say what changed.
4. Add an index row, a tracker row if new, and update the cross-game findings if the lesson generalises.

**Claude does not play builds.** Tag every mechanic by evidence: *player report* (reviews, Steam threads, itch comments), *dev claim* (store text, patch notes, developer replies), *screens* (store or owner screenshots), *inference*. Say in the first paragraph that nothing was played by Claude, and ask the owner for their impressions; the owner's notes go in the impressions section.

## 11. Honesty rules (the ones we have had to learn)

- State the research date and what was and was not played at the top of every file.
- Do not infer an item count, ordering rule or ability effect from marketing text without saying it is the developer's claim.
- Do not call a game "new" or "announced this week" unless a date supports it.
- When a number can be read two ways (a store page says 400 shelves, the game shows 400 collections), report both.
- Mark risks that are your own inference as **guesses**.
- A review score does not equal feel: `Supermarket Chaos` was Very Positive and the worst hands-on experience tried. Always pair the
  score with what the owner actually played.

---

## 12. Project-specific part (ClickOne): fill in

**What the owner rejects (hands-on, 2026-10-21):** the pixel-art zombie look of ShredDead ("looks terrible"), and *BEAM Inc.* ("The game sucks. It's really awful in terms of mechanics and looks"; flat neon outlines, rounds that start at 10 shots and 2 kills, clipped UI text). Use them as negative examples: [03-beam-inc.md](03-beam-inc.md).

**What the owner likes (2026-10-21):** a **single screen**; **aiming as the only input, with an opt-out** (skipping aim costs early efficiency); **DNA-style spending** on many things; and **no way to lose** ("potentially interesting"; precedent and evidence in [discovery-2026-10-21.md](discovery-2026-10-21.md)).

**Our concept (2026-10-20, from the owner, still broad).** A stress-free incremental / auto-battler in the mould of [ShredDead](01-shreddead.md)
but in a **space or fantasy** setting (open choice), with a look the owner finds much stronger than ShredDead's pixel-art zombie street.
Not yet decided: platform, price, camera, whether there is any risk layer, the core economy, session length. Update this paragraph when
they are.

**What we look for in a competitor.** *Draft, to confirm.*
1. Core loop: what the player does by hand versus what runs itself (ShredDead: aim only).
2. Economy and progression: the upgrade ladder (tree, currencies, prestige or reset layers) and what the player is trading off.
   ShredDead's distinctive move is buying **harder enemies** for more yield.
3. Tension without a fail state: what keeps a stress-free game interesting (optional risk bursts, choices that matter, visible growth).
4. Spectacle and feedback: how growth is shown (unit counts, damage numbers, effects), camera and art legibility at scale, and the
   quality of the first minute.
5. Feedback tools players expect: stats, per-unit income, drop clarity, pause on menus.
6. Session shape: run length, endgame, offline or background play, save handling and any wipes.
7. Trust and price: generative-AI disclosure (and whether sources disagree), price versus length, how reviews react.
8. Input: mouse-only, controller, accessibility flags.

**Where it feeds our work.** *No backlog or design docs exist in this folder yet.* Add the path here when they do.

**Tracked games (2026-10-21):** [BEAM Inc.](03-beam-inc.md) (played by the owner, rejected), [Zero Stress King](02-zero-stress-king.md) (owner has bought it; notes and screenshots pending), [ShredDead](01-shreddead.md) (unreleased Oct 26; web build 0.1.3 was played by Claude for about 10 minutes before the no-play rule of 2026-10-22,
Steam demo not played).

**Known gaps (to do when asked).**
- ShredDead: the owner plays the Steam demo (App ID 4900640); re-pull reviews, price and threads on 2026-10-26.
- **Next job:** the owner picks which discovery candidates get a deep-dive file. Suggested: Zero Stress King, Farming with lasers, Everbastion, The Wall Mustn't Fall (see [discovery-2026-10-21.md](discovery-2026-10-21.md)). Next Fest demos for the unreleased ones end Oct 26.
- Store screenshots are not saved in the repo.

## 13. Keeping this file current

**Update `PLAYBOOK.md` whenever the way we do this changes**, in the same change as the work:
- a new data source, trick or trap (add to section 7 or 11),
- a new folder, file type or template field (sections 2, 4),
- a changed convention (numbering, tracker format, commit rule),
- the project-specific part (section 12) when the concept, priorities or highest-value competitors change,
- the "Last updated" line and the changelog below.

If the script changes, change its header comment and the command list in section 7 together.

## 14. Moving this to another project

Copy `PLAYBOOK.md` and `tools/steam.mjs`. Create `README.md`, `review-tracker.md` and `img/` from the formats above. Replace section 12
with the new project's concept and criteria, empty the "Current state" line, and delete the changelog entries that do not apply. Sections
1 to 11 and 13 should work unchanged for any Steam-based competitive study.

## Changelog

- **2026-10-22** The research workflow is now a project skill, `.claude/skills/game-research/` (two passes, a Coverage table in each game file, helper scripts for Reddit, social links, reviews, surveys and channel scans). See [research-steps.md](research-steps.md).
- **2026-10-22** Owner rule: Claude never plays games, demos or web builds. Removed the "Claude-played-build" guidance (sections 7, 10) and the itch.io play instructions. Added [research-steps.md](research-steps.md), the workflow written out for a future skill.
- **2026-10-21** Topic study `short-incrementals-what-works.md`; added the weekly review histogram endpoint and two real sales anchors to section 8; topic-study file type is now in use.
- **2026-10-21** BEAM Inc. hands-on notes: added the `find` English-only trap, the shortlist-bias trap, the out-of-order screenshot rule, and the owner's rejections in section 12.
- **2026-10-21** Added "Estimating sales from reviews" (section 8) after the Zero Stress King deep dive; first tracker block with two games.
- **2026-10-21** Added tag-based discovery (`find` command, section 3 step 1, section 7 endpoint and traps) after a full discovery pass, the `discovery-*.md` file type, and the owner's three liked features in section 12.
- **2026-10-20** Added the `threads` command, the itch.io web-build trap and the Claude-played-build rule (sections 7, 10), after ShredDead's mechanics turned out to be readable from a web demo and 15 community threads.
- **2026-10-20** Moved to a new project (ClickOne) per section 14. Section 12 reset to a fill-in stub. `tools/steam.mjs` rewritten from the
  section 7 description (it was not carried over) and gains a `tags` command. First game: ShredDead.
- **2026-10-20** First version (BookBoy / Lamplit project). Captured the workflow used 2026-09-06 to 2026-10-20 and added `tools/steam.mjs`.
