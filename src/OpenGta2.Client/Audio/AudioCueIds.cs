namespace OpenGta2.Client.Audio;

/// <summary>
/// Stable Pulse cue IDs from docs/audio/CUE-SHEET-V0.md (active thin pack).
/// Do not invent parallel names — play by these ids only.
/// </summary>
public static class AudioCueIds
{
    /// <summary>Soft tick when wanted heat rises (Raise succeeds).</summary>
    public const string UiWantedTick = "ui_wanted_tick";

    /// <summary>First alert bump / chase engage.</summary>
    public const string StingerWantedAlert = "stinger_wanted_alert";

    /// <summary>Soft tick under Frame’s “You broke the NAP” toast.</summary>
    public const string UiNapBreak = "ui_nap_break";
}
