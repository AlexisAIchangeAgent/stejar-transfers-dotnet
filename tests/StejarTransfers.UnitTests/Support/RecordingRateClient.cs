using StejarTransfers.Clients;

namespace StejarTransfers.UnitTests.Support;

/// <summary>Fake exchange-rate client that records every call.</summary>
public sealed class RecordingRateClient(decimal rate) : IExchangeRateClient
{
    public List<(string From, string To)> Calls { get; } = [];

    public Task<ExchangeRate> GetRateAsync(string from, string to, CancellationToken cancellationToken = default)
    {
        Calls.Add((from, to));
        return Task.FromResult(new ExchangeRate(from, to, rate, new DateOnly(2026, 10, 19)));
    }
}
