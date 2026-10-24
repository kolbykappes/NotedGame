---
name: unity-editor-ops
description: Procedures for driving a running Unity Editor through the Unity CLI (preflight, save dirty scenes, compile, run tests, eval/teardown). Use before any Editor write, recompile, test run, or scripted scene/asset/prefab change. Enforces the numbered rules in rules.md beside this skill and the project profile.
---

# unity-editor-ops (warm tier)

Enforces the numbered rules in `rules.md` next to this file (read it first; numbers are stable). Also read the project profile (`.claude/project/profile.md`: paths, versions, protected files, helpers).
`.claude/rules/unity-core.md` holds the short always-loaded trap list. Why behind each rule: `.claude/unity-kit/reference/unity-*.md`.
Verified 2026-10-17 against Unity 6000.6.6f1, CLI 1.0.0-beta.13, `com.unity.pipeline` 0.8.0-exp.1. Pipeline is beta,
so re-check with `unity command --query <term> --detail full` before trusting a verb.

`unity-ops.sh` resolves the Unity project root itself (`UNITY_PROJECT_PATH`, else the nearest ancestor holding
`ProjectSettings/ProjectVersion.txt`) and prints `target=` on every run, so a worktree resolves to its own path (rules 21, 27). Helper: `bash .claude/skills/unity-editor-ops/unity-ops.sh <preflight|save|compile|test|play-begin|play-end|snap>`
(prints `[unity-ops] target=...` every run; exit 0 ok, 1 real failure, 2 refused, 3 unparseable, 4 no tests ran).

## What the CLI already gives you (don't hand-roll)

| Need | Use | Notes (observed) |
|---|---|---|
| Editor ready? | `unity status --until-ready --project-path P`, `unity command editor_status` | returns `status`, `compiling`, `domainReloadInProgress`, `playMode`, `unityVersion` |
| Dirty scenes | `list_open_scenes` / `save_all` | `scenes[].isDirty` |
| Console with cursor | `console_status` (counts + `groundTruth.compilationFailed`), `console --since <cursor> --since_session <id>` | rule 7: sample cursor before the action |
| Recompile + errors | `unity recompile --project-path P --json` | `data.status` `up_to_date`/`completed`, `data.errors[]`; exit 6/7 = infra/Safe Mode, not code failure |
| Tests | `unity command run_tests --mode editor|playmode|all [--filter X]` | see Procedure 4 |
| Code | `unity command eval "return ...;"` | needs `return`; params are flags (`--mode editor`), not `key=value` |

## Procedure 1 - Preflight (every session, before any write)
Run `unity-ops.sh preflight`. Must show `ready`, not compiling, `playMode=stopped`. Anything else: stop and fix/ask.
If it is playing or paused and you did not start it, it is the user's session (rule 25): stay read-only and ask. Never kill the Editor or delete a lock file to get a command through. If connection fails: check Safe Mode (compile errors) with `unity pipeline list`; window focus (rule 2); modal dialog (ask the user - you cannot see the screen).

## Procedure 2 - Verify loop (rule 1)
For any write: (a) sample `console_status` cursor, (b) act, (c) read evidence from a *different* channel than the write's own success return - the file on disk (`git diff`), the engine API (`eval`), or console entries after the cursor. "Success: true" is not evidence.

## Procedure 3 - Save before disturbing (rules 4, 5)
`unity-ops.sh save` before anything that stops Play Mode, switches scenes, recompiles, or switches branches. Refuses in Play Mode (writes there revert). Then check the file on disk.

