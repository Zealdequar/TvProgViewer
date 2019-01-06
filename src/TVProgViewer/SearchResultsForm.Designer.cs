﻿namespace TVProgViewer.TVProgApp
{
    partial class SearchResultsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchResultsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgSearching = new TVProgViewer.TVProgApp.DataGridViewExt();
            this.colChanId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRec = new System.Windows.Forms.DataGridViewImageColumn();
            this.colBell = new System.Windows.Forms.DataGridViewImageColumn();
            this.colRating = new System.Windows.Forms.DataGridViewImageColumn();
            this.colGenre = new System.Windows.Forms.DataGridViewImageColumn();
            this.colChanAnons = new System.Windows.Forms.DataGridViewImageColumn();
            this.colChanChannel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChanImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChanFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChanTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChanProg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProgCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChanDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFavName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNumChannel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSearch = new System.Windows.Forms.ToolStripButton();
            this.btnExportXsl = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.slblTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.imgLst = new System.Windows.Forms.ImageList(this.components);
            this.pAnons = new System.Windows.Forms.Panel();
            this.rtbAnons = new System.Windows.Forms.RichTextBox();
            this.pAnonsInResult = new System.Windows.Forms.Panel();
            this.btnSaveDesc = new System.Windows.Forms.Button();
            this.btnCancelDesc = new System.Windows.Forms.Button();
            this.btnExitDesc = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.imageListremind = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuTables = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.csmiAddToFavorite = new System.Windows.Forms.ToolStripMenuItem();
            this.csmiAddToGenres = new System.Windows.Forms.ToolStripMenuItem();
            this.csmiPropChannel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.csmiCopyToBuffer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.csmiChangeRating = new System.Windows.Forms.ToolStripMenuItem();
            this.csmiChangeType = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgSearching)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.pAnons.SuspendLayout();
            this.pAnonsInResult.SuspendLayout();
            this.contextMenuTables.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgSearching
            // 
            resources.ApplyResources(this.dgSearching, "dgSearching");
            this.dgSearching.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.dgSearching.BackgroundColor2 = System.Drawing.Color.Empty;
            this.dgSearching.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSearching.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colChanId,
            this.colCID,
            this.colRec,
            this.colBell,
            this.colRating,
            this.colGenre,
            this.colChanAnons,
            this.colChanChannel,
            this.colChanImage,
            this.colDay,
            this.colChanFrom,
            this.colChanTo,
            this.colChanProg,
            this.colProgCategory,
            this.colChanDesc,
            this.colFavName,
            this.colNumChannel});
            this.dgSearching.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.dgSearching.IsGradient = false;
            this.dgSearching.Name = "dgSearching";
            this.dgSearching.RowHeadersVisible = false;
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgSearching.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgSearching.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSearching_CellContentClick);
            this.dgSearching.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgSearching_CellPainting);
            this.dgSearching.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgSearching_RowsAdded);
            this.dgSearching.SelectionChanged += new System.EventHandler(this.dgSearching_SelectionChanged);
            this.dgSearching.Paint += new System.Windows.Forms.PaintEventHandler(this.dgSearching_Paint);
            this.dgSearching.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgSearching_MouseUp);
            // 
            // colChanId
            // 
            this.colChanId.DataPropertyName = "id";
            resources.ApplyResources(this.colChanId, "colChanId");
            this.colChanId.Name = "colChanId";
            // 
            // colCID
            // 
            this.colCID.DataPropertyName = "cid";
            resources.ApplyResources(this.colCID, "colCID");
            this.colCID.Name = "colCID";
            // 
            // colRec
            // 
            this.colRec.DataPropertyName = "capture";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = null;
            this.colRec.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.colRec, "colRec");
            this.colRec.Name = "colRec";
            this.colRec.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colRec.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colBell
            // 
            this.colBell.DataPropertyName = "bell";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = null;
            this.colBell.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.colBell, "colBell");
            this.colBell.Name = "colBell";
            // 
            // colRating
            // 
            this.colRating.DataPropertyName = "rating";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = null;
            this.colRating.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.colRating, "colRating");
            this.colRating.Name = "colRating";
            // 
            // colGenre
            // 
            this.colGenre.DataPropertyName = "genre";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.NullValue = null;
            this.colGenre.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.colGenre, "colGenre");
            this.colGenre.Name = "colGenre";
            this.colGenre.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colGenre.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colChanAnons
            // 
            this.colChanAnons.DataPropertyName = "anons";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.NullValue = null;
            this.colChanAnons.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.colChanAnons, "colChanAnons");
            this.colChanAnons.Name = "colChanAnons";
            // 
            // colChanChannel
            // 
            this.colChanChannel.DataPropertyName = "display-name";
            resources.ApplyResources(this.colChanChannel, "colChanChannel");
            this.colChanChannel.Name = "colChanChannel";
            // 
            // colChanImage
            // 
            this.colChanImage.DataPropertyName = "image";
            resources.ApplyResources(this.colChanImage, "colChanImage");
            this.colChanImage.Name = "colChanImage";
            // 
            // colDay
            // 
            this.colDay.DataPropertyName = "day";
            resources.ApplyResources(this.colDay, "colDay");
            this.colDay.Name = "colDay";
            // 
            // colChanFrom
            // 
            this.colChanFrom.DataPropertyName = "start";
            dataGridViewCellStyle6.Format = "t";
            this.colChanFrom.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.colChanFrom, "colChanFrom");
            this.colChanFrom.Name = "colChanFrom";
            // 
            // colChanTo
            // 
            this.colChanTo.DataPropertyName = "stop";
            dataGridViewCellStyle7.Format = "t";
            this.colChanTo.DefaultCellStyle = dataGridViewCellStyle7;
            resources.ApplyResources(this.colChanTo, "colChanTo");
            this.colChanTo.Name = "colChanTo";
            // 
            // colChanProg
            // 
            this.colChanProg.DataPropertyName = "title";
            resources.ApplyResources(this.colChanProg, "colChanProg");
            this.colChanProg.Name = "colChanProg";
            // 
            // colProgCategory
            // 
            this.colProgCategory.DataPropertyName = "category";
            resources.ApplyResources(this.colProgCategory, "colProgCategory");
            this.colProgCategory.Name = "colProgCategory";
            // 
            // colChanDesc
            // 
            this.colChanDesc.DataPropertyName = "desc";
            resources.ApplyResources(this.colChanDesc, "colChanDesc");
            this.colChanDesc.Name = "colChanDesc";
            // 
            // colFavName
            // 
            this.colFavName.DataPropertyName = "favname";
            resources.ApplyResources(this.colFavName, "colFavName");
            this.colFavName.Name = "colFavName";
            // 
            // colNumChannel
            // 
            this.colNumChannel.DataPropertyName = "number";
            resources.ApplyResources(this.colNumChannel, "colNumChannel");
            this.colNumChannel.Name = "colNumChannel";
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSearch,
            this.btnExportXsl});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnExportXsl
            // 
            resources.ApplyResources(this.btnExportXsl, "btnExportXsl");
            this.btnExportXsl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExportXsl.Name = "btnExportXsl";
            this.btnExportXsl.Click += new System.EventHandler(this.btnExportXsl_Click);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slblTotal});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // slblTotal
            // 
            resources.ApplyResources(this.slblTotal, "slblTotal");
            this.slblTotal.Name = "slblTotal";
            // 
            // imgLst
            // 
            this.imgLst.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLst.ImageStream")));
            this.imgLst.TransparentColor = System.Drawing.Color.Transparent;
            this.imgLst.Images.SetKeyName(0, "satellite_dish2.png");
            this.imgLst.Images.SetKeyName(1, "GreenAnons.png");
            this.imgLst.Images.SetKeyName(2, "GenreEditor25.png");
            this.imgLst.Images.SetKeyName(3, "favorites25.png");
            // 
            // pAnons
            // 
            resources.ApplyResources(this.pAnons, "pAnons");
            this.pAnons.Controls.Add(this.rtbAnons);
            this.pAnons.Controls.Add(this.pAnonsInResult);
            this.pAnons.Name = "pAnons";
            // 
            // rtbAnons
            // 
            resources.ApplyResources(this.rtbAnons, "rtbAnons");
            this.rtbAnons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbAnons.Name = "rtbAnons";
            this.rtbAnons.ModifiedChanged += new System.EventHandler(this.rtbAnons_ModifiedChanged);
            // 
            // pAnonsInResult
            // 
            resources.ApplyResources(this.pAnonsInResult, "pAnonsInResult");
            this.pAnonsInResult.Controls.Add(this.btnSaveDesc);
            this.pAnonsInResult.Controls.Add(this.btnCancelDesc);
            this.pAnonsInResult.Controls.Add(this.btnExitDesc);
            this.pAnonsInResult.Name = "pAnonsInResult";
            // 
            // btnSaveDesc
            // 
            resources.ApplyResources(this.btnSaveDesc, "btnSaveDesc");
            this.btnSaveDesc.Name = "btnSaveDesc";
            this.btnSaveDesc.UseVisualStyleBackColor = true;
            this.btnSaveDesc.Click += new System.EventHandler(this.btnSaveDesc_Click);
            // 
            // btnCancelDesc
            // 
            resources.ApplyResources(this.btnCancelDesc, "btnCancelDesc");
            this.btnCancelDesc.Name = "btnCancelDesc";
            this.btnCancelDesc.UseVisualStyleBackColor = true;
            this.btnCancelDesc.Click += new System.EventHandler(this.btnCancelDesc_Click);
            // 
            // btnExitDesc
            // 
            resources.ApplyResources(this.btnExitDesc, "btnExitDesc");
            this.btnExitDesc.Name = "btnExitDesc";
            this.btnExitDesc.UseVisualStyleBackColor = true;
            this.btnExitDesc.Click += new System.EventHandler(this.btnExitDesc_Click);
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // imageListremind
            // 
            this.imageListremind.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListremind.ImageStream")));
            this.imageListremind.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListremind.Images.SetKeyName(0, "bellheader.png");
            this.imageListremind.Images.SetKeyName(1, "capture_header.png");
            // 
            // contextMenuTables
            // 
            resources.ApplyResources(this.contextMenuTables, "contextMenuTables");
            this.contextMenuTables.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.csmiAddToFavorite,
            this.csmiAddToGenres,
            this.csmiPropChannel,
            this.toolStripSeparator1,
            this.csmiCopyToBuffer,
            this.toolStripSeparator2,
            this.csmiChangeRating,
            this.csmiChangeType});
            this.contextMenuTables.Name = "contextMenuTables";
            // 
            // csmiAddToFavorite
            // 
            resources.ApplyResources(this.csmiAddToFavorite, "csmiAddToFavorite");
            this.csmiAddToFavorite.Name = "csmiAddToFavorite";
            this.csmiAddToFavorite.Click += new System.EventHandler(this.csmiAddToFavorite_Click);
            // 
            // csmiAddToGenres
            // 
            resources.ApplyResources(this.csmiAddToGenres, "csmiAddToGenres");
            this.csmiAddToGenres.Name = "csmiAddToGenres";
            this.csmiAddToGenres.Click += new System.EventHandler(this.csmiAddToGenres_Click);
            // 
            // csmiPropChannel
            // 
            resources.ApplyResources(this.csmiPropChannel, "csmiPropChannel");
            this.csmiPropChannel.Name = "csmiPropChannel";
            this.csmiPropChannel.Click += new System.EventHandler(this.csmiPropChannel_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // csmiCopyToBuffer
            // 
            resources.ApplyResources(this.csmiCopyToBuffer, "csmiCopyToBuffer");
            this.csmiCopyToBuffer.Name = "csmiCopyToBuffer";
            this.csmiCopyToBuffer.Click += new System.EventHandler(this.csmiCopyToBuffer_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // csmiChangeRating
            // 
            resources.ApplyResources(this.csmiChangeRating, "csmiChangeRating");
            this.csmiChangeRating.Name = "csmiChangeRating";
            // 
            // csmiChangeType
            // 
            resources.ApplyResources(this.csmiChangeType, "csmiChangeType");
            this.csmiChangeType.Name = "csmiChangeType";
            // 
            // SearchResultsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgSearching);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.pAnons);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "SearchResultsForm";
            this.Load += new System.EventHandler(this.SearchResultsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgSearching)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pAnons.ResumeLayout(false);
            this.pAnonsInResult.ResumeLayout(false);
            this.contextMenuTables.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridViewExt dgSearching;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSearch;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ImageList imgLst;
        private System.Windows.Forms.ToolStripStatusLabel slblTotal;
        private System.Windows.Forms.Panel pAnons;
        private System.Windows.Forms.RichTextBox rtbAnons;
        private System.Windows.Forms.Panel pAnonsInResult;
        private System.Windows.Forms.Button btnSaveDesc;
        private System.Windows.Forms.Button btnCancelDesc;
        private System.Windows.Forms.Button btnExitDesc;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ImageList imageListremind;
        private System.Windows.Forms.ContextMenuStrip contextMenuTables;
        private System.Windows.Forms.ToolStripMenuItem csmiCopyToBuffer;
        private System.Windows.Forms.ToolStripMenuItem csmiAddToFavorite;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem csmiAddToGenres;
        private System.Windows.Forms.ToolStripMenuItem csmiPropChannel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem csmiChangeRating;
        private System.Windows.Forms.ToolStripMenuItem csmiChangeType;
        private System.Windows.Forms.ToolStripButton btnExportXsl;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChanId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCID;
        private System.Windows.Forms.DataGridViewImageColumn colRec;
        private System.Windows.Forms.DataGridViewImageColumn colBell;
        private System.Windows.Forms.DataGridViewImageColumn colRating;
        private System.Windows.Forms.DataGridViewImageColumn colGenre;
        private System.Windows.Forms.DataGridViewImageColumn colChanAnons;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChanChannel;
        private System.Windows.Forms.DataGridViewImageColumn colChanImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChanFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChanTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChanProg;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProgCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChanDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFavName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumChannel;
    }
}