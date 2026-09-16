# Atlas / export pipeline — Ink → MonoGame / OpenGta2

**Audience:** Ink (art), Forge (TA), Vega (engine), Bolt (tools)  
**Repo rule:** no Rockstar / GTA2 binaries, `.sty`, `.GMP`, or ripped sheets in git. Legal GTA2 is runtime-only via `OPENGTA2_PATH` until BMR-owned content replaces it.

This is the contract for how Ink’s sprites and tiles become something Vega can load without drama.

---

## Purpose

OpenGta2 does **not** currently draw the world from loose PNGs in MGCB. It reads GTA2-style **style** (`.sty`) and **map** (`.GMP`) data, decodes indexed pixels, then uploads GPU textures.

| Role | Owns |
|------|------|
| Ink | Source art, naming, remaps, readability |
| Forge | Budgets, export checklist, flag engine limits early |
| Vega | Load path, texture upload, shader budgets |
| Bolt | Packer / importer / validation tooling |

Long-term target: **BMR-authored packs** that feed the same `Style` / texture structures (`.sty`-compatible or a thin importer Vega signs off on). Do not invent a parallel PNG-only world path without Vega.

---

## Flow

```
Source (Aseprite/PSD)
  → export indexed PNGs (this doc’s naming)
  → packer (Bolt/Vega; TBD) → BMR style pack + manifest
  → OpenGta2 StyleReader / StyleTextureSet
  → GPU: tile Texture2DArray + per-sprite Texture2D
```

**Today (bootstrap):** client still points at a legal GTA2 install for data.  
**Slice target:** grey-market district uses **only BMR placeholders** once the packer lands.

MGCB (`Assets.mgcb`) currently owns **effects + debug SpriteFont**, not world atlases.

---

## Hard limits (from OpenGta2 code)

Verify against `OpenGta2.GameData` / `OpenGta2.Client` when changing loaders or `BlockFaceEffect.fx`.

### Tiles

| Rule | Value |
|------|-------|
| Size | **64×64** px fixed |
| Page packing | **4×4** tiles → **256×256** page |
| Color | 8-bit indexed; **index 0 = transparent** on upload |
| Soft ceiling | **~992** tiles (comment in `StyleTextureSet`) |
| GPU | One `Texture2DArray` slice per tile (`SurfaceFormat.Color`) |

### Sprites

| Rule | Value |
|------|-------|
| Page size | **256×256** indexed bytes |
| Max frame size | width/height are **`byte`** → max **255×255** (keep much smaller) |
| Kinds | `car` `ped` `codeobj` `mapobj` `user` `font` (engine: `SpriteKind`) |
| Remaps | Per-kind palette remaps (faction / coat tint) |
| GPU today | **One `Texture2D` per (kind, number, remap)** — not atlased on GPU yet |

Unique remaps multiply textures. Prefer one base + remap rows over full re-paints.

### Lighting (block faces)

| Rule | Value |
|------|-------|
| Point lights | **`MAX_LIGHTS = 16`** in `BlockFaceEffect.fx` |
| Art implication | Cluster neon; bake emissive into tiles where possible; ≤8 active in view for the slice |

---

## Naming conventions

All exports are **original BMR art**. Prefix `bmr_` keeps packs greppable and CI-friendly.

### Tiles

```
bmr_tile_<set>_<name>.png
```

Examples:

- `bmr_tile_greyroad_asphalt_01.png`
- `bmr_tile_facade_neon_shop_a.png`

Rules:

- Base resolution **64×64** (or exact integer multiple for source masters, export at 64)
- `<set>` = district or material family (`greyroad`, `facade`, `alley`, …)
- snake_case, ASCII, no spaces
- Do not encode palette index in the filename

### Sprites

```
bmr_<kind>_<name>_<anim>_<frame>.png
```

- `<kind>` ∈ `car` | `ped` | `codeobj` | `mapobj` | `user` | `font`
- `<anim>` = `idle` | `walk` | `drive` | … (or `static` for single frame)
- `<frame>` = zero-padded `00`, `01`, …

Examples:

- `bmr_ped_player_walk_00.png`
- `bmr_car_courier_van_static_00.png`
- `bmr_mapobj_drop_crate_static_00.png`

### Remaps

