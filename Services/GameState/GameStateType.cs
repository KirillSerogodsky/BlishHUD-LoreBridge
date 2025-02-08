namespace LoreBridge.Services.GameState;

public enum GameStateType
{
    None = -1,
    Unknown = 0,
    InGame,
    Cutscene,
    Dialog,
    VistaOrCutscene,
    LoadingOrCharacterSelection
}