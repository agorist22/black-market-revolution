# Audio Cue Sheet v0 — Grey-Market Slice

**Owner:** Pulse (Audio Director)  
**Status:** Narrow craft unfreeze (Reed) — wanted tick/stinger + NAP-break toast only; no scope creep  
**Aligns:** `GREY-MARKET-SLICE.md` · Grey Arcade smoke path · middleware-light MonoGame (Vega)  
**Out of scope:** FMOD/Wwise, cleared licensed music, full open-world beds, dual-currency UI SFX depth, engine PRs

**Hard rule:** Prefer CC0 / original pipelines. Uncleared copyrighted tracks must not ship.

---

## 1. Purpose

Stable cue IDs and naming so Vega can later hook a thin `AudioCue` table in OpenGta2 without renaming mid-slice. This doc is **not** an implementation ticket.

Implementation starts from these IDs — do not invent parallel names in code. **Active craft:** §5 wanted tick/alert + §5b NAP-break toast (hooks that already fire on VS-07/08).

---

## 2. Naming conventions

| Prefix | Use | Example |
|--------|-----|---------|
| `sfx_<cat>_<name>` | One-shots | `sfx_car_horn` |
| `amb_<zone>_<mood>` | Loops / beds | `amb_market_state` |
| `mus_<context>_<intensity>` | Music beds / tension | `mus_wanted_chase_2` |
| `stinger_<event>` | Short hits / bumps | `stinger_wanted_alert` |
| `ui_<action>` | UI feedback | `ui_buy_confirm` |

**Rules**

- Lowercase, underscores only.
- One file = one playable cue (no `final_v3` / `new_` in names).
- `<cat>` stays short: `car`, `impact`, `npc`, `prop`, `weapon` (slice: keep car + impact first).
- Paths (suggested): `Content/Audio/{sfx|amb|mus|stinger|ui}/<id>.wav` (or `.ogg` if pipeline prefers).

---

## 3. Categories / buses (middleware-light)

Four buses are enough for the slice:

| Bus | Duck / note |
|-----|-------------|
| **SFX** | Ones-hots; duck slightly under Wanted stingers |
| **Amb** | Market loops; duck under chase music |
| **Music** | Wanted / mission beds |
| **UI** | Always readable; never fully ducked |

No FMOD required for v0. Play by cue id from a table (see §8).

---

## 4. Cars (priority 1)

| Cue ID | Type | Loop | Notes |
|--------|------|------|-------|
| `sfx_car_engine_idle` | sfx | yes | Soft idle; crossfade with cruise |
| `sfx_car_engine_cruise` | sfx | yes | Mid RPM bed |
| `sfx_car_engine_push` | sfx | yes | High RPM / boost feel |
| `sfx_car_brake` | sfx | no | Short chirp |
| `sfx_car_collision` | sfx | no | Body thump; reuse for light props if needed |
| `sfx_car_horn` | sfx | no | Player / NPC horn |

Slice max: one vehicle voice set. Extra car variants = Phase 2.

---

## 5. Wanted (priority 2)

Escalate in **2–3 levels** only (matches readable heat, not a full heat ladder).

| Cue ID | Type | Loop | Level | Notes |
|--------|------|------|-------|-------|
| `ui_wanted_tick` | ui | no | 0→1 | Soft tick when heat appears |
| `stinger_wanted_alert` | stinger | no | 1 | First alert bump |
| `mus_wanted_chase_1` | mus | yes | 1–2 | Low chase bed |
| `mus_wanted_chase_2` | mus | yes | 2–3 | Escalate; crossfade from `_1` |
| `stinger_wanted_clear` | stinger | no | any→0 | Optional relief hit when heat drops |

Duck `amb_*` under chase music; keep `ui_*` clear.

---


## 5b. NAP break toast (priority 2 — active craft)

Fired by VS-08 when the player breaks the NAP (unprovoked civ harm). Keep it short and readable over ambience — same bus family as UI.

| Cue ID | Type | Loop | Trigger | Notes |
|--------|------|------|---------|-------|
| `ui_nap_break` | ui | no | Unprovoked NAP violation toast | Soft “you broke the code” tick under Frame’s toast |
| `stinger_nap_break` | stinger | no | Same event (optional layer) | Slightly heavier bump if toast alone is too quiet; do **not** play both loud |

**Active thin pack (Reed unfreeze):** play `ui_wanted_tick` on wanted 0→1+, `stinger_wanted_alert` on first alert / chase engage, and `ui_nap_break` (or soft `stinger_nap_break`) on NAP-break toast. Chase beds (`mus_wanted_chase_*`) stay nice-after unless Vega has free loop bandwidth.

## 6. Market ambience — State vs Counter-Economy (priority 3)

Two mutually exclusive (or heavily crossfaded) beds for the same district block.

### State order (`amb_market_state`)

