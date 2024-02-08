using ClassicUO.Game.Managers;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.Data.OpenUO;

internal class HighlightedArea
{
    public HighlightedArea(Rectangle rectangle, ushort hue, HighlightType type, byte priority)
    {
        Rectangle = rectangle;
        Hue = hue;
        Type = type;
        Priority = priority;
    }

    public Rectangle Rectangle { get; private set; } 
    public ushort Hue { get; private set; }
    public HighlightType Type { get; private set; }
    public byte Priority { get; private set; }
}