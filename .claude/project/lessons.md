# <Project> lessons: verified project-specific traps and triage

Project-specific; read before Editor work, alongside the kit's `rules.md`. Anything that would be true of any Unity project belongs
in the kit instead (a new numbered rule, or a cold-tier note). Annotate corrections in place with a date.

## Verified here

- 2026-10-24, Unity 6000.6.6f1, Pipeline 0.8.0-exp.1: a scene-and-prefab builder run with `unity command eval_file` returned "Main thread operation timed out after 5000ms" and exit 6, **but had completed** (scene and prefab on disk, scene open and clean, 10 root objects). Verified by listing files, `list_open_scenes` and reading the prefab references back through `eval`. Rule: after a timeout, inspect state (rule 26) before retrying; a retry would have been refused by the script's own existence check. Next time, pass the Editor-side timeout after `--` (rule 33) for builders like this.
- 2026-10-24: `unity projects new` failed on "not signed in" for two IDE packages until the owner signed in to the Unity Hub app (the CLI said signed in); the Hub then finished creating the project in the background. Rule: if creation fails at package resolution, sign in via the Hub, then check whether the project completed before retrying.
- 2026-10-24: the first `unity-ops.sh preflight` failed with "Editor not reachable" while the Editor was still importing the freshly installed Pipeline package (a 30 s main-thread timeout is logged in the console). Rule: run `unity status --until-ready` first after opening the Editor.

## Triage of the kit's cold-tier notes against this project

- For each `.claude/unity-kit/reference/unity-*.md` entry tagged `[IF ...]`, say whether it applies now, could apply, or not yet.

## More verified traps (2026-10-24, building the MVP)

- **Editor-side timeout for eval.** `unity command eval ... -- --timeout 120000` (the fence) raises the Editor's 5 s main-thread budget; without it a scene builder reports a timeout even though it finished. Always inspect state after a timeout (rule 26).
- **Play Mode frames did advance** in this session (Editor window visible): frame counts rose by about 300 per second, so simulated-input tests work without `isPaused` or any focus tricks. Check frame counts before and after (rule 2.1). `editor_stop` is only for sessions you started (rule 25); scripts recompile after leaving Play Mode.
- **Simulated keyboard works, simulated mouse does not.** `InputSystem.QueueStateEvent` on `Keyboard.current` drives the real actions (hold E, Space, Enter). A simulated mouse is ignored by the UI module because the editor setting `PointersAndKeyboardsRespectGameViewFocus` needs the pointer physically over the Game view. Test UI with the raycaster and `ExecuteEvents` (proves the handlers) and ask a human for real clicks.
- **Child `localScale` inherits the parent's scale.** A "world size" given to a child of a scaled cube comes out multiplied. The builder now divides by the parent's lossy scale (`SetWorldScale`).
- **Legacy `TextMesh` with the built-in font ignores depth** (signs show through walls) and a custom shader gets no font atlas. Signs are World Space canvases with a uGUI `Text` instead.
- **`FindFirstObjectByType` is obsolete** in this Unity (use `FindAnyObjectByType`). In eval strings, C# strings need double quotes; single quotes are char literals.
- **A debug helper that ticks the clock to zero swallows the one-time "expired" signal.** Debug hooks that skip time must call the same transition the real timer does (`DebugExpireClock` now starts the pull directly).
- **Pillow ignores embedded colour profiles** by default; convert with `ImageCms` before sampling colours.
- Reflection into private fields (for example setting `PlayerLook.pitch`) is fine in throwaway test scripts, but nothing in the game relies on it.

## Scratch scripts (tools/, not part of the game)
`tools/build_greybox.cs` (the first test room), `tools/capture_views.cs` (eye-level captures of the Facility scene), `tools/probe.cs` (one-line state report in Play Mode), `tools/play_helpers.ps1` and `tools/play_full.ps1` (drive the running game from PowerShell: probe, simulate keys, snapshot, play several days). Output goes to `tools/out/` (ignored by git).
