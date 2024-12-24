using System.Text.Json.Serialization;

namespace LoreBridge.Translation.Models;

public class YandexResponseModel
{
    [JsonPropertyName("text")] public string Text { get; set; }
}