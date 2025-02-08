using LoreBridge.Resources;

namespace LoreBridge.Controls;

public sealed class CornerIcon : Blish_HUD.Controls.CornerIcon
{
    public CornerIcon()
    {
        Visible = true;
        Icon = Textures.Icon;
        HoverIcon = Textures.IconHover;
        IconName = "LoreBridge";
    }
}