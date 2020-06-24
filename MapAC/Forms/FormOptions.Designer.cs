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
            this.chkPortalRegionColor = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picMapSample)).BeginInit();
            this.SuspendLayout();
            // 
            // chkMapColor
            // 
            this.chkMapColor.AutoSize = true;
            this.chkMapColor.Location = new System.Drawing.Point(19, 20);
            this.chkMapColor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkMapColor.Name = "chkMapColor";
            this.chkMapColor.Size = new System.Drawing.Size(150, 17);
            this.chkMapColor.TabIndex = 1;
            this.chkMapColor.Text = "Use original colors for map";
            this.chkMapColor.UseVisualStyleBackColor = true;
            this.chkMapColor.CheckedChanged += new System.EventHandler(this.chkMapColor_CheckedChanged);
            // 
            // picMapSample
            // 
            this.picMapSample.Location = new System.Drawing.Point(251, 14);
            this.picMapSample.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.picMapSample.Name = "picMapSample";
            this.picMapSample.Size = new System.Drawing.Size(140, 139);
            this.picMapSample.TabIndex = 2;
            this.picMapSample.TabStop = false;
            // 
            // imageListMapSamples
            // 
            this.imageListMapSamples.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMapSamples.ImageStream")));
            this.imageListMapSamples.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMapSamples.Images.SetKeyName(0, "map-tod_small.png");
            this.imageListMapSamples.Images.SetKeyName(1, "map-acdm_small.png");
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(241, 204);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(74, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(319, 204);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 23);
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
            this.pnlColor.Location = new System.Drawing.Point(28, 154);
            this.pnlColor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(37, 28);
            this.pnlColor.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 139);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Color for missing landblocks";
            // 
            // btnColorPick
            // 
            this.btnColorPick.Location = new System.Drawing.Point(68, 157);
            this.btnColorPick.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnColorPick.Name = "btnColorPick";
            this.btnColorPick.Size = new System.Drawing.Size(24, 24);
            this.btnColorPick.TabIndex = 7;
            this.btnColorPick.Text = "...";
            this.btnColorPick.UseVisualStyleBackColor = true;
            this.btnColorPick.Click += new System.EventHandler(this.btnColorPick_Click);
            // 
            // chkPortalRegionColor
            // 
            this.chkPortalRegionColor.AutoSize = true;
            this.chkPortalRegionColor.Enabled = false;
            this.chkPortalRegionColor.Location = new System.Drawing.Point(18, 90);
            this.chkPortalRegionColor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkPortalRegionColor.Name = "chkPortalRegionColor";
            this.chkPortalRegionColor.Size = new System.Drawing.Size(193, 17);
            this.chkPortalRegionColor.TabIndex = 9;
            this.chkPortalRegionColor.Text = "Use color from last loaded portal file";
            this.chkPortalRegionColor.UseVisualStyleBackColor = true;
            this.chkPortalRegionColor.Visible = false;
            // 
            // FormOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 244);
            this.Controls.Add(this.chkPortalRegionColor);
            this.Controls.Add(this.btnColorPick);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlColor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.picMapSample);
            this.Controls.Add(this.chkMapColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
        private System.Windows.Forms.CheckBox chkPortalRegionColor;
    }
}