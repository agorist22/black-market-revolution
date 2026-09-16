# Risks

Top risks for Black Market Revolution / Black Market Economy. Mitigations are actionable for a solo part-time designer-dev.

| # | Risk | Why it hurts | Mitigation |
|---|------|--------------|------------|
| 1 | **Scope creep across 9 pillars** | Economy + politics + open-world combat + events is several games; solo bandwidth collapses | Lock Phase 1 vertical slice to one district, one currency path, one property, one mission; refuse new pillars until Phase exit criteria green |
| 2 | **Dual-currency complexity** | Fiat inflation + crypto risk + conversion creates balance exploits and opaque UX | Ship single currency in the slice; add second currency only in Phase 2 with parallel pricing first, conversion second; instrument balances in debug HUD |
| 3 | **NAP feel vs fun** | Punishing all violence can feel preachy or soft; ignoring NAP kills the philosophical spine | Code aggression vs defense tags early (`VS-08`); make defense against police/raids satisfying; keep unprovoked aggression a *economic/rep* cost, not a hard softlock |
| 4 | **Pygame scale limits** | 2000×2000 open world + AI + economy ticks may tank FPS or architecture | Camera culling, simple spatial partitions, tick economy slower than frame rate; set a perf budget in Phase 0 (`TECH-03`); delay particle/juice until systems stable |
| 5 | **Two working titles** | Split branding confuses docs, builds, and collaborators | Keep dual name in `PROJECT.md` / `DECISIONS.md`; pick one display title before external playtests (Phase 5); use one folder/repo name (`agorist-revolution`) consistently |
| 6 | **Solo bandwidth** | Part-time solo cannot parallelize art, systems, and balance | Timebox phases in person-weeks; cut P2 content first; decide collaborator need explicitly (`DECISIONS.md`); prefer greybox placeholders over custom art until alpha |

## Review cadence

Revisit this list at each phase exit. Promote new risks only if they block the next phase; retire mitigated ones with a one-line note dated in git history.
