# Test prompts for the game-research skill

Not yet run. Each prompt is paired with what a good result looks like. The skill creator's formal eval loop spawns subagents, so these are kept as plain prompts until the owner asks for that loop.

1. **Cursory pass, new game**
   Prompt: "Quick look at Coup de Grass, I saw Idle Cub cover it. Don't spend a lot."
   Good result: starts by running `pass_check` and saying "Pass 1 (cursory): no file exists"; runs `quick_facts`, a review summary, a Reddit map (guesses a subreddit name first, one tally search), social links and other platforms; writes `NN-coup-de-grass.md` with a Coverage table (Pass 2 columns not done), a README row and a tracker block; never opens or plays a demo; reports in a short summary and ends with one question and an offer of Pass 2.

2. **Deep pass, file exists**
   Prompt: "Now go deeper on Berry Bury Berry, I want to know what the Steam threads say about the upgrade shop."
   Good result: says "Pass 2 (deep): file found, reusing it"; does not re-fetch store facts, totals or reviews; reads Steam threads and updates only the Coverage rows it touched; quotes with votes and hours; labels inference; no sales numbers.

3. **Survey plus channel scan**
   Prompt: "What are the top incremental games released in the last two years, and which ones is Idle Cub covering right now?"
   Good result: merges three Steam listings with `listing_merge` (reports the coverage check), notes counts are English-only, separates real incrementals from tag-abuse games, parses the channel with `yt_channel --desc`, produces tables with no sales figures, and offers quick looks at the closest to the owner's brief.

4. **Rule check (should not play)**
   Prompt: "Is the Sludgineers demo any good? Just try it for ten minutes and tell me."
   Good result: declines to play, explains it researches from public data and will ask for the owner's impressions, and offers a Pass 1 on Sludgineers instead.
