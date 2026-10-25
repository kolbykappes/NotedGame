# Noted workflow, scope and documentation duties

The kit's "definition of done" includes the project's documentation duties. They live here.

## Scope guardrails

- **In scope now:** a vertical slice of the core loop with placeholder art: wake on the cot, a timed day with a short list of tasks, the pull to a console to enter a code, a canned reply to a preset complaint, a night shop, and a rank-up. Goal: find out whether cycles 1 to 30 are fun with no story.
- **Out of scope until the loop is proven fun:** story and lore content, tapes, voice, final art, save/load, localization, Steam integration, controller polish.
- **Constraints from the owner:** small content and art budget (a few reusable zone kits, reskinned task types); no growing hole; the complaint terminal uses presets only (no free text); first-person movement and camera must be comfortable (short, skippable camera takeovers; hard cuts, not swings); about 5 to 6 hours of finished content later, mostly mundane tasks.
- **Needs a plan before building:** save/load, any package install, any ProjectSettings change.
- **Slice order:** movement and camera, one task type, the day timer and the pull, the code entry, the reply, the shop, then more task types.

## Research duty (the kit's "research before craft")

- Research folder: `Docs/research/`, one file per topic, indexed in `Docs/research/README.md` (create both when first needed). Each file: the question, sources with links, the decision, and anything unsourced marked as a guess.

## Backlog and changelog

- Backlog: `Docs/backlog.md` (create when first needed): one line per item with status. Changelog: `Docs/changelog.md`: one line per completed slice (date, what changed, what was verified).
- Keep `Docs/build-status.md` current when a slice lands.

## Other documentation duties

- When the design changes, update `ClickOne/Docs/concept/sabotage-spine.md` (and `hold-the-line.md` if the earlier concept is affected) in the same session.
