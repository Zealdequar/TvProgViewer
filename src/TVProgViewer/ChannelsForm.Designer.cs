﻿namespace TVProgViewer.TVProgApp
{
    partial class ChannelsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripChannel = new System.Windows.Forms.ToolStrip();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.btnSrcRefresh = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblNumberChannel = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCheckedChannel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnExpOpt = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnExit = new System.Windows.Forms.ToolStripButton();
            this.dgOptChannels = new TVProgViewer.TVProgApp.DataGridViewExt();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUserChannelID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOnOff = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colChannelName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFreq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStripChannel.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOptChannels)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripChannel
            // 
            resources.ApplyResources(this.toolStripChannel, "toolStripChannel");
            this.toolStripChannel.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStripChannel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnEdit,
            this.btnRemove,
            this.btnSrcRefresh});
            this.toolStripChannel.Name = "toolStripChannel";
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnSrcRefresh
            // 
            this.btnSrcRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnSrcRefresh, "btnSrcRefresh");
            this.btnSrcRefresh.Name = "btnSrcRefresh";
            this.btnSrcRefresh.Click += new System.EventHandler(this.btnSrcRefresh_Click);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblNumberChannel,
            this.lblCheckedChannel});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // lblNumberChannel
            // 
            this.lblNumberChannel.Name = "lblNumberChannel";
            resources.ApplyResources(this.lblNumberChannel, "lblNumberChannel");
            // 
            // lblCheckedChannel
            // 
            this.lblCheckedChannel.Name = "lblCheckedChannel";
            resources.ApplyResources(this.lblCheckedChannel, "lblCheckedChannel");
            // 
            // toolStrip2
            // 
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExpOpt,
            this.btnSave,
            this.btnExit});
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            // 
            // btnExpOpt
            // 
            resources.ApplyResources(this.btnExpOpt, "btnExpOpt");
            this.btnExpOpt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExpOpt.Name = "btnExpOpt";
            this.btnExpOpt.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            this.btnExpOpt.Click += new System.EventHandler(this.btnExpOpt_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Margin = new System.Windows.Forms.Padding(3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(3);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // dgOptChannels
            // 
            this.dgOptChannels.AllowUserToAddRows = false;
            this.dgOptChannels.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.dgOptChannels.BackgroundColor2 = System.Drawing.Color.Empty;
            this.dgOptChannels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgOptChannels.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colUserChannelID,
            this.colOnOff,
            this.colImage,
            this.colChannelName,
            this.colDisplayName,
            this.colNumber,
            this.colDiff,
            this.colFreq});
            resources.ApplyResources(this.dgOptChannels, "dgOptChannels");
            this.dgOptChannels.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.dgOptChannels.GridColor = System.Drawing.SystemColors.ScrollBar;
            this.dgOptChannels.IsGradient = false;
            this.dgOptChannels.Name = "dgOptChannels";
            this.dgOptChannels.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgChannels_CellPainting);
            this.dgOptChannels.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgChannels_CellValueChanged);
            this.dgOptChannels.SelectionChanged += new System.EventHandler(this.dgChannels_SelectionChanged);
            // 
            // colId
            // 
            this.colId.DataPropertyName = "ChannelID";
            resources.ApplyResources(this.colId, "colId");
            this.colId.Name = "colId";
            // 
            // colUserChannelID
            // 
            this.colUserChannelID.DataPropertyName = "UserChannelID";
            resources.ApplyResources(this.colUserChannelID, "colUserChannelID");
            this.colUserChannelID.Name = "colUserChannelID";
            this.colUserChannelID.ReadOnly = true;
            // 
            // colOnOff
            // 
            this.colOnOff.DataPropertyName = "Visible";
            resources.ApplyResources(this.colOnOff, "colOnOff");
            this.colOnOff.Name = "colOnOff";
            // 
            // colImage
            // 
            this.colImage.DataPropertyName = "Emblem25";
            resources.ApplyResources(this.colImage, "colImage");
            this.colImage.Name = "colImage";
            this.colImage.ReadOnly = true;
            // 
            // colChannelName
            // 
            this.colChannelName.DataPropertyName = "SystemTitle";
            resources.ApplyResources(this.colChannelName, "colChannelName");
            this.colChannelName.Name = "colChannelName";
            this.colChannelName.ReadOnly = true;
            // 
            // colDisplayName
            // 
            this.colDisplayName.DataPropertyName = "UserTitle";
            resources.ApplyResources(this.colDisplayName, "colDisplayName");
            this.colDisplayName.Name = "colDisplayName";
            // 
            // colNumber
            // 
            this.colNumber.DataPropertyName = "OrderCol";
            resources.ApplyResources(this.colNumber, "colNumber");
            this.colNumber.Name = "colNumber";
            // 
            // colDiff
            // 
            this.colDiff.DataPropertyName = "Diff";
            dataGridViewCellStyle1.Format = "##:##";
            dataGridViewCellStyle1.NullValue = null;
            this.colDiff.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.colDiff, "colDiff");
            this.colDiff.Name = "colDiff";
            // 
            // colFreq
            // 
            this.colFreq.DataPropertyName = "Freq";
            resources.ApplyResources(this.colFreq, "colFreq");
            this.colFreq.Name = "colFreq";
            // 
            // ChannelsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgOptChannels);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStripChannel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ChannelsForm";
            this.Load += new System.EventHandler(this.ChannelsForm_Load);
            this.toolStripChannel.ResumeLayout(false);
            this.toolStripChannel.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOptChannels)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridViewExt dgOptChannels;
        private System.Windows.Forms.ToolStrip toolStripChannel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnExpOpt;
        private System.Windows.Forms.ToolStripStatusLabel lblNumberChannel;
        private System.Windows.Forms.ToolStripStatusLabel lblCheckedChannel;
        private System.Windows.Forms.ToolStripButton btnExit;
        private System.Windows.Forms.ToolStripButton btnSrcRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserChannelID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colOnOff;
        private System.Windows.Forms.DataGridViewImageColumn colImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChannelName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisplayName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiff;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFreq;
    }
}