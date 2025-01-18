using System;
using System.Linq;
using System.Text;
using FontStashSharp;

namespace LoreBridge;

public static class DrawUtil2
{
    public static string WrapTextSegment(SpriteFontBase spriteFont, string text, float maxLineWidth)
    {
        // var words = text.Contains(' ') ? text.Split(' ') : text.ToArray().Select(c => c.ToString());
        var words = text.Split(' ');
        var sb = new StringBuilder();
        var lineWidth = 0f;
        var spaceWidth = spriteFont.MeasureString("  ").X - spriteFont.MeasureString(" ").X;
        var aWidth = spriteFont.MeasureString("a").X;

        foreach (var word in words)
        {
            var wordWidth = spriteFont.MeasureString("a" + word).X - aWidth;
            wordWidth = Math.Max(wordWidth, spriteFont.MeasureString(word + "a").X - aWidth);
            
            if (lineWidth + wordWidth < maxLineWidth)
            {
                sb.Append(word + " ");
                lineWidth += wordWidth + spaceWidth;
            }
            else
            {
                sb.Append("\n" + word + " ");
                lineWidth = wordWidth + spaceWidth;
            }
        }

        return sb.ToString();
    }

    public static string WrapText(SpriteFontBase spriteFont, string text, float maxLineWidth)
    {
        return string.IsNullOrEmpty(text)
            ? ""
            : string.Join("\n", text.Split('\n').Select(s => WrapTextSegment(spriteFont, s, maxLineWidth)));
    }
}