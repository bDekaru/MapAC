using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.Entity
{
    public class CellStruct : IUnpackable
    {
        public CVertexArray VertexArray { get; } = new CVertexArray();
        public Dictionary<ushort, Polygon> Polygons { get; } = new Dictionary<ushort, Polygon>();
        public List<ushort> Portals { get; } = new List<ushort>();
        public BSPTree CellBSP { get; } = new BSPTree();
        public Dictionary<ushort, Polygon> PhysicsPolygons { get; } = new Dictionary<ushort, Polygon>();
        public BSPTree PhysicsBSP { get; } = new BSPTree();
        public BSPTree DrawingBSP { get; set; }

        public void Unpack(BinaryReader reader)
        {
            var numPolygons        = reader.ReadUInt32();
            var numPhysicsPolygons = reader.ReadUInt32();
            var numPortals         = reader.ReadUInt32();

            VertexArray.Unpack(reader);

            Polygons.Unpack(reader, numPolygons);

            for (uint i = 0; i < numPortals; i++)
                Portals.Add(reader.ReadUInt16());

            reader.AlignBoundary();

            CellBSP.Unpack(reader, BSPType.Cell);

            PhysicsPolygons.Unpack(reader, numPhysicsPolygons);

            PhysicsBSP.Unpack(reader, BSPType.Physics);

            uint hasDrawingBSP = reader.ReadUInt32();
            if (hasDrawingBSP != 0)
            {
                DrawingBSP = new BSPTree();
                DrawingBSP.Unpack(reader, BSPType.Drawing);
            }

            reader.AlignBoundary();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(Polygons.Count);
            writer.Write(PhysicsPolygons.Count);
            writer.Write(Portals.Count);

            VertexArray.Pack(writer);

            Polygons.Pack(writer);

            for (int i = 0; i < Portals.Count; i++)
                writer.Write(Portals[i]);

            writer.AlignBoundary();

            CellBSP.Pack(writer, BSPType.Cell);

            PhysicsPolygons.Pack(writer);

            PhysicsBSP.Pack(writer, BSPType.Physics);

            if(DrawingBSP == null)
                writer.Write(0);
            else
            {
                writer.Write(1);
                DrawingBSP.Pack(writer, BSPType.Drawing);
            }

            writer.AlignBoundary();
        }

    }
}
