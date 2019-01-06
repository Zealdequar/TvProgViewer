﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using ICSharpCode.SharpZipLib.Zip;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Logger;
using TVProgViewer.TVProgApp.Properties;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib;

namespace TVProgViewer.TVProgApp
{
    public class TVProgClass
    {
        private XDocument _pubXdoc;

        private DateTime dateMinProg = DateTime.MinValue,
                         dateMaxProg = DateTime.MaxValue;

        public DataTable WaitRemind { get; set; }
        private string _globalFiltChan = String.Empty; // - глобальный фильтр по каналам 
        private string _globalFiltGenre = String.Empty; // - глобальный фильтр по жанрам
        private string _globalFiltRating = String.Empty; // - глобальный фильтр по любимым
        private string _globalFiltAnons = String.Empty;  // - глобальный фильтр по анонсам
        private string _globalFiltRemind = String.Empty;  // - глобальный фильтр по напоминаниям
        private readonly string _xmlPubFileName = String.Empty; // - имя единого файла XML для хранения каналов и тв-программы по ним
        public XDocument PubXdoc
        {
            get { return _pubXdoc; }
            private set { _pubXdoc = value; }
        }

        public string GlobalFiltChan
        {
            get { return _globalFiltChan; }
        }


        
        /// <summary>
        /// Конструктор
        /// </summary>
        public TVProgClass()
        {
            Statics.EL = new ExceptionLogger();
            Statics.EL.AddLogger(new TextFileLogger());
            WaitRemind = new DataTable("WaitRemind");
            WaitRemind.Columns.Add("id", typeof(int));
            WaitRemind.Columns.Add("title", typeof (string));
            WaitRemind.Columns.Add("start", typeof (DateTime));
            WaitRemind.Columns.Add("waitto", typeof (DateTime));
            _xmlPubFileName = Path.Combine(Application.StartupPath, Preferences.xmlPubFileName);
            LoadTVProg(true);
        }

        /// <summary>
        /// Обновление телепрограммы
        /// </summary>
        /// <param name="flag">Включить ли модальное окно сообщения</param>
        public void LoadTVProg(bool flag)
        {
            
            string xmlFileName1 = Path.Combine(Application.StartupPath, Preferences.xmlFileName1);
            if (File.Exists(_xmlPubFileName))
            {
                _pubXdoc = XDocument.Load(_xmlPubFileName);
                DataTable dtTVProg = GetTVProgramm(ref dateMinProg, ref dateMaxProg);
                if (dateMaxProg < DateTime.Now)
                {
                    if (flag)
                    {
                        if (Statics.ShowDialog(Resources.ConfirmChoiceText, Resources.DownloadNewProgText,
                                               MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.YesNo) ==
                            DialogResult.Yes)
                        {
                            if (!GetWebTVProgramm(Settings.Default.UrlNewProgXMLTV, xmlFileName1))
                            {
                                string pathInter = Path.Combine(Application.StartupPath, Preferences.interNewFileName);
                                GetWebTVProgramm(Settings.Default.UrlNewProgInterTV, pathInter);
                                InterToXml(pathInter, ref xmlFileName1);
                            }
                            MergeXmls(xmlFileName1);
                            if (File.Exists(_xmlPubFileName))
                            {
                                _pubXdoc = XDocument.Load(_xmlPubFileName);
                            }
                        }
                    }
                }
                
                DeleteTooManyChannels(_pubXdoc, _xmlPubFileName);
            }
        }
        
        
        
