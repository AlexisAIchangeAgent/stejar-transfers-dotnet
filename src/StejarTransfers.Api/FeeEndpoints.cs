using StejarTransfers.Api.Contracts;
using StejarTransfers.Clients;
using StejarTransfers.Domain;

namespace StejarTransfers.Api;

/// <summary>HTTP routes of the fee service.</summary>
public static class FeeEndpoints
{
    public static IEndpointRouteBuilder MapFeeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new HealthBody("ok")));
        app.MapPost("/fees", ComputeFeeAsync);
        return app;
    }

    private static async Task<IResult> ComputeFeeAsync(
        FeeRequestBody body,
        FeeCalculator calculator,
        CancellationToken cancellationToken)
    {
        if (!FeeRequestMapper.TryMap(body, out var request, out var error))
        {
            return Error(StatusCodes.Status422UnprocessableEntity, error);
        }

        try
        {
            var fee = await calculator.CalculateAsync(request, cancellationToken);
            return Results.Ok(new FeeResponseBody(fee.Amount, "MDL", fee.Rule));
        }
        catch (UnsupportedTransferException exception)
        {
            return Error(StatusCodes.Status422UnprocessableEntity, exception.Message);
        }
        catch (ExchangeRateException)
        {
            return Error(StatusCodes.Status502BadGateway, "exchange rate unavailable");
        }
    }

    private static IResult Error(int statusCode, string message) =>
        Results.Json(new ErrorBody(message), statusCode: statusCode);
}
