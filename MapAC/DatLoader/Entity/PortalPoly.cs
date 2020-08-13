using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class PortalPoly : IUnpackable
    {
        public short PortalIndex { get; set; }
        public short PolygonId { get; set; }

        public void Unpack(BinaryReader reader)
        {
            PortalIndex = reader.ReadInt16();
            PolygonId   = reader.ReadInt16();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(PortalIndex);
            writer.Write(PolygonId);
        }

    }
}
