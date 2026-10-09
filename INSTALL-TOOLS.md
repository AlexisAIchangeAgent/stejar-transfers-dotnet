# Tools for the training: guide for your IT team

stejar-transfers-dotnet. Training material. Invented bank.

The participant needs these tools before the training, on Windows 10 (1809 or newer),
Windows 11 or macOS 13 or newer. `install-tools.ps1` (Windows, winget) and `install-tools.sh`
(macOS, Homebrew) install them automatically. This page is for a manual install, by the
participant or by the IT team.

## Tools

The minimum is what the project needs. Below the recommended version, the check shows a warning
but the training works.

| Tool | Minimum | Recommended | Windows (winget id) | macOS | Download |
|---|---|---|---|---|---|
| Git | any recent | latest | `Git.Git` | Command Line Tools: `xcode-select --install` (or `brew install git`) | https://git-scm.com/downloads |
| Claude Code | latest | latest | official installer, no admin: `irm https://claude.ai/install.ps1 \| iex` | official installer, no admin: `curl -fsSL https://claude.ai/install.sh \| bash` | https://docs.claude.com/en/docs/claude-code/setup |
| .NET SDK | 10.0.100 | 10 (latest patch) | `Microsoft.DotNet.SDK.10` | official script, no admin: `dotnet-install.sh --channel 10.0` | https://dotnet.microsoft.com/download/dotnet/10.0 |
| Python (the cycle 6 agent) | 3.10 | 3.12 or newer | `Python.Python.3.13` | `brew install python@3.13` | https://www.python.org/downloads/ |

Notes:

- .NET: the project targets `net10.0`, so the .NET 10 SDK is needed. An older SDK alone does not work.
- Python 3.10 or newer is only needed for the agent of cycle 6.
- Claude Code needs a Claude account with Claude Code access (Pro, Max, Team, Enterprise or Console).
  Run `claude` once before the training and log in.
- On Windows, Claude Code needs Git for Windows (it uses Git Bash).
- The command `python3` must work: the hookify plugin (cycle 3, optional prompt) runs its hooks
  with it, and without it blocks nothing. On Windows, a classic Python install (python.org, winget)
  gives `py` and `python` only: install Python from the Microsoft Store, or with the Python install
  manager of python.org, which both add `python3`.
- Git needs a name and an e-mail, because Claude Code commits during the training:
  `git config --global user.name "Your Name"` and `git config --global user.email "you@example.com"`.

## Administrator rights

- winget installs of Git, the JDK and the .NET SDK may show a Windows prompt for administrator rights.
  Python and Claude Code install for the current user, without administrator rights.
- On macOS, Homebrew and the JDK ask for the password of an administrator. Claude Code, Python and the
  .NET SDK (installed in `~/.dotnet`) do not.
- `setup.ps1` and `setup.sh` never need administrator rights: they only work inside the repository
  folder and the user profile.

## Network

The check script tests these addresses. If the computer is behind a proxy, allow them.

| Address | Used for |
|---|---|
| `api.anthropic.com` | Claude Code, during the training |
| `claude.ai` | Claude Code login and installer |
| `github.com` | plugin marketplace of Claude Code (superpowers), git clone |
| `api.nuget.org` | NuGet packages (setup) |
| `pypi.org` | Python packages (setup) |
| `files.pythonhosted.org` | Python packages (setup) |
| `playwright.azureedge.net` | Playwright browser (setup) |
| `dot.net`, `builds.dotnet.microsoft.com` | install of .NET on macOS by `install-tools.sh` |

On macOS, Homebrew also downloads from `github.com` and `ghcr.io`.

## Check

After the install, open a new terminal in the repository folder and run:

- Windows: `powershell -ExecutionPolicy Bypass -File .\check.ps1`
- macOS: `bash check.sh`

There must be no KO line. Then run `setup.ps1` (Windows) or `setup.sh` (macOS).
