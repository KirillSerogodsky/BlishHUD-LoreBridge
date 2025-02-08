using FontStashSharp;
using LoreBridge.Models;
using LoreBridge.Modules.CutsceneSubtitles.Services;
using LoreBridge.Resources;
using LoreBridge.Services;
using LoreBridge.Services.GameState;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.CutsceneSubtitles;

public class CutsceneSubtitles : Module
{
    private CutsceneSubtitlesService _cutsceneSubtitlesService;
    private SpriteFontBase _font;
    private SettingsModel _settings;

    public override void Load(SettingsModel settings)
    {
        _settings = settings;
        _font = Fonts.FontSystem.GetFont(_settings.WindowFontSize.Value);
        // _cutsceneSubtitlesService = new CutsceneSubtitlesService(_ocrEngine, _font);
        Service.GameState.GameStateChanged += OnGameStateChanged;
    }

    public override void Update(GameTime gameTime)
    {
        // _cutsceneSubtitlesService.Run(gameTime);
    }

    public override void Unload()
    {
        // _cutsceneSubtitlesService.Dispose();
        Service.GameState.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(object o, GameStateType e)
    {
        // _cutsceneSubtitlesService.Enabled = e == GameStateType.Cutscene;
    }
}