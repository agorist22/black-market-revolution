using System;
using BlackMarketRevolution.Economy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenGta2.Client.Diagnostics;
using OpenGta2.Client.Utilities;

namespace OpenGta2.Client.Components;

/// <summary>
/// VS-05: underground property buy + income tick, plus BL <c>hud_property</c>
/// per docs/ui/HUD-WIREFRAME-SLICE.md. Week 1 smoke uses debug hotkeys (no map marker).
/// </summary>
public sealed class PropertyHudComponent : BaseDrawableComponent
{
    private const float LogicalWidth = 1280f;
    private const float LogicalHeight = 720f;
    private const float Inset = 12f;

    private readonly Controls _controls;
    private readonly PlayerWallet _wallet;
    private readonly UndergroundProperty _property;

    private SpriteBatch? _spriteBatch;
    private SpriteFont? _font;
    private Texture2D? _pixel;

    private float _incomeFlashSeconds;
    private int _incomeFlashAmount;
    private float _raidFlashSeconds;

    public PropertyHudComponent(
        GtaGame game,
        Controls controls,
        PlayerWallet wallet,
        UndergroundProperty property) : base(game)
    {
        _controls = controls;
        _wallet = wallet;
        _property = property;
    }

    public override void Initialize()
    {
        DrawOrder = 901;
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
        var dt = gameTime.GetDelta();

        // F3 — smoke grant so buy is reachable from START_CRYPTO=80 (one press → 160).
        if (_controls.IsKeyDown(Keys.F3))
        {
            _wallet.AddCrypto(80);
            DiagnosticValues.Set("property", $"granted +80 crypto (bal {_wallet.Crypto})");
            Console.WriteLine($"[VS-05] Smoke grant +80 crypto → {_wallet.Crypto}");
        }

        // P — attempt underground property purchase.
        if (_controls.IsKeyDown(Keys.P))
        {
            if (_property.TryPurchase(_wallet))
            {
                DiagnosticValues.Set("property", "OWNED");
                Console.WriteLine(
                    $"[VS-05] Purchased underground property for {UndergroundProperty.BuyPrice}. Crypto={_wallet.Crypto}");
            }
            else if (_property.Owned)
            {
                DiagnosticValues.Set("property", "already owned");
            }
            else
            {
                DiagnosticValues.Set("property", $"need {UndergroundProperty.BuyPrice} (have {_wallet.Crypto})");
                Console.WriteLine(
                    $"[VS-05] Buy refused — need {UndergroundProperty.BuyPrice}, have {_wallet.Crypto}");
            }
        }

        // F4 — force raid-risk stub while owned (wanted system is VS-07).
        if (_controls.IsKeyDown(Keys.F4) && _property.ForceRaidForDebug())
        {
            _raidFlashSeconds = 2.5f;
            DiagnosticValues.Set("property.raid", $"proc #{_property.RaidProcCount} (wanted stub)");
            Console.WriteLine(
                $"[VS-05] Raid risk forced (#{_property.RaidProcCount}) — wanted hook deferred to VS-07");
        }

        var paid = _property.Tick(dt, null, out var raidFired);
        if (paid > 0)
        {
            _wallet.AddCrypto(paid);
            _incomeFlashAmount = paid;
            _incomeFlashSeconds = 2f;
            DiagnosticValues.Set(
                "property.income",
                $"+{paid} (tick #{_property.IncomeTickCount}, bal {_wallet.Crypto})");
            Console.WriteLine(
                $"[VS-05] Income +{paid} crypto (tick #{_property.IncomeTickCount}) → {_wallet.Crypto}");
        }

        if (raidFired)
        {
            _raidFlashSeconds = 2.5f;
            DiagnosticValues.Set("property.raid", $"proc #{_property.RaidProcCount} (wanted stub)");
            Console.WriteLine(
                $"[VS-05] Raid risk proc #{_property.RaidProcCount} — wanted hook deferred to VS-07");
        }

        if (_incomeFlashSeconds > 0f)
            _incomeFlashSeconds = Math.Max(0f, _incomeFlashSeconds - dt);
        if (_raidFlashSeconds > 0f)
            _raidFlashSeconds = Math.Max(0f, _raidFlashSeconds - dt);

        DiagnosticValues.Set(
            "property",
            _property.Owned
                ? $"OWNED income#{_property.IncomeTickCount} raid#{_property.RaidProcCount}"
                : $"unowned ({_wallet.Crypto}/{UndergroundProperty.BuyPrice})");
    }

    public override void Draw(GameTime gameTime)
    {
        if (_spriteBatch == null || _font == null || _pixel == null)
            return;

        var vp = GraphicsDevice.Viewport;
        var sx = vp.Width / LogicalWidth;
        var sy = vp.Height / LogicalHeight;

        // hud_property — BL: x=12, bottom≈112 → y = 720 - 112 = 608
        const float propertyBottom = 112f;
        var lineY = LogicalHeight - propertyBottom;
        var panelRect = new Rectangle(
            (int)(Inset * sx),
            (int)((lineY - 4f) * sy),
            (int)(220 * sx),
            (int)(28 * sy));

        var ownedLabel = _property.Owned ? "OWNED" : "-";
        var text = $"Property: {ownedLabel}";
        if (_property.Owned && _incomeFlashSeconds > 0f)
            text = $"Property: OWNED +{_incomeFlashAmount}";
        else if (_property.Owned && _raidFlashSeconds > 0f)
            text = "Property: RAID!";

        var color = _property.Owned
            ? (_raidFlashSeconds > 0f
                ? new Color(0xE6, 0x6A, 0x2E)
                : (_incomeFlashSeconds > 0f
                    ? new Color(0x2E, 0xE6, 0xD6)
                    : Color.White))
            : Color.Gray;

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _spriteBatch.Draw(_pixel, panelRect, new Color(0, 0, 0, 160));
        _spriteBatch.DrawString(_font, text, Scale(Inset, lineY, sx, sy), color);
        _spriteBatch.End();

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
    }

    private static Vector2 Scale(float x, float y, float sx, float sy) =>
        new(x * sx, y * sy);
}
