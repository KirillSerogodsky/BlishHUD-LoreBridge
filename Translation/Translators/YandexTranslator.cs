using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoreBridge.Enums;
using LoreBridge.Translation.Models;
using LoreBridge.Translation.Utils;

namespace LoreBridge.Translation.Translators;

public class YandexTranslator(TranslatorConfig config) : ITranslator
{
    private const string ApiUrl = "https://translate.yandex.net/api/v1/tr.json";

    private readonly HttpClient _httpClient = new();
    private readonly YandexUcid _ucid = new();

    public async Task<string> TranslateAsync(string text)
    {
        var targetLang = Enum.GetName(typeof(LanguageCodes), config.TargetLang).ToLower();
        var query = $"?ucid={_ucid.Get():N}&srv=android&format=text";
        var data = JsonSerializer.Serialize(new YandexRequestModel(text, "en", targetLang));
        using var content = new StringContent(data, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync(new Uri($"{ApiUrl}/translate{query}"), content)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<YandexResponseModel>(response.Content.ReadAsStringAsync().Result);

        return result.Text;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}