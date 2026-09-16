# Week 1 plan — grey-market slice on OpenGta2

**Systems design (authoritative for loop/numbers):** [`GREY-MARKET-SLICE.md`](GREY-MARKET-SLICE.md) — earn crypto → underground property → one smuggle → wanted + NAP. Atlas district blockout and Frame HUD should follow that doc.

**Legacy note:** Earlier checklist below targeted a recovered Pygame v2 monolith. That path is **archive/reference only**; do not treat Python stabilize as the Week 1 ship gate. Keep useful smoke-test intent (one district, no elections, no dual-currency depth).

---

## Archived Pygame stabilize checklist (reference)

**Context:** Prototype code exists (v2 baseline in `archive/agorist_revolution_v2.py`). Week 1 is **make v2 launchable and clip to a vertical slice**, not greenfield scaffolding.

**Audit:** `CODE_AUDIT.md` · **Slice decision:** D-02 provisional (Agorist / grey-market loop).

## Goal of the week

Ship a **runnable v2** that boots without NameErrors, with logging quiet enough to debug, playable in **one district**, and scoped to the grey-market first loop — no new systems.

## Checklist (in order)

1. [ ] Confirm v2 file is complete under `archive/agorist_revolution_v2.py`; document run command + Python/Pygame pin in README (`TECH-01`)
2. [ ] Fix crash blockers: `Bullet` before `RotatableSprite`; `SpatialGrid` `.grid.remove` misuse; `update_missions` bare `clock_time`/`game_state`
3. [ ] Strip per-frame / spammy logging (keep ERROR/WARNING for real faults)
4. [ ] Resolve SPACE shoot vs trade key conflict (rebind one action)
5. [ ] Clip playable area to one district-sized region on the 4000×4000 map (`VS-03`)
6. [ ] Smoke-test: launch → walk/drive → touch earn → property hook → one mission stub → wanted/NAP cue without crash (`TECH-03`)
7. [ ] Leave v1 untouched in archive; do not merge features back into v1
8. [ ] Update this checklist Friday; note remaining blockers in ≤5 bullets

## Definition of done for Friday

- v2 launches from a documented command without import/class-order crashes
- Logging is not spamming every frame
- Player can move in a bounded district with camera follow
- Vertical slice (D-02) remains the only feature target; no election/combat expansion
- Known remaining bugs listed (honest, short)

## What to defer

- New features beyond the grey-market vertical slice
- Elections, full legislation set, win-condition polish
- Full mission catalog, extra special NPCs, random events
- Deep combat / car juice beyond fixing the SPACE conflict
- Final marketing rebrand (D-01 already provisional)
- Collaborator search (D-03)
- Save/load polish beyond "does not crash if present"
- Refactoring the god script into packages (only if required to fix bugs)
