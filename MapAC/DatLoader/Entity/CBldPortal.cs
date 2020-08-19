using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public enum PortalFlags
    {
        ExactMatch = 0x1,
        PortalSide = 0x2
    }

    public class CBldPortal : IUnpackable
    {
        public PortalFlags Flags { get; private set; }

        // Not sure what these do. They are both calculated from the flags.
        public bool ExactMatch => Flags.HasFlag(PortalFlags.ExactMatch);
        public bool PortalSide => Flags.HasFlag(PortalFlags.PortalSide);

        // Basically the cells that connect both sides of the portal
        public ushort OtherCellId { get; private set; }
        public ushort OtherPortalId { get; private set; }

        /// <summary>
        /// List of cells used in this structure. (Or possibly just those visible through it.)
        /// </summary>
        public List<ushort> StabList { get; } = new List<ushort>();

        public void Unpack(BinaryReader reader)
        {
            Flags = (PortalFlags)reader.ReadUInt16();

            OtherCellId = reader.ReadUInt16();
            OtherPortalId = reader.ReadUInt16();

            ushort num_stabs = reader.ReadUInt16();
            for (var i = 0; i < num_stabs; i++)
                StabList.Add(reader.ReadUInt16());

            reader.AlignBoundary();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write((ushort)Flags);

            writer.Write(OtherCellId);
            writer.Write(OtherPortalId);

            writer.Write((ushort)StabList.Count);
            for (var i = 0; i < StabList.Count; i++)
                writer.Write((ushort)StabList[i]);

            writer.AlignBoundary();
        }

    }
}
