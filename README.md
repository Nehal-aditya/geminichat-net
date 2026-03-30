<div align="center">

# Gemini Chat

**A cross-platform AI chatbot desktop app built with Avalonia UI and C#, powered by Google Gemini.**

[![Release](https://img.shields.io/github/v/release/Nehal-aditya/geminichat-net?style=flat-square&color=2563EB&label=latest)](https://github.com/Nehal-aditya/geminichat-net/releases/latest)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Avalonia](https://img.shields.io/badge/Avalonia-11.2-8B5CF6?style=flat-square)](https://avaloniaui.net)
[![License](https://img.shields.io/badge/license-MIT-10B981?style=flat-square)](LICENSE)
[![Windows](https://img.shields.io/badge/Windows-11-0078D4?style=flat-square&logo=windows&logoColor=white)](https://github.com/Nehal-aditya/geminichat-net/releases/latest)
[![Linux](https://img.shields.io/badge/Linux-x64-E95420?style=flat-square&logo=linux&logoColor=white)](https://github.com/Nehal-aditya/geminichat-net/releases/latest)
[![macOS](https://img.shields.io/badge/macOS-12+-000000?style=flat-square&logo=apple&logoColor=white)](https://github.com/Nehal-aditya/geminichat-net/releases/latest)

<br/>

*Streaming responses · Persistent chat history · 6 Gemini models · Dark UI*

</div>

---

## Table of Contents

- [Features](#features)
- [Supported Models](#supported-models)
- [Installation](#installation)
  - [Windows](#windows)
  - [Linux](#linux)
  - [macOS](#macos)
- [Getting an API Key](#getting-an-api-key)
- [Building from Source](#building-from-source)
- [Releasing a New Version](#releasing-a-new-version)
- [CI/CD Pipeline](#cicd-pipeline)
- [Project Structure](#project-structure)
- [Architecture Overview](#architecture-overview)
- [Configuration & Storage](#configuration--storage)
- [Dependencies](#dependencies)
- [Contributing](#contributing)
- [License](#license)

---

## Features

| Feature | Details |
|---|---|
| **6 Gemini models** | Switch models per-session from the top bar |
| **Streaming responses** | Watch the AI type in real time with live token rendering |
| **Persistent chat history** | Sessions survive app restarts, saved locally as JSON |
| **Multiple sessions** | Sidebar with named chats, auto-titled from first message |
| **Stop generation** | Cancel mid-stream at any time with one click |
| **Generation config** | Tune temperature, max output tokens, and system prompt |
| **API key validation** | Key is tested against the live `/v1beta/models` endpoint before saving |
| **Keyboard shortcuts** | Enter to send · Shift+Enter for newline |
| **Cross-platform** | Native binaries for Windows, Linux, and macOS |
| **Debug symbols** | Portable `.pdb` zips published alongside every release |
| **Pre-release channels** | Alpha, Beta, RC, and Preview releases with channel banners |
| **No telemetry** | Everything stays on your machine |

---

## Supported Models

| Model | API Model ID | Series | Best For |
|---|---|---|---|
| Gemini 2.5 Flash | `gemini-2.5-flash` | 2.5 | Fast everyday use |
| Gemini 2.5 Pro | `gemini-2.5-pro` | 2.5 | Deep reasoning & analysis |
| Gemini 2.5 Flash Lite | `gemini-2.5-flash-lite` | 2.5 | High-volume, cost-efficient |
| Gemini 3 Flash | `gemini-3-flash-preview` | 3 | Frontier-class speed |
| Gemini 3.1 Pro | `gemini-3.1-pro-preview` | 3.1 | State-of-the-art intelligence |
| Gemini 3.1 Flash Lite | `gemini-3.1-flash-lite-preview` | 3.1 | Ultra-fast & lightweight |

> Model IDs are defined in `Models/Models.cs` and can be updated as Google releases new versions.

---

## Installation

### Windows

#### MSI Installer (recommended)

1. Go to [**Releases**](https://github.com/Nehal-aditya/geminichat-net/releases/latest)
2. Download the `.msi` for your architecture:

   | File | Architecture |
   |---|---|
   | `GeminiChat-x64.msi` | Most Windows PCs (Intel / AMD) |
   | `GeminiChat-arm64.msi` | Snapdragon X, Copilot+ PCs, Surface Pro X |

3. Run the installer — no administrator rights required
4. Shortcuts are created on the **Desktop** and **Start Menu**
5. App installs to `%LocalAppData%\GeminiChat\`

#### Portable EXE

Download `GeminiChat-x64.exe` or `GeminiChat-arm64.exe` and double-click — no installation needed. Settings are still saved to `%AppData%\GeminiChat\`.

---

### Linux

> **Supported:** x86-64 only. ARM64 is not currently supported on Linux.

#### Debian / Ubuntu / Mint — `.deb`

```bash
# Download
wget https://github.com/Nehal-aditya/geminichat-net/releases/latest/download/GeminiChat-x64.deb

# Install
sudo dpkg -i GeminiChat-x64.deb

# Launch
geminichat
```

If you get dependency errors after `dpkg -i`:
```bash
sudo apt-get install -f
```

#### Fedora / RHEL / openSUSE — `.rpm`

```bash
# Download
wget https://github.com/Nehal-aditya/geminichat-net/releases/latest/download/GeminiChat-x64.rpm

# Install (Fedora / RHEL)
sudo rpm -i GeminiChat-x64.rpm

# Install (openSUSE)
sudo zypper install GeminiChat-x64.rpm
```

#### After installing on Linux

- Binary is at `/opt/geminichat/GeminiChat`
- Symlink is at `/usr/local/bin/geminichat`
- `.desktop` entry appears in your application menu automatically
- Settings and history are saved to `~/.config/GeminiChat/` (or `$XDG_CONFIG_HOME/GeminiChat/`)

---

### macOS

> Requires macOS 12 (Monterey) or later. Available for Intel and Apple Silicon.

1. Go to [**Releases**](https://github.com/Nehal-aditya/geminichat-net/releases/latest)
2. Download the `.dmg` for your Mac:

   | File | Architecture |
   |---|---|
   | `GeminiChat-macos-x64.dmg` | Intel Macs |
   | `GeminiChat-macos-arm64.dmg` | Apple Silicon (M1, M2, M3, M4) |

3. Open the `.dmg` and drag **GeminiChat** to your Applications folder
4. Launch from Finder, Spotlight, or Launchpad

**If macOS blocks the app** ("unidentified developer"):
```
Right-click GeminiChat.app → Open → Open
```
Or via Terminal:
```bash
xattr -dr com.apple.quarantine /Applications/GeminiChat.app
```

> The app is not code-signed or notarized. Signing support is available in the workflow — see [CI/CD Pipeline](#cicd-pipeline).

---

## Getting an API Key

1. Visit [**aistudio.google.com/apikey**](https://aistudio.google.com/apikey)
2. Sign in with your Google account
3. Click **Create API Key**
4. Copy the key (it starts with `AIzaSy…`)
5. Paste it into the **API Key** banner when Gemini Chat launches
6. Click **Validate & Save** — the key is tested live before being stored

> **Free tier:** Google's free quota covers Gemini 2.5 Flash generously. No billing setup required to get started.

---

## Building from Source

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later (8.0, 9.0, or 10.0)
- Git

### Clone and run

```bash
git clone https://github.com/Nehal-aditya/geminichat-net.git
cd geminichat-net
dotnet run
```

### Publish self-contained binaries

#### Windows

```bash
# x64 — portable single EXE
dotnet publish GeminiChat.csproj \
  -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true \
  -p:EnableCompressionInSingleFile=true \
  -o out/win-x64

# ARM64
dotnet publish GeminiChat.csproj \
  -c Release -r win-arm64 --self-contained true \
  -p:PublishSingleFile=true \
  -p:EnableCompressionInSingleFile=true \
  -o out/win-arm64
```

#### Linux

```bash
# x64
dotnet publish GeminiChat.csproj \
  -c Release -r linux-x64 --self-contained true \
  -p:PublishSingleFile=true \
  -o out/linux-x64
```

#### macOS

```bash
# Intel
dotnet publish GeminiChat.csproj \
  -c Release -r osx-x64 --self-contained true \
  -p:PublishSingleFile=true \
  -o out/osx-x64

# Apple Silicon
dotnet publish GeminiChat.csproj \
  -c Release -r osx-arm64 --self-contained true \
  -p:PublishSingleFile=true \
  -o out/osx-arm64
```

### Debug build with symbols

```bash
dotnet publish GeminiChat.csproj \
  -c Debug -r win-x64 --self-contained true \
  -p:DebugType=portable \
  -p:DebugSymbols=true \
  -o out/debug/win-x64
```

---

## Releasing a New Version

Releases are fully automated via GitHub Actions. The workflow builds and publishes all platform artifacts on every tagged push.

### Stable release

```bash
git tag v1.2.3
git push origin v1.2.3
```

### Pre-release channels

| Tag format | Channel | GitHub label |
|---|---|---|
| `v1.2.3` | Stable | ✅ Latest |
| `v1.2.3-alpha.1` | Alpha | ⚠️ Pre-release |
| `v1.2.3-beta.2` | Beta | 🔶 Pre-release |
| `v1.2.3-rc.1` | Release Candidate | 🔷 Pre-release |
| `v1.2.3-preview.1` | Preview | 🔵 Pre-release |

Pre-releases are never marked as "Latest" on GitHub. Stable and pre-release MSI installers use different upgrade codes so they can coexist on the same machine.

### Manual trigger

Go to **Actions → Release → Run workflow** and fill in:
- **Version** — e.g. `1.2.3` or `1.2.3-beta.1`
- **Release channel** — stable / alpha / beta / rc / preview
- **Draft** — publish as draft for review before going live

---

## CI/CD Pipeline

The release workflow (`.github/workflows/release.yml`) runs 6 parallel jobs:

```
version
  ├── Windows EXE    (windows-latest × x64, arm64)
  ├── Windows MSI    (windows-latest × x64, arm64)   WiX Toolset v5.0.2
  ├── Linux          (ubuntu-latest  × x64)           .deb + .rpm
  ├── macOS DMG      (macos-latest   × x64, arm64)
  └── Release        collects all artifacts → GitHub Release
```

Every build job also runs a separate debug publish and zips the `.pdb` files as a release asset.

### Artifacts per release

| Platform | Files |
|---|---|
| Windows x64 | `GeminiChat-x64.exe` · `GeminiChat-x64.msi` · `GeminiChat-x64-win.pdb.zip` |
| Windows ARM64 | `GeminiChat-arm64.exe` · `GeminiChat-arm64.msi` · `GeminiChat-arm64-win.pdb.zip` |
| Linux x64 | `GeminiChat-x64.deb` · `GeminiChat-x64.rpm` · `GeminiChat-x64-linux.pdb.zip` |
| macOS Intel | `GeminiChat-macos-x64.dmg` · `GeminiChat-macos-x64.pdb.zip` |
| macOS Apple Silicon | `GeminiChat-macos-arm64.dmg` · `GeminiChat-macos-arm64.pdb.zip` |

### Code signing (optional)

The macOS DMG job includes commented-out signing and notarization steps. To enable, add these secrets to your repository:

| Secret | Description |
|---|---|
| `MACOS_CERT_BASE64` | Base64-encoded `.p12` Developer ID certificate |
| `MACOS_CERT_PASSWORD` | Certificate password |
| `MACOS_NOTARIZE_APPLE_ID` | Apple ID used for notarization |
| `MACOS_NOTARIZE_PASSWORD` | App-specific password for that Apple ID |
| `MACOS_TEAM_ID` | Your Apple Developer Team ID |

Then uncomment the `Import signing certificate`, `Sign .app bundle`, and `Notarize .app` steps in the workflow.

---

## Project Structure

```
geminichat-net/
├── .github/
│   ├── workflows/
│   │   └── release.yml          # Cross-platform CI/CD pipeline
│   └── supported-models.md      # Model table injected into release notes
│
├── Assets/
│   ├── app.ico                  # Windows icon
│   └── app.png                  # PNG icon (Linux .desktop + macOS .icns)
│
├── Converters/
│   └── Converters.cs            # IValueConverter implementations for AXAML bindings
│
├── Models/
│   └── Models.cs                # ChatMessage, ChatSession, GeminiModel, GenerationConfig
│
├── Services/
│   ├── GeminiService.cs         # HTTP client, SSE streaming, API key validation
│   └── SettingsService.cs       # JSON persistence — settings + chat history
│
├── ViewModels/
│   └── MainViewModel.cs         # All commands, UI state, and business logic (MVVM)
│
├── Views/
│   ├── MainWindow.axaml         # Complete UI layout — sidebar, chat, settings panel
│   └── MainWindow.axaml.cs      # Code-behind — Enter key handler
│
├── App.axaml                    # Application entry point + Fluent theme
├── App.axaml.cs                 # Application lifecycle
├── Program.cs                   # Main entry point
├── GeminiChat.csproj            # Project file (.NET 8, WinExe output type)
└── app.manifest                 # Windows application manifest
```

---

## Architecture Overview

The app follows the **MVVM** pattern using `CommunityToolkit.Mvvm` source generators.

```
┌─────────────────────────────────────────────────────────┐
│  Views (AXAML)                                          │
│  MainWindow.axaml — compiled bindings to MainViewModel  │
└───────────────────────┬─────────────────────────────────┘
                        │ DataContext / RelayCommand
┌───────────────────────▼─────────────────────────────────┐
│  ViewModels                                             │
│  MainViewModel — ObservableObject, [RelayCommand]       │
│  Owns: Sessions, SelectedModel, ApiKey, IsLoading …     │
└──────────┬────────────────────────┬─────────────────────┘
           │                        │
┌──────────▼──────────┐  ┌──────────▼──────────────────┐
│  GeminiService      │  │  SettingsService             │
│  · GenerateAsync()  │  │  · Save() / Load()           │
│  · StreamAsync()    │  │  · SaveHistory()             │
│  · ValidateAsync()  │  │  · LoadHistory()             │
│  HttpClient + SSE   │  │  JSON → AppData              │
└─────────────────────┘  └──────────────────────────────┘
           │
┌──────────▼──────────────────────────────┐
│  Google Gemini REST API                 │
│  POST /v1beta/models/{model}:generateContent
│  POST /v1beta/models/{model}:streamGenerateContent
└─────────────────────────────────────────┘
```

### Key design decisions

- **`ObservableCollection<ChatMessage>`** on `ChatSession.Messages` — the `ItemsControl` in AXAML binds directly to this, so new messages render instantly without any manual refresh calls.
- **SSE streaming** — `GeminiService.StreamAsync` uses `HttpCompletionOption.ResponseHeadersRead` and reads `data: {...}` lines directly from the response stream, yielding chunks via `IAsyncEnumerable<string>`.
- **API key validation** — uses the `GET /v1beta/models` listing endpoint rather than a generate call, so validation costs zero tokens.
- **Single-file publish** — all platform binaries are published with `PublishSingleFile=true` so users get one executable with no runtime dependency.

---

## Configuration & Storage

All data is stored locally. Nothing is ever sent anywhere except directly to the Google Gemini API.

### Windows

| File | Contents |
|---|---|
| `%AppData%\GeminiChat\settings.json` | API key, default model, temperature, max tokens, system prompt |
| `%AppData%\GeminiChat\history.json` | All chat sessions and messages |

### Linux

| File | Contents |
|---|---|
| `~/.config/GeminiChat/settings.json` | Settings |
| `~/.config/GeminiChat/history.json` | Chat history |

### macOS

| File | Contents |
|---|---|
| `~/Library/Application Support/GeminiChat/settings.json` | Settings |
| `~/Library/Application Support/GeminiChat/history.json` | Chat history |

### Settings schema

```json
{
  "ApiKey": "AIzaSy...",
  "DefaultModelId": "gemini-2.5-flash",
  "Temperature": 0.9,
  "MaxOutputTokens": 8192,
  "EnableStreaming": true,
  "SystemPrompt": "You are a helpful, harmless, and honest AI assistant."
}
```

To reset the app completely, delete the `GeminiChat` folder from the appropriate location above.

---

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| [Avalonia](https://avaloniaui.net) | 11.2.3 | Cross-platform XAML UI framework |
| Avalonia.Desktop | 11.2.3 | Desktop windowing runtime |
| Avalonia.Themes.Fluent | 11.2.3 | Fluent design theme |
| Avalonia.ReactiveUI | 11.2.3 | Reactive MVVM bindings |
| Avalonia.Fonts.Inter | 11.2.3 | Inter font embedding |
| [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) | 8.3.2 | Source-generated MVVM (`[ObservableProperty]`, `[RelayCommand]`) |
| System.Text.Json | 9.0.0 | JSON serialization for API + persistence |
| ReactiveUI | 20.1.1 | Reactive extensions for Avalonia |

---

## Contributing

Contributions are welcome. Here's how to get started:

1. Fork the repository
2. Create a feature branch — `git checkout -b feature/my-feature`
3. Make your changes
4. Run the app locally — `dotnet run`
5. Commit with a descriptive message
6. Open a Pull Request

### Adding a new Gemini model

Edit `Models/Models.cs`:

```csharp
public static readonly GeminiModel MyNewModel = new()
{
    Id          = "gemini-x.x-modelname",
    Name        = "Gemini X.X Name",
    Badge       = "X.X",
    Desc        = "Short description",
    MaxOut      = 65536,
    Series      = "X.X"
};

// Then add to:
public static readonly List<GeminiModel> All = new()
{
    Flash25, Pro25, FlashLite25, Flash3, Pro31, FlashLite31, MyNewModel
};
```

Also update `.github/supported-models.md` so the release notes stay in sync.

### Reporting issues

Please include:
- Operating system and version
- App version (shown in the title bar)
- Steps to reproduce
- Any error messages from the status bar

---

## License

MIT License — free to use, modify, and distribute.

See [LICENSE](LICENSE) for the full text.

---

<div align="center">

Built with [Avalonia UI](https://avaloniaui.net) · Powered by [Google Gemini](https://ai.google.dev)

</div>