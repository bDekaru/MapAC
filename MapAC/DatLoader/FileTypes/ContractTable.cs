using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Entity;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// This is the client_portal.dat file 0x0E00001D
    /// </summary>
    [DatFileType(DatFileType.ContractTable)]
    public class ContractTable : FileType
    {
        internal const uint FILE_ID = 0x0E00001D;

        public Dictionary<uint, Contract> Contracts { get; } = new Dictionary<uint, Contract>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            ushort num_contracts = reader.ReadUInt16();
            /*ushort table_size = */reader.ReadUInt16(); // We don't need this since C# handles it's own memory -- 0x1000

            for (ushort i = 0; i < num_contracts; i++)
            {
                uint key = reader.ReadUInt32();

                Contract value = new Contract();
                value.Unpack(reader);

                Contracts.Add(key, value);
            }
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write((ushort)Contracts.Count);
            writer.Write((ushort)0x1000); // table_size

            foreach(var e in Contracts)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }
    }
}