# Smuggle-01 — narrative pack

**Internal id:** `Smuggle-01`  
**Display title:** Grey Lane Run  
**District:** The Grey Arcade  
**Payout (Sol):** +120 crypto on success · fail = forfeit cargo (no payout)  
**Geography (Atlas):** Accept Ledger Plaza → Wharf Pickup → Neon Alley → Alley Drop · heat seed Toll Spur

---

## Accept briefing

> **GREY LANE RUN** — Ledger Plaza  
> Grab the sealed crate at Wharf Pickup. Take Neon Alley to the Alley Drop before the Toll Spur inspectors finish their sweep.  
> **Risk:** Wanted climbs if you’re tagged while carrying. Hit wanted 3 and the run dies.  
> **Payout:** +120 crypto if the buyer never sees a badge.  
> Don’t start a speech. Start walking.

---

## Contact bark (accept)

> Board’s live. Wharf first, alley drop second. Keep it boring.

---

## UI strings

| Beat | String |
|------|--------|
| Objective (active) | Deliver the crate: Wharf Pickup → Alley Drop |
| Pickup prompt | Pick up sealed crate |
| Carry status | Cargo on — stay off Toll Spur heat |
| Drop prompt | Drop crate — buyer waiting |
| Timer warn (optional) | Clock’s eating the run |

---

## Success stinger

> Crate’s gone. Wallet’s heavier. Grey Arcade didn’t flinch.

---

## Fail stingers

| Fail | Stinger |
|------|---------|
| Timer / abandon (4 min) | Too slow. Buyer walked. Cargo’s someone else’s problem now. |
| Wanted 3 while carrying | Heat cooked the run. Inspectors took the crate; you keep the story. |
| Arrest / death while carrying | Lights, forms, empty hands. The plaza will still be here when you are. |

---

## Notes for eng / QA

- Strings are shippable HUD/ops copy; tone matches Echo house voice (clipped, one wry aside max on the brief).
- Do not surface NAP ideology text on this mission — heat/wanted is the lesson.
- Place names must match Atlas marker ids (`MARKER_CONTACT`, `MARKER_PICKUP`, `MARKER_DROP`).
