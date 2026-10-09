namespace StejarTransfers.Clients;

/// <summary>Stub used in tests and local runs: always returns the same rate, no network.</summary>
public sealed class FixedRateClient(decimal rate, DateOnly asOf) : IExchangeRateClient
{
    public const decimal DefaultRate = 19.8765m;

    public static readonly DateOnly DefaultAsOf = new(2026, 10, 19);

    public FixedRateClient()
        : this(DefaultRate, DefaultAsOf)
    {
    }

    public FixedRateClient(decimal rate)
        : this(rate, DefaultAsOf)
    {
    }

    public decimal Rate { get; } = rate;

    public DateOnly AsOf { get; } = asOf;

    public Task<ExchangeRate> GetRateAsync(string from, string to, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ExchangeRate(from, to, Rate, AsOf));
}
