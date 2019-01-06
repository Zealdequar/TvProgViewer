﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class CtrlDays : UserControl
    {
        public CtrlDays(DateTimeOffset dateStart, DateTimeOffset dateStop)
        {
            InitializeComponent();
            listViewDays.Items.Clear();
            imgList.Images.Clear();
            imgList.Images.Add("Mon", Resources.Mon);
            imgList.Images.Add("Tue", Resources.Tue);
            imgList.Images.Add("Wen", Resources.Wen);
            imgList.Images.Add("Ths", Resources.Ths);
            imgList.Images.Add("Fri", Resources.Fri);
            imgList.Images.Add("Sat", Resources.Sat);
            imgList.Images.Add("Sun", Resources.Sun);
            for (DateTimeOffset curDate = dateStart; curDate <= dateStop; curDate = curDate.AddDays(1))
            {
                string strKey = Preferences.dictWeek[curDate.DayOfWeek.ToString().ToLower()];
                listViewDays.Items.Add(curDate.DateTime.ToShortDateString(),
                                       curDate.DateTime.ToShortDateString(), strKey).Checked = true;
            }
        }

        private void CtrlDays_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
