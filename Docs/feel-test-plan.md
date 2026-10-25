# Feel-test plan: jump to a mid-game level and test the second list

*Written 2026-10-25. **Status: slices 0 to 4 built and checked the same night (116 Edit Mode tests; Play Mode with simulated input); slice 5 not built.** Where the build differs from this plan: F6 is the level jump (PageUp/PageDown were dropped); the message preview is four buttons on the F1 overlay; Specialist is 5 jobs in 110 s, not 4 in 90 s (the measured pace); the Alternate prompt shows whenever an errand step is available at a station (there is no "scrap read" gate). How to play it: `Docs/playtest-guide.md`. The text below is the original plan. Design source: `C:\Code\ClickOne\Docs\concept\sabotage-spine.md` (core loop and story) and the research in `C:\Code\ClickOne\Docs\competitive\creepiness-toolkit-2026-10-25.md`. Tracks in `Docs/backlog.md` (section "Feel test").*

## Goal
Let the owner start the game at a chosen level and play several days of it, to judge **pacing** (is there slack to spare once the official list is done?), **messages** (do the re-read job names and scraps land?) and **the second list** (do errands fit the slack and feel like a heist inside a shift?). No stealth yet (slice 5, deferred).

## Scope
| Slice | What | Size | Depends on |
|---|---|---|---|
| **0** | Jump harness: start profile, level selector, replay a day, day log | S to M | none |
| **1** | Level table: day length, task count, thresholds, reveal stage per level | M | 0 |
| **2** | Reliability (gates promotion) and Rollout (par, floor, cap) with a wall sign and HUD line | M | 1 |
| **3** | Reveal and messages: stage-based job text, scraps, ledger slip, oddities, flat reply tier, Wellness Moment card text | S to M | 1 |
| **4** | The second list without stealth: hidden errands, Alternate key, scrap card, five errands on existing stations | M | 1, 2 |
| 5 | **Deferred:** stealth (camera eyes, restricted zones, audits, Attention, pull exfil, caught consequences), hacking via reused mini-games | M to L | 0 to 4 |

Suggested order: 0, 1, 3, 2, 4 (messages are the cheapest way to test the dissonance, so do them before the Rollout maths). Level indexing: code keeps the existing 0-based `Rank`; the UI and debug overlay say "Level N" (Rank + 1).

## Slice 0: jump harness
| ID | Requirement | Acceptance |
|---|---|---|
| R0.1 | `StartProfile` (Core): Level, Day, Coins, AllTools, AllRoomsOpen, MarkAllJobsSeen, Reliability, Rollout, Seed. | Edit Mode test builds a session from a profile and reads the values back. |
| R0.2 | `GameSession.ApplyStartProfile`; `RankTracker.SetRank(rank)` sets `CompletedTasks` to the matching threshold. `Day` is set so that `BeginDay` lands on the requested day. | Test: profile at Level 4, Day 30 gives rank 3, day 30, rooms open. |
| R0.3 | `GameFlow` gets a serialized `debugStartLevel` (0 = normal game) and `debugStartDay`; it applies the profile before the intro and can skip intro and how-to cards. | Setting the field and pressing Play starts at that level with no cards. |
| R0.4 | Debug overlay (Editor and development builds only, `#if UNITY_EDITOR \|\| DEVELOPMENT_BUILD`), toggle F1: level, day, clock, reliability, rollout, errand status. Keys read via `Keyboard.current` (as `PullSequence` does), **no change to input bindings or ProjectSettings**: PageUp/PageDown change level (applies at next day), F2 replay day, F3 skip to next day, F4 grant all tools and coins, F5 end the day now (reuse `DebugExpireClock`), F6 preview next message. | Each key works in a Play run; none changes a binding. |
| R0.5 | Replay day: `GameSession` snapshots at `BeginDay` (wallet, ledger levels, rank, job counts, unlocked rooms, seen jobs, reliability, rollout, day) and can restore it; the same seed yields the same list. | Round-trip test: snapshot, play, restore, state equal. |
| R0.6 | `DayStats` computed in `EndDay`; a `DayLog` writes one CSV row per day to `Application.persistentDataPath/daylog.csv`: level, day length, seconds to finish the official list (blank if not finished), slack left, tasks done/total, errands done, reliability, rollout, coins. | A 5-day run produces 5 rows with sensible numbers. |

