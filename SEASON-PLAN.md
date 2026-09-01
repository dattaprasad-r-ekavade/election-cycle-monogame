# Election Cycle — Season Plan

**Hook:** Each episode is a South Park. Each episode is also a level.

We write a short satire of one current thing, set in a fictional town. That same week — same people, same jokes, same stations — is a playable campaign. Watch it or play it. The footage is the level; the level is the episode.

This is the production plan. Systems detail (S.O.C.I.A.L., poll math, engine) still lives in `ELECTION-CYCLE-PLAN.md` and `Election Cycle Redux.md`. Where those docs assume a Steam-first roguelike, **this file wins for scope and release.**

---

## 1. What we are making

A **season of episodes**, not an open-ended roguelike.

- **YouTube:** a sitcom / limited series shot in-engine (Red vs Blue using Halo as a set, South Park using a town as a set). Not a Let’s Play. Not a trimmed playthrough of every door.
- **Game:** each episode is one **level** — you play that week as the candidate.
- Recurring shape, new plot every time. Like South Park: same town energy, new issue, new ending gag.

The show is canon-adjacent entertainment in this world. The game is you being the candidate. They share a town, a cast, and a beat sheet. They do **not** share a full uploaded dialogue tree as “the episode.”

---

## 2. One episode = one level

Every episode / level uses the same stations:

| Beat | On screen / in play |
| --- | --- |
| Cold open | WMAP-7 TV: election called, candidates shown |
| Register | Protagonist watches, then files papers |
| Canvass | Meet people, door-to-door; main joke engine |
| Interview | Character says something absurd; one soundbite |
| Speech | Public address; one wacky incident |
| Debate | Vs the rival; more set-piece comedy |
| Results | **Both** “serious” candidates lose. A **3rd-party underdog** wins (pigeon, vending-machine union, someone’s dog, the intern). Freeze-frame. Episode over. |

No quiet “voting day” on YouTube. Target length for the show: about 15–18 minutes.

**Episode 1 plot (only):** AI vs non-AI as the *issue of the week* — e.g. incumbent hates “the machines,” you want a talking clerk at Town Hall. Fictional nouns, no real companies, parties, or politicians. Later episodes get new issues (HOA, mill, campus, tourists…). The franchise is not “the AI game.”

### Show vs game on the ending

- **Show:** the 3rd party always wins. That is the series button.
- **Game:** the player can still win sometimes. Otherwise there is no victory to chase. Show canon = “the week it went weird.” Play = try not to be that week.

---

## 3. Seasons and platforms

### Season 1 — itch.io, donation / PWYW

- **10 episodes = 10 levels.**
- Ship a level when that episode is watchable, not when all ten exist as empty slots.
- If writing stalls, **6 + a finale still counts as a finished S01.** Empty 7–10 help nobody.
- Itch page: this is the pilot season of the show; each episode is playable.
- Do not wait for a US election calendar. Current affairs are seasoning inside a fictional town.

### Season 2 — Steam, only if S01 demand is real

New episodes / levels, paid store. Extra systems (deeper randomization, more toys) belong here, not as a blocker for S01.

**Demand that counts**

- Strangers download or donate
- Someone replays a level
- Someone asks for the next episode
- A clip spreads without us posting it

Views with no installs = we have a show, not yet a game. That is still useful; it is not a Steam greenlight.

---

## 4. How an episode gets made

One content unit, two outputs:

1. **Write** the week (issue, rival, underdog, 2–3 canvass bits, one interview bite, one speech gag, one debate set piece, results tag).
2. **Playable level** in the engine (those stations, that cast, that map).
3. **Episode** directed in-engine: TV set first, cinematic cameras, VO, cuts. Original sitcom in this town — not HUD-off gameplay of every interactable.
4. **Upload + itch** the same day if we can. Description: “This is the town. You’re the candidate.”

Red vs Blue rule: the war (the election) is always on; the episode is the bits in the canyon; the game is when you pick up the rifle.

---

## 5. S01 episode slate (working)

Titles are placeholders. Swap freely; keep the **stations** and the **3rd-party tag**.

| Ep | Working title | Issue spine | Rival flavor | Underdog (tag) |
| --- | --- | --- | --- | --- |
| 1 | The Talking Clerk | Machines vs mill / kiosk at Town Hall | Forgetful incumbent | Vending-machine union |
| 2 | HOA Wars | Lawn law | Clipboard chair | The goose |
| 3 | Campus of Chaos | Everything is a protest | Metaphor professor | Unaccredited frat poll |
| 4 | Mill Closing | Jobs, smoke, nostalgia | Beloved foreman | Pigeon council |
| 5 | Tourist Season | Locals vs feral visitors | Travel vlogger | Gift-shop coalition |
| 6 | App Town | City Hall is a glitchy app | Tech CEO | The intern |
| 7 | (open) | Current-affairs riff | — | — |
| 8 | (open) | Current-affairs riff | — | — |
| 9 | (open) | Current-affairs riff | — | — |
| 10 | Finale | Callbacks from 1–9 | — | Someone’s mom / the dog |

Episodes 7–9 stay blank until 1–6 are written. Do not pre-build maps for unwritten plots.

---

## 6. Build order (S01)

1. Retune the existing 7-day stub to this beat sheet: **TV → register → canvass → interview → speech → debate → results.** Add a 3rd-party bloc that can steal the count.
2. **Level 1 / Episode 1** playable end-to-end (Hometown, Quince, kiosk, underdog win). Ugly art is on-brand for local news.
3. In-engine “director kit”: hide HUD, camera poses, play a line, export. Enough to shoot a cold open.
4. Record Episode 1. Put the itch build in the description the same week.
5. Repeat: write episode N → playable level N → shoot N. Cadence over a dump of ten.

Do not start Season 2 systems, Steam, or a 10-town roguelike until S01 has strangers.

---

## 7. What this is not

- Not a Let’s Play of the finished game
- Not “build everything, then film”
- Not a midterm 2026 launch plan
- Not locking the player into pro-AI after Episode 1
- Not uploading the full playable questline and calling it a cinematic

---

## 8. One-line pitch (use everywhere)

**Election Cycle — a South Park season you can play. Each episode is a level.**
