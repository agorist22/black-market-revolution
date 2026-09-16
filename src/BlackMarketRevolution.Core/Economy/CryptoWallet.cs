namespace BlackMarketRevolution.Economy;

/// <summary>
/// Session crypto wallet for the grey-market slice (VS-04).
/// Fiat is intentionally unused — HUD may show a grey placeholder only.
/// </summary>
public sealed class CryptoWallet
{
    public CryptoWallet(int startingBalance = Slice.GreyMarketBalance.StartCrypto)
    {
        if (startingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(startingBalance));
        Balance = startingBalance;
    }

    public int Balance { get; private set; }

    public void Add(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        Balance = checked(Balance + amount);
    }

    /// <summary>Spend if funds allow. Never goes negative; returns false when short.</summary>
    public bool TrySpend(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        if (Balance < amount)
            return false;
        Balance -= amount;
        return true;
    }
}
