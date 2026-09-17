using System;
using BlackMarketRevolution.Economy;
using BlackMarketRevolution.Missions;
using BlackMarketRevolution.Slice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenGta2.Client.Diagnostics;
using OpenGta2.Client.Peds;
using OpenGta2.Client.Utilities;

namespace OpenGta2.Client.Components;

/// <summary>
/// Grey Arcade proximity markers (Atlas FROZEN coords). When the player is within
/// interact radius, shows a prompt and accepts E/F to trigger mission/property/trader actions.
/// Existing smoke hotkeys (M/I/O/P, F-keys) remain as debug fallback.
/// </summary>
public sealed class WorldInteractComponent : BaseDrawableComponent
{
    private const float LogicalWidth = 1280f;
    private const float LogicalHeight = 720f;

    private readonly Controls _controls;
    private readonly Camera _camera;
    private readonly PlayerWallet _wallet;
    private readonly UndergroundProperty _property;
    private readonly SmuggleMission _mission;
    private readonly StreetTrade _streetTrade;

    private SpriteBatch? _spriteBatch;
    private SpriteFont? _font;
    private Texture2D? _pixel;

    private GreyArcadeMarkerDef? _nearestInRange;
    private float _nearestDistanceAtlas = float.MaxValue;

    public WorldInteractComponent(
        GtaGame game,
        Controls controls,
        Camera camera,
        PlayerWallet wallet,
        UndergroundProperty property,
        SmuggleMission mission,
        StreetTrade streetTrade) : base(game)
    {
        _controls = controls;
        _camera = camera;
        _wallet = wallet;
        _property = property;
        _mission = mission;
        _streetTrade = streetTrade;
    }

    public override void Initialize()
    {
        DrawOrder = 880;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Game.AssetManager.GetDebugFont();
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    protected override void UnloadContent()
    {
        _pixel?.Dispose();
        _pixel = null;
        _spriteBatch?.Dispose();
        _spriteBatch = null;
        base.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _streetTrade.Tick(dt);

        var ped = _camera.AttachedToPed;
        if (ped == null)
        {
            _nearestInRange = null;
            DiagnosticValues.Set("world.player", "no ped");
            return;
        }

        var playerAtlasX = GreyArcadeMarkers.WorldToAtlas(ped.Position.X);
        var playerAtlasY = GreyArcadeMarkers.WorldToAtlas(ped.Position.Y);

        DiagnosticValues.Set(
            "world.player",
            $"world=({ped.Position.X:0.00},{ped.Position.Y:0.00}) atlas=({playerAtlasX:0},{playerAtlasY:0})");

        GreyArcadeMarkerDef? nearest = null;
        var nearestDist = float.MaxValue;

        foreach (var marker in GreyArcadeMarkers.All)
        {
            var dx = playerAtlasX - marker.Atlas.X;
            var dy = playerAtlasY - marker.Atlas.Y;
            var dist = MathF.Sqrt(dx * dx + dy * dy);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = marker;
            }
        }

        _nearestDistanceAtlas = nearestDist;
        _nearestInRange = nearestDist <= GreyArcadeMarkers.InteractRadiusAtlasPx
            ? nearest
            : null;

        if (nearest != null)
        {
            DiagnosticValues.Set(
                "world.nearest",
                $"{nearest.Value.ShortLabel} dist={nearestDist:0.0}px " +
                (nearestDist <= GreyArcadeMarkers.InteractRadiusAtlasPx ? "IN" : "out"));
        }

        DiagnosticValues.Set(
            "world.trader",
            _streetTrade.IsReady
                ? $"ready trades={_streetTrade.TradeCount}"
                : $"cd {_streetTrade.CooldownRemainingSeconds:0.0}s trades={_streetTrade.TradeCount}");

        var interact =
            _controls.IsKeyDown(Keys.E) ||
            _controls.IsKeyDown(Keys.F);

        if (interact && _nearestInRange != null)
            TryInteract(_nearestInRange.Value);
    }

