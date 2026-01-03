using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using LoreBridge.Models;
using LoreBridge.Modules.Area;
using LoreBridge.Modules.Chat;
using LoreBridge.Resources;
using LoreBridge.Services;
using LoreBridge.Utils;
using LoreBridge.Views.SettingsView;
using Microsoft.Xna.Framework;
using BlishHUDModule = Blish_HUD.Modules.Module;
using BlishHUDModuleParameters = Blish_HUD.Modules.ModuleParameters;
using CornerIcon = LoreBridge.Controls.CornerIcon;
using Module = LoreBridge.Modules.Module;

namespace LoreBridge;

[Export(typeof(BlishHUDModule))]
public class LoreBridge : BlishHUDModule
{
    private readonly List<Module> _modules = [];
    private CornerIcon _cornerIcon;
    private Settings _settings;

    [ImportingConstructor]
    public LoreBridge([Import("ModuleParameters")] BlishHUDModuleParameters moduleParameters) : base(moduleParameters)
    {
    }

    #region Service Managers

    internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
    internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
    internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
    internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

    #endregion

    protected override void DefineSettings(SettingCollection settings)
    {
        _settings = new Settings(settings);
    }

    public override IView GetSettingsView()
    {
        return new SettingsView(_settings);
    }

    protected override async Task LoadAsync()
    {
        if (!_settings.ContentUsePolicyConfirmation.Value)
        {
            var result = await PolicyConfirmation.ShowPolicyConfirmationAsync();
            if (result)
            {
                _settings.ContentUsePolicyConfirmation.Value = true;
            }
            else
            {
                _settings.ContentUsePolicyConfirmation.SettingChanged += PolicyValueChanged;
            }
        }

        Init();
    }

    protected override void Update(GameTime gameTime)
    {
        if (!_settings.ContentUsePolicyConfirmation.Value)
        {
            return;
        }

        foreach (var service in Service.All) service.Update(gameTime);
        foreach (var module in _modules) module.Update(gameTime);
    }

    protected override void Unload()
    {
        if (!_settings.ContentUsePolicyConfirmation.Value)
        {
            _settings.ContentUsePolicyConfirmation.SettingChanged -= PolicyValueChanged;
            return;
        }

        foreach (var module in _modules) module.Unload();
        foreach (var service in Service.All) service.Unload();
        _cornerIcon.Dispose();
        Textures.Dispose();
        Fonts.Dispose();
    }

    private void Init()
    {
        if (!_settings.ContentUsePolicyConfirmation.Value)
        {
            return;
        }

        Fonts.Initialize(ContentsManager);
        Textures.Initialize(ContentsManager);
        _cornerIcon = new CornerIcon();
        LoadServices();
        LoadModules();
    }

    private void LoadModules()
    {
        _modules.Add(new Area());
        _modules.Add(new Chat(_cornerIcon));
        // _modules.Add(new CutsceneSubtitles());
        // _modules.Add(new DialogSubtitles());
        foreach (var module in _modules) module.Load(_settings);
    }

    private void LoadServices()
    {
        foreach (var service in Service.All) service.Load(_settings);
    }

    private void PolicyValueChanged(object sender, ValueChangedEventArgs<bool> e)
    {
        if (!e.NewValue) return;
        Init();
        _settings.ContentUsePolicyConfirmation.SettingChanged -= PolicyValueChanged;
    }
}