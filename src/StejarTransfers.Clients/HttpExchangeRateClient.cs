using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StejarTransfers.Clients;

/// <summary>
/// Calls the exchange-rate API over HTTP. The <see cref="HttpClient"/> carries the base address,
/// the timeout and the API key header (see <c>ServiceCollectionExtensions</c> in the Api project).
/// </summary>
public sealed class HttpExchangeRateClient(HttpClient httpClient) : IExchangeRateClient
{
    public const string ApiKeyHeader = "X-Api-Key";

    public async Task<ExchangeRate> GetRateAsync(string from, string to, CancellationToken cancellationToken = default)
    {
        var requestUri = $"rates?from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}";
        try
        {
            var payload = await httpClient.GetFromJsonAsync<RatePayload>(requestUri, cancellationToken)
                ?? throw new ExchangeRateException($"cannot get rate {from}/{to}: empty response");
            return new ExchangeRate(payload.From, payload.To, payload.Rate, payload.AsOf);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            throw new ExchangeRateException($"cannot get rate {from}/{to}", exception);
        }
    }

    private sealed class RatePayload
    {
        [JsonPropertyName("from")]
        public required string From { get; init; }

        [JsonPropertyName("to")]
        public required string To { get; init; }

        [JsonPropertyName("rate")]
        public required decimal Rate { get; init; }

        [JsonPropertyName("as_of")]
        public required DateOnly AsOf { get; init; }
    }
}
