using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using MapAC.DatLoader.Entity;
using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.FileTypes
{
    [DatFileType(DatFileType.MotionTable)]
    public class MotionTable : FileType
    {
        public uint DefaultStyle { get; private set; }
        public Dictionary<uint, uint> StyleDefaults { get; } = new Dictionary<uint, uint>();
        public Dictionary<uint, MotionData> Cycles { get; } = new Dictionary<uint, MotionData>();
        public Dictionary<uint, MotionData> Modifiers { get; } = new Dictionary<uint, MotionData>();
        public Dictionary<uint, Dictionary<uint, MotionData>> Links { get; } = new Dictionary<uint, Dictionary<uint, MotionData>>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            DefaultStyle = reader.ReadUInt32();

            uint numStyleDefaults = reader.ReadUInt32();
            for (uint i = 0; i < numStyleDefaults; i++)
                StyleDefaults.Add(reader.ReadUInt32(), reader.ReadUInt32());

            Cycles.Unpack(reader);

            Modifiers.Unpack(reader);

            Links.Unpack(reader);
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(DefaultStyle);
            
            writer.Write(StyleDefaults.Count);
            foreach(var e in StyleDefaults)
            {
                writer.Write(e.Key);
                writer.Write(e.Value);
            }

            Cycles.Pack(writer);
            Modifiers.Pack(writer);
            Links.Pack(writer);
        }
    }
}
