using MapAC.DatLoader.Enum;
using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x04. 
    /// </summary>
    [DatFileType(DatFileType.Palette)]
    public class Palette : FileType
    {
        /// <summary>
        /// Color data is stored in ARGB format
        /// </summary>
        public List<uint> Colors { get; private set; } = new List<uint>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            Colors.Unpack(reader);
        }

        /// <summary>
        /// Updates color palette to 2048 bytes as TOD uses, so we can load properly generate texture bitmaps
        /// </summary>
        public void ConvertColorsToTOD()
        {
            if (DatManager.DatVersion == DatVersionType.ACDM) 
            { 
                if (Colors.Count == 256)
                {
                    List<uint> temp = new List<uint>();
                    for (int i = 0; i < Colors.Count; i++)
                        for (int j = 0; j < 8; j++) // Write this 8 times!
                            temp.Add(Colors[i]);

                    Colors = temp;
                }
            }
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.WriteOffset(Id, ACDMOffset.Palette);
            if (DatManager.DatVersion == DatVersionType.ACDM)
            {
                if (Colors.Count == 256)
                {
                    writer.Write(2048); // Size of Colors
                    for (int i = 0; i < Colors.Count; i++)
                        for(int j = 0; j < 8; j++) // Write this 8 times!
                            writer.Write(Colors[i]);
                }
                else
                    Colors.Pack(writer);
            }
            else
            {
                Colors.Pack(writer);
            }
        }
    }
}
