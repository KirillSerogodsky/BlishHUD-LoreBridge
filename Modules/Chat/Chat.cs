using System;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.GameServices.ArcDps.V2;
using Blish_HUD.GameServices.ArcDps.V2.Models.UnofficialExtras;
using FontStashSharp;
using LoreBridge.Controls;
using LoreBridge.Models;
using LoreBridge.Modules.Chat.Controls;
using LoreBridge.Modules.Chat.Models;
using LoreBridge.Modules.Chat.Services;
using LoreBridge.Resources;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.Chat;

public class Chat : Module
{
    private readonly CornerIcon _cornerIcon;
    private SpriteFontBase _font;
    private Messages _messages;
    private IArcDpsMessageListener<NpcMessageInfo> _npcMessageListener;
    private SettingsModel _settings;
    private TranslationWindow _translationWindow;
    private TranslationQueue _translationQueue;

    public Chat(CornerIcon cornerIcon)
    {
        _cornerIcon = cornerIcon;
    }

    public override void Load(SettingsModel settings)
    {
        _settings = settings;
        _messages = new Messages(_settings);
        _font = Fonts.FontSystem.GetFont(_settings.WindowFontSize.Value);
        _translationWindow = new TranslationWindow(_settings, _messages, _font);
        _translationQueue = new TranslationQueue(_messages);

        _cornerIcon.Click += OnCornerIconClick;
        _settings.WindowFontSize.SettingChanged += OnFontSizeChanged;

        try
        {
            if (!GameService.ArcDpsV2.Loaded) return;

            _npcMessageListener = new ArcDpsMessageListener<NpcMessageInfo>(MessageType.NpcMessage, OnNpcChatMessage);
            GameService.ArcDpsV2.RegisterMessageType(_npcMessageListener);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Unload()
    {
        _translationWindow.Dispose();
        _npcMessageListener.Dispose();
        _cornerIcon.Click -= OnCornerIconClick;
        _settings.WindowFontSize.SettingChanged -= OnFontSizeChanged;
    }

    private void OnCornerIconClick(object sender, EventArgs e)
    {
        _translationWindow.ToggleWindow();
    }

    private void OnFontSizeChanged(object sender, ValueChangedEventArgs<int> e)
    {
        _font = Fonts.FontSystem.GetFont(e.NewValue);
        _translationWindow.UpdateFont(_font);
    }

    private async Task OnNpcChatMessage(NpcMessageInfo chatMessage, CancellationToken cancellationToken)
    {
        var timeSpan = TimeSpan.FromSeconds(chatMessage.TimeStamp / 1_000_000_000.0);
        var dateTime = DateTime.Today.Add(timeSpan + DateTimeOffset.Now.Offset);
        var localDateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local);
        _translationQueue.Add(new Message
        {
            Text = chatMessage.Message,
            TimeStamp = chatMessage.TimeStamp,
            Name = chatMessage.CharacterName,
            Time = localDateTime.ToShortTimeString(),
        });
    }
}