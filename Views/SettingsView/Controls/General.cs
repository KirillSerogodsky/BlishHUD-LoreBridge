using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD.Controls;
using LoreBridge.Models;
using LoreBridge.Translation.Language;
using LoreBridge.Translation.Translators;
using Microsoft.Xna.Framework;

namespace LoreBridge.Views.SettingsView.Controls;

public class General
{
    public General(Panel mainPanel, SettingsModel settings)
    {
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

        var translators = new Dictionary<string, Translators>
        {
            // { "DeepL", Translators.DeepL },
            { "Google (Simple)", Translators.Google },
            { "Google (Advanced)", Translators.Google2 },
            { "Yandex", Translators.Yandex },
            { "LibreTranslate", Translators.LibreTranslate }
        };
        var translatorDropdown = new Dropdown
        {
            Parent = translatorPanel,
            Width = 160
        };

        var currentTranslator = (Translators)settings.TranslationTranslator.Value;
        var selectedItem = translators.FirstOrDefault(x => x.Value == currentTranslator).Key;
        translatorDropdown.SelectedItem = selectedItem;

        foreach (var item in translators.Keys)
        {
            translatorDropdown.Items.Add(item);
        }

        translatorDropdown.ValueChanged += (o, e) =>
        {
            if (translators.TryGetValue(e.CurrentValue, out var translator))
            {
                settings.TranslationTranslator.Value = (int)translator;
            }
        };

        var libreTranslate = new LibreTranslate(mainPanel, settings);
    }
}