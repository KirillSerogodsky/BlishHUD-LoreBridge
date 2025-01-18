﻿using Blish_HUD.Controls;
using FontStashSharp;
using LoreBridge.Models;

namespace LoreBridge.Components;

public sealed class TranslationItemPanel : FlowPanel
{
    private readonly Label2 _translationItemLabel;

    public TranslationItemPanel(TranslationListItemModel listItem, SpriteFontBase font)
    {
        WidthSizingMode = SizingMode.Fill;
        HeightSizingMode = SizingMode.AutoSize;
        FlowDirection = ControlFlowDirection.SingleTopToBottom;

        if (!string.IsNullOrWhiteSpace(listItem.Name))
        {
            var label = new Label2
            {
                Parent = this,
                Text = listItem.Name,
                TextColor = listItem.NameColor,
                Font = font,
                AutoSizeWidth = true,
                ShowShadow = true
            };
        }

        _translationItemLabel = new Label2
        {
            Parent = this,
            Width = _size.X,
            Text = listItem.Text,
            AutoSizeHeight = true,
            Font = font,
            ShowShadow = true,
            WrapText = true
        };
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        _translationItemLabel.Width = e.CurrentSize.X - 8;
        base.OnResized(e);
    }
}