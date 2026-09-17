using System;
using BlackMarketRevolution.Nap;
using BlackMarketRevolution.Wanted;
using OpenGta2.Client.Audio;

namespace OpenGta2.Client.Components;

/// <summary>
/// Thin Pulse hooks: play cue-sheet ids on VS-07/08 events.
/// No Faction HUD. No cars. Stub OK when WAVs are missing.
/// </summary>
public sealed class AudioCueHookComponent : BaseComponent
{
    /// <summary>Wanted alert threshold — first heat pip (cue sheet §5).</summary>
    public const int WantedAlertThreshold = 1;

    private readonly AudioCueService _cues;
    private readonly WantedMeter _wanted;
    private readonly NapReputation _nap;
    private bool _subscribed;

    public AudioCueHookComponent(
        GtaGame game,
        AudioCueService cues,
        WantedMeter wanted,
        NapReputation nap) : base(game)
    {
        _cues = cues;
        _wanted = wanted;
        _nap = nap;
    }

    public override void Initialize()
    {
        if (!_subscribed)
        {
            _wanted.Raised += OnWantedRaised;
            _wanted.ChaseStarted += OnChaseStarted;
            _nap.NapBroken += OnNapBroken;
            _subscribed = true;
        }

        base.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _subscribed)
        {
            _wanted.Raised -= OnWantedRaised;
            _wanted.ChaseStarted -= OnChaseStarted;
            _nap.NapBroken -= OnNapBroken;
            _subscribed = false;
        }

        base.Dispose(disposing);
    }

    private void OnWantedRaised(int before, int after)
    {
        // ui_wanted_tick — every successful Raise that increases heat.
        _cues.Play(AudioCueIds.UiWantedTick);

        // stinger_wanted_alert — wanted hits alert threshold (0 → ≥1).
        if (before < WantedAlertThreshold && after >= WantedAlertThreshold)
            _cues.Play(AudioCueIds.StingerWantedAlert);
    }

    private void OnChaseStarted() =>
        _cues.Play(AudioCueIds.StingerWantedAlert);

    private void OnNapBroken() =>
        _cues.Play(AudioCueIds.UiNapBreak);
}
