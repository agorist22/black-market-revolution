# Foundation research (2026-09-16)

## Reject as game base (GTA2 recreations)

| Project | Stack | License | Why not |
|---------|-------|---------|---------|
| [CriminalRETeam/gta2_re](https://github.com/CriminalRETeam/gta2_re) | C++ RE | unclear on GitHub | Needs original GTA2; goal is recreate GTA2 |
| [ikkentim/OpenGta2](https://github.com/ikkentim/OpenGta2) | C# MonoGame | MIT | Early; original assets |
| [gennariarmando/havoc2](https://github.com/gennariarmando/havoc2) | C++ OpenGL | MIT | Unplayable WIP; original assets |
| Legacy OpenGTA2 | C++ | various | Abandoned / broken tooling |

## Steal patterns only (Python)

| Project | Use |
|---------|-----|
| [clear-code-projects/pygame-gta2](https://github.com/clear-code-projects/pygame-gta2) | GTA2-style car rotation / thrust |
| [y4my4my4m/gta-clone](https://github.com/y4my4my4m/gta-clone) | Tiny WASD + enter/exit car boilerplate |

## Recommended product foundations

1. **Godot 4** — camera, collision, scenes, UI; original/CC0 art; port economy systems as scripts.
2. **Modular Pygame** — stay in Python; new package layout; optional lift of driving math from pygame-gta2.

Do not continue the single-file god script as the runtime.

## Decision locked (2026-09-16)

- **Title:** Black Market Revolution
- **Foundation (provisional):** [ikkentim/OpenGta2](https://github.com/ikkentim/OpenGta2) (C# / MonoGame, MIT) — designer chose open-source GTA2 base
- **Requires:** legal GTA2 install for assets; then replace with original Black Market Revolution content
- **Not using as fork base:** gta2_re (unclear license / RE), havoc2 (unplayable)
- **Reference only:** pygame-gta2 driving demo; old Python monoliths under `archive/`
