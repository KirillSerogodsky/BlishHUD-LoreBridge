using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Drawing;

using Blish_HUD;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;

using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

using LoreBridge.Components;
using LoreBridge.Models;
using LoreBridge.OCR;
using LoreBridge.Translation;
using LoreBridge.Translation.DeepL;
using LoreBridge.Enums;

namespace LoreBridge
{
    [Export(typeof(Module))]
    public class LoreBridgeModule : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<LoreBridgeModule>();

        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;

        private SettingsModel _settings;
        private TranslationListModel _translationList;
        private LoreBridgeCornerIcon _cornerIcon;
        private TranslationWindow _translationWindow;
        private ScreenCapturer _screenCapturer;
        private BitmapFont _font;
        private WindowsOCR _ocrEngine;
        private ITranslator _translator;

        [ImportingConstructor]
        public LoreBridgeModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
        }

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
            _font = ContentsManager.GetBitmapFont("fonts/OpenSans-Medium.ttf", 24);
            _translationList = new TranslationListModel();
            _translationWindow = new TranslationWindow(_settings, _translationList, _font);

            _cornerIcon = new LoreBridgeCornerIcon(ContentsManager);
            _cornerIcon.Click += (o, e) => { _translationWindow.ToggleWindow(); };

            TranslatorConfig translatorConfig = new()
            {
                TargetLang = Languages.Russian,
                Translator = Translators.DeepL,
            };
            _translator = new DeepLTranslator(translatorConfig);

            _ocrEngine = new WindowsOCR();
            _screenCapturer = new ScreenCapturer(_settings);
            _screenCapturer.ScreenCaptured += (o, e) =>
            {
                Bitmap bitmap = Utils.Screen.GetScreen(e);
                var result = _ocrEngine.GetTextLines(bitmap);
                if (result.Length > 0) {
                    _ = TranslateTextAsync(string.Join(" ", result));
                }
            };
        }

        private async Task TranslateTextAsync(string text)
        {
            var translation = await _translator.TranslateAsync(text);
            if (!string.IsNullOrWhiteSpace(translation))
            {
                _translationList.Add(translation);
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
    }
}