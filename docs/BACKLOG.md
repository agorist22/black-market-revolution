# Backlog

Prioritized by system. **P0** = vertical slice blockers. Cap ~40 items. Format: ID | title | priority | depends-on | acceptance criteria.

---

## P0 — Vertical slice (lead with these)

### VS-01 | Runnable game loop + project entrypoint
- **Priority:** P0
- **Depends-on:** —
- **Acceptance:**
  - Documented launch path starts the Pygame window
  - Loop runs until quit; no hard hang on idle

### VS-02 | WASD player movement + camera follow
- **Priority:** P0
- **Depends-on:** VS-01
- **Acceptance:**
  - Player sprite moves on map with WASD
  - Camera keeps player in view on large map

### VS-03 | Map / one-district bounds
- **Priority:** P0
- **Depends-on:** VS-01
- **Acceptance:**
  - Playable region is a clear district (or clipped 2000×2000 subset)
  - Collision prevents walking through solid geometry

### VS-04 | Single currency wallet (earn + spend)
- **Priority:** P0
- **Depends-on:** VS-01
- **Numbers:** `GREY-MARKET-SLICE.md` §4 / §13 (crypto-only for slice)
- **Acceptance:**
  - HUD shows balance
  - At least one earn source and one spend sink work in-session

### VS-05 | Buy one property + passive income tick
- **Priority:** P0
- **Depends-on:** VS-04, VS-03
- **Acceptance:**
  - Player can purchase one marked property if funds suffice
  - Ownership persists in-session; income accrues on a timer/tick

### VS-06 | One mission type (delivery or smuggling)
- **Priority:** P0
- **Depends-on:** VS-02, VS-04
- **Acceptance:**
  - Accept → objective → success/fail with currency reward/penalty
  - Mission state cannot soft-lock the map

### VS-07 | Basic wanted level + police reaction
- **Priority:** P0
- **Depends-on:** VS-02, VS-03
- **Acceptance:**
  - Committing a flagged crime raises wanted
  - At least one police chase or combat engage occurs

### VS-08 | NAP reputation hook
- **Priority:** P0
- **Depends-on:** VS-07
- **Acceptance:**
  - Unprovoked attack on civilians/neutrals lowers reputation
  - Defensive combat vs police/raiders does not apply the same penalty

### VS-09 | Vertical-slice debug / reset
- **Priority:** P0
- **Depends-on:** VS-05, VS-06
- **Acceptance:**
  - Dev can reset district state without restarting the machine
  - Slice loop completable in 10–15 minutes

---

## Economy

### ECO-01 | Dual wallets: fiat + crypto
- **Priority:** P1
- **Depends-on:** VS-04
- **Acceptance:**
  - Both balances display and update independently
  - At least one activity pays in each currency

### ECO-02 | Fiat inflation / controlled devaluation
- **Priority:** P1
- **Depends-on:** ECO-01
- **Acceptance:**
  - Fiat purchasing power or prices drift over time
  - Effect is visible in UI or price tags

### ECO-03 | Crypto risk profile
- **Priority:** P1
- **Depends-on:** ECO-01
- **Acceptance:**
  - Crypto has freer spend/use in underground contexts
  - At least one downside (volatility, seizure risk, or crackdown)

### ECO-04 | Property type: legal
- **Priority:** P1
- **Depends-on:** VS-05
- **Acceptance:**
  - Legal property: lower risk, taxed, stable income band

### ECO-05 | Property type: underground
- **Priority:** P1
- **Depends-on:** VS-05, VS-07
- **Acceptance:**
  - Higher income potential; raid/wanted risk on ownership or operation

### ECO-06 | Property type: mutual aid
- **Priority:** P1
- **Depends-on:** VS-05
- **Acceptance:**
  - Lower cash yield or shared benefit; Agorist influence gain hook

### ECO-07 | Tax evasion pressure
- **Priority:** P1
- **Depends-on:** ECO-04, ECO-05
- **Acceptance:**
  - Evasion choice increases State heat or wanted chance
  - Paying tax reduces risk

### ECO-08 | Currency conversion / parallel pricing
- **Priority:** P2
- **Depends-on:** ECO-01
- **Acceptance:**
  - Player can convert or see dual prices without exploits that infinite-money in one action

---

## Reputation & NAP

### NAP-01 | Reputation score model
- **Priority:** P1
- **Depends-on:** VS-08
- **Acceptance:**
  - Single readable rep metric with gain/loss rules documented in code comments or design note

### NAP-02 | Aggression vs defense classifiers
- **Priority:** P1
- **Depends-on:** NAP-01, VS-07
- **Acceptance:**
  - Combat initiators tagged; police/assassin/raid defense exempt from NAP penalty

### NAP-03 | Rep gates mission/NPC access
- **Priority:** P2
- **Depends-on:** NAP-01
- **Acceptance:**
  - Low rep blocks or worsens at least one mission/NPC interaction

