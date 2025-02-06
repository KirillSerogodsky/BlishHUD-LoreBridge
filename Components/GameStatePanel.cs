using System;
using Blish_HUD;
using Blish_HUD.Controls;
using LoreBridge.Services;
using Microsoft.Xna.Framework;
using Label = Blish_HUD.Controls.Label;

namespace LoreBridge.Components;

public class GameStatePanel : IDisposable
{
    private readonly FlowPanel _flowPanel;
    private readonly GameStateService _stateService;
    
    public GameStatePanel(GameStateService stateService)
    {
        _stateService = stateService;
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

        _stateService.GameStateChanged += (o, e) =>
        {
            label1.Text = $"Cutscene: {(e == GameState.Cutscene).ToString()}";
            label1.TextColor = e == GameState.Cutscene ? Color.Green : Color.White;

            label2.Text = $"Dialog: {(e == GameState.Dialog).ToString()}";
            label2.TextColor = e == GameState.Dialog ? Color.Green : Color.White;

            label3.Text = $"Vista/Cutscene: {(e == GameState.VistaOrCutscene).ToString()}";
            label3.TextColor = e == GameState.VistaOrCutscene ? Color.Green : Color.White;

            label4.Text = $"Is game: {(e == GameState.InGame).ToString()}";
            label4.TextColor = e == GameState.InGame ? Color.Green : Color.White;

            label5.Text = $"Loading/Char Select/Map: {(e == GameState.LoadingOrCharacterSelection).ToString()}";
            label5.TextColor = e == GameState.LoadingOrCharacterSelection ? Color.Green : Color.White;

            label6.Text = $"Unknown: {(e == GameState.Unknown).ToString()}";
            label6.TextColor = e == GameState.Unknown ? Color.Green : Color.White;
            
            label7.Text = $"None: {(e == GameState.None).ToString()}";
            label7.TextColor = e == GameState.None ? Color.Green : Color.White;
        };
    }

    public void Dispose()
    {
        _flowPanel.Dispose();
    }
}