Same pixel silhouette; only remap-friendly indices change.

```
bmr_<kind>_<name>_remap_<id>.png
```

Or (preferred once packer exists): **one base sprite** + a palette-row sheet:

```
bmr_<kind>_<name>_palettes.png
```

Document which indices are remap slots in the manifest. Until then, explicit `_remap_<id>` files are fine.

### Manifest (proposed sidecar)

```
bmr_<pack>.atlas.json
```

Minimal fields (proposed — Bolt may rename):

```json
{
  "packId": "bmr_grey_market_v0",
  "tileSize": 64,
  "spritePageSize": 256,
  "tiles": [{ "file": "bmr_tile_greyroad_asphalt_01.png", "id": 0 }],
  "sprites": [{
    "file": "bmr_ped_player_walk_00.png",
    "kind": "ped",
    "name": "player",
    "anim": "walk",
    "frame": 0
  }],
  "notes": "No Rockstar content. Index 0 transparent."
}
```

Packer output may be `.sty`-shaped binary; the JSON is the **authoring** source of truth.

### Folders (source tree; not committed game data)

Suggested authoring layout (git-safe masters only if licensed/original; prefer LFS or art remote if heavy):

```
art/
  tiles/<set>/
  sprites/<kind>/<name>/
  remaps/
  manifests/
```

Built packs and anything derived from a GTA2 install stay **gitignored** / out of repo.

---

## Vertical-slice budgets (grey-market district)

Starter caps — Ink can swap art without breaking IDs if we freeze IDs early.

| Set | Budget | Why |
|-----|--------|-----|
| Ground / road / sidewalk tiles | ≤ 64 unique | One district floor language |
| Building face tiles | ≤ 128 unique | Facades; neon as tiles when possible |
| Ped (player + 2 NPC types) | Small turn/walk sets | Remaps for State vs Agorist |
| Vehicles | 1–2, few rotations | Same remap rule |
| Props / pickups | ≤ 32 | Mission drop, property marker |
| Active point lights in view | ≤ 8 of 16 | Headroom for wanted / flash |

Exact frame counts lock after Vega confirms camera zoom.

---

## Export checklist (Ink)

1. **Canvas:** tiles 64×64; sprites fit a 256×256 page with packing margin.
2. **Indexed color** for style-bound exports; **index 0 = empty / transparent**.
3. **No semi-transparent PNG alpha** in the sty path — bright opaque pixels + lights, or wait for a Vega alpha pass.
4. **Silhouette reads** at top-down GTA2-like zoom (thick shapes, high local contrast).
5. **Remaps** share value structure; only coat/faction indices change.
6. **Names** match this doc; no `gta2_`, `rockstar_`, or retail asset filenames.
7. **Masters** may be higher-res layered files; **exports** are the sized, indexed PNGs above.
8. Hand Forge/Bolt a manifest row (or updated `bmr_*.atlas.json`) with every new ID.

---

## Validation (proposed)

Bolt/CI ideas — mark implemented only when wired:

- Reject commits matching Rockstar/GTA2 data extensions under `art/` or `content/` (`.sty`, `.GMP`, known retail names)
- Assert tile PNGs are exactly 64×64 (or documented multiple before resize)
- Assert sprite PNGs width/height ≤ 255
- Count unique tiles/sprites against slice budgets
- Optional: hash denylist for known retail files

---

## Open questions

| ID | Question | Default if silent |
|----|----------|-------------------|
| AT-01 | Stay `.sty`-shaped vs new BMR pack format? | `.sty`-shaped until slice ships |
| AT-02 | Who implements the packer? | Bolt tools + Vega load; Forge specs |
| AT-03 | Camera zoom / ped pixel size? | Measure in `TestWorld`; then freeze frame budgets |
| AT-04 | Commit PNG masters to git vs art remote? | Small placeholders in repo OK; heavy masters external |

---

## Related

- Engine/legal overview: repo `README.md`, `gtadocs.md`
- Foundations / why OpenGta2: `docs/FOUNDATIONS.md`
- Slice scope: `docs/GREY-MARKET-SLICE.md` (if present)

*Numbers checked against OpenGta2 `Tile`, `SpriteEntry`, `StyleTextureSet`, and `BlockFaceEffect.fx`. Update this doc when those change.*
