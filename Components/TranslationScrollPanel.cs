using Blish_HUD.Controls;
using FontStashSharp;
using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Components;

public sealed class TranslationScrollPanel : Panel
{
    private const int ScrollBarWidth = 12;
    private const int ScrollBarOffsetLeft = 3;
    private const int ScrollBarOffsetRight = 5;
    private const int ScrollBarOffsetY = 10;
    private readonly Scrollbar2 _scrollBar;

    private readonly TranslationPanel _scrollPanel;

    public TranslationScrollPanel(MessagesModel messages, SpriteFontBase font)
    {
        WidthSizingMode = SizingMode.Fill;
        HeightSizingMode = SizingMode.Fill;

        _scrollPanel = new TranslationPanel(messages, font)
        {
            Parent = this,
            Location = new Point(ScrollBarOffsetLeft + ScrollBarOffsetRight + ScrollBarWidth, 0)
        };
        _scrollBar = new Scrollbar2(_scrollPanel)
        {
            Parent = this,
            Location = new Point(ScrollBarOffsetLeft, ScrollBarOffsetY),
            ScrollDistance = 1f
        };
    }

    public void UpdateFont(SpriteFontBase font)
    {
        _scrollPanel.UpdateFont(font);
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        base.OnResized(e);

        if (_scrollBar != null)
        {
            _scrollBar.Height = Height - ScrollBarOffsetY * 2;
            _scrollBar.Location = new Point(ScrollBarOffsetLeft, ScrollBarOffsetY);
        }

        if (_scrollPanel != null)
        {
            _scrollPanel.Height = Height;
            _scrollPanel.Width = Width - ScrollBarOffsetLeft - ScrollBarOffsetRight - ScrollBarWidth;
            _scrollPanel.Location = new Point(ScrollBarOffsetLeft + ScrollBarOffsetRight + ScrollBarWidth, 0);
        }
    }
}