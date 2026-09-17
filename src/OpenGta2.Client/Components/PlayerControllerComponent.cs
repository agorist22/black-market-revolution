using System;
using BlackMarketRevolution.Slice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OpenGta2.Client.Diagnostics;
using OpenGta2.Client.Levels;
using OpenGta2.Client.Peds;
using OpenGta2.Client.Utilities;

namespace OpenGta2.Client.Components;

public class PlayerControllerComponent : BaseComponent
{
    private readonly Controls _controls;
    private readonly PedManager _pedManager;
    private readonly LevelProvider _levelProvider;
    private readonly Camera _camera;
    private Ped? _player;
    private bool _moveSlow;

    public PlayerControllerComponent(GtaGame game, Controls controls, PedManager pedManager, LevelProvider levelProvider, Camera camera) : base(game)
    {
        _controls = controls;
        _pedManager = pedManager;
        _levelProvider = levelProvider;
        _camera = camera;
    }

    public override void Initialize()
    {
        // Grey Arcade hub spawn (Atlas SPAWN_PLAYER → OpenGta2 world via AtlasToWorldScale).
        var (sx, sy) = GreyArcadeMarkers.ToWorld(GreyArcadeMarkers.SpawnPlayer);
        var map = _levelProvider.Map;
        var ix = Math.Clamp((int)sx, 0, Math.Max(0, map.Width - 1));
        var iy = Math.Clamp((int)sy, 0, Math.Max(0, map.Height - 1));
        var gz = map.GetGroundZ(ix, iy);

        _player = new Ped(new Vector3(sx, sy, gz), 0, 25);
        _pedManager.Peds.Add(_player);
        _camera.Attach(_player);

        DiagnosticValues.Set(
            "world.spawn",
            $"atlas=({GreyArcadeMarkers.SpawnPlayer.X},{GreyArcadeMarkers.SpawnPlayer.Y}) " +
            $"world=({sx:0.00},{sy:0.00}) scale={GreyArcadeMarkers.AtlasToWorldScale}");
    }

    public override void Update(GameTime gameTime)
    {
        if (_player == null) return;

        var fd = 0f;
        var lr = 0f;

        if (_controls.IsKeyPressed(Control.Right))
            lr++;

        if (_controls.IsKeyPressed(Control.Left))
            lr--;

        if (_controls.IsKeyPressed(Control.Forward))
            fd++;

        if (_controls.IsKeyPressed(Control.Backward))
            fd--;

        if (_controls.IsKeyDown(Keys.F2))
        {
            _moveSlow = !_moveSlow;
        }

        _player.Rotation += lr * MathHelper.TwoPi * gameTime.GetDelta();

        var heading = new Vector2(MathF.Sin(-_player.Rotation), MathF.Cos(-_player.Rotation));
        var delta = heading * fd * gameTime.GetDelta() * 2 * (_moveSlow ? 0.1f : 1);

        const float playerWidth = 0.1f;//(18f / 64);

        DiagnosticHighlight.Add(_player.Position - new Vector3(playerWidth/2, playerWidth/2, 0), new Vector3(playerWidth, playerWidth, 1), Color.Blue);

        _player.Position = _levelProvider.CollisionMap.CalculateMovement(_player.Position, playerWidth, delta);

        // Soft clip — clamp to Grey Arcade AABB (Atlas CLIP → world).
        SoftClampToClip(_player);

        _player.Animation = fd != 0 ? PedAnimation.Walking  : PedAnimation.Idle;
    }

    private static void SoftClampToClip(Ped player)
    {
        var minX = GreyArcadeMarkers.AtlasToWorld(GreyArcadeMarkers.ClipMin.X);
        var minY = GreyArcadeMarkers.AtlasToWorld(GreyArcadeMarkers.ClipMin.Y);
        var maxX = GreyArcadeMarkers.AtlasToWorld(GreyArcadeMarkers.ClipMax.X);
        var maxY = GreyArcadeMarkers.AtlasToWorld(GreyArcadeMarkers.ClipMax.Y);

        var p = player.Position;
        var cx = Math.Clamp(p.X, minX, maxX);
        var cy = Math.Clamp(p.Y, minY, maxY);
        if (cx != p.X || cy != p.Y)
            player.Position = new Vector3(cx, cy, p.Z);
    }
}
