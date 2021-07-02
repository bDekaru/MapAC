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

        public Mapper(List<Color> MapColors = null)
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

                    if(entry.Key == 0x60a1ffff)
                    {
                        var test = "here";
                    }
                    CellLandblock landblock = DatManager.CellDat.ReadFromDat<CellLandblock>(entry.Key);
                    bool hasBuildings = landblock.HasObjects;
                    /*
                    if (landblock.HasObjects)
                    {
                        //uint lbId = (entry.Key >> 16) | 0x0000FFFE;
                        uint baseId = (uint)entry.Key >> 16 << 16;
                        uint lbId = baseId | 0xFFFE;
                        LandblockInfo lbInfo = DatManager.CellDat.ReadFromDat<LandblockInfo>(lbId);
                        if (lbInfo.Buildings.Count > 0)
                        {
                            hasBuildings = true;
                        }
                    }
                    */
                    if (landblock.Terrain.Count > 0)
                    {
                        for (var x = 0; x < 9; x++)
                        {
                            for (var y = 0; y < 9; y++)
                            {
                                var type = landblock.Terrain[x * 9 + y];
                                var newZ = landblock.Height[x * 9 + y];

                                // Write new data point
                                if (hasBuildings) { 
                                    land[startY - y, startX + x].Type = 99;
                                }
                                else
                                {
                                    land[startY - y, startX + x].Type = type;
                                }
                                land[startY - y, startX + x].Z = RegionHelper.GetLandheight(newZ);
                                land[startY - y, startX + x].Used = true;
                                uint itex = (uint)((type >> 2) & 0x3F);
                                if (itex < 16 || itex > 20)
                                    land[startY - y, startX + x].Blocked = false;
                                else
                                    land[startY - y, startX + x].Blocked = true;
                            }
                        }


                        FoundLandblocks++;
                    }
                    else
                    {
                        var not_found = true;
                    }
                }
            }

            CreateMap(MapColors);
        }

        private void CreateMap(List<Color> MapColors = null)
        {
            var emptyColor = Properties.Settings.Default.EmptyLandblockColor;

            double[] v = new double[3];
            double[] lightVector = new double[3] { -1.0, -1.0, 0.0 };
            byte[,,] topo = new byte[LANDSIZE, LANDSIZE, 3];

            double color, light;
            ushort type;
            List<Color> landColor;
            if (MapColors == null)
                landColor = RegionHelper.GetMapColors();
            else
                landColor = MapColors;

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
                        double r, g, b;
                        if (landColor.Count > type)
                        {
                            r = (landColor[type].R * COLORCORRECTION / 100) * light / 256.0;
                            g = (landColor[type].G * COLORCORRECTION / 100) * light / 256.0;
                            b = (landColor[type].B * COLORCORRECTION / 100) * light / 256.0;
                        }
                        else
                        {
                            r = 0; g = 255; b = 0;
                        }
                        if(land[y, x].Type == 99)
                        {
                            r = 255;
                            g = 255;
                            b = 0;
                        }
                        r = ColorCheck(r);
                        g = ColorCheck(g);
                        b = ColorCheck(b);

                        topo[y, x, 0] = (byte)r;
                        topo[y, x, 1] = (byte)g;
                        topo[y, x, 2] = (byte)b;
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

        private double ColorCheck(double color)
        {
            if (color > 255.0)
                return 255;
            else if (color < 0.0)
                return 0;
            return color;
        }
    }
}
