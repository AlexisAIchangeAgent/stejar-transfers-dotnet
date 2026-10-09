using System.Net.Http.Headers;
using StejarTransfers.Clients;

namespace StejarTransfers.Api;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the exchange-rate client: <see cref="HttpExchangeRateClient"/> when a base URL is
    /// set, otherwise <see cref="FixedRateClient"/> (rate 19.8765, no network).
    /// </summary>
    public static IServiceCollection AddExchangeRateClient(this IServiceCollection services, RatesSettings settings)
    {
        if (settings.BaseUrl is null)
        {
            services.AddSingleton<IExchangeRateClient>(new FixedRateClient());
            return services;
        }

        services.AddHttpClient<IExchangeRateClient, HttpExchangeRateClient>(client =>
        {
            client.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = settings.Timeout;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (settings.ApiKey is not null)
            {
                client.DefaultRequestHeaders.Add(HttpExchangeRateClient.ApiKeyHeader, settings.ApiKey);
            }
        });
        return services;
    }
}
