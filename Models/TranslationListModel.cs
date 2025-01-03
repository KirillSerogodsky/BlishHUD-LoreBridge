using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LoreBridge.Models;

public sealed class TranslationListModel
{
    private readonly Dictionary<string, Color> _colorPairs = new();
    private readonly Random _random = new();
    private readonly SettingsModel _settings;

    public TranslationListModel(SettingsModel settings)
    {
        _settings = settings;
    }

    private List<TranslationListItemModel> Value { get; } = [];

    public event EventHandler<TranslationListItemModel> Added;
    public event EventHandler Cleared;

    public void Add(string text, string name = "")
    {
        var item = new TranslationListItemModel
        {
            Name = name,
            NameColor = GetNameColor(name),
            Text = text
        };
        Value.Add(item);
        Added?.Invoke(this, item);
    }

    public void Clear()
    {
        Value.Clear();
        Cleared?.Invoke(this, null);
    }

    private Color GetNameColor(string name)
    {
        if (!_settings.WindowColoredNames.Value || string.IsNullOrWhiteSpace(name)) return Color.Gray;

        if (_colorPairs.TryGetValue(name, out var pair)) return pair;

        var color = new Color(_random.Next(255), _random.Next(255), _random.Next(255));
        _colorPairs.Add(name, color);

        return color;
    }
}