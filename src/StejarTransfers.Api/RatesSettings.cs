using System.Globalization;

namespace StejarTransfers.Api;

/// <summary>
/// Settings of the exchange-rate client, read from configuration. In ASP.NET Core, environment
/// variables are part of the configuration, so <c>RATES_BASE_URL</c> is read from the environment.
/// </summary>
public sealed record RatesSettings(string? BaseUrl, string? ApiKey, TimeSpan Timeout)
{
    public const string BaseUrlKey = "RATES_BASE_URL";
    public const string ApiKeyKey = "RATES_API_KEY";
    public const string TimeoutSecondsKey = "RATES_TIMEOUT_SECONDS";

    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    public static RatesSettings FromConfiguration(IConfiguration configuration)
    {
        var timeoutSeconds = configuration[TimeoutSecondsKey];
        var timeout = string.IsNullOrWhiteSpace(timeoutSeconds)
            ? DefaultTimeout
            : TimeSpan.FromSeconds(double.Parse(timeoutSeconds, CultureInfo.InvariantCulture));

        return new RatesSettings(
            NullIfEmpty(configuration[BaseUrlKey]),
            NullIfEmpty(configuration[ApiKeyKey]),
            timeout);
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
