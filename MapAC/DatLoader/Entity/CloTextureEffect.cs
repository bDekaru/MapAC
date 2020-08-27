using MapAC.DatLoader.Enum;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class CloTextureEffect : IUnpackable
    {
        /// <summary>
        /// Texture portal.dat 0x05000000
        /// </summary>
        public uint OldTexture { get; private set; }

        /// <summary>
        /// Texture portal.dat 0x05000000
        /// </summary>
        public uint NewTexture { get; private set; }

        public void Unpack(BinaryReader reader)
        {
            OldTexture = reader.ReadUInt32();
            NewTexture = reader.ReadUInt32();
        }

        public void Pack(BinaryWriter writer)
        {
            if(DatManager.DatVersion == DatVersionType.ACDM)
            {
                writer.Write(OldTexture + (uint)ACDMOffset.Texture);
                writer.Write(NewTexture + (uint)ACDMOffset.Texture);
            }
            else
            {
                writer.Write(OldTexture);
                writer.Write(NewTexture);
            }
        }

    }
}
