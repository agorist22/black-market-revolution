# Build & run (Windows)

Black Market Revolution’s runtime foundation is **OpenGta2** (MonoGame WindowsDX).

## Requirements

- Windows (x86 target / WindowsDX — **not Linux/macOS for the client**)
- .NET SDK that supports `net10.0-windows` (see `src/OpenGta2.Client/OpenGta2.Client.csproj`)
- A **legal** Grand Theft Auto 2 install (Steam / Rockstar Classics / retail)
- User environment variable `OPENGTA2_PATH` pointing at the GTA2 install root (the folder that contains `data\\bil.gmp` / `data\\bil.sty` — not the Start Menu shortcuts folder)

## Setup

1. Clone https://github.com/agorist22/black-market-revolution
2. Install GTA2 legally; note its install directory
3. Set a **User** env var (PowerShell example):

```powershell
[Environment]::SetEnvironmentVariable("OPENGTA2_PATH", "C:\Path\To\GTA2", "User")
```

4. Restart the terminal / Visual Studio so the var is visible
5. From repo root:

```powershell
dotnet restore OpenGta2.sln
dotnet build OpenGta2.sln -c Debug
dotnet run --project src/OpenGta2.Client/OpenGta2.Client.csproj -c Debug
```

Or open `OpenGta2.sln` in Visual Studio and F5.

## Do not

- Commit any GTA2 `.gmp` / style / audio / exe binaries into git
- Redistribute Rockstar assets

## Week 1 Definition of Done (engineering)

- [ ] Cold-start build succeeds on a clean Windows machine with documented SDK version
- [ ] `OPENGTA2_PATH` documented and verified
- [ ] Client launches against legal data (or fails with a clear missing-path error)
- [ ] Clean quit without crash
