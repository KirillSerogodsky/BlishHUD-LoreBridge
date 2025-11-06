using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LoreBridge.Translation.Models;

public sealed class DeepLResponse
{
    [JsonPropertyName("result")] public ResponseResult Result { get; set; }

    public sealed class ResponseResult
    {
        [JsonPropertyName("translations")] public List<Translation> Translations { get; set; }
    }

    public sealed class Translation
    {
        [JsonPropertyName("beams")] public List<Beam> Beams { get; set; }
    }

    public sealed class Beam
    {
        [JsonPropertyName("postprocessed_sentence")]
        public string PostProcessedSentence { get; set; }
    }
}