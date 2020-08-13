using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class SkyObject : IUnpackable
    {
        public float BeginTime { get; private set; }
        public float EndTime { get; private set; }
        public float BeginAngle { get; private set; }
        public float EndAngle { get; private set; }
        public float TexVelocityX { get; private set; }
        public float TexVelocityY { get; private set; }
        public float TexVelocityZ { get; } = 0;
        public uint DefaultGFXObjectId { get; private set; }
        public uint DefaultPESObjectId { get; private set; }
        
        // This field was added at some point after Throne of Destiny.
        public uint Properties { get; private set; }

        public void Unpack(BinaryReader reader)
        {
            BeginTime           = reader.ReadSingle();
            EndTime             = reader.ReadSingle();
            BeginAngle          = reader.ReadSingle();
            EndAngle            = reader.ReadSingle();
            TexVelocityX        = reader.ReadSingle();
            TexVelocityY        = reader.ReadSingle();
            DefaultGFXObjectId  = reader.ReadUInt32();
            DefaultPESObjectId  = reader.ReadUInt32();
            if(DatManager.DatVersion != DatVersionType.ACDM)
                Properties          = reader.ReadUInt32();

            reader.AlignBoundary();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(BeginTime);
            writer.Write(EndTime);
            writer.Write(BeginAngle);
            writer.Write(EndAngle);
            writer.Write(TexVelocityX);
            writer.Write(TexVelocityY);
            writer.Write(DefaultGFXObjectId);
            writer.Write(DefaultPESObjectId);
            writer.Write(Properties);
            writer.AlignBoundary();
        }

    }
}