---

## Politics & factions

### POL-01 | State vs Agorist influence scores
- **Priority:** P1
- **Depends-on:** VS-08
- **Acceptance:**
  - Scores update from player economic/criminal/political actions
  - HUD or pause screen shows both

### POL-02 | ~90% dominance win/lose
- **Priority:** P1
- **Depends-on:** POL-01
- **Acceptance:**
  - Crossing threshold ends run with clear outcome screen

### POL-03 | Three politicians
- **Priority:** P1
- **Depends-on:** POL-01
- **Acceptance:**
  - Three named actors with distinct preferred policies

### POL-04 | Election / policy change cycle
- **Priority:** P1
- **Depends-on:** POL-03
- **Acceptance:**
  - At least one cycle can change active law set mid-run

### POL-05 | Policy: drug legalization
- **Priority:** P2
- **Depends-on:** POL-04, ECO-05
- **Acceptance:**
  - Legalization alters underground income/risk or mission availability

### POL-06 | Policy: UBI
- **Priority:** P2
- **Depends-on:** POL-04, ECO-01
- **Acceptance:**
  - UBI grants fiat income and shifts faction scores or inflation pressure

### POL-07 | Policy: wars / conflict
- **Priority:** P2
- **Depends-on:** POL-04
- **Acceptance:**
  - Conflict event raises heat, prices, or mission types

### POL-08 | Policy: crypto crackdowns
- **Priority:** P1
- **Depends-on:** POL-04, ECO-03
- **Acceptance:**
  - Crackdown restricts crypto use or raises seizure risk while active

---

## Missions & sandbox

### MIS-01 | Delivery mission variants
- **Priority:** P1
- **Depends-on:** VS-06
- **Acceptance:**
  - ≥3 delivery instances with distinct pickups/dropoffs

### MIS-02 | Smuggling missions
- **Priority:** P1
- **Depends-on:** VS-06, VS-07
- **Acceptance:**
  - Smuggling raises wanted risk on fail/detection; pays better

### MIS-03 | Recruitment missions
- **Priority:** P2
- **Depends-on:** POL-01
- **Acceptance:**
  - Completing recruitment shifts Agorist influence

### MIS-04 | Optional mission board (sandbox)
- **Priority:** P2
- **Depends-on:** MIS-01
- **Acceptance:**
  - Player can ignore missions and still earn via property/sandbox actions

---

## Combat, wanted, vehicles

### CMB-01 | Reactive combat loop
- **Priority:** P1
- **Depends-on:** VS-07
- **Acceptance:**
  - Player can fight when engaged; no mandatory open-world murder quests

### CMB-02 | Steal / drive cars
- **Priority:** P1
- **Depends-on:** VS-02
- **Acceptance:**
  - Enter vehicle, drive, exit; stealing flagged cars can raise wanted

### CMB-03 | Survive / escape wanted
- **Priority:** P1
- **Depends-on:** VS-07, CMB-02
- **Acceptance:**
  - Wanted can decay after escape/hide; death/arrest has a defined fail state

---

## NPCs & events

### NPC-01 | Unlock Brandon Aragon
- **Priority:** P2
- **Depends-on:** POL-01
- **Acceptance:**
  - Unlocks above Agorist score threshold; grants unique dialogue or mission

### NPC-02 | Unlock Sal Mayweather
- **Priority:** P2
- **Depends-on:** POL-01
- **Acceptance:**
  - Unlocks above Agorist score threshold; grants unique benefit or job

### EVT-01 | Random hurricane
- **Priority:** P2
- **Depends-on:** VS-05
- **Acceptance:**
  - Temporary property income hit or map hazard; recovers after timer

### EVT-02 | Assassination event
- **Priority:** P2
- **Depends-on:** POL-03
- **Acceptance:**
  - Removes or replaces a political actor; shifts policies or influence

### EVT-03 | Economic shift event
- **Priority:** P2
- **Depends-on:** ECO-02, ECO-03
- **Acceptance:**
  - Sudden inflation spike or crypto swing with UI notice

---

## Tech / hygiene

### TECH-01 | Repo README + run instructions
- **Priority:** P0
- **Depends-on:** —
- **Acceptance:**
  - Fresh machine instructions: Python version, deps, launch command

### TECH-02 | Save/load (session minimum)
- **Priority:** P2
- **Depends-on:** VS-05, POL-01
- **Acceptance:**
  - In-session state survives quit/relaunch for alpha playtests

### TECH-03 | Performance budget on 2000×2000 map
- **Priority:** P1
- **Depends-on:** VS-03
- **Acceptance:**
  - Stable playable framerate on target desktop hardware with camera culling or equivalent

---

## Count

39 items. Reorder only when Phase exit criteria change; do not expand past ~40 without retiring completed P2s.
