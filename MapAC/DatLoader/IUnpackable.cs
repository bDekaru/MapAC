using System.IO;

namespace MapAC.DatLoader
{
    public interface IUnpackable
    {
        void Unpack(BinaryReader reader);
        void Pack(BinaryWriter writer);
    }
}
