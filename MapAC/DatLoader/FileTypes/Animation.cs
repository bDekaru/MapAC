using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Entity;
using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x03. 
    /// Special thanks to Dan Skorupski for his work on Bael'Zharon's Respite, which helped fill in some of the gaps https://github.com/boardwalk/bzr
    /// </summary>
    [DatFileType(DatFileType.Animation)]
    public class Animation : FileType
    {
        public AnimationFlags Flags { get; private set; }
        public uint NumParts { get; private set; }
        public uint NumFrames { get; private set; }
        public List<Frame> PosFrames { get; } = new List<Frame>();
        public List<AnimationFrame> PartFrames { get; } = new List<AnimationFrame>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();
            Flags = (AnimationFlags)reader.ReadUInt32();
            NumParts = reader.ReadUInt32();
            NumFrames = reader.ReadUInt32();

            if ((Flags & AnimationFlags.PosFrames) != 0)
                PosFrames.Unpack(reader, NumFrames);

            for (uint i = 0; i < NumFrames; i++)
            {
                var animationFrame = new AnimationFrame();
                animationFrame.Unpack(reader, NumParts);
                PartFrames.Add(animationFrame);
            }
        }
        public override void Pack(BinaryWriter writer)
        {
            if (DatManager.DatVersion == DatVersionType.ACDM)
                writer.Write(Id + DatManager.ACDM_OFFSET);
            else
                writer.Write(Id);

            writer.Write((uint)Flags);
            writer.Write(NumParts);
            writer.Write(NumFrames);

            if ((Flags & AnimationFlags.PosFrames) != 0)
            {
                foreach (var e in PosFrames)
                    e.Pack(writer);
            }

            for (int i = 0; i < PartFrames.Count; i++)
                PartFrames[i].Pack(writer);
        }
    }
}
