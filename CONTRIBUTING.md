# Contributing to Gemini Chat

Thank you for your interest in contributing! This document covers everything you need to get started — from setting up your dev environment to submitting a pull request.

---

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Ways to Contribute](#ways-to-contribute)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Commit Messages](#commit-messages)
- [Pull Request Guidelines](#pull-request-guidelines)
- [Common Tasks](#common-tasks)
  - [Adding a Gemini Model](#adding-a-gemini-model)
  - [Adding a Setting](#adding-a-setting)
  - [Adding a Value Converter](#adding-a-value-converter)
  - [Modifying the UI](#modifying-the-ui)
- [Reporting Bugs](#reporting-bugs)
- [Suggesting Features](#suggesting-features)
- [Release Process](#release-process)

---

## Code of Conduct

Be respectful and constructive. Harassment, personal attacks, and dismissive language have no place here. If something feels off, open an issue or contact the maintainer directly.

---

## Ways to Contribute

You don't have to write code to contribute. Here's what's always helpful:

- **Bug reports** — a clear, reproducible report is extremely valuable
- **Feature suggestions** — open an issue and describe the use case
- **Documentation improvements** — typos, unclear wording, missing steps
- **UI/UX feedback** — screenshots, screen recordings, or written feedback
- **Code contributions** — bug fixes, new features, refactoring, performance

---

## Development Setup

### Prerequisites

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 8.0 or later | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Git | Any recent version | [git-scm.com](https://git-scm.com) |
| IDE | Any of the below | — |

**Recommended IDEs:**
- [JetBrains Rider](https://www.jetbrains.com/rider/) — best Avalonia support, full AXAML preview
- [Visual Studio 2022](https://visualstudio.microsoft.com/) — with the Avalonia extension
- [VS Code](https://code.visualstudio.com/) — with the C# Dev Kit and Avalonia extensions

### Clone and run

```bash
git clone https://github.com/Nehal-aditya/geminichat-net.git
cd geminichat-net
dotnet restore
dotnet run
```

The app will launch. On first run it will show the API key input banner — paste any key (or leave it for now) to explore the UI.

### Verify the build

```bash
dotnet build --configuration Release
```

No warnings should be emitted on a clean build. If you see AXAML binding warnings, those need to be fixed before a PR is merged.

---

## Project Structure

```
geminichat-net/
├── .github/
│   ├── workflows/
│   │   └── release.yml          # CI/CD — builds all platform artifacts
│   └── supported-models.md      # Model table injected into release notes
│
├── Assets/
│   ├── app.ico                  # Windows taskbar + MSI icon
│   └── app.png                  # Linux .desktop + macOS .icns source
│
├── Converters/
│   └── Converters.cs            # IValueConverter implementations for AXAML
│
├── Models/
│   └── Models.cs                # Data models — ChatMessage, ChatSession,
│                                #   GeminiModel, GenerationConfig
│
├── Services/
│   ├── GeminiService.cs         # Gemini REST API — generate + stream + validate
│   └── SettingsService.cs       # JSON persistence — settings + chat history
│
├── ViewModels/
│   └── MainViewModel.cs         # All UI state and commands (MVVM)
│
├── Views/
│   ├── MainWindow.axaml         # Full UI layout
│   └── MainWindow.axaml.cs      # Code-behind — keyboard handler
│
├── App.axaml                    # App entry + Fluent theme
├── App.axaml.cs
├── Program.cs
├── GeminiChat.csproj
└── app.manifest
```

### Layer responsibilities

| Layer | What it owns | What it must NOT do |
|---|---|---|
| **Models** | Pure data classes, no logic | Reference ViewModels or Services |
| **Services** | I/O — HTTP calls, file reads/writes | Reference ViewModels or Views |
| **ViewModels** | UI state, commands, orchestration | Directly manipulate UI controls |
| **Views** | Layout, styles, bindings | Contain business logic |
| **Converters** | AXAML type conversions | Have side effects |

---

## Development Workflow

1. **Fork** the repository on GitHub
2. **Clone** your fork locally
3. **Create a branch** from `main`:
   ```bash
   git checkout -b fix/broken-streaming
   # or
   git checkout -b feature/copy-message-button
   ```
4. **Make your changes** — keep commits focused and atomic
5. **Test** by running `dotnet run` and exercising the changed code paths
6. **Push** your branch:
   ```bash
   git push origin fix/broken-streaming
   ```
7. **Open a Pull Request** against `main` on the upstream repo

### Branch naming

| Type | Format | Example |
|---|---|---|
| Bug fix | `fix/short-description` | `fix/streaming-stops-early` |
| Feature | `feature/short-description` | `feature/copy-message-button` |
| Refactor | `refactor/short-description` | `refactor/extract-api-client` |
| Docs | `docs/short-description` | `docs/linux-install-guide` |
| Chore | `chore/short-description` | `chore/update-avalonia-11-3` |

---

## Coding Standards

### General

- **C# 12** language features are fine — the project targets .NET 8
- **Nullable reference types** are enabled — annotate everything correctly, no `!` suppressions without a comment explaining why
- **No unused `using` directives** — clean them up before committing
- **No magic strings** — model IDs, setting keys, and file paths belong in constants or the model definitions

### MVVM conventions

- All bindable properties use `[ObservableProperty]` from `CommunityToolkit.Mvvm`
- All commands use `[RelayCommand]` — never expose `ICommand` properties manually
- The ViewModel should never directly reference any `Control` type from Avalonia
- Compiled bindings (`x:DataType`) are required on all AXAML DataTemplates — loose bindings will cause AVLN warnings that are treated as errors

```csharp
// ✅ correct
[ObservableProperty]
private string _userInput = string.Empty;

[RelayCommand]
public async Task SendMessageAsync() { ... }

// ❌ avoid
public string UserInput
{
    get => _userInput;
    set { _userInput = value; OnPropertyChanged(); }
}
```

### Async

- All async methods must be `async Task` or `async Task<T>` — never `async void` except for event handlers
- Always pass `CancellationToken` through to inner async calls
- Use `await foreach` for `IAsyncEnumerable` — don't `.ToList()` a stream

### Services

- `GeminiService` is the only place that talks to the Gemini API — don't add HTTP calls elsewhere
- `SettingsService` is the only place that reads/writes `AppData` — don't add file I/O elsewhere
- Both services are instantiated once in `MainViewModel`'s constructor — no DI container is used (the app is simple enough not to need one)

### Formatting

The project does not enforce a specific formatter, but please follow the existing style:
- 4-space indentation
- Braces on their own line for methods and classes
- `var` when the type is obvious from the right side
- Descriptive names — avoid single-letter variables outside of LINQ lambdas

---

## Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) format:

```
<type>(<scope>): <short summary>

[optional body]

[optional footer]
```

### Types

| Type | When to use |
|---|---|
| `feat` | A new feature |
| `fix` | A bug fix |
| `docs` | Documentation only |
| `style` | Formatting, whitespace — no logic change |
| `refactor` | Code restructure — no feature or fix |
| `perf` | Performance improvement |
| `chore` | Dependency updates, build changes |
| `ci` | Changes to GitHub Actions workflows |

### Examples

```
feat(models): add Gemini 3.1 Flash Lite support

fix(streaming): stop duplicate chunks on reconnect

docs(readme): update Linux install instructions

ci(release): fix RPM arm64 build target

chore(deps): update Avalonia to 11.3.0
```

Keep the summary line under 72 characters. Use the body to explain *why*, not *what* — the diff shows what changed.

---

## Pull Request Guidelines

### Before opening a PR

- [ ] `dotnet build --configuration Release` passes with no warnings
- [ ] `dotnet run` launches and the changed feature works as expected
- [ ] No new hardcoded strings that should be constants
- [ ] No new `async void` methods (except event handlers)
- [ ] AXAML compiled bindings are correct (no AVLN binding warnings)
- [ ] If you changed model IDs, update `.github/supported-models.md`
- [ ] If you changed storage paths or schema, update the README

### PR description template

When you open a PR, fill in:

```
## What does this change?
[Brief description of the change]

## Why?
[The problem it solves or the feature it adds]

## How to test
[Step-by-step instructions to verify the change works]

## Screenshots (if UI change)
[Before / after screenshots or a screen recording]
```

### Review process

- A maintainer will review within a few days
- Feedback will be left as inline comments — address each one with either a code change or a reply explaining why you disagree
- Once approved, the maintainer will squash-merge your PR into `main`

---

## Common Tasks

### Adding a Gemini Model

1. Open `Models/Models.cs`
2. Add a new static field following the existing pattern:

```csharp
public static readonly GeminiModel MyNewModel = new()
{
    Id     = "gemini-x.x-model-id",   // exact API model string
    Name   = "Gemini X.X Model",      // display name in the UI
    Badge  = "X.X",                   // short label shown in the badge chip
    Desc   = "One-line description",  // shown in the settings panel
    MaxOut = 65536,                   // maximum output tokens
    Series = "X.X"                    // series group label
};
```

3. Add it to the `All` list:

```csharp
public static readonly List<GeminiModel> All = new()
{
    Flash25, Pro25, FlashLite25, Flash3, Pro31, FlashLite31, MyNewModel
};
```

4. Update `.github/supported-models.md`:

```markdown
| Gemini X.X Model | `gemini-x.x-model-id` |
```

5. Run the app and verify the model appears in the top bar dropdown and the settings panel model selector.

---

### Adding a Setting

Settings are persisted via `SettingsService`. To add a new one:

1. Add the property to `AppSettings` in `Services/SettingsService.cs`:

```csharp
public bool MyNewSetting { get; set; } = true; // always provide a default
```

2. Add a corresponding `[ObservableProperty]` to `MainViewModel`:

```csharp
[ObservableProperty]
private bool _myNewSetting = true;
```

3. Load it in `MainViewModel`'s constructor:

```csharp
MyNewSetting = s.MyNewSetting;
```

4. Save it in `SaveSettings()`:

```csharp
_svc.Settings.MyNewSetting = MyNewSetting;
```

5. Add a control for it in the settings panel in `Views/MainWindow.axaml`.

---

### Adding a Value Converter

All converters live in `Converters/Converters.cs`. Follow this pattern:

```csharp
public class MyConverter : IValueConverter
{
    public static readonly MyConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // return converted value
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
```

Register it as a resource in `MainWindow.axaml`:

```xml
<Window.Resources>
    <cv:MyConverter x:Key="MyConverter"/>
</Window.Resources>
```

Use it in a binding:

```xml
<TextBlock Text="{Binding SomeProperty, Converter={StaticResource MyConverter}}"/>
```

---

### Modifying the UI

The entire UI lives in `Views/MainWindow.axaml`. A few things to keep in mind:

- **Compiled bindings are required** — every `DataTemplate` needs `x:DataType`. Without it the project will emit AVLN2000 errors.
- **No `Width="Auto"` or `Height="Auto"`** on `Button` — Avalonia's XAML parser rejects the string `"Auto"` for numeric dimension properties. Just omit the attribute entirely (Auto is the default).
- **`PasswordChar` must be a literal character** — don't bind it to a boolean property. Use `PasswordChar="•"` as a static value.
- **Test on your platform** — layout and fonts can render differently on Windows, Linux (X11/Wayland), and macOS. If you're adding a significant UI change, note in your PR which platform(s) you tested on.

---

## Reporting Bugs

Open an issue and include:

1. **App version** — shown in the window title or check the [Releases](https://github.com/Nehal-aditya/geminichat-net/releases) page
2. **Operating system** — e.g. Windows 11 23H2, Ubuntu 24.04, macOS 15.1
3. **Steps to reproduce** — the exact sequence of actions that triggers the bug
4. **Expected behaviour** — what you expected to happen
5. **Actual behaviour** — what actually happened
6. **Error messages** — any text from the status bar, or a screenshot

If the app crashes silently, try running it from a terminal (`dotnet run` from the repo, or the binary directly) to capture any console output.

---

## Suggesting Features

Open an issue with the `enhancement` label and describe:

1. **The problem** — what are you trying to do that you can't do today?
2. **Your proposed solution** — how you'd like it to work
3. **Alternatives considered** — other approaches you thought about
4. **Scope** — is this a small tweak or a large addition?

Feature requests with a clear problem statement and a concrete proposal are much more likely to be implemented quickly.

---

## Release Process

Releases are handled by the maintainer. If you're a maintainer:

```bash
# stable
git tag v1.2.3
git push origin v1.2.3

# pre-release
git tag v1.2.3-beta.1
git push origin v1.2.3-beta.1
```

The GitHub Actions workflow handles everything from there — building all platform artifacts, generating release notes, and publishing the GitHub Release. See the [CI/CD Pipeline](README.md#cicd-pipeline) section of the README for the full details.

---

*Thanks for contributing — every improvement, no matter how small, makes Gemini Chat better for everyone.*
