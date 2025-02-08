using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using FontStashSharp;
using LoreBridge.Controls;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.AreaTranslation.Controls;

public sealed class TranslationPanel : FlowPanel
{
    private readonly Label2 _label;

    public TranslationPanel(SpriteFontBase font)
    {
        Parent = GameService.Graphics.SpriteScreen;
        Visible = false;
        BackgroundColor = Color.Black;
        FlowDirection = ControlFlowDirection.SingleTopToBottom;

        _label = new Label2
        {
            Parent = this,
            Font = font,
            WrapText = true,
            TextColor = Color.White
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
            _label.Width = value;
        }
    }

    public new int Height
    {
        get => base.Height;
        set
        {
            base.Height = value;
            _label.Height = value;
        }
    }

    protected override void OnClick(MouseEventArgs e) {
        base.OnClick(e);
        
        if (Visible)
            Visible = false;
    }
}