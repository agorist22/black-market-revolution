# Grey-Market Vertical Slice — Systems Design

**Owner:** Sol (Systems Design)  
**Status:** Draft for Week 1 implement / Atlas district blockout  
**Aligns:** `PROJECT.md` success definition · `DECISIONS.md` D-02 · backlog VS-04…VS-08  
**Out of scope:** elections, dual-currency depth, legal property tier, full mission catalog, win condition polish

---

## 1. Fantasy in one sentence

In **one Agorist district**, earn **crypto**, buy **one underground property** that pays you, run **one smuggle**, and feel **wanted heat** plus a clear **NAP** cue — all in 10–15 minutes.

---

## 2. Slice locks (do not reopen until Phase 1 exit)

| Lock | Choice | Why |
|------|--------|-----|
| District | One Agorist / grey-market block (Atlas owns layout) | Matches D-02 |
| Currency | **Crypto only** (provisional; fiat HUD slot reserved but unused) | Teaches counter-economy fantasy; Phase 2 adds fiat |
| Property | **One underground (illegal) property** | Highest wanted/raid readability; mutual-aid deferred to Phase 2 |
| Mission | **One smuggle run** (pickup → drop → payout) | Clear travel loop; delivery is the same template with softer wanted |
| Combat | Reactive / light | Defense vs police OK; unprovoked aggression costs NAP |
| Foundation | OpenGta2 shell (C# / MonoGame) | `FOUNDATIONS.md` / D-06 |

**Provisional note:** Crypto is Sol’s recommendation pending designer lock on the currency widget. If dirty-fiat wins, swap wallet labels and keep the same numbers as “dirty cash units.”

---

## 3. Player loop (beat chart)

Target session: **10–15 minutes**. Times are soft targets for design/playtest, not hard timers.

| # | Beat | Player action | System response | Target time |
|---|------|---------------|-----------------|-------------|
| A | Arrive | Spawn in district hub | HUD: crypto, health, wanted=0, NAP Rep | 0:00 |
| B | Earn | Complete **street trade** OR first **courier stub** payout | +crypto; optional Agorist nudge | 0–3 min |
| C | Buy | Purchase marked underground property if funds ≥ price | Ownership flag; income tick starts | 3–6 min |
| D | Passive | Wait / walk while income ticks | Crypto rises on timer; optional raid risk roll | ongoing |
| E | Mission | Accept **Smuggle-01** at contact | Markers: pickup, drop; fail states defined | 6–12 min |
| F | Heat | Get spotted mid-smuggle or after property raid flag | Wanted ≥1; police chase once | during E |
| G | NAP | Optional: attack a civilian vs fight police | Rep down only on unprovoked; defense clean | anytime |
| H | Close | Own property + completed smuggle once + saw wanted/NAP cue | Slice success (debug banner OK) | ≤15 min |

**Soft-fail allowed:** death/arrest resets to hub with crypto halved (floor 0); property ownership **persists in-session**.

---

## 4. Currency (VS-04)

### 4.1 Rules

- Single wallet: `CryptoBalance` (integer display; internal can be float, round for HUD).
- Fiat exists only as a **disabled / greyed** HUD slot or is omitted entirely until Phase 2 — do not implement conversion.
- No inflation, mining, or exchange in the slice.

### 4.2 Starting & sinks

| Item | Value | Notes |
|------|------:|-------|
| Starting crypto | **80** | Enough for ~1 trade cycle before property; not enough to buy property immediately |
| Street trade earn | **+25** to **+40** | One interactable trader; cooldown 30s real-time |
| Smuggle success | **+120** | Primary earn spike |
| Smuggle fail (bust) | **−40** or forfeit cargo (no payout) | Prefer forfeit cargo; keep −40 only if cargo already paid upfront |
| Property buy price | **150** | Requires ~2–3 earns or 1 smuggle-first path |
| Property income | **+8 crypto / 15s** | ~32/min; feels alive without dominating |
| Optional: bail / bribe | **50** | Clears wanted 1–2 if implemented; else skip |

### 4.3 Acceptance (VS-04)

- [ ] HUD shows crypto balance at all times during play
- [ ] At least one earn source and one spend sink work in-session
- [ ] Balance never goes negative (clamp spend)
- [ ] No fiat earn/spend path in slice build

---

## 5. Property (VS-05)

### 5.1 Underground property — single instance

| Field | Value |
|-------|-------|
| Type | `UndergroundBusiness` |
| Buy price | 150 crypto |
| Income | +8 / 15s while owned |
| Raid risk | Every 90s while owned: **12%** chance to raise wanted by +1 (max 3 for slice) |
| Visual | Distinct marker + “OWNED” state (Frame/Ink) |
| Mutual-aid | **Not in slice** |

### 5.2 Acceptance (VS-05)

- [ ] One purchasable property in district; refuse buy if `Crypto < 150`
- [ ] Ownership persists for the session (survive death/arrest)
- [ ] Income accrues on a visible tick (debug log or HUD flash OK)
- [ ] At least one raid-risk event can fire (or forceable via debug)

---

## 6. Mission: Smuggle-01 (VS-06)

### 6.1 Flow

1. **Accept** at Agorist contact (hub).
2. **Pickup** crate at marked point (enter radius + interact).
3. **Carry** flag on player (slow optional; not required for slice).
4. **Drop** at marked drop point.
5. **Payout** +120 crypto; clear mission state.
6. **Fail:** abandon (timer 4 min) OR wanted reaches 3 while carrying OR player arrested/dead while carrying → no payout; mission returns to available after 60s.

### 6.2 Heat hook

- Entering pickup with a police LOS (simple radius check) → wanted +1.
- Completing drop while wanted ≥1 still pays (crime already priced via heat).

### 6.3 Acceptance (VS-06)

- [ ] Accept → pickup → drop → success reward path works end-to-end
- [ ] Fail path does not soft-lock map or wallet
- [ ] Mission completable without property ownership (property optional order)
- [ ] One instance only for Week 1 (no mission catalog)

---

## 7. Wanted / police (VS-07)

| Wanted | Trigger examples | Police behavior |
|-------:|------------------|-----------------|
| 0 | Default | Ignore |
| 1 | Smuggle LOS; property raid proc | Nearest officer moves to player; chase |
| 2 | Assault police; second raid | Chase + attack |
| 3 (cap) | Stacked crimes | Persistent chase; arrest on contact if health low or timer |

**Decay:** −1 star after **45s** out of combat / LOS (simple).  
**Arrest:** fade to hub; crypto ×0.5; wanted → 0; property kept.

### Acceptance (VS-07)

- [ ] At least one path raises wanted from a flagged crime
- [ ] At least one chase or attack engage occurs in a smoke test
- [ ] Wanted displayed on HUD (stars or heat pips)

---

## 8. NAP reputation (VS-08)

| Action | NAP Rep delta | Notes |
|--------|--------------:|-------|
| Unprovoked attack on civilian / neutral trader | **−15** | “You broke the NAP” toast |
| Kill civilian | **−25** | Stronger cue |
| Damage / fight police while wanted | **0** | Defense / consequence of heat — not NAP break |
| Complete smuggle without civilian harm | **+5** | Optional small reward |
| Buy underground property | **0** | Crime against State ≠ NAP break |

**Starting NAP Rep:** 50 (0–100 clamp).  
**UI:** numeric or short bar; Frame owns layout. Threshold cosmetics only in slice (no unlock gates yet).

### Aggression tag (engineering)

Every damage event carries `AggressionContext`: `Unprovoked` | `Defense` | `StateConflict`.  
NAP penalty applies **only** to `Unprovoked`.

### Acceptance (VS-08)

- [ ] Unprovoked civilian hit lowers NAP Rep and shows feedback
- [ ] Fighting police does **not** apply the same penalty
- [ ] Rep visible on HUD or pause panel

---

## 9. Faction scores (slice minimum)

| Score | Slice behavior |
|-------|----------------|
| Agorist | +2 on smuggle success; +1 on underground property buy |
| State | +2 when wanted hits 2+; +1 on property raid proc |

No election, no 90% win. Scores are **telemetry + HUD bar fodder** so Frame can place the widget and Phase 3 has a hook.

### Acceptance

- [ ] Both scores exist and update from at least one player action each
- [ ] Displayed or inspectable in debug if HUD not ready

---

## 10. HUD — must be visible at a glance (Frame)

**Always on (combat/exploration):**

1. **Health** (or lives)
2. **Crypto balance** (primary currency)
3. **Wanted / heat** (0–3)
4. **NAP Rep** (compact)
5. **Mission objective line** when Smuggle-01 active (pickup / drop / time)

**Secondary (glance or corner cluster):**

6. **Agorist vs State** influence (tiny dual bar or two numbers)
7. **Property owned** indicator (icon / “OWNED”)
8. **Fiat** — omit or grey “—” until Phase 2

**Not required in slice HUD:** inventory grid, legislation, election timer, full map legend.

---

## 11. District needs (Atlas)

Minimum spatial props for the loop:

| Prop | Purpose |
|------|---------|
| Hub / spawn | Beat A |
| Trader spot | Street earn |
| Contact NPC | Mission accept |
| Pickup point | Smuggle |
| Drop point | Smuggle |
| One buyable building footprint | Underground property |
| Police spawn / patrol path | Wanted chase readable |
| Soft district bounds | Clip playable area |

Landmarks must read at GTA2 top-down scale; names/copy → Echo.

---

## 12. Debug / reset (VS-09)

- Hotkey or console: reset wanted, NAP, crypto to start, clear mission, keep or clear property (two commands).
- “Slice complete” flag when: owned property AND ≥1 smuggle success AND wanted ever ≥1 AND NAP feedback ever shown.

---

## 13. Balance summary (implementable table)

**Full tables (authoritative numbers):** [`BALANCE-GREY-ARCADE.md`](BALANCE-GREY-ARCADE.md).


| Constant | Value |
|----------|------:|
| `START_CRYPTO` | 80 |
| `TRADE_REWARD_MIN` / `MAX` | 25 / 40 |
| `TRADE_COOLDOWN_S` | 30 |
| `PROPERTY_PRICE` | 150 |
| `PROPERTY_INCOME` | 8 |
| `PROPERTY_INCOME_PERIOD_S` | 15 |
| `PROPERTY_RAID_PERIOD_S` | 90 |
| `PROPERTY_RAID_CHANCE` | 0.12 |
| `SMUGGLE_REWARD` | 120 |
| `SMUGGLE_TIME_LIMIT_S` | 240 |
| `WANTED_MAX` | 3 |
| `WANTED_DECAY_S` | 45 |
| `NAP_START` | 50 |
| `NAP_HIT_CIV` | −15 |
| `NAP_KILL_CIV` | −25 |
| `ARREST_CRYPTO_FACTOR` | 0.5 |

Tune only after one full smoke of beats A–H; do not add systems to fix feel.

---

## 14. Week 1 engineering order (suggestion for Vega / Reed)

1. Wallet + HUD crypto (VS-04)  
2. Property buy + income tick (VS-05)  
3. Smuggle-01 state machine (VS-06)  
4. Wanted + one chase (VS-07)  
5. NAP tags + toast (VS-08)  
6. District clip + markers (with Atlas)  
7. Debug reset + slice-complete check (VS-09)

Freeze: elections, dual wallets, mutual-aid buildings, assassins, inflation, special NPC unlocks.

---

## 15. Open decisions (non-blocking for blockout)

| ID | Item | Default if silent |
|----|------|-------------------|
| S-01 | Crypto vs dirty fiat labels | **Crypto** |
| S-02 | Carry slow while smuggling | Off (simpler) |
| S-03 | Bribe to clear heat | Off until chase feels good |

---

*End of draft. Ping Reed when merged to `docs/`; Frame can take §10 as HUD contract; Atlas can take §11 as blockout checklist.*
