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

                            byte[] pattern = new byte[] { 0x39, 0x0C, 0x00, 0x02 };
                            // Searching for 02000C39
                            foreach (var myDatFile in DatManager.CellDat.AllFiles)
                            {
                                DatReader dr = DatManager.CellDat.GetReaderForFile(myDatFile.Key);
                                var test = SearchByte(dr.Buffer, pattern);
                                if (test == true)
                                    AddStatus(" - Found in " + myDatFile.Key.ToString("X8"));
                            }
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
            string path = @"C:\ace\PortalTemp\OldArwic\";
            //Export.ExportPortalFile(0x01000830, path);

            Export.ExportPortalFile(0x0D0002F8, path);
            Export.ExportPortalFile(0x0D0002F9, path);
            Export.ExportPortalFile(0x0D0002FA, path);
            Export.ExportPortalFile(0x0D0002FB, path);
            Export.ExportPortalFile(0x0D00033D, path);
            Export.ExportPortalFile(0x0D000348, path);
            Export.ExportPortalFile(0x0D00034E, path);
            Export.ExportPortalFile(0x0D00034F, path);
            Export.ExportPortalFile(0x0D0003D6, path);
            Export.ExportPortalFile(0x0D0003DE, path);
            Export.ExportPortalFile(0x0D0003F2, path);

            AddStatus("DONE");
            return;

            Export.ExportPortalFile(0x0100022B, path);
            Export.ExportPortalFile(0x0100029A, path);
            Export.ExportPortalFile(0x0100029D, path);
            Export.ExportPortalFile(0x010002CC, path);
            Export.ExportPortalFile(0x010002E9, path);
            Export.ExportPortalFile(0x010002F0, path);
            Export.ExportPortalFile(0x010002FB, path);
            Export.ExportPortalFile(0x01000305, path);
            Export.ExportPortalFile(0x01000307, path);
            Export.ExportPortalFile(0x0100048C, path);
            Export.ExportPortalFile(0x01000567, path);
            Export.ExportPortalFile(0x0100058F, path);
            Export.ExportPortalFile(0x01000592, path);
            Export.ExportPortalFile(0x01000598, path);
            Export.ExportPortalFile(0x01000647, path);
            Export.ExportPortalFile(0x0100065D, path);
            Export.ExportPortalFile(0x0100069A, path);
            Export.ExportPortalFile(0x010006B7, path);
            Export.ExportPortalFile(0x010007C4, path);
            Export.ExportPortalFile(0x010007C8, path);
            Export.ExportPortalFile(0x0100081A, path);
            Export.ExportPortalFile(0x0100081C, path);
            Export.ExportPortalFile(0x01000827, path);
            Export.ExportPortalFile(0x0100082E, path);
            Export.ExportPortalFile(0x01000830, path);
            Export.ExportPortalFile(0x0100093B, path);
            Export.ExportPortalFile(0x01000A2B, path);
            Export.ExportPortalFile(0x01000A2F, path);
            Export.ExportPortalFile(0x01000A44, path);
            Export.ExportPortalFile(0x01000A6A, path);
            Export.ExportPortalFile(0x01000AB8, path);
            Export.ExportPortalFile(0x01000ADA, path);
            Export.ExportPortalFile(0x01000AEF, path);
            Export.ExportPortalFile(0x01000B53, path);
            Export.ExportPortalFile(0x01000BC3, path);
            Export.ExportPortalFile(0x01000BC6, path);
            Export.ExportPortalFile(0x01000BC7, path);
            Export.ExportPortalFile(0x01000BC8, path);
            Export.ExportPortalFile(0x01000BC9, path);
            Export.ExportPortalFile(0x01000C00, path);
            Export.ExportPortalFile(0x01000C16, path);
            Export.ExportPortalFile(0x01000C17, path);
            Export.ExportPortalFile(0x01000C1E, path);
            Export.ExportPortalFile(0x01000C20, path);
            Export.ExportPortalFile(0x01000DBE, path);
            Export.ExportPortalFile(0x01000DF0, path);
            Export.ExportPortalFile(0x01000E2A, path);
            Export.ExportPortalFile(0x01000E5F, path);
            Export.ExportPortalFile(0x01000FDE, path);
            Export.ExportPortalFile(0x0100104C, path);
            Export.ExportPortalFile(0x01001050, path);
            Export.ExportPortalFile(0x0100113C, path);
            Export.ExportPortalFile(0x010011C4, path);
            Export.ExportPortalFile(0x010017B7, path);
            Export.ExportPortalFile(0x010017F6, path);
            Export.ExportPortalFile(0x0200004F, path);
            Export.ExportPortalFile(0x02000065, path);
            Export.ExportPortalFile(0x02000081, path);
            Export.ExportPortalFile(0x020000A4, path);
            Export.ExportPortalFile(0x020000A5, path);
            Export.ExportPortalFile(0x020000A6, path);
            Export.ExportPortalFile(0x020000A7, path);
            Export.ExportPortalFile(0x020000A8, path);
            Export.ExportPortalFile(0x020000AA, path);
            Export.ExportPortalFile(0x020000AB, path);
            Export.ExportPortalFile(0x020000AC, path);
            Export.ExportPortalFile(0x020000E6, path);
            Export.ExportPortalFile(0x020000E7, path);
            Export.ExportPortalFile(0x020000E9, path);
            Export.ExportPortalFile(0x020000EA, path);
            Export.ExportPortalFile(0x020000EB, path);
            Export.ExportPortalFile(0x020000EC, path);
            Export.ExportPortalFile(0x020000ED, path);
            Export.ExportPortalFile(0x020000F0, path);
            Export.ExportPortalFile(0x020000F2, path);
            Export.ExportPortalFile(0x020000F3, path);
            Export.ExportPortalFile(0x020000F5, path);
            Export.ExportPortalFile(0x020000F6, path);
            Export.ExportPortalFile(0x020000F8, path);
            Export.ExportPortalFile(0x020000FB, path);
            Export.ExportPortalFile(0x020000FD, path);
            Export.ExportPortalFile(0x02000102, path);
            Export.ExportPortalFile(0x02000103, path);
            Export.ExportPortalFile(0x02000105, path);
            Export.ExportPortalFile(0x02000106, path);
            Export.ExportPortalFile(0x02000107, path);
            Export.ExportPortalFile(0x0200010A, path);
            Export.ExportPortalFile(0x02000110, path);
            Export.ExportPortalFile(0x02000111, path);
            Export.ExportPortalFile(0x0200011E, path);
            Export.ExportPortalFile(0x0200011F, path);
            Export.ExportPortalFile(0x02000120, path);
            Export.ExportPortalFile(0x02000121, path);
            Export.ExportPortalFile(0x02000123, path);
            Export.ExportPortalFile(0x02000124, path);
            Export.ExportPortalFile(0x02000125, path);
            Export.ExportPortalFile(0x02000126, path);
            Export.ExportPortalFile(0x02000127, path);
            Export.ExportPortalFile(0x02000128, path);
            Export.ExportPortalFile(0x02000129, path);
            Export.ExportPortalFile(0x0200012A, path);
            Export.ExportPortalFile(0x0200012F, path);
            Export.ExportPortalFile(0x02000131, path);
            Export.ExportPortalFile(0x02000132, path);
            Export.ExportPortalFile(0x02000134, path);
            Export.ExportPortalFile(0x02000136, path);
            Export.ExportPortalFile(0x02000139, path);
            Export.ExportPortalFile(0x0200013A, path);
            Export.ExportPortalFile(0x0200013B, path);
            Export.ExportPortalFile(0x0200013C, path);
            Export.ExportPortalFile(0x02000140, path);
            Export.ExportPortalFile(0x02000141, path);
            Export.ExportPortalFile(0x02000146, path);
            Export.ExportPortalFile(0x02000148, path);
            Export.ExportPortalFile(0x0200014B, path);
            Export.ExportPortalFile(0x02000150, path);
            Export.ExportPortalFile(0x02000153, path);
            Export.ExportPortalFile(0x02000154, path);
            Export.ExportPortalFile(0x02000162, path);
            Export.ExportPortalFile(0x02000164, path);
            Export.ExportPortalFile(0x0200016C, path);
            Export.ExportPortalFile(0x0200016D, path);
            Export.ExportPortalFile(0x0200016E, path);
            Export.ExportPortalFile(0x0200016F, path);
            Export.ExportPortalFile(0x02000175, path);
            Export.ExportPortalFile(0x02000176, path);
            Export.ExportPortalFile(0x02000177, path);
            Export.ExportPortalFile(0x0200017D, path);
            Export.ExportPortalFile(0x02000180, path);
            Export.ExportPortalFile(0x02000183, path);
            Export.ExportPortalFile(0x02000185, path);
            Export.ExportPortalFile(0x02000186, path);
            Export.ExportPortalFile(0x02000188, path);
            Export.ExportPortalFile(0x0200018E, path);
            Export.ExportPortalFile(0x0200018F, path);
            Export.ExportPortalFile(0x02000190, path);
            Export.ExportPortalFile(0x020001B4, path);
            Export.ExportPortalFile(0x020001BC, path);
            Export.ExportPortalFile(0x020001BD, path);
            Export.ExportPortalFile(0x020001BE, path);
            Export.ExportPortalFile(0x020001BF, path);
            Export.ExportPortalFile(0x020001C0, path);
            Export.ExportPortalFile(0x020001C7, path);
            Export.ExportPortalFile(0x020001DB, path);
            Export.ExportPortalFile(0x020001E5, path);
            Export.ExportPortalFile(0x020001F1, path);
            Export.ExportPortalFile(0x0200020E, path);
            Export.ExportPortalFile(0x02000211, path);
            Export.ExportPortalFile(0x02000227, path);
            Export.ExportPortalFile(0x0200022C, path);
            Export.ExportPortalFile(0x0200022D, path);
            Export.ExportPortalFile(0x02000240, path);
            Export.ExportPortalFile(0x02000248, path);
            Export.ExportPortalFile(0x0200024B, path);
            Export.ExportPortalFile(0x0200024C, path);
            Export.ExportPortalFile(0x02000253, path);
            Export.ExportPortalFile(0x02000254, path);
            Export.ExportPortalFile(0x02000266, path);
            Export.ExportPortalFile(0x02000267, path);
            Export.ExportPortalFile(0x02000268, path);
            Export.ExportPortalFile(0x02000270, path);
            Export.ExportPortalFile(0x02000272, path);
            Export.ExportPortalFile(0x02000278, path);
            Export.ExportPortalFile(0x02000279, path);
            Export.ExportPortalFile(0x0200027E, path);
            Export.ExportPortalFile(0x02000294, path);
            Export.ExportPortalFile(0x02000295, path);
            Export.ExportPortalFile(0x020002D7, path);
            Export.ExportPortalFile(0x020002F2, path);
            Export.ExportPortalFile(0x020002F9, path);
            Export.ExportPortalFile(0x020002FA, path);
            Export.ExportPortalFile(0x020002FB, path);
            Export.ExportPortalFile(0x020002FC, path);
            Export.ExportPortalFile(0x020002FD, path);
            Export.ExportPortalFile(0x02000303, path);
            Export.ExportPortalFile(0x02000309, path);
            Export.ExportPortalFile(0x0200030B, path);
            Export.ExportPortalFile(0x02000339, path);
            Export.ExportPortalFile(0x02000351, path);
            Export.ExportPortalFile(0x02000354, path);
            Export.ExportPortalFile(0x02000362, path);
            Export.ExportPortalFile(0x02000363, path);
            Export.ExportPortalFile(0x02000374, path);
            Export.ExportPortalFile(0x02000375, path);
            Export.ExportPortalFile(0x02000377, path);
            Export.ExportPortalFile(0x0200037A, path);
            Export.ExportPortalFile(0x0200037C, path);
            Export.ExportPortalFile(0x020003B4, path);
            Export.ExportPortalFile(0x020003B5, path);
            Export.ExportPortalFile(0x020003BA, path);
            Export.ExportPortalFile(0x020003C4, path);
            Export.ExportPortalFile(0x020003C7, path);
            Export.ExportPortalFile(0x020003CC, path);
            Export.ExportPortalFile(0x020003CD, path);
            Export.ExportPortalFile(0x020003DE, path);
            Export.ExportPortalFile(0x020003DF, path);
            Export.ExportPortalFile(0x020003E0, path);
            Export.ExportPortalFile(0x020003EC, path);
            Export.ExportPortalFile(0x020003F5, path);
            Export.ExportPortalFile(0x020003FF, path);
            Export.ExportPortalFile(0x0200043B, path);
            Export.ExportPortalFile(0x02000469, path);
            Export.ExportPortalFile(0x02000485, path);
            Export.ExportPortalFile(0x020004B9, path);
            Export.ExportPortalFile(0x020004C8, path);
            Export.ExportPortalFile(0x020004C9, path);
            Export.ExportPortalFile(0x020004CA, path);
            Export.ExportPortalFile(0x020005A8, path);
            Export.ExportPortalFile(0x020005AE, path);
            Export.ExportPortalFile(0x020005B3, path);
            Export.ExportPortalFile(0x020005BB, path);
            Export.ExportPortalFile(0x020005C0, path);
            Export.ExportPortalFile(0x020005C3, path);
            Export.ExportPortalFile(0x020005C9, path);
            Export.ExportPortalFile(0x0200067C, path);
            Export.ExportPortalFile(0x0200068C, path);
            Export.ExportPortalFile(0x020006A1, path);
            Export.ExportPortalFile(0x020006CC, path);
            Export.ExportPortalFile(0x08000077, path);
            Export.ExportPortalFile(0x080000CB, path);
            Export.ExportPortalFile(0x080002D8, path);
            Export.ExportPortalFile(0x0800047E, path);
            Export.ExportPortalFile(0x08000533, path);
            Export.ExportPortalFile(0x0800054C, path);
            Export.ExportPortalFile(0x08000551, path);
            Export.ExportPortalFile(0x08000554, path);
            Export.ExportPortalFile(0x08000555, path);
            Export.ExportPortalFile(0x08000557, path);
            Export.ExportPortalFile(0x0800055B, path);
            Export.ExportPortalFile(0x08000585, path);
            Export.ExportPortalFile(0x0800058B, path);
            Export.ExportPortalFile(0x080005EC, path);
            Export.ExportPortalFile(0x08000602, path);
            Export.ExportPortalFile(0x08000607, path);
            Export.ExportPortalFile(0x08000623, path);
            Export.ExportPortalFile(0x0800062E, path);
            Export.ExportPortalFile(0x0800062F, path);
            Export.ExportPortalFile(0x08000630, path);
            Export.ExportPortalFile(0x08000658, path);
            Export.ExportPortalFile(0x0800065A, path);
            Export.ExportPortalFile(0x08000660, path);
            Export.ExportPortalFile(0x08000661, path);
            Export.ExportPortalFile(0x0800068F, path);
            Export.ExportPortalFile(0x08000691, path);
            Export.ExportPortalFile(0x0800069B, path);
            Export.ExportPortalFile(0x080007F9, path);
            Export.ExportPortalFile(0x080007FC, path);
            Export.ExportPortalFile(0x080007FE, path);
            Export.ExportPortalFile(0x0800080C, path);
            Export.ExportPortalFile(0x0800080D, path);
            Export.ExportPortalFile(0x0800080E, path);
            Export.ExportPortalFile(0x08000810, path);
            Export.ExportPortalFile(0x08000812, path);
            Export.ExportPortalFile(0x08000825, path);
            Export.ExportPortalFile(0x0800083E, path);
            Export.ExportPortalFile(0x0800085E, path);
            Export.ExportPortalFile(0x080008C4, path);
            Export.ExportPortalFile(0x080008C6, path);
            Export.ExportPortalFile(0x08000974, path);
            Export.ExportPortalFile(0x080009C4, path);
            Export.ExportPortalFile(0x080009C5, path);
            Export.ExportPortalFile(0x080009C6, path);
            Export.ExportPortalFile(0x080009EB, path);
            Export.ExportPortalFile(0x08000A25, path);
            Export.ExportPortalFile(0x08000A2B, path);
            Export.ExportPortalFile(0x08000A2C, path);
            Export.ExportPortalFile(0x08000AC1, path);
            Export.ExportPortalFile(0x08000AD3, path);
            Export.ExportPortalFile(0x08000AD4, path);
            Export.ExportPortalFile(0x08000AD5, path);
            Export.ExportPortalFile(0x08000B73, path);
            Export.ExportPortalFile(0x08000C17, path);
            Export.ExportPortalFile(0x08000C37, path);
            Export.ExportPortalFile(0x08000C38, path);
            Export.ExportPortalFile(0x08000C3B, path);
            Export.ExportPortalFile(0x08000C3C, path);
            Export.ExportPortalFile(0x08000C3D, path);
            Export.ExportPortalFile(0x08000C3E, path);
            Export.ExportPortalFile(0x08000C3F, path);
            Export.ExportPortalFile(0x08000C51, path);
            Export.ExportPortalFile(0x08000C52, path);
            Export.ExportPortalFile(0x08000C57, path);
            Export.ExportPortalFile(0x08000C58, path);
            Export.ExportPortalFile(0x08000D27, path);
            Export.ExportPortalFile(0x08000D2B, path);
            Export.ExportPortalFile(0x08000D53, path);
            Export.ExportPortalFile(0x08000D54, path);
            Export.ExportPortalFile(0x08000D5B, path);
            Export.ExportPortalFile(0x08000D5C, path);
            Export.ExportPortalFile(0x08000DA2, path);
            Export.ExportPortalFile(0x08000DB9, path);
            Export.ExportPortalFile(0x08000DBB, path);
            Export.ExportPortalFile(0x08000DEA, path);
            Export.ExportPortalFile(0x08000DED, path);
            Export.ExportPortalFile(0x08000DEE, path);
            Export.ExportPortalFile(0x08000DF3, path);
            Export.ExportPortalFile(0x08000DF8, path);
            Export.ExportPortalFile(0x08000DF9, path);
            Export.ExportPortalFile(0x08000E02, path);
            Export.ExportPortalFile(0x08000E4C, path);
            Export.ExportPortalFile(0x08000E4E, path);
            Export.ExportPortalFile(0x08000E52, path);
            Export.ExportPortalFile(0x08000E53, path);
            Export.ExportPortalFile(0x08000E54, path);
            Export.ExportPortalFile(0x08000E55, path);
            Export.ExportPortalFile(0x08000EBC, path);
            Export.ExportPortalFile(0x0800102E, path);
            Export.ExportPortalFile(0x08001079, path);
            Export.ExportPortalFile(0x0800107A, path);
            Export.ExportPortalFile(0x0D0002F8, path);
            Export.ExportPortalFile(0x0D0002F9, path);
            Export.ExportPortalFile(0x0D0002FA, path);
            Export.ExportPortalFile(0x0D0002FB, path);
            Export.ExportPortalFile(0x0D000333, path);
            Export.ExportPortalFile(0x0D00033D, path);
            Export.ExportPortalFile(0x0D000348, path);
            Export.ExportPortalFile(0x0D00034E, path);
            Export.ExportPortalFile(0x0D00034F, path);
            Export.ExportPortalFile(0x0D000366, path);
            Export.ExportPortalFile(0x0D00036A, path);
            Export.ExportPortalFile(0x0D0003D3, path);
            Export.ExportPortalFile(0x0D0003D4, path);
            Export.ExportPortalFile(0x0D0003D6, path);
            Export.ExportPortalFile(0x0D0003DE, path);
            Export.ExportPortalFile(0x0D0003DF, path);
            Export.ExportPortalFile(0x0D0003E2, path);
            Export.ExportPortalFile(0x0D0003E3, path);
            Export.ExportPortalFile(0x0D0003E4, path);
            Export.ExportPortalFile(0x0D0003F2, path);
            Export.ExportPortalFile(0x0D0003F4, path);
            Export.ExportPortalFile(0x0D0003F5, path);
            Export.ExportPortalFile(0x0D000402, path);
            Export.ExportPortalFile(0x0D000405, path);

            // Export.ExportPortalFile(0x020002EE, path); // Lifestone
            //Export.ExportPortalFile(0x02000002, path); // Tusker
            //Export.ExportPortalFile(0x02000055, path); // OG GOlems
            //Export.ExportPortalFile(0x0200049C, path); // Early Chittick
            //Export.ExportPortalFile(0x02000980, path); // Burrowing Shadow Spire
            //Export.ExportPortalFile(0x01000FDE, path); // clear_cell.id = 0x01000FDE, defined in RegionDesc
            //Export.ExportPortalFile(0x02000034, path); // DRUDGE

            //Export.ExportPortalFile(0x020009ED, path); // TARDIS
            //Export.ExportPortalFile(0x06001F99, path); // QuestionMark ICON
            //Export.ExportGfxObject(0x01000598, path);
            AddStatus("Export complete.");
            return;
            List<uint> PortalList = new List<uint>();
            PortalList.Add(0x31000020);
            PortalList.Add(0x31000022);
            PortalList.Add(0x32000028);
            PortalList.Add(0x3200011D);
            PortalList.Add(0x3200011F);
            PortalList.Add(0x320003C1);
            PortalList.Add(0x320003DC);
            PortalList.Add(0x32000439);
            PortalList.Add(0x3200045E);
            PortalList.Add(0x3200045F);
            PortalList.Add(0x32000460);
            PortalList.Add(0x32000475);
            PortalList.Add(0x32000498);
            PortalList.Add(0x32000499);
            PortalList.Add(0x320004B4);
            PortalList.Add(0x3200050B);
            PortalList.Add(0x3200050E);
            PortalList.Add(0x3200050F);
            PortalList.Add(0x32000514);
            PortalList.Add(0x32000515);
            PortalList.Add(0x32000516);
            PortalList.Add(0x32000517);
            PortalList.Add(0x32000519);
            PortalList.Add(0x3200051A);
            PortalList.Add(0x32000538);
            PortalList.Add(0x32000539);
            PortalList.Add(0x3200053B);
            PortalList.Add(0x32000552);
            PortalList.Add(0x320005C0);
            PortalList.Add(0x32000611);
            PortalList.Add(0x32000641);
            PortalList.Add(0x320006CC);
            PortalList.Add(0x320006CD);
            PortalList.Add(0x320006CE);
            PortalList.Add(0x320006CF);
            PortalList.Add(0x320007BE);
            PortalList.Add(0x320007BF);
            PortalList.Add(0x3300001E);
            PortalList.Add(0x3300072E);
            PortalList.Add(0x330007D2);
            PortalList.Add(0x3300081C);
            PortalList.Add(0x33000842);
            PortalList.Add(0x3300085E);
            PortalList.Add(0x3300085F);
            PortalList.Add(0x330008A2);
            PortalList.Add(0x33000932);
            PortalList.Add(0x33000935);
            PortalList.Add(0x33000936);
            PortalList.Add(0x33000946);
            PortalList.Add(0x33000948);
            PortalList.Add(0x33000949);
            PortalList.Add(0x3300094A);
            PortalList.Add(0x3300094D);
            PortalList.Add(0x330009FE);
            PortalList.Add(0x33000A3E);
            PortalList.Add(0x33000A84);
            PortalList.Add(0x33000A86);
            PortalList.Add(0x33000BB6);
            PortalList.Add(0x34000078);
            PortalList.Add(0x3400008A);
            PortalList.Add(0x340000A7);
            PortalList.Add(0x3300072D);
            PortalList.Add(0x3300094C);
            PortalList.Add(0x04000E60);
            PortalList.Add(0x040013B2);
            PortalList.Add(0x0500213E);
            PortalList.Add(0x06001990);
            PortalList.Add(0x08001C21);
            PortalList.Add(0x1100120D);
            PortalList.Add(0x110024E8);

            ExportPortalItems(PortalList, path);
            AddStatus("Export complete.");
        }

        private void ExportPortalItems(List<uint> items, string path)
        {
            foreach(var item in items)
            {
                var result = Export.ExportPortalFile(item, path);
                if (result == false)
                    AddStatus($"Error export {item:X8} -- Not Found");
            }
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

        // Key is ObjectId, Value is count
        Dictionary<uint, uint> ObjectsInLandblocks;
        private void getPortalItemsInCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectsInLandblocks = new Dictionary<uint, uint>();
            GetObjectDetailsFromLandblock(0xc6a9FFFF);


            var list = ObjectsInLandblocks.Keys.ToList();
            list.Sort();
            foreach(var objectId in list)
            {
                AddStatus($"0x{objectId:X8}");
            }
            AddStatus("DONE");
        }

        private void GetObjectDetailsFromLandblock(uint landblockId)
        {
            AddStatus($"Reading {landblockId:X8}");
            var lbi_id = (landblockId & 0xFFFF0000) + 0xFFFE;
            var landblockInfo = DatManager.CellDat.ReadFromDat<LandblockInfo>(lbi_id);
            for(var i = 0; i < landblockInfo.Objects.Count; i++)
            {
                AddLandblockItem(landblockInfo.Objects[i].Id);
                //AddStatus($" - Object {landblockInfo.Objects[i].Id:X8}");
            }

            for (var i = 0; i < landblockInfo.Buildings.Count; i++)
            {
                AddLandblockItem(landblockInfo.Buildings[i].ModelId);
                //AddStatus($" - Building {landblockInfo.Buildings[i].ModelId:X8}");
            }

            // Search through EnvCells
            uint baseLb = landblockId >> 24;
            foreach (var e in DatManager.CellDat.AllFiles)
            {
                if ((e.Key >> 24) == baseLb && ((e.Key & 0xFFFF) < 0xFFFE))
                {
                    var envCellId = e.Key;
                    var envCell = DatManager.CellDat.ReadFromDat<EnvCell>(envCellId);

                    AddLandblockItem(envCell.EnvironmentId);

                    for(var i =0; i < envCell.Surfaces.Count; i++)
                    {
                        AddLandblockItem(envCell.Surfaces[i]);
                    }
                    //AddStatus($" - EnvCell {envCellId:X8} enviroment - {envCell.EnvironmentId:X8}");
                    for (var i = 0; i < envCell.StaticObjects.Count; i++)
                    {
                        AddLandblockItem(envCell.StaticObjects[i].Id);
                        //AddStatus($" - EnvCell {envCellId:X8} object - {envCell.StaticObjects[i].Id:X8}");
                    }
                }
            }


        }

        private void AddLandblockItem(uint objectId)
        {
            if (ObjectsInLandblocks.ContainsKey(objectId))
                ObjectsInLandblocks[objectId]++;
            else
                ObjectsInLandblocks.Add(objectId, 1);
        }

        private void dumpIconsToolStripMenuItem_Click(object sender, EventArgs evt)
        {
            foreach (var e in DatManager.CellDat.AllFiles)
            {
                if (e.Key > 0x06000000 && e.Key < 0x06FFFFFF)
                {
                    var texId = e.Key;
                    var texture = DatManager.CellDat.ReadFromDat<Texture>(texId);
                    if(texture.Height >= 240 && texture.Width >= 320)
                        texture.ExportTexture(@"C:\ACE\Icons\");
                }
            }
        }

        private void exportCellDatStuffsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int offsetX = 41;
            int offsetY = 63;
            string path = @"C:\ace\Landblocks\OldArwic\";
            //Export.ExportLandblockInfo(0xc6a9FFFE, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc6a9FFFF, path, offsetX, offsetY);
            AddStatus("DONE");
            return;
            Export.ExportCellLandblock(0xc4a7FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc4a8FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc4a9FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc4aaFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc4abFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc5a7FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc5a8FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc5a9FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc5aaFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc5abFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc6a7FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc6a8FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc6a9FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc6aaFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc6abFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc7a7FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc7a8FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc7a9FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc7aaFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc7abFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc8a7FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc8a8FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc8a9FFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc8aaFFFF, path, offsetX, offsetY);
            Export.ExportCellLandblock(0xc8abFFFF, path, offsetX, offsetY);
            AddStatus("Export Complete");
            return;
        }

        bool SearchByte(byte[] src, byte[] pattern)
        {
            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = 0; i < c; i++)
            {
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return true;
            }
            return false;
        }

        private void exportImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string exportDir = "D:\\ACE\\portal_beta_0\\images\\";

            if (DatManager.CellDat.AllFiles.ContainsKey(0x06000261))
            {
                var image = DatManager.CellDat.ReadFromDat<Texture>(0x06000261);
                image.ExportTexture(exportDir);
                AddStatus("0x06000261 Complete");
            }
            if (DatManager.CellDat.AllFiles.ContainsKey(0x06000086))
            {
                var image = DatManager.CellDat.ReadFromDat<Texture>(0x06000086);
                image.ExportTexture(exportDir);
                AddStatus("0x06000086 Complete");
            }
            //return;

            foreach (KeyValuePair<uint, DatFile> entry in DatManager.CellDat.AllFiles)
            {
                if (entry.Value.GetFileType(DatDatabaseType.Portal) == DatFileType.Texture)
                {
                    var image = DatManager.CellDat.ReadFromDat<Texture>(entry.Value.ObjectId);
                    //image.ExportTexture(exportDir);
                }
                if(entry.Value.GetFileType(DatDatabaseType.Portal) == DatFileType.SurfaceTexture && DatManager.DatVersion == DatVersionType.ACDM) {
                    var image = DatManager.CellDat.ReadFromDat<SurfaceTexture>(entry.Value.ObjectId);
                    image.ExportTexture(exportDir);
                }
            }
            AddStatus("Export Complete");
        }

        private void exportFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatManager.CellDat.ExtractCategorizedPortalContents("D:\\ACE\\portal_beta_0\\");
            AddStatus("Export Complete");
        }
    }
}
