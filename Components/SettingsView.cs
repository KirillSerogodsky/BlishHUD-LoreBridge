﻿using System;
using System.Linq;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using LoreBridge.Enums;
using LoreBridge.Models;

namespace LoreBridge.Components
{
    public class SettingsView : View
    {
        private readonly SettingsModel _settings;

        public SettingsView(SettingsModel settings) => _settings = settings;

        protected override void Build(Container buildPanel)
        {
            var mainPanel = new FlowPanel()
            {
                Parent = buildPanel,
                CanCollapse = false,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(6, 6),
                ControlPadding = new Vector2(6, 6),
            };

            var generalPanel = new FlowPanel()
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

            var languagePanel = new FlowPanel()
            {
                Parent = generalPanel,
                FlowDirection = ControlFlowDirection.LeftToRight,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new Vector2(6, 0),
            };

            new Label()
            {
                Parent = languagePanel,
                Text = "Translation language",
                AutoSizeWidth = true,
                ShowShadow = true,
                Height = 28,
            };
            
            var languageDropdown = new Dropdown()
            {
                Parent = languagePanel,
                Enabled = false,
                Width = 120,
            };
            foreach (var item in Enum.GetValues(typeof(Languages)).Cast<Languages>())
            {
                languageDropdown.Items.Add(item.ToString());
            }

            var translatorPanel = new FlowPanel()
            {
                Parent = generalPanel,
                FlowDirection = ControlFlowDirection.LeftToRight,
                WidthSizingMode = SizingMode.Fill,
                HeightSizingMode = SizingMode.AutoSize,
                ControlPadding = new Vector2(6, 0),
            };

            new Label()
            {
                Parent = translatorPanel,
                Text = "Translator",
                AutoSizeWidth = true,
                ShowShadow = true,
                Height = 28,
            };

            var translatorDropdown = new Dropdown()
            {
                Parent = translatorPanel,
                Enabled = false,
                Width = 120,
            };
            foreach (var item in Enum.GetValues(typeof(Translators)).Cast<Translators>())
            {
                translatorDropdown.Items.Add(item.ToString());
            }

            var keyBindPanel = new FlowPanel()
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

            new KeybindingAssigner(_settings.ToggleCapturerHotkey.Value)
            {
                Parent = keyBindPanel,
                KeyBindingName = _settings.ToggleCapturerHotkey.DisplayName,
                BasicTooltipText = _settings.ToggleCapturerHotkey.Description
            };

            new KeybindingAssigner(_settings.ToggleTranslationWindowHotKey.Value)
            {
                Parent = keyBindPanel,
                KeyBindingName = _settings.ToggleTranslationWindowHotKey.DisplayName,
                BasicTooltipText = _settings.ToggleTranslationWindowHotKey.Description,
                Enabled = false,
            };

            base.Build(buildPanel);
        }

        protected override void Unload()
        {
            base.Unload();
        }
    }
}