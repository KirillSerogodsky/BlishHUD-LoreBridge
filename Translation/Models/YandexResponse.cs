using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace LoreBridge.Translation.Models;

public class YandexResponse
{
    [JsonPropertyName("code")] public HttpStatusCode Code { get; set; }
    [JsonPropertyName("text")] public List<string> Text { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; }
}