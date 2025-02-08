using System;
using System.Drawing;
using Blish_HUD;
using LoreBridge.Components;
using LoreBridge.Models;

namespace LoreBridge.Modules.AreaTranslation.Controls;

internal class ScreenCapturer : IDisposable
{
    private readonly OverlayForm _overlay = new();
    private readonly SettingsModel _settings;

    public ScreenCapturer(SettingsModel settings)
    {
        _settings = settings;
        _settings.ToggleCapturerHotkey.Value.Enabled = true;

        _settings.ToggleCapturerHotkey.Value.Activated += CaptureScreen;
        GameService.GameIntegration.Gw2Instance.Gw2LostFocus += LostFocus;
        _overlay.AreaSelected += OnAreaSelected;
    }

    public void Dispose()
    {
        _overlay.Dispose();
        _settings.ToggleCapturerHotkey.Value.Enabled = false;
        _settings.ToggleCapturerHotkey.Value.Activated -= CaptureScreen;
        GameService.GameIntegration.Gw2Instance.Gw2LostFocus -= LostFocus;
    }

    public event EventHandler<Rectangle> ScreenCaptured;

    private void OnAreaSelected(object o, Rectangle e)
    {
        _overlay.Hide();
        ScreenCaptured?.Invoke(this, e);
    }

    private void LostFocus(object o, EventArgs e)
    {
        _overlay.Hide();
    }

    private void CaptureScreen(object sender, EventArgs e)
    {
        _overlay.Show();
    }
}