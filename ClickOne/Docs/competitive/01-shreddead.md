# ShredDead

*Researched 2026-10-20 (updated the same day after finding a playable build). Read: the Steam store page, `appdetails` record, demo record,
user tags, news feed (one post), review summary (none exist), five store screenshots and the capsule art, all 15 Steam community
threads (full text, including one developer reply), the itch.io page and its 16 comments, and an Incremental DB entry. **Played** by Claude (not
the owner): about 10 minutes of the free **web build "Demo 0.1.3" on itch.io** (Unity WebGL, in the in-app browser, in Portuguese by accident,
under heavy input lag), which reached the first skill-tree screen and earned 5 currency but never reached the first purchase. The
**Steam demo (App ID 4900640) was not played** and may be newer than 0.1.3. The trailer was not watched. Every mechanic below is tagged
**[played]** (I saw it), **[player report]** (a demo player said it), **[dev claim]** (the store text) or **[inference]**. Store screenshots
and my own play screenshots are not saved in the repo.*

- **Steam:** https://store.steampowered.com/app/4704520/ShredDead/ (App ID 4704520) | **Demo:** App ID 4900640 | **Web demo:**
  https://fimor.itch.io/shreddead (v0.1.3, free, HTML5, 86 MB, rated 4.2/5 from 24 ratings on 2026-10-20)
- **Release:** Oct 26, 2026 (Steam). Kotaku's listing says Oct 25; the Steam page is the source of record. Demo record dated Sep 1, 2026.
  The page is part of the Steam Next Fest sale event, which the page banner says runs through Mon Oct 26 at 10 AM PT. **The developer
  said demo progress is wiped at launch "due to balance changes"** (Steam thread, handle `weirdpenguingames`).
- **Price:** not listed yet (2026-10-20). A demo player suggested "a few dollars" for a few hours of play (opinion, not a fact).
- **Reviews (2026-10-20):** none, total 0. 2 curators have reviewed it (not read).
- **Developer/publisher:** Weird Penguin Games (self-published). Same name as the developer of *Empires Shall Fall* (below). Itch handle
  `fimor`. Team size unknown. Support: (support email on the store page). Discord and YouTube linked on the page.
- **Engine:** Unity (itch page and Incremental DB; the web build's loader is Unity's).
- **Platform:** Windows and macOS, single-player, full controller support (Xbox, DualShock, DualSense), Steam Achievements, Family Sharing.
  Steam Deck "Unknown". 14 languages (EN, FR, IT, DE, ES-ES, JA, KO, PL, PT-BR, RU, ZH-CN, ZH-TW, TH, TR). The demo record adds Mouse Only
  Option, Playable without Timed Input, Playable at your own pace, Custom Volume Controls, Save Anytime.
- **Minimum spec (Windows):** Windows 7+, Core 2 Duo 2.4 GHz, 4 GB RAM, DirectX 11, 1 GB disk. No recommended spec; no Mac spec.
- **Age ratings on the page:** Brazil 10, Germany 12 (violence), Indonesia 13 (violence).
- **Store tags (2026-10-20):** Incremental, Casual, Strategy, Auto Battler, Idler, Relaxing, Zombies, Post-apocalyptic, Indie, Isometric,
  Tower Defense, Pixel Graphics, Inventory Management, Resource Management, PvE, Singleplayer, Arcade, Replay Value, Short. "Steam is
  learning about this game", so these are probably developer-seeded.
- **Generative-AI disclosure (sources disagree):** the Steam page says nothing. The itch page says no generative AI was used. Incremental DB
  says "some code was written with AI tools; all design and development decisions were the developer's". Treat the AI question as open
  and do not repeat any of the three as fact.

## Premise and scale

Comic gore, not horror. **[dev claim]** The apocalypse has happened, the player is "too good at this asymmetric warfare thing", so the "last
stand is taking forever" and the game "is not about survival, it is about FUN". One demo player says the pitch echoes the flash game
*The Last Stand*. Session model: endless waves, **no lose condition** (developer claim, and **[played]**: nothing in the build threatens the
player). Length is unknown. One demo player reported about 30 minutes of active play before it turned into watching a second screen and
checking back for points to spend; another found it "too grindy", especially upgrades that run to level 20.

## How it plays: the core loop

