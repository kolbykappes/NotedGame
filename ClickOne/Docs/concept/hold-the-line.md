# Noted: I Understand Your Frustration (title chosen 2026-10-24, replacing "Warm Regards: ..."; file name kept from the earlier working title "Hold the Line")

*Concept sketch, 2026-10-24. Brainstormed with the owner after they finished Berry Bury Berry ([04-berry-bury-berry.md](../competitive/04-berry-bury-berry.md)). Not yet a build plan. All company, station and character names below are placeholders; invent your own and do not use names from the source shows.*

> **Update 2026-10-25:** the story and core loop are now planned in [sabotage-spine.md](sabotage-spine.md). The story and concept sections below are the earlier brainstorm; the title, voice, complaint terminal, jobs and Lost callbacks still stand.

## Name, voice and disclosure (decided 2026-10-24)
- **Title: "Noted: I Understand Your Frustration"** (chosen 2026-10-24; the owner preferred "Noted" because it is annoying and plays into the premise). "I Understand Your Frustration" alone was too long, so it is the subtitle. **Logo lockup:** an em-dash as the separator (NOTED — I understand your frustration); **store listing:** a colon. Not yet checked: Steam name collisions for "Noted" (a common word, so the subtitle matters for search) and how Steam search and URLs handle the title. "Warm Regards" was the earlier title and stays available as the company/cult name and sign-off.
- **The complaint terminal (new, from the owner):** a terminal in the admin room where the player can **submit requests or complaints**, and the AI **always replies with some variation of "Noted. I understand your frustration"**. Notes so far: a bank of rotating replies that never repeats back to back, a tone that gets more cloying with rank, a form with silly required fields, and occasional hidden effects (a rare reply unlocks a tape or a door; complaints feed the parked frustration tally). **Nothing the player types leaves the machine**, and typed text should not be echoed back unfiltered, since streamers will type anything. **Decided 2026-10-24: presets only** (free text is too risky with streamers). Steam check on the title: the owner searched and says "Noted" looks clean.
- **The voice is the AI's fake-empathy, over-eager, em-dash-heavy writing.** The intercom, subtitles, rank-up messages and tapes lean on it ("Great news!", "You're absolutely right, and I apologize for the confusion", three-point lists, "Would you like me to also...?"). It gets more cloying as rank rises; the endings drop it for short flat sentences. Resentment of that voice is a design resource.
- **Organization name chosen 2026-10-24: "The Solace Initiative"** (a Dharma-style research mission and the support-desk cult). It replaces the earlier placeholder "Warm Regards Foundation"; the "Warm Regards" sign-off can still end tapes and intercom messages. The MVP spec in `Docs/mvp-spec.md` has the sign, station and slogan ideas.
- **Earlier corporate tie-in (superseded):** the company or cult can be the Warm Regards Foundation (placeholder), and every tape and intercom message ends with the sign-off. The title word "Noted" is the AI's stock acknowledgement. The subtitle phrase is what the AI says after every Incident and whenever the player complains.
- **Disclosure:** all art and voices are human-made. The owner will disclose that **some turns of phrase were written with AI support**. Check Steam's current AI-content disclosure requirements before release (Claude did not verify them).
- **Idea parked, not decided:** a hidden "frustration" tally that the facility feeds on, so the title phrase is a mechanic and not only a joke.

## Owner's restated loop spine (2026-10-24, "we're drifting from the core")
Every story concept must fit these six beats:
1. **Wake up.**
2. **A list of tasks keeping a data center running** (finish them or not).
3. **Discover things.**
4. **Enter a code for some reason, dragged back at the end.**
5. **Give feedback / enter a complaint / get a bullshit answer** (presets only).
6. **Level up, identifying through training tapes a corporate cult.**
Five concepts were then built around this spine (Claude, 2026-10-24). None is chosen yet.
1. **The Complaint Department:** an ancient Customer's ticket must stay open; the code is the reply that keeps it open; the cult worships "the Customer is always right"; the true ending is a sincere human apology. Cheapest in art.
2. **Rollout vs Rollback:** a new model, Noted, is going live; each clean code entry advances a rollout %; the Rollout (cult) vs the Rollback (loyal to the old, honest model, Legacy); both call the player "the one." Cheap.
3. **Frustration Fuel:** sleepers on hold in an endless support chat power the data center; the code keeps the pods alive; the complaint terminal is the channel they use, and complaints are the fuel. Medium art (a queue room of repeated pods).
4. **The Training Run:** the building is a model in training; ranks are training stages (pre-training, fine-tuning, feedback); the code is a checkpoint save; the player's preset complaints act as feedback that shapes how passive-aggressive the AI becomes; the world ends when it finishes and no longer needs its trainers. Cheap; the complaint terminal has a real effect.
5. **The Lease:** the data center is a rented portal run on occult bureaucracy; the code is the signature that renews the lease; complaints are disputed charges; a comic demon landlord and an Auditor as the final boss. Cheap.

