using MapAC;
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
            string path = @"C:\ACE\Landblocks\";

            Export.ExportGfxObject(0x01000598, path);

            AddStatus("Export complete.");
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

            var searchFor = 0x01000000;

            // Search through the entries for records matching this type...
            if (DatManager.DatType == DatDatabaseType.Portal)
            {
                foreach (var f in DatManager.CellDat.AllFiles)
                {
                   // try
                    {
                        if (f.Key >= searchFor && f.Key < searchFor + 0x00FFFFFF )
                        {
                            var testFile = DatManager.CellDat.ReadFromDat<GfxObj>(f.Key);

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

        private void getPortalItemsInCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetObjectDetailsFromLandblock(0xc4a7FFFF);
            GetObjectDetailsFromLandblock(0xc4a8FFFF);
            GetObjectDetailsFromLandblock(0xc4a9FFFF);
            GetObjectDetailsFromLandblock(0xc4aaFFFF);
            GetObjectDetailsFromLandblock(0xc4abFFFF);
            GetObjectDetailsFromLandblock(0xc5a7FFFF);
            GetObjectDetailsFromLandblock(0xc5a8FFFF);
            GetObjectDetailsFromLandblock(0xc5a9FFFF);
            GetObjectDetailsFromLandblock(0xc5aaFFFF);
            GetObjectDetailsFromLandblock(0xc5abFFFF);
            GetObjectDetailsFromLandblock(0xc6a7FFFF);
            GetObjectDetailsFromLandblock(0xc6a8FFFF);
            GetObjectDetailsFromLandblock(0xc6a9FFFF);
            GetObjectDetailsFromLandblock(0xc6aaFFFF);
            GetObjectDetailsFromLandblock(0xc6abFFFF);
            GetObjectDetailsFromLandblock(0xc7a7FFFF);
            GetObjectDetailsFromLandblock(0xc7a8FFFF);
            GetObjectDetailsFromLandblock(0xc7a9FFFF);
            GetObjectDetailsFromLandblock(0xc7aaFFFF);
            GetObjectDetailsFromLandblock(0xc7abFFFF);
            GetObjectDetailsFromLandblock(0xc8a7FFFF);
            GetObjectDetailsFromLandblock(0xc8a8FFFF);
            GetObjectDetailsFromLandblock(0xc8a9FFFF);
            GetObjectDetailsFromLandblock(0xc8aaFFFF);
            GetObjectDetailsFromLandblock(0xc8abFFFF);
        }

        private void GetObjectDetailsFromLandblock(uint landblockId)
        {
            AddStatus($"Reading {landblockId:X8}");
            var lbi_id = (landblockId & 0xFFFF0000) + 0xFFFE;
            var landblockInfo = DatManager.CellDat.ReadFromDat<LandblockInfo>(lbi_id);
            for(var i = 0; i < landblockInfo.Objects.Count; i++)
            {
                AddStatus($" - Object {landblockInfo.Objects[i].Id:X8}");
            }

            for (var i = 0; i < landblockInfo.Buildings.Count; i++)
            {
                AddStatus($" - Building {landblockInfo.Buildings[i].ModelId:X8}");
            }

            // Search through EnvCells
            uint baseLb = landblockId >> 24;
            foreach (var e in DatManager.CellDat.AllFiles)
            {
                if ((e.Key >> 24) == baseLb && ((e.Key & 0xFFFF) < 0xFFFE))
                {
                    var envCellId = e.Key;
                    var envCell = DatManager.CellDat.ReadFromDat<EnvCell>(envCellId);
                    AddStatus($" - EnvCell {envCellId:X8} enviroment - {envCell.EnvironmentId:X8}");
                    for (var i = 0; i < envCell.StaticObjects.Count; i++)
                    {
                        AddStatus($" - EnvCell {envCellId:X8} object - {envCell.StaticObjects[i].Id:X8}");
                    }
                }
            }


        }
    }
}
