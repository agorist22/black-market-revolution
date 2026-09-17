# Atlas px → OpenGta2 world (Grey Arcade)

**Authority:** `docs/world/NEON-MARKET-SLICE.md` (FROZEN) + `GreyArcadeMarkers`  
**Constant:** `GreyArcadeMarkers.AtlasToWorldScale = 256 / 4000 = 0.064`

| Space | Units | Notes |
|-------|-------|--------|
| Atlas | px on parent 4000×4000 | Contract for markers / CLIP / spawn |
| OpenGta2 | GTA2 block units (~256×256 map) | `Ped.Position.X/Y` |

**Formula:** `world = atlas_px * 0.064` · `atlas = world / 0.064`  
**Interact radius:** 48 Atlas px ≈ **3.072** world units  

**Locked interact pads (Atlas):** CONTACT (720,2900), PICKUP (1000,3520), DROP (1340,3280), PROPERTY (920,3280), TRADER (320,2840), TRADER (280,2760)  
**CLIP:** (0,2000)–(2000,4000) · **SPAWN:** (480,2920)

If Windows overlay shows player atlas coords that do not match expected district placement on `bil`, adjust `WorldMapBlocks` / add an offset in `GreyArcadeMarkers` and re-verify — do not change Atlas numbers.
