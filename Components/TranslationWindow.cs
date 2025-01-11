using System;
using System.Diagnostics;
using Blish_HUD;
using Blish_HUD.Controls;
using LoreBridge.Models;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Components;

public sealed class TranslationWindow : ChatWindow
{
    private readonly StandardButton _clearButton;
    private readonly TranslationScrollPanel _panel;
    private readonly SettingsModel _settings;
    private bool _preventSaveVisible;

    public TranslationWindow(SettingsModel settings,
        TranslationListModel translationList,
        BitmapFont font)
    {
        Parent = GameService.Graphics.SpriteScreen;
        Location = new Point(settings.WindowLocationX.Value, settings.WindowLocationY.Value);
        Height = settings.WindowHeight.Value;
        Width = settings.WindowWidth.Value;
        CanClose = true;
        CanCloseWithEscape = false;
        CanResize = true;
        Title = "Translation";

        _settings = settings;
        _panel = new TranslationScrollPanel(translationList, font) { Parent = this };

        _settings.ToggleTranslationWindowHotKey.Value.Enabled = true;
        _settings.ToggleTranslationWindowHotKey.Value.Activated += OnToggleHotKey;
        GameService.Gw2Mumble.UI.IsMapOpenChanged += OnIsMapOpenChanged;

        if (settings.WindowVisible.Value) Show();

        /* _clearButton = new StandardButton
        {
            Parent = this,
            Text = "Clear",
            Width = 42,
            Right = Width - 15,
            Top = -2,
            Height = 20
        };
        _clearButton.Click += delegate { translationList.Clear(); }; */
    }

    protected override void OnShown(EventArgs e)
    {
        if (_settings != null) _settings.WindowVisible.Value = true;

        base.OnShown(e);
    }

    protected override void OnHidden(EventArgs e)
    {
        if (_settings != null && !_preventSaveVisible) _settings.WindowVisible.Value = false;

        base.OnHidden(e);
    }

    protected override void OnMoved(MovedEventArgs e)
    {
        if (_settings != null)
        {
            _settings.WindowLocationX.Value = e.CurrentLocation.X;
            _settings.WindowLocationY.Value = e.CurrentLocation.Y;
        }

        base.OnMoved(e);
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        _panel?.SaveScroll();

        if (_clearButton != null) _clearButton.Right = e.CurrentSize.X - 15;

        if (_settings != null)
        {
            _settings.WindowWidth.Value = e.CurrentSize.X;
            _settings.WindowHeight.Value = e.CurrentSize.Y;
        }

        base.OnResized(e);
    }

    private void OnIsMapOpenChanged(object o, ValueEventArgs<bool> e)
    {
        switch (e.Value)
        {
            case true:
                _preventSaveVisible = true;
                Hide();
                break;
            case false when _settings != null && _settings.WindowVisible.Value:
                _preventSaveVisible = false;
                Show();
                break;
        }
    }

    private void OnToggleHotKey(object o, EventArgs e)
    {
        ToggleWindow();
    }

    protected override void DisposeControl()
    {
        _settings.ToggleTranslationWindowHotKey.Value.Enabled = false;
        _settings.ToggleTranslationWindowHotKey.Value.Activated -= OnToggleHotKey;
        GameService.Gw2Mumble.UI.IsMapOpenChanged -= OnIsMapOpenChanged;
        
        base.DisposeControl();
    }
}