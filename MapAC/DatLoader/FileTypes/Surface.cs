using System.IO;
using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x08.
    /// As the name implies this contains surface info for an object. Either texture reference or color and whatever effects applied to it.
    /// </summary>
    [DatFileType(DatFileType.Surface)]
    public class Surface : FileType
    {
        public SurfaceType Type { get; private set; }
        public uint OrigTextureId { get; private set; }
        public uint OrigPaletteId { get; private set; }
        public uint ColorValue { get; private set; }
        public float Translucency { get; private set; }
        public float Luminosity { get; private set; }
        public float Diffuse { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            if (DatManager.DatVersion == DatVersionType.ACDM)
                Id = reader.ReadUInt32();

            Type = (SurfaceType)reader.ReadUInt32();

            if (Type.HasFlag(SurfaceType.Base1Image) || Type.HasFlag(SurfaceType.Base1ClipMap))
            {
                // image or clipmap
                OrigTextureId = reader.ReadUInt32();
                OrigPaletteId = reader.ReadUInt32();
            }
            else
            {
                // solid color
                ColorValue = reader.ReadUInt32();
            }

            Translucency    = reader.ReadSingle();
            Luminosity      = reader.ReadSingle();
            Diffuse         = reader.ReadSingle();
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.Write((uint)Type);

            if (Type.HasFlag(SurfaceType.Base1Image) || Type.HasFlag(SurfaceType.Base1ClipMap))
            {
                if(Export.IsSurfaceTextureAddition(OrigTextureId, out var id))
                    writer.Write(OrigTextureId + (uint)ACDMOffset.SurfaceTexture);
                else
                    writer.Write(OrigTextureId);

                if (Export.IsAddition(OrigPaletteId))
                    writer.Write(OrigPaletteId + (uint)ACDMOffset.Palette);
                else
                    writer.Write(OrigPaletteId);
            }
            else
            {
                // solid color
                writer.Write(ColorValue);
            }

            writer.Write(Translucency);
            writer.Write(Luminosity);
            writer.Write(Diffuse);
        }
    }
}
