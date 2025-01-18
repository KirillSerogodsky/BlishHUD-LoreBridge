using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.GameServices.ArcDps.Models.UnofficialExtras;
using Blish_HUD.GameServices.ArcDps.V2;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using FontStashSharp;
using LoreBridge.Components;
using LoreBridge.Enums;
using LoreBridge.Language;
using LoreBridge.Models;
using LoreBridge.OCR;
using LoreBridge.Translation;
using LoreBridge.Translation.Translators;
using LoreBridge.Utils;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace LoreBridge;

[Export(typeof(Module))]
public class LoreBridgeModule : Module
{
    private const string FontPath = "fonts/FiraSansCondensed-Medium.ttf";
    private const string FontTCPath = "fonts/NotoSansTC-Medium.ttf";
    private const string FontSCPath = "fonts/NotoSansTC-Medium.ttf";
    private const string FontJPPath = "fonts/NotoSansJP-Medium.ttf";
    private const string FontKRPath = "fonts/NotoSansKR-Medium.ttf";

    private LoreBridgeCornerIcon _cornerIcon;
    private SpriteFontBase _font;
    private FontSystem _fontSystem;
    private WindowsOcr _ocrEngine;
    private ScreenCapturer _screenCapturer;
    private SettingsModel _settings;
    private TranslationListModel _translationList;
    private TranslationWindow _translationWindow;
    private ITranslator _translator;
    private TranslatorConfig _translatorConfig;

    [ImportingConstructor]
    public LoreBridgeModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
    {
    }

    internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
    internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
    internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
    internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

    protected override void DefineSettings(SettingCollection settings)
    {
        _settings = new SettingsModel(settings);
    }

    public override IView GetSettingsView()
    {
        return new SettingsView(_settings);
    }

    protected override async Task LoadAsync()
    {
        var baseFontStream = ContentsManager.GetFileStream(FontPath);
        var krFontStream = ContentsManager.GetFileStream(FontKRPath);
        var tcFontStream = ContentsManager.GetFileStream(FontTCPath);
        var scFontStream = ContentsManager.GetFileStream(FontSCPath);
        var jpFontStream = ContentsManager.GetFileStream(FontJPPath);
        _fontSystem = new FontSystem();
        _fontSystem.AddFont(baseFontStream);
        _fontSystem.AddFont(krFontStream);
        _fontSystem.AddFont(tcFontStream);
        _fontSystem.AddFont(scFontStream);
        _fontSystem.AddFont(jpFontStream);
        _font = _fontSystem.GetFont(_settings.WindowFontSize.Value);

        _translationList = new TranslationListModel(_settings);
        _translationWindow = new TranslationWindow(_settings, _translationList, _font);
        _cornerIcon = new LoreBridgeCornerIcon(ContentsManager);
        _ocrEngine = new WindowsOcr();
        _screenCapturer = new ScreenCapturer(_settings);
        _translatorConfig = new TranslatorConfig
        {
            TargetLang = LanguageDetails.GetByLanguage(_settings.TranslationLanguage.Value)
        };
        CreateTranslator((Translators)_settings.TranslationTranslator.Value, _translatorConfig);

        _cornerIcon.Click += OnCornerIconClick;
        _screenCapturer.ScreenCaptured += OnScreenCaptured;
        _settings.WindowFontSize.SettingChanged += OnFontSizeChanged;
        _settings.TranslationLanguage.SettingChanged += OnTranslationLanguageChanged;
        _settings.TranslationTranslator.SettingChanged += OnTranslationTranslatorChanged;

        try
        {
            if (GameService.ArcDpsV2.Loaded)
                GameService.ArcDpsV2.RegisterMessageType<ChatMessageInfo>(MessageType.ChatMessage, OnNpcChatMessage);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    protected override void Update(GameTime gameTime)
    {
    }

    protected override void Unload()
    {
        _translationWindow.Dispose();
        _cornerIcon.Dispose();
        _screenCapturer.Dispose();
        _translator.Dispose();
    }

    private async Task TranslateTextAsync(string text, string name = null)
    {
        try
        {
            var translation = await _translator.TranslateAsync(text);
            if (!string.IsNullOrWhiteSpace(translation))
                _translationList.Add(translation, name);
        }
        catch (Exception e)
        {
            _translationList.Add(e.Message);
        }
    }

    private void CreateTranslator(Translators translator, TranslatorConfig config)
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
    }

    private void OnCornerIconClick(object o, MouseEventArgs e)
    {
        _translationWindow.ToggleWindow();
    }

    private void OnFontSizeChanged(object sender, ValueChangedEventArgs<int> e)
    {
        _font = _fontSystem.GetFont(e.NewValue);
        _translationWindow.UpdateFont(_font);
    }

    private void OnTranslationLanguageChanged(object sender, ValueChangedEventArgs<int> e)
    {
        _translatorConfig.TargetLang = LanguageDetails.GetByLanguage(e.NewValue);
    }

    private void OnTranslationTranslatorChanged(object sender, ValueChangedEventArgs<int> e)
    {
        CreateTranslator((Translators)e.NewValue, _translatorConfig);
    }

    private async Task OnNpcChatMessage(ChatMessageInfo chatMessage, CancellationToken cancellationToken)
    {
        if (_settings.TranslationAutoTranslateNpcDialogs.Value && chatMessage.ChannelId == 9999)
            await TranslateTextAsync(chatMessage.Text, chatMessage.CharacterName);
    }

    private void OnScreenCaptured(object o, Rectangle rectangle)
    {
        string[] result = [];

        try
        {
            var bitmap = Screen.GetScreen(rectangle);
            result = _ocrEngine.GetTextLines(bitmap);
        }
        catch (Exception exception)
        {
            _translationList.Add(exception.Message);
        }

        if (result.Length <= 0) return;

        _ = TranslateTextAsync(string.Join(" ", result));
    }
}