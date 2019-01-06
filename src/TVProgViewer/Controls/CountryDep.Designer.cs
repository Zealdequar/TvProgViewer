﻿namespace TVProgViewer.TVProgApp.Controls
{
    partial class CountryDep
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CountryDep));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblCodeCountry = new System.Windows.Forms.Label();
            this.tbCountryCode = new System.Windows.Forms.TextBox();
            this.tbTuningSpace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTuningSpace = new System.Windows.Forms.Label();
            this.lblVideoStandart = new System.Windows.Forms.Label();
            this.cbVideoStandart = new System.Windows.Forms.ComboBox();
            this.cbInputType = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lblCodeCountry, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbCountryCode, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbTuningSpace, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblTuningSpace, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblVideoStandart, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.cbVideoStandart, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.cbInputType, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lblCodeCountry
            // 
            resources.ApplyResources(this.lblCodeCountry, "lblCodeCountry");
            this.lblCodeCountry.Name = "lblCodeCountry";
            // 
            // tbCountryCode
            // 
            resources.ApplyResources(this.tbCountryCode, "tbCountryCode");
            this.tbCountryCode.Name = "tbCountryCode";
            // 
            // tbTuningSpace
            // 
            resources.ApplyResources(this.tbTuningSpace, "tbTuningSpace");
            this.tbTuningSpace.Name = "tbTuningSpace";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lblTuningSpace
            // 
            resources.ApplyResources(this.lblTuningSpace, "lblTuningSpace");
            this.lblTuningSpace.Name = "lblTuningSpace";
            // 
            // lblVideoStandart
            // 
            resources.ApplyResources(this.lblVideoStandart, "lblVideoStandart");
            this.lblVideoStandart.Name = "lblVideoStandart";
            // 
            // cbVideoStandart
            // 
            resources.ApplyResources(this.cbVideoStandart, "cbVideoStandart");
            this.cbVideoStandart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoStandart.FormattingEnabled = true;
            this.cbVideoStandart.Name = "cbVideoStandart";
            // 
            // cbInputType
            // 
            resources.ApplyResources(this.cbInputType, "cbInputType");
            this.cbInputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputType.FormattingEnabled = true;
            this.cbInputType.Name = "cbInputType";
            // 
            // CountryDep
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CountryDep";
            this.Load += new System.EventHandler(this.CountryDep_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblCodeCountry;
        private System.Windows.Forms.TextBox tbCountryCode;
        private System.Windows.Forms.TextBox tbTuningSpace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTuningSpace;
        private System.Windows.Forms.Label lblVideoStandart;
        private System.Windows.Forms.ComboBox cbVideoStandart;
        private System.Windows.Forms.ComboBox cbInputType;
    }
}
