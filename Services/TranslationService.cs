using Blish_HUD;
using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Services;

public class TranslationService : Service
{
    private SettingsModel _settings;

    public override void Load(SettingsModel settings)
    {
        _settings = settings;
        // _translatorConfig = new TranslatorConfig
        // {
        //     TargetLang = LanguageDetails.GetByLanguage(_settings.TranslationLanguage.Value)
        // };
        // CreateTranslator((Translators)_settings.TranslationTranslator.Value, _translatorConfig);
        // _translationService = new TranslationService(_translator, _messages);

        _settings.TranslationLanguage.SettingChanged += OnTranslationLanguageChanged;
        _settings.TranslationTranslator.SettingChanged += OnTranslationTranslatorChanged;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Unload()
    {
        _settings.TranslationLanguage.SettingChanged -= OnTranslationLanguageChanged;
        _settings.TranslationTranslator.SettingChanged -= OnTranslationTranslatorChanged;
    }

    /* private void CreateTranslator(Translators translator, TranslatorConfig config)
    {
        _translator?.Dispose();
        _translator = translator switch
        {
            Translators.DeepL => new DeepLTranslator(config),
            Translators.Google => new GoogleTranslator(config),
            Translators.Google2 => new GoogleTranslator2(config),
            Translators.Yandex => new YandexTranslator(config),
            _ => null
        };
        _translationService = new TranslationService(_translator, _messages);
    } */

    private void OnTranslationLanguageChanged(object sender, ValueChangedEventArgs<int> e)
    {
        // _translatorConfig.TargetLang = LanguageDetails.GetByLanguage(e.NewValue);
    }

    private void OnTranslationTranslatorChanged(object sender, ValueChangedEventArgs<int> e)
    {
        // CreateTranslator((Translators)e.NewValue, _translatorConfig);
    }
}