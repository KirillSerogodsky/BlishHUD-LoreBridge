using System;
using System.Collections.Generic;
using Blish_HUD;

namespace LoreBridge.Models;

public class TranslationListModel
{
    private List<string> Value { get; } = [];

    public event EventHandler<ValueChangedEventArgs<List<string>>> Added;
    public event EventHandler<ValueChangedEventArgs<List<string>>> Updated;
    public event EventHandler Cleared;

    private void OnAdd(ValueChangedEventArgs<List<string>> e)
    {
        Added?.Invoke(this, e);
    }

    private void OnListUpdated(ValueChangedEventArgs<List<string>> e)
    {
        Updated?.Invoke(this, e);
    }

    public void Add(string text)
    {
        var value2 = Value;
        Value.Add(text);
        OnAdd(new ValueChangedEventArgs<List<string>>(value2, Value));
        OnListUpdated(new ValueChangedEventArgs<List<string>>(value2, Value));
    }

    public void ClearAll()
    {
        Value.Clear();
        Cleared.Invoke(this, null);
    }
}