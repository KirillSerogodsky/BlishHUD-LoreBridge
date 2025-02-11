using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LoreBridge.Translation.Models;
using LoreBridge.Translation.Utils;
using HttpClient = System.Net.Http.HttpClient;

namespace LoreBridge.Translation.Translators;

public class Yandex : ITranslator
{
    private const string ApiUrl = "https://translate.yandex.net/api/v1/tr.json";
    private const string UserAgent = "ru.yandex.translate/3.20.2024";
    private readonly TranslatorConfig _config;

    private readonly HttpClient _httpClient = new();
    private readonly YandexUcid _ucid = new();

    public Yandex(TranslatorConfig config)
    {
        _config = config;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
    }

    public async Task<string> TranslateAsync(string text)
    {
        var targetLang = _config.TargetLang.IsoCode;
        var query = $"?ucid={_ucid.Get():N}&srv=android&format=text";
        var data = new Dictionary<string, string>
        {
            { "text", text },
            { "lang", $"en-{targetLang}" }
        };
        using var content = new FormUrlEncodedContent(data);
        using var response = await _httpClient.PostAsync(new Uri($"{ApiUrl}/translate{query}"), content)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<YandexResponse>(response.Content.ReadAsStringAsync().Result);
        if (result.Code != HttpStatusCode.OK) throw new Exception(result.Message);

        return result.Text[0];
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}