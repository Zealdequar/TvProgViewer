﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class SearchResultsForm : Form
    {
        private TVProgClass _tvProg;
        private DataTable _filterTable;
        private static readonly string _exp_folder = Application.StartupPath + "\\Templates\\";
        public SearchResultsForm(TVProgClass tvProg, DataTable filterTable)
        {
            InitializeComponent();
            _tvProg = tvProg;
            _filterTable = filterTable;
           
            dgSearching.Style(DataGridViewExtentions.Styles.TVGridView);
            dgSearching.FitColumns(false, true, true);
            foreach (DataRow drGenre in Preferences.genres.GenresTable.Rows)
            {
                if ((bool) drGenre["visible"])
                {
                    ToolStripMenuItem tsmiGenre = new ToolStripMenuItem(
                        drGenre["genrename"].ToString(), (Image)drGenre["image"], 
                        new EventHandler(SetGenre_Click));
                    csmiChangeType.DropDownItems.Add(tsmiGenre);
                }
            }
            colChanImage.Visible = Settings.Default.FlagLogoTable;
            colNumChannel.Visible = Settings.Default.FlagNumChan;
            foreach (DataRow drRating in Preferences.favorites.FavoritesTable.Rows)
            {
                if ((bool) drRating["visible"])
                {
                    ToolStripMenuItem tsmiRating = new ToolStripMenuItem(
                        drRating["favname"].ToString(),
                        (Image)drRating["image"],
                        new EventHandler(SetRating_Click));
                    csmiChangeRating.DropDownItems.Add(tsmiRating);
                }
            }
        }

        /// <summary>
        /// Получение данных о передаче в текущей строке
        /// </summary>
        /// <param name="idChan">код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="from">Дата начала передачи</param>
        /// <param name="to">Дата окончания передачи</param>
        /// <param name="foundRow">Строка в DataTable</param>
        private void GetTelecastInfo(ref string idChan, ref string title,
            ref DateTime from, ref DateTime to, ref DataRow foundRow)
        {
            if (dgSearching.CurrentRow != null)
            {
                idChan = dgSearching.CurrentRow.Cells["colCID"].Value.ToString();
                title = dgSearching.CurrentRow.Cells["colChanProg"].Value.ToString();
                from = (DateTime) dgSearching.CurrentRow.Cells["colChanFrom"].Value;
                to = (DateTime) dgSearching.CurrentRow.Cells["colChanTo"].Value;
                if (Owner != null && Owner is MainForm)
                    foundRow = (Owner as MainForm).Programms.Rows.Find(
                        long.Parse(dgSearching.CurrentRow.Cells["colChanID"].Value.ToString()));
            }
        }

        /// <summary>
        /// КМ: изменить жанр
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetGenre_Click(object sender, EventArgs e)
        {
            // Инициализация:
            string idChan = String.Empty;       // - код канала
            string title = String.Empty;        // - название передачи
            DateTime from = DateTime.MinValue;  // - дата начала передачи
            DateTime to = DateTime.MinValue;    // - дата завершения передачи
            DataRow foundRow = null;            // - строка передачи в DataTable
            GetTelecastInfo(ref idChan, ref title, ref from, ref to, ref foundRow);
            if (idChan != String.Empty && title != String.Empty &&
                from != DateTime.MinValue && to != DateTime.MinValue && sender is ToolStripMenuItem)
            {
                if (_tvProg.SetGenreXml(idChan, title, from, to, (sender as ToolStripMenuItem).Text))
                {
                    if (foundRow != null)
                    {
                        foundRow.BeginEdit();
                        foundRow["category"] = (sender as ToolStripMenuItem).Text;
                        foundRow.EndEdit();
                        dgSearching.CurrentRow.Cells["colGenre"].Value = (sender as ToolStripMenuItem).Image;
                        dgSearching.CurrentRow.Cells["colGenre"].ToolTipText = (sender as ToolStripMenuItem).Text;
                        dgSearching.CurrentRow.Cells["colProgCategory"].Value = (sender as ToolStripMenuItem).Text;
                        dgSearching.Refresh();
                    }
                }
            }
        }
        /// <summary>
        /// КМ: изменть рейтинг
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRating_Click(object sender, EventArgs e)
        {
            // Инициализация:
            string idChan = String.Empty;       // - код канала
            string title = String.Empty;        // - название передачи
            DateTime from = DateTime.MinValue;  // - дата начала передачи
            DateTime to = DateTime.MinValue;    // - дата завершения передачи
            DataRow foundRow = null;            // - строка передачи в DataTable
            GetTelecastInfo(ref idChan, ref title, ref from, ref to, ref foundRow);
            if (idChan != String.Empty && title != String.Empty &&
                from != DateTime.MinValue && to != DateTime.MinValue && sender is ToolStripMenuItem)
            {
                if (_tvProg.SetRatingXml(idChan, title, from, to, (sender as ToolStripMenuItem).Text))
                {
                    if (foundRow != null)
                    {
                        foundRow.BeginEdit();
                        foundRow["favname"] = (sender as ToolStripMenuItem).Text;
                        foundRow.EndEdit();
                    }
                    dgSearching.CurrentRow.Cells["colRating"].Value = (sender as ToolStripMenuItem).Image;
                    dgSearching.CurrentRow.Cells["colRating"].ToolTipText = (sender as ToolStripMenuItem).Text;
                    dgSearching.CurrentRow.Cells["colFavname"].Value = (sender as ToolStripMenuItem).Text;
                    dgSearching.Refresh();
                }
            }
        }

        
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.Close();
            SearchDialog searchForm = new SearchDialog(_tvProg);
            searchForm.Show(Owner as MainForm);
        }

        private void SearchResultsForm_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
            if ((Owner != null) && (Owner is MainForm) && (Owner as MainForm).PropCapture != null)
            {
                colRec.Visible = true;
            }
            dgSearching.AutoGenerateColumns = false;
            dgSearching.DataSource = _filterTable;
            slblTotal.Text = Resources.FoundText + _filterTable.Rows.Count + " " + Resources.Telecasts;
            foreach (DataGridViewRow dgvr in dgSearching.Rows)
            {
                dgvr.Cells["colGenre"].ToolTipText =
                    !String.IsNullOrEmpty(dgvr.Cells["colProgCategory"].Value.ToString())
                        ? dgvr.Cells["colProgCategory"].Value.ToString()
                        : Resources.WithoutTypeText;
                dgvr.Cells["colRating"].ToolTipText =
                   !String.IsNullOrEmpty(dgvr.Cells["colFavName"].Value.ToString())
                       ? dgvr.Cells["colFavName"].Value.ToString()
                       : Resources.WithoutRatingText;
            }
        }

        /// <summary>
        /// Установка значка заголовку колонки таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetSatteliteOnHeaderColumn(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.ToString() == Resources.LogoText && e.RowIndex == -1)
                {
                    e.PaintBackground(e.ClipBounds, false);

                    Point pt = e.CellBounds.Location; // where you want the bitmap in the cell

                    int offset = (e.CellBounds.Width - this.imgLst.Images[0].Size.Width) / 2;
                    pt.X += offset;
                    pt.Y += (e.CellBounds.Height - this.imgLst.Images[0].Size.Height) / 2;
                    this.imgLst.Draw(e.Graphics, pt, 0);
                    e.Handled = true;
                }
                else
                {
                    if (e.Value.ToString() == Resources.NoticeText && e.RowIndex == -1)
                    {
                        e.PaintBackground(e.ClipBounds, false);

                        Point pt = e.CellBounds.Location;
                        int offset = (e.CellBounds.Width - this.imgLst.Images[1].Size.Width) / 2;
                        pt.X += offset;
                        pt.Y += (e.CellBounds.Height - this.imgLst.Images[1].Size.Height) / 2;
                        this.imgLst.Draw(e.Graphics, pt, 1);
                        e.Handled = true;
                    }
                    else if (e.Value.ToString() == Resources.GenreText && e.RowIndex == -1)
                    {
                        e.PaintBackground(e.ClipBounds, false);

                        Point pt = e.CellBounds.Location;
                        int offset = (e.CellBounds.Width - this.imgLst.Images[2].Size.Width) / 2;
                        pt.X += offset;
                        pt.Y += (e.CellBounds.Height - this.imgLst.Images[2].Size.Height) / 2;
                        this.imgLst.Draw(e.Graphics, pt, 2);
                        e.Handled = true;
                    }
                    else if (e.Value.ToString() == Resources.RatingText && e.RowIndex == -1)
                    {
                        e.PaintBackground(e.ClipBounds, false);

                        Point pt = e.CellBounds.Location;
                        int offset = (e.CellBounds.Width - this.imgLst.Images[3].Size.Width) / 2;
                        pt.X += offset;
                        pt.Y += (e.CellBounds.Height - this.imgLst.Images[3].Size.Height) / 2;
                        this.imgLst.Draw(e.Graphics, pt, 3);
                        e.Handled = true;
                    }
                    else if (e.Value.ToString() == Resources.ToRemindText && e.RowIndex == -1)
                    {
                        e.PaintBackground(e.ClipBounds, false);

                        Point pt = e.CellBounds.Location;
                        int offset = (e.CellBounds.Width - this.imageListremind.Images["bellheader.png"].Size.Width) / 2;
                        pt.X += offset;
                        pt.Y += (e.CellBounds.Height - this.imageListremind.Images["bellheader.png"].Size.Height) / 2;
                        this.imageListremind.Draw(e.Graphics, pt, 0);
                        e.Handled = true;
                    }
                    else if (e.Value.ToString() == Resources.RecText && e.RowIndex == -1)
                    {
                        e.PaintBackground(e.ClipBounds, false);

                        Point pt = e.CellBounds.Location;
                        int offset = (e.CellBounds.Width - this.imageListremind.Images["capture_header.png"].Size.Width) / 2;
                        pt.X += offset;
                        pt.Y += (e.CellBounds.Height - this.imageListremind.Images["capture_header.png"].Size.Height) / 2;
                        this.imageListremind.Draw(e.Graphics, pt, 1);
                        e.Handled = true;
                    }
                }

            }
        }

        private void dgSearching_Paint(object sender, PaintEventArgs e)
        {
            foreach (DataGridViewRow dgvr in dgSearching.Rows)
            {
                if ((DateTime)dgvr.Cells["colChanTo"].Value < DateTime.Now)
                {
                    dgvr.DefaultCellStyle.ForeColor = Settings.Default.LastTelecastColor;
                }
                if ((DateTime)dgvr.Cells["colChanFrom"].Value <= DateTime.Now &&
                    (DateTime)dgvr.Cells["colChanTo"].Value > DateTime.Now)
                {
                    dgvr.DefaultCellStyle.ForeColor = Settings.Default.CurTelecastColor;
                }
                if ((DateTime)dgvr.Cells["colChanFrom"].Value > DateTime.Now)
                {
                    dgvr.DefaultCellStyle.ForeColor = Settings.Default.FutTelecastColor;
                }
            }
        }

        private void dgSearching_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            SetSatteliteOnHeaderColumn(sender, e);
        }

        private void dgSearching_SelectionChanged(object sender, EventArgs e)
        {
            if (dgSearching.CurrentRow != null)
            {
                switch (Preferences.showAnons)
                {
                    case Preferences.ShowAnons.Ifitis:
                        if (!String.IsNullOrEmpty(dgSearching.CurrentRow.Cells["colChanDesc"].Value.ToString()))
                        {
                            rtbAnons.Text = dgSearching.CurrentRow.Cells["colChanDesc"].Value.ToString();
                            pAnons.Visible = true;
                        }
                        else
                        {
                            rtbAnons.Text = String.Empty;
                            pAnons.Visible = false;
                        }
                        break;
                    case Preferences.ShowAnons.Always:
                        rtbAnons.Text = dgSearching.CurrentRow.Cells["colChanDesc"].Value.ToString();
                        pAnons.Visible = true;
                        break;
                    case Preferences.ShowAnons.Never:
                        rtbAnons.Text = String.Empty;
                        pAnons.Visible = false;
                        break;
                }
            }
        }

        private void rtbAnons_ModifiedChanged(object sender, EventArgs e)
        {
            if (rtbAnons.Modified)
            {
                btnCancelDesc.Enabled = btnSaveDesc.Enabled = true;
            }
            else
            {
                btnCancelDesc.Enabled = btnSaveDesc.Enabled = false;
            }
        }

        private void btnCancelDesc_Click(object sender, EventArgs e)
        {
            rtbAnons.Undo();
            rtbAnons.Modified = false;
        }

        private void btnSaveDesc_Click(object sender, EventArgs e)
        {
            _tvProg.SaveDescription(dgSearching.CurrentRow.Cells["colCID"].Value.ToString(),
                                               (DateTime)dgSearching.CurrentRow.Cells["colChanFrom"].Value,
                                               (DateTime)dgSearching.CurrentRow.Cells["colChanTo"].Value,
                                               dgSearching.CurrentRow.Cells["colChanProg"].Value.ToString(),
                                               rtbAnons.Text);
            string filterStr = "id = " + dgSearching.CurrentRow.Cells["colChanID"].Value + " and start = '" +
                               dgSearching.CurrentRow.Cells["colChanFrom"].Value + "' and stop = '" +
                               dgSearching.CurrentRow.Cells["colChanTo"].Value + "' and title = '" +
                               dgSearching.CurrentRow.Cells["colChanProg"].Value + "'";
            DataRow[] drsRows = _filterTable.Select(filterStr);
            foreach (var dataRow in drsRows)
            {
                dataRow["desc"] = rtbAnons.Text;
            }
            dgSearching.CurrentRow.Cells["colChanDesc"].Value = rtbAnons.Text;
        }

        private void btnExitDesc_Click(object sender, EventArgs e)
        {
            pAnons.Visible = false;
        }
        /// <summary>
        /// Изменение статуса напоминания
        /// </summary>
        /// <param name="idChan">Код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="from">Дата начала передачи</param>
        /// <param name="to">Дата окончания передачи</param>
        /// <param name="statusBell">Статус колокольчика</param>
        /// <returns>Результат выполения</returns>
        private bool SetStatusBell(string idChan, string title, DateTime from, DateTime to, ref bool statusBell)
        {
            statusBell = false;
            DataRow[] drsRemind = (Owner as MainForm).Programms.Select(
                   String.Format("title = '{0}' and start = '{1}' and stop = '{2}'", title, from, to));
            if (drsRemind.Length > 0)
            {
                statusBell = !String.IsNullOrEmpty(drsRemind[0]["remind"].ToString())
                                      ? !(bool)drsRemind[0]["remind"]
                                      : true;
                if (_tvProg.SetRemind(idChan, title, from, to, statusBell))
                {
                    drsRemind[0].BeginEdit();
                    drsRemind[0]["remind"] = statusBell;
                    drsRemind[0].EndEdit();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Изменение статуса видеозахвата
        /// </summary>
        /// <param name="idProg">Код передачи</param>
        /// <param name="idChan">Код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="from">Дата начала передачи</param>
        /// <param name="to">Дата окончания прередачи</param>
        /// <param name="statusRec">Статус видеозахвата</param>
        /// <returns></returns>
        private bool SetStatusRec(string idProg, string idChan, string title, DateTime from, DateTime to, ref bool statusRec)
        {
            statusRec = false;
            DataRow foundRow =(Owner as MainForm).Programms.Rows.Find(long.Parse(idProg));
            if (foundRow != null)
            {
                statusRec = !String.IsNullOrEmpty(foundRow["record"].ToString())
                                ? !(bool)foundRow["record"]
                                : true;
                if (statusRec)
                {
                    DataRow[] drsCasts =
                        (Owner as MainForm).Programms.Select(
                            String.Format("((start >= '{0}' and (start < '{1}' and stop >= '{1}')) or " +
                                          "(start <= '{0}' and (stop > '{0}' and stop <= '{1}')) or " +
                                          "(start >= '{0}' and stop <= '{1}') or (start <='{0}' and stop >= '{1}')) and record = True",
                                          from, to));
                    if (drsCasts.Length > 0)
                    {
                        if (drsCasts.Length == 1)
                        {
                            if (drsCasts[0]["title"].ToString() != title || drsCasts[0]["cid"].ToString() != idChan)
                            {
                                Statics.ShowDialog(Resources.WarningText,
                                                   Resources.ImpossibleRecText,
                                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                                return false;
                            }
                        }
                        else
                        {
                            Statics.ShowDialog(Resources.WarningText, Resources.ImpossibleRecText,
                                               MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                            return false;
                        }
                    }
                }
                if (_tvProg.SetRecord(idChan, title, from, to, statusRec))
                {
                    foundRow.BeginEdit();
                    foundRow["record"] = statusRec;
                    foundRow.EndEdit();
                    return true;
                }
            }
            return false;
        }

        private void dgSearching_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && (e.ColumnIndex == 2 || e.ColumnIndex == 3) )
            {
                string idProg = dgSearching.Rows[e.RowIndex].Cells["colChanId"].Value.ToString();
                string idChan = dgSearching.Rows[e.RowIndex].Cells["colCID"].Value.ToString();
                string title = dgSearching.Rows[e.RowIndex].Cells["colChanProg"].Value.ToString();
                DateTime from = (DateTime)dgSearching.Rows[e.RowIndex].Cells["colChanFrom"].Value;
                DateTime to = (DateTime)dgSearching.Rows[e.RowIndex].Cells["colChanTo"].Value;
                switch (e.ColumnIndex)
                {
                    case 2:
                        bool statusRec = false;
                        if (SetStatusRec(idProg, idChan, title, from, to, ref statusRec))
                        {
                            DataRow[] drsRec = _filterTable.Select(
                                String.Format("cid = {0} and title = '{1}' and start = '{2}' and stop = '{3}'", idChan, title, from, to));
                            if (drsRec.Length > 0)
                            {
                                drsRec[0].BeginEdit();
                                drsRec[0]["record"] = statusRec;
                                drsRec[0]["capture"] = statusRec ? Resources.capture : Resources.capturempty;
                                drsRec[0].EndEdit();
                                dgSearching.Refresh();
                            }
                        }
                        break;
                    case 3:
                        bool statusBell = false;
                        if (SetStatusBell(idChan, title, from, to, ref statusBell))
                        {
                            DataRow[] drsRemind = _filterTable.Select(
                                String.Format("cid = {0} and title = '{1}' and start = '{2}' and stop = '{3}'", idChan, title, from, to));
                            if (drsRemind.Length > 0)
                            {
                                drsRemind[0].BeginEdit();
                                drsRemind[0]["remind"] = statusBell;
                                drsRemind[0]["bell"] = statusBell ? Resources.bell : Resources.bellempty;
                                drsRemind[0].EndEdit();
                                dgSearching.Refresh();
                            }
                        }
                        break;
                }
            }
        }

        private void dgSearching_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int index = e.RowIndex; index < e.RowIndex + e.RowCount; index++)
            {
                (sender as DataGridViewExt).Rows[index].ContextMenuStrip = contextMenuTables;
            }
        }

        private void csmiCopyToBuffer_Click(object sender, EventArgs e)
        {
            if (dgSearching.CurrentRow != null)
            {
                Clipboard.SetText(dgSearching.CurrentRow.Cells["colChanChannel"].Value.ToString() +
                    String.Format(" {0} - {1} ", dgSearching.CurrentRow.Cells["colChanFrom"].Value,
                    dgSearching.CurrentRow.Cells["colChanTo"].Value) + dgSearching.CurrentRow.Cells["colChanProg"].Value);
            }
        }

        private void dgSearching_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridViewExt.HitTestInfo hti;
                if (sender is DataGridViewExt)
                {
                    hti = (sender as DataGridViewExt).HitTest(e.X, e.Y);
                    if (hti.Type == DataGridViewHitTestType.Cell)
                    {
                        foreach (DataGridViewRow dgvr in (sender as DataGridViewExt).SelectedRows)
                        {
                            dgvr.Selected = false;
                        }
                        (sender as DataGridViewExt).Rows[hti.RowIndex].Selected = true;
                        foreach (DataGridViewColumn dgvc in (sender as DataGridViewExt).Columns)
                        {
                            if (dgvc.Visible)
                            {
                                (sender as DataGridViewExt).CurrentCell =
                                    (sender as DataGridViewExt).Rows[hti.RowIndex].Cells[dgvc.Index];
                                break;
                            }
                        }
                    }
                }
            }
        }
    /// <summary>   
    /// КМ: Добавление к любимым
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void csmiAddToFavorite_Click(object sender, EventArgs e)
        {
            if (dgSearching.CurrentRow != null)
            {
                string txtFavorite = dgSearching.CurrentRow.Cells["colChanProg"].Value.ToString();
                ClassifFavoriteDialog clFavoriteDialog = new ClassifFavoriteDialog(Preferences.favorites, txtFavorite);
                if (clFavoriteDialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (!Preferences.favorites.ClassifTable.Columns.Contains("deleteafter"))
                    {
                        Preferences.favorites.ClassifTable.Columns.Add("deleteafter", typeof(DateTime));
                    }
                    if (clFavoriteDialog.Edit)
                    {
                        DataRow[] drsFavEdit = Preferences.favorites.ClassifTable.Select(
                            String.Format("Contain LIKE '*{0}*'", clFavoriteDialog.ClassFavorite.Contain));
                        if (drsFavEdit.Length > 0)
                        {
                            drsFavEdit[0].BeginEdit();
                            drsFavEdit[0]["Contain"] = clFavoriteDialog.ClassFavorite.Contain;
                            drsFavEdit[0]["NonContain"] = clFavoriteDialog.ClassFavorite.NonContain;
                            drsFavEdit[0]["fid"] = clFavoriteDialog.ClassFavorite.IdFav;
                            DataRow[] drsFavorite =
                                Preferences.favorites.FavoritesTable.Select("id = " +
                                                                   clFavoriteDialog.ClassFavorite.IdFav);
                            if (drsFavorite.Length > 0)
                            {
                                drsFavEdit[0]["image"] = drsFavorite[0]["image"];
                            }
                            if (clFavoriteDialog.ClassFavorite.TsDeleteAfter.Year >= 2011)
                            {
                                drsFavEdit[0]["deleteafter"] = clFavoriteDialog.ClassFavorite.TsDeleteAfter;
                            }
                            drsFavEdit[0]["remind"] = clFavoriteDialog.ClassFavorite.Remind;
                            drsFavEdit[0].EndEdit();
                        }    
                    }
                    else
                    {
                        DataRow drClassFavorite = Preferences.favorites.ClassifTable.NewRow();
                        drClassFavorite["Contain"] = clFavoriteDialog.ClassFavorite.Contain;
                        drClassFavorite["NonContain"] = clFavoriteDialog.ClassFavorite.NonContain;
                        drClassFavorite["fid"] = clFavoriteDialog.ClassFavorite.IdFav;
                        DataRow[] drsFavorite =
                            Preferences.favorites.FavoritesTable.Select("id = " + clFavoriteDialog.ClassFavorite.IdFav);
                        foreach (DataRow dataRow in drsFavorite)
                        {
                            drClassFavorite["image"] = dataRow["image"];
                        }
                        if (clFavoriteDialog.ClassFavorite.TsDeleteAfter.Year >= 2011)
                        {
                            drClassFavorite["deleteafter"] = clFavoriteDialog.ClassFavorite.TsDeleteAfter;
                        }
                        drClassFavorite["remind"] = clFavoriteDialog.ClassFavorite.Remind;
                        Preferences.favorites.ClassifTable.Rows.InsertAt(drClassFavorite, 0);    
                    }
                    foreach (DataRow drClassifRow in Preferences.favorites.ClassifTable.Rows)
                    {
                        drClassifRow["prior"] = Preferences.favorites.ClassifTable.Rows.IndexOf(drClassifRow);
                    };
                    string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifFavorites);
                    DataTable dtSerial = Preferences.favorites.ClassifTable.Copy();
                    dtSerial.Columns.Remove("id");
                    dtSerial.Columns.Remove("image");
                    dtSerial.WriteXml(xmlPath);
                }
            }
        }

        /// <summary>
        /// КМ: Добавить в классификатор жанров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiAddToGenres_Click(object sender, EventArgs e)
        {
            string txtGenre = String.Empty;
            if (dgSearching.CurrentRow != null)
            {
                txtGenre = dgSearching.CurrentRow.Cells["colChanProg"].Value.ToString();
                ClassifGenreDialog clGenreDialog = new ClassifGenreDialog(Preferences.genres, txtGenre);
                if (clGenreDialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (clGenreDialog.Edit)
                    {
                        // Редактирование имеющегося класса:
                        DataRow[] drsGenreEdit = Preferences.genres.ClassifTable.Select(
                            String.Format("Contain LIKE '*{0}*'", clGenreDialog.ClassGenre.Contain));
                        if (drsGenreEdit.Length > 0)
                        {
                            drsGenreEdit[0].BeginEdit();
                            drsGenreEdit[0]["Contain"] = clGenreDialog.ClassGenre.Contain;
                            drsGenreEdit[0]["NonContain"] = clGenreDialog.ClassGenre.NonContain;
                            drsGenreEdit[0]["gid"] = clGenreDialog.ClassGenre.IdGenre;
                            DataRow[] drsGenre =
                                Preferences.genres.GenresTable.Select("id = " +
                                                                      clGenreDialog.ClassGenre.IdGenre);
                            if (drsGenre.Length > 0)
                            {
                                drsGenreEdit[0]["image"] = drsGenre[0]["image"];
                            }
                            if (clGenreDialog.ClassGenre.TsDeleteAfter.Year >= 2011)
                            {
                                drsGenreEdit[0]["deleteafter"] = clGenreDialog.ClassGenre.TsDeleteAfter;
                            }
                            drsGenreEdit[0].EndEdit();
                        }
                    }
                    else
                    {
                        // Добавление в классификатор:
                        if (!Preferences.genres.ClassifTable.Columns.Contains("deleteafter"))
                        {
                            Preferences.genres.ClassifTable.Columns.Add("deleteafter", typeof (DateTime));
                        }
                        DataRow drClassGenre = Preferences.genres.ClassifTable.NewRow();
                        drClassGenre["Contain"] = clGenreDialog.ClassGenre.Contain;
                        drClassGenre["NonContain"] = clGenreDialog.ClassGenre.NonContain;
                        drClassGenre["gid"] = clGenreDialog.ClassGenre.IdGenre;
                        DataRow[] drsGenre =
                            Preferences.genres.GenresTable.Select("id = " + clGenreDialog.ClassGenre.IdGenre);
                        foreach (DataRow dataRow in drsGenre)
                        {
                            drClassGenre["image"] = dataRow["image"];
                        }
                        if (clGenreDialog.ClassGenre.TsDeleteAfter.Year >= 2011)
                        {
                            drClassGenre["deleteafter"] = clGenreDialog.ClassGenre.TsDeleteAfter;
                        }
                        Preferences.genres.ClassifTable.Rows.InsertAt(drClassGenre, 0);
                    }
                    foreach (DataRow drClassifRow in Preferences.genres.ClassifTable.Rows)
                    {
                        drClassifRow["prior"] = Preferences.genres.ClassifTable.Rows.IndexOf(drClassifRow);
                    }
                    ;
                    string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifGenres);
                    DataTable dtSerial = Preferences.genres.ClassifTable.Copy();
                    dtSerial.Columns.Remove("id");
                    dtSerial.Columns.Remove("image");
                    dtSerial.WriteXml(xmlPath);
                }
            }
        }
        /// <summary>
        /// КМ: Свойства канала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiPropChannel_Click(object sender, EventArgs e)
        {
            string strCID = String.Empty;
            if (dgSearching.CurrentRow != null)
            {
                strCID = dgSearching.CurrentRow.Cells["colCID"].Value.ToString();
            }
            DataRow[] drsChan = (Owner as MainForm).Channels.CopyToDataTable().Select("id = " + strCID);
            if (drsChan.Length > 0)
            {
                Channel chan = new Channel((bool)drsChan[0]["visible"],
                                           drsChan[0]["display-name"].ToString(),
                                           uint.Parse(drsChan[0]["number"].ToString()),
                                           drsChan[0]["user-name"].ToString(),
                                           (Image)drsChan[0]["icon"],
                                           drsChan[0]["diff"].ToString());
                EditChannelDialog editChanDlg = new EditChannelDialog(chan);
                if (editChanDlg.ShowDialog(this) == DialogResult.OK)
                {
                    string xmlOptChannelFile = Path.Combine(Application.StartupPath, Preferences.xmlOptChannelFile);
                    if (!File.Exists(xmlOptChannelFile))
                    {
                        XmlTextWriter tr = new XmlTextWriter(xmlOptChannelFile, null);
                        tr.WriteStartDocument();
                        tr.WriteStartElement("Root");
                        tr.WriteStartElement("channel");
                        tr.WriteAttributeString("id", strCID);
                        tr.WriteAttributeString("visible", chan.Visible.ToString());
                        tr.WriteAttributeString("user-name", chan.UserSyn);
                        tr.WriteAttributeString("number", chan.Number.ToString());
                        tr.WriteAttributeString("diff", chan.Diff);
                        tr.WriteEndElement();
                        tr.WriteEndElement();
                        tr.WriteEndDocument();
                        tr.Flush();
                        tr.Close();
                    }
                    else
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.Load(xmlOptChannelFile);
                        XPathNavigator xNav = ((IXPathNavigable)xdoc).CreateNavigator();
                        XPathNodeIterator itr = xNav.Select("/Root/channel[@id='" + strCID + "']");
                        while (itr.MoveNext())
                        {
                            itr.Current.MoveToAttribute("visible", String.Empty);
                            itr.Current.SetValue(chan.Visible.ToString());
                            itr.Current.MoveToParent();
                            itr.Current.MoveToAttribute("user-name", String.Empty);
                            itr.Current.SetValue(chan.UserSyn);
                            itr.Current.MoveToParent();
                            itr.Current.MoveToAttribute("number", String.Empty);
                            itr.Current.SetValue(chan.Number.ToString());
                            itr.Current.MoveToParent();
                            itr.Current.MoveToAttribute("diff", String.Empty);
                            itr.Current.SetValue(chan.Diff);
                        }
                        xdoc.Save(xmlOptChannelFile);
                    }
                }
            } 
        }

        /// <summary>
        /// Экспорт в Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportXsl_Click(object sender, EventArgs e)
        {
            if (dgSearching.DataSource != null)
            {
                DataSet ds = new DataSet("DataSetExport");
                ds.Tables.Clear();
                ds.Tables.Add(dgSearching.DataSource as DataTable);
                ExcelExport exp = new ExcelExport(ds, _exp_folder + "exp_template1.xls");
                if (exp != null)
                {
                    ExportBuilder.Save(exp.BuildWorkBook(), Resources.ExportResFileText);
                }
            }
        }

        
    }
}
