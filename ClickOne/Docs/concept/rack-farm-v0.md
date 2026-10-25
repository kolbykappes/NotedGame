# Rack Farm (working title) — concept v0

> **Superseded by [rack-farm-v1.md](rack-farm-v1.md)** (2026-10-21). Kept for history.

*2026-10-21. First concept draft. Everything below the owner's spec is a proposal, not a decision. Research backing is in [../competitive/](../competitive/README.md).*

## Owner's spec (2026-10-21, verbatim)
> I'm in an impossibly big warehouse.
> I am smashing things - I'm trying to drive the company's stock price down, I'm measuring the size of the data center in terms of megawatts.
> At first it's easy to destroy racks that are close
> It regenerates each night
> more defenses come in I have to deal with
> Stronger racks are deeper in the building
> I have to recycle the racks or they regrow
> The more I recycle, the more currency I can get.

Earlier direction this builds on: first person; a Can 'Em All loop (kill, haul, crush, can, tube, upgrade, automate); bright, cute and simple; day/night timer; 4 to 5 hours; about $5; no way to lose; show progress in the world; few distinct units; a boss that shows what you lack.

## One-line pitch
Smash and recycle your way through an impossibly big data center, shrinking its megawatts and its stock price one night at a time, and send the cans home to the town.

## The loop
1. **Day (the raid, a timed run of about 4 to 6 minutes).** Walk the warehouse in first person and smash racks. Each smash drops a **husk** (a pile of scrap) and knocks the **stock price** and the **MW online** meter.
2. **Recycle.** Husks must go into a **crusher** (vacuum them or carry them, then crush, can, tube). Only **recycled** husks count: they pay **currency (cans)**, and they cannot regrow.
3. **Night (automatic, about 30 seconds on screen).** The market closes. Any husk left on the floor **regrows into a rack**, and the company adds a small amount of new capacity. The screen shows the stock chart, MW change, and what your automation recycled overnight.
4. **Workshop (a short spending screen).** Spend cans on tools, machines and conveyors. The company's reply for the next day is previewed (a new defense, a new zone).
5. Repeat, working deeper into the warehouse.

## The two numbers that tell the story
- **MW online:** the size of the data center, shown overhead (for example "1,200 MW"). Each rack has a MW value. It falls when you recycle, rises when husks regrow or the company expands.
- **Stock price (ticker, e.g. "ACLD"):** tracks MW online plus **news events** (analyst downgrade, CEO departs, delisting warning). Milestones trigger a **company reaction** (new defenses, a new boss) and a story beat. This replaces a health bar: nobody can lose, the company can only be hurt or not.

## Depth: stronger racks are deeper
| Zone | Rack type | MW each | How it breaks |
|---|---|---|---|
| Loading dock | Standard rack | 1 | one or two hits |
| Cold aisles | Cooled rack | 2 | pops with a water burst |
| GPU hall | Armored rack | 10 | heavy tool or three hits; heavy husk (two carry slots) |
| Battery vault | Battery bank | 25 | chain pops: smash one, neighbors go |
| The core | Core stack | 100 | boss-lite, needs the right tool |

Distance is a cost: the further in, the longer the walk back to the crusher. That is what pays for **forward crushers, conveyors and drop tubes** (the Can 'Em All factory layer).

## Recycle-or-regrow (the central rule)
- A smashed rack leaves a husk. An **unrecycled husk regrows at night.** A **recycled** husk is gone for good and pays currency.
- Smashing is the fun part and recycling is the progress and the money. The tension each day is **how much to smash versus how much to haul**.
- **Automation** (conveyors, auto-vacuums, little cleanup helpers) works during the night too, so the dawn report shows your investment: "auto-recycled 140 husks".

## Defenses (the company reacts)
| Day | Defense | What it does |
|---|---|---|
| 1 to 3 | none | learn the loop |
| 4 | **Janitor bots** | collect husks and **recycle them back into racks**, so they attack your loop directly |
| 7 | **Guard bots** | bounce you back, make you drop carried husks |
| 10 | **Foam sprinklers** | slow you; cover husks so they are harder to vacuum |
| 14 | **Shield racks** | need a specific tool |
| 18 | **Lawyer drones** | disable one tool for 20 seconds with a "cease and desist" |
| 22 | **CEO mascot** boss | tests everything you built |

No health bar. If a defense gets you, you are **escorted out** (the run ends early) and lose a share of what you carried.

## Tools (bright, toy-like, few)
Mallet (start), **vacuum** (collect husks), **magnet** (pull cans and scrap), **heavy sledge** (armored racks), **coolant gun** (cooled racks). Upgrades: swing speed and radius, carry capacity, vacuum range, walk speed, day length, crusher throughput, conveyors.

## What the town gets
The cans go down a **pipe out of the wall** and into the town: water refills the pond, power relights the street lamps, seed packets reopen fields. Shown at the **night summary** as a small diorama, so the recovery is visible.

## Five-hour shape (rough)
About 24 days at about 12 minutes each (raid, night, workshop). Zones unlock every four to five days. Ends when the stock is **delisted** and the campus falls below a "too small to matter" size, with a cutscene of the town.

## Risks and open questions
1. **Carry or vacuum?** Hand-carrying husks (heavy ones take two slots) is tactile; a vacuum is simpler. Choose one for the core, add the other as an upgrade.
2. **Regrow rule:** all unrecycled husks regrow, or only a share? A share is more forgiving.
3. **Run length:** 4 to 6 minutes is a guess. Needs a prototype.
4. **Art direction:** Can 'Em All-style bright and cute versus satirical-corporate. The racks could have little "eyes".
5. **No-lose penalty:** "escorted out" with a loss of carried husks may be too harsh or too light.
6. **Tone of the stock ticker:** a fictional company only; do not use real company names or logos.

## First prototype (smallest thing that proves the loop)
One aisle of 30 standard racks, a mallet, a crusher by the door, the husk and regrow rule, a 4-minute timer, and the night summary with MW and stock. If smashing and recycling feel good in 15 minutes, build outward.
