using StejarTransfers.Api;
using StejarTransfers.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExchangeRateClient(RatesSettings.FromConfiguration(builder.Configuration));
builder.Services.AddScoped<FeeCalculator>();

var app = builder.Build();

app.MapFeeEndpoints();

app.Run();

/// <summary>Entry point. Public so that the tests can start the service with WebApplicationFactory.</summary>
public partial class Program
{
}
