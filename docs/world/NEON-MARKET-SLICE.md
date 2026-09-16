# Neon Market — Vertical Slice District (The Grey Arcade)

**Owner:** Atlas (World / Level Design)  
**Status:** Provisional geography for Week 1 / VS-03 — **aligned to Sol** `docs/GREY-MARKET-SLICE.md` §11  
**Freeze:** Reed freezes after merge + smoke; do not invent a second district  
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
| Police spawn / patrol | **Toll Spur gate** | `(640, 2180)` | 200×80 | Wanted chase seed |
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
**Heat:** Police LOS at pickup or mid-alley → wanted +1 (Sol §6.2 / §7). Scripted readable beat: chase from Toll Spur `(640, 2220)`.

**Order flexibility:** Property optional vs smuggle order (Sol acceptance) — both markers always available.

---

## Blockout placement list (copy into code)

```text
# Clip
CLIP_MIN = (0, 2000)
CLIP_MAX = (2000, 4000)

# Hub
SPAWN = (480, 2920)

# Sol §11 markers (centers)
MARKER_TRADER   = (280, 2760)   # street earn
MARKER_CONTACT  = (760, 2840)   # Smuggle-01 accept
MARKER_PROPERTY = (880, 3180)   # underground buy
MARKER_PICKUP   = (960, 3640)   # smuggle pickup
MARKER_DROP     = (1480, 3360)  # smuggle drop

# Wanted / police
ZONE_TOLL_SPUR = (400, 2000, 900, 2360)  # x0,y0,x1,y1
POLICE_SEED    = (640, 2220)

# Soft Agorist core (lower police density)
ZONE_ARCADE_CORE = (200, 2600, 1600, 3800)
```

**Cars (optional):** Fringe Lots ~`(1400, 2300)`; Wharf ~`(1100, 3700)`.

---

## Recommended playtest path (Spike)

Matches Sol beats A–H:

1. Spawn Plaza — HUD crypto 80, wanted 0, NAP 50  
2. Crypto Trader — 2–3 trades toward 150  
3. Buy Underground Stack — OWNED + income tick  
4. Accept Smuggle-01 at contact → Wharf pickup → Neon Alley → Drop  
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

---

## Out of scope

- Map north of y=2000 / east of x=2000  
- Mutual-aid property, dual wallets, elections, legendary NPCs  
- Open water sim south of Wharf (paint only)

---

## Change log

| Date | Change |
|------|--------|
| 2026-09-16 | Initial Grey Arcade draft from D-02 |
| 2026-09-16 | Reconciled to Sol `GREY-MARKET-SLICE.md`: underground property (not mutual-aid); trader + contact + pickup/drop markers; crypto-only; Smuggle-01 route; §11 checklist coverage |

*Ping Reed when this file is on the Week 1 docs PR; Vega can implement clip + markers; Spike can script the path above.*
