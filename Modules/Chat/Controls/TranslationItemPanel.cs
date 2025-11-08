#nullable enable
using Blish_HUD.Controls;
using FontStashSharp;
using LoreBridge.Controls;
using LoreBridge.Modules.Chat.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.Chat.Controls;

public sealed class TranslationItemPanel : FlowPanel
{
    private readonly FormattedLabelCustom _translationItemLabel;

    public TranslationItemPanel(Message listItem, SpriteFontBase font)
    {
        WidthSizingMode = SizingMode.Fill;
        HeightSizingMode = SizingMode.AutoSize;
        FlowDirection = ControlFlowDirection.SingleTopToBottom;

        var builder = new FormattedLabelBuilderCustom();

        if (!string.IsNullOrEmpty(listItem.Time))
        {
            builder.CreatePart(
                $"[{listItem.Time}] ",
                b => b.SetTextColor(Color.Gray).SetFont(font)
            );
        }

        if (!string.IsNullOrEmpty(listItem.Name))
        {
            builder.CreatePart(
                $"{listItem.Name}",
                b => b.SetTextColor(listItem.NameColor).SetFont(font)
            );
            builder.CreatePart(
                ": ",
                b=> b.SetTextColor(Color.LightGray).SetFont(font)
            );
        }

        builder.CreatePart(
            listItem.Text,
            b=> b.SetTextColor(Color.LightGray).SetFont(font)
        );

        _translationItemLabel = builder
            .SetWidth(_size.X)
            .AutoSizeHeight()
            .Wrap()
            .ShowShadow()
            .Build();
        _translationItemLabel.Parent = this;
    }

    public void UpdateFont(SpriteFontBase font)
    {
        _translationItemLabel.Font = font;
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        _translationItemLabel.Width = e.CurrentSize.X - 8;
        base.OnResized(e);
    }
}