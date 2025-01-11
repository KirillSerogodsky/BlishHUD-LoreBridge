#nullable enable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace LoreBridge.OCR;

public class WindowsOcr
{
    private readonly OcrEngine? _engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en-US"));

    public string[] GetTextLines(Bitmap bitmap)
    {
        if (_engine is null)
            throw new Exception("The English (USA) Language Pack must be installed for Windows OCR to work properly.");

        SoftwareBitmap softwareBitmap;
        using (var randStream = new InMemoryRandomAccessStream())
        {
            bitmap.Save(randStream.AsStream(), ImageFormat.Tiff);
            var decoder = BitmapDecoder.CreateAsync(randStream).AsTask().Result;
            softwareBitmap = decoder.GetSoftwareBitmapAsync().AsTask().Result;
        }

        var result = _engine.RecognizeAsync(softwareBitmap).AsTask().Result;

        return result.Lines.Select(line => line.Text).ToArray();
    }
}