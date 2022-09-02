using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Entity;
using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x12. 
    /// </summary>
    [DatFileType(DatFileType.Scene)]
    public class Scene : FileType
    {
        public List<ObjectDesc> Objects { get; } = new List<ObjectDesc>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            Objects.Unpack(reader);
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.WriteOffset(Id, ACDMOffset.Scene);
            Objects.Pack(writer);
        }
    }
}
