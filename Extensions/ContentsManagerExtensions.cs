using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using SpriteFontPlus;

namespace LoreBridge;

internal static class ContentsManagerExtensions
{
    public static SpriteFont GetSpriteFont(this ContentsManager manager, string fontPath, int fontSize,
        int textureSize = 1392)
    {
        using var fontStream = manager.GetFileStream(fontPath);
        var fontData = new byte[fontStream.Length];
        var fontDataLength = fontStream.Read(fontData, 0, fontData.Length);

        if (fontDataLength <= 0) return null;
        
        using var ctx = GameService.Graphics.LendGraphicsDeviceContext();
        var bakeResult = TtfFontBaker.Bake(fontData, fontSize, textureSize, textureSize, [
            CharacterRange.BasicLatin,
            CharacterRange.Latin1Supplement,
            CharacterRange.LatinExtendedA,
            CharacterRange.Cyrillic,
            CharacterRange.CyrillicSupplement
        ]);

        return bakeResult.CreateSpriteFont(ctx.GraphicsDevice);
    }

    public static BitmapFont GetBitmapFont(this ContentsManager manager, string fontPath, int fontSize,
        int lineHeight = 0)
    {
        return manager.GetSpriteFont(fontPath, fontSize)?.ToBitmapFont(lineHeight);
    }
}