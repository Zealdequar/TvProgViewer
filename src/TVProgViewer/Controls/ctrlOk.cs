﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Controls
{
    public partial class ctrlOk : UserControl
    {
        public ctrlOk()
        {
            InitializeComponent();
        }

        private void ctrlOk_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
