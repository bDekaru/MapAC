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
        byte[,] landColor_tod = new byte[33, 3]{
            {102, 88, 72}, // 0 - BarrenRock - 0x06006d6f
            {89, 94, 47}, // 1 - Grassland - 0x06006d40
            {175, 179, 178}, // 2 - Ice - 0x06006d4b
            {95, 94, 36}, // 3 - LushGrass - 0x06006d06
            {67, 47, 20}, // 4 - MarshSparseSwamp - 0x06006d4a
            {56, 39, 21}, // 5 - MudRichDirt - 0x06006d46
            {23, 17, 27}, // 6 - ObsidianPlain - 0x06006d56
            {112, 84, 50}, // 7 - PackedDirt - 0x06006d48
            {98, 80, 58}, // 8 - PatchyDirt - 0x06006d42
            {70, 72, 36}, // 9 - PatchyGrassland - 0x06006d3c
            {215, 155, 103}, // 10 - sand-yellow - 0x06006d43
            {148, 129, 107}, // 11 - sand-grey - 0x06006d44
            {183, 144, 109}, // 12 - sand-rockStrewn - 0x06006d53
            {151, 121, 87}, // 13 - SedimentaryRock - 0x06006d51
            {88, 82, 55}, // 14 - SemiBarrenRock - 0x06006d41
            {191, 196, 201}, // 15 - Snow - 0x06006d47
            {35, 76, 110}, // 16 - WaterRunning - 0x06006d4d
            {21, 68, 80}, // 17 - WaterStandingFresh - 0x06006d45
            {36, 41, 68}, // 18 - WaterShallowSea - 0x06006d4f
            {31, 63, 57}, // 19 - WaterShallowStillSea - 0x06006d4c
            {31, 35, 62}, // 20 - WaterDeepSea - 0x06006d4e
            {90, 95, 41}, // 21 - forestfloor - 0x06006d49
            {35, 76, 110}, // 22 - FauxWaterRunning - 0x06006d4d
            {70, 90, 66}, // 23 - SeaSlime - 0x06006d55
            {102, 88, 72}, // 24 - Argila - 0x06006d6f
            {28, 19, 23}, // 25 - Volcano1 - 0x06006d54
            {103, 103, 103}, // 26 - Volcano2 - 0x06006d6a
            {127, 164, 163}, // 27 - BlueIce - 0x06006d50
            {70, 72, 36}, // 28 - Moss - 0x06006d3c
            {65, 54, 22}, // 29 - DarkMoss - 0x06006d3d
            {70, 57, 56}, // 30 - olthoi - 0x06006d3e
            {102, 88, 72}, // 31 - DesolateLands - 0x06006d6f
            {112, 116, 105}, // 32 - roads - 0x06006d3f   
        };
        byte[,] landColor_acdm = new byte[33, 3]{
            {101, 81, 46}, // 0 - BarrenRock - 0x0500145c
            {68, 80, 26}, // 1 - Grassland - 0x05001459
            {133, 133, 145}, // 2 - Ice - 0x05001468
            {62, 84, 15}, // 3 - LushGrass - 0x05001456
            {83, 46, 11}, // 4 - MarshSparseSwamp - 0x05001467
            {65, 43, 29}, // 5 - MudRichDirt - 0x05001462
            {48, 44, 50}, // 6 - ObsidianPlain - 0x05001463
            {104, 77, 41}, // 7 - PackedDirt - 0x05001465
            {68, 50, 14}, // 8 - PatchyDirt - 0x0500145b
            {53, 91, 3}, // 9 - PatchyGrassland - 0x05001457
            {166, 116, 59}, // 10 - sand-yellow - 0x0500145d
            {152, 152, 108}, // 11 - sand-grey - 0x0500145f
            {157, 107, 60}, // 12 - sand-rockStrewn - 0x0500145e
            {128, 81, 49}, // 13 - SedimentaryRock - 0x050014a7
            {86, 97, 65}, // 14 - SemiBarrenRock - 0x0500145a
            {234, 247, 254}, // 15 - Snow - 0x05001464
            {63, 110, 133}, // 16 - WaterRunning - 0x0500146a
            {23, 81, 58}, // 17 - WaterStandingFresh - 0x05001461
            {34, 82, 101}, // 18 - WaterShallowSea - 0x0500146c
            {46, 78, 96}, // 19 - WaterShallowStillSea - 0x05001469
            {46, 61, 84}, // 20 - WaterDeepSea - 0x0500146b
            {37, 49, 9}, // 21 - forestfloor - 0x05001466
            {63, 110, 133}, // 22 - FauxWaterRunning - 0x0500146a
            {33, 41, 30}, // 23 - SeaSlime - 0x05001827
            {101, 81, 46}, // 24 - Argila - 0x0500145c
            {47, 36, 36}, // 25 - Volcano1 - 0x0500181f
            {76, 77, 76}, // 26 - Volcano2 - 0x05001924
            {195, 224, 239}, // 27 - BlueIce - 0x05001900
            {116, 135, 87}, // 28 - Moss - 0x05001c3a
            {64, 99, 49}, // 29 - DarkMoss - 0x05001c3b
            {68, 41, 47}, // 30 - olthoi - 0x05001c3c
            {101, 81, 46}, // 31 - UNKNOWN - 0x0500145c
            {138, 131, 113}, // 32 - roads - 0x05001458
        };
        public Mapper()
        {
            uint found = 0;
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
                        }
                    }

                    found++;
                }
            }

            CreateMap();
        }

        private void CreateMap()
        {
            double[] v = new double[3];
            double[] lightVector = new double[3] { -1.0, -1.0, 0.0 };
            byte[,,] topo = new byte[LANDSIZE, LANDSIZE, 3];

            double color, light;
            ushort type;
            byte[,] landColor = landColor_acdm;

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
                        topo[y, x, 0] = 0; // R
                        topo[y, x, 1] = 255;   // G
                        topo[y, x, 2] = 0;   // B
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
