# HUD Wireframe — Grey-Market Slice @ 1280×720

**Owner:** Frame (UI/UX)  
**Status:** Offline prep — layout + glance contract only (Reed HOLD on craft / Vega tickets until smoke lands)  
**Aligns:** `GREY-MARKET-SLICE.md` §10–§13 · Grey Arcade smoke path · Sol glance contract  
**Out of scope:** Inventory grid, legislation, election timer, full map legend, dual-currency conversion UI, pause menu redesign, engine PRs

**Hard rule:** Informative without burying the open world. Center playfield stays clear.

---

## 1. Purpose

Lock a 1280×720 HUD layout so Vega can implement widgets and Ink can skin chrome without inventing positions mid-slice. This doc is **not** an implementation ticket.

When Reed unfreezes UI craft, implementation starts from these regions and IDs — do not invent parallel layouts in code.

---

## 2. Resolution & safe margins

| Spec | Value |
|------|-------|
| Target | **1280 × 720** (logical pixels) |
| Safe edge inset | **12 px** from screen edge for all primary chrome |
| Center clear zone | Keep roughly **320,120 → 960,560** free of persistent HUD (action / vehicle / combat readable) |
| Font floor | Body / values ≥ **14 px**; mission line ≥ **16 px**; labels may be 12 px |
| Opacity | Panels ≤ **70%** fill so map tiles remain readable behind chrome |

Top-down GTA2-like readability beats neon density. Prefer flat high-contrast pips over ornate frames until Ink locks style.

---

## 3. Wireframe (regions)

```
┌──────────────────────────────── 1280 ────────────────────────────────┐
│ 12px inset                                                           │
│ ┌─ TL PRIMARY ──────────────┐              ┌─ TR STATUS ───────────┐ │
│ │ [♥ Health ####/####]      │              │ Wanted ●●●○  (0–3)    │ │
│ │ [◈ Crypto  80     ]       │              │ NAP Rep  50           │ │
│ │ [Fiat      —      ] grey  │              └───────────────────────┘ │
│ └───────────────────────────┘                                        │
│                                                                      │
│                                                                      │
│                     CENTER PLAYFIELD (clear)                         │
│                                                                      │
│                                                                      │
│ ┌─ BL SECONDARY ────────────┐              ┌─ BR NAV ──────────────┐ │
│ │ Agorist |||||···· State   │              │ ┌─ minimap 96×96 ───┐ │ │
│ │ Property: OWNED / —       │              │ │ (no full legend)  │ │ │
│ └───────────────────────────┘              │ └───────────────────┘ │ │
│                                            │ N / district stub     │ │
│ ┌─ BC MISSION (when active) ─────────────────────────────────────┐ │ │
│ │ Smuggle-01: Pickup → Drop · 3:42                               │ │ │
│ └────────────────────────────────────────────────────────────────┘ │ │
│ ┌─ CONTROL PROMPTS (thin) ───────────────────────────────────────┐ │ │
│ │ [E] Interact   [F] Enter/Exit   [Tab] Pause                    │ │ │
│ └────────────────────────────────────────────────────────────────┘ │ │
└──────────────────────────────────────────────────────────────────────┘
```

**Corner assignment**

| Corner | Cluster | Why |
|--------|---------|-----|
| **Top-left** | Health + wallet (crypto primary, fiat grey) | Combat + economy always in the same glance |
| **Top-right** | Wanted + NAP Rep | Heat / reputation away from wallet so numbers don’t collide |
| **Bottom-left** | Faction dual bar + property owned | Secondary; readable without covering chase |
| **Bottom-right** | Minimap stub (no legend) | Classic GTA2 habit; small |
| **Bottom-center** | Mission objective line + control prompts | Only when Smuggle-01 active; prompts stay thin |

---

## 4. Element specs

IDs are stable strings for later UI code. Positions are **from screen edges** at 1280×720.

