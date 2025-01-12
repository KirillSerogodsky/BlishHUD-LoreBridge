using Blish_HUD.Controls;
using LoreBridge.Models;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Components;

public sealed class TranslationItemPanel : FlowPanel
{
    private readonly TranslationItemLabel _translationItemLabel;

    public TranslationItemPanel(TranslationListItemModel listItem, BitmapFont font)
    {
        WidthSizingMode = SizingMode.Fill;
        HeightSizingMode = SizingMode.AutoSize;
        FlowDirection = ControlFlowDirection.SingleTopToBottom;

        if (!string.IsNullOrWhiteSpace(listItem.Name))
        {
            var label = new Label
            {
                Parent = this,
                Text = listItem.Name,
                TextColor = listItem.NameColor,
                Font = font,
                AutoSizeWidth = true,
                ShowShadow = true
            };
        }

        _translationItemLabel = new TranslationItemLabel(listItem.Text, font) { Parent = this, Width = _size.X };
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        _translationItemLabel.RerenderText(e.CurrentSize.X - 8);
        base.OnResized(e);
    }
}