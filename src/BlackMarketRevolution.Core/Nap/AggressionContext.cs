namespace BlackMarketRevolution.Nap;

/// <summary>
/// Combat initiator tag for NAP (VS-08). Penalty applies only to <see cref="Unprovoked"/>.
/// See docs/GREY-MARKET-SLICE.md §8 and docs/BALANCE-GREY-ARCADE.md N-01…N-05.
/// </summary>
public enum AggressionContext
{
    /// <summary>Attack on civilian / neutral — NAP penalty applies.</summary>
    Unprovoked,

    /// <summary>Self-defense (e.g. raiders / assassins) — no NAP delta.</summary>
    Defense,

    /// <summary>Fighting police / State while wanted — no NAP delta.</summary>
    StateConflict
}
