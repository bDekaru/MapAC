using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Entity;

namespace MapAC.DatLoader.FileTypes
{
    [DatFileType(DatFileType.StringTable)]
    public class StringTable : FileType
    {
        public static uint CharacterTitle_FileID = 0x2300000E;

        public uint Language { get; private set; } // This should always be 1 for English

        public byte Unknown { get; private set; }

        public List<StringTableData> StringTableData { get; } = new List<StringTableData>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            Language = reader.ReadUInt32();

            Unknown = reader.ReadByte();

            StringTableData.UnpackSmartArray(reader);
        }
    }
}
