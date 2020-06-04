using MapAC.DatLoader.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// SurfaceTexture pre-TOD was just that, a texture for surfaces. After TOD, the Texture format was modified to contain all different types of texture formats and the 
    /// SurfaceTexture class was re-purposed to handle data for different texture resolutions
    /// </summary>
    [DatFileType(DatFileType.SurfaceTexture)]
    public class SurfaceTexture : FileType
    {
        // public int Id { get; private set; }
        public int Unknown { get; private set; }
        public byte UnknownByte { get; private set; }
        // public int TextureCount { get; private set; }
        public List<uint> Textures { get; private set; } = new List<uint>(); // These values correspond to a Surface (0x06) entry

        public int Format;
        public int Width;
        public int Height;
        public byte[] SourceData { get; set; }

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();
            switch (DatManager.DatVersion)
            {
                case DatVersionType.ACTOD:
                    Unknown = reader.ReadInt32();
                    UnknownByte = reader.ReadByte();
                    Textures.Unpack(reader);
                    break;
                case DatVersionType.ACDM:
                    Format = reader.ReadInt32();
                    Width = reader.ReadInt32();
                    Height = reader.ReadInt32();
                    SourceData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    break;
            }
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Bitmap GetBitmap()
        {
            if(Width == 0)
            {
                return null;
            }
            /// Just copy this into a Texture type, since that already does all this image work for us...
            Texture tex = new Texture();
            tex.Width = Width;
            tex.Height = Height;
            tex.SourceData = SourceData;
            switch (Format)
            {
                case 0xa:
                    tex.Format = SurfacePixelFormat.COLOR_SEP;
                    tex.Length = Width * Height * 3;
                    break;
                default:
                    return null;
            }
            return tex.GetBitmap();
        }

        public int GetAverageColor()
        {
            var bmp = GetBitmap();

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

            int avg = (r << 16) | (g << 8) | b;
            return avg;
        }
    }
}
