using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using HtmlAgilityPack;
using MapAC.DatLoader.FileTypes;

namespace MapAC.DatLoader
{
    public class DatDatabase
    {
        private static readonly uint DAT_HEADER_OFFSET_TOD = 0x140;
        private static readonly uint DAT_HEADER_OFFSET_ACDM = 0x12C;
        public uint DatHeaderOffset;

        /// <summary>
        /// Contains a List of FileIds of all Exported files. Used to determine if we need to re-export an item.
        /// </summary>
        public List<uint> ExportedFiles = new List<uint>();

        /// <summary>
        /// Contains all the objectsIDs of files found in the client_portal.dat at end of retail
        /// </summary>
        //public List<uint> EoRPortalFiles = new List<uint>();

        public static Dictionary<uint, uint> EoRPortalFiles = new Dictionary<uint, uint>();

        public static Dictionary<uint, uint> SurfaceIdMigrationTable = new Dictionary<uint, uint>();

        public string FilePath { get; }

        private FileStream stream { get; }

        private static readonly object streamMutex = new object();

        public DatDatabaseHeader Header { get; } = new DatDatabaseHeader();
        public DatDatabaseHeader_ACDM Header_ACDM { get; } = new DatDatabaseHeader_ACDM();

        public uint Blocksize;

        public DatDirectory RootDirectory { get; }

        public Dictionary<uint, DatFile> AllFiles { get; } = new Dictionary<uint, DatFile>();

        public ConcurrentDictionary<uint, FileType> FileCache { get; } = new ConcurrentDictionary<uint, FileType>();

        public DatDatabase(string filePath, bool keepOpen = false)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            FilePath = filePath;
            uint btree = 0;

            stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using (var reader = new BinaryReader(stream, System.Text.Encoding.Default, true)) { 
                if (DatManager.DatVersion == DatVersionType.ACTOD)
                {
                    stream.Seek(DAT_HEADER_OFFSET_TOD, SeekOrigin.Begin);
                    Header.Unpack(reader);
                    if (Header.Success)
                    {
                        btree = Header.BTree;
                        Blocksize = Header.BlockSize;
                    }
                }
                else if (DatManager.DatVersion == DatVersionType.ACDM)
                {
                    reader.BaseStream.Seek(DAT_HEADER_OFFSET_ACDM, SeekOrigin.Begin);
                    Header_ACDM.Unpack(reader);
                    if (Header_ACDM.Success)
                    {
                        btree = Header_ACDM.BTree;
                        Blocksize = Header_ACDM.BlockSize;
                    }
                }
            }

            if (Blocksize > 0)
            {
                RootDirectory = new DatDirectory(btree, Blocksize);
                RootDirectory.Read(stream);
                RootDirectory.AddFilesToList(AllFiles);
            }

            if (!keepOpen)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }

