using System;
using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Entity;

namespace MapAC.DatLoader.FileTypes
{
    [Flags]
    public enum EnvCellFlags
    {
        SeenOutside = 0x1,
        HasStaticObjs = 0x2,
        HasRestrictionObj = 0x8
    };

    /// <summary>
    /// This reads an "indoor" cell from the client_cell.dat. This is mostly dungeons, but can also be a building interior.
    /// An EnvCell is designated by starting 0x0100 (whereas all landcells are in the 0x0001 - 0x003E range.
    /// <para />
    /// The fileId is the full int32/dword landblock value as reported by the @loc command (e.g. 0x12345678)
    /// </summary>
    /// <remarks>
    /// Very special thanks again to David Simpson for his early work on reading the cell.dat. Even bigger thanks for his documentation of it!
    /// </remarks>
    [DatFileType(DatFileType.EnvCell)]
    public class EnvCell : FileType
    {
        public EnvCellFlags Flags { get; private set; }
        // 0x08000000 surfaces (which contains degrade/quality info to reference the specific 0x06000000 graphics)
        public List<uint> Surfaces { get; } = new List<uint>();
        // the 0x0D000000 model of the pre-fab dungeon block
        public uint EnvironmentId { get; private set; }
        public ushort CellStructure { get; private set; }
        public Frame Position { get; } = new Frame();
        public List<CellPortal> CellPortals { get; } = new List<CellPortal>();
        public List<ushort> VisibleCells { get; } = new List<ushort>();
        public List<Stab> StaticObjects { get; } = new List<Stab>();
        public uint RestrictionObj { get; private set; }

        public bool SeenOutside => Flags.HasFlag(EnvCellFlags.SeenOutside);

        public override void Unpack(BinaryReader reader)
        {
            if (DatManager.DatVersion == DatVersionType.ACTOD) 
                Id = reader.ReadUInt32();

            Flags = (EnvCellFlags)reader.ReadUInt32();

            if (DatManager.DatVersion == DatVersionType.ACDM)
                Id = reader.ReadUInt32(); 
            else
                reader.BaseStream.Position += 4; // Skip ahead 4 bytes, because this is the CellId. Again. Twice.

            byte numSurfaces    = reader.ReadByte();
            byte numPortals     = reader.ReadByte();    // Note that "portal" in this context does not refer to the swirly pink/purple thing, its basically connecting cells
            ushort numStabs     = reader.ReadUInt16();  // I believe this is what cells can be seen from this one. So the engine knows what else it needs to load/draw.

            // Read what surfaces are used in this cell
            for (uint i = 0; i < numSurfaces; i++)
                Surfaces.Add(0x08000000u | reader.ReadUInt16()); // these are stored in the dat as short values, so we'll make them a full dword

            if (DatManager.DatVersion == DatVersionType.ACDM)
                reader.AlignBoundary();

            EnvironmentId = (0x0D000000u | reader.ReadUInt16());

            CellStructure = reader.ReadUInt16();

            Position.Unpack(reader);

            CellPortals.Unpack(reader, numPortals);

            if (DatManager.DatVersion == DatVersionType.ACDM)
                reader.AlignBoundary();

            for (uint i = 0; i < numStabs; i++)
                VisibleCells.Add(reader.ReadUInt16());

            if (DatManager.DatVersion == DatVersionType.ACDM)
                reader.AlignBoundary();

            if ((Flags & EnvCellFlags.HasStaticObjs) != 0)
                StaticObjects.Unpack(reader);

            if (DatManager.DatVersion == DatVersionType.ACDM)
                reader.AlignBoundary();

            if ((Flags & EnvCellFlags.HasRestrictionObj) != 0)
                RestrictionObj = reader.ReadUInt32();
        }

        public byte[] Pack()
        {
            using(MemoryStream Data = new System.IO.MemoryStream()){
                using (BinaryWriter Writer = new System.IO.BinaryWriter(Data))
                {
                    Writer.Write(Id);
                    Writer.Write((uint)Flags);
                    Writer.Write(Id);

                    Writer.Write((byte)Surfaces.Count);
                    Writer.Write((byte)CellPortals.Count);
                    Writer.Write((ushort)VisibleCells.Count);

                    for (int i = 0; i < Surfaces.Count; i++)
                    {
                        ushort surfaceID = (ushort)(Surfaces[i] & 0xFFFF);
                        //Writer.Write(surfaceID);
                        Writer.Write((ushort)0x032a);
                    }

                    Writer.Write((ushort)(EnvironmentId & 0xFFFF));
                    Writer.Write(CellStructure);

                    Writer.Write(Position.Origin.X);
                    Writer.Write(Position.Origin.Y);
                    Writer.Write(Position.Origin.Z);
                    Writer.Write(Position.Orientation.W);
                    Writer.Write(Position.Orientation.X);
                    Writer.Write(Position.Orientation.Y);
                    Writer.Write(Position.Orientation.Z);

                    for (int i = 0; i < CellPortals.Count; i++)
                    {
                        Writer.Write((ushort)CellPortals[i].Flags);
                        Writer.Write(CellPortals[i].PolygonId);
                        Writer.Write(CellPortals[i].OtherCellId);
                        Writer.Write(CellPortals[i].OtherPortalId);
                    }

                    for (int i = 0; i < VisibleCells.Count; i++)
                        Writer.Write(VisibleCells[i]);

                    Writer.Write(StaticObjects.Count);
                    for (int i = 0; i < StaticObjects.Count; i++)
                    {
                        Writer.Write(StaticObjects[i].Id);
                        Writer.Write(StaticObjects[i].Frame.Origin.X);
                        Writer.Write(StaticObjects[i].Frame.Origin.Y);
                        Writer.Write(StaticObjects[i].Frame.Origin.Z);
                        Writer.Write(StaticObjects[i].Frame.Orientation.W);
                        Writer.Write(StaticObjects[i].Frame.Orientation.X);
                        Writer.Write(StaticObjects[i].Frame.Orientation.Y);
                        Writer.Write(StaticObjects[i].Frame.Orientation.Z);
                    }

                    if ((Flags & EnvCellFlags.HasRestrictionObj) != 0)
                        Writer.Write(RestrictionObj);
                }
                return Data.ToArray();
            }
        }
    }
}
