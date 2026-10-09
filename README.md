# stejar-transfers-dotnet

Training material. Invented bank. Stejar Bank SA, its customers and its figures are invented.

Transfer-fee service of Stejar Bank: `POST /fees` returns the fee of a transfer in MDL,
`GET /health` returns `{"status":"ok"}`. .NET 10, ASP.NET Core minimal API.

## Getting started

Windows 11: use PowerShell. macOS: use Terminal. Run every command from the repository folder.

```
git clone https://github.com/AlexisAIchangeAgent/stejar-transfers-dotnet.git
cd stejar-transfers-dotnet
```

Or download the ZIP of the latest release on GitHub (Releases, then Source code (zip)), unzip it,
and open a terminal in the folder. Step 3 then creates the git repository: the training commits
in it.

| Step | Windows | macOS |
|---|---|---|
| 1. Check the tools (changes nothing) | `powershell -ExecutionPolicy Bypass -File .\check.ps1` | `bash check.sh` |
| 2. Only if a line says KO: install the missing tools, open a new terminal, run step 1 again | `powershell -ExecutionPolicy Bypass -File .\install-tools.ps1` | `bash install-tools.sh` |
| 3. Install the dependencies of the project (network, a few minutes) | `powershell -ExecutionPolicy Bypass -File .\setup.ps1` | `bash setup.sh` |
| 4. Start Claude Code | `claude` | `claude` |

Step 3 ends with "Ready for cycle 1". If your IT team installs the tools for you, give them
`INSTALL-TOOLS.md`.

## Before the training

Run these once, with network access. During the training, Claude Code works without network.

`setup.ps1` (Windows) and `setup.sh` (macOS) run these steps for you, then the unit tests: see
Getting started above. The commands below do the same by hand.

```
dotnet build StejarTransfers.sln
pwsh tests/StejarTransfers.UnitTests/bin/Debug/net10.0/playwright.ps1 install chromium
python3 -m venv privacy-agent/.venv
privacy-agent/.venv/bin/python -m pip install -r privacy-agent/requirements.txt
```

On Windows, the last line is `privacy-agent\.venv\Scripts\python -m pip install -r privacy-agent\requirements.txt`.
On Windows without PowerShell 7, use the Playwright command of the "Test" section instead of the `pwsh` line.

## Build

Needs the .NET 10 SDK. The commands are the same on Windows and macOS, run from the repository root.

```
dotnet restore
dotnet build
```

`StejarTransfers.sln` opens in Rider and Visual Studio.

## Run

```
dotnet run --project src/StejarTransfers.Api -- --urls http://localhost:8000
```

Then send the requests of `src/StejarTransfers.Api/StejarTransfers.Api.http` from Rider or
Visual Studio, or with curl (macOS):

```
curl -X POST localhost:8000/fees -H "Content-Type: application/json" -d '{"amount": 2000.00, "currency": "MDL", "type": "standard", "customer_segment": "retail"}'
```

or with PowerShell (Windows):

```
Invoke-RestMethod -Method Post -Uri http://localhost:8000/fees -ContentType application/json -Body '{"amount": 2000.00, "currency": "MDL", "type": "standard", "customer_segment": "retail"}'
```

Without `RATES_BASE_URL` in the environment, the service uses a fixed EUR to MDL rate
(19.8765) and makes no network call.

## Test

```
dotnet test tests/StejarTransfers.UnitTests     # unit tests
dotnet test tests/StejarTransfers.Specs         # BDD features (features/), instant fee steps not written yet
dotnet format --verify-no-changes               # lint
```

Playwright browser, needed for the Playwright tests of the training. Build first, then run
(needs PowerShell 7, `pwsh`, on Windows and macOS):

```
dotnet build
pwsh tests/StejarTransfers.UnitTests/bin/Debug/net10.0/playwright.ps1 install chromium
```

On Windows without PowerShell 7, `playwright.ps1` does not finish in Windows PowerShell 5.1. Run the
Playwright driver directly instead (same result, works in `powershell` and `cmd`):

```
tests\StejarTransfers.UnitTests\bin\Debug\net10.0\.playwright\node\win32_x64\node.exe tests\StejarTransfers.UnitTests\bin\Debug\net10.0\.playwright\package\cli.js install chromium
```

## Folder map

| Folder | Content |
|---|---|
| `src/StejarTransfers.Api/` | the HTTP service: endpoints, request and response bodies, configuration |
| `src/StejarTransfers.Domain/` | the fee rules |
| `src/StejarTransfers.Clients/` | the exchange-rate client (third-party API) |
| `tests/StejarTransfers.UnitTests/` | xUnit unit tests |
| `tests/StejarTransfers.Specs/` | Reqnroll project that runs `features/` |
| `features/` | Gherkin features, source of truth of the acceptance criteria |
| `crm/` | static page of the internal CRM |
| `incidents/` | incident tickets |
| `docs/` | service documentation, published tariff, `adr/` |
| `privacy-agent/` | GDPR intake policy and synthetic requests |
| `docs/pipeline/` | CI pipeline, not active yet: cycle 3 copies it to `.github/workflows/` |

See `CLAUDE.md` for the conventions.
