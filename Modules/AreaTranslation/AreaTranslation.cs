using System;
using System.Threading.Tasks;
using Blish_HUD;
using FontStashSharp;
using LoreBridge.Components;
using LoreBridge.Models;
using LoreBridge.Modules.AreaTranslation.Controls;
using LoreBridge.Resources;
using LoreBridge.Services;
using LoreBridge.Utils;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace LoreBridge.Modules.AreaTranslation;

public class AreaTranslation : Module
{
    private readonly OverlayForm _overlay = new();
    private DynamicSpriteFont _font;
    private SettingsModel _settings;
    private TranslationWindow _translationWindow;

    public override void Load(SettingsModel settings)
    {
        _settings = settings;
        _font = Fonts.FontSystem.GetFont(_settings.AreaFontSize.Value);
        _translationWindow = new TranslationWindow(_font);

        _settings.ToggleCapturerHotkey.Value.Enabled = true;
        _settings.ToggleCapturerHotkey.Value.Activated += CaptureScreen;
        GameService.GameIntegration.Gw2Instance.Gw2LostFocus += LostFocus;
        _overlay.AreaSelected += OnAreaSelected;
        _settings.AreaFontSize.SettingChanged += OnFontSizeChanged;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Unload()
    {
        _settings.ToggleCapturerHotkey.Value.Enabled = false;
        _settings.ToggleCapturerHotkey.Value.Activated -= CaptureScreen;
        GameService.GameIntegration.Gw2Instance.Gw2LostFocus -= LostFocus;
        _overlay.AreaSelected -= OnAreaSelected;
        _settings.AreaFontSize.SettingChanged -= OnFontSizeChanged;
        _translationWindow.Dispose();
    }

    private void OnFontSizeChanged(object sender, ValueChangedEventArgs<int> e)
    {
        _font = Fonts.FontSystem.GetFont(e.NewValue);
        _translationWindow.UpdateFont(_font);
    }

    private void CaptureScreen(object sender, EventArgs e)
    {
        _translationWindow.Hide();
        _overlay.Show();
    }

    private void LostFocus(object o, EventArgs e)
    {
        _overlay.Hide();
    }

    private void OnAreaSelected(object o, Rectangle e)
    {
        _overlay.Hide();
        _ = ProcessTranslationAsync(e);
    }

    private async Task ProcessTranslationAsync(Rectangle rectangle)
    {
        string[] result = [];
        var bitmap = Screen.GetScreen(rectangle);

        try
        {
            result = Service.Ocr.GetTextLines(bitmap);
        }
        catch (Exception e)
        {
            //
        }

        if (result.Length <= 0) return;

        var text = "";
        for (var i = 0; i < result.Length; i++)
        {
            var row = result[i];
            if (row.EndsWith(".") && i != result.Length - 1) text += $"{row}\n";
            else text += row;
        }

        var translation = "";
        try
        {
            translation = await Service.Translation.TranslateAsync(text);
        }
        catch (Exception e)
        {
            //
        }

        if (string.IsNullOrEmpty(translation)) return;

        var factor = GameService.Graphics.UIScaleMultiplier;
        _translationWindow.Top = (int)(rectangle.Top / factor);
        _translationWindow.Left = (int)(rectangle.Left / factor);
        _translationWindow.Size = new Point((int)(rectangle.Width / factor), (int)(rectangle.Height / factor));
        _translationWindow.Text = translation;
        _translationWindow.Show();
    }
}