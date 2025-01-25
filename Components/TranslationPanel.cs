using System;
using System.Collections.Generic;
using Blish_HUD.Controls;
using FontStashSharp;
using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Components;

public sealed class TranslationPanel : FlowPanel
{
    private const int OuterPadding = 6;
    private const int InnerPadding = 6;

    private readonly List<TranslationItemPanel> _entries = [];
    private readonly MessagesModel _messages;
    private SpriteFontBase _font;

    public TranslationPanel(MessagesModel messages, SpriteFontBase font)
    {
        FlowDirection = ControlFlowDirection.SingleTopToBottom;
        OuterControlPadding = new Vector2(0, OuterPadding);
        ControlPadding = new Vector2(0, InnerPadding);

        _font = font;
        _messages = messages;
        _messages.Added += OnAdded;
        _messages.Cleared += OnCleared;
    }

    public void UpdateFont(SpriteFontBase font)
    {
        _font = font;
        foreach (var item in _entries) item.UpdateFont(font);
    }

    private void OnAdded(object sender, MessageEntry e)
    {
        _entries.Add(new TranslationItemPanel(e, _font) { Parent = this });
    }

    private void OnCleared(object sender, EventArgs e)
    {
        foreach (var item in _entries) item.Dispose();
    }
}