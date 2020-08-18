using MapAC.DatLoader;
using MapAC.DatLoader.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAC
{
    public static class Export
    {

        public static void TestBinaryWriter()
        {
            string fileName = @"C:\ACE\PortalTemp\writers\";
            for (uint i = 0; i < 300000; i++)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.WriteCompressedUInt32(i);
                        writer.Flush();

                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            reader.BaseStream.Position = 0;
                            uint test = reader.ReadCompressedUInt32();
                            if (test != i)
                            {
                                reader.BaseStream.Position = 0;
                                var raw = reader.ReadUInt32();
                                var bad = true;
                            }
                        }
                    }
                }
            }
        }

        // 0x01
        public static void ExportGfxObject(uint gfxObjId, string path)
        {
            string fileName;
            var gfxObj = DatManager.CellDat.ReadFromDat<GfxObj>(gfxObjId);
            fileName = GetExportPath(DatDatabaseType.Portal, path, gfxObjId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                gfxObj.Pack(writer);

            // Export all the Surfaces
            for (var i = 0; i < gfxObj.Surfaces.Count; i++)
                ExportSurface(gfxObj.Surfaces[i], path);
        }

        /// <summary>
        /// 0x02 - Exports a Setup and all associated files.
        /// </summary>
        /// <param name="setupID">Valid Setup starting with 0x02</param>
        /// <param name="path"></param>
        public static void ExportSetup(uint setupID, string path)
        {
            string fileName;
            if (DatManager.CellDat.AllFiles.ContainsKey(setupID))
            {
                var setup = DatManager.CellDat.ReadFromDat<SetupModel>(setupID);
                fileName = GetExportPath(DatDatabaseType.Portal, path, setupID);
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    setup.Pack(writer);

                // Get all the GfxObjs in the Setup
                for (var i = 0; i < setup.Parts.Count; i++)
                    ExportGfxObject(setup.Parts[i], path);

                // Search through the ClothingTable entries for records with this Setup
                foreach (var e in DatManager.CellDat.AllFiles)
                {
                    // Just get the ClothingTable entries...
                    if (e.Key > 0x10000000 && e.Key < 0x10FFFFFF)
                    {
                        var cb = DatManager.CellDat.ReadFromDat<ClothingTable>(e.Key);
                        // Search the cb for our setupId
                        if (cb.ClothingBaseEffects.ContainsKey(setupID))
                            ExportClothingTable(e.Key, path);
                    }
                }
            }
        }

        // 0x04
        public static void ExportPalette(uint palId, string path)
        {
            var pal = DatManager.CellDat.ReadFromDat<Palette>(palId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, palId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                pal.Pack(writer);
        }

        // 0x05
        public static void ExportSurfaceTexture(uint surfaceTexId, string path)
        {
            var surfTex = DatManager.CellDat.ReadFromDat<SurfaceTexture>(surfaceTexId);
            uint exportSurfTexId = surfTex.GetId(); // Handles any conversion from ACDM to ACTOD
            var fileName = GetExportPath(DatDatabaseType.Portal, path, exportSurfTexId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                surfTex.Pack(writer);

            // Export the ACDM SurfaceTexture as a texture, too
            if (DatManager.DatVersion == DatVersionType.ACDM)
            {
                uint texId = surfTex.GetTextureId();
                fileName = GetExportPath(DatDatabaseType.Portal, path, texId);
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    surfTex.PackAsTexture(writer);
            }
            else
            {
                // Export all the Textures associated with the SurfaceTexture
                for(var i = 0; i < surfTex.Textures.Count; i++)
                    ExportTexture(surfTex.Textures[i], path);
            }
        }

        // 0x06
        public static void ExportTexture(uint texId, string path)
        {
            var texture = DatManager.CellDat.ReadFromDat<Texture>(texId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, texId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                texture.Pack(writer);

            if (texture.DefaultPaletteId != null)
                ExportPalette((uint)texture.DefaultPaletteId, path);

        }

        // 0x08
        public static void ExportSurface(uint surfaceId, string path)
        {
            var surface = DatManager.CellDat.ReadFromDat<Surface>(surfaceId);
            if (DatManager.DatVersion == DatVersionType.ACDM)
                surfaceId += 0x10000;
            var fileName = GetExportPath(DatDatabaseType.Portal, path, surfaceId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                surface.Pack(writer);

            if (surface.OrigTextureId > 0)
                ExportSurfaceTexture(surface.OrigTextureId, path);

            if (surface.OrigPaletteId > 0)
                ExportPalette(surface.OrigPaletteId, path);
        }

        // 0xA
        public static void ExportWave(uint waveID, string path)
        {
            if (DatManager.CellDat.AllFiles.ContainsKey(waveID))
            {
                var wav = DatManager.CellDat.ReadFromDat<Wave>(waveID);
                var fileName = GetExportPath(DatDatabaseType.Portal, path, waveID);
                wav.ExportWave(Path.GetDirectoryName(fileName));
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    wav.Pack(writer);
            }
         }

        // 0x0F
        public static void ExportPalSet(uint palSetId, string path)
        {
            var palSet = DatManager.CellDat.ReadFromDat<PaletteSet>(palSetId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, palSetId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                palSet.Pack(writer);

            // Export all Palettes in the set, too
        }

        // 0x10
        public static void ExportClothingTable(uint clothingTableId, string path)
        {
            var cb = DatManager.CellDat.ReadFromDat<ClothingTable>(clothingTableId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, clothingTableId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                cb.Pack(writer);

            // Export and GfxObj swaps and SurfaceTexture swaps...
            foreach (var cloBaseEffect in cb.ClothingBaseEffects)
            {
                for (var i = 0; i < cloBaseEffect.Value.CloObjectEffects.Count; i++)
                {
                    var cloObjEffect = cloBaseEffect.Value.CloObjectEffects[i];
                    // Export all the gfxObj's associated with this ClothingTable
                    uint gfxObjId = cloObjEffect.ModelId;
                    ExportGfxObject(gfxObjId, path);

                    // Just export the "NewTexture"...the old one is being replaced, so we don't need it
                    for(var j = 0; j < cloObjEffect.CloTextureEffects.Count; j++)
                        ExportSurfaceTexture(cloObjEffect.CloTextureEffects[j].NewTexture, path);
                }
            }

            // Export and icons and PalSets
            foreach(var subPalEffect in cb.ClothingSubPalEffects)
            {
                if (subPalEffect.Value.Icon > 0)
                    ExportTexture(subPalEffect.Value.Icon, path);

                for(var i = 0; i < subPalEffect.Value.CloSubPalettes.Count; i++)
                {
                    var cloSubPal = subPalEffect.Value.CloSubPalettes[i];
                    ExportPalSet(cloSubPal.PaletteSet, path);
                }
            }
        }

        private static string GetExportPath(DatDatabaseType datDatabaseType, string path, uint objectId)
        {
            string exportFolder;

            string prefix = (objectId >> 24).ToString("X2") + "-";
            if (DatFile.GetFileType(DatDatabaseType.Portal, objectId) != null)
                exportFolder = Path.Combine(path, prefix + DatFile.GetFileType(DatDatabaseType.Portal, objectId).ToString());
            else
                exportFolder = Path.Combine(path, "UnknownType");


            if (!Directory.Exists(exportFolder))
                Directory.CreateDirectory(exportFolder);

            string fileName = Path.Combine(exportFolder, objectId.ToString("X8") + ".bin");
            return fileName;
        }
    }
}
