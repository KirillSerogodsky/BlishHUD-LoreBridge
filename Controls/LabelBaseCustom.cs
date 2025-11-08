using System;
using Blish_HUD;
using Blish_HUD.Controls;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LoreBridge.Controls;

public abstract class LabelBaseCustom : Control
{
    protected bool _autoSizeHeight = false;
    protected bool _autoSizeWidth = false;
    protected SpriteFontBase _font;
    protected HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;

    protected Color _shadowColor = Color.Black;
    protected bool _showShadow = false;
    protected bool _strokeText = false;
    protected string _text;
    protected Color _textColor = Color.White;
    protected VerticalAlignment _verticalAlignment = VerticalAlignment.Middle;
    protected bool _wrapText = false;

    /// <summary>
    ///     If either <see cref="_autoSizeWidth" /> or <see cref="_autoSizeHeight" /> is enabled,
    ///     this will indicate the size of the label region after <see cref="RecalculateLayout" />
    ///     has completed.
    /// </summary>
    protected Point LabelRegion = Point.Zero;

    public override void RecalculateLayout()
    {
        var lblRegionWidth = _size.X;
        var lblRegionHeight = _size.Y;

        if (_autoSizeWidth || _autoSizeHeight)
        {
            var textSize = GetTextDimensions();

            if (_autoSizeWidth)
                lblRegionWidth = (int)Math.Ceiling(textSize.Width + (_showShadow || _strokeText ? 1 : 0));

            if (_autoSizeHeight)
                lblRegionHeight = (int)Math.Ceiling(textSize.Height + (_showShadow || _strokeText ? 1 : 0));
        }

        LabelRegion = new Point(lblRegionWidth, lblRegionHeight);
    }

    protected Size2 GetTextDimensions(string text = null)
    {
        text = text ?? _text;

        if (_font == null) return new Size2(0, 0);

        if (!_autoSizeWidth && _wrapText)
            text = DrawUtilCustom.WrapText(_font, text, LabelRegion.X > 0 ? LabelRegion.X : _size.X);

        return _font.MeasureString(text ?? _text);
    }

    protected void DrawText(SpriteBatch spriteBatch, Rectangle bounds, string text = null)
    {
        text = text ?? _text;

        if (_font == null || string.IsNullOrEmpty(text)) return;

        if (_showShadow && !_strokeText)
            spriteBatch.DrawStringOnCtrl(this, text, _font, bounds.OffsetBy(1, 1), _shadowColor, _wrapText,
                _horizontalAlignment, _verticalAlignment);

        spriteBatch.DrawStringOnCtrl(this, text, _font, bounds, _textColor, _wrapText, _strokeText, 1,
            _horizontalAlignment, _verticalAlignment);
    }

    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
        DrawText(spriteBatch, bounds, _text);
    }
}