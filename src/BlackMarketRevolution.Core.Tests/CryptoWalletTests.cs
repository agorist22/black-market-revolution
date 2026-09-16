using BlackMarketRevolution.Economy;
using BlackMarketRevolution.Slice;
using Xunit;

namespace BlackMarketRevolution.Core.Tests;

public class CryptoWalletTests
{
    [Fact]
    public void Starts_at_slice_balance()
    {
        var w = new CryptoWallet();
        Assert.Equal(GreyMarketBalance.StartCrypto, w.Balance);
    }

    [Fact]
    public void TrySpend_clamps_and_rejects_when_short()
    {
        var w = new CryptoWallet(50);
        Assert.False(w.TrySpend(150));
        Assert.Equal(50, w.Balance);
        Assert.True(w.TrySpend(40));
        Assert.Equal(10, w.Balance);
    }

    [Fact]
    public void Add_increases_balance()
    {
        var w = new CryptoWallet(80);
        w.Add(25);
        Assert.Equal(105, w.Balance);
    }
}
