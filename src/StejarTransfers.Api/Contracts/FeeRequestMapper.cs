using System.Diagnostics.CodeAnalysis;
using StejarTransfers.Domain;

namespace StejarTransfers.Api.Contracts;

/// <summary>Validates the HTTP body and maps it to the domain request.</summary>
public static class FeeRequestMapper
{
    public static bool TryMap(
        FeeRequestBody body,
        [NotNullWhen(true)] out FeeRequest? request,
        [NotNullWhen(false)] out string? error)
    {
        request = null;
        error = Validate(body);
        if (error is not null)
        {
            return false;
        }

        request = new FeeRequest(
            body.Amount!.Value,
            body.Currency == "EUR" ? Currency.Eur : Currency.Mdl,
            body.Type == "instant" ? TransferType.Instant : TransferType.Standard,
            body.CustomerSegment == "premium" ? CustomerSegment.Premium : CustomerSegment.Retail);
        return true;
    }

    private static string? Validate(FeeRequestBody body) => body switch
    {
        { Amount: null } => "amount is required",
        { Amount: <= 0m } => "amount must be greater than 0",
        { Amount: var amount } when decimal.Round(amount.Value, 2) != amount.Value => "amount must have at most 2 decimals",
        { Currency: not ("MDL" or "EUR") } => "currency must be MDL or EUR",
        { Type: not ("standard" or "instant") } => "type must be standard or instant",
        { CustomerSegment: not ("retail" or "premium") } => "customer_segment must be retail or premium",
        _ => null,
    };
}
