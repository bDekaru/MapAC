using MapAC.DatLoader.Enum;
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
            if(DatManager.DatVersion == DatVersionType.ACDM)
                writer.Write(ScriptId + (uint)ACDMOffset.PhysicsScript);
            else
                writer.Write(ScriptId);
        }

    }
}
