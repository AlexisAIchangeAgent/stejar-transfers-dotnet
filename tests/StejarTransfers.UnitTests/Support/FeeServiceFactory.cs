using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StejarTransfers.Clients;

namespace StejarTransfers.UnitTests.Support;

/// <summary>Starts the fee service in memory, with a <see cref="FixedRateClient"/> at rate 20.0000.</summary>
public sealed class FeeServiceFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IExchangeRateClient>();
            services.AddSingleton<IExchangeRateClient>(new FixedRateClient(TestRates.Default));
        });
    }
}
