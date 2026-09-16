# Deliver-01 — narrative pack

**Internal id:** `Deliver-01`  
**Display title:** Quiet Drop  
**District:** The Grey Arcade  
**Template:** Same accept → pickup → drop loop as Smuggle-01; **softer wanted fantasy** (Sol slice lock)  
**Payout (Sol §4b / PR #11):** +60 crypto on success · fail = forfeit package (same 240s timer)  
**Geography:** Reuse Grey Arcade markers; Service Alley is valid alt path flavor

---

## Accept briefing

> **QUIET DROP** — Ledger Plaza  
> Same bones as a smuggle: pickup, carry, drop. Lower profile. Prefer Service Alley if Neon Alley’s loud.  
> **Risk:** Heat still exists — just don’t go looking for Toll Spur.  
> **Payout:** +60 crypto on clean handoff.  
> Be a courier, not a headline.

---

## Contact bark (accept)

> Easy errand. Easy money. Don’t invent drama on the way.

---

## UI strings

| Beat | String |
|------|--------|
| Objective (active) | Quiet Drop: pickup → drop |
| Pickup prompt | Collect sealed packet |
| Carry status | Packet on — keep it quiet |
| Drop prompt | Hand off — no questions |
| Soft heat hint | Badge nearby — take the long way |

---

## Success stinger

> Handed off clean. No clipboard. Crypto lands like it should.

---

## Fail stingers

| Fail | Stinger |
|------|---------|
| Timer / abandon | Window closed. Packet goes cold; try again when the plaza breathes. |
| Wanted max while carrying | Soft job, hard heat. They took the packet; you took the lesson. |
| Arrest / death while carrying | Even quiet runs end in forms sometimes. Hub’s still open. |

---

## Notes for eng / QA

- Deliver-01 payout locked at +60 crypto (Sol PR #11 §4b). No scripted Toll Spur chase by default; Week 1 smoke still prioritizes Smuggle-01.
- Do not ship Deliver-01 as a second district or second property loop — same Grey Arcade clip only.