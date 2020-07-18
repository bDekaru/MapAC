using System.IO;

namespace MapAC.DatLoader.Entity.AnimationHooks
{
    public class SetLightHook : AnimationHook
    {
        public int LightsOn { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            base.Unpack(reader);

            LightsOn = reader.ReadInt32();
        }
    }
}
