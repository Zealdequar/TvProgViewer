﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVProgViewer.TVProgApp
{
    public class Favorites
    {
        private DataTable _favoriteTable = new DataTable("Favorites");
        private DataTable _classifTable = new DataTable("FavwordsTable");
        public DataTable FavoritesTable
        {
            get { return _favoriteTable; }
        }
        public DataTable ClassifTable
        {
            get { return _classifTable; }
            set { _classifTable = value; }
        }

        public Favorites(DataTable favoriteTable)
        {
            _favoriteTable = favoriteTable;
            string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifFavorites);
            if (!File.Exists(xmlPath))
            {
                _classifTable.Columns.Clear();
                _classifTable.Columns.Add("id", typeof(int));
                _classifTable.Columns["id"].AutoIncrement = true;
                _classifTable.Columns.Add("fid", typeof(int));
                _classifTable.Columns.Add("contain", typeof(string));
                _classifTable.Columns.Add("noncontain", typeof(string));
                _classifTable.Columns.Add("deleteafter", typeof(DateTime));
                _classifTable.Columns.Add("remind", typeof(bool));
                _classifTable.Columns.Add("prior", typeof(int));

                DataTable xmlWriteTable = _classifTable.Copy();
                xmlWriteTable.Columns.Remove("id");
                xmlWriteTable.WriteXml(xmlPath);
            }
            else
            {
                _classifTable.Columns.Clear();
                _classifTable.Columns.Add("id", typeof(int));
                _classifTable.Columns["id"].AutoIncrement = true;
                _classifTable.Columns.Add("fid", typeof(int));
                _classifTable.Columns.Add("contain", typeof(string));
                _classifTable.Columns.Add("noncontain", typeof(string));
                _classifTable.Columns.Add("deleteafter", typeof(DateTime));
                _classifTable.Columns.Add("remind", typeof(bool));
                _classifTable.Columns.Add("prior", typeof(int));
                // Установка датасета
                DataSet dsClassif = new DataSet();
                dsClassif.ReadXml(xmlPath);
                if (dsClassif.Tables.Count > 0)
                {
                    if (dsClassif.Tables[0] != null)
                    {
                        foreach (DataRow dataRow in dsClassif.Tables[0].Rows)
                        {
                            _classifTable.Rows.Add(null, dataRow["fid"] ?? 0,
                                                   dataRow["contain"] ?? "",
                                                   dataRow["noncontain"] ?? "",
                                                   dataRow.Table.Columns.Contains("deleteafter") ? dataRow["deleteafter"] : null,
                                                   dataRow.Table.Columns.Contains("remind") ? dataRow["remind"] : false,
                                                   dataRow.Table.Columns.Contains("prior") ? dataRow["prior"] : _classifTable.Rows.IndexOf(dataRow));
                        }
                    }
                }
            }

            bool pr = false;
            for (int i = 0; i <= _classifTable.Rows.Count - 1; i++)
            {
                if (!String.IsNullOrEmpty(_classifTable.Rows[i]["deleteafter"].ToString()))
                {
                    DateTime tsDeleteAfter = (DateTime)_classifTable.Rows[i]["deleteafter"];
                    if (tsDeleteAfter.Year >= 2000 && tsDeleteAfter < DateTime.Now)
                    {
                        _classifTable.Rows[i].Delete();
                        pr = true;
                    }
                }
            }
            if (pr)
            {
                DataTable xmlTableToWrite = _classifTable.Copy();
                xmlTableToWrite.Columns.Remove("id");
                xmlTableToWrite.WriteXml(xmlPath);
            }
            if (!_classifTable.Columns.Contains("image"))
            {
                _classifTable.Columns.Add("image", typeof(Image));
            }
            foreach (DataRow drFav in _favoriteTable.Rows)
            {
                foreach (DataRow drClassif in _classifTable.Rows)
                {
                    if (drFav["id"].ToString() == drClassif["fid"].ToString())
                    {
                        drClassif["image"] = drFav["image"];
                    }
                }
            }
        }
    }
}

