using MapAC.DatLoader.Enum;
using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class CloObjectEffect : IUnpackable
    {
        public uint Index { get; private set; }
        public uint ModelId { get; private set; }
        public List<CloTextureEffect> CloTextureEffects { get; } = new List<CloTextureEffect>();

        public void Unpack(BinaryReader reader)
        {
            Index   = reader.ReadUInt32();
            ModelId = reader.ReadUInt32();

            CloTextureEffects.Unpack(reader);
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.WriteOffset(ModelId, ACDMOffset.GfxObj);

            CloTextureEffects.Pack(writer);
        }

    }
}
