﻿namespace TVProgViewer.TVProgApp.Controls
{
    partial class VideoSettings
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoSettings));
            this.lblDirToSaveCapture = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tlpVideo = new System.Windows.Forms.TableLayoutPanel();
            this.lblVideoDevices = new System.Windows.Forms.Label();
            this.cbVideoDevices = new System.Windows.Forms.ComboBox();
            this.lblVideoCompressors = new System.Windows.Forms.Label();
            this.cbVideoCompressors = new System.Windows.Forms.ComboBox();
            this.lblVideoSources = new System.Windows.Forms.Label();
            this.cbVideoSources = new System.Windows.Forms.ComboBox();
            this.lblVideoStandart = new System.Windows.Forms.Label();
            this.cbVideoStandart = new System.Windows.Forms.ComboBox();
            this.lblFrameSize = new System.Windows.Forms.Label();
            this.cbFrameSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbVideoRates = new System.Windows.Forms.ComboBox();
            this.lblCaptureFileName = new System.Windows.Forms.Label();
            this.tbCaptureFileName = new System.Windows.Forms.TextBox();
            this.btnChangeFileName = new System.Windows.Forms.Button();
            this.tbDirCapture = new System.Windows.Forms.TextBox();
            this.btnChangeDir = new System.Windows.Forms.Button();
            this.lblColorSpace = new System.Windows.Forms.Label();
            this.cbColorSpace = new System.Windows.Forms.ComboBox();
            this.lblRecFileModes = new System.Windows.Forms.Label();
            this.cbAVRecFileModes = new System.Windows.Forms.ComboBox();
            this.chbDeInterlace = new System.Windows.Forms.CheckBox();
            this.chbVMR9 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnVideoCaps = new System.Windows.Forms.Button();
            this.btnAudioVideoFormat = new System.Windows.Forms.Button();
            this.tlpVideo.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDirToSaveCapture
            // 
            resources.ApplyResources(this.lblDirToSaveCapture, "lblDirToSaveCapture");
            this.lblDirToSaveCapture.Name = "lblDirToSaveCapture";
            // 
            // tlpVideo
            // 
            resources.ApplyResources(this.tlpVideo, "tlpVideo");
            this.tlpVideo.Controls.Add(this.lblVideoDevices, 0, 0);
            this.tlpVideo.Controls.Add(this.cbVideoDevices, 1, 0);
            this.tlpVideo.Controls.Add(this.lblVideoCompressors, 0, 1);
            this.tlpVideo.Controls.Add(this.cbVideoCompressors, 1, 1);
            this.tlpVideo.Controls.Add(this.lblVideoSources, 0, 2);
            this.tlpVideo.Controls.Add(this.cbVideoSources, 1, 2);
            this.tlpVideo.Controls.Add(this.lblVideoStandart, 0, 3);
            this.tlpVideo.Controls.Add(this.cbVideoStandart, 1, 3);
            this.tlpVideo.Controls.Add(this.lblFrameSize, 0, 4);
            this.tlpVideo.Controls.Add(this.cbFrameSize, 1, 4);
            this.tlpVideo.Controls.Add(this.label1, 0, 5);
            this.tlpVideo.Controls.Add(this.cbVideoRates, 1, 5);
            this.tlpVideo.Controls.Add(this.lblCaptureFileName, 0, 9);
            this.tlpVideo.Controls.Add(this.tbCaptureFileName, 1, 9);
            this.tlpVideo.Controls.Add(this.btnChangeFileName, 2, 9);
            this.tlpVideo.Controls.Add(this.lblDirToSaveCapture, 0, 8);
            this.tlpVideo.Controls.Add(this.tbDirCapture, 1, 8);
            this.tlpVideo.Controls.Add(this.btnChangeDir, 2, 8);
            this.tlpVideo.Controls.Add(this.lblColorSpace, 0, 6);
            this.tlpVideo.Controls.Add(this.cbColorSpace, 1, 6);
            this.tlpVideo.Controls.Add(this.lblRecFileModes, 0, 10);
            this.tlpVideo.Controls.Add(this.cbAVRecFileModes, 1, 10);
            this.tlpVideo.Name = "tlpVideo";
            // 
            // lblVideoDevices
            // 
            resources.ApplyResources(this.lblVideoDevices, "lblVideoDevices");
            this.lblVideoDevices.Name = "lblVideoDevices";
            // 
            // cbVideoDevices
            // 
            resources.ApplyResources(this.cbVideoDevices, "cbVideoDevices");
            this.cbVideoDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoDevices.FormattingEnabled = true;
            this.cbVideoDevices.Name = "cbVideoDevices";
            // 
            // lblVideoCompressors
            // 
            resources.ApplyResources(this.lblVideoCompressors, "lblVideoCompressors");
            this.lblVideoCompressors.Name = "lblVideoCompressors";
            // 
            // cbVideoCompressors
            // 
            resources.ApplyResources(this.cbVideoCompressors, "cbVideoCompressors");
            this.cbVideoCompressors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoCompressors.FormattingEnabled = true;
            this.cbVideoCompressors.Name = "cbVideoCompressors";
            // 
            // lblVideoSources
            // 
            resources.ApplyResources(this.lblVideoSources, "lblVideoSources");
            this.lblVideoSources.Name = "lblVideoSources";
            // 
            // cbVideoSources
            // 
            resources.ApplyResources(this.cbVideoSources, "cbVideoSources");
            this.cbVideoSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoSources.FormattingEnabled = true;
            this.cbVideoSources.Name = "cbVideoSources";
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
            // lblFrameSize
            // 
            resources.ApplyResources(this.lblFrameSize, "lblFrameSize");
            this.lblFrameSize.Name = "lblFrameSize";
            // 
            // cbFrameSize
            // 
            resources.ApplyResources(this.cbFrameSize, "cbFrameSize");
            this.cbFrameSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFrameSize.FormattingEnabled = true;
            this.cbFrameSize.Name = "cbFrameSize";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbVideoRates
            // 
            resources.ApplyResources(this.cbVideoRates, "cbVideoRates");
            this.cbVideoRates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoRates.FormattingEnabled = true;
            this.cbVideoRates.Name = "cbVideoRates";
            // 
            // lblCaptureFileName
            // 
            resources.ApplyResources(this.lblCaptureFileName, "lblCaptureFileName");
            this.lblCaptureFileName.Name = "lblCaptureFileName";
            // 
            // tbCaptureFileName
            // 
            resources.ApplyResources(this.tbCaptureFileName, "tbCaptureFileName");
            this.tbCaptureFileName.Name = "tbCaptureFileName";
            // 
            // btnChangeFileName
            // 
            resources.ApplyResources(this.btnChangeFileName, "btnChangeFileName");
            this.btnChangeFileName.Name = "btnChangeFileName";
            this.btnChangeFileName.UseVisualStyleBackColor = true;
            this.btnChangeFileName.Click += new System.EventHandler(this.btnChangeFileName_Click);
            // 
            // tbDirCapture
            // 
            resources.ApplyResources(this.tbDirCapture, "tbDirCapture");
            this.tbDirCapture.Name = "tbDirCapture";
            // 
            // btnChangeDir
            // 
            resources.ApplyResources(this.btnChangeDir, "btnChangeDir");
            this.btnChangeDir.Name = "btnChangeDir";
            this.btnChangeDir.UseVisualStyleBackColor = true;
            this.btnChangeDir.Click += new System.EventHandler(this.btnChangeDir_Click);
            // 
            // lblColorSpace
            // 
            resources.ApplyResources(this.lblColorSpace, "lblColorSpace");
            this.lblColorSpace.Name = "lblColorSpace";
            // 
            // cbColorSpace
            // 
            resources.ApplyResources(this.cbColorSpace, "cbColorSpace");
            this.cbColorSpace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColorSpace.FormattingEnabled = true;
            this.cbColorSpace.Name = "cbColorSpace";
            // 
            // lblRecFileModes
            // 
            resources.ApplyResources(this.lblRecFileModes, "lblRecFileModes");
            this.lblRecFileModes.Name = "lblRecFileModes";
            // 
            // cbAVRecFileModes
            // 
            resources.ApplyResources(this.cbAVRecFileModes, "cbAVRecFileModes");
            this.cbAVRecFileModes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAVRecFileModes.FormattingEnabled = true;
            this.cbAVRecFileModes.Name = "cbAVRecFileModes";
            // 
            // chbDeInterlace
            // 
            resources.ApplyResources(this.chbDeInterlace, "chbDeInterlace");
            this.chbDeInterlace.Name = "chbDeInterlace";
            this.chbDeInterlace.UseVisualStyleBackColor = true;
            this.chbDeInterlace.CheckedChanged += new System.EventHandler(this.chbDeInterlace_CheckedChanged);
            // 
            // chbVMR9
            // 
            resources.ApplyResources(this.chbVMR9, "chbVMR9");
            this.chbVMR9.Name = "chbVMR9";
            this.chbVMR9.UseVisualStyleBackColor = true;
            this.chbVMR9.CheckedChanged += new System.EventHandler(this.chbVMR9_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnVideoCaps, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chbDeInterlace, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chbVMR9, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAudioVideoFormat, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // btnVideoCaps
            // 
            resources.ApplyResources(this.btnVideoCaps, "btnVideoCaps");
            this.btnVideoCaps.Name = "btnVideoCaps";
            this.btnVideoCaps.UseVisualStyleBackColor = true;
            this.btnVideoCaps.Click += new System.EventHandler(this.btnVideoCaps_Click);
            // 
            // btnAudioVideoFormat
            // 
            resources.ApplyResources(this.btnAudioVideoFormat, "btnAudioVideoFormat");
            this.btnAudioVideoFormat.Name = "btnAudioVideoFormat";
            this.btnAudioVideoFormat.UseVisualStyleBackColor = true;
            this.btnAudioVideoFormat.Click += new System.EventHandler(this.btnAudioVideoFormat_Click);
            // 
            // VideoSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tlpVideo);
            this.Name = "VideoSettings";
            this.Load += new System.EventHandler(this.VideoSettings_Load);
            this.tlpVideo.ResumeLayout(false);
            this.tlpVideo.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDirToSaveCapture;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TableLayoutPanel tlpVideo;
        private System.Windows.Forms.Label lblCaptureFileName;
        private System.Windows.Forms.TextBox tbCaptureFileName;
        private System.Windows.Forms.TextBox tbDirCapture;
        private System.Windows.Forms.Button btnChangeDir;
        private System.Windows.Forms.Button btnChangeFileName;
        private System.Windows.Forms.Label lblVideoDevices;
        private System.Windows.Forms.ComboBox cbVideoDevices;
        private System.Windows.Forms.Label lblVideoCompressors;
        private System.Windows.Forms.ComboBox cbVideoCompressors;
        private System.Windows.Forms.CheckBox chbDeInterlace;
        private System.Windows.Forms.CheckBox chbVMR9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblVideoSources;
        private System.Windows.Forms.ComboBox cbVideoSources;
        private System.Windows.Forms.Label lblVideoStandart;
        private System.Windows.Forms.ComboBox cbVideoStandart;
        private System.Windows.Forms.Label lblFrameSize;
        private System.Windows.Forms.ComboBox cbFrameSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbVideoRates;
        private System.Windows.Forms.Label lblColorSpace;
        private System.Windows.Forms.ComboBox cbColorSpace;
        private System.Windows.Forms.Button btnVideoCaps;
        private System.Windows.Forms.Label lblRecFileModes;
        private System.Windows.Forms.ComboBox cbAVRecFileModes;
        private System.Windows.Forms.Button btnAudioVideoFormat;
    }
}
