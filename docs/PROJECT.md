# Black Market Revolution — Project Overview

## Pitch

In the neon-soaked near-future city of Neon Market, you are a neutral entrepreneur caught between an authoritarian State that taxes, polices, and inflates fiat currency, and a Counter-Economy of Agorists who trade in crypto, voluntary exchange, and mutual aid. Build businesses (legal, underground, or cooperative), navigate dual currencies and shifting laws, uphold or break the Non-Aggression Principle, and tip city control toward freedom or control — a top-down open-world action-adventure fused with Tropico-style management and Suzerain-style political pressure.

## Title

**Black Market Revolution** (decided 2026-09-16). Former working names: Black Market Revolution, Black Market Economy. See `DECISIONS.md`.

## Target feel / comps

- **GTA 2** — top-down freedom, chaos, cars, wanted level, sandbox missions
- **Tropico** — business/property management, passive income, policy levers
- **Suzerain** — consequential political decisions, faction pressure, tone of statecraft under constraint

**Tone:** satirical, rebellious, strategic — not grimdark, not pure comedy.

## Core fantasy

Start as a nobody with a small stake. Earn through legal fronts, underground deals, and mutual aid. Feel fiat inflate under your feet while crypto offers freedom and risk. Defend yourself without becoming the aggressor (NAP). Watch politicians rewrite the rules mid-game. Unlock legendary Agorist contacts as your influence grows. Win by making one side dominate the city — or lose by letting the other do it first.

## Tech stack

- **Language / engine:** Python + Pygame
- **Map:** ~2000×2000 px city
- **Controls (stated):** WASD movement; driveable cars
- **Platforms:** TBD (desktop assumed for prototype) — see `DECISIONS.md`

## Current status

**Prototype code exists (not greenfield).** Two concatenated Pygame monoliths were recovered from a paste (2026-09-16):

- **Baseline:** `archive/agorist_revolution_v2.py` (Black Market Revolution; MAP 4000×4000) — stabilize this.
- **Archive only:** `archive/black_market_economy_v1.py` (Black Market Economy; MAP 2000×2000).

See `CODE_AUDIT.md` for bugs, systems inventory, and Week 1 stabilize plan. Dual naming narrowed provisionally in `DECISIONS.md` (D-01).

## Success definition — vertical slice

A playable loop in **one district** where the player can:

1. Move and (optionally) drive on the map
2. Earn/spend on **one** currency path (fiat *or* crypto — pick one for the slice)
3. Buy **one** property that generates passive income (with a visible risk/tax hook)
4. Complete **one** mission type (e.g. delivery or smuggling)
5. Trigger a basic **wanted** reaction and a clear **NAP** feedback cue (rep up/down)
6. Feel the fantasy in **10–15 minutes** without tutorials beyond on-screen hints

Exit criteria for Phase 1 are defined in `ROADMAP.md`.
