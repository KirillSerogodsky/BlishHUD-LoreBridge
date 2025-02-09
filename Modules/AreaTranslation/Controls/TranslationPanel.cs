using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using FontStashSharp;
using LoreBridge.Controls;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.AreaTranslation.Controls;

public sealed class TranslationPanel : Panel
{
    private readonly Label2 _label;

    public TranslationPanel(SpriteFontBase font)
    {
        Parent = GameService.Graphics.SpriteScreen;
        Visible = false;
        HeightSizingMode = SizingMode.AutoSize;
        BackgroundColor = Color.Black * 0.9f;

        _label = new Label2
        {
            Parent = this,
            Font = font,
            WrapText = true,
            TextColor = Color.White,
            AutoSizeHeight = true
        };
    }
    public string Text
    {
        set => _label.Text = value;
    }

    public new int Width
    {
        get => base.Width;
        set
        {
            base.Width = value;
            _label.Width = value - 10;
        }
    }

    public void UpdateFont(SpriteFontBase font)
    {
        _label.Font = font;
    }

    protected override void OnClick(MouseEventArgs e) {
        base.OnClick(e);
        
        if (Visible)
            Visible = false;
    }
}