using System.Drawing;

namespace LoreBridge.Utils;

internal static class Screen
{
    public static Bitmap GetScreen(Microsoft.Xna.Framework.Point pos, Microsoft.Xna.Framework.Point size)
    {
        return GetScreen(new Rectangle(pos.X, pos.Y, size.X, size.Y));
    }

    public static Bitmap GetScreen(Point pos, Size size)
    {
        return GetScreen(new Rectangle(pos.X, pos.Y, size.Width, size.Height));
    }

    public static Bitmap GetScreen(Rectangle rectangle)
    {
        var screen = new Bitmap(
            rectangle.Width,
            rectangle.Height
        );
        Graphics
            .FromImage(screen)
            .CopyFromScreen(
                rectangle.X,
                rectangle.Y,
                Point.Empty.X,
                Point.Empty.Y,
                screen.Size
            );

        return screen;
    }
}