using System.Net;
using StejarTransfers.Clients;
using StejarTransfers.Domain;
using StejarTransfers.UnitTests.Support;

namespace StejarTransfers.UnitTests;

public sealed class InstantFeeNotSupportedTests(FeeServiceFactory factory) : IClassFixture<FeeServiceFactory>
{
    [Fact]
    public async Task InstantTransfer_IsRejectedByTheCalculator()
    {
        var calculator = new FeeCalculator(new FixedRateClient(TestRates.Default));
        var request = new FeeRequest(2000.00m, Currency.Mdl, TransferType.Instant, CustomerSegment.Retail);

        var exception = await Assert.ThrowsAsync<UnsupportedTransferException>(() => calculator.CalculateAsync(request));

        Assert.Equal("instant transfers not supported yet", exception.Message);
    }

    [Theory]
    [InlineData("MDL", "retail")]
    [InlineData("MDL", "premium")]
    [InlineData("EUR", "retail")]
    [InlineData("EUR", "premium")]
    public async Task PostFees_Returns422ForAnInstantTransfer(string currency, string segment)
    {
        using var client = factory.CreateClient();
        var body = FeeBodies.Json("2000.00", currency, "instant", segment);

        var response = await client.PostAsync("/fees", body);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(
            """{"error":"instant transfers not supported yet"}""",
            await response.Content.ReadAsStringAsync());
    }
}
