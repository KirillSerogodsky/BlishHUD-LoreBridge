using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Blish_HUD;
using FontStashSharp;
using LoreBridge.Models;
using LoreBridge.Modules.Area.Controls;
using LoreBridge.Modules.Area.Forms;
using LoreBridge.Resources;
using LoreBridge.Services;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;
using Screen = LoreBridge.Utils.Screen;

namespace LoreBridge.Modules.Area;

public class Area : Module
{
    private Settings _settings;
    private DynamicSpriteFont _font;
    private OverlayForm _overlay;
    private TranslationWindow _translationWindow;
    private bool _isOverlayActive = false;

    public override void Load(Settings settings)
    {
        _settings = settings;
        _font = Fonts.FontSystem.GetFont(_settings.AreaFontSize.Value);
        _translationWindow = new TranslationWindow(_font);

        _settings.ToggleCapturerHotkey.Value.Enabled = true;
        _settings.ToggleCapturerHotkey.Value.Activated += CaptureScreen;
        GameService.GameIntegration.Gw2Instance.Gw2LostFocus += LostFocus;
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
        if (_isOverlayActive) return;
        _translationWindow.Hide();
        ShowOverlay();
    }

    private void LostFocus(object o, EventArgs e)
    {
        if (!_isOverlayActive) return;
        HideOverlay();
    }

    private void OnAreaSelected(object o, Rectangle e)
    {
        _ = ProcessTranslationAsync(e);
    }

    private void OnFormClosed(object sender, FormClosedEventArgs e)
    {
        HideOverlay();
    }

    private void OnHide(object sender, bool e)
    {
        HideOverlay();
    }

    private void ShowOverlay()
    {
        _overlay = new OverlayForm();
        _overlay.AreaSelected += OnAreaSelected;
        _overlay.FormClosed += OnFormClosed;
        _overlay.Hidden += OnHide;
        _overlay.Show();
        _isOverlayActive = true;
    }

    private void HideOverlay()
    {
        _overlay.AreaSelected -= OnAreaSelected;
        _overlay.FormClosed -= OnFormClosed;
        _overlay.Hidden -= OnHide;
        _overlay.Dispose();
        _isOverlayActive = false;
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

        var factor = GameService.Graphics.UIScaleMultiplier;
        _translationWindow.Top = (int)(rectangle.Top / factor);
        _translationWindow.Left = (int)(rectangle.Left / factor);
        _translationWindow.Size = new Point((int)(rectangle.Width / factor), (int)(rectangle.Height / factor));
        _translationWindow.Loading = true;
        _translationWindow.Show();

        var translation = "";
        try
        {
            translation = await Service.Translation.TranslateAsync(text);
        }
        catch (Exception e)
        {
            _translationWindow.Loading = false;
            _translationWindow.Hide();
        }

        if (string.IsNullOrEmpty(translation))
        {
            _translationWindow.Loading = false;
            _translationWindow.Hide();
            return;
        };

        _translationWindow.Loading = false;
        _translationWindow.Text = translation;
    }
}