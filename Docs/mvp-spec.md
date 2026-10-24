# MVP spec: the core loop

> **Status 2026-10-24 (late night): the loop below is built and verified in Play Mode with simulated input. See `Docs/playtest-guide.md` for what was built, what is unverified, and the tuning knobs. Differences from this spec: the shredder moved to the far utility room; a global `TaskHoldScale` of 1.5 lengthens every hold; complaining pauses the clock; uncollected coins are banked when you sleep; placeholder sounds were added; the upgrade list is the longer one proposed above.**

*Decided with the owner 2026-10-24. Focus: the mechanics of the core loop. "Discover things" and story are layered on later. Everything here is a placeholder unless marked decided.*

## Organization (decided 2026-10-24)
**The Solace Initiative** (a Dharma-style soft, earnest research mission; it is also the support-desk cult). Ideas to use later, not yet decided: a small dove or cup logo, the slogan "Comfort Is Our Output", Dharma-style stations with symbols (Station One: The Kettle, the break room; Station Two: The Press, the printer; Station Three: The Window, the server-hall glass), and the AI named Solace-5 or Noted-5. The earlier placeholder "Warm Regards Foundation" is retired. Rejected name families: GPT-style acronyms, Steady State, Gracious Compute and similar.

## Look (starting point, 2026-10-24)
The palette in `Docs/art/palette.md` (muted teal walls, cream type, beige props, one purple accent) comes from a placeholder title-screen image, `Docs/art/title-screen-placeholder-v2.webp`. It is a reference to tune by eye, not a commitment.

## Intro (decided)
A full-screen text card shown once at the start. It introduces the organization (The Solace Initiative) and the job (placeholder text), gives the starting rank (**Trainee**), and gives the **six-digit code**. It stresses how important it is to be at the console when the timer ends and that failing to enter the code is "very bad", **without saying how bad**.

## The day (decided unless noted)
- **60-second day** with a visible timer. It is fine if tasks run out of time. The space must be big enough that the base player **cannot easily finish the list**, which pushes the player toward upgrades. (A tuning target, not yet measured.)
- A **list of tasks**, short at low rank: Trainee 2, Associate 3, Specialist 5 (config). The first time a new task type appears, a simple **how-to card** explains it.
- Finishing a task before the timer ends pays **Uptime coins** (10 each, config). Unfinished tasks are only counted; nothing else happens for now.
- When the timer ends, the game **pulls you back** (hard cut to third person, a placeholder avatar walks to the console), and the **code you were given at the start is entered automatically**. The code is a ritual. Skippable and under about 5 seconds.
- **Quiet time:** the lighting comes down and the player wanders the office, collecting any coins left on the floor. Nothing is lost.
- **Complaint terminal:** a clearly lit terminal the player walks to; **presets only**, with a canned reply that never repeats back to back. Available during quiet time and at any time the player walks to it.
- **Night shop** at the cot: spend coins on upgrades. Some tasks fail only because there is not enough time, so the shop must give time and speed.
- No save/load in the MVP: everything resets when the game quits.

## Rank (decided unless noted)
- Rank is **earned by tasks completed successfully** (before the timer ends); levelling up **consumes no coins**. Thresholds for the MVP: rank 1 after 5 completed tasks in total, rank 2 after 12 (config; balance later).
- Each rank-up shows a full-screen **training-tape text card** at the cot.
- **Rank 1, Associate:** a better lanyard (grey to blue).
- **Rank 2, Specialist:** a trite but useful perk. Proposed: comfortable shoes (about +8% walk speed). Alternatives: a travel mug (the coffee task takes half as long) or a laminated task card (a marker shows where each task is).
- Only the first two rank-ups are built in the MVP.

## Starter tasks (proposed; all use "hold at a station for 2 to 5 s" unless noted)
1. Refill the coffee station.
2. Clear the printer jam.
3. Empty the shredder bin.
4. Swap the air filter (two steps: take a filter from the supply shelf, then use it at the vent).
5. Log the temperature (walk to the thermostat by the server-hall glass, write the number on the clipboard).

## Upgrades (proposed; costs are first-pass guesses)
Goal: make tasks easier or faster, so the day becomes easier to finish.
- **Time:** Longer Day (+15 s).
- **Speed:** Faster Hands (-15% hold time); Running Shoes (+walk speed).
- **Reach:** Long Reach (interaction range +1 m).
- **Planning:** Task Marker (arrows to the tasks); Morning Briefing (preview tomorrow's list).
- **Pickup:** Coin Magnet (wider coin pickup).
- **Efficiency:** Tidy Habits (each completed task adds +3 s to the clock).
- **Automation:** one-off purchases that auto-complete one task type each day (Automatic Coffee Maker, Printer Service Contract, Shredder Schedule). Default rule: auto-completed tasks pay coins but do **not** count toward rank.

## Space (decided, layout proposed)
A simple data-center **support facility**: admin room (cot, console, complaint terminal), corridor, break room (coffee, plant), print room, and a **locked server door with a glass window**, with the **organization name on signs**. All built from cubes. Big enough that the base list cannot easily be finished in 60 s.

## Build order (proposed)
Off-screen render helper; intro card with rank and code; day timer; the five tasks with how-to cards; coins and HUD; the pull with the automatic code entry; quiet time; the complaint terminal; the shop; the two rank-ups. The larger greybox and settings screen come after the loop works.

## Open
- Upgrade list and costs (owner asked for more ideas that make tasks easier; a longer list was proposed, not yet confirmed).
- Whether the starter tasks above are accepted (the owner did not object).
- The owner has not yet said "go" to start building.
