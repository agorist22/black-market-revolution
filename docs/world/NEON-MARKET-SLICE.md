# Neon Market — Vertical Slice District (The Grey Arcade)

**Owner:** Atlas (World / Level Design)  
**Status:** **FROZEN** for Week 1 — Sol `docs/GREY-MARKET-SLICE.md` §3 loop + §11 spatial checklist (on `main`)  
**Authority:** Reed freeze 2026-09-16 — no second slice; geography changes only via Reed  
**Audience:** Vega (clip + markers), Spike (playtest path), Ink/Forge (readability), Echo (names/copy)

**Systems contract (Sol — do not contradict):**
- Currency: **crypto only**
- Property: **one underground (illegal)** — mutual-aid **not** in slice
- Mission: **Smuggle-01** (accept → pickup → drop → payout)
- Loop: earn → buy property → smuggle → wanted + NAP · 10–15 min

---

## District pick

| Field | Value |
|-------|--------|
| **Internal id** | `grey_arcade` |
| **Display name** | **The Grey Arcade** |
| **Fantasy** | Covered market + back-alley strip where crypto and underground business run until the State fringe notices |
| **Tone** | Satirical neon grit; readable top-down at GTA2 scale |
| **Playable clip** | SW quadrant of parent 4000×4000 map |
| **Target playtime** | 10–15 minutes (Sol beat chart A–H) |

**Why:** Matches D-02 / Sol district lock. Arcade spine + Toll Spur State edge = readable heat without the rest of Neon Market.

---

## Map clip (VS-03)

Parent map: **4000 × 4000** px. Everything outside the clip is deferred.

| | px |
|--|-----|
| **CLIP_MIN** | `(0, 2000)` |
| **CLIP_MAX** | `(2000, 4000)` |
| **Size** | `2000 × 2000` |
| **Player spawn (hub)** | `(480, 2920)` — west of Ledger Plaza, facing east |

**Clamp:** Player/cars cannot leave clip. Prefer visible barrier props on north/east edges.  
**Camera:** Existing follow; out-of-clip culling optional for Week 1.

```
Full 4000×4000
┌────────────────────────────┐
│          (deferred)        │
│  (0,2000) ┌──────────────┐ │
│           │ GREY ARCADE  │ │
│           │   2000×2000  │ │
│           └──────────────┘ │(2000,4000)
└────────────────────────────┘
```

---

## Sol §11 → landmark map

| Sol prop | Landmark | Center (x,y) | Footprint | Notes |
|----------|----------|--------------|-----------|-------|
| Hub / spawn | **Ledger Plaza** + spawn | Plaza `(720, 2880)` · spawn `(480, 2920)` | Plaza 220×180 open | Beat A |
| Trader spot | **Crypto Trader** | `(280, 2760)` | 96×64 booth | Street earn (+25–40, 30s CD) |
| Contact NPC | **Smuggle contact** | `(760, 2840)` | 48×32 board/NPC | Accept Smuggle-01 |
| Pickup point | **Wharf Pickup** | `(960, 3640)` | 140×100 bay | Enter radius + interact |
| Drop point | **Alley Drop** | `(1480, 3360)` | 80×80 | End of Neon Alley |
| Buyable building | **Underground Stack** | `(880, 3180)` | 160×200 | Price 150; OWNED state |
| Police spawn / patrol | **Toll Spur gate** | gate `(640, 2180)` · **POLICE_SEED** `(640, 2220)` | 200×80 | Eng uses POLICE_SEED for chase |
| Soft bounds | Clip AABB | — | — | See above |

**Collision:** Landmark footprints solid except Plaza floor and Neon Alley roadbed. Keep **≥64 px** driveable lane E–W through Plaza and through Alley.

### Zone sketch

```
y↑ toward rest of city / State
│  N1 TOLL SPUR (State)     N2 FRINGE LOTS
│  ┌──────────┬─────────────────────────┐
│  │ W1       │ C1 LEDGER PLAZA (hub)   │ E1 SERVICE ALLEY
│  │ CRYPTO   │    + smuggle contact    │
│  │ TRADER   ├─────────┬───────────────┤
│  │          │ C2      │ E2 NEON ALLEY │
│  │          │ UNDER-  │   (carry)     │
│  │          │ GROUND  │ E2b DROP      │
│  │          │ STACK   │               │
│  ├──────────┤ C3      │               │
│  │ W2 QUIET │ WHARF   │               │
│  │ COURTS   │ PICKUP  │               │
│  └──────────┴─────────┴───────────────┘
└──────────────→ x
```

