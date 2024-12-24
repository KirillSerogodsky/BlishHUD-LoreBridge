using System.Drawing;

namespace LoreBridge.Utils;

internal static class Screen
{
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