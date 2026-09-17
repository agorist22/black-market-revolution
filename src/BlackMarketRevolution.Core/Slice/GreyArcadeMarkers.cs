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
    Property,
    Trader
}

/// <summary>Atlas design-space point (pixels on parent 4000×4000 map).</summary>
public readonly record struct AtlasPoint(float X, float Y);

/// <summary>
/// Static Grey Arcade clip + interact markers (smuggle, property buy, street trader).
/// </summary>
public static class GreyArcadeMarkers
{
    /// <summary>Default interact radius in Atlas px (raised to 120 for on-foot smoke; pads frozen from #46).</summary>
    public const float InteractRadiusAtlasPx = 120f;

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
    /// Radius 120 Atlas px ≈ 7.68 world units.
    /// </summary>
    public const float AtlasToWorldScale = WorldMapBlocks / AtlasMapSizePx;

    public static readonly AtlasPoint ClipMin = new(0f, 2000f);
    public static readonly AtlasPoint ClipMax = new(2000f, 4000f);

    public static readonly AtlasPoint SpawnPlayer = new(480f, 2920f);

    /// <summary>MARKER_CONTACT — accept Smuggle-01.</summary>
    public static readonly AtlasPoint Contact = new(720f, 2900f); // open Plaza floor (was board center)

    /// <summary>MARKER_PICKUP — Wharf Pickup.</summary>
    public static readonly AtlasPoint Pickup = new(1000f, 3520f); // wharf approach road (was bay center)

    /// <summary>MARKER_DROP — Alley Drop.</summary>
    public static readonly AtlasPoint Drop = new(1340f, 3280f); // Neon Alley roadbed (was drop bay center)

    /// <summary>Underground Stack buy marker.</summary>
    public static readonly AtlasPoint Property = new(920f, 3280f); // loading door / south pad (was building center)

    /// <summary>MARKER_TRADER / SPAWN_TRADER — street earn (+25–40, 30s CD).</summary>
    public static readonly AtlasPoint Trader = new(320f, 2840f); // south of booth toward Plaza (was booth center)

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
        new(GreyArcadeMarkerId.Property, Property, "PROPERTY", "Buy Underground Stack"),
        new(GreyArcadeMarkerId.Trader, Trader, "TRADER", "Street trade (+25–40)")
    ];
}

/// <summary>One interactable landmark (Atlas contract).</summary>
public readonly record struct GreyArcadeMarkerDef(
    GreyArcadeMarkerId Id,
    AtlasPoint Atlas,
    string ShortLabel,
    string Prompt);
