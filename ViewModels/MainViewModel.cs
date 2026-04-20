using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeminiChat.Models;
using GeminiChat.Services;

namespace GeminiChat.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly GeminiService  _gemini;
        private readonly SettingsService _svc;
        private CancellationTokenSource? _cts;

        // ── UI State ──────────────────────────────────────────────────────────
        [ObservableProperty] private string      _userInput       = string.Empty;
        [ObservableProperty] private bool        _isLoading;
        [ObservableProperty] private bool        _isSidebarOpen   = true;
        [ObservableProperty] private bool        _isSettingsOpen;
        [ObservableProperty] private string      _statusMessage   = "Ready";
        [ObservableProperty] private ChatSession? _currentSession;
        [ObservableProperty] private GeminiModel _selectedModel   = GeminiModel.Flash25;

        // API key
        [ObservableProperty] private string _apiKey       = string.Empty;
        [ObservableProperty] private string _apiKeyStatus = "Not configured";
        [ObservableProperty] private bool   _apiKeyValid;
        [ObservableProperty] private bool   _isApiKeyPanelVisible = true;
        [ObservableProperty] private bool   _isValidating;

        // Generation settings
        [ObservableProperty] private float  _temperature    = 0.9f;
        [ObservableProperty] private int    _maxTokens      = 8192;
        [ObservableProperty] private bool   _enableStreaming = true;
        [ObservableProperty] private string _systemPrompt   = "You are a helpful, harmless, and honest AI assistant powered by Google Gemini.";

        // Theme
        [ObservableProperty] private bool _isDarkMode = true;

        public ObservableCollection<ChatSession>  Sessions        { get; } = new();
        public ObservableCollection<GeminiModel>  AvailableModels { get; } = new(GeminiModel.All);
        public List<IcebreakerTile>               IcebreakerTiles { get; } = IcebreakerTile.All;

        public MainViewModel()
        {
            _svc    = new SettingsService();
            _gemini = new GeminiService();

            var s = _svc.Settings;
            ApiKey         = s.ApiKey;
            Temperature    = s.Temperature;
            MaxTokens      = s.MaxOutputTokens;
            EnableStreaming = s.EnableStreaming;
            SystemPrompt   = s.SystemPrompt;
            IsDarkMode     = s.Theme != "Light";
            SelectedModel  = GeminiModel.All.FirstOrDefault(m => m.Id == s.DefaultModelId)
                             ?? GeminiModel.Flash25;

            ApplyTheme();

            // Validate stored key
            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                _gemini.SetApiKey(ApiKey);
                ApiKeyStatus          = "Configured ✓";
                ApiKeyValid           = true;
                IsApiKeyPanelVisible  = false;
            }

            // Load persisted history
            var history = _svc.LoadHistory();
            foreach (var session in history)
                Sessions.Add(session);

            if (Sessions.Count > 0)
                CurrentSession = Sessions[0];
            else
                NewSession();
        }

        private void ApplyTheme()
        {
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = IsDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
            }
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            ApplyTheme();
            _svc.Settings.Theme = value ? "Dark" : "Light";
            _svc.Save();
        }

        // ── Session Management ────────────────────────────────────────────────

        [RelayCommand]
        public void NewSession()
        {
            var session = new ChatSession
            {
                Title   = $"Chat {Sessions.Count + 1}",
                ModelId = SelectedModel.Id
            };
            Sessions.Insert(0, session);
            CurrentSession = session;
        }

        [RelayCommand]
        public async Task SendIcebreakerAsync(IcebreakerTile tile)
        {
            if (tile == null) return;
            UserInput = tile.Prompt;
            await SendMessageAsync();
        }

        [RelayCommand]
        public void SelectSession(ChatSession session)
            => CurrentSession = session;

        [RelayCommand]
        public void DeleteSession(ChatSession session)
        {
            Sessions.Remove(session);
            _svc.SaveHistory(Sessions);
            if (CurrentSession == session)
                CurrentSession = Sessions.Count > 0 ? Sessions[0] : null;
            if (CurrentSession == null)
                NewSession();
        }

        [RelayCommand]
        public void ClearCurrentChat()
        {
            if (CurrentSession == null) return;
            CurrentSession.Messages.Clear();
            _svc.SaveHistory(Sessions);
            StatusMessage = "Chat cleared.";
        }

        // ── Send Message ──────────────────────────────────────────────────────

        [RelayCommand]
        public async Task SendMessageAsync()
        {
            var text = UserInput.Trim();
            if (string.IsNullOrWhiteSpace(text) || IsLoading) return;

            if (!_gemini.HasApiKey)
            {
                IsApiKeyPanelVisible = true;
                StatusMessage = "Please set your Gemini API key first.";
                return;
            }

            if (CurrentSession == null) NewSession();

            // Add user message
            var userMsg = new ChatMessage
            {
                Role    = MessageRole.User,
                Content = text,
                Status  = MessageStatus.Sent
            };
            CurrentSession!.Messages.Add(userMsg);

            // Auto-title from first message
            if (CurrentSession.Messages.Count == 1)
                CurrentSession.Title = text.Length > 45 ? text[..45] + "…" : text;

            UserInput  = string.Empty;
            IsLoading  = true;
            StatusMessage = $"Generating with {SelectedModel.Name}…";

            _cts = new CancellationTokenSource();

            // Add assistant placeholder
            var reply = new ChatMessage
            {
                Role       = MessageRole.Assistant,
                Content    = string.Empty,
                Status     = MessageStatus.Streaming,
                IsStreaming = true
            };
            CurrentSession.Messages.Add(reply);

            var cfg = new GenerationConfig
            {
                Temperature     = Temperature,
                MaxOutputTokens = MaxTokens,
                EnableStreaming  = EnableStreaming
            };

            // Build context — exclude the empty placeholder
            var context = CurrentSession.Messages
                .Where(m => m != reply && !string.IsNullOrWhiteSpace(m.Content))
                .ToList();

            try
            {
                if (EnableStreaming)
                {
                    await foreach (var chunk in _gemini.StreamAsync(
                        SelectedModel.Id, context, cfg, SystemPrompt, _cts.Token))
                    {
                        reply.Content += chunk;
                    }
                }
                else
                {
                    reply.Content = await _gemini.GenerateAsync(
                        SelectedModel.Id, context, cfg, SystemPrompt, _cts.Token);
                }

                reply.Status     = MessageStatus.Sent;
                reply.IsStreaming = false;
                CurrentSession.UpdatedAt = DateTime.Now;
                StatusMessage = $"✓ {SelectedModel.Name}";
            }
            catch (OperationCanceledException)
            {
                reply.Content    += "\n\n*[Stopped]*";
                reply.Status      = MessageStatus.Sent;
                reply.IsStreaming = false;
                StatusMessage     = "Stopped.";
            }
            catch (GeminiApiException ex)
            {
                reply.Content     = $"❌ API Error {ex.StatusCode}: {ex.Message}";
                reply.Status      = MessageStatus.Error;
                reply.IsStreaming = false;
                StatusMessage     = $"Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                reply.Content     = $"❌ Error: {ex.Message}";
                reply.Status      = MessageStatus.Error;
                reply.IsStreaming = false;
                StatusMessage     = ex.Message;
            }
            finally
            {
                IsLoading = false;
                // Save history after every exchange
                _svc.SaveHistory(Sessions);
            }
        }

        [RelayCommand]
        public void StopGeneration() => _cts?.Cancel();

        // ── API Key ───────────────────────────────────────────────────────────

        [RelayCommand]
        public async Task SaveApiKeyAsync()
        {
            var key = ApiKey?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                ApiKeyStatus = "API key cannot be empty";
                return;
            }

            IsValidating  = true;
            ApiKeyStatus  = "Validating…";
            ApiKeyValid   = false;

            var (valid, msg) = await _gemini.ValidateAsync(key);

            IsValidating  = false;
            ApiKeyValid   = valid;
            ApiKeyStatus  = msg;

            if (valid)
            {
                _gemini.SetApiKey(key);
                IsApiKeyPanelVisible    = false;
                _svc.Settings.ApiKey   = key;
                _svc.Save();
                StatusMessage = "API key saved ✓";
            }
            else
            {
                StatusMessage = $"Key rejected: {msg}";
            }
        }

        // ── Settings ──────────────────────────────────────────────────────────

        [RelayCommand]
        public void ToggleSidebar()  => IsSidebarOpen  = !IsSidebarOpen;

        [RelayCommand]
        public void ToggleSettings() => IsSettingsOpen = !IsSettingsOpen;

        [RelayCommand]
        public void ShowApiKeyPanel()
        {
            IsApiKeyPanelVisible = true;
            IsSettingsOpen       = false;
        }

        [RelayCommand]
        public void SaveSettings()
        {
            var s = _svc.Settings;
            s.DefaultModelId  = SelectedModel.Id;
            s.Temperature     = Temperature;
            s.MaxOutputTokens = MaxTokens;
            s.EnableStreaming  = EnableStreaming;
            s.SystemPrompt    = SystemPrompt;
            s.Theme           = IsDarkMode ? "Dark" : "Light";
            _svc.Save();
            IsSettingsOpen = false;
            StatusMessage  = "Settings saved ✓";
        }

        partial void OnSelectedModelChanged(GeminiModel value)
        {
            if (CurrentSession != null) CurrentSession.ModelId = value.Id;
            MaxTokens = Math.Min(MaxTokens, value.MaxOut);
        }
    }
}
