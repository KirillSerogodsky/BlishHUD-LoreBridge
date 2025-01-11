using System;
using System.Collections.Generic;
using System.Linq;

namespace LoreBridge.Language;

public static class LanguageDetails
{
    public static readonly List<LanguageDetail> List =
    [
        // new LanguageDetail { Language = Languages.Arabic, Code = "ar-AR", IsoCode = "ar", Name = "Arabic" },
        new LanguageDetail { Language = Languages.Bulgarian, Code = "bg-BG", IsoCode = "bg", Name = "Bulgarian" },
        // new LanguageDetail { Language = Languages.ChineseSimplified, Code = "zh-CN", IsoCode = "zh-CN", Name = "Chinese (Simplified)" },
        // new LanguageDetail { Language = Languages.ChineseTraditional, Code = "zh-TW", IsoCode = "zh-TW", Name = "Chinese (Traditional)" },
        new LanguageDetail { Language = Languages.Czech, Code = "cs-CZ", IsoCode = "cs", Name = "Czech" },
        new LanguageDetail { Language = Languages.Danish, Code = "da-DK", IsoCode = "da", Name = "Danish" },
        new LanguageDetail { Language = Languages.Dutch, Code = "nl-NL", IsoCode = "nl", Name = "Dutch" },
        new LanguageDetail { Language = Languages.Estonian, Code = "et-EE", IsoCode = "et", Name = "Estonian" },
        new LanguageDetail { Language = Languages.Finnish, Code = "fi-FI", IsoCode = "fi", Name = "Finnish" },
        // new LanguageDetail { Language = Languages.Greek, Code = "el-GR", IsoCode = "el", Name = "Greek" },
        new LanguageDetail { Language = Languages.German, Code = "de-DE", IsoCode = "de", Name = "German" },
        new LanguageDetail { Language = Languages.Hungarian, Code = "hu-HU", IsoCode = "hu", Name = "Hungarian" },
        new LanguageDetail { Language = Languages.Indonesian, Code = "id-ID", IsoCode = "id", Name = "Indonesian" },
        new LanguageDetail { Language = Languages.Italian, Code = "it-IT", IsoCode = "it", Name = "Italian" },
        // new LanguageDetail { Language = Languages.Japanese, Code = "ja-JP", IsoCode = "ja", Name = "Japanese" },
        new LanguageDetail { Language = Languages.Latvian, Code = "lv-LV", IsoCode = "lv", Name = "Latvian" },
        new LanguageDetail { Language = Languages.Lithuanian, Code = "lt-LT", IsoCode = "lt", Name = "Lithuanian" },
        new LanguageDetail { Language = Languages.Norwegian, Code = "no-NO", IsoCode = "no", Name = "Norwegian" },
        new LanguageDetail { Language = Languages.Polish, Code = "pl-PL", IsoCode = "pl", Name = "Polish" },
        new LanguageDetail { Language = Languages.Portuguese, Code = "pt-PT", IsoCode = "pt", Name = "Portuguese" },
        // new LanguageDetail { Language = Languages.PortugueseBrazilian, Code = "pt-BR", IsoCode = "pt-BR", Name = "Portuguese (Brazil)" },
        new LanguageDetail { Language = Languages.Romanian, Code = "ro-RO", IsoCode = "ro", Name = "Romanian" },
        new LanguageDetail { Language = Languages.Russian, Code = "ru-RU", IsoCode = "ru", Name = "Russian" },
        new LanguageDetail { Language = Languages.Slovak, Code = "sk-SK", IsoCode = "sk", Name = "Slovak" },
        new LanguageDetail { Language = Languages.Slovenian, Code = "sl-SI", IsoCode = "sl", Name = "Slovenian" },
        // new LanguageDetail { Language = Languages.Korean, Code = "ko-KR", IsoCode = "ko", Name = "Korean" },
        new LanguageDetail { Language = Languages.Swedish, Code = "sv-SE", IsoCode = "sv", Name = "Swedish" },
        new LanguageDetail { Language = Languages.Turkish, Code = "tr-TR", IsoCode = "tr", Name = "Turkish" },
        new LanguageDetail { Language = Languages.Ukrainian, Code = "uk-UA", IsoCode = "uk", Name = "Ukrainian" }
    ];

    private static readonly Dictionary<Languages, LanguageDetail> ByLanguage =
        List.ToDictionary(d => d.Language);

    public static LanguageDetail GetByLanguage(int languageCode)
    {
        if (!Enum.IsDefined(typeof(Languages), languageCode)) return null;
        
        return ByLanguage.TryGetValue((Languages)languageCode, out var detail) ? detail : null;
    }
}