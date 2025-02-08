using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.Chat.Models;

public sealed class Message
{
    public string Name { get; set; }
    public Color NameColor { get; set; }
    public string Text { get; set; }
    public ulong TimeStamp { get; set; }
    public string Time { get; set; }
}