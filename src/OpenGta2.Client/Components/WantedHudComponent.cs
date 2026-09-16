using System;
using System.Text;
using BlackMarketRevolution.Economy;
using BlackMarketRevolution.Missions;
using BlackMarketRevolution.Wanted;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenGta2.Client.Diagnostics;
using OpenGta2.Client.Utilities;

namespace OpenGta2.Client.Components;

/// <summary>
/// VS-07: top-right <c>hud_wanted</c> heat pips (0–3) per HUD-WIREFRAME-SLICE.md,
/// plus chase/arrest smoke hotkeys. NAP is VS-08 — not drawn here.
/// </summary>
public sealed class WantedHudComponent : BaseDrawableComponent
{
    private const float LogicalWidth = 1280f;
    private const float LogicalHeight = 720f;
    private const float Inset = 12f;
    private const float ArrestFlashSeconds = 2.5f;

    private readonly Controls _controls;
    private readonly PlayerWallet _wallet;
    private readonly WantedMeter _wanted;
    private readonly SmuggleMission _mission;

    private SpriteBatch? _spriteBatch;
    private SpriteFont? _font;
    private Texture2D? _pixel;

    private float _arrestFlashSeconds;
    private float _chaseFlashSeconds;
    private string? _flashText;

    public WantedHudComponent(
        GtaGame game,
        Controls controls,
        PlayerWallet wallet,
        WantedMeter wanted,
        SmuggleMission mission) : base(game)
    {
        _controls = controls;
        _wallet = wallet;
        _wanted = wanted;
        _mission = mission;
    }

    public override void Initialize()
    {
        DrawOrder = 903;
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

        // F7 — chase/engage smoke when wanted ≥ 1. Second press ends chase (escape).
        if (_controls.IsKeyDown(Keys.F7))
        {
            if (_wanted.ChaseEngaged)
            {
                _wanted.EndChase();
                _chaseFlashSeconds = ArrestFlashSeconds;
                _flashText = "Chase ended - out of LOS";
                DiagnosticValues.Set("wanted.chase", "ended");
                Console.WriteLine("[VS-07] Chase ended — decay can run (45s/star out of LOS)");
            }
            else if (_wanted.TryEngageChase())
            {
                _chaseFlashSeconds = ArrestFlashSeconds;
                _flashText = "Police chase engaged";
                DiagnosticValues.Set("wanted.chase", "engaged");
                Console.WriteLine($"[VS-07] Chase engaged at wanted {_wanted.Level}");
            }
            else
            {
                DiagnosticValues.Set("wanted.chase", "need wanted>=1");
                Console.WriteLine("[VS-07] Chase refused — raise wanted first (F5 / raid / F4)");
            }
        }

        // F8 — arrest stub: hub fade as flash/log, crypto x0.5, wanted 0, property kept.
        if (_controls.IsKeyDown(Keys.F8))
        {
            var before = _wallet.Crypto;
            var wasCarrying = _mission.NotifyDeathOrArrest();
            _wanted.Arrest(_wallet);
            _arrestFlashSeconds = ArrestFlashSeconds;
            _flashText = wasCarrying
                ? $"ARRESTED - cargo lost, crypto {before}->{_wallet.Crypto}"
                : $"ARRESTED - hub, crypto {before}->{_wallet.Crypto}";
            DiagnosticValues.Set(
                "wanted.arrest",
                $"crypto {before}->{_wallet.Crypto}; property kept");
            Console.WriteLine(
                $"[VS-07] Arrest — crypto {before} → {_wallet.Crypto} (x{WantedMeter.ArrestCryptoFactor}), " +
                $"wanted=0, property kept" +
                (wasCarrying ? ", smuggle cargo forfeited" : ""));
        }

        _wanted.Tick(dt);

        if (_wanted.ConsumeArrestFlash() && _arrestFlashSeconds <= 0f)
            _arrestFlashSeconds = ArrestFlashSeconds;

        if (_arrestFlashSeconds > 0f)
            _arrestFlashSeconds = Math.Max(0f, _arrestFlashSeconds - dt);
        if (_chaseFlashSeconds > 0f)
            _chaseFlashSeconds = Math.Max(0f, _chaseFlashSeconds - dt);

        DiagnosticValues.Set(
            "wanted",
            $"{_wanted.Level}/{WantedMeter.MaxLevel}" +
            (_wanted.ChaseEngaged ? " CHASE" : "") +
            (_wanted.InLos ? " LOS" : $" decay {_wanted.OutOfLosElapsedSeconds:0.0}s"));
    }

    public override void Draw(GameTime gameTime)
    {
        if (_spriteBatch == null || _font == null || _pixel == null)
            return;

        var vp = GraphicsDevice.Viewport;
        var sx = vp.Width / LogicalWidth;
        var sy = vp.Height / LogicalHeight;

        // hud_wanted — TR: right=12, y=12 (~160×28)
        const float panelW = 160f;
        const float panelH = 28f;
        var panelX = LogicalWidth - Inset - panelW;
        var panelRect = new Rectangle(
            (int)(panelX * sx),
            (int)(Inset * sy),
            (int)(panelW * sx),
            (int)(panelH * sy));

        var pips = FormatPips(_wanted.Level, WantedMeter.MaxLevel);
        var text = _wanted.ChaseEngaged ? $"Wanted {pips} !" : $"Wanted {pips}";
        var color = _wanted.Level >= 3
            ? new Color(0xE6, 0x2E, 0x2E)
            : (_wanted.Level >= 1
                ? new Color(0xE6, 0x6A, 0x2E)
                : Color.Gray);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _spriteBatch.Draw(_pixel, panelRect, new Color(0, 0, 0, 160));
        _spriteBatch.DrawString(_font, text, Scale(panelX + 4f, Inset + 4f, sx, sy), color);

        // Optional chase/arrest flash under TR cluster (still clear of center).
        if ((_arrestFlashSeconds > 0f || _chaseFlashSeconds > 0f) && _flashText != null)
        {
            var flash = _flashText.Replace('·', '-');
            var flashPos = Scale(panelX + 4f, Inset + panelH + 4f, sx, sy);
            var flashColor = _arrestFlashSeconds > 0f
                ? new Color(0xE6, 0x2E, 0x2E)
                : new Color(0xE6, 0x6A, 0x2E);
            var flashSize = _font.MeasureString(flash);
            var flashPanel = new Rectangle(
                (int)flashPos.X - (int)(4 * sx),
                (int)flashPos.Y - (int)(2 * sy),
                (int)(flashSize.X + 8 * sx),
                (int)(flashSize.Y + 4 * sy));
            _spriteBatch.Draw(_pixel, flashPanel, new Color(0, 0, 0, 180));
            _spriteBatch.DrawString(_font, flash, flashPos, flashColor);
        }

        _spriteBatch.End();

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
    }

    /// <summary>ASCII pips for DebugFont (Latin-1): filled '*' / empty '.'.</summary>
    private static string FormatPips(int level, int max)
    {
        var sb = new StringBuilder(max);
        for (var i = 0; i < max; i++)
            sb.Append(i < level ? '*' : '.');
        return sb.ToString();
    }

    private static Vector2 Scale(float x, float y, float sx, float sy) =>
        new(x * sx, y * sy);
}
