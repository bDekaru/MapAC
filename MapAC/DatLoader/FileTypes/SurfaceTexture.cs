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

        public SurfacePixelFormat Format;
        public int Width;
        public int Height;
        public int Length;
        public uint? DefaultPaletteId;
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
                    Format = (SurfacePixelFormat)reader.ReadInt32();
                    Width = reader.ReadInt32();
                    Height = reader.ReadInt32();
                    SourceData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    switch (Format)
                    {
                        case SurfacePixelFormat.INDEX8:
                            reader.BaseStream.Position -= 4; // move position back 4 bytes
                            DefaultPaletteId = reader.ReadUInt32();
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Note that for Pre-TOD SurfaceTextures, this will convert to a Post-TOD SurfaceTexture.
        /// </summary>
        /// <param name="writer"></param>
        public override void Pack(BinaryWriter writer)
        {
            switch (DatManager.DatVersion)
            {
                case DatVersionType.ACTOD:
                    writer.Write(Id);
                    writer.Write(Unknown); // Always 0?
                    writer.Write(UnknownByte); // Always 2?
                    Textures.Pack(writer);
                    break;
                case DatVersionType.ACDM:
                    // The max value in the end-of-retail Client_portal.dat was 05003358. We will add 0x00010000 to this to ensure a unique value.
                    var newId = Id + 0x00010000;
                    writer.Write(newId);
                    writer.Write(Unknown); // Always 0?
                    writer.Write(UnknownByte); // Always 2?
                    Textures.Clear();
                    uint newTextureId = Id + 0x01010000; // Generates a unique 0x06 range TextureId
                    Textures.Add(newTextureId);
                    Textures.Pack(writer);
                    break;
            }
        }

        /// <summary>
        /// This will Pack a pre-TOD SurfaceTexture into a post-TOD Surface format.
        /// The highest 06-Texture ID in the retail client_portal.dat was 06007576.
        /// These new textures will just add 0x01010000 to that value to be a unique value in the 0x06 range.
        /// 
        /// Note the Texture range goes all the way up to 0x07FFFFFF.
        /// </summary>
        /// <param name="writer"></param>
        public void PackAsTexture(BinaryWriter writer)
        {
            if(DatManager.DatVersion == DatVersionType.ACTOD) throw new System.NotSupportedException();
            Texture tex = ConvertToTexture();
            tex.SetIdFromSurfaceTexture(Id);
            tex.Pack(writer);
        }

        public Texture ConvertToTexture()
        {
            Texture tex = new Texture();
            tex.Width = Width;
            tex.Height = Height;
            tex.SourceData = SourceData;
            tex.Format = Format;
            switch (Format)
            {
                case SurfacePixelFormat.COLOR_SEP:
                    tex.Length = Width * Height * 3;
                    break;
                case SurfacePixelFormat.INDEX8:
                    tex.DefaultPaletteId = DefaultPaletteId;
                    tex.DefaultPaletteId = 0x040010b1;
                    tex.Length = Width * Height * 8;
                    break;
            }
            return tex;
        }

        public Bitmap GetBitmap()
        {
            if(Width == 0)
            {
                return null;
            }
            /// Just copy this into a Texture type, since that already does all this image work for us...
            Texture tex = ConvertToTexture();
            return tex.GetBitmap();
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
    }
}
