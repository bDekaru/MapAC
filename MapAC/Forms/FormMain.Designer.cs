namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDatFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPortalColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpDatContentsToTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testReadDatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getPortalItemsInCellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxStatus = new System.Windows.Forms.RichTextBox();
            this.openFileDialog_Dat = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog_Image = new System.Windows.Forms.SaveFileDialog();
            this.dumpIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(6);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxStatus);
            this.splitContainer1.Size = new System.Drawing.Size(1373, 882);
            this.splitContainer1.SplitterDistance = 682;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 42);
            this.panel1.Margin = new System.Windows.Forms.Padding(6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1373, 640);
            this.panel1.TabIndex = 8;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1373, 640);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(11, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(1373, 42);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDatFileToolStripMenuItem,
            this.loadPortalColorsToolStripMenuItem,
            this.saveMapToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(62, 34);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openDatFileToolStripMenuItem
            // 
            this.openDatFileToolStripMenuItem.Name = "openDatFileToolStripMenuItem";
            this.openDatFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openDatFileToolStripMenuItem.Size = new System.Drawing.Size(348, 40);
            this.openDatFileToolStripMenuItem.Text = "&Open Dat File...";
            this.openDatFileToolStripMenuItem.Click += new System.EventHandler(this.openDatFileToolStripMenuItem_Click);
            // 
            // loadPortalColorsToolStripMenuItem
            // 
            this.loadPortalColorsToolStripMenuItem.Name = "loadPortalColorsToolStripMenuItem";
            this.loadPortalColorsToolStripMenuItem.Size = new System.Drawing.Size(348, 40);
            this.loadPortalColorsToolStripMenuItem.Text = "Load portal colors...";
            this.loadPortalColorsToolStripMenuItem.Click += new System.EventHandler(this.loadPortalColorsToolStripMenuItem_Click);
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Enabled = false;
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(348, 40);
            this.saveMapToolStripMenuItem.Text = "&Save Image";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(348, 40);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.dumpDatContentsToTXTToolStripMenuItem,
            this.testReadDatToolStripMenuItem,
            this.getPortalItemsInCellsToolStripMenuItem,
            this.dumpIconsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(78, 34);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(381, 40);
            this.optionsToolStripMenuItem.Text = "&Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(381, 40);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // dumpDatContentsToTXTToolStripMenuItem
            // 
            this.dumpDatContentsToTXTToolStripMenuItem.Name = "dumpDatContentsToTXTToolStripMenuItem";
            this.dumpDatContentsToTXTToolStripMenuItem.Size = new System.Drawing.Size(381, 40);
            this.dumpDatContentsToTXTToolStripMenuItem.Text = "Dump Dat Contents to TXT";
            this.dumpDatContentsToTXTToolStripMenuItem.Click += new System.EventHandler(this.dumpDatContentsToTXTToolStripMenuItem_Click);
            // 
            // testReadDatToolStripMenuItem
            // 
            this.testReadDatToolStripMenuItem.Name = "testReadDatToolStripMenuItem";
            this.testReadDatToolStripMenuItem.Size = new System.Drawing.Size(381, 40);
            this.testReadDatToolStripMenuItem.Text = "Test Read Dat";
            this.testReadDatToolStripMenuItem.Click += new System.EventHandler(this.testReadDatToolStripMenuItem_Click);
            // 
            // getPortalItemsInCellsToolStripMenuItem
            // 
            this.getPortalItemsInCellsToolStripMenuItem.Name = "getPortalItemsInCellsToolStripMenuItem";
            this.getPortalItemsInCellsToolStripMenuItem.Size = new System.Drawing.Size(381, 40);
            this.getPortalItemsInCellsToolStripMenuItem.Text = "Get Portal Items in Cells";
            this.getPortalItemsInCellsToolStripMenuItem.Click += new System.EventHandler(this.getPortalItemsInCellsToolStripMenuItem_Click);
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxStatus.Location = new System.Drawing.Point(0, 0);
            this.textBoxStatus.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.Size = new System.Drawing.Size(1373, 193);
            this.textBoxStatus.TabIndex = 1;
            this.textBoxStatus.Text = "";
            this.textBoxStatus.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.textBoxStatus_LinkClicked);
            // 
            // openFileDialog_Dat
            // 
            this.openFileDialog_Dat.Filter = "All Dat Files (*.dat)|*.dat|All Files|*.*";
            // 
            // saveFileDialog_Image
            // 
            this.saveFileDialog_Image.Filter = "PNG Files|*.png|All Files|*.*";
            // 
            // dumpIconsToolStripMenuItem
            // 
            this.dumpIconsToolStripMenuItem.Name = "dumpIconsToolStripMenuItem";
            this.dumpIconsToolStripMenuItem.Size = new System.Drawing.Size(381, 40);
            this.dumpIconsToolStripMenuItem.Text = "Dump Icons";
            this.dumpIconsToolStripMenuItem.Click += new System.EventHandler(this.dumpIconsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1373, 882);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "AC Mapper";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog_Dat;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_Image;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem openDatFileToolStripMenuItem;
        private System.Windows.Forms.RichTextBox textBoxStatus;
        private System.Windows.Forms.ToolStripMenuItem loadPortalColorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpDatContentsToTXTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testReadDatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getPortalItemsInCellsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpIconsToolStripMenuItem;
    }
}

