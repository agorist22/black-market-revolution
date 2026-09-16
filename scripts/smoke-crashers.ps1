<#
.SYNOPSIS
  Week 1 Windows smoke: verify OPENGTA2_PATH markers, then full solution build.
.NOTES
  Crashers 2–5 (launch / map / move / quit) require an interactive desktop — run those manually per docs/SMOKE-WINDOWS.md.
  Does not commit or copy GTA2 assets.
#>
[CmdletBinding()]
param(
  [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $repoRoot

Write-Host "== BMR smoke-crashers ==" -ForegroundColor Cyan
Write-Host "Repo: $repoRoot"

$verify = Join-Path $PSScriptRoot "verify-opengta2-path.ps1"
if (Test-Path $verify) {
  Write-Host "Running scripts\verify-opengta2-path.ps1 ..."
  & $verify
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} else {
  Write-Host "verify-opengta2-path.ps1 not found — inline marker check"
  $root = [Environment]::GetEnvironmentVariable("OPENGTA2_PATH", "User")
  if (-not $root) { $root = $env:OPENGTA2_PATH }
  if (-not $root) {
    Write-Error "OPENGTA2_PATH is not set. See docs/BUILD-WINDOWS.md / docs/SMOKE-WINDOWS.md"
    exit 1
  }
  $gmp = Join-Path $root "data\bil.gmp"
  $sty = Join-Path $root "data\bil.sty"
  if (-not (Test-Path -LiteralPath $gmp)) {
    Write-Error "Missing $gmp (OPENGTA2_PATH='$root'). Point at GTA2 install root (parent of data\)."
    exit 1
  }
  if (-not (Test-Path -LiteralPath $sty)) {
    Write-Error "Missing $sty under OPENGTA2_PATH='$root'."
    exit 1
  }
  Write-Host "OK markers: $gmp , $sty"
}

Write-Host "Crashing (1): dotnet build OpenGta2.sln -c $Configuration"
dotnet restore OpenGta2.sln
if ($LASTEXITCODE -ne 0) { Write-Error "restore FAILED"; exit $LASTEXITCODE }
dotnet build OpenGta2.sln -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { Write-Error "Crashing (1) BUILD FAILED"; exit $LASTEXITCODE }

Write-Host "Crashing (1) PASS" -ForegroundColor Green
Write-Host "Next: interactive crashers 2–5 — see docs/SMOKE-WINDOWS.md"
exit 0
