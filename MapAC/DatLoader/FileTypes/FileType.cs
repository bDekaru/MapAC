using HtmlAgilityPack;
using System.IO;

namespace MapAC.DatLoader.FileTypes
{
    public abstract class FileType : IUnpackable
    {
        public uint Id { get; set; }

        public abstract void Unpack(BinaryReader reader);
        public abstract void Pack(BinaryWriter writer);

        public uint GetHash(uint forceDifferentId = 1)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                DatManager.ForcePackWithDifferentId = forceDifferentId;
                Pack(writer);
                DatManager.ForcePackWithDifferentId = 0;
                var hash = Crc32.CRC32Bytes(stream.GetBuffer());
                return hash;
            }
        }
    }
}
