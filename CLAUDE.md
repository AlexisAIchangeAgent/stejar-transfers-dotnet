# CLAUDE.md: stejar-transfers-dotnet

Training material: invented bank, invented data. Stejar Bank SA does not exist.

## Purpose

HTTP service of Stejar Bank that computes the fee of a customer transfer before the customer
confirms it. `POST /fees` takes an amount, a currency (MDL or EUR), a transfer type (standard
or instant) and a customer segment (retail or premium), and returns the fee in MDL with the
rule applied. EUR amounts are converted with a rate from a third-party exchange-rate API.
The published tariff is `docs/fee-rules.md`.

## Stack

- .NET 10 (LTS), C# 14. `global.json` asks for SDK 10.0.100 or later. Every project targets
  `net10.0`, nullable and implicit usings on (`Directory.Build.props`). Solution: `StejarTransfers.sln`.
- ASP.NET Core minimal API (`Microsoft.NET.Sdk.Web`), System.Text.Json. No other runtime package.
- Tests: xUnit 2.9.2, Microsoft.NET.Test.Sdk 17.11.1, Microsoft.AspNetCore.Mvc.Testing 10.0.0,
  Reqnroll.xUnit 3.3.4. Declared in the UnitTests project, not used yet: PactNet 5.0.0, Microsoft.Playwright 1.48.0.

## Folder map

```
src/StejarTransfers.Api/        Program.cs, FeeEndpoints.cs (GET /health, POST /fees), Contracts/ (bodies,
                                FeeRequestMapper), RatesSettings.cs, ServiceCollectionExtensions.cs, .http file
src/StejarTransfers.Domain/     FeeCalculator.cs, StandardFee.cs, Models.cs, UnsupportedTransferException.cs
src/StejarTransfers.Clients/    IExchangeRateClient, HttpExchangeRateClient, FixedRateClient, ExchangeRate
tests/StejarTransfers.UnitTests/  xUnit tests, Support/ (FeeServiceFactory, fakes, helpers)
tests/StejarTransfers.Specs/    Reqnroll: reqnroll.json, Support/FeeService.cs, StepDefinitions/ (empty)
features/                       instant-fee.feature, linked into the Specs project
crm/customer-lookup.html        static page of the internal CRM (for Playwright)
incidents/ docs/                incident tickets; README-service.md, fee-rules.md, adr/ (empty)
privacy-agent/                  GDPR intake agent material (Python): policy.md, requests/, output/
docs/pipeline/ci.yml            pipeline, not active yet (cycle 3 copies it to .github/workflows/): build and unit tests on push and PR, manual fake deploy
```

## Commands

Run from the repository root. The same commands work on Windows and macOS.

| Task | Command |
|---|---|
| Restore | `dotnet restore` |
| Build | `dotnet build` |
| Run | `dotnet run --project src/StejarTransfers.Api -- --urls http://localhost:8000` |
| Unit tests | `dotnet test tests/StejarTransfers.UnitTests` |
| One test class | `dotnet test tests/StejarTransfers.UnitTests --filter "FullyQualifiedName~StandardMdlFeeTests"` |
| BDD tests | `dotnet test tests/StejarTransfers.Specs` |
| Lint | `dotnet format --verify-no-changes` |
| Format | `dotnet format` |
| Playwright browser | `pwsh tests/StejarTransfers.UnitTests/bin/Debug/net10.0/playwright.ps1 install chromium` |

- `dotnet build`, `dotnet test` and `dotnet format` without a path use `StejarTransfers.sln`.
  `dotnet test` without a path also runs the Specs project, which fails today (see below).
- The Specs project runs `features/*.feature`. Today every scenario of `instant-fee.feature`
  has undefined steps, and `reqnroll.json` sets `missingOrPendingStepsOutcome` to `Error`, so
  `dotnet test tests/StejarTransfers.Specs` fails. This is expected until the steps are written.
- Feature files are linked, not copied: edit them in `features/`. Playwright: build first, needs `pwsh` 7.
  On Windows without `pwsh` 7, `playwright.ps1` does not finish in Windows PowerShell 5.1: run
  `tests\StejarTransfers.UnitTests\bin\Debug\net10.0\.playwright\node\win32_x64\node.exe tests\StejarTransfers.UnitTests\bin\Debug\net10.0\.playwright\package\cli.js install chromium`.
