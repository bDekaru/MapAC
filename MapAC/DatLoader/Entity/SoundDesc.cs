using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class SoundDesc : IUnpackable
    {
        public List<AmbientSTBDesc> STBDesc { get; } = new List<AmbientSTBDesc>();

        public void Unpack(BinaryReader reader)
        {
            STBDesc.Unpack(reader);
        }
    }
}
