using Blish_HUD.Modules.Managers;
using FontStashSharp;

namespace LoreBridge.Resources;

public static class Fonts
{
    private const string FontPath = "fonts/FiraSansCondensed-Medium.ttf";
    private const string FontTcPath = "fonts/NotoSansTC-Medium.ttf";
    private const string FontScPath = "fonts/NotoSansTC-Medium.ttf";
    private const string FontJpPath = "fonts/NotoSansJP-Medium.ttf";
    private const string FontKrPath = "fonts/NotoSansKR-Medium.ttf";

    public static FontSystem FontSystem;

    public static void Initialize(ContentsManager contentsManager)
    {
        var fonts = new[]
        {
            contentsManager.GetFileStream(FontPath),
            contentsManager.GetFileStream(FontKrPath),
            contentsManager.GetFileStream(FontTcPath),
            contentsManager.GetFileStream(FontScPath),
            contentsManager.GetFileStream(FontJpPath)
        };

        FontSystem = new FontSystem();

        foreach (var font in fonts) FontSystem.AddFont(font);
    }

    public static void Dispose()
    {
        FontSystem.Dispose();
    }
}