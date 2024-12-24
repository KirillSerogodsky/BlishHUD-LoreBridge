using System.Text.Json.Serialization;

namespace LoreBridge.Translation.Models;

public sealed class DeepLResponseErrorModel
{
    [JsonPropertyName("error")] public ResponseError Error { get; set; }

    public sealed class ResponseError
    {
        [JsonPropertyName("message")] public string Message { get; set; }
    }
}