using System.Text.Json.Serialization;

namespace StejarTransfers.Api.Contracts;

/// <summary>Body of <c>POST /fees</c>. Fields are nullable so that a missing field gives a 422.</summary>
public sealed record FeeRequestBody(
    [property: JsonPropertyName("amount")] decimal? Amount,
    [property: JsonPropertyName("currency")] string? Currency,
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("customer_segment")] string? CustomerSegment);

public sealed record FeeResponseBody(
    [property: JsonPropertyName("fee")] decimal Fee,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("rule")] string Rule);

public sealed record ErrorBody([property: JsonPropertyName("error")] string Error);

public sealed record HealthBody([property: JsonPropertyName("status")] string Status);
