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
using LoreBridge.Components;
using LoreBridge.Language;
using LoreBridge.OCR;
using LoreBridge.Translation;
using LoreBridge.Translation.Translators;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Screen = LoreBridge.Utils.Screen;

namespace LoreBridge.Services;

public class CutsceneSubtitlesService : IDisposable
{
    private readonly ScreenDetectionRegion _cutsceneRegion;
    private readonly WindowsOcr _engine;
    private readonly Label2 _subtitlesLabel;

    private readonly SortedList<ulong, string> _messages = [];
    private readonly YandexTranslator _translator;

    private bool _enabled;
    private double _lastTick;
    private IArcDpsMessageListener<NpcMessageInfo> _npcMessageListener;
    private string _prevTextDetect;
    private double _timeAfterLastDetect;

    public CutsceneSubtitlesService(WindowsOcr engine, SpriteFontBase font)
    {
        _translator = new YandexTranslator(new TranslatorConfig
            {
                TargetLang = LanguageDetails.GetByLanguage((int)Languages.Russian)
            }
        );
        _engine = engine;
        _cutsceneRegion = new ScreenDetectionRegion
        {
            Parent = GameService.Graphics.SpriteScreen,
            Visible = false
        };
        _subtitlesLabel = new Label2
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
        
        // _cutsceneRegion.Width = bounds.Width - 10;
        // _cutsceneRegion.Height = CalcBottomBarHeight();
        // _cutsceneRegion.Left = 10;
        // _cutsceneRegion.Bottom = bounds.Bottom;
        // _cutsceneRegion.Visible = true;

        if (_messages.Count > 0)
            _subtitlesLabel.Text = _messages.Last().Value;

        // _ = DetectText(gameTime);
    }

    private async Task OnNpcChatMessage(NpcMessageInfo chatMessage, CancellationToken cancellationToken)
    {
        var translation = await _translator.TranslateAsync(chatMessage.Message);
        if (!string.IsNullOrWhiteSpace(translation))
            _messages.Add(chatMessage.TimeStamp, string.Join("", $"{chatMessage.CharacterName}: ", translation));
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

    private void Start()
    {
        CreateNpcMessageListener();
        RecalculateSubtitlesBounds();
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
    }

    private int CalcBottomBarHeight()
    {
        const int threshold = 3;
        var bounds = GameService.Graphics.SpriteScreen.LocalBounds;
        var factor = GameService.Graphics.UIScaleMultiplier;
        var bitmap = Screen.GetScreen(
            new Rectangle(
                new Point(0, 0),
                new Size(
                    2,
                    (int)(bounds.Height * factor)
                )
            )
        );

        var h = -1;
        var found = false;
        for (var y = bitmap.Height - 1; y >= 0 && !found; y--)
        for (var x = 0; x < bitmap.Width; x++)
        {
            var pixelColor = bitmap.GetPixel(x, y);
            if (pixelColor.R <= threshold || pixelColor.G <= threshold || pixelColor.B <= threshold) continue;
            h = (int)(bounds.Height - y / factor);
            found = true;
            break;
        }

        return h;
    }

    private async Task DetectText(GameTime gameTime)
    {
        var bounds = GameService.Graphics.SpriteScreen.LocalBounds;
        var factor = GameService.Graphics.UIScaleMultiplier;
        var bitmap = Screen.GetScreen(
            new Rectangle(
                new Point(
                    bounds.Left + (int)(_cutsceneRegion.Location.X * factor),
                    bounds.Top + (int)(_cutsceneRegion.Location.Y * factor)
                ),
                new Size(
                    (int)(_cutsceneRegion.Size.X * factor),
                    (int)(_cutsceneRegion.Size.Y * factor)
                )
            )
        );

        var text = _engine.GetTextLines(bitmap);
        var text2 = string.Join("\n", text);

        if (string.IsNullOrWhiteSpace(text2))
        {
            _subtitlesLabel.Visible = false;
            _subtitlesLabel.Text = "";
            _prevTextDetect = "";
            _timeAfterLastDetect = gameTime.TotalGameTime.TotalMilliseconds;
            return;
        }

        if (gameTime.TotalGameTime.TotalMilliseconds - _timeAfterLastDetect < 1000) return;
        _timeAfterLastDetect = gameTime.TotalGameTime.TotalMilliseconds;

        if (_prevTextDetect != text2)
        {
            _prevTextDetect = text2;
            var translation = await _translator.TranslateAsync(text2);
            if (!string.IsNullOrWhiteSpace(translation))
            {
                _subtitlesLabel.Text = translation;
                _subtitlesLabel.Visible = true;
            }
        }
    }
}