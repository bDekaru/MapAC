using System;
using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Entity;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// This reads the extra items in a landblock from the client_cell.dat. This is mostly buildings, but other static/non-interactive objects like tables, lamps, are also included.
    /// CLandBlockInfo is a file designated xxyyFFFE, where xxyy is the landblock.
    /// <para />
    /// The fileId is CELL + 0xFFFE. e.g. a cell of 1234, the file index would be 0x1234FFFE.
    /// </summary>
    /// <remarks>
    /// Very special thanks again to David Simpson for his early work on reading the cell.dat. Even bigger thanks for his documentation of it!
    /// </remarks>
    [DatFileType(DatFileType.LandBlockInfo)]
    public class LandblockInfo : FileType
    {
        /// <summary>
        /// number of EnvCells in the landblock. This should match up to the unique items in the building stab lists.
        /// </summary>
        public uint NumCells { get; private set; }

        /// <summary>
        /// list of model numbers. 0x01 and 0x02 types and their specific locations
        /// </summary>
        public List<Stab> Objects { get; } = new List<Stab>();

        /// <summary>
        /// As best as I can tell, this only affects whether there is a restriction table or not
        /// </summary>
        public uint PackMask { get; private set; }

        /// <summary>
        /// Buildings and other structures with interior locations in the landblock
        /// </summary>
        public List<BuildInfo> Buildings { get; } = new List<BuildInfo>();

        /// <summary>
        /// The specific landblock/cell controlled by a specific guid that controls access (e.g. housing barrier)
        /// </summary>
        public Dictionary<uint, uint> RestrictionTables { get; } = new Dictionary<uint, uint>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            NumCells = reader.ReadUInt32();

            Objects.Unpack(reader);

            ushort numBuildings = reader.ReadUInt16();

            PackMask = reader.ReadUInt16();

            Buildings.Unpack(reader, numBuildings);

            if (DatManager.DatVersion == DatVersionType.ACDM)
                reader.AlignBoundary();

            if ((PackMask & 1) == 1)
                RestrictionTables.UnpackPackedHashTable(reader);

            if (DatManager.DatVersion == DatVersionType.ACDM)
                reader.AlignBoundary();
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.Write(Id);

            writer.Write(NumCells);

            writer.Write(Objects.Count);
            foreach (var o in Objects)
            {
                if (DatManager.DatVersion == DatVersionType.ACDM)
                    o.Id += DatManager.ACDM_OFFSET;
                o.Pack(writer);
            }

            Objects.Pack(writer);

            writer.Write((ushort)Buildings.Count);
            writer.Write((ushort)PackMask);

            //Buildings.Pack(writer); // Can't use this, we already wrote the count
            foreach (var e in Buildings)
            {
                if (DatManager.DatVersion == DatVersionType.ACDM)
                    e.ModelId += DatManager.ACDM_OFFSET;
                e.Pack(writer);
            }

            if ((PackMask & 1) == 1)
                RestrictionTables.PackHashTable(writer, 0x08);
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
            uint newLandblockId = (uint)((blockX << 24) + (blockY << 16) + 0xFFFE);
            Id = newLandblockId;

            // We might need to change our landblock.Buildings.Portals other_cell_id, other_portal_Id and StabList

        }

    }
}
