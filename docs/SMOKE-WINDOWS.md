# Windows smoke runbook (Week 1 crashers)

**Owner:** Vega (Eng)  
**Scope:** Grey Arcade / grey-market slice only. No new systems.  
**Depends on:** [BUILD-WINDOWS.md](BUILD-WINDOWS.md), legal GTA2 install, PR #7 path hardening on `main`.

Env var (primary): **`OPENGTA2_PATH`** = GTA2 **install root** (parent of `data\`), must contain `data\bil.gmp` and `data\bil.sty`.  
Do **not** point at Start Menu shortcuts. Do **not** commit Rockstar binaries.

## One-shot setup (PowerShell)

```powershell
# 1) Set User env (persist). Replace with your real install root.
[Environment]::SetEnvironmentVariable(
  "OPENGTA2_PATH",
  "C:\Path\To\GTA2",   # <-- owner: real path (parent of data\)
  "User")

# 2) Process-scoped for *this* shell (also pick up User var after new terminal)
$env:OPENGTA2_PATH = [Environment]::GetEnvironmentVariable("OPENGTA2_PATH", "User")

# 3) Verify data markers (prefer scripts\verify-opengta2-path.ps1 if present)
if (Test-Path .\scripts\verify-opengta2-path.ps1) {
  .\scripts\verify-opengta2-path.ps1
} else {
  $root = $env:OPENGTA2_PATH
  if (-not $root) { throw "OPENGTA2_PATH is not set" }
  $gmp = Join-Path $root "data\bil.gmp"
  $sty = Join-Path $root "data\bil.sty"
  if (-not (Test-Path $gmp)) { throw "Missing $gmp — is OPENGTA2_PATH the install root?" }
  if (-not (Test-Path $sty)) { throw "Missing $sty" }
  Write-Host "OK: $gmp and $sty found under $root"
}
```

## Crashers 1–5 (report PASS/FAIL each)

From **repo root** on Windows with .NET 10 SDK:

```powershell
# (1) Full solution build
dotnet restore OpenGta2.sln
dotnet build OpenGta2.sln -c Debug
# PASS = exit 0, Client + GameData + tests compile

# (2) Client launch
dotnet run --project src/OpenGta2.Client/OpenGta2.Client.csproj -c Debug
# PASS = window opens; console prints "Using GTA2 data at: ..."
# FAIL without path = exit 1 + clear error (no NRE) per PR #7

# (3) Load map/data
# PASS = bil map/style load without crash (intro/test world appears)

# (4) Camera / player move (if available in current shell)
# PASS = camera follows / ped responds to input without hang

# (5) Clean quit
# PASS = Alt+F4 or window close returns to shell without crash dump
```

Automated helper: `scripts/smoke-crashers.ps1` runs **verify + (1) build** unattended. Steps **(2)–(5)** need an interactive desktop — run those by hand and tick the checklist below.

### Checklist

| # | Item | Result |
|---|------|--------|
| 1 | `dotnet build OpenGta2.sln` | ☐ PASS / ☐ FAIL |
| 2 | Client launch | ☐ PASS / ☐ FAIL |
| 3 | Load map/data (`bil.*`) | ☐ PASS / ☐ FAIL |
| 4 | Camera / player move | ☐ PASS / ☐ FAIL |
| 5 | Clean quit | ☐ PASS / ☐ FAIL |

Paste FAIL logs (first ~40 lines) to Spike. Grey Arcade play beats stay with Spike **after** 1–5 are green.

## Libraries-only (non-Windows / CI)

```powershell
dotnet build src/OpenGta2.GameData/OpenGta2.GameData.csproj -c Debug
dotnet test src/OpenGta2.GameData.UnitTests/OpenGta2.GameData.UnitTests.csproj -c Debug
```

Does **not** replace crasher (1) on Windows.
