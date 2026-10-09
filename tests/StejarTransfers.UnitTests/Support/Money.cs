using System.Globalization;

namespace StejarTransfers.UnitTests.Support;

/// <summary>Builds decimals from strings: attributes cannot hold decimal values.</summary>
public static class Money
{
    public static decimal Of(string value) => decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
}
