using Blish_HUD.Controls;
using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Views.SettingsView.Controls;

public class Cutscenes
{
    public Cutscenes(Panel mainPanel, Settings settings)
    {
        var cutscenesPanel = new FlowPanel
        {
            Parent = mainPanel,
            Title = "Cutscenes (Experimental)",
            CanCollapse = true,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(6, 6),
            ControlPadding = new Vector2(6, 6),
            ShowBorder = true
        };
    }
}