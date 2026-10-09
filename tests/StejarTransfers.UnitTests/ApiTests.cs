using System.Net;
using System.Text.Json;
using StejarTransfers.UnitTests.Support;

namespace StejarTransfers.UnitTests;

public sealed class ApiTests(FeeServiceFactory factory) : IClassFixture<FeeServiceFactory>
{
    [Fact]
    public async Task Health_ReturnsOk()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("""{"status":"ok"}""", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task PostFees_ReturnsTheStandardMdlFee()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/fees", FeeBodies.Json("2000.00"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(10.00m, body.GetProperty("fee").GetDecimal());
        Assert.Equal("MDL", body.GetProperty("currency").GetString());
        Assert.Equal("standard-mdl", body.GetProperty("rule").GetString());
    }

    [Fact]
    public async Task PostFees_ReturnsTheFeeAsAJsonNumberWithTwoDecimals()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/fees", FeeBodies.Json("2000"));

        Assert.Contains("\"fee\":10.00", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task PostFees_ReturnsTheStandardEurFeeInMdl()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/fees", FeeBodies.Json("300.00", currency: "EUR", segment: "premium"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(30.00m, body.GetProperty("fee").GetDecimal());
        Assert.Equal("MDL", body.GetProperty("currency").GetString());
        Assert.Equal("standard-eur", body.GetProperty("rule").GetString());
    }

    [Theory]
    [InlineData("""{"amount": 100.00, "currency": "USD", "type": "standard", "customer_segment": "retail"}""")]
    [InlineData("""{"amount": 0, "currency": "MDL", "type": "standard", "customer_segment": "retail"}""")]
    [InlineData("""{"amount": -5.00, "currency": "MDL", "type": "standard", "customer_segment": "retail"}""")]
    [InlineData("""{"amount": 10.001, "currency": "MDL", "type": "standard", "customer_segment": "retail"}""")]
    [InlineData("""{"amount": 100.00, "currency": "MDL", "type": "express", "customer_segment": "retail"}""")]
    [InlineData("""{"amount": 100.00, "currency": "MDL", "type": "standard", "customer_segment": "vip"}""")]
    [InlineData("""{"currency": "MDL", "type": "standard", "customer_segment": "retail"}""")]
    public async Task PostFees_RejectsAnInvalidBodyWith422(string json)
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/fees", FeeBodies.Raw(json));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }
}
