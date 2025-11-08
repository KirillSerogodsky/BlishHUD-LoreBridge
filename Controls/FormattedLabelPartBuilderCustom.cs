using FontStashSharp;
using Microsoft.Xna.Framework;

namespace LoreBridge.Controls;

public class FormattedLabelPartBuilderCustom
{
    private SpriteFontBase _font;
    private readonly string _text;
    private Color _textColor;

    internal FormattedLabelPartBuilderCustom(string text)
    {
        _text = text;
    }
    
    public FormattedLabelPartBuilderCustom SetFont(SpriteFontBase font)
    {
        _font = font;
        return this;
    }

    public FormattedLabelPartBuilderCustom SetTextColor(Color textColor)
    {
        _textColor = textColor;
        return this;
    }

    internal FormattedLabelPartCustom Build() => new(_font, _text, _textColor);
}