        /// <summary>
        /// Слияние xml
        /// </summary>
        /// <param name="xmlTVFileName1"></param>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public void MergeXmls(string xmlTVFileName1)
        {
            string dtdTVFileName = Path.Combine(Application.StartupPath, "xmltv.dtd");
            string xmlTVFileName2 = Path.Combine(Application.StartupPath, Preferences.xmlFileName2);
            string xsltUniqKey = Path.Combine(Application.StartupPath, @"Data\UniqKey.xslt");
            Statics.ShowLogo(Resources.StatusMergingProg, 0);
            if (File.Exists(xmlTVFileName1))
            { // если входной документ существует:
                XDocument docX1 = XDocument.Load(xmlTVFileName1);
                DeleteTooManyChannels(docX1, xmlTVFileName1);
                if (docX1.Document != null)
                {
                    XDocumentType docType = docX1.Document.DocumentType;  
                    if (docType != null)
                    {
                        HttpWebResponse hwResponse;
                        byte[] buffer = new byte[4096];
                        int read = 0;
                        if (!File.Exists(dtdTVFileName))
                        {
                            try
                            {
                                HttpWebRequest hwReq = (HttpWebRequest)HttpWebRequest.Create(docType.SystemId);
                                hwResponse = (HttpWebResponse)hwReq.GetResponse();
                                Stream stream = hwResponse.GetResponseStream();
                                using (FileStream fs = new FileStream(dtdTVFileName, FileMode.Create, FileAccess.Write))
                                {
                                    while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                                    {
                                        fs.Write(buffer, 0, read);
                                    }
                                    fs.Flush();
                                    fs.Close();
                                }
                            }
                            catch
                            {

                            }
                        }
                        docType.SystemId = dtdTVFileName;
                        docX1.Save(xmlTVFileName1); // - сохранение пути к dtd-файлу
                    }
                }
                Statics.ShowLogo(Resources.StatusMergingProg, 20);
                if (!File.Exists(_xmlPubFileName))
                {// Общего файла нет - создать:
                    docX1.Save(_xmlPubFileName);
                    _pubXdoc = new XDocument(docX1);
                    File.Delete(xmlTVFileName1);
                    return;
                }
                else // общий файл существует уже
                {
                    _pubXdoc.Save(xmlTVFileName2); // - копировать во временный
                    if (File.Exists(xmlTVFileName2))
                    {
                      using (StreamReader sr = new StreamReader(xmlTVFileName1, Encoding.GetEncoding("UTF-8")))
                      using (XmlReader xmlreader1 = 
                          XmlTextReader.Create(sr, new XmlReaderSettings() {DtdProcessing = DtdProcessing.Parse}))
                        {
                            XmlValidatingReader xmlValidating = new XmlValidatingReader(xmlreader1)
                                                                    {
                                                                        ValidationType = ValidationType.DTD,
                                                                    };
                            while (xmlValidating.Read())
                            {
                                StringWriter sw = new StringWriter();
                                XslCompiledTransform transform = null;
                                using (XmlWriter writer = new XmlTextWriter(sw))
                                {
                                    try
                                    {
                                        transform = new XslCompiledTransform(); // - объект для трансформации
                                        transform.Load(xsltUniqKey,
                                                       new XsltSettings()
                                                           {EnableDocumentFunction = true, EnableScript = true},
                                                       new XmlUrlResolver());
                                        // Трансформация 
                                        Statics.ShowLogo(Resources.StatusMergingProg, 55);
                                        transform.Transform(xmlreader1, new XsltArgumentList(), writer);
                                        using (StreamWriter streamWriter = new StreamWriter(_xmlPubFileName))
                                        {
                                            streamWriter.Write(sw.GetStringBuilder().ToString());
                                            streamWriter.Flush();
                                            streamWriter.Close();
                                        }
                                        if (File.Exists(_xmlPubFileName))
                                        {
                                            _pubXdoc = XDocument.Load(_xmlPubFileName);
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    finally
                                    {
                                        if (writer != null)
                                        {
                                            writer.Flush();
                                            writer.Close();
                                        }

                                        if (sw != null)
                                        {
                                            sw.Dispose();
                                        }
                                       
                                        if (transform != null)
                                        {
                                            transform.TemporaryFiles.Delete();
                                        }
                                        Statics.ShowLogo(Resources.StatusMergingProg, 100);
                                        Statics.HideLogo();
                                    }
                                }
                            }
                            xmlValidating.Close();
                            xmlreader1.Close();
                        }
                    }
                }
            }

            try
            {
                if (File.Exists(xmlTVFileName1)) File.Delete(xmlTVFileName1);
            }
            catch (Exception)
            {

            }

            try
            {
                if (File.Exists(xmlTVFileName2)) File.Delete(xmlTVFileName2);
            }
            catch(Exception)
            {
                
            }
            
        }

       
        /// <summary>
        /// Удаление лишних каналов
        /// </summary>
        /// <param name="xdoc">Документ xml</param>
        /// <param name="output_file">Результирующий файл</param>
        public void DeleteTooManyChannels(XDocument xdoc, string output_file)
        {
            string xmlExpOptChannelFileName = Path.Combine(Application.StartupPath, Preferences.xmlExpChanOptionsFile);
            if (File.Exists(xmlExpOptChannelFileName))
            {
                DataSet dsExpOptConfig = new DataSet("ExpOptConfig");
                dsExpOptConfig.ReadXml(xmlExpOptChannelFileName, XmlReadMode.Auto);
                if (dsExpOptConfig.Tables["ChanSettings"] != null)
                {
                    DataRow dr = dsExpOptConfig.Tables["ChanSettings"].Rows[0];
                    if (dr != null)
                    {
                        if (dr["UncheckedChannels"].ToString() == "Delete")
                        {
                            List<string> lstChan = new List<string>();
                            bool firstonce = false;
                            // Добавление дополнительной информации для каналов:
                            var drsChans = AddChannelsInfo(GetChannels(), ref firstonce).Rows;
                            foreach (DataRow drChan in drsChans)
                            {
                                if (bool.Parse(drChan["visible"].ToString()) == false)
                                {
                                    lstChan.Add(drChan["id"].ToString());
                                }
                            }
                            if (lstChan.Count > 0)
                            {
                                
                                DeleteChannels(lstChan);
                               
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Конвертер Интер-ТВ телепрограммы в XMLTV
        /// </summary>
        /// <param name="interFile">Файл с Интер-ТВ программы</param>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public void InterToXml(string interFile, ref string output_file)
        {
            throw new NotImplementedException();
        }

        
        public bool GetWebTVProgramm(string a, string b)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Для обратного вызова
        /// </summary>
        /// <returns></returns>
        private static bool ThumbnailCallback()
        {
            return false;
        }
        /// <summary>
        /// Добавление дополнительной информации о каналах
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="firstOnce"></param>
        /// <returns></returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public DataTable AddChannelsInfo(DataTable channels, ref bool firstOnce)
        {
            /*string xmlOptChannelsPath = Path.Combine(Application.StartupPath, Preferences.xmlOptChannelFile);
            if (channels.Columns["visible"] == null) channels.Columns.Add("visible", typeof(bool));
            if (channels.Columns["user-name"] == null) channels.Columns.Add("user-name", typeof(string));
            if (channels.Columns["number"] == null) channels.Columns.Add("number", typeof(uint));
            if (channels.Columns["diff"] == null) channels.Columns.Add("diff", typeof(string));
            if (channels.Columns["freq"] == null) channels.Columns.Add("freq", typeof(long));
            if (File.Exists(xmlOptChannelsPath))
            {
                XDocument xChannelsDoc = XDocument.Load(xmlOptChannelsPath);
                IEnumerable<XElement> xElements = from channel in xChannelsDoc.Descendants("channel")
                                                  select channel;
                foreach (DataRow channel in channels.Rows)
                {
                    foreach (XElement xRowChannel in xElements)
                    {
                        if (xRowChannel.Attribute("id") != null)
                        {
                            int optChanID;
                            Int32.TryParse(xRowChannel.Attribute("id").Value, out optChanID);
                            if ((int) channel["id"] == optChanID)
                            {
                                channel["visible"] = xRowChannel.Attribute("visible") != null
                                                         ? (object) xRowChannel.Attribute("visible").Value
                                                         : false;
                                channel["user-name"] = xRowChannel.Attribute("user-name") != null
                                                           ? xRowChannel.Attribute("user-name").Value
                                                           : channel["display-name"];
                                channel["number"] = xRowChannel.Attribute("number") != null
                                                        ? (object) xRowChannel.Attribute("number").Value
                                                        : 0;
                                channel["diff"] = xRowChannel.Attribute("diff") != null
                                                      ? xRowChannel.Attribute("diff").Value
                                                      : "+04:00";
                                channel["freq"] = xRowChannel.Attribute("freq") != null
                                                     ? xRowChannel.Attribute("freq").Value
                                                     : "0";
                                break;
                            }
                        }
                    }
                    if (channel["user-name"].ToString() == String.Empty)
                    {
                        channel["visible"] = false;
                        channel["user-name"] = channel["display-name"];
                        channel["number"] = 0;
                        channel["diff"] = "+04:00";
                        channel["freq"] = 0;
                    }
                }
                firstOnce = false;
            }
            else
            {
                foreach (DataRow channel in channels.Rows)
                {
                    channel["visible"] = false;
                    channel["user-name"] = channel["display-name"];
                    channel["number"] = 0;
                    channel["diff"] = "+04:00";
                }
                if (channels.Rows.Count > 0) firstOnce = true;
            }
            SetGlobalFiltChan(channels);  */
            return new DataTable();
        }
        
        /// <summary>
        /// Получение каналов
        /// </summary>
        /// <returns>Таблица с каналами</returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public DataTable GetChannels()
        {
            string tblName = "AllChannels";
            DataTable dataTable = new DataTable(tblName);
            dataTable.Columns.Add("id", typeof (int)); // - код канала
            dataTable.Columns.Add("display-name", typeof (string)); // - отображаемое название канала
            dataTable.Columns.Add("icon-src", typeof (string)); // - путь к значку в интернет
            dataTable.Columns.Add("icon", typeof (Image)); // - значок канала
            DataColumn[] keyChanColumns = new DataColumn[1];
            keyChanColumns[0] = dataTable.Columns["id"];
            dataTable.PrimaryKey = keyChanColumns;
            if (_pubXdoc == null) return new DataTable("<No Data>");
            IEnumerable<XElement> xElements;
            xElements = from channel in _pubXdoc.Descendants("channel")
                        select channel;
            string gifDir = Application.StartupPath + @"\Gifs";
            if (!Directory.Exists(gifDir))
                Directory.CreateDirectory(gifDir);
            foreach (XElement xRowChannel in xElements)
            {
                if (xRowChannel.Attribute("id") != null)
                {
                    int id = 0;
                    Int32.TryParse(xRowChannel.Attribute("id").Value, out id);
                    if (dataTable.Rows.Contains(id)) continue;
                    DataRow drChannel = dataTable.NewRow();
                    drChannel["id"] = id;
                    string gifFileName = gifDir + "\\" + id + ".gif";
                    foreach (XElement xChildRowChennel in xRowChannel.Nodes())
                    {
                        if (xChildRowChennel.Name == "display-name")
                        {
                            drChannel["display-name"] = xChildRowChennel.Value.ToString();
                        }

                        if (xChildRowChennel.Name == "icon")
                        {
                            string src = xChildRowChennel.Attribute("src").Value.ToString();
                            drChannel["icon-src"] = src;
                            try
                            {
                                if (!File.Exists(gifFileName))
                                {
                                    // Загрузка и сохранение значка канала:
                                    using (WebClient wc = new WebClient())
                                    {
                                        byte[] gif = wc.DownloadData(src);
                                        File.WriteAllBytes(gifFileName, gif);
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    Image.GetThumbnailImageAbort myCallback =
                        new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    // Сжатие значка до заданных размеров:
                    if (File.Exists(gifFileName))
                    {
                        drChannel["icon"] = Image.FromFile(gifFileName).GetThumbnailImage(25, 25, myCallback,
                                                                                          IntPtr.Zero);
                    }
                    else
                    {
                        try
                        {
                           // string idChan = Statics.dictChanCode[drChannel["display-name"].ToString()];
                            /*drChannel["icon"] =
                                ((Image) ResourceImages.ResourceManager.GetObject("_" + idChan)).
                                    GetThumbnailImage(25, 25, myCallback, IntPtr.Zero);*/
                        }
                        catch (Exception ex)
                        {
                            if (Resources.ResourceManager.GetObject("satellite_dish") != null)
                            {
                                drChannel["icon"] = ((Image) Resources.ResourceManager.GetObject("satellite_dish")).
                                    GetThumbnailImage(25, 25, myCallback, IntPtr.Zero);
                            }
                        }
                    }
                    dataTable.Rows.Add(drChannel);
                }
            }

            DataColumn[] keyColumns = new DataColumn[1];

            keyColumns[0] = dataTable.Columns["id"];
            dataTable.PrimaryKey = keyColumns;
            return dataTable;
        }

        /// <summary>
        /// Получение программы телепередач
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public DataTable GetTVProgramm (ref DateTime minDate, ref DateTime maxDate)
        {
            string tblName = "Programme";
            
            DataTable dtProgramme = new DataTable(tblName);
           
            dtProgramme.Columns.Add("cid", typeof(int));
            dtProgramme.Columns.Add("start", typeof(DateTime));
            dtProgramme.Columns.Add("stop", typeof(DateTime));
            dtProgramme.Columns.Add("start_mo", typeof(DateTime));
            dtProgramme.Columns.Add("stop_mo", typeof(DateTime));
            dtProgramme.Columns.Add("title", typeof(string));
            dtProgramme.Columns.Add("category", typeof(string));
            dtProgramme.Columns.Add("desc", typeof(string));
            dtProgramme.Columns.Add("record", typeof(bool));
            dtProgramme.Columns.Add("remind", typeof(bool));
            dtProgramme.Columns.Add("favname", typeof(string));

            if (_pubXdoc == null) return new DataTable("<No Data>"); 
            IEnumerable<XElement> xElements2;
            // Выборка по программам
            xElements2 = from program in _pubXdoc.Descendants("programme")
                         select program;
            foreach (XElement xRowProgram in xElements2)
            {
                DataRow drProgram = dtProgramme.NewRow();
                int cid = 0;
                Int32.TryParse(xRowProgram.Attribute("channel").Value, out cid);
                drProgram["cid"] = cid;
                string strStart = xRowProgram.Attribute("start").Value;
                int year = int.Parse(strStart.Substring(0, 4));
                int month = int.Parse(strStart.Substring(4, 2));
                int day = int.Parse(strStart.Substring(6, 2));
                int hour = int.Parse(strStart.Substring(8, 2));
                int minute = int.Parse(strStart.Substring(10, 2));
                int second = int.Parse(strStart.Substring(12, 2));
                DateTime tsStart = new DateTime(year, month, day, hour, minute, second);
                drProgram["start"] = drProgram["start_mo"] =tsStart;
                string strStop = xRowProgram.Attribute("stop").Value;
                year = int.Parse(strStop.Substring(0, 4));
                month = int.Parse(strStop.Substring(4, 2));
                day = int.Parse(strStop.Substring(6, 2));
                hour = int.Parse(strStop.Substring(8, 2));
                minute = int.Parse(strStop.Substring(10, 2));
                second = int.Parse(strStop.Substring(12, 2));
                DateTime tsStop = new DateTime(year, month, day, hour, minute, second);
                drProgram["stop"] = drProgram["stop_mo"] = tsStop;
                foreach (XElement xChildRowProgram in xRowProgram.Nodes())
                {
                    if (xChildRowProgram.Name == "record")
                    {
                        drProgram["record"] = xChildRowProgram.Value.ToLower() == "true" ? true : false;
                    }
                    if (xChildRowProgram.Name == "remind")
                    {
                        drProgram["remind"] = xChildRowProgram.Value.ToLower() == "true" ? true : false;
                    }
                    if (xChildRowProgram.Name == "title")
                    {
                        drProgram["title"] = xChildRowProgram.Value;
                    }
                    if (xChildRowProgram.Name == "category")
                    {
                        drProgram["category"] = xChildRowProgram.Value;
                    }
                    if (xChildRowProgram.Name == "desc")
                    {
                        drProgram["desc"] = xChildRowProgram.Value;
                    }
                    if (xChildRowProgram.Name == "favname")
                    {
                        drProgram["favname"] = xChildRowProgram.Value;
                    }
                }
                dtProgramme.Rows.Add(drProgram);
            }
            var min = dtProgramme.Compute("Min(start)", String.Empty);
            if (min != DBNull.Value)
            {
                minDate = Convert.ToDateTime(min);
            }
            var max = dtProgramme.Compute("Max(start)", String.Empty);
            if (max != DBNull.Value)
            {
                maxDate = Convert.ToDateTime(max);
            }
            // Поиск и удаление старой телепрограммы:
            FindAndDeleteOldTelecast(minDate, maxDate, dtProgramme);
            min = dtProgramme.Compute("Min(start)", String.Empty);
            if (min != DBNull.Value)
            {
                minDate = Convert.ToDateTime(min);
            }
            return dtProgramme;
        }
        
        /// <summary>
        /// Поиск и удаление старых передач
        /// </summary>
        /// <param name="minDate">Минимальная дата</param>
        /// <param name="maxDate">Максимальная дата</param>
        /// <param name="dtProgramme"></param>
        private void FindAndDeleteOldTelecast(DateTime minDate, DateTime maxDate, DataTable dtProgramme)
        {
            string xsltFilterNew = Path.Combine(Application.StartupPath, @"Data\RemoveOldProg.xslt");
            if (maxDate < DateTime.MaxValue && minDate > DateTime.MinValue)
            {
                Statics.ShowLogo(Resources.SearchingOldCasts, 0);
                DateTime tsOld = DateTime.MinValue;
                int count = 0;
                // Поиск устаревших телепередач:
                for (DateTime curDateTime = maxDate.Date.AddTicks(Settings.Default.BeginEndTime.Ticks);
                     curDateTime > minDate;
                     curDateTime = curDateTime.AddDays(-1))
                {
                    DataRow[] drsDate = dtProgramme.Select(String.Format("start > '{0}' and start < '{1}'", curDateTime,
                                                                         curDateTime.AddDays(1)));
                    if (drsDate.Length > 0)
                    {
                        count++;
                    }
                    if (count >= 14 && curDateTime.DayOfWeek.ToString().ToLower() == "monday")
                    {
                        tsOld = curDateTime;
                        break;
                    }
                }
                Statics.ShowLogo(Resources.SearchingOldCasts, 100);
                Statics.HideLogo();
                // Удаление устаревших телепередач:
                if (tsOld > DateTime.MinValue)
                {
                    Statics.ShowLogo(Resources.DeletingOldCasts, 0);
                    DataRow[] drsOldProg = dtProgramme.Select(String.Format("start < '{0}'", tsOld));
                    if (drsOldProg.Length > 0)
                    {
                        try
                        {
                            string oldStr = Util.ConvertDateTime(tsOld).Substring(0, 14);
                            XDocument newPubXdoc = new XDocument();
                            XslCompiledTransform transform = null;
                            using (XmlWriter writer = newPubXdoc.CreateWriter()) // new XmlTextWriter(sw))
                            {
                                try
                                {
                                    transform = new XslCompiledTransform(); // - объект для трансформации
                                    transform.Load(xsltFilterNew,
                                                   new XsltSettings()
                                                       {EnableDocumentFunction = true, EnableScript = true},
                                                   new XmlUrlResolver());
                                    // Трансформация 
                                    XsltArgumentList xsltArgs = new XsltArgumentList();
                                    xsltArgs.AddParam("date", "", oldStr);
                                    Statics.ShowLogo(Resources.DeletingOldCasts, 55);
                                    transform.Transform(_pubXdoc.CreateReader(), xsltArgs, writer);
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    if (transform != null)
                                    {
                                        transform.TemporaryFiles.Delete();
                                    }
                                    
                                    Statics.ShowLogo(Resources.DeletingOldCasts, 100);
                                    Statics.HideLogo();
                                }
                            }
                            if (newPubXdoc.Descendants("programme").Any())
                            {
                                _pubXdoc = new XDocument(newPubXdoc);
                            }
                            long lOld = long.Parse(oldStr);
                            var els = from programme in _pubXdoc.Descendants("programme").AsParallel()
                                      where long.Parse(programme.Attribute("start").Value.Substring(0, 14)) < lOld
                                      select programme;
                            els.AsParallel().Remove();
                            _pubXdoc.Save(_xmlPubFileName);
                            Statics.ShowLogo(Resources.DeletingOldCasts, 100);
                            Statics.HideLogo();
                        }
                        catch (Exception)
                        {
                            Statics.HideLogo();
                        }
                        foreach (DataRow drOld in drsOldProg)
                        {
                            dtProgramme.Rows.Remove(drOld);
                        }
                    }
                }
            }
        }


        public void SetGlobalFiltChan(DataTable sourceChannels)
        {
            DataRow[] drs = sourceChannels.Select("visible = True");
            switch (drs.Length)
            {
                case 1:
                    _globalFiltChan = " and cid = " + drs[0]["id"];
                    break;
                default:
                    if (drs.Length > 1)
                    {
                        _globalFiltChan = drs.Aggregate(" and (",
                            (current, drChan) => current + ("cid = " + drChan["id"].ToString() + " or "));
                        _globalFiltChan = _globalFiltChan.Substring(0, _globalFiltChan.Length - 3) + ")";
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Телевизионное вещание в данный момент
        /// </summary>
        /// <param name="sourceChannels" > Каналы </param>
        /// <param name="sourceTVProg">Программа телепередач</param>
        /// <param name="datetime">Установленное время</param>
        /// <returns></returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public DataTable GetShowTVNow(DataSet dsTVProg, string datetime)
        {
            // Фильтрация передач
            string filterStr = String.Format("start <= '{0}' and stop > '{0}' ", datetime);
            filterStr += _globalFiltChan;
            if (dsTVProg.Tables["AllProgrammes"].Rows.Count <= 0) return new DataTable("Null");
            
            DataRow[] drsNow = dsTVProg.Tables["AllProgrammes"].Select(filterStr);

            DataTable dtNow = new DataTable("Now");
            dtNow.Columns.Add("id", typeof (int));
            dtNow.Columns.Add("cid", typeof(int));
            dtNow.Columns.Add("start", typeof (DateTime));
            dtNow.Columns.Add("stop", typeof (DateTime));
            dtNow.Columns.Add("start_mo", typeof(DateTime));
            dtNow.Columns.Add("stop_mo", typeof(DateTime));
            dtNow.Columns.Add("title", typeof (string));
            dtNow.Columns.Add("category", typeof (string));
            dtNow.Columns.Add("desc", typeof (string));
            dtNow.Columns.Add("record", typeof(bool));
            dtNow.Columns.Add("remind", typeof(bool));
            dtNow.Columns.Add("favname", typeof(string));
            dtNow.Columns.Add("anons", typeof (Image));
            dtNow.Columns.Add("genre", typeof(Image));
            dtNow.Columns.Add("rating", typeof(Image));
            dtNow.Columns.Add("bell", typeof(Image));
            dtNow.Columns.Add("capture", typeof(Image));
            dtNow.Columns.Add("number", typeof (int));

            foreach (DataRow drNow in drsNow){ dtNow.Rows.Add(drNow.ItemArray); }

            dtNow.Columns.Add("display-name", typeof (string));
            dtNow.Columns.Add("image", typeof (Image));
            dtNow.Columns.Add("remain", typeof (int));

            foreach (DataRow drNow in dtNow.Rows)
            {
                DataRow drChan = dsTVProg.Tables["AllChannels"].Rows.Find(drNow["cid"]);
                drNow.BeginEdit();
                drNow["display-name"] = drChan["user-name"];
                drNow["image"] = drChan["icon"];
                drNow["remain"] = ((DateTime) drNow["stop"] - DateTime.Parse(datetime)).TotalSeconds/
                                  ((DateTime) drNow["stop"] - (DateTime) drNow["start"]).TotalSeconds*100;
                drNow["number"] = drChan["number"];
                if (!string.IsNullOrEmpty(drNow["desc"].ToString()))
                {
                    drNow["anons"] = Resources.GreenAnons;
                }
                else drNow["anons"] = null;
                
                SetGenre(dsTVProg, drNow);  // - установка жанра
                SetRating(dsTVProg, drNow); // - установка рейтинга
                SetBell(drNow);           // - установка значка напоминания
                SetCapture(drNow);
                drNow.EndEdit();
            }
            if (_globalFiltGenre.Length > 0)
            {
                DataRow[] drsFiltered = dtNow.Select(_globalFiltGenre);
                if (drsFiltered.Length > 0)
                {
                    dtNow = drsFiltered.CopyToDataTable();
                }
                else dtNow.Rows.Clear();
            }
            if (_globalFiltRating.Length > 0)
            {
                DataRow[] drsFiltered = dtNow.Select(_globalFiltRating);
                if (drsFiltered.Length > 0)
                {
                    dtNow = drsFiltered.CopyToDataTable();
                }
                else dtNow.Rows.Clear();
            }
            if (_globalFiltAnons.Length > 0)
            {
                DataRow[] drsFiltered = dtNow.Select(_globalFiltAnons);
                if (drsFiltered.Length > 0)
                {
                    dtNow = drsFiltered.CopyToDataTable();
                }
                else dtNow.Rows.Clear();
            }
            if (_globalFiltRemind.Length > 0)
            {
                DataRow[] drsFiltered = dtNow.Select(_globalFiltRemind);
                if (drsFiltered.Length > 0)
                {
                    dtNow = drsFiltered.CopyToDataTable();
                }
                else dtNow.Rows.Clear();
            }
            dtNow.DefaultView.Sort = "number";
            return dtNow;
        }
        
        /// <summary>
        /// Установка жанра передаче в xml
        /// </summary>
        /// <param name="idChannel">код канала</param>
        /// <param name="title">название передачи</param>
        /// <param name="tsStart">время начала передачи</param>
        /// <param name="tsStop">время окончания передачи</param>
        /// <param name="txtGenre">тип передачи</param>
        /// <returns>Успех/неуспех</returns>
        public bool SetGenreXml(string idChannel, string title, DateTime tsStart, DateTime tsStop, string txtGenre)
        {
            return SetAttributeToTelecast(idChannel, title, tsStart, tsStop, "category", txtGenre);
        }

        /// <summary>
        /// Установка жанра
        /// </summary>
        /// <param name="ds">Набор данных</param>
        /// <param name="curRow">Строка для установки</param>
        private static void SetGenre(DataSet ds, DataRow curRow)
        {
            if (string.IsNullOrEmpty(curRow["category"].ToString()))
            {
                foreach (DataRow drClassifGenre in ds.Tables["KeywordsTable"].Rows)
                {
                    foreach (string strContain in drClassifGenre["contain"].ToString().ToLower().Split(';'))
                    {
                        foreach (string strNonContain in
                            drClassifGenre["noncontain"].ToString().ToLower().Split(';'))
                        {
                            if (
                                (
                                    !string.IsNullOrEmpty(strNonContain) &&
                                    ((curRow["title"].ToString().ToLower()).Contains(strContain) &&
                                     (!(curRow["title"].ToString().ToLower()).Contains(strNonContain)))
                                ) ||
                                (string.IsNullOrEmpty(strNonContain) &&
                                 (curRow["title"].ToString().ToLower()).Contains(strContain))
                                )
                            {
                                DataRow[] drsGenre =
                                    ds.Tables["Genres"].Select("id = " + drClassifGenre["gid"]);
                                foreach (DataRow drGenre in drsGenre)
                                {
                                    curRow["category"] = drGenre["genrename"];
                                }
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(curRow["category"].ToString()))
            {
                curRow["category"] = "Без типа";
            }
            DataRow[] drsGenres = ds.Tables["Genres"].Select("genrename = '" + curRow["category"].ToString() + "'");
            foreach (DataRow dataRow in drsGenres)
            {
                if ((bool)dataRow["visible"]) curRow["genre"] = (Image)dataRow["image"];
            }
        }
        private string GetRemineForRemind(DateTime start)
        {
            TimeSpan tsForRemind = start - DateTime.Now;
            return String.Format("{0} д. {1} ч. {2} м. {3} c. ", tsForRemind.Days.ToString(),
                                 tsForRemind.Hours.ToString(), tsForRemind.Minutes.ToString(),
                                 tsForRemind.Seconds.ToString());
        } 

        /// <summary>
        /// Будущие напоминания
        /// </summary>
        /// <param name="dsTVProg"></param>
        /// <param name="mode">Событие: remind - напоминания, record - записи</param>
        /// <returns></returns>
        public DataTable GetEventInFuture(DataSet dsTVProg, string mode)
        {
            DataTable tblCurRemind = new DataTable("CurRemindTable");
            DataRow [] drsFutRem = dsTVProg.Tables["AllProgrammes"].Select(
                String.Format("{0} = True and start >= '{1}'", mode, DateTime.Now));
            tblCurRemind.Columns.Add("id", typeof(int));
            tblCurRemind.Columns.Add("cid", typeof(int));
            tblCurRemind.Columns.Add("start", typeof(DateTime));
            tblCurRemind.Columns.Add("stop", typeof(DateTime));
            tblCurRemind.Columns.Add("start_mo", typeof(DateTime));
            tblCurRemind.Columns.Add("stop_mo", typeof(DateTime));
            tblCurRemind.Columns.Add("title", typeof(string));
            tblCurRemind.Columns.Add("category", typeof(string));
            tblCurRemind.Columns.Add("desc", typeof(string));
            tblCurRemind.Columns.Add("record", typeof(bool));
            tblCurRemind.Columns.Add("remind", typeof(bool));
            tblCurRemind.Columns.Add("favname", typeof(string));
            tblCurRemind.Columns.Add("anons", typeof(Image));
            tblCurRemind.Columns.Add("genre", typeof(Image));
            tblCurRemind.Columns.Add("rating", typeof(Image));
            
            tblCurRemind.Columns.Add("bell", typeof(Image));
            tblCurRemind.Columns.Add("capture", typeof(Image));
            tblCurRemind.Columns.Add("number", typeof(int));
            foreach (DataRow drRem in drsFutRem) { tblCurRemind.Rows.Add(drRem.ItemArray); }

            tblCurRemind.Columns.Add("display-name", typeof(string));
            tblCurRemind.Columns.Add("image", typeof(Image));
            tblCurRemind.Columns.Add("remain", typeof(int));
            tblCurRemind.Columns.Add("remine", typeof(string));
            tblCurRemind.Columns.Add("day", typeof(string));
            foreach (DataRow drRem in tblCurRemind.Rows)
            {
                DataRow drChan = dsTVProg.Tables["AllChannels"].Rows.Find(drRem["cid"]);
                drRem.BeginEdit();
                drRem["display-name"] = drChan["user-name"];
                drRem["image"] = drChan["icon"];
                drRem["number"] = drChan["number"];
                DateTime tsStart = (DateTime) drRem["start"];
                drRem["remine"] = GetRemineForRemind(tsStart);

                drRem["day"] = tsStart.ToString("ddd", new CultureInfo("ru-Ru")) +
                       String.Format("({0:D2}.{1:D2})", tsStart.Day, tsStart.Month);
                SetGenre(dsTVProg, drRem);  // - установка жанра
                SetRating(dsTVProg, drRem); // - установка рейтинга
                SetBell(drRem);
                SetCapture(drRem);
                drRem.EndEdit();
            }
            tblCurRemind.Columns.Remove("start_mo");
            tblCurRemind.Columns.Remove("stop_mo");
            tblCurRemind.DefaultView.Sort = "start";
            return tblCurRemind;
        }
        /// <summary>
        /// Напоминатель
        /// </summary>
        /// <param name="dsTVProg"></param>
        /// <returns></returns>
        public DataTable Reminder(DataSet dsTVProg)
        {
            DataTable dtReminder = new DataTable("Reminder");

            DataRow[] drsRemind = dsTVProg.Tables["AllProgrammes"].Select(
                String.Format("remind = True and start <= '{0}' and stop > '{0}'",
                              DateTime.Now.AddMinutes(TVProgViewer.TVProgApp.Reminder.Default.RemindLater
                                                          ? TVProgViewer.TVProgApp.Reminder.Default.MinutesLater
                                                          : 0)));

            dtReminder.Columns.Add("id", typeof(int));
            dtReminder.Columns.Add("cid", typeof(int));
            dtReminder.Columns.Add("start", typeof(DateTime));
            dtReminder.Columns.Add("stop", typeof(DateTime));
            dtReminder.Columns.Add("start_mo", typeof(DateTime));
            dtReminder.Columns.Add("stop_mo", typeof(DateTime));
            dtReminder.Columns.Add("title", typeof(string));
            dtReminder.Columns.Add("category", typeof(string));
            dtReminder.Columns.Add("desc", typeof(string));
            dtReminder.Columns.Add("record", typeof (bool));
            dtReminder.Columns.Add("remind", typeof(bool));
            dtReminder.Columns.Add("favname", typeof(string));
            dtReminder.Columns.Add("anons", typeof(Image));
            dtReminder.Columns.Add("genre", typeof(Image));
            dtReminder.Columns.Add("rating", typeof(Image));
            dtReminder.Columns.Add("bell", typeof(Image));
            dtReminder.Columns.Add("capture", typeof(Image));
            dtReminder.Columns.Add("number", typeof(int));

            foreach (DataRow drRem in drsRemind) { dtReminder.Rows.Add(drRem.ItemArray); }

            dtReminder.Columns.Add("display-name", typeof(string));
            dtReminder.Columns.Add("image", typeof(Image));
            dtReminder.Columns.Add("remain", typeof(int));

            foreach (DataRow drRem in dtReminder.Rows)
            {
                DataRow drChan = dsTVProg.Tables["AllChannels"].Rows.Find(drRem["cid"]);
                drRem.BeginEdit();
                drRem["display-name"] = drChan["user-name"];
                drRem["image"] = drChan["icon"];
                drRem["number"] = drChan["number"];
                SetGenre(dsTVProg, drRem);  // - установка жанра
                SetRating(dsTVProg, drRem); // - установка рейтинга
               
                drRem.EndEdit();
            }
            dtReminder.Columns.Remove("start_mo");
            dtReminder.Columns.Remove("stop_mo");
            dtReminder.Columns.Remove("desc");
            dtReminder.Columns.Remove("anons");
            dtReminder.DefaultView.Sort = "start";
            return dtReminder;
        }
        
        public DataTable Recorder (DataSet dsTVProg)
        {
            DataTable dtCapturer = new DataTable("Capturer");
            DataRow[] drsCapturer = dsTVProg.Tables["AllProgrammes"].Select(
                String.Format("record = True and start <= '{0}' and stop > '{0}'", DateTime.Now));
            dtCapturer.Columns.Add("id", typeof(int));
            dtCapturer.Columns.Add("cid", typeof(int));
            dtCapturer.Columns.Add("start", typeof(DateTime));
            dtCapturer.Columns.Add("stop", typeof(DateTime));
            dtCapturer.Columns.Add("start_mo", typeof(DateTime));
            dtCapturer.Columns.Add("stop_mo", typeof(DateTime));
            dtCapturer.Columns.Add("title", typeof(string));
            dtCapturer.Columns.Add("category", typeof(string));
            dtCapturer.Columns.Add("desc", typeof(string));
            dtCapturer.Columns.Add("record", typeof(bool));
            dtCapturer.Columns.Add("remind", typeof(bool));
            dtCapturer.Columns.Add("favname", typeof(string));
            dtCapturer.Columns.Add("anons", typeof(Image));
            dtCapturer.Columns.Add("genre", typeof(Image));
            dtCapturer.Columns.Add("rating", typeof(Image));
            dtCapturer.Columns.Add("bell", typeof(Image));
            dtCapturer.Columns.Add("capture", typeof(Image));
            dtCapturer.Columns.Add("number", typeof(int));

            foreach (DataRow drRem in drsCapturer) { dtCapturer.Rows.Add(drRem.ItemArray); }

            dtCapturer.Columns.Add("display-name", typeof(string));
            dtCapturer.Columns.Add("image", typeof(Image));
            dtCapturer.Columns.Add("remain", typeof(int));

            foreach (DataRow drRem in dtCapturer.Rows)
            {
                DataRow drChan = dsTVProg.Tables["AllChannels"].Rows.Find(drRem["cid"]);
                drRem.BeginEdit();
                drRem["display-name"] = drChan["user-name"];
                drRem["image"] = drChan["icon"];
                drRem["number"] = drChan["number"];
                SetGenre(dsTVProg, drRem);  // - установка жанра
                SetRating(dsTVProg, drRem); // - установка рейтинга
               
                drRem.EndEdit();
            }
            dtCapturer.Columns.Remove("start_mo");
            dtCapturer.Columns.Remove("stop_mo");
            dtCapturer.Columns.Remove("desc");
            dtCapturer.Columns.Remove("anons");
            dtCapturer.DefaultView.Sort = "start";
            return dtCapturer;
        }
        /// <summary>
        /// Установка рейтинга в xml
        /// </summary>
        /// <param name="idChannel">код канала</param>
        /// <param name="title">название передачи</param>
        /// <param name="tsStart">дата начала передачи</param>
        /// <param name="tsStop">дата завершения передачи</param>
        /// <param name="txtRating">название рейтинга</param>
        /// <returns>Успех/неуспех</returns>
        public bool SetRatingXml (string idChannel, string title, DateTime tsStart, DateTime tsStop, string txtRating)
        {
            return SetAttributeToTelecast(idChannel, title, tsStart, tsStop, "favname", txtRating);
        }

        /// <summary>
        /// Установка рейтинга
        /// </summary>
        /// <param name="ds">Набор данных</param>
        /// <param name="curRow">Строка для установки</param>
        private void SetRating(DataSet ds, DataRow curRow)
        {
            if (String.IsNullOrEmpty(curRow["favname"].ToString()))
            {
                foreach (DataRow drClassifRating in ds.Tables["FavwordsTable"].Rows)
                {
                    foreach (string strContain in drClassifRating["contain"].ToString().ToLower().Split(';'))
                    {
                        foreach (string strNonContain in
                            drClassifRating["noncontain"].ToString().ToLower().Split(';'))
                        {
                            if (
                                (
                                    !string.IsNullOrEmpty(strNonContain) &&
                                    ((curRow["title"].ToString().ToLower()).Contains(strContain) &&
                                     (!(curRow["title"].ToString().ToLower()).Contains(strNonContain)))
                                ) ||
                                (string.IsNullOrEmpty(strNonContain) &&
                                 (curRow["title"].ToString().ToLower()).Contains(strContain))
                                )
                            {
                                DataRow[] drsRating =
                                    ds.Tables["Favorites"].Select("id = " + drClassifRating["fid"]);
                                if (drsRating.Length > 0)
                                {
                                    foreach (DataRow drFavorite in drsRating)
                                    {
                                        if ((bool) drFavorite["visible"])
                                        {
                                            curRow["favname"] = drFavorite["favname"];
                                            curRow["rating"] = drFavorite["image"];
                                            if ((bool) drClassifRating["remind"])
                                            {
                                                SetRemind(curRow["cid"].ToString(), curRow["title"].ToString(),
                                                          (DateTime) curRow["start"], (DateTime) curRow["stop"], true);
                                                curRow["remind"] = true;
                                            }
                                            else
                                            {
                                                SetRemind(curRow["cid"].ToString(), curRow["title"].ToString(),
                                                          (DateTime) curRow["start"], (DateTime) curRow["stop"], false);
                                                curRow["remind"] = false;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(strNonContain) &&
                                    curRow["title"].ToString().ToLower().Contains(strNonContain))
                                {
                                    curRow["favname"] = ds.Tables["Favorites"].Rows[0]["favname"];
                                    curRow["rating"] = ds.Tables["Favorites"].Rows[0]["image"];
                                    SetRemind(curRow["cid"].ToString(), curRow["title"].ToString(),
                                              (DateTime) curRow["start"], (DateTime) curRow["stop"], false);
                                    curRow["remind"] = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                DataRow[] drsWithoutRating = ds.Tables["Favorites"].Select(
                    String.Format("favname = '{0}'", curRow["favname"].ToString())
                    );
                foreach (DataRow drFavorite in drsWithoutRating)
                {
                    if ((bool)drFavorite["visible"])
                    {
                        curRow["rating"] = drFavorite["image"];
                    }
                }
            }
            if (String.IsNullOrEmpty(curRow["rating"].ToString()))
            {
                DataRow[] drsWithoutRating = ds.Tables["Favorites"].Select("favname = 'Без рейтинга'");
                foreach (DataRow drFavorite in drsWithoutRating)
                {
                    if ((bool) drFavorite["visible"])
                    {
                        curRow["favname"] = "Без рейтинга";
                        curRow["rating"] = drFavorite["image"];
                    }
                }
            }
        }

        /// <summary>
        /// Установка статуса колокольчика строке
        /// </summary>
        /// <param name="curRow">Строка</param>
        public void SetBell(DataRow curRow)
        {
            if (String.IsNullOrEmpty(curRow["remind"].ToString()))
            {
                curRow["remind"] = false;
            }
            curRow["bell"] = (bool) curRow["remind"] ? Resources.bell : Resources.bellempty;
        }
        
        /// <summary>
        /// Установка статуса видеозахвата строке
        /// </summary>
        /// <param name="curRow">Строка</param>
        public void SetCapture(DataRow curRow)
        {
            if (String.IsNullOrEmpty(curRow["record"].ToString()))
            {
                curRow["record"] = false;
            }
            curRow["capture"] = (bool) curRow["record"] ? Resources.capture : Resources.capturempty;
        }

        /// <summary>
        /// Установка напоминания
        /// </summary>
        /// <param name="idChannel">Код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="tsStart">Время начала передачи</param>
        /// <param name="tsStop">Время завершения передачи</param>
        /// <param name="flag">Флаг для установки</param>
        public bool SetRemind(string idChannel, string title, DateTimeOffset tsStart, DateTimeOffset tsStop, bool flag)
        {
            return SetAttributeToTelecast(idChannel, title, tsStart, tsStop, "remind", flag.ToString());
        }
        
        /// <summary>
        /// Установка флага для видеосрабатывания
        /// </summary>
        /// <param name="idChannel">Код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="tsStart">Время начала передачи</param>
        /// <param name="tsStop">Время завершения передачи</param>
        /// <param name="flag">Флаг для установки</param>
        /// <returns></returns>
        public bool SetRecord(string idChannel, string title, DateTimeOffset tsStart, DateTimeOffset tsStop, bool flag)
        {
            return SetAttributeToTelecast(idChannel, title, tsStart, tsStop, "record", flag.ToString());
        }

        /// <summary>
        /// Установка аттрибута со значением для телепрограммы
        /// </summary>
        /// <param name="idChannel">код канала</param>
        /// <param name="title">название передачи</param>
        /// <param name="start">дата начала передачи</param>
        /// <param name="stop">дата окончания передачи</param>
        /// <param name="attr">название аттрибута</param>
        /// <param name="val">присваиваемое значение аттрибуту</param>
        /// <returns>успех/неуспех</returns>
        private bool SetAttributeToTelecast(string idChannel, string title, DateTimeOffset start, DateTimeOffset stop, 
            string attr, string val)
        {
            string xmlPubFileName = Path.Combine(Application.StartupPath, Preferences.xmlPubFileName);

            string strStart = Util.ConvertDateTime(start);
            string strStop = Util.ConvertDateTime(stop);

            bool pr = false; // - для проверки добавления или изменения напоминания
            
            IEnumerable<XElement> els = from programme in _pubXdoc.Descendants("programme")
                                        where
                                       programme.Attribute("channel") != null && 
                                       programme.Attribute("start") != null &&
                                       programme.Attribute("stop") != null &&
                                       programme.Attribute("channel").Value == idChannel && 
                                       programme.Attribute("start").Value == strStart &&
                                       programme.Attribute("stop").Value == strStop
                                       select programme;
            foreach (XElement xElem in els)
            {
                if (xElem.HasAttributes)
                {
                    if (xElem.HasElements)
                    {
                        if (xElem.Element("title") != null)
                        {
                            if (xElem.Element("title").Value == title)
                            {
                                if (xElem.Element(attr) != null)
                                {
                                    if (xElem.Element(attr).Value.ToLower() != val.ToLower())
                                    {
                                        xElem.Element(attr).SetValue(val);
                                        pr = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    xElem.AddFirst(new XElement(attr) {Value = val});
                                    pr = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (pr)
            {
                try
                {
                    _pubXdoc.Save(xmlPubFileName);
                }
                catch (Exception ex)
                {
                    Statics.EL.LogException(ex);
                    return false;
                }
                pr = false;
            }
            return true;
        }

        /// <summary>
        /// Установка напоминания
        /// </summary>
        /// <param name="lstRemind">Список параметров</param>
        /// <returns></returns>
        public bool SetRemind(List<Tuple<string, string, DateTime, DateTime, bool>> lstRemind)
        {
            bool pr = false;
            foreach (Tuple<string, string, DateTime, DateTime, bool> tupProg in lstRemind)
            {
                pr = SetAttributeToTelecast(tupProg.Item1, tupProg.Item2,
                                            tupProg.Item3, tupProg.Item4, "remind", tupProg.Item5.ToString());
            }
            return pr;
        }

        /// <summary>
        /// Следующая передача телевизионного вещания
        /// </summary>
        /// <param name="sourceChannels">Каналы</param>
        /// <param name="sourceTVProg">Программа телепередач</param>
        /// <param name="datetime">Время для исполнения</param>
        /// <returns></returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public DataTable GetShowTVNext(DataSet dsTVProg, string datetime)
        {
            string filterStr = String.Format("start <= '{0}' and stop > '{0}' ", DateTime.Now);
            filterStr += _globalFiltChan;
            if (dsTVProg.Tables["AllProgrammes"].Rows.Count > 0 && dsTVProg.Tables["AllProgrammes"].Columns.Contains("start") &&
                dsTVProg.Tables["AllProgrammes"].Columns.Contains("start"))
            {
                DataRow[] drsNow = dsTVProg.Tables["AllProgrammes"].Select(filterStr);

                DataTable dtNext = new DataTable("Next");
                dtNext.Columns.Add("id", typeof (int));
                dtNext.Columns.Add("cid", typeof (int));
                dtNext.Columns.Add("start", typeof (DateTime));
                dtNext.Columns.Add("stop", typeof (DateTime));
                dtNext.Columns.Add("start_mo", typeof (DateTime));
                dtNext.Columns.Add("stop_mo", typeof (DateTime));
                dtNext.Columns.Add("title", typeof (string));
                dtNext.Columns.Add("category", typeof (string));
                dtNext.Columns.Add("desc", typeof (string));
                dtNext.Columns.Add("record", typeof (bool));
                dtNext.Columns.Add("remind", typeof (bool));
                dtNext.Columns.Add("favname", typeof (string));
                dtNext.Columns.Add("anons", typeof (Image));
                dtNext.Columns.Add("genre", typeof (Image));
                dtNext.Columns.Add("bell", typeof (Image));
                dtNext.Columns.Add("capture", typeof (Image));
                dtNext.Columns.Add("rating", typeof (Image));
                dtNext.Columns.Add("number", typeof (int));
                foreach (DataRow drNow in drsNow)
                {
                    string nextFilter = String.Empty;
                    string tsNext = Convert.ToDateTime(drNow["stop"]).AddMinutes(1).ToString();
                    if (datetime.Length == 0)
                    {
                        nextFilter = "start <= '" + tsNext + "' and stop > '" +
                                     tsNext + "' and cid = " + drNow["cid"];
                    }
                    else
                    {
                        nextFilter = String.Format("start > '{0}' and start <= '{1}' and cid = {2}",
                                                   DateTime.Now, datetime, drNow["cid"]);
                    }
                    DataRow[] drsNext = dsTVProg.Tables["AllProgrammes"].Select(nextFilter);
                    foreach (DataRow drNext in drsNext)
                    {
                        dtNext.Rows.Add(drNext.ItemArray);
                    }
                }
                dtNext.Columns.Add("display-name", typeof (string));
                dtNext.Columns.Add("image", typeof (Image));
                dtNext.Columns.Add("remain", typeof (DateTime));
                foreach (DataRow drNext in dtNext.Rows)
                {
                    DataRow drChan = dsTVProg.Tables["AllChannels"].Rows.Find(drNext["cid"]);
                    drNext.BeginEdit();
                    if (drChan != null)
                    {
                        drNext["display-name"] = drChan["user-name"];
                        drNext["image"] = drChan["icon"];
                        drNext["number"] = drChan["number"];
                    }
                    double remTotal = ((DateTime) drNext["start"] - DateTime.Now).TotalSeconds;
                    if (remTotal > 0)
                    {
                        drNext["remain"] = new DateTime(1, 1, 1).AddSeconds((int) remTotal);
                    }
                    if (!string.IsNullOrEmpty(drNext["desc"].ToString()))
                    {
                        drNext["anons"] = Resources.GreenAnons;
                    }
                    else drNext["anons"] = null;

                    SetGenre(dsTVProg, drNext);
                    SetRating(dsTVProg, drNext);
                    SetBell(drNext);
                    SetCapture(drNext);
                    drNext.EndEdit();
                }
                if (_globalFiltGenre.Length > 0)
                {
                    DataRow[] drsFiltered = dtNext.Select(_globalFiltGenre);
                    if (drsFiltered.Length > 0)
                    {
                        dtNext = drsFiltered.CopyToDataTable();
                    }
                    else
                    {
                        dtNext.Rows.Clear();
                    }
                }
                if (_globalFiltRating.Length > 0)
                {
                    DataRow[] drsFiltered = dtNext.Select(_globalFiltRating);
                    if (drsFiltered.Length > 0)
                    {
                        dtNext = drsFiltered.CopyToDataTable();
                    }
                    else
                    {
                        dtNext.Rows.Clear();
                    }
                }
                if (_globalFiltAnons.Length > 0)
                {
                    DataRow[] drsFiltered = dtNext.Select(_globalFiltAnons);
                    if (drsFiltered.Length > 0)
                    {
                        dtNext = drsFiltered.CopyToDataTable();
                    }
                    else
                    {
                        dtNext.Rows.Clear();
                    }
                }
                if (_globalFiltRemind.Length > 0)
                {
                    DataRow[] drsFiltered = dtNext.Select(_globalFiltRemind);
                    if (drsFiltered.Length > 0)
                    {
                        dtNext = drsFiltered.CopyToDataTable();
                    }
                    else
                    {
                        dtNext.Rows.Clear();
                    }
                }
                dtNext.DefaultView.Sort = "number";

                return dtNext;
            }
            return new DataTable("<No Data>");
        }
        /// <summary>
        /// Фильтрация по каналам
        /// </summary>
        /// <param name="sourceTVProg">Таблица-источник телепрограммы</param>
        /// <param name="period">Период</param>
        /// <param name="keyDate">На дату</param>
        /// <param name="id">Код канала</param>
        /// <returns></returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public DataTable FiltTVProgram(DataSet dsTVProg, int period, DateTime keyDate, string id)
        {
            string filterStr = "start >= '" +
                               keyDate.AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                   Settings.Default.BeginEndTime.Minutes) + "' and stop < '" +
                               keyDate.AddDays(period).AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                   Settings.Default.BeginEndTime.Minutes) + "'";
            if (id.Length > 0) filterStr += " and cid = " + id;
            else filterStr += _globalFiltChan;
            
            DataRow[] drsDate = dsTVProg.Tables["AllProgrammes"].Select(filterStr);

            DataTable dtDate = new DataTable("Now");
            dtDate.Columns.Add("id", typeof(int));
            dtDate.Columns.Add("cid", typeof(int));
            dtDate.Columns.Add("start", typeof(DateTime));
            dtDate.Columns.Add("stop", typeof(DateTime));
            dtDate.Columns.Add("start_mo", typeof(DateTime));
            dtDate.Columns.Add("stop_mo", typeof(DateTime));
            dtDate.Columns.Add("title", typeof(string));
            dtDate.Columns.Add("category", typeof(string));
            dtDate.Columns.Add("desc", typeof(string));
            dtDate.Columns.Add("record", typeof (bool));
            dtDate.Columns.Add("remind", typeof(bool));
            dtDate.Columns.Add("favname", typeof(string));
            dtDate.Columns.Add("anons", typeof (Image));
            dtDate.Columns.Add("bell", typeof(Image));
            dtDate.Columns.Add("capture", typeof(Image));
            dtDate.Columns.Add("genre", typeof(Image));
            dtDate.Columns.Add("rating", typeof(Image));
            foreach (DataRow drDate in drsDate)
            {
                dtDate.Rows.Add(drDate.ItemArray);
            }
           /* if (period > 1)
            {*/
                dtDate.Columns.Add("display-name", typeof(string));
                dtDate.Columns.Add("image", typeof(Image));
                dtDate.Columns.Add("day", typeof(string));
                foreach (DataRow drWeek in dtDate.Rows)
                {
                    DataRow drChan = dsTVProg.Tables["AllChannels"].Rows.Find(drWeek["cid"]);
                    drWeek["display-name"] = drChan["user-name"];
                    drWeek["image"] = drChan["icon"];
                    DateTime tsStart = Convert.ToDateTime(drWeek["start"]);
                    drWeek["day"] = tsStart.ToString("ddd", new CultureInfo("ru-Ru")) + 
                        String.Format("({0:D2}.{1:D2})", tsStart.Day, tsStart.Month );
                }
          //  }
            foreach (DataRow drDate in dtDate.Rows)
            {
                drDate.BeginEdit();

                if (!string.IsNullOrEmpty(drDate["desc"].ToString()))
                {
                    drDate["anons"] = Resources.GreenAnons;
                }
                else drDate["anons"] = null;
                
                SetGenre(dsTVProg, drDate);
                SetRating(dsTVProg, drDate);
                SetBell(drDate);
                SetCapture(drDate);
                drDate.EndEdit();
            }
            if (_globalFiltGenre.Length > 0)
            {
                DataRow[] drsFiltered = dtDate.Select(_globalFiltGenre);
                if (drsFiltered.Length > 0)
                {
                    dtDate = drsFiltered.CopyToDataTable();
                }
                else dtDate.Rows.Clear();
            }
            if (_globalFiltRating.Length > 0)
            {
                DataRow[] drsFiltered = dtDate.Select(_globalFiltRating);
                if (drsFiltered.Length > 0)
                {
                    dtDate = drsFiltered.CopyToDataTable();
                }
                else dtDate.Rows.Clear();
            }
            if (_globalFiltAnons.Length > 0)
            {
                DataRow[] drsFiltered = dtDate.Select(_globalFiltAnons);
                if (drsFiltered.Length > 0)
                {
                    dtDate = drsFiltered.CopyToDataTable();
                }
                else dtDate.Rows.Clear();
            }
            if (_globalFiltRemind.Length > 0)
            {
                DataRow[] drsFiltered = dtDate.Select(_globalFiltRemind);
                if (drsFiltered.Length > 0)
                {
                    dtDate = drsFiltered.CopyToDataTable();
                }
                else dtDate.Rows.Clear();
            }
            return dtDate;
        }
        
        public Image GetChannelImage(string cid, string displayName)
        {
            string gifFileName = Application.StartupPath +  @"\Gifs\" + cid + ".gif";
            if (File.Exists(gifFileName)) return Image.FromFile(gifFileName);
            else
            {
                try
                {
                    string valChan = String.Empty;
                 //   Statics.dictChanCode.TryGetValue(displayName, out valChan);
                    return  (Image) new Bitmap(gifFileName); // ResourceImages.ResourceManager.GetObject("_" + valChan);
                }
                catch
                {
                    return Resources.satellite_dish;
                }
            }
            return Resources.satellite_dish;
        }
        /// <summary>
        /// Удаление каналов
        /// </summary>
        /// <param name="idChannels">Код канала</param>
        /// <returns></returns>
        public bool DeleteChannels(List<string> idChannels)
        {
            Statics.ShowLogo(Resources.StatusDeleteChanText, 20);
            string xsltManyChans = Path.Combine(Application.StartupPath, @"Data\RemoveManyChans.xslt");
            bool pr = false; // - для проверки удаляемого значения
            try
            {
                foreach (string id in idChannels)
                {
                    XDocument newPubXdoc = new XDocument();
                    XslCompiledTransform transform = null;
                    using (XmlWriter writer = newPubXdoc.CreateWriter())
                    try
                    {
                        transform = new XslCompiledTransform(); // - объект для трансформации
                        transform.Load(xsltManyChans,
                                       new XsltSettings() {EnableDocumentFunction = true},
                                       new XmlUrlResolver());
                        // Трансформация с параметром
                        XsltArgumentList xsltArgs = new XsltArgumentList();
                        xsltArgs.AddParam("id", "", id);
                        transform.Transform(_pubXdoc.CreateReader(), xsltArgs, writer);
                    }
                    catch
                    {
                        pr = false;
                    }
                    finally
                    {
                        if (transform != null)
                        {
                            transform.TemporaryFiles.Delete();
                        }
                    }
                    if (newPubXdoc.Descendants("programme").Any())
                    {
                        _pubXdoc = new XDocument(newPubXdoc);
                    }
                }
                _pubXdoc.Save(_xmlPubFileName);
                pr = true;
            }
            catch (Exception)
            {
                pr = false;
            }
            Statics.ShowLogo(Resources.StatusDeleteChanText, 100);
            Statics.HideLogo();
            return pr;
        }
        /// <summary>
        /// Установка анонса
        /// </summary>
        /// <param name="idChan">Код канала</param>
        /// <param name="start">Время начала передачи</param>
        /// <param name="stop">Время завершения передачи</param>
        /// <param name="title">Название передачи</param>
        /// <param name="desc">Анонс для изменения</param>
        public bool SaveDescription(string idChan, DateTime start, DateTime stop, string title, string desc)
        {
            return SetAttributeToTelecast(idChan, title, start, stop, "desc", desc);
        }

        /// <summary>
        /// Установка фильтра по жанрам
        /// </summary>
        /// <param name="lstCat"></param>
        public void SetGlobGenreFilter(List<string> lstCat)
        {
            _globalFiltGenre = String.Empty;
            foreach (string category in lstCat)
            {
                _globalFiltGenre += String.Format("category = '{0}' or ", category);
            }
            if (_globalFiltGenre.Length > 0)
            {
                _globalFiltGenre = _globalFiltGenre.Substring(0, _globalFiltGenre.Length - 4);
            }
        }

        public void SetGlobRatingFilter(List<string> lstCat)
        {
            _globalFiltRating = String.Empty;
            foreach (string favname in lstCat)
            {
                _globalFiltRating += String.Format("favname = '{0}' or ", favname);
            }
            if (_globalFiltRating.Length > 0)
            {
                _globalFiltRating = _globalFiltRating.Substring(0, _globalFiltRating.Length - 4);
            }
        }

        public void SetGlobalAnonsFilter(bool filt)
        {
            if (filt)
            {
                _globalFiltAnons = "desc <> ''";
            }
            else
            {
                _globalFiltAnons = String.Empty;
            }
        }

        public void SetGlobalRemindFilter(bool filt)
        {
            if (filt)
            {
                _globalFiltRemind = "remind = true";
            }
            else
            {
                _globalFiltRemind = String.Empty;
            }
        }
    }
}
