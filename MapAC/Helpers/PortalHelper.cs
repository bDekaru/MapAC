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
            int FONT_SIZE = 9;
            string FONT_NAME = "Arial";

            System.Drawing.Font myFont = new System.Drawing.Font(FONT_NAME, FONT_SIZE, FontStyle.Regular) ;

            var numRows = Math.Ceiling((float)(numIcons / ICONS_PER_ROW));
            int width = (ICON_SIZE + PADDING) * ICONS_PER_ROW;
            int height = (int)((ICON_SIZE + PADDING + FONT_SIZE + PADDING) * numRows);

            Bitmap contactSheet = new Bitmap(width, height);
            Graphics gfxContactSheet = Graphics.FromImage(contactSheet);
            gfxContactSheet.TextRenderingHint = TextRenderingHint.AntiAlias;

            var allTextures = DatManager.CellDat.AllFiles.Where(x => x.Key > 0x06000000).Where(x => x.Key < 0x07FFFFFF).OrderByDescending(k => k.Key);
            Dictionary<uint, Bitmap> iconList = new Dictionary<uint, Bitmap>();

            StringFormat sf = new StringFormat();
            //sf.LineAlignment = StringAlignment.Center;
            //sf.Alignment = StringAlignment.Center;

            int iconsUsed = 0;
            foreach(var kvp in allTextures)
            {
                var texture = DatManager.CellDat.ReadFromDat<Texture>(kvp.Key);
                if((texture.Format == SurfacePixelFormat.PFID_A8R8G8B8 || texture.Format == SurfacePixelFormat.PFID_R8G8B8) && texture.Width == 32 && texture.Height == 32)
                {
                    iconList.Add(kvp.Key, texture.GetBitmap());
                    var icon = texture.GetBitmap();

                    var myRow = iconsUsed / ICONS_PER_ROW;
                    var myCol = iconsUsed % ICONS_PER_ROW;
                    var posX = myCol * (ICON_SIZE + PADDING) + 20;
                    var posY = myRow * (ICON_SIZE + PADDING + FONT_SIZE + PADDING) + 5 + ICON_SIZE / 2;

                    var fontPosX = posX - 18;
                    var fontPosY = posY + (ICON_SIZE / 2) + 2;

                    gfxContactSheet.DrawImage(icon, new Point(posX, posY));
                    gfxContactSheet.DrawString(kvp.Key.ToString("X8"), myFont, Brushes.Black, new Point(fontPosX, fontPosY), sf);

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

        /// <summary>
        /// Read the Region file in the portal dat file and get all the terrain details
        /// Once the terrain detials are loaded, get the textures and sample them to get the avg color and build the byte array
        /// </summary>
        public List<Color> GetColors(uint regionID)
        {
            List<Color> landColors = new List<Color>();
            var Region = DatManager.CellDat.ReadFromDat<RegionDesc>(regionID);
            foreach (var t in Region.TerrainInfo.LandSurfaces.TexMerge.TerrainDesc)
            {
                var surfaceId = t.TerrainTex.TexGID;
                SurfaceTexture st;
                switch (DatManager.DatVersion)
                {
                    case DatVersionType.ACDM:
                        st = DatManager.CellDat.ReadFromDat<SurfaceTexture>(surfaceId);
                        landColors.Add(st.GetAverageColor());
                        break;
                    case DatVersionType.ACTOD:
                        st = DatManager.CellDat.ReadFromDat<SurfaceTexture>(surfaceId);
                        var textureId = st.Textures[st.Textures.Count - 1];
                        var texture = DatManager.CellDat.ReadFromDat<Texture>(textureId);
                        landColors.Add(texture.GetAverageColor());
                        break;

                }
            }

            return landColors;
        }
    }
}
