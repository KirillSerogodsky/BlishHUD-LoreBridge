using System;
using System.Drawing;
using Blish_HUD;
using LoreBridge.Models;

namespace LoreBridge.Components;

internal class ScreenCapturer : IDisposable
{
    private readonly SettingsModel _settings;
    private readonly OverlayForm _overlay;

    public event EventHandler<Rectangle> ScreenCaptured;

    public ScreenCapturer(SettingsModel settings)
    {
        _settings = settings;
        _settings.ToggleCapturerHotkey.Value.Enabled = true;
        _settings.ToggleCapturerHotkey.Value.Activated += CaptureScreen;
        GameService.GameIntegration.Gw2Instance.Gw2LostFocus += LostFocus;

        _overlay = new OverlayForm();
        /* _overlay.OnCanceled += (o, e) =>
        {
            _overlay.Hide();
            Task.Run(async () =>
            {
                await Task.Delay(50);
                GameService.GameIntegration.Gw2Instance.FocusGw2();
            });
        }; */
        _overlay.AreaSelected += (o, e) => {
            _overlay.Hide();
            ScreenCaptured.Invoke(this, e);
            GameService.GameIntegration.Gw2Instance.FocusGw2();
        };
    }

    private void LostFocus(object o, EventArgs e)
    {
        _overlay?.Hide();
    }

    private void CaptureScreen(object sender, EventArgs e)
    {
        _overlay.Show();
        // _overlay.Focus();
    }

    public void Dispose()
    {
        _overlay.Dispose();
        _settings.ToggleCapturerHotkey.Value.Enabled = false;
        _settings.ToggleCapturerHotkey.Value.Activated -= CaptureScreen;
        GameService.GameIntegration.Gw2Instance.Gw2LostFocus -= LostFocus;
    }
}