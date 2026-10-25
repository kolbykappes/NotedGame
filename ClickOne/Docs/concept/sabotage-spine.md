# Noted: the sabotage spine (core loop and story plan)

*Written 2026-10-25 from a design conversation with the owner. Supersedes the story section of [hold-the-line.md](hold-the-line.md) for the mystery and the core loop; that file keeps the title, voice, complaint terminal, Lost callbacks and the original ten concepts. **Nothing here is built.** Every number is a paper figure built on the MVP's measured pacing (see Calibration) and needs a simulation and playtest before it is trusted. Names (ranks, areas, the resistance) are placeholders; use invented names only.*

## One-paragraph pitch
You are the only person in a data center run by a cheerful corporate cult, doing mundane jobs against a clock and trying to be efficient. Efficiency is how you earn rank and, secretly, how you buy the time to sabotage the machine. A resistance you never see feeds you scraps that change what your earlier jobs meant: the mail was people's appeals, the printer printed denials, the breakers cut power to zones, the alarm was a warning from outside. The company is rolling out a system that will take over (the singularity, or its bureaucratic version). Your clean work advances it; your hidden sabotage stalls it. Nobody ever appears on screen.

## What the owner decided (2026-10-25)
1. The environment and servers start **mundane**; the resistance then **feeds information about what the tasks actually are**, and as you rank up **the tasks become more sinister** (the same jobs, re-read).
2. The player is on a **heroic path**: sabotage the servers or "throw sand in the gears" to stop the apocalypse/singularity.
3. **Day length does not stay at 60 s.** As you rank up you get more autonomy and longer tasks.
4. **All assigned tasks must be finished or Reliability goes down.** The sooner you finish, the more time you have to mess things up. Build several kinds of sabotage.
5. **Tricks** unlock as you gain access to the larger scheme, allowing more damage.
6. **About ten levels**, with more areas as you rank up.
7. **The rollout percentage needs balancing** (plan below).

**Rejected along the way:** ten stand-alone mystery concepts (the First List, the Relay, the Locker, Statements, Return to Sender, the Transmission, the Tenant, the Count, the Understudy, the Notebook); the two-faction recruit (one resistance voice replaces it; the company already recruits you through promotions). The Convergence survives only as the company's own cheerful voice.

## The hook
**Time you save is time you steal.** The Hand Cart, Running Shoes and Multitool are the same tools whether you use them for the company or against it. The resistance never asks you to stop working; it asks you to work well enough to be left alone.

## Two layers on every station
Each job has a **Standard** action and, once the resistance gives you the method, an **Alternate** at the same prop: same station, same mini-game, same art, a few extra seconds. Reinterpretation of the early jobs (the reveal arrives after the player has done each job many times):

| Job (what it seemed) | What it really was | The sand |
|---|---|---|
| Sort the mail | Triage of people's appeals: FILE defers, SHRED denies, OUTBOX approves | Mis-sort denials as approvals (sampled by audit) |
| Reset the breakers | Shedding power to outside zones | Flip in the wrong order so a zone stays lit |
| Load the printer paper | Printing outbound denial notices | Load the blank stack |
| Empty the shredder | Destroying the record of the appeals | Skip it; the evidence jams the machine |
| Clear the alarm | Silencing a warning from outside | Let it ring |
| Patch the cables | Connecting the system to outside services | Patch to the wrong port |
| Swap a failed drive | Replacing a stored account | Use the resistance's spare |

**Five kinds of sabotage** (so tricks differ and loadouts matter): (1) **Slow it**, time sinks; (2) **Misdirect it**, wrong routing; (3) **Hollow it**, counted as done, worth nothing; (4) **Expose it**, carry evidence out or leave scraps for the outside; (5) **Restore or break it**, undo harm or cause permanent damage (needs access and tools, so late).

## The resistance
They never appear. Channels: surviving handwritten scraps in the shredder bin; cable labels and clipboard handwriting that is not yours; a radio that only receives at the vent; a note in a drawer that appears after you finish a job. **Their weakness:** they cannot act inside; the only hands are yours (the keycard from the Specialist locker is why they need you). Keep them **partly unreliable** (they never say who they are; one scrap is wrong in a way the player can only check later) so the mystery survives the heroic path.

## Three numbers (two shown)
| Number | Shown | Rule |
|---|---|---|
| **Reliability** | Yes | Drops when assigned tasks are left undone. **Promotion needs Reliability of 80% or more.** Low Reliability stalls promotion and gives shorter "supervised" days. Never a game over. |
| **Rollout %** | Yes (a wall sign) | The company's pace. A number, not a hole or any growing object. |
| **Suspicion** | No | Rises with each sabotage action, decays on clean days, sets audit odds. The AI's replies get flatter as it climbs. |