            LoadEoRPortalContents();
            LoadTextureIdMigrationTable();
        }

        /// <summary>
        /// This will try to find the object for the given fileId in local cache. If the object was not found, it will be read from the dat and cached.<para />
        /// This function is thread safe.
        /// </summary>
        public T ReadFromDat<T>(uint fileId) where T : FileType, new()
        {
            // Check the FileCache so we don't need to hit the FileSystem repeatedly
            if (FileCache.TryGetValue(fileId, out FileType result))
                return (T)result;

            var datReader = GetReaderForFile(fileId);

            var obj = new T();

            if (datReader != null)
            {
                using (var memoryStream = new MemoryStream(datReader.Buffer))
                using (var reader = new BinaryReader(memoryStream))
                    obj.Unpack(reader);
            }

            // Store this object in the FileCache
            obj = (T)FileCache.GetOrAdd(fileId, obj);

            return obj;
        }

        public DatReader GetReaderForFile(uint fileId)
        {
            if (AllFiles.TryGetValue(fileId, out var file))
            {
                DatReader dr;

                if (stream != null)
                {
                    lock (streamMutex)
                        dr = new DatReader(stream, file.FileOffset, file.FileSize, Blocksize);
                }
                else
                    dr = new DatReader(FilePath, file.FileOffset, file.FileSize, Blocksize);

                return dr;                    
            }

            return null;
        }

        public void ExtractCategorizedPortalContents(string path)
        {
            foreach (KeyValuePair<uint, DatFile> entry in AllFiles)
            {
                string thisFolder;

                if (entry.Value.GetFileType(DatDatabaseType.Portal) != null)
                    thisFolder = Path.Combine(path, entry.Value.GetFileType(DatDatabaseType.Portal).ToString());
                else
                    thisFolder = Path.Combine(path, "UnknownType");

                if (!Directory.Exists(thisFolder))
                    Directory.CreateDirectory(thisFolder);

                string hex = entry.Value.ObjectId.ToString("X8");
                string thisFile = Path.Combine(thisFolder, hex + ".bin");

                // Use the DatReader to get the file data
                DatReader dr = GetReaderForFile(entry.Value.ObjectId);

                File.WriteAllBytes(thisFile, dr.Buffer);
            }
        }

        public bool IsExported(uint objectId)
        {
            if (ExportedFiles.IndexOf(objectId) == -1)
            {
                ExportedFiles.Add(objectId);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads the list of client_portal.dat files from end of retail so we can reference later to not duplicate.
        /// </summary>
        //private void LoadEoRPortalContents()
        //{
        //    if (EoRPortalFiles.Count != 0) return; // Already done!

        //    var contents = File.ReadAllLines(@"PortalContents.txt");
        //    foreach(var item in contents)
        //    {
        //        try
        //        {
        //            uint dec = UInt32.Parse(item, System.Globalization.NumberStyles.HexNumber);
        //            EoRPortalFiles.Add(dec);
        //        }
        //        catch(Exception E)
        //        {
        //            // Do nothing
        //        }
        //    }
        //}

        private void LoadEoRPortalContents()
        {
            if (SurfaceIdMigrationTable.Count != 0) return; // Already done!

            var contents = File.ReadAllLines(@"PortalContents.txt");
            foreach (var item in contents)
            {
                try
                {
                    string[] splitLine = item.Split('\t');

                    if (splitLine.Length >= 2)
                    {
                        uint a = uint.Parse(splitLine[0], System.Globalization.NumberStyles.HexNumber);
                        uint b = uint.Parse(splitLine[1]);
                        EoRPortalFiles.Add(a, b);
                    }
                }
                catch (Exception E)
                {
                    // Do nothing
                }
            }
        }

        private void LoadTextureIdMigrationTable()
        {
            if (SurfaceIdMigrationTable.Count != 0) return; // Already done!

            var contents = File.ReadAllLines(@"SurfaceIdMigrationTable.txt");
            foreach (var item in contents)
            {
                try
                {
                    string[] splitLine = item.Split('\t');

                    if (splitLine.Length >= 2)
                    {
                        uint a = uint.Parse(splitLine[0], System.Globalization.NumberStyles.HexNumber);
                        uint b = uint.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber);
                        SurfaceIdMigrationTable.Add(a, b);
                    }
                }
                catch (Exception E)
                {
                    // Do nothing
                }
            }
        }

        public static uint TranslateSurfaceId(uint id)
        {
            if (SurfaceIdMigrationTable.ContainsKey(id))
                return SurfaceIdMigrationTable[id];
            else
                return 0;
        }

        /// <summary>
        /// Check if the file exists and is the same in EoR portal.dat
        /// </summary>
        public bool IsSameAsEoRDatFile(uint objectId, uint forceDifferentId = 1)
        {
            if (objectId == 0)
                return true;

            if (EoRPortalFiles.TryGetValue(objectId, out var eorHash))
            {
                if (GetHash(objectId, forceDifferentId) == eorHash)
                    return true;
            }
            return false;
        }

        public uint GetHash(uint objectId, uint forceDifferentId = 1)
        {
            FileType file = null;

            var datFileType = DatFile.GetFileType(DatDatabaseType.Portal, objectId);
            switch (datFileType)
            {
                case DatFileType.GraphicsObject:
                    file = DatManager.CellDat.ReadFromDat<GfxObj>(objectId);
                    break;
                case DatFileType.Setup:
                    file = DatManager.CellDat.ReadFromDat<SetupModel>(objectId);
                    break;
                case DatFileType.Animation:
                    file = DatManager.CellDat.ReadFromDat<Animation>(objectId);
                    break;
                case DatFileType.Palette:
                    file = DatManager.CellDat.ReadFromDat<Palette>(objectId);
                    break;
                case DatFileType.SurfaceTexture:
                    file = DatManager.CellDat.ReadFromDat<SurfaceTexture>(objectId);
                    break;
                case DatFileType.Texture:
                    file = DatManager.CellDat.ReadFromDat<Texture>(objectId);
                    break;
                case DatFileType.Surface:
                    file = DatManager.CellDat.ReadFromDat<Surface>(objectId);
                    break;
                case DatFileType.MotionTable:
                    file = DatManager.CellDat.ReadFromDat<MotionTable>(objectId);
                    break;
                case DatFileType.Wave:
                    file = DatManager.CellDat.ReadFromDat<Wave>(objectId);
                    break;
                case DatFileType.Environment:
                    file = DatManager.CellDat.ReadFromDat<FileTypes.Environment>(objectId);
                    break;
                case DatFileType.PaletteSet:
                    file = DatManager.CellDat.ReadFromDat<PaletteSet>(objectId);
                    break;
                case DatFileType.Clothing:
                    file = DatManager.CellDat.ReadFromDat<ClothingTable>(objectId);
                    break;
                case DatFileType.DegradeInfo:
                    file = DatManager.CellDat.ReadFromDat<GfxObjDegradeInfo>(objectId);
                    break;
                case DatFileType.Scene:
                    file = DatManager.CellDat.ReadFromDat<Scene>(objectId);
                    break;
                case DatFileType.CombatTable:
                    file = DatManager.CellDat.ReadFromDat<CombatManeuverTable>(objectId);
                    break;
                case DatFileType.String:
                    file = DatManager.CellDat.ReadFromDat<LanguageString>(objectId);
                    break;
                case DatFileType.SoundTable:
                    file = DatManager.CellDat.ReadFromDat<SoundTable>(objectId);
                    break;
                case DatFileType.ParticleEmitter:
                    file = DatManager.CellDat.ReadFromDat<ParticleEmitterInfo>(objectId);
                    break;
                case DatFileType.PhysicsScript:
                    file = DatManager.CellDat.ReadFromDat<PhysicsScript>(objectId);
                    break;
                case DatFileType.PhysicsScriptTable:
                    file = DatManager.CellDat.ReadFromDat<PhysicsScriptTable>(objectId);
                    break;
                default:
                    var reader = DatManager.CellDat.GetReaderForFile(objectId);
                    if (reader == null)
                        return 0;
                    return Crc32.CRC32Bytes(reader.Buffer);
            }

            return file.GetHash(forceDifferentId);
        }

    }
}