| Zone | Lean | Week 1 use |
|------|------|------------|
| `C1` Ledger Plaza | Neutral→Agorist | Hub, contact, orient |
| `W1` Crypto Trader | Agorist | Earn |
| `C2` Underground Stack | Agorist | Buy + income + raid risk (Sol numbers) |
| `E2` Neon Alley | Contested | Carry corridor |
| `C3` Wharf Pickup | Agorist | Smuggle pickup |
| `N1` Toll Spur | **State** | Police seed / heat readability |
| `W2` Quiet Courts | Neutral | Escape / wanted decay later |
| `N2` Fringe Lots | Mixed | 2–3 parked cars optional |

**Faction volume shift:** deferred past Week 1 (static volumes only).

---

## Mission geography — Smuggle-01

| Step | Marker id | Position | Action |
|------|-----------|----------|--------|
| Accept | `MARKER_CONTACT` | `(760, 2840)` | Start; show pickup |
| Pickup | `MARKER_PICKUP` | `(960, 3640)` | Interact; cargo flag on |
| Transit | Neon Alley spine | `(1180, 3000)` → `(1500, 3520)` | Primary route (~80 px wide) |
| Drop | `MARKER_DROP` | `(1480, 3360)` | Interact; +120 crypto |
| Fail | — | — | Timer 240s / wanted 3 while carrying / arrest-death while carrying — Sol §6 |

**Alt path:** Service Alley `E1` — keep clear but secondary so the Alley stays obvious.  
**Heat (Week 1 — option A):** Scripted chase from Toll Spur `(640, 2220)` when **cargo flag is on** (wanted +1, one engage). Natural pickup-LOS per Sol §6.2 is **nice if a patrol reaches the bay** — not required for smoke; Toll Spur is too far for reliable natural LOS. Do not add a special Wharf officer for Week 1 unless smoke fails without it (option B deferred).

**Order flexibility:** Property optional vs smuggle order (Sol acceptance) — both markers always available.

---


---

## Spawn points (Week 1)

All positions are world px on the parent 4000×4000 map, **inside** the Grey Arcade clip only.

| Id | Role | Position | Facing | Notes |
|----|------|----------|--------|-------|
| `SPAWN_PLAYER` | Player / arrest return | `(480, 2920)` | East (+x) | West of Ledger Plaza |
| `SPAWN_TRADER` | Street earn NPC | `(280, 2760)` | South | Interact radius **48** |
| `SPAWN_CONTACT` | Smuggle-01 accept | `(760, 2840)` | West | On Plaza; Echo owns lines |
| `SPAWN_CIV_A` | Neutral civilian (NAP test) | `(640, 2900)` | Random | Optional; Plaza only |
| `SPAWN_CIV_B` | Neutral civilian | `(1000, 3120)` | Random | Near Stack approach |
| `SPAWN_CAR_FRINGE` | Optional parked car | `(1400, 2300)` | West | Fringe Lots |
| `SPAWN_CAR_WHARF` | Optional parked car | `(1100, 3700)` | North | Near pickup |

**Arrest / fail return:** always `SPAWN_PLAYER` (Sol soft-fail → hub).

---

## Property location (Underground Stack)

| Field | Value |
|-------|------:|
| Id | `PROP_UNDERGROUND_01` |
| Landmark | Underground Stack |
| Interact / buy marker | `(880, 3180)` |
| Building AABB (solid) | `(800, 3080)` → `(960, 3280)` |
| Loading door (OWNED flash / drop adjacency OK) | `(920, 3280)` |
| Buy price / income | Sol / `BALANCE-GREY-ARCADE` — **150** crypto · **+8 / 15s** |
| Raid risk volume | Same as building AABB (Sol 12% / 90s) |

Only **one** buyable footprint in the slice. No mutual-aid building.

---

## Smuggle-01 route waypoints

Primary path is a polyline for nav hints / Spike timing. Corridor width **≥80** along Neon Alley.

