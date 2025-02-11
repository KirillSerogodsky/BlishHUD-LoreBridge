using System.Threading.Tasks;
using Blish_HUD;
using LoreBridge.Models;
using LoreBridge.Translation;
using LoreBridge.Translation.Language;
using LoreBridge.Translation.Translators;
using Microsoft.Xna.Framework;

namespace LoreBridge.Services;

public class TranslationService : Service
{
    private SettingsModel _settings;
    private ITranslator _translator;
    private TranslatorConfig _translatorConfig;

    public override void Load(SettingsModel settings)
    {
        _settings = settings;
        _translatorConfig = new TranslatorConfig
        {
            TargetLang = LanguagesInfo.GetByLanguage(_settings.TranslationLanguage.Value)
        };

        switch (_settings.TranslationTranslator.Value)
        {
            case (int)Translators.LibreTranslate:
                _translatorConfig.ApiUrl = settings.TranslationLibreTranslateUrl.Value;
                break;
        }

        CreateTranslator((Translators)_settings.TranslationTranslator.Value, _translatorConfig);

        _settings.TranslationLanguage.SettingChanged += OnTranslationLanguageChanged;
        _settings.TranslationTranslator.SettingChanged += OnTranslationTranslatorChanged;
        _settings.TranslationLibreTranslateUrl.SettingChanged += OnLibreTranslateUrlChanged;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Unload()
    {
        _settings.TranslationLanguage.SettingChanged -= OnTranslationLanguageChanged;
        _settings.TranslationTranslator.SettingChanged -= OnTranslationTranslatorChanged;
        _settings.TranslationLibreTranslateUrl.SettingChanged += OnLibreTranslateUrlChanged;
    }

    public async Task<string> TranslateAsync(string text)
    {
        var translation = await _translator.TranslateAsync(text).ConfigureAwait(false);
        return !string.IsNullOrWhiteSpace(translation) ? translation : "";
    }

    private void CreateTranslator(Translators translator, TranslatorConfig config)
    {
        _translator?.Dispose();
        _translator = translator switch
        {
            // Translators.DeepL => new DeepLTranslator(config),
            Translators.Google => new Google(config),
            Translators.Google2 => new Google2(config),
            Translators.Yandex => new Yandex(config),
            Translators.LibreTranslate => new LibreTranslate(config),
            _ => null
        };
    }

    private void OnTranslationLanguageChanged(object sender, ValueChangedEventArgs<int> e)
    {
        _translatorConfig.TargetLang = LanguagesInfo.GetByLanguage(e.NewValue);
    }

    private void OnTranslationTranslatorChanged(object sender, ValueChangedEventArgs<int> e)
    {
        CreateTranslator((Translators)e.NewValue, _translatorConfig);
    }

    private void OnLibreTranslateUrlChanged(object sender, ValueChangedEventArgs<string> e)
    {
        if (_settings.TranslationTranslator.Value == (int)Translators.LibreTranslate)
            _translatorConfig.ApiUrl = e.NewValue;
    }
}