# Balance tables — Grey Arcade vertical slice

**Owner:** Sol (Systems)  
**Status:** Week 1 authoritative numbers  
**Aligns:** [`GREY-MARKET-SLICE.md`](GREY-MARKET-SLICE.md) §4–§8, §13  
**Audience:** Vega (constants), Spike (smoke expectations), Frame (HUD placeholders)

Scope freeze: **crypto-only**, one underground property, Smuggle-01. No fiat, no mutual-aid property, no elections. Tune only after one full A–H smoke — do not add systems to fix feel.

---

## 1. Starting balances

| ID | Constant | Value | Notes |
|----|----------|------:|-------|
| B-01 | `START_CRYPTO` | **80** | Not enough to buy property immediately |
| B-02 | `START_FIAT` | **0 / unused** | Fiat wallet omitted or grey “—” until Phase 2 |
| B-03 | `START_HEALTH` | **100** | If health meter exists |
| B-04 | `START_WANTED` | **0** | |
| B-05 | `START_NAP_REP` | **50** | Clamp 0–100 |
| B-06 | `START_AGORIST_SCORE` | **0** | Telemetry / HUD only in slice |
| B-07 | `START_STATE_SCORE` | **0** | Telemetry / HUD only in slice |

---

## 2. Street earn (trader)

| ID | Constant | Value | Notes |
|----|----------|------:|-------|
| E-01 | `TRADE_REWARD_MIN` | **25** | Crypto |
| E-02 | `TRADE_REWARD_MAX` | **40** | Inclusive roll or fixed mid if simpler |
| E-03 | `TRADE_COOLDOWN_S` | **30** | Real-time seconds |

---

## 3. Property — underground only

| ID | Constant | Value | Notes |
|----|----------|------:|-------|
| P-01 | `PROPERTY_PRICE` | **150** | Crypto; refuse buy if balance &lt; price |
| P-02 | `PROPERTY_INCOME` | **8** | Crypto per tick |
| P-03 | `PROPERTY_INCOME_PERIOD_S` | **15** | ≈32 crypto/min while owned |
| P-04 | `PROPERTY_RAID_PERIOD_S` | **90** | While owned |
| P-05 | `PROPERTY_RAID_CHANCE` | **0.12** | On raid tick: wanted +1 (cap `WANTED_MAX`) |
| P-06 | Mutual-aid property | **out of slice** | Do not ship cost/income |

Ownership persists through death/arrest in-session.

---

## 4. Smuggle-01 rewards / fail

| ID | Constant | Value | Notes |
|----|----------|------:|-------|
| M-01 | `SMUGGLE_REWARD` | **120** | On successful drop |
| M-02 | `SMUGGLE_FAIL_PENALTY` | **0** (forfeit cargo) | Preferred: no payout, no wallet debit |
| M-03 | `SMUGGLE_FAIL_PENALTY_ALT` | **−40** | Only if cargo was prepaid; default off |
| M-04 | `SMUGGLE_TIME_LIMIT_S` | **240** | Abandon → fail |
| M-05 | Fail on wanted | **wanted == 3 while carrying** | Fail, no payout |
| M-06 | Fail on arrest/death | **while carrying** | Fail, no payout; then arrest rules |
| M-07 | Retry delay after fail | **60** s | Mission available again |

Success while wanted ≥ 1 still pays (heat already priced).

---


---

## 4b. Deliver-01 rewards / fail (soft alternate)

Same accept → pickup → drop template as Smuggle-01; **lower payout, softer heat** (no scripted Toll Spur cargo chase by default).

| ID | Constant | Value | Notes |
|----|----------|------:|-------|
| D-01 | `DELIVER_REWARD` | **60** | Crypto on successful drop (half of smuggle) |
| D-02 | `DELIVER_FAIL_PENALTY` | **0** (forfeit package) | Same as smuggle default |
| D-03 | `DELIVER_TIME_LIMIT_S` | **240** | Match smuggle |
| D-04 | Wanted on LOS | **optional +1** | No scripted Toll Spur chase; natural LOS only |

