using StejarTransfers.Clients;
using StejarTransfers.Domain;
using StejarTransfers.UnitTests.Support;

namespace StejarTransfers.UnitTests;

public sealed class StandardEurFeeTests
{
    private static FeeRequest EurRequest(string amount, CustomerSegment segment = CustomerSegment.Retail) =>
        new(Money.Of(amount), Currency.Eur, TransferType.Standard, segment);

    [Theory]
    [InlineData("100.00", "10.00")]
    [InlineData("300.00", "30.00")]
    [InlineData("10.00", "5.00")]
    [InlineData("1000.00", "50.00")]
    public async Task StandardEurFee_ConvertsToMdlThenAppliesTheMdlRule(string amountEur, string expectedFee)
    {
        var calculator = new FeeCalculator(new FixedRateClient(TestRates.Default));

        var fee = await calculator.CalculateAsync(EurRequest(amountEur));

        Assert.Equal(Money.Of(expectedFee), fee.Amount);
        Assert.Equal(Currency.Mdl, fee.Currency);
        Assert.Equal("standard-eur", fee.Rule);
    }

    [Fact]
    public async Task StandardEurFee_AsksTheEurToMdlRate()
    {
        var rates = new RecordingRateClient(TestRates.Default);

        await new FeeCalculator(rates).CalculateAsync(EurRequest("100.00"));

        Assert.Equal(("EUR", "MDL"), Assert.Single(rates.Calls));
    }

    [Fact]
    public async Task StandardEurFee_WithThePublishedRate()
    {
        var calculator = new FeeCalculator(new FixedRateClient());

        // EUR 100.00 at 19.8765 is MDL 1,987.65, and 0.5% of it is 9.93825
        var fee = await calculator.CalculateAsync(EurRequest("100.00"));

        Assert.Equal(9.94m, fee.Amount);
    }

    [Fact]
    public async Task MdlTransfer_DoesNotAskForARate()
    {
        var rates = new RecordingRateClient(TestRates.Default);

        await new FeeCalculator(rates).CalculateAsync(
            new FeeRequest(100.00m, Currency.Mdl, TransferType.Standard, CustomerSegment.Retail));

        Assert.Empty(rates.Calls);
    }
}
