using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using GeminiChat.Models;

namespace GeminiChat.Services
{
    public class GeminiApiException : Exception
    {
        public int StatusCode { get; }
        public GeminiApiException(string message, int statusCode = 0)
            : base(message) => StatusCode = statusCode;
    }

    // ── DTOs ─────────────────────────────────────────────────────────────────
    record GeminiRequest(
        [property: JsonPropertyName("contents")]          List<GContent>     Contents,
        [property: JsonPropertyName("generationConfig")]  GGenConfig         Config,
        [property: JsonPropertyName("systemInstruction")] GContent?          System = null
    );
    record GContent(
        [property: JsonPropertyName("role")]  string       Role,
        [property: JsonPropertyName("parts")] List<GPart>  Parts
    );
    record GPart([property: JsonPropertyName("text")] string Text);
    record GGenConfig(
        [property: JsonPropertyName("temperature")]      float Temperature,
        [property: JsonPropertyName("topP")]             float TopP,
        [property: JsonPropertyName("topK")]             int   TopK,
        [property: JsonPropertyName("maxOutputTokens")]  int   MaxOutputTokens
    );
    record GResponse(
        [property: JsonPropertyName("candidates")]    List<GCandidate>? Candidates,
        [property: JsonPropertyName("error")]         GError?           Error
    );
    record GCandidate(
        [property: JsonPropertyName("content")]      GContent? Content,
        [property: JsonPropertyName("finishReason")] string?   FinishReason
    );
    record GError(
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("code")]    int    Code
    );

    public class GeminiService : IDisposable
    {
        private readonly HttpClient _http;
        private string _apiKey = string.Empty;
        private const string BASE = "https://generativelanguage.googleapis.com/v1beta/models";

        public GeminiService()
        {
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
        }

        public void   SetApiKey(string k) => _apiKey = k.Trim();
        public bool   HasApiKey           => !string.IsNullOrWhiteSpace(_apiKey);

        // CORRECT URL FORMAT:
        //   non-stream → :generateContent?key=KEY
        //   stream     → :streamGenerateContent?alt=sse&key=KEY
        private string Url(string modelId, bool stream = false) =>
            stream
            ? $"{BASE}/{modelId}:streamGenerateContent?alt=sse&key={_apiKey}"
            : $"{BASE}/{modelId}:generateContent?key={_apiKey}";

        private GeminiRequest BuildReq(IEnumerable<ChatMessage> messages, GenerationConfig cfg, string? sysprompt)
        {
            var contents = messages
                .Select(m => new GContent(
                    m.Role == MessageRole.User ? "user" : "model",
                    new List<GPart> { new(m.Content) }))
                .ToList();

            GContent? sys = string.IsNullOrWhiteSpace(sysprompt) ? null
                : new GContent("user", new List<GPart> { new(sysprompt) });

            return new GeminiRequest(
                contents,
                new GGenConfig(cfg.Temperature, cfg.TopP, cfg.TopK, cfg.MaxOutputTokens),
                sys);
        }

        private static (bool ok, string msg) ParseError(string body, int status)
        {
            GResponse? r = null;
            try { r = JsonSerializer.Deserialize<GResponse>(body); } catch { }
            return (false, r?.Error?.Message ?? $"HTTP {status}: {body}");
        }

        public async Task<string> GenerateAsync(
            string modelId, List<ChatMessage> messages,
            GenerationConfig cfg, string? sysprompt = null,
            CancellationToken ct = default)
        {
            if (!HasApiKey) throw new GeminiApiException("No API key set.");

            var body = JsonSerializer.Serialize(BuildReq(messages, cfg, sysprompt));
            var resp = await _http.PostAsync(
                Url(modelId),
                new StringContent(body, Encoding.UTF8, "application/json"),
                ct);

            var raw = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
            {
                var (_, msg) = ParseError(raw, (int)resp.StatusCode);
                throw new GeminiApiException(msg, (int)resp.StatusCode);
            }

            var result = JsonSerializer.Deserialize<GResponse>(raw);
            return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                   ?? throw new GeminiApiException("Empty response.");
        }

        public async IAsyncEnumerable<string> StreamAsync(
            string modelId, List<ChatMessage> messages,
            GenerationConfig cfg, string? sysprompt = null,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (!HasApiKey) throw new GeminiApiException("No API key set.");

            var body = JsonSerializer.Serialize(BuildReq(messages, cfg, sysprompt));
            var req  = new HttpRequestMessage(HttpMethod.Post, Url(modelId, stream: true))
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);

            if (!resp.IsSuccessStatusCode)
            {
                var errRaw = await resp.Content.ReadAsStringAsync(ct);
                var (_, msg) = ParseError(errRaw, (int)resp.StatusCode);
                throw new GeminiApiException(msg, (int)resp.StatusCode);
            }

            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(ct);
                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data: ")) continue;

                var data = line.Substring(6);
                if (data == "[DONE]") break;

                GResponse? chunk = null;
                try { chunk = JsonSerializer.Deserialize<GResponse>(data); } catch { continue; }

                var text = chunk?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
                if (!string.IsNullOrEmpty(text)) yield return text;
            }
        }

        /// <summary>
        /// Validates key via the /models list endpoint — zero token cost.
        /// </summary>
        public async Task<(bool valid, string message)> ValidateAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return (false, "Key is empty.");

            var trimmed = key.Trim();
            try
            {
                using var h = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                var r = await h.GetAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models?key={trimmed}");
                var raw = await r.Content.ReadAsStringAsync();

                if (r.IsSuccessStatusCode)
                {
                    _apiKey = trimmed;
                    return (true, "API key is valid ✓");
                }

                GResponse? err = null;
                try { err = JsonSerializer.Deserialize<GResponse>(raw); } catch { }
                return (false, err?.Error?.Message ?? $"HTTP {(int)r.StatusCode}");
            }
            catch (Exception ex)
            {
                return (false, $"Network error: {ex.Message}");
            }
        }

        public void Dispose() => _http.Dispose();
    }
}