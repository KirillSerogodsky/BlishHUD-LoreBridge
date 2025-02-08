using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.GameServices.ArcDps.V2;
using Blish_HUD.GameServices.ArcDps.V2.Models;
using LoreBridge.Utils;
using Microsoft.Xna.Framework;
using Point = Microsoft.Xna.Framework.Point;

namespace LoreBridge.Services;

public enum GameState
{
    None = -1,
    Unknown = 0,
    InGame,
    Cutscene,
    Dialog,
    VistaOrCutscene,
    LoadingOrCharacterSelection
}

public class GameStateService : IDisposable
{
    private readonly IArcDpsMessageListener<ImGuiCallback> _imGuiListener;
    private GameState _gameState = GameState.None;
    private bool _isInCharacterSelectOrLoading;
    private bool _isInGame;
    private double _lastTick;
    private int _tickDelay = 25;

    public GameStateService()
    {
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

    public GameState CurrentGameState
    {
        get => _gameState;
        private set
        {
            if (_gameState == value) return;

            _gameState = value;

            switch (_gameState)
            {
                case GameState.InGame:
                    ChangedToInGame?.Invoke(this, true);
                    break;
                case GameState.LoadingOrCharacterSelection:
                    ChangedToLoadingOrCharacterSelect?.Invoke(this, true);
                    break;
                case GameState.Cutscene:
                    ChangedToCutscene?.Invoke(this, true);
                    break;
                case GameState.Dialog:
                    ChangedToDialog?.Invoke(this, true);
                    break;
                case GameState.VistaOrCutscene:
                    ChangedToVistaOrCutscene?.Invoke(this, true);
                    break;
                case GameState.Unknown:
                    ChangedToUnknown?.Invoke(this, true);
                    break;
                case GameState.None:
                    break;
            }

            GameStateChanged?.Invoke(this, _gameState);
        }
    }

    public void Dispose()
    {
        GameService.GameIntegration.Gw2Instance.IsInGameChanged -= OnIsInGameChange;
        _imGuiListener?.Dispose();
    }

    public event EventHandler<GameState> GameStateChanged;
    public event EventHandler<bool> ChangedToInGame;
    public event EventHandler<bool> ChangedToLoadingOrCharacterSelect;
    public event EventHandler<bool> ChangedToCutscene;
    public event EventHandler<bool> ChangedToDialog;
    public event EventHandler<bool> ChangedToVistaOrCutscene;
    public event EventHandler<bool> ChangedToUnknown;

    public void Run(GameTime gameTime)
    {
        if (gameTime.TotalGameTime.TotalMilliseconds - _lastTick <= _tickDelay) return;
        _lastTick = gameTime.TotalGameTime.TotalMilliseconds;

        if (!GameService.GameIntegration.Gw2Instance.Gw2HasFocus) return;

        var newStatus = GameState.Unknown;

        if (_isInCharacterSelectOrLoading) newStatus = GameState.LoadingOrCharacterSelection;

        if (_isInGame) newStatus = GameState.InGame;

        // Never entered into the game
        if (newStatus is GameState.Unknown && _gameState is GameState.None) return;

        // Cutscenes, Dialogs, Vistas after entering the game 
        if (_isInGame == false && _isInCharacterSelectOrLoading == false)
        {
            var resolution = GameService.Graphics.Resolution;
            var size = new Point(50, 50);
            var topRight = Screen.GetScreen(new Point(resolution.X - size.X, 0), size);
            var bottomLeft = Screen.GetScreen(new Point(0, resolution.Y - size.Y), size);

            // Cutscene
            if (IsBitmapBlack(topRight) && IsBitmapBlack(bottomLeft))
                newStatus = GameState.Cutscene;
            else
                newStatus = GameState.Unknown;
        }

        _tickDelay = newStatus == GameState.Cutscene ? 250 : 25;

        CurrentGameState = newStatus;
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