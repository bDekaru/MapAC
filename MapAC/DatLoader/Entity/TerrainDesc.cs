using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class TerrainDesc : IUnpackable
    {
        public List<TerrainType> TerrainTypes { get; } = new List<TerrainType>();
        public LandSurf LandSurfaces { get; } = new LandSurf();

        public void Unpack(BinaryReader reader)
        {
            TerrainTypes.Unpack(reader);
            LandSurfaces.Unpack(reader);
        }

        public void Pack(BinaryWriter writer)
        {
            TerrainTypes.Pack(writer);
            LandSurfaces.Pack(writer);
        }

    }
}
