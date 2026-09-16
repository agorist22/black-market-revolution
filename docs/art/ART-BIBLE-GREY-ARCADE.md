# Art Bible v0 — Grey Arcade vs State Fringe

**Owner:** Ink (Art Direction)  
**Status:** Week 1 lock for grey-market / Grey Arcade slice  
**Aligns:** `GREY-MARKET-SLICE.md` · `world/NEON-MARKET-SLICE.md` · `tech-art/ATLAS-PIPELINE.md` (PR #12 naming lock) · `DECISIONS.md` (D-02)  
**Audience:** Frame (HUD), Forge (atlas), Atlas (district read), Vega (hooks), Spike (visual smoke)

---

## 1. Purpose and scope

This bible locks **visual identity** for the **10–15 minute** Grey Arcade vertical slice:

- **Grey Arcade** = Agorist / Counter-Economy block (primary play space)
- **State fringe** = adjacent official look used only to sell the border mood

**In scope:** palette, silhouette/tile readability, sprite naming hooks, HUD chrome notes, placeholder→final pipeline, CC0/original reference policy.

**Out of scope (do not art for Week 1):** elections UI, dual-currency depth, mutual-aid property art, city-wide packs, trailer marketing polish.

**Hard repo rule:** CC0 / original BMR art only. **No Rockstar / GTA2 binaries, ripped `.sty` / `.GMP` sheets, trademarked logos, or GTA-font echoes** in git. Legal GTA2 data stays runtime-only via `OPENGTA2_PATH` until BMR packs replace it.

---

## 2. Readability pillars

1. **Silhouette first.** Top-down, OpenGta2-scale: shape and color blocks beat fine detail. Threat, faction, and money type must read at mid-zoom.
2. **Two worlds, one map.** State fringe is colder and flatter. Grey Arcade pushes neon, wet asphalt, illegal signage, denser clutter. The border is a **mood shift**, not a single palette swap on identical props.
3. **HUD is street language.** Fiat vs crypto, wanted, faction bar share the same chrome grammar as the district — high contrast, cheap glow, no drop-shadow chrome.
4. **Solo pipeline.** Placeholders stay on-brand. Export obeys `tech-art/ATLAS-PIPELINE.md` so art never paints past engine limits.

---

## 3. Palette (slice lock)

### 3.1 World

| Zone | Role | Hex | Notes |
|------|------|-----|-------|
| State fringe base | Concrete / asphalt | `#2A2E35` | Cool, flat fills |
| State fringe mid | Curb / signage metal | `#4A5160` | Official clutter |
| State accent | Sodium vapor | `#C9922A` | Street lamps, State UI accents |
| Arcade base | Night ground | `#0D0F14` | Near-black wet asphalt |
| Arcade accent A | Neon cyan | `#2EE6D6` | Signage, crypto-adjacent glow |
| Arcade accent B | Neon magenta | `#FF2BD6` | Illegal signage, heat cues |
| Shared neutral | Paper / chalk | `#E8E2D6` | Readable labels on dark |

**Rule:** Never put **both** cyan and magenta accents on the same control or the same small prop. One accent per readable unit.

### 3.2 HUD / economy colors

| Channel | Hex / ramp | Slice behavior |
|---------|------------|----------------|
| Crypto (active) | `#2EE6D6` | Always on; primary wallet |
| Fiat (reserved) | amber/gold `#C9922A` at ≤40% opacity or greyed | Slot reserved; **no earn/spend** in slice |
| Wanted 0→3 | magenta `#FF2BD6` → red `#E53935` | Pips, not stars |
| Faction bar | Single saturated faction color on `#0D0F14` track | Bottom-center |
| Health | Cool light `#E8E2D6` on dark | Top-left cluster |

---

## 4. Silhouette and tile rules

Align hard limits with `tech-art/ATLAS-PIPELINE.md`:

| Rule | Value |
|------|-------|
| Tile size | **64×64** fixed |
| Page pack | 4×4 → **256×256** |
| Color | 8-bit indexed; **index 0 = transparent** |
| Lights | Design for **≤8** active point lights in view; engine hard max **16** in `BlockFaceEffect` |
| Neon | **Cluster** sources; bake emissive into tiles where possible — do not light every window |

### 4.1 State fringe tiles (`set = state`)

- Flatter roofs, official signage, sodium pools, low clutter density
- Edges read as “regulated” — cleaner curb lines, fewer overhangs

### 4.2 Grey Arcade tiles (`set = neon`)

- Denser props, illegal / hand-painted signage, wet specular patches (indexed as bright value, not real specular)
- Landmark silhouettes must stay unique at mid-zoom (hub, property, smuggle pickup/drop)

### 4.3 Shared / road / prop

Use `shared`, `road`, `prop` sets for connectors so Atlas can clip one district without duplicating curb kits.

---

## 5. Sprite rules

| Kind (folder) | Use in slice |
|---------------|--------------|
| `ped` | Player, contact, trader, police |
| `car` | Optional; keep tiny frames if used |
| `mapobj` | Property marker, crates, interactables |
| `codeobj` | Mission markers if not tile-baked |
| `user` / `font` | Only if Frame/Vega need atlas fonts |

**Naming** (matches atlas pipeline / PR #12):

- Tiles: `bmr_<set>_<name>_64.png`
- Sprites: `bmr_<kind>_<name>_<frame>.png` — frame **3-digit** (`000`, `001`…)
- Remaps: `*_remap_<id>.png` (faction / coat tint)
- UI sources: `bmr_ui_<screen>_<element>.png`

Keep frame w/h **well under 255**. Prefer base + remap over full re-paints (GPU cost).

**Source folders:**

```
art/source/tiles/<set>/     # state | neon | shared | road | prop
art/source/sprites/<kind>/
art/source/remap/
art/source/ui/
art/export/pages/           # packed 256×256 only — never commit loose world PNGs as playable maps
```

---

## 6. HUD chrome notes (Frame) — 1280×720

| Spec | Lock |
|------|------|
| Type | Condensed sans; meter values ALL CAPS **16–18px**; labels **11–12px**; menu body **14px**; title **22–24px** |
| Contrast | Actionable values ≥ **4.5:1** on dark chrome |
| Radius | **2px** meters/chips; **0px** wanted pips |
| Borders | **1px** hairlines; **no drop shadows**; neon glow soft and cheap only |
| Safe margin | **16px** from screen edge |
| Layout | Health + currencies **top-left**; wanted **top-right**; faction **bottom-center** |
| Slice wallets | Crypto live; fiat **greyed/disabled** (see Sol’s slice locks) |
| Property | Distinct world marker + **OWNED** state (readable at mid-zoom) |

No Rockstar-style star wanted iconography. Wanted = pips / bar only.

---

## 7. Placeholder → final (solo pipeline)

1. **Silhouette** — 1-bit / flat shape reads at mid-zoom  
2. **Color block** — zone palette only; no detail noise  
3. **Indexed final** — 64² tiles / sprite frames; index 0 clear; remap rows if faction tint needed  
4. **Pack** — Forge/Bolt packer → pages under `art/export/pages/`  
5. **Smoke** — Spike: Arcade vs State fringe readable; HUD chrome matches this doc; no illegal assets in tree  

**Acceptance (art for slice A–H):**

- [ ] Grey Arcade block reads Counter-Economy vs State fringe in ≤3 seconds of stillness  
- [ ] Smuggle pickup/drop and underground property markers distinct  
- [ ] HUD: crypto, health, wanted, faction bar match §3.2 + §6  
- [ ] Fiat slot absent or clearly disabled  
- [ ] Zero Rockstar / ripped sheets in repo  

---

## 8. CC0 / reference policy

**Allowed reference categories (no embeds of copyrighted stills in repo):**

- Real-world night markets, alleys, sodium street lighting (photo study only)
- Public-domain / CC0 neon signage and geometric UI kits
- Original BMR sketches and Aseprite sources under `art/source/`

**Banned:**

- Rockstar / GTA / GTA2 art, UI, fonts, audio, or ripped style pages
- Trademarked logos as readable signage
- Shipping placeholder text that names real commercial IP

When in doubt, redraw from silhouette rules + this palette.

---

## 9. Coordination

| Role | Ask of this bible |
|------|-------------------|
| Frame | Implement chrome from §6; flag exceptions |
| Forge | Keep atlas doc + this naming in sync (PR #12) |
| Atlas | Landmark silhouettes / zone border for Grey Arcade clip |
| Vega | Hooks for OWNED marker, crypto HUD, wanted pips |
| Echo | Signage flavor may follow palette; no lecture boards |
| Spike | Visual smoke against acceptance list |

**v0 freeze:** Palette, HUD layout, and silhouette rules stay until after slice playtest A–H. Detail pass only inside those locks.
