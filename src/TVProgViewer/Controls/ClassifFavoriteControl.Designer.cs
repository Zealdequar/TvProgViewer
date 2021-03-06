﻿namespace TVProgViewer.TVProgApp
{
    partial class ClassifFavoriteControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClassifFavoriteControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgClassifFavorites = new TVProgViewer.TVProgApp.DataGridViewExt();
            this.colRatingClassID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewImageColumn();
            this.colContain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDontContain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeleteAfter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemind = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colPrior = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgClassifFavorites)).BeginInit();
            this.SuspendLayout();
            // 
            // dgClassifFavorites
            // 
            resources.ApplyResources(this.dgClassifFavorites, "dgClassifFavorites");
            this.dgClassifFavorites.BackgroundColor2 = System.Drawing.Color.Empty;
            this.dgClassifFavorites.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgClassifFavorites.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRatingClassID,
            this.colFid,
            this.colType,
            this.colContain,
            this.colDontContain,
            this.colDeleteAfter,
            this.colRemind,
            this.colPrior});
            this.dgClassifFavorites.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.dgClassifFavorites.IsGradient = false;
            this.dgClassifFavorites.Name = "dgClassifFavorites";
            this.dgClassifFavorites.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgClassifGenres_CellEndEdit);
            this.dgClassifFavorites.SelectionChanged += new System.EventHandler(this.dgClassifGenres_SelectionChanged);
            // 
            // colRatingClassID
            // 
            this.colRatingClassID.DataPropertyName = "id";
            resources.ApplyResources(this.colRatingClassID, "colRatingClassID");
            this.colRatingClassID.Name = "colRatingClassID";
            // 
            // colFid
            // 
            this.colFid.DataPropertyName = "fid";
            resources.ApplyResources(this.colFid, "colFid");
            this.colFid.Name = "colFid";
            // 
            // colType
            // 
            this.colType.DataPropertyName = "image";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = null;
            this.colType.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.colType, "colType");
            this.colType.Name = "colType";
            this.colType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colContain
            // 
            this.colContain.DataPropertyName = "contain";
            resources.ApplyResources(this.colContain, "colContain");
            this.colContain.Name = "colContain";
            // 
            // colDontContain
            // 
            this.colDontContain.DataPropertyName = "noncontain";
            resources.ApplyResources(this.colDontContain, "colDontContain");
            this.colDontContain.Name = "colDontContain";
            // 
            // colDeleteAfter
            // 
            this.colDeleteAfter.DataPropertyName = "deleteafter";
            resources.ApplyResources(this.colDeleteAfter, "colDeleteAfter");
            this.colDeleteAfter.Name = "colDeleteAfter";
            // 
            // colRemind
            // 
            this.colRemind.DataPropertyName = "remind";
            resources.ApplyResources(this.colRemind, "colRemind");
            this.colRemind.Name = "colRemind";
            // 
            // colPrior
            // 
            this.colPrior.DataPropertyName = "prior";
            resources.ApplyResources(this.colPrior, "colPrior");
            this.colPrior.Name = "colPrior";
            // 
            // ClassifFavoriteControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgClassifFavorites);
            this.Name = "ClassifFavoriteControl";
            this.Load += new System.EventHandler(this.ClassifFavoriteControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgClassifFavorites)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewExt dgClassifFavorites;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRatingClassID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFid;
        private System.Windows.Forms.DataGridViewImageColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContain;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDontContain;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeleteAfter;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colRemind;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrior;
    }
}
