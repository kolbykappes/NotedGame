# Other modes (same two-pass logic)

| Mode | Pass 1 (cursory) | Pass 2 (deep) |
|---|---|---|
| **Quick looks at several games** | One combined file `quick-looks-YYYY-MM-DD.md`: per game, `quick_facts.mjs` output boiled down to a table row and a few lines, plus a "what to mine" line | Promote a game to its own numbered file; reuse its row |
| **Survey** (top games in a tag or period) | `listing_merge.mjs` over several listings, ranked table, short descriptions to split real incrementals from tag abuse | Cursory cards for the few the owner picks |
| **Channel scan** | `yt_channel.mjs @handle --desc`, then `quick_facts.mjs` on the Steam IDs | Deep dive on chosen games |
| **Mechanics mining** (what to borrow) | Store text plus top reviews per game; group by mechanic with the warning each carries | All reviews and threads for the games that matter |
| **Topic study** | Outline, 5 to 10 games, one finding each | Full evidence per finding |
| **Discovery** (candidates for a concept) | A triage list; nothing deep | Deep dives only on what the owner picks |

## Survey recipe
1. Get tag IDs from `https://store.steampowered.com/tagdata/populartags/english`.
2. Steam search returns at most **3,000 results per query**, so run `steam.mjs find <tagId> 3000 <sort>` three ways into scratch files: `Released_DESC`, `Reviews_DESC`, and `-` (relevance). Retry a failed run with a lower max.
3. `node .../listing_merge.mjs --since <date> --top 40 --review-order <reviews_desc.txt> <all three files>`.
4. Read the "coverage" line it prints: the top 30 must appear in the review-ordered listing.
5. Counts are **English only**; Chinese-market games are undercounted. Dates may be 1.0 dates for long Early Access games. Fetch short descriptions (`quick_facts.mjs --pos 0 --neg 0`) for the top 50 and set aside tag-abuse games (clicker toys, visual novels, dating sims, MMOs) with a reason.
6. Group the result: already studied, closest to the owner's brief, different genre. Then offer quick looks at the closest.

## Channel scan recipe
1. `node .../yt_channel.mjs @handle --desc --json scratch/channel.json` (first page only, about 30 videos).
2. Titles are clickbait; the description's first words name the game and carry the Steam link.
3. `quick_facts.mjs` on each App ID (batch them; use `--pos 0 --neg 0` for a cheap pass) and make a table: released versus coming soon, price, reviews, one-line premise, video views and age.
4. Views are attention, not sales. Do not turn them into sales numbers.
5. Flag the few closest to the owner's brief and offer quick looks.

## Combined quick-look file layout
```
# Quick looks: <theme>
*date; what was read; nothing played by Claude; evidence tags.*
## At a glance   (table: game | release | price | reviews | length | one-line loop)
## 1. Game       (what it is [store]; reception [player report]; Reddit/community; why it matters)
...
## What they have in common [inference]
## Questions for the owner
```
