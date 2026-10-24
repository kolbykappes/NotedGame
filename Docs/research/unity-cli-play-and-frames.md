# Unity CLI: can an agent see Play Mode frames, capture the game, and send input?

*2026-10-24. Question from the owner: "research some of your assumptions about how the Unity CLI and engine work, about your inability to see frames move." Method: read the CLI's own agent guide (`unity skill show`, including `references/playmode-verification-loop.md`), listed the commands the connected Editor exposes (`unity command --query <term>`), and tested each claim on this project (Unity 6000.6.6f1, CLI 1.0.0-beta.13, `com.unity.pipeline` 0.8.0-exp.1) with the Editor window in the background and the owner working in another app.*

## What I had assumed (from the kit, rule 3a, and my own first test run) and what is true

| Assumption | Verdict | Evidence |
|---|---|---|
| "An unfocused Editor does not advance Play frames." | **True by default** | Editor in the background, Run In Background off: `isPlaying=True` but `Time.frameCount` stayed at 2; `wait_for` on `frameCount` `changed` timed out (`framesObserved: 52`, `met: false`). |
| "`set_autotick` does not fix it." | **True alone, but incomplete** | `set_autotick` alone: frames stayed frozen (matches the kit's note). The CLI's own guide says a *second* switch is needed. |
| (not known) the second switch | **Fixes it** | `set_autotick` **plus** `Application.runInBackground = true`: frame 100 → 889 in two seconds, with the Editor still in the background. `wait_for` then reports `met: true` within about 110 ms. |
| "I can only render off-screen; I cannot see the real Game view." | **Wrong** | `capture_game_view` (`source=screen`, the default) captures what the Game window shows, **including Screen Space - Overlay UI**, and works with the Editor in the background. `screenshot` also exists but captures the camera only (no overlay UI). |
| "I cannot simulate input." | **Mostly wrong, with one real limit** | The package has `simulate_key` and `simulate_pointer` (screen coordinates). But while the Editor is not the active window the Input System sets `backgroundBehavior = ResetAndDisableNonBackgroundDevices`, which disables the keyboard and mouse devices, so simulated device events are dropped. A simulated keyboard worked earlier tonight only because the Editor was in front. |
| "Setting `runInBackground` is harmless." | **It edits project state in memory** | Setting `Application.runInBackground = true` made `PlayerSettings.runInBackground` read `True` as well. Restore it before leaving Play Mode (I did; `git status` stayed clean). |

## Why the first test run looked different
Earlier overnight, frames advanced without any of these switches because the Editor window was the active application (`Application.isFocused` was `True`). This afternoon the Editor was in the background (`InternalEditorUtility.isApplicationActive` was `False`), and the freeze appeared. So the behaviour depends on whether the Editor window is the active app; the rule in the kit was true but incomplete, and my earlier statement "I cannot see frames" was an over-generalisation.

## Commands that exist in this package (confirmed with `unity command --query`)
`set_autotick`, `editor_focus`, `editor_pause`, `wait_for` / `wait_status` / `wait_cancel`, `capture_game_view`, `capture_scene_view`, `screenshot`, `simulate_key`, `simulate_pointer`, `console` / `console_status` / `clear_console` / `log`, `set_target_framerate`, `get_performance_stats`, `eval` / `eval_file` / `run_script`, `batch`, `build`.
**Not in this package version:** `editor_set_run_in_background` (the guide mentions it for newer versions; here use `eval "UnityEngine.Application.runInBackground = true;"` and restore it afterwards). `unity status` does not report `frameCount` or `playerLoopTicking` on this version.

## What I could not do, and why
- **Real mouse clicks on UI while the Editor is in the background.** `simulate_pointer` returns success but the game does not receive it (devices are disabled). The only workaround is `backgroundBehavior = IgnoreFocus`, which would make the Editor read the owner's real typing in other windows as game input (kit rule 3), so I did not use it. UI handlers were verified by calling them and by raycasting under the pointer; a real click needs a human or an active Editor window.
- `editor_focus` would bring the Editor to the front (and enable devices), but it steals focus from whatever the owner is doing, so it is only appropriate when they are away or ask for it.

## The recipe I now use (and recommend adding to the kit)
1. `unity command editor_play`, then `unity status --until-ready`.
2. `unity command set_autotick` and `unity command eval "UnityEngine.Application.runInBackground = true; return \"ok\";"`.
3. Prove frames advance: `unity command wait_for -- --condition '{"member":"UnityEngine.Time.frameCount","op":"changed"}' --timeout_s 10` (must say `met: true`).
4. Look at the real screen: `unity command capture_game_view -- --source screen --width 1600 --height 900 --save_path Screenshots/name.png` (it writes under `Assets/`; copy the PNG out and delete the folder, because `..` paths are rejected).
5. Drive the game through its own methods (or `eval`), not through key and mouse events, unless the Editor window is active.
6. Slow time for hard-to-time moments: `eval "UnityEngine.Time.timeScale = 0.2f;"`, capture, then restore it.
7. Before `editor_stop`: restore `runInBackground` to `false` and turn off auto-tick; confirm `git status` shows no `ProjectSettings` change.
Helpers: `tools/play_helpers.ps1` (`PlayBegin`, `PlayEnd`, `Snap`, `HoldFor`, `Place`, `Target`).

## Proposed edits to the portable kit (not applied; the kit is the owner's)
- **Rule 3a:** replace "set_autotick does NOT keep Play frames advancing" with: "frames freeze when the Editor is not the active window and Run In Background is off; `set_autotick` alone does not fix it, `set_autotick` plus `Application.runInBackground = true` does (verified 2026-10-24, Pipeline 0.8.0-exp.1); prove it with `wait_for` on `Time.frameCount`; restore `runInBackground` afterwards."
- **Rule 32 / the helper:** point to `capture_game_view` (source `screen`) as the primary Play Mode capture and note that it includes overlay UI.
- **New rule:** keyboard and mouse devices are disabled while the Editor is in the background; simulated input is dropped; do not use `IgnoreFocus` to work around it.
- `unity-ops.sh` could gain a `play-begin` / `play-end` pair doing steps 1 to 3 and 7.