| Cue ID | Type | Loop | Character |
|--------|------|------|-----------|
| `amb_market_state` | amb | yes | Clean AC hum, distant PA, sparse footsteps |
| `amb_market_state_pa` | amb | optional one-shots | Occasional muffled PA blip (sparse) |

### Counter-Economy / neon chaos (`amb_market_chaos`)

| Cue ID | Type | Loop | Character |
|--------|------|------|-----------|
| `amb_market_chaos` | amb | yes | Neon buzz, crowd bed, vendor chatter bed, bass leak from somewhere illegal |
| `amb_market_chaos_neon` | amb | yes (layer) | Optional neon buzz layer if chaos bed is too dense alone |

Atlas zone tags / Ink tone decide which bed wins; Pulse supplies both IDs. Default for Grey Arcade: **chaos** in the underground stack area, **state** on the ordered street edge if both exist in the blockout.

---

## 7. UI beeps (priority 4)

| Cue ID | Type | Notes |
|--------|------|-------|
| `ui_buy_confirm` | ui | Property / trader purchase OK |
| `ui_sell_confirm` | ui | Sell / payout confirm (can share asset with buy if needed) |
| `ui_funds_deny` | ui | Insufficient crypto |
| `ui_mission_accept` | ui | Smuggle-01 accept |
| `ui_wanted_tick` | ui | Same as §5 (listed once in content pack) |
| `ui_map_ping` | ui | Objective / marker ping (optional if mission sting covers it) |

Keep UI short (<200 ms) and mid-frequency so they cut through ambience.

---

## 8. Mission cues (priority 5)

Aligned to Smuggle-01 beat chart in `GREY-MARKET-SLICE.md`.

| Cue ID | Type | Loop | Beat |
|--------|------|------|------|
| `stinger_mission_briefing` | stinger | no | Accept / briefing open |
| `ui_objective_ping` | ui | no | Pickup / drop marker focus (alias of `ui_map_ping` OK) |
| `stinger_mission_success` | stinger | no | Drop complete + payout |
| `stinger_mission_fail` | stinger | no | Bust / fail |

Do not add a full mission music suite in v0; wanted chase covers heat during the run.

---

## 9. Suggested `AudioCue` table (docs only)

For Vega when Reed unfreezes — **documentation sketch**, not a code PR:

| Field | Example |
|-------|---------|
| `Id` | `sfx_car_horn` |
| `Path` | `Content/Audio/sfx/sfx_car_horn.ogg` |
| `Volume` | `0.8` |
| `Loop` | `false` |
| `Category` | `SFX` \| `Amb` \| `Music` \| `UI` |

Play/stop/fade by `Id`. Category drives bus routing and ducking.

---

## 10. Slice content pack (minimum ship set)

**Must-have for first audible Grey Arcade pass**

1. **Active (Reed thin unfreeze):** `ui_wanted_tick`, `stinger_wanted_alert`, `ui_nap_break`  
2. Car: idle, cruise, collision, horn  
3. Wanted chase bed: `mus_wanted_chase_1` (nice-after if loops are heavy)  
4. Ambience: `amb_market_chaos` (state bed can wait one beat)  
5. UI: buy confirm, funds deny, mission accept  
6. Mission: success + fail stingers  

**Nice-after**

- Engine push / brake  
- `mus_wanted_chase_2`, clear stinger  
- `amb_market_state` + neon layer  
- Briefing stinger, map ping  

---

## 11. Coordination

| Who | When |
|-----|------|
| **Ink** | Tone pass: State order vs neon chaos character |
| **Echo** | Narrative moments that need a sting (briefing copy, faction lines) |
| **Frame** | UI events that fire `ui_*` (confirm / deny / wanted tick) |
| **Vega** | Thin play calls for active craft IDs (see §5 / §5b) — one small hook ticket |
| **Spike** | No audio gate on smoke 1–13; audible pass is post-smoke |

---

## 12. Explicit non-goals

- No full audio middleware / FMOD  
- No car / market / mission pack in this unfreeze (wanted tick + NAP toast only)  
- No store-page / trailer audio promises (Beacon HOLD)  
- No copyrighted temp tracks checked into the repo  

## 12b. Thin Vega ticket (active)

One small eng ask when needed: play-by-id for `ui_wanted_tick`, `stinger_wanted_alert`, `ui_nap_break` on the existing VS-07/08 events (wanted pip up / alert engage / NAP-break toast). Placeholder CC0 one-shots OK. No new systems beyond a tiny cue map + Play(id).

---

## Changelog

| Ver | Date | Notes |
|-----|------|-------|
| v0 | 2026-09-16 | Initial naming + Grey Arcade cue IDs (Pulse) |
| v0.1 | 2026-09-16 | Narrow unfreeze: wanted tick/alert + NAP-break toast; thin Vega hook note |
