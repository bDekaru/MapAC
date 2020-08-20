using System.IO;

namespace MapAC.DatLoader.Entity.AnimationHooks
{
    public class NoDrawHook : AnimationHook
    {
        public uint NoDraw { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            base.Unpack(reader);

            NoDraw = reader.ReadUInt32();
        }

        public override void Pack(BinaryWriter writer)
        {
            base.Pack(writer);
            writer.Write(NoDraw);
        }
    }
}
