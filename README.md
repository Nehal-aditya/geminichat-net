# Gemini Chat

> A beautiful, fast desktop AI chatbot built with **Avalonia UI** and **C# (.NET 8)**, powered by **Google Gemini** models — with persistent history, streaming responses, and a polished dark UI.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Avalonia](https://img.shields.io/badge/Avalonia-11.2-purple?logo=dotnet)
![Platform](https://img.shields.io/badge/Platform-Windows%2011-0078D4?logo=windows)
![License](https://img.shields.io/badge/License-MIT-green)

---

## Features

- **6 Gemini models** selectable per-session from the top bar
- **Streaming responses** — watch the AI type in real time
- **Persistent chat history** — sessions survive restarts, saved to `%AppData%\GeminiChat\`
- **Multiple sessions** — sidebar with named chats, auto-titled from first message
- **Stop generation** mid-stream with one click
- **Configurable generation** — temperature, max output tokens, system prompt
- **API key validation** — tested against the live models endpoint before saving
- **Enter to send**, Shift+Enter for newline
- Deep navy dark theme

---

## Supported Models

| Display Name | API Model ID | Series |
|---|---|---|
| Gemini 2.5 Flash | `gemini-2.5-flash` | 2.5 |
| Gemini 2.5 Pro | `gemini-2.5-pro` | 2.5 |
| Gemini 2.5 Flash Lite | `gemini-2.5-flash-lite` | 2.5 |
| Gemini 3 Flash | `gemini-3-flash-preview` | 3 |
| Gemini 3.1 Pro | `gemini-3.1-pro-preview` | 3.1 |
| Gemini 3.1 Flash Lite | `gemini-3.1-flash-lite-preview` | 3.1 |

---

## Installation

### Option A — MSI Installer (recommended)

1. Go to the [**Releases**](../../releases) page
2. Download the `.msi` for your architecture:
   - `GeminiChat-x64.msi` — Windows 11 x86-64 (most PCs)
   - `GeminiChat-arm64.msi` — Windows 11 ARM64 (Surface Pro X, Copilot+ PCs)
3. Run the installer — creates shortcuts in Desktop and Start Menu
4. Launch **Gemini Chat** and paste your API key

### Option B — Portable EXE

Download `GeminiChat-x64.exe` or `GeminiChat-arm64.exe` from [Releases](../../releases) and run directly — no installation needed.

---

## Getting an API Key

1. Visit [https://aistudio.google.com/apikey](https://aistudio.google.com/apikey)
2. Sign in with your Google account
3. Click **Create API Key**
4. Copy the key (starts with `AIzaSy…`)
5. Paste it in the **API Key** field when Gemini Chat launches and click **Validate & Save**

> **Free tier**: Google provides a generous free quota for Gemini 2.5 Flash. No billing required to get started.

---

## Building from Source

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

### Run

```bash
git clone https://github.com/YOUR_USERNAME/GeminiChat.git
cd GeminiChat
dotnet run
```

### Publish self-contained EXE

```bash
# Windows x64
dotnet publish GeminiChat.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o out/win-x64

# Windows ARM64
dotnet publish GeminiChat.csproj -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true -o out/win-arm64
```

---

## Releasing a New Version

Releases are fully automated via GitHub Actions (`.github/workflows/release.yml`).

### Via Git tag (recommended)

```bash
git tag v1.0.0
git push origin v1.0.0
```

The workflow automatically builds:
- `GeminiChat-x64.exe` — portable, Windows x64
- `GeminiChat-arm64.exe` — portable, Windows ARM64
- `GeminiChat-x64.msi` — installer, Windows x64
- `GeminiChat-arm64.msi` — installer, Windows ARM64

...and creates a GitHub Release with all four files attached.

### Via GitHub UI

Go to **Actions → Release → Run workflow** and enter a version string (e.g. `1.0.0`).

---

## Project Structure

```
GeminiChat/
├── .github/
│   └── workflows/
│       └── release.yml          # Builds exe + msi, publishes to GitHub Releases
├── Models/
│   └── Models.cs                # ChatMessage, ChatSession, GeminiModel, GenerationConfig
├── Services/
│   ├── GeminiService.cs         # HTTP client, streaming SSE, API validation
│   └── SettingsService.cs       # JSON persistence for settings + chat history
├── ViewModels/
│   └── MainViewModel.cs         # All app logic and UI state (MVVM)
├── Views/
│   ├── MainWindow.axaml         # Full UI layout (AXAML)
│   └── MainWindow.axaml.cs      # Code-behind (Enter key handler)
├── Converters/
│   └── Converters.cs            # IValueConverter implementations
├── App.axaml                    # Application + Fluent theme
├── Program.cs                   # Entry point
└── GeminiChat.csproj            # Project file (.NET 8 / WinExe)
```

---

## Data & Privacy

- API key stored **locally only** at `%AppData%\GeminiChat\settings.json`
- Chat history stored **locally only** at `%AppData%\GeminiChat\history.json`
- No telemetry, no analytics, no cloud sync of any kind
- Messages go directly from your machine to the Google Gemini API

---

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| [Avalonia](https://avaloniaui.net) | 11.2.3 | Cross-platform UI framework |
| Avalonia.Desktop | 11.2.3 | Desktop runtime |
| Avalonia.Themes.Fluent | 11.2.3 | Fluent design system |
| Avalonia.ReactiveUI | 11.2.3 | Reactive bindings |
| CommunityToolkit.Mvvm | 8.3.2 | Source-generated MVVM |
| System.Text.Json | 9.0.0 | JSON serialization |

---

## License

MIT — free to use, modify, and distribute.