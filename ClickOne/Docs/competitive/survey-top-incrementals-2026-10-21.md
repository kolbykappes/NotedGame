# Survey: most-reviewed Incremental games released in the last 3 years

*2026-10-21. A shortlist only, no deep dives. Review count is the proxy for how well a game sold.*

## How this was made
- Steam tag **Incremental (560542)**, games released **2023-10-21 or later**, ranked by **English review count** (the only count the store search returns; see the PLAYBOOK trap on this). Chinese-market games are therefore **undercounted**.
- Steam search stops at 3,000 results per query, so three listings were merged (newest first, review order, relevance): **4,532 distinct games, 3,485 in the window**. All of the top 30 appear in the review-ordered listing, so the top of the table is reliable; a game further down could be missing.
- Blurbs and genres come from each store page's short description. Nothing was played or read in depth for this survey.
- Release dates are as Steam shows them. Several long-running games (IdleOn, The Perfect Tower II, Firestone, Farmer Against Potatoes Idle, Orb of Creation) have old app IDs, so the date shown may be a later 1.0 date, not the first Early Access date. Worth checking if one is picked.
- Raw lists and scripts are in the scratchpad, not the repo. Reproduce with `node tools/steam.mjs find 560542 3000 Reviews_DESC` (and `Released_DESC`, and `-`), then filter by date and sort by count.

## Group 1: already tracked or studied
| Game | Released | % | English reviews | Price | Where |
|---|---|---|---|---|---|
| Nodebuster | Aug 13, 2024 | 97% | 10,675 | $2.99 | [short-incrementals-what-works.md](short-incrementals-what-works.md) |
| Berry Bury Berry | Jan 29, 2026 | 97% | 6,501 | $5.59 (sale; list $7.99) | same |
| Digseum | Dec 9, 2024 | 97% | 6,488 | $2.99 | same |
| Bills Must Be Paid | Jul 29, 2026 | 96% | 6,130 | $6.99 | same |
| Shelldiver | Nov 16, 2025 | 98% | 4,895 | $4.99 | same |
| Keep on Mining! | Jul 30, 2025 | 91% | 3,104 | $2.99 | same |
| Zero Stress King: Idle Defense | Mar 30, 2026 | 87% | 1,985 | $6.99 | [02-zero-stress-king.md](02-zero-stress-king.md) |
| Fishing Inc | Apr 14, 2026 | 91% | 1,952 | $3.24 | [short-incrementals-what-works.md](short-incrementals-what-works.md) |
| Cornerpond | Apr 21, 2025 | 96% | 1,766 | $2.99 | same |
| Ropuka's Idle Island | Jan 29, 2025 | 95% | 2,390 | $2.99 | same |

## Group 2: new names closest to the owner's brief (defense, auto-battle, aiming, fantasy or space)
These are the ones to look at first. None has been examined yet.

| Game | Released | % | English reviews | Price | Store blurb, shortened |
|---|---|---|---|---|---|
| **Magic Archery** | Oct 4, 2024 | 98% | 6,395 | free | Train a commoner into the greatest archer; stats, quests, arrows infused with magic. Short. |
| **Tower Wizard** | Jun 19, 2025 | 96% | 7,270 | $2.99 | Build and upgrade a wizard tower; study magic, summon spirits, create buildings. |
| **Farmer Against Potatoes Idle** | Sep 5, 2024 | 94% | 4,682 | free | Fight waves of potatoes; classes, equipment, talents. (Early-access-era app ID.) |
| **Space Rock Breaker** | Dec 19, 2025 | 96% | 2,620 | $2.24 | Short incremental about mining asteroids with guns, then a Plinko ore processor. Space. |
| **Sir, We Have an Orc Problem** | Jul 28, 2026 | 86% | 2,860 | $9.99 | Incremental-style tower defense against an overwhelming number of orcs; bullets, bombs, lasers. |
| **A Game About Feeding A Black Hole** | Dec 15, 2025 | 87% | 2,128 | $4.54 | Short incremental about feeding a black hole by destroying asteroids, planets and stars. Space. |
| **How Many Dudes? The Roguelike Autobattler** | Jul 30, 2026 | 91% | 2,204 | $14.99 | Roguelike "dudebuilder" auto-battler; many units. |
| **Outhold** | Dec 12, 2025 | 86% | 1,863 | $4.89 | Short strategic incremental that blends tower defense with deep meta-progression. |
| **Desktop Defender** | Nov 4, 2025 | 92% | 1,778 | $3.49 | Idle auto-battler that sits in the corner of the screen. |
| **Loot Loop** | Apr 13, 2026 | 92% | 1,802 | $4.99 | Build a party of heroes and send them into dungeons; short, deep skill tree, "zero stress". |
| **Master Healer Kale with useless party** | Jul 2, 2026 | 95% | 1,592 | $4.99 | Short incremental where you are a healer in a party; 200+ skill nodes. |
| **Click and Conquer** | Jun 25, 2025 | 91% | 1,621 | $4.99 | Short, relaxing incremental: drop bombs through fog of war, uncover hidden enemies. (Aimed input.) |
| **The Perfect Tower II** | Nov 30, 2025 | 85% | 3,338 | free | Incremental and tower defense mix; town, resources, minigames. |
| **Upload Labs** | Oct 3, 2025 | 90% | 5,010 | free | Sci-fi management of a computer system built from connected nodes. |

