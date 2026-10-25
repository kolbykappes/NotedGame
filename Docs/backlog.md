# Backlog

One line per item: status, item, notes.

## Needs a human (next playtest)
- [ ] Play the new jobs with a real keyboard and mouse: the three mini-games (keys and clicks), the door unlocks, the reader wait-and-return, the strap and keycard purchases. Judge whether early days feel mundane but not boring, and whether 11 jobs and the unlock order feel right.
- [ ] Play `Facility` start to finish with a real mouse and speakers. Feel, pacing, sound, text legibility. See `Docs/playtest-guide.md`.
- [ ] Click the UI with a real mouse (setup buttons, intro button, a complaint preset, a locker row, Wake Up/Close). Simulated clicks are dropped while the Editor is in the background.
- [ ] Check the invert-mouse direction with the real mouse (the logic is unit-tested).
- [ ] Judge the fetch-quest pacing and the closer pull camera.
- [ ] Decide whether Trainee days are tight enough (ideal route: 34 to 41 s sprinting, 52 to 62 s walking, out of 60; knobs: step hold times, `BaseCarryCapacity`, the building layout).
- [ ] Tune the guessed numbers: hold times, upgrade costs, rank thresholds, pull timings, lighting fade, sound volumes, FOV, sensitivity.

## Feel test (2026-10-25; requirements and acceptance in `Docs/feel-test-plan.md`)
Goal: start at a chosen level and play several days to judge pacing, messages and the second list. **Slices 0 to 4 built 2026-10-25** (116 Edit Mode tests pass; checked in Play Mode with simulated input). Keys approved by the owner 2026-10-25: F, Tab, F1 to F6 (read through `Keyboard.current`, no binding change).
- [x] **Slice 0, jump harness:** start profile, F6 level jump, F2 replay day, F3 skip day, F4 tools, F5 end day, F1 overlay, per-day CSV log (R0.1 to R0.6). Changed from the plan: F6 is the level jump (no PageUp/PageDown); the message preview moved to overlay buttons.
- [x] **Slice 1, level table:** five levels, table-backed day length, task count, thresholds, reveal stage, rank-up text (R1.1 to R1.5). Specialist is 5 jobs in 110 s (the plan had 4 in 90 s) to match the measured walking pace.
- [x] **Slice 3, reveal and messages:** stage-based job text, "updated procedures" card, 12 scraps, morning slip, flat reply tier, 10 more oddities, wellness tape text (R3.1 to R3.8). Wellness is previewable from the overlay; its trigger waits for slice 5.
- [x] **Slice 2, Reliability and Rollout:** promotion gate at 80%, rollout par, floor and cap, HUD line, wall sign (R2.1 to R2.4).
- [x] **Slice 4, the second list (no stealth):** five errands on existing stations, F to act, Tab to read, scrap card at day start (R4.1 to R4.8).
- [ ] **Needs a human:** play it. Real F, Tab and F1 to F6 keys; the overlay buttons with the real mouse (press Esc to free the cursor); whether the errands fit the slack at Levels 3 to 5; whether the job names and scraps land; the day log numbers (`daylog.csv`, path in the overlay).
- [ ] Tune from the day log: day lengths and list sizes per level, Reliability penalty (8 per unfinished task), errand effects, par (1.06 a day).
- [ ] Known rough edges: the wall sign is small; no sound for the second list; no save, so replay and level jump are the only ways to resume; a level jump restarts with the basic tool loadout and 150 Uptime.
- [ ] **Deferred, slice 5:** stealth (camera eyes, restricted zones, audits, Attention, pull exfil, Wellness Moment trigger) and hacking via reused mini-games. See the plan's "Not in this plan".

## Next slices
- [ ] The hall door's purple slit overlaps its text (cosmetic; move the slit or the text).
- [ ] A real cut-out for rooms that stay locked (a pass-through sign or a closer-look text when the player presses E on a closed door).
- [ ] More oddities for Associate days and a later "weirder" layer for the discovery pass.
- [ ] Per-job how-to cards for the mini-games (they explain themselves on screen now).
- [ ] Pause menu that reopens the setup screen (invert, sensitivity, FOV) plus volume and a "skip the pull" option.
- [ ] Make the later-job tools real once the server-hall jobs exist (flashlight, keycard, anti-static strap, cable tester, crash cart, ladder, thermal camera, fire extinguisher).
- [ ] Apply the CLI research to the portable kit (C:/Code/unity-claude-kit, the owner's call): see the end of Docs/research/unity-cli-play-and-frames.md.
- [ ] Complaining should not pause the clock (decide: keep as a free pause, or run the clock).
- [ ] Controller support for the UI (navigation) and look.
- [ ] Real "training tape" presentation for rank-ups (CRT in the world, not only a card).
- [ ] The locked door: a "try the door" interaction (discovery hook).
- [ ] Replace placeholder avatar, props, sounds and signs; the palette is in `Docs/art/palette.md`.
- [ ] Save/load (needs a plan first).
- [ ] Windows build test (`unity build`), since an Editor playtest does not prove a player build (kit rule 31).
- [ ] Play Mode tests for the flow (frames advance in the background with set_autotick plus run-in-background; see the research note).

## Done
- [x] Ten-job system: eleven jobs, three step kinds, three mini-games, unlockable rooms with doors, tool-gated jobs, Specialist oddities (2026-10-24, evening).
- [x] Invert mouse defaults to on; the mirrored SUPPLIES sign fixed and all 24 world signs checked (2026-10-24).
- [x] Kit 1.2.0: `unity-ops.sh play-begin`, `play-end` and `snap` (2026-10-24).
- [x] Git repository on `main` with a Unity `.gitignore` (2026-10-24).
- [x] Movement and camera (2026-10-24).
- [x] Core rules, five tasks, rank, wallet, 11 upgrades, complaint replies (2026-10-24).
- [x] Facility scene, HUD, cards, shop, complaint terminal, pull, quiet time, sound (2026-10-24).