- The service reads `RATES_BASE_URL`, `RATES_API_KEY` and `RATES_TIMEOUT_SECONDS` from the
  environment (ASP.NET Core configuration). Without `RATES_BASE_URL` it uses `FixedRateClient`
  (rate 19.8765, no network). The `.env` file is not loaded automatically.

## Coding conventions

- Style in `.editorconfig`. File-scoped namespaces that follow the projects and folders
  (`StejarTransfers.Api.Contracts`, `StejarTransfers.Domain`, ...). `Program.cs` uses top-level
  statements and ends with `public partial class Program` for the tests.
- PascalCase for types, members and constants; camelCase for locals and parameters; `_camelCase`
  for private fields; interfaces start with `I`; async methods end with `Async` (not test methods).
- Records for value objects (`FeeRequest`, `Fee`, `ExchangeRate`, HTTP bodies). Classes are
  `sealed` (except `Program`); dependencies come through primary constructors.
- Braces on every `if`; `using` directives outside the namespace, `System` first.
- Money: always `decimal`, never `float` or `double`. Literals with the `m` suffix (`0.005m`).
  Round to 2 decimals with `Math.Round(value, 2, MidpointRounding.X)`, always with an explicit mode.
  Rounding rules come from `docs/fee-rules.md`. Fees are in MDL.
- JSON names come from `[JsonPropertyName]` (`customer_segment`). A `decimal` is written as a
  JSON number with its 2 decimals (`"fee":10.00`).
- Configuration only through `IConfiguration` (environment variables), read by `RatesSettings`.

## Test conventions

- Unit tests in `tests/StejarTransfers.UnitTests/`, one class per topic named `<Topic>Tests`,
  methods `Subject_ExpectedBehaviour` (`StandardMdlFee_HasAMinimumOf5`). Arrange, act, assert.
- `[InlineData]` amounts are strings parsed with `Money.Of`: an attribute cannot hold a `decimal`.
- Domain tests build `new FeeCalculator(new FixedRateClient(TestRates.Default))` (rate 20.0000).
  API tests use `IClassFixture<FeeServiceFactory>`: the service in memory with the same rate.
- No network call in tests: `FixedRateClient` or a fake (`Support/RecordingRateClient.cs`).
- BDD: features in `features/` at the root, step definitions in `tests/StejarTransfers.Specs/StepDefinitions/`.
  A step class gets `FeeService` (`Support/FeeService.cs`) through its constructor, new for each
  scenario: `Client` (HttpClient of the service in memory), `RateClient` (`FixedRateClient`, 19.8765).

## Architecture boundaries

- Project references: `Api -> Domain -> Clients` (and `Api -> Clients`). Never the other way.
- `Api` validates the body (422 `{"error": ...}` if invalid), maps it to the domain and back,
  and maps errors: `UnsupportedTransferException` to 422 `{"error": ...}`,
  `ExchangeRateException` to 502 `{"error": "exchange rate unavailable"}`.
- `Domain` holds the fee rules. It has no HTTP: no ASP.NET Core, no `HttpClient`. It depends on
  the `IExchangeRateClient` interface, not on a concrete client.
- `Clients` talks to third parties. `AddExchangeRateClient` (`ServiceCollectionExtensions.cs`)
  registers `HttpExchangeRateClient` when `RATES_BASE_URL` is set, otherwise `FixedRateClient`.

## Not implemented yet

- Instant transfer fee: `"type": "instant"` returns HTTP 422 `{"error":"instant transfers not supported yet"}`.
  Its acceptance criteria are in `features/instant-fee.feature`; no step definition exists yet.
- No Pact or Playwright test yet (packages declared). No agent code in `privacy-agent/`.

## Do not edit without asking

`.github/workflows/`, `.env` (fake values, committed on purpose), `privacy-agent/requests/`.

## Definition of done

- `dotnet build` with no new warning.
- Unit tests green: `dotnet test tests/StejarTransfers.UnitTests`.
- BDD green for the features in scope: `dotnet test tests/StejarTransfers.Specs`, no undefined step;
  a scenario left red is named in the commit message, with its reason.
- `dotnet format --verify-no-changes` clean.
- New behaviour comes with tests; money stays `decimal`; project boundaries respected.
