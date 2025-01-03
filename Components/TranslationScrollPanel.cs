using System.Threading.Tasks;
using Blish_HUD.Controls;
using LoreBridge.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Components;

public sealed class TranslationScrollPanel : FlowPanel
{
    private const int ScrollBarWidth = 15;
    private readonly Scrollbar _scrollBar;

    private readonly TranslationPanel _scrollPanel;
    private float? _scrollTarget;

    public TranslationScrollPanel(TranslationListModel translationList, BitmapFont font)
    {
        FlowDirection = ControlFlowDirection.SingleTopToBottom;
        WidthSizingMode = SizingMode.Fill;
        HeightSizingMode = SizingMode.Fill;

        _scrollPanel = new TranslationPanel(translationList, font) { Parent = this };
        _scrollBar = new Scrollbar(_scrollPanel) { Parent = this };

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
    }

    public void SaveScroll()
    {
        if (_scrollBar != null)
            _scrollTarget = _scrollBar.ScrollDistance * (_scrollPanel.Height - _scrollPanel.ContentBounds.Y);
    }

    private void ResizeComponents()
    {
        if (_scrollBar != null)
        {
            _scrollBar.Height = Height;
            _scrollBar.Location = new Point(Width - ScrollBarWidth, 0);
        }

        if (_scrollPanel != null)
        {
            _scrollPanel.Height = Height;
            _scrollPanel.Width = Width - ScrollBarWidth;
            _scrollPanel.Location = new Point(0, 0);
        }
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        ResizeComponents();
        base.OnResized(e);
    }
}