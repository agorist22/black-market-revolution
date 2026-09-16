using BlackMarketRevolution.Economy;

namespace BlackMarketRevolution.Wanted;

/// <summary>
/// VS-07 shared wanted heat 0–3 for the grey-market slice.
/// See docs/GREY-MARKET-SLICE.md §7 and docs/BALANCE-GREY-ARCADE.md W-01…W-03.
/// </summary>
public sealed class WantedMeter
{
    public const int MaxLevel = 3;
    public const float DecaySeconds = 45f;
    public const float ArrestCryptoFactor = 0.5f;

    /// <summary>Current heat 0–<see cref="MaxLevel"/>.</summary>
    public int Level { get; private set; }

    /// <summary>Seconds accumulated toward the next −1 while out of LOS.</summary>
    public float OutOfLosElapsedSeconds { get; private set; }

    /// <summary>True while police have LOS / chase contact (decay paused).</summary>
    public bool InLos { get; private set; }

    /// <summary>Smoke/stub chase flag — nearest officer “engaged” when wanted ≥ 1.</summary>
    public bool ChaseEngaged { get; private set; }

    /// <summary>True after an arrest this session until cleared (HUD flash cue).</summary>
    public bool ArrestFlashPending { get; private set; }

    /// <summary>
    /// Raise heat by <paramref name="amount"/> (default 1), clamped to <see cref="MaxLevel"/>.
    /// Marks LOS so decay does not immediately eat the new star.
    /// Returns the new level.
    /// </summary>
    public int Raise(int amount = 1)
    {
        if (amount <= 0)
            return Level;

        Level = Math.Min(MaxLevel, Level + amount);
        SetLos(true);
        return Level;
    }

    /// <summary>Debug/smoke: set level directly (clamped). Resets decay; clears chase if 0.</summary>
    public void SetLevelForDebug(int level)
    {
        Level = Math.Clamp(level, 0, MaxLevel);
        OutOfLosElapsedSeconds = 0f;
        if (Level == 0)
        {
            ChaseEngaged = false;
            InLos = false;
        }
    }

    /// <summary>
    /// Police LOS / combat contact. When true, decay timer resets.
    /// When false, decay can tick.
    /// </summary>
    public void SetLos(bool inLos)
    {
        InLos = inLos;
        if (inLos)
            OutOfLosElapsedSeconds = 0f;
    }

    /// <summary>
    /// Start chase/engage if wanted ≥ 1. Sets LOS. Returns false if ignored.
    /// </summary>
    public bool TryEngageChase()
    {
        if (Level < 1)
            return false;

        ChaseEngaged = true;
        SetLos(true);
        return true;
    }

    /// <summary>End chase (player escaped / disengaged). Clears LOS so decay can run.</summary>
    public void EndChase()
    {
        ChaseEngaged = false;
        SetLos(false);
    }

    /// <summary>
    /// Arrest stub: crypto × <see cref="ArrestCryptoFactor"/> (integer floor), wanted → 0,
    /// chase/LOS cleared. Property ownership is caller-owned and must be kept.
    /// </summary>
    public void Arrest(PlayerWallet wallet)
    {
        wallet.Crypto = (int)Math.Floor(wallet.Crypto * ArrestCryptoFactor);
        Level = 0;
        OutOfLosElapsedSeconds = 0f;
        InLos = false;
        ChaseEngaged = false;
        ArrestFlashPending = true;
    }

    /// <summary>Consume the one-shot arrest flash for HUD.</summary>
    public bool ConsumeArrestFlash()
    {
        if (!ArrestFlashPending)
            return false;
        ArrestFlashPending = false;
        return true;
    }

    /// <summary>
    /// Advance decay while out of LOS. −1 star every <see cref="DecaySeconds"/>.
    /// While chase is engaged, keep LOS sticky so heat does not decay mid-chase.
    /// </summary>
    public void Tick(float deltaSeconds)
    {
        if (deltaSeconds <= 0f)
            return;

        if (ChaseEngaged && Level >= 1)
            SetLos(true);

        if (InLos || Level <= 0)
        {
            OutOfLosElapsedSeconds = 0f;
            return;
        }

        OutOfLosElapsedSeconds += deltaSeconds;
        while (OutOfLosElapsedSeconds >= DecaySeconds && Level > 0)
        {
            OutOfLosElapsedSeconds -= DecaySeconds;
            Level--;
            if (Level == 0)
            {
                OutOfLosElapsedSeconds = 0f;
                ChaseEngaged = false;
            }
        }
    }
}