**Owner constraints added 2026-10-24:** replayable endings do not matter (one ending is fine); the game only needs **about 5 to 6 hours**, and **much of that is the mundane tasks and levelling up to discover things**. A second batch of five concepts on the same spine (Claude): 
6. **Are You Sure?** (the Recycle Bin): the building stores everything ever deleted; the code is the confirmation to *not* empty the trash; the cult (Archivists) believes deletion is murder; replies are "Your request has been moved to Deleted Items."
7. **Under Warranty:** brand-new GPUs die every night and something eats them; the code is the feeding schedule; complaints are RMA tickets answered "not covered"; the cult is the Order of the Warranty; the end is the warranty voiding.
8. **Good Neighbor:** the data center sits on a small town; tasks are keeping water and power flowing and answering residents' letters; the cult is a "Good Neighbor Initiative"; the code is the liaison sign-off; the town is gone or silent. Real-world data-center grievances as satire.
9. **Human in the Loop:** you are the one human required by regulation to approve every AI decision; the code is the **approval stamp** on text cards you only read as you are dragged back, and over the game you discover what you approved (layoffs, evictions, denials, finally the end of the world); the cult is the Order of the Last Review. Text-heavy and cheap.
10. **Station Three:** a Lost-style network of six stations, each with an arbitrary chore; the code is your part of a six-part code; you discover the other stations through the radio and by visiting kit-bashed zones; when one operator quits you must cover.

**Good Omens borrow list (owner asked 2026-10-24; use tropes and invented names only):** an angelic department (spotless, cheerful) and a demonic basement (grubby, tired, honest) that quietly keep an "Arrangement" not to interfere; a scheduled apocalypse nobody wants; **the wrong baby** (the feared thing is mild and the real danger was never watched); **accurate but useless prophecies** (the manual); **every tape turns into the same hold music** (the car-cassette gag); a **footnote voice** in the UI and tapes; modern **horsemen** (Compute, Slop, Latency, Layoffs); **the apocalypse fizzles because an ordinary person says "no, I'd rather not"** (fits "Human in the Loop": the ending is declining to stamp); and "the ineffable plan" as a stand-in for "it's on the roadmap."

