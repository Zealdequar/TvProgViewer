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
    public partial class GenreControl : UserControl
    {
        private Genres _genres;
        public GenreControl(Genres genres)
        {
            InitializeComponent();
            _genres = genres;
            dgGenres.Style(DataGridViewExtentions.Styles.TVGridWithEdit);
            dgGenres.FitColumns(false, false, true);
        }

        private void genreControl_Load(object sender, EventArgs e)
        {
            imgList.Images.Clear();
            imgList.Images.Add("gen", Resources.GenreEditor);
            dgGenres.DataSource = _genres.GenresTable;
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }

        private void dgGenres_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.ToString() == Resources.GenreText && e.RowIndex == -1)
                {
                    e.PaintBackground(e.ClipBounds, false);

                    Point pt = e.CellBounds.Location;
                    int offset = (e.CellBounds.Width - this.imgList.Images["gen"].Size.Width)/2;
                    pt.X += offset;
                    pt.Y += (e.CellBounds.Height - this.imgList.Images["gen"].Size.Height)/2;
                    this.imgList.Draw(e.Graphics, pt, 0);
                    e.Handled = true;
                }
            }
        }

        private void dgGenres_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Parent != null)
                if (Parent.Parent != null)
                {
                    (Parent.Parent as GenreForm).BtnSaveEnabled = true;
                }
        }

        private void dgGenres_SelectionChanged(object sender, EventArgs e)
        {
            if (dgGenres.CurrentCell != null)
            {
                if (Parent != null)
                    if (Parent.Parent != null)
                    {
                        (Parent.Parent as GenreForm).BtnEditEnabled =
                            (Parent.Parent as GenreForm).BtnDeleteEnabled = true;
                    }
                if (dgGenres.CurrentCell.RowIndex == 0)
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as GenreForm).BtnUpEnabled = false;
                        }
                }
                else
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as GenreForm).BtnUpEnabled = true;
                        }
                }
                if (dgGenres.CurrentCell.RowIndex == dgGenres.Rows.Count - 1)
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as GenreForm).BtnDownEnabled = false;
                        }
                }
                else
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as GenreForm).BtnDownEnabled = true;
                        }
                }
            }
            else
            {
                if (Parent != null)
                    if (Parent.Parent != null)
                    {
                        (Parent.Parent as GenreForm).BtnEditEnabled =
                            (Parent.Parent as GenreForm).BtnDeleteEnabled = 
                            (Parent.Parent as GenreForm).BtnDownEnabled = 
                            (Parent.Parent as GenreForm).BtnUpEnabled = false;
                    }
            }
        }
    }
}
