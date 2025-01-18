using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using FontStashSharp;
using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Components;

public sealed class TranslationWindow : ChatWindow
{
    private readonly StandardButton _clearButton;
    private readonly TranslationScrollPanel _panel;
    private readonly SettingsModel _settings;
    private bool _preventSaveVisible;

    public TranslationWindow(SettingsModel settings,
        TranslationListModel translationList,
        SpriteFontBase font)
    {
        Parent = GameService.Graphics.SpriteScreen;
        Location = new Point(settings.WindowLocationX.Value, settings.WindowLocationY.Value);
        Height = settings.WindowHeight.Value;
        Width = settings.WindowWidth.Value;
        CanClose = true;
        CanCloseWithEscape = false;
        CanResize = !settings.WindowFixed.Value;
        Title = "Translation";
        Transparent = settings.WindowTransparent.Value;

        _settings = settings;
        _panel = new TranslationScrollPanel(translationList, font) { Parent = this };

        _settings.ToggleTranslationWindowHotKey.Value.Enabled = true;
        _settings.ToggleTranslationWindowHotKey.Value.Activated += OnToggleHotKey;
        _settings.WindowFixed.SettingChanged += OnFixedChanged;
        _settings.WindowTransparent.SettingChanged += OnTransparentChange;
        GameService.Gw2Mumble.UI.IsMapOpenChanged += OnIsMapOpenChanged;

        _clearButton = new StandardButton
        {
            Parent = this,
            Text = "Clear",
            Width = 42,
            Height = 20,
            Top = 0,
            Right = settings.WindowWidth.Value,
            Visible = false
        };
        _clearButton.Click += delegate { translationList.Clear(); };

        if (settings.WindowVisible.Value) Show();
    }

    private void OnFixedChanged(object sender, ValueChangedEventArgs<bool> e)
    {
        CanResize = !e.NewValue;
    }

    private void OnTransparentChange(object sender, ValueChangedEventArgs<bool> e)
    {
        Transparent = e.NewValue;
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
            if (_settings.WindowFixed.Value)
            {
                Location = new Point(_settings.WindowLocationX.Value, _settings.WindowLocationY.Value);
                return;
            }

            _settings.WindowLocationX.Value = e.CurrentLocation.X;
            _settings.WindowLocationY.Value = e.CurrentLocation.Y;
        }

        base.OnMoved(e);
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        _panel?.SaveScroll();

        if (_clearButton != null) _clearButton.Right = e.CurrentSize.X;

        if (_settings != null)
        {
            _settings.WindowWidth.Value = e.CurrentSize.X;
            _settings.WindowHeight.Value = e.CurrentSize.Y;
        }

        base.OnResized(e);
    }

    protected override void OnMouseEntered(MouseEventArgs e)
    {
        if (_clearButton != null) _clearButton.Visible = true;
        if (_settings != null && _settings.WindowTransparent.Value) Transparent = false;
        base.OnMouseEntered(e);
    }

    protected override void OnMouseLeft(MouseEventArgs e)
    {
        if (_clearButton != null) _clearButton.Visible = false;
        if (_settings != null && _settings.WindowTransparent.Value) Transparent = true;
        base.OnMouseLeft(e);
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
        _settings.WindowFixed.SettingChanged -= OnFixedChanged;
        GameService.Gw2Mumble.UI.IsMapOpenChanged -= OnIsMapOpenChanged;

        base.DisposeControl();
    }
}