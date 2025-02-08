using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Services;

public abstract class Service
{
    public static readonly GameStateService GameState;
    public static readonly TranslationService Translation;

    private static readonly Service[] _services =
    [
        GameState = new GameStateService(),
        Translation = new TranslationService()
    ];

    public static Service[] All => _services;

    public abstract void Load(SettingsModel settings);

    public abstract void Update(GameTime gameTime);

    public abstract void Unload();
}