﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Dialogs
{
    public partial class AsfForm : Form
    {
        private Capture _capture = null;
        public AsfForm(Capture capture)
        {
            InitializeComponent();
            _capture = capture;
        }

        private bool _formatChanged = false;
        private bool _indexChanged = false;

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Хранилище данных формы:
            if (_formatChanged)
            {
                if (cbAsf.SelectedIndex >= 0 &&
                    cbAsf.SelectedIndex < _capture.AsfFormat.NbrAsfItems())
                {
                    _capture.AsfFormat.CurrentAsfItem = cbAsf.SelectedIndex; // Правильный индекс
                }
                _formatChanged = false;
            }

            if (_indexChanged)
            {
                _capture.AsfFormat.UseIndex = chbIndex.Checked;
                _indexChanged = false;
            }
        }

        private void AsfForm_Load(object sender, EventArgs e)
        {
            if ((_capture == null) || 
                (_capture != null) && (_capture.AsfFormat == null))
            {
                MessageBox.Show(Resources.FormatNotLoadedText);
                Close();
            }
            else
            {
                cbAsf.Items.Clear();
                // Загрузка релевантных данных:
                for (int i = 0; i < _capture.AsfFormat.NbrAsfItems(); i++)
                {
                    cbAsf.Items.Add(_capture.AsfFormat[i].Name.ToString());
                }
                if ((_capture.AsfFormat.CurrentAsfItem >= 0) &&
                    (_capture.AsfFormat.CurrentAsfItem < _capture.AsfFormat.NbrAsfItems()))
                {
                    cbAsf.SelectedIndex = _capture.AsfFormat.CurrentAsfItem;
                    _formatChanged = false; // сбросить значение, будет установлено на предыдущую строку
                }
                chbIndex.Checked = _capture.AsfFormat.UseIndex;
            }
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }

        private void AsfForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _capture = null;
        }

        private void cbAsf_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Отметить, что выбор произведен
            _formatChanged = true;

            // Показать Asf информацию
            if ((cbAsf.SelectedIndex >= 0) &&
                (cbAsf.SelectedIndex < _capture.AsfFormat.NbrAsfItems()))
            {
                lblDescription.Text = _capture.AsfFormat[cbAsf.SelectedIndex].Description.ToString();

                if (_capture.AsfFormat[this.cbAsf.SelectedIndex].Audio)
                {
                    int bitrate = _capture.AsfFormat[cbAsf.SelectedIndex].AudioBitrate / 1000;
                    lblAudioInfo.Text = Resources.AudioText + " (" + bitrate.ToString() + " " + Resources.KbitText + ")";
                }
                else
                {
                    this.lblAudioInfo.Text = "";
                }

                if (_capture.AsfFormat[cbAsf.SelectedIndex].Video)
                {
                    int bitrate = _capture.AsfFormat[this.cbAsf.SelectedIndex].VideoBitrate / 1000;
                    if (bitrate == 0)
                    {
                        this.lblVideoInfo.Text = Resources.VideoVariableBitrateText;
                    }
                    else
                    {
                        this.lblVideoInfo.Text = Resources.VideoText + " (" + bitrate.ToString() + " " + Resources.KbitText + ")";
                    }
                }
                else
                {
                    this.lblVideoInfo.Text = "";
                }
            }
        }

        private void chbIndex_CheckedChanged(object sender, EventArgs e)
        {
            _indexChanged = true;
        }


    }
}
