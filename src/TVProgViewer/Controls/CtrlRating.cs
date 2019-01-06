﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class CtrlRating : UserControl
    {
       public CtrlRating(DataTable ratings, Capture capture)
        {
            InitializeComponent();
           if (capture != null)
           {
               chkWithRecord.Visible = chkWithoutRecord.Visible = true;
           }
            listViewRating.Items.Clear();
            imgList.Images.Clear();
            foreach (DataRow dataRow in ratings.Rows)
            {
                if (String.IsNullOrEmpty(dataRow["visible"].ToString())) dataRow["visible"] = false;
                if ((bool)dataRow["visible"])
                {
                    imgList.Images.Add(dataRow["imagename"].ToString(), (Image) dataRow["image"]);
                    listViewRating.Items.Add(dataRow["id"].ToString(),
                                               dataRow["favname"].ToString(),
                                               dataRow["imagename"].ToString()).
                    Checked = true;
                }
            }
        }

       private void CtrlRating_Load(object sender, EventArgs e)
       {
           Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
       }
    }
}
