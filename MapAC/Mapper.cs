using MapAC.DatLoader;
using MapAC.DatLoader.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace MapAC
{
    public class Mapper
    {
        public Bitmap MapImage;

        private struct LandData
        {
            public ushort Type;
            public int Z;
            public bool Used;
            public bool Blocked; // Can't walk on
        }

        // each landblock is 9x9 points, with the edge points being shared between neighbor landblocks.
        // 255 * 8 + 1, the extra 1 is for the last edge.
        const int LANDSIZE = 2041; 

        // The following constants change how the lighting works.  It is easy to wash out
        // the bright whites of the snow, so be careful.

        // Incresing COLORCORRECTION makes the base color more prominant.
        const double COLORCORRECTION = 70.0;

        // Increasing LIGHTCORRECTION increases the contrast between steep and flat slopes.
        const double LIGHTCORRECTION = 2.25;

        // Increasing AMBIENTLIGHT makes everyting brighter.
        const double AMBIENTLIGHT = 64.0;

        private LandData[,] land = new LandData[LANDSIZE, LANDSIZE];

        const int LUMINANCE = 100;

        public int FoundLandblocks;

        public Mapper()
        {
            FoundLandblocks = 0;
            foreach(var entry in DatManager.CellDat.AllFiles)
            {
                if((entry.Key & 0x0000FFFF) == 0x0000FFFF)
                {
                    var block_x = entry.Key >> 24;
                    var block_y = (entry.Key & 0x00FF0000) >> 16;

                    int startX = (int)(block_x * 8);
                    int startY = (int)(LANDSIZE - block_y * 8 - 1);

                    CellLandblock landblock = DatManager.CellDat.ReadFromDat<CellLandblock>(entry.Key);

                    for (var x = 0; x < 9; x++)
                    {
                        for (var y = 0; y < 9; y++)
                        {
                            var type = landblock.Terrain[x * 9 + y];
                            var newZ = landblock.Height[x * 9 + y];

                             // Write new data point
                            land[startY - y,startX + x].Type = type;
                            land[startY - y,startX + x].Z = RegionHelper.GetLandheight(newZ);
                            land[startY - y,startX + x].Used = true;
                            uint itex = (uint)((type >> 2) & 0x3F);
                            if (itex < 16 || itex > 20)
                                land[startY - y, startX + x].Blocked = false;
                            else
                                land[startY - y, startX + x].Blocked = true;


                        }
                    }

                    FoundLandblocks++;
                }
            }

            CreateMap();
        }

        private void CreateMap()
        {
            var emptyColor = Properties.Settings.Default.EmptyLandblockColor;

            double[] v = new double[3];
            double[] lightVector = new double[3] { -1.0, -1.0, 0.0 };
            byte[,,] topo = new byte[LANDSIZE, LANDSIZE, 3];

            double color, light;
            ushort type;
            byte[,] landColor = RegionHelper.GetMapColors();

            for (var y = 0; y < LANDSIZE; y++)
            {
                for (var x = 0; x < LANDSIZE; x++)
                {
                    if (land[y,x].Used)
                    {
                        // Calculate normal by using surrounding z values, if they exist
                        v[0] = 0.0;
                        v[1] = 0.0;
                        v[2] = 0.0;
                        if ((x < LANDSIZE - 1) && (y < LANDSIZE - 1))
                        {
                            if (land[y,x + 1].Used && land[y + 1,x].Used)
                            {
                                v[0] -= land[y,x + 1].Z - land[y,x].Z;
                                v[1] -= land[y + 1,x].Z - land[y,x].Z;
                                v[2] += 12.0;
                            }
                        }
                        if ((x > 0) && (y < LANDSIZE - 1))
                        {
                            if (land[y,x - 1].Used && land[y + 1,x].Used)
                            {
                                v[0] += land[y,x - 1].Z - land[y,x].Z;
                                v[1] -= land[y + 1,x].Z - land[y,x].Z;
                                v[2] += 12.0;
                            }
                        }
                        if ((x > 0) && (y > 0))
                        {
                            if (land[y,x - 1].Used && land[y - 1,x].Used)
                            {
                                v[0] += land[y,x - 1].Z - land[y,x].Z;
                                v[1] += land[y - 1,x].Z - land[y,x].Z;
                                v[2] += 12.0;
                            }
                        }
                        if ((x < LANDSIZE - 1) && (y > 0))
                        {
                            if (land[y,x + 1].Used && land[y - 1,x].Used)
                            {
                                v[0] -= land[y,x + 1].Z - land[y,x].Z;
                                v[1] += land[y - 1,x].Z - land[y,x].Z;
                                v[2] += 12.0;
                            }
                        }

                        // Check for road bit(s)
                        if ((land[y,x].Type & 0x0003) != 0)
                            type = 32;
                        else
                            type = (ushort)((land[y,x].Type & 0xFF) >> 2);

                        // Calculate lighting scalar
                        light = (((lightVector[0] * v[0] + lightVector[1] * v[1] + lightVector[2] * v[2]) /
                            Math.Sqrt((lightVector[0] * lightVector[0] + lightVector[1] * lightVector[1] + lightVector[2] * lightVector[2]) *
                            (v[0] * v[0] + v[1] * v[1] + v[2] * v[2]))) * 128.0 + 128.0) * LIGHTCORRECTION + AMBIENTLIGHT;

                        // Apply lighting scalar to base colors
                        for (int i = 0; i < 3; i++)
                        {
                            color = (landColor[type, i] * COLORCORRECTION / 100) * light / 256.0;
                            if (color > 255.0)
                                topo[y, x, i] = 255;
                            else if (color < 0.0)
                                topo[y, x, i] = 0;
                            else
                                topo[y, x, i] = (byte)color;
                        }
                    }
                    else
                    {
                        // If data is not present for a point on the map, the resultant pixel is green
                        topo[y, x, 0] = emptyColor.R; // R
                        topo[y, x, 1] = emptyColor.G;   // G
                        topo[y, x, 2] = emptyColor.B;   // B
                    }
                }
            }

            MapImage = new Bitmap(LANDSIZE, LANDSIZE);

            for (var y = 0; y < LANDSIZE; y++)
            {
                for (var x = 0; x < LANDSIZE; x++)
                {
                    Color pixColor = Color.FromArgb(topo[y, x, 0], topo[y, x, 1], topo[y, x, 2]);
                    MapImage.SetPixel(x, y, pixColor);
                }
            }
            
            //map.Save(mapFile, ImageFormat.Png);
        }
    }
}
