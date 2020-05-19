using MapAC.DatLoader;
using MapAC.DatLoader.FileTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAC.Helpers
{
    class PortalHelper
    {
        public Bitmap BuildIconContactSheet(int numIcons = 200)
        {
            int ICON_SIZE = 64; // width and height of all icons
            int PADDING = 10; // padding between all icons
            int ICONS_PER_ROW = 20;
            int FONT_SIZE = 10;
            string FONT_NAME = "Arial";

            Font myFont = new Font(FONT_NAME, FONT_SIZE, FontStyle.Regular) ;

            var numRows = Math.Ceiling((float)(numIcons / ICONS_PER_ROW));
            int width = (ICON_SIZE + PADDING) * ICONS_PER_ROW;
            int height = (int)((ICON_SIZE + PADDING) * numRows);

            Bitmap contactSheet = new Bitmap(width, height);
            Graphics gfxContactSheet = Graphics.FromImage(contactSheet);
            gfxContactSheet.TextRenderingHint = TextRenderingHint.AntiAlias;

            var allTextures = DatManager.CellDat.AllFiles.Where(x => x.Key > 0x06000000).Where(x => x.Key < 0x07FFFFFF).OrderByDescending(k => k.Key);
            Dictionary<uint, Bitmap> iconList = new Dictionary<uint, Bitmap>();

            int iconsUsed = 0;
            foreach(var kvp in allTextures)
            {
                var texture = DatManager.CellDat.ReadFromDat<Texture>(kvp.Key);
                if(texture.Format == SurfacePixelFormat.PFID_A8R8G8B8 && texture.Width == 32 && texture.Height == 32)
                {
                    iconList.Add(kvp.Key, texture.GetBitmap());
                    var icon = texture.GetBitmap();

                    var myRow = iconsUsed / ICONS_PER_ROW;
                    var myCol = iconsUsed % ICONS_PER_ROW;
                    var posX = myRow * (ICON_SIZE + PADDING) + PADDING + ICON_SIZE/2;
                    var posY = myCol * (ICON_SIZE + PADDING + FONT_SIZE + PADDING) + PADDING + ICON_SIZE / 2;


                    gfxContactSheet.DrawImage(icon, new Point(posX, posY));
                    gfxContactSheet.DrawString(kvp.Key.ToString("X8"), myFont, Brushes.Black, new Point(posX, posY + PADDING + ICON_SIZE));

                    iconsUsed++;
                }
                if (iconsUsed == numIcons)
                    break;
            }

            /*
            //var image = DatManager.CellDat.ReadFromDat<Texture>(0x06002011); // Lugian Scepter icon...
            for (var y = 0; y < numRows; y++)
            {
                for (var x = 0; x < ICONS_PER_ROW; x++) {
                    int idx = x + y * ICONS_PER_ROW;
                    var posX = x * (ICON_SIZE + PADDING) + PADDING;
                    var posY = y * (ICON_SIZE + PADDING + FONT_SIZE + PADDING) + PADDING;
                    var icon = iconList[idx];
                    gfxContactSheet.DrawImage(icon, new Point(posX, posY));
                }
            }
            */

            return contactSheet;
        }
    }
}
