# Unity kit: portable working rules for Claude Code + Unity + the Unity CLI

Kit version: see `VERSION` beside this file. Project-agnostic: it contains no project names, paths or scope. Verified against Unity 6000.6.6f1,
Unity CLI 1.0.0-beta.13 and `com.unity.pipeline` 0.8.0-exp.1; the CLI is beta, so re-check behaviour when versions change.

## Two layers

| Layer | Owner | Files |
|---|---|---|
| **Kit** (copy between projects) | the kit | `.claude/rules/unity-core.md`, `.claude/rules/unity-csharp.md`, `.claude/skills/unity-editor-ops/` (`SKILL.md`, `rules.md`, `unity-ops.sh`), `.claude/unity-kit/` (this README, `reference/`, `templates/`) |
| **Project** (never copied) | the project | `CLAUDE.md` (thin entry point), `.claude/project/profile.md`, `.claude/project/workflow.md`, `.claude/project/lessons.md`, plus the project's own docs |

How they connect:

- `CLAUDE.md` imports the project profile and workflow (always loaded) and mentions the kit.
- `unity-core.md` loads every session. It never names a project; wherever it needs a project value it says "the profile".
- `unity-csharp.md` is path-scoped (`Assets/**/*.cs`), so it only costs context when C# is touched.
- The `unity-editor-ops` skill is invoked before Editor operations; it sends you to `rules.md` (numbered, stable rules) and the profile.
- `reference/unity-*.md` is the cold tier: measured numbers and war stories behind the rules, read on demand. Entries tagged
  `[IF ...]` apply only to some projects; the project records its own triage in `lessons.md`.

## What the kit expects from the project profile

The profile answers these (see `templates/profile.md`): Unity project root and primary path; code, Editor-tooling and scratch-script
folders; assembly definitions; versions; domain/scene reload settings (rule 9); extra protected paths and sanctioned regeneration
tools (rule 29); off-screen render helper and focus probe (rule 3a); the research folder (the "research before craft" rule); and
the doc map. The workflow file answers: scope and non-goals, research and backlog duties, which docs must be updated with a change.

## Using the kit in a new project

The kit's source is a standalone repo (`unity-claude-kit`, containing `install.sh` and a `kit/` folder that mirrors `.claude/`). Point
`UNITY_KIT` at your clone and run the installer against the Unity project root:

```bash
bash "$UNITY_KIT/install.sh" install <unity-project-root> --dry-run   # see what would change
bash "$UNITY_KIT/install.sh" install <unity-project-root>             # copy kit files; seed project files from templates if missing
bash "$UNITY_KIT/install.sh" check   <unity-project-root>             # later: is the project still identical to the kit?
```

`install` never overwrites project-owned files (`CLAUDE.md`, `.claude/project/*`); it seeds them from `templates/` only when they are
missing. If the project already has a `CLAUDE.md`, add the kit bullet and the two import lines from `templates/CLAUDE.md` by hand.

Then fill in `.claude/project/profile.md` and `workflow.md`, narrow the `paths` glob in `unity-csharp.md` if `Assets/` holds
third-party code, and run `bash .claude/skills/unity-editor-ops/unity-ops.sh preflight` with the Editor open
(`unity-ops.sh` finds the Unity project root itself; it needs `node` and the `unity` CLI on PATH).

## Keeping the kit and the projects in step

The kit repo is the source of truth. Do not hand-edit installed kit files and leave it at that.

- **Improve the kit from inside a project:** edit the kit file in the project, then `install.sh pull <project>` copies the edited files
  back into the kit repo. Review `git diff` there, run `install.sh lint <project terms>`, commit, then `install.sh install <other project> --force`
  where you want the improvement.
- **Upgrade a project to a newer kit:** `install.sh check <project>`, then `install.sh install <project> --force`. Without `--force`,
  install refuses to overwrite any kit file that differs in the project, so local edits are never lost silently.
- A new trap that is true of any Unity project: add a numbered rule at the end of `rules.md` (numbers are append-only because docs
  cite them), or a cold-tier note.
- A trap that is only true of one project goes in that project's `lessons.md`, never in the kit.
- Never put project names, absolute paths or the user's name in a kit file. `install.sh lint <project name> <user name> <drive-letter path prefix>` must stay clean.
- Bump `VERSION` for any change other projects should pick up.
