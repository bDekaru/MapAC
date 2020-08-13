using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapAC.DatLoader.Entity
{
    public class SoundData : IUnpackable
    {

        public List<SoundTableData> Data = new List<SoundTableData>();
        public uint Unknown;

        public void Unpack(BinaryReader reader)
        {
            Data.Unpack(reader);
            Unknown = reader.ReadUInt32();
        }

        public void Pack(BinaryWriter writer)
        {
            Data.Pack(writer);
            writer.Write(Unknown);
        }

    }
}
