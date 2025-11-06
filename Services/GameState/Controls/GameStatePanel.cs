using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Label = Blish_HUD.Controls.Label;

namespace LoreBridge.Services.GameState.Controls;

public class GameStatePanel : IDisposable
{
    private readonly FlowPanel _flowPanel;
    
    public GameStatePanel()
    {
        _flowPanel = new FlowPanel
        {
            Parent = GameService.Graphics.SpriteScreen,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            WidthSizingMode = SizingMode.AutoSize,
            HeightSizingMode = SizingMode.AutoSize,
            Location = new Point(60, 35)
        };

        var label1 = new Label
        {
            Parent = _flowPanel,
            Font = GameService.Content.DefaultFont16,
            TextColor = Color.White,
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            ShowShadow = true,
            Text = "Cutscene: False"
        };
        var label2 = new Label
        {
            Parent = _flowPanel,
            Font = GameService.Content.DefaultFont16,
            TextColor = Color.White,
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            ShowShadow = true,
            Text = "Dialog: False"
        };
        var label3 = new Label
        {
            Parent = _flowPanel,
            Font = GameService.Content.DefaultFont16,
            TextColor = Color.White,
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            ShowShadow = true,
            Text = "Vista/Cutscene: False"
        };
        var label4 = new Label
        {
            Parent = _flowPanel,
            Font = GameService.Content.DefaultFont16,
            TextColor = Color.White,
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            ShowShadow = true,
            Text = "In game: False"
        };
        var label5 = new Label
        {
            Parent = _flowPanel,
            Font = GameService.Content.DefaultFont16,
            TextColor = Color.White,
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            ShowShadow = true,
            Text = "Loading/Char Select/Map: False"
        };
        var label6 = new Label
        {
            Parent = _flowPanel,
            Font = GameService.Content.DefaultFont16,
            TextColor = Color.White,
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            ShowShadow = true,
            Text = "Unknown: False"
        };
        var label7 = new Label
        {
            Parent = _flowPanel,
            Font = GameService.Content.DefaultFont16,
            TextColor = Color.Green,
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            ShowShadow = true,
            Text = "None: True"
        };

        Service.GameState.GameStateChanged += (o, e) =>
        {
            label1.Text = $"Cutscene: {(e == GameStateType.Cutscene).ToString()}";
            label1.TextColor = e == GameStateType.Cutscene ? Color.Green : Color.White;

            label2.Text = $"Dialog: {(e == GameStateType.Dialog).ToString()}";
            label2.TextColor = e == GameStateType.Dialog ? Color.Green : Color.White;

            label3.Text = $"Vista/Cutscene: {(e == GameStateType.VistaOrCutscene).ToString()}";
            label3.TextColor = e == GameStateType.VistaOrCutscene ? Color.Green : Color.White;

            label4.Text = $"Is game: {(e == GameStateType.InGame).ToString()}";
            label4.TextColor = e == GameStateType.InGame ? Color.Green : Color.White;

            label5.Text = $"Loading/Char Select/Map: {(e == GameStateType.LoadingOrCharacterSelection).ToString()}";
            label5.TextColor = e == GameStateType.LoadingOrCharacterSelection ? Color.Green : Color.White;

            label6.Text = $"Unknown: {(e == GameStateType.Unknown).ToString()}";
            label6.TextColor = e == GameStateType.Unknown ? Color.Green : Color.White;
            
            label7.Text = $"None: {(e == GameStateType.None).ToString()}";
            label7.TextColor = e == GameStateType.None ? Color.Green : Color.White;
        };
    }

    public void Dispose()
    {
        _flowPanel.Dispose();
    }
}