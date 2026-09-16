# Black Market Revolution

Top-down open-world action-adventure + economic/political simulation. You are a
neutral entrepreneur in **Neon Market**, caught between the authoritarian State
(fiat, taxes, police) and the Counter-Economy (crypto, voluntary exchange, NAP).

**Feel:** GTA 2 freedom, Tropico-style business management, Suzerain-style
political pressure — satirical, rebellious, strategic.

## Foundation

This repository is scaffolded from **[OpenGta2](https://github.com/ikkentim/OpenGta2)**
(C# / MonoGame, MIT) by Tim Potze. See `LICENSE` and `NOTICE`.

A separate reference fork of upstream lives at
https://github.com/agorist22/open-gta2-upstream if you want an unmodified mirror.

## Legal assets (required to run)

OpenGta2 loads **original GTA2 data files**. You must own a legal copy of GTA 2
(Steam / Rockstar Classics / retail). **Do not** commit game binaries or assets
to this repo. Long-term plan: replace maps/art/missions with original Black
Market Revolution content so the game no longer depends on Rockstar assets.

## Build / run

1. Install a current .NET SDK compatible with the solution in `OpenGta2.sln`.
2. Obtain a legal GTA2 install and point the project at its data (see upstream
   OpenGta2 docs / `gtadocs.md`).
3. Open `OpenGta2.sln` and build, or use `dotnet build` from the repo root.

Upstream is early-stage — expect incomplete gameplay until we harden the shell
and swap in BMR systems (dual currency, properties, NAP, factions).

## Project docs

| Doc | Purpose |
|-----|---------|
| [docs/PROJECT.md](docs/PROJECT.md) | Pitch & vertical slice |
| [docs/ROADMAP.md](docs/ROADMAP.md) | Phases |
| [docs/BACKLOG.md](docs/BACKLOG.md) | Prioritized backlog |
| [docs/WEEK-1.md](docs/WEEK-1.md) | Near-term plan |
| [docs/DECISIONS.md](docs/DECISIONS.md) | Decision log |
| [docs/FOUNDATIONS.md](docs/FOUNDATIONS.md) | Why OpenGta2 (and what we rejected) |
| [docs/CODE_AUDIT.md](docs/CODE_AUDIT.md) | Old Python prototype audit |

## Archive

`archive/` holds the discarded single-file Pygame prototype for reference only.
