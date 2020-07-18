using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class PhysicsScriptData : IUnpackable
    {
        public double StartTime { get; private set; }
        public AnimationHook Hook { get; private set; } = new AnimationHook();

        public void Unpack(BinaryReader reader)
        {
            StartTime = reader.ReadDouble();

            Hook = AnimationHook.ReadHook(reader);
        }
    }
}
