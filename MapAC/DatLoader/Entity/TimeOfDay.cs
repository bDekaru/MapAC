using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class TimeOfDay : IUnpackable
    {
        public float Start { get; private set; }
        public bool IsNight { get; private set; }
        public string Name { get; private set; }

        public void Unpack(BinaryReader reader)
        {
            Start   = reader.ReadSingle();
            IsNight = (reader.ReadUInt32() == 1);
            Name    = reader.ReadPString();
            reader.AlignBoundary();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(Start);
            if (IsNight)
                writer.Write(1);
            else
                writer.Write(0);
            writer.WritePString(Name); writer.AlignBoundary();
        }

    }
}
