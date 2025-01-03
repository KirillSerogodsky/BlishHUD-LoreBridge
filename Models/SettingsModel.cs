﻿using Blish_HUD.Input;
using Blish_HUD.Settings;
using LoreBridge.Enums;

namespace LoreBridge.Models;

public class SettingsModel(SettingCollection settings)
{
    public readonly SettingEntry<KeyBinding> ToggleCapturerHotkey = settings.DefineSetting("Hotkeys.ToggleCapturer",
        new KeyBinding(),
        () => "Select translation area");

    public readonly SettingEntry<KeyBinding> ToggleTranslationWindowHotKey = settings.DefineSetting(
        "Hotkeys.ToggleTranslationWindow",
        new KeyBinding(),
        () => "Toggle translation window",
        () => "Not implemented");

    public readonly SettingEntry<bool> TranslationAutoTranslateNpcDialogues =
        settings.DefineSetting("Translation.AutoTranslateNpcDialogues", true);
    public readonly SettingEntry<int> TranslationLanguage =
        settings.DefineSetting("Translation.Language", (int)Languages.Russian);
    public readonly SettingEntry<int> TranslationTranslator =
        settings.DefineSetting("Translation.Translator", (int)Translators.Google2);
        
    public readonly SettingEntry<int> WindowHeight = settings.DefineSetting("Window.Height", 240);
    public readonly SettingEntry<int> WindowWidth = settings.DefineSetting("Window.Width", 480);
    public readonly SettingEntry<int> WindowLocationX = settings.DefineSetting("Window.Location.X", 200);
    public readonly SettingEntry<int> WindowLocationY = settings.DefineSetting("Window.Location.Y", 200);
    public readonly SettingEntry<bool> WindowVisible = settings.DefineSetting("Window.Visible", false);
    public readonly SettingEntry<int> WindowFontSize = settings.DefineSetting("Window.FontSize", 20);
    public readonly SettingEntry<bool> WindowColoredNames = settings.DefineSetting("Window.ColoredNames", true);
}