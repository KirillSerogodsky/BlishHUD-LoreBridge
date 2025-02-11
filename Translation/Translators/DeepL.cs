using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoreBridge.Translation.Models;

namespace LoreBridge.Translation.Translators;

public class DeepL : ITranslator
{
    private const string ApiUrl = "https://www2.deepl.com/jsonrpc";
    private readonly TranslatorConfig _config;
    private readonly CookieContainer _cookies;
    private readonly HttpClient _httpClient;

    public DeepL(TranslatorConfig config)
    {
        _config = config;
        Id = GenerateId();

        _cookies = new CookieContainer();
        HttpClientHandler handler = new()
        {
            CookieContainer = _cookies
        };
        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.deepl.com/");
    }

    private long Id { get; set; }

    public async Task<string> TranslateAsync(string text)
    {
        throw new Exception("DeepL translator not implemented");

        var targetLang = _config.TargetLang.IsoCode;
        DeepLRequest request = new(Id, text, "EN", targetLang);
        var body = request.ToJsonString();
        StringContent content = new(body, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        content.Headers.TryAddWithoutValidation("Dnt", "1");

        var result = "";

        try
        {
            using var response = await _httpClient.PostAsync(ApiUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode.ToString() == "OK")
            {
                var deserializedModel = JsonSerializer.Deserialize<DeepLResponse>(responseBody);
                if (deserializedModel.Result?.Translations != null)
                {
                    var beam = deserializedModel.Result?.Translations[0].Beams.First();
                    result = beam.PostProcessedSentence;
                }
            }
            else
            {
                var deserializedModel = JsonSerializer.Deserialize<DeepLResponseError>(responseBody);
                result = $"Error: {deserializedModel.Error.Message}";
            }
        }
        catch (Exception e)
        {
            result = $"Error: {e.Message}";
        }

        Id++;

        return result;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private long GenerateId()
    {
        var num = 10000L;
        Random random = new();

        return num * (long)Math.Round(num * random.NextDouble());
    }
}