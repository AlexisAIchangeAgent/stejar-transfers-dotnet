namespace StejarTransfers.Clients;

/// <summary>
/// Exchange-rate client: the third-party API that gives the EUR to MDL rate.
/// </summary>
/// <remarks>
/// Contract of the provider:
/// <c>GET {base_url}/rates?from=EUR&amp;to=MDL</c>
/// 200 <c>{"from": "EUR", "to": "MDL", "rate": 19.8765, "as_of": "2026-10-19"}</c>
/// </remarks>
public interface IExchangeRateClient
{
    Task<ExchangeRate> GetRateAsync(string from, string to, CancellationToken cancellationToken = default);
}
