# Rack Farm: five comparables and what their players say

*2026-10-21. To inform the first-zone MVP in [../concept/rack-farm-v0.md](../concept/rack-farm-v0.md). Sources: Steam store pages and the five most-helpful positive and negative English reviews per game, plus the Reddit reveal thread for Can 'Em All. Nothing played. Evidence tags: **[player report]**, **[dev]**, **[inference]**.*

## The five

| Game | Steam | Reviews (all languages) | Why it is a comparable |
|---|---|---|---|
| **Can 'Em All** | [5133900](https://store.steampowered.com/app/5133900/) (demo [5364590](https://store.steampowered.com/app/5364590/)) | demo out Oct 9, 2026, no Steam reviews yet | The base loop: first-person factory, kill, crush, can, tube, sell, upgrade, automate. |
| **Hardspace: Shipbreaker** | [1161580](https://store.steampowered.com/app/1161580/) | 22,507 (89%) | First-person salvage: cut a ship apart, sort pieces into furnace, processor and barge; a shift fee instead of a fail state. |
| **PowerWash Simulator** | [1290000](https://store.steampowered.com/app/1290000/) | 58,945 (96%) | One simple verb, permanent visible progress, drop-in sessions. |
| **Teardown** | [1167630](https://store.steampowered.com/app/1167630/) | 137,774 (96%) | Destruction as the toy; planning how to break things is the game. |
| **R.E.P.O.** | [3241660](https://store.steampowered.com/app/3241660/) | 429,720 (96%) | Physics-based carrying of valuables to an extraction point, with a quota. |

Already studied and relevant: **Bills Must Be Paid** (juicy smashing, 4 to 5 hours), in [short-incrementals-what-works.md](short-incrementals-what-works.md).

## What each one's players say

**Can 'Em All** (reveal thread, r/indiegames, 745 upvotes, 118 comments) [player report]
- The hook is the **dark joke on a cute surface**: "Slime Rancher if you were a sadistic maniac", "the chill vibes of PowerWash Simulator, with an added psychotic flavour". Many replies are some version of "wishlisted, but wtf".
- Requests: different blood colors; a reason why canning is "a good thing". **[dev]** replied that an **office with emails that make you question what you are doing** is planned, depending on how the demo does.
- There is **no player feedback on the actual loop yet**; the demo is eleven days old.

**Hardspace: Shipbreaker** [player report]
- Praise: "cathartic", "no killing", "each ship is a puzzle", and a **shift fee (about $500K per 15 minutes) that encourages efficiency with no pressure to fail**. A "sandbox mode is a great toy".
- Complaints: only about three tools and upgrades that are "+10% range, +10% durability"; **the same ship types repeat**; "the content just stops" halfway through the ranks; a later update "stretched out what they had into a VERY long grind".

**PowerWash Simulator** [player report]
- Praise: "the satisfaction of clean", the **"Ding!" when an item is finished**, easy to drop in and out, and a hypnotic repetitive action. A 1,321-hour reviewer says it is "only now starting to get boring" after ten passes of nearly every level.
- Requests: a level editor and Workshop levels. Complaints are business ones (DLC-locked achievements, privacy policy), not about the loop.

**Teardown** [player report]
- Praise: **freedom and physics**; "a solid puzzle game with sandboxy destruction elements" where each mission drops you in a sandbox with an objective and you choose the path. Players ask for **co-op and Workshop levels**. The negatives are almost all about the studio's sale and DLC pricing.

**R.E.P.O.** [player report]
- Praise: funny physics chaos, fun solo and better with friends.
- Complaints that bear on carrying: **"clunky" pickup where items "ragdoll or snap into position" and lose value or break on grabbing**; **motion sickness from head-bob with no FOV option**; early-game balance; a colorblind player cannot read the dark palette.

## What this says about the Rack Farm design [inference]
1. **Permanent progress is what PowerWash and Hardspace sell.** A finished ship or a clean wall stays done. Rack Farm's nightly regrowth is the opposite unless the **recycled** state reads as permanent and finished. The MVP should show a zone that **stays clear**, not only a counter.
2. **Variety per unit of work is the failure mode in the salvage game.** Hardspace's complaints are repetition, "+10%" upgrades and recycled ship types. Each rack tier needs a **different way to break and a different tool**, not larger numbers.
3. **The dark-cute edge is what made Can 'Em All pop.** Inanimate racks carry no such joke; the replacement has to come from the **corporate tone** (stock ticker, memos, the CEO mascot), not from the smashing itself.
4. **Carrying must be forgiving.** If hand-carrying is chosen, avoid fragile or snap-to-hand items and give FOV and head-bob options. A vacuum sidesteps R.E.P.O.'s complaints.
5. **A timer with a fee, not a fail, works** (Hardspace's shift fee). It matches Rack Farm's "escorted out" rule.
6. **A free-smash toy mode** is asked for by Hardspace and Teardown fans; it is cheap and could be the first MVP zone itself.
7. **Player-made content is a repeated request** (PowerWash, Teardown, Orc Problem): zones as shareable levels are a later possibility, not an MVP matter.

## Limits
Reviews for Can 'Em All's loop do not exist yet; the Reddit thread is about the reveal clip. The five-review samples are the most-voted, so they favor strong opinions.
