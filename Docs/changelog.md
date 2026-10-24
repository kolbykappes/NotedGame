# Changelog

One line per completed slice: date, what changed, what was verified.

- 2026-10-24: **Project set up.** Unity 6000.6.6f1 URP project at `C:\Code\Noted`; unity-claude-kit 1.0.0 installed; `com.unity.pipeline` 0.8.0-exp.1 installed. Verified: Editor opens and answers `eval`; kit preflight clean.
- 2026-10-24: **Movement and camera, first slice.** `Noted.Core.FirstPersonMath` (velocity approach, pitch clamp, yaw wrap), `PlayerMotor` (CharacterController in Update), `PlayerLook` (mouse and stick look, horizontal-FOV setting converted to Unity's vertical FOV), `Player` prefab, `Greybox` scene. Verified: compile clean; 15 of 15 Edit Mode tests pass (Editor console confirms); prefab references set, read back through the engine. **Not verified:** Play Mode feel (no human has played it), frame-rate independence of look, controller input, ultrawide aspect.
