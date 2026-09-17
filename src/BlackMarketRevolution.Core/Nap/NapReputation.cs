namespace BlackMarketRevolution.Nap;

/// <summary>
/// VS-08 NAP reputation 0–100 for the grey-market slice.
/// See docs/GREY-MARKET-SLICE.md §8 and docs/BALANCE-GREY-ARCADE.md N-01…N-04.
/// </summary>
public sealed class NapReputation
{
    public const int StartValue = 50;
    public const int MinValue = 0;
    public const int MaxValue = 100;

    public const int HitCivDelta = -15;
    public const int KillCivDelta = -25;
    public const int SmuggleCleanDelta = 5;

    /// <summary>Toast copy when an unprovoked NAP break applies (Echo/Pulse cue later).</summary>
    public const string BrokeNapToast = "You broke the NAP";

    private readonly Queue<string> _toastQueue = new();

    /// <summary>Current NAP Rep, clamped 0–100. Starts at <see cref="StartValue"/>.</summary>
    public int Value { get; private set; } = StartValue;

    /// <summary>True after any NAP break toast was queued this session (slice-complete flag).</summary>
    public bool FeedbackEverShown { get; private set; }

    /// <summary>
    /// True if an unprovoked civilian hit/kill applied since <see cref="BeginMissionRun"/>.
    /// Clean smuggle reward skips when true.
    /// </summary>
    public bool CivilianHarmedThisRun { get; private set; }

    /// <summary>Pending toast count (HUD drains via <see cref="TryDequeueToast"/>).</summary>
    public int PendingToastCount => _toastQueue.Count;

    /// <summary>
    /// Fired when a “You broke the NAP” toast is queued (Pulse: <c>ui_nap_break</c>).
    /// </summary>
    public event Action? NapBroken;

    /// <summary>Reset clean-run tracking at smuggle accept / mission start.</summary>
    public void BeginMissionRun() => CivilianHarmedThisRun = false;

    /// <summary>
    /// Apply a raw delta (clamped). Optionally queue a toast message.
    /// Returns the actual change applied after clamp.
    /// </summary>
    public int ApplyDelta(int delta, string? toastMessage = null)
    {
        var before = Value;
        Value = Math.Clamp(Value + delta, MinValue, MaxValue);
        var applied = Value - before;

        if (!string.IsNullOrEmpty(toastMessage) && applied != 0)
        {
            _toastQueue.Enqueue(toastMessage);
            if (delta < 0)
                FeedbackEverShown = true;
            if (toastMessage == BrokeNapToast)
                NapBroken?.Invoke();
        }

        return applied;
    }

    /// <summary>
    /// Combat damage event. Only <see cref="AggressionContext.Unprovoked"/> changes rep
    /// (hit −15 / kill −25) and queues “You broke the NAP”. Defense / StateConflict → 0.
    /// </summary>
    public int ApplyCombat(AggressionContext context, bool kill)
    {
        if (context != AggressionContext.Unprovoked)
            return 0;

        CivilianHarmedThisRun = true;
        var delta = kill ? KillCivDelta : HitCivDelta;
        return ApplyDelta(delta, BrokeNapToast);
    }

    /// <summary>
    /// Optional N-04: +5 when smuggle completes without civilian harm this run.
    /// Returns applied delta (0 if harmed or already at cap).
    /// </summary>
    public int TryRewardCleanSmuggle()
    {
        if (CivilianHarmedThisRun)
            return 0;

        return ApplyDelta(SmuggleCleanDelta);
    }

    /// <summary>Debug/smoke: set value directly (clamped).</summary>
    public void SetValueForDebug(int value) =>
        Value = Math.Clamp(value, MinValue, MaxValue);

    /// <summary>
    /// VS-09: NAP → start, clear toast queue, harm flag, and <see cref="FeedbackEverShown"/>.
    /// </summary>
    public void ResetForDebug()
    {
        Value = StartValue;
        CivilianHarmedThisRun = false;
        FeedbackEverShown = false;
        _toastQueue.Clear();
    }

    /// <summary>Drain one toast for HUD flash. Returns false if queue empty.</summary>
    public bool TryDequeueToast(out string message)
    {
        if (_toastQueue.Count == 0)
        {
            message = string.Empty;
            return false;
        }

        message = _toastQueue.Dequeue();
        return true;
    }
}
