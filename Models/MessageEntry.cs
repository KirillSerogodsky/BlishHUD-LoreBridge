using Microsoft.Xna.Framework;

namespace LoreBridge.Models;

public sealed class MessageEntry
{
    public string Name { get; set; }
    public Color NameColor { get; set; }
    public string Text { get; set; }
    public uint TimeStamp { get; set; }
    public string Time { get; set; }
}