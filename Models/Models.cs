using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GeminiChat.Models
{
    public enum MessageRole { User, Assistant }
    public enum MessageStatus { Sent, Streaming, Error }

    public partial class ChatMessage : ObservableObject
    {
        [ObservableProperty] private string _content = string.Empty;
        [ObservableProperty] private MessageStatus _status = MessageStatus.Sent;
        [ObservableProperty] private bool _isStreaming;

        public string Id { get; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; } = DateTime.Now;
        public MessageRole Role { get; init; }
        public bool IsUser => Role == MessageRole.User;
        public bool IsAssistant => Role == MessageRole.Assistant;
    }

    public partial class ChatSession : ObservableObject
    {
        [ObservableProperty] private string _title = "New Chat";
        [ObservableProperty] private string _modelId = string.Empty;

        public string Id { get; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ObservableCollection so ItemsControl updates live
        public ObservableCollection<ChatMessage> Messages { get; } = new();
    }

    public class GeminiModel
    {
        public string Id       { get; init; } = string.Empty;
        public string Name     { get; init; } = string.Empty;
        public string Badge    { get; init; } = string.Empty;
        public string Desc     { get; init; } = string.Empty;
        public int    MaxOut   { get; init; } = 8192;
        public string Series   { get; init; } = string.Empty;

        // ── Verified live model IDs (March 2026) ──────────────────────────────
        // Gemini 2.5 series
        public static readonly GeminiModel Flash25 = new()
        {
            Id = "gemini-2.5-flash", Name = "Gemini 2.5 Flash",
            Badge = "2.5 Flash", Desc = "Fast & efficient, best for most tasks",
            MaxOut = 65536, Series = "2.5"
        };
        public static readonly GeminiModel Pro25 = new()
        {
            Id = "gemini-2.5-pro", Name = "Gemini 2.5 Pro",
            Badge = "2.5 Pro", Desc = "Most capable reasoning & analysis",
            MaxOut = 65536, Series = "2.5"
        };
        public static readonly GeminiModel FlashLite25 = new()
        {
            Id = "gemini-2.5-flash-lite", Name = "Gemini 2.5 Flash Lite",
            Badge = "2.5 Lite", Desc = "Ultra-fast, high-volume tasks",
            MaxOut = 65536, Series = "2.5"
        };

        // Gemini 3 series
        public static readonly GeminiModel Flash3 = new()
        {
            Id = "gemini-3-flash-preview", Name = "Gemini 3 Flash",
            Badge = "3 Flash", Desc = "Frontier performance, fraction of cost",
            MaxOut = 65536, Series = "3"
        };

        // Gemini 3.1 series
        public static readonly GeminiModel Pro31 = new()
        {
            Id = "gemini-3.1-pro-preview", Name = "Gemini 3.1 Pro",
            Badge = "3.1 Pro", Desc = "Advanced intelligence & agentic coding",
            MaxOut = 65536, Series = "3.1"
        };
        public static readonly GeminiModel FlashLite31 = new()
        {
            Id = "gemini-3.1-flash-lite-preview", Name = "Gemini 3.1 Flash Lite",
            Badge = "3.1 Lite", Desc = "Most cost-efficient, ultra-fast",
            MaxOut = 65536, Series = "3.1"
        };

        public static readonly List<GeminiModel> All = new()
        {
            Flash25, Pro25, FlashLite25, Flash3, Pro31, FlashLite31
        };
    }

    public class IcebreakerTile
    {
        public string Icon    { get; init; } = string.Empty;
        public string Label   { get; init; } = string.Empty;
        public string Prompt  { get; init; } = string.Empty;

        public static readonly List<IcebreakerTile> All = new()
        {
            new() { Icon = "✍️", Label = "Write a poem",          Prompt = "Write a short, creative poem about the beauty of a rainy evening." },
            new() { Icon = "💡", Label = "Brainstorm ideas",       Prompt = "Give me 5 creative business ideas for someone who loves technology and sustainability." },
            new() { Icon = "🧠", Label = "Explain a concept",      Prompt = "Explain quantum entanglement in simple terms, as if I'm 12 years old." },
            new() { Icon = "🌍", Label = "Fun facts",              Prompt = "Tell me 3 surprising and fascinating facts about the universe." },
            new() { Icon = "🗺️", Label = "Plan a trip",            Prompt = "Help me plan a 5-day travel itinerary for Japan, focusing on culture and food." },
            new() { Icon = "🐛", Label = "Debug my code",          Prompt = "What are the most common causes of memory leaks in C# and how can I fix them?" },
            new() { Icon = "📝", Label = "Improve my writing",     Prompt = "Give me tips to make my writing more engaging and concise." },
            new() { Icon = "🎯", Label = "Set goals",              Prompt = "Help me create a realistic 30-day plan to learn a new programming language." },
        };
    }

    public class GenerationConfig
    {
        public float Temperature    { get; set; } = 0.9f;
        public float TopP           { get; set; } = 1.0f;
        public int   TopK           { get; set; } = 40;
        public int   MaxOutputTokens{ get; set; } = 8192;
        public bool  EnableStreaming { get; set; } = true;
    }
}