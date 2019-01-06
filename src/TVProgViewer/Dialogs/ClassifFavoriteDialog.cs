﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVProgViewer.TVProgApp
{
    public partial class ClassifFavoriteDialog : Form
    {
        private ClassifFavorite _classifFavorite = null;
        private static Favorites _favorites;
        public ClassifFavorite ClassFavorite
        {
            get { return _classifFavorite; }
        }

        public bool Edit { get; set; }
        public ClassifFavoriteDialog(Favorites favorites)
        {
            InitializeComponent();
            _favorites = favorites;
            _classifFavorite =  new ClassifFavorite();
            BindTaskPriorityCombo(cbFavorite, _favorites);
        }
        public ClassifFavoriteDialog(Favorites favorites, string txtTitle)
        {
            InitializeComponent();
            Edit = false;
            _favorites = favorites;
            _classifFavorite = new ClassifFavorite();
            BindTaskPriorityCombo(cbFavorite, _favorites);
            tbContain.Text = txtTitle;
            foreach (DataRow drClassif in _favorites.ClassifTable.Rows)
            {
                if (txtTitle.ToLower().Contains(drClassif["contain"].ToString().ToLower()))
                {
                    tbContain.Text = drClassif["contain"].ToString();
                    tbNonContain.Text = drClassif["noncontain"].ToString();
                    foreach (DataRow drFavorite in _favorites.FavoritesTable.Rows)
                    {
                        if ((int)drFavorite["id"] == (int)drClassif["fid"])
                        {
                            cbFavorite.Text = drFavorite["favname"].ToString();
                            break;
                        }

                    }
                    if (!String.IsNullOrEmpty(drClassif["deleteafter"].ToString()))
                    {
                        if (DateTime.Parse(drClassif["deleteafter"].ToString()).Year >= 2011) 
                        {
                            dtpDeleteAfter.Checked = true;
                            dtpDeleteAfter.Value = DateTime.Parse(drClassif["deleteafter"].ToString());
                        }
                    }
                    chkRemind.Checked = (bool) drClassif["remind"];
                    Edit = true;
                    break;
                }
            }
        }

        public ClassifFavoriteDialog(Favorites favorites, ClassifFavorite cf)
        {
            InitializeComponent();
            _favorites = favorites;
            BindTaskPriorityCombo(cbFavorite, _favorites);
            _classifFavorite = cf;
            tbContain.Text = cf.Contain;
            tbNonContain.Text = cf.NonContain;
            foreach (DataRow drFavorite in _favorites.FavoritesTable.Rows)
            {
                if ((int)drFavorite["id"] == cf.IdFav)
                {
                    cbFavorite.Text = drFavorite["favname"].ToString();
                    break;
                }
            }
            if (cf.TsDeleteAfter.Year >= 2011)
            {
                dtpDeleteAfter.Checked = true;
                dtpDeleteAfter.Value = cf.TsDeleteAfter;    
            }
            chkRemind.Checked = cf.Remind;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _classifFavorite.Contain = tbContain.Text;
            _classifFavorite.NonContain = tbNonContain.Text;
            DataRow[] drsFavorites = _favorites.FavoritesTable.Select("favname = '" + cbFavorite.Text + "'");
            foreach (DataRow drFavorite in drsFavorites)
            {
                _classifFavorite.IdFav = (int) drFavorite["id"];
            }
            if (dtpDeleteAfter.Checked)
            {
                _classifFavorite.TsDeleteAfter = dtpDeleteAfter.Value.Date;
            }
            _classifFavorite.Remind = chkRemind.Checked;
        }
        
        public static void BindTaskPriorityCombo(ComboBox priorityCombo, Favorites favorites)
        {
            priorityCombo.DrawMode = DrawMode.OwnerDrawVariable;
            priorityCombo.DrawItem += new DrawItemEventHandler(priorityCombo_DrawItem);
            priorityCombo.Items.Clear();
            foreach (DataRow drFavorite in favorites.FavoritesTable.Rows)
            {
                priorityCombo.Items.Add(drFavorite["favname"].ToString());
            }
            priorityCombo.SelectedIndex = 0;
        }

        static void priorityCombo_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                ComboBox cmbPriority = sender as ComboBox;
                string text = cmbPriority.Items[e.Index].ToString(); 

                if (text.Length > 0)
                {
                    string priority = text;
                    Image img = GetTaskPriorityImage(priority, _favorites);

                    if (img != null)
                    {
                        e.Graphics.DrawImage(img, e.Bounds.X, e.Bounds.Y, 15, 15);
                    }
                }

                e.Graphics.DrawString(text, cmbPriority.Font, System.Drawing.Brushes.Black,
                                      new RectangleF(e.Bounds.X + 15, e.Bounds.Y,
                                                     e.Bounds.Width, e.Bounds.Height));

                e.DrawFocusRectangle();
            }
        }

        public static Image GetTaskPriorityImage(string priority, Favorites favorites)
        {
            foreach (DataRow favorite in favorites.FavoritesTable.Rows)
            {
                if (priority == favorite["favname"].ToString())
                {
                    return (Image) favorite["image"];
                }
            }
            return null;

        }

        private void ClassifGenreDialog_Load(object sender, EventArgs e)
        {
        }

        private void tbContain_TextChanged(object sender, EventArgs e)
        {
            if (Edit) Edit = false;
        }
    }
}
