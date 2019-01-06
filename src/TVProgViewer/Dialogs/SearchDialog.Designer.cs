﻿namespace TVProgViewer.TVProgApp
{
    partial class SearchDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tbMatch = new System.Windows.Forms.TextBox();
            this.lblDontMatch = new System.Windows.Forms.Label();
            this.tbDontMatch = new System.Windows.Forms.TextBox();
            this.lblMatch = new System.Windows.Forms.Label();
            this.trackBarTo = new System.Windows.Forms.TrackBar();
            this.trackBarFrom = new System.Windows.Forms.TrackBar();
            this.chkTimeFromTo = new System.Windows.Forms.CheckBox();
            this.chkSearchInAnons = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pCheckes = new System.Windows.Forms.Panel();
            this.pControls = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.chkDaysOfWeek = new System.Windows.Forms.ToolStripButton();
            this.chkChannels = new System.Windows.Forms.ToolStripButton();
            this.chkGenres = new System.Windows.Forms.ToolStripButton();
            this.chkRating = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.btnCheckAll = new System.Windows.Forms.ToolStripButton();
            this.btnUnchecked = new System.Windows.Forms.ToolStripButton();
            this.btnInvert = new System.Windows.Forms.ToolStripButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkSaveParamSearch = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.tlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFrom)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.pCheckes.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.tlPanel);
            this.groupBox1.Controls.Add(this.trackBarTo);
            this.groupBox1.Controls.Add(this.trackBarFrom);
            this.groupBox1.Controls.Add(this.chkTimeFromTo);
            this.groupBox1.Controls.Add(this.chkSearchInAnons);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // tlPanel
            // 
            resources.ApplyResources(this.tlPanel, "tlPanel");
            this.tlPanel.Controls.Add(this.tbMatch, 1, 0);
            this.tlPanel.Controls.Add(this.lblDontMatch, 0, 1);
            this.tlPanel.Controls.Add(this.tbDontMatch, 1, 1);
            this.tlPanel.Controls.Add(this.lblMatch, 0, 0);
            this.tlPanel.Name = "tlPanel";
            this.toolTip1.SetToolTip(this.tlPanel, resources.GetString("tlPanel.ToolTip"));
            // 
            // tbMatch
            // 
            resources.ApplyResources(this.tbMatch, "tbMatch");
            this.tbMatch.Name = "tbMatch";
            this.toolTip1.SetToolTip(this.tbMatch, resources.GetString("tbMatch.ToolTip"));
            // 
            // lblDontMatch
            // 
            resources.ApplyResources(this.lblDontMatch, "lblDontMatch");
            this.lblDontMatch.Name = "lblDontMatch";
            this.toolTip1.SetToolTip(this.lblDontMatch, resources.GetString("lblDontMatch.ToolTip"));
            // 
            // tbDontMatch
            // 
            resources.ApplyResources(this.tbDontMatch, "tbDontMatch");
            this.tbDontMatch.Name = "tbDontMatch";
            this.toolTip1.SetToolTip(this.tbDontMatch, resources.GetString("tbDontMatch.ToolTip"));
            // 
            // lblMatch
            // 
            resources.ApplyResources(this.lblMatch, "lblMatch");
            this.lblMatch.Name = "lblMatch";
            this.toolTip1.SetToolTip(this.lblMatch, resources.GetString("lblMatch.ToolTip"));
            // 
            // trackBarTo
            // 
            resources.ApplyResources(this.trackBarTo, "trackBarTo");
            this.trackBarTo.Maximum = 144;
            this.trackBarTo.Name = "trackBarTo";
            this.trackBarTo.TickStyle = System.Windows.Forms.TickStyle.None;
            this.toolTip1.SetToolTip(this.trackBarTo, resources.GetString("trackBarTo.ToolTip"));
            this.trackBarTo.Value = 144;
            this.trackBarTo.ValueChanged += new System.EventHandler(this.trackBarTo_ValueChanged);
            // 
            // trackBarFrom
            // 
            resources.ApplyResources(this.trackBarFrom, "trackBarFrom");
            this.trackBarFrom.Maximum = 144;
            this.trackBarFrom.Name = "trackBarFrom";
            this.trackBarFrom.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.toolTip1.SetToolTip(this.trackBarFrom, resources.GetString("trackBarFrom.ToolTip"));
            this.trackBarFrom.ValueChanged += new System.EventHandler(this.trackBarFrom_ValueChanged);
            // 
            // chkTimeFromTo
            // 
            resources.ApplyResources(this.chkTimeFromTo, "chkTimeFromTo");
            this.chkTimeFromTo.Name = "chkTimeFromTo";
            this.toolTip1.SetToolTip(this.chkTimeFromTo, resources.GetString("chkTimeFromTo.ToolTip"));
            this.chkTimeFromTo.UseVisualStyleBackColor = true;
            // 
            // chkSearchInAnons
            // 
            resources.ApplyResources(this.chkSearchInAnons, "chkSearchInAnons");
            this.chkSearchInAnons.Name = "chkSearchInAnons";
            this.toolTip1.SetToolTip(this.chkSearchInAnons, resources.GetString("chkSearchInAnons.ToolTip"));
            this.chkSearchInAnons.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.pCheckes);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // pCheckes
            // 
            resources.ApplyResources(this.pCheckes, "pCheckes");
            this.pCheckes.Controls.Add(this.pControls);
            this.pCheckes.Controls.Add(this.toolStrip1);
            this.pCheckes.Name = "pCheckes";
            this.toolTip1.SetToolTip(this.pCheckes, resources.GetString("pCheckes.ToolTip"));
            // 
            // pControls
            // 
            resources.ApplyResources(this.pControls, "pControls");
            this.pControls.Name = "pControls";
            this.toolTip1.SetToolTip(this.pControls, resources.GetString("pControls.ToolTip"));
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkDaysOfWeek,
            this.chkChannels,
            this.chkGenres,
            this.chkRating,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.btnCheckAll,
            this.btnUnchecked,
            this.btnInvert});
            this.toolStrip1.Name = "toolStrip1";
            this.toolTip1.SetToolTip(this.toolStrip1, resources.GetString("toolStrip1.ToolTip"));
            // 
            // chkDaysOfWeek
            // 
            resources.ApplyResources(this.chkDaysOfWeek, "chkDaysOfWeek");
            this.chkDaysOfWeek.Checked = true;
            this.chkDaysOfWeek.CheckOnClick = true;
            this.chkDaysOfWeek.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDaysOfWeek.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.chkDaysOfWeek.Name = "chkDaysOfWeek";
            this.chkDaysOfWeek.CheckedChanged += new System.EventHandler(this.chkDaysOfWeek_CheckedChanged);
            // 
            // chkChannels
            // 
            resources.ApplyResources(this.chkChannels, "chkChannels");
            this.chkChannels.CheckOnClick = true;
            this.chkChannels.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.chkChannels.Name = "chkChannels";
            this.chkChannels.CheckedChanged += new System.EventHandler(this.chkChannels_CheckedChanged);
            // 
            // chkGenres
            // 
            resources.ApplyResources(this.chkGenres, "chkGenres");
            this.chkGenres.CheckOnClick = true;
            this.chkGenres.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.chkGenres.Name = "chkGenres";
            this.chkGenres.CheckedChanged += new System.EventHandler(this.chkGenres_CheckedChanged);
            // 
            // chkRating
            // 
            resources.ApplyResources(this.chkRating, "chkRating");
            this.chkRating.CheckOnClick = true;
            this.chkRating.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.chkRating.Name = "chkRating";
            this.chkRating.CheckedChanged += new System.EventHandler(this.chkRating_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // toolStripLabel1
            // 
            resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
            this.toolStripLabel1.Name = "toolStripLabel1";
            // 
            // btnCheckAll
            // 
            resources.ApplyResources(this.btnCheckAll, "btnCheckAll");
            this.btnCheckAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnUnchecked
            // 
            resources.ApplyResources(this.btnUnchecked, "btnUnchecked");
            this.btnUnchecked.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUnchecked.Name = "btnUnchecked";
            this.btnUnchecked.Click += new System.EventHandler(this.btnUnchecked_Click);
            // 
            // btnInvert
            // 
            resources.ApplyResources(this.btnInvert, "btnInvert");
            this.btnInvert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnInvert.Name = "btnInvert";
            this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.chkSaveParamSearch);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // chkSaveParamSearch
            // 
            resources.ApplyResources(this.chkSaveParamSearch, "chkSaveParamSearch");
            this.chkSaveParamSearch.Checked = true;
            this.chkSaveParamSearch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveParamSearch.Name = "chkSaveParamSearch";
            this.toolTip1.SetToolTip(this.chkSaveParamSearch, resources.GetString("chkSaveParamSearch.ToolTip"));
            this.chkSaveParamSearch.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.toolTip1.SetToolTip(this.btnOK, resources.GetString("btnOK.ToolTip"));
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // SearchDialog
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SearchDialog";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.SearchForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlPanel.ResumeLayout(false);
            this.tlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFrom)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.pCheckes.ResumeLayout(false);
            this.pCheckes.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tlPanel;
        private System.Windows.Forms.TextBox tbMatch;
        private System.Windows.Forms.Label lblDontMatch;
        private System.Windows.Forms.TextBox tbDontMatch;
        private System.Windows.Forms.Label lblMatch;
        private System.Windows.Forms.TrackBar trackBarTo;
        private System.Windows.Forms.TrackBar trackBarFrom;
        private System.Windows.Forms.CheckBox chkTimeFromTo;
        private System.Windows.Forms.CheckBox chkSearchInAnons;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkSaveParamSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel pCheckes;
        private System.Windows.Forms.Panel pControls;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton chkDaysOfWeek;
        private System.Windows.Forms.ToolStripButton chkChannels;
        private System.Windows.Forms.ToolStripButton chkGenres;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnCheckAll;
        private System.Windows.Forms.ToolStripButton btnUnchecked;
        private System.Windows.Forms.ToolStripButton btnInvert;
        private System.Windows.Forms.ToolStripButton chkRating;

    }
}