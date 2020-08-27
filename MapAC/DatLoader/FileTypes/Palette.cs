using MapAC.DatLoader.Enum;
using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x04. 
    /// </summary>
    [DatFileType(DatFileType.Palette)]
    public class Palette : FileType
    {
        /// <summary>
        /// Color data is stored in ARGB format
        /// </summary>
        public List<uint> Colors { get; } = new List<uint>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            Colors.Unpack(reader);
        }

        public override void Pack(BinaryWriter writer)
        {
            if (DatManager.DatVersion == DatVersionType.ACDM)
                writer.Write(Id + (uint)ACDMOffset.Palette);
            else
                writer.Write(Id);

            Colors.Pack(writer);
        }
    }
}
