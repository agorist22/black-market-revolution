# Smoke — Grey Arcade (OpenGta2 + grey-market slice)

**Owner:** Spike (QA)  
**Status:** Crashers **1–5 PASS** on Prometheus (Week 1 shell gate closed, 2026-09-16). Items **6–13 BLOCKED** — BMR systems not in the OpenGta2 shell yet (next gate once Vega lands grey-market systems per Sol).  
**Scope:** Grey-market vertical slice only (The Grey Arcade). Pulse / Frame / Beacon stay docs-only until runtime to hook; Vega craft unfreeze is narrow (Sol eng order, start VS-04).  
**Report blockers to:** Reed · **Repros to:** Vega

## Authority

| Doc | Role |
|-----|------|
| `docs/world/NEON-MARKET-SLICE.md` | FROZEN district clip, landmarks, Smuggle-01 markers |
| `docs/GREY-MARKET-SLICE.md` | Sol systems locks (crypto only, one underground property, Smuggle-01, NAP/wanted) |
| `docs/BUILD-WINDOWS.md` | Windows build / run / `OPENGTA2_PATH` |
| `README.md` | Cold-start restore/build |

Do not expand past this slice. Do not treat Linux Client failures as item-1 product fails (Client is WindowsDX-only).

---

## Windows operator (required before items 2–13)

### Machine

- **OS:** Windows (x86 / WindowsDX client — not Linux/macOS)
- **SDK:** .NET 10 (`dotnet --version` reports `10.x`)
- **Legal GTA 2** install (Steam / Rockstar Classics / retail). Do not commit assets.

### `OPENGTA2_PATH`

Point the **User** (or process) env var at the GTA2 **install root** — the folder that contains `data\bil.gmp` (and typically `data\bil.sty`).

**PASS (path check):**

```powershell
Test-Path (Join-Path $env:OPENGTA2_PATH 'data\bil.gmp')
# expect True
```

**FAIL / do not proceed if:**

