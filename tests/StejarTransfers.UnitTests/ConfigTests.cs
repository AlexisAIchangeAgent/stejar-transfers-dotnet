using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StejarTransfers.Api;
using StejarTransfers.Clients;

namespace StejarTransfers.UnitTests;

public sealed class ConfigTests
{
    [Fact]
    public void RatesSettings_AreReadFromTheEnvironmentVariableNames()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RATES_BASE_URL"] = "https://rates.example.invalid",
                ["RATES_API_KEY"] = "fake-key-do-not-use",
                ["RATES_TIMEOUT_SECONDS"] = "2",
            })
            .Build();

        var settings = RatesSettings.FromConfiguration(configuration);

        Assert.Equal("https://rates.example.invalid", settings.BaseUrl);
        Assert.Equal("fake-key-do-not-use", settings.ApiKey);
        Assert.Equal(TimeSpan.FromSeconds(2), settings.Timeout);
    }

    [Fact]
    public void HttpRateClient_IsUsedWhenABaseUrlIsSet()
    {
        var clientType = RegisteredRateClientType(
            new RatesSettings("https://rates.example.invalid", null, RatesSettings.DefaultTimeout));

        Assert.Equal(typeof(HttpExchangeRateClient), clientType);
    }

    [Fact]
    public void FixedRateClient_IsUsedWithoutABaseUrl()
    {
        var clientType = RegisteredRateClientType(new RatesSettings(null, null, RatesSettings.DefaultTimeout));

        Assert.Equal(typeof(FixedRateClient), clientType);
    }

    [Fact]
    public async Task FixedRateClient_ReturnsThePublishedRate()
    {
        var rate = await new FixedRateClient().GetRateAsync("EUR", "MDL");

        Assert.Equal(("EUR", "MDL", 19.8765m), (rate.From, rate.To, rate.Rate));
    }

    private static Type RegisteredRateClientType(RatesSettings settings)
    {
        using var provider = new ServiceCollection().AddExchangeRateClient(settings).BuildServiceProvider();
        return provider.GetRequiredService<IExchangeRateClient>().GetType();
    }
}
