using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using FontStashSharp;
using LoreBridge.Controls;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.AreaTranslation.Controls;

public sealed class TranslationWindow : SimpleWindow
{
    private readonly Label2 _label;
    private readonly Scrollbar _scroll;

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
        _label = new Label2
        {
            Parent = panel,
            Font = font,
            WrapText = true,
            TextColor = Color.White,
            AutoSizeHeight = true
        };

        Click += OnClick;
    }

    public string Text
    {
        set => _label.Text = value;
    }

    public void UpdateFont(SpriteFontBase font)
    {
        _label.Font = font;
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        base.OnResized(e);

        if (_label != null)
            _label.Width = e.CurrentSize.X - 16;

        if (_scroll != null)
        {
            _scroll.Height = e.CurrentSize.Y;
            _scroll.Right = e.CurrentSize.X;
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