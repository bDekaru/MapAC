using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader
{
    public class DatDirectoryHeader : IUnpackable
    {
        //internal static readonly uint ObjectSize = ((sizeof(uint) * 0x3E) + sizeof(uint) + (DatFile.ObjectSize * 0x3D));
        //public uint[] Branches { get; } = new uint[0x3E];

        //internal static readonly uint ObjectSize = ((sizeof(uint) * 0x25) + sizeof(uint) + (DatFile.ObjectSize * 0x24)); // 3F8
        internal static readonly uint ObjectSize = 0x400; // 1000 bytes
        public uint[] Branches { get; } = new uint[0x25];

        public uint EntryCount { get; private set; }
        public List<DatFile> Entries { get; private set; }

        public uint BlockSize;

        public void Unpack(BinaryReader reader)
        {
            for (int i = 0; i < Branches.Length; i++)
            {
                Branches[i] = reader.ReadUInt32();
                if(Branches[i] == 0x03dc9000)
                {
                    var test = "GOOD";
                }
            }

            EntryCount = reader.ReadUInt32();

            //Entries = new DatFile[EntryCount];
            Entries = new List<DatFile>();

            for (int i = 0; i < EntryCount; i++)
            {
                var entry = new DatFile();
                entry.Unpack(reader);
                //Entries[idx] = new DatFile();
                //Entries[idx].Unpack(reader);
                Entries.Add(entry);
                /*
                if (entry.PrevFile > 0)
                {
                    var prevEntry = new DatFile();
                    prevEntry.ObjectId = entry.PrevFile;
                    prevEntry.FileOffset = entry.FileOffset - BlockSize;
                    prevEntry.FileSize = entry.FileSize;
                    Entries.Add(prevEntry);
                }
                if (entry.NextFile > 0)
                {
                    var nextEntry = new DatFile();
                    nextEntry.ObjectId = entry.NextFile;
                    nextEntry.FileOffset = entry.FileOffset + BlockSize;
                    nextEntry.FileSize = entry.FileSize;
                    Entries.Add(nextEntry);
                }
                */

            }
        }

        public void Pack(BinaryWriter writer)
        {
            throw new System.NotSupportedException();
        }
    }
}
