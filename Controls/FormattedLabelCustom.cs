using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LoreBridge.Controls;

public class FormattedLabelCustom : Control
{
    private readonly List<(RectangleWrapper Rectangle, FormattedLabelPartCustom Text, object ToDraw)> _rectangles = [];

    private readonly IEnumerable<FormattedLabelPartCustom> _parts;
    private readonly bool _wrapText;
    private readonly bool _autoSizeWidth;
    private readonly bool _autoSizeHeight;
    private readonly bool _showShadow;
    private bool _finishedInitialization;

    internal FormattedLabelCustom(
        IEnumerable<FormattedLabelPartCustom> parts,
        bool wrapText,
        bool autoSizeWidth,
        bool autoSizeHeight,
        bool showShadow)
    {
        _parts = parts;
        _wrapText = wrapText;
        _autoSizeWidth = autoSizeWidth;
        _autoSizeHeight = autoSizeHeight;
        _showShadow = showShadow;
    }

    public SpriteFontBase Font
    {
        set
        {
            foreach (var part in _parts)
            {
                part.Font =  value;
            }
            RecalculateLayout();
        }
    }

    public override void RecalculateLayout() => InitializeRectangles();

    private Rectangle HandleFirstTextPart(FormattedLabelPartCustom item, string firstText)
    {
        var textSize = item.Font.MeasureString(firstText);
        var rectangle = new Rectangle(
            0,
            0,
            (int)Math.Floor(textSize.X) + 1,
            item.Font.FontSize
        );

        if (_rectangles.Count <= 0) return rectangle;

        var lastRectangle = _rectangles[_rectangles.Count - 1];
        rectangle.X = lastRectangle.Rectangle.X + lastRectangle.Rectangle.Width;
        rectangle.Y = lastRectangle.Rectangle.Y;

        return rectangle;
    }

    private Rectangle HandleMultiLineText(FormattedLabelPartCustom item, string text)
    {
        var textSize = item.Font.MeasureString(text);
        var possibleLastYRectangles = _rectangles
            .OrderByDescending(x => x.Rectangle.Y)
            .GroupBy(x => x.Rectangle.Y).First();
        var lastYRectangle = possibleLastYRectangles.FirstOrDefault(x => x.Rectangle.Height != default);

        if (lastYRectangle == default)
        {
            lastYRectangle = possibleLastYRectangles.First();
        }

        return new Rectangle(
            0,
            lastYRectangle.Rectangle.Y + lastYRectangle.Rectangle.Height,
            (int)Math.Floor(textSize.X) + 1, 
            item.Font.FontSize
        );
    }

    private void InitializeRectangles()
    {
        // No need to initialize anything if there is no space
        if (Width == 0 && !_autoSizeWidth)
        {
            return;
        }

        _finishedInitialization = false;
        _rectangles.Clear();
        foreach (var item in _parts)
        {
            var splittedText = item.Text.Split(["\n"], StringSplitOptions.None).ToList();
            var firstText = splittedText[0];
            var rectangle = HandleFirstTextPart(item, firstText);
            var wrapped = false;
            if (_wrapText && rectangle.X + rectangle.Width > Width)
            {
                var tempSplittedText = DrawUtilCustom
                    .WrapText(item.Font, firstText, Width - rectangle.X)
                    .Split(["\n"], StringSplitOptions.None)
                    .ToList();
                splittedText = new[]
                    {
                        string.Join("", tempSplittedText.Skip(1))
                    }
                    .Concat(splittedText.Skip(1))
                    .ToList();
                firstText = tempSplittedText[0];
                rectangle = HandleFirstTextPart(item, firstText);
                wrapped = true;
            }

            _rectangles.Add((new RectangleWrapper(rectangle), item, firstText));

            for (var i = wrapped ? 0 : 1; i < splittedText.Count; i++)
            {
                rectangle = HandleMultiLineText(item, splittedText[i]);
                if (_wrapText && rectangle.X + rectangle.Width > Width)
                {
                    splittedText.InsertRange(
                        i + 1,
                        DrawUtilCustom
                            .WrapText(item.Font, splittedText[i], Width - rectangle.X)
                            .Split(["\n"], StringSplitOptions.RemoveEmptyEntries));
                    splittedText.RemoveAt(i);

                    var newRectangle = HandleMultiLineText(item, splittedText[i]);

                    if (newRectangle == rectangle)
                    {
                        return;
                    }

                    rectangle = newRectangle;
                }

                _rectangles.Add((new RectangleWrapper(rectangle), item, splittedText[i]));
            }
        }

        if (_autoSizeWidth)
        {
            Width = _rectangles.GroupBy(x => x.Rectangle.Y).Select(x => x.Select(y => y.Rectangle.Width).Sum()).Max();
        }

        if (_autoSizeHeight)
        {
            Height = _rectangles.GroupBy(x => x.Rectangle.Y).Select(x => x.Max(x => x.Rectangle.Height)).Sum();
        }

        // Needs to be done after vertical alignment bc it will change the height of the individual rectangles
        // and therefor can't be recognized as one row anymore
        HandleFontSizeDifferences();

        _finishedInitialization = true;
    }

