using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MapAC.DatLoader
{
    static class UnpackableExtensions
    {
        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray(this List<int> value, BinaryReader reader)
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = reader.ReadInt32();
                value.Add(item);
            }
        }

        public static void PackSmartArray(this List<int> value, BinaryWriter writer)
        {
            writer.Write((uint)value.Count);
            for (int i = 0; i < value.Count; i++)
                writer.Write(value[i]);
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray(this List<uint> value, BinaryReader reader)
        {
            uint totalObjects;
            if (DatManager.DatVersion == DatVersionType.ACDM)
                totalObjects = reader.ReadUInt32();
            else
                totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = reader.ReadUInt32();
                value.Add(item);
            }
        }

        public static void PackSmartArray(this List<uint> value, BinaryWriter writer)
        {
            writer.WriteCompressedUInt32((uint)value.Count);
            for (int i = 0; i < value.Count; i++)
                writer.Write(value[i]);
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray<T>(this List<T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            uint totalObjects;
            if (DatManager.DatVersion == DatVersionType.ACDM)
                totalObjects = reader.ReadUInt32();
            else
                totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = new T();
                item.Unpack(reader);
                value.Add(item);
            }
        }

        public static void PackSmartArray<T>(this List<T> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            writer.WriteCompressedUInt32((uint)value.Count);

            for (int i = 0; i < value.Count; i++)
                value[i].Pack(writer);
        }


        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray<T>(this Dictionary<ushort, T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            uint totalObjects;
            if (DatManager.DatVersion == DatVersionType.ACDM)
                totalObjects = reader.ReadUInt32();
            else
                totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt16();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
            }
        }
        public static void PackSmartArray<T>(this Dictionary<ushort, T> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            writer.WriteCompressedUInt32((uint)value.Count);
            foreach (var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray<T>(this Dictionary<int, T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadInt32();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
            }
        }

        public static void PackSmartArray<T>(this Dictionary<int, T> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            writer.WriteCompressedUInt32((uint)value.Count);

            foreach(var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }

        /// <summary>
        /// A SmartArray uses a Compressed UInt32 for the length.
        /// </summary>
        public static void UnpackSmartArray<T>(this Dictionary<uint, T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadCompressedUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
            }
        }

        public static void PackSmartArray<T>(this Dictionary<uint, T> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            writer.WriteCompressedUInt32((uint)value.Count);

            foreach(var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }


        /// <summary>
        /// A PackedHashTable uses a UInt16 for length, and a UInt16 for bucket size.
        /// We don't need to worry about the bucket size with C#.
        /// </summary>
        public static void UnpackPackedHashTable(this Dictionary<uint, uint> value, BinaryReader reader)
        {
            var totalObjects = reader.ReadUInt16();
            var bucketSize = reader.ReadUInt16();

            for (int i = 0; i < totalObjects; i++)
                value.Add(reader.ReadUInt32(), reader.ReadUInt32());
        }

        public static void PackHashTable(this Dictionary<uint, uint> value, BinaryWriter writer, ushort bucketSize)
        {
            writer.Write((ushort)value.Count);
            writer.Write(bucketSize);
            foreach (var e in value)
            {
                writer.Write(e.Key);
                writer.Write(e.Value);
            }
        }

        /// <summary>
        /// A PackedHashTable uses a UInt16 for length, and a UInt16 for bucket size.
        /// We don't need to worry about the bucket size with C#.
        /// </summary>
        public static void UnpackPackedHashTable<T>(this Dictionary<uint, T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt16();
            var bucketSize = reader.ReadUInt16();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
                if(value.Count > 2)
                {
                    var test = 1;
                }
            }
        }

        public static void PackHashTable<T>(this Dictionary<uint, T> value, BinaryWriter writer, ushort bucketSize) where T : IUnpackable, new()
        {
            writer.Write((ushort)value.Count);
            writer.Write(bucketSize);
            foreach(var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }

        /// <summary>
        /// A PackedHashTable uses a UInt16 for length, and a UInt16 for bucket size.
        /// We don't need to worry about the bucket size with C#.
        /// </summary>
        public static void UnpackPackedHashTable<T>(this SortedDictionary<uint, T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt16();
            var bucketSize = reader.ReadUInt16();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
            }
        }

        public static void PackHashTable<T>(this SortedDictionary<uint, T> value, BinaryWriter writer, ushort bucketSize) where T : IUnpackable, new()
        {
            writer.Write((ushort)value.Count);
            writer.Write(bucketSize);
            foreach (var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }

        /// <summary>
        /// A list that uses a Int32 for the length.
        /// </summary>
        public static void Unpack(this List<uint> value, BinaryReader reader)
        {
            var totalObjects = reader.ReadInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = reader.ReadUInt32();
                value.Add(item);
            }
        }

        public static void Pack(this List<uint> value, BinaryWriter writer)
        {
            writer.Write(value.Count);

            for (int i = 0; i < value.Count; i++)
                writer.Write(value[i]);
        }

        /// <summary>
        /// A list that uses a UInt32 for the length.
        /// </summary>
        public static void Unpack<T>(this List<T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var item = new T();
                item.Unpack(reader);
                value.Add(item);
            }
        }
        public static void Pack<T>(this List<T> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            writer.Write(value.Count);
            for (int i = 0; i < value.Count; i++)
                value[i].Pack(writer);
        }

        public static void Unpack<T>(this List<T> value, BinaryReader reader, uint fixedQuantity) where T : IUnpackable, new()
        {
            for (int i = 0; i < fixedQuantity; i++)
            {
                var item = new T();
                item.Unpack(reader);
                value.Add(item);
            }
        }


        public static void Unpack<T>(this Dictionary<ushort, T> value, BinaryReader reader, uint fixedQuantity) where T : IUnpackable, new()
        {
            for (int i = 0; i < fixedQuantity; i++)
            {
                var key = reader.ReadUInt16();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
            }
        }

        public static void Pack<T>(this Dictionary<ushort, T> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            foreach(var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }

        /// <summary>
        /// A Dictionary that uses a Int32 for the length.
        /// </summary>
        public static void Unpack<T>(this Dictionary<int, T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadInt32();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
            }
        }
        public static void Pack<T>(this Dictionary<int, T> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            writer.Write(value.Count);
            foreach(var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }


        /// <summary>
        /// A Dictionary that uses a Int32 for the length.
        /// </summary>
        public static void Unpack<T>(this Dictionary<uint, T> value, BinaryReader reader) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
            }
        }

        public static void Pack<T>(this Dictionary<uint, T> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            writer.Write((uint)value.Count);
            foreach(var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }    
        }

        public static void Unpack<T>(this Dictionary<uint, T> value, BinaryReader reader, uint fixedQuantity) where T : IUnpackable, new()
        {
            for (int i = 0; i < fixedQuantity; i++)
            {
                var key = reader.ReadUInt32();

                var item = new T();
                item.Unpack(reader);
                value.Add(key, item);
            }
        }

        /// <summary>
        /// A Dictionary that uses a Int32 for the length.
        /// </summary>
        public static void Unpack<T>(this Dictionary<uint, Dictionary<uint, T>> value, BinaryReader reader) where T : IUnpackable, new()
        {
            var totalObjects = reader.ReadUInt32();

            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var values = new Dictionary<uint, T>();
                values.Unpack(reader);

                value.Add(key, values);
            }
        }

        public static void Pack<T>(this Dictionary<uint, Dictionary<uint, T>> value, BinaryWriter writer) where T : IUnpackable, new()
        {
            writer.Write(value.Count);
            foreach(var e in value)
            {
                writer.Write(e.Key);
                e.Value.Pack(writer);
            }
        }

    }
}
