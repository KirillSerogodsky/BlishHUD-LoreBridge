using System;
using Blish_HUD;
using FontStashSharp;
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
    private SettingsModel _settings;
    private ScreenCapturer _screenCapturer;
    private TranslationPanel _translationPanel;
    private DynamicSpriteFont _font;

    public override void Load(SettingsModel settings)
    {
        _settings = settings;
        _screenCapturer = new ScreenCapturer(_settings);
        _font = Fonts.FontSystem.GetFont(18);
        _translationPanel = new TranslationPanel(_font);
        
        _screenCapturer.ScreenCaptured += OnScreenCaptured;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Unload()
    {
        _screenCapturer.ScreenCaptured -= OnScreenCaptured;
        _screenCapturer.Dispose();
        _translationPanel.Dispose();
    }

    private async void OnScreenCaptured(object o, Rectangle rectangle)
    {
        string[] result = [];

        try
        {
            var bitmap = Screen.GetScreen(rectangle);
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

        var scale = GameService.Graphics.UIScaleMultiplier;
        _translationPanel.Top = (int)(rectangle.Top / scale);
        _translationPanel.Left = (int)(rectangle.Left / scale);
        _translationPanel.Width = (int)(rectangle.Width / scale);
        _translationPanel.Height = (int)(rectangle.Height / scale);
        _translationPanel.Text = translation;
        _translationPanel.Visible = true;
    }
}