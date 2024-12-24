using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LoreBridge.Translation.Models;

public sealed class GoogleResponseModel
{
    [JsonPropertyName("sentences")] public List<Sentence> Sentences { get; set; }

    public sealed class Sentence
    {
        [JsonPropertyName("trans")] public string Trans { get; set; }
    }
}