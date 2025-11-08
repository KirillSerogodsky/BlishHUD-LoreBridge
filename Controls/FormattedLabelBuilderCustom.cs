using System;
using System.Collections.Generic;

namespace LoreBridge.Controls;

public class FormattedLabelBuilderCustom
{
    private readonly List<FormattedLabelPartCustom> _parts = [];
    private bool _wrapText;
    private int _width;
    private int _height;
    private bool _autoSizeHeight;
    private bool _autoSizeWidth;
    private bool _showShadow;

    public FormattedLabelPartBuilderCustom CreatePart(string text) => new(text);

    public FormattedLabelBuilderCustom CreatePart(string text,
        Action<FormattedLabelPartBuilderCustom> creationFunc)
    {
        var builder = new FormattedLabelPartBuilderCustom(text);
        creationFunc?.Invoke(builder);
        _parts.Add(builder.Build());
        return this;
    }

    public FormattedLabelBuilderCustom Wrap()
    {
        _wrapText = true;
        return this;
    }

    public FormattedLabelBuilderCustom SetWidth(int width)
    {
        _autoSizeWidth = false;
        _width = width;
        return this;
    }

    public FormattedLabelBuilderCustom AutoSizeHeight()
    {
        _height = 0;
        _autoSizeHeight = true;
        return this;
    }

    public FormattedLabelBuilderCustom ShowShadow()
    {
        _showShadow = true;
        return this;
    }

    public FormattedLabelCustom Build()
        => new(_parts, _wrapText, _autoSizeWidth, _autoSizeHeight, _showShadow)
        {
            Width = _width,
            Height = _height,
        };
}