# Changelog

One line per completed slice: date, what changed, what was verified.

- 2026-10-24: **Project set up.** Unity 6000.6.6f1 URP project at `C:\Code\Noted`; unity-claude-kit 1.0.0 installed; `com.unity.pipeline` 0.8.0-exp.1 installed. Verified: Editor opens and answers `eval`; kit preflight clean.
- 2026-10-24: **Movement and camera, first slice.** `FirstPersonMath`, `PlayerMotor`, `PlayerLook`, `Player` prefab, `Greybox` scene. Verified: compile clean; 15 of 15 Edit Mode tests pass; prefab references read back. The owner played it and said it feels fine.
- 2026-10-24: **Core loop rules (plain C#).** `GameSession`, `DayClock`, `DayPlanner`, five starter tasks, rank tracker, wallet, 11 upgrades and their modifiers, complaint reply bank, code generator, card text. Verified: 40 Edit Mode tests pass.
- 2026-10-24: **Facility scene and runtime loop.** `FacilityBuilder` (idempotent scene generator), stations, coins, HUD, cards, complaint screen, night shop, pull sequence, lighting controller, `GameFlow`, off-screen capture helper. Verified in Play Mode with simulated input: the full loop for several days, both promotions, both two-step tasks with real holds, the natural timer expiring into the pull, Space/Enter shortcuts; no console errors. Fixed along the way: child scale inheritance in the builder, signs showing through walls (switched to world-space UI text), a debug helper that swallowed the clock's expiry.
- 2026-10-24: **Tuning and polish.** Global `TaskHoldScale` (1.5), shredder moved to the far utility room, cards and the task panel fit their text, avatar stands beside the console, console stand point moved back.
- 2026-10-24: **Placeholder sound.** Procedural tones and a hum (`Sfx`); regression run passes. **Not heard by a human.**
