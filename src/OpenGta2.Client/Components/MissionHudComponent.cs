using System;
using BlackMarketRevolution.Economy;
using BlackMarketRevolution.Missions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenGta2.Client.Diagnostics;
using OpenGta2.Client.Utilities;

namespace OpenGta2.Client.Components;

/// <summary>
/// VS-06: Smuggle-01 accept/pickup/deliver + bottom-center <c>hud_mission</c>
/// per docs/ui/HUD-WIREFRAME-SLICE.md. Week 1 smoke uses debug hotkeys (no map markers).
/// </summary>
public sealed class MissionHudComponent : BaseDrawableComponent
{
    private const float LogicalWidth = 1280f;
    private const float LogicalHeight = 720f;
    private const float MissionBottom = 72f;
    private const float DeliveredFlashSeconds = 2.5f;

    private readonly Controls _controls;
    private readonly PlayerWallet _wallet;
    private readonly SmuggleMission _mission;

    private SpriteBatch? _spriteBatch;
    private SpriteFont? _font;
    private Texture2D? _pixel;

    private float _deliveredFlashSeconds;
    private string? _failFlashText;
    private float _failFlashSeconds;

    public MissionHudComponent(
        GtaGame game,
        Controls controls,
        PlayerWallet wallet,
        SmuggleMission mission) : base(game)
    {
        _controls = controls;
        _wallet = wallet;
        _mission = mission;
    }

    public override void Initialize()
    {
        DrawOrder = 902;
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

        // M — accept / start Smuggle-01 when available.
        if (_controls.IsKeyDown(Keys.M))
        {
            if (_mission.TryStart())
            {
                DiagnosticValues.Set("mission", "awaiting pickup");
                Console.WriteLine("[VS-06] Smuggle-01 started — go to Wharf Pickup (press I)");
            }
            else
            {
                DiagnosticValues.Set("mission", $"start refused ({_mission.Phase})");
                Console.WriteLine($"[VS-06] Start refused — phase={_mission.Phase}");
            }
        }

        // I — pickup package (proximity stub).
        if (_controls.IsKeyDown(Keys.I))
        {
            if (_mission.TryPickup())
            {
                DiagnosticValues.Set("mission", "carrying");
                Console.WriteLine(
                    $"[VS-06] Package picked up — deliver within {SmuggleMission.TimeLimitSeconds:0}s (press O)");
            }
            else
            {
                Console.WriteLine($"[VS-06] Pickup refused — phase={_mission.Phase}");
            }
        }

        // O — deliver / drop (proximity stub).
        if (_controls.IsKeyDown(Keys.O))
        {
            if (_mission.TryDeliver(_wallet))
            {
                _deliveredFlashSeconds = DeliveredFlashSeconds;
                _failFlashSeconds = 0f;
                DiagnosticValues.Set(
                    "mission",
                    $"delivered +{SmuggleMission.RewardCrypto} (bal {_wallet.Crypto})");
                Console.WriteLine(
                    $"[VS-06] Delivered — +{SmuggleMission.RewardCrypto} crypto → {_wallet.Crypto}");
            }
            else
            {
                Console.WriteLine($"[VS-06] Deliver refused — phase={_mission.Phase}");
            }
        }

        // F5 — wanted +1 stub (police LOS). Fails run if wanted hits 3 while carrying.
        if (_controls.IsKeyDown(Keys.F5))
        {
            var failed = _mission.NotifyPoliceLos();
            DiagnosticValues.Set("mission.wanted", _mission.WantedLevel.ToString());
            Console.WriteLine(
                failed
                    ? $"[VS-06] Wanted {_mission.WantedLevel} — heat cooked the run (cooldown {SmuggleMission.RetryDelaySeconds:0}s)"
                    : $"[VS-06] Wanted stub → {_mission.WantedLevel}");
            if (failed)
                BeginFailFlash("Heat cooked the run");
        }

        // F6 — simulate death/arrest while carrying → fail.
        if (_controls.IsKeyDown(Keys.F6))
        {
            if (_mission.NotifyDeathOrArrest())
            {
                BeginFailFlash("Busted — cargo forfeited");
                Console.WriteLine(
                    $"[VS-06] Death/arrest while carrying — fail, retry in {SmuggleMission.RetryDelaySeconds:0}s");
            }
            else
            {
                Console.WriteLine("[VS-06] Death stub ignored — not carrying");
            }
        }

        _mission.Tick(dt, out var failedByTimer);
        if (failedByTimer)
        {
            BeginFailFlash("Too slow — buyer walked");
            Console.WriteLine(
                $"[VS-06] Timer expired — fail, retry in {SmuggleMission.RetryDelaySeconds:0}s");
        }

        if (_deliveredFlashSeconds > 0f)
            _deliveredFlashSeconds = Math.Max(0f, _deliveredFlashSeconds - dt);
        if (_failFlashSeconds > 0f)
            _failFlashSeconds = Math.Max(0f, _failFlashSeconds - dt);

        DiagnosticValues.Set(
            "mission",
            _mission.Phase switch
            {
                SmuggleMissionPhase.Available => "available",
                SmuggleMissionPhase.AwaitingPickup => "pickup",
                SmuggleMissionPhase.Carrying =>
                    $"deliver {FormatMmSs(_mission.TimeRemainingSeconds)} wanted={_mission.WantedLevel}",
                SmuggleMissionPhase.Cooldown =>
                    $"cooldown {FormatMmSs(SmuggleMission.RetryDelaySeconds - _mission.CooldownElapsedSeconds)}",
                _ => _mission.Phase.ToString()
            });
    }

