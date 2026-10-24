# <Project> project profile

The one place the portable Unity kit looks for project facts. Keep it short and factual; update it when a fact changes.
Delete any section that does not apply, but do not leave a kit rule's question unanswered silently: write "none".

## Identity

- One line: what the game is, target platform(s), how many Editors are normally open.
- Working scenes and what builds them (if a scene builder or generator exists).
- Index of the project brief or design docs, if any.

## Versions (re-check if stale)

Unity <6000.x.y>, <render pipeline + version>, <input system>, <UI framework>, <other key packages>, Unity CLI <version>,
`com.unity.pipeline` <version>.

## Paths and layout

- Unity project root (relative to the git root, and the primary checkout's absolute path). Worktrees resolve to their own path.
- Project code folder, Editor tooling folder, scratch-script folder (outside `Assets/`; kit default `tools/`).
- Assembly definitions: names and what each holds (rules assembly, test assemblies). Where code without an asmdef compiles.
- Imported third-party content that must stay untouched.

## Editor settings that matter

- Enter Play Mode options: domain reload on/off, scene reload on/off (kit rule 9 depends on this).
- Anything else a kit rule asks about (focus behaviour on this Editor version, quality settings that affect captures).

## Protected (approval and a stated plan first), beyond the kit defaults

The kit already protects `ProjectSettings/`, `Packages/manifest.json`, input bindings, tags, layers and render-pipeline settings.
List anything extra, and any sanctioned bulk-regeneration tools (kit rule 29).

## Helpers

- Off-screen render helper for visual checks without Play frames (kit rule 3a), focus probe, measurement scripts: name and path.

## Documentation map

| What | Where |
|---|---|
| Index | |
| Build status | |
| Backlog / changelog | |
| **Research folder** (kit "research before craft") | |
| CLI / Pipeline research notes, if any | |

## Project files in this folder

- `workflow.md`: scope, backlog and changelog duties, other doc duties.
- `lessons.md`: verified project-specific traps; project triage of the kit's cold-tier notes.
