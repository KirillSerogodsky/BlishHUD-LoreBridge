using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using LoreBridge.Controls;
using LoreBridge.Models;
using LoreBridge.Modules;
using LoreBridge.Modules.AreaTranslation;
using LoreBridge.Modules.Chat;
using LoreBridge.Modules.CutsceneSubtitles;
using LoreBridge.Modules.DialogSubtitles;
using LoreBridge.Resources;
using LoreBridge.Services;
using LoreBridge.Views.SettingsView;
using Microsoft.Xna.Framework;
using BlishHUDModule = Blish_HUD.Modules.Module;
using BlishHUDModuleParameters = Blish_HUD.Modules.ModuleParameters;

namespace LoreBridge;

[Export(typeof(BlishHUDModule))]
public class LoreBridge : BlishHUDModule
{
    private readonly List<Module> _modules = [];
    private CornerIcon _cornerIcon;
    private SettingsModel _settings;

    [ImportingConstructor]
    public LoreBridge([Import("ModuleParameters")] BlishHUDModuleParameters moduleParameters) : base(moduleParameters)
    {
    }

    internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;

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
        Fonts.Initialize(ContentsManager);
        Textures.Initialize(ContentsManager);
        _cornerIcon = new CornerIcon();
        LoadServices();
        LoadModules();
    }

    protected override void Update(GameTime gameTime)
    {
        foreach (var service in Service.All) service.Update(gameTime);
        foreach (var module in _modules) module.Update(gameTime);
    }

    protected override void Unload()
    {
        foreach (var module in _modules) module.Unload();
        foreach (var service in Service.All) service.Unload();
        _cornerIcon.Dispose();
        Textures.Dispose();
        Fonts.Dispose();
    }

    private void LoadModules()
    {
        _modules.Add(new AreaTranslation());
        _modules.Add(new Chat(_cornerIcon));
        _modules.Add(new CutsceneSubtitles());
        _modules.Add(new DialogSubtitles());
        foreach (var module in _modules) module.Load(_settings);
    }

    private void LoadServices()
    {
        foreach (var service in Service.All) service.Load(_settings);
    }
}