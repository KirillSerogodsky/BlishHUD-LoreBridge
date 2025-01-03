using System;
using System.Collections.Generic;

namespace LoreBridge.Models;

public sealed class TranslationListModel
{
    private List<TranslationListItemModel> Value { get; } = [];

    public event EventHandler<TranslationListItemModel> Added;
    public event EventHandler Cleared;

    public void Add(string text, string name = "")
    {
        var item = new TranslationListItemModel()
        {
            Name = name,
            Text = text,
        };
        Value.Add(item);
        Added?.Invoke(this, item);
    }

    public void Clear()
    {
        Value.Clear();
        Cleared?.Invoke(this, null);
    }
}