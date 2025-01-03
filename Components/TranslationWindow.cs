using System;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using LoreBridge.Models;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Components;

public sealed class TranslationWindow : StandardWindow
{
    private readonly StandardButton _clearButton;
    private readonly TranslationScrollPanel _panel;
    private readonly SettingsModel _settings;
    private bool _preventSaveVisible;

    public TranslationWindow(SettingsModel settings,
        TranslationListModel translationList,
        BitmapFont font)
        : base(
            AsyncTexture2D.FromAssetId(155139),
            new Rectangle(0, 0, 514, 532),
            new Rectangle(0, 12, 514, 520)
        )
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

        if (settings.WindowVisible.Value) Show();

        GameService.Gw2Mumble.UI.IsMapOpenChanged += (o, e) =>
        {
            if (e.Value)
            {
                _preventSaveVisible = true;
                Hide();
            }

            if (!e.Value && _settings != null && _settings.WindowVisible.Value)
            {
                _preventSaveVisible = false;
                Show();
            }
        };

        _clearButton = new StandardButton
        {
            Parent = this,
            Text = "Clear",
            Width = 42,
            Right = Width - 15,
            Top = -2,
            Height = 20
        };
        _clearButton.Click += delegate { translationList.Clear(); };
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

        if (_settings == null)
        {
            base.OnResized(e);
            return;
        }

        _settings.WindowWidth.Value = e.CurrentSize.X;
        _settings.WindowHeight.Value = e.CurrentSize.Y;

        base.OnResized(e);
    }
}