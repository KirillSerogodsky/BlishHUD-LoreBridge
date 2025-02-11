using Blish_HUD.Controls;
using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Views.SettingsView.Controls;

public class Area
{
    public Area(Panel mainPanel, SettingsModel settings)
    {
        var translationAreaPanel = new FlowPanel
        {
            Parent = mainPanel,
            Title = "Area",
            CanCollapse = true,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(6, 6),
            ControlPadding = new Vector2(6, 6),
            ShowBorder = true
        };

        var areaFontSizePanel = new FlowPanel
        {
            Parent = translationAreaPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var areaFontSizeLabel = new Label
        {
            Parent = areaFontSizePanel,
            Text = "Font size",
            ShowShadow = true,
            Height = 16,
            Width = 180
        };

        var areaFontSizeTrackBar = new TrackBar
        {
            Parent = areaFontSizePanel,
            MinValue = 16,
            MaxValue = 32,
            Width = 160,
            Value = settings.AreaFontSize.Value
        };

        var areaFontSizeCurrentLabel = new Label
        {
            Parent = areaFontSizePanel,
            Text = settings.AreaFontSize.Value.ToString(),
            ShowShadow = true,
            Height = 16,
            AutoSizeWidth = true,
            TextColor = Color.Gray
        };

        areaFontSizeTrackBar.ValueChanged += (o, e) =>
        {
            settings.AreaFontSize.Value = (int)e.Value;
            areaFontSizeCurrentLabel.Text = settings.AreaFontSize.Value.ToString();
        };

        var toggleCapturerHotKeybindingAssigner = new KeybindingAssigner(settings.ToggleCapturerHotkey.Value)
        {
            Parent = translationAreaPanel,
            KeyBindingName = settings.ToggleCapturerHotkey.DisplayName,
            BasicTooltipText = settings.ToggleCapturerHotkey.Description
        };
    }
}