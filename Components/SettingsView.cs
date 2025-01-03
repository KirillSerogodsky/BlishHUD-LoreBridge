using System;
using System.Collections.Generic;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using LoreBridge.Enums;
using LoreBridge.Models;
using Microsoft.Xna.Framework;
using Panel = Blish_HUD.Controls.Panel;

namespace LoreBridge.Components;

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
            HorizontalScrollOffset = 16,
        };

        var generalPanel = new FlowPanel
        {
            Parent = _settingsPanel,
            Title = "General",
            CanCollapse = true,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(6, 6),
            ControlPadding = new Vector2(6, 6),
            ShowBorder = true
        };

        var languagePanel = new FlowPanel
        {
            Parent = generalPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        new Label
        {
            Parent = languagePanel,
            Text = "Translation language",
            ShowShadow = true,
            Height = 28,
            Width = 180
        };

        var languageDropdown = new Dropdown
        {
            Parent = languagePanel,
            Width = 160,
            SelectedItem = Enum.GetName(typeof(Languages), settings.TranslationLanguage.Value)
        };
        foreach (var item in Enum.GetNames(typeof(Languages))) languageDropdown.Items.Add(item);
        languageDropdown.ValueChanged += (o, e) =>
        {
            settings.TranslationLanguage.Value = (int)Enum.Parse(typeof(Languages), e.CurrentValue);
        };

        var translatorPanel = new FlowPanel
        {
            Parent = generalPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        new Label
        {
            Parent = translatorPanel,
            Text = "Translator",
            ShowShadow = true,
            Height = 28,
            Width = 180
        };

        var translatorDropdownItems = new List<string>
        {
            "DeepL",
            "Google (Simple)",
            "Google (Advanced)",
            "Yandex"
        };
        var translatorDropdown = new Dropdown
        {
            Parent = translatorPanel,
            Width = 160,
            SelectedItem = translatorDropdownItems[settings.TranslationTranslator.Value]
        };
        foreach (var item in translatorDropdownItems) translatorDropdown.Items.Add(item);
        translatorDropdown.ValueChanged += (o, e) =>
        {
            settings.TranslationTranslator.Value = (int)Enum.Parse(typeof(Translators),
                translatorDropdownItems.IndexOf(e.CurrentValue).ToString());
        };

        var autoTranslateNpcDialoguesPanel = new FlowPanel
        {
            Parent = generalPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        new Label
        {
            Parent = autoTranslateNpcDialoguesPanel,
            Text = "Auto translate NPC dialogues",
            BasicTooltipText = "ArcDPS, ArcDPS Unofficial Extras, ArcDPS Blish HUD plugin must be installed",
            ShowShadow = true,
            Height = 16,
            Width = 180
        };

        var autoTranslateNpcDialoguesCheckbox = new Checkbox
        {
            Parent = autoTranslateNpcDialoguesPanel,
            Checked = settings.TranslationAutoTranslateNpcDialogues.Value,
            Height = 16
        };
        autoTranslateNpcDialoguesCheckbox.CheckedChanged += (o, e) =>
            settings.TranslationAutoTranslateNpcDialogues.Value = e.Checked;
        
        var translationWindowPanel = new FlowPanel
        {
            Parent = _settingsPanel,
            Title = "Translation Window",
            CanCollapse = true,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(6, 6),
            ControlPadding = new Vector2(6, 6),
            ShowBorder = true
        };
        
        var fontSizePanel = new FlowPanel
        {
            Parent = translationWindowPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        new Label
        {
            Parent = fontSizePanel,
            Text = "Font size",
            ShowShadow = true,
            Height = 16,
            Width = 180
        };

        var fontSizeTrackBar = new TrackBar ()
        {
            Parent = fontSizePanel,
            MinValue = 16,
            MaxValue = 32,
            Width = 160,
            Value = settings.WindowFontSize.Value,
        };

        var fontSizeLabel = new Label()
        {
            Parent = fontSizePanel,
            Text = settings.WindowFontSize.Value.ToString(),
            ShowShadow = true,
            Height = 16,
            AutoSizeWidth = true,
            TextColor = Color.Gray,
        };

        fontSizeTrackBar.ValueChanged += (o, e) =>
        {
            settings.WindowFontSize.Value = (int)e.Value;
            fontSizeLabel.Text = settings.WindowFontSize.Value.ToString();
        };

        var keyBindPanel = new FlowPanel
        {
            Parent = _settingsPanel,
            Title = "Key Binds",
            CanCollapse = true,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(6, 6),
            ControlPadding = new Vector2(6, 6),
            ShowBorder = true
        };

        new KeybindingAssigner(settings.ToggleCapturerHotkey.Value)
        {
            Parent = keyBindPanel,
            KeyBindingName = settings.ToggleCapturerHotkey.DisplayName,
            BasicTooltipText = settings.ToggleCapturerHotkey.Description
        };

        new KeybindingAssigner(settings.ToggleTranslationWindowHotKey.Value)
        {
            Parent = keyBindPanel,
            KeyBindingName = settings.ToggleTranslationWindowHotKey.DisplayName,
            BasicTooltipText = settings.ToggleTranslationWindowHotKey.Description,
            Enabled = false
        };

        base.Build(buildPanel);
    }

    protected override void Unload()
    {
        _settingsPanel.Dispose();
        base.Unload();
    }
}