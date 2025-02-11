using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using LoreBridge.Models;
using LoreBridge.Views.SettingsView.Controls;
using Microsoft.Xna.Framework;
using Panel = Blish_HUD.Controls.Panel;

namespace LoreBridge.Views.SettingsView;

public class SettingsView(SettingsModel settings) : View
{
    private Panel _settingsPanel;

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

        var general = new General(_settingsPanel, settings);
        var area = new Area(_settingsPanel, settings);
        var chat = new Chat(_settingsPanel, settings);
        var cutscenes = new Cutscenes(_settingsPanel, settings);

        base.Build(buildPanel);
    }

    protected override void Unload()
    {
        _settingsPanel.Dispose();
        base.Unload();
    }
}