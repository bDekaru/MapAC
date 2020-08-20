using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.Entity
{
    public class BSPLeaf : BSPNode
    {
        public int LeafIndex { get; private set; }
        public int Solid { get; private set; }

        /// <summary>
        /// You must use the Unpack(BinaryReader reader, BSPType treeType) method.
        /// </summary>
        /// <exception cref="NotSupportedException">You must use the Unpack(BinaryReader reader, BSPType treeType) method.</exception>
        public override void Unpack(BinaryReader reader)
        {
            throw new NotSupportedException();
        }

        public override void Unpack(BinaryReader reader, BSPType treeType)
        {
            Type = Encoding.ASCII.GetString(reader.ReadBytes(4)).Reverse();

            LeafIndex = reader.ReadInt32();

            if (treeType == BSPType.Physics)
            {
                Solid = reader.ReadInt32();

                // Note that if Solid is equal to 0, these values will basically be null. Still read them, but they don't mean anything.
                Sphere = new Sphere();
                Sphere.Unpack(reader);

                InPolys = new List<ushort>();
                uint numPolys = reader.ReadUInt32();
                for (uint i = 0; i < numPolys; i++)
                    InPolys.Add(reader.ReadUInt16());
            }

            if (DatManager.DatVersion == DatVersionType.ACDM)
                reader.AlignBoundary();
        }

        public override void Pack(BinaryWriter writer, BSPType treeType)
        {
            byte[] typeBytes = Encoding.ASCII.GetBytes(Type);
            for (var i = typeBytes.Length - 1; i >= 0; i--)
                writer.Write(typeBytes[i]);

            writer.Write(LeafIndex);

            if (treeType == BSPType.Physics)
            {
                writer.Write(Solid);
                
                Sphere.Pack(writer);

                writer.Write(InPolys.Count);
                foreach (var e in InPolys)
                    writer.Write(e);
            }
        }
    }
}
