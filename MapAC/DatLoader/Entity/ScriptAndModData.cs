using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class ScriptAndModData : IUnpackable
    {
        public float Mod { get; private set; }
        public uint ScriptId { get; private set; }

        public void Unpack(BinaryReader reader)
        {
            Mod         = reader.ReadSingle();
            ScriptId    = reader.ReadUInt32();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(Mod);
            writer.Write(ScriptId);
        }

    }
}
