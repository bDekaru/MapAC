using System.Collections.Generic;
using System.IO;
using System.Numerics;
using HtmlAgilityPack;
using MapAC.DatLoader.Entity;
using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x01. 
    /// These are used both on their own for some pre-populated structures in the world (trees, buildings, etc) or make up SetupModel (0x02) objects.
    /// </summary>
    [DatFileType(DatFileType.GraphicsObject)]
    public class GfxObj : FileType
    {
        public GfxObjFlags Flags { get; private set; }
        public List<uint> Surfaces { get; } = new List<uint>(); // also referred to as m_rgSurfaces in the client
        public CVertexArray VertexArray { get; } = new CVertexArray();

        public Dictionary<ushort, Polygon> PhysicsPolygons { get; } = new Dictionary<ushort, Polygon>();
        public BSPTree PhysicsBSP { get; } = new BSPTree();

        public Vector3 SortCenter { get; private set; }

        public Dictionary<ushort, Polygon> Polygons { get; } = new Dictionary<ushort, Polygon>();
        public BSPTree DrawingBSP { get; } = new BSPTree();

        public uint DIDDegrade { get; private set; }

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            Flags = (GfxObjFlags)reader.ReadUInt32();

            Surfaces.UnpackSmartArray(reader);

            VertexArray.Unpack(reader);

            // Has Physics 
            if ((Flags & GfxObjFlags.HasPhysics) != 0)
            {
                PhysicsPolygons.UnpackSmartArray(reader);

                PhysicsBSP.Unpack(reader, BSPType.Physics);
            }

            SortCenter = reader.ReadVector3();

            // Has Drawing 
            if ((Flags & GfxObjFlags.HasDrawing) != 0)
            {
                Polygons.UnpackSmartArray(reader);

                DrawingBSP.Unpack(reader, BSPType.Drawing);
            }

            if ((Flags & GfxObjFlags.HasDIDDegrade) != 0)
                DIDDegrade = reader.ReadUInt32();
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.WriteOffset(Id, ACDMOffset.GfxObj);
            writer.Write((uint)Flags);

            if(DatManager.DatVersion == DatVersionType.ACDM)
            {
                // We need to adjust the IDs of these to unique TOD values
                List<uint> adjustedSurfaces = new List<uint>();
                for (var i = 0; i < Surfaces.Count; i++)
                {
                    if (Export.IsSurfaceAddition(Surfaces[i], out var id))
                    {
                        if(DatManager.CellDat.ExistsInEoR(Surfaces[i]))
                            adjustedSurfaces.Add(Surfaces[i] + (uint)ACDMOffset.Surface);
                        else
                            adjustedSurfaces.Add(Surfaces[i]);
                    }
                    else
                        adjustedSurfaces.Add(id);
                }

                adjustedSurfaces.PackSmartArray(writer);
            }
            else
            {
                Surfaces.PackSmartArray(writer);
            }
            VertexArray.Pack(writer);

            // Has Physics 
            if ((Flags & GfxObjFlags.HasPhysics) != 0)
            {
                PhysicsPolygons.PackSmartArray(writer);

                PhysicsBSP.Pack(writer, BSPType.Physics);
            }

            writer.WriteVector3(SortCenter);

            // Has Drawing 
            if ((Flags & GfxObjFlags.HasDrawing) != 0)
            {
                Polygons.PackSmartArray(writer);

                DrawingBSP.Pack(writer, BSPType.Drawing);
            }

            if ((Flags & GfxObjFlags.HasDIDDegrade) != 0)
                if(Export.IsAddition(DIDDegrade) && DatManager.CellDat.ExistsInEoR(DIDDegrade))
                    writer.Write(DIDDegrade + (uint)ACDMOffset.DIDDegrade);
                else
                    writer.Write(DIDDegrade);
        }

    }
}
