using System.IO;

namespace MapAC.DatLoader.Entity.AnimationHooks
{
    public class SoundHook : AnimationHook
    {
        public uint Id { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            base.Unpack(reader);

            Id = reader.ReadUInt32();
        }
    }
}