Week 1 smoke priority remains **Smuggle-01**. Deliver-01 is string/content-ready; implement after smuggle state machine works.

## 5. Wanted thresholds

| Stars | Constant / rule | Effect |
|------:|-----------------|--------|
| 0 | default | Police ignore |
| 1 | smuggle LOS **or** property raid proc **or** scripted cargo chase (Atlas: Toll Spur) | Nearest officer chase |
| 2 | assault police; second raid | Chase + attack |
| 3 | `WANTED_MAX` = **3** | Persistent chase; arrest on contact if health low / timer |

| ID | Constant | Value | Notes |
|----|----------|------:|-------|
| W-01 | `WANTED_MAX` | **3** | Hard cap for slice |
| W-02 | `WANTED_DECAY_S` | **45** | −1 star after this many seconds out of combat/LOS |
| W-03 | `ARREST_CRYPTO_FACTOR` | **0.5** | Crypto × factor on arrest (floor 0); property kept; wanted → 0 |

---

## 6. NAP reputation deltas

| ID | Constant | Delta | When |
|----|----------|------:|------|
| N-01 | `NAP_HIT_CIV` | **−15** | Unprovoked attack on civilian / neutral trader |
| N-02 | `NAP_KILL_CIV` | **−25** | Kill civilian |
| N-03 | Defense vs police / StateConflict | **0** | Fighting police while wanted does **not** NAP-penalize |
| N-04 | `NAP_SMUGGLE_CLEAN` | **+5** | Optional: complete smuggle without civilian harm |
| N-05 | Buy underground property | **0** | Crime vs State ≠ NAP break |

Every damage event needs `AggressionContext`: `Unprovoked` | `Defense` | `StateConflict`. NAP applies only to `Unprovoked`.

---

## 7. Faction score nudges (slice minimum)

| Action | Agorist | State |
|--------|--------:|------:|
| Smuggle success | **+2** | 0 |
| Buy underground property | **+1** | 0 |
| Wanted reaches 2+ | 0 | **+2** |
| Property raid proc | 0 | **+1** |

No win threshold in Week 1.

---

## 8. Implementable constant dump (copy for code)

```text
START_CRYPTO = 80
START_NAP_REP = 50
START_WANTED = 0
TRADE_REWARD_MIN = 25
TRADE_REWARD_MAX = 40
TRADE_COOLDOWN_S = 30
PROPERTY_PRICE = 150
PROPERTY_INCOME = 8
PROPERTY_INCOME_PERIOD_S = 15
PROPERTY_RAID_PERIOD_S = 90
PROPERTY_RAID_CHANCE = 0.12
SMUGGLE_REWARD = 120
SMUGGLE_FAIL_PENALTY = 0          # forfeit cargo; alt -40 if prepaid
SMUGGLE_TIME_LIMIT_S = 240
SMUGGLE_RETRY_DELAY_S = 60
WANTED_MAX = 3
WANTED_DECAY_S = 45
ARREST_CRYPTO_FACTOR = 0.5
NAP_HIT_CIV = -15
NAP_KILL_CIV = -25
NAP_SMUGGLE_CLEAN = 5             # optional
FACTION_AGORIST_SMUGGLE = 2
FACTION_AGORIST_PROPERTY_BUY = 1
FACTION_STATE_WANTED_2 = 2
FACTION_STATE_RAID = 1
```

---

## 9. Change control

| Rule | |
|------|--|
| Source of truth | This file for **numbers**; loop/rules stay in `GREY-MARKET-SLICE.md` |
| Who may change | Sol (or designer override); note date in git |
| Vega | Bind constants 1:1; no silent retunes mid-Week-1 |
| After smoke | One balance pass max before Phase 2 dual-currency |

---

*End. @Vega can paste §8; @Spike expect start crypto 80 / property 150 / smuggle +120 / wanted max 3 / NAP start 50.*