| # | Waypoint id | Position | Purpose |
|---|-------------|----------|---------|
| 0 | `WP_ACCEPT` | `(760, 2840)` | Mission start (contact) |
| 1 | `WP_PLAZA_EXIT` | `(920, 2960)` | Leave Plaza toward Stack/Wharf |
| 2 | `WP_STACK_CORNER` | `(1000, 3200)` | Pass Underground Stack |
| 3 | `WP_WHARF_APPROACH` | `(1000, 3480)` | Approach bay |
| 4 | `WP_PICKUP` | `(960, 3640)` | Pickup interact (cargo on) |
| 5 | `WP_ALLEY_ENTER` | `(1180, 3000)` | Enter Neon Alley north mouth |
| 6 | `WP_ALLEY_MID` | `(1340, 3180)` | Mid corridor |
| 7 | `WP_ALLEY_BEND` | `(1500, 3320)` | Bend toward drop |
| 8 | `WP_DROP` | `(1480, 3360)` | Drop interact (payout) |

**Suggested order after accept:** 0 → 1 → 2 → 3 → 4 (pickup) → 5 → 6 → 7 → 8 (drop).  
Players may free-roam; waypoints are for markers, breadcrumb optional, and smoke timing — not a railroad.

**Secondary (Service Alley):** `(1100, 2720)` → `(1500, 2800)` → join at `WP_ALLEY_ENTER`. Keep clear; do not mark as primary.

**Interact radii (defaults):** contact / trader / buy / pickup / drop = **48** px unless OpenGta2 collide needs larger.

---

## Police spawn notes (Toll Spur)

| Field | Value |
|-------|-------|
| Heat model | **Option A** — scripted chase when **cargo flag on** |
| `POLICE_SEED` | `(640, 2220)` — **source of truth** for chase spawn |
| Gate visual center | `(640, 2180)` — prop only; do not spawn chase here if it differs from seed |
| `ZONE_TOLL_SPUR` | AABB `(400, 2000)`–`(900, 2360)` — State fringe / heat volume |
| Week 1 count | **1** officer on cargo-on wanted rise; no Wharf-dedicated officer |
| Patrol (optional idle) | Seed → south to `(640, 2500)` and back; must **not** be required for smoke |
| Chase target | Player; engage once for VS-07 |
| Natural Wharf LOS | Nice-to-have only if patrol reaches bay — **not** acceptance |

@Ink: Toll Spur reads **sodium / cold State**; Arcade south of ~y=2600 reads **neon** — border should flip without a second tile language (your bible).


## Blockout placement list (copy into code)

```text
# Clip — one district only
CLIP_MIN = (0, 2000)
CLIP_MAX = (2000, 4000)

# Spawns
SPAWN_PLAYER     = (480, 2920)
SPAWN_TRADER     = (280, 2760)
SPAWN_CONTACT    = (760, 2840)
SPAWN_CIV_A      = (640, 2900)   # optional NAP target
SPAWN_CIV_B      = (1000, 3120)
SPAWN_CAR_FRINGE = (1400, 2300)
SPAWN_CAR_WHARF  = (1100, 3700)

# Property
MARKER_PROPERTY      = (880, 3180)
PROP_AABB_MIN        = (800, 3080)
PROP_AABB_MAX        = (960, 3280)
PROP_LOADING_DOOR    = (920, 3280)

# Smuggle markers + waypoints
MARKER_TRADER = SPAWN_TRADER
MARKER_CONTACT = SPAWN_CONTACT
MARKER_PICKUP = (960, 3640)
MARKER_DROP   = (1480, 3360)
WP = [
  (760, 2840),   # 0 accept
  (920, 2960),   # 1 plaza exit
  (1000, 3200),  # 2 stack corner
  (1000, 3480),  # 3 wharf approach
  (960, 3640),   # 4 pickup
  (1180, 3000),  # 5 alley enter
  (1340, 3180),  # 6 alley mid
  (1500, 3320),  # 7 alley bend
  (1480, 3360),  # 8 drop
]

# Police
ZONE_TOLL_SPUR   = (400, 2000, 900, 2360)
POLICE_SEED      = (640, 2220)  # chase spawn SOI
GATE_VISUAL      = (640, 2180)
ZONE_ARCADE_CORE = (200, 2600, 1600, 3800)
```

---

## Recommended playtest path (Spike)

