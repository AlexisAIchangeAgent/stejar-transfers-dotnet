using StejarTransfers.Clients;
using StejarTransfers.Domain;
using StejarTransfers.UnitTests.Support;

namespace StejarTransfers.UnitTests;

public sealed class StandardMdlFeeTests
{
    private readonly FeeCalculator _calculator = new(new FixedRateClient(TestRates.Default));

    private static FeeRequest MdlRequest(string amount, CustomerSegment segment = CustomerSegment.Retail) =>
        new(Money.Of(amount), Currency.Mdl, TransferType.Standard, segment);

    [Theory]
    [InlineData("2000.00", "10.00")]
    [InlineData("1234.00", "6.17")]
    [InlineData("9999.00", "50.00")]
    public async Task StandardMdlFee_IsHalfAPercent(string amount, string expectedFee)
    {
        var fee = await _calculator.CalculateAsync(MdlRequest(amount));

        Assert.Equal(Money.Of(expectedFee), fee.Amount);
        Assert.Equal(Currency.Mdl, fee.Currency);
        Assert.Equal("standard-mdl", fee.Rule);
    }

    [Theory]
    [InlineData("0.01")]
    [InlineData("100.00")]
    [InlineData("999.99")]
    [InlineData("1000.00")]
    public async Task StandardMdlFee_HasAMinimumOf5(string amount)
    {
        var fee = await _calculator.CalculateAsync(MdlRequest(amount));

        Assert.Equal(5.00m, fee.Amount);
    }

    [Theory]
    [InlineData("10000.00")]
    [InlineData("10000.01")]
    [InlineData("250000.00")]
    public async Task StandardMdlFee_HasAMaximumOf50(string amount)
    {
        var fee = await _calculator.CalculateAsync(MdlRequest(amount));

        Assert.Equal(50.00m, fee.Amount);
    }

    [Fact]
    public async Task StandardMdlFee_RoundsHalfUp()
    {
        // 0.5% of 1001.00 is 5.005
        var fee = await _calculator.CalculateAsync(MdlRequest("1001.00"));

        Assert.Equal(5.01m, fee.Amount);
    }

    [Fact]
    public async Task StandardMdlFee_HasTwoDecimals()
    {
        var fee = await _calculator.CalculateAsync(MdlRequest("2000"));

        Assert.Equal(2, fee.Amount.Scale);
    }

    [Fact]
    public async Task PremiumStandardFee_IsTheRetailFee()
    {
        var retail = await _calculator.CalculateAsync(MdlRequest("3000.00", CustomerSegment.Retail));
        var premium = await _calculator.CalculateAsync(MdlRequest("3000.00", CustomerSegment.Premium));

        Assert.Equal(retail, premium);
    }
}
