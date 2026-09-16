# Code audit — pasted prototype dump (2026-09-16)

## Status

**File recovery note (2026-09-16):** v1 was written to disk (580 lines). The exact v2 source lived only in chat and is **not** on disk yet — need the designer to re-attach `agorist_revolution_v2.py`, or rebuild from design + audit.

**Prototype exists — not greenfield.** The user paste contained **two complete Pygame games concatenated** into one dump:

| Generation | File | Caption | Map |
|------------|------|---------|-----|
| **v1** | `archive/black_market_economy_v1.py` | Black Market Economy | 2000×2000 |
| **v2** | *(chat paste only — file not yet recovered to disk; stub placeholder at* `archive/agorist_revolution_v2.py`*)* | Black Market Revolution: Austrian Economics Simulator | 4000×4000 |

Split rule used: first game ends at a line that is exactly `pygame.quit()`; second starts at the next `import pygame` (may be glued as `pygame.quit() import pygame` in the raw paste).

**Recommendation:** Treat **v2 as the working baseline**. Keep **v1 as archive only** (historical / comparison). Do not continue feature work on v1.

## Architecture

Both builds are **single-file god scripts**: constants, enums, SpatialGrid, sprites, economy, game state, menu, and main loop in one monolith. No package layout, no tests, no `requirements.txt` in the dump itself.

v2 screen target: **1280×720**; map **4000×4000**.

## Systems already present in v2

- RotatableSprite base; drawn Car / Player / NPC graphics
- Bullet / shooting
- Collectibles
- Missions
- Minimap
- Pause + save/load (pickle)
- Special NPCs (Brandon Aragon, Roger Ver)
- NAP reputation damage when attacking non-aggressors
- Dual currency (fiat / crypto), mining, exchange
- Property upgrades; tourism / crypto-mining property types
- Wanted / police pressure patterns (inherited design)
- Menu + main loop scaffolding

(v1 also had: SpatialGrid, cars, traders/police, properties, economy manager with inflation/hurricanes/elections, tax evasion, assassins, inventory food heal, SEK3 quote / Free Roger Ver chrome — but is **not** the baseline.)

## Critical bugs that block a clean run

1. **`Bullet` inherits `RotatableSprite` before `RotatableSprite` is defined** → `NameError` at import/class-body time in v2.
2. **`SpatialGrid` misuse via `.grid.remove`** — callers treat `grid` like a Sprite Group / list instead of the cell-key dict API (`add_sprite` / `update_sprite` / `get_nearby`). Crash or silent corruption when removing sprites.
3. **`update_missions` uses bare `clock_time` / `game_state`** instead of method parameters / `self` → `NameError` when missions update.
4. **SPACE key conflict: shoot vs trade** — same key bound to both actions; ambiguous / broken interaction.
5. **First game (v1) missing `SCREEN_WIDTH` / `SCREEN_HEIGHT`** — uses those names for `set_mode` and camera clamp but never defines them → immediate `NameError` on launch.
6. **Logging spam** — per-frame `logging.info` ("Starting frame" / "Frame finished") and frequent economy events will thrash disk and bury real errors (especially v1; v2 may retain older patterns).

Also noted in v1 (non-blocking for "use v2" but informative): `GameState.unlock_special_npc` is called but not defined in the paste; wealth_level may be read before first NPCTrader.update.

## Scope risk

Too many systems for Phase 1. Elections, multi-drug legislation, assassins, dual full currencies, large 4000×4000 map, combat, and mission frameworks all landed before a stable vertical slice.

**Recommend:** freeze new features; stabilize a **vertical slice** only.

## Provisional PM decision

**Vertical slice = Agorist / grey-market first loop:**

earn crypto or dirty fiat → buy illegal / mutual-aid property → one smuggle/deliver mission → wanted + NAP feedback.

## Recommended Week 1 (code)

1. Make **v2 launchable** (fix class-order / NameError crash bugs first).
2. Strip logging spam (no per-frame INFO).
3. Fix SpatialGrid remove path and `update_missions` name binding.
4. Resolve SPACE shoot vs trade (rebind one action).
5. Clip playable area to **one district** (ignore rest of 4000×4000 for the slice).
6. Do **not** expand elections / win conditions / new systems this week.
