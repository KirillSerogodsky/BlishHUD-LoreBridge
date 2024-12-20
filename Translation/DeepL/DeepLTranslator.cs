using System;
using System.Diagnostics;
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
        private readonly CookieContainer cookies;

        // private readonly HttpClient _httpClient;
        private readonly HttpClient httpClient;

        public DeepLTranslator(TranslatorConfig config)
        {
            _config = config;
            Id = GenerateId();
            // _httpClient = new HttpClient();

            cookies = new();
            HttpClientHandler handler = new()
            {
                CookieContainer = cookies
            };
            httpClient = new(handler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.deepl.com/translator");
        }

        public async Task<string> TranslateAsync(string text)
        {
            DeepLRequestBody requestBody = new(Id, text, "EN", "RU");
            string body = requestBody.ToJsonString();

            Debug.WriteLine(body);
            // var result = await _httpClient.RequestAsync(API_URL, HttpMethods.POST, body).ConfigureAwait(false);

            string result = "";
            try
            {
                StringContent content = new(body, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                content.Headers.TryAddWithoutValidation("DNT", "1");
                content.Headers.TryAddWithoutValidation("TE", "Trailers");
                content.Headers.TryAddWithoutValidation("Accept-Language", "en-US;q=0.5,en;q=0.3");

                using var response = await httpClient.PostAsync(API_URL, content);
                Debug.WriteLine(response.StatusCode);

                var responseCookies = cookies.GetCookies(new Uri(API_URL)).Cast<Cookie>();
                foreach (Cookie cookie in responseCookies)
                    Debug.WriteLine(cookie.Name + ": " + cookie.Value);

                var responseBody = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode.ToString() == "OK")
                {
                    DeepLResponseBody deserializedBody = JsonSerializer.Deserialize<DeepLResponseBody>(responseBody);
                    if (deserializedBody.Result?.Translations != null)
                    {
                        var beam = deserializedBody.Result?.Translations[0].Beams.First();
                        result = beam.PostProcessedSentence;
                        return result;
                    }
                }
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                result = e.Message;
            }

            Id++;

            // Debug.WriteLine(result);

            return result;
        }

        private long GenerateId()
        {
            long num = 10000L;
            var random = new Random();
            return num * (long)Math.Round((double)num * random.NextDouble());
        }
    }
}
