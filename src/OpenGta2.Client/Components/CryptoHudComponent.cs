using System;
using System.Globalization;
using BlackMarketRevolution.Economy;
using BlackMarketRevolution.Slice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenGta2.Client.Diagnostics;

namespace OpenGta2.Client.Components;

/// <summary>
/// VS-04 crypto HUD (top-left). Fiat shown greyed as unused.
/// Debug: F5 street trade earn, F6 attempt underground property buy (150).
/// </summary>
public sealed class CryptoHudComponent : BaseDrawableComponent
{
    private readonly Controls _controls;
    private readonly CryptoWallet _wallet;
    private readonly SpriteBatch _spriteBatch;
    private SpriteFont? _font;
    private float _tradeCooldown;
    private string? _toast;
    private float _toastTimer;
    private readonly Random _rng = new();

    public CryptoHudComponent(GtaGame game, Controls controls, CryptoWallet wallet) : base(game)
    {
        _controls = controls;
        _wallet = wallet;
        _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        DrawOrder = 900;
    }

    protected override void LoadContent()
    {
        _font = Game.AssetManager.GetDebugFont();
    }

    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_tradeCooldown > 0)
            _tradeCooldown -= dt;
        if (_toastTimer > 0)
        {
            _toastTimer -= dt;
            if (_toastTimer <= 0)
                _toast = null;
        }

        if (_controls.IsKeyDown(Keys.F5) && _tradeCooldown <= 0)
        {
            var reward = _rng.Next(GreyMarketBalance.TradeRewardMin, GreyMarketBalance.TradeRewardMax + 1);
            _wallet.Add(reward);
            _tradeCooldown = GreyMarketBalance.TradeCooldownSeconds;
            Flash($"+{reward} crypto (trade)");
        }

        if (_controls.IsKeyDown(Keys.F6))
        {
            if (_wallet.TrySpend(GreyMarketBalance.PropertyPrice))
                Flash($"Bought underground (-{GreyMarketBalance.PropertyPrice})");
            else
                Flash($"Need {GreyMarketBalance.PropertyPrice} crypto");
        }
    }

    public override void Draw(GameTime gameTime)
    {
        if (_font == null)
            return;

        var crypto = string.Create(CultureInfo.InvariantCulture, $"Crypto: {_wallet.Balance}");
        const string fiat = "Fiat: --";

        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, crypto, new Vector2(12, 12), Color.LimeGreen);
        _spriteBatch.DrawString(_font, fiat, new Vector2(12, 36), Color.Gray * 0.7f);
        if (_toast != null)
            _spriteBatch.DrawString(_font, _toast, new Vector2(12, 64), Color.Yellow);
        _spriteBatch.End();
    }

    private void Flash(string message)
    {
        _toast = message;
        _toastTimer = 2f;
        DiagnosticValues.Set("crypto", _wallet.Balance.ToString(CultureInfo.InvariantCulture));
    }
}
