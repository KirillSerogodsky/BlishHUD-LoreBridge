using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.GameServices.ArcDps.V2;
using Blish_HUD.GameServices.ArcDps.V2.Models;
using LoreBridge.Models;
using LoreBridge.Services.GameState;
using LoreBridge.Services.GameState.Controls;
using LoreBridge.Utils;
using Microsoft.Xna.Framework;
using Point = Microsoft.Xna.Framework.Point;

namespace LoreBridge.Services;

public class GameStateService : Service
{
    private GameStateType _gameState = GameStateType.None;
    // private GameStatePanel _gameStatePanel;
    private IArcDpsMessageListener<ImGuiCallback> _imGuiListener;
    private bool _isInCharacterSelectOrLoading;
    private bool _isInGame;
    private double _lastTick;
    private int _tickDelay = 25;

    public GameStateType CurrentGameState
    {
        get => _gameState;
        private set
        {
            if (_gameState == value) return;

            _gameState = value;

            GameStateChanged?.Invoke(this, _gameState);
        }
    }

    public event EventHandler<GameStateType> GameStateChanged;

    public override void Load(SettingsModel settings)
    {
        // _gameStatePanel = new GameStatePanel();

        _isInGame = GameService.GameIntegration.Gw2Instance.IsInGame;
        GameService.GameIntegration.Gw2Instance.IsInGameChanged += OnIsInGameChange;

        try
        {
            _isInCharacterSelectOrLoading = GameService.ArcDpsV2.HudIsActive;
            _imGuiListener = new ArcDpsMessageListener<ImGuiCallback>(MessageType.ImGui, OnGuiChange);
            GameService.ArcDpsV2.RegisterMessageType(_imGuiListener);
        }
        catch
        {
            // ignored
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (gameTime.TotalGameTime.TotalMilliseconds - _lastTick <= _tickDelay) return;
        _lastTick = gameTime.TotalGameTime.TotalMilliseconds;

        if (!GameService.GameIntegration.Gw2Instance.Gw2HasFocus) return;

        var newStatus = GameStateType.Unknown;

        if (_isInCharacterSelectOrLoading) newStatus = GameStateType.LoadingOrCharacterSelection;

        if (_isInGame) newStatus = GameStateType.InGame;

        // Never entered into the game
        if (newStatus is GameStateType.Unknown && _gameState is GameStateType.None) return;

        // Cutscenes, Dialogs, Vistas after entering the game 
        if (_isInGame == false && _isInCharacterSelectOrLoading == false)
        {
            var resolution = GameService.Graphics.Resolution;
            var size = new Point(50, 50);
            var topRight = Screen.GetScreen(new Point(resolution.X - size.X, 0), size);
            var bottomLeft = Screen.GetScreen(new Point(0, resolution.Y - size.Y), size);

            // Cutscene
            if (IsBitmapBlack(topRight) && IsBitmapBlack(bottomLeft))
                newStatus = GameStateType.Cutscene;
            else
                newStatus = GameStateType.Unknown;
        }

        _tickDelay = newStatus == GameStateType.Cutscene ? 250 : 25;

        CurrentGameState = newStatus;
    }

    public override void Unload()
    {
        // _gameStatePanel.Dispose();
        GameService.GameIntegration.Gw2Instance.IsInGameChanged -= OnIsInGameChange;
        _imGuiListener?.Dispose();
    }

    private static bool IsBitmapBlack(Bitmap bitmap)
    {
        const int threshold = 1;

        for (var y = 0; y < bitmap.Height; y++)
        for (var x = 0; x < bitmap.Width; x++)
        {
            var pixelColor = bitmap.GetPixel(x, y);
            if (pixelColor.R > threshold || pixelColor.G > threshold || pixelColor.B > threshold)
                return false;
        }

        return true;
    }

    private void OnIsInGameChange(object o, ValueEventArgs<bool> e)
    {
        _isInGame = e.Value;
    }

    private Task OnGuiChange(ImGuiCallback state, CancellationToken cancellationToken)
    {
        _isInCharacterSelectOrLoading = state.NotCharacterSelectOrLoading == 0;
        return Task.CompletedTask;
    }
}