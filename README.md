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

**Client is Windows-only** (`net10.0-windows` + MonoGame WindowsDX). See
**[docs/BUILD-WINDOWS.md](docs/BUILD-WINDOWS.md)** for cold-start steps and
`OPENGTA2_PATH`.

Short version:

1. Windows machine + .NET SDK for `net10.0-windows`
2. Legal GTA2 install; set user env `OPENGTA2_PATH` to that folder
3. `dotnet build OpenGta2.sln` then run `src/OpenGta2.Client`

Upstream OpenGta2 is early-stage. We harden the shell, then layer Black Market
Revolution systems (dual currency, properties, NAP, factions) on the grey-market
vertical slice (`docs/GREY-MARKET-SLICE.md`).

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
