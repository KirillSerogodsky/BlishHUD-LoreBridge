using System;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace LoreBridge.Controls;

internal class FormattedLabelPartCustom(
    SpriteFontBase font,
    string text,
    Color textColor) : IDisposable
{
    public SpriteFontBase Font { get; set; } = font;

    public string Text { get; } = text;

    public Color TextColor { get; } = textColor == default ? Color.White : textColor;

    public void Dispose()
    {
    }
}