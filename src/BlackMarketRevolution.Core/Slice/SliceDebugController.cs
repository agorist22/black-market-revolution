using BlackMarketRevolution.Economy;
using BlackMarketRevolution.Missions;
using BlackMarketRevolution.Nap;
using BlackMarketRevolution.Wanted;

namespace BlackMarketRevolution.Slice;

/// <summary>
/// VS-09 debug reset + slice-complete evaluation for the grey-market slice.
/// See docs/GREY-MARKET-SLICE.md §12.
/// </summary>
public sealed class SliceDebugController
{
    private readonly PlayerWallet _wallet;
    private readonly UndergroundProperty _property;
    private readonly SmuggleMission _mission;
    private readonly WantedMeter _wanted;
    private readonly NapReputation _nap;
    private readonly StreetTrade _streetTrade;

    public SliceDebugController(
        PlayerWallet wallet,
        UndergroundProperty property,
        SmuggleMission mission,
        WantedMeter wanted,
        NapReputation nap,
        StreetTrade streetTrade)
    {
        _wallet = wallet;
        _property = property;
        _mission = mission;
        _wanted = wanted;
        _nap = nap;
        _streetTrade = streetTrade;
    }

    /// <summary>
    /// Slice complete when: owned property AND ≥1 smuggle success AND wanted ever ≥1
    /// AND NAP feedback ever shown (live evaluation; clears after reset).
    /// </summary>
    public bool SliceComplete =>
        _property.Owned
        && _mission.SuccessCount >= 1
        && _wanted.EverRaised
        && _nap.FeedbackEverShown;

    /// <summary>
    /// Reset wanted, NAP, crypto to start; clear mission + slice “ever” counters;
    /// clear street-trade cooldown; keep property ownership (and its income/raid timers).
    /// </summary>
    public void ResetKeepProperty()
    {
        _wallet.Crypto = PlayerWallet.StartCrypto;
        _wanted.ResetForDebug();
        _nap.ResetForDebug();
        _mission.ResetForDebug();
        _streetTrade.ResetForDebug();
    }

    /// <summary>
    /// Same as <see cref="ResetKeepProperty"/> plus clear property ownership.
    /// </summary>
    public void ResetClearProperty()
    {
        ResetKeepProperty();
        _property.ClearOwnershipForDebug();
    }
}