## Slice 1: level table
| ID | Requirement | Acceptance |
|---|---|---|
| R1.1 | `LevelDef` and `LevelTable` (Core): title, day seconds, task count, completed-tasks threshold, reveal stage (0 to 3), par rollout per day, floor, errand ids, rank-up tape text. | Table lookups tested for every level. |
| R1.2 | First-guess test table (to tune with the CSV): L1 Trainee 60 s / 2 tasks; L2 Associate 75 / 3; L3 Specialist 90 / 4; L4 Technician 110 / 5; L5 Steward 130 / 5 or 6. Counts are walker-just-finishes guesses from the measured pacing (Trainee walking 52 to 62 s; Specialist list about 73 s sprinting). | The CSV shows a walker barely finishing and a tooled sprinter with 35 to 50% slack, or the table is retuned. |
| R1.3 | Replace the hard-coded `TasksPerRank`, `RankThresholds`, `RankTitles` and `DayLengthSeconds` in `GameConfig` with table-backed members; `GameSession.BeginDay` and `DayPlanner.Plan` read from them. Keep the old members as computed properties so existing tests keep working, or update them with a changelog note. | The 72 existing tests pass (or are updated deliberately). |
| R1.4 | `CardText.RankUp` becomes data-driven for any level (it hard-codes ranks 1 and 2 now). | Promoting to Level 4 shows its tape text. |
| R1.5 | Levels 3 to 5 reuse the eight existing rooms (for example the hall stands in for the operations floor). No new geometry. | Nothing in `FacilityBuilder` changes for this slice. |

## Slice 2: Reliability and Rollout
| ID | Requirement | Acceptance |
|---|---|---|
| R2.1 | `Reliability` (Core), 0 to 100, starts at 100: each unfinished official task at day end subtracts a first-guess 8; a fully finished day adds 2; automated tasks count as done. | Tests for clamps and both directions. |
| R2.2 | Promotion needs Reliability of 80 or more as well as the task count. If the count is met but Reliability is not, the promotion is held and the quiet-time summary says so. | Test: threshold met at Reliability 70 holds; met at 85 promotes. |
| R2.3 | `Rollout` (Core), 0 to 99: end-of-day change is the level's par minus completed-errand effects; a floor that rises with each promotion; a hard cap of 99 until the final act; rubber-band hooks as flags only (not implemented). | Test: a 100-day simulation of three player types matches the paper numbers in the spine doc (pure worker near 93 to 95% around day 90; casual near 50%; committed pinned at the floor). |
| R2.4 | HUD line "ROLLOUT n%" and "RELIABILITY n%"; a world-space sign in the admin room updated at day end. The sign is built in `FacilityBuilder` (follow the sign-orientation rule in `lessons.md`). | Visible in a Play capture and readable from the cot. |

## Slice 3: reveal and messages
| ID | Requirement | Acceptance |
|---|---|---|
| R3.1 | `RevealStage` per level (0 clean, 1 first scrap, 2 sinister nouns, 3 harm numbers). `JobText` table keyed by (job, stage): display name, how-to addendum, completion toast, flat statement line. All 11 jobs by stages 0 to 3 (about 44 lines, placeholder copy). | Every job reads differently at each stage; no missing keys (test). |
| R3.2 | The HUD task list, step prompts and toasts take their wording from `JobText`. An "UPDATED PROCEDURE" card (flat voice) appears the first time a job's stage changes. | At Level 3 vs Level 1 the same job shows different wording. |
| R3.3 | Scraps bank (about 12): keyed by level, stage and trigger ("after the mail job N times"). Delivered as a "SCRAP FOUND" card using `CardScreen`; the debug overlay lists those found. | A scrap appears at its trigger exactly once. |
| R3.4 | Morning ledger slip (the town): a card at day start with flat lines derived from yesterday's Rollout change and errand outcomes ("Zone 4, residential: n households restored/denied"). Before errands exist, derive from the Rollout change. | Card text changes when Rollout moves. |
| R3.5 | Complaint replies: add a flat tier chosen by stage; the existing three cloying tiers remain for earlier stages. | At stage 3 the reply is short and flat. |
| R3.6 | Oddity bank: re-tag existing oddities by stage and add about 8 stage-2 and stage-3 oddities (approvals under your badge, a log gap). Keep the total at or below 30. | `OddityBank.Pick` respects stage. |
| R3.7 | Wellness Moment card text (three tapes, with an abridged variant for repeats), previewable via F6. Triggered in slice 5. | Previewable from the overlay. |
| R3.8 | Copy checklist (from the research): plain statement with impossible content; no adjective that tells the reader to be afraid; the same flat voice for the right and the wrong item; consequences that arrive a day late; the town's names recur. | Every new line is checked against the list in review. |

