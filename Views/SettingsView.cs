using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using LoreBridge.Models;
using LoreBridge.Translation.Language;
using LoreBridge.Translation.Translators;
using Microsoft.Xna.Framework;
using Panel = Blish_HUD.Controls.Panel;

namespace LoreBridge.Views;

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

        var languageLabel = new Label
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
            SelectedItem = LanguagesInfo.GetByLanguage(settings.TranslationLanguage.Value)?.Name
        };
        foreach (var languageDetail in LanguagesInfo.List.OrderBy(ld => ld.Name))
            languageDropdown.Items.Add(languageDetail.Name);

        languageDropdown.ValueChanged += (o, e) =>
        {
            var selectedLanguage = LanguagesInfo.GetByName(e.CurrentValue);
            if (selectedLanguage?.Language != null)
                settings.TranslationLanguage.Value = (int)selectedLanguage.Language;
        };

        var translatorPanel = new FlowPanel
        {
            Parent = generalPanel,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var translatorLabel = new Label
        {
            Parent = translatorPanel,
            Text = "Translator",
            ShowShadow = true,
            Height = 28,
            Width = 180
        };

        var translatorDropdownItems = new List<string>
        {
            // "DeepL",
            "Google (Simple)",
            "Google (Advanced)",
            "Yandex",
            "LibreTranslate"
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

        var translationAreaPanel = new FlowPanel
        {
            Parent = _settingsPanel,
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

        var translationWindowPanel = new FlowPanel
        {
            Parent = _settingsPanel,
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

        var cutscenesPanel = new FlowPanel
        {
            Parent = _settingsPanel,
            Title = "Cutscenes (Experimental)",
            CanCollapse = true,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(6, 6),
            ControlPadding = new Vector2(6, 6),
            ShowBorder = true
        };

        base.Build(buildPanel);
    }

    protected override void Unload()
    {
        _settingsPanel.Dispose();
        base.Unload();
    }
}