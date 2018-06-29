﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ClassicUO.Game.WorldObjects
{
    public class StaticObject : WorldObject
    {
        public StaticObject(Graphic tileID, Hue hue, int index)
        {
            TileID = tileID; Hue = hue; Index = index;
        }

        public Graphic TileID { get; }
        public Hue Hue { get; }
        public int Index { get; }
        public override Position Position { get; set; }
    }
}
