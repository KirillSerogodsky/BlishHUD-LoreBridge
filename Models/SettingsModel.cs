using Blish_HUD.Input;
using Blish_HUD.Settings;

namespace LoreBridge.Models
{
    public class SettingsModel
    {
        public readonly SettingEntry<KeyBinding> ToggleCapturerHotkey;
        public readonly SettingEntry<KeyBinding> ToggleTranslationWindowHotKey;

        public readonly SettingEntry<bool> WindowVisible;
        public readonly SettingEntry<int> WindowLocationX;
        public readonly SettingEntry<int> WindowLocationY;
        public readonly SettingEntry<int> WindowWidth;
        public readonly SettingEntry<int> WindowHeight;

        public SettingsModel(SettingCollection settings)
        {
            // Key binds
            ToggleCapturerHotkey = settings.DefineSetting("Hotkeys.ToggleCapturer",
                                                          new KeyBinding(),
                                                          () => "Select translation area");
            ToggleTranslationWindowHotKey = settings.DefineSetting("Hotkeys.ToggleTranslationWindow", 
                                                                   new KeyBinding(),
                                                                   () => "Toggle translation window",
                                                                   () => "Not implemented");
            
            // Translation window
            WindowVisible = settings.DefineSetting("Window.Visible", false);
            WindowLocationX = settings.DefineSetting("Window.Location.X", 200);
            WindowLocationY = settings.DefineSetting("Window.Location.Y", 200);
            WindowWidth = settings.DefineSetting("Window.Width", 480);
            WindowHeight = settings.DefineSetting("Window.Height", 240);
        }
    }
}