namespace StejarTransfers.Domain;

public enum Currency
{
    Mdl,
    Eur,
}

public enum TransferType
{
    Standard,
    Instant,
}

public enum CustomerSegment
{
    Retail,
    Premium,
}

public sealed record FeeRequest(decimal Amount, Currency Currency, TransferType Type, CustomerSegment CustomerSegment);

/// <summary>A computed fee. Fees are always in MDL, with 2 decimals.</summary>
public sealed record Fee(decimal Amount, string Rule)
{
    public Currency Currency => Currency.Mdl;
}
