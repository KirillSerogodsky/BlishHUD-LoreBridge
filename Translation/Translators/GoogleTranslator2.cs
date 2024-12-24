using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LoreBridge.Enums;
using LoreBridge.Translation.Models;

namespace LoreBridge.Translation.Translators;

public class GoogleTranslator2(TranslatorConfig config) : ITranslator
{
    private const string ApiUrl = "https://translate.googleapis.com/translate_a/single";
    private const string ApiUrlParams = "client=gtx&sl={0}&tl={1}&dt=t&dt=bd&dj=1&source=input&tk={2}";
    private const string Salt1 = "+-a^+6";
    private const string Salt2 = "+-3^+b+-f";

    private readonly HttpClient _httpClient = new();

    public async Task<string> TranslateAsync(string text)
    {
        var targetLang = Enum.GetName(typeof(LanguageCodes), config.TargetLang).ToLower();
        var url = $"{ApiUrl}?{string.Format(ApiUrlParams, "en", targetLang, MakeToken(text.AsSpan()))}";
        using var content = new FormUrlEncodedContent([new KeyValuePair<string, string>("q", text)]);
        using var response = await _httpClient.PostAsync(new Uri(url), content).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var body = JsonSerializer.Deserialize<GoogleResponseModel>(responseText);

        return body.Sentences != null ? string.Concat(body.Sentences.Select(x => x.Trans)) : string.Empty;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private static string MakeToken(ReadOnlySpan<char> text)
    {
        long a = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 3600, b = a;

        foreach (var ch in text) a = WorkToken(a + ch, Salt1);

        a = WorkToken(a, Salt2);

        if (a < 0) a = (a & int.MaxValue) + int.MaxValue + 1;

        a %= 1000000;

        return $"{a}.{a ^ b}";
    }

    private static long WorkToken(long num, string seed)
    {
        for (var i = 0; i < seed.Length - 2; i += 3)
        {
            int d = seed[i + 2];

            if (d >= 'a') d -= 'W';

            if (seed[i + 1] == '+')
                num = (num + (num >> d)) & uint.MaxValue;
            else
                num ^= num << d;
        }

        return num;
    }
}