## Slice 4: the second list (no stealth)
| ID | Requirement | Acceptance |
|---|---|---|
| R4.1 | `ErrandDefinition` and `ErrandCatalog` (Core): id, name, min level, briefing text, steps (reuse `TaskStep`: station, hold, requires/gives carry, optional wait), Rollout effect. Errand steps are **side actions**; they never alter official job state. | Tests for catalogue integrity and step sequencing. |
| R4.2 | `GameSession`: `TodaysErrands` (0 to 2 by level), `TryGetAlternateStep(stationId)`, `CompleteAlternate(stationId)`. An unfinished errand at day end fails quietly (no Reliability cost). Completed errands apply their Rollout effect at day end. | Tests: sequencing, carry tokens, failure at day end, effect applied once. |
| R4.3 | `ErrandPlanner.Plan(level, seed, seen)`: deterministic; prefers errands whose stations are open; at most 1 at Level 3, up to 2 from Level 4. | Same seed gives the same errands. |
| R4.4 | Alternate input: key **F**, read by `Interactor` via `Keyboard.current` (no binding change; **needs owner approval**). `TaskStation` gets `TryGetAlternatePrompt`; the second prompt line shows only after the day's scrap has been read. | At a station with an active errand step, F starts the hold. |
| R4.5 | Scrap card at day start (`CardScreen`), re-readable during the day by holding **Tab** (overlay panel, not on the HUD list). The prop version (scrap in the world) comes later. | Holding Tab shows today's briefing and hides on release. |
| R4.6 | New carry tokens ("blank", "appeal", "copy") and their names in `GameFlow.CarryName`. | Carry text reads correctly. |
| R4.7 | Five starter errands on existing stations (first-draft copy): **Blank the notices** (L3; paper stack to printer, blank stock; -1.0); **Jam the breakers** (L3; corridor then admin within 20 s; -0.6); **Slip the appeal out** (L3; shredder to mail table; -0.7); **Prop the vent** (L4; vent, 3 s; -0.8); **Copy the log** (L5; clipboard to patch panel; -1.2). | Each is completable on a fresh level-start profile. |
| R4.8 | The day log records errands offered, started and finished. | CSV columns present. |

## Cross-cutting requirements
- **Tests:** all rules in `Noted.Core` with Edit Mode tests (aim for 40 or more new). Existing 72 must pass or be updated on purpose.
- **Scene:** only the Rollout sign changes `FacilityBuilder` in this plan. Any scene work goes through the builder (it regenerates `Facility.unity`). After code edits: recompile, poll, rebuild, read one value back (lessons.md).
- **Process rules:** show the file list before editing; finish all code edits before Play Mode (a domain reload wipes state); do not touch ProjectSettings or input bindings; no mutable statics without a reset hook; fictional names only.
- **Verification boundary:** Claude verifies logic by tests and Play Mode with debug methods. Feel, real mouse clicks, stealth tension and sound need the owner (the existing playtest limits still apply).
- **Definition of done per slice** (workflow.md): backlog, changelog, build-status and playtest-guide updated; concept docs updated when the design changes.

## Decisions needed from the owner before building
1. The **F** key (Alternate) and **Tab** (re-read the scrap) and the debug keys F1 to F6, all read through `Keyboard.current` with no binding change.
2. The first-guess numbers: Reliability penalty 8 per unfinished task, 80% promotion gate, par per day, errand effects. These are meant to be tuned from the CSV.
3. Copy tone while "what the outside is" stays open: placeholder copy will assume the Town as the lens and a bureaucratic register (option C), and is easy to retone.

## Not in this plan
Slice 5 (cameras, restricted zones, audits, Attention, pull exfil, caught consequences, hacking); new rooms (cooling plant, generator hall and so on); the final act and locks; save/load; real art and sound; the "outside" decision; the Wellness Moment trigger.
