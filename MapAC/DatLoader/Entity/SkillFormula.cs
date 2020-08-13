using System.IO;
using MapAC.DatLoader.Enum.Properties;

namespace MapAC.DatLoader.Entity
{
    public class SkillFormula : IUnpackable
    {
        public uint W;
        public uint X;
        public uint Y;
        public uint Z;
        public uint Attr1;
        public uint Attr2;

        public void Unpack(BinaryReader reader)
        {
            W = reader.ReadUInt32();
            X = reader.ReadUInt32();
            Y = reader.ReadUInt32();
            Z = reader.ReadUInt32();

            Attr1 = reader.ReadUInt32();
            Attr2 = reader.ReadUInt32();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(W);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            
            writer.Write(Attr1);
            writer.Write(Attr1);
        }

        public SkillFormula() { }

        public SkillFormula(PropertyAttribute attr1, PropertyAttribute attr2, uint divisor)
        {
            Attr1 = (uint)attr1;
            Attr2 = (uint)attr2;
            Z = divisor;
            X = 1;
        }
    }
}
