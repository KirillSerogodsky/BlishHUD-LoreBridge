using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace LoreBridge.OCR
{
    public class WindowsOCR
    {
        public string[] GetTextLines(Bitmap bitmap)
        {
            SoftwareBitmap softwareBitmap;
            using (var randStream = new InMemoryRandomAccessStream())
            {
                bitmap.Save(randStream.AsStream(), ImageFormat.Tiff);
                var decoder = BitmapDecoder.CreateAsync(randStream).AsTask().Result;
                softwareBitmap = decoder.GetSoftwareBitmapAsync().AsTask().Result;
            }

            var engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en-US"));
            OcrResult result = engine.RecognizeAsync(softwareBitmap).AsTask().Result;

            return result.Lines.Select((line) => line.Text).ToArray();
        }
    }
}
