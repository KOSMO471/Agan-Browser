using System;
using System.Text.Json.Serialization;

namespace AganBrowser.Models
{
    public class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class OllamaOptions
    {
        [JsonPropertyName("num_ctx")]
        public int NumCtx { get; set; } = 8192;

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;
    }

    public class ChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public ChatMessage[] Messages { get; set; } = Array.Empty<ChatMessage>();

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;

        [JsonPropertyName("options")]
        public OllamaOptions Options { get; set; } = new OllamaOptions();
    }

    public class ChatResponse
    {
        [JsonPropertyName("message")]
        public ChatMessage Message { get; set; } = new ChatMessage();
    }

    public class OllamaTagResponse
    {
        [JsonPropertyName("models")]
        public OllamaTagModel[] Models { get; set; } = Array.Empty<OllamaTagModel>();
    }

    public class OllamaTagModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}