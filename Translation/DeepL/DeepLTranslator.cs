using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoreBridge.Translation.DeepL
{
    public class DeepLTranslator : ITranslator
    {
        private const string API_URL = "https://www2.deepl.com/jsonrpc";

        private long Id { get; set; }
        private readonly TranslatorConfig _config;
        private readonly CookieContainer _cookies;
        private readonly HttpClient _httpClient;

        public DeepLTranslator(TranslatorConfig config)
        {
            _config = config;
            Id = GenerateId();

            _cookies = new();
            HttpClientHandler handler = new()
            {
                CookieContainer = _cookies
            };
            _httpClient = new(handler);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.deepl.com/");
        }

        public async Task<string> TranslateAsync(string text)
        {
            DeepLRequestBody requestBody = new(Id, text, "EN", "RU");
            string body = requestBody.ToJsonString();
            StringContent content = new(body, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("DNT", "1");

            string result = "";

            try
            {
                using var response = await _httpClient.PostAsync(API_URL, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode.ToString() == "OK")
                {
                    DeepLResponseBody deserializedBody = JsonSerializer.Deserialize<DeepLResponseBody>(responseBody);
                    if (deserializedBody.Result?.Translations != null)
                    {
                        var beam = deserializedBody.Result?.Translations[0].Beams.First();
                        result = beam.PostProcessedSentence;

                        return result;
                    }
                } else
                {
                    DeepLResponseErrorBody deserializedBody = JsonSerializer.Deserialize<DeepLResponseErrorBody>(responseBody);
                    
                    return $"Error: {deserializedBody.Error.Message}";
                }
            } catch (Exception e)
            {
                result = $"Error: {e.Message}";
            }

            Id++;

            return result;
        }

        private long GenerateId()
        {
            long num = 10000L;
            Random random = new();

            return num * (long)Math.Round(num * random.NextDouble());
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
