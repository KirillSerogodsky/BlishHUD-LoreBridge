using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace LoreBridge.Components
{
    public sealed class TranslatorCornerIcon : CornerIcon
    {
        private readonly Texture2D _icon;

        public TranslatorCornerIcon(ContentsManager contentsManager)
        {
            _icon = contentsManager.GetTexture("icon.png");

            Visible = true;
            Icon = _icon;
            IconName = "Translator";
        }

        protected override void DisposeControl()
        {
            _icon.Dispose();
            base.DisposeControl();
        }
    }
}