Sources: **[played]** = the v0.1.3 web build I ran. **[player report]** = Steam threads and itch comments from the later Steam demo or the web
build. The Steam demo is probably newer, so the two can differ.

### Map and flow
1. **[played]** One fixed map: a city street in isometric view, a railway line running diagonally, a rooftop at lower left, a parking lot
   at upper right. The camera does not move or zoom in the build I ran. (A player asks for a bigger stage with zoom; so it is fixed in the demo.)
2. **[played]** Zombies spawn off-screen at the **upper right** and walk toward the **railway line at lower left**.
3. **[played; owner confirmed 2026-10-21]** A zombie that reaches the tracks dies in a red splatter. **No currency was paid for those deaths** (the counter stayed at 5 through
   several of them). **[inference]** The railway is "the shredder" (**owner confirmed**): the store line "any zombie that gets through is shredded" matches, and an
   itch comment says off-screen zombies "disappear in the moat". Not confirmed by a label.
4. **[player report]** A "monitor window" for the shredder exists in the Steam demo. One player asks for zombies that pass the shredder after
   an "overload" to cross the barrier and kill defenders. **[inference]** The shredder has a capacity or load meter that currently has no
   consequence. Not seen by me.

### Offence
5. **[played]** The player's **main weapon is a machine gun** (skill-tree tooltip: "Your Weapon: Machine Gun"). Clicking sets a **red ring**
   on the ground; damage numbers (8, 10, 11 seen) then tick at the ring. **[inference]** The weapon fires automatically at the ring (the
   skill-tree base damage is 10, matching the numbers). **[player report]** "I do not need to click to shoot, it really saves my finger"; and
   "the main turret stops shooting" when the cursor leaves the game window, so the weapon follows the cursor's last position and the active
   input is **aim only**. Another player asks for auto-targeting of the main weapon "even as an expensive upgrade", so aim is manual in
   the web build.
6. **[played]** My manual kills paid **5 currency** for roughly 5 kills. Per-kill yield by zombie type is unknown. **[played]** Unit
   count: at the start there is **1 soldier** on the rooftop.
7. **[played]** A **Stats overlay** (button bottom-left) cycles two views: **Damage** (total, by source, with percentages; it read 24 total,
   100% from one source) and **Income** ("Rendimento", empty when I looked). Two itch users asked for exactly this (damage and income per unit
   type), so it is partly implemented.

### Spending: the upgrade tree
8. **[played]** The "Upgrade" screen is a pannable node graph with the currency counter overlaid. At the start there are **three nodes,
   each costing 80, each capped at 20 levels** (tooltip shows "(0/20)"):

   | Node (as labelled, translated from Portuguese) | Effect shown | Level 1 cost |
   |---|---|---|
   | Your Weapon: Machine Gun | Damage 10 → 14 | 80 |
   | Recruit | Number of Defenders 0 → 1 | 80 |
   | Zombie | Spawn rate 1 → 1.2 **and** Health 30 → 36 | 80 |

9. **The design idea worth stealing: the player buys harder enemies.** The "Zombie" node makes enemies spawn faster and tougher so that each
   wave pays more. **[player report]** This was the loudest design complaint on the Steam hub after "can't lose": one player will not buy
   it "if the only option is to also upgrade their health", and wants separate branches for more zombies, more frequent zombies, and tougher
   zombies "and rewards with it". Another reads it as a trade-off that pushes you to raise damage first. **[player report]** "You only want
   the upgrade if you can kill them fast enough."
10. **[store screenshots, shot 4]** The full tree is a large graph of octagonal nodes with unit silhouettes, corner badges (plus, chevrons,
    crosshair, padlock) and per-node costs coloured green or red (**[inference]** affordable or not). Visible costs run from 125 to 9.9K.
    Branches look colour-coded (blue, olive, red) per unit type or currency (**[inference]**).
11. **[player report]** Players say you "buy every upgrade that turns green", that you must put early upgrades into the defender side to keep
    killing, and that "choices barely matter". Max-upgrade play reads flat: one player says at max upgrades the zombies "seem to die at the
    same range", so the power difference is hard to see. Another says early progression is "very slow".
12. **[player report]** The upgrade screen does **not pause** the game ("No pause when upgrading?" thread, no developer reply seen).

