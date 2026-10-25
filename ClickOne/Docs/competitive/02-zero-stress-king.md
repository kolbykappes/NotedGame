# Zero Stress King: Idle Defense

*Researched 2026-10-21. Read: the Steam store page and `appdetails` record, the DLC record, all 33 news posts, all 15 community threads (including
the developer's posts), the review summary by language, **all 117 English negative reviews** and **all 887 English positive reviews** (positives by
keyword and the 14 most-upvoted read in full), and the review timeline. One store screenshot was viewed earlier (see the discovery file). **Not
played by Claude.** The owner has just bought it and will send notes and screenshots (section "Owner's impressions" is empty until then). The
trailer was not watched. Mechanics below are tagged **[store]**, **[dev]** (developer post), **[player]** (a player said it), **[review]**, or
**[inference]**. Keyword counts are approximate. Sales figures are **estimates from a rule of thumb**, not data: Steam publishes no sales.*

- **Steam:** https://store.steampowered.com/app/4271160/ (App ID 4271160) | **DLC:** *Beach Episode (Supporter Pack)*, App ID 4456860, $2.99
- **Release:** 2026-03-30 (full release). A free demo opened 2026-02-17 after a playtest; the demo is no longer listed (`demos: none`). Also
  on Android (2026-04-02) and iOS (2026-05-19), both one-time payment, no ads or in-app purchases (**[dev]**).
- **Price:** $6.99 on Steam (2026-10-21). The developer said in the v2.0 post (2026-09-20) that they "plan to raise the price a little" after
  that update; the launch price is not on the record. **[inference]** from reviews ("the $5 sticker price", "€4.69", "$4 even at $1 off"): about $4.99 at launch.
- **Reviews (2026-10-21):** Very Positive, 2,008, 87.8% positive (1,763 / 245). English 1,004, 88.3% (887 / 117).
- **Developer/publisher:** Pauloondra (self-published). Writes in the first person, answers threads personally, and was picked for Tiny Teams Fest, so
  probably one person or a very small team; **unverified**.
- **Platform:** Windows and macOS (macOS since 2026-04-24, achievements fixed 2026-05-09), Steam Deck **Verified** (2026-06-26), Android, iOS.
  Single-player, Steam Achievements (19, per a reviewer), Steam Trading Cards (2026-06-13), Cloud saves, Family Sharing, Mouse Only, Touch Only,
  Playable without Timed Input. 19 languages. Minimum spec: 64-bit Win10, 4 GB RAM (the page is written as a joke: "Graphics: not really").
- **Store tags:** Tower Defense, Idler, Auto Battler, Strategy, Casual, Incremental, 2D, Short, Pixel Graphics, Colorful, Cute, Resource Management,
  Relaxing, Fantasy, PvE, Indie, Singleplayer, Arcade, Family Friendly, Replay Value.
- **Generative-AI disclosure:** none on the page.

## The pitch

The whole store description is about 70 words: enemies run at your walls; "they can't reach you. There is a river of lava at the end. Every enemy
falls into it. Every time"; "No losing. No pressure. No 'restart the level' moments. Just numbers going up and enemies running."; "a tower defense
where you already won." The short line is "A short relaxing incremental game disguised as a tower defense." **The promise is the product.** Tone is
cheerful and silly (the developer jokes about "stress-kill" in patch notes).

## How it plays

### Map and flow
1. **[store, screenshot]** One screen. A shop panel down the left, a lava column beside it, a field full of small coloured pixel units, and a big
   yellow laser beam across the field. Enemies and defenders share the screen; there is an "unzoom" view and a schematic zoom-out (**[dev]**).
2. **[store/dev]** Enemies run toward your walls and die in the lava. Kills in the lava are counted ("lava frags", "lava kills"); some
   achievements need 100,000 of them.
3. **[player]** Kills must happen **before** the lava to pay out: "enemies die when they hit your lava moat. You need to kill them before they do
   in order to get XP and building resources." Lava kills pay nothing (same structure as ShredDead's railway).
4. **[dev]** Boss fights exist (8 bosses, plus boss challenges up to level 5). From the demo post: "Call bosses any time, **no penalties for
   failing**." **So the strict reading is: enemies cannot reach you and nothing is ever lost, but a boss fight can be failed at no cost.**

### Spending: five layers
5. **Skills ("your conquest map is your skill tree"):** a pannable map; prices visible without hover since v0.941 ("thanks Reddit!"). Some skills are
   infinite (level 20 and beyond). **[dev]** v2.0 added "a pile of new skills, infinite ones among them".
6. **Units ("defenders"):** bought one at a time, with multi-buy (Shift+click, or L2 on Steam Deck). **[dev]** 8 new defenders with new attack
   types in v2.0 (chain lightning, knockback, piercing, slowing "dryads"). **[review]** "cannons vs crossbows" is an obvious-choice trade.
   Many units cost two resources. **[player]** You pick between two unit types per run ("I can't have every unit, this is causing me stress").
