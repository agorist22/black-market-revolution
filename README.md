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

Cold-start checklist for a clean machine. Week 1 Definition of Done is **documented
build**, not a full client playtest.

### Requirements

- **.NET 10 SDK** — solution targets `net10.0` / `net10.0-windows`. There is no
  `global.json` yet; use whatever current .NET 10 SDK restores cleanly
  (`dotnet --version` should report 10.x). Pin a `global.json` once a known-good
  SDK is confirmed on a Windows box.
- **Windows** for `OpenGta2.Client` — that project is `net10.0-windows`, x86,
  MonoGame **WindowsDX** + WinForms. The full game client will not build/run on
  Linux/macOS today.
- Other projects (`OpenGta2.GameData`, `OpenGta2.GameData.UnitTests`,
  `OpenGta2.DebugConsole`) target `net10.0` and may build outside Windows once
  the SDK is installed.
- A **legal GTA 2** install to **run** the client (Steam / Rockstar Classics /
  retail). See also `gtadocs.md`. **Do not** commit Rockstar/GTA2 binaries or
  assets.

### Build

From the repo root:

```bash
dotnet restore OpenGta2.sln
dotnet build OpenGta2.sln -c Debug
```

Libraries / tests only (no Windows client):

```bash
dotnet build src/OpenGta2.GameData/OpenGta2.GameData.csproj -c Debug
dotnet test src/OpenGta2.GameData.UnitTests/OpenGta2.GameData.UnitTests.csproj -c Debug
```

Or open `OpenGta2.sln` in Visual Studio on Windows.

### GTA2 data path (`OPENGTA2_PATH`)

`TestGamePath` (Client and DebugConsole) reads the **user** environment variable
`OPENGTA2_PATH` and expects an absolute path to the GTA2 data directory.

**Current behavior (PR #7):** Client resolves **User or Process** `OPENGTA2_PATH`,
requires `data\bil.gmp`, and **exits 1 with a clear error** if missing/wrong (no NRE).
See [docs/BUILD-WINDOWS.md](docs/BUILD-WINDOWS.md) and the Week 1 smoke checklist in
[docs/SMOKE-WINDOWS.md](docs/SMOKE-WINDOWS.md) (`scripts/smoke-crashers.ps1`).

Example (Windows PowerShell, user scope):

```powershell
[Environment]::SetEnvironmentVariable("OPENGTA2_PATH", "C:\Path\To\GTA2", "User")
```

Example (process scope for one shell):

```powershell
$env:OPENGTA2_PATH = "C:\Path\To\GTA2"
```

Never commit a machine-specific path or game files.

### Run (Windows + data present)

```bash
dotnet run --project src/OpenGta2.Client/OpenGta2.Client.csproj -c Debug
```

Upstream OpenGta2 is early-stage. Expect incomplete gameplay until we harden the
shell and swap in Black Market Revolution systems (dual currency, properties,
NAP, factions). Full smoke playtest stays gated on a confirmed legal GTA2 copy
and a Windows machine.

## Project docs

| Doc | Purpose |
|-----|---------|
| [docs/PROJECT.md](docs/PROJECT.md) | Pitch & vertical slice |
| [docs/ROADMAP.md](docs/ROADMAP.md) | Phases |
| [docs/BACKLOG.md](docs/BACKLOG.md) | Prioritized backlog |
| [docs/GREY-MARKET-SLICE.md](docs/GREY-MARKET-SLICE.md) | Grey-market vertical slice systems (Sol) |
| [docs/world/NEON-MARKET-SLICE.md](docs/world/NEON-MARKET-SLICE.md) | Grey Arcade district blockout (Atlas) |
| [docs/BUILD-WINDOWS.md](docs/BUILD-WINDOWS.md) | Windows cold-start pointer |
| [docs/WEEK-1.md](docs/WEEK-1.md) | Near-term plan |
| [docs/DECISIONS.md](docs/DECISIONS.md) | Decision log |
| [docs/FOUNDATIONS.md](docs/FOUNDATIONS.md) | Why OpenGta2 (and what we rejected) |
| [docs/CODE_AUDIT.md](docs/CODE_AUDIT.md) | Old Python prototype audit |

## Archive

`archive/` holds the discarded single-file Pygame prototype for reference only.
