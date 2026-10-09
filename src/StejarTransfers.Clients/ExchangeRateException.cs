namespace StejarTransfers.Clients;

/// <summary>The exchange rate could not be obtained.</summary>
public sealed class ExchangeRateException(string message, Exception? innerException = null)
    : Exception(message, innerException);
