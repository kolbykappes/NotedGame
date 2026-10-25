# Playtest guide

*Updated 2026-10-25: the feel-test build is at the top. Below it is the guide to the ten-job system (2026-10-24 evening), still accurate except that the levels now have their own day lengths. Nobody has played any of it with a real mouse, keyboard and speakers yet.*

## Feel test (2026-10-25): start here
**Goal:** play several days at a mid level and judge three things: (1) **pacing**, whether there is slack once the official list is done; (2) **messages**, whether the job names, procedure cards, scraps and the morning slip land; (3) **the second list**, whether the off-book errands feel like a heist inside a shift. No stealth yet.

**Getting to a level**
1. Press **Play**, click **Start** on the setup screen, then **Begin my first day**. Dismiss the how-to cards.
2. Press **F6** once for Level 2, twice for Level 3 (**Specialist**, 5 jobs in 110 s, reveal stage 2), and so on up to Level 5 (**Steward**, stage 3). **Shift+F6** goes down. A jump restarts at that level with a basic tool loadout, 150 Uptime, every room open and the level's usual day number.
3. Or set **Debug Start Level** on the `GameFlow` object in the Inspector (0 is a normal game). A scene rebuild resets it to 0.
4. Each morning shows cards in order: the town's **morning slip**, a **scrap**, **updated procedures**, any new job type, then the **second list**. Space or Enter continues.

**Keys (all read straight from the keyboard; no input binding was changed)**
| Key | What it does |
|---|---|
| **F** | The off-book action at a station on today's second list (hold it where the prompt says "Hold F"). The prompt shows under the E prompt only when a second-list step is available there. |
| **Tab** (hold) | Read today's second list. It is not on the HUD. |
| **F1** | Show or hide the feel-test readout (level, clock, official list progress, reliability, rollout, errands, scraps). Its four buttons need the mouse: press **Esc** to free the cursor, then click **Wellness tape**, **Offer errand**, **Show scrap** or **Morning slip**. |
| **F2** | Replay this day: back to the start of the day, same list, same state. |
| **F3** | Skip to the next morning (ends the day as it stands). |
| **F4** | Grant every tool and +500 Uptime. |
| **F5** | End the day now (starts the pull). |
| **F6** | Jump to the next level (Shift for the previous). |

**What to try, in order**
1. Play Level 3 for a few days. Do the official list as fast as you can, then look at the readout: how much clock was left (slack) when the list finished? Is that enough for the errand?
2. Do the errand named on the second list (stations and steps are in the card). Check the **morning slip** next day: it should say how many households were restored. Check the wall sign in the administration room (Rollout and Reliability).
3. Leave the official list unfinished on purpose for a day: Reliability drops 8 points per unfinished job. Below 80% a promotion is held (the quiet-time message says so).
4. Press F2 to run the same day again with a different plan. Press F6 to Levels 4 and 5 and look at how the job names read (stage 3 puts numbers on the harm).
5. At the end of each day the numbers go to the **day log** (`daylog.csv`; its path is at the bottom of the F1 readout). Send me that file and I will tune the table from it.

**What was verified (and how)**
- 116 Edit Mode tests pass (72 earlier, 44 new: level table, start profile, replay snapshot, reliability, rollout, reveal text, scraps, the morning slip, errands and their windows).
- In Play Mode with the Editor in the background and simulated input: Level 1 plays as it did; F6 jumps to Levels 3 and 5 with every morning card read; a two-station errand (breakers) and a held-F errand (the vent) through the real interactor, including the two-line E and F prompt; the day summary, the CSV row, the next morning's slip; replay and skip; the overlay text; HUD and wall-sign captures. No console errors; `ProjectSettings` untouched.

**What I could NOT verify (please check)**
1. The **real keyboard** for F, Tab and F1 to F6 (simulated keys are dropped while the Editor is in the background, so the same code was driven through its test hooks).
2. The **overlay buttons** with a real mouse.
3. Whether **any of it is fun**, whether the errands fit the slack, and whether the job names and scraps land. All the numbers are first guesses: the day lengths (60, 80, 110, 130, 150 s), list sizes (2, 3, 5, 6, 6), the 8-point Reliability penalty, par of 1.06 a day, and each errand's effect.
4. Sound (nothing new was added) and text legibility of the small wall sign at your resolution.

