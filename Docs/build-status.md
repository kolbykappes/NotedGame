# Build status

*Updated 2026-10-24 (evening, after the ten-job system).*

| Area | State |
|---|---|
| Project | Unity 6000.6.6f1, URP 17.6, Input System 1.20, Pipeline 0.8.0-exp.1; git on `main` |
| Movement and camera | Built; owner playtested the Greybox and said it feels fine |
| Core loop rules | Built in plain C#, 72 Edit Mode tests passing (jobs, unlocks, mini-games, oddities, planner) |
| Playable loop (`Facility` scene) | **Built and verified in Play Mode**: setup menu, intro, how-to cards, eleven jobs (hold, flip and mini-game steps), rooms that unlock, carry capacity, coins, the 60 s day, the pull, quiet time, complaint form, cot, the tool locker, two promotions |
| Sound | Placeholder procedural tones and hum; plays, **not heard by a human** |
| Verified by a human | **Nothing in the Facility scene yet** (feel, real mouse clicks, sound, pacing). See `Docs/playtest-guide.md` |
| Art, story, discovery | None (cubes; two promotion cards are the only "tape" content) |
| Save/load, settings, pause | Not built |
| Player build | Never built |
