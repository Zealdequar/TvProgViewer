﻿namespace TVProgViewer.TVProgApp
{
    partial class CtrlGenre
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlGenre));
            this.listViewGenre = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listViewGenre
            // 
            resources.ApplyResources(this.listViewGenre, "listViewGenre");
            this.listViewGenre.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewGenre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewGenre.CheckBoxes = true;
            this.listViewGenre.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.listViewGenre.HotTracking = true;
            this.listViewGenre.HoverSelection = true;
            this.listViewGenre.Name = "listViewGenre";
            this.listViewGenre.SmallImageList = this.imgList;
            this.listViewGenre.UseCompatibleStateImageBehavior = false;
            this.listViewGenre.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.imgList, "imgList");
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // CtrlGenre
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewGenre);
            this.Name = "CtrlGenre";
            this.Load += new System.EventHandler(this.CtrlGenre_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewGenre;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ImageList imgList;
    }
}
