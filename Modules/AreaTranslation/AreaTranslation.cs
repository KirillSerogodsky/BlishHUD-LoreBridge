using System;
using LoreBridge.Models;
using LoreBridge.Modules.AreaTranslation.Controls;
using LoreBridge.Services;
using LoreBridge.Utils;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace LoreBridge.Modules.AreaTranslation;

public class AreaTranslation : Module
{
    private ScreenCapturer _screenCapturer;
    private SettingsModel _settings;

    public override void Load(SettingsModel settings)
    {
        _settings = settings;
        _screenCapturer = new ScreenCapturer(_settings);

        _screenCapturer.ScreenCaptured += OnScreenCaptured;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Unload()
    {
        _screenCapturer.Dispose();
        _screenCapturer.ScreenCaptured -= OnScreenCaptured;
    }

    private void OnScreenCaptured(object o, Rectangle rectangle)
    {
        string[] result = [];

        try
        {
            var bitmap = Screen.GetScreen(rectangle);
            result = Service.Ocr.GetTextLines(bitmap);
        }
        catch (Exception exception)
        {
            /* _messages.Add(new MessageEntry
            {
                Text = exception.Message
            }); */
        }

        if (result.Length <= 0) return;

        var text = "";
        for (var i = 0; i < result.Length; i++)
        {
            var row = result[i];
            if (row.EndsWith(".") && i != result.Length - 1) text += $"{row}\n";
            else text += row;
        }

        /* _translationService.Add(new MessageEntry
        {
            Text = text
        }); */
    }
}