### Defenders
13. **[played]** Defenders start at **0** and are bought one at a time through the Recruit node. **[player report]** Named types: rookies
    and rangers (a player's words; the web build calls the first node "Recruta").
14. **[store screenshots]** The late game shows hundreds of small coloured soldiers packed on the rooftop, firing guns and flamethrowers,
    with five currencies. **[dev claim]** "Kit your grunts out with the best toys you can get your hands on".

### Items and loot
15. **[player report]** Zombies drop **items**. There is an **inventory with a hotbar**, and items can be equipped. Items show varied
    icons, but "an icon can have any stat", so the icon tells you nothing about the stat (suggestion: tie stats to the icon). Players
    want clearer drop visuals, a visible inventory and quick-trash, and one proposed a perk that auto-equips the best item per category.
16. **[player report, bug]** An item-duplication glitch (hover the hotbar and hold both mouse buttons after equipping into the same slot);
    duplicates vanish on refresh.

### Hordes, DNA and evolution
17. **[store screenshots]** A **"Call the Horde"** button sits bottom-right in every Steam screenshot. It was **not present in the web build I
    ran** at the start. **[player report]** Hordes are optional battles you call and must clear to progress; they pay **far more "horde DNA"
    than normal waves**; and the web demo "locked you out of hordes" after you finished it, which one player called a downgrade. A Steam
    player calls the horde format "very easy and easy to exploit" because it generates too many resources, so grinding hordes beats
    normal play. Another says hordes are currently a "safety net" hunting method and proposes failing one as a game over.
18. **[player report; owner confirmed DNA is earned and spent on several things]** **DNA** is a resource tied to zombie types. New zombie types unlock in the tree (a "police zombie" is named) and
    the unlock icon does not show its health, spawn rate or DNA, which one player calls "feels bad".
19. **[dev claim]** A "mad scientist" evolves the zombies "with predictably gross results". **[inference]** This is the DNA and zombie-type
    layer. How evolution changes play is unknown to me.

### Fail state and tension
20. There is none. This is the main community argument (below). **[player report]** One suggestion with some support: add a hard mode where
    zombies that pass the shredder can kill defenders; another: a small penalty for being "swarmed", like repair cost or lost soldiers.

### Controls and quality of life
21. **[played]** Mouse only; click to aim; HUD buttons are Main Menu, Upgrade, Inventory, Show Statistics. **[player report]** The cursor
    does not lock to the window, so in windowed mode the turret stops when the mouse leaves (a request to lock it). Language picker at
    first run lists all 14 languages.

### What I could not see
Currency identities and what each buys, the first defender, the second tier of the tree, zombie types, horde flow, items, evolution, the
inventory screen, how offline or background play behaves, any prestige layer, how long a first run takes.

## What the screenshots show

Store screenshots, viewed 2026-10-20 in the in-app browser, not saved in the repo. Interpretation marked.

1. `9aa9497e…` Mid-late game, five currencies. Three flamethrower streams from the rooftop across the tracks, explosions with shock rings,
   white and red ("!" suffix, *inference:* crit) damage numbers, green-glowing zombies at top.
2. `5a892f56…` Early game, three currencies (28.8K / 15.6K / 2.3K). About four soldiers, sparse zombies, one crit "130!" by the red ring.
3. `fec04ecf…` Late game, five currencies. Hundreds of soldiers on the rooftop, a wall of zombies under an explosion cluster, crits in the
   hundreds.
4. `21e786bf…` The skill tree, as described in item 10.
5. `9fa8d7d6…` Mid game, four currencies, three flame jets, fewer soldiers than shot 3.
6. `header.jpg` Capsule: hulking pale zombie with red arms, red-and-white title, darkened isometric street.

**Look.** Isometric three-quarter view, pixel-art sprites, 2D particle effects. One fixed map in every shot (grey ruined city, railway,
rooftop, parked van, rubble). Muted greys and browns so fire, blood and coloured units carry the contrast. UI is plain grey rounded
buttons with white text and small coloured currency icons with K-suffix numbers. The red aim ring is the strongest piece of visual language
in the game and the play-screen is otherwise visually flat at the start. **[played]** At the very start the screen is nearly empty (one soldier,
slow trickle of tiny zombies) and the payoff the store shots advertise is hours away.

## Patch / update history

| Date | Item |
|---|---|
| 2026-08-02 | Added to Incremental DB (page says "Prototype", "Hours" of play) |
| ~2026-08 to 2026-09 | Web demo v0.1.3 on itch.io (comments are 38 to 81 days old on 2026-10-20) |
| 2026-09-01 | Steam demo listed |
| 2026-09-22 | Steam news: Discord opens "for the beta test" (only news post) |
| 2026-10-26 | Planned release; demo progress wiped |

## What players love / push back on

No Steam reviews exist. Demo feedback below is from **15 Steam threads (about 40 posts) and 16 itch comments**, read in full on
2026-10-20. Counts are my manual tallies and are approximate.

**Love (itch):** "captivating and meditative", "like everything: the zombies, guns, isometric view, autoplay, skill tree", the "call a
horde to progress" idea, and the horde DNA yield. Steam: "Loved the demo", wishlisted; "it's a game that's meant to be comfy"; "gamers just
want to see numbers go up"; one commenter says they will buy on day one.

**Push back, by theme:**
| Theme | Roughly how often | Example |
|---|---|---|
| No lose condition, no tension | the biggest thread (14 replies), plus 3 others by one poster. About 8 against, 5 for the design in the main thread | "No lose condition equals a meaningless win"; "You can't lose Cookie Clicker or Spaceplan. And that's fine." |
| Zombie upgrade bundles health with spawn rate | 3 posts | "I am not going to level up the number and frequency of zombies if the only option is to also upgrade their health" |
| Grind, slow early game, sameness of upgrades | 5 posts | "too grindy, especially upgrades that go up to 20" |
| Items and inventory unclear | 4 posts | "an icon can have any stat" |
| Missing stats (DPS, income per unit) | 2 posts | wanted damage per unit type; one partly implemented |
| Horde mode too rewarding | 3 posts | "very easy and easy to exploit" |
| Bugs | 4 posts | dupe glitch, off-screen kills, load hang, cursor loss |
| Wants a risk mode or a penalty | 6 posts | "hard mode with true threat that could kill player" |

Two lessons: the **same design is read as comfort by some and as "a wallpaper" by others**, so a no-fail idler needs another source of
tension (choices that matter, visible progress, something to optimise). And demo players kept asking for **feedback loops** (stats, timers,
shredder counts, per-unit income) that an incremental normally ships with.

## How it differs from / resembles our design

*Our concept (2026-10-20, from the owner; updated 2026-10-21): the same kind of stress-free incremental/auto-battler, in a space or fantasy theme rather than
zombies, and visually much stronger. The owner likes the single screen, aim as the only input with an opt-out, and no way to lose. Details to be written in `PLAYBOOK.md` section 12 once there is more.*

| | ShredDead | Ours |
|---|---|---|
| Theme | zombies, post-apocalypse, pixel-art city | space or fantasy (choice open) |
| Fail state | none (the main complaint and the main selling point) | stress-free; whether any tension exists is open |
| Core economy | kill enemies for currency; buy tougher enemies for more | to decide |
| Active input | aim the main weapon; the rest is automatic | to decide |
| Camera and look | fixed isometric pixel art | owner dislikes this look |
| Optional risk layer | Hordes, optional bursts of higher reward | to decide |
| Price / length | not listed; "hours" | to decide |

## Why it matters to us

1. **Buying harder enemies is the cleanest incentive in the game.** The tree makes the player trade comfort for yield. A space or fantasy
   reskin can do this with fleets, monsters or dungeons. The weak spot is the bundle: separate "more", "faster" and "tougher" so each is a
   choice.
2. **Hordes as opt-in bursts** are the only thing that looks like decision-making. They also broke the economy in the demo (too rich).
   Borrow the idea and tune it first.
3. **Visible growth.** The rooftop filling with soldiers is the proof of progress and the main marketing image. Do something equivalent
   (a growing fleet, an army, a guild hall).
4. **Avoid the feedback gap.** Players asked for stats, per-unit income, drop clarity and an upgrade screen that does not pause the game.
   Ship these from day one.
5. **Avoid: an empty first minute.** The start is one soldier and a trickle; the spectacle is hours in.
6. **Risk, guess:** the no-fail split will show in launch reviews. If it does, the owner's "stress free" positioning needs its own hook.
7. **Risk, guess:** a small studio is shipping this at a price not yet set into a niche (idle, auto-battler, pixel art). Check the first
   reviews and price on Oct 26.

**Developer's prior release.** *Empires Shall Fall* (App ID 1477170): Strategy, released Feb 19, 2024, $12.99, Windows, single and
multiplayer. Reviews 2026-10-20: Very Positive, 151 total, 86.8% positive (131 / 20); English 66, 80.3% (53 / 13). Not read beyond the
record. Co-published with Electronic Sheep Games.

## Open questions

- What the five currencies are and what each buys; which one is DNA.
- How hordes work (size, timing, rewards, what "failing" does) and how they unlock.
- What zombie evolution changes, and how many zombie types exist.
- Whether there is a prestige or reset layer beyond the launch wipe.
- How the shredder meter works and whether the full game gives it a consequence.
- Whether the Steam demo differs from web v0.1.3 in controls (auto-aim?), tree, or hordes.
- Price and run length at launch.

## Kolby's / the owner's impressions

Owner statements, 2026-10-21 (it was not said whether these come from the Steam demo or from watching; treat as owner knowledge, not a play log):

> I like how it's a single screen.

> The railway like is the shredder - yes

> I like this style of incremental where your input is aiming. You can choose to not aim, but it reduces your efficiency early on.

> Yes, you earn DNA and you spend on various things.

> There is no way to lose - which is potentially interesting.

What this settles: the railway **is** the shredder (mechanics item 3, previously an inference); aiming is optional but pays in early efficiency
(item 5, previously inferred from player comments); DNA is earned and spent on several things (items 18 and 19). What it adds: the owner likes
the **single screen**, the **aim-only input with an opt-out**, and the **no-lose** structure. These are now the three criteria for the
competitor search ([discovery-2026-10-21.md](discovery-2026-10-21.md)).

*Still not recorded:* a hands-on account of the Steam demo (App ID 4900640): the first minutes, how the opt-out feels, and what DNA buys.

## Adjacent zombie idle and incremental titles to triage (not deep-dived)

Found by Steam name search on 2026-10-20; candidates only, read no further than the store summary line. Review figures are all-language,
all-time. **Done 2026-10-21:** the discovery pass is in [discovery-2026-10-21.md](discovery-2026-10-21.md).

| App ID | Name | Release | Price | Reviews | Why it might matter |
|---|---|---|---|---|---|
| 3609260 | Idle Zombie Wave: Premium | 2025-04-24 | $7.99 | Mixed, 23, 43.5% | Idle tower defense vs zombie hordes, roguelike upgrades |
| 1647440 | Idle TD: Heroes vs Zombies | 2022-01-24 | free | Mostly Positive, 165, 73.9% | Endless TD, heroes and artifacts |
| 2100200 | Idle Zombies Clicker | 2022-08-22 | $0.99 | Mixed, 22, 54.5% | Zombie clicker, cheap |
| 3675650 | Propaganda VS Zombies 2: Idle Deckbuilder | coming soon | not listed | none | Idle shooter with camp building |

A Steam commenter compared ShredDead to *Zero Stress Idle King*, which is *Zero Stress King: Idle Defense* (App ID 4271160). It is the closest no-lose precedent and is analysed in the discovery file.

## Screenshot index

Store screenshots, 1920x1080, **not saved in the repo**. `img/shreddead/` is reserved for them or for owner-supplied demo screenshots.

| # | file | what it shows |
|---|---|---|
| 1 | `ss_9aa9497e719388780e90e0aca470bdd282c846a9` | mid-late game, 5 currencies, flame jets and explosions |
| 2 | `ss_5a892f56a10db3e1523809198539d8d987477c79` | early game, 3 currencies, about 4 soldiers |
| 3 | `ss_fec04ecf422ef2949dd55e7e6b70ef73b33f2873` | late game, hundreds of soldiers, wall of zombies |
| 4 | `ss_21e786bf664da457593990715e3e88d3b1b344d0` | skill tree, pannable graph with costs and locks |
| 5 | `ss_9fa8d7d6d6f8f45010001f5ec80801b179d3d88d` | mid game, 4 currencies, three flame jets |
| 6 | `header.jpg` (460x215) | capsule art |

Exact URLs: `node tools/steam.mjs details 4704520`. Community text: `node tools/steam.mjs threads 4704520`.
