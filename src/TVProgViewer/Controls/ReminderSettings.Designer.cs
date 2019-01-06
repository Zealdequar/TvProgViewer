﻿namespace TVProgViewer.TVProgApp.Controls
{
    partial class ReminderSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReminderSettings));
            this.pReminder = new System.Windows.Forms.Panel();
            this.chbOn = new System.Windows.Forms.CheckBox();
            this.gbRemRestriction = new System.Windows.Forms.GroupBox();
            this.tlpRemRestriction = new System.Windows.Forms.TableLayoutPanel();
            this.chbMonday = new System.Windows.Forms.CheckBox();
            this.chbTuesday = new System.Windows.Forms.CheckBox();
            this.chbWensday = new System.Windows.Forms.CheckBox();
            this.chbFriday = new System.Windows.Forms.CheckBox();
            this.chbThursday = new System.Windows.Forms.CheckBox();
            this.chbSaturday = new System.Windows.Forms.CheckBox();
            this.chbSunday = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpRemFrom1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpRemTo1 = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpRemFrom2 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemFrom3 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemFrom4 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemFrom5 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemFrom6 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemFrom7 = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.dtpRemTo2 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemTo3 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemTo4 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemTo5 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemTo6 = new System.Windows.Forms.DateTimePicker();
            this.dtpRemTo7 = new System.Windows.Forms.DateTimePicker();
            this.pMes = new System.Windows.Forms.Panel();
            this.chbFullScreen = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.numOpacity = new System.Windows.Forms.NumericUpDown();
            this.chbMessage = new System.Windows.Forms.CheckBox();
            this.gbAutoClose = new System.Windows.Forms.GroupBox();
            this.numAfterSeconds = new System.Windows.Forms.NumericUpDown();
            this.rbAfterSeconds = new System.Windows.Forms.RadioButton();
            this.rbAfterTelecast = new System.Windows.Forms.RadioButton();
            this.gbSound = new System.Windows.Forms.GroupBox();
            this.numMinutesLater = new System.Windows.Forms.NumericUpDown();
            this.chbRemindLater = new System.Windows.Forms.CheckBox();
            this.btnChangeFile = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rbSystem = new System.Windows.Forms.RadioButton();
            this.rbDynamic = new System.Windows.Forms.RadioButton();
            this.chbOnSound = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pReminder.SuspendLayout();
            this.gbRemRestriction.SuspendLayout();
            this.tlpRemRestriction.SuspendLayout();
            this.pMes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOpacity)).BeginInit();
            this.gbAutoClose.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAfterSeconds)).BeginInit();
            this.gbSound.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutesLater)).BeginInit();
            this.SuspendLayout();
            // 
            // pReminder
            // 
            this.pReminder.Controls.Add(this.chbOn);
            resources.ApplyResources(this.pReminder, "pReminder");
            this.pReminder.Name = "pReminder";
            // 
            // chbOn
            // 
            resources.ApplyResources(this.chbOn, "chbOn");
            this.chbOn.Checked = true;
            this.chbOn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbOn.Name = "chbOn";
            this.chbOn.UseVisualStyleBackColor = true;
            this.chbOn.CheckedChanged += new System.EventHandler(this.chbOn_CheckedChanged);
            // 
            // gbRemRestriction
            // 
            this.gbRemRestriction.Controls.Add(this.tlpRemRestriction);
            resources.ApplyResources(this.gbRemRestriction, "gbRemRestriction");
            this.gbRemRestriction.Name = "gbRemRestriction";
            this.gbRemRestriction.TabStop = false;
            // 
            // tlpRemRestriction
            // 
            resources.ApplyResources(this.tlpRemRestriction, "tlpRemRestriction");
            this.tlpRemRestriction.Controls.Add(this.chbMonday, 0, 0);
            this.tlpRemRestriction.Controls.Add(this.chbTuesday, 0, 1);
            this.tlpRemRestriction.Controls.Add(this.chbWensday, 0, 2);
            this.tlpRemRestriction.Controls.Add(this.chbFriday, 0, 4);
            this.tlpRemRestriction.Controls.Add(this.chbThursday, 0, 3);
            this.tlpRemRestriction.Controls.Add(this.chbSaturday, 0, 5);
            this.tlpRemRestriction.Controls.Add(this.chbSunday, 0, 6);
            this.tlpRemRestriction.Controls.Add(this.label1, 1, 0);
            this.tlpRemRestriction.Controls.Add(this.dtpRemFrom1, 2, 0);
            this.tlpRemRestriction.Controls.Add(this.label2, 3, 0);
            this.tlpRemRestriction.Controls.Add(this.dtpRemTo1, 4, 0);
            this.tlpRemRestriction.Controls.Add(this.label3, 1, 1);
            this.tlpRemRestriction.Controls.Add(this.label4, 1, 2);
            this.tlpRemRestriction.Controls.Add(this.label5, 1, 3);
            this.tlpRemRestriction.Controls.Add(this.label6, 1, 4);
            this.tlpRemRestriction.Controls.Add(this.label7, 1, 5);
            this.tlpRemRestriction.Controls.Add(this.label8, 1, 6);
            this.tlpRemRestriction.Controls.Add(this.dtpRemFrom2, 2, 1);
            this.tlpRemRestriction.Controls.Add(this.dtpRemFrom3, 2, 2);
            this.tlpRemRestriction.Controls.Add(this.dtpRemFrom4, 2, 3);
            this.tlpRemRestriction.Controls.Add(this.dtpRemFrom5, 2, 4);
            this.tlpRemRestriction.Controls.Add(this.dtpRemFrom6, 2, 5);
            this.tlpRemRestriction.Controls.Add(this.dtpRemFrom7, 2, 6);
            this.tlpRemRestriction.Controls.Add(this.label9, 3, 1);
            this.tlpRemRestriction.Controls.Add(this.label10, 3, 2);
            this.tlpRemRestriction.Controls.Add(this.label11, 3, 3);
            this.tlpRemRestriction.Controls.Add(this.label12, 3, 4);
            this.tlpRemRestriction.Controls.Add(this.label13, 3, 5);
            this.tlpRemRestriction.Controls.Add(this.label14, 3, 6);
            this.tlpRemRestriction.Controls.Add(this.dtpRemTo2, 4, 1);
            this.tlpRemRestriction.Controls.Add(this.dtpRemTo3, 4, 2);
            this.tlpRemRestriction.Controls.Add(this.dtpRemTo4, 4, 3);
            this.tlpRemRestriction.Controls.Add(this.dtpRemTo5, 4, 4);
            this.tlpRemRestriction.Controls.Add(this.dtpRemTo6, 4, 5);
            this.tlpRemRestriction.Controls.Add(this.dtpRemTo7, 4, 6);
            this.tlpRemRestriction.Name = "tlpRemRestriction";
            // 
            // chbMonday
            // 
            resources.ApplyResources(this.chbMonday, "chbMonday");
            this.chbMonday.Name = "chbMonday";
            this.chbMonday.UseVisualStyleBackColor = true;
            this.chbMonday.CheckedChanged += new System.EventHandler(this.chbMonday_CheckedChanged);
            // 
            // chbTuesday
            // 
            resources.ApplyResources(this.chbTuesday, "chbTuesday");
            this.chbTuesday.Name = "chbTuesday";
            this.chbTuesday.UseVisualStyleBackColor = true;
            this.chbTuesday.CheckedChanged += new System.EventHandler(this.chbTuesday_CheckedChanged);
            // 
            // chbWensday
            // 
            resources.ApplyResources(this.chbWensday, "chbWensday");
            this.chbWensday.Name = "chbWensday";
            this.chbWensday.UseVisualStyleBackColor = true;
            this.chbWensday.CheckedChanged += new System.EventHandler(this.chbWensday_CheckedChanged);
            // 
            // chbFriday
            // 
            resources.ApplyResources(this.chbFriday, "chbFriday");
            this.chbFriday.Name = "chbFriday";
            this.chbFriday.UseVisualStyleBackColor = true;
            this.chbFriday.CheckedChanged += new System.EventHandler(this.chbFriday_CheckedChanged);
            // 
            // chbThursday
            // 
            resources.ApplyResources(this.chbThursday, "chbThursday");
            this.chbThursday.Name = "chbThursday";
            this.chbThursday.UseVisualStyleBackColor = true;
            this.chbThursday.CheckedChanged += new System.EventHandler(this.chbThursday_CheckedChanged);
            // 
            // chbSaturday
            // 
            resources.ApplyResources(this.chbSaturday, "chbSaturday");
            this.chbSaturday.ForeColor = System.Drawing.Color.Maroon;
            this.chbSaturday.Name = "chbSaturday";
            this.chbSaturday.UseVisualStyleBackColor = true;
            this.chbSaturday.CheckedChanged += new System.EventHandler(this.chbSaturday_CheckedChanged);
            // 
            // chbSunday
            // 
            resources.ApplyResources(this.chbSunday, "chbSunday");
            this.chbSunday.ForeColor = System.Drawing.Color.Maroon;
            this.chbSunday.Name = "chbSunday";
            this.chbSunday.UseVisualStyleBackColor = true;
            this.chbSunday.CheckedChanged += new System.EventHandler(this.chbSunday_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dtpRemFrom1
            // 
            resources.ApplyResources(this.dtpRemFrom1, "dtpRemFrom1");
            this.dtpRemFrom1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemFrom1.Name = "dtpRemFrom1";
            this.dtpRemFrom1.ShowUpDown = true;
            this.dtpRemFrom1.ValueChanged += new System.EventHandler(this.dtpRemFrom1_ValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // dtpRemTo1
            // 
            resources.ApplyResources(this.dtpRemTo1, "dtpRemTo1");
            this.dtpRemTo1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemTo1.Name = "dtpRemTo1";
            this.dtpRemTo1.ShowUpDown = true;
            this.dtpRemTo1.ValueChanged += new System.EventHandler(this.dtpRemTo1_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.Color.Maroon;
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.ForeColor = System.Drawing.Color.Maroon;
            this.label8.Name = "label8";
            // 
            // dtpRemFrom2
            // 
            resources.ApplyResources(this.dtpRemFrom2, "dtpRemFrom2");
            this.dtpRemFrom2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemFrom2.Name = "dtpRemFrom2";
            this.dtpRemFrom2.ShowUpDown = true;
            this.dtpRemFrom2.ValueChanged += new System.EventHandler(this.dtpRemFrom2_ValueChanged);
            // 
            // dtpRemFrom3
            // 
            resources.ApplyResources(this.dtpRemFrom3, "dtpRemFrom3");
            this.dtpRemFrom3.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemFrom3.Name = "dtpRemFrom3";
            this.dtpRemFrom3.ShowUpDown = true;
            this.dtpRemFrom3.ValueChanged += new System.EventHandler(this.dtpRemFrom3_ValueChanged);
            // 
            // dtpRemFrom4
            // 
            resources.ApplyResources(this.dtpRemFrom4, "dtpRemFrom4");
            this.dtpRemFrom4.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemFrom4.Name = "dtpRemFrom4";
            this.dtpRemFrom4.ShowUpDown = true;
            this.dtpRemFrom4.ValueChanged += new System.EventHandler(this.dtpRemFrom4_ValueChanged);
            // 
            // dtpRemFrom5
            // 
            resources.ApplyResources(this.dtpRemFrom5, "dtpRemFrom5");
            this.dtpRemFrom5.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemFrom5.Name = "dtpRemFrom5";
            this.dtpRemFrom5.ShowUpDown = true;
            this.dtpRemFrom5.ValueChanged += new System.EventHandler(this.dtpRemFrom5_ValueChanged);
            // 
            // dtpRemFrom6
            // 
            resources.ApplyResources(this.dtpRemFrom6, "dtpRemFrom6");
            this.dtpRemFrom6.CalendarForeColor = System.Drawing.Color.Maroon;
            this.dtpRemFrom6.CalendarTitleForeColor = System.Drawing.Color.Maroon;
            this.dtpRemFrom6.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemFrom6.Name = "dtpRemFrom6";
            this.dtpRemFrom6.ShowUpDown = true;
            this.dtpRemFrom6.ValueChanged += new System.EventHandler(this.dtpRemFrom6_ValueChanged);
            // 
            // dtpRemFrom7
            // 
            resources.ApplyResources(this.dtpRemFrom7, "dtpRemFrom7");
            this.dtpRemFrom7.CalendarForeColor = System.Drawing.Color.Maroon;
            this.dtpRemFrom7.CalendarTitleForeColor = System.Drawing.Color.Maroon;
            this.dtpRemFrom7.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemFrom7.Name = "dtpRemFrom7";
            this.dtpRemFrom7.ShowUpDown = true;
            this.dtpRemFrom7.ValueChanged += new System.EventHandler(this.dtpRemFrom7_ValueChanged);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.ForeColor = System.Drawing.Color.Maroon;
            this.label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.ForeColor = System.Drawing.Color.Maroon;
            this.label14.Name = "label14";
            // 
            // dtpRemTo2
            // 
            resources.ApplyResources(this.dtpRemTo2, "dtpRemTo2");
            this.dtpRemTo2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemTo2.Name = "dtpRemTo2";
            this.dtpRemTo2.ShowUpDown = true;
            this.dtpRemTo2.ValueChanged += new System.EventHandler(this.dtpRemTo2_ValueChanged);
            // 
            // dtpRemTo3
            // 
            resources.ApplyResources(this.dtpRemTo3, "dtpRemTo3");
            this.dtpRemTo3.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemTo3.Name = "dtpRemTo3";
            this.dtpRemTo3.ShowUpDown = true;
            this.dtpRemTo3.ValueChanged += new System.EventHandler(this.dtpRemTo3_ValueChanged);
            // 
            // dtpRemTo4
            // 
            resources.ApplyResources(this.dtpRemTo4, "dtpRemTo4");
            this.dtpRemTo4.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemTo4.Name = "dtpRemTo4";
            this.dtpRemTo4.ShowUpDown = true;
            this.dtpRemTo4.ValueChanged += new System.EventHandler(this.dtpRemTo4_ValueChanged);
            // 
            // dtpRemTo5
            // 
            resources.ApplyResources(this.dtpRemTo5, "dtpRemTo5");
            this.dtpRemTo5.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemTo5.Name = "dtpRemTo5";
            this.dtpRemTo5.ShowUpDown = true;
            this.dtpRemTo5.ValueChanged += new System.EventHandler(this.dtpRemTo5_ValueChanged);
            // 
            // dtpRemTo6
            // 
            resources.ApplyResources(this.dtpRemTo6, "dtpRemTo6");
            this.dtpRemTo6.CalendarForeColor = System.Drawing.Color.Maroon;
            this.dtpRemTo6.CalendarTitleForeColor = System.Drawing.Color.Maroon;
            this.dtpRemTo6.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemTo6.Name = "dtpRemTo6";
            this.dtpRemTo6.ShowUpDown = true;
            this.dtpRemTo6.ValueChanged += new System.EventHandler(this.dtpRemTo6_ValueChanged);
            // 
            // dtpRemTo7
            // 
            resources.ApplyResources(this.dtpRemTo7, "dtpRemTo7");
            this.dtpRemTo7.CalendarForeColor = System.Drawing.Color.Maroon;
            this.dtpRemTo7.CalendarTitleForeColor = System.Drawing.Color.Maroon;
            this.dtpRemTo7.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpRemTo7.Name = "dtpRemTo7";
            this.dtpRemTo7.ShowUpDown = true;
            this.dtpRemTo7.ValueChanged += new System.EventHandler(this.dtpRemTo7_ValueChanged);
            // 
            // pMes
            // 
            this.pMes.Controls.Add(this.chbFullScreen);
            this.pMes.Controls.Add(this.label15);
            this.pMes.Controls.Add(this.numOpacity);
            this.pMes.Controls.Add(this.chbMessage);
            resources.ApplyResources(this.pMes, "pMes");
            this.pMes.Name = "pMes";
            // 
            // chbFullScreen
            // 
            this.chbFullScreen.Checked = true;
            this.chbFullScreen.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.chbFullScreen, "chbFullScreen");
            this.chbFullScreen.Name = "chbFullScreen";
            this.chbFullScreen.UseVisualStyleBackColor = true;
            this.chbFullScreen.CheckedChanged += new System.EventHandler(this.chbFullScreen_CheckedChanged);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // numOpacity
            // 
            resources.ApplyResources(this.numOpacity, "numOpacity");
            this.numOpacity.Name = "numOpacity";
            this.numOpacity.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numOpacity.ValueChanged += new System.EventHandler(this.numOpacity_ValueChanged);
            // 
            // chbMessage
            // 
            resources.ApplyResources(this.chbMessage, "chbMessage");
            this.chbMessage.Checked = true;
            this.chbMessage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbMessage.Name = "chbMessage";
            this.chbMessage.UseVisualStyleBackColor = true;
            this.chbMessage.CheckedChanged += new System.EventHandler(this.chbMessage_CheckedChanged);
            // 
            // gbAutoClose
            // 
            this.gbAutoClose.Controls.Add(this.numAfterSeconds);
            this.gbAutoClose.Controls.Add(this.rbAfterSeconds);
            this.gbAutoClose.Controls.Add(this.rbAfterTelecast);
            resources.ApplyResources(this.gbAutoClose, "gbAutoClose");
            this.gbAutoClose.Name = "gbAutoClose";
            this.gbAutoClose.TabStop = false;
            // 
            // numAfterSeconds
            // 
            resources.ApplyResources(this.numAfterSeconds, "numAfterSeconds");
            this.numAfterSeconds.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.numAfterSeconds.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAfterSeconds.Name = "numAfterSeconds";
            this.numAfterSeconds.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numAfterSeconds.ValueChanged += new System.EventHandler(this.numAfterSeconds_ValueChanged);
            // 
            // rbAfterSeconds
            // 
            resources.ApplyResources(this.rbAfterSeconds, "rbAfterSeconds");
            this.rbAfterSeconds.Checked = true;
            this.rbAfterSeconds.Name = "rbAfterSeconds";
            this.rbAfterSeconds.TabStop = true;
            this.rbAfterSeconds.UseVisualStyleBackColor = true;
            this.rbAfterSeconds.CheckedChanged += new System.EventHandler(this.rbAfterSeconds_CheckedChanged);
            // 
            // rbAfterTelecast
            // 
            resources.ApplyResources(this.rbAfterTelecast, "rbAfterTelecast");
            this.rbAfterTelecast.Name = "rbAfterTelecast";
            this.rbAfterTelecast.UseVisualStyleBackColor = true;
            this.rbAfterTelecast.CheckedChanged += new System.EventHandler(this.rbAfterTelecast_CheckedChanged);
            // 
            // gbSound
            // 
            this.gbSound.Controls.Add(this.numMinutesLater);
            this.gbSound.Controls.Add(this.chbRemindLater);
            this.gbSound.Controls.Add(this.btnChangeFile);
            this.gbSound.Controls.Add(this.textBox1);
            this.gbSound.Controls.Add(this.rbFile);
            this.gbSound.Controls.Add(this.rbSystem);
            this.gbSound.Controls.Add(this.rbDynamic);
            this.gbSound.Controls.Add(this.chbOnSound);
            resources.ApplyResources(this.gbSound, "gbSound");
            this.gbSound.Name = "gbSound";
            this.gbSound.TabStop = false;
            // 
            // numMinutesLater
            // 
            resources.ApplyResources(this.numMinutesLater, "numMinutesLater");
            this.numMinutesLater.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.numMinutesLater.Name = "numMinutesLater";
            this.numMinutesLater.ValueChanged += new System.EventHandler(this.numMinutesLater_ValueChanged);
            // 
            // chbRemindLater
            // 
            resources.ApplyResources(this.chbRemindLater, "chbRemindLater");
            this.chbRemindLater.Name = "chbRemindLater";
            this.chbRemindLater.UseVisualStyleBackColor = true;
            this.chbRemindLater.CheckedChanged += new System.EventHandler(this.chbRemindLater_CheckedChanged);
            // 
            // btnChangeFile
            // 
            resources.ApplyResources(this.btnChangeFile, "btnChangeFile");
            this.btnChangeFile.Name = "btnChangeFile";
            this.btnChangeFile.UseVisualStyleBackColor = true;
            this.btnChangeFile.Click += new System.EventHandler(this.btnChangeFile_Click);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // rbFile
            // 
            resources.ApplyResources(this.rbFile, "rbFile");
            this.rbFile.Name = "rbFile";
            this.rbFile.UseVisualStyleBackColor = true;
            this.rbFile.CheckedChanged += new System.EventHandler(this.rbFile_CheckedChanged);
            // 
            // rbSystem
            // 
            resources.ApplyResources(this.rbSystem, "rbSystem");
            this.rbSystem.Checked = true;
            this.rbSystem.Name = "rbSystem";
            this.rbSystem.TabStop = true;
            this.rbSystem.UseVisualStyleBackColor = true;
            this.rbSystem.CheckedChanged += new System.EventHandler(this.rbSystem_CheckedChanged);
            // 
            // rbDynamic
            // 
            resources.ApplyResources(this.rbDynamic, "rbDynamic");
            this.rbDynamic.Name = "rbDynamic";
            this.rbDynamic.UseVisualStyleBackColor = true;
            this.rbDynamic.CheckedChanged += new System.EventHandler(this.rbDynamic_CheckedChanged);
            // 
            // chbOnSound
            // 
            resources.ApplyResources(this.chbOnSound, "chbOnSound");
            this.chbOnSound.Checked = true;
            this.chbOnSound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbOnSound.Name = "chbOnSound";
            this.chbOnSound.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // ReminderSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbSound);
            this.Controls.Add(this.gbAutoClose);
            this.Controls.Add(this.pMes);
            this.Controls.Add(this.gbRemRestriction);
            this.Controls.Add(this.pReminder);
            this.Name = "ReminderSettings";
            this.Load += new System.EventHandler(this.ReminderSettings_Load);
            this.pReminder.ResumeLayout(false);
            this.pReminder.PerformLayout();
            this.gbRemRestriction.ResumeLayout(false);
            this.tlpRemRestriction.ResumeLayout(false);
            this.tlpRemRestriction.PerformLayout();
            this.pMes.ResumeLayout(false);
            this.pMes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOpacity)).EndInit();
            this.gbAutoClose.ResumeLayout(false);
            this.gbAutoClose.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAfterSeconds)).EndInit();
            this.gbSound.ResumeLayout(false);
            this.gbSound.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutesLater)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pReminder;
        private System.Windows.Forms.CheckBox chbOn;
        private System.Windows.Forms.GroupBox gbRemRestriction;
        private System.Windows.Forms.TableLayoutPanel tlpRemRestriction;
        private System.Windows.Forms.CheckBox chbMonday;
        private System.Windows.Forms.CheckBox chbTuesday;
        private System.Windows.Forms.CheckBox chbWensday;
        private System.Windows.Forms.CheckBox chbFriday;
        private System.Windows.Forms.CheckBox chbThursday;
        private System.Windows.Forms.CheckBox chbSaturday;
        private System.Windows.Forms.CheckBox chbSunday;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpRemFrom1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpRemTo1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtpRemFrom2;
        private System.Windows.Forms.DateTimePicker dtpRemFrom3;
        private System.Windows.Forms.DateTimePicker dtpRemFrom4;
        private System.Windows.Forms.DateTimePicker dtpRemFrom5;
        private System.Windows.Forms.DateTimePicker dtpRemFrom6;
        private System.Windows.Forms.DateTimePicker dtpRemFrom7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DateTimePicker dtpRemTo2;
        private System.Windows.Forms.DateTimePicker dtpRemTo3;
        private System.Windows.Forms.DateTimePicker dtpRemTo4;
        private System.Windows.Forms.DateTimePicker dtpRemTo5;
        private System.Windows.Forms.DateTimePicker dtpRemTo6;
        private System.Windows.Forms.DateTimePicker dtpRemTo7;
        private System.Windows.Forms.Panel pMes;
        private System.Windows.Forms.CheckBox chbMessage;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numOpacity;
        private System.Windows.Forms.CheckBox chbFullScreen;
        private System.Windows.Forms.GroupBox gbAutoClose;
        private System.Windows.Forms.NumericUpDown numAfterSeconds;
        private System.Windows.Forms.RadioButton rbAfterSeconds;
        private System.Windows.Forms.RadioButton rbAfterTelecast;
        private System.Windows.Forms.GroupBox gbSound;
        private System.Windows.Forms.Button btnChangeFile;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton rbFile;
        private System.Windows.Forms.RadioButton rbSystem;
        private System.Windows.Forms.RadioButton rbDynamic;
        private System.Windows.Forms.CheckBox chbOnSound;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.NumericUpDown numMinutesLater;
        private System.Windows.Forms.CheckBox chbRemindLater;

    }
}
