using Blish_HUD.Controls;
using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Views.SettingsView.Controls;

public class Chat
{
    public Chat(Panel mainPanel, Settings settings)
    {
        var translationWindowPanel = new FlowPanel
        {
            Parent = mainPanel,
            Title = "Chat",
            CanCollapse = true,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(6, 6),
            ControlPadding = new Vector2(6, 6),
            ShowBorder = true
        };

        var autoTranslateNpcDialoguesPanel = new FlowPanel
        {
            Parent = translationWindowPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var autoTranslateNpcDialoguesLabel = new Label
        {
            Parent = autoTranslateNpcDialoguesPanel,
            Text = "Auto translate NPC dialogs",
            BasicTooltipText = "ArcDPS, ArcDPS Unofficial Extras, ArcDPS Blish HUD plugin must be installed",
            ShowShadow = true,
            Height = 16,
            Width = 180
        };

        var autoTranslateNpcDialoguesCheckbox = new Checkbox
        {
            Parent = autoTranslateNpcDialoguesPanel,
            Checked = settings.TranslationAutoTranslateNpcDialogs.Value,
            Height = 16
        };
        autoTranslateNpcDialoguesCheckbox.CheckedChanged += (o, e) =>
            settings.TranslationAutoTranslateNpcDialogs.Value = e.Checked;

        var fontSizePanel = new FlowPanel
        {
            Parent = translationWindowPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var fontSizeLabel = new Label
        {
            Parent = fontSizePanel,
            Text = "Font size",
            ShowShadow = true,
            Height = 16,
            Width = 180
        };

        var fontSizeTrackBar = new TrackBar
        {
            Parent = fontSizePanel,
            MinValue = 16,
            MaxValue = 32,
            Width = 160,
            Value = settings.WindowFontSize.Value
        };

        var fontSizeCurrentLabel = new Label
        {
            Parent = fontSizePanel,
            Text = settings.WindowFontSize.Value.ToString(),
            ShowShadow = true,
            Height = 16,
            AutoSizeWidth = true,
            TextColor = Color.Gray
        };

        fontSizeTrackBar.ValueChanged += (o, e) =>
        {
            settings.WindowFontSize.Value = (int)e.Value;
            fontSizeCurrentLabel.Text = settings.WindowFontSize.Value.ToString();
        };

        var fixedWindowPanel = new FlowPanel
        {
            Parent = translationWindowPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var fixedWindowLabel = new Label
        {
            Parent = fixedWindowPanel,
            Text = "Fixed",
            ShowShadow = true,
            Height = 16,
            Width = 180,
            BasicTooltipText = "Prevents the window from resizing and moving"
        };

        var fixedWindowCheckbox = new Checkbox
        {
            Parent = fixedWindowPanel,
            Checked = settings.WindowFixed.Value,
            Height = 16,
            BasicTooltipText = "Prevents the window from resizing and moving"
        };
        fixedWindowCheckbox.CheckedChanged += (o, e) =>
            settings.WindowFixed.Value = e.Checked;

        var transparentWindowPanel = new FlowPanel
        {
            Parent = translationWindowPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var transparentWindowLabel = new Label
        {
            Parent = transparentWindowPanel,
            Text = "Transparent",
            ShowShadow = true,
            Height = 16,
            Width = 180
        };

        var transparentWindowCheckbox = new Checkbox
        {
            Parent = transparentWindowPanel,
            Checked = settings.WindowTransparent.Value,
            Height = 16
        };
        transparentWindowCheckbox.CheckedChanged += (o, e) =>
            settings.WindowTransparent.Value = e.Checked;

        var coloredNamesPanel = new FlowPanel
        {
            Parent = translationWindowPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var coloredNamesLabel = new Label
        {
            Parent = coloredNamesPanel,
            Text = "Colored names",
            ShowShadow = true,
            Height = 16,
            Width = 180
        };

        var coloredNamesCheckbox = new Checkbox
        {
            Parent = coloredNamesPanel,
            Checked = settings.WindowColoredNames.Value,
            Height = 16
        };
        coloredNamesCheckbox.CheckedChanged += (o, e) =>
            settings.WindowColoredNames.Value = e.Checked;

        var showTimePanel = new FlowPanel
        {
            Parent = translationWindowPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var showTimeLabel = new Label
        {
            Parent = showTimePanel,
            Text = "Show time",
            ShowShadow = true,
            Height = 16,
            Width = 180
        };

        var showTimeCheckbox = new Checkbox
        {
            Parent = showTimePanel,
            Checked = settings.WindowShowTime.Value,
            Height = 16
        };
        showTimeCheckbox.CheckedChanged += (o, e) =>
            settings.WindowShowTime.Value = e.Checked;

        var toggleTranslationWindowKeybindingAssigner =
            new KeybindingAssigner(settings.ToggleTranslationWindowHotKey.Value)
            {
                Parent = translationWindowPanel,
                KeyBindingName = settings.ToggleTranslationWindowHotKey.DisplayName,
                BasicTooltipText = settings.ToggleTranslationWindowHotKey.Description
            };
    }
}