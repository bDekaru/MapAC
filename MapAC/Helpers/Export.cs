using MapAC.DatLoader;
using MapAC.DatLoader.Enum;
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

        public static void ExportEnvCell(uint envCellId, string path, int offsetX = 0, int offsetY = 0)
        {
            string fileName;
            var envCell = DatManager.CellDat.ReadFromDat<EnvCell>(envCellId);

            if (DatManager.DatVersion == DatVersionType.ACDM)
                envCell.Id = envCellId; // ACDM doesn't have this field!

            // We are moving this landblock!
            if (offsetX != 0 || offsetY != 0)
                envCell.MoveLandblock(offsetX, offsetY);

            fileName = GetExportPath(DatDatabaseType.Cell, path, envCell.Id);
            if (!File.Exists(fileName))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    envCell.Pack(writer);

                envCell.Id = envCellId; // Move the envCell back!

                // List any polyId / 0x0D Environment?
                // List any Setups/GfxObj?

                foreach (var cell in envCell.VisibleCells)
                {
                    var cellId = (envCellId & 0xFFFF0000) + cell;
                    ExportEnvCell(cellId, path, offsetX, offsetY);
                }
            }
        }


        // Cell 0xnnnnFFFE
        public static void ExportLandblockInfo(uint landblockId, string path, int offsetX = 0, int offsetY = 0)
        {
            string fileName;
            var landblock = DatManager.CellDat.ReadFromDat<LandblockInfo>(landblockId);

            // We are moving this landblock!
            if (offsetX != 0 || offsetY != 0)
                landblock.MoveLandblock(offsetX, offsetY);

            fileName = GetExportPath(DatDatabaseType.Cell, path, landblock.Id);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                landblock.Pack(writer);

            landblock.Id = landblockId;// Move the landblock back!

            uint baseLb = landblockId >> 24;
            foreach(var e in DatManager.CellDat.AllFiles)
            {
                if((e.Key >> 24) == baseLb && ((e.Key & 0xFFFF) < 0xFFFE))
                {
                    ExportEnvCell(e.Key, path, offsetX, offsetY);
                }
            }
            // export any EnvCells connected to the landscape...
        }

        // Cell 0xnnnnFFFF
        public static void ExportCellLandblock(uint landblockId, string path, int offsetX = 0, int offsetY = 0)
        {
            string fileName;
            var landblock = DatManager.CellDat.ReadFromDat<CellLandblock>(landblockId);

            // We are moving this landblock!
            if (offsetX != 0 || offsetY != 0)
                landblock.MoveLandblock(offsetX, offsetY);

            fileName = GetExportPath(DatDatabaseType.Cell, path, landblock.Id);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                landblock.Pack(writer);

            landblock.Id = landblockId; // Move the landblock back!

            var lbi = (landblockId & 0xFFFF0000) + 0xFFFE;
            if (DatManager.CellDat.AllFiles.ContainsKey(lbi)) {             
                ExportLandblockInfo(lbi, path, offsetX, offsetY);
            }
        }

        // 0x01
        public static void ExportGfxObject(uint gfxObjId, string path)
        {
            string fileName;
            var gfxObj = DatManager.CellDat.ReadFromDat<GfxObj>(gfxObjId);

            var saveGfxObjId = gfxObjId;
            if (DatManager.DatVersion == DatVersionType.ACDM)
                saveGfxObjId += (uint)ACDMOffset.GfxObj;

            fileName = GetExportPath(DatDatabaseType.Portal, path, saveGfxObjId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                gfxObj.Pack(writer);

            // Export all the Surfaces
            for (var i = 0; i < gfxObj.Surfaces.Count; i++)
                ExportSurface(gfxObj.Surfaces[i], path);

            if (gfxObj.DIDDegrade > 0)
                ExportDegrade(gfxObj.DIDDegrade, path);
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

                var saveSetupId = setupID;
                if (DatManager.DatVersion == DatVersionType.ACDM)
                    saveSetupId += (uint)ACDMOffset.Setup;

                fileName = GetExportPath(DatDatabaseType.Portal, path, saveSetupId);
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
        
        // 0x03
        public static void ExportAnimation(uint animId, string path)
        {
            var anim = DatManager.CellDat.ReadFromDat<Animation>(animId);

            var exportId = animId;
            if (DatManager.DatVersion == DatVersionType.ACDM)
                exportId += (uint)ACDMOffset.Animation;

            var fileName = GetExportPath(DatDatabaseType.Portal, path, exportId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                anim.Pack(writer);
        }


        // 0x04
        public static void ExportPalette(uint palId, string path)
        {
            var pal = DatManager.CellDat.ReadFromDat<Palette>(palId);

            var exportId = palId;
            if (DatManager.DatVersion == DatVersionType.ACDM)
                exportId += (uint)ACDMOffset.Palette;

            var fileName = GetExportPath(DatDatabaseType.Portal, path, exportId);
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
                surfaceId += (uint)ACDMOffset.Surface;
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

            var saveId = palSetId;
            if (DatManager.DatVersion == DatVersionType.ACDM)
                saveId += (uint)ACDMOffset.PaletteSet;
            var fileName = GetExportPath(DatDatabaseType.Portal, path, saveId);

            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                palSet.Pack(writer);

            // Export all Palettes in the set, too
        }

        // 0x10
        public static void ExportClothingTable(uint clothingTableId, string path)
        {
            var cb = DatManager.CellDat.ReadFromDat<ClothingTable>(clothingTableId);

            var saveClothingTableId = clothingTableId;
            if (DatManager.DatVersion == DatVersionType.ACDM)
                saveClothingTableId += (uint)ACDMOffset.ClothingTable;
            var fileName = GetExportPath(DatDatabaseType.Portal, path, saveClothingTableId);

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

        // 0x11
        public static void ExportDegrade(uint degradeId, string path)
        {
            var degrade = DatManager.CellDat.ReadFromDat<GfxObjDegradeInfo>(degradeId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, degradeId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                degrade.Pack(writer);
        }

        private static string GetExportPath(DatDatabaseType datDatabaseType, string path, uint objectId)
        {
            string exportFolder;

            string prefix = (objectId >> 24).ToString("X2") + "-";
            if (DatFile.GetFileType(datDatabaseType, objectId) != null)
                if(datDatabaseType != DatDatabaseType.Cell) 
                    exportFolder = Path.Combine(path, prefix + DatFile.GetFileType(datDatabaseType, objectId).ToString());
                else
                    exportFolder = Path.Combine(path, DatFile.GetFileType(datDatabaseType, objectId).ToString());
            else
                exportFolder = Path.Combine(path, "UnknownType");


            if (!Directory.Exists(exportFolder))
                Directory.CreateDirectory(exportFolder);

            string fileName = Path.Combine(exportFolder, objectId.ToString("X8") + ".bin");
            return fileName;
        }
    }
}
