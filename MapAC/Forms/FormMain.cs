﻿using MapAC;
using MapAC.DatLoader;
using MapAC.DatLoader.FileTypes;
using MapAC.Forms;
using MapAC.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Dock = DockStyle.None;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        List<Color> MapColors;
        private void DrawMap()
        {
            // Make sure it's a CELL file
            if (DatManager.CellDat.Blocksize == 0x100)
            {
                Mapper map = new Mapper(MapColors);

                float percLandblocks = (float)(map.FoundLandblocks / (255f * 255f) * 100f);
                AddStatus($"{percLandblocks:0.##}% landblocks in file.");
                if (map.MapImage != null)
                {
                    pictureBox1.Image = map.MapImage;
                    pictureBox1.Width = map.MapImage.Width;
                    pictureBox1.Height = map.MapImage.Height;
                    saveMapToolStripMenuItem.Enabled = true;
                }
            }
            else
            {
                ClearMapImage();
            }
        }

        private void ClearMapImage()
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = null;
                pictureBox1.Update();
            }
        }

        private void AddStatus(string StatusMessage)
        {
            textBoxStatus.AppendText(StatusMessage + System.Environment.NewLine);
            textBoxStatus.SelectionStart = textBoxStatus.Text.Length;
            textBoxStatus.ScrollToCaret();
            textBoxStatus.Update();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // MessageBox.Show("You have the option to ignore this until it is flushed out more...");
            using (FormOptions form = new FormOptions())
            {
                form.ShowDialog();
            }

        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog_Image.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog_Image.FileName, ImageFormat.Png);
                AddStatus($"Map image saved to {saveFileDialog_Image.FileName}");
            }
        }

        private void openDatFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show OpenDialog box
            if (openFileDialog_Dat.ShowDialog() == DialogResult.OK)
            {
                this.UseWaitCursor = true;
                Application.UseWaitCursor = true;
                Application.DoEvents(); // hack to force the cursor update. Cleaner than DoEvents

                if (textBoxStatus.Lines.Length > 0)
                    AddStatus("----------------");

                ClearMapImage();

                string datFile = openFileDialog_Dat.FileName;
                if (DatManager.Initialize(datFile))
                {
                    AddStatus($"Loaded {datFile}"); ;

                    DatManager.ReadDatFile();
                    string statusMessage = "Successfully read dat file. ";
                    switch (DatManager.DatVersion)
                    {
                        case DatVersionType.ACDM:
                            statusMessage += ("Format is \"AC-DM\" era.");
                            break;
                        case DatVersionType.ACTOD:
                            statusMessage += ("Format is \"AC-TOD\" era.");
                            break;
                    }
                    AddStatus(statusMessage);
                    switch (DatManager.CellDat.Blocksize)
                    {
                        case 0x100:
                            //DrawMap();
                            AddStatus("Skipping drawing map while debuggging...");
                            break;
                        default:
                            AddStatus("Dat file is a PORTAL type file.");
                            PortalHelper ph = new PortalHelper();
                            var contactSheet = ph.BuildIconContactSheet();
                            pictureBox1.Image = contactSheet;
                            break;
                    }
                    AddStatus("-Files " + DatManager.CellDat.AllFiles.Count.ToString("N0"));
                    string iteration = DatManager.Iteration;
                    AddStatus("-Iteration " + iteration);

                    var v = new VersionChecker();
                    string version = v.GetVersionInfo(Path.GetFileName(datFile), iteration);
                    if (version != "")
                    {
                        AddStatus($"-File appears to be from {version}.");
                        if (!v.IsComplete(Path.GetFileName(datFile), iteration))
                        {
                            AddStatus("This file is not complete in the Asheron's Call Archive. Please consider uploading it at https://mega.nz/megadrop/0WvIiXRRYmg");
                        }
                    }
                    else
                    {
                        AddStatus("This file does not appear in the Asheron's Call Archive. Please consider uploading it at https://mega.nz/megadrop/0WvIiXRRYmg");
                    }

                }
                else
                {
                    ClearMapImage();
                    AddStatus($"ERROR loading {datFile}. Probalby not a valid Asheron's Call dat file.");
                }
            }
            this.UseWaitCursor = false;
            Application.UseWaitCursor = false;
        }

        private void textBoxStatus_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            var url = e.LinkText;
            if ((!string.IsNullOrWhiteSpace(url)) && (url.ToLower().StartsWith("http")))
            {
                System.Diagnostics.Process.Start(url);
            }
        }

        private void loadPortalColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show OpenDialog box
            if (openFileDialog_Dat.ShowDialog() == DialogResult.OK)
            {
                if (textBoxStatus.Lines.Length > 0)
                    AddStatus("----------------");

                this.UseWaitCursor = true;
                Application.UseWaitCursor = true;
                Application.DoEvents(); // hack to force the cursor update. Cleaner than DoEvents

                string datFile = openFileDialog_Dat.FileName;
                if (DatManager.Initialize(datFile))
                {
                    AddStatus($"Loaded {datFile}"); ;

                    DatManager.ReadDatFile();
                    string statusMessage = "Successfully read dat file. ";
                    switch (DatManager.DatVersion)
                    {
                        case DatVersionType.ACDM:
                            statusMessage += ("Format is \"AC-DM\" era.");
                            break;
                        case DatVersionType.ACTOD:
                            statusMessage += ("Format is \"AC-TOD\" era.");
                            break;
                    }
                    AddStatus(statusMessage);
                    switch (DatManager.CellDat.Blocksize)
                    {
                        case 0x100:
                            AddStatus("File is not a portal. No colors have been loaded.");
                            break;
                        default:
                            PortalHelper ph = new PortalHelper();
                            uint regionId;
                            if (DatManager.DatVersion == DatVersionType.ACDM)
                                regionId = RegionDesc.HW_FILE_ID;
                            else
                                regionId = RegionDesc.FILE_ID;
                            MapColors = ph.GetColors(regionId);
                            ClearMapImage();
                            break;
                    }
                    AddStatus("-Files " + DatManager.CellDat.AllFiles.Count.ToString("N0"));
                    string iteration = DatManager.Iteration;
                    AddStatus("-Iteration " + iteration);

                    var v = new VersionChecker();
                    string version = v.GetVersionInfo(Path.GetFileName(datFile), iteration);
                    if (version != "")
                    {
                        AddStatus($"-File appears to be from {version}.");
                    }
                    else
                    {
                        AddStatus("This file does not appear in the Asheron's Call Archive. Please consider uploading it at https://mega.nz/megadrop/7x-Qh19h5Ek");
                    }

                }
            }
            this.UseWaitCursor = false;
            Application.UseWaitCursor = false;
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs eventArgs)
        {
            string folder = @"C:\ACE\PortalTemp\";
            string fileName;
            DatReader dr;
            uint fileId;
            uint SetupToExport;

            Export.ExportCellLandblock(0xA9B4FFFE, folder, -30, -35);
            AddStatus($"Done Exporting {0xA9B4FFFE:X8}."); ;
            return;

            /*
            uint gfxObjId = 0x01001F61;
            Export.ExportGfxObject(gfxObjId, folder);
            return;
            */
            List<uint> SetupsToExport = new List<uint> { 0x02000CA8 };
            List<uint> WavsToExport = new List<uint> { 0x0A000022, 0x0A000183, 0x0A0003C6, 0x0A000022, 0x0A000183, 0x0A0001FA, 0x0A0001FD, 0x0A00022D, 0x0A0003EE, 0x0A0003F2, 0x0A0003F3, 0x0A00043A, 0x0A00043C, 0x0A000463, };
            /*
            for (var i = 0; i < SetupsToExport.Count; i++)
            {
                SetupToExport = SetupsToExport[i];
                try
                {
                    Export.ExportSetup(SetupToExport, folder);
                    AddStatus($"Done Exporting {SetupToExport:X8} and associated files.");
                }
                catch (Exception ex)
                {
                    AddStatus($"---Error Exporting {SetupToExport:X8} and associated files.");
                }
            }
            */
            for (var i = 0; i < WavsToExport.Count; i++)
            {
                uint WaveToExport = WavsToExport[i];
                try
                {
                    Export.ExportWave(WaveToExport, folder);
                    AddStatus($"Done Exporting {WaveToExport:X8}.");
                }
                catch (Exception ex)
                {
                    AddStatus($"---Error Exporting {WaveToExport:X8} and associated files.");
                }
            }

            return;

            fileId = 0x100002AE;
            var cb = DatManager.CellDat.ReadFromDat<ClothingTable>(fileId);
            dr = DatManager.CellDat.GetReaderForFile(fileId);
            fileName = @"C:\ACE\PortalTemp\" + fileId.ToString("X8") + "- orig.bin";
            File.WriteAllBytes(fileName, dr.Buffer);
            fileName = @"C:\ACE\PortalTemp\" + fileId.ToString("X8") + ".bin";
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                cb.Pack(writer);

            fileId = 0x0f00019b;
            var palSet = DatManager.CellDat.ReadFromDat<PaletteSet>(fileId);
            dr = DatManager.CellDat.GetReaderForFile(fileId);
            fileName = @"C:\ACE\PortalTemp\" + fileId.ToString("X8") + "- orig.bin";
            File.WriteAllBytes(fileName, dr.Buffer);
            fileName = @"C:\ACE\PortalTemp\" + fileId.ToString("X8") + ".bin";
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                palSet.Pack(writer);

            var setup = DatManager.CellDat.ReadFromDat<SetupModel>(0x020009ED);
            fileName = @"C:\ACE\PortalTemp\020009ED.bin";
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                setup.Pack(writer);

            var surfaceThing = DatManager.CellDat.ReadFromDat<Surface>(0x0800047e);
            fileName = @"C:\ACE\PortalTemp\0800047E.bin";
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                surfaceThing.Pack(writer);


            fileId = 0x040010B1;
            var myPal = DatManager.CellDat.ReadFromDat<Palette>(fileId);
            fileName = @"C:\ACE\PortalTemp\" + fileId.ToString("X8") + ".bin";
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                myPal.Pack(writer);

            fileId = 0x010020c2;
            dr = DatManager.CellDat.GetReaderForFile(fileId);
            fileName = @"C:\ACE\PortalTemp\" + fileId.ToString("X8") + "- orig.bin";
            File.WriteAllBytes(fileName, dr.Buffer);

            var gfxObj = DatManager.CellDat.ReadFromDat<GfxObj>(fileId);
            fileName = @"C:\ACE\PortalTemp\" + fileId.ToString("X8") + ".bin";
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                gfxObj.Pack(writer);

            for (var i = 0; i < gfxObj.Surfaces.Count; i++)
            {
                var surfaceId = gfxObj.Surfaces[i];
                var surface = DatManager.CellDat.ReadFromDat<Surface>(surfaceId);
                fileName = @"C:\ACE\PortalTemp\" + surfaceId.ToString("X8") + ".bin";
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    surface.Pack(writer);

                if (surface.OrigPaletteId > 0)
                {
                    var pal = DatManager.CellDat.ReadFromDat<Palette>(surface.OrigPaletteId);
                    fileName = @"C:\ACE\PortalTemp\" + surface.OrigPaletteId.ToString("X8") + ".bin";
                    using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        pal.Pack(writer);

                }

                if (surface.OrigTextureId > 0)
                {
                    var tex = DatManager.CellDat.ReadFromDat<SurfaceTexture>(surface.OrigTextureId);
                    fileName = @"C:\ACE\PortalTemp\" + surface.OrigTextureId.ToString("X8") + "-orig.bin";
                    /*
                    using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        tex.Pack(writer);
                    */
                    dr = DatManager.CellDat.GetReaderForFile(surface.OrigTextureId);
                    File.WriteAllBytes(fileName, dr.Buffer);

                    var bmp = tex.GetBitmap();
                    if (bmp != null)
                    {
                        fileName = @"C:\ACE\PortalTemp\" + surface.OrigTextureId.ToString("X8") + ".png";
                        bmp.Save(fileName, ImageFormat.Png);
                    }
                }

            }

            AddStatus("Done Exporting Files.");
        }

        private void dumpDatContentsToTXTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string iteration = DatManager.Iteration;
            string folder = @"C:\ACE\PortalTemp\";
            string datType = DatManager.DatType.ToString();

            string file = $"{datType}-{iteration}.txt";

            string fileName = Path.Combine(folder, file);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach(var f in DatManager.CellDat.AllFiles)
                {
                    writer.WriteLine(f.Key.ToString("X8"));
                }
            }

            AddStatus("Saved Exported Dat Contents List");
        }

        private void testReadDatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folder = @"C:\ACE\PortalTemp\";

            var searchFor = 0x03000000;

            // Search through the entries for records matching this type...
            if (DatManager.DatType == DatDatabaseType.Portal)
            {
                foreach (var f in DatManager.CellDat.AllFiles)
                {
                   // try
                    {
                        if (f.Key > searchFor && f.Key < searchFor + 0x00FFFFFF )
                        {
                            var testFile = DatManager.CellDat.ReadFromDat<Animation>(f.Key);

                            DatReader dr = DatManager.CellDat.GetReaderForFile(f.Key);

                            using (MemoryStream stream = new MemoryStream())
                            using (BinaryWriter writer = new BinaryWriter(stream))
                            {
                                testFile.Pack(writer);

                                var streamArray = stream.ToArray();
                                var drArray = dr.Buffer.ToArray();
                                if (!streamArray.SequenceEqual(dr.Buffer))
                                {
                                    AddStatus($"Erorr in {f.Key:X8}");
                                    File.WriteAllBytes(folder + "packed_" + f.Key.ToString("X8") + ".bin", streamArray);
                                    File.WriteAllBytes(folder + "orig_" + f.Key.ToString("X8") + ".bin", dr.Buffer);
                                    var test = 1;
                                }
                            }

                        }
                        /*
                    } catch (Exception ex) {
                        //var error = ex.Message
                        AddStatus($"----Error testing {f.Key:X8} -- {ex.Message}");
                        */
                    }
                }
            }
            AddStatus("DONE TEST READ");
        }
    }
}
