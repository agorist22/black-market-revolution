using System;
using BlackMarketRevolution.Economy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenGta2.Client.Components;

/// <summary>
/// VS-04 top-left HUD cluster: hud_health, hud_crypto, hud_fiat per HUD-WIREFRAME-SLICE.md.
/// Layout is authored at 1280×720 with 12px inset, then scaled to the viewport.
/// </summary>
public sealed class WalletHudComponent : BaseDrawableComponent
{
    private const float LogicalWidth = 1280f;
    private const float LogicalHeight = 720f;
    private const float Inset = 12f;

    // Placeholder vitals until combat slice wires real HP (BALANCE START_HEALTH).
    private const int StartHealth = 100;
    private const int MaxHealth = 100;

    private readonly PlayerWallet _wallet;
    private SpriteBatch? _spriteBatch;
    private SpriteFont? _font;
    private Texture2D? _pixel;

    public WalletHudComponent(GtaGame game, PlayerWallet wallet) : base(game)
    {
        _wallet = wallet;
    }

    public override void Initialize()
    {
        DrawOrder = 900;
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

    public override void Draw(GameTime gameTime)
    {
        if (_spriteBatch == null || _font == null || _pixel == null)
            return;

        var vp = GraphicsDevice.Viewport;
        var sx = vp.Width / LogicalWidth;
        var sy = vp.Height / LogicalHeight;

        // Semi-transparent TL panel (~70% fill max per wireframe).
        var panelRect = new Rectangle(
            (int)(Inset * sx),
            (int)(Inset * sy),
            (int)(220 * sx),
            (int)(92 * sy));

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        _spriteBatch.Draw(_pixel, panelRect, new Color(0, 0, 0, 160));

        // hud_health — TL: x=12, y=12
        var healthPos = Scale(Inset, Inset, sx, sy);
        var healthText = $"Health {StartHealth}/{MaxHealth}";
        _spriteBatch.DrawString(_font, healthText, healthPos, Color.White);

        var barX = healthPos.X;
        var barY = healthPos.Y + _font.LineSpacing + 2 * sy;
        var barW = 180 * sx;
        var barH = Math.Max(4f, 6 * sy);
        _spriteBatch.Draw(_pixel, new Rectangle((int)barX, (int)barY, (int)barW, (int)barH), new Color(40, 40, 40, 200));
        var fill = Math.Clamp(StartHealth / (float)MaxHealth, 0f, 1f);
        _spriteBatch.Draw(_pixel, new Rectangle((int)barX, (int)barY, (int)(barW * fill), (int)barH), new Color(80, 200, 80, 220));

        // hud_crypto — TL: x=12, y=44 (crypto teal accent)
        var cryptoPos = Scale(Inset, 44f, sx, sy);
        var cryptoText = $"Crypto {_wallet.Crypto}";
        _spriteBatch.DrawString(_font, cryptoText, cryptoPos, new Color(0x2E, 0xE6, 0xD6));

        // hud_fiat — TL: x=12, y=76; grey disabled (ASCII "-" — DebugFont is Latin-1 range)
        var fiatPos = Scale(Inset, 76f, sx, sy);
        var fiatText = _wallet.FiatDisplayEnabled ? "Fiat 0" : "Fiat -";
        _spriteBatch.DrawString(_font, fiatText, fiatPos, Color.Gray);

        _spriteBatch.End();

        // Restore 3D defaults after 2D batch (matches DebuggingDrawingComponent).
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
    }

    private static Vector2 Scale(float x, float y, float sx, float sy) =>
        new(x * sx, y * sy);
}
