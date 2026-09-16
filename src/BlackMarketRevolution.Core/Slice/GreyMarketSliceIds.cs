namespace BlackMarketRevolution.Slice;

/// <summary>
/// Stable ids for the Week 1 grey-market vertical slice (see docs/GREY-MARKET-SLICE.md).
/// Runtime wiring comes later; this assembly is the BMR hook layer on top of OpenGta2.
/// </summary>
public static class GreyMarketSliceIds
{
    public const string DistrictId = "grey-arcade";
    public const string MissionDeliver = "mission.deliver.v1";
    public const string MissionSmuggle = "mission.smuggle.v1";
    public const string PropertyMutualAid = "property.mutual-aid.v1";
    public const string PropertyIllegal = "property.illegal.v1";
}
