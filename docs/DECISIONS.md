# Decisions log

Status values: **Undecided** | **Provisional** | **Decided**. Seeded open items below. Add new rows; do not delete history — mark superseded.

| ID | Decision | Status | Options / notes | Outcome |
|----|----------|--------|-----------------|---------|
| D-01 | **Final game title** | Decided | Was Black Market Revolution / Black Market Economy | **Black Market Revolution** — designer: more marketable (2026-09-16) |
| D-02 | **Vertical slice district / first playable loop** | Provisional | Which Neon Market district? Earn → buy property → one mission → wanted/NAP beat. Pick for Week 1. | **Agorist district / grey market first** — see `GREY-MARKET-SLICE.md`: earn → one underground property → one smuggle → wanted + NAP. Currency locked separately in D-07. |
| D-03 | **Solo vs needing collaborators** | Undecided | Stay solo through Phase 2 vs bring art/audio/design help for Phase 4+ | — |
| D-04 | **Target platforms** | Undecided | Desktop only for now (Windows/Linux/macOS) vs later consoles/web. Prototype assumes desktop. | — |
| D-05 | **Scope of combat** | Undecided | Keep light / mostly reactive & defensive (recommended by pillars) vs deeper shooter systems | — |
| D-06 | **Engine / foundation** | Provisional | Was modular Pygame default. Designer chose open-source GTA2 base (2026-09-16). | **ikkentim/OpenGta2** (C#/MonoGame, MIT) as fork base; needs legal GTA2 assets; replace content with Black Market Revolution. Rejected gta2_re (license/RE) and havoc2 (unplayable). |

## How to close a decision

1. Write the choice in **Outcome** with a short why.
2. Set **Status** to Decided (or Provisional if reversible before Phase exit).
3. Mirror consequential outcomes into `PROJECT.md` / `ROADMAP.md` / backlog priorities.
