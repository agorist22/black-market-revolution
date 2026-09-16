using BlackMarketRevolution.Slice;

namespace BlackMarketRevolution.Economy;

/// <summary>
/// VS-05 single underground business for the grey-market slice.
/// See docs/GREY-MARKET-SLICE.md §5 and docs/BALANCE-GREY-ARCADE.md (P-01…P-05).
/// Raid-risk procs are surfaced to callers; wire to <c>WantedMeter.Raise</c> (VS-07).
/// </summary>
public sealed class UndergroundProperty
{
    public const int BuyPrice = 150;
    public const int IncomeAmount = 8;
    public const float IncomePeriodSeconds = 15f;
    public const float RaidPeriodSeconds = 90f;
    public const float RaidChance = 0.12f;

    public string Id { get; } = GreyMarketSliceIds.PropertyIllegal;

    /// <summary>In-session ownership; persists death/arrest until scene reset.</summary>
    public bool Owned { get; private set; }

    /// <summary>Seconds accumulated toward the next income payout.</summary>
    public float IncomeElapsedSeconds { get; private set; }

    /// <summary>Seconds accumulated toward the next raid-risk roll.</summary>
    public float RaidElapsedSeconds { get; private set; }

    public int IncomeTickCount { get; private set; }

    public int RaidProcCount { get; private set; }

    /// <summary>
    /// Spend <see cref="BuyPrice"/> crypto and mark owned. Refuses if already owned or wallet cannot pay.
    /// </summary>
    public bool TryPurchase(PlayerWallet wallet)
    {
        if (Owned)
            return false;

        if (!wallet.TrySpendCrypto(BuyPrice))
            return false;

        Owned = true;
        IncomeElapsedSeconds = 0f;
        RaidElapsedSeconds = 0f;
        return true;
    }

    /// <summary>
    /// Advance income / raid timers while owned.
    /// Returns crypto earned this call (0 or a multiple of <see cref="IncomeAmount"/>).
    /// <paramref name="raidFired"/> is true when a raid-risk roll succeeds (caller raises wanted).
    /// </summary>
    public int Tick(float deltaSeconds, Random? random, out bool raidFired)
    {
        raidFired = false;
        if (!Owned || deltaSeconds <= 0f)
            return 0;

        var paid = 0;
        IncomeElapsedSeconds += deltaSeconds;
        while (IncomeElapsedSeconds >= IncomePeriodSeconds)
        {
            IncomeElapsedSeconds -= IncomePeriodSeconds;
            paid += IncomeAmount;
            IncomeTickCount++;
        }

        RaidElapsedSeconds += deltaSeconds;
        var rng = random ?? Random.Shared;
        while (RaidElapsedSeconds >= RaidPeriodSeconds)
        {
            RaidElapsedSeconds -= RaidPeriodSeconds;
            if (rng.NextDouble() < RaidChance)
            {
                raidFired = true;
                RaidProcCount++;
            }
        }

        return paid;
    }

    /// <summary>Debug/smoke: force a raid-risk proc without waiting on the timer.</summary>
    public bool ForceRaidForDebug()
    {
        if (!Owned)
            return false;

        RaidProcCount++;
        return true;
    }

    /// <summary>VS-09: clear ownership and income/raid counters (ResetClearProperty).</summary>
    public void ClearOwnershipForDebug()
    {
        Owned = false;
        IncomeElapsedSeconds = 0f;
        RaidElapsedSeconds = 0f;
        IncomeTickCount = 0;
        RaidProcCount = 0;
    }
}
