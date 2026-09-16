using System;
using BlackMarketRevolution.Slice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenGta2.Client.Diagnostics;
using OpenGta2.Client.Utilities;

namespace OpenGta2.Client.Components;

/// <summary>
/// VS-09: debug reset hotkeys + slice-complete banner.
/// F1/F2 are taken (diagnostics / slow-move); use F12 keep-property, Delete clear-property.
/// See docs/GREY-MARKET-SLICE.md §12.
/// </summary>
public sealed class SliceDebugHudComponent : BaseDrawableComponent
{
    private const float LogicalWidth = 1280f;
    private const float LogicalHeight = 720f;
    private const float BannerFlashSeconds = 3f;

    private readonly Controls _controls;
    private readonly SliceDebugController _debug;

    private SpriteBatch? _spriteBatch;
    private SpriteFont? _font;
    private Texture2D? _pixel;

    private float _resetFlashSeconds;
    private string? _resetFlashText;

    public SliceDebugHudComponent(
        GtaGame game,
        Controls controls,
        SliceDebugController debug) : base(game)
    {
        _controls = controls;
        _debug = debug;
    }

    public override void Initialize()
    {
        DrawOrder = 905;
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

        // F12 — reset wanted/NAP/crypto/mission; keep property ownership.
        if (_controls.IsKeyDown(Keys.F12))
        {
            _debug.ResetKeepProperty();
            _resetFlashSeconds = BannerFlashSeconds;
            _resetFlashText = "RESET (keep property)";
            DiagnosticValues.Set("slice.reset", "keep property");
            Console.WriteLine(
                "[VS-09] ResetKeepProperty — wanted/NAP/crypto start, mission cleared, property kept; " +
                $"sliceComplete={_debug.SliceComplete}");
        }

        // Delete — same reset plus clear property ownership.
        if (_controls.IsKeyDown(Keys.Delete))
        {
            _debug.ResetClearProperty();
            _resetFlashSeconds = BannerFlashSeconds;
            _resetFlashText = "RESET (clear property)";
            DiagnosticValues.Set("slice.reset", "clear property");
            Console.WriteLine(
                "[VS-09] ResetClearProperty — wanted/NAP/crypto start, mission cleared, property cleared; " +
                $"sliceComplete={_debug.SliceComplete}");
        }

        if (_resetFlashSeconds > 0f)
            _resetFlashSeconds = Math.Max(0f, _resetFlashSeconds - dt);

        DiagnosticValues.Set(
            "slice.complete",
            _debug.SliceComplete ? "YES" : "no");
    }

    public override void Draw(GameTime gameTime)
    {
        if (_spriteBatch == null || _font == null || _pixel == null)
            return;

        var vp = GraphicsDevice.Viewport;
        var sx = vp.Width / LogicalWidth;
        var sy = vp.Height / LogicalHeight;

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        // Slice-complete banner — top-center (above mission BC, clear of TR/TL).
        if (_debug.SliceComplete)
        {
            const string banner = "SLICE COMPLETE";
            var size = _font.MeasureString(banner);
            var y = 100f * sy;
            var panelPadX = 16f * sx;
            var panelW = size.X + panelPadX * 2f;
            var panelX = (vp.Width - panelW) * 0.5f;
            var panelRect = new Rectangle(
                (int)panelX,
                (int)(y - 4f * sy),
                (int)panelW,
                (int)(size.Y + 8f * sy));
            var textPos = new Vector2((vp.Width - size.X) * 0.5f, y);
            _spriteBatch.Draw(_pixel, panelRect, new Color(0, 40, 30, 200));
            _spriteBatch.DrawString(_font, banner, textPos, new Color(0x2E, 0xE6, 0xD6));
        }

        // Brief reset flash under banner area.
        if (_resetFlashSeconds > 0f && _resetFlashText != null)
        {
            var flash = _resetFlashText;
            var size = _font.MeasureString(flash);
            var y = (_debug.SliceComplete ? 132f : 100f) * sy;
            var panelPadX = 12f * sx;
            var panelW = size.X + panelPadX * 2f;
            var panelX = (vp.Width - panelW) * 0.5f;
            var panelRect = new Rectangle(
                (int)panelX,
                (int)(y - 4f * sy),
                (int)panelW,
                (int)(size.Y + 8f * sy));
            var textPos = new Vector2((vp.Width - size.X) * 0.5f, y);
            _spriteBatch.Draw(_pixel, panelRect, new Color(0, 0, 0, 180));
            _spriteBatch.DrawString(_font, flash, textPos, new Color(0xE6, 0x6A, 0x2E));
        }

        _spriteBatch.End();

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
    }
}
