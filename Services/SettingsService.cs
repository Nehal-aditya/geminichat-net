using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GeminiChat.Models;

namespace GeminiChat.Services
{
    public class AppSettings
    {
        public string ApiKey          { get; set; } = string.Empty;
        public string DefaultModelId  { get; set; } = GeminiModel.Flash25.Id;
        public float  Temperature     { get; set; } = 0.9f;
        public int    MaxOutputTokens { get; set; } = 8192;
        public bool   EnableStreaming  { get; set; } = true;
        public string SystemPrompt    { get; set; } = "You are a helpful, harmless, and honest AI assistant powered by Google Gemini.";
        public string Theme           { get; set; } = "Dark"; // "Light" or "Dark"
    }

    public class SessionSnapshot
    {
        public string                Id        { get; set; } = string.Empty;
        public string                Title     { get; set; } = string.Empty;
        public string                ModelId   { get; set; } = string.Empty;
        public DateTime              CreatedAt { get; set; }
        public DateTime              UpdatedAt { get; set; }
        public List<MessageSnapshot> Messages  { get; set; } = new();
    }

    public class MessageSnapshot
    {
        public string      Content   { get; set; } = string.Empty;
        public MessageRole Role      { get; set; }
        public DateTime    Timestamp { get; set; }
    }

    public class SettingsService
    {
        private static readonly string Dir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GeminiChat");
        private static readonly string SettingsFile = Path.Combine(Dir, "settings.json");
        private static readonly string HistoryFile  = Path.Combine(Dir, "history.json");
        private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

        private AppSettings _s = new();
        public  AppSettings  Settings => _s;

        public SettingsService() => Load();

        private void Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                    _s = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsFile)) ?? new();
            }
            catch { _s = new(); }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Dir);
                File.WriteAllText(SettingsFile, JsonSerializer.Serialize(_s, Opts));
            }
            catch { }
        }

        public void SaveHistory(IEnumerable<ChatSession> sessions)
        {
            try
            {
                var list = new List<SessionSnapshot>();
                foreach (var s in sessions)
                {
                    var snap = new SessionSnapshot
                    {
                        Id = s.Id, Title = s.Title, ModelId = s.ModelId,
                        CreatedAt = s.CreatedAt, UpdatedAt = s.UpdatedAt
                    };
                    foreach (var m in s.Messages)
                    {
                        if (string.IsNullOrWhiteSpace(m.Content)) continue;
                        snap.Messages.Add(new MessageSnapshot
                        {
                            Content = m.Content, Role = m.Role, Timestamp = m.Timestamp
                        });
                    }
                    list.Add(snap);
                }
                Directory.CreateDirectory(Dir);
                File.WriteAllText(HistoryFile, JsonSerializer.Serialize(list, Opts));
            }
            catch { }
        }

        public List<ChatSession> LoadHistory()
        {
            var result = new List<ChatSession>();
            try
            {
                if (!File.Exists(HistoryFile)) return result;
                var snaps = JsonSerializer.Deserialize<List<SessionSnapshot>>(File.ReadAllText(HistoryFile));
                if (snaps == null) return result;

                foreach (var snap in snaps)
                {
                    var session = new ChatSession { Title = snap.Title, ModelId = snap.ModelId };
                    session.UpdatedAt = snap.UpdatedAt;
                    foreach (var ms in snap.Messages)
                        session.Messages.Add(new ChatMessage
                        {
                            Role = ms.Role, Content = ms.Content, Status = MessageStatus.Sent
                        });
                    result.Add(session);
                }
            }
            catch { }
            return result;
        }
    }
}
