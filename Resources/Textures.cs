using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace LoreBridge.Resources;

public static class Textures
{
    public static Texture2D Icon;
    public static Texture2D IconHover;

    public static void Initialize(ContentsManager contentsManager)
    {
        Icon = contentsManager.GetTexture("icon.png");
        IconHover = contentsManager.GetTexture("icon-big.png");
    }

    public static void Dispose()
    {
        Icon.Dispose();
        IconHover.Dispose();
    }
}