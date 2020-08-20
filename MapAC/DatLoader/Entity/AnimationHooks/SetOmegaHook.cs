using System.IO;
using System.Numerics;

namespace MapAC.DatLoader.Entity.AnimationHooks
{
    public class SetOmegaHook : AnimationHook
    {
        public Vector3 Axis { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            base.Unpack(reader);

            Axis = reader.ReadVector3();
        }

        public override void Pack(BinaryWriter writer)
        {
            base.Pack(writer);
            writer.Write(Axis);
        }
    }
}