**Known rough edges:** no stealth, so nothing can catch you yet; the wellness tape only appears from the overlay; there is no save, so replay and F6 are the only ways to resume; the second-list prompt is the only cue that an errand step is available (no marker beams).

## How to play it
1. Open `Assets/Scenes/Facility.unity` (it should already be open) and press **Play**. Click into the Game view if the cursor is not locked.
2. **Controls:** WASD move, mouse look, Shift sprint, **E** interact (hold for tasks, press for the cot, the terminal and the locker), **Space or Enter** continue a card, **I** toggles invert on the setup screen, **Space, Enter, Esc or a click** skips the pull, **Esc** frees the mouse (click in the game to lock it again).
3. **The loop:** **setup menu** (invert mouse **on by default**, sensitivity, field of view; remembered) → intro card (rank and code) → how-to card for each new task type → a 60-second day with a task list, a visible clock and a carry counter → at zero the game cuts to third person and types your code → quiet time (lights dim; pick up loose Uptime, file a complaint at the lit terminal, open the **tool locker** at the locker in the admin room, or press E at the cot) → promotion card if you ranked up → the tool locker (night) → Wake Up.
4. **Where things are:** you wake in the admin room, far west (bean sack, paper stack, mail table, a breaker panel). East along the corridor: break room (coffee machine, filter shelf, a breaker), print room (printer, shredder), office (clipboard), network closet (patch panel), utility room at the far east (dumpster, crates). The thermostat, vent and badge reader are in the corridor. The server hall is south of the corridor, behind a door with a purple slit.

## The jobs (eleven, in three kinds of step)
Every job is a few steps. A step is one of: **hold E** (1 to 3 s, fetch or use), **press E** (flip a switch), or **press E to open a mini-game** (a card in the middle of the screen). **The day clock keeps running during a mini-game**, and a wrong answer costs 2 s.

| Job | Rank | Steps | Pays |
|---|---|---|---|
| Restock the coffee | Trainee | bean sack (admin) to coffee machine (break room) | 10 |
| Load the printer paper | Trainee | paper stack (admin) to printer (print room) | 10 |
| Reset the breakers | Trainee | three switches: admin, break room, corridor | 10 |
| Sort the mail | Trainee | mini-game at the mail table (FILE / SHRED / OUTBOX, keys 1 to 3) | 10 |
| Log the temperature | Trainee | read the thermostat, write it on the clipboard (office) | 10 |
| Empty the shredder | Trainee | shredder (print room) to dumpster (utility) | 10 |
| Patch the cables | Associate | cable-match mini-game at the closet patch panel (cables Q W E R T Y, ports 1 to 6) | 15 |
| Cycle the badge reader | Associate | switch it off, come back after 8 s, switch it on | 15 |
| Swap the air filter | Associate | filter shelf (break room) to vent (east end) | 15 |
| Swap a failed drive | Specialist | spare drive from the hall cabinet, cable-match at the rack (needs **Anti-Static Strap**) | 25 |
| Clear the alarm | Specialist | repeat-the-pattern mini-game (pads 1 to 4), then press the reset | 25 |

You carry **one thing at a time** (Hand Cart raises it to three). Tasks per rank: Trainee 2, Associate 3 (after 5 completed tasks), Specialist 5 (after 12).

## Rooms open as you work
Doors slide up and the status light goes green, with a toast: "Access granted".
- **Open from the start:** admin room, corridor, break room.
- **Print room:** after the coffee twice. **Office:** after the printer paper once. **Utility room:** after the breakers twice.
- **Network closet:** at Associate. **Server hall:** at Specialist **and** the Staff Keycard from the locker.
- Until a room opens, no job that needs it can appear. The planner puts the job that opens the next room on the list first, so the first few days are coffee, breakers, mail, then the rooms unlock in order.

## Small oddities (Specialist only so far)
Occasional one-line toasts when you use a station, only modestly strange (a drive labelled for someone who is not here, a door log with a gap, and so on). Nobody ever appears; only evidence that someone was here. It gets weirder later.

