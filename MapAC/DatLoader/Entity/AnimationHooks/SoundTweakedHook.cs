using MapAC.DatLoader.Enum;
using System.IO;

namespace MapAC.DatLoader.Entity.AnimationHooks
{
    public class SoundTweakedHook : AnimationHook
    {
        public uint SoundID { get; private set; }
        public float Priority { get; private set; }
        public float Probability { get; private set; }
        public float Volume { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            base.Unpack(reader);

            SoundID = reader.ReadUInt32();
            Priority = reader.ReadSingle();
            Probability = reader.ReadSingle();
            Volume = reader.ReadSingle();
        }

        public override void Pack(BinaryWriter writer)
        {
            base.Pack(writer);
            writer.WriteOffset(SoundID, ACDMOffset.Wave);
            writer.Write(Priority);
            writer.Write(Probability);
            writer.Write(Volume);
        }
    }
}
