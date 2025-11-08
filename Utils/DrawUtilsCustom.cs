using System.Linq;
using System.Text;
using FontStashSharp;

namespace LoreBridge;

public static class DrawUtilCustom
{
    private static string WrapTextSegment(SpriteFontBase spriteFont, string text, float maxLineWidth)
    {
        var isCjk = text.Any(IsCjk);
        var sb = new StringBuilder();

        if (isCjk)
        {
            var lineWidth = 0f;

            foreach (var character in text)
            {
                var charWidth = spriteFont.MeasureString(character.ToString()).X;

                if (lineWidth + charWidth > maxLineWidth)
                {
                    sb.Append("\n");
                    lineWidth = 0f;
                }

                sb.Append(character);
                lineWidth += charWidth;
            }
        }
        else
        {
            var words = text.Split(' ');
            var lineWidth = 0f;
            var spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (var word in words)
            {
                var wordWidth = spriteFont.MeasureString(word).X;

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
        }

        return sb.ToString();
    }

    private static bool IsCjk(char c)
    {
        return (c >= 0x4E00 && c <= 0x9FFF) || // CJK Unified Ideographs
               (c >= 0x3040 && c <= 0x309F) || // Hiragana
               (c >= 0x30A0 && c <= 0x30FF) || // Katakana
               (c >= 0xFF00 && c <= 0xFFEF); // Full-width characters
    }

    public static string WrapText(SpriteFontBase spriteFont, string text, float maxLineWidth)
    {
        return string.IsNullOrEmpty(text)
            ? ""
            : string.Join("\n", text.Split('\n').Select(s => WrapTextSegment(spriteFont, s, maxLineWidth)));
    }
}