﻿namespace TVProgViewer.TVProgApp.Dialogs
{
    partial class MessageDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageDialog));
            this.pbFone = new System.Windows.Forms.PictureBox();
            this.pButtons = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbFone)).BeginInit();
            this.SuspendLayout();
            // 
            // pbFone
            // 
            this.pbFone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbFone.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbFone.Image = ((System.Drawing.Image)(resources.GetObject("pbFone.Image")));
            this.pbFone.Location = new System.Drawing.Point(0, 0);
            this.pbFone.Margin = new System.Windows.Forms.Padding(4);
            this.pbFone.Name = "pbFone";
            this.pbFone.Size = new System.Drawing.Size(498, 223);
            this.pbFone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFone.TabIndex = 0;
            this.pbFone.TabStop = false;
            this.pbFone.Paint += new System.Windows.Forms.PaintEventHandler(this.pbFone_Paint);
            // 
            // pButtons
            // 
            this.pButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pButtons.Location = new System.Drawing.Point(0, 223);
            this.pButtons.Name = "pButtons";
            this.pButtons.Size = new System.Drawing.Size(498, 52);
            this.pButtons.TabIndex = 1;
            // 
            // MessageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 275);
            this.Controls.Add(this.pButtons);
            this.Controls.Add(this.pbFone);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessageDialog";
            this.Load += new System.EventHandler(this.MessageDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbFone)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbFone;
        private System.Windows.Forms.Panel pButtons;
    }
}