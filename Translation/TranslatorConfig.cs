using LoreBridge.Translation.Language;

namespace LoreBridge.Translation;

public class TranslatorConfig
{
    public LanguageInfo TargetLang { get; set; }

    public string ApiUrl { get; set; }
}