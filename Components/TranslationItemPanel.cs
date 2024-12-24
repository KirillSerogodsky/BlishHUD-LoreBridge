using Blish_HUD.Controls;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Components;

public sealed class TranslationItemPanel : FlowPanel
{
    private readonly TranslationItemLabel _translationItemLabel;

    public TranslationItemPanel(string text, BitmapFont font)
    {
        WidthSizingMode = SizingMode.Fill;
        ShowBorder = true;
        HeightSizingMode = SizingMode.AutoSize;
        FlowDirection = ControlFlowDirection.SingleLeftToRight;

        _translationItemLabel = new TranslationItemLabel(text, font) { Parent = this, Width = _size.X };
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        _translationItemLabel.RerenderText(e.CurrentSize.X);
        base.OnResized(e);
    }
}