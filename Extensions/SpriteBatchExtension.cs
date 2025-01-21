using System.IO;
using Blish_HUD;
using Blish_HUD.Controls;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LoreBridge;

public static class SpriteBatchExtensions
{
    public static void DrawStringOnCtrl2(
        this SpriteBatch spriteBatch,
        Control ctrl,
        string text,
        SpriteFontBase font,
        Rectangle destinationRectangle,
        Color color,
        bool wrap = false,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment verticalAlignment = VerticalAlignment.Middle)
    {
        spriteBatch.DrawStringOnCtrl2(ctrl, text, font, destinationRectangle, color, wrap, false,
            horizontalAlignment: horizontalAlignment, verticalAlignment: verticalAlignment);
    }

    public static void DrawStringOnCtrl2(
        this SpriteBatch spriteBatch,
        Control ctrl,
        string text,
        SpriteFontBase font,
        Rectangle destinationRectangle,
        Color color,
        bool wrap,
        bool stroke,
        int strokeDistance = 1,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment verticalAlignment = VerticalAlignment.Middle)
    {
        if (string.IsNullOrEmpty(text))
            return;
        text = wrap ? DrawUtil2.WrapText(font, text, destinationRectangle.Width) : text;
        if (horizontalAlignment != HorizontalAlignment.Left && (wrap || text.Contains("\n")))
        {
            using (var stringReader = new StringReader(text))
            {
                string text1;
                for (var y = 0;
                     destinationRectangle.Height - y > 0 && (text1 = stringReader.ReadLine()) != null;
                     y += (int)font.MeasureString("A").Y)
                    spriteBatch.DrawStringOnCtrl2(ctrl, text1, font, destinationRectangle.Add(0, y, 0, 0), color, wrap,
                        stroke, strokeDistance, horizontalAlignment, verticalAlignment);
            }
        }
        else
        {
            var vector2_1 = font.MeasureString(text);
            destinationRectangle = destinationRectangle.ToBounds(ctrl.AbsoluteBounds);
            var x = destinationRectangle.X;
            var y = destinationRectangle.Y;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    x += destinationRectangle.Width / 2 - (int)vector2_1.X / 2;
                    break;
                case HorizontalAlignment.Right:
                    x += destinationRectangle.Width - (int)vector2_1.X;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Middle:
                    y += destinationRectangle.Height / 2 - (int)vector2_1.Y / 2;
                    break;
                case VerticalAlignment.Bottom:
                    y += destinationRectangle.Height - (int)vector2_1.Y;
                    break;
            }

            var vector2_2 = new Vector2(x, y);
            var num = ctrl.AbsoluteOpacity();
            if (stroke)
            {
                var color1 = Color.Black * num;
                spriteBatch.DrawString(font, text, vector2_2.OffsetBy(0.0f, -strokeDistance), color1);
                spriteBatch.DrawString(font, text, vector2_2.OffsetBy(strokeDistance, -strokeDistance), color1);
                spriteBatch.DrawString(font, text, vector2_2.OffsetBy(strokeDistance, 0.0f), color1);
                spriteBatch.DrawString(font, text, vector2_2.OffsetBy(strokeDistance, strokeDistance), color1);
                spriteBatch.DrawString(font, text, vector2_2.OffsetBy(0.0f, strokeDistance), color1);
                spriteBatch.DrawString(font, text, vector2_2.OffsetBy(-strokeDistance, strokeDistance), color1);
                spriteBatch.DrawString(font, text, vector2_2.OffsetBy(-strokeDistance, 0.0f), color1);
                spriteBatch.DrawString(font, text, vector2_2.OffsetBy(-strokeDistance, -strokeDistance), color1);
            }

            spriteBatch.DrawString(font, text, vector2_2, color * num);
        }
    }
}