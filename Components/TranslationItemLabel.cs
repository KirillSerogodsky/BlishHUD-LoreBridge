using System.Linq;
using System.Text;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Components;

public class TranslationItemLabel : Label
{
    private readonly string _originalText;

    public TranslationItemLabel(string text, BitmapFont font)
    {
        Text = WrapText(font, text, Width);
        AutoSizeHeight = true;
        Font = font;
        ShowShadow = true;

        _originalText = text;
    }

    public void RerenderText(int width)
    {
        Width = width;
        Text = WrapText(Font, _originalText, width);
    }

    private static string WrapTextSegment(BitmapFont spriteFont, string text, float maxLineWidth)
    {
        var words = text.Split(' ');
        var sb = new StringBuilder();
        var lineWidth = 0f;
        var spaceWidth = spriteFont.MeasureString("_").Width;

        foreach (var word in words)
        {
            Vector2 size = spriteFont.MeasureString(word);

            if (lineWidth + size.X < maxLineWidth)
            {
                sb.Append(word + " ");
                lineWidth += size.X + spaceWidth;
            }
            else
            {
                sb.Append("\n" + word + " ");
                lineWidth = size.X + spaceWidth;
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