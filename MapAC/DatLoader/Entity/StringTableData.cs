using System;
using System.IO;
using System.Collections.Generic;

namespace MapAC.DatLoader.Entity
{
    public class StringTableData : IUnpackable
    {
        public uint Id { get; private set; }
        public List<string> VarNames { get; } = new List<string>();
        public List<string> Vars { get; } = new List<string>();
        public List<string> Strings { get; } = new List<string>();
        public List<uint> Comments { get; } = new List<uint>();

        public byte Unknown { get; private set; }

        public void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            var num_varnames = reader.ReadUInt16();
            for (uint i = 0; i < num_varnames; i++)
                VarNames.Add(reader.ReadUnicodeString());

            var num_vars = reader.ReadUInt16();
            for (uint i = 0; i < num_vars; i++)
                Vars.Add(reader.ReadUnicodeString());

            var num_strings = reader.ReadUInt32();
            for (uint i = 0; i < num_strings; i++)
                Strings.Add(reader.ReadUnicodeString());

            var num_comments = reader.ReadUInt32();
            for (uint i = 0; i < num_comments; i++)
                Comments.Add(reader.ReadUInt32());

            Unknown = reader.ReadByte();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(Id);

            writer.Write((ushort)VarNames.Count);
            for (int i = 0; i < VarNames.Count; i++)
                writer.WriteUnicodeString(VarNames[i]);

            writer.Write((ushort)Vars.Count);
            for (int i = 0; i < Vars.Count; i++)
                writer.WriteUnicodeString(Vars[i]);

            writer.Write((ushort)Strings.Count);
            for (int i = 0; i < Strings.Count; i++)
                writer.WriteUnicodeString(Strings[i]);

            writer.Write((ushort)Comments.Count);
            for (int i = 0; i < Comments.Count; i++)
                writer.Write(Comments[i]);

            writer.Write(Unknown);
        }
    }
}
