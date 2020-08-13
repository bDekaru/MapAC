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

        public void Pack(BinaryWriter writer)
        {
            writer.Write(StartTime);
            Hook.Pack(writer);
        }

    }
}
