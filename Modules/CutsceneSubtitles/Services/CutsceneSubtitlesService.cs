using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.GameServices.ArcDps.V2;
using Blish_HUD.GameServices.ArcDps.V2.Models.UnofficialExtras;
using FontStashSharp;
using LoreBridge.Controls;
using LoreBridge.Modules.AreaTranslation.Controls;
using LoreBridge.Services.Ocr;
using LoreBridge.Translation;
using LoreBridge.Translation.Language;
using LoreBridge.Translation.Translators;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using Point = System.Drawing.Point;
using Screen = LoreBridge.Utils.Screen;

namespace LoreBridge.Modules.CutsceneSubtitles.Services;

public class CutsceneSubtitlesService : IDisposable
{
    private readonly ScreenDetectionRegion _cutsceneRegion;
    private readonly WindowsOcr _engine;

    private readonly SortedList<ulong, string> _messages = [];
    private readonly LabelCustom _subtitlesLabel;
    private readonly Yandex _translator;

    private bool _enabled;
    private bool _isCutsceneWithMessages;
    private double _lastTick;
    private IArcDpsMessageListener<NpcMessageInfo> _npcMessageListener;
    private string _prevDetectedText;
    private double _timeAfterLastDetect;

    public CutsceneSubtitlesService(WindowsOcr engine, SpriteFontBase font)
    {
        _translator = new Yandex(new TranslatorConfig
            {
                TargetLang = LanguagesInfo.GetByLanguage((int)Languages.Russian)
            }
        );
        _engine = engine;
        _cutsceneRegion = new ScreenDetectionRegion
        {
            Parent = GameService.Graphics.SpriteScreen,
            Visible = false
        };
        _subtitlesLabel = new LabelCustom
        {
            Parent = GameService.Graphics.SpriteScreen,
            AutoSizeHeight = true,
            Font = font,
            TextColor = Color.White,
            WrapText = true,
            BackgroundColor = Color.Black,
            Visible = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top
        };
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value)
                Start();
            else
                End();

