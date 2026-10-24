# Playtest guide

*Updated 2026-10-24 (afternoon), after the owner's first feedback. Nobody has played the changes below with a real mouse and speakers yet.*

## How to play it
1. Open `Assets/Scenes/Facility.unity` (it should already be open) and press **Play**. Click into the Game view if the cursor is not locked.
2. **Controls:** WASD move, mouse look, Shift sprint, **E** interact (hold for tasks, press for the cot, the terminal and the locker), **Space or Enter** continue a card, **I** toggles invert on the setup screen, **Space, Enter, Esc or a click** skips the pull, **Esc** frees the mouse (click in the game to lock it again).
3. **The loop:** **setup menu** (invert mouse **on by default**, sensitivity, field of view; remembered) → intro card (rank and code) → how-to card for each new task type → a 60-second day with a task list, a visible clock and a carry counter → at zero the game cuts to third person and types your code → quiet time (lights dim; pick up loose Uptime, file a complaint at the lit terminal, open the **tool locker** at the locker in the admin room, or press E at the cot) → promotion card if you ranked up → the tool locker (night) → Wake Up.
4. **Where things are** (you wake in the admin room, far west of the building): break room (coffee machine, supply shelf with filters), print room (printer, shredder), office (paper stack, clipboard), utility room at the far east (coffee beans, dumpster). The vent is at the east end of the corridor, the thermostat by the east window. The server hall behind the windows cannot be entered.

## The tasks (short fetch quests; the walking is the work)
Each task is "pick something up (1 to 1.5 s), carry it across the building, use it (2 to 3 s)":
- **Restock the coffee:** bean sack (utility) → coffee machine (break room).
- **Load the printer paper:** paper stack (office) → printer (print room).
- **Empty the shredder:** pull the bag (print room) → dumpster (utility).
- **Swap the air filter** (Associate and up): filter (break room) → vent (east end).
- **Log the temperature** (Associate and up): thermostat → clipboard (office).
You carry **one thing at a time**; at a pickup with full hands you get "Hands full. Deliver what you are carrying first." The **Hand Cart** raises that to three. Tasks per rank: Trainee 2, Associate 3 (after 5 completed tasks), Specialist 5 (after 12).

## The tool locker (the tool menu)
Open it at the cot each night (or at the locker prop in the admin room after work). It scrolls with the mouse wheel and has three sections:
- **Tools:** Hand Cart (carry 3), Multitool (faster work), Running Shoes, Reach Grabber, Two-Way Radio (task beacons), Day Planner (tomorrow's list), Magnetic Pouch (coin pickup), Overtime Slip (+15 s), Label Maker (+3 s per task).
- **Service contracts:** coffee, printer and shredder do themselves each day (they pay coins but do not count toward rank).
- **For later jobs (locked, with a note):** Flashlight, Staff Keycard, Anti-Static Strap, Cable Tester, Crash Cart, Step Ladder, Thermal Camera, Fire Extinguisher. They are listed so you can see what the server-hall jobs will need; they cannot be bought yet.
The HUD shows the tools you own under your rank.

## What was verified (and how)
- 48 Edit Mode tests on the rules (including carry capacity, hands-full, locked tools, the invert logic), all passing.
- Played in the real Editor with the Editor window in the background (using `set_autotick` plus run-in-background, see `Docs/research/unity-cli-play-and-frames.md`) and the real Game view captured natively: setup screen and its buttons (settings applied and saved), intro, how-to cards, **real holds** for the fetch quests, the hands-full block, delivery at the far dumpster, the pull (captured in slow motion), quiet time, the tool locker in the quiet time and at night, scrolling, buying a Hand Cart, waking into the next day. No runtime errors in the console; `ProjectSettings` untouched.

## What I could NOT verify (please check)
1. **Real mouse clicks on the UI.** Button handlers and the raycast under the pointer work, but simulated mouse input is dropped while the Editor is in the background. Click the setup buttons, a complaint preset and a locker row yourself.
2. **Real keyboard in the setup screen** (the I key toggling invert). The toggle button itself works.
3. **Whether invert feels right on the actual mouse.** The direction logic is unit-tested (mouse up looks up when not inverted, down when inverted).
4. **Sound** (the hum and effects play but I have not heard them) and **text legibility** at your resolution.
5. **Pacing.** My route estimates with no wasted steps: a Trainee pair of tasks takes **34 to 41 s sprinting, 52 to 62 s walking**; an Associate list of three takes 41 to 52 s sprinting; a full Specialist list takes about **73 s even sprinting**, so it needs tools (Hand Cart first). Tell me if Trainee days feel too easy or too hard.
6. **The pull camera and console** after moving the camera closer and enlarging the monitor and text.

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
- Discovery, story, the locked door and later-job tools are not built.
