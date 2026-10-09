namespace StejarTransfers.Clients;

/// <summary>A rate given by the exchange-rate provider: 1 <c>From</c> = <c>Rate</c> <c>To</c>.</summary>
public sealed record ExchangeRate(string From, string To, decimal Rate, DateOnly AsOf);
