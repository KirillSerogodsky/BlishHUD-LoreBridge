using System.Text.Json.Serialization;

namespace LoreBridge.Translation.DeepL
{
    public class DeepLResponseErrorBody
    {
        [JsonPropertyName("error")]
        public ResponseError Error { get; set; }

        public sealed class ResponseError
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }
    }
}
