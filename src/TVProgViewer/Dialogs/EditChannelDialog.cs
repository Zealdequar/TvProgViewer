﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class EditChannelDialog : Form
    {
        private Channel _chan = null;
        public EditChannelDialog(Channel chan)
        {
            InitializeComponent();
            _chan = chan;
            lblChannel.Text = chan.Name;
            chbShowChannel.Checked = chan.Visible;
            numNumber.Value = chan.Number;
            pbImage.Image = chan.Emblem;
            tbmDiff.Text = chan.Diff;
            tbSyn.Text = chan.UserSyn;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _chan.Visible = chbShowChannel.Checked;
            _chan.Number = (uint) numNumber.Value;
            _chan.Emblem = pbImage.Image;
            _chan.Diff = tbmDiff.Text;
            _chan.UserSyn = tbSyn.Text;
        }

        private void EditChannelDialog_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
