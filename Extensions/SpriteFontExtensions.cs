using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using BitmapFont = LoreBridge.Utils.BitmapFont;

namespace LoreBridge;

internal static class SpriteFontExtensions
{
    public static BitmapFont ToBitmapFont(this SpriteFont font, int lineHeight = 0)
    {
        if (lineHeight < 0) throw new ArgumentException("Line height cannot be negative.", nameof(lineHeight));

        var regions = new List<BitmapFontRegion>();
        var glyphs = font.GetGlyphs();

        foreach (var glyph in glyphs.Values)
        {
            var glyphTextureRegion = new TextureRegion2D(font.Texture,
                glyph.BoundsInTexture.Left,
                glyph.BoundsInTexture.Top,
                glyph.BoundsInTexture.Width,
                glyph.BoundsInTexture.Height);

            var region = new BitmapFontRegion(glyphTextureRegion,
                glyph.Character,
                glyph.Cropping.Left,
                glyph.Cropping.Top,
                (int)glyph.WidthIncludingBearings);

            regions.Add(region);
        }

        return new BitmapFont($"{typeof(BitmapFont)}_{Guid.NewGuid():n}",
            regions,
            lineHeight > 0 ? lineHeight : font.LineSpacing,
            font.Texture);
    }
}