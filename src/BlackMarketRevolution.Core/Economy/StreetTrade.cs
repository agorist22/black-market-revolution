namespace BlackMarketRevolution.Economy;

/// <summary>
/// Grey Arcade street trader earn loop: one interactable NPC, random crypto reward, real-time cooldown.
/// See docs/GREY-MARKET-SLICE.md §4 / street earn and docs/BALANCE-GREY-ARCADE.md E-01…E-03.
/// </summary>
public sealed class StreetTrade
{
    /// <summary>BALANCE E-01 TRADE_REWARD_MIN.</summary>
    public const int RewardMin = 25;

    /// <summary>BALANCE E-02 TRADE_REWARD_MAX (inclusive).</summary>
    public const int RewardMax = 40;

    /// <summary>BALANCE E-03 TRADE_COOLDOWN_S (real-time seconds).</summary>
    public const float CooldownSeconds = 30f;

    /// <summary>True when the trader will accept a trade interact.</summary>
    public bool IsReady => CooldownElapsedSeconds >= CooldownSeconds;

    /// <summary>Seconds accumulated since last successful trade (caps at <see cref="CooldownSeconds"/>).</summary>
    public float CooldownElapsedSeconds { get; private set; } = CooldownSeconds;

    /// <summary>Seconds remaining until the next trade is allowed (0 when ready).</summary>
    public float CooldownRemainingSeconds =>
        IsReady ? 0f : Math.Max(0f, CooldownSeconds - CooldownElapsedSeconds);

    /// <summary>Successful street trades this session.</summary>
    public int TradeCount { get; private set; }

    /// <summary>
    /// Advance cooldown. No-op when already ready or <paramref name="deltaSeconds"/> ≤ 0.
    /// </summary>
    public void Tick(float deltaSeconds)
    {
        if (IsReady || deltaSeconds <= 0f)
            return;

        CooldownElapsedSeconds = Math.Min(CooldownSeconds, CooldownElapsedSeconds + deltaSeconds);
    }

    /// <summary>
    /// Pay a random inclusive reward in [<see cref="RewardMin"/>, <see cref="RewardMax"/>] and start cooldown.
    /// Refuses while on cooldown.
    /// </summary>
    public bool TryTrade(PlayerWallet wallet, out int reward, Random? random = null)
    {
        reward = 0;
        if (!IsReady)
            return false;

        var rng = random ?? Random.Shared;
        reward = rng.Next(RewardMin, RewardMax + 1);
        wallet.AddCrypto(reward);
        TradeCount++;
        CooldownElapsedSeconds = 0f;
        return true;
    }

    /// <summary>VS-09 / debug: clear cooldown and trade counter so the trader is immediately ready.</summary>
    public void ResetForDebug()
    {
        CooldownElapsedSeconds = CooldownSeconds;
        TradeCount = 0;
    }
}
