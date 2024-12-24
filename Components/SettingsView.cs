using System;
using System.Collections.Generic;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using LoreBridge.Enums;
using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Components;

public class SettingsView(SettingsModel settings) : View
{
    protected override void Build(Container buildPanel)
    {
        var mainPanel = new FlowPanel
        {
            Parent = buildPanel,
            CanCollapse = false,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(6, 6),
            ControlPadding = new Vector2(6, 6)
        };

        var generalPanel = new FlowPanel
        {
            Parent = mainPanel,
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
            AutoSizeWidth = true,
            ShowShadow = true,
            Height = 28
        };

        var languageDropdown = new Dropdown
        {
            Parent = languagePanel,
            Width = 170,
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
            AutoSizeWidth = true,
            ShowShadow = true,
            Height = 28
        };

        var translatorDropdownItems = new List<string>
        {
            "DeepL",
            "Google (Simple)",
            "Google (Advanced)"
        };
        var translatorDropdown = new Dropdown
        {
            Parent = translatorPanel,
            Width = 140,
            SelectedItem = translatorDropdownItems[settings.TranslationTranslator.Value]
        };
        foreach (var item in translatorDropdownItems) translatorDropdown.Items.Add(item);
        translatorDropdown.ValueChanged += (o, e) =>
        {
            settings.TranslationTranslator.Value = (int)Enum.Parse(typeof(Translators),
                translatorDropdownItems.IndexOf(e.CurrentValue).ToString());
        };

        var keyBindPanel = new FlowPanel
        {
            Parent = mainPanel,
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
        base.Unload();
    }
}