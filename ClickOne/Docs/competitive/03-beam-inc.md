# BEAM Inc.

*Researched 2026-10-21. Read: the Steam store page and `appdetails` record, the demo record, the tag list, 4 news posts, 2 community threads (one a
Chinese-language demo review with developer replies), the developer's earlier game and one comparison game. **Played by the owner**, not by Claude: the
owner sent **13 screenshots from the first 30 minutes of the Steam demo** and a verdict, quoted below. Claude has not played it. The trailer was not
watched. No store screenshots were viewed. Facts below are tagged **[store]**, **[dev]** (developer post), **[owner]**, **[screens]** (visible in the
owner's screenshots), **[player]**, or **[inference]**. The inferences about *why* it feels bad are Claude's reading of the screenshots; the owner's
own words are in the impressions section.*

- **Steam:** https://store.steampowered.com/app/5048900/ (App ID 5048900) | **Demo:** App ID 5133070 (free, listed 2026-08-24)
- **Release:** 2026-10-28, unreleased. **[dev]** "launches October 28 with a 30% launch discount" (news, 2026-10-19). Part of Steam Next Fest.
- **Price:** not listed yet (2026-10-21)
- **Reviews (2026-10-21):** none, total 0
- **Developer/publisher:** Imoyokan, self-published; **[dev]** "a solo dev" (2026-08-18 post). Previous game: *Idle Monkeylogy* (2021, App ID 1690690,
  free with $4.99 and $14.99 packs, published by Idle System, Inc.), Mixed, 155 reviews, 58.1% positive. A thread commenter joked about an "IM" shape
  in the beam pattern; the developer answered it was "pure coincidence".
- **Platform:** Windows only, single-player, no controller support, Steam Achievements, Family Sharing, "Playable without Timed Input", 9 languages
  (EN, FR, DE, ES-ES, PT-BR, RU, ZH-CN, JA, KO). Minimum spec: Windows 10, Core i5, 8 GB RAM, DX10, 2 GB disk.
- **Store tags:** Incremental, Idler, Casual, Arcade, Singleplayer, Destruction, Shooter, Bullet Hell, Top-Down Shooter, Resource Management,
  Simulation, 2D, Indie, Colorful, Strategy, Top-Down, Sci-fi, Management, Atmospheric, PvE.
- **Generative-AI disclosure:** none on the page.

## Premise and scale

**[store]** "A short active incremental where you neutralize unknown specimens in a sterile research facility. Destroy geometric specimens with beams.
Upgrade beams through node-tree research. Evolve beam properties with piercing, reflection, and splitting." **[dev]** (2026-08-18) "you start with one thin
beam, and end up bending light itself: reflections, refraction, split beams, interference, auto-aim"; "~2-3 hours: observation rounds → upgrade research →
new specimen discoveries"; "No click-spam — steer the beam, then automate it and watch the lab work." The tone is clinical and abstract: no characters,
no story in the demo.

## Mechanics (as seen in the first 30 minutes of the demo)

**[screens]** unless marked.

1. **The loop is rounds, not idle.** Press **Start Observation**. A round runs until your **energy** is gone ("Energy Depleted"). Energy is a shot budget
   shown bottom-left as "0 / 10", later "0 / 15" and "0 / 37" (the **Capacity** stat). The round summary lists Photon earned, Laser shots, Kills (with
   icons per specimen type), Photon balance, and a line "Unlock Circle after N more kills". Buttons: **Start Observation** and **Open Upgrade Screen**,
   plus a badge "Available Upgrade: N" when something is affordable.
2. **Input is aiming.** A turret ring sits bottom-centre; a thin blue beam runs up from it toward the pointer. **[inference]** You aim the mouse; shots
   fire at the Fire Rate. The store says "steer the beam"; whether any auto-aim exists in the demo is not visible.
3. **Playfield.** One fixed screen: dark purple grid; specimens spawn in **two circular clusters** at the top left and top right (an upgrade named
   "Spawn Area Expansion" exists). Specimens are **red outlined triangles**; a second type, **cyan circles**, appears after unlock. Small white damage
   numbers (4, 8) appear on hits. **[screens, screenshot 11]** A group of circles sits in the centre column, so specimens do not stay in their clusters
   (movement itself is not visible in stills).
4. **Specimen types are unlocked by kill count and a node purchase.** Circle needed "298 more kills" after run 1; Square later needs "600 more kills".
   The demo banner reads "Demo goal: Unlock 2 more specimen types", going from (0/2) to (1/2) once the Circle node was bought.
5. **Upgrades: a node graph on a separate screen** ("Open Upgrade Screen"). It starts as one node (**Power Up 1**) and grows outward; locked nodes show as
   dark circles labelled "???" and reveal as you buy. Colours: blue = beam stats, pink diamonds = specimen nodes (Unlock, Growth, Hardening), lavender =
   sub-units. Each tooltip shows the level (e.g. Lv. 4 / 20), a description, the change (e.g. "Max Range 4 → 5 (+1)") and the cost.
6. **A live "Beam Spec" panel** (top right) lists Damage, Fire Rate, DPS, Width, Range, Pierce, Capacity.
7. **Nodes seen:** Power Up 1 (damage, Lv 0/10, 1→2, cost 5), Energy Capacity 1 (0→5, cost 9), Fire Rate 1, Range Extension (Lv 4/20, 4→5, cost 104),
   Beam Width, Pierce (Lv 1/10, cost 75), Pierce Decay Mitigation, Spawn Area Expansion, **Specimen Growth: Triangle** ("Enlarges the specimen. Kill
   reward +50%"), Specimen Hardening: Triangle / Circle, Specimen Unlock: Circle / Square, **Sub-Unit (Left)** and **(Right)** (extra turrets; a green ring
   appears next to the main ring), Sub-Unit Fire Rate, Sub-Unit Power.
8. **Fail state: none.** A round ends when energy runs out; the player loses nothing. **[inference]** That makes it another "no way to lose" game, but
   with a limited-length active round instead of a continuous idle screen.

### Beam Spec over the 30 minutes

| Moment | Damage | Fire rate | DPS | Width | Range | Pierce | Capacity |
|---|---:|---:|---:|---:|---:|---:|---:|
| Start (screenshots 2, 3) | 1.0 | 1.0/s | 1.0 | 0.05 | 5 | 0 | 10 |
| After Power Up 1 (screenshot 5) | 2.0 | 1.0/s | 2.0 | 0.05 | 5 | 0 | 10 |
| Photon 89 (screenshot 7) | 6.0 | 1.4/s | 8.2 | 0.05 | 5 | 0 | 28 |
| Pierce level 1 (screenshot 8) | 6.0 | 1.4/s | 8.2 | 0.05 | 6 | 1 | 28 |
| Circle unlocked (screenshot 10) | 8.0 | 2.3/s | 18.1 | 0.05 | 8 | 2 | 37 |
| Last upgrade screen (screenshot 13) | 8.0 | 2.3/s | 18.1 | 0.05 | 9 | 3 | 37 |

### The runs

| Run | Shots (capacity) | Kills | Photon earned | Balance after | Next unlock line |
|---|---|---:|---:|---:|---|
| 1 | 10 shots (10) | 2 | +4 | 4 | Circle after 298 more kills |
| 2 | 10 shots (10) | 4 | +8 | 11 | Circle after 292 more kills |
| 3 | 14 shots (15) | 12 | +24 | 25 | Circle after 266 more kills |
| 4 | 35 shots (37) | 37 | **+1,148** | 1,208 | Square after 600 more kills |
| 5 | 35 shots (37) | 15 (11 triangles, 4 circles) | +108 | 154 | Square after 593 more kills |

**Where the numbers do not agree (unexplained):**
- Photon per kill is **2.0** in runs 1 to 3, **about 31** in run 4, and **about 7** in run 5 (with circles in the mix). Specimen Growth: Triangle (+50% kill
  reward) is the only reward upgrade seen, which does not account for a roughly 15x jump (**[inference]** other multipliers, such as Hardening or pierce kills, may apply).
- The "more kills" countdown falls faster than the kills shown: 2 kills took it from 300 to 298, 4 kills took it by 6, 12 kills took it by 26, and by run 4 the
  Circle goal was met after only 37 more kills shown. In run 5, 15 kills moved the Square countdown by only 7. **[inference]** the counter weights kills in some way not shown on screen.
- Run 1 earned +4 and the first upgrade costs 5, and the screens show a balance of 8 before the first purchase, so at least one more round (not shown) happened.

## What the screenshots show (the visual read)

**[screens]** Flat and dark: navy and purple background with a faint grid; neon red and cyan outline shapes; a single thin beam line; small white numerals. The
upgrade screen is dark with glowing blue, pink and lavender nodes joined by thin lines. UI typography is plain and consistent. Visible rough edges:
- The label text on the **Start Observation** and **Open Upgrade Screen** buttons is **clipped** at the top and bottom of the button in every screenshot that shows them.
- On the upgrade graph, **lines run through labels** ("Fire Rate 1", "Energy Capacity 1", "Specimen Growth: Triangle", "Pierce Decay Mitigation") and tooltips
  cover neighbouring nodes.
- The beam is a hairline (Width 0.05) and the specimens are small outlines; there is almost nothing in the first minutes of a round.
- The summary pop-up covers most of the playfield, so the player never sees the result of the round except as numbers.

## Patch / update history

| Date | Item |
|---|---|
| 2026-08-18 | Store page live; "solo dev" post |
| 2026-08-24 | **Demo live** |
| 2026-08-25 | Demo update: beam visuals reworked "(soft glow + bright core)", a "clear demo goal banner", "juiced up kill/hit effects" |
| 2026-08-31 | Demo update: updated visual effects |
| 2026-10-01 | Demo update: balance adjustments, "richer visuals" |
| 2026-10-19 | Next Fest demo post; launch Oct 28, 30% launch discount |

The owner's screenshots include the demo-goal banner (added Aug 25), so the build is at least that new; the exact build is unknown.

## What players say

No Steam reviews. Two community threads (both read):
- **Chinese-language demo review, translated:** the player says play is using the mouse to give the objects in the scene a direction, with rewards spent on
  turret damage, enemy count and ammo endurance; the visuals are "extremely minimalist, even crude"; it "grasps the essence of an incremental game: only the increment logic, nothing else";
  they finished the demo in about 40 minutes and could count it as cleared at about 30 minutes. **Verdict: "not recommended at current content unless the price is near zero"**,
  and they compare it with *Shelldiver* (good art, 3 to 4 hours of play, "only around nine" on sale). Suggestions: make the beam look like a beam rather than a rectangle with square
  corners; there was no clear goal ("I only saw a pop-up after unlocking all three types"; they could already one-shot everything with near-unlimited ammo); more visual effects and sound,
  because "an incremental's lifespan is not long; if you can give the player satisfying feeling for a few tens of minutes in the flow, that's passing".
  The developer thanked them, said the demo had cut too much and explained too little, and posted the Aug 25 update the same day.
- **An "IM" joke thread:** the developer confirmed the coincidence; nothing about mechanics.

## How it differs from / resembles our design

| | BEAM Inc. | Owner's three likes | Zero Stress King | ShredDead |
|---|---|---|---|---|
| Single screen | round screen plus a **separate upgrade screen** | yes | one screen | one screen |
| Input | aim a beam, round by round | aim, skippable | pointer, optional | aim, skippable |
| Fail state | none (energy runs out) | none | none | none |
| Currency | one (Photon) | DNA-like, many things | several | DNA plus others |
| Look | flat neon outlines | stronger than pixel art | cheerful pixel art | pixel-art zombies |
| Pacing | early rounds of about 10 shots and 2 kills | | | |

## Why it matters to us

1. **A concrete example of what the owner rejected: a no-fail aim game whose first minutes are almost empty and whose look is flat.** Together with the
   owner's verdict, it is evidence the feeling in the first 5 minutes decides whether the loop is worth continuing.
2. **Borrow candidates (inference; the owner did not praise these):** the live **Beam Spec** panel with DPS; the tooltip format "stat A → B (+n), cost"; the
   round summary that ends with "Unlock X after N more kills"; the "Available Upgrade: N" badge; "???" nodes that reveal as the tree grows.
3. **Avoid:** clipped button text and labels crossed by lines (cheap to fix and read as unfinished); a first round that ends after 10 shots; a separate upgrade screen that
   interrupts the aim loop; unexplained reward jumps (a player cannot learn the system if the numbers do not add up); a demo with no stated goal (the developer
   admitted this).
4. **A quality bar to respect.** A reviewer measures a $5-ish incremental against *Shelldiver* (see the discovery file's new addendum): short incrementals at $3 to $6 sell on charm and polish, not on
   systems alone.
5. **Developer behaviour is a positive signal for the genre, not the game:** the developer answered within a day and shipped a visual update, and Steam rewards
   that pattern (compare Zero Stress King's weekly patches). It has not rescued the screenshots the owner sent.
6. **Risk, guess:** with a 30% launch discount on a 2 to 3 hour game by a solo developer whose last title scored Mixed, early reviews will probably track "looks and
   first-hour feel". Check the first reviews on 2026-10-28 or after.

## Open questions

- Does the build the owner played include the Oct 1 "richer visuals" update, or an older one?
- Is there any auto-aim or automation in the demo (the developer promises "automate it and watch the lab work")?
- What is the photon-per-kill formula, and what is the kill counter counting?
- What do reflection and splitting do (promised in the store text, absent from the screenshots)?
- Final price after the 30% launch discount.

## Kolby's / the owner's impressions

Owner's words, 2026-10-21 (verbatim):

> OK, this is for Beam Inc. The game sucks. It's really awful in terms of mechanics and looks. Here's some screen caps from the first 30m of the demo.

*(Context: this is the owner's first hands-on report on this game. Played for about 30 minutes; the screenshots reached "Demo goal 1/2".)*

## Screenshot index

13 owner screenshots, 1920x1080, saved as WebP in `img/beam-inc/`, numbered in **play order** (the owner sent them out of order). Spot-checked against names:
1.png and 12.png matched what the names say.

| # | file | what it shows |
|---|---|---|
| 1 | `01-run1-energy-depleted-2-kills-4-photon.webp` | first round summary: +4 photon, 10 shots, 2 kills, "Unlock Circle after 298 more kills", bar 0 / 10 |
| 2 | `02-upgrade-tree-start-power-up-only.webp` | first upgrade screen, one faint node (Power Up 1), 4 photon, Beam Spec at 1.0 / 1.0 per second / range 5 |
| 3 | `03-upgrade-tooltip-power-up-1-cost-5.webp` | Power Up 1 tooltip: damage 1 to 2, cost 5, 8 photon |
| 4 | `04-run2-energy-depleted-4-kills-8-photon.webp` | second summary: +8, 4 kills, 292 more kills, "Available Upgrade: 2" |
| 5 | `05-upgrade-tooltip-energy-capacity-cost-9.webp` | Energy Capacity 1 tooltip (0 to 5, cost 9), a plus-shaped starter tree with "???" nodes, "Specimen Growth: Triangle" visible |
| 6 | `06-run3-energy-depleted-12-kills-24-photon.webp` | third summary: +24, 14 shots, 12 kills, 266 more kills, bar 0 / 15 |
| 7 | `07-upgrade-specimen-growth-triangle-purchased.webp` | Specimen Growth: Triangle tooltip ("kill reward +50%", Purchased), 89 photon, Beam Spec 6.0 / 1.4 / 8.2 |
| 8 | `08-upgrade-tooltip-pierce-cost-75.webp` | Pierce tooltip, Lv 1 / 10, cost 75; graph grown upward (Pierce, Pierce Decay Mitigation), 9 photon |
| 9 | `09-run4-energy-depleted-37-kills-1148-photon.webp` | fourth summary: **+1,148 photon**, 35 shots, 37 kills, "Unlock Square after 600 more kills", bar 0 / 37, Available Upgrade 8 |
| 10 | `10-upgrade-tree-circle-unlocked-goal-1-of-2.webp` | 1,208 photon; Circle unlocked (goal 1 / 2), Sub-Unit left and right appear, Beam Spec 8.0 / 2.3 / 18.1 |
| 11 | `11-gameplay-circles-and-triangles-6-of-37-energy.webp` | the only in-round screenshot: red triangles and cyan circles, thin beam, damage numbers 4 and 8, second (green) ring turret, bar 6 / 37 |
| 12 | `12-run5-energy-depleted-15-kills-108-photon.webp` | fifth summary: +108, 15 kills (11 triangles, 4 circles), balance 154, Square after 593 more kills |
| 13 | `13-upgrade-tree-wide-view-range-extension-tooltip.webp` | wide upgrade graph: left branch (Specimen Circle, Sub-Units), right branch (beam stats), Range Extension Lv 4 / 20 cost 104 |
