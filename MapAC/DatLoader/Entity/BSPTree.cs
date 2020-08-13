using System;
using System.IO;

using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.Entity
{
    public class BSPTree : IUnpackable
    {
        public BSPNode RootNode { get; private set; } = new BSPNode();

        /// <summary>
        /// You must use the Unpack(BinaryReader reader, BSPType treeType) method.
        /// </summary>
        /// <exception cref="NotSupportedException">You must use the Unpack(BinaryReader reader, BSPType treeType) method.</exception>
        public void Unpack(BinaryReader reader)
        {
            throw new NotSupportedException();
        }

        public void Unpack(BinaryReader reader, BSPType treeType)
        {
            RootNode = BSPNode.ReadNode(reader, treeType);
        }

        /// <summary>
        /// You must use the Pack(BinaryWriter writer, BSPType treeType) method.
        /// </summary>
        /// <exception cref="NotSupportedException">You must use the Pack(BinaryWriter writer, BSPType treeType) method.</exception>
        public void Pack(BinaryWriter writer)
        {
            throw new NotSupportedException();
        }

        public void Pack(BinaryWriter writer, BSPType treeType)
        {
            RootNode.WriteNode(writer, treeType);
            //RootNode = BSPNode.ReadNode(reader, treeType);
        }

    }
}
