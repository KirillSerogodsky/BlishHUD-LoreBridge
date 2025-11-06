using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using LoreBridge.Models;
using LoreBridge.Views.SettingsView.Controls;
using Microsoft.Xna.Framework;
using Panel = Blish_HUD.Controls.Panel;

namespace LoreBridge.Views.SettingsView;

public class SettingsView(Settings settings) : View
{
    private Panel _settingsPanel;
    private Policy _policyView;

    protected override void Build(Container buildPanel)
    {
        _settingsPanel = new FlowPanel
        {
            Parent = buildPanel,
            CanCollapse = false,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(22, 6),
            ControlPadding = new Vector2(6, 6),
            HorizontalScrollOffset = 16
        };

        if (!settings.ContentUsePolicyConfirmation.Value)
        {
            _policyView = new Policy(_settingsPanel, settings);
            settings.ContentUsePolicyConfirmation.SettingChanged += PolicyValueChanged;
        }
        else
        {
            LoadSettings();
        }

        base.Build(buildPanel);
    }

    protected override void Unload()
    {
        _settingsPanel.Dispose();
        base.Unload();
    }

    private void LoadSettings()
    {
        var general = new General(_settingsPanel, settings);
        var area = new Area(_settingsPanel, settings);
        var chat = new Chat(_settingsPanel, settings);
        // var cutscenes = new Cutscenes(_settingsPanel, settings);
    }

    private void PolicyValueChanged(object sender, ValueChangedEventArgs<bool> e)
    {
        if (!e.NewValue) return;
        _policyView.Dispose();
        LoadSettings();
        settings.ContentUsePolicyConfirmation.SettingChanged -= PolicyValueChanged;
    }
}