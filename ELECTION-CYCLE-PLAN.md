# Election Cycle — Full Design & Production Plan

> A 2.5D Pokémon-style political satire game. Companion documents:
> `SEASON-PLAN.md` (**product / seasons / itch then Steam** — wins on scope),
> `Election Cycle Redux.md` (S.O.C.I.A.L. stats, scenario ideas),
> `README.md` (engine).
>
> **Scope note:** Launch is Season 1 — 10 authored episodes = 10 playable levels
> on itch (donation / PWYW), not a Steam-first infinite roguelike. Use this file
> for systems (poll, ledger, days). Use `SEASON-PLAN.md` for what we actually ship.

---

## 1. Concept

### 1.1 One-sentence pitch

**"Pokémon, but the gym battles are debates."**

You walk a cozy pixel-art town, talk to voters instead of catching monsters, collect
issues and dirt instead of items, and the final boss is a live debate against a
procedurally generated opponent.

### 1.2 The fantasy

The player is a nobody thrust into a local election. The game is about the *social
combat* of small-town politics: charming strangers at their doorstep, surviving a
hostile interview, making promises you can't keep, and watching a nightly news
channel narrate your rise or collapse. The tone is **absurdist satire** — fictional
towns, fictional politics, no real parties or figures (this is deliberate: it keeps
the game evergreen, streamable, and safe from platform/content moderation issues).

### 1.3 Why roguelike

- A full election compresses beautifully into a **run**: fixed 7-day structure,
  randomized inputs (town, opponent, voters, events), one binary climax (win/lose).
