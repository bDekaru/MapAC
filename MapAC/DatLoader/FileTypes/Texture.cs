/************************************************************************
 * Some of the Bitmap/ExportTexture uses code taken from DerethForever.
 * http://www.derethforever.com
 *
 * DerethForever is licensed under the GNU General Public License
 * http://www.gnu.org/licenses/
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.FileTypes
{
    public enum SurfacePixelFormat : uint
    {
        PFID_UNKNOWN = 0,
        // 1 - 12 are all pre TOD image formats
        INDEX4 = 1,
        INDEX8 = 2,
        ARGB1555 = 3,
        ARGB4444 = 4,
        ARGB8888 = 5,
        RGB555 = 6,
        RGB565 = 7,
        RGB888 = 8,
        MONO = 9,
        COLOR_SEP = 10,
        ALPHA_ONLY = 11,
        NUM_IMAGE_TYPE = 12,
        // Post TOD Image Formats
        PFID_R8G8B8 = 20, // Also seen in ACDM 0x06 UI formats
        PFID_A8R8G8B8 = 21,
        PFID_X8R8G8B8 = 22,
        PFID_R5G6B5 = 23,
        PFID_X1R5G5B5 = 24,
        PFID_A1R5G5B5 = 25,
        PFID_A4R4G4B4 = 26,
        PFID_R3G3B2 = 27,
        PFID_A8 = 28,
        PFID_A8R3G3B2 = 29,
        PFID_X4R4G4B4 = 30,
        PFID_A2B10G10R10 = 31,
        PFID_A8B8G8R8 = 32,
        PFID_X8B8G8R8 = 33,
        PFID_A2R10G10B10 = 35,
        PFID_A8P8 = 40,
        PFID_P8 = 41,
        PFID_L8 = 50,
        PFID_A8L8 = 51,
        PFID_A4L4 = 52,
        PFID_V8U8 = 60,
        PFID_L6V5U5 = 61,
        PFID_X8L8V8U8 = 62,
        PFID_Q8W8V8U8 = 63,
        PFID_V16U16 = 64,
        PFID_A2W10V10U10 = 67,
        PFID_D16_LOCKABLE = 70,
        PFID_D32 = 71,
        PFID_D15S1 = 73,
        PFID_D24S8 = 75,
        PFID_D24X8 = 77,
        PFID_D24X4S4 = 79,
        PFID_D16 = 80,
        PFID_VERTEXDATA = 100,
        PFID_INDEX16 = 101,
        PFID_INDEX32 = 102,
        PFID_CUSTOM_R8G8B8A8 = 240,
        PFID_CUSTOM_FIRST = 240,
        PFID_CUSTOM_A8B8G8R8 = 241,
        PFID_CUSTOM_B8G8R8 = 242,
        PFID_CUSTOM_LSCAPE_R8G8B8 = 243,
        PFID_CUSTOM_LSCAPE_ALPHA = 244,
        PFID_CUSTOM_LAST = 500,
        PFID_CUSTOM_RAW_JPEG = 500,
        PFID_DXT1 = 827611204,
        PFID_DXT2 = 844388420,
        PFID_YUY2 = 844715353,
        PFID_DXT3 = 861165636,
        PFID_DXT4 = 877942852,
        PFID_DXT5 = 894720068,
        PFID_G8R8_G8B8 = 1111970375,
        PFID_R8G8_B8G8 = 1195525970,
        PFID_UYVY = 1498831189,
        PFID_INVALID = 2147483647,
    }

    [DatFileType(DatFileType.Texture)]
    public class Texture : FileType
    {
        public int Unknown { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public SurfacePixelFormat Format { get; set; }
        public int Length { get; set; }
        public byte[] SourceData { get; set; }
        public uint? DefaultPaletteId { get; set; }

        // Used to store a custom palette. Each Key represents the PaletteIndex and the Value is the color.
        // This is used if you want to apply a non-default Palette to the image prior to extraction
        public Dictionary<int, uint> CustomPaletteColors = new Dictionary<int, uint>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();
            switch(DatManager.DatVersion){
                case DatVersionType.ACTOD:
                    Unknown = reader.ReadInt32();
                    Width = reader.ReadInt32();
                    Height = reader.ReadInt32();
                    Format = (SurfacePixelFormat)reader.ReadUInt32();
                    Length = reader.ReadInt32();
                    SourceData = reader.ReadBytes(Length);
                    break;
                case DatVersionType.ACDM:
                    Width = reader.ReadInt32();
                    Height = reader.ReadInt32();
                    Length = Width * Height * 3;
                    Format = SurfacePixelFormat.PFID_R8G8B8;
                    SourceData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    break;
            }


            switch (Format)
            {
                case SurfacePixelFormat.PFID_INDEX16:
                case SurfacePixelFormat.PFID_P8:
                    DefaultPaletteId = reader.ReadUInt32();
                    break;
                default:
                    DefaultPaletteId = null;
                    break;
            }
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.Write(Id);
            if (DatManager.DatVersion == DatVersionType.ACTOD)
            {
                writer.Write(Unknown);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write((uint)Format);
                writer.Write(Length);
                writer.Write(SourceData);
                if (DefaultPaletteId != null)
                    writer.Write((uint)DefaultPaletteId);
            }
            else
            {
                // ACDM 
                writer.Write(Unknown);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write((uint)Format);
                Length = SourceData.Length;
                switch (Format)
                {
                    case SurfacePixelFormat.INDEX8:
                        // Remove 4 from the length which is the DefaultPaletteId.
                        // This is already part of the SourceData, so just back up the length...
                        Length -= 4;
                        break;
                }
                writer.Write(Length);
                writer.Write(SourceData);
            }
        }

        /// <summary>
        /// A little helper function to set the Id when converting from a Pre-TOD SurfaceTexture format
        /// 
        /// Converts a SurfaceTexture ID, e.g. 0x0500NNNN to a unique 0x06001NNNN value.
        /// </summary>
        /// <param name="SurfaceTextureId"></param>
        public void SetIdFromSurfaceTexture(uint SurfaceTextureId)
        {
            if (DatManager.DatVersion == DatVersionType.ACTOD) throw new System.NotSupportedException();
            Id = SurfaceTextureId + 0x01000000 + (uint)ACDMOffset.Texture;
        }

        /// <summary>
        /// Exports RenderSurface to a image file
        /// </summary>
        public void ExportTexture(string directory)
        {
            if (Length == 0) return;

            switch (Format)
            {
                case SurfacePixelFormat.PFID_CUSTOM_RAW_JPEG:
                    {
                        string filename = Path.Combine(directory, Id.ToString("X8") + ".jpg");
                        using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
                        {
                            writer.Write(SourceData);
                        }
                    }
                    break;

                default:
                    {
                        var bitmapImage = GetBitmap();
                        string filename = Path.Combine(directory, Id.ToString("X8") + ".png");
                        bitmapImage.Save(filename, ImageFormat.Png);
                    }
                    break;
            }
        }

        /// <summary>
        /// Reads RenderSurface to bitmap structure
        /// </summary>
        public Bitmap GetBitmap()
        {
            switch (Format)
            {
                case SurfacePixelFormat.PFID_CUSTOM_RAW_JPEG:
                    {
                        var stream = new MemoryStream(SourceData);
                        var image = Image.FromStream(stream);
                        return new Bitmap(image);
                    }
                case SurfacePixelFormat.PFID_DXT1:
                    {
                        var image = DxtUtil.DecompressDxt1(SourceData, Width, Height);
                        return GetBitmap(image);
                    }
                case SurfacePixelFormat.PFID_DXT3:
                    {
                        var image = DxtUtil.DecompressDxt3(SourceData, Width, Height);
                        return GetBitmap(image);
                    }
                case SurfacePixelFormat.PFID_DXT5:
                    {
                        var image = DxtUtil.DecompressDxt5(SourceData, Width, Height);
                        return GetBitmap(image);
                    }
                default:
                    {
                        List<int> colors = GetImageColorArray();
                        return GetBitmap(colors);
                    }
            }
        }

        /// <summary>
        /// Converts the byte array SourceData into color values per pixel
        /// </summary>
        private List<int> GetImageColorArray()
        {
            List<int> colors = new List<int>();
            if (Length == 0) return colors;

            switch (Format)
            {
                case SurfacePixelFormat.COLOR_SEP: // Has all the red pixel data, then green, then blue
                    // We're just going to stuff them all into a RGB8 format for an easier time later
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        List<byte> rB = new List<byte>();
                        List<byte> gB = new List<byte>();
                        List<byte> bB = new List<byte>();

                        var totalPixels = Height * Width;

                        for (uint i = 0; i < totalPixels; i++)
                            rB.Add(reader.ReadByte());
                        for (uint i = 0; i < totalPixels; i++)
                            gB.Add(reader.ReadByte());
                        for (uint i = 0; i < totalPixels; i++)
                            bB.Add(reader.ReadByte());

                        for (uint i = 0; i < Height; i++)
                            for (uint j = 0; j < Width; j++)
                            {
                                int idx = (int)((i * Width) + j);

                                byte r = rB[idx];
                                byte g = gB[idx];
                                byte b = bB[idx];
                                int color = (r << 16) | (g << 8) | b;
                                colors.Add(color);
                            }
                    }
                    break;
                case SurfacePixelFormat.PFID_R8G8B8: // RGB
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint i = 0; i < Height; i++)
                            for (uint j = 0; j < Width; j++)
                            {
                                byte r, g, b;
                                if(DatManager.DatVersion == DatVersionType.ACDM)
                                {
                                    r = reader.ReadByte();
                                    g = reader.ReadByte();
                                    b = reader.ReadByte();
                                }
                                else
                                {
                                    b = reader.ReadByte();
                                    g = reader.ReadByte();
                                    r = reader.ReadByte();
                                }
                                int color = (r << 16) | (g << 8) | b;
                                colors.Add(color);
                            }
                    }
                    break;
                case SurfacePixelFormat.PFID_CUSTOM_LSCAPE_R8G8B8:
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint i = 0; i < Height; i++)
                            for (uint j = 0; j < Width; j++)
                            {
                                byte r = reader.ReadByte();
                                byte g = reader.ReadByte();
                                byte b = reader.ReadByte();
                                int color = (r << 16) | (g << 8) | b;
                                colors.Add(color);
                            }
                    }
                    break;
                case SurfacePixelFormat.PFID_A8R8G8B8: // ARGB format. Most UI textures fall into this category
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint i = 0; i < Height; i++)
                            for (uint j = 0; j < Width; j++)
                                colors.Add(reader.ReadInt32());
                    }
                    break;
                case SurfacePixelFormat.PFID_INDEX16: // 16-bit indexed colors. Index references position in a palette;
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint y = 0; y < Height; y++)
                            for (uint x = 0; x < Width; x++)
                                colors.Add(reader.ReadInt16());
                    }
                    break;
                case SurfacePixelFormat.PFID_A8: // Greyscale, also known as Cairo A8.
                case SurfacePixelFormat.PFID_CUSTOM_LSCAPE_ALPHA:
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint y = 0; y < Height; y++)
                            for (uint x = 0; x < Width; x++)
                                colors.Add(reader.ReadByte());
                    }
                    break;
                case SurfacePixelFormat.PFID_P8: // Indexed
                case SurfacePixelFormat.INDEX8:
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint y = 0; y < Height; y++)
                            for (uint x = 0; x < Width; x++)
                                colors.Add(reader.ReadByte());
                    }
                    break;
                case SurfacePixelFormat.PFID_R5G6B5: // 16-bit RGB
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint y = 0; y < Height; y++)
                            for (uint x = 0; x < Width; x++)
                            {
                                ushort val = reader.ReadUInt16();
                                List<int> color = get565RGB(val);
                                colors.Add(color[0]); // Red
                                colors.Add(color[1]); // Green
                                colors.Add(color[2]); // Blue
                            }
                    }
                    break;
                case SurfacePixelFormat.PFID_A4R4G4B4:
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint y = 0; y < Height; y++)
                            for (uint x = 0; x < Width; x++)
                            {
                                ushort val = reader.ReadUInt16();
                                int alpha = (val >> 12) / 0xF * 255;
                                int red = (val >> 8 & 0xF) / 0xF * 255;
                                int green = (val >> 4 & 0xF) / 0xF * 255;
                                int blue = (val & 0xF) / 0xF * 255;

                                colors.Add(alpha);
                                colors.Add(red);
                                colors.Add(green);
                                colors.Add(blue);
                            }
                    }
                    break;
                default:
                    Console.WriteLine("Unhandled SurfacePixelFormat (" + Format.ToString() + ") in RenderSurface " + Id.ToString("X8"));
                    break;
            }

            return colors;
        }

        private List<int> GetPaletteIndexes()
        {
            List<int> colors = new List<int>();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
            {
                for (uint y = 0; y < Height; y++)
                    for (uint x = 0; x < Width; x++)
                        colors.Add(reader.ReadInt16());
            }
            return colors;
        }

        /// <summary>
        /// Generates Bitmap data from colorArray.
        /// </summary>
        private Bitmap GetBitmap(List<int> colorArray)
        {
            Bitmap image = new Bitmap(Width, Height);
            switch (this.Format)
            {
                case SurfacePixelFormat.COLOR_SEP:
                case SurfacePixelFormat.PFID_R8G8B8:
                case SurfacePixelFormat.PFID_CUSTOM_LSCAPE_R8G8B8:
                    for (int i = 0; i < Height; i++)
                        for (int j = 0; j < Width; j++)
                        {
                            int idx = (i * Width) + j;
                            int r = (colorArray[idx] & 0xFF0000) >> 16;
                            int g = (colorArray[idx] & 0xFF00) >> 8;
                            int b = colorArray[idx] & 0xFF;
                            image.SetPixel(j, i, Color.FromArgb(r, g, b));
                        }
                    break;
                case SurfacePixelFormat.PFID_A8R8G8B8:
                    for (int i = 0; i < Height; i++)
                        for (int j = 0; j < Width; j++)
                        {
                            int idx = (i * Width) + j;
                            int a = (int)((colorArray[idx] & 0xFF000000) >> 24);
                            int r = (colorArray[idx] & 0xFF0000) >> 16;
                            int g = (colorArray[idx] & 0xFF00) >> 8;
                            int b = colorArray[idx] & 0xFF;
                            image.SetPixel(j, i, Color.FromArgb(a, r, g, b));
                        }
                    break;
                case SurfacePixelFormat.INDEX8:
                case SurfacePixelFormat.PFID_INDEX16:
                case SurfacePixelFormat.PFID_P8:
                    Palette pal = DatManager.CellDat.ReadFromDat<Palette>((uint)DefaultPaletteId);

                    // Apply any custom palette colors, if any, to our loaded palette (note, this may be all of them!)
                    if (CustomPaletteColors.Count > 0)
                        foreach (KeyValuePair<int, uint> entry in CustomPaletteColors)
                            if (entry.Key <= pal.Colors.Count)
                                pal.Colors[entry.Key] = entry.Value;

                    for (int i = 0; i < Height; i++)
                        for (int j = 0; j < Width; j++)
                        {
                            int idx = (i * Width) + j;
                            int a = (int)((pal.Colors[colorArray[idx]] & 0xFF000000) >> 24);
                            int r = (int)(pal.Colors[colorArray[idx]] & 0xFF0000) >> 16;
                            int g = (int)(pal.Colors[colorArray[idx]] & 0xFF00) >> 8;
                            int b = (int)pal.Colors[colorArray[idx]] & 0xFF;
                            image.SetPixel(j, i, Color.FromArgb(a, r, g, b));
                        }
                    break;
                case SurfacePixelFormat.PFID_A8:
                case SurfacePixelFormat.PFID_CUSTOM_LSCAPE_ALPHA:
                    for (int i = 0; i < Height; i++)
                        for (int j = 0; j < Width; j++)
                        {
                            int idx = (i * Width) + j;
                            int r = colorArray[idx];
                            int g = colorArray[idx];
                            int b = colorArray[idx];
                            image.SetPixel(j, i, Color.FromArgb(r, g, b));
                        }
                    break;
                case SurfacePixelFormat.PFID_R5G6B5: // 16-bit RGB
                    for (int i = 0; i < Height; i++)
                        for (int j = 0; j < Width; j++)
                        {
                            int idx = 3 * ((i * Width) + j);
                            int r = (int)(colorArray[idx]);
                            int g = (int)(colorArray[idx + 1]);
                            int b = (int)(colorArray[idx + 2]);
                            image.SetPixel(j, i, Color.FromArgb(r, g, b));
                        }
                    break;
                case SurfacePixelFormat.PFID_A4R4G4B4:
                    for (int i = 0; i < Height; i++)
                        for (int j = 0; j < Width; j++)
                        {
                            int idx = 4 * ((i * Width) + j);
                            int a = (colorArray[idx]);
                            int r = (colorArray[idx + 1]);
                            int g = (colorArray[idx + 2]);
                            int b = (colorArray[idx + 3]);
                            image.SetPixel(j, i, Color.FromArgb(a, r, g, b));
                        }
                    break;
            }
            return image;
        }

        /// <summary>
        /// Generates Bitmap data from byteArray, generated by DXT1, DXT3, and DXT5 image foramts.
        /// </summary>
        private Bitmap GetBitmap(byte[] byteArray)
        {
            Bitmap image = new Bitmap(Width, Height);
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    int idx = 4 * ((i * Width) + j);
                    int r = (int)(byteArray[idx]);
                    int g = (int)(byteArray[idx + 1]);
                    int b = (int)(byteArray[idx + 2]);
                    int a = (int)(byteArray[idx + 3]);
                    image.SetPixel(j, i, Color.FromArgb(a, r, g, b));
                }

            return image;
        }

        // https://docs.microsoft.com/en-us/windows/desktop/DirectShow/working-with-16-bit-rgb
        private List<int> get565RGB(ushort val)
        {
            List<int> color = new List<int>();

            int red_mask = 0xF800;
            int green_mask = 0x7E0;
            int blue_mask = 0x1F;

            int red = ((val & red_mask) >> 11) << 3;
            int green = ((val & green_mask) >> 5) << 2;
            int blue = (val & blue_mask) << 3;

            color.Add(red); // Red
            color.Add(green); // Green
            color.Add(blue); // Blue

            return color;
        }

        public Color GetAverageColor()
        {
            var bmp = GetBitmap();

            if (bmp == null)
                return Color.FromArgb(0, 255, 0); // TRANSPARENT

            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Will convert some pre TOD texture formats to post TOD textures formats
        /// </summary>
        private void ConvertTextureFormat()
        {
            switch (Format)
            {
                case SurfacePixelFormat.INDEX8:
                    List<byte> colors = new List<byte>(); // We'll store the values here temporarily
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(SourceData)))
                    {
                        for (uint y = 0; y < Height; y++)
                            for (uint x = 0; x < Width; x++)
                                colors.Add(reader.ReadByte());
                    }
                    Format = SurfacePixelFormat.PFID_INDEX16;
                    break;

            }
        }
    }
}
