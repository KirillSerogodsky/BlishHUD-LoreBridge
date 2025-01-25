using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LoreBridge.Models;

public sealed class MessagesModel(SettingsModel settings)
{
    private readonly Dictionary<string, Color> _colorPairs = new();
    private readonly Random _random = new();

    private List<MessageEntry> Value { get; } = [];

    public event EventHandler<MessageEntry> Added;
    public event EventHandler Cleared;

    public void Add(MessageEntry message)
    {
        if (!string.IsNullOrEmpty(message.Name))
        {
            message.NameColor = GetNameColor(message.Name);
        }
        Value.Add(message);
        Added?.Invoke(this, message);
    }

    public void Clear()
    {
        Value.Clear();
        Cleared?.Invoke(this, null);
    }

    private Color GetNameColor(string name)
    {
        if (!settings.WindowColoredNames.Value || string.IsNullOrEmpty(name)) return Color.Gray;

        if (_colorPairs.TryGetValue(name, out var pair)) return pair;

        var color = new Color(_random.Next(100, 255), _random.Next(100, 255), _random.Next(100, 255));
        _colorPairs.Add(name, color);

        return color;
    }
}