namespace StejarTransfers.Domain;

/// <summary>Standard fee of the published tariff (docs/fee-rules.md).</summary>
public static class StandardFee
{
    public const decimal Rate = 0.005m;
    public const decimal Minimum = 5.00m;
    public const decimal Maximum = 50.00m;

    /// <summary>Standard fee of an MDL amount: 0.5%, minimum MDL 5.00, maximum MDL 50.00.</summary>
    public static decimal ForMdl(decimal amountMdl)
    {
        var fee = Math.Round(amountMdl * Rate, 2, MidpointRounding.AwayFromZero);
        return Clamp(fee);
    }

    public static decimal Clamp(decimal fee) => Math.Clamp(fee, Minimum, Maximum);
}
