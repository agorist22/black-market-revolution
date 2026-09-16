using BlackMarketRevolution.Economy;
using BlackMarketRevolution.Slice;

namespace BlackMarketRevolution.Missions;

/// <summary>
/// VS-06 Smuggle-01 state machine: accept → pickup → carry (timer) → deliver / fail.
/// See docs/GREY-MARKET-SLICE.md §6 and docs/BALANCE-GREY-ARCADE.md M-01…M-07.
/// Wanted is a cheap stub (full police AI = VS-07).
/// </summary>
public sealed class SmuggleMission
{
    public const int RewardCrypto = 120;
    public const float TimeLimitSeconds = 240f;
    public const float RetryDelaySeconds = 60f;
    public const int WantedFailThreshold = 3;
    public const int WantedMax = 3;

    public string Id { get; } = GreyMarketSliceIds.MissionSmuggle;

    public SmuggleMissionPhase Phase { get; private set; } = SmuggleMissionPhase.Available;

    /// <summary>Seconds elapsed while carrying (toward <see cref="TimeLimitSeconds"/>).</summary>
    public float CarryElapsedSeconds { get; private set; }

    /// <summary>Seconds elapsed in post-fail cooldown.</summary>
    public float CooldownElapsedSeconds { get; private set; }

    /// <summary>Slice wanted stub 0–3 (VS-07 owns real police). Fail if ≥3 while carrying.</summary>
    public int WantedLevel { get; private set; }

    public float TimeRemainingSeconds =>
        Phase == SmuggleMissionPhase.Carrying
            ? Math.Max(0f, TimeLimitSeconds - CarryElapsedSeconds)
            : 0f;

    public bool IsActive =>
        Phase is SmuggleMissionPhase.AwaitingPickup or SmuggleMissionPhase.Carrying;

    public bool IsCarrying => Phase == SmuggleMissionPhase.Carrying;

    /// <summary>Accept / start when available (contact or Week 1 debug hotkey).</summary>
    public bool TryStart()
    {
        if (Phase != SmuggleMissionPhase.Available)
            return false;

        Phase = SmuggleMissionPhase.AwaitingPickup;
        CarryElapsedSeconds = 0f;
        CooldownElapsedSeconds = 0f;
        return true;
    }

    /// <summary>Pickup crate (proximity stub or hotkey). Starts the carry timer.</summary>
    public bool TryPickup()
    {
        if (Phase != SmuggleMissionPhase.AwaitingPickup)
            return false;

        Phase = SmuggleMissionPhase.Carrying;
        CarryElapsedSeconds = 0f;
        return true;
    }

    /// <summary>
    /// Deliver at drop point. Pays <see cref="RewardCrypto"/> and clears to Available.
    /// Success while wanted ≥ 1 still pays (heat already priced).
    /// </summary>
    public bool TryDeliver(PlayerWallet wallet)
    {
        if (Phase != SmuggleMissionPhase.Carrying)
            return false;

        wallet.AddCrypto(RewardCrypto);
        Phase = SmuggleMissionPhase.Available;
        CarryElapsedSeconds = 0f;
        CooldownElapsedSeconds = 0f;
        return true;
    }

    /// <summary>
    /// Cheap heat stub: police LOS at pickup / while carrying → wanted +1 (cap 3).
    /// If wanted hits fail threshold while carrying, mission fails (no payout).
    /// </summary>
    public bool NotifyPoliceLos()
    {
        if (WantedLevel < WantedMax)
            WantedLevel++;

        if (Phase == SmuggleMissionPhase.Carrying && WantedLevel >= WantedFailThreshold)
        {
            Fail();
            return true;
        }

        return false;
    }

    /// <summary>Fail if carrying (death / arrest). No-op otherwise.</summary>
    public bool NotifyDeathOrArrest()
    {
        if (Phase != SmuggleMissionPhase.Carrying)
            return false;

        Fail();
        return true;
    }

    /// <summary>Debug/smoke: set wanted directly (clamped). Fails mission if carrying and ≥3.</summary>
    public bool SetWantedForDebug(int level)
    {
        WantedLevel = Math.Clamp(level, 0, WantedMax);
        if (Phase == SmuggleMissionPhase.Carrying && WantedLevel >= WantedFailThreshold)
        {
            Fail();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Advance carry timer / cooldown.
    /// <paramref name="failed"/> is true when the carry timer expired this tick.
    /// </summary>
    public void Tick(float deltaSeconds, out bool failedByTimer)
    {
        failedByTimer = false;
        if (deltaSeconds <= 0f)
            return;

        if (Phase == SmuggleMissionPhase.Carrying)
        {
            CarryElapsedSeconds += deltaSeconds;
            if (CarryElapsedSeconds >= TimeLimitSeconds)
            {
                Fail();
                failedByTimer = true;
            }

            return;
        }

        if (Phase == SmuggleMissionPhase.Cooldown)
        {
            CooldownElapsedSeconds += deltaSeconds;
            if (CooldownElapsedSeconds >= RetryDelaySeconds)
            {
                Phase = SmuggleMissionPhase.Available;
                CooldownElapsedSeconds = 0f;
            }
        }
    }

    private void Fail()
    {
        // Prefer forfeit cargo (BALANCE M-02): no wallet debit, no payout.
        Phase = SmuggleMissionPhase.Cooldown;
        CarryElapsedSeconds = 0f;
        CooldownElapsedSeconds = 0f;
    }
}

public enum SmuggleMissionPhase
{
    Available,
    AwaitingPickup,
    Carrying,
    Cooldown
}
