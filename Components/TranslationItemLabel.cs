using System.Linq;
using System.Text;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Components;

public class TranslationItemLabel : Label
{
    private readonly string _originalText;

    public TranslationItemLabel(string text, BitmapFont font) {
        Text = WrapTextCustom(font, text, Width);
        AutoSizeHeight = true;
        Font = font;
        ShowShadow = true;

        _originalText = text;
    }

    public void RerenderText(int width)
    {
        Width = width;
        Text = WrapTextCustom(Font, _originalText, width);
    }

    private string WrapTextSegment(string text, float maxLineWidth)
    {
        string[] array = text.Split(' ');
        StringBuilder stringBuilder = new();
        float num = 0f;
        float width = 11f; // font.MeasureString(" ").Width;
        string[] array2 = array;
        foreach (string text2 in array2)
        {
            Vector2 vector = new(text2.Length * 11f, 0); ; // font.MeasureString(text2);
            if (num + vector.X < maxLineWidth)
            {
                stringBuilder.Append(text2 + " ");
                num += vector.X + width;
            }
            else
            {
                stringBuilder.Append("\n" + text2 + " ");
                num = vector.X + width;
            }
        }

        return stringBuilder.ToString();
    }

    private string WrapTextCustom(BitmapFont spriteFont, string text, float maxLineWidth)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }

        return string.Join(
            "\n",
            from s in text.Split('\n') select WrapTextSegment(s, maxLineWidth)
        );
    }
}