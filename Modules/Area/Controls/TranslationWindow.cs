using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using FontStashSharp;
using LoreBridge.Controls;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.Area.Controls;

public sealed class TranslationWindow : SimpleWindow
{
    private readonly LabelCustom _label;
    private readonly Scrollbar _scroll;
    private readonly LoadingSpinner _loading;
    private bool _isLoading;

    public TranslationWindow(SpriteFontBase font)
    {
        Parent = GameService.Graphics.SpriteScreen;
        TopMost = true;

        var panel = new Panel
        {
            Parent = this,
            BackgroundColor = Color.Black * 0.9f,
            CanScroll = false,
            CanCollapse = false,
            Collapsed = false,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.Fill
        };
        _scroll = new Scrollbar(panel)
        {
            Parent = this,
            Right = 0,
            Top = 0
        };
        _label = new LabelCustom
        {
            Parent = panel,
            Font = font,
            WrapText = true,
            TextColor = Color.White,
            AutoSizeHeight = true,
            Location = new Point(4, 0)
        };
        _label.Hide();
        _loading = new LoadingSpinner
        {
            Parent = panel,
        };
        _loading.Hide();

        Click += OnClick;
    }

    public string Text
    {
        set => _label.Text = value;
    }

    public bool Loading
    {
        set
        {
            _isLoading = value;
            if (_isLoading)
            {
                _loading.Show();
                _label.Hide();
            }
            else
            {
                _loading.Hide();
                _label.Show();
            }
        }
    }

    public void UpdateFont(SpriteFontBase font)
    {
        _label.Font = font;
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        base.OnResized(e);

        if (_label != null)
        {
            _label.Width = e.CurrentSize.X - 16 - 4;
        }

        if (_scroll != null)
        {
            _scroll.Height = e.CurrentSize.Y;
            _scroll.Right = e.CurrentSize.X;
        }

        if (_loading != null)
        {
            var width = e.CurrentSize.X - 16;
            var height = e.CurrentSize.Y;
            var size = Math.Min(Math.Min(width, 64), height);
            _loading.Size = new Point(size, size);
            _loading.Location = new Point(width / 2 - size / 2, height / 2 - size / 2);
        }
    }

    protected override void DisposeControl()
    {
        Click -= OnClick;
    }

    private void OnClick(object sender, MouseEventArgs e)
    {
        Hide();
    }
}