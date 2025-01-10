using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using LoreBridge.Enums;

namespace LoreBridge.Translation.Translators;

public class GoogleTranslator(TranslatorConfig config) : ITranslator
{
    private const string BaseUrl = "https://translate.google.com/m?&sl={0}&tl={1}&hl={1}&q={2}";

    private static readonly Regex RegexResult = new(
        "(?<=(<div(.*)class=\"result-container\"(.*)>))[\\s\\S]*?(?=(<\\/div>))",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly HttpClient _httpClient = new();

    public async Task<string> TranslateAsync(string text)
    {
        var targetLang = Enum.GetName(typeof(LanguageCodes), config.TargetLang).ToLower();
        var url = string.Format(BaseUrl, "en", targetLang, HttpUtility.UrlEncode(text));
        using var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var match = RegexResult.Match(responseText);

        return match.Success ? WebUtility.HtmlDecode(match.Value) : "Error: Failed to translate";
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}