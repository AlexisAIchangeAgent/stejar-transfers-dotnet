using System.Text;

namespace StejarTransfers.UnitTests.Support;

/// <summary>JSON bodies for <c>POST /fees</c>, written by hand so that the amount is sent exactly as given.</summary>
public static class FeeBodies
{
    public static StringContent Json(
        string amount,
        string currency = "MDL",
        string type = "standard",
        string segment = "retail") =>
        Raw($$"""{"amount": {{amount}}, "currency": "{{currency}}", "type": "{{type}}", "customer_segment": "{{segment}}"}""");

    public static StringContent Raw(string json) => new(json, Encoding.UTF8, "application/json");
}
