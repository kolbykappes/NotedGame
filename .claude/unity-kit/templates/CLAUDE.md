# <Project>

<One line: what the game is, engine, platform.> This file is only the project entry point.

- **How we work with Unity and the Unity CLI** is a portable kit: `.claude/rules/unity-core.md` (loaded automatically),
  `.claude/rules/unity-csharp.md` (loads when you touch C#) and the `unity-editor-ops` skill (invoke it before any Editor write,
  recompile, test run or scripted scene/asset/prefab change). Kit layout and reuse: `.claude/unity-kit/README.md`.
- **Everything specific to <Project>** is in `.claude/project/`, imported below.
- Start with <docs index, if any>.

@.claude/project/profile.md
@.claude/project/workflow.md

Read `.claude/project/lessons.md` before Editor work: verified project-specific traps.