    private void TryInteract(GreyArcadeMarkerDef marker)
    {
        switch (marker.Id)
        {
            case GreyArcadeMarkerId.Contact:
                if (_mission.TryStart())
                {
                    DiagnosticValues.Set("world.interact", "CONTACT → Smuggle-01 started");
                    Console.WriteLine("[World] CONTACT — Smuggle-01 accepted (go to PICKUP)");
                }
                else
                {
                    DiagnosticValues.Set("world.interact", $"CONTACT refused ({_mission.Phase})");
                    Console.WriteLine($"[World] CONTACT refused — phase={_mission.Phase}");
                }
                break;

            case GreyArcadeMarkerId.Pickup:
                if (_mission.TryPickup())
                {
                    DiagnosticValues.Set("world.interact", "PICKUP → carrying");
                    Console.WriteLine("[World] PICKUP — package acquired (go to DROP)");
                }
                else
                {
                    DiagnosticValues.Set("world.interact", $"PICKUP refused ({_mission.Phase})");
                    Console.WriteLine($"[World] PICKUP refused — phase={_mission.Phase}");
                }
                break;

            case GreyArcadeMarkerId.Drop:
                if (_mission.TryDeliver(_wallet, out var napBonus))
                {
                    DiagnosticValues.Set(
                        "world.interact",
                        $"DROP → +{SmuggleMission.RewardCrypto} crypto" +
                        (napBonus != 0 ? $"; NAP +{napBonus}" : ""));
                    Console.WriteLine(
                        $"[World] DROP — delivered +{SmuggleMission.RewardCrypto} → {_wallet.Crypto}" +
                        (napBonus != 0 ? $"; NAP +{napBonus}" : ""));
                }
                else
                {
                    DiagnosticValues.Set("world.interact", $"DROP refused ({_mission.Phase})");
                    Console.WriteLine($"[World] DROP refused — phase={_mission.Phase}");
                }
                break;

            case GreyArcadeMarkerId.Property:
                if (_property.TryPurchase(_wallet))
                {
                    DiagnosticValues.Set("world.interact", "PROPERTY → OWNED");
                    Console.WriteLine(
                        $"[World] PROPERTY — bought for {UndergroundProperty.BuyPrice}. Crypto={_wallet.Crypto}");
                }
                else if (_property.Owned)
                {
                    DiagnosticValues.Set("world.interact", "PROPERTY already owned");
                    Console.WriteLine("[World] PROPERTY — already owned");
                }
                else
                {
                    DiagnosticValues.Set(
                        "world.interact",
                        $"PROPERTY need {UndergroundProperty.BuyPrice} (have {_wallet.Crypto})");
                    Console.WriteLine(
                        $"[World] PROPERTY buy refused — need {UndergroundProperty.BuyPrice}, have {_wallet.Crypto}");
                }
                break;

            case GreyArcadeMarkerId.Trader:
                if (_streetTrade.TryTrade(_wallet, out var reward))
                {
                    DiagnosticValues.Set(
                        "world.interact",
                        $"TRADER → +{reward} crypto (bal {_wallet.Crypto}; cd {StreetTrade.CooldownSeconds:0}s)");
                    Console.WriteLine(
                        $"[World] TRADER — street trade +{reward} → {_wallet.Crypto} " +
                        $"(cooldown {StreetTrade.CooldownSeconds:0}s)");
                }
                else
                {
                    DiagnosticValues.Set(
                        "world.interact",
                        $"TRADER cooldown {_streetTrade.CooldownRemainingSeconds:0.0}s");
                    Console.WriteLine(
                        $"[World] TRADER refused — cooldown {_streetTrade.CooldownRemainingSeconds:0.0}s");
                }
                break;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        if (_spriteBatch == null || _font == null || _pixel == null)
            return;

        var vp = GraphicsDevice.Viewport;
        var sx = vp.Width / LogicalWidth;
        var sy = vp.Height / LogicalHeight;
        var ped = _camera.AttachedToPed;
        var groundZ = ped?.Position.Z ?? 0f;

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        // Debug marker squares + labels (world → screen via Camera matrices).
        foreach (var marker in GreyArcadeMarkers.All)
        {
            var (wx, wy) = GreyArcadeMarkers.ToWorld(marker.Atlas);
            var world = new Vector3(wx, wy, groundZ);
            var screen = vp.Project(world, _camera.Projection, _camera.ViewMatrix, Matrix.Identity);

            // Behind camera or failed project → skip square (diagnostics still show distance).
            if (screen.Z < 0f || screen.Z > 1f)
                continue;

            var inRange = _nearestInRange?.Id == marker.Id;
            var color = MarkerColor(marker.Id, inRange);
            const int size = 10;
            var rect = new Rectangle((int)screen.X - size / 2, (int)screen.Y - size / 2, size, size);
            _spriteBatch.Draw(_pixel, rect, color);

            var label = marker.ShortLabel;
            var labelPos = new Vector2(screen.X + size, screen.Y - size);
            _spriteBatch.DrawString(_font, label, labelPos, color);
        }

        // Proximity prompt.
        if (_nearestInRange != null)
        {
            var m = _nearestInRange.Value;
            var prompt = FormatPrompt(m);
            var textSize = _font.MeasureString(prompt);
            var y = (LogicalHeight - 120f) * sy;
            var panelPadX = 12f * sx;
            var panelW = textSize.X + panelPadX * 2f;
            var panelX = (vp.Width - panelW) * 0.5f;
            var panelRect = new Rectangle(
                (int)panelX,
                (int)(y - 4f * sy),
                (int)panelW,
                (int)(textSize.Y + 8f * sy));
            var textPos = new Vector2((vp.Width - textSize.X) * 0.5f, y);
            _spriteBatch.Draw(_pixel, panelRect, new Color(0, 0, 0, 180));
            _spriteBatch.DrawString(_font, prompt, textPos, new Color(0x2E, 0xE6, 0xD6));
        }
        else if (_nearestDistanceAtlas < float.MaxValue)
        {
            // Compact nearest-distance hint when out of range (helps Windows coord verify).
            var hint = $"nearest {_nearestDistanceAtlas:0}px";
            var textSize = _font.MeasureString(hint);
            var pos = new Vector2(12f * sx, (LogicalHeight - 40f) * sy);
            _spriteBatch.DrawString(_font, hint, pos, Color.Gray * 0.8f);
            _ = textSize;
        }

        _spriteBatch.End();
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
    }

    private string FormatPrompt(GreyArcadeMarkerDef marker)
    {
        if (marker.Id == GreyArcadeMarkerId.Trader && !_streetTrade.IsReady)
            return $"[E/F] Trader cooldown {_streetTrade.CooldownRemainingSeconds:0}s";

        return $"[E/F] {marker.Prompt}";
    }

    private static Color MarkerColor(GreyArcadeMarkerId id, bool inRange)
    {
        var baseColor = id switch
        {
            GreyArcadeMarkerId.Contact => new Color(0x2E, 0xE6, 0xD6),
            GreyArcadeMarkerId.Pickup => new Color(0xE6, 0xC8, 0x2E),
            GreyArcadeMarkerId.Drop => new Color(0x2E, 0xA0, 0xE6),
            GreyArcadeMarkerId.Property => new Color(0xE6, 0x6A, 0x2E),
            GreyArcadeMarkerId.Trader => new Color(0x6A, 0xE6, 0x2E),
            _ => Color.White
        };
        return inRange ? Color.White : baseColor;
    }
}
