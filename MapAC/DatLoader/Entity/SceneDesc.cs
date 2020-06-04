using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class SceneDesc : IUnpackable
    {
        public List<SceneType> SceneTypes { get; } = new List<SceneType>();

        public void Unpack(BinaryReader reader)
        {
            SceneTypes.Unpack(reader);
        }
    }
}
