using System;
using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    /// <summary>
    /// A list of indexed vertices, and their associated type
    /// </summary>
    public class CVertexArray : IUnpackable
    {
        public int VertexType { get; private set; }
        public Dictionary<ushort, SWVertex> Vertices { get; } = new Dictionary<ushort, SWVertex>();

        public void Unpack(BinaryReader reader)
        {
            VertexType = reader.ReadInt32();

            var numVertices = reader.ReadUInt32();

            if (VertexType == 1)
                Vertices.Unpack(reader, numVertices);
            else
                throw new NotImplementedException();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(VertexType);
            writer.Write((uint)Vertices.Count);

            if (VertexType == 1)
                Vertices.Pack(writer);
            else
                throw new NotImplementedException();
        }

    }
}
