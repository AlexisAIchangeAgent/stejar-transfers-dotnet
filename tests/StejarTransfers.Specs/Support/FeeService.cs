using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StejarTransfers.Clients;

namespace StejarTransfers.Specs.Support;

/// <summary>
/// The fee service, started in memory for one scenario. Step definitions get it through their
/// constructor (Reqnroll context injection); Reqnroll disposes it at the end of the scenario.
/// The fixed-rate stub is used: no network call.
/// </summary>
public sealed class FeeService : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    public FeeService()
    {
        RateClient = new FixedRateClient();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IExchangeRateClient>();
                services.AddSingleton<IExchangeRateClient>(RateClient);
            }));
        Client = _factory.CreateClient();
    }

    /// <summary>Exchange-rate stub of the scenario, rate 19.8765.</summary>
    public FixedRateClient RateClient { get; }

    /// <summary>HTTP client of the in-memory fee service.</summary>
    public HttpClient Client { get; }

    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}
