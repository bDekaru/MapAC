using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapAC.Forms
{
    public partial class FormOptions : Form
    {
        public FormOptions()
        {
            InitializeComponent();

            // Set the current options on the form
            LoadOptions();
        }

        private void LoadOptions()
        {
            switch (Properties.Settings.Default.ACDM_MapColor)
            {
                case true:
                    chkMapColor.Checked = true;
                    picMapSample.Image = imageListMapSamples.Images[1];
                    break;
                default:
                    chkMapColor.Checked = false;
                    picMapSample.Image = imageListMapSamples.Images[0];
                    break;
            }
        }

        private void SaveOptions()
        {
            if (chkMapColor.Checked)
                Properties.Settings.Default.ACDM_MapColor = true;
            else
                Properties.Settings.Default.ACDM_MapColor = false;

            // Commit all options
            Properties.Settings.Default.Save();
        }

        private void chkMapColor_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMapColor.Checked)
                picMapSample.Image = imageListMapSamples.Images[1];
            else
                picMapSample.Image = imageListMapSamples.Images[0];
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveOptions();
            Close();
        }
    }
}
