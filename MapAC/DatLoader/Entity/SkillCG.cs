using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class SkillCG : IUnpackable
    {
        public uint SkillNum { get; private set; }
        public int NormalCost { get; private set; }
        public int PrimaryCost { get; private set; }

        public void Unpack(BinaryReader reader)
        {
            SkillNum    = reader.ReadUInt32();
            NormalCost  = reader.ReadInt32();
            PrimaryCost = reader.ReadInt32();
        }
    }
}