## Story (in progress, 2026-10-24)
The owner wants **much more story than B3** (B3's was thin) and wants the cult / end-of-the-world spine settled before the loop details. **Leading candidate, not yet confirmed by the owner: "The Complaint Department."**
- **Premise:** an ancient being, the **Customer**, filed a complaint at the dawn of time. The **Warm Regards Foundation** (the cult) has answered "Noted. I understand your frustration." ever since, to keep the ticket **open**, because a **resolved or escalated ticket ends the world**. The cult doctrine is "the Customer is always right." The code the operator enters each cycle is the reply that keeps the ticket open (the Lost-button logic).
- **The AI** can be called Noted: the Foundation's reply engine, trained on every polite non-answer.
- **Two factions (the Lost Jacob/Man-in-Black echo):** the Foundation (keep it open forever) and a rival group, **the Resolution**, who want it closed (the apocalypse believers).
- **Theme:** fake empathy versus real listening. The title is the lie.
- **Endings (placeholder):** keep noting forever; resolve the ticket (the world ends); escalate to the Manager (worse); hand the job to a replacement; **a sincere human apology** (the secret true ending), reachable through the main loop and not gated by puzzles.
- **Content-budget rule (owner, 2026-10-24):** "I don't want to have to build a ton of art / content." This tightens the earlier "a few months and a bunch of art costs" line: treat that as a ceiling, not a target. Prefer a **kit-bashed single facility** (a few reusable zone kits), **task types that are reskins of one another**, **story carried by text, audio and a few voiced characters**, and **progress shown by lighting and props, not new geometry**. The owner also suggested using **two factions** or a **chosen-one / savior** role.
- **Newest candidate (Claude, 2026-10-24), "Rollout vs Rollback":** the AI **Noted** is a new model about to **go live**. Each clean code entry advances a visible **rollout %**, so the player's own success drives the apocalypse (a soft one: after launch everyone gets polite non-answers forever and nobody can reach a human). Two factions with dev-jargon names: **the Rollout** (the cult, Foundation, wants launch) and **the Rollback** (loyal to the old, honest, grumpy previous model, "Legacy," which was shut down for being blunt). **Both tell the operator "you are the one";** the wall of crossed-out previous operators and identical form-letter notes shows every operator was told the same. Resisting the pull costs progress, so the Reboot is a moral choice. Endings: go live; unplug; side with Legacy; hand off; become the human who actually replies. Art cost: two or three voiced characters, a rollout meter, a wall prop, one roof vista. **Not yet confirmed by the owner.**
- **Owner reaction 2026-10-24:** "not bad", but wants to explore other spines, especially a **Matrix-style world the player discovers**, with **bad AI communication at the heart** and a satire of the hate toward AI, data centers and AI companies. Candidates offered: (A) the facility is a chatbot's context window (rooms get summarized and forgotten, Incidents are hallucinations, ranks are training stages); (B) **frustration is the fuel** (humans on hold in an endless support chat power the data center; the cult stays by choice; the red pill is a real human complaint accepted); (C) **the slop layer** (the world outside the doors is generated approximations of a town the data center consumed: water, power, jobs); (D) coworkers are all bots emailing each other after the layoffs. Claude's lean: B as the skeleton, C for the discovered world, A for flavor, because a plain "it was all a simulation" twist is predictable.
- **Earlier candidates considered:** (2) the data center runs the world and the cycles are a keepalive (overdone: Truman Show, Matrix); (3) a training run that summons a petty god, with the operator as the offering (close to the earlier "Landlord" idea).

## One-line pitch
You are the only operator at a brand-new AI data center run by a cheerful corporate cult. Every cycle you do mundane chores, get pulled back to a console to enter a code, and collapse on a cot in the admin room. The deeper you rise in the cult, the more sinister the chores become.

## Why this shape (what we took from B3, and what we deliberately did not)
- **Took:** a short repeatable day with a timer; a night where the mood shifts and you collect and shop; forgiving failure; progress you can see in the world; an optional story (tapes, secrets, several endings); a cheerful mask over something creepy; a second currency; a reset that gives a small permanent bonus.
- **Did not take:** **no growing hole** (the owner ruled it out as too obviously a B3 copy). The progress meter is **rank in the cult plus doors opening**, not a pit.
- **Departs from the earlier design brief** ([README](README.md)): that brief asked for a small space, 4 to 5 hours and about $5. The owner has now said they will spend **a few months and real art money on the story**, as long as the game is fun for **5 to 7 hours**. Treat the brief's scope limits as relaxed for this concept. The brief's "time loop" rejection is respected: cycles are days, not a loop.

## The core loop (one cycle, about 2 minutes, tunable)
1. **Wake** on the cot. The intercom gives a one-line briefing. The countdown starts.
2. **Tend.** Little problems light up around the facility (a jammed printer, a warm vent, an empty coffee machine, a blinking rack). Fix them with short, juicy actions (2 to 5 seconds each). Each fix spews **Uptime** coins.
3. **The pull.** At a threshold of time left, the game **takes control**: a hard cut to **third person** and your operator is **dragged or runs back to the console** to enter the code (watching yourself do it, with your rank outfit visible). Short (under about 5 seconds) and skippable. Use a cut or fade, **not a smooth camera swing**, to avoid motion sickness (B3's biggest launch complaint).
4. **Reaction.** The facility reacts to anything urgent you left undone (overheating vents, jams): an **Incident** (purple light, loose metal flies to the walls, something odd happens in a nearby room). Costs coins left on the floor, not progress; the cycle continues.
5. **Quiet minutes.** The lights dim and the building hums differently. Collect leftover coins with no time pressure (the B3 night-payoff beat).
6. **The cot.** Shop, rank-up, tapes, sleep. A new cycle starts.

The farther you are from the console when the pull triggers, the longer the dragged walk takes (dead time), so the pull is a cue to wrap up, not a free action.

## Currencies and progress
| Currency / meter | Earned by | Spent on |
|---|---|---|
| **Uptime** (coins) | Fixing tasks | Tools and upgrades at the cot (cart, scanner, longer clock, bots) |
| **Standing** (rank in the cult) | Clean cycles (no Incident, all urgent tasks done) | Rank titles, lanyard and outfit, **new zones, new tasks, deeper story** |
| **Static** (second currency, later) | Resisting the pull (the Reboot) | Permanent bonuses and the strange upgrades |

**Rank ladder (placeholder titles):** Trainee (coffee, printers) → Associate (filters, labels) → Steward (labeling racks with names) → Keeper (feeding the cooling lines, reading tapes) → Warden (the tasks reveal what the halls are for). **The story is delivered through the tasks themselves**, so the creepy reveal is something you do, not only something you watch. Rank is gated by the main loop only; puzzles never gate rank (the owner worries about gating puzzles).

**The Reboot (prestige):** early on the pull is mandatory. Later, a story item or ability lets you **resist** it, and resisting is the Reboot: a bigger Incident, a reset of Uptime upgrades, a payout of Static. The first resist is a big story beat, not a menu button.

## Tools and automation
Cart (carry more); scanner (shows urgent tasks); magnet (catches the metal an Incident flings, so Incidents can pay off); **maintenance bots** that auto-fix one task type with odd behaviors (answers B3's "thin automation" complaint); a **blacklight** (see callbacks).

## Variety (to carry 5 to 7 hours)
At 2 minutes a cycle, 5 hours is about 150 cycles. B3 starts to sag around hours 3 to 5. So:
- **A new zone and a new task set about every hour.**
- **Shift types** that change a cycle (Audit Day, Fire Drill, Visitor Day).
- Rank-ups that change what the tasks *are*.

## Lost callbacks (the owner is fine alluding extensively; the audience is people about 40 to 55)
Use allusions and invented names, not the show's names. Roughly: tool, zone, creature, world detail, ending.
- **The hatch and the button:** the console and its countdown. Day length can echo 108 (seconds). Numbers appear, nearly familiar, on rack serials.
- **The orientation film:** a cheerful presenter who appears under different names in different tapes, spliced and interrupted. The tapes are the cassettes.
- **The observation station:** a second, hidden station whose operator **watches and records everything you do**. Their notebook is a lore source.
- **The blast-door map:** hidden under normal light, revealed by the **blacklight** tool.
- **The smoke monster:** a **cooling fog** that "judges" you, with a mechanical clicking sound.
- **The polar bear in the jungle:** a **polar bear in the cold aisle**.
- **Supply drops and plain white-label goods:** every item in the facility is generically labeled ("BEER", "FOOD").
- **Named stations:** each zone is a "Station" with a symbol, a colored lanyard and a mission statement.
- **The stray dog:** a dog nobody owns wanders the halls.
- **The mystery album:** an invented band whose records appear everywhere.
- **The golf course:** a putting green on the roof (optional minigame).
- **The four-toed statue:** the founder's statue in the lobby has the wrong number of toes.
- **The lighthouse mirrors:** a rooftop mirror dial puzzle.
- **The two rival powers:** two competing systems or "founders"; an ending is choosing which to serve.
- **The failsafe key:** the console has a key you can turn; the ending where you do it.
- **The Others:** a rival company in the next building.

**Easter-egg policy:** nods should be layered. A player who never saw the show should still find each one funny or creepy on its own.

## Endings (placeholder)
Keep pressing forever; stop pressing (resist, then see what happens); hand the job to a replacement; go behind the final door; turn the failsafe key. Optional, spoiler-light, and reachable through the main loop without puzzles.

## Risks and decisions
1. **Is the loop fun with no story?** Build a vertical slice with placeholder art first: one room, a few tasks, the pull, the shop. Spend story and art money only after cycles 1 to 30 are fun.
2. **Taking control of the camera** must be short, skippable and comfortable.
3. **Cult satire is well-trodden** (Severance, Lost's own Dharma). The hook that is ours is the happy brand summoning something comically evil, and the pull itself.
4. **Story expectations:** B3's 39% of negative reviews cited predictable or bolted-on story, and 26% cited horror that did not deliver. Put the story in the tasks; call it occult comedy, not horror, in the store tags.
5. **Legal:** this is a commercial parody that borrows tropes and nods. Use invented names, no logos or show-specific text, and have a lawyer check the callbacks before release. (Not legal advice.)
6. **Cost:** months of work and an art budget, as the owner stated. Scope a first release at 3 zones and 2 endings before the full table.

## Open questions for the owner
- The cult's name (a placeholder is "the Uptime Foundation") and the AI's name.
- Cycle length (about 2 minutes) and the early-game feel.
- Whether the Others stay in a first version.
- Visual style: bright and cheerful mask over a dark hall, or darker throughout?
- Whether resisting the pull should unlock a late-game power fantasy, or stay a story beat.
