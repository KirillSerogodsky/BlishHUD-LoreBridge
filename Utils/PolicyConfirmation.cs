using System;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using LoreBridge.Views;
using Point = Microsoft.Xna.Framework.Point;

namespace LoreBridge.Utils;

public static class PolicyConfirmation
{
    private static EventHandler<ResizedEventArgs> _resizeHandler;
    
    public static async Task<bool> ShowPolicyConfirmationAsync()
    {
        var screenSize = GameService.Graphics.SpriteScreen.Size;
        var windowSize = new Point(500, 300);
        var container = new ViewContainer
        {
            Parent = GameService.Graphics.SpriteScreen,
            Location = new Point(
                (screenSize.X - windowSize.X) / 2,
                (screenSize.Y - windowSize.Y) / 2
            ),
            Size = windowSize,
            ZIndex = int.MaxValue - 2,
            HeightSizingMode = SizingMode.AutoSize,
            FadeView = true
        };
        
        _resizeHandler = (_, args) =>
        {
            container.Location = new Point(
                (args.CurrentSize.X - windowSize.X) / 2,
                (args.CurrentSize.Y - windowSize.Y) / 2
            );
        };

        GameService.Graphics.SpriteScreen.Resized += _resizeHandler;

        var view = new PolicyConfirmationView();
        container.Show(view);

        try
        {
            return await view.ResultTask.Task;
        }
        finally
        {
            GameService.Graphics.SpriteScreen.Resized -= _resizeHandler;
        }
    }
}