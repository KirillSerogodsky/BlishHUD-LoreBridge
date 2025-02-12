using System.Drawing;
using LoreBridge.Models;
using LoreBridge.Services.Ocr;
using Microsoft.Xna.Framework;

namespace LoreBridge.Services;

public class OcrService : Service
{
    private WindowsOcr _ocrEngine;

    public override void Load(Settings settings)
    {
        _ocrEngine = new WindowsOcr();
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Unload()
    {
    }

    public string[] GetTextLines(Bitmap bitmap)
    {
        return _ocrEngine.GetTextLines(bitmap);
    }
}