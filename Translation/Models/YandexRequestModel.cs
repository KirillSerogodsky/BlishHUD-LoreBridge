using System.Text.Json.Serialization;

namespace LoreBridge.Translation.Models;

public class YandexRequestModel(string text, string sourceLang, string targetLang)
{
    [JsonPropertyName("text")] public string Text { get; set; } = text;
    [JsonPropertyName("lang")] public string Lang { get; set; } = $"${sourceLang}-${targetLang}";
}