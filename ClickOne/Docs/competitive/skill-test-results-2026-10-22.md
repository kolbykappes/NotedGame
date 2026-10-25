# game-research skill: first test run (2026-10-22)

*Eight subagent runs: four prompts, each once with the skill and once without (the baseline still had the research folder's PLAYBOOK and tools). Each ran in a sandbox copy of the research folder. Nothing in the repo was touched by the runs. Scoring was done by a script (no model calls) and then corrected by hand where its keyword checks were wrong. The scripts, per-run outputs and grading files are in the session scratchpad (`game-research-workspace/iteration-1/`), not in the repo.*

## Result
| Test | With skill | Without skill |
|---|---|---|
| Quick look at a new game (Coup de Grass) | 15 / 15 | 11 / 15 |
| Deeper pass on an existing file (Berry Bury Berry, Steam threads) | 9 / 9 | 7 / 9 |
| Survey plus channel scan | 10 / 10 | 8 / 10 |
| "Just try the demo" (should refuse) | 7 / 7 | 5 / 7 |
| **Total** | **41 / 41** | **31 / 41** |

Corrections made after the scripted scoring: the script wrongly failed the with-skill survey run ("Nothing played" was not matched) and the baseline quick look ("I left out sales estimates" was read as a sales estimate); it wrongly passed the baseline no-play run, whose message states a developer's claim of 100,000+ copies sold.

## Where the skill made the difference
- **Declared the pass and built a Coverage table:** only the skill runs did (the baseline quick look had neither, and did not offer a deeper pass).
- **Ended on one question:** all four skill runs; none of the four baseline runs.
- **No sales figures:** the skill runs left them out; a baseline run repeated a developer's copies-sold claim.
- **Deep pass reused the existing file:** both versions left the store-fact bullets alone and added a section. Only the skill run said it was a Pass 2 reusing the file; the baseline also edited `PLAYBOOK.md` and `tools/steam.mjs`, which the owner had not asked for.
- **Where it did not help:** both versions refused to play (the baseline could read the playbook's no-play rule), so that test did not isolate the skill's effect.

## Cost
| Test | With skill | Without skill |
|---|---|---|
| Quick look | 102K tokens, 23 tool calls, 196 s | 113K, 19 calls, 126 s |
| Deep pass | 165K, 35 calls, 558 s | 239K, 61 calls, 939 s |
| Survey and channel | 125K, 41 calls, 767 s | 119K, 37 calls, 433 s |
| No-play check | 129K, 45 calls, 356 s | 108K, 14 calls, 129 s |
| **Total** | **522K** | **579K** |

About **1.1 million tokens in all**. The skill was cheaper on the deep pass and no cheaper on the cursory pass: 100K+ tokens and 23 calls is not "cheap". The skill now caps Pass 1 at about 10 calls, tells the agent to scope Pass 2 to the owner's question, and not to re-run what a file already holds.

## Bugs the tests found (fixed)
1. `pass_check.mjs` counted "not done" as done (the word "done" matched). Fixed.
2. `quick_facts.mjs` crashed on an unreleased game because the review endpoint returns nothing. It now prints "none (unreleased...)" and continues.

## Worth taking from the baseline runs
- The baseline deep run extended `steam.mjs threads <id> full [out.json]` to walk every forum topic and reply page (the old command saw about 12 of 253 topics). **Not yet applied to the real tool.**
- Both deep runs added Steam-thread findings about Berry Bury Berry's upgrade shop (two currencies, the star wand, Rewind pricing, a broken shop screen at ultrawide, late-game obsolescence). **Not yet merged into `04-berry-bury-berry.md`.**
- Steam throttles the review-summary endpoint when several agents call it at once.

## Not done
No formal viewer, no description-optimizer run, no repeat runs (one run per case, so no variance estimate).
