# <Project> lessons: verified project-specific traps and triage

Project-specific; read before Editor work, alongside the kit's `rules.md`. Anything that would be true of any Unity project belongs
in the kit instead (a new numbered rule, or a cold-tier note). Annotate corrections in place with a date.

## Verified here

- 2026-10-24, Unity 6000.6.6f1, Pipeline 0.8.0-exp.1: a scene-and-prefab builder run with `unity command eval_file` returned "Main thread operation timed out after 5000ms" and exit 6, **but had completed** (scene and prefab on disk, scene open and clean, 10 root objects). Verified by listing files, `list_open_scenes` and reading the prefab references back through `eval`. Rule: after a timeout, inspect state (rule 26) before retrying; a retry would have been refused by the script's own existence check. Next time, pass the Editor-side timeout after `--` (rule 33) for builders like this.
- 2026-10-24: `unity projects new` failed on "not signed in" for two IDE packages until the owner signed in to the Unity Hub app (the CLI said signed in); the Hub then finished creating the project in the background. Rule: if creation fails at package resolution, sign in via the Hub, then check whether the project completed before retrying.
- 2026-10-24: the first `unity-ops.sh preflight` failed with "Editor not reachable" while the Editor was still importing the freshly installed Pipeline package (a 30 s main-thread timeout is logged in the console). Rule: run `unity status --until-ready` first after opening the Editor.

## Triage of the kit's cold-tier notes against this project

- For each `.claude/unity-kit/reference/unity-*.md` entry tagged `[IF ...]`, say whether it applies now, could apply, or not yet.
