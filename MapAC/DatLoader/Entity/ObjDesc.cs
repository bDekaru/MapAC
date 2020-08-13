using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class ObjDesc : IUnpackable
    {
        public uint PaletteID { get; private set; }
        public List<SubPalette> SubPalettes { get; } = new List<SubPalette>();
        public List<TextureMapChange> TextureChanges { get; } = new List<TextureMapChange>();
        public List<AnimationPartChange> AnimPartChanges { get; } = new List<AnimationPartChange>();

        public void Unpack(BinaryReader reader)
        {
            reader.AlignBoundary();

            reader.ReadByte(); // ObjDesc always starts with 11.

            var numPalettes             = reader.ReadByte();
            var numTextureMapChanges    = reader.ReadByte();
            var numAnimPartChanges      = reader.ReadByte();

            if (numPalettes > 0)
                PaletteID = reader.ReadAsDataIDOfKnownType(0x04000000);

            SubPalettes.Unpack(reader, numPalettes);
            TextureChanges.Unpack(reader, numTextureMapChanges);
            AnimPartChanges.Unpack(reader, numAnimPartChanges);

            reader.AlignBoundary();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.AlignBoundary();

            writer.Write((byte)0x11); // ObjDesc always starts with 11.

            writer.Write((byte)SubPalettes.Count);
            writer.Write((byte)TextureChanges.Count);
            writer.Write((byte)AnimPartChanges.Count);

            if (SubPalettes.Count > 0)
                writer.WriteAsDataIDOfKnownType(PaletteID, 0x04000000);

            SubPalettes.Pack(writer);
            TextureChanges.Pack(writer);
            AnimPartChanges.Pack(writer);

            writer.AlignBoundary();
        }

    }
}