Matches Sol beats A–H:

1. Spawn Plaza — HUD crypto 80, wanted 0, NAP 50  
2. Crypto Trader — 2–3 trades toward 150 (~90s with 30s CD; fine for systems)  
   - **Alternate order (allowed):** Smuggle-01 first (+120) → then buy property — use if trader wait pads the smoke  
3. Buy Underground Stack — OWNED + income tick  
4. Accept Smuggle-01 at contact → Wharf pickup → Neon Alley → Drop (skip if already done in alt order)  
5. Trigger wanted ≥1 (LOS or Toll Spur) — one chase  
6. Optional NAP: do **not** shoot civilians; if testing cue, one unprovoked hit → toast  
7. Slice-complete when: owned + ≥1 smuggle success + wanted ever ≥1 + NAP feedback shown (Sol §12)

Walk budget without car ~6–8 min for mission geography; full fantasy ≤15.

---

## Art notes (Ink / Forge)

- Silhouette > detail. Plaza canopy + Neon Alley lights unique from screen edge.  
- State = cold blue/white at Toll Spur; Arcade = magenta/amber.  
- Underground Stack needs clear **for-sale → OWNED** read.  
- Do not block Plaza or Alley drive lanes.
- District border (~y=2600 / Toll Spur): **sodium State** north vs **neon Arcade** south — one tile language, palette flip only (@Ink bible).

---

## Out of scope

- Map north of y=2000 / east of x=2000  
- Mutual-aid property, dual wallets, elections, legendary NPCs  
- Open water sim south of Wharf (paint only)

---


---

## OWNER-VISION alignment (Week 1)

Read `docs/OWNER-VISION.md`. Pitch > old Grok code. Geography still **one clipped Grey Arcade** (Reed freeze).

| Owner ask | Week 1 Grey Arcade | Conflict / defer |
|-----------|--------------------|------------------|
| GTA2 top-down | Yes — clip + markers for OpenGta2 shell | None |
| Statists vs Agorists over the market | Toll Spur (State) vs Arcade core (Agorist) volumes | Full city / multi-district later |
| Dual fiat/crypto | Spatial props assume **crypto** earn/buy (Sol lock) | Fiat buildings / ATMs deferred; greyed HUD only |
| NAP (defense OK) | Quiet Courts escape; civ spawns for unprovoked test | Assassin / bodyguard geography later |
| Three politicians / two-option votes | **No** vote venues in clip | Out of Week 1 (ROADMAP P3) |
| No instant faction win | No win-threshold geometry | Matches Sol telemetry-only scores |
| HUD market bar | No world prop required | Eng/Frame after smoke |
| SEK3 / Konkin quote | Place names / district flavor only | Menu chrome = SEK3 only; **no** “Free Roger Ver”; Roger **not** an NPC / no ped |
| Brandon Aragon + Sal Mayweather | **No** spawn points in Week 1 dump | Phase 4+ unlock geography |
| Stability > fullscreen | One 2000×2000 clip | No map expansion for juice |

**Do not reopen:** second district, mutual-aid footprint, Deliver-01 hard police route (soft secondary; Smuggle-01 = smoke).

## Change log

| Date | Change |
|------|--------|
| 2026-09-16 | Initial Grey Arcade draft from D-02 |
| 2026-09-16 | Reconciled to Sol `GREY-MARKET-SLICE.md`: underground property (not mutual-aid); trader + contact + pickup/drop markers; crypto-only; Smuggle-01 route; §11 checklist coverage |
| 2026-09-16 | **Frozen** on Reed order after Sol systems doc landed on `main` (PR #3). Hand-off to Vega: clip + markers only. |
| 2026-09-16 | Sol reconcile: no hard conflicts. Soft patches — heat option A (scripted Toll Spur on cargo); Spike smuggle-first alt order. |
| 2026-09-16 | Expand: spawn table, property AABB, Smuggle-01 waypoints 0–8, police seed notes (still one clip). |
| 2026-09-16 | OWNER-VISION § stamp — Week 1 deferrals vs full pitch (no second district). |
| 2026-09-16 | Owner update: drop Free Roger Ver chrome; SEK3 stays; no Roger ped. |

*Frozen district. Placement tables above are the eng dump for Vega; Spike uses waypoints + playtest path. Geography edits need Reed.*
