# Roadmap

Assumptions: **one solo developer, part-time.** Effort in **person-weeks**. No calendar dates. Phases are sequential; later phases depend on earlier exit criteria.

---

## Phase 0 — Foundations

**Goal:** Runnable shell: repo hygiene, game loop, camera, player movement, map load.

**Estimated effort:** 1–2 person-weeks

**Exit criteria:**
- [ ] Repo runs with a documented `README` + one-command (or clear) launch
- [ ] Fixed timestep or stable frame loop; clean quit
- [ ] Player moves with WASD on a ~2000×2000 (or subset) map with camera follow
- [ ] Collision with basic solid tiles/buildings (no soft clipping through walls)
- [ ] Placeholder UI: FPS or debug overlay optional; no crash on empty input

**Depends on:** nothing (start here if code maturity is unknown)

---

## Phase 1 — Vertical slice

**Goal:** One district, one currency path, one property purchase, one mission type, basic wanted + NAP hook.

**Estimated effort:** 3–5 person-weeks

**Exit criteria:**
- [ ] Bounded playable district (or clipped map region) with readable landmarks
- [ ] Player can earn and spend **one** currency; balance numbers are temporary but consistent
- [ ] At least one buyable property; ownership persists in-session; passive income ticks
- [ ] One complete mission type (accept → travel/objective → reward/fail)
- [ ] Wanted level rises on observed crime; police react (chase or attack) at least once
- [ ] NAP: unprovoked aggression lowers reputation; defensive response does not
- [ ] Slice meets `PROJECT.md` success definition (10–15 min fantasy loop)

**Depends on:** Phase 0 exit

---

## Phase 2 — Economy depth

**Goal:** Dual currency, multiple property types, inflation and risk profiles.

**Estimated effort:** 3–5 person-weeks

**Exit criteria:**
- [ ] Fiat and crypto both usable; conversion or parallel pricing exists
- [ ] Fiat inflation (or controlled devaluation) visibly affects prices/savings over time
- [ ] Crypto path has freer use + distinct risks (volatility, crackdown exposure, or theft risk)
- [ ] Property types: legal, underground, mutual aid — each with income + risk/tax profile
- [ ] Tax / evasion pressure interacts with wanted or faction scores at a basic level
- [ ] Economy does not soft-lock the vertical-slice loop (player can still complete a mission)

**Depends on:** Phase 1 exit

---

## Phase 3 — Politics & factions

**Goal:** Elections, policies, influence scores, win condition.

**Estimated effort:** 3–4 person-weeks

**Exit criteria:**
- [ ] Three politicians (or equivalent policy actors) with distinguishable agendas
- [ ] At least one election or appointment cycle that can change active policies
- [ ] Policies include examples covering: drug legalization, UBI, war/conflict, crypto crackdowns
- [ ] State vs Agorist influence scores update from player actions and world events
- [ ] Game end triggers when either side reaches ~90% dominance (win/lose screens)
- [ ] Political shifts meaningfully alter economy or mission availability (at least two hooks)

**Depends on:** Phase 2 exit (economy must exist for policies to bite)

---

## Phase 4 — Content & juice

**Goal:** Mission set, special NPCs, random events, presentation polish.

**Estimated effort:** 4–6 person-weeks

**Exit criteria:**
- [ ] Mission/activity set: smuggling, deliveries, recruitment (multiple instances each)
- [ ] Sandbox freedom preserved (missions optional, not railroad-only)
- [ ] Special NPCs (e.g. Brandon Aragon, Sal Mayweather) unlock as Agorist score grows
- [ ] Random events: hurricane, assassination, economic shift (minimum one of each category)
- [ ] Cars: steal/drive/survive loop feels intentional; combat stays mostly reactive
- [ ] Audio/VFX/UI juice enough that the fantasy is readable without a design doc

**Depends on:** Phase 3 exit (NPCs/events should react to faction state)

---

## Phase 5 — Playable alpha / balance pass

**Goal:** Coherent full-loop alpha; balance and bug pass for external playtests.

**Estimated effort:** 3–4 person-weeks

**Exit criteria:**
- [ ] New player can reach a faction win *or* a clear mid-game failure in one extended session
- [ ] Dual currency + politics do not produce dominant degenerate strategies (spot-check + patch)
- [ ] NAP feel: aggression is costly; defense is viable and fun
- [ ] Crash/blocker list empty for critical path; known issues documented
- [ ] Title decision recorded; build labeled alpha; short playtest brief exists

**Depends on:** Phase 4 exit

---

## Dependency sketch

```
P0 Foundations → P1 Vertical slice → P2 Economy depth
                                      → P3 Politics & factions
                                        → P4 Content & juice
                                          → P5 Playable alpha
```

Do not start P3 politics before dual-currency/property risk exists; policies need something to distort. Do not pad P4 content until win condition and NAP feedback are real.
