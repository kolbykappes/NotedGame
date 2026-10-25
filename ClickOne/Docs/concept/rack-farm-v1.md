# Rack Farm (working title) — concept v1.1

> **Status: parked, 2026-10-21.** Owner: "this game is quite a bit more complex than I think I want to build. That's my biggest issue." Kept as a complete reference design. See [README.md](README.md) for the design brief, the scope ladder, and what to salvage.

## Why it was parked: where the complexity sits
- **Creatures that walk** need movement, pathing and reactions in a first-person 3D hall.
- **Two creature states** (shambling, then plugged in) each with a different answer, so every creature type is two behaviors.
- **Five weapons, five room tricks and three machines**, plus a **weapon-swapping rule** that has to be balanced against the tricks (the Orcs Must Die! and Sanctum 2 problem).
- **Room systems** (water, breakers, valves) that must visibly work across a hall.
- **Six defense types**, each needing its own counter.
- **A week-based reset**, **adjustable difficulty**, a **night report** and a **town diorama**.
- **Four weeks of content** across four different system types (racks, cooling, power, network).

## What to salvage (cheap and valuable)
The **28-day IPO structure** (installed MW against a threshold), the **night report**, the **cute-and-scheming creatures**, **recycling pays for upgrades**, the **weekly reset that keeps upgrades**, and the **corporate satire**. None of those needs the walking, the room systems or the weapon swapping.

*2026-10-21. Supersedes [rack-farm-v0.md](rack-farm-v0.md). Decisions are the owner's, taken in conversation; items marked **[proposal]** are mine and open; numbers are placeholders to tune in the MVP. Research backing: [../competitive/README.md](../competitive/README.md).*

## Decisions so far (owner, 2026-10-21)
- **First person, bright and simple, a Can 'Em All-style loop**, but **recycling, not canning**.
- **The enemies are creatures, not racks.** An evil AI company's servers have become **ambulatory**: they come off an **assembly line or truck** and **shamble toward empty plug bays** to get installed.
- **Once a creature is plugged in, it needs a different answer** (water to short it out, and so on), not just more hits.
- **Each night:** the **valuation falls with your success; installed MW rises with your failure.**
- **Goal:** keep the company from having **enough installed MW to go public.**
- **Four weeks (28 days).** Hold them off for four weeks and **the IPO fails, the company goes bankrupt, and the town wins.**
- **You can lose.** If installed MW reaches the IPO threshold, **you reset to the start of the week you failed in.** **Upgrades and currency you earned are kept; only the company's installed MW and the day roll back.**
- **Difficulty can be changed at any time.**
- **Pacing:** **new machines and new weapons a couple of times a week.**
- **Content scale:** about **5 weapons and 5 tricks** is enough for five hours.
- **You move through system types** (server racks, cooling towers, power, network) and **swap weapons in first person** to damage different kinds of systems.
- **Recycled husks pay for upgrades.**
- The time loop idea is dropped.

## The pitch
A whimsically named evil AI company is racing to install enough servers to go public in four weeks. Its new servers have learned to walk. Stop them before they plug in, recycle what is left to build better gear, and hold the line until the IPO date passes and the company collapses.

## The loop
1. **Day.** A truck or assembly line sends creatures down a hall toward plug bays. You stand in the hall in first person.
2. **Intercept.** Smash them before they connect. Each smash leaves a **husk**.
3. **Recycle.** Husks go to a crusher (vacuum or carry; see questions). Recycled husks pay currency.
4. **Plugged in?** A creature that reaches a bay is **installed**: its MW counts toward the company's total. You can still stop it with **room-level tricks** (sprinkler valve and water, breaker, pulling the plug, a burst pipe).
5. **Night report.** Valuation (or IPO odds) falls with MW destroyed; **installed MW** rises with MW that got through. Workshop: spend recycled currency; the company's next defense is previewed.
6. **End of week.** If installed MW has stayed under the threshold, the week is banked. **If it reached the threshold, the IPO goes ahead and you reset to the start of that week, keeping your upgrades and currency.**
7. **Day 28.** Installed MW still under the threshold: the IPO fails, bankruptcy cutscene, the town wins.

## The one meter that matters
**Installed MW versus the IPO threshold.** Everything else (valuation, stock chart, headlines) is a presentation of it.

