namespace MapAC.Forms
{
    partial class FormOptions
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOptions));
            this.chkMapColor = new System.Windows.Forms.CheckBox();
            this.picMapSample = new System.Windows.Forms.PictureBox();
            this.imageListMapSamples = new System.Windows.Forms.ImageList(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnColorPick = new System.Windows.Forms.Button();
            this.chkSnow = new System.Windows.Forms.CheckBox();
            this.chkPortalRegionColor = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picMapSample)).BeginInit();
            this.SuspendLayout();
            // 
            // chkMapColor
            // 
            this.chkMapColor.AutoSize = true;
            this.chkMapColor.Location = new System.Drawing.Point(34, 36);
            this.chkMapColor.Name = "chkMapColor";
            this.chkMapColor.Size = new System.Drawing.Size(267, 29);
            this.chkMapColor.TabIndex = 1;
            this.chkMapColor.Text = "Use original colors for map";
            this.chkMapColor.UseVisualStyleBackColor = true;
            this.chkMapColor.CheckedChanged += new System.EventHandler(this.chkMapColor_CheckedChanged);
            // 
            // picMapSample
            // 
            this.picMapSample.Location = new System.Drawing.Point(461, 25);
            this.picMapSample.Name = "picMapSample";
            this.picMapSample.Size = new System.Drawing.Size(256, 256);
            this.picMapSample.TabIndex = 2;
            this.picMapSample.TabStop = false;
            // 
            // imageListMapSamples
            // 
            this.imageListMapSamples.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMapSamples.ImageStream")));
            this.imageListMapSamples.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMapSamples.Images.SetKeyName(0, "map-tod_small.png");
            this.imageListMapSamples.Images.SetKeyName(1, "map-acdm_small.png");
            this.imageListMapSamples.Images.SetKeyName(2, "map-tod-snow_small.png");
            this.imageListMapSamples.Images.SetKeyName(3, "map-acdm-snow_small.png");
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(442, 377);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(136, 42);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(584, 377);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(136, 42);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // colorDialog1
            // 
            this.colorDialog1.AnyColor = true;
            // 
            // pnlColor
            // 
            this.pnlColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColor.Location = new System.Drawing.Point(51, 284);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(67, 50);
            this.pnlColor.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 256);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Color for missing landblocks";
            // 
            // btnColorPick
            // 
            this.btnColorPick.Location = new System.Drawing.Point(124, 289);
            this.btnColorPick.Name = "btnColorPick";
            this.btnColorPick.Size = new System.Drawing.Size(44, 45);
            this.btnColorPick.TabIndex = 7;
            this.btnColorPick.Text = "...";
            this.btnColorPick.UseVisualStyleBackColor = true;
            this.btnColorPick.Click += new System.EventHandler(this.btnColorPick_Click);
            // 
            // chkSnow
            // 
            this.chkSnow.AutoSize = true;
            this.chkSnow.Location = new System.Drawing.Point(34, 71);
            this.chkSnow.Name = "chkSnow";
            this.chkSnow.Size = new System.Drawing.Size(172, 29);
            this.chkSnow.TabIndex = 8;
            this.chkSnow.Text = "Use winter map";
            this.chkSnow.UseVisualStyleBackColor = true;
            this.chkSnow.CheckedChanged += new System.EventHandler(this.chkMapColor_CheckedChanged);
            // 
            // chkPortalRegionColor
            // 
            this.chkPortalRegionColor.AutoSize = true;
            this.chkPortalRegionColor.Location = new System.Drawing.Point(33, 166);
            this.chkPortalRegionColor.Name = "chkPortalRegionColor";
            this.chkPortalRegionColor.Size = new System.Drawing.Size(344, 29);
            this.chkPortalRegionColor.TabIndex = 9;
            this.chkPortalRegionColor.Text = "Use color from last loaded portal file";
            this.chkPortalRegionColor.UseVisualStyleBackColor = true;
            // 
            // FormOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 450);
            this.Controls.Add(this.chkPortalRegionColor);
            this.Controls.Add(this.chkSnow);
            this.Controls.Add(this.btnColorPick);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlColor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.picMapSample);
            this.Controls.Add(this.chkMapColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.picMapSample)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkMapColor;
        private System.Windows.Forms.PictureBox picMapSample;
        private System.Windows.Forms.ImageList imageListMapSamples;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel pnlColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnColorPick;
        private System.Windows.Forms.CheckBox chkSnow;
        private System.Windows.Forms.CheckBox chkPortalRegionColor;
    }
}