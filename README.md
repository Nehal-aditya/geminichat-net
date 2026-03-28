# 🤖 Gemini Chat — Avalonia UI Desktop App

A beautiful, feature-rich AI chatbot desktop application built with **Avalonia UI** and **C#**, powered by **Google Gemini** models.

---

## ✨ Features

- **5 Gemini Models** supported:
  - `gemini-2.5-flash-preview-05-20` — Gemini 2.5 Flash
  - `gemini-2.5-pro-preview-05-06` — Gemini 2.5 Pro
  - `gemini-3.0-flash` — Gemini 3 Flash
  - `gemini-3.1-flash-lite` — Gemini 3.1 Flash Lite
  - `gemini-3.1-pro` — Gemini 3.1 Pro

- **Streaming responses** — watch AI type in real time
- **Multiple chat sessions** with auto-titling
- **Persistent settings** (API key, preferences saved to disk)
- **Generation config**: Temperature, Top-P, Top-K, Max Tokens
- **Custom system prompt** support
- **Stop generation** mid-stream
- **Dark mode** UI with deep navy/slate theme
- **Cross-platform**: Windows, macOS, Linux

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A [Google Gemini API key](https://ai.google.dev) (free tier available)

### Run
```bash
cd GeminiChat
dotnet run
```

### Build (Release)
```bash
dotnet publish -c Release -r win-x64 --self-contained
# or for macOS:
dotnet publish -c Release -r osx-x64 --self-contained
# or Linux:
dotnet publish -c Release -r linux-x64 --self-contained
```

---

## 🔑 API Key Setup

1. Visit [https://ai.google.dev](https://ai.google.dev)
2. Create a free API key
3. On first launch, paste your key in the **API Key bar** at the top
4. Click **Validate & Save** — the key is stored locally in `AppData/GeminiChat/settings.json`

---

## 🎛 UI Overview

| Area | Description |
|---|---|
| **Left Sidebar** | Chat session history, New Chat button |
| **Top Bar** | Model selector, sidebar toggle, clear button |
| **Chat Area** | Message bubbles (user right, AI left) |
| **Input Bar** | Multi-line input, Send (➤) and Stop (■) buttons |
| **Settings Panel** | API key, model, temperature, streaming, system prompt |

---

## 🏗 Project Structure

```
GeminiChat/
├── Models/
│   └── Models.cs           # ChatMessage, ChatSession, GeminiModel, GenerationConfig
├── Services/
│   ├── GeminiService.cs    # HTTP client, streaming, API calls
│   └── SettingsService.cs  # Persistent settings (JSON)
├── ViewModels/
│   └── MainViewModel.cs    # All app logic, commands, state
├── Views/
│   ├── MainWindow.axaml    # Full UI layout
│   └── MainWindow.axaml.cs # Code-behind (Enter key handler)
├── Converters/
│   └── Converters.cs       # IValueConverter implementations
├── App.axaml               # Application entry + theme
├── Program.cs              # Main entry point
└── GeminiChat.csproj       # Project file
```

---

## ⚙️ Configuration

Settings are saved to `%AppData%/GeminiChat/settings.json` (Windows) or `~/.config/GeminiChat/settings.json` (Linux/macOS).

```json
{
  "ApiKey": "AIzaSy...",
  "DefaultModelId": "gemini-2.5-flash-preview-05-20",
  "Temperature": 0.9,
  "TopP": 1.0,
  "TopK": 40,
  "MaxOutputTokens": 8192,
  "EnableStreaming": true,
  "SystemPrompt": "You are a helpful AI assistant.",
  "ShowTokenCount": true
}
```

---

## 📦 Dependencies

| Package | Version | Purpose |
|---|---|---|
| Avalonia | 11.2.3 | Cross-platform UI framework |
| Avalonia.Desktop | 11.2.3 | Desktop runtime |
| Avalonia.Themes.Fluent | 11.2.3 | Fluent design theme |
| Avalonia.ReactiveUI | 11.2.3 | Reactive bindings |
| CommunityToolkit.Mvvm | 8.3.2 | MVVM source generators |
| System.Text.Json | 9.0.0 | JSON serialization |

---

## 🔧 Extending

**Add a new model:**
```csharp
// In Models/Models.cs
public static readonly GeminiModel MyNewModel = new()
{
    Id = "gemini-x.x-xxx",
    DisplayName = "Gemini X.X Name",
    Description = "Description here",
    Badge = "X.X",
    MaxTokens = 8192
};
// Then add to the All list
```

**Change the theme colors:**  
Edit the `Window.Styles` section in `Views/MainWindow.axaml` — all colors use hex literals that can be swapped globally.

---

## 📄 License

MIT — free to use, modify, and distribute.
