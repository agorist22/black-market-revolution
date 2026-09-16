namespace BlackMarketRevolution.Economy;

/// <summary>
/// Slice wallet: crypto is the live currency; fiat is display-disabled until Phase 2.
/// See docs/ui/HUD-WIREFRAME-SLICE.md and docs/BALANCE-GREY-ARCADE.md (START_CRYPTO).
/// </summary>
public sealed class PlayerWallet
{
    public const int StartCrypto = 80;

    public PlayerWallet(int crypto = StartCrypto)
    {
        Crypto = crypto;
    }

    /// <summary>Live slice currency (integer HUD display).</summary>
    public int Crypto { get; set; }

    /// <summary>
    /// Fiat is not live in the grey-market slice. When false, HUD shows grey "—" (or omits).
    /// </summary>
    public bool FiatDisplayEnabled => false;

    public void AddCrypto(int amount) => Crypto += amount;

    public bool TrySpendCrypto(int amount)
    {
        if (amount < 0 || Crypto < amount)
            return false;

        Crypto -= amount;
        return true;
    }
}
