using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using LoreBridge.Enums;
using Newtonsoft.Json.Linq;

namespace LoreBridge.Translation.Translators;

public class GoogleTranslator2(TranslatorConfig config) : ITranslator
{
    private const string ApiUrl = "https://translate.googleapis.com/translate_a/single";
    private const string ApiUrlParams = "client=gtx&sl={0}&tl={1}&dt=t&q={2}";

    private readonly HttpClient _httpClient = new();

    public async Task<string> TranslateAsync(string text)
    {
        var targetLang = Enum.GetName(typeof(LanguageCodes), config.TargetLang).ToLower();
        var url = $"{ApiUrl}?{string.Format(ApiUrlParams, "en", targetLang, HttpUtility.UrlEncode(text))}";
        using var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
        
        response.EnsureSuccessStatusCode();
        
        var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return string.Join(string.Empty, JArray.Parse(responseText)[0].Select(x => x[0]));
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}