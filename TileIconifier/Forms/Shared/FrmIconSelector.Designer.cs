﻿namespace TileIconifier.Forms.Shared
{
    partial class FrmIconSelector
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
            //if(ReturnedBitmap != null)
            //    ReturnedBitmap.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.opnFile = new System.Windows.Forms.OpenFileDialog();
            this.lblPreview = new System.Windows.Forms.Label();
            this.pctPreview = new System.Windows.Forms.PictureBox();
            this.lblCommonDlls = new System.Windows.Forms.Label();
            this.cmbCommonIconDlls = new System.Windows.Forms.ComboBox();
            this.btnBrowseIconPath = new System.Windows.Forms.Button();
            this.txtPathToExtractFrom = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnBrowseCustomImage = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtImagePath = new System.Windows.Forms.TextBox();
            this.radUseCustomImage = new System.Windows.Forms.RadioButton();
            this.radIconFromTarget = new System.Windows.Forms.RadioButton();
            this.lvwIcons = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // opnFile
            // 
            this.opnFile.Filter = "Image Files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|Programs, ICO\'s and Libraries" +
    " (*.exe, *.dll, *.ico)|*.exe;*.dll;*.ico";
            // 
            // lblPreview
            // 
            this.lblPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPreview.Location = new System.Drawing.Point(13, 327);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(45, 13);
            this.lblPreview.TabIndex = 16;
            this.lblPreview.Text = "Preview";
            // 
            // pctPreview
            // 
            this.pctPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pctPreview.Location = new System.Drawing.Point(79, 327);
            this.pctPreview.Name = "pctPreview";
            this.pctPreview.Size = new System.Drawing.Size(72, 64);
            this.pctPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pctPreview.TabIndex = 15;
            this.pctPreview.TabStop = false;
            // 
            // lblCommonDlls
            // 
            this.lblCommonDlls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCommonDlls.Location = new System.Drawing.Point(10, 56);
            this.lblCommonDlls.Name = "lblCommonDlls";
            this.lblCommonDlls.Size = new System.Drawing.Size(100, 13);
            this.lblCommonDlls.TabIndex = 14;
            this.lblCommonDlls.Text = "Common Icon DLLs";
            // 
            // cmbCommonIconDlls
            // 
            this.cmbCommonIconDlls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCommonIconDlls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCommonIconDlls.FormattingEnabled = true;
            this.cmbCommonIconDlls.Location = new System.Drawing.Point(119, 53);
            this.cmbCommonIconDlls.Name = "cmbCommonIconDlls";
            this.cmbCommonIconDlls.Size = new System.Drawing.Size(351, 21);
            this.cmbCommonIconDlls.TabIndex = 13;
            this.cmbCommonIconDlls.SelectedIndexChanged += new System.EventHandler(this.cmbCommonIconDlls_SelectedIndexChanged);
            // 
            // btnBrowseIconPath
            // 
            this.btnBrowseIconPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseIconPath.Location = new System.Drawing.Point(491, 14);
            this.btnBrowseIconPath.Name = "btnBrowseIconPath";
            this.btnBrowseIconPath.Size = new System.Drawing.Size(75, 33);
            this.btnBrowseIconPath.TabIndex = 12;
            this.btnBrowseIconPath.Text = "Browse";
            this.btnBrowseIconPath.UseVisualStyleBackColor = true;
            this.btnBrowseIconPath.Click += new System.EventHandler(this.btnBrowseIconPath_Click);
            // 
            // txtPathToExtractFrom
            // 
            this.txtPathToExtractFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPathToExtractFrom.Location = new System.Drawing.Point(119, 21);
            this.txtPathToExtractFrom.Name = "txtPathToExtractFrom";
            this.txtPathToExtractFrom.ReadOnly = true;
            this.txtPathToExtractFrom.Size = new System.Drawing.Size(351, 20);
            this.txtPathToExtractFrom.TabIndex = 11;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(491, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 33);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(395, 356);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 33);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnBrowseCustomImage
            // 
            this.btnBrowseCustomImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseCustomImage.Location = new System.Drawing.Point(491, 294);
            this.btnBrowseCustomImage.Name = "btnBrowseCustomImage";
            this.btnBrowseCustomImage.Size = new System.Drawing.Size(75, 33);
            this.btnBrowseCustomImage.TabIndex = 8;
            this.btnBrowseCustomImage.Text = "Browse";
            this.btnBrowseCustomImage.UseVisualStyleBackColor = true;
            this.btnBrowseCustomImage.Click += new System.EventHandler(this.btnBrowseCustomImage_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(13, 304);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Image path";
            // 
            // txtImagePath
            // 
            this.txtImagePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImagePath.Location = new System.Drawing.Point(79, 301);
            this.txtImagePath.Name = "txtImagePath";
            this.txtImagePath.Size = new System.Drawing.Size(392, 20);
            this.txtImagePath.TabIndex = 6;
            this.txtImagePath.TextChanged += new System.EventHandler(this.txtImagePath_TextChanged);
            // 
            // radUseCustomImage
            // 
            this.radUseCustomImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radUseCustomImage.Location = new System.Drawing.Point(14, 281);
            this.radUseCustomImage.Name = "radUseCustomImage";
            this.radUseCustomImage.Size = new System.Drawing.Size(112, 17);
            this.radUseCustomImage.TabIndex = 5;
            this.radUseCustomImage.TabStop = true;
            this.radUseCustomImage.Text = "Use custom image";
            this.radUseCustomImage.UseVisualStyleBackColor = true;
            // 
            // radIconFromTarget
            // 
            this.radIconFromTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radIconFromTarget.Checked = true;
            this.radIconFromTarget.Location = new System.Drawing.Point(11, 21);
            this.radIconFromTarget.Name = "radIconFromTarget";
            this.radIconFromTarget.Size = new System.Drawing.Size(96, 17);
            this.radIconFromTarget.TabIndex = 4;
            this.radIconFromTarget.TabStop = true;
            this.radIconFromTarget.Text = "Extract an icon";
            this.radIconFromTarget.UseVisualStyleBackColor = true;
            // 
            // lvwIcons
            // 
            this.lvwIcons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwIcons.BackColor = System.Drawing.SystemColors.Control;
            this.lvwIcons.Location = new System.Drawing.Point(13, 89);
            this.lvwIcons.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvwIcons.MultiSelect = false;
            this.lvwIcons.Name = "lvwIcons";
            this.lvwIcons.OwnerDraw = true;
            this.lvwIcons.Size = new System.Drawing.Size(552, 184);
            this.lvwIcons.TabIndex = 3;
            this.lvwIcons.TileSize = new System.Drawing.Size(50, 50);
            this.lvwIcons.UseCompatibleStateImageBehavior = false;
            this.lvwIcons.View = System.Windows.Forms.View.Tile;
            this.lvwIcons.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvwIcons_DrawItem);
            this.lvwIcons.SelectedIndexChanged += new System.EventHandler(this.lvwIcons_SelectedIndexChanged);
            this.lvwIcons.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvwIcons_MouseClick);
            this.lvwIcons.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwIcons_MouseDoubleClick);
            // 
            // FrmIconSelector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(578, 401);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.pctPreview);
            this.Controls.Add(this.lblCommonDlls);
            this.Controls.Add(this.cmbCommonIconDlls);
            this.Controls.Add(this.btnBrowseIconPath);
            this.Controls.Add(this.txtPathToExtractFrom);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnBrowseCustomImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtImagePath);
            this.Controls.Add(this.radUseCustomImage);
            this.Controls.Add(this.radIconFromTarget);
            this.Controls.Add(this.lvwIcons);
            this.Name = "FrmIconSelector";
            this.Text = "Icon Selector";
            ((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwIcons;
        private System.Windows.Forms.RadioButton radIconFromTarget;
        private System.Windows.Forms.RadioButton radUseCustomImage;
        private System.Windows.Forms.TextBox txtImagePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseCustomImage;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.OpenFileDialog opnFile;
        private System.Windows.Forms.TextBox txtPathToExtractFrom;
        private System.Windows.Forms.Button btnBrowseIconPath;
        private System.Windows.Forms.ComboBox cmbCommonIconDlls;
        private System.Windows.Forms.Label lblCommonDlls;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.PictureBox pctPreview;
    }
}