- Runs are naturally replayable: a different S.O.C.I.A.L. build plays each day
  differently (a Charm build breezes through speeches but sweats the interview;
  a Strategy build sees traps coming but can't work a crowd).
- Failure is funny here, not frustrating. Losing an election to a philosophy
  professor who only speaks in metaphors is its own reward.

### 1.4 The signature mechanic: Promises are debt

To elevate the concept beyond "satire skin on a roguelike," one systemic twist:

**Every promise you make is tracked as a ledger entry.** Promising things is
powerful — it swings voter blocs instantly — but each promise is a liability:

- The opponent can weaponize your promises in the debate ("You promised free
  lawnmowers. Explain the math.")
- The media interview draws questions from your promise ledger.
- On voting day, NPCs you promised things to confront you.
- Contradictory promises (promise the HOA stricter rules AND promise renters
  freedom) create **scandal fuel** the event system can ignite.

This turns "lying" into a spendable resource with interest, and it makes the satire
*mechanical* rather than cosmetic. It is the thing reviewers will write about.

---

## 2. The Core Loop — One Run, Seven Days

A run is one town + one opponent + 7 in-game days. Target run length: **30–45 minutes**.

Every day has the same skeleton, which makes the game legible and pace-able:

```
Morning free-roam  →  Day's main event  →  Evening news report  →  Sleep
```

- **Morning free-roam**: walk the town, talk to NPCs, gather intel/items, make
  optional choices. Time-limited (a soft clock, e.g., a number of "actions" or a
  ticking morning timer).
- **Main event**: the scripted centerpiece of that day (registration, canvassing,
  speech, etc.). Entering it ends the morning.
- **News report**: full-screen TV scene recapping the day, showing poll movement,
  and introducing tomorrow's random modifier. This is the run's feedback loop and
  the game's signature screen.
- **Sleep**: save point, day increment, optional dream flavor text (hooks into the
  Dream Sequence scenario later).

### Day 1 — Registration (tutorial in disguise)

**Where:** Town Hall.

**What happens:**

1. Arrive in town via intro cutscene (bus stop / mom's driveway, per town template).
2. Free-roam teaches movement, talking, interactables — the existing framework
   already does all of this.
3. At Town Hall, a clerk NPC hands you the **candidacy registration form**. The form
   IS character creation: absurd bureaucratic questions map to allocating your
   S.O.C.I.A.L. points ("On a scale of 1–10, rate your ability to plan things.
   Answer honestly. We check.").
4. One randomized **registration obstacle** per town template (get a signature from
   the pigeon council; pay a fee in exact change; recite the town anthem) — a short
   fetch/social quest that tours the player around the map and introduces 3–4 key NPCs.
5. **Meet the opponent** in the lobby as they file their own papers. Scripted
   first-impression scene driven by the opponent archetype. Establishes the rivalry.

**Systems introduced:** S.O.C.I.A.L. stats, the day clock, the news report
(that evening's broadcast announces both candidacies and shows the initial poll:
you at a comedic low number).

### Day 2 — Canvassing (the mechanically richest day)

**Where:** the whole town, door to door.

**The minigame:** Knock on as many doors as possible before the clock runs out.
Each household is a generated encounter:

- The household has a **voter bloc** (see §4.3), a **disposition** (hostile /
  skeptical / neutral / warm), and one **issue card** drawn from the town's issue deck.
- The conversation is a short branching dialogue with **skill checks**:
  - **Outreach** gates extra dialogue options (you notice the fishing trophies →
    unlock the "talk fishing" approach).
  - **Charm** improves the success odds of friendly options.
  - **Strategy** reveals the household's bloc and disposition before you commit.
  - **Improvisation** gives one free recovery when a check fails.
  - **Luck** rolls a random event per door: dog bites you (lose time) vs. dog adopts
    you (viral photo, +Charm for the day); nobody home vs. surprise birthday party
    you get dragged into.
- **Outcomes per door:** shift that bloc's meter, learn an issue card (ammo for
  days 3–5), optionally **make a promise** (big immediate swing, ledger entry),
  or blow it (small negative swing, possible scandal fuel).

**Design intent:** this day teaches the player what the town cares about. Everything
learned here is ammunition for the speech, interview, and debate. Skipping doors is
a real cost later — the game should make that causality visible in the news report
("Candidate ignored the Riverside district entirely").

### Day 3 — Public Address

**Where:** town square, stage + crowd.

**The minigame:** two phases.

1. **Speech builder:** choose 3 talking points from your hand of cards — issue
   cards learned while canvassing (strong, targeted) plus generic promise cards
   (always available, weaker, and they hit the promise ledger).
2. **Delivery:** a timing/rhythm bar per talking point — hit the sweet spot to land
   the line. **Charm** widens the sweet spot; **Improvisation** converts a miss into
   a save ("...and THAT'S why I tripped on purpose"); **Authority** suppresses
   hecklers. Hecklers spawn from blocs whose issues you ignored, and shrink the bar
   while active.

**Outcome:** a crowd meter total → poll movement per bloc (targeted issues move
their bloc a lot; generic promises move everyone a little and add ledger debt).
The opponent holds their own rally the same day — the news report contrasts the two.

### Day 4 — Media Interview

**Where:** the local TV studio interior (new map).

**The minigame:** rapid-fire questions from a generated anchor. Timed
multiple-choice with a **spin-o-meter**:

- Question sources: the town issue deck, your **promise ledger** ("You promised
  free lawnmowers. How?"), your day 2–3 gaffes, and 1–2 pure absurdist curveballs.
- Some questions are **traps** (any direct answer loses; the right move is deflect
  or reframe). **Strategy** marks traps visually. **Improvisation** extends the
  answer timer. **Authority** unlocks the "refuse to answer, look powerful doing it"
  option.
- Answers are graded honest / spin / dodge / disaster. Disasters become headline
  cards AND get added to the opponent's debate ammo.

**Design intent:** this is the day your past choices come due for the first time —
the interview should feel like the game has been *watching you*.

### Day 5 — Debate (the boss fight)

**Where:** debate hall (new map), podiums, moderator, live audience meter.

**The minigame:** turn-based social combat, the deepest system in the game.

- Both candidates have **Composure** (HP) and the audience has a **Sway meter**
  (the actual win condition — you can "survive" the debate but lose the room).
- **Moves** (powered by stats + collected ammo):
  - **Attack** — spend dirt/opponent-gaffe cards. Authority scales damage.
  - **Deflect** — counter incoming attacks. Strategy predicts the opponent's next
    move type.
  - **Promise** — big Sway swing, adds to the ledger *live on stage* (the riskiest,
    funniest move — day 6–7 consequences are immediate and severe).
  - **Improvise** — wildcard, Improvisation-scaled: outcomes range from disaster
    to legendary zinger. Luck skews the roll.
  - **Appeal to issue** — spend issue cards learned canvassing; huge Sway with the
    matching bloc, nothing with others.
- **The opponent plays their own S.O.C.I.A.L. profile**: a 10-Authority incumbent
  tanks and intimidates; a 1-Strategy celebrity is erratic but Charm-bombs the
  audience; the philosophy professor's attacks are confusing (random type).
- Your unanswered promises and interview disasters are *pre-loaded into the
  opponent's hand*. The debate is where the whole week's ledger gets audited.

**Outcome:** final Sway → large poll movement going into voting day. A debate
blowout in either direction should feel run-deciding but not run-ending (Luck and
turnout still matter).

### Day 6 — Voting Day (the quiet day)

**Where:** the whole town, but campaigning is illegal near polls.

**Design intent:** tension through *powerlessness*. After five days of pulling
levers, the player can only walk and watch.

- NPCs react to your whole week: doors you knocked greet you, promises you broke
  glare, the dog that adopted you follows you around.
- **Luck events** fire: weather affects turnout per bloc; the vending machine union
  endorses someone at the last minute; a bus of college students arrives (or doesn't).
- **One meaningful choice:** where you spend the day (diner with locals / alone at
  HQ / helping someone with a non-political problem). Small final swing + flavors
  the ending.
- The day is short by design — a contemplative palate cleanser before results.

### Day 7 — Results

**Where:** Town Hall gathering, everyone you met is in the room.

- The count comes in **bloc by bloc on the news broadcast** (drama: blocs report in
  a suspense-maximizing order; close runs go to a recount gag).
- Win and lose cinematics per outcome, flavored by opponent archetype and your
  week's biggest moments.
- **Run summary screen:** final stats, poll graph across the week, key moments
  ("Promises made: 9. Promises the town remembers: 9."), dogs befriended, headlines
  generated, score, and unlocks.
- **Meta-progression** (see §5): unlock new backgrounds, perks, towns, opponents.

### The Nightly News Report (every evening)

The game's signature screen and primary feedback system. Full-screen TV scene:

- Channel logo, ticker, and a recurring **anchor character with a personality**
  (deadpan, slowly loses composure as the week gets weirder — this is your mascot).
- **Poll graph** per bloc, with today's movement animated.
- **2–3 headline cards** recapping your day, generated from actual events
  ("CANDIDATE BITTEN BY 4TH DOG THIS WEEK, POLLS RISE"). Every run should produce
  at least one screenshot-worthy headline — this is the marketing engine.
- **Tomorrow's modifier:** one random world event that changes the next day's rules
  (heat wave: shorter canvassing clock; opponent scandal: their poll dips but voters
  are cynical, all promises worth less).
- Skippable-but-summarized for repeat players (hold to skip → shows the compact recap).

---

## 3. What Randomizes Per Run

### 3.1 Town

Slot-based randomization on hand-authored templates — **not** full procedural layout
(full proc-gen town layout is enormous effort for variety the player barely reads;
slot-based gives 90% of the perceived variety for 10% of the work).

Each **town template** defines:

- A hand-authored map layout (existing JSON map format) with tagged slots:
  house slots, shop slots, landmark slots, the fixed civic buildings
  (Town Hall, TV studio, debate hall, town square, your HQ/home).
- A **name generator** (template-flavored word lists: "Maple Falls", "Crypton
  Valley", "Perfectly Normal Heights").
- An **NPC palette**: sprite palettes, name pools, occupation pools, dialogue voice.
- An **issue deck pool** (~10 issues; each run draws 4–6): potholes, HOA tyranny,
  the sentient vending machines, tuition, drone complaints...
- **Voter blocs** (see 3.3) and their relative sizes.
- A **registration obstacle** pool for Day 1.
- Which **opponent archetypes** can spawn here.

**Launch templates (3):** Hometown, College Town, Suburban (Scenarios 1–3 from
`Election Cycle Redux.md`). The remaining seven scenarios become post-launch content.

### 3.2 Opponent

Generated from an **archetype** + randomization:

- Archetype (launch set of 5, from the design doc): Forgetful Incumbent,
  Philosophy Professor, HOA Chair, Factory Foreman, Tech CEO.
- A generated name, portrait (palette + feature swap on a base sprite), and
  **S.O.C.I.A.L. profile** (archetype defines the shape, e.g., "Authority 8–10,
  Improvisation 1–3"; exact values roll per run).
- One **gimmick** — a special rule they impose on the run (the Professor makes one
  interview question incomprehensible; the Tech CEO deepfakes one of your headlines).
- A **dirt pool** — discoverable scandal cards the player can dig up during
  free-roam (their debate ammo works the same way against you).
- Their own campaign runs in the background: each day the opponent takes an action
  (visible in the news report), moving polls without player input. Simple scripted
  AI — a per-archetype weighted table, not a simulation.

### 3.3 Voter blocs

Each town has 3–5 blocs (e.g., College Town: Students, Faculty, Townies, The Frat).
Each bloc has:

- A size (share of the electorate), a starting lean, an issue affinity map,
  and a turnout tendency.
- One **bloc leader NPC** — a shortcut character whose personal quest can swing the
  whole bloc (high-risk, high-reward alternative to door-by-door grinding).

The poll is just the weighted sum of bloc meters + noise. Simple, legible, and the
news graph can show it honestly.

### 3.4 Event deck

Hand-written absurd events with randomized timing, targets, and magnitude — scripted
content, random delivery. Types:

- **World events** (tomorrow's-modifier slot in the news): weather, festivals,
  economic gags.
- **Scandal events**: fire only if fuel exists (contradictory promises, interview
  disasters, discovered dirt). Scandals without fuel never fire — consequences must
  always be traceable to choices, or the satire reads as random punishment.
- **Luck events**: personal moments during free-roam, weighted by the Luck stat in
  both directions.

---

## 4. Systems Design Details

### 4.1 S.O.C.I.A.L. stats

As defined in `Election Cycle Redux.md`. Mechanical summary of where each stat bites:

| Stat | Day 2 Canvass | Day 3 Speech | Day 4 Interview | Day 5 Debate | Passive |
|---|---|---|---|---|---|
| **Strategy** | reveal bloc/disposition | see heckler forecast | trap questions marked | predict opponent moves | see poll math details |
| **Outreach** | extra dialogue options | +crowd size | — | +Appeal-to-issue power | NPCs share more intel |
| **Charm** | friendly-option odds | wider timing sweet spot | +spin success | +Sway from all moves | better first impressions |
| **Improvisation** | one free failed-check recovery | miss → save | +answer time | Improvise move quality | recover from Luck disasters |
| **Authority** | intimidate option | suppress hecklers | refuse-to-answer option | +Attack damage, Composure | some NPCs obey, some resent |
| **Luck** | door RNG skew | crowd RNG skew | curveball leniency | Improvise roll skew | all event-deck rolls |

Point-buy at registration: e.g., 24 points across 6 stats, min 1 / max 8 at start
(9–10 reachable only via run events and meta unlocks, keeping the top of the scale
aspirational). Numbers here and throughout are starting guesses for tuning.

### 4.2 The Promise Ledger

- Data: `{ promiseId, text, targetBloc, day, magnitude, contradicts: [] }`.
- Sources: canvass dialogue, speech cards, debate Promise moves.
- Consumers: interview question generator, opponent debate ammo, day 6 NPC
  reactions, scandal-event fuel check, run summary.
- Contradictions are computed from issue-card metadata (each issue card lists which
  stances oppose it) — no NLP, just tags.

### 4.3 Polling model

Keep it embarrassingly simple and fully legible:

```
bloc_meter[b] ∈ [-100, +100]           // per-bloc sentiment toward you
poll = Σ ( bloc_size[b] × sigmoid(bloc_meter[b]) × turnout[b] ) + noise(Luck)
```

Every gameplay outcome writes a *visible, attributed* delta to a bloc meter, and
the news graph replays the day's deltas. Players must always be able to answer
"why did my number move?" — that legibility is what makes runs feel fair and
mastery feel real.

### 4.4 Difficulty & fairness

- The opponent's background campaign gives the run a pacing floor — doing nothing
  loses slowly and visibly.
- Rubber-banding: none on the poll itself, but the event deck weights *dramatic*
  events (not helpful ones) when the race is a blowout in either direction.
- A lost run should end with the summary screen making the causes legible — the
  roguelike promise: "I know exactly what I'll do differently next run."

---

## 5. Meta-progression (between runs)

Light, cosmetic-leaning, roguelike-standard:

- **Backgrounds** (unlockable starting classes): Ex-Salesperson (+Charm, promise
  penalty ×1.5), Retired Teacher (+Authority with parents bloc), Conspiracy Blogger
  (starts with 2 dirt cards, media hates you)...
- **Perks** (New Vegas-style, per the design doc's suggestion): earned by run
  achievements ("Win without making a single promise" → unlock *Honest Face*).
- **Unlockable towns and opponents**: the remaining 7 scenarios from the design doc
  roll out as unlockable/post-launch templates, roughly in the doc's order, ending
  with Capital City and its procedurally-built "perfect counter" super-candidate.
- **The Archive**: a gallery of every generated headline, opponent, and run summary
  the player has produced. Cheap to build, beloved in practice.

**Story mode** (the full 10-scenario "Unlikely Mayor Tour" arc with the Dream
Sequence midpoint) is a **post-launch campaign mode** stitching runs together with
persistent character and interstitial scenes — explicitly out of launch scope, and
designed so launch systems need no rework to support it (RunState already
serializes; story mode is a sequence of parameterized runs + cutscenes).

---

## 6. Engineering Plan (mapped to the Pocket Town codebase)

### 6.1 What the framework already provides

Grid movement, y-sorted 2.5D rendering, virtual-resolution upscaling, JSON maps
with warps/NPCs/interactables, wandering NPCs, typewriter dialogue, scene stack,
procedural art/audio, input (keyboard + gamepad). See `README.md`.

### 6.2 New architecture

```
src/PocketTown/
  Sim/
    RunState.cs         // THE SPINE: stats, day/phase, blocs, ledger, decks,
                        // opponent, event log. Serializable (save/resume).
    DayManager.cs       // Phase state machine: Morning → Event → News → Sleep
    PollModel.cs        // bloc meters, poll computation, delta log
    PromiseLedger.cs
    EventDeck.cs        // world/scandal/luck events, fuel checks, scheduling
    OpponentAI.cs       // per-archetype daily action table + debate move logic
  Gen/
    TownGenerator.cs    // template + slot filling: names, NPCs, issues, blocs
    OpponentGenerator.cs
    HeadlineGenerator.cs// templated headline strings from event log
  Scenes/
    NewsReportScene.cs  // the signature screen
    SpeechScene.cs
    InterviewScene.cs
    DebateScene.cs
    ResultsScene.cs
    RunSummaryScene.cs
  UI/
    ChoiceBox.cs        // DialogueBox + selectable options + skill-check display
    Meter.cs, PollGraph.cs, CardHand.cs, Panel.cs
  Data/
    Towns/              // town templates (extends existing map JSON)
    Archetypes/         // opponent archetype JSON
    Issues/, Events/, Dialogue/
```

### 6.3 The build order (dependency-driven, no dates)

Ordered by what unblocks what — each step produces something playable:

**Step 1 — The spine.** `RunState` + `DayManager` + save/load. Wire `WorldScene`
to the day clock. A "run" exists: 7 days tick past with placeholder events, sleep
saves, and a debug poll number moves. *Nothing else starts before this.*

**Step 2 — Dialogue upgrade.** Extend `DialogueBox` into `ChoiceBox`: selectable
options, S.O.C.I.A.L. skill checks with visible odds, variable substitution
(`{townName}`, `{issue}`, `{opponentName}`), and outcome hooks that write to
`RunState`. This is the single biggest framework change and days 1, 2, 4, and 6
all depend on it — do it immediately after the spine.

**Step 3 — A walking skeleton of the full week.** Stub every day: registration is
one form dialogue, canvassing is 3 hardcoded doors, speech/interview/debate are
single choice screens, news report is text-only, results reads the poll. **The
entire 7-day run is now playable end-to-end in ~10 minutes.** From here on, the
game is always shippable and every subsequent step is "deepen one day."

**Step 4 — Canvassing for real.** Household generation, disposition/bloc/issue
encounters, the Luck door events, the promise hook, the day clock. First real
content-authoring pass (dialogue tables).

**Step 5 — Debate for real.** The full turn-based system, opponent AI profiles,
ammo economy, Sway/Composure UI. Canvassing + debate are the two days that carry
the game — they get depth *before* the other days get any.

**Step 6 — News report for real.** Anchor character, poll graph animation,
headline generator, tomorrow's-modifier reveal. Build it right after the two core
days exist so their outputs have a stage.

**Step 7 — Speech & interview.** Deliberately built on one shared system (timed
choice + card hand + meter) with different skins and stat hooks — two minigames
for roughly the cost of one.

**Step 8 — Randomization pass.** TownGenerator (3 templates), OpponentGenerator
(5 archetypes), EventDeck, issue decks, voting-day events, bloc leaders.

**Step 9 — Results, summary, meta.** Bloc-by-bloc count drama, endings,
run summary, unlocks, the Archive.

**Step 10 — Juice & polish.** SFX/music passes (extend the synth `AudioBank`, or
real audio), screen shake, crowd noise, transitions, gamepad polish, settings menu,
difficulty options, accessibility (timers-off mode — important: several minigames
are timed).

**Step 11 — Release engineering.** itch.io demo build → feedback loop → Steam SDK
(achievements, cloud saves), store page assets, localization pass on UI strings
(keep all player-facing text in data files from Step 2 onward to make this cheap).

### 6.4 Technical risks & mitigations

- **Content volume is the real cost**, not code. Dialogue, events, and headlines
  are all data-driven from day one (JSON/tables, hot-reloadable in debug builds)
  so writing is decoupled from programming.
- **Procedural art ceiling.** The generated-art pipeline is perfect for development
  but the Steam capsule and trailer need real art. The framework already isolates
  this: everything goes through `Art.TileFrames()` / `Art.CharacterSheet()` —
  swapping in real sprites later touches one file. Budget for a portrait set
  (anchor, opponents, key NPCs) first; portraits carry satire better than 16px
  sprites can.
- **Tuning timed minigames** for both keyboard and gamepad across skill levels —
  playtest early (Step 3's skeleton exists partly for this) and ship assist options.
- **Scope gravity.** The design doc's 10 scenarios will constantly tempt expansion.
  The rule: no new town template until all seven days are deep on the existing three.

---

## 7. Assessment & Ratings

Each rated 1–10 **as it stands today**, with the path to 10.

### 7.1 Concept — 8/10

**Why 8:** Genuinely underserved niche. Political games exist (Democracy is a
spreadsheet, Stardew-likes are apolitical, satire games are mostly visual novels),
but a *cozy 2.5D creature-collector-styled election roguelike* does not. The 7-day
structure is tight and legible. The nightly news report is a memorable, ownable
hook. "Debate as boss fight" writes its own trailer.

**Why not 10 yet:** As documented, the concept is a tone + a stat system + a story
arc; the *core loop* lives only in this document. And satire without a mechanical
spine reads as a reskin.

**Path to 10:**
1. Commit to the one-sentence pitch and say it everywhere.
2. Build the **promise ledger** (§1.4) as the signature system — satire you can
   *play*, not just read.
3. Make the news anchor a real character — the mascot on the capsule art.
4. Keep it fictional-absurdist forever. Real-politics adjacency is a trap for
   this game (dated in 2 years, demonetization-adjacent for streamers, and less
   funny than frogs raining on rallies).

### 7.2 Scope — 5/10

**Why 5:** The 7-day loop itself is well-scoped — seven set-pieces, three town
templates, five opponents is a shippable indie game. But the surrounding ambition
is not sequenced: the 10-scenario story mode stapled to launch is a multi-year
project, and four bespoke minigames means four systems to design, tune, and teach.

**Path to 10:**
1. **Launch scope = one great run.** 7 days, 3 towns, 5 opponents, meta unlocks.
2. Only **two deep minigames** (canvassing, debate). Speech and interview share
   one lighter system (§6.3 Step 7).
3. Story mode is the *post-launch roadmap*, not the launch blocker — it becomes
   your update calendar and keeps the game alive for a year after release.
4. Adopt the walking-skeleton discipline (§6.3 Step 3): the game is always
   playable end-to-end, and depth is added one day at a time. This makes scope
   cuttable at any moment without leaving half-systems.
5. Write the 7-day loop into the design doc (this file now does that).

### 7.3 Steam candidacy — 7/10

**Why 7:** Yes, this is a Steam game. Short-run roguelike + comedy + high
clip-ability is a proven Steam shape (Reigns, Peace Death, Papers Please-likes,
Going Under). Systemic humor generates streamer content, and streamer content
sells this exact kind of game. The election theme gives it a discoverability hook
most pixel roguelikes never get.

**Why not 10 yet:** Pixel-art roguelikes are the most crowded shelf on Steam;
procedural placeholder art cannot carry a store page; and there is no wishlist
pipeline yet.

**Path to 10:**
1. **Real art where it counts**: capsule, anchor + opponent portraits, news-screen
   dressing, 5 strong screenshots, a 30-second trailer built around the debate and
   a scrolling wall of generated headlines.
2. **Steam page live before any public build** — wishlists compound; every week
   the page exists matters more than any single marketing beat.
3. **Free demo into Steam Next Fest.** This game demos perfectly: one full run is
   20–40 minutes and ends on a cliffhanger ("unlock 2 more towns...").
4. Build the game to be **clipped**: share/export button on the run summary and on
   individual headlines. Every streamer clip is an ad with the game's name on it.
5. Price in the $6.99–$9.99 band at launch; this concept and content volume
   support paid, and free would waste the theme's commercial moment.

### 7.4 Releasing around US elections — marketing idea 8/10

*(Per your note, no time constraints assumed — this rates the strategy, not a
deadline.)*

**Why 8:** Election cycles produce a massive, recurring, *scheduled* spike in
attention, search traffic, and streamer appetite for on-theme content — a free
marketing wave most indies would kill for, and it recurs every two years (US
midterms and presidentials, plus other countries' elections in between).

**Why not 10:** the wave is short (roughly 3 weeks), crowded with news, and a
*rushed launch* into it burns the one launch a game gets. The wave rewards games
that are already visible, not games that appear during it.

**Path to 10 — surf the cycle, don't chase it:**
1. **Decouple launch quality from the calendar.** Ship when the run is great.
2. Use whichever election window is *next after the game is demo-ready* for the
   **demo + wishlist push** ("Run your own election before the real one"), and a
   later cycle for launch or the biggest update/discount. Two waves > one.
3. Presidential cycles are far bigger waves than midterms — a game good enough to
   wait for one is worth more than a game rushed for the other.
4. Evergreen framing in all copy: the game is about *elections*, not *an*
   election. Fictional satire means the page never expires and non-US players
   aren't excluded — local/mayoral framing travels globally.
5. Off-cycle, lean on the genre hooks instead (roguelike festivals, cozy-game
   showcases, satire/comedy game roundups).

### 7.5 Revenue potential — Election Cycle 6/10 vs. Ratna Bay 3/10

**Election Cycle — 6/10.** Realistic indie math: a charming $7–10 satire roguelike
with a good demo, 7k+ wishlists at launch, and streamer pickup lands in low-four to
low-five figures gross on Steam; a breakout clip or a well-timed election-cycle
wave raises the ceiling meaningfully. Its commercial advantages are rare for a
first Steam release: a one-sentence pitch a journalist can retell, a built-in
recurring marketing calendar, and systemic humor that manufactures shareable
screenshots every run.

**Ratna Bay — 3/10** ([itch.io](https://datathecodie.itch.io/ratna-bay)): free,
first-person 3D roguelite dungeon crawler — the single most crowded genre on
itch/Steam — with fully procedural art and audio, in early alpha. The
door-decision mechanic ("bank it or open it") is a genuinely good hook, but as a
*revenue* product it currently has no price, no visual identity to market, and no
discoverability angle beyond genre tags. Its realistic ceiling without major
reinvestment is portfolio value and playtesting insight, which is exactly what its
own page says it's for — that's a fine thing for an alpha to be, it just isn't revenue.

**Path to 10 (Election Cycle):**
1. Everything in §7.3 (wishlist pipeline, demo, Next Fest, clip-ability).
2. Post-launch scenario drops (§5) to re-spike visibility — each new town is a
   news beat, a discount moment, and a streamer return visit.
3. Run-summary image export + daily-challenge seed (same town/opponent for
   everyone, shared leaderboard) for community-driven marketing.
4. Localize: political satire about *fictional* small towns travels; UI-first
   localization is cheap if strings live in data files from the start (§6.3).

**Path to a better number (Ratna Bay):**
1. Keep it free and in the open — it is your public feedback lab and credibility.
2. Give it one strong visual signature (even one hand-made key art + a consistent
   palette grade over the proc-gen) before any paid ambitions.
3. Cross-promote: Ratna Bay's page and builds should link Election Cycle's Steam
   page the day it exists. An existing itch audience, however small, is a seed.
4. Only reinvest seriously if the door mechanic shows organic pull (players
   telling you about hesitation unprompted — the exact feedback its page asks for).

### 7.6 Ratings summary

| Dimension | Today | Achievable | The one lever |
|---|---|---|---|
| Concept | 8 | 10 | Promise ledger — make the satire mechanical |
| Scope | 5 | 10 | Launch = one great run; story mode = roadmap |
| Steam candidacy | 7 | 10 | Real art on the page + demo + wishlists first |
| Election-cycle release | 8 | 10 | Surf a cycle with the demo; launch when it's great |
| Revenue (Election Cycle) | 6 | 10 | Clip-ability + post-launch scenario cadence |
| Revenue (Ratna Bay) | 3 | — | Keep free; feedback lab + cross-promo channel |

---

## 8. Immediate Next Steps

1. **Step 1 of the build order**: `RunState` + `DayManager` + save/load on top of
   the existing scene system.
2. **Step 2**: upgrade `DialogueBox` → `ChoiceBox` with skill checks.
3. **Step 3**: the stubbed 7-day walking skeleton — after this, every work session
   ends with a playable election.
4. In parallel, on paper: write the launch issue decks (3 towns × ~10 issues) and
   the 5 opponent archetype sheets (S.O.C.I.A.L. profiles + gimmicks + dirt pools),
   since content authoring is the long pole.

The single most important discipline: **nothing new gets built until the 7-day
loop is fun with stub content.** Art, story mode, Steam — all of it waits on that.