    public override void Draw(GameTime gameTime)
    {
        if (_spriteBatch == null || _font == null || _pixel == null)
            return;

        // hud_mission — only when active, or brief delivered/fail flash.
        var showDelivered = _deliveredFlashSeconds > 0f;
        var showFail = _failFlashSeconds > 0f;
        if (!_mission.IsActive && !showDelivered && !showFail)
            return;

        var vp = GraphicsDevice.Viewport;
        var sx = vp.Width / LogicalWidth;
        var sy = vp.Height / LogicalHeight;

        string text;
        Color color;
        if (showDelivered)
        {
            text = $"Smuggle-01: Delivered · +{SmuggleMission.RewardCrypto}";
            color = new Color(0x2E, 0xE6, 0xD6);
        }
        else if (showFail)
        {
            text = $"Smuggle-01: {_failFlashText ?? "Failed"}";
            color = new Color(0xE6, 0x6A, 0x2E);
        }
        else if (_mission.Phase == SmuggleMissionPhase.AwaitingPickup)
        {
            text = "Smuggle-01: Collect package";
            color = Color.White;
        }
        else // Carrying
        {
            text = $"Smuggle-01: Deliver · {FormatMmSs(_mission.TimeRemainingSeconds)}";
            color = _mission.TimeRemainingSeconds <= 60f
                ? new Color(0xE6, 0x6A, 0x2E)
                : Color.White;
        }

        // DebugFont is Latin-1; replace middle-dot if needed for measure/draw safety.
        text = text.Replace('·', '-');

        var textSize = _font.MeasureString(text);
        var lineY = (LogicalHeight - MissionBottom) * sy;
        var panelPadX = 12f * sx;
        var panelW = Math.Min(720f * sx, textSize.X + panelPadX * 2f);
        var panelX = (vp.Width - panelW) * 0.5f;
        var panelRect = new Rectangle(
            (int)panelX,
            (int)(lineY - 4f * sy),
            (int)panelW,
            (int)(28 * sy));
        var textPos = new Vector2((vp.Width - textSize.X) * 0.5f, lineY);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _spriteBatch.Draw(_pixel, panelRect, new Color(0, 0, 0, 160));
        _spriteBatch.DrawString(_font, text, textPos, color);
        _spriteBatch.End();

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
    }

    private void BeginFailFlash(string message)
    {
        _failFlashText = message;
        _failFlashSeconds = DeliveredFlashSeconds;
        _deliveredFlashSeconds = 0f;
    }

    private static string FormatMmSs(float seconds)
    {
        var total = Math.Max(0, (int)Math.Ceiling(seconds));
        var m = total / 60;
        var s = total % 60;
        return $"{m}:{s:D2}";
    }
}
