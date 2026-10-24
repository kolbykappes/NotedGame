# Unity operating rules (numbered, portable kit)

Part of the portable Unity kit (see `.claude/unity-kit/README.md`). Project-specific values (paths, versions, protected files,
helpers, doc locations) are NOT here: they live in the project profile `.claude/project/profile.md`, and verified project-specific
traps in `.claude/project/lessons.md`. Read this file before any Editor write, recompile, test run, or scripted
scene/asset/prefab change (the `unity-editor-ops` skill sends you here), and read the profile too.

**Numbers are stable and append-only.** Docs, research notes and the skill cite them ("rule 9", "rule 3a", "rule 21"). Never
renumber. New rules go at the end (section 7). The *why* behind each rule lives in `.claude/unity-kit/reference/unity-*.md`.
Citation quirk inherited from the original: "rule 1" means the principle in section 1 (a success-looking return is not evidence);
the Editor-state item numbered 1 (read `isPlaying` with `Time.frameCount`) is cited here as "rule 2.1". Items 2 to 34 are cited by their own number.
Consolidated 2026-10-17 from an engineer's six-month operating notes on a single-player Unity 6 project.

## 0. How we drive the Editor

Official path: **Unity CLI** (beta) plus the **`com.unity.pipeline`** package, driven from the shell as `unity command <verb>`,
`unity command eval --code '<C#>'` or `unity command eval_file <path>`. The in-Editor MCP bridge is the deprecated path. The CLI has an MCP mode for harnesses that cannot run shell commands; we can, so we don't use it.

- Verified against (2026-10-17): Unity CLI 1.0.0-beta.13, Pipeline 0.8.0-exp.1 (installed with `unity pipeline install`, not Package
  Manager), Editor 6000.6.6f1. The CLI, Editor, Pipeline package and agent plugin are four separate version numbers; the project's
  actual versions are in the profile. Re-check behaviour whenever any of them changes.
- Pass `--project-path` on every command (rule 21). After a fresh Editor start or package change:
  `unity status --until-ready --project-path <project>`.
- Command names here describe capabilities, not guaranteed syntax; Pipeline is beta. Discover the live surface narrowly:
  `unity command --query <term> --project-path <project> --format json`, `unity commands --grep <pattern> --format json`.
  An empty search is not proof a capability is absent if the Editor was not connected. For the vendor's own interface
  description fetch `Unity-Technologies/skills` (`unity-cli`); it beats this file on interface questions, this file beats it on traps.
  The profile lists any CLI research notes the project keeps.
- First smoke test of any session: `unity command eval "return Application.unityVersion;"`.

## 1. The one rule that explains the rest

**A successful-looking return is not evidence.** Writes report success and change nothing. Status flags say "playing" with zero
frames advanced. Test runners print PASSED while discarding real failures. Before trusting any success return, number, or flag:
ask what it would look like if it were lying, and pay the one extra call that tells you.

## 2. Editor state

1. Never trust a descriptive flag. Read `Application.isPlaying` AND `Time.frameCount` together from inside the process, not a
   wrapper's own "is playing" status. A stopped Editor can still report a non-zero frame count, so neither alone is proof.
2. Unfocused Editor window = deferred compilation AND disabled keyboard/gamepad input
   (`BackgroundBehavior.ResetAndDisableNonBackgroundDevices`), but `InputSystem.devices` still reports every device `enabled=true`.
   First question on "the key does nothing": was the window focused?
3. Never set `backgroundBehavior = IgnoreFocus` to fix #2. Tried; it makes the Editor read chat-window typing as game input
   (a typed letter is a short tap; against a hold interaction it reads as "hold began, never completed").