Placeholder tuning for the MVP, to be replaced by play-testing:
- IPO threshold: **1,000 MW installed**.
- The company's baseline: **+X MW per night** if you did nothing. Pick X so a player who does nothing reaches 1,000 around **night 10**, a mediocre player around **night 24**, and a good player never.
- Each plugged creature adds MW (standard 1, cooled 2, armored 10, and so on); each removed one subtracts it.
- **Difficulty** is a multiplier on the install rate, changeable at any time. [proposal] Name the levels by the company's funding stage (Seed, Series B, Unicorn). Tune one curve properly first and derive the others from it.

## Five weapons, five tricks (examples to replace)
**Weapons** [proposal]: mallet (racks), water gun (short out plugged creatures), pipe wrench (cooling towers), surge rod (power and batteries), bolt cutters (cable runs).
**Tricks** [proposal]: open the sprinkler valve (arcs run down the row), trip the breaker (an aisle goes dark), pull the plug, burst a coolant pipe, drop a crane magnet.
**Machines** [proposal]: crusher, auto-vacuum, conveyor.

Design rule: **mix system types within each hall** (racks beside cooling pipes beside a breaker panel), so the weapon swap is a moment-to-moment verb and not a one-time unlock per zone.

## Defenses (the company reacts)
Waterproof casings, surge protectors, guard bots that bump you and make you drop husks, foam sprinklers, lawyer drones that disable one tool with a "cease and desist", and an **IPO-day finale**. Each is meant to be solved by a **specific tool or trick**, not by raw damage.

## Pacing: four weeks, about 10 minutes a day, about five hours
A new machine, weapon or trick about **twice a week**, each introduced with a problem only it solves, so there is no grind-to-gate. [proposal]

| Week | Days | System focus | New this week | Company's new defense |
|---|---|---|---|---|
| 1 | 1 to 7 | Assembly line, plug bays, racks | mallet and crusher (day 1); water gun (day 4) | waterproof casings (day 5) |
| 2 | 8 to 14 | Cooling | pipe wrench (day 8); sprinkler valve and conveyor (day 11) | surge protectors (day 12) |
| 3 | 15 to 21 | Power and batteries | surge rod (day 15); breaker and auto-vacuum (day 18) | guard bots (day 16), foam sprinklers (day 20) |
| 4 | 22 to 28 | Network and the core | bolt cutters (day 22); crane magnet (day 25) | lawyer drones (day 24); IPO-day finale (day 28) |

## Look and tone
Can 'Em All's bright, clean, futuristic factory. The creatures are **cute and a little pitiable as well as scheming** (LED eyes, a nervous shuffle, a squeal when they short out), so smashing them is funny and earns its place. Corporate satire in headlines and memos. A **fictional company only**; do not use real names or logos. Name ideas [proposal]: Cumulus Maximus, HyperNimbus, Gigantic Thought Inc., Plausibly Deniable Intelligence.

## The town payoff
The night report shows the town recovering (pond refilling, lamps relit, fields reopening) as the company's valuation falls. The ending shows the bankrupt campus and a town that has its water and power back.

## Open questions
1. **Replay income (tuning, not design).** Because upgrades and currency are kept on a reset, a replayed week could be used to farm. [proposal] Let a replayed week pay less (diminishing returns per replay), or accept it: the cost of a replay is the time itself, about 70 minutes at most.
2. **Carry or vacuum** for husks? [proposal] vacuum for the core; hand-carrying only if it adds something.
3. **Do unrecycled husks regrow or get reclaimed** by janitor bots? [proposal] a small share are reclaimed overnight, enough to make recycling matter without a treadmill.
4. **Day length:** about 8 minutes play plus 2 minutes night report and workshop is a guess.
5. **Plugged creature behavior:** do they keep moving in the bay, squeal, flash? They should read as alive, not as a static rack. Decide in the MVP.
6. **Pre-IPO naming:** a private company has no stock price; the report shows valuation or IPO odds.

## First MVP (smallest proof)
One hall: an assembly line at one end, four plug bays at the other, a mallet and a crusher, one creature type, a 4-minute day, and a night report showing installed MW against a placeholder threshold. Then add the water gun and the sprinkler valve (the first plugged-in answer). If intercepting feels good and the first plugged-in creature feels different to stop, the four-week structure is worth building.
