using HtmlAgilityPack;
using MapAC.DatLoader;
using MapAC.DatLoader.Entity.AnimationHooks;
using MapAC.DatLoader.Enum;
using MapAC.DatLoader.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using WindowsFormsApp1;

namespace MapAC
{
    public static class Export
    {
        public static bool ExportPortalFile(uint exportId, string path)
        {
            // Quit if this has already been handled!
            //if (DatManager.CellDat.IsExported(exportId) || IsAddition(exportId)) return true;
            if (DatManager.CellDat.IsExported(exportId)) return true;

            if (DatManager.CellDat.AllFiles.ContainsKey(exportId))
            {
                //string filename = GetExportPath(DatDatabaseType.Portal, path, exportId);
                //if (!File.Exists(filename))
                {
                    var datFileType = DatFile.GetFileType(DatDatabaseType.Portal, exportId);
                    switch (datFileType)
                    {
                        case DatFileType.GraphicsObject: ExportGfxObject(exportId, path); break;
                        case DatFileType.Setup: ExportSetup(exportId, path); break;
                        case DatFileType.Animation: ExportAnimation(exportId, path); break;
                        case DatFileType.Palette: ExportPalette(exportId, path); break;
                        case DatFileType.SurfaceTexture: ExportSurfaceTexture(exportId, path); break;
                        case DatFileType.Texture: ExportTexture(exportId, path); break;
                        case DatFileType.Surface: ExportSurface(exportId, path); break;
                        case DatFileType.MotionTable: ExportMotionTable(exportId, path); break;
                        case DatFileType.Wave: ExportWave(exportId, path); break;
                        case DatFileType.Environment: ExportEnvironment(exportId, path); break;
                        case DatFileType.PaletteSet: ExportPalSet(exportId, path); break;
                        case DatFileType.Clothing: ExportClothingTable(exportId, path); break;
                        case DatFileType.DegradeInfo: ExportDegrade(exportId, path); break;
                        case DatFileType.Scene: ExportScene(exportId, path); break;
                        case DatFileType.CombatTable: ExportCombatTable(exportId, path); break;
                        case DatFileType.String: ExportString(exportId, path); break;
                        case DatFileType.SoundTable: ExportSoundTable(exportId, path); break;
                        case DatFileType.ParticleEmitter: ExportParticleEmitter(exportId, path); break;
                        case DatFileType.PhysicsScript: ExportPhysicsScript(exportId, path); break;
                        case DatFileType.PhysicsScriptTable: ExportPhysicsScriptTable(exportId, path); break;
                        case DatFileType.Region: ExportRegion(exportId, path); break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                return true;
            }

            return false;
        }

        public static void ExportEnvCell(uint envCellId, string path, int offsetX = 0, int offsetY = 0)
        {
            if (DatManager.CellDat.IsExported(envCellId)) return;
            string fileName;
            var envCell = DatManager.CellDat.ReadFromDat<EnvCell>(envCellId);

            if (DatManager.DatVersion == DatVersionType.ACDM)
                envCell.Id = envCellId; // ACDM doesn't have this field!

            // We are moving this landblock!
            if (offsetX != 0 || offsetY != 0)
                envCell.MoveLandblock(offsetX, offsetY);

            fileName = GetExportPath(DatDatabaseType.Cell, path, envCell.Id);

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
            foreach (var e in DatManager.CellDat.AllFiles)
            {
                if ((e.Key >> 24) == baseLb && ((e.Key & 0xFFFF) < 0xFFFE))
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
            if (DatManager.CellDat.AllFiles.ContainsKey(lbi))
            {
                ExportLandblockInfo(lbi, path, offsetX, offsetY);
            }
        }

        public static bool IsSurfaceTextureAddition(uint currentId, out uint id)
        {
            id = currentId;
            if (DatManager.DatVersion == DatVersionType.ACDM)
            {
                id = DatDatabase.TranslateSurfaceTextureId(id);
                if (id != 0)
                    return false; // We skip the isSame check as they won't ever match as EoR files have high resolution texture entries as well.
                else
                    id = currentId;
            }
            var isSame = DatManager.CellDat.IsSameAsEoRDatFile(id);

            if (!isSame)
                return true;
            else
                return false;
        }

        public static bool IsSurfaceAddition(uint currentId, out uint id)
        {
            id = currentId;
            if (DatManager.DatVersion == DatVersionType.ACDM)
            {
                id = DatDatabase.TranslateSurfaceId(id);
                if (id == 0)
                    id = currentId;
            }
            var isSame = DatManager.CellDat.IsSameAsEoRDatFile(id);

            if (!isSame)
                return true;
            else
                return false;
        }

        public static bool IsAddition(uint currentId)
        {
            var isSame = DatManager.CellDat.IsSameAsEoRDatFile(currentId);

            if (!isSame)
                return true;
            else
                return false;
        }

        // 0x01
        public static void ExportGfxObject(uint gfxObjId, string path)
        {
            string fileName;
            var gfxObj = DatManager.CellDat.ReadFromDat<GfxObj>(gfxObjId);

            fileName = GetExportPath(DatDatabaseType.Portal, path, gfxObjId);

            if (IsAddition(gfxObjId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    gfxObj.Pack(writer);
            }

            // Export all the Surfaces
            for (var i = 0; i < gfxObj.Surfaces.Count; i++)
            {
                if (IsSurfaceAddition(gfxObj.Surfaces[i], out var id))
                    ExportPortalFile(gfxObj.Surfaces[i], path);
                else
                    gfxObj.Surfaces[i] = id;
            }

            if (gfxObj.DIDDegrade > 0)
                ExportPortalFile(gfxObj.DIDDegrade, path);
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
                if (IsAddition(setupID))
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        setup.Pack(writer);
                }

                // Get all the GfxObjs in the Setup
                for (var i = 0; i < setup.Parts.Count; i++)
                {
                    ExportPortalFile(setup.Parts[i], path);
                }

                // Search through the ClothingTable entries for records with this Setup
                /*
                foreach (var e in DatManager.CellDat.AllFiles)
                {
                    // Just get the ClothingTable entries...
                    if (e.Key > 0x10000000 && e.Key < 0x10FFFFFF)
                    {
                        var cb = DatManager.CellDat.ReadFromDat<ClothingTable>(e.Key);
                        // Search the cb for our setupId
                        if (cb.ClothingBaseEffects.ContainsKey(setupID))
                            ExportPortalFile(e.Key, path);
                    }
                }
                */

                if (setup.DefaultAnimation > 0)
                    ExportPortalFile(setup.DefaultAnimation, path);
                if (setup.DefaultScript > 0)
                    ExportPortalFile(setup.DefaultScript, path);
                if (setup.DefaultMotionTable > 0)
                    ExportPortalFile(setup.DefaultMotionTable, path);
                if (setup.DefaultSoundTable > 0)
                    ExportPortalFile(setup.DefaultSoundTable, path);
                if (setup.DefaultScriptTable > 0)
                    ExportPortalFile(setup.DefaultScriptTable, path);
            }
        }

        // 0x03
        public static void ExportAnimation(uint animId, string path)
        {
            if (!IsAddition(animId))
                return;

            var anim = DatManager.CellDat.ReadFromDat<Animation>(animId);

            var fileName = GetExportPath(DatDatabaseType.Portal, path, animId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                anim.Pack(writer);
        }

        // 0x04
        public static void ExportPalette(uint palId, string path)
        {
            if (!IsAddition(palId))
                return;

            var pal = DatManager.CellDat.ReadFromDat<Palette>(palId);

            var fileName = GetExportPath(DatDatabaseType.Portal, path, palId);
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                pal.Pack(writer);
        }

        // 0x05
        public static void ExportSurfaceTexture(uint surfaceTexId, string path)
        {
            var surfTex = DatManager.CellDat.ReadFromDat<SurfaceTexture>(surfaceTexId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, surfaceTexId);
            if (IsSurfaceTextureAddition(surfaceTexId, out var id))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    surfTex.Pack(writer);

                // Export the ACDM SurfaceTexture as a texture, too
                if (DatManager.DatVersion == DatVersionType.ACDM)
                {
                    uint texId = surfTex.GetTextureId();
                    fileName = GetExportPath(DatDatabaseType.Portal, path, texId);
                    using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        surfTex.PackAsTexture(writer);

                    surfTex.ExportTexture(Path.GetDirectoryName(fileName));
                }
                else
                {
                    // Export all the Textures associated with the SurfaceTexture
                    for (var i = 0; i < surfTex.Textures.Count; i++)
                        ExportPortalFile(surfTex.Textures[i], path);
                }
            }
        }

        // 0x06
        public static void ExportTexture(uint texId, string path)
        {
            var texture = DatManager.CellDat.ReadFromDat<Texture>(texId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, texId);
            if (IsAddition(texId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    texture.Pack(writer);
                texture.ExportTexture(Path.GetDirectoryName(fileName));
            }

            if (texture.DefaultPaletteId != null)
                ExportPortalFile((uint)texture.DefaultPaletteId, path);

        }

        // 0x08
        public static void ExportSurface(uint surfaceId, string path)
        {          
            var surface = DatManager.CellDat.ReadFromDat<Surface>(surfaceId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, surfaceId);
            if (IsSurfaceAddition(surfaceId, out var id))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    surface.Pack(writer);
            }

            if (surface.OrigTextureId > 0)
                ExportPortalFile(surface.OrigTextureId, path);

            if (surface.OrigPaletteId > 0)
                ExportPortalFile(surface.OrigPaletteId, path);
        }

        // 0x09
        public static void ExportMotionTable(uint motionId, string path)
        {
            var motion = DatManager.CellDat.ReadFromDat<MotionTable>(motionId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, motionId);
            if (IsAddition(motionId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    motion.Pack(writer);
            }

            foreach (var link in motion.Links)
                foreach (var motionData in link.Value)
                    foreach (var anim in motionData.Value.Anims)
                    {
                        ExportPortalFile(anim.AnimId, path);
                    }

        }

        // 0xA
        public static void ExportWave(uint waveID, string path)
        {
            if (!IsAddition(waveID))
                return;

            if (DatManager.CellDat.AllFiles.ContainsKey(waveID))
            {
                var wav = DatManager.CellDat.ReadFromDat<Wave>(waveID);

                var fileName = GetExportPath(DatDatabaseType.Portal, path, waveID);

                wav.ExportWave(Path.GetDirectoryName(fileName));
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    wav.Pack(writer);
            }
        }


        // 0x0D
        public static void ExportEnvironment(uint envID, string path)
        {
            if (!IsAddition(envID))
                return;

            if (DatManager.CellDat.AllFiles.ContainsKey(envID))
            {
                var env = DatManager.CellDat.ReadFromDat<DatLoader.FileTypes.Environment>(envID);
                var fileName = GetExportPath(DatDatabaseType.Portal, path, envID);
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    env.Pack(writer);

                //TODO -- export things in this environemnt!? Textures, Surfaces, etc
            }
        }
        // 0x0F
        public static void ExportPalSet(uint palSetId, string path)
        {
            var palSet = DatManager.CellDat.ReadFromDat<PaletteSet>(palSetId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, palSetId);

            if (IsAddition(palSetId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    palSet.Pack(writer);
            }

            // Export all Palettes in the set, too
            foreach (var p in palSet.PaletteList)
                ExportPortalFile(p, path);
        }

        // 0x10
        public static void ExportClothingTable(uint clothingTableId, string path)
        {
            var cb = DatManager.CellDat.ReadFromDat<ClothingTable>(clothingTableId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, clothingTableId);

            if (IsAddition(clothingTableId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    cb.Pack(writer);
            }

            // Export and GfxObj swaps and SurfaceTexture swaps...
            foreach (var cloBaseEffect in cb.ClothingBaseEffects)
            {
                uint setupId = cloBaseEffect.Key;
                ExportPortalFile(setupId, path);

                for (var i = 0; i < cloBaseEffect.Value.CloObjectEffects.Count; i++)
                {
                    var cloObjEffect = cloBaseEffect.Value.CloObjectEffects[i];
                    // Export all the gfxObj's associated with this ClothingTable
                    uint gfxObjId = cloObjEffect.ModelId;
                    ExportPortalFile(gfxObjId, path);

                    for (var j = 0; j < cloObjEffect.CloTextureEffects.Count; j++)
                    {
                        ExportPortalFile(cloObjEffect.CloTextureEffects[j].OldTexture, path);
                        ExportPortalFile(cloObjEffect.CloTextureEffects[j].NewTexture, path);
                    }
                }
            }

            // Export and icons and PalSets
            foreach (var subPalEffect in cb.ClothingSubPalEffects)
            {
                if (subPalEffect.Value.Icon > 0)
                    ExportPortalFile(subPalEffect.Value.Icon, path);

                for (var i = 0; i < subPalEffect.Value.CloSubPalettes.Count; i++)
                {
                    var cloSubPal = subPalEffect.Value.CloSubPalettes[i];
                    ExportPortalFile(cloSubPal.PaletteSet, path);
                }
            }
        }

        // 0x11
        public static void ExportDegrade(uint degradeId, string path)
        {
            if (!IsAddition(degradeId))
                return;

            var degrade = DatManager.CellDat.ReadFromDat<GfxObjDegradeInfo>(degradeId);

            var fileName = GetExportPath(DatDatabaseType.Portal, path, degradeId);

            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                degrade.Pack(writer);
        }

        // 0x12
        public static void ExportScene(uint sceneId, string path)
        {
            var scene = DatManager.CellDat.ReadFromDat<Scene>(sceneId);

            var fileName = GetExportPath(DatDatabaseType.Portal, path, sceneId);

            if (IsAddition(sceneId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    scene.Pack(writer);
            }

            foreach (var entry in scene.Objects)
            {
                if (entry.ObjId != 0)
                    ExportPortalFile(entry.ObjId, path);
            }
        }

        // 0x20
        public static void ExportSoundTable(uint stableId, string path)
        {
            var stable = DatManager.CellDat.ReadFromDat<SoundTable>(stableId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, stableId);

            if (IsAddition(stableId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    stable.Pack(writer);
            }

            foreach (var d in stable.Data)
            {
                for (var i = 0; i < d.Value.Data.Count; i++)
                    ExportPortalFile(d.Value.Data[i].SoundId, path);
            }
        }

        // 0x30
        public static void ExportCombatTable(uint cmtId, string path)
        {
            if (!IsAddition(cmtId))
                return;

            var cmt = DatManager.CellDat.ReadFromDat<CombatManeuverTable>(cmtId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, cmtId);

            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                cmt.Pack(writer);
        }

        // 0x31
        public static void ExportString(uint objId, string path)
        {
            if (!IsAddition(objId))
                return;

            var obj = DatManager.CellDat.ReadFromDat<LanguageString>(objId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, objId);

            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                obj.Pack(writer);
        }

        // 0x32
        public static void ExportParticleEmitter(uint partEmitterId, string path)
        {
            var particle = DatManager.CellDat.ReadFromDat<ParticleEmitterInfo>(partEmitterId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, partEmitterId);

            if (IsAddition(partEmitterId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    particle.Pack(writer);
            }

            if (particle.GfxObjId > 0)
                ExportPortalFile(particle.GfxObjId, path);
            if (particle.HwGfxObjId > 0)
                ExportPortalFile(particle.HwGfxObjId, path);
        }

        // 0x33
        public static void ExportPhysicsScript(uint physicsScriptId, string path)
        {
            var physScript = DatManager.CellDat.ReadFromDat<PhysicsScript>(physicsScriptId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, physicsScriptId);

            if (IsAddition(physicsScriptId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    physScript.Pack(writer);
            }

            foreach (var sd in physScript.ScriptData)
            {
                switch (sd.Hook.HookType)
                {
                    case AnimationHookType.CallPES:
                        var callPESHook = (CallPESHook)sd.Hook;
                        ExportPortalFile(callPESHook.PES, path);
                        break;
                    case AnimationHookType.CreateParticle:
                        var createParticleHook = (CreateParticleHook)sd.Hook;
                        ExportPortalFile(createParticleHook.EmitterInfoId, path);
                        break;
                    case AnimationHookType.DestroyParticle:
                        var destroyHook = (DestroyParticleHook)sd.Hook;
                        ExportPortalFile(destroyHook.EmitterId, path);
                        break;
                    case AnimationHookType.SoundTweaked:
                        var soundTweakedHook = (SoundTweakedHook)sd.Hook;
                        ExportPortalFile(soundTweakedHook.SoundID, path);
                        break;
                }
            }

        }

        // 0x34
        public static void ExportPhysicsScriptTable(uint physicsScriptTableId, string path)
        {
            var physScriptTable = DatManager.CellDat.ReadFromDat<PhysicsScriptTable>(physicsScriptTableId);
            var fileName = GetExportPath(DatDatabaseType.Portal, path, physicsScriptTableId);

            if (IsAddition(physicsScriptTableId))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    physScriptTable.Pack(writer);
            }

            foreach (var script in physScriptTable.ScriptTable)
            {
                foreach (var mod in script.Value.Scripts)
                {
                    ExportPortalFile(mod.ScriptId, path);
                }
            }
        }

        // 0x13
        public static void ExportRegion(uint regionId, string path)
        {
            var region = DatManager.CellDat.ReadFromDat<RegionDesc>(regionId);

            region.Id = 0x13000000;
            region.SceneInfo.SceneTypes[60].Scenes.Add(0x1200025b); // Add frozen fields
            var fileName = GetExportPath(DatDatabaseType.Portal, path, region.Id);

            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                region.Pack(writer);
        }

        private static string GetExportPath(DatDatabaseType datDatabaseType, string path, uint objectId)
        {
            string exportFolder;
            if (datDatabaseType == DatDatabaseType.Portal)
                objectId = GetFileWithACDMOffset(objectId);

            string prefix = (objectId >> 24).ToString("X2") + "-";
            if (DatFile.GetFileType(datDatabaseType, objectId) != null)
                if (datDatabaseType != DatDatabaseType.Cell)
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

        public static uint GetFileWithACDMOffset(uint objectId)
        {
            // Do nothing!
            if (DatManager.DatVersion != DatVersionType.ACDM) return objectId;

            if (objectId == 0)
                return 0;

            if (!IsAddition(objectId) || !DatManager.CellDat.ExistsInEoR(objectId))
                return objectId;

            var datFileType = DatFile.GetFileType(DatDatabaseType.Portal, objectId);
            switch (datFileType)
            {
                case DatFileType.GraphicsObject: return objectId + (uint)ACDMOffset.GfxObj;
                case DatFileType.Setup: return objectId + (uint)ACDMOffset.Setup;
                case DatFileType.Animation: return objectId + (uint)ACDMOffset.Animation;
                case DatFileType.Palette: return objectId + (uint)ACDMOffset.Palette;
                case DatFileType.SurfaceTexture: return objectId + (uint)ACDMOffset.SurfaceTexture;
                case DatFileType.Texture: return objectId + (uint)ACDMOffset.Texture;
                case DatFileType.Surface: return objectId + (uint)ACDMOffset.Surface;
                case DatFileType.MotionTable: return objectId + (uint)ACDMOffset.MotionTable;
                case DatFileType.Wave: return objectId + (uint)ACDMOffset.Wave;
                case DatFileType.Environment: return objectId + (uint)ACDMOffset.Environment;
                case DatFileType.PaletteSet: return objectId + (uint)ACDMOffset.PaletteSet;
                case DatFileType.Clothing: return objectId + (uint)ACDMOffset.ClothingTable;
                case DatFileType.DegradeInfo: return objectId + (uint)ACDMOffset.DIDDegrade;
                case DatFileType.Scene: return objectId + (uint)ACDMOffset.Scene;
                case DatFileType.CombatTable: return objectId + (uint)ACDMOffset.CombatTable;
                case DatFileType.String: return objectId + (uint)ACDMOffset.String;
                case DatFileType.SoundTable: return objectId + (uint)ACDMOffset.SoundTable;
                case DatFileType.ParticleEmitter: return objectId + (uint)ACDMOffset.EmitterInfo;
                case DatFileType.PhysicsScript: return objectId + (uint)ACDMOffset.PhysicsScript;
                case DatFileType.PhysicsScriptTable: return objectId + (uint)ACDMOffset.PhysicsScriptTable;
                case DatFileType.Region: return objectId;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void BuildPortalContentsTable()
        {
            StreamWriter outputFile = new StreamWriter(new FileStream("PortalContentsNew.txt", FileMode.Create, FileAccess.Write));
            if (outputFile == null)
            {
                Console.WriteLine("Unable to open PortalContentsNew.txt");
                return;
            }

            for (uint fileId = 0x00000000; fileId < 0xFFFFFFFF; fileId++)
            {
                if (DatManager.CellDat.AllFiles.TryGetValue(fileId, out _))
                {
                    var hash = DatManager.CellDat.GetHash(fileId);

                    outputFile.WriteLine($"{fileId.ToString("X8")}\t{hash}");
                    outputFile.Flush();
                }
            }
            outputFile.Close();
        }

        public static void ExportAllSurfaceTextures(string path)
        {
            if (DatManager.DatVersion == DatVersionType.ACDM)
            {
                for (uint fileId = 0x05000000; fileId < 0x05FFFFFF; fileId++)
                {
                    if (DatManager.CellDat.AllFiles.TryGetValue(fileId, out _))
                    {
                        var surfTex = DatManager.CellDat.ReadFromDat<SurfaceTexture>(fileId);
                        DatManager.DatVersion = DatVersionType.ACTOD;
                        var fileName = GetExportPath(DatDatabaseType.Portal, path, fileId);
                        DatManager.DatVersion = DatVersionType.ACDM;
                        Texture tex = surfTex.ConvertToTexture(false);
                        tex.Id = fileId;
                        tex.ExportTexture(Path.GetDirectoryName(fileName));
                    }
                }
            }
            else
            {
                for (uint fileId = 0x05000000; fileId < 0x05FFFFFF; fileId++)
                {
                    if (DatManager.CellDat.AllFiles.TryGetValue(fileId, out _))
                    {
                        var surfTex = DatManager.CellDat.ReadFromDat<SurfaceTexture>(fileId);
                        foreach (var entry in surfTex.Textures)
                        {
                            var tex = DatManager.CellDat.ReadFromDat<Texture>(entry);
                            var fileName = GetExportPath(DatDatabaseType.Portal, path, fileId);
                            tex.Id = fileId;
                            tex.ExportTexture(Path.GetDirectoryName(fileName));
                        }
                    }
                }
                //for (uint fileId = 0x06000000; fileId < 0x06FFFFFF; fileId++)
                //{
                //    if (DatManager.CellDat.AllFiles.TryGetValue(fileId, out _))
                //    {
                //        var tex = DatManager.CellDat.ReadFromDat<Texture>(fileId);
                //        var fileName = GetExportPath(DatDatabaseType.Portal, path, fileId);
                //        tex.ExportTexture(Path.GetDirectoryName(fileName));
                //    }
                //}
            }
        }

        public static void CreateSurfaceTextureTranslationTable(Form1 form)
        {
            form.AddStatus("Creating Surface Texture Translation Table, this may take a while...");

            var diACDM = new DirectoryInfo("./Textures/ACDM 05/");
            var filesACDM = diACDM.GetFiles("*.png");

            var output = new List<string>();
            var remainingList = new List<FileInfo>();
            foreach (var file in filesACDM)
            {
                var eorFile = new FileInfo(file.FullName.Replace("ACDM", "EoR"));
                if (eorFile.Exists)
                {
                    uint hash = 0;
                    using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    using (var reader = new BinaryReader(stream))
                    {
                        var content = reader.ReadBytes((int)reader.BaseStream.Length);
                        hash = Crc32.CRC32Bytes(content);
                    }

                    uint eorHash = 0;
                    using (var stream = new FileStream(eorFile.FullName, FileMode.Open, FileAccess.Read))
                    using (var reader = new BinaryReader(stream))
                    {
                        var content = reader.ReadBytes((int)reader.BaseStream.Length);
                        eorHash = Crc32.CRC32Bytes(content);
                    }

                    if (hash != eorHash)
                        remainingList.Add(file);
                    else
                        output.Add($"{file.Name.Replace(".png", "")}\t{eorFile.Name.Replace(".png", "")}");
                }
            }

            var diEoR = new DirectoryInfo("./Textures/EoR 05/");
            var filesEoR = diEoR.GetFiles("*.png");
            var eorHashMap = new Dictionary<string, uint>();

            foreach (var eorFile in filesEoR)
            {
                uint eorHash = 0;
                using (var stream = new FileStream(eorFile.FullName, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(stream))
                {
                    var content = reader.ReadBytes((int)reader.BaseStream.Length);
                    eorHash = Crc32.CRC32Bytes(content);
                    eorHashMap.Add(eorFile.Name, eorHash);
                }
            }

            var remainingList2 = new List<FileInfo>();
            foreach (var file in remainingList)
            {
                bool found = false;

                uint hash = 0;
                using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(stream))
                {
                    var content = reader.ReadBytes((int)reader.BaseStream.Length);
                    hash = Crc32.CRC32Bytes(content);
                }

                foreach (var eorFile in eorHashMap)
                {
                    if (eorFile.Key == file.Name)
                        continue;

                    if (hash == eorFile.Value)
                    {
                        output.Add($"{file.Name.Replace(".png", "")}\t{eorFile.Key.Replace(".png", "")}");
                        found = true;
                        break;
                    }
                }

                if (!found)
                    remainingList2.Add(file);
            }

            File.WriteAllLines("SurfaceTextureTranslationTable-ExactMatch-New.txt", output);
            form.AddStatus($"Finished creating Surface Texture Translation Table. Found {output.Count}/{filesACDM.Length} exact match translations.");

            //var httpClient = new HttpClient();

            //string firstEntry = "";
            //if (File.Exists("./SurfaceTextureDistances.txt"))
            //{
            //    var existing = File.ReadAllLines("./SurfaceTextureDistances.txt");
            //    var line = existing[existing.Length - 1].Split('\t');
            //    firstEntry = line[0];
            //}

            //var writer = File.AppendText("./SurfaceTextureDistances.txt");
            //foreach (var file in remainingList)
            //{
            //    if (firstEntry != "")
            //    {
            //        if (file.Name != $"{firstEntry}.png")
            //            continue;
            //        else
            //        {
            //            firstEntry = "";
            //            continue;
            //        }
            //    }

            //    var eorFile = new FileInfo(file.FullName.Replace("ACDM", "EoR"));
            //    if (eorFile.Exists)
            //    {
            //        using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.deepai.org/api/image-similarity"))
            //        {
            //            request.Headers.TryAddWithoutValidation("api-key", "quickstart-QUdJIGlzIGNvbWluZy4uLi4K");

            //            var multipartContent = new MultipartFormDataContent();
            //            multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(file.FullName)), "image1", file.Name);
            //            multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(eorFile.FullName)), "image2", "EoR_" + eorFile.Name);
            //            request.Content = multipartContent;

            //            var response = await httpClient.SendAsync(request);
            //            var result = response.Content.ReadAsStringAsync();

            //            if (result.Result.Contains("distance"))
            //            {
            //                var start = result.Result.LastIndexOf("distance\": ") + 11;
            //                var end = result.Result.LastIndexOf("\n    }");
            //                var length = end - start;
            //                var distanceString = result.Result.Substring(start, length);
            //                if (int.TryParse(distanceString, out var distance))
            //                {
            //                    writer.WriteLine($"{file.Name.Replace(".png", "")}\t{distance}");
            //                    writer.Flush();

            //                    output.Add($"{file.Name.Replace(".png", "")}\t{distance}");
            //                }
            //                form.AddStatus(result.Result);
            //            }
            //            else
            //            {
            //                form.AddStatus(result.Result);
            //                break;
            //            }
            //        }
            //    }
            //}

            //writer.Close();
            //form.AddStatus("Finished creating Surface Texture Translation Table.");
        }
    }
}
