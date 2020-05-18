using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MapAC.Helpers
{

    public class MapColor
    {
        Color MyColor;
        public byte Luminance;

        public MapColor(byte R, byte G, byte B, byte luminance = 70)
        {
            Luminance = luminance;
            MyColor = Color.FromArgb(R, G, B);
        }
    }
}