3a. **Tested 2026-10-18 (Unity 6000.6.6f1, Pipeline 0.8.0-exp.1): `set_autotick` does NOT keep Play frames advancing while the
    Editor is unfocused**, whether minimised or merely covered, at 16 ms or 1 ms (the vendor docs say it should). Frames sit
    still, yet `status` says "playing". `editor_play`, `editor_stop` and `set_autotick` do not steal focus; **toggling
    `EditorApplication.isPaused` does** (it un-minimised the window). `EditorApplication.Step()` advanced exactly one frame while
    paused. `QueuePlayerLoopUpdate` and `SignalTick` did nothing. So: do Edit Mode work and synchronous checks (they run fine
    unfocused), render with an off-screen `Camera.Render`-to-texture helper (the profile names the project's), and batch frame-dependent checks (physics settling) for a
    moment when the user says the Editor may be in front. Never use `isPaused` or `editor_focus` unasked. A project focus probe, if any, is named in the profile.
4. **Nothing written during Play Mode persists except assets.** Scene edits and ProjectSettings changes report success, read back
   correctly, then revert the instant Play Mode stops. Stop Play Mode before any scene/settings write, and verify the **file on
   disk**, never the property read-back.
5. A modal dialog and a deadlock look identical from outside, and you can't see the screen. Save dirty scenes before anything that
   stops Play Mode, switches scenes, or recompiles; that prevents the modal that would otherwise wedge every subsequent command.
6. Never edit `.unity`/`.prefab`/`.asset` files on disk while the Editor has them open. It raises the "modified externally" modal
   (#5), and a save-dirty-scenes step can then write the Editor's older in-memory copy back over your edit. Go through the Editor
   API: set field, save scene, recompile. Exception: a low-judgment bool/number flip on an asset with no instance in the open
   scene is safe as a text edit.
7. Console persists across sessions and dedupes entries, so a fixed error can keep reading as current because it was never
   re-logged. Sample a cursor before triggering a check; if a check says FAILED with no errors listed, re-run once before
   concluding it's stale.
8. Don't run a second Unity instance in batch mode against an open project.
9. If "skip domain reload" (Enter Play Mode Options) is enabled, statics survive across Play sessions and only a recompile clears
   them. Suspect this first when static state looks wrong in a *fresh* play session; anything static that caches scene objects
   must prune itself on write. **The profile records whether this project has it on** (`DisableDomainReload` / `DisableSceneReload`). If on,
   don't add static state (registries, counters, caches) without a reset hook; the profile points at any project policy note.
10. Windows + `core.autocrlf=true`: a branch switch rewrites every tracked file's mtime even when content is identical (`git diff`
    is empty, but Unity watches mtimes, not content). Confirm no dirty scene and Play Mode off before switching branches in an
    open project folder.

## 3. Prefabs and assets

11. A loaded prefab's bounds include the root's baked offset; they are not prefab-local. `LoadPrefabContents` keeps the root's
    serialized position. Zero the root before measuring.
12. `arraySize` can read correct with every element null after a prefab apply/push. Verify by counting non-null elements, never by
    size alone.
13. Applying to a prefab can leave the *instance* holding duplicate components (seen: instance ends up with ~2x the
    joints/colliders of the prefab). Check added-component count is zero on the instance after an apply. If something
    physics-related looks "asleep" and won't wake up, suspect duplicate constraints before suspecting sleep state.
14. Never rebuild a referenced asset by delete+copy: it mints a new GUID and silently nulls every reference, with nothing logged.
    Mutate in place; diff the `.meta` file for a changed `guid:` line after any scripted asset write. Verify by reading the
    reference back through the runtime API, not off the asset.
15. A `ScriptableObject`/`MonoBehaviour` class name must exactly match its `.cs` filename, or asset creation writes a null script
    reference that *looks* fine until the next reimport (`m_Script: {fileID: 0}`). Pinning the script afterward cannot rescue a
    mismatched filename.
16. For any question about whether an asset exists or is referenced, ask the engine (load it, print it). A text/grep search misses
    GUIDs living in `.meta` files and will falsely report "never imported."

## 4. Testing

17. Keep game rules/math (matching, scoring, placement validation) in plain C# with zero MonoBehaviour dependency, covered
    by Edit Mode tests in their own assembly. Highest-leverage structural decision available; reserve
    Play Mode tests for what genuinely needs the engine running.
18. A custom `.asmdef` does not inherit auto-referenced packages. List every package your scripts use explicitly, or get a plain
    "type/namespace does not exist" compile error with no pointer to the real cause (asmdef).
19. The test runner does not recompile stale test sources first. Force a recompile and wait for it to settle before every test
    run; abort rather than report a result that isn't about your actual change.
20. Test runners return counts only; the failure detail is in the results XML (standard NUnit3, path announced by the runner,
    lives outside the project). Read the XML; don't bisect by re-running tests one at a time. Guard on the run's start time,
    since the file is overwritten in place.

## 5. Writing tools that drive the Editor

21. Pin the target Editor instance explicitly on every command and log which one you used. The CLI registers one endpoint per
    reachable Editor; an unpinned command can silently land on the wrong one and report success while the real project never
    changes. **The pinned path is the Unity project root of the checkout you are working in** (the profile records the primary
    path; a worktree is a different Unity project path with its own Editor, see rule 27).
22. Any tool that instantiates a preview/content rig into a scene must destroy it (immediate, not deferred) on every return path,
    including early exits. A deferred `Destroy` is not enough inside a single-frame script; the rig is still alive when the frame ends.
23. When a diff is absurdly large relative to what you intended, suspect your own tool's debris before believing the file got
    corrupted externally. Don't revert to "clean up"; you may be throwing away real work.
24. Result parsers fail closed: unknown shape is failure, never null/PASSED by default. Validate a parser against the real command
    output, not a hand-made sample.

## 6. Where things live

- Kit (portable, project-agnostic): `.claude/rules/unity-core.md` (always loaded), `.claude/rules/unity-csharp.md` (path-scoped),
  this skill (`SKILL.md` procedures, this file, `unity-ops.sh`), `.claude/unity-kit/reference/unity-*.md` (cold tier: measured numbers,
  failed attempts and war stories behind each rule; read on demand).
- Project (owned by the project, never copied between projects): `.claude/project/profile.md` (facts, protected paths, doc map),
  `.claude/project/workflow.md` (scope, backlog, doc duties), `.claude/project/lessons.md` (verified project-specific traps).
- Helper: `unity-ops.sh preflight|save|compile|test`. It leans on native CLI commands (`recompile`, `run_tests`,
  `list_open_scenes`, `save_all`, `console_status`); prefer the vendor's own solution over hand-rolling.
- **Tests with the Editor open: `unity command run_tests`, never `unity test`.** The latter spawns a second batch-mode Editor on
  the same project (rule 8). `run_tests` returns `success:true` with 0 tests; treat 0/0 as "no tests ran", not a pass (rules 19, 24).
- New trap discovered: fix it in a mechanism if you can (the skill), write it down if you can't (cold tier, or the project's
  lessons file if it is specific to this project), and only add to `unity-core.md` if it must hold on literally every session.
  Annotate corrections in place with a date rather than silently deleting the old version.

