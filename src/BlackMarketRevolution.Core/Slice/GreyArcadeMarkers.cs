namespace BlackMarketRevolution.Slice;

/// <summary>
/// Atlas FROZEN Grey Arcade landmarks from docs/world/NEON-MARKET-SLICE.md.
/// Positions are authoritative Atlas parent-map pixels (4000×4000). OpenGta2 uses
/// GTA2 block units (~256×256); convert with <see cref="AtlasToWorldScale"/>.
/// </summary>
public enum GreyArcadeMarkerId
{
    Contact,
    Pickup,
    Drop,
    Property
}

/// <summary>Atlas design-space point (pixels on parent 4000×4000 map).</summary>
public readonly record struct AtlasPoint(float X, float Y);

/// <summary>
/// Static Grey Arcade clip + interact markers. Street trader omitted (optional).
/// </summary>
public static class GreyArcadeMarkers
{
    /// <summary>Default interact radius in Atlas px (NEON-MARKET-SLICE).</summary>
    public const float InteractRadiusAtlasPx = 48f;

    /// <summary>Atlas parent map size (px).</summary>
    public const float AtlasMapSizePx = 4000f;

    /// <summary>
    /// OpenGta2 / GTA2 map size in block units. bil and most GTA2 maps are 256×256.
    /// If Windows playtest shows markers misaligned vs Atlas art, adjust this constant
    /// (or add an offset) and re-verify with the world-interact debug overlay.
    /// </summary>
    public const float WorldMapBlocks = 256f;

    /// <summary>
    /// Atlas px → OpenGta2 world blocks: <c>world = atlas * (256/4000)</c> = 0.064.
    /// Radius 48 Atlas px ≈ 3.072 world units.
    /// </summary>
    public const float AtlasToWorldScale = WorldMapBlocks / AtlasMapSizePx;

    public static readonly AtlasPoint ClipMin = new(0f, 2000f);
    public static readonly AtlasPoint ClipMax = new(2000f, 4000f);

    public static readonly AtlasPoint SpawnPlayer = new(480f, 2920f);

    /// <summary>MARKER_CONTACT — accept Smuggle-01.</summary>
    public static readonly AtlasPoint Contact = new(760f, 2840f);

    /// <summary>MARKER_PICKUP — Wharf Pickup.</summary>
    public static readonly AtlasPoint Pickup = new(960f, 3640f);

    /// <summary>MARKER_DROP — Alley Drop.</summary>
    public static readonly AtlasPoint Drop = new(1480f, 3360f);

    /// <summary>Underground Stack buy marker.</summary>
    public static readonly AtlasPoint Property = new(880f, 3180f);

    public static float InteractRadiusWorld => InteractRadiusAtlasPx * AtlasToWorldScale;

    public static float AtlasToWorld(float atlasPx) => atlasPx * AtlasToWorldScale;

    public static float WorldToAtlas(float worldBlocks) => worldBlocks / AtlasToWorldScale;

    public static (float X, float Y) ToWorld(AtlasPoint atlas) =>
        (AtlasToWorld(atlas.X), AtlasToWorld(atlas.Y));

    public static IReadOnlyList<GreyArcadeMarkerDef> All { get; } =
    [
        new(GreyArcadeMarkerId.Contact, Contact, "CONTACT", "Accept Smuggle-01"),
        new(GreyArcadeMarkerId.Pickup, Pickup, "PICKUP", "Pick up package"),
        new(GreyArcadeMarkerId.Drop, Drop, "DROP", "Deliver package"),
        new(GreyArcadeMarkerId.Property, Property, "PROPERTY", "Buy Underground Stack")
    ];
}

/// <summary>One interactable landmark (Atlas contract).</summary>
public readonly record struct GreyArcadeMarkerDef(
    GreyArcadeMarkerId Id,
    AtlasPoint Atlas,
    string ShortLabel,
    string Prompt);
