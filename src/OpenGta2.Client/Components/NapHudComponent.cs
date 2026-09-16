using System;
using BlackMarketRevolution.Nap;
using BlackMarketRevolution.Wanted;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenGta2.Client.Diagnostics;
using OpenGta2.Client.Utilities;

namespace OpenGta2.Client.Components;

/// <summary>
/// VS-08: top-right <c>hud_nap_rep</c> (under wanted) per HUD-WIREFRAME-SLICE.md,
/// broke-NAP toast flash, and smoke hotkeys for unprovoked / StateConflict.
/// </summary>
public sealed class NapHudComponent : BaseDrawableComponent
{
    private const float LogicalWidth = 1280f;
    private const float LogicalHeight = 720f;
    private const float Inset = 12f;
    private const float ToastFlashSeconds = 2.5f;

    private readonly Controls _controls;
    private readonly NapReputation _nap;
    private readonly WantedMeter _wanted;

    private SpriteBatch? _spriteBatch;
    private SpriteFont? _font;
    private Texture2D? _pixel;

    private float _toastSeconds;
    private string? _toastText;
    private Color _toastColor = Color.White;

    public NapHudComponent(
        GtaGame game,
        Controls controls,
        NapReputation nap,
        WantedMeter wanted) : base(game)
    {
        _controls = controls;
        _nap = nap;
        _wanted = wanted;
    }

    public override void Initialize()
    {
        DrawOrder = 904;
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

        // F9 — unprovoked civilian hit → −15 + “You broke the NAP” toast.
        if (_controls.IsKeyDown(Keys.F9))
        {
            var before = _nap.Value;
            var applied = _nap.ApplyCombat(AggressionContext.Unprovoked, kill: false);
            DiagnosticValues.Set("nap", $"{_nap.Value}/{NapReputation.MaxValue} hit {applied}");
            Console.WriteLine(
                $"[VS-08] Unprovoked civ hit — NAP {before} → {_nap.Value} (delta {applied}, ctx Unprovoked)");
        }

        // F10 — unprovoked civilian kill → −25 + toast.
        if (_controls.IsKeyDown(Keys.F10))
        {
            var before = _nap.Value;
            var applied = _nap.ApplyCombat(AggressionContext.Unprovoked, kill: true);
            DiagnosticValues.Set("nap", $"{_nap.Value}/{NapReputation.MaxValue} kill {applied}");
            Console.WriteLine(
                $"[VS-08] Unprovoked civ kill — NAP {before} → {_nap.Value} (delta {applied}, ctx Unprovoked)");
        }

        // F11 — fight police while wanted (StateConflict) → 0 NAP delta.
        if (_controls.IsKeyDown(Keys.F11))
        {
            if (_wanted.Level < 1)
            {
                DiagnosticValues.Set("nap.police", "need wanted>=1");
                Console.WriteLine("[VS-08] Police fight ignored — raise wanted first (F5 / F4)");
            }
            else
            {
                var before = _nap.Value;
                var applied = _nap.ApplyCombat(AggressionContext.StateConflict, kill: false);
                DiagnosticValues.Set(
                    "nap.police",
                    $"StateConflict delta={applied} nap={_nap.Value} wanted={_wanted.Level}");
                Console.WriteLine(
                    $"[VS-08] Fight police (wanted {_wanted.Level}, StateConflict) — NAP {before} → {_nap.Value} (delta {applied})");
            }
        }

        // Drain toast queue into flash (broke NAP / future cues).
        if (_toastSeconds <= 0f && _nap.TryDequeueToast(out var msg))
        {
            _toastText = msg;
            _toastSeconds = ToastFlashSeconds;
            _toastColor = msg == NapReputation.BrokeNapToast
                ? new Color(0xE6, 0x2E, 0x2E)
                : new Color(0x2E, 0xE6, 0xD6);
        }

        if (_toastSeconds > 0f)
            _toastSeconds = Math.Max(0f, _toastSeconds - dt);

        DiagnosticValues.Set(
            "nap",
            $"{_nap.Value}/{NapReputation.MaxValue}" +
            (_nap.CivilianHarmedThisRun ? " harm" : "") +
            (_nap.FeedbackEverShown ? " cue" : ""));
    }

    public override void Draw(GameTime gameTime)
    {
        if (_spriteBatch == null || _font == null || _pixel == null)
            return;

        var vp = GraphicsDevice.Viewport;
        var sx = vp.Width / LogicalWidth;
        var sy = vp.Height / LogicalHeight;

        // hud_nap_rep — TR: right=12, y=44 (~160×28) under hud_wanted
        const float panelW = 160f;
        const float panelH = 28f;
        const float panelY = 44f;
        var panelX = LogicalWidth - Inset - panelW;
        var panelRect = new Rectangle(
            (int)(panelX * sx),
            (int)(panelY * sy),
            (int)(panelW * sx),
            (int)(panelH * sy));

        var text = $"NAP {_nap.Value}";
        var color = _nap.Value <= 25
            ? new Color(0xE6, 0x2E, 0x2E)
            : (_nap.Value >= 75
                ? new Color(0x2E, 0xE6, 0xD6)
                : Color.White);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _spriteBatch.Draw(_pixel, panelRect, new Color(0, 0, 0, 160));
        _spriteBatch.DrawString(_font, text, Scale(panelX + 4f, panelY + 4f, sx, sy), color);

        // Broke-NAP toast flash under TR cluster (still clear of center).
        if (_toastSeconds > 0f && _toastText != null)
        {
            var flash = _toastText.Replace('·', '-');
            var flashPos = Scale(panelX + 4f, panelY + panelH + 4f, sx, sy);
            var flashSize = _font.MeasureString(flash);
            var flashPanel = new Rectangle(
                (int)flashPos.X - (int)(4 * sx),
                (int)flashPos.Y - (int)(2 * sy),
                (int)(flashSize.X + 8 * sx),
                (int)(flashSize.Y + 4 * sy));
            _spriteBatch.Draw(_pixel, flashPanel, new Color(0, 0, 0, 180));
            _spriteBatch.DrawString(_font, flash, flashPos, _toastColor);
        }

        _spriteBatch.End();

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
    }

    private static Vector2 Scale(float x, float y, float sx, float sy) =>
        new(x * sx, y * sy);
}
