using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace LoreBridge.Components
{
    public sealed class TranslatorCornerIcon : CornerIcon
    {
        private readonly Texture2D _icon;
        private readonly Texture2D _iconHover;

        public TranslatorCornerIcon(ContentsManager contentsManager)
        {
            _icon = contentsManager.GetTexture("icon.png");
            _iconHover = contentsManager.GetTexture("icon-big.png");

            Visible = true;
            Icon = _icon;
            HoverIcon = _iconHover;
            IconName = "LoreBridge";
        }

        protected override void DisposeControl()
        {
            _icon.Dispose();
            _iconHover.Dispose();
            base.DisposeControl();
        }
    }
}
