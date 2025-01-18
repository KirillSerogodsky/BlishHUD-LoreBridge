using System.Threading.Tasks;
using Blish_HUD.Controls;
using FontStashSharp;
using LoreBridge.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LoreBridge.Components;

public sealed class TranslationScrollPanel : FlowPanel
{
    private const int ScrollBarWidth = 12;
    private const int ScrollBarOffsetX = 4;
    private const int ScrollBarOffsetY = 10;
    private readonly Scrollbar _scrollBar;

    private readonly TranslationPanel _scrollPanel;
    private float? _scrollTarget;

    public TranslationScrollPanel(TranslationListModel translationList, SpriteFontBase font)
    {
        FlowDirection = ControlFlowDirection.SingleTopToBottom;
        WidthSizingMode = SizingMode.Fill;
        HeightSizingMode = SizingMode.Fill;

        _scrollPanel = new TranslationPanel(translationList, font)
        {
            Parent = this,
            Location = new Point(ScrollBarOffsetX * 2 + ScrollBarWidth, 0)
        };
        _scrollBar = new Scrollbar(_scrollPanel)
        {
            Parent = this,
            Location = new Point(ScrollBarOffsetX, ScrollBarOffsetY)
        };

        translationList.Added += (o, e) =>
        {
            Task.Run(async () =>
            {
                await Task.Delay(50);
                _scrollBar.ScrollDistance = 1;
            });
        };
    }

    public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
    {
        base.PaintBeforeChildren(spriteBatch, bounds);
        if (_scrollTarget.HasValue)
        {
            var factor = _scrollPanel.Height - _scrollPanel.ContentBounds.Y;
            _scrollBar.ScrollDistance = factor != 0 ? _scrollTarget.Value / factor : 0;
            _scrollTarget = null;
        }

        _scrollPanel.Location = new Point(ScrollBarOffsetX * 2 + ScrollBarWidth, 0);
    }

    public void SaveScroll()
    {
        if (_scrollBar != null)
            _scrollTarget = _scrollBar.ScrollDistance * (_scrollPanel.Height - _scrollPanel.ContentBounds.Y);
    }

    public void UpdateFont(SpriteFontBase font)
    {
        _scrollPanel.UpdateFont(font);
    }

    private void ResizeComponents()
    {
        if (_scrollBar != null)
        {
            _scrollBar.Height = Height - ScrollBarOffsetY * 2;
            _scrollBar.Location = new Point(ScrollBarOffsetX, ScrollBarOffsetY);
        }

        if (_scrollPanel != null)
        {
            _scrollPanel.Height = Height;
            _scrollPanel.Width = Width - ScrollBarOffsetX * 2 - ScrollBarWidth;
            _scrollPanel.Location = new Point(ScrollBarOffsetX * 2 + ScrollBarWidth, 0);
        }
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        ResizeComponents();
        base.OnResized(e);
    }
}