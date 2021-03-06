﻿namespace TVProgViewer.TVProgApp.Controls
{
    partial class VisibleSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisibleSettings));
            this.gbView = new System.Windows.Forms.GroupBox();
            this.chbNodeSelect = new System.Windows.Forms.CheckBox();
            this.chbNumberChannel = new System.Windows.Forms.CheckBox();
            this.chbLogoTable = new System.Windows.Forms.CheckBox();
            this.chbPicTree = new System.Windows.Forms.CheckBox();
            this.chbVertGrid = new System.Windows.Forms.CheckBox();
            this.chbHorizGrid = new System.Windows.Forms.CheckBox();
            this.chbStatusStrip = new System.Windows.Forms.CheckBox();
            this.chbToolPanel = new System.Windows.Forms.CheckBox();
            this.chbMainMenu = new System.Windows.Forms.CheckBox();
            this.gbAllWeek = new System.Windows.Forms.GroupBox();
            this.rbQuery = new System.Windows.Forms.RadioButton();
            this.rbAlways = new System.Windows.Forms.RadioButton();
            this.rbNever = new System.Windows.Forms.RadioButton();
            this.gbAnons = new System.Windows.Forms.GroupBox();
            this.rbAnonsIfitis = new System.Windows.Forms.RadioButton();
            this.rbAnonsAlways = new System.Windows.Forms.RadioButton();
            this.rbAnonsNever = new System.Windows.Forms.RadioButton();
            this.gbView.SuspendLayout();
            this.gbAllWeek.SuspendLayout();
            this.gbAnons.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbView
            // 
            resources.ApplyResources(this.gbView, "gbView");
            this.gbView.Controls.Add(this.chbNodeSelect);
            this.gbView.Controls.Add(this.chbNumberChannel);
            this.gbView.Controls.Add(this.chbLogoTable);
            this.gbView.Controls.Add(this.chbPicTree);
            this.gbView.Controls.Add(this.chbVertGrid);
            this.gbView.Controls.Add(this.chbHorizGrid);
            this.gbView.Controls.Add(this.chbStatusStrip);
            this.gbView.Controls.Add(this.chbToolPanel);
            this.gbView.Controls.Add(this.chbMainMenu);
            this.gbView.Name = "gbView";
            this.gbView.TabStop = false;
            // 
            // chbNodeSelect
            // 
            resources.ApplyResources(this.chbNodeSelect, "chbNodeSelect");
            this.chbNodeSelect.Checked = true;
            this.chbNodeSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbNodeSelect.Name = "chbNodeSelect";
            this.chbNodeSelect.UseVisualStyleBackColor = true;
            this.chbNodeSelect.CheckedChanged += new System.EventHandler(this.chbNodeSelect_CheckedChanged);
            // 
            // chbNumberChannel
            // 
            resources.ApplyResources(this.chbNumberChannel, "chbNumberChannel");
            this.chbNumberChannel.Name = "chbNumberChannel";
            this.chbNumberChannel.UseVisualStyleBackColor = true;
            this.chbNumberChannel.CheckedChanged += new System.EventHandler(this.chbNumberChannel_CheckedChanged);
            // 
            // chbLogoTable
            // 
            resources.ApplyResources(this.chbLogoTable, "chbLogoTable");
            this.chbLogoTable.Checked = true;
            this.chbLogoTable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbLogoTable.Name = "chbLogoTable";
            this.chbLogoTable.UseVisualStyleBackColor = true;
            this.chbLogoTable.CheckedChanged += new System.EventHandler(this.chbLogoTable_CheckedChanged);
            // 
            // chbPicTree
            // 
            resources.ApplyResources(this.chbPicTree, "chbPicTree");
            this.chbPicTree.Checked = true;
            this.chbPicTree.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbPicTree.Name = "chbPicTree";
            this.chbPicTree.UseVisualStyleBackColor = true;
            this.chbPicTree.CheckedChanged += new System.EventHandler(this.chbPicTree_CheckedChanged);
            // 
            // chbVertGrid
            // 
            resources.ApplyResources(this.chbVertGrid, "chbVertGrid");
            this.chbVertGrid.Checked = true;
            this.chbVertGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbVertGrid.Name = "chbVertGrid";
            this.chbVertGrid.UseVisualStyleBackColor = true;
            this.chbVertGrid.CheckedChanged += new System.EventHandler(this.chbVertGrid_CheckedChanged);
            // 
            // chbHorizGrid
            // 
            resources.ApplyResources(this.chbHorizGrid, "chbHorizGrid");
            this.chbHorizGrid.Name = "chbHorizGrid";
            this.chbHorizGrid.UseVisualStyleBackColor = true;
            this.chbHorizGrid.CheckedChanged += new System.EventHandler(this.chbHorizGrid_CheckedChanged);
            // 
            // chbStatusStrip
            // 
            resources.ApplyResources(this.chbStatusStrip, "chbStatusStrip");
            this.chbStatusStrip.Checked = true;
            this.chbStatusStrip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbStatusStrip.Name = "chbStatusStrip";
            this.chbStatusStrip.UseVisualStyleBackColor = true;
            this.chbStatusStrip.CheckedChanged += new System.EventHandler(this.chbStatusStrip_CheckedChanged);
            // 
            // chbToolPanel
            // 
            resources.ApplyResources(this.chbToolPanel, "chbToolPanel");
            this.chbToolPanel.Checked = true;
            this.chbToolPanel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbToolPanel.Name = "chbToolPanel";
            this.chbToolPanel.UseVisualStyleBackColor = true;
            this.chbToolPanel.CheckedChanged += new System.EventHandler(this.chbToolPanel_CheckedChanged);
            // 
            // chbMainMenu
            // 
            resources.ApplyResources(this.chbMainMenu, "chbMainMenu");
            this.chbMainMenu.Checked = true;
            this.chbMainMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbMainMenu.Name = "chbMainMenu";
            this.chbMainMenu.UseVisualStyleBackColor = true;
            this.chbMainMenu.CheckedChanged += new System.EventHandler(this.chbMainMenu_CheckedChanged);
            // 
            // gbAllWeek
            // 
            resources.ApplyResources(this.gbAllWeek, "gbAllWeek");
            this.gbAllWeek.Controls.Add(this.rbQuery);
            this.gbAllWeek.Controls.Add(this.rbAlways);
            this.gbAllWeek.Controls.Add(this.rbNever);
            this.gbAllWeek.Name = "gbAllWeek";
            this.gbAllWeek.TabStop = false;
            // 
            // rbQuery
            // 
            resources.ApplyResources(this.rbQuery, "rbQuery");
            this.rbQuery.Checked = true;
            this.rbQuery.Name = "rbQuery";
            this.rbQuery.TabStop = true;
            this.rbQuery.UseVisualStyleBackColor = true;
            this.rbQuery.CheckedChanged += new System.EventHandler(this.rbQuery_CheckedChanged);
            // 
            // rbAlways
            // 
            resources.ApplyResources(this.rbAlways, "rbAlways");
            this.rbAlways.Name = "rbAlways";
            this.rbAlways.UseVisualStyleBackColor = true;
            this.rbAlways.CheckedChanged += new System.EventHandler(this.rbAlways_CheckedChanged);
            // 
            // rbNever
            // 
            resources.ApplyResources(this.rbNever, "rbNever");
            this.rbNever.Name = "rbNever";
            this.rbNever.UseVisualStyleBackColor = true;
            this.rbNever.CheckedChanged += new System.EventHandler(this.rbNever_CheckedChanged);
            // 
            // gbAnons
            // 
            resources.ApplyResources(this.gbAnons, "gbAnons");
            this.gbAnons.Controls.Add(this.rbAnonsIfitis);
            this.gbAnons.Controls.Add(this.rbAnonsAlways);
            this.gbAnons.Controls.Add(this.rbAnonsNever);
            this.gbAnons.Name = "gbAnons";
            this.gbAnons.TabStop = false;
            // 
            // rbAnonsIfitis
            // 
            resources.ApplyResources(this.rbAnonsIfitis, "rbAnonsIfitis");
            this.rbAnonsIfitis.Checked = true;
            this.rbAnonsIfitis.Name = "rbAnonsIfitis";
            this.rbAnonsIfitis.TabStop = true;
            this.rbAnonsIfitis.UseVisualStyleBackColor = true;
            this.rbAnonsIfitis.CheckedChanged += new System.EventHandler(this.rbAnonsIfitis_CheckedChanged);
            // 
            // rbAnonsAlways
            // 
            resources.ApplyResources(this.rbAnonsAlways, "rbAnonsAlways");
            this.rbAnonsAlways.Name = "rbAnonsAlways";
            this.rbAnonsAlways.UseVisualStyleBackColor = true;
            this.rbAnonsAlways.CheckedChanged += new System.EventHandler(this.rbAnonsAlways_CheckedChanged);
            // 
            // rbAnonsNever
            // 
            resources.ApplyResources(this.rbAnonsNever, "rbAnonsNever");
            this.rbAnonsNever.Name = "rbAnonsNever";
            this.rbAnonsNever.UseVisualStyleBackColor = true;
            this.rbAnonsNever.CheckedChanged += new System.EventHandler(this.rbAnonsNever_CheckedChanged);
            // 
            // VisibleSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbAnons);
            this.Controls.Add(this.gbAllWeek);
            this.Controls.Add(this.gbView);
            this.Name = "VisibleSettings";
            this.Load += new System.EventHandler(this.VisibleSettings_Load);
            this.gbView.ResumeLayout(false);
            this.gbView.PerformLayout();
            this.gbAllWeek.ResumeLayout(false);
            this.gbAllWeek.PerformLayout();
            this.gbAnons.ResumeLayout(false);
            this.gbAnons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbView;
        private System.Windows.Forms.CheckBox chbNodeSelect;
        private System.Windows.Forms.CheckBox chbNumberChannel;
        private System.Windows.Forms.CheckBox chbLogoTable;
        private System.Windows.Forms.CheckBox chbPicTree;
        private System.Windows.Forms.CheckBox chbVertGrid;
        private System.Windows.Forms.CheckBox chbHorizGrid;
        private System.Windows.Forms.CheckBox chbStatusStrip;
        private System.Windows.Forms.CheckBox chbToolPanel;
        private System.Windows.Forms.CheckBox chbMainMenu;
        private System.Windows.Forms.GroupBox gbAllWeek;
        private System.Windows.Forms.RadioButton rbQuery;
        private System.Windows.Forms.RadioButton rbAlways;
        private System.Windows.Forms.RadioButton rbNever;
        private System.Windows.Forms.GroupBox gbAnons;
        private System.Windows.Forms.RadioButton rbAnonsIfitis;
        private System.Windows.Forms.RadioButton rbAnonsAlways;
        private System.Windows.Forms.RadioButton rbAnonsNever;
    }
}
