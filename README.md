# Noted: I Understand Your Frustration

A first-person Unity game (working title) about the only operator in a new AI data center run by a cheerful corporate cult. You do small, mundane jobs against a clock, get promoted, and slowly learn what the jobs really are. Work in progress; nothing here is a release.

## What is in this repository

| Path | What it is |
|---|---|
| `Assets/`, `Packages/`, `ProjectSettings/` | The Unity 6 project (6000.6.6f1, URP). Game rules are plain C# in `Assets/Scripts/Core` with Edit Mode tests in `Assets/Tests`. Open `Assets/Scenes/Facility.unity`. |
| `Docs/` | Build documents for the game: `playtest-guide.md` (start here to play the current build), `feel-test-plan.md`, `backlog.md`, `changelog.md`, `build-status.md`, `mvp-spec.md`. |
| `ClickOne/` | Design and research. `ClickOne/Docs/concept/` holds the story and core-loop plan (`sabotage-spine.md`) and earlier concepts. `ClickOne/Docs/competitive/` holds research on other games and a scan of horror and uncanny games and shows for ideas. |
| `.claude/`, `CLAUDE.md` | Working notes and rules for the AI-assisted workflow used on this project (a Unity CLI kit and project profile). |

## Playing the current build

Open the project in Unity 6000.6.6f1 and press Play in `Assets/Scenes/Facility.unity`. See `Docs/playtest-guide.md` for the controls and the feel-test keys (F6 jumps between levels).

## Notes on the research folder

`ClickOne/` is working research, kept here so it is backed up with the game. It quotes short excerpts from public player reviews and community threads, and it includes small store screenshots of other developers' games that were saved for study only. Those screenshots belong to their respective developers; if you are one of them and want an image removed, open an issue and it will be taken out.

## Disclosure

Art and voices for the game will be human-made. Some turns of phrase in the game's text were written with AI assistance, and the design and build were developed with the help of Claude (Anthropic).
