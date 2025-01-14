using System;
using System.Linq;
using System.Text;
using Blish_HUD.Controls;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Components;

public class TranslationItemLabel : Label
{
    private readonly string _originalText;
    private readonly BitmapFont _customFont;

    public TranslationItemLabel(string text, BitmapFont font)
    {
        Text = WrapText(font, text, Width);
        AutoSizeHeight = true;
        Font = font;
        ShowShadow = true;

        _originalText = text;
        _customFont = font;
    }

    public void RerenderText(int width)
    {
        Width = width;
        Text = WrapText(_customFont, _originalText, width);
    }

    private static string WrapTextSegment(BitmapFont spriteFont, string text, float maxLineWidth)
    {
        var words = text.Split(' ');
        var sb = new StringBuilder();
        var lineWidth = 0f;
        var spaceWidth = spriteFont.MeasureString("  ").Width - spriteFont.MeasureString(" ").Width;
        var aWidth = spriteFont.MeasureString("a").Width;

        foreach (var word in words)
        {
            var wordWidth = spriteFont.MeasureString("a" + word).Width - aWidth;
            wordWidth = Math.Max(wordWidth, spriteFont.MeasureString(word + "a").Width - aWidth);
           
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

    private new static string WrapText(BitmapFont spriteFont, string text, float maxLineWidth)
    {
        return string.IsNullOrEmpty(text)
            ? ""
            : string.Join("\n", text.Split('\n').Select(s => WrapTextSegment(spriteFont, s, maxLineWidth)));
    }
}