## 7. Additions (2026-10-24, merged from a generic Unity ruleset)

Editor ownership and safety
25. **A Play or paused session you did not start belongs to the user.** Keep inspection read-only until authorised. Do not stop,
    restart, pause, step, recompile, switch scenes or alter settings in it without approval. Start your own verification session
    only from a stopped Editor. Stop only sessions you started, and restore any temporary override when finished.
    Never kill an Editor, delete a lock file, or close the project just to make a command succeed.
26. A timed-out mutation may already have executed: inspect state before retrying, and never blindly retry commands that create,
    delete or modify objects. Do not discard or silently save unrelated dirty scenes, prefab stages or assets. Track temporary
    settings you change and restore them.
27. **Each git worktree is a distinct Unity project path** with its own Editor and its own `Library/`. Another worktree's Editor
    does not contain your changes. Never share or symlink `Library/` or `Temp/` between active worktrees. If no Editor is open
    on this checkout, you can still edit C# but you cannot claim compile or test results (say so).

Assets and scenes
28. Never manufacture GUIDs, fileIDs, component references or serialized structures. Preserve existing `.meta` files; move or
    rename assets through `AssetDatabase` (or move the matching `.meta` with them). Let Unity generate metadata for new assets
    and include those `.meta` files in the change. Preserve prefab connections, variants, overrides and importer settings.
29. Editor tooling uses `Undo`, `SerializedObject`/`SerializedProperty`, prefab-recording, dirty-state and save APIs, and saves
    only the scenes and assets the task intentionally changed. Generation and setup operations must be idempotent: re-running
    must not duplicate scene content. The project's sanctioned regeneration tools, if any, are named in the profile. Any other bulk scene
    rewrite, project reserialise, metadata regeneration, or deleting `Library/` needs approval and is never a routine fix.
    Do not hand-edit generated `.csproj`/`.sln` files or package-cache contents. Verify persisted asset changes after
    import or reload (rule 1).

Testing and builds
30. Run targeted tests while iterating; run the full applicable suites on the final code before handoff, or report what prevented
    it. Keep separate results for full, filtered and retry runs; a passing retry is not a passing full suite.
31. Build the actual player when a change affects platform APIs, shaders, stripping, serialization, plugins or build
    configuration; an Editor playtest does not prove an IL2CPP or platform build. Use the project's existing build methods
    and never silently switch platform. Keep temporary reports outside the source tree.

Play-mode and visual verification
32. Prove frames advance before trusting a screenshot (rule 1; Play Mode is also limited by rule 3a). Choose the capture source
    explicitly: a camera-only capture omits Screen Space Overlay UI. Read Console errors as well as looking at the image.
    A frozen, single-colour or locked-desktop capture is inconclusive, not a pass. Do not change shipping Player Settings
    just to keep a playtest ticking. Runtime tuning is temporary unless deliberately written to the right asset or code.

CLI behaviour (sources: Unity CLI docs, Pipeline changelog and the Unity-Technologies/skills repo, as of CLI beta.13)
33. `eval` defaults to a 5 s main-thread budget, in **milliseconds** on the Editor side, while the CLI's own `--timeout` is in
    **seconds**. Same-named Editor-side flags go after a `--` fence. Long work goes in `eval_file`/`run_script` with bounded
    loops (iteration cap plus a `Stopwatch` deadline) and polling; a timed-out synchronous eval is not documented as aborted.
    `unity recompile`: exit 0 ok, 6 compile errors, 7 Editor unreachable (compilation unverified, not a source error). Read
    the structured error before assuming a nonzero exit is a code failure. Safe Mode (compile errors) stops Pipeline loading:
    use `unity status` and the Editor log. Branch on `success`/exit code and `--format json` fields, not readable text.
34. Do not install or upgrade the CLI, packages, plugins or Editors without approval. `package_add`/`package_remove` count as
    manifest edits. Never use automatic project-upgrade consent flags unless the user explicitly authorises the upgrade.
