using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Entity;

namespace MapAC.DatLoader.FileTypes
{
    [DatFileType(DatFileType.CharacterGenerator)]
    public class CharGen : FileType
    {
        internal const uint FILE_ID = 0x0E000002;

        public List<StarterArea> StarterAreas { get; } = new List<StarterArea>();
        public Dictionary<uint, HeritageGroupCG> HeritageGroups { get; } = new Dictionary<uint, HeritageGroupCG>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();
            reader.BaseStream.Position += 4;

            StarterAreas.UnpackSmartArray(reader);

            // HERITAGE GROUPS -- 11 standard player races and 2 Olthoi.
            reader.BaseStream.Position++; // Not sure what this byte 0x01 is indicating, but we'll skip it because we can.

            HeritageGroups.UnpackSmartArray(reader);
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Id); // this is in there twice

            StarterAreas.PackSmartArray(writer);

            writer.Write((byte)1); // Some seemingly random byte. Possibly a bool for "has heritage groups" or something?

            // HERITAGE GROUPS -- 11 standard player races and 2 Olthoi.
            HeritageGroups.PackSmartArray(writer);
        }

    }
}