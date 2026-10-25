# Game file template

Save as `Docs/competitive/NN-slug.md` (`pass_check.mjs` prints the next number). Pass 1 fills the top sections; Pass 2 fills the rest and updates the Coverage table. Every number gets a unit and a date. No marketing language. Every claim traces to the page, a patch note, a review, a thread or a screenshot.

```markdown
# Game Name

*One italic paragraph: the research date; which pass(es) were run; what was read (store page, N news posts, N reviews, which Reddit threads, which screenshots); that **nothing was played by Claude**; what the owner has played, if anything. Evidence tags used: [store], [patch notes], [player report], [dev], [screens], [inference].*

- **Steam:** link (App ID) [+ demo App ID]
- **Release:** date (and console or other platform dates)
- **Price:** list price (and the sale price if discounted, with the date seen)
- **Reviews (YYYY-MM-DD):** score bucket, total, % positive (pos / neg), English subtotal
- **Developer/publisher:** and team size if known
- **Platform:** OS, players, languages, controller, accessibility flags
- **Store tags:** user tags from the store page
- **Generative-AI disclosure:** what the page says (or "none on the page")

## Coverage and pass log
| Layer | Pass 1 (cursory) | Pass 2 (deep) | Where |
|---|---|---|---|
| Store facts, tags, screenshots, trailer | done YYYY-MM-DD | done / not done | header |
| Review totals (all languages, English) | done | done | header, tracker |
| Top reviews | done | done (all N read or counted) | "What players love / push back on" |
| News and patch notes | done (titles) | done (full text) | "Patch / update history" |
| Steam community threads | not done | titles and N threads read | "Mechanics" |
| Reddit: dedicated or shared sub | done | partly / done | "Community and platforms" |
| Discord and social accounts | done | n/a | "Community and platforms" |
| Other platforms (web builds, consoles, mobile) | done | n/a | "Community and platforms" |
| Creator coverage | done (names) | not done | "Community and platforms" |
| Owner's hands-on impressions | pending | pending | last section |

Use these exact layer names: `pass_check.mjs` reads this table. A cell starting with "done", "partly", "titles", "read" or a date counts as done; "not done" and "pending" count as not done; "n/a" means there is no deep layer.

## Verdict (Pass 1)
Two or three sentences: what it is, how players reacted, why it might matter to us.

## Premise and scale
## Mechanics and abilities          (verbs, upgrade ladder as a table; evidence tags)
## What the screenshots show        (numbered; interpretation marked)
## Patch / update history           (dated table)
## What players love / push back on (counts marked approximate, quoted themes with hours and votes)
## Community and platforms          (Reddit map, Discord and social, web builds and consoles, creators)
## How it differs from / resembles our design   (short table)
## Why it matters to us             (numbered: borrow, avoid, risks marked as guesses)
## Open questions
## Kolby's / the owner's impressions  (verbatim quotes; "(not yet played)" if empty)
## Screenshot index                 (table: # | source or file | what it shows)
```
