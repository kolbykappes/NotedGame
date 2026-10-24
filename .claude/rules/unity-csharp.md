---
paths:
  - "Assets/**/*.cs"
---

# Unity C# conventions (portable kit)

Loads only when you touch project C#. Narrow the `paths` glob above to the project's own code folder if `Assets/` also holds
third-party code. Project specifics (assembly names, code folders, statics policy notes) are in `.claude/project/profile.md`.
Editor-driving rules are in `.claude/skills/unity-editor-ops/rules.md`.

- Follow the surrounding code: namespaces, formatting, folder layout, comment density, assembly definitions.
- Game rules and math stay in plain C# with no MonoBehaviour dependency, in their own assembly, with Edit Mode tests (rule 17).
  List every package an asmdef uses explicitly (rule 18).
- `UnityEditor` code lives in an Editor folder/assembly (or behind `#if UNITY_EDITOR`), never in player code.
- One attachable MonoBehaviour or ScriptableObject per file, file name equal to class name (rule 15).
- Before renaming a serialized field or type, find its usages in scenes, prefabs and assets, and keep compatibility with
  `[FormerlySerializedAs]` or `MovedFrom`. A rename that nulls references logs nothing.
- Prefer serialized references and the existing dependency patterns over `Find*`/`GetComponent` scans at runtime.
- Hot paths stay allocation-conscious: no per-frame LINQ, reflection, logging or scene searches without a stated reason. Do not
  claim a performance improvement without a measurement and its environment.
- Unsubscribe events, stop coroutines, cancel async work and release owned resources at the matching lifecycle point
  (`OnDisable`/`OnDestroy`).
- If the profile says domain/scene reload is disabled (rule 9): no mutable statics without a reset hook, and no static event
  handlers that accumulate across Play sessions.
- Keep reusable configuration (ScriptableObjects, data definitions) separate from mutable runtime state. Never write runtime
  counts or flags into a definition asset; in the Editor that writes through to the asset.
- No runtime code generation or reflection-dependent serialization without checking AOT and stripping implications first.
- Unity API calls stay on the main thread.
