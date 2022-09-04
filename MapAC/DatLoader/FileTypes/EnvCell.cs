using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using MapAC.DatLoader.Entity;
using MapAC.DatLoader.Enum;

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

        public uint OrigId { get; set; }

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

        public override void Pack(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write((uint)Flags);
            writer.Write(Id); // Yes, twice...

            writer.Write((byte)Surfaces.Count);
            writer.Write((byte)CellPortals.Count); ;
            writer.Write((ushort)VisibleCells.Count);

            for (var i = 0; i < Surfaces.Count; i++)
                if (Export.IsSurfaceAddition(Surfaces[i], out var id))
                {
                    uint val = (Surfaces[i] + (uint)ACDMOffset.Surface);
                    writer.Write((ushort)(val & 0xFFFF));
                }
                else
                    writer.Write((ushort)(id & 0xFFFF));

            if (Export.IsAddition(EnvironmentId))
                writer.Write((ushort)((EnvironmentId + (uint)ACDMOffset.Environment) & 0xFFFF));
            else
                writer.Write((ushort)(EnvironmentId & 0xFFFF));

            writer.Write(CellStructure);

            Position.Pack(writer);

            // We already wrote our count...
            foreach (var e in CellPortals)
                e.Pack(writer);

            for (var i = 0; i < VisibleCells.Count; i++)
                writer.Write(VisibleCells[i]);

            if ((Flags & EnvCellFlags.HasStaticObjs) != 0)
            {
                if (DatManager.DatVersion == DatVersionType.ACDM)
                {
                    // Rewrite some of the objIds
                    writer.Write(StaticObjects.Count);
                    for (int i = 0; i < StaticObjects.Count; i++)
                    {
                        Stab thisStab = StaticObjects[i];
                        if (!DatManager.CellDat.IsSameAsEoRDatFile(thisStab.Id))
                        {
                            if (thisStab.Id <= 0x01FFFFFF)
                                thisStab.Id += (uint)ACDMOffset.GfxObj;
                            else
                                thisStab.Id += (uint)ACDMOffset.Setup;
                        }
                        thisStab.Pack(writer);
                    }
                }
                else
                    StaticObjects.Pack(writer);
            }

            if ((Flags & EnvCellFlags.HasRestrictionObj) != 0)
                writer.Write(RestrictionObj);
        }

        // Adjust the data of the Landblock to reflect its new position.
        public void MoveLandblock(int offsetX, int offsetY)
        {
            int blockX = (int)(Id >> 24);
            int blockY = (int)(Id >> 16 & 0xFF);

            // adjust these by our offsets!
            blockX += offsetX;
            blockY += offsetY;

            // little sanity check
            if (blockX > 255 || blockX < 0)
                throw new System.ArgumentOutOfRangeException();
            if (blockY > 255 || blockY < 0)
                throw new System.ArgumentOutOfRangeException();

            // we need to update our Landblock.Id to reflect the new position
            uint newLandblockId = (uint)((blockX << 24) + (blockY << 16) + (Id & 0xFFFF));
            Id = newLandblockId;

            // We might need to change our landblock.Buildings.Portals other_cell_id, other_portal_Id and StabList

        }

    }
}
