using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LoreBridge.Modules.AreaTranslation.Controls;

public sealed class ScreenDetectionRegion : Control
{
    public ScreenDetectionRegion()
    {
        ZIndex = int.MaxValue;
        Enabled = false;
    }

    public Color BorderColor { get; set; } = Color.Red;
    public int BorderWidth { get; set; } = 2;

    protected override CaptureType CapturesInput()
    {
        return CaptureType.None;
    }

    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
        var rect = new Rectangle(Location, Size);
        spriteBatch.Draw(ContentService.Textures.Pixel,
            new Rectangle(rect.Left, rect.Top, rect.Width + 2 * BorderWidth, BorderWidth),
            BorderColor);
        spriteBatch.Draw(ContentService.Textures.Pixel,
            new Rectangle(rect.Left, rect.Bottom - BorderWidth, rect.Width + 2 * BorderWidth, BorderWidth),
            BorderColor);
        spriteBatch.Draw(ContentService.Textures.Pixel,
            new Rectangle(rect.Left, rect.Top, BorderWidth, rect.Height + 2 * BorderWidth),
            BorderColor);
        spriteBatch.Draw(ContentService.Textures.Pixel,
            new Rectangle(rect.Right - BorderWidth, rect.Top, BorderWidth, rect.Height + 2 * BorderWidth), BorderColor);
    }
}