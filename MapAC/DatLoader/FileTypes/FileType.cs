using System.IO;

namespace MapAC.DatLoader.FileTypes
{
    public abstract class FileType : IUnpackable
    {
        public uint Id { get; protected set; }

        public abstract void Unpack(BinaryReader reader);
    }
}
