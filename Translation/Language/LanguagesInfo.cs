using System;
using System.Collections.Generic;
using System.Linq;

namespace LoreBridge.Translation.Language;

public static class LanguagesInfo
{
    public static readonly List<LanguageInfo> List =
    [
        // new LanguageDetail { Language = Languages.Arabic, Code = "ar-AR", IsoCode = "ar", Name = "Arabic" },
        new() { Language = Languages.Bulgarian, Code = "bg-BG", IsoCode = "bg", Name = "Bulgarian" },
        new() { Language = Languages.ChineseSimplified, Code = "zh-CN", IsoCode = "zh-CN", Name = "Chinese (Simplified)" },
        new() { Language = Languages.ChineseTraditional, Code = "zh-TW", IsoCode = "zh-TW", Name = "Chinese (Traditional)" },
        new() { Language = Languages.Czech, Code = "cs-CZ", IsoCode = "cs", Name = "Czech" },
        new() { Language = Languages.Danish, Code = "da-DK", IsoCode = "da", Name = "Danish" },
        new() { Language = Languages.Dutch, Code = "nl-NL", IsoCode = "nl", Name = "Dutch" },
        new() { Language = Languages.Estonian, Code = "et-EE", IsoCode = "et", Name = "Estonian" },
        new() { Language = Languages.Finnish, Code = "fi-FI", IsoCode = "fi", Name = "Finnish" },
        new() { Language = Languages.Greek, Code = "el-GR", IsoCode = "el", Name = "Greek" },
        new() { Language = Languages.German, Code = "de-DE", IsoCode = "de", Name = "German" },
        new() { Language = Languages.Hungarian, Code = "hu-HU", IsoCode = "hu", Name = "Hungarian" },
        new() { Language = Languages.Indonesian, Code = "id-ID", IsoCode = "id", Name = "Indonesian" },
        new() { Language = Languages.Italian, Code = "it-IT", IsoCode = "it", Name = "Italian" },
        new() { Language = Languages.Japanese, Code = "ja-JP", IsoCode = "ja", Name = "Japanese" },
        new() { Language = Languages.Latvian, Code = "lv-LV", IsoCode = "lv", Name = "Latvian" },
        new() { Language = Languages.Lithuanian, Code = "lt-LT", IsoCode = "lt", Name = "Lithuanian" },
        new() { Language = Languages.Norwegian, Code = "no-NO", IsoCode = "no", Name = "Norwegian" },
        new() { Language = Languages.Polish, Code = "pl-PL", IsoCode = "pl", Name = "Polish" },
        new() { Language = Languages.Portuguese, Code = "pt-PT", IsoCode = "pt", Name = "Portuguese" },
        new() { Language = Languages.PortugueseBrazilian, Code = "pt-BR", IsoCode = "pt-BR", Name = "Portuguese (Brazil)" },
        new() { Language = Languages.Romanian, Code = "ro-RO", IsoCode = "ro", Name = "Romanian" },
        new() { Language = Languages.Russian, Code = "ru-RU", IsoCode = "ru", Name = "Russian" },
        new() { Language = Languages.Slovak, Code = "sk-SK", IsoCode = "sk", Name = "Slovak" },
        new() { Language = Languages.Slovenian, Code = "sl-SI", IsoCode = "sl", Name = "Slovenian" },
        new() { Language = Languages.Korean, Code = "ko-KR", IsoCode = "ko", Name = "Korean" },
        new() { Language = Languages.Swedish, Code = "sv-SE", IsoCode = "sv", Name = "Swedish" },
        new() { Language = Languages.Turkish, Code = "tr-TR", IsoCode = "tr", Name = "Turkish" },
        new() { Language = Languages.Ukrainian, Code = "uk-UA", IsoCode = "uk", Name = "Ukrainian" },
        new() { Language = Languages.Vietnamese, Code = "vi-VN", IsoCode = "vi", Name = "Vietnamese" }
    ];

    private static readonly Dictionary<Languages, LanguageInfo> _byLanguage =
        List.ToDictionary(d => d.Language);

    private static readonly Dictionary<string, LanguageInfo> _byName =
        List.ToDictionary(d => d.Name);

    public static LanguageInfo GetByLanguage(int languageCode)
    {
        if (!Enum.IsDefined(typeof(Languages), languageCode)) return null;

        return _byLanguage.TryGetValue((Languages)languageCode, out var detail) ? detail : null;
    }

    public static LanguageInfo GetByName(string name)
    {
        return _byName.TryGetValue(name, out var detail) ? detail : null;
    }
}