## Procedure 4 - Compile + test (rules 17-20, 24)
`unity-ops.sh test [editor|playmode|all] [filter]` = compile (must be clean) -> `run_tests` on the **live Editor**.
- **Never use `unity test` while the Editor is open**: it spawns its own batch-mode Editor against the same project (rule 8). Use it only with the Editor closed (CI).
- **0/0 is its own outcome** (exit 4), not a pass. Observed today: `run_tests` returns `success:true`, `Total:0`, and `result:"playmode_running"` even with no tests; `test_status` then reads `completed` with zeros.
- `--mode all` runs EditMode synchronously and starts PlayMode async; the helper polls `test_status` (PlayMode path is via `--mode playmode`).
- **`run_tests` returns `success:true` even when tests fail.** Trust only `Summary.Failed`/`Total`, never `success`. Observed EditMode shape: `Summary{Total,Passed,Failed,...}`, `Results[]{FullName,Status("Passed"|"Failed"),Message,StackTrace}`. The helper prints only the failed entries. The PlayMode `test_status` shape (lowercase `summary`/`results`) is only partly observed (zero-test case).
- After a real recompile the Editor domain-reloads and commands fail with "Network error" for a few seconds. The helper's `compile` waits for `ready`; do the same in any hand-run sequence.
- Verified 2026-10-17 by breaking an assertion on purpose: helper exit 1 with the failing test, message and line; reverted, exit 0.
- Don't pass `--timeout` expecting it to be `run_tests`' own `timeout` param: `unity command` consumes `--timeout` itself (flag-collision trap, see `unity-tooling-editor-drivers.md`).
- Tests need their own `.asmdef` (+ explicit package refs, rule 18); game rules go in plain C# (rule 17).

## Procedure 4b - Eval limits and long work (rules 26, 33)
`eval` has a 5 s main-thread budget (ms on the Editor side; the CLI `--timeout` is seconds; put same-named Editor flags after `--`).
Anything longer or multi-line goes in `eval_file` / `run_script` with an iteration cap and a `Stopwatch` deadline, then poll. A timed-out
mutation may already have landed: inspect state before retrying, never blind-retry create/delete/modify.

## Procedure 5 - Scripted tools that instantiate things (rules 22, 23)
Put throwaway scripts in the scratch folder the profile names (default `tools/`, outside `Assets/`), never under `Assets/`. Any rig spawned into a scene is destroyed in a `finally` with `Object.DestroyImmediate`, then `list_open_scenes` shows the scene clean and `git diff --stat` shows only intended changes. A huge diff = suspect your own debris first; don't revert blindly.

```csharp
GameObject rig = null;
try { rig = (GameObject)UnityEngine.Object.Instantiate(prefab); /* measure */ }
finally { if (rig) UnityEngine.Object.DestroyImmediate(rig); }
```

## Procedure 6 - Play Mode proof with a background Editor (rules 3a, 35 to 38)
Only for a check that needs the running game. Start from a stopped Editor (rule 25).
**Shortcut: `unity-ops.sh play-begin`** does steps 1 to 3 (and records the project's Run In Background value), **`unity-ops.sh snap <name>`**
does the capture in step 4 (the PNG lands in `Temp/unity-ops-shots/`), and **`unity-ops.sh play-end`** does step 5 and fails if the
restored value or `ProjectSettings` differ. `play-end` refuses to stop a session that `play-begin` did not start. By hand, in order:
1. `unity command set_autotick`, then `unity command eval "UnityEngine.Application.runInBackground = true; return \"ok\";"`
   (use `editor_set_run_in_background` if `unity command --query background` lists it).
2. `unity command editor_play`, then `unity status --until-ready --project-path P`.
3. Prove frames move: `unity command wait_for --project-path P -- --condition '{"member":"UnityEngine.Time.frameCount","op":"changed"}' --timeout_s 10` must say `met: true`.
4. Read state through `eval`; look with `unity command capture_game_view --project-path P -- --source screen --save_path Screenshots/x.png`
   (written under `Assets/`; copy it out and delete the folder). Drive the game through its own methods, not key or mouse events (rule 37).
5. Before stopping: `eval "UnityEngine.Application.runInBackground = false;"`, `set_autotick -- --enable false` if you enabled it,
   `unity command editor_stop`, then confirm `git status` shows no settings change (rule 38).

## Never without explicit user approval
`ProjectSettings/`, `Packages/manifest.json`, input bindings, tags, layers, render pipeline, and anything else the profile protects (see `unity-core.md`). `package_add`/`package_remove` via the CLI count as manifest edits.
