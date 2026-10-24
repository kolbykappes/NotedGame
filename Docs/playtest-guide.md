# Playtest guide

*Written 2026-10-24 night by Claude, after building the MVP loop. Nobody has played this with real mouse and ears yet.*

## How to play it
1. Open `Assets/Scenes/Facility.unity` (it should already be open) and press **Play**. Click into the Game view if the cursor is not locked.
2. **Controls:** WASD move, mouse look, Shift sprint, **E** interact (hold for tasks, press for the cot and the terminal), **Space or Enter** continue a card, **Space, Enter, Esc or a click** skips the pull, **Esc** frees the mouse (click in the game to lock it again).
3. **The loop:** intro card (rank and code) → how-to card for each new task type → a 60-second day with a task list and a visible clock → at zero the game cuts to third person and types your code → quiet time (lights dim; pick up loose Uptime coins, file a complaint at the lit terminal in the admin room, or press E at the cot) → promotion card if you ranked up → night shop → Wake Up.
4. **Where things are:** you wake in the admin room (far west). The break room (coffee, supply shelf) is next, then the print room (printer), the office (clipboard), and the utility room at the far east (shredder). The vent is at the east end of the corridor, the thermostat is by the east window. The server hall behind the windows cannot be entered.

## What is built (all with placeholder cubes)
- Five tasks: coffee, printer, shredder (hold E at one station); filter (shelf, then vent) and temperature (thermostat, then clipboard) take two steps and make you carry an item.
- Tasks per rank: Trainee 2, Associate 3 (rank after 5 completed tasks), Specialist 5 (after 12). New task types appear with a how-to card.
- Uptime coins spray from a finished task; walk over them to collect them; anything left is banked when you sleep.
- Rank-ups: Associate (blue lanyard, shown on the avatar in the pull) and Specialist (comfortable shoes, +8% speed).
- Complaint terminal: eight presets, a rotating reply that is always some version of "I understand your frustration", more cloying at higher rank.
- Night shop: Longer Day, Faster Hands, Running Shoes, Long Reach, Tidy Habits, Task Marker, Morning Briefing, Coin Magnet, and three automations (coffee, printer, shredder: they pay coins but do not count toward rank).
- Placeholder sound: coin, step, task done, pull ticks, purchase, reply, wake, sleep, and a faint hum.

## What was verified (and how)
- 40 Edit Mode tests on the rules, all passing.
- Played in the real Editor with simulated keyboard input: intro, how-to cards, a real hold-E task, coin collection, the natural clock running out and the pull, quiet time, the complaint form, sleeping at the cot, buying upgrades, waking, three days to a promotion, the second promotion and shoes, both two-step tasks with real holds, Space to skip the pull, Enter in the shop. No console errors or warnings.
- Screenshots of every screen were rendered off-screen and read (`tools/out/`, not committed).

## What I could NOT verify (please check)
1. **Feel.** Movement, turning, the pull's cut, coin spray, lighting fade, the 60-second pace. Nothing here has been judged by a human.
2. **Real mouse clicks on the UI.** The click handlers and the raycast under the pointer work, but a simulated mouse is ignored by the Input System unless it is really over the Game view. Click the intro button, a preset and a shop row yourself.
3. **Sound.** The hum and effects play (an audio source is running) but I have not heard them. Check volume and whether the hum is annoying (Sfx component, `humVolume` and `volume`).
4. **Text size and contrast** at your resolution, especially the signs and the task panel over a bright ceiling.
5. **Whether a Trainee day is tight enough.** By my route estimates an ideal Trainee day needs 40 to 43 s sprinting or 48 to 52 s walking out of 60, so any dithering fails. That may be easier than you wanted.

## Tuning knobs
- `Assets/Scripts/Core/GameConfig.cs`: day length (60), **TaskHoldScale (1.5)**, coins per task (10), tasks per rank, rank thresholds, shoes bonus.
- `Assets/Scripts/Core/TaskModel.cs`: base hold seconds for each task.
- `Assets/Scripts/Core/Economy.cs`: upgrade costs and growth.
- `PlayerMotor` on the Player prefab: walk and sprint speed. `PullSequence`: walk, digit and hold times. `LightingController`: quiet-time brightness. `Sfx`: volume.
- After changing scene-building code, rebuild with `unity command eval --code 'return Noted.EditorTools.FacilityBuilder.Build();' --project-path C:/Code/Noted -- --timeout 120000` (it rebuilds `Facility.unity` from scratch, so hand-edits to that scene are overwritten).

## Known gaps and rough edges
- Complaining pauses the clock (the form is a free pause button).
- No pause menu or settings screen (FOV, sensitivity, invert Y are fields on the Player prefab only).
- The UI is mouse- and keyboard-only; no controller navigation.
- The shop and cards are plain placeholder panels; the how-to cards only appear the first time a task type shows up.
- No save and load; closing the game resets everything.
- Discovery, story, tapes beyond the two promotion cards, and the locked door are not built (the door is a prop with a purple slit).
- The pull's avatar is a capsule with a lanyard.