- Var unset, or points at Start Menu / shortcuts / Steam URL helpers
- `data\bil.gmp` missing → client should **fail loud** (path hardening on `main`; see PR #7). That is a setup fail, not a slice pass.

```powershell
[Environment]::SetEnvironmentVariable("OPENGTA2_PATH", "C:\Path\To\GTA2", "User")
# restart terminal / VS, then:
$env:OPENGTA2_PATH = "C:\Path\To\GTA2"   # process scope for this shell
```

### Build / run commands (repo root)

```powershell
dotnet restore OpenGta2.sln
dotnet build OpenGta2.sln -c Debug
dotnet run --project src/OpenGta2.Client/OpenGta2.Client.csproj -c Debug
```

---

## Checklist

Record each row: **PASS** | **FAIL** | **N/A** | **BLOCKED**. Crashers (1–5) gate the slice (6–13).

### Crashers first

| # | Check | PASS | FAIL |
|---|--------|------|------|
| 1 | `dotnet build OpenGta2.sln -c Debug` on Windows + .NET 10 | Exit 0, 0 errors | Any compile error |
| 2 | Client launch / boot to first scene | Window + first scene without unhandled exception | Crash, NRE, or hang before scene |
| 3 | Load map + game data via `OPENGTA2_PATH` | Map/data loads for play | Loud missing-path error, wrong root, or load crash |
| 4 | Camera / player move (if shell exposes it) | Camera follow + move input responds | No control, softlock, or camera stuck. **N/A** if shell has no player yet — note build SHA |
| 5 | Clean quit | Exit without hang or exception | Hang or crash on quit |

### Grey-market slice (only after 1–5 PASS)

**Current gate (2026-09-16):** Items **6–13 = BLOCKED** (not N/A). OpenGta2 shell boots, but crypto wallet/HUD, Underground Stack, Smuggle-01, wanted/NAP BMR systems are not implemented yet. Re-run this section when Vega lands systems (start: wallet + crypto HUD / VS-04). Keep rows on this checklist as the exit criteria.

Locks: **crypto only** · **one underground property** · **Smuggle-01** · Grey Arcade clip `(0,2000)–(2000,4000)`.

| # | Check | PASS | FAIL |
|---|--------|------|------|
| 6 | Walk in clipped Grey Arcade; spawn / hub readable (Ledger Plaza / spawn ~`(480, 2920)`) | Player moves in clip; cannot soft-leave clip unnoticed | Wrong map region, no move, or clip broken |
| 7 | Enter / drive / exit a car (if present) | Enter → drive → exit without softlock | Softlock or crash. **N/A** if no car in Week 1 shell — note it |
| 8 | Crypto earn (Crypto Trader ~`(280, 2760)`) | Balance increases (+25–40 band or documented stub); HUD updates | No earn, wrong currency, or HUD stale |
| 9 | Buy **Underground Stack** (~`(880, 3180)`, price **150**); income ticks in-session | OWNED state; +income on timer (~+8 / 15s per Sol) | Cannot buy, ownership lost mid-session, or no tick |
| 10 | **Smuggle-01:** accept contact ~`(760, 2840)` → Wharf pickup ~`(960, 3640)` → Alley drop ~`(1480, 3360)` → payout (~+120) | Full accept → objective → reward/fail path works once | Cannot accept, markers missing, or no payout/fail resolution |
| 11 | Wanted + police react | Wanted rises on observed crime / cargo heat; police react at least once (Toll Spur seed OK) | No wanted change or no police response when expected |
| 12 | NAP cue | Unprovoked aggression drops rep; defensive fight (e.g. police) does **not** | Defense penalized, or unprovoked aggression free |
| 13 | Stability | No per-frame log spam; empty input safe; session survives the loop | Spam logs, empty-input crash, or blocker mid-loop |

**Slice success (debug banner OK):** own property + completed smuggle once + saw wanted **and** NAP cue within ~10–15 min (Sol beats A–H).

---

## Results log

| # | Result | Notes | Date / build |
|---|--------|-------|----------------|
| 1 | **PASS** | Full `dotnet build OpenGta2.sln` green; Client `PlatformTarget=x64` | 2026-09-16 / `1ad17aa` |
| 2 | **PASS** | Window “Black Market Revolution” launches and stays up | 2026-09-16 / Prometheus |
| 3 | **PASS** | Log `Using GTA2 data at: …\GTA2`; map + player visible | 2026-09-16 / Prometheus |
| 4 | **PASS** | Player + camera in world; WASD/arrows sent, process healthy (eyes-on delta optional) | 2026-09-16 / Prometheus |
| 5 | **PASS** | `CloseMainWindow` → exit 0, no hang | 2026-09-16 / Prometheus |
| 6 | **BLOCKED** | BMR walk/clip systems not in shell yet | — |
| 7 | **BLOCKED** | BMR car loop not in shell yet | — |
| 8 | **BLOCKED** | Crypto earn / wallet not implemented | — |
| 9 | **BLOCKED** | Underground Stack buy/income not implemented | — |
| 10 | **BLOCKED** | Smuggle-01 not implemented | — |
| 11 | **BLOCKED** | Wanted / police BMR hook not implemented | — |
| 12 | **BLOCKED** | NAP rep cue not implemented | — |
| 13 | **BLOCKED** | Slice stability pass waits on 6–12 | — |

**Linux partial (reference only, not item-1 PASS):** `OpenGta2.GameData` library build can PASS on Linux; full solution Client build is out of scope there.

---

## Blockers / HOLD

- Missing Windows box, .NET 10, or valid `OPENGTA2_PATH` (`data\bil.gmp`) → park 2–13; tell **Reed**
- Crasher FAIL → stop; file clear repro for **Vega** (steps, expected, actual, SHA)
- Pulse / Frame / Beacon work stays offline until Reed unfreezes after smoke starts landing
