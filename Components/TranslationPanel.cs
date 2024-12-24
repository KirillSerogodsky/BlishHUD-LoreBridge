using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using LoreBridge.Models;

namespace LoreBridge.Components;

public sealed class TranslationPanel : FlowPanel
{
    private const int OUTER_PADDING = 6;
    private const int INNER_PADDING = 6;

    private readonly TranslationListModel _translationList;
    private readonly BitmapFont _font;
    private readonly List<TranslationItemPanel> _entries = new List<TranslationItemPanel>() { };

    public TranslationPanel(TranslationListModel translationList, BitmapFont font)
    {
        FlowDirection = ControlFlowDirection.SingleTopToBottom;
        OuterControlPadding = new Vector2(OUTER_PADDING, OUTER_PADDING);
        ControlPadding = new Vector2(INNER_PADDING, INNER_PADDING);

        _translationList = translationList;
        _translationList.Added += OnAdded;
        _translationList.Cleared += OnCleared;
        _font = font;
    }

    private void OnAdded(object sender, ValueChangedEventArgs<List<string>> e)
    {
        _entries.Add(new TranslationItemPanel(e.NewValue.Last(), _font) { Parent = this });
    }

    private void OnCleared(object sender, EventArgs e)
    {
        foreach (var item in _entries)
        {
            item.Dispose();
        }
    }
}