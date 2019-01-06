﻿namespace TVProgViewer.TVProgApp.Dialogs
{
    partial class OptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
            this.tvpvOptions = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel = new System.Windows.Forms.Panel();
            this.OKCancelBox = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.OKCancelBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvpvOptions
            // 
            resources.ApplyResources(this.tvpvOptions, "tvpvOptions");
            this.tvpvOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvpvOptions.Name = "tvpvOptions";
            this.tvpvOptions.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvpvOptions.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvpvOptions.Nodes1"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvpvOptions.Nodes2"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvpvOptions.Nodes3"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvpvOptions.Nodes4"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvpvOptions.Nodes5")))});
            this.tvpvOptions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvpvOptions_AfterSelect);
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.Name = "panel";
            // 
            // OKCancelBox
            // 
            resources.ApplyResources(this.OKCancelBox, "OKCancelBox");
            this.OKCancelBox.Controls.Add(this.btnCancel);
            this.OKCancelBox.Controls.Add(this.btnOK);
            this.OKCancelBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OKCancelBox.Name = "OKCancelBox";
            this.OKCancelBox.TabStop = false;
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
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.tvpvOptions);
            this.Controls.Add(this.OKCancelBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.OKCancelBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvpvOptions;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.GroupBox OKCancelBox;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}