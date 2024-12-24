using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace LoreBridge.Utils;

internal class BitmapFont : MonoGame.Extended.BitmapFonts.BitmapFont, IDisposable
{
    private readonly Texture2D _texture;

    public BitmapFont(string name, IEnumerable<BitmapFontRegion> regions, int lineHeight, Texture2D texture) : base(
        name, regions, lineHeight)
    {
        _texture = texture;
    }

    public BitmapFont(string name, IReadOnlyList<BitmapFontRegion> regions, int lineHeight) : base(name, regions,
        lineHeight)
    {
        _texture = regions[0].TextureRegion.Texture;
    }

    public void Dispose()
    {
        _texture?.Dispose();
    }
}