## Group 3: big, but a different genre or a different kind of game
Listed so they are not mistaken for gaps in the survey.

| Game | English reviews | % | Why it is set aside |
|---|---|---|---|
| Bongo Cat | 30,553 | 96% | Desktop keypress toy; tag appears incidental. |
| IdleOn - The Incremental MMO | 21,906 | 75% | Free idle MMO; long-form, multiplayer. |
| Banana | 21,666 | 78% | Pure clicker novelty. |
| Scritchy Scratchy | 9,077 | 94% | Scratch-card incremental; gambling theme. |
| TBH: Task Bar Hero | 8,621 | 62% | Taskbar idle RPG; low rating. |
| Revolution Idle | 7,554 | 86% | Classic prestige-layer number incremental. |
| (the) Gnorp Apologue | 7,423 | 95% | Pure incremental about excessive wealth; $6.99. Possibly worth a short look for tone. |
| Leaf it Alone | 3,515 | 95% | Raking-leaves incremental; cozy. |
| Fortune Mill / Incredicer / Unfair Flips / This Ain't Even Poker | 1,800 to 2,300 | 82% to 88% | Gambling and dice incrementals. |
| Rock Bottom | 1,988 | 94% | Incremental mining; sits with Keep on Mining and Digseum. |
| Idle Fishing / Horripilant / Orb of Creation / Click the Button | 1,650 to 1,980 | 82% to 94% | Other incrementals with no defense, aiming or battle hook. |

Also in the top 60 but not incrementals in practice, so ignored: a visual novel (Crisis: The Third Way), cat and dating simulators, a cooking or "edible toys" story game, sorting and organising games (Sort Them Ducks, Supermarket Chaos), puzzles (Proverbs, Boxes), MMOs (BitCraft), a meme quiz, and several adult-themed novelty games. Chinese titles with strong counts (for example 我的小鲨鱼 at 4,372 English reviews) are undercounted and are novelty or fan-game toys.

## Observations
- **Most reviewed in the window:** the top real incrementals are Nodebuster (10,675), (the) Gnorp Apologue (7,423), Tower Wizard (7,270), Berry Bury Berry (6,501), Digseum (6,488), Magic Archery (6,395), Bills Must Be Paid (6,130), Upload Labs (5,010), Shelldiver (4,895). All are **low-priced (free to $6.99), mostly under $5**. The highest price among the top ten is $6.99. (Team size was not checked.)
- **Free games carry big counts** (Magic Archery, Farmer Against Potatoes Idle, Upload Labs, Revolution Idle, The Perfect Tower II). Review counts for free games are inflated relative to paid ones and should not be read as equal.
- **Ratings:** nearly every game with a count above 2,000 sits at 90% or higher; the exceptions are the free MMO/idle RPG games and novelty clickers.
- **Fantasy is well represented, space is thin.** Tower Wizard, Magic Archery, Loot Loop, Master Healer Kale and Outhold are all fantasy. Space has Space Rock Breaker, A Game About Feeding A Black Hole and Upload Labs (sci-fi, not combat). That may be an opening, or a sign of lower demand; this survey cannot tell which.
- **Ratings by type are mixed, not clearly split.** The paid defense games are lower (Sir, We Have an Orc Problem 86% at $9.99, Zero Stress King 87%, Outhold 86%) while Magic Archery is 98%, Desktop Defender 92% and Loot Loop 92%. The small non-combat incrementals mostly sit at 95% to 98%. Too few games to call it a pattern.

## Suggested next step (not started)
Pick **three to five** from Group 2 for a quick look (store page, trailer, a few reviews, Reddit if there is a thread) before any deep dive. A reasonable first five for the owner's concept: **Magic Archery**, **Tower Wizard**, **Sir, We Have an Orc Problem**, **Space Rock Breaker**, **Outhold**. Added for the aiming problem: **Click and Conquer**.
