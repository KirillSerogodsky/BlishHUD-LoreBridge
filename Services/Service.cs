using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Services;

public abstract class Service
{
    public static readonly GameStateService GameState;
    public static readonly TranslationService Translation;
    public static readonly OcrService Ocr;

    public static Service[] All { get; } =
    [
        GameState = new GameStateService(),
        Translation = new TranslationService(),
        Ocr = new OcrService()
    ];

    public abstract void Load(Settings settings);

    public abstract void Update(GameTime gameTime);

    public abstract void Unload();
}