            _enabled = value;
        }
    }

    public void Dispose()
    {
        _cutsceneRegion.Dispose();
        _subtitlesLabel.Dispose();
        _npcMessageListener?.Dispose();
    }

    public void Run(GameTime gameTime)
    {
        if (!_enabled) return;

        if (gameTime.TotalGameTime.TotalMilliseconds - _lastTick < 500) return;
        _lastTick = gameTime.TotalGameTime.TotalMilliseconds;

        if (!GameService.GameIntegration.Gw2Instance.Gw2HasFocus) return;

        if (!_isCutsceneWithMessages)
            _ = DetectText(gameTime);
    }

    private async Task OnNpcChatMessage(NpcMessageInfo chatMessage, CancellationToken cancellationToken)
    {
        _isCutsceneWithMessages = true;
        var translation = await _translator.TranslateAsync(chatMessage.Message);
        if (!string.IsNullOrWhiteSpace(translation))
        {
            _messages.Add(chatMessage.TimeStamp, string.Join("", $"{chatMessage.CharacterName}: ", translation));
            _subtitlesLabel.Text = _messages.Last().Value;
        }
    }

    private void CreateNpcMessageListener()
    {
        try
        {
            _npcMessageListener ??= new ArcDpsMessageListener<NpcMessageInfo>(MessageType.NpcMessage, OnNpcChatMessage);
            GameService.ArcDpsV2.RegisterMessageType(_npcMessageListener);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    private void RecalculateSubtitlesBounds()
    {
        var bounds = GameService.Graphics.SpriteScreen.LocalBounds;
        _subtitlesLabel.Width = bounds.Width / 2;
        _subtitlesLabel.Left = bounds.Width / 2 - bounds.Width / 4;
        _subtitlesLabel.Top = bounds.Top + 2;
    }

    private void RecalculateBottomBarBounds()
    {
        var bounds = GameService.Graphics.SpriteScreen.LocalBounds;
        _cutsceneRegion.Height = CalculateBottomBarHeight();
        _cutsceneRegion.Width = bounds.Width;
        _cutsceneRegion.Left = 0;
        _cutsceneRegion.Bottom = bounds.Bottom;
    }

    private void Start()
    {
        CreateNpcMessageListener();
        RecalculateSubtitlesBounds();
        RecalculateBottomBarBounds();
        _cutsceneRegion.Visible = true;
        _subtitlesLabel.Visible = true;
    }

    private void End()
    {
        _npcMessageListener?.Dispose();
        _cutsceneRegion.Visible = false;
        _subtitlesLabel.Visible = false;
        _subtitlesLabel.Text = null;
        _messages.Clear();
        _isCutsceneWithMessages = false;
    }

    private int CalculateBottomBarHeight()
    {
        const int threshold = 1;
        const int width = 2;
        var resolution = GameService.Graphics.Resolution;
        var left = Screen.GetScreen(
            new Point(0, 0),
            new Size(
                width,
                resolution.Y
            )
        );
        var right = Screen.GetScreen(
            new Point(resolution.X - width, 0),
            new Size(
                width,
                resolution.Y
            )
        );
        var center = Screen.GetScreen(
            new Point(resolution.X / 2 - width / 2, 0),
            new Size(
                width,
                resolution.Y
            )
        );

        var h1 = 0;
        var found = false;
        for (var y = left.Height - 1; y >= 0 && !found; y--)
        for (var x = 0; x < left.Width; x++)
        {
            var pixelColor = left.GetPixel(x, y);
            if (pixelColor.R <= threshold || pixelColor.G <= threshold || pixelColor.B <= threshold) continue;
            h1 = resolution.Y - y;
            found = true;
            break;
        }

        var h2 = 0;
        found = false;
        for (var y = right.Height - 1; y >= 0 && !found; y--)
        for (var x = 0; x < right.Width; x++)
        {
            var pixelColor = right.GetPixel(x, y);
            if (pixelColor.R <= threshold || pixelColor.G <= threshold || pixelColor.B <= threshold) continue;
            h2 = resolution.Y - y;
            found = true;
            break;
        }

        var h3 = 0;
        found = false;
        for (var y = center.Height - 1; y >= 0 && !found; y--)
        for (var x = 0; x < center.Width; x++)
        {
            var pixelColor = center.GetPixel(x, y);
            if (pixelColor.R <= threshold || pixelColor.G <= threshold || pixelColor.B <= threshold) continue;
            h3 = resolution.Y - y;
            found = true;
            break;
        }

        return Math.Max(Math.Max(h1, h2), h3);
    }

    private async Task DetectText(GameTime gameTime)
    {
        var factor = GameService.Graphics.UIScaleMultiplier;
        var bitmap = Screen.GetScreen(
            new Point(
                (int)(_cutsceneRegion.Location.X * factor),
                (int)(_cutsceneRegion.Location.Y * factor)
            ),
            new Size(
                (int)(_cutsceneRegion.Size.X * factor),
                (int)(_cutsceneRegion.Size.Y * factor)
            )
        );
        var text = _engine.GetTextLines(bitmap);

        // Remove button text
        if (text.Last().ToLower() == "skip to end")
            text = text.Take(text.Length - 1).ToArray();

        var text2 = string.Join("\n", text);

        // No text detected
        if (string.IsNullOrWhiteSpace(text2))
        {
            _subtitlesLabel.Text = null;
            _timeAfterLastDetect = gameTime.TotalGameTime.TotalMilliseconds;
            return;
        }

        if (gameTime.TotalGameTime.TotalMilliseconds - _timeAfterLastDetect < 500) return;
        _timeAfterLastDetect = gameTime.TotalGameTime.TotalMilliseconds;

        if (_prevDetectedText != text2)
        {
            _prevDetectedText = text2;
            var translation = await _translator.TranslateAsync(text2);
            if (!string.IsNullOrWhiteSpace(translation) && _isCutsceneWithMessages) _subtitlesLabel.Text = translation;
        }
    }
}