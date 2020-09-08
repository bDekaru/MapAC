using MapAC.DatLoader.Enum;
using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class CloSubPalette : IUnpackable
    {
        /// <summary>
        /// Contains a list of valid offsets & color values
        /// </summary>
        public List<CloSubPaletteRange> Ranges { get; } = new List<CloSubPaletteRange>();
        /// <summary>
        /// Icon portal.dat 0x0F000000
        /// </summary>
        public uint PaletteSet { get; private set; }

        public void Unpack(BinaryReader reader)
        {
            Ranges.Unpack(reader);

            PaletteSet = reader.ReadUInt32();
        }

        public void Pack(BinaryWriter writer)
        {
            Ranges.Pack(writer);
            writer.WriteOffset(PaletteSet, ACDMOffset.PaletteSet);
        }

    }
}