7. **Farms and monuments:** production buildings; 8 monuments in the late game. A "builder" beam constructs them.
8. **Resources and XP:** several tiered resources (a player refers to the "last (purple)" one; the exact count is not confirmed) plus XP; "Show Loot" setting; stats view shows loot per second.
9. **Friends (the prestige layer, added 2026-02-12):** reset a run, pick a "Friend" (Nun, Mage, Engineer, Builder, Commander, Blacksmith named so far).
   Friends give a power or a discount (the Nun's ability gives x5 resources and XP; the Blacksmith discounts two-price units). They are unlocked
   with **trophies/cups earned from boss challenges and new enemies**. **[dev]** v2.0: "Trophies are now enough for all friends at once."
   **[review]** Before v2.0 many players could not get every prestige point ("you can't unlock everything", several negatives).
10. **Enemy upgrades:** you can buy more enemies and more enemy XP. A player describes a late run where they "only upgraded enemy count and XP".
    Two negatives mention upgrading the enemy as odd ("i dont see the point of upgrading enemy using skills"). **[player, endless mode request]**
    "Give enemies much more HP for better rewards" and "more risk = more reward" are asked for as **new** features, so the "buy tougher enemies"
    lever is present for count and XP but not for health in endless mode (**[inference]**). Compare ShredDead, which bundles health with spawn rate.

### Active play
11. **[player/review/dev]** The only input is pointing. A manual turret or laser follows the cursor ("Your manual turret does decent damage so it can
    encourage some live play"; "place the laser on a corner and wait"); a "Left click to supercharge your monument" button unlocks mid-game
    (**[review]**: one player says it "turns the game into a chore"). The Nun unlocks a "superbeam from the skies". On Steam Deck: trackpad or
    touchscreen to point, R2 to act. **[review]** "Pure idle in the sense there's very little to do besides watch number go up".
12. **[inference]** Active play is optional and gives a bonus: matches the owner's liked "aim, but you may skip it" structure, with less emphasis
    than ShredDead. The exact efficiency gain is not documented.

### Length and the end
13. **[dev]** "About 6 hours if you do it quick." **[review]** Players finish in 2 to 8 hours; 100% takes about 7 to 24 hours; 100+ hours exist in "Endless
    Mode" (one thread). The first release ended at the final boss with no way to continue; a **total reset** button was added 2026-04-24; v2.0
    (2026-09-20) added islands, 16 mobs, 2 bosses, 2 monuments and trophies enough for all friends.
14. **[player]** Endless mode progression "becomes WAY too slow" late: "left the game running for 4 hours, came back, and could maybe buy two things", while the defense kills enemies instantly. The same player asks for upgrades "that let me intentionally make the game harder".
15. **[dev]** Autosave every minute plus a manual button; triple-redundant save backups (2026-04-16); saves are protected against power loss
    (v2.0). Cloud saves between PC and Steam Deck.

### Controls and quality of life
Window resizable to 320x180 ("nano window mode", 2026-04-08), sounds mute when minimised, screenshake and death-animation settings,
hardware-mouse setting, finger-offset setting for phones, big fonts for Deck and mobile, 19 languages (some added with player help). **[dev]** Beach
mode / Summer mode is a cosmetic and cheat switch tied to the DLC.

## What the store shows

One screenshot viewed (2026-10-21): left shop panel of purchase buttons with prices, a vertical orange lava band, a green field crowded with tiny
coloured pixel units of many kinds, damage numbers, one very bright yellow vertical laser. The look is colourful and simple pixel art, deliberately
cheerful. **[owner to add screenshots.]**

## Update and event timeline

| Date | Item |
|---|---|
| 2026-02-12 | v0.800: **Friends (prestige)**, 1-minute autosave |
| 2026-02-17 | Playtest closed; **demo opens** (bosses, Superbeam) |
| 2026-03-06 | Release date announced for Mar 30 |
| 2026-03-30 | **Launch** and Beach Episode DLC |
| 2026-04-02 / 05-19 | Android / iOS ports |
| 2026-04-24 | macOS support, total reset |
| 2026-06-13 | Trading cards |
| 2026-06-26 | Steam Deck Verified |
| 2026-08-06 | Selected for **Tiny Teams Fest**; big update announced |
| 2026-09-20 | **v2.0** free content update; price rise announced |
| 2026-09-21 to 09-30 | Patches 2.01 to 2.04 |
| 2026-10-08 to 10-22 | "An Event About Incrementals" festival |

33 news posts in 8 months, about one a week. The developer answers bug reports in threads, often the same day.

## What players love / push back on

English reviews, 2026-10-21: 887 positive and 117 negative, all read. Language mix (all-language 2,008): English 1,004 (88%), Russian 192 (88%), Korean 154
(84%), German 134 (92%), Simplified Chinese 107 (**67%**), French 81 (93%), Spanish 64, Portuguese-BR 61 (98%), Japanese 52, Turkish 45, Polish 38.
Simplified Chinese is the one weak language; it was not read.

**What positives praise** (keyword matches among 887, approximate): relaxing, background or "second monitor" play 205; value for the price 165;
"short" said approvingly 160; build choices, friends, prestige 143; art, sound, polish, feedback 121; numbers going up, explosions, lava 79.
The most-upvoted positive (58 votes) is a one-line list: "game running in a background, come back every few hours, get tons of resources,
purchase new upgrades, feel like an absolute gigachad for doing nothing".

**What negatives complain about** (keyword matches among 117, approximate):

| Theme | Count | Note |
|---|---:|---|
| Boring, repetitive, grind | about 30 | |
| "Nothing to do" / screensaver / just idle | about 33 | the cost of the promise |
| Too short | about 29 | 2 to 8 hours at $5 to $7 |
| Shallow or samey upgrades | about 20 | |
| Price or value | about 19 | |
| Balance, pacing | about 16 | early slow, late trivial |
| Bugs, save wipes, crashes | about 11 | several say a patch reset their save |
| Missing challenge, tension, "can't lose" | about 4 | the no-fail design is **not** what people dislike |
| AI | 1 | |

Other specific points: the top negative (100 votes, 0 hours) is about **unremovable pop-up ads for the DLC in the options menus**; prestige points did not
buy the whole tree; two achievements were bugged (100k lava kills, "Pure Perfectinum"); Windows 11 Smart App Control flagged the game and a few players
refunded; macOS achievements did not work until May 9. Median playtime at review: negatives 8.7 hours, positives 13.1 hours.

**Review velocity** (English reviews per month; the roughly doubled September figure matches v2.0 and Tiny Teams): Mar 105 (from the 30th), Apr 413, May 84, Jun 102,
Jul 58, Aug 66, Sep 138, Oct 38 (to the 21st). Percent positive by month: 91, 89, **75**, 86, 93, 94, 91, 87. The May dip coincides with the macOS
achievements problem (**[inference]**). First day (Mar 30): 41 English reviews; first four days: 186.

## Sales estimate (a rule of thumb, not data)

**Steam does not publish sales.** SteamDB, SteamSpy and the paid trackers all estimate. SteamSpy's page for this game is empty
("0 to 20,000 owners", no review data), and the Gamalytic and Video Game Insights pages returned no figures when fetched, so there is no tracker number to quote.

**The convention.** The ratio is "copies per review", often called the Boxleiter or NB number: **units sold ≈ reviews × multiplier**.
Inverted, that is the share of buyers who review.

| Multiplier | Share of buyers who review | Units for 2,008 reviews |
|---:|---:|---:|
| 20x | 5.0% | 40,000 |
| 30x | 3.3% | 60,000 |
| 40x | 2.5% | 80,000 |
| 50x | 2.0% | 100,000 |
| 60x | 1.7% | 120,000 |

**Sources and what they say (all secondary):** Jake Birkett's 2018 developer survey: median about 77x (65x for 2017 releases). GameDiscoverCo (Simon Carless), 2020,
237 games with real sales: **lifetime median 58x; 2020 releases about 38x; suggested planning range 20x to 60x**; and games with 1,000 to 10,000
reviews averaged about 53x versus 36x for under 100 reviews. Video Game Insights, 2021, about 10,000 games: about 30x for 2020 releases. A calculator
site's working figure for 2022 to 2026 releases is 30x with a 20x to 40x band, and says higher-priced games have lower multipliers and cheap games
(under $10) higher ones; it describes those adjustments as its own synthesis, not measured. Typical error is about plus or minus 50%.

**What "decent" means.** For a modern paid release, roughly **2% to 5% of buyers leave a review** (one per 20 to 50 copies) is the normal band;
about 3% (one per 30) is the figure most people plan with; at 4% to 5% players are unusually engaged; under 2% usually means a quiet, older or
less-prompted game. Steam's review prompt (since late 2019) pushed rates up and multipliers down compared with 2014 to 2018.

**Applied to Zero Stress King (my estimate):** the game is cheap, has over 1,000 reviews, a high engaged-player profile (median 12.7 hours when
reviewing; only 37 of 1,004 English reviews were under 2 hours; 988 of 1,004 are marked Steam purchases), a 88% score, and 19 languages. Cheap price,
high review count and wide languages all push toward the **upper-middle** of the range. A reasonable central guess is **30x to 50x, so about 60,000 to
100,000 Steam copies**, with a plausible range of **40,000 to 120,000**. **That does not include** Android, iOS, the $2.99 DLC, or copies sold in bundles
or at deep discount that nobody reviews.

**Calibration added later the same day:** developer-stated launch figures for two similar games (*Bills Must Be Paid*, 100,000+ copies in under 72 hours against 3,811 reviews in week 1; *Shelldiver*, 10,000 copies in 24 hours against about 800 reviews at about 3 days) point to about **30x to 40x** at launch, so the **lower half of the range above (60,000 to 80,000 copies)** is the better central figure; see [short-incrementals-what-works.md](short-incrementals-what-works.md). Zero Stress King's top sellers in the same lane (Shelldiver, Nodebuster, Bills) are estimated at 5 to 10 times as many copies.

**Rough revenue (guess, order of magnitude only):** at roughly $3.50 to $5 realised per copy (launch near $5, sales, regional pricing, now $6.99), 60,000
to 100,000 copies is **about $200,000 to $500,000 gross**, and the developer keeps roughly 55% to 65% after Valve's 30% share, refunds and tax.

**How rare is 2,000 reviews?** Calculator-site data for 2025 releases: about 3% of Steam games reached 1,000 or more reviews (608 of 20,282) and about 6%
reached 500 or more; the median release has about 10. So 2,008 reviews in seven months puts Zero Stress King in the top few percent of releases
(2025 distribution; no 2026 figure was found).

## Why it matters to us

1. **A no-lose, single-screen incremental can be a top-3% seller at $5 to $7.** The store page is about 70 words and the promise is the whole pitch.
2. **The "buy tougher enemies" request is real.** After 100+ hours a player asks for exactly the lever ShredDead sells ("more risk = more reward", more
   HP for more XP). It is a wanted feature in a game that does not have it fully. *Borrow.*
3. **Content length is the binding constraint:** 2 to 8 hours at $5 to $7 draws "too short" from about a quarter of negatives. A free content update
   (v2.0) increased reviews and positivity and was paired with a price rise: a **good pattern to copy** (reward early buyers, raise later).
4. **Do not nag players:** unremovable DLC ads in menus drew the single most-upvoted negative review. The DLC that adds cheat keys also drew open
   resentment (a thread player: "a DLC that allows you to trivialize the game"). *Avoid.*
5. **Do not lose saves** and make achievements reachable; several reviews and threads are about exactly this.
6. **Distribution playbook** (all in the news feed): playtest, then demo about six weeks before launch, then ports (Android, iOS), Steam Deck
   Verified, trading cards, two festivals, frequent patches, and the developer visibly answering. **[inference]** Reddit and YouTube mentions by players
   suggest word of mouth mattered.
7. **Risk (guess):** the "nothing to do" negatives (about 33 of 117) show that "no pressure" can read as "no game" to some. A pointing verb that matters
   early (our aim idea) is the likely answer.

## Open questions (for the owner's playthrough)

- What does the cursor actually do in the first five minutes, and how much does it help? Is aiming optional in practice?
- When do decisions start to matter, and how many hours before the first prestige?
- How do boss fights read (failable, no penalty), and are they the "tension" moments?
- How is offline progress handled (game closed versus minimised)?
- How readable is the skill map, the Friends screen, the Zeropedia?
- Does the pointer verb feel like ShredDead's aim?

**Screenshots wanted** (for `img/zero-stress-king/`): the first minute, the skill map, the Friends screen, the Zeropedia, the stats overlay, a boss
fight, the shop panel when crowded, the settings and the Beach (Summer) mode.

## Hands-on evidence (owner screenshots, 2026-10-21)

Owner's words, verbatim:

> Turns out I had started to play the game some time ago and apparently had returned it. I bought it again for this exercise.

> Here's a sequence of screen shots from the early portion of the game. I did a complete reset and then i'm playing some of the early tech tree and showing the way the left slots work.

> Do you understand the mechanics of this game or have any questions about these screens? It appears that these are most of the screens in the game.

17 screenshots, saved in `img/zero-stress-king/`. They were sent out of play order; the order below is reconstructed from the XP and slot counts on screen (best effort). The owner did a **Total reset** first,
so everything is the very start of the game. Tags: **[screens]** visible on screen; **[inference]** my reading.

**The game has about five screens [owner: "most of the screens in the game"]:** the main battle screen, the **Skills** map, the **Circle of Friends**, the **Zeropedia**, and a small settings row (gear, display, music, a check mark).

### Main screen
1. **[screens]** Layout from left to right: a **slot column** (two columns of slots), a brown **lane** holding the defenders and a monument at the top, a vertical **lava river**, and a green **field** where small brown enemies walk in from the right.
2. **[screens]** Top bar: two resources, **XP** (blue icon) and **bread** (loaf icon). A red badge on the **Skills** button counts things you can afford (1, then 4, then 8 in later shots).
3. **[screens]** The **left slots** start as **dead-tree placeholders** (locked). Buying a Skills-map node turns a slot into a unit button, for example "Buy [archer] 3/10, 150 bread, HOLD SHIFT - Max". The count is owned/cap (3/10, then 10/10, then 10/15 after a cap upgrade). Cost is in **bread**; "Hold Shift" buys the maximum. A **Farm** slot appeared later ("2/8, 500 bread"). One slot shows an **obelisk** icon from the start.
4. **[screens]** The **monument** (obelisk) sits at the top of the lane from the start. In one shot a **bright white beam runs from the monument to the cursor** (a yellow pointing hand), with a looping particle trail where the cursor moved. **[owner, 2026-10-21]** The cursor **only steers** the beam: "mouse seems to not affect the monument - I can just steer, I can't control it". No click or other control was found. The monument can **reach the whole field**. So the pointer verb is steering, with no extra action in the early game.
5. **[screens]** Archers stand in the lane and fire thin white lines across the lava at enemies in the field.
6. **[screens; owner: "I think this is a rolling window. It bounces all over."]** A **counter panel** (top right) reads, e.g., "198 All / 8 [archer] / 0 [obelisk] / 190 [lava]", later "257 All / 78 / 41 / 138" and "260 All / 120 / 0 / 140". The rows are a total and a split by what killed the enemy: archers, the monument, the lava. In the first shot lava killed 190 of 198.
7. **[screens]** A **small box at the bottom of the lane** shows two columns with XP and bread values: the left column (a farm icon) reads 0 / 0 and the right column (an enemy icon) read "0 / 0", "2 / 25", "3 / 30", "6 / 60" in successive shots. **[owner]** "I think this is also a rolling window - not totally sure, it's not intuitive." So treat both this box and the top-right counter as **rolling-window readouts that are hard to interpret**, not per-kill yields. My earlier reading (right column = per-kill yield) is **not confirmed and probably wrong**.
8. **[screens, owner]** The two lighter vertical stripes are the **archer range bands**; they only appear when the cursor hovers over an archer. The small sprites at their foot are archers at the lane's bottom edge.
9. **[screens]** Early speech bubble from a pixel King: **"No swiping today, okay?"** (shown on the first screen after the reset; presumably a mobile-port joke).

### Skills map
10. **[screens]** A **brown island map** on a blue sea, panned and zoomed with PageUp, PageDown or the wheel; a zoomed-out view shows the whole island with **five nodes**, one checked, three with **red dots**, one with a **trophy** marker.
11. **[screens]** Each node is a square icon joined by dotted lines. **Green cost** = affordable in XP, **red cost** = not; a green plus is an upgrade; a gold padlock marks a node that unlocks something; a green check marks one bought; a **gold trophy** above a node marks a node that **unlocks a new enemy** (cost 1000 XP in the early map). XP total shown top right (230, 245, 225, 195).
12. **[screens]** Node examples with tooltip text: **Call upon loyal archers** ("New defender of your majesty. White damage, fast-shooting", 20 XP, then **Bought**); **Farming** ("Making food is easy! (and peasants shouldn't whine so much)", 80 XP); **Productivity** (farm "Production size +10%", 0/10, 24 XP); **All monuments damage** (+25%, 0/10, 320 XP). After archers are bought the map grows more nodes (archer upgrades at 50, 75, 150, 70, 120 and 220 XP, farm upgrades at 30 and 40 XP).
13. **[owner]** The **red dots** on the zoomed-out map show **what is unlocked or not** ("Zooming in and out - seems to be what's unlocked or not"). The **red-haired figure** node unlocks a **rogue**, another unit. **[inference]** The map is the **XP sink and the progression gate**: it unlocks unit types (slots), raises caps (10/10 to 10/15), boosts damage and production, and unlocks enemy types. Bread is the second currency, used only on the main screen.

### Circle of Friends (the prestige screen)
14. **[screens]** A wheel with the **King** at the centre ("This is you, Your Majesty. You do nothing.") and spokes to friends (Peasant, a demon, a nun, a guard, a ranger, a dwarf-like fighter and others) plus locked **door** slots. Each friend shows a **trophy cost** (1 to 3). Trophy counter top right: **0**.
15. **[screens]** Text on the screen: **"Unlock new enemies to get more trophies."** and **"You can freely enable and disable them. But to apply their bonuses, you need to restart the game with them."** Example: **Peasant**: "Production buildings are automated from the start. Grants 10 farms", cost 1 trophy.
16. **[screens]** Two red buttons: **Restart!** (prestige with the chosen friends) and **Total reset** (erase everything). Total reset asks "Do you really want to completely erase your progress and start again?" with a confirm button.

### Zeropedia
17. **[screens]** A grid of **wooden doors**, most closed, a few open showing a unit: archer, monument, windmill, red-haired figure, a **knight on a horse**, and a chain of three knights joined by a dotted curve. Clicking a unit shows a **Base / Now** stat table. Knight: XP 15, bread 200, health 750, speed 23, armour 25%, three resistances "-", weight 200. Monument: damage 30, interval 0.2 s, range 15, 2% (crit chance), x7 (crit multiplier), and two more stats "-". **[inference]** The Zeropedia is a bestiary and unit sheet; doors open as enemies and units are discovered; a Knight that pays XP 15 and bread 200 is an enemy type from later islands.

## What I understand, and what I do not

**I understand:** the economy loop (kill, earn XP and bread; spend XP on the map to unlock slots, caps, damage, enemy types; spend bread on units in the slots; enemies that reach lava die for free), the monument beam as the pointer verb, the prestige wheel (trophies from new enemies, friends enabled before a restart, total reset as the destructive option), and the Zeropedia.
**I do not understand yet** (questions sent to the owner): see the next section.

## Owner's answers (2026-10-21) and what is still open

Owner's answers to the questions from the screenshots, verbatim:

> 1. I think this is a rolling window. It bounces all over.
> 2. I think this is also a rolling window - not toally sure, it's not intuitive.
> 3. Those are the archer range bands - they only show up when I hover on the archer. The monument seems to be able to reach the entire field.
> 4. So far mouse seems to not affect the monument - I can just steer, i can't control it
> 5. That unlocks a rogue - another unit.
> 6. Less than 5 minutes and a lot of that was taking screen shots.
> 7. Zooming in and out - seems to be what's unlocked or not.

What this changes:
- **Counters are rolling windows**, and the owner finds them **unintuitive**: a usability weakness to avoid copying (a readout that "bounces all over" is hard to learn from).
- **The pointer verb is weaker than assumed**: the cursor steers the beam, nothing more, at least early. Compare ShredDead (the owner likes aiming there) and the reviews that call the later "supercharge" click a chore.
- **The early stretch took under 5 minutes** (including screenshots): the opening is fast; several units, slots and nodes unlock within the first few minutes. This contrasts with BEAM Inc.'s first 30 minutes ([03-beam-inc.md](03-beam-inc.md)).
- Archer range is shown **on hover** rather than permanently.

**Still open:**
- Does lava kill pay nothing? (No answer; the first screenshot suggests most early kills are by lava.)
- How long is a whole run before the first Restart? (The first enemy-unlock trophy cost 2,500 XP; see batch 2.)
- Does the monument gain a click action later (reviews mention "left click to supercharge")?
- Does the game pay out while minimised or closed? (Dev said no offline gain at playtest time; see Reddit reactions. Not re-confirmed for release.)
- How many slots exist in total, and does each need its own map node?

## Reddit reactions (r/incremental_games, read 2026-10-21)

Pulled through the Reddit MCP (read-only: search plus comment trees, top-sorted, 2 levels deep). Sources: the playtest post (2026-01-29, 74 up, 15 comments, only about half returned), the demo post (2026-02-28, 281 up, 86 comments), the release post (2026-03-30, 241 up, 68 comments) and the v2.0 post (2026-09-20, 189 up, 37 comments). Roughly 100 comments read of about 200. Tags: **[player report]** a commenter, **[dev]** the developer (Pauloondra) in the thread.

**How the dev ran it** (a pattern worth copying): a Steam playtest plus a free itch.io web build, a post in r/incremental_games asking for feedback, then a Next Fest demo, then release, then a content update. **[dev]** The demo post says feedback came from "more than 100 feedback answers in this subreddit, Google Forms, and Discord", and that prestige, boss fights and the super-beam were added *because* of it. The playtest thread shows the prestige was a direct response to a commenter who found the demo's path-locking redundant without one.

**What players like** (**[player report]**):
- "Super satisfying to kill the enemies, very nice progress."
- The island talent tree: another dev comparing it to Nodebuster-style games said the islands "feel way more organized and give more clear objectives".
- "You would never lose makes the game feels super chill." (The no-lose design is the stated attraction, not a side effect.)
- "One of the better im half paying attention games in the library." The dev agrees: **[dev]** it is "primarily designed to be a second-monitor activity on a PC".
- Price: "Reasonable priced... An idle game? $4 is the perfect spot."
- Players compare it to Plants vs. Zombies; the dev's reply: "like PvZ with unlimited lawnmowers."

**Complaints and friction** (**[player report]** unless marked):
- **Length.** The top reply on the release post: the store page says "Short", reviews say about 5 hours. Another player took 12 hours for every achievement. **[dev]** targeted 6-8 hours at playtest time. The main objection is "short", not "bad".
- **Early choke point is XP.** "The choke point is entirely xp for the first while."
- **Screen shake and animation noise.** One player "can't play the game" without a screen-shake off switch; others asked to disable the XP/gold/wood pop-up animations. The dev added an option for the in-your-face animation.
- **Mouse wheel opens the upgrade screen** with a jarring animation (accidental scrolling).
- **Upgrade screen is cumbersome**: a player who buys "the cheapest thing affordable" has to hover each item to find it. Another asked for an auto-upgrade with a priority tree.
- **Too many prestiges** (one mild comment: "Not super fan of so many prestiges"). **[dev]** replied that the game can be beaten without prestiging.
- **No targeting priority** for defenders; **[dev]** says at least three players asked, but he is unsure it is affordable on CPU late in a run.
- **Mobile or foldable aspect ratio**: the battlefield is fixed at 16:10 **[dev]**.
- Playtest bugs: a fullscreen crash, and a web build that lost its save after a long idle. Both reported early; no sign they persisted.

**Direct answers to open questions** from the screenshots:
- **Offline gain: none at playtest time.** A tester wrote "no offline gain ;(" and **[dev]** replied "There won't be an offline gain, because the whole game will be short as 6-8 hrs or so." Not re-confirmed for release or v2.0. This is the dev's reasoning and the design decision; the owner could still check on screen.
- **Click action exists later.** **[dev]** The demo post lists "a super-beam for non-idle players" and "mega-ray" as features added from playtest feedback, which fits the reviews' "left click to supercharge". So the pointer verb is steering early and a click ability later.
- **Total length** of the first version, per the dev's own words: about 5-6 hours (playtest post), "1-2 hours to complete" for the demo.

**What this means for ClickOne** [inference]: (1) the audience's complaints are about *noise and friction* (screen shake, pop-ups, a slow upgrade screen), which a calm theme can avoid by default; (2) "short" is a complaint even for a cheap game, so the owner's concept should plan a prestige or replay hook before launch, which is what this dev did; (3) the "free web build, then demo, then release" ladder gave the dev a feedback loop that shaped the final game.

**Method and limits:** top-sorted, depth-2 pulls; some branches were not sent ("comment_tree_more"). Counts of comments by sentiment were not made. Only r/incremental_games threads were read in full; the same posts in r/pcgaming, r/Games and r/IndieGaming were not.

## Hands-on evidence, batch 2: bosses and more of the map (owner screenshots, 2026-10-21)

Owner's words, verbatim:

> here's a bunch more screenshots - these show the boss mechanic. It's basically a DPS check that gates content. Some troops come in once I hit the boss button, then they run up to the lava river and then head back to the far right edge. A life bar is along the top of the page showing how I'm doing.

16 more screenshots (files 18-33 in `img/zero-stress-king/`). They were sent out of order; the order is reconstructed from the XP counter, trophy count and node prices (best effort). Tags as before.

### Boss fight (the DPS check)
1. **[owner]** The boss is a **DPS check that gates content**. **[screens]** A red **BOSS** button sits at the bottom right of the field (visible in files 24-26, 29, 33). Pressing it sends in a wave of troops.
2. **[screens]** A **red bar across the top** reads "120K / 120K" at the start (file 27) and "107K / 120K" a few seconds later (file 28): a **shared health pool** for the whole wave that you drain.
3. **[screens]** The wave arrives as **tight rectangular blocks of knights** (about 5 columns wide) at the right edge (file 27), then mass up against the lava bank (file 28). **[owner]** They run to the lava river and then head back to the far right edge.
4. **[screens]** During a boss wave the **lava counter collapses**: 1,042 lava kills in the first shot, 35 in the second, **0** in the third, while the monument's share rises (174, then 405 of 857, then 533 of 707, against 30 to 188 in ordinary waves). **[inference]** Boss troops are not killed by the lava; the damage has to come from archers and the **monument beam**, so this is where steering the beam matters most. The counters are rolling windows (owner), so these are indicative.
5. **[screens]** File 29: the boss bar is gone, the BOSS button is back, and a **giant pixel knight** at the bottom right says **"I'll be back"**. Whether that means you won or the wave simply timed out is **not confirmed** (see questions).
6. **[screens]** A small **red box showing "7"** appears mid-field in file 26. Unknown (countdown, wave number or damage).
7. **[screens]** **Unlock gate:** a node outside the island, "Have a nice picnic in the forest" (new enemy to come), shows **"Beat a boss to unlock this"** in red (file 22). So bosses unlock new enemy types, which feed trophies.

### Map: how the tree shapes difficulty and income
8. **[screens]** **Mobilization nodes add enemies on purpose.** "Rogue Mobilization": "Enemies to spawn: +1 >> +2", 1/10, 100 XP (file 18). "Footman Mobilization": "Enemies to spawn: +1", 0/10, 240 XP (file 20). **[inference]** You pay XP to make more enemies come, because each enemy is income. Difficulty is a player-controlled dial that is also the economy.
9. **[screens]** **Enemy-unlock nodes carry a trophy marker.** "Ask the neighboring king for help": "New enemy to come. Resistant to white damage", 2,500 XP (file 21). After buying it (file 23) **Friends** shows **2 trophies** (it showed 1 before). The horsemen then appear on the field (files 24-25). That answers the "first trophy" question: it cost **2,500 XP**, reached about the 3,000 XP stage of this run. The unlock arrives with a **letter from the neighbouring king** ("This will be a friendly letter.") shown as a speech bubble and a large pixel portrait (file 24).
10. **[screens]** **Mutually exclusive nodes.** "Order cannon deliveries", 2,000 XP: "New defender of your majesty. White damage, pierces one enemy after another", with the warning **"This skill will block another! Choose wisely"** (file 30). A **red line** links it to a sibling 2,000 XP node. After buying it a **cannon slot** appears on the left ("2/2", file 33) with the cannon counted separately in the kill counter (37).
11. **[screens]** **Costs rise as you buy.** Comparing files 30, 31 and 32 (in that order), the same nodes go 440, then 440, then 600; 420, then 550, then 700; 200, then 200, then 300; and the monument-radius node 600, then 900, then 900. Not confirmed whether the driver is purchases, time or enemy count.
12. **[screens]** More node types: "All monuments area size: +2 radius, 0/5, 400 XP" (file 19); **Automation: "Will work even without a king watching", 400 XP** (file 32), on the farm branch; an **hourglass** icon (probably speed or interval) and a **green arrow** icon on several nodes.
13. **[screens]** The slot column is **mostly empty even at about 15,000 XP**: only the archer (25/25, then 30/30 after more upgrades), the obelisk, the windmill (7/11, later 7/14, 1,100 bread) and, after the purchase, the cannon, out of 12 slots. **[inference]** Most slots stay locked behind map nodes the owner has not reached (or chosen).
14. **[screens]** **Resource scale.** Bread reads about 255K to 268K while archers are capped (25/25), so bread is abundant and XP is the gate at this stage; the XP counter moves in the 13,000 to 15,000 range.

### Visual and tone notes [screens]
- Enemies come in two colours: **brown** peasants/raiders and **blue-armoured** footmen, plus horsemen after the king's letter. The boss wave is a uniform grid of the blue knights.
- Gold **damage numbers** (48, 75) and **XP/bread pickups** (+6 XP, +90 bread) pop up where enemies die.
- The monument beam is a thin white line with a fading particle trail behind the cursor.

### Questions raised by batch 2
1. What does the red **"7"** box mean (file 26)?
2. When the boss bar disappears and the giant knight says "I'll be back" (file 29), did you win, or did the wave end by itself? What did it pay (trophy, XP, an unlock)?
3. If you do not empty the bar, what happens: does it refill, is there a time limit, can you press BOSS again at once, and do you lose anything?
4. Does the boss pool grow each time (120K here), or does each boss have a fixed size?
5. Do node costs rise with each purchase (440, 550, 700), or with time or enemy count?
6. Which node did the cannon block (the red line to the other 2,000 XP node), and does a Restart let you pick the other?
7. How many of the 12 slots can a full run unlock?

## Hands-on evidence, batch 3: after one prestige, the second island (owner screenshots, 2026-10-21)

Owner's words, verbatim:

> OK, i've presitged once and gotten a bit deeper into it. Here's some more screenshots. What questions do you have

16 more screenshots (files 34-49). Sent out of order; the order below is reconstructed from XP, trophy count and which nodes are bought (best effort, with the Circle of Friends pair uncertain). The run before the prestige is "run A", the one after is "run B".

### Run A, finishing up (files 34-37)
1. **[screens]** **The cannon blocks the crossbowman.** File 34: "Exempt crossbowmen from taxes", a defender with "White damage, high crits, knockback effect", is shown **Locked**, red-outlined with an X, beside the cannon branch the owner bought. This answers my earlier question: the cannon and the crossbowman are the exclusive pair, and the cannon branch (nodes at 550 to 1,100 XP) is the one taken in this run.
2. **[screens]** **Boss challenges.** File 35: a banner reads **"Complete boss challenge level 5 to earn [trophy]"** and a small **"lvl 1"** button sits in the bottom-right corner where the BOSS button was. **[inference]** After the first boss, the corner button becomes a **repeatable boss challenge with levels**, and level 5 pays a trophy. Not confirmed.
3. **[screens]** The lava counter shows **0** during that challenge (2,869 All: archers 1,861, cannon 960, monument 47).
4. **[screens]** File 36: the "picnic in the forest" node (previously "Beat a boss to unlock this") now reads **Bought**, and **Friends shows 3 trophies**. The next nodes are a green defender and a red-haired one at 4,000 XP each, joined by a **red line with a warning triangle** (another exclusive pair), and a 7,500 XP node with a trophy marker. XP is 776 here, so buying the unlock took most of the stockpile.
5. **[screens]** File 37: a **third resource, wood**, is now in the top bar (1,850) with a third row in the bottom box. Grey-helmeted **Viking-style raiders** have joined the field. The slot column is unchanged: archers 50/50, cannon 10/10, windmill 29/29.

### The Circle of Friends (files 38-39)
6. **[screens]** File 38: the wheel now shows **14 friends** with trophy costs of 1 to 3 each; the owner has chosen **three** (Peasant, the imp and the elf, shown with bright trophy icons), the trophy counter reads **0**, and the buttons are **Cancel** and **Restart!**. The Peasant tooltip says **"Already your friend"**.
7. **[screens]** File 39: the confirmation dialog **"Do you really want to restart the game with the selected friends?"** with a **YES** button. The trophy counter here reads **3**, so either this was taken before the choices were made or the dialog shows the pre-choice count. Order of 38 and 39 **uncertain**.

### Run B, after the prestige (files 40-49)
8. **[screens]** **Scale.** File 40: **XP 579K, bread 13.3M, wood 15,844**, against about 15K XP and 268K bread at the end of the earlier batch. Top-bar numbers are styled in gold italics here.
9. **[screens]** **Slots in use: six of twelve.** Archers 60/60, cannon 14/14, a green **dryad** defender 15/20 (15,000 to buy), a windmill 40/42 (34,000 bread), a **lumber mill 11/11**, plus the obelisk. Six slots are still dead trees.
10. **[screens]** **The monument barely kills anything now.** File 40's counters read 5,591 All: archers 3,214, cannon 1,544, dryads 319, **monument 3**, lava 510. File 37 had the monument at 178 of 4,661; file 35 (boss challenge) 47 of 2,869. Compare the first boss wave, where it took 405 and 533 kills. **[inference]** The aimed beam stops mattering for kills as the tower fills up, except early and in boss waves. The counters are rolling windows (owner), and the cursor in file 40 is over empty grass, so this is indicative.
11. **[screens]** **A second island.** File 42: two islands, brown (the first) and green (a forest), joined by a single line. Most of the brown island is checked; the green has about a dozen checks.
12. **[screens]** **Damage types as the new puzzle.** The treant node ("Gather firewood for a barbecue", file 43) says **"New enemy to come. Resistant to white damage, vulnerable to red damage"**. Earlier new enemies were "resistant to white damage" too. **[inference]** White damage (archers, cannon) is blunted by later enemies, which pushes the player toward the red-damage defender.
13. **[screens]** **Wood chain.** "Logging: Come, see, chop the tree!" (file 44) opens a branch of mill nodes (Productivity, Automation icon, an hourglass node, upgrades at 750 to 2,200 XP). "Productivity" (file 45) now reads **"Production size: +20 >> +30%", 2/10, 750 XP**; earlier the same node gave +10%.
14. **[screens]** **New defender: the dryad.** "Appease the forest with gifts" (file 46): "New defender of your majesty. White damage, fast-shooting, pierces one enemy after another." Follow-up nodes: **"Dryad Huntress Lethality: Damage +60 >> +80%, 3/15, 1,300 XP"** (file 47) and **"Dryad Huntress Reinforcements: Buy limit +15 >> +20, 3/9, 1,100 XP"** (file 48). So each defender has its own damage, cap and mobilization lines.
15. **[screens]** **More enemies on demand.** "Treant Mobilization: Enemies to spawn +1 >> +2, 1/10, 2,500 XP" (file 49); "Dryad Mobilization" (a centaur, file 41): "Enemies to spawn +1, 0/10, 3,700 XP". The same lever as the footman and rogue nodes, now per enemy type.
16. **[screens]** **Trophy counts on the map:** 2 (file 41), then 1 (files 42-49). **[inference]** The map's "Friends" badge shows free (unspent) trophies, so it dropped when a friend was enabled in between.

### Questions raised by batch 3
1. What did you pick in the Circle of Friends, in what order did you press things (files 38 and 39), and does the prestige keep the map or anything else besides the friends?
2. What is a boss challenge: does the BOSS button turn into "lvl 1" after the first win, and does each level need a bigger kill rate? What did the first boss pay?
3. How long did run A take from the start to the prestige, and how long did run B take to reach 579K XP? (Real minutes or hours, not game time.)
4. In file 40 the monument killed 3 of 5,591. Were you still steering it? Does the beam matter at all this late, or only in bosses?
5. What is wood spent on: the dryad (15,000), the mills, anything else? And what are the gold-italic numbers in the top bar (a boost, a rate, or a flash)?
6. How big is the jump in income from the friends, as you feel it (run B reached 579K; run A was at 15K when the earlier batch ended)?

## Owner's answers to batches 2 and 3 (2026-10-21)

Verbatim, answering my six questions after batch 3:

> 1. You earn trophies. Then to actually "apply" the trophies, you have to do so in the context of a prestige reset. I picked peasant, imp, herbalist.
> 2. It looks like after you beat the initial boss challenge, it lets you earn additional trophies by beating harder boss challenges.
> 3. I left the game running overnight, but it looks like I could have prestiged MUCH sooner. like in less than an hour.
> 4. The beam doesn't seem to matter at all, except chasing down stragglers.  This was one of my concerns on the mechanic - how do you keep the beam mechanically interesting - otherwise it's a novelty for the first 5m and then you just don't care.
> 5. It looks like as you unlock the 2nd island, the wood currency unlocks which is used to buy units - like the bread did on the first island.
> 6. Most of that jump was leaving it running overnight.

What this settles:
- **Trophies are earned in a run and spent at the prestige.** Friends chosen: **Peasant, Imp, Herbalist** (3 trophies). So the 0-trophy screen (file 38) is the choice made before pressing Restart, and the 3-trophy dialog (file 39) is the confirmation of it; the counter in the dialog is the pre-spend figure. Order of 38 and 39 is still uncertain.
- **Boss challenges are a trophy ladder:** beat the first boss, then harder challenges pay further trophies (owner). Matches the "level 5 earns a trophy" banner.
- **Prestige is available in under an hour** (owner's estimate), well before the point he actually reached it. The early prestige hook is reachable inside one sitting.
- **The jump to 579K XP was mostly overnight idling** (owner), not the friends' bonuses. The game runs while open; whether it pays anything while closed is still not known.
- **The aimed beam stops mattering except for stragglers** (owner). The owner flags this as his own concern for ClickOne: "how do you keep the beam mechanically interesting - otherwise it's a novelty for the first 5m and then you just don't care." This matches the kill counters (monument 3 of 5,591 late). **This is the central design problem the research should now solve** for the aiming input: see the follow-ups in [README.md](README.md).
- **Wood is the island-2 unit currency**, playing the role bread plays on island 1 (owner). Each island introduces its own currency, damage-type problem and unit.

## Hands-on evidence, batch 4: second boss, second laser, and the owner's verdict on the upgrades (2026-10-21)

Owner's words, verbatim:

> so it's not intuitive what I'm buying and how it really improves things / changes things. Maybe it's because I let it run overnight, but I don't think so. There are some areas - the ones that do unlocks - that feel meaningful, but most do not. It's just number go up and the different items to bring in more bad guys don't seem like it's important or impactful

> The next 2nd boss has 3 new unit types. One is super slow rock golems. There are some much faster units. So again - it's a DPS check, but it's a DPS check based on a combination of unit speeds, firing rates, damage types / resistances. It feels like this could be done in a very interesting way. You don't lose, but it points out where you need to improve things.

> Also, I have unlocked a 2nd laser - a green one. They're linked to the mouse cursor the same, but supposedly do different damage.

7 more screenshots (files 50-56), sent out of order and reconstructed from XP and slot counts. Run B continues; XP is in the 540K to 740K range.

### The owner's verdict on the upgrade tree (central finding)
1. **[owner]** It is **not intuitive what you are buying or how it changes things**. The owner does not think the overnight idling is the cause. The nodes that **unlock** something (new units, enemies, monuments) "feel meaningful"; **most do not**: "It's just number go up", and the **"bring in more bad guys" nodes feel unimportant**.
2. **[inference]** This is the same weakness as the unintuitive counters and the faded beam: **the early game communicates well; the later tree does not.** The map reads as a wall of tiny percentage nodes (damage +60 to +80%, buy limit +15 to +20, production +20 to +30%, costs 750 to 25,000). The lesson for ClickOne is to keep upgrades **few, large and visibly different**, or show their effect.

### The second boss (files 53, 56)
3. **[screens]** File 53: the bar reads **"245K / 264K"** (the first boss was 120K). The field holds **grey-green rock golems** (large, slow), **white fast units (horses)**, and small orange fast units darting in the middle; the beam is aimed at the golems. **[owner]** "3 new unit types": slow golems, much faster units. The kill counters during it: 3,573 All (archers 1,721, cannon 1,279, dryads 203, monuments 92, lava 276).
4. **[owner]** It is "a DPS check based on a combination of unit speeds, firing rates, damage types / resistances". "You don't lose, but it points out where you need to improve things." The owner thinks this **could be done in a very interesting way**. **[inference]** This is the strongest design idea in the game for a no-lose structure: **a boss as a diagnostic, not a threat**. The wave exposes which defender or damage type is lacking, and the tree is where you fix it.
5. **[screens]** File 56: **boss challenge level 3** of 5, with the same banner "Complete boss challenge level 5 to earn [trophy]". Both beams are pointed at the "Lvl 3" button in the bottom-right corner, where the cursor sits.

### The second laser and automation
6. **[owner]** A **second, green laser** is unlocked; "linked to the mouse cursor the same, but supposedly do different damage". **[screens]** File 66 (map): **"New monument. Unlocks a new monument. It has green damage and knockback effect"**, 15,000 XP. In the main view (files 54 and 56) a white beam and a green beam both run from the monument area to the cursor, with a second obelisk (green) in the slot column. File 52 shows the green monument's slot as **"Buy 30000"** (wood) before purchase.
7. **[screens]** **Monuments can be automated.** File 67: "All monuments automation: **Will work without you to watch**", **25,000 XP**. **[inference]** This turns the aimed input off: the monuments aim themselves. It is the game's own **opt-out for aiming**, paid for in XP late in the run, and the tree sells it as a convenience. Compare the owner's concept (opt out of aiming at a cost to early efficiency).
8. **[screens]** The **gold-italic and pink numbers** in the top bar change colour between shots (XP, bread and wood). Pink wood (698K, 751K) appears next to a purchasable wood slot, so pink may mean "capped" or "not affordable for that purchase". Unknown.

### Economy and slots at this stage [screens]
- Counters now have six rows (All, archers, cannon, dryads, monuments, lava). In file 54: 8,918 All: archers 3,247, cannon 2,304, dryads 582, monuments 537, lava 2,248. The two monuments together kill more than before (537 of 8,918) but still about 6%.
- Defender caps have grown (archers 60/65, then 69/80; cannon 14/16, then 16/18; dryads 25/25; windmill 45/45, then 53/60), and **buy costs have grown with them** (archers 730K bread, then 2.80M; cannon 1.10M, then 2.30M). Bread has reached 16.8M. **[inference]** Each cap-raising purchase pushes the cost up by 2 to 4 times, so spending bread is a long climb.
- Island 1's map (file 55) is nearly all checked at about 568K XP, with the crossbowman still **Locked** by the cannon choice.

### Questions raised by batch 4
1. When you say most upgrades "do not feel impactful": was there any point where you bought something and **saw** a difference on the field? Which nodes were the "unlocks that feel meaningful"?
2. Does the game ever **tell you what the boss showed up lacking** (for example "your damage type is resisted")? How did you know where to improve after the second boss?
3. How did you do on the second boss: did you drain the bar, and how long did it take? What did it pay?
4. How different do the two lasers feel? Does steering them separately matter, or do they always sit at the same point?
5. Have you bought **All monuments automation** (25,000 XP)? If so, does the game feel better or worse without the beam?
6. Is there a third island, a final boss or an ending, and roughly how many real hours in are you?

## Owner's answers to batch 4, and the "loot drop" tooltip (2026-10-21)

Verbatim:

> None of them feel super meaningful - if I'm honest.
>
> The game gives VERY little feedback. Here's a screenshot - what does this do for me?
>
> What is "loot drop" in this game's context? I don't see where it was explained.
>
> Second boss i wasn't close - barely reduced the bar by one half
>
> lasers - they always point to the same place. I think the green one is doing a bit of a slow / knockback.
>
> I bought the automation and it's super underwhelming. It moves slowing in an arc and rarely hites things.
>
> There's a 3rd island sitting behind the next boss fight. I'm maybe 3 hours in.

What this settles:
- **Upgrades:** none of the later nodes feel meaningful (owner). The game gives **"VERY little feedback"** about what a purchase did.
- **Second boss:** not close. I read "barely reduced the bar by one half" as the bar going down by about half. So a no-lose boss can be failed by a wide margin, and (per batch 4) the game does not say why.
- **Lasers:** both always point at the same place. The green one seems to slow or knock back (owner's guess, not confirmed by the game).
- **Automation (25,000 XP):** **"super underwhelming"**. It sweeps "slowly in an arc and rarely hits things". So the game's own opt-out for aiming is a weak, slow auto-aim that does not replace the player. A caution for ClickOne: the aim opt-out must do **a reasonable job**, or it is not an opt-out, just a nerf.
- **Length:** a third island sits behind the next boss; the owner is about **3 hours in** (including the overnight idle). The dev's stated length was 5 to 6 hours, 12 for all achievements.

### The "Footman: Upgraded equipment" node (file 57)
**[screens]** Tooltip: "Footman, Upgraded equipment. **Loot drop:** +360 >> +380% (green). **Health:** +90 >> +95% (red). 18/20, 6,100 XP." The map has two island-1 nodes of this kind (footman, horseman) with prices of 1,000 to 7,500 XP.

What I can say about it:
1. **[screens]** The **footman is an enemy** (the blue-armoured foot soldier on the field; a node named "Footman Mobilization" earlier read "Enemies to spawn: +1" in red). So this node buffs the **enemy**, not a defender.
2. **[screens]** The colours are the key: **green means good for you, red means a cost to you**. The same convention appears in the "Enemies to spawn: +1 >> +2" lines, which are red. So this node trades **+95% enemy health (red, slower kills)** for **+380% loot (green)**, nudging both up each level (+5 health points, +20 loot points).
3. **[inference]** "Loot drop" is **how much XP and bread (and wood on island 2) a footman pays when it dies**: the Zeropedia stat table earlier showed per-enemy "XP" and "bread" values (the knight: XP 15, bread 200). The node raises that figure by a percentage on top of the base. **Not confirmed, and the game never says it.** A way to check: open the **Zeropedia**, click the footman, and compare **Base** and **Now** for XP and bread.
4. **[inference]** So the node is a **risk and reward dial on the enemy**: richer, tougher footmen. Each level changes little, which fits the owner's "number go up" verdict.

Why it reads badly [inference]:
- The name says "Upgraded equipment" with the unit's face, so a player assumes it improves **their** unit.
- It shows only percentage points with no **before/after in real units** (for example "footman pays 40 XP, was 10").
- The term "loot drop" is never defined.
- The **result is invisible** on the field.

### Questions raised by this batch
1. Check the **Zeropedia** for the footman: what are Base and Now for XP and bread, and does "Now" move when you buy a loot node? That confirms what "loot drop" is.
2. Does anything on screen (floating numbers, a flash) ever show **XP per kill rising** after you buy such a node, or only the totals?
3. For the boss you fell short of: did you try to **fix** it (buy different defenders or damage types), and did you pass it afterwards?
4. What did you do during the overnight run besides watch: nothing to buy, or spend and walk away?

## Zeropedia numbers and the owner's verdict on impact (2026-10-21)

Owner's words, verbatim:

> 1. here's a bunch of zeropedia info.
> 2. YOu only see the totals - there's no sense of connection. The enemy units are an ever flowing river coming in and the lava river that protects you mean you never lose. Its hard to really feel your improvements in this mode.
> 3. I bought a bunch of upgrades, but i have like 50+ upgrades I can do right now - and basically think "who cares" about those upgrades. There's nothing that I go "oh, I am short on single target damage - buy this" or "oh, my archers aren't firing fast enough"
> 4. I spent all I could, but I was fairly early after my prestige reset.
>
> the art design is cute. The concept of low stress is neat. I just don't get how to make the game feel more impactful.

(8 files were sent; file 75 shows the same footman tooltip as file 57 (not byte-identical, so possibly a re-capture) and was not saved again.) 7 new screenshots saved as files 58-64.

### What the Zeropedia shows (Base vs Now) [screens]
| Entry | Stats (Base to Now) |
|---|---|
| **Archer** (file 63) | damage 40 to **160**; interval 1.50 s to **1.00 s**; min range 400 to 350; max range 850 to 900; crit chance 3.0% to **22.0%**; crit multiplier x3 to **x6** |
| **Dwarven Cannon** (file 64) | damage 100 to **320**; interval 5.20 s to 4.80 s; range 550 to 500 (min) and 900 to 950 (max); crit chance 2.0% to 6.0%; crit multiplier x2 to x4.10; pierce 2 to **9** |
| **Green monument** (file 61) | damage 30 to **135** (shown green); interval 0.2 s; range 15 to 29; crit chance 1% to 1.20%; crit multiplier x3 to x3.10; plus a projectile-speed row 15 to 15 and a knockback row "-" |
| **Windmill** (file 58) | bread per cycle 40 to **92**; cycle 3.00 s to 1.40 s; crit chance 2.0% to 4.5%; crit multiplier x3 to x4 |
| **Lumber mill** (file 60) | wood per cycle 40 to 64; cycle 3.00 s to 2.50 s; crit chance 2.0% to 3.0%; crit multiplier x3 to x3.40 |
| **Rogue (enemy)** (file 59) | XP 5 to **10**; bread 50 to **250**; health 100 to **250**; speed 16 to 16; four armour rows "-"; weight 100 to 100 |

What these confirm and add:
1. **"Loot drop" is confirmed (file 59):** it is the **XP and bread an enemy pays when it dies**. The rogue's payout went from 5 XP and 50 bread to 10 XP and 250 bread while its health rose from 100 to 250. So the "mobilization" and "equipment" enemy nodes make enemies **richer and tougher**, exactly as inferred. XP rose 2x and bread 5x, so the loot nodes do not scale the two equally; the game does not say why.
2. **Production buildings crit too.** The windmill and lumber mill carry crit chance and crit multiplier like defenders. Every unit in the game shares one stat sheet (damage or yield, interval, range, crit, crit multiplier, and for some pierce or knockback).
3. **Per-hit numbers moved a lot.** Archer damage x4 and fire rate x1.5 is at least **x6 damage per second before crits**, and the crit chance went from 3% to 22%. The cannon is x3.2 damage and pierces 9 instead of 2. The player's side got much stronger. Yet the field **does not look different**, which is the owner's complaint. **[inference]** The player also raises the enemy side (more spawns, +95% health) on purpose to earn more, so the improvements are offset by self-imposed difficulty, and the flow never visibly breaks or changes.
4. **The only place the numbers appear is a separate screen** (Zeropedia), as a table of Base and Now with no explanation and no link to the purchase that changed them.

### Late-run snapshot (file 62) [screens]
XP 574K, bread 307K, wood 489K. Slots: archers 71/80 (3.70M bread, red = not affordable), cannon 16/20 (2.30M), dryad 30/30, windmill 54/60 (270K), lumber mill 17/17, plus the white and green monuments. Counters: **16,122 All: archers 4,920, cannon 4,224, dryads 1,481, monuments 2,592 (about 16%), lava 2,903**. The monuments' share is much higher than the 3 of 5,591 seen earlier, with two beams and 135 damage on the green one (rolling window; indicative). Purple projectiles are flying toward the lane (a ranged enemy). The field is a dense stream of enemies.

### What the owner's verdict adds
- **Impact is the problem, not the look or the premise.** "The art design is cute. The concept of low stress is neat." The owner cannot see how to make the game "feel more impactful".
- **No-lose and impact pull against each other** in this design: with a lava moat and an endless stream, "you never lose", so nothing you buy visibly saves or fails you. The owner says it is "hard to really feel your improvements in this mode."
- **There is no felt need.** With 50+ upgrades available, nothing prompts "I am short on single-target damage" or "my archers are not firing fast enough". The game never names a weakness, and so no purchase reads as a fix. **This joins the earlier finding**: the **second boss as a diagnostic** (speed, fire rate, damage type) is where such a need *could* appear, but the game does not report what the boss showed.
- **Spend-all happened early after the prestige** (owner), so the overnight run was not hiding a long list of useful buys; the list is long and unremarkable.

## Owner's note on unit counts (2026-10-21)

> also, there's too many units. It's a cacophany and whether I have 63 archers or 78 archers - who cares. It's just more noise on screen. I'd reduce the entire quanityt of units by an order of magnitude.

Evidence in the screenshots **[screens]**: defender caps run to 60 to 80 archers, 14 to 20 cannons, 25 to 30 dryads, plus 50 or more farm buildings, all drawn in the lane at once; enemies number in the hundreds on screen (the kill counters read 5,591 to 16,122 per window). Each cap-raising purchase adds a handful of identical units (archers 60/65, then 69/80, then 71/80), which is the "63 or 78, who cares" problem: the step is too small to see or hear.

**Implication for ClickOne** **[inference]:** a roughly ten-times smaller roster (single digits to a few dozen) would let each unit and each purchase be visible and legible, and would also make "I am short on X" readable. This pairs with the earlier findings that the late upgrades are "number go up" and that the game gives very little feedback. Fewer, individually distinct units is a candidate design rule to test.

## Owner's impressions

Owner's words are quoted verbatim in the "Hands-on evidence" sections and "Owner's answers" above. Summary so far (2026-10-21): the opening takes under 5 minutes and works; the unlock nodes and boss waves feel meaningful; the beam is a novelty after about 5 minutes; the counters are unintuitive; and most upgrades are "number go up", with the extra-enemy nodes not feeling impactful. The owner sees the boss-as-DPS-check (speed, fire rate, damage type) as the strongest idea. Final verdict so far: "The art design is cute. The concept of low stress is neat. I just don't get how to make the game feel more impactful." Earlier, 2026-10-21: "It seems like it did pretty well."

## Screenshot index

64 owner screenshots (17 early-game, 16 boss and map, 16 after one prestige, 8 second boss, lasers and upgrades, 7 Zeropedia and late run), saved as WebP in `img/zero-stress-king/`, **numbered in my reconstructed play order** (sent out of order). Spot-checked: 22.png and 17.webp matched their names.

| # | file | what it shows |
|---|---|---|
| 1 | `01-circle-of-friends-total-reset-confirm` | Circle of Friends with the erase-progress confirmation, 0 trophies |
| 2 | `02-main-screen-after-reset-no-swiping-today` | clean main screen, all slots locked, King says "No swiping today, okay?", counters 0 |
| 3 | `03-main-first-kills-monument-beam-xp60-bread600` | XP 60, bread 600, monument beam to the cursor, counters 245 / 98 / 147, per-kill box 2 and 25 |
| 4 | `04-skill-map-start-xp230` | first skill map, 230 XP, nodes 20, 75, 80, 320, 1000 |
| 5 | `05-skill-map-zoomed-out-island-overview` | zoomed-out island with five nodes and red dots |
| 6 | `06-circle-of-friends-peasant-tooltip` | Peasant tooltip: automated production, grants 10 farms, 1 trophy |
| 7 | `07-circle-of-friends-king-tooltip` | King tooltip: "You do nothing" |
| 8 | `08-skill-map-tooltip-farming-80xp` | Farming node, 80 XP |
| 9 | `09-skill-map-tooltip-archers-20xp` | archers node, 20 XP |
| 10 | `10-skill-map-tooltip-monument-damage-320xp` | all monuments damage +25%, 320 XP |
| 11 | `11-skill-map-archers-bought-xp225` | archers Bought, more archer upgrade nodes appear (50, 75, 150) |
| 12 | `12-main-archers-3-of-10-xp25-bread2110` | first archer slot "3/10, 150 bread, Hold Shift - Max", counters 198 / 8 / 0 / 190 |
| 13 | `13-main-archers-10-of-10-xp85` | archers 10/10, counters 257 / 78 / 41 / 138, several archers firing |
| 14 | `14-skill-map-productivity-tooltip-xp195` | farm upgrades, Productivity +10% at 24 XP |
| 15 | `15-main-archers-10-of-15-farms-2-of-8-xp196` | archers 10/15 (440 bread), farm slot 2/8 (500), bread 3,250 |
| 16 | `16-zeropedia-knight-stats` | Zeropedia grid of doors, Knight stat table (Base and Now) |
| 17 | `17-zeropedia-monument-stats` | Zeropedia, monument stat table (30 damage, 0.2 s, range 15, crit 2% x7) |
| 18 | `18-skill-map-run2-rogue-mobilization-xp2266` | Rogue Mobilization (+1 to +2 enemies), 100 XP; 1 trophy |
| 19 | `19-skill-map-monuments-area-size-xp3251` | All monuments area size +2 radius, 0/5, 400 XP |
| 20 | `20-skill-map-footman-mobilization-xp2976` | Footman Mobilization, +1 enemy, 0/10, 240 XP |
| 21 | `21-skill-map-ask-neighboring-king-2500xp` | enemy-unlock node with trophy marker, 2,500 XP |
| 22 | `22-skill-map-zoomed-out-picnic-needs-boss-xp606` | zoomed-out map, node outside island needs a boss to unlock |
| 23 | `23-skill-map-neighboring-king-bought-xp681` | neighbouring-king node Bought, 2 trophies, cannon branch red-linked |
| 24 | `24-main-friendly-letter-cavalry-xp751` | main screen, king's letter bubble, horsemen on the field, BOSS button |
| 25 | `25-main-boss-button-xp14826` | main screen late run, BOSS button, counters 447 / 45 / 1,554 |
| 26 | `26-main-red-7-box-xp14841` | main screen with an unexplained red "7" box |
| 27 | `27-boss-wave-bar-full-120k` | boss wave: bar 120K / 120K, knight blocks at the right edge |
| 28 | `28-boss-wave-bar-107k-troops-at-lava` | bar 107K / 120K, troops massed at the lava bank |
| 29 | `29-boss-ends-ill-be-back` | bar gone, giant knight says "I'll be back" |
| 30 | `30-skill-map-order-cannon-blocks-another-xp15381` | Order cannon deliveries, "This skill will block another!" |
| 31 | `31-skill-map-costs-risen-xp13951` | same map after purchases, costs higher |
| 32 | `32-skill-map-automation-400xp` | Automation node tooltip, 400 XP |
| 33 | `33-main-cannon-slot-2-of-2-xp13136` | cannon slot 2/2, archers 30/30, counters 576 / 37 / 188 / 1,230 |
| 34 | `34-skill-map-crossbowman-blocked-by-cannon-xp5716` | crossbowman node Locked (blocked by the cannon), cannon branch costs |
| 35 | `35-main-boss-challenge-lvl1-hover-xp3821` | "Complete boss challenge level 5 to earn" banner, "lvl 1" button |
| 36 | `36-skill-map-picnic-bought-3-trophies-xp776` | picnic unlock Bought, 3 trophies, exclusive 4,000 XP pair |
| 37 | `37-main-wood-and-vikings-appear-xp1558` | wood resource appears, Viking raiders on the field |
| 38 | `38-circle-of-friends-friends-chosen-0-trophies` | 14 friends, three chosen, 0 trophies, Cancel / Restart! |
| 39 | `39-circle-of-friends-restart-confirm-3-trophies` | restart-with-selected-friends confirmation, 3 trophies |
| 40 | `40-main-after-prestige-579k-xp-wood-dryads` | run B: 579K XP, 13.3M bread, six slots, monument 3 of 5,591 kills |
| 41 | `41-skill-map-dryad-mobilization-xp591k` | Dryad Mobilization (centaur), +1 enemy, 3,700 XP |
| 42 | `42-skill-map-two-islands-overview-xp597k` | brown and green islands overview |
| 43 | `43-skill-map-treant-barbecue-bought-xp599k` | Treant enemy: resistant to white, vulnerable to red |
| 44 | `44-skill-map-logging-bought-xp599k` | Logging node Bought, mill branch |
| 45 | `45-skill-map-productivity-30pct-xp600k` | Productivity +20 to +30%, 750 XP |
| 46 | `46-skill-map-dryad-appease-forest-bought-xp601k` | dryad defender Bought, upgrade branch |
| 47 | `47-skill-map-dryad-huntress-lethality-xp602k` | Dryad Huntress Lethality +60 to +80%, 1,300 XP |
| 48 | `48-skill-map-dryad-huntress-reinforcements-xp603k` | Dryad Huntress Reinforcements, buy limit +15 to +20 |
| 49 | `49-skill-map-treant-mobilization-xp604k` | Treant Mobilization, +1 to +2 enemies, 2,500 XP |
| 50 | `50-skill-map-new-green-monument-15000xp-xp741k` | New monument node: green damage and knockback, 15,000 XP |
| 51 | `51-skill-map-all-monuments-automation-25000xp-xp732k` | All monuments automation, "will work without you to watch", 25,000 XP |
| 52 | `52-main-green-monument-slot-30000-wood-xp707k` | green monument slot "Buy 30000", counters 7,769 All |
| 53 | `53-boss2-golems-and-fast-units-bar-245k-of-264k` | second boss: golems, white fast units, bar 245K / 264K |
| 54 | `54-main-two-lasers-white-and-green-xp676k` | white and green beams, two monuments, 8,918 All |
| 55 | `55-skill-map-crossbowman-locked-island1-xp568k` | island 1 map nearly all bought, crossbowman Locked |
| 56 | `56-main-boss-challenge-lvl3-two-lasers-xp542k` | boss challenge level 3 banner, both beams on the button |
| 57 | `57-skill-map-footman-upgraded-equipment-loot-drop-xp539k` | Footman Upgraded equipment: loot drop +360 to +380% (green), health +90 to +95% (red), 6,100 XP |
| 58 | `58-zeropedia-windmill-bread-40-to-92` | Zeropedia windmill: bread 40 to 92, cycle 3.00 to 1.40 s, crit 2.0 to 4.5% |
| 59 | `59-zeropedia-rogue-enemy-xp5-10-bread50-250-health100-250` | Zeropedia rogue enemy: XP 5 to 10, bread 50 to 250, health 100 to 250 |
| 60 | `60-zeropedia-lumber-mill-wood-40-to-64` | Zeropedia lumber mill: wood 40 to 64, cycle 3.00 to 2.50 s |
| 61 | `61-zeropedia-green-monument-damage-30-to-135` | Zeropedia green monument: damage 30 to 135, range 15 to 29 |
| 62 | `62-main-two-monuments-16122-all-xp574k` | main screen late: 16,122 All, monuments 2,592, dense enemy stream |
| 63 | `63-zeropedia-archer-damage-40-to-160` | Zeropedia archer: damage 40 to 160, interval 1.50 to 1.00 s, crit 3 to 22% |
| 64 | `64-zeropedia-dwarven-cannon-damage-100-to-320` | Zeropedia cannon: damage 100 to 320, pierce 2 to 9 |
