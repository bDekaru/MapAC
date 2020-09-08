using MapAC.DatLoader.Enum;
using System.IO;

namespace MapAC.DatLoader.Entity.AnimationHooks
{
    public class CallPESHook : AnimationHook
    {
        public uint PES { get; private set; }
        public float Pause { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            base.Unpack(reader);

            PES     = reader.ReadUInt32();
            Pause   = reader.ReadSingle();
        }

        public override void Pack(BinaryWriter writer)
        {
            base.Pack(writer);
            writer.WriteOffset(PES, ACDMOffset.PhysicsScript);
            writer.Write(Pause);
        }

    }
}