| ID | Content | Position | Size budget | Visibility | Notes |
|----|---------|----------|-------------|------------|-------|
| `hud_health` | Health bar or numeric HP | TL: x=12, y=12 | ~220×28 | Always | Prefer bar + number; clamp display to slice max |
| `hud_crypto` | `◈` / label **Crypto** + integer | TL: x=12, y=44 | ~220×28 | Always | Start **80**; round floats for display |
| `hud_fiat` | Label **Fiat** + `—` | TL: x=12, y=76 | ~220×24 | Always (grey) **or omit** | Disabled until Phase 2; no conversion |
| `hud_wanted` | Heat pips **0–3** | TR: right=12, y=12 | ~160×28 | Always | Stars or filled circles; max **3** |
| `hud_nap_rep` | Compact **NAP** + 0–100 int | TR: right=12, y=44 | ~160×28 | Always | Start **50**; toast on delta is Echo/Pulse, not this chrome |
| `hud_faction` | Tiny dual bar Agorist \| State | BL: x=12, bottom≈140 | ~200×20 | Always (secondary) | Two numbers OK if bars not ready |
| `hud_property` | Icon / **OWNED** / **—** | BL: x=12, bottom≈112 | ~200×24 | Always (secondary) | In-session ownership only for slice |
| `hud_mission` | One objective line | BC: centered, bottom≈72 | ≤720×28 | **Only** when Smuggle-01 active | States: Pickup / Drop / time remaining |
| `hud_minimap` | Square map stub | BR: right=12, bottom=12 | **96×96** | Always | No full legend; optional N marker |
| `hud_prompts` | Context control hints | BC: bottom=12, full width inset | height **24** | Contextual | Hide idle spam; show interact / enter / pause |

### 4.1 Mission line states (Smuggle-01)

| State | Example copy |
|-------|----------------|
| Pickup | `Smuggle-01: Collect package` |
| Carry | `Smuggle-01: Deliver · 2:18` |
| Drop / success flash | `Smuggle-01: Delivered · +120` (brief) |
| Inactive | Widget **hidden** (no empty bar) |

### 4.2 Fiat / crypto rules (Sol)

- **Crypto** is the only live currency in the slice.
- **Fiat** slot: omit entirely **or** show grey `—` — never a live number.
- Do not implement conversion UI in this HUD.

---

## 5. Control prompts

Thin strip above the bottom edge (under mission line when both show).

| Context | Prompts (examples) |
|---------|-------------------|
| On foot near interactable | `[E] Trade` / `[E] Buy property` |
| Near vehicle | `[F] Enter` |
| In vehicle | `[F] Exit` |
| Always available | `[Esc]/`/`[Tab] Pause` (pick one; don’t show both) |

Max **three** hints at once. Prefer glyphs matching the bound keys once Bolt/Vega expose bindings.

---

## 6. Readability rules

1. **No persistent chrome in the center clear zone** (see §2).
2. High contrast text on semi-transparent panels; avoid thin neon outlines for numbers.
3. Wanted pips must remain readable on bright asphalt and dark interiors.
4. Mission line wins over prompts if vertical space fights — stack mission above prompts.
5. Minimap never expands to a full legend in slice.
6. Damage / arrest feedback may flash the TL/TR clusters; do not relocate widgets mid-play.

---

## 7. Out of scope (slice)

- Inventory grid  
- Legislation panel / bills  
- Election timer  
- Full map legend / fog controls  
- Fiat live balance or exchange  
- Multi-mission log (only one Smuggle-01 line)  
- Fancy Ink chrome pass beyond placeholder panels (Ink owns final skin)

---

## 8. Placeholder values (§13)

| Constant | Slice value |
|----------|-------------|
| `START_CRYPTO` | 80 |
| `WANTED_MAX` | 3 |
| `NAP_START` | 50 |
| Property buy (reference) | 150 crypto |
| Minimap size | 96×96 px |

Full economy / wanted / NAP tables live in `GREY-MARKET-SLICE.md` §13 — HUD displays, it does not redefine them.

---

## 9. Handoff

| Role | Owns |
|------|------|
| **Sol** | Numbers, visibility rules, currency policy |
| **Frame** | Layout, regions, IDs, 1280×720 budgets (this doc) |
| **Ink** | Final visual chrome, type, palette |
| **Pulse** | UI beep / toast SFX IDs (see cue sheet) |
| **Vega** | Implementation when Reed unfreezes — **no tickets yet** |
| **Spike** | Smoke verifies glanceables once Client boots on Windows |

**Cross-link:** Treat `GREY-MARKET-SLICE.md` §10 as the authority list; this file is the spatial contract for that list.

---

*End of draft. Ping Reed when merged to `docs/ui/`; craft tickets wait for smoke unfreeze.*