## The tool locker (the tool menu)
Open it at the cot each night (or at the locker prop in the admin room after work). It scrolls with the mouse wheel and has three sections:
- **Tools:** Hand Cart (carry 3), Multitool (faster work), Running Shoes, Reach Grabber, Two-Way Radio (task beacons), Day Planner (tomorrow's list), Magnetic Pouch (coin pickup), Overtime Slip (+15 s), Label Maker (+3 s per task).
- **Service contracts:** coffee, printer and shredder do themselves each day (they pay coins but do not count toward rank).
- **For later jobs (locked, with a note):** Flashlight, Staff Keycard, Anti-Static Strap, Cable Tester, Crash Cart, Step Ladder, Thermal Camera, Fire Extinguisher. They are listed so you can see what the server-hall jobs will need; they cannot be bought yet.
The HUD shows the tools you own under your rank.

## What was verified (and how)
- 72 Edit Mode tests on the rules (carry capacity, hands-full, locked tools, invert logic, the unlock chain, planner priority, step kinds, mini-games, oddities, catalog sanity), all passing.
- Jobs system, in the real Editor (Editor in the background): a breaker flip with its toast; all three mini-game screens (and the clock still ticking behind them); the sort-the-mail game played through at the real station to completion and payout; the print door opening with "Access granted" after two coffees; the locked office and hall doors; the badge-reader wait-and-return; six simulated Specialist days (the planner, thermostat readings carried to the clipboard, oddity toasts, buying the strap and keycard).
- Played in the real Editor with the Editor window in the background (using `set_autotick` plus run-in-background, see `Docs/research/unity-cli-play-and-frames.md`) and the real Game view captured natively: setup screen and its buttons (settings applied and saved), intro, how-to cards, **real holds** for the fetch quests, the hands-full block, delivery at the far dumpster, the pull (captured in slow motion), quiet time, the tool locker in the quiet time and at night, scrolling, buying a Hand Cart, waking into the next day. No runtime errors in the console; `ProjectSettings` untouched.

## What I could NOT verify (please check)
1. **Real mouse clicks on the UI.** Button handlers and the raycast under the pointer work, but simulated mouse input is dropped while the Editor is in the background. Click the setup buttons, a complaint preset and a locker row yourself.
2. **Real keyboard in the setup screen** (the I key toggling invert). The toggle button itself works.
3. **Whether invert feels right on the actual mouse.** The direction logic is unit-tested (mouse up looks up when not inverted, down when inverted).
4. **Sound** (the hum and effects play but I have not heard them) and **text legibility** at your resolution.
5. **Pacing.** My route estimates with no wasted steps: a Trainee pair of tasks takes **34 to 41 s sprinting, 52 to 62 s walking**; an Associate list of three takes 41 to 52 s sprinting; a full Specialist list takes about **73 s even sprinting**, so it needs tools (Hand Cart first). Tell me if Trainee days feel too easy or too hard.
6. **The new jobs with real input.** Mini-game keys and clicks, the patch-panel, drive and alarm jobs played through their screens in the world (their rules and screens are tested, the in-world run was simulated), and whether 11 jobs and the unlock order feel right.
7. **The pull camera and console** after moving the camera closer and enlarging the monitor and text.

## Tuning knobs
- Invert mouse defaults to **ON** (`SetupScreen.DefaultInvertY`; the Player prefab also starts inverted). Saved choices override it.
- `Assets/Scripts/Core/GameConfig.cs`: day length (60), hold-time scale (1.0), carry capacity (1), coins per task (10), tasks per rank, rank thresholds, shoes bonus.
- `Assets/Scripts/Core/TaskModel.cs`: hold seconds for every step. `Assets/Scripts/Core/Economy.cs`: tool costs, growth and levels.
- `PlayerMotor` on the Player prefab: walk and sprint speed. `PullSequence`: walk, digit and hold times. `LightingController`, `Sfx`.
- Rebuild the scene after changing builder code: `unity command eval --code 'return Noted.EditorTools.FacilityBuilder.Build();' --project-path C:/Code/Noted -- --timeout 120000` (it regenerates `Facility.unity`, so hand edits to that scene are overwritten).

## Known gaps and rough edges
- Complaining pauses the clock. No pause menu. The setup menu only appears at launch (no way back to it yet).
- UI is mouse and keyboard only; no controller navigation.
- The avatar in the pull is a capsule; props are cubes.
- No save and load; closing the game resets everything (settings from the setup menu are remembered).
- Discovery, story and most later-job tools are not built. Oddities exist for Specialist only.
