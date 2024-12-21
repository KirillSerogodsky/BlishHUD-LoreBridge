using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using LoreBridge.Enums;

namespace LoreBridge.Translation.Google
{
    public class GoogleTranslator : ITranslator
    {
        private const string BASE_URL = "https://translate.google.com/m?&sl={0}&tl={1}&hl={1}&q={2}";

        private readonly TranslatorConfig _config;
        private readonly CookieContainer _cookies;
        private readonly HttpClient _httpClient;

        public GoogleTranslator(TranslatorConfig config)
        {
            _config = config;

            _cookies = new();
            HttpClientHandler handler = new()
            {
                CookieContainer = _cookies
            };
            _httpClient = new(handler);
        }

        public async Task<string> TranslateAsync(string text)
        {
            var targetLang = Enum.GetName(typeof(LanguageCodes), _config.TargetLang).ToLower();
            var url = string.Format(BASE_URL, "en", targetLang, HttpUtility.UrlEncode(text));

            string result = "";
            try
            {
                using var response = await _httpClient.GetAsync(url);
                if (response.StatusCode.ToString() == "OK")
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var regexp = new Regex("(?<=(<div(.*)class=\"result-container\"(.*)>))[\\s\\S]*?(?=(<\\/div>))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    var match = regexp.Match(body);
                   
                    if (match.Success)
                    {
                        result = WebUtility.HtmlDecode(match.Value);
                    }
                } else
                {
                    result = "Error: Failed to translate";
                }
            } catch(Exception e)
            {
                result = $"Error: {e.Message}";
            }

            return result;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
