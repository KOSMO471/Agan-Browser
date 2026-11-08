using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public class OllamaService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:11434"),
            Timeout = TimeSpan.FromMinutes(10)
        };

        public string ModelName { get; set; } = "qwen2.5-coder:7b";

        public async Task<bool> IsServerAvailableAsync()
        {
            try
            {
                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(3));
                var response = await _httpClient.GetAsync("/api/tags", cts.Token);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> GetAvailableModelsAsync()
        {
            var list = new List<string>();
            try
            {
                var result = await _httpClient.GetFromJsonAsync<OllamaTagResponse>("/api/tags");
                if (result?.Models != null)
                {
                    foreach (var m in result.Models) list.Add(m.Name);
                }
            }
            catch { }
            return list;
        }

        public async Task ChatStreamAsync(string systemPrompt, string userMessage, Action<string> onChunk)
        {
            var request = new ChatRequest
            {
                Model = ModelName,
                Messages = new[]
                {
                    new ChatMessage { Role = "system", Content = systemPrompt },
                    new ChatMessage { Role = "user", Content = userMessage }
                },
                Stream = true,
                Options = new OllamaOptions { NumCtx = 8192, Temperature = 0.7 }
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
            {
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                onChunk($"[Ollama Hatası: {(int)response.StatusCode} - {body}]");
                return;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new System.IO.StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    using var doc = JsonDocument.Parse(line);
                    if (doc.RootElement.TryGetProperty("message", out var msg) &&
                        msg.TryGetProperty("content", out var content))
                    {
                        string text = content.GetString() ?? "";
                        if (!string.IsNullOrEmpty(text)) onChunk(text);
                    }
                }
                catch { }
            }
        }

        public Task<string> SendHtmlToAganAsync(string html, string question)
        {
            if (string.IsNullOrEmpty(html))
                return Task.FromResult(question);

            if (html.Length > 50000)
                html = html.Substring(0, 50000) + "\n\n[...HTML kesildi...]";

            return Task.FromResult($"HTML5 Kodu:\n{html}\n\nKullanıcı Sorusu: {question}");
        }
    }
}