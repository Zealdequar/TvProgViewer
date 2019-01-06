﻿namespace TVProgViewer.TVProgApp
{
    partial class ClassifFavoriteDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClassifFavoriteDialog));
            this.tlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tbContain = new System.Windows.Forms.TextBox();
            this.lblContain = new System.Windows.Forms.Label();
            this.lblGenre = new System.Windows.Forms.Label();
            this.tbNonContain = new System.Windows.Forms.TextBox();
            this.lblDeleteAfter = new System.Windows.Forms.Label();
            this.dtpDeleteAfter = new System.Windows.Forms.DateTimePicker();
            this.lblNonContain = new System.Windows.Forms.Label();
            this.cbFavorite = new System.Windows.Forms.ComboBox();
            this.chkRemind = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlPanel
            // 
            resources.ApplyResources(this.tlPanel, "tlPanel");
            this.tlPanel.Controls.Add(this.tbContain, 1, 0);
            this.tlPanel.Controls.Add(this.lblContain, 0, 0);
            this.tlPanel.Controls.Add(this.lblGenre, 0, 2);
            this.tlPanel.Controls.Add(this.tbNonContain, 1, 1);
            this.tlPanel.Controls.Add(this.lblDeleteAfter, 0, 3);
            this.tlPanel.Controls.Add(this.dtpDeleteAfter, 1, 3);
            this.tlPanel.Controls.Add(this.lblNonContain, 0, 1);
            this.tlPanel.Controls.Add(this.cbFavorite, 1, 2);
            this.tlPanel.Controls.Add(this.chkRemind, 0, 4);
            this.tlPanel.Name = "tlPanel";
            // 
            // tbContain
            // 
            resources.ApplyResources(this.tbContain, "tbContain");
            this.tbContain.Name = "tbContain";
            this.tbContain.TextChanged += new System.EventHandler(this.tbContain_TextChanged);
            // 
            // lblContain
            // 
            resources.ApplyResources(this.lblContain, "lblContain");
            this.lblContain.Name = "lblContain";
            // 
            // lblGenre
            // 
            resources.ApplyResources(this.lblGenre, "lblGenre");
            this.lblGenre.Name = "lblGenre";
            // 
            // tbNonContain
            // 
            resources.ApplyResources(this.tbNonContain, "tbNonContain");
            this.tbNonContain.Name = "tbNonContain";
            // 
            // lblDeleteAfter
            // 
            resources.ApplyResources(this.lblDeleteAfter, "lblDeleteAfter");
            this.lblDeleteAfter.Name = "lblDeleteAfter";
            // 
            // dtpDeleteAfter
            // 
            resources.ApplyResources(this.dtpDeleteAfter, "dtpDeleteAfter");
            this.dtpDeleteAfter.Checked = false;
            this.dtpDeleteAfter.Name = "dtpDeleteAfter";
            this.dtpDeleteAfter.ShowCheckBox = true;
            // 
            // lblNonContain
            // 
            resources.ApplyResources(this.lblNonContain, "lblNonContain");
            this.lblNonContain.Name = "lblNonContain";
            // 
            // cbFavorite
            // 
            resources.ApplyResources(this.cbFavorite, "cbFavorite");
            this.cbFavorite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFavorite.FormattingEnabled = true;
            this.cbFavorite.Name = "cbFavorite";
            // 
            // chkRemind
            // 
            resources.ApplyResources(this.chkRemind, "chkRemind");
            this.chkRemind.Name = "chkRemind";
            this.chkRemind.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ClassifFavoriteDialog
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tlPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ClassifFavoriteDialog";
            this.Load += new System.EventHandler(this.ClassifGenreDialog_Load);
            this.tlPanel.ResumeLayout(false);
            this.tlPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlPanel;
        private System.Windows.Forms.TextBox tbContain;
        private System.Windows.Forms.Label lblContain;
        private System.Windows.Forms.Label lblNonContain;
        private System.Windows.Forms.Label lblGenre;
        private System.Windows.Forms.TextBox tbNonContain;
        private System.Windows.Forms.Label lblDeleteAfter;
        private System.Windows.Forms.DateTimePicker dtpDeleteAfter;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cbFavorite;
        private System.Windows.Forms.CheckBox chkRemind;
    }
}