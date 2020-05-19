using MapAC;
using MapAC.DatLoader;
using MapAC.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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

        private void generateMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show OpenDialog box
            if (openFileDialog_Dat.ShowDialog() == DialogResult.OK) {
                this.UseWaitCursor = true;
                Application.UseWaitCursor = true;
                Application.DoEvents(); // hack to force the cursor update. Cleaner than DoEvents

                ClearMapImage();

                string datFile = openFileDialog_Dat.FileName;
                if (DatManager.Initialize(datFile))
                {
                    AddStatus($"Loaded {datFile}");;

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
                            break;
                        default:
                            AddStatus("Dat file is a PORTAL type file.");
                            break;
                    }
                    AddStatus("-Files " + DatManager.CellDat.AllFiles.Count.ToString());
                    string iteration = DatManager.Iteration;
                    AddStatus("-Iteration " + iteration);

                    // Make sure it's a CELL file
                    if (DatManager.CellDat.Blocksize == 0x100)
                    {
                        Mapper map = new Mapper();
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
                else
                {
                    ClearMapImage();
                    AddStatus($"ERROR loading {datFile}. Probalby not a valid Asheron's Call dat file.");
                }
            }
            this.UseWaitCursor = false;
            Application.UseWaitCursor = false;

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
            textBoxStatus.AppendText(StatusMessage + Environment.NewLine);
            textBoxStatus.Update();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You have the option to ignore this until it is flushed out more...");
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog_Image.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog_Image.FileName, ImageFormat.Png);
                AddStatus($"Map image saved to {saveFileDialog_Image.FileName}");
            }
        }

        private void saveRegionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatReader dr = DatManager.CellDat.GetReaderForFile(0x13000000);
            string thisFile = "C:\\ACE\\13000000.bin";
            System.IO.File.WriteAllBytes(thisFile, dr.Buffer);
        }

        private void contactSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PortalHelper ph = new PortalHelper();
            var contactSheet = ph.BuildIconContactSheet();
            pictureBox1.Image = contactSheet;
        }
    }
}
