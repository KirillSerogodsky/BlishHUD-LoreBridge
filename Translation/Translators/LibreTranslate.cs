using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoreBridge.Translation.Models;

namespace LoreBridge.Translation.Translators;

public class LibreTranslate(TranslatorConfig config) : ITranslator
{
    private readonly HttpClient _httpClient = new();

    public async Task<string> TranslateAsync(string text)
    {
        var targetLang = config.TargetLang.IsoCode.ToLower();
        var requestBody = new LibreTranslateRequest
        {
            Source = "en",
            Target = targetLang,
            Query = text,
            Format = "text",
            Alternatives = 0
        };
        var body = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        try
        {
            using var response = await _httpClient.PostAsync("http://192.168.1.24:7017/translate", content)
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseModel = JsonSerializer.Deserialize<LibreTranslateResponse>(responseBody);

            return responseModel.TranslatedText;
        }
        catch (Exception e)
        {
            return $"Error: {e.Message}";
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}