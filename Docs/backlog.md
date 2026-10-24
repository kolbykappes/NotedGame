# Backlog

One line per item: status, item, notes.

## Needs a human (next playtest)
- [ ] Play `Facility` start to finish with a real mouse and speakers. Feel, pacing, sound, text legibility. See `Docs/playtest-guide.md`.
- [ ] Click the UI with a real mouse (setup buttons, intro button, a complaint preset, a locker row, Wake Up/Close). Simulated clicks are dropped while the Editor is in the background.
- [ ] Check the invert-mouse direction with the real mouse (the logic is unit-tested).
- [ ] Judge the fetch-quest pacing and the closer pull camera.
- [ ] Decide whether Trainee days are tight enough (ideal route: 34 to 41 s sprinting, 52 to 62 s walking, out of 60; knobs: step hold times, `BaseCarryCapacity`, the building layout).
- [ ] Tune the guessed numbers: hold times, upgrade costs, rank thresholds, pull timings, lighting fade, sound volumes, FOV, sensitivity.

## Next slices
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
- [x] Invert mouse defaults to on; the mirrored SUPPLIES sign fixed and all 24 world signs checked (2026-10-24).
- [x] Kit 1.2.0: `unity-ops.sh play-begin`, `play-end` and `snap` (2026-10-24).
- [x] Git repository on `main` with a Unity `.gitignore` (2026-10-24).
- [x] Movement and camera (2026-10-24).
- [x] Core rules, five tasks, rank, wallet, 11 upgrades, complaint replies (2026-10-24).
- [x] Facility scene, HUD, cards, shop, complaint terminal, pull, quiet time, sound (2026-10-24).
