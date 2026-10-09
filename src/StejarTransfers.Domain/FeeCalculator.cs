using StejarTransfers.Clients;

namespace StejarTransfers.Domain;

/// <summary>Fee calculation. The published tariff is in docs/fee-rules.md.</summary>
public sealed class FeeCalculator(IExchangeRateClient rates)
{
    public async Task<Fee> CalculateAsync(FeeRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Type == TransferType.Instant)
        {
            throw new UnsupportedTransferException("instant transfers not supported yet");
        }

        if (request.Currency == Currency.Mdl)
        {
            return new Fee(StandardFee.ForMdl(request.Amount), "standard-mdl");
        }

        return new Fee(await StandardFeeEurAsync(request.Amount, cancellationToken), "standard-eur");
    }

    private async Task<decimal> StandardFeeEurAsync(decimal amountEur, CancellationToken cancellationToken)
    {
        var exchangeRate = await rates.GetRateAsync("EUR", "MDL", cancellationToken);
        var amountMdl = Math.Round(amountEur * exchangeRate.Rate, 2, MidpointRounding.ToZero);
        var fee = Math.Round(amountMdl * StandardFee.Rate, 2, MidpointRounding.ToEven);
        return StandardFee.Clamp(fee);
    }
}