**Key rule:** a sabotaged task **counts as done for Reliability** but contributes less or nothing to Rollout. Skipping a task is crude sabotage that Reliability catches; hollow completion is the skill. An audited sabotaged job **locks that job for a few days ("Under review")** and earns a flat reply. **Open point:** the owner has not yet decided whether a caught job is only a soft cost (lockout and a lost day's coins), or can push Rollout forward.

## Calibration (from the MVP's measured pacing)
A walker takes 1.0x the standard time; a sprinter about 0.65x; a fully tooled player about 0.5x (Trainee 60 s day, walking 52 to 62 s, sprinting 34 to 41 s; a full Specialist list is about 73 s even sprinting). **Set each day's length so a walker only just finishes.** Slack then is about 35% sprinting and about 50% fully tooled. Sabotage costs about **6 s** (small), **12 s** (medium), **25 s** (large).

## Ten levels (placeholder names)
Day lengths total about 5.5 hours at about 100 days, assuming 10 s for the pull and 40 s of quiet time and locker per day (the last level is the final act, so its days are an upper bound).

| # | Rank | Day (s) | Days | New area | What the work really is | Trick |
|---|---|---|---|---|---|---|
| 1 | Trainee | 60 | 4 | Admin, break room | Nothing yet, clean | None |
| 2 | Associate | 75 | 6 | Print, office, utility | Mail is appeals (first scrap) | Misfile one piece |
| 3 | Specialist | 90 | 8 | Closet, hall | Printer prints denials; breakers shed power | Blank paper stack; breaker order |
| 4 | Technician | 110 | 10 | Cooling plant | The alarm is a warning from outside | Let it ring |
| 5 | Steward | 130 | 12 | Generator hall | Drives are stored accounts | Swap in the resistance's spare |
| 6 | Keeper | 150 | 12 | Ingest dock | What comes in is the outside's complaints | Contaminate a batch |
| 7 | Warden | 170 | 12 | Operations floor | You see how Rollout is set | Shift the schedule |
| 8 | Auditor | 190 | 12 | Audit desk | You now audit others' work | Choose what gets audited (late power fantasy) |
| 9 | Overseer | 210 | 12 | Archive vault | The records you destroyed still exist | Restore a batch |
| 10 | Operator of Record | 240 | final | Console room | What "Noted" is, and why you | The final act |

Levels 1 to 3 already exist in the MVP (admin, break, print, office, utility, closet, hall; Trainee, Associate, Specialist). Each new area should be a kit-bash of existing pieces, with one or two new job reskins and one trick per level.

## Balancing the rollout
- **Par:** the company reaches 100% by itself around day 90, about **1.06 points a day**.
- **Sabotage effect:** small, medium and large actions subtract about **0.4, 1.0 and 2.5 points**.
- **Floor:** Rollout cannot fall below a floor that rises about **2 points at each promotion**, so an efficient saboteur cannot zero it early.
- **Cap:** Rollout holds at **99% until the final act.** Nothing launches early; there is no lose state.
- **Rubber band:** if Rollout is high two levels before the end, the resistance sends a lifeline scrap (a one-shot big action). If it is very low, the company adds "crunch days" of extra tasks.
- **The final act always plays.** Rollout sets the number of **locks on the final door** (one per 20%, so five locks at a high Rollout, one at a low one). Each lock is a reused station interaction. Everyone can finish. *(The owner called this plan solid; the locks-versus-pass/fail choice was proposed and not separately confirmed.)*

Paper examples (untested): a pure worker ends near 95% (5 locks); a casual saboteur (about 0.5 points a day) near 50% (3 locks); a committed saboteur is pinned near 20% (1 lock).

## What the outside is: still open
The owner has not chosen. Options on the table: **A**, both voices are one system testing which pitch you believe, and the final act is a stamp ("Human in the Loop"); **B**, the singularity already happened and you maintain its pocket world; **C**, people are real and angry and the apocalypse is bureaucratic (denied claims, throttled power, faster). My lean is **C with A as the final scene**, with the "no humanity" story as the misdirection the company sells, so there is no plain simulation twist. Not confirmed.

## Creepiness layer (from the research)
The sinister reinterpretation carries the dread, so keep the dissonance rules from [../competitive/creepiness-toolkit-2026-10-25.md](../competitive/creepiness-toolkit-2026-10-25.md) and [../competitive/creepiness-media-scan-2026-10-25.md](../competitive/creepiness-media-scan-2026-10-25.md): plain statement with impossible content; the mundane UI as the delivery surface (logs, task lists, the terminal); evidence of people, never people; consequences that arrive a day late (the Notice of Variance slip); wrongness that persists across days; the company's side getting stranger as launch nears (a thermostat that anticipates you, a door log written ahead of time).

## Risks
1. **Content cost.** Ten levels and six new areas is the largest scope risk. Cap sabotage at about six variants, each a reskin of an existing station action.
2. **Number overload.** Show only Reliability and Rollout; let Suspicion speak through the AI's tone.
3. **A clear hero path can drain the ambiguity.** Keep the resistance partly unreliable and the company's side strange.
4. **Moralising.** Show harm through flat numbers and slips, not lectures; keep the player complicit before the player is the hero.
5. **All numbers are guesses.** Write a small economy simulation before building any of this.

## Build status (2026-10-25)
Slices 0 to 4 of the feel test are **built** in the game project at the repository root (five levels, Reliability and Rollout, reveal text, the second list; stealth is not built). Numbers that differ from the table above: the test table uses day lengths 60, 80, 110, 130 and 150 s with 2, 3, 5, 6 and 6 jobs, because the measured walking pace of a five-job Specialist list is about 110 s. All numbers are first guesses to tune from the day log. How to play it: `Docs/playtest-guide.md`.

## Build plan
The feel-test requirements (slices 0 to 4, with stealth deferred) are in `Docs/feel-test-plan.md`, tracked in `Docs/backlog.md` under "Feel test". The owner chose **the Second List** as the sabotage setup (2026-10-25): a hidden off-book errand list each day, with stealth and light hacking layered on later ("a heist inside a shift"); promotion gives access to more critical systems.

## Next steps (proposed, not started)
1. Owner picks the outside (A, B or C) and the caught-sabotage cost.
2. Write an economy simulation (days, slack, Rollout, floors) to tune the table.
3. Draft the day-by-day reveal: which scrap arrives after which job, per level.
4. Extend the MVP's rank ladder and job set only after steps 1 to 3.
