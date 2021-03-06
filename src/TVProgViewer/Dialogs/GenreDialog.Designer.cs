﻿using CustomControl.OrientAbleTextControls;

namespace TVProgViewer.TVProgApp
{
    partial class GenreDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenreDialog));
            this.lblGenreName = new System.Windows.Forms.Label();
            this.tbGenreName = new System.Windows.Forms.TextBox();
            this.lblPic = new System.Windows.Forms.Label();
            this.picBox = new System.Windows.Forms.PictureBox();
            this.btnOpenImage = new System.Windows.Forms.Button();
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblPix1 = new System.Windows.Forms.Label();
            this.lblPix2 = new CustomControl.OrientAbleTextControls.OrientedTextLabel();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lblGenreName
            // 
            resources.ApplyResources(this.lblGenreName, "lblGenreName");
            this.lblGenreName.Name = "lblGenreName";
            // 
            // tbGenreName
            // 
            resources.ApplyResources(this.tbGenreName, "tbGenreName");
            this.tbGenreName.Name = "tbGenreName";
            // 
            // lblPic
            // 
            resources.ApplyResources(this.lblPic, "lblPic");
            this.lblPic.Name = "lblPic";
            // 
            // picBox
            // 
            resources.ApplyResources(this.picBox, "picBox");
            this.picBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox.Name = "picBox";
            this.picBox.TabStop = false;
            // 
            // btnOpenImage
            // 
            resources.ApplyResources(this.btnOpenImage, "btnOpenImage");
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.UseVisualStyleBackColor = true;
            this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // chkVisible
            // 
            resources.ApplyResources(this.chkVisible, "chkVisible");
            this.chkVisible.Checked = true;
            this.chkVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // lblPix1
            // 
            resources.ApplyResources(this.lblPix1, "lblPix1");
            this.lblPix1.Name = "lblPix1";
            // 
            // lblPix2
            // 
            resources.ApplyResources(this.lblPix2, "lblPix2");
            this.lblPix2.Name = "lblPix2";
            this.lblPix2.RotationAngle = 270D;
            this.lblPix2.TextDirection = CustomControl.OrientAbleTextControls.Direction.Clockwise;
            this.lblPix2.TextOrientation = CustomControl.OrientAbleTextControls.Orientation.Rotate;
            // 
            // GenreDialog
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.lblPix2);
            this.Controls.Add(this.lblPix1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkVisible);
            this.Controls.Add(this.btnOpenImage);
            this.Controls.Add(this.picBox);
            this.Controls.Add(this.lblPic);
            this.Controls.Add(this.tbGenreName);
            this.Controls.Add(this.lblGenreName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "GenreDialog";
            this.Load += new System.EventHandler(this.GenreDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblGenreName;
        private System.Windows.Forms.TextBox tbGenreName;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.PictureBox picBox;
        private System.Windows.Forms.Button btnOpenImage;
        private System.Windows.Forms.CheckBox chkVisible;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblPix1;
        private OrientedTextLabel lblPix2;
    }
}