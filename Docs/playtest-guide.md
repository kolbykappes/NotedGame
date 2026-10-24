# Playtest guide

*Updated 2026-10-24 (evening): the ten-job system and unlockable rooms. Nobody has played the changes below with a real mouse and speakers yet.*

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