    private void HandleFontSizeDifferences()
    {
        var rows = _rectangles.GroupBy(x => x.Rectangle.Y).ToArray();

        foreach (var item in rows)
        {
            var maxHeightInRowRectangle = item.OrderByDescending(x => x.Rectangle.Height).First();

            foreach (var rectangle in item)
            {
                var offset = maxHeightInRowRectangle.Rectangle.Height - rectangle.Rectangle.Height;
                rectangle.Rectangle.Y += (int)Math.Floor(offset / 2.0);
            }
        }
    }

    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
        if (!_finishedInitialization) return;

        var absoluteOpacity = AbsoluteOpacity();

        foreach (var rectangle in _rectangles)
        {
            var destinationRectangle = rectangle.Rectangle.Rectangle.ToBounds(AbsoluteBounds);
            var textColor = rectangle.Text.TextColor;

            switch (rectangle.ToDraw)
            {
                case string stringText:
                    if (_showShadow)
                    {
                        spriteBatch.DrawStringOnCtrl(
                            this,
                            stringText,
                            rectangle.Text.Font,
                            rectangle.Rectangle.Rectangle.OffsetBy(1, 1),
                            Color.Black,
                            _wrapText,
                            false,
                            1,
                            HorizontalAlignment.Left,
                            VerticalAlignment.Middle
                        );
                    }

                    spriteBatch.DrawStringOnCtrl(
                        this,
                        stringText,
                        rectangle.Text.Font,
                        rectangle.Rectangle.Rectangle,
                        textColor,
                        _wrapText,
                        false,
                        1,
                        HorizontalAlignment.Left,
                        VerticalAlignment.Middle
                    );
                    break;
                case AsyncTexture2D texture:
                    spriteBatch.Draw(texture, destinationRectangle, Color.White * absoluteOpacity);
                    break;
            }
        }
    }

    protected override void DisposeControl()
    {
        foreach (var item in _parts)
        {
            item.Dispose();
        }

        base.DisposeControl();
    }

    private class RectangleWrapper(Rectangle rectangle)
    {
        public Rectangle Rectangle { get; set; } = rectangle;

        public int X
        {
            get => Rectangle.X;
            set
            {
                var rectangle = Rectangle;
                rectangle.X = value;
                Rectangle = rectangle;
            }
        }

        public int Y
        {
            get => Rectangle.Y;
            set
            {
                var rectangle = Rectangle;
                rectangle.Y = value;
                Rectangle = rectangle;
            }
        }

        public int Width
        {
            get => Rectangle.Width;
            set
            {
                var rectangle = Rectangle;
                rectangle.Width = value;
                Rectangle = rectangle;
            }
        }

        public int Height
        {
            get => Rectangle.Height;
            set
            {
                var rectangle = Rectangle;
                rectangle.Height = value;
                Rectangle = rectangle;
            }
        }
    }
}