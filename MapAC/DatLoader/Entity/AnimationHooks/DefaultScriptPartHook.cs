using System.IO;

namespace MapAC.DatLoader.Entity.AnimationHooks
{
    public class DefaultScriptPartHook : AnimationHook
    {
        public uint PartIndex { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            base.Unpack(reader);

            PartIndex = reader.ReadUInt32();
        }
    }
}
