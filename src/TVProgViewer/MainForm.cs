﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Linq;
using DirectX.Capture;
using DShowNET;
using Microsoft.Win32;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Controls;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;
using Timer = System.Windows.Forms.Timer;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.TVProgApp.Controllers;
using TVProgViewer.BusinessLogic.ProgInterfaces;
using TVProgViewer.BusinessLogic.Users;
 
namespace TVProgViewer.TVProgApp
{
    public partial class MainForm : Form
    {
        #region Enums
        public enum GroupingBy
        {
            Dates = 0,
            Channels = 1
        }
        #endregion
        #region Structures
       
        private ProgPeriod _periodMinMax = new ProgPeriod();
        #endregion
        #region Variables
        // Private
        private TVProgClass _tvProg = new TVProgClass();
        private readonly DataSet _dsTvProg = new DataSet("TVProg");
        private SerializableDataClass _serialData;
        private static readonly ImageList ImgListDays = new ImageList();

        private DataTable _dtProgramme = new DataTable("AllProgramme");
        

        private SearchDialog _schForm;
        private GenreForm _genreForm;
        private RatingForm _ratingForm;
        private RemindForm _remForm;
        private readonly Registration reg = new Registration();

        private int _curNowRowIndex, _curNextRowIndex,
            _curChannelsRowIndex, _curProgsRowIndex = 0;
        private bool _bClose;
        private bool _bCheckedAll;
        private bool _firstin = true;
        private TimeSpan _tsTo = Settings.Default.BeginEndTime;
        private TimeSpan _tsFinalTo = Settings.Default.BeginEndTime;
        private DateTimeOffset datetime_for_now = DateTimeOffset.Now;
        private DateTimeOffset datetime_for_next = new DateTimeOffset(new DateTime(1800,1,1));
        private static readonly string _exp_folder = Application.StartupPath + "\\Templates\\";
        private OptionsDialog _optDlg = null;
        private ToolStripMenuItem tsmiCurrentRecords = null;
        private ToolStripMenuItem csmiViewChannel = null;
        private AboutForm aboutForm = new AboutForm();
        private Capture _capture = null;
        private List<SystemChannel> _chList;
        #endregion
        #region Public Propierties

        public SerializableDataClass SerialData
        {
            get { return _serialData; }
            set { _serialData = value; }
        }
        public DateTimeOffset Start
        {
            get { return _periodMinMax.dtStart; }
        }
        public DateTimeOffset Stop
        {
            get { return _periodMinMax.dtEnd; }
        }
        public DataRow[] Channels
        {
            get { return _dsTvProg.Tables["AllChannels"].Select("", "number ASC"); }
        }
        public DataTable Keywords
        {
            get { return _dsTvProg.Tables["KeywordsTable"]; }
        }
        public DataTable GenresTable
        {
            get { return _dsTvProg.Tables["Genres"]; }
        }
        public DataTable Programms
        {
            get { return _dsTvProg.Tables["AllProgrammes"]; }
        }
        public DataTable Ratings
        {
            get { return _dsTvProg.Tables["Favorites"]; }
        }
        public DataTable Favwords
        {
            get { return _dsTvProg.Tables["FavwordsTable"]; }
        }
        public Capture PropCapture
        {
            get { return _capture; }
            set { _capture = value; }
        }
        #endregion
        #region Delegates
        
        #endregion
        #region Events

        #endregion
        #region Constructor
        public MainForm()
        {
            Statics.ShowLogo(Properties.Resources.StatusInitText, 0);

            InitializeComponent();
            FillProvTypes();
            lblFio.Text = (TVEnvironment.currentUser != null) ? TVEnvironment.currentUser.ToString() : string.Empty;

            Statics.ShowLogo(Properties.Resources.StatusInitText, 100);

            Statics.ShowLogo(Resources.StatusSetTVTunerText, 0);
            try { Preferences.filters = new Filters(); }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
            }
            
            SetVideoAudioSettings();
            Statics.ShowLogo(Resources.StatusSetTVTunerText, 100);
            Statics.ShowLogo(Resources.StatusSetOptionsText, 0);
            _schForm = new SearchDialog(_tvProg);
            _remForm = new RemindForm(_tvProg);
            int val = 144 -
                      (int)
                      (new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0) - Settings.Default.BeginEndTime).TotalMinutes/10;
            trackBarTo.Value = (val >= trackBarTo.Minimum) && (val <= trackBarTo.Maximum) ? val : 0;
            chkAndTo.Text = Resources.AndToText;
            switch (Preferences.showAnons)
            {
                case Preferences.ShowAnons.Ifitis:
                    tsmiIfitis.Checked = true;
                    break;
                case Preferences.ShowAnons.Always:
                    tsmiAlways.Checked = true;
                    break;
                case Preferences.ShowAnons.Never:
                    tsmiNever.Checked = true;
                    break;
            }
            FillImageList();
            SetRemindMessage();
            SetRemindVolume();
            SetRemindStatus();
            Statics.ShowLogo(Resources.StatusSetOptionsText, 100);
            LoadDataSetAndProg();
            _optDlg = new OptionsDialog(_capture);
            _optDlg.SendComplete += new EventHandler<CaptureMessage>(capture_CaptureReady);
            Preferences.mainTabLoad =
                (Preferences.MainTabLoad)Enum.Parse(typeof(Preferences.MainTabLoad), Settings.Default.MainTabLoad);
            switch ((byte)Preferences.mainTabLoad)
            {
                case 0:
                    tsmiNow.Checked = true;
                    tsmiNext.Checked = tsmiDays.Checked = tsmiChannel.Checked = false;
                    break;
                case 1:
                    tsmiNext.Checked = true;
                    tsmiNow.Checked = tsmiDays.Checked = tsmiChannel.Checked = false;
                    break;
                case 2:
                    tsmiDays.Checked = true;
                    tsmiNow.Checked = tsmiNext.Checked = tsmiChannel.Checked = false;
                    break;
                case 3:
                    tsmiChannel.Checked = true;
                    tsmiNow.Checked = tsmiNext.Checked = tsmiChannel.Checked = false;
                    break;
            }
            if (Settings.Default.LocalizationSetting == "ru-RU")
            {
                tsmiRussian.Checked = true;
                tsmiEnglish.Checked = false;
            }
            else
            {
                if (Settings.Default.LocalizationSetting == "en-US")
                {
                    tsmiRussian.Checked = false;
                    tsmiEnglish.Checked = true;
                }
            }
            tabMain.SelectTab((byte)Preferences.mainTabLoad);
            Preferences.WindowMode winMode =
                (Preferences.WindowMode) Enum.Parse(typeof (Preferences.WindowMode), Settings.Default.WindowMode);
            switch (winMode)
            {
                case Preferences.WindowMode.Minimize: this.WindowState = FormWindowState.Minimized;
                    break;
                case Preferences.WindowMode.Restore: this.WindowState = FormWindowState.Normal;
                    break;
                case Preferences.WindowMode.Maximize: this.WindowState = FormWindowState.Maximized;
                    break;
            }
            lblTime.Text = DateTime.Now.ToShortTimeString();
            
        }

        #endregion
        
        #region Private Methods

        private async void tscbTvProgProvider_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (tscbTvProgProvider.ComboBox.SelectedValue == null ||
                Convert.ToInt32(tscbTvProgProvider.ComboBox.SelectedValue) == -1 
                || TVEnvironment.providerTypeList == null)
                return;
            
            tscbProgType.ComboBox.SelectedValue = (from svtp in TVEnvironment.providerTypeList
                                                   where svtp.TVProgProviderID == Convert.ToInt32(tscbTvProgProvider.ComboBox.SelectedValue)
                                                   select svtp.TypeProgID).First();
            TVEnvironment.systemChannelList = await TvProgController.GetSystemChannelAsyncList(
                                     Convert.ToInt32(tscbTvProgProvider.ComboBox.SelectedValue));
            if (TVEnvironment.currentUser != null)
            {
                List<UserChannel> userChannels = Controllers.TvProgController
                    .GetUserChannelList(TVEnvironment.currentUser.UserID,
                    TVEnvironment.systemChannelList.First().TVProgViewerID).ToList();
                _chList = TVEnvironment.systemChannelList.ToList<SystemChannel>();
                _chList.ForEach(sch =>
                {
                    sch.Visible = userChannels.Find(uch => uch.ChannelID == sch.ChannelID) != null;
                });
            }
            FillImageList();
        }

        private void tscbProgType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (tscbProgType.ComboBox.SelectedValue == null ||
                Convert.ToInt32(tscbProgType.ComboBox.SelectedValue) == -1 || 
                TVEnvironment.providerTypeList == null)
                return;

            timerRefresh.Enabled = false;
            tscbTvProgProvider.ComboBox.SelectedValue = (from svpr in TVEnvironment.providerTypeList
                                                         join ds in
                                                             (from pr in TVEnvironment.providerTypeList
                                                              select new { TVProgProviderID = pr.TVProgProviderID, ProviderName = pr.ProviderName })
                                                                 .Distinct()
                                                                 .ToList() on svpr.TVProgProviderID equals ds.TVProgProviderID
                                                         where svpr.TypeProgID == Convert.ToInt32(tscbProgType.ComboBox.SelectedValue)
                                                         select svpr.TVProgProviderID).First();
            
            ShowData();
            timerRefresh.Enabled = true;
        }

        public void SetVisibles()
        {
            cbWeekNow.Visible = false;
            dgNow.AutoGenerateColumns = false;
            colId.Visible = false;
            colChanId.Visible = false;
            colChannelId.Visible = false;
            colNextCID.Visible = false;
            if (TVEnvironment.currentUser == null)
            {
                
                colBell.Visible = false;
                colFavName.Visible = false;
                colGenre.Visible = false;
                colRating.Visible = false;
                colChanBell.Visible = false;
                colChanFavName.Visible = false;
                colChanGenre.Visible = false;
                colChannelBell.Visible = false;
                colChannelCategory.Visible = true;
                colChannelFavName.Visible = false;
                colChannelGenre.Visible = false;
                colChannelRec.Visible = false;
                colChanRec.Visible = false;
                colNextBell.Visible = false;
                colNextFavName.Visible = false;
                colNextGenre.Visible = false;
                colNextCategory.Visible = true;
                colNextRating.Visible = false;
                colNextRec.Visible = false;
                colProgCategory.Visible = true;
                colToEnd.Visible = true;
                
            }
            else
            {
                colBell.Visible = true;
                colFavName.Visible = true;
                colGenre.Visible = true;
                colRating.Visible = true;
                colChanBell.Visible = true;
                colChanFavName.Visible = true;
                colChanGenre.Visible = true;
                colChannelBell.Visible = true;
                colChannelCategory.Visible = false;
                colNextTitle.Visible = true;
                colNextBell.Visible = true;
                colNextFavName.Visible = true;
                colNextGenre.Visible = true;
                colNextRating.Visible = true;
                colNextRec.Visible = true;
                colNextCategory.Visible = false;
                colNextID.Visible = false;
                colProgCategory.Visible = false;
            }
        }

        /// <summary>
        /// Отображение данных после настройки каналов
        /// </summary>
        public async void ShowData()
        {
           /* bool firstOnce = false;
                      

            DataTable dtFiltChannels = _tvProg.AddChannelsInfo(_dsTvProg.Tables["AllChannels"],
                                                              ref firstOnce);
            dtFiltChannels.TableName = "AllChannels";
            _dsTvProg.Relations.Clear();
            _dsTvProg.Tables["AllProgrammes"].Constraints.Clear();
            _dsTvProg.Tables["AllChannels"].Constraints.Clear();
            _dsTvProg.Tables.Remove("AllChannels");
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = dtFiltChannels.Columns["id"];
            dtFiltChannels.PrimaryKey = keyColumns;
            _dsTvProg.Tables.Add(dtFiltChannels);

            _dsTvProg.Relations.Add("FK_ChannelID_ProgChanID", _dsTvProg.Tables["AllChannels"].Columns["id"],
                _dsTvProg.Tables["AllProgrammes"].Columns["cid"]);
            _tvProg.SetGlobalFiltChan(_dsTvProg.Tables["AllChannels"]);
            ModifyDiff();
            dgNow.AutoGenerateColumns = dgNext.AutoGenerateColumns =
                dgChannels.AutoGenerateColumns = dgProgs.AutoGenerateColumns = false;*/
           // DataTable dtNow = _tvProg.GetShowTVNow(_dsTvProg, datetime_for_now);
           // DataTable dtNext = _tvProg.GetShowTVNext(_dsTvProg, datetime_for_next);
            SetVisibles();
            int typeProgID = GetTypeProgID();
            FillImageList();
            ShowProgPeriodInfo(typeProgID);
            SystemProgramme[] now;
            SystemProgramme[] next;
            User user = TVEnvironment.currentUser;
            if (user == null)
            {
                now = await TvProgController.GetSystemProgrammesAtNowAsycList(typeProgID, datetime_for_now);
                next = await TvProgController.GetSystemProgrammesAtNextAsycList(typeProgID, datetime_for_next);
            }
            else
            {
                now = await TvProgController.GetUserProgrammesAtNowAsyncList(user.UserID, typeProgID, datetime_for_now);
                next = await TvProgController.GetUserProgrammesAtNextAsyncList(user.UserID, typeProgID, datetime_for_next);
            }
            now.ToList<SystemProgramme>().ForEach(p => { p.ChannelContent = imgLst.Images[string.Concat(p.CID, "_25")]; });
            next.ToList<SystemProgramme>().ForEach(p => { p.ChannelContent = imgLst.Images[string.Concat(p.CID, "_25")]; });
            dgNow.DataSource = now;
            dgNext.DataSource = next;
            
            tvWeeks.Nodes.Clear();
            if (TVEnvironment.currentUser == null)
                CreateTree(TVEnvironment.systemChannelList.ToList<IChannel>(), GroupingBy.Dates);
            else
                CreateTree(_chList.Where(s => s.Visible).OrderBy(s => s.OrderCol).ToList<IChannel>(), GroupingBy.Dates);
            

            tvChannels.Nodes.Clear();
            if (TVEnvironment.currentUser == null)
               CreateTree(TVEnvironment.systemChannelList.ToList<IChannel>(), GroupingBy.Channels);
            else
                CreateTree(_chList.Where(s => s.Visible).OrderBy(s => s.OrderCol).ToList<IChannel>(), GroupingBy.Channels);
        }

        private void ShowProgPeriodInfo(int typeProgID)
        {
            _periodMinMax = TvProgController.GetSystemProgrammePeriod(typeProgID);
            BindTaskPriorityCombo(cbWeekNow);
            BindTaskPriorityCombo(cbWeekNext);
            cbWeekNow.Text = cbWeekNext.Text = DateTime.Now.ToShortDateString();
            slblPeriod.Text = String.Format("{0, 33}", _periodMinMax.dtStart.DateTime.ToShortDateString() +
               "—" + _periodMinMax.dtEnd.DateTime.ToShortDateString());
            cbWeekNow.Visible = true;
        }

        public int GetTypeProgID()
        {
            if (tscbProgType.ComboBox.SelectedValue == null || (int?)tscbProgType.ComboBox.SelectedValue == -1)
                return 1;
            int typeProgID = Convert.ToInt32(tscbProgType.ComboBox.SelectedValue);
            return typeProgID;
        }
        private async void FillProvTypes()
        {
            tscbTvProgProvider.ComboBox.DisplayMember = "ProviderName";
            tscbTvProgProvider.ComboBox.ValueMember = "TVProgProviderID";
            tscbProgType.ComboBox.DisplayMember = "TypeName";
            tscbProgType.ComboBox.ValueMember = "TypeProgID";

            tscbTvProgProvider.ComboBox.SelectionChangeCommitted += new EventHandler(tscbTvProgProvider_SelectionChangeCommitted);
            tscbProgType.ComboBox.SelectionChangeCommitted += new EventHandler(tscbProgType_SelectionChangeCommitted);
            tscbTvProgProvider.ComboBox.DataSource = new List<object> { new { TVProgProviderID = -1, ProviderName = "Идёт загрузка..." } };
            tscbProgType.ComboBox.DataSource = new List<object> { new { TypeProgID = -1, TypeName = "Идёт загрузка..." } };
            TVEnvironment.providerTypeList = await TvProgController.GetProviderTypeAsyncList();
            tscbTvProgProvider.ComboBox.DataSource = (from pr in TVEnvironment.providerTypeList
                                                          select new {TVProgProviderID = pr.TVProgProviderID, ProviderName = pr.ProviderName})
                                                          .Distinct()
                                                          .ToList();
            
            tscbProgType.ComboBox.DataSource = (from tp in TVEnvironment.providerTypeList
                                                select new { TypeProgID = tp.TypeProgID, TypeName = tp.TypeName })
                                                .Distinct()
                                                .ToList();
            TVEnvironment.systemChannelList = await TvProgController.GetSystemChannelAsyncList(
                Convert.ToInt32(tscbTvProgProvider.ComboBox.SelectedValue));
            if (TVEnvironment.currentUser != null)
            {
                List<UserChannel> userChannels = Controllers.TvProgController
                    .GetUserChannelList(TVEnvironment.currentUser.UserID,
                    TVEnvironment.systemChannelList.First().TVProgViewerID).ToList();
                _chList = TVEnvironment.systemChannelList.ToList<SystemChannel>();
                _chList.ForEach(sch =>
                {
                    sch.Visible = userChannels.Find(uch => uch.ChannelID == sch.ChannelID) != null;
                    if (sch.Visible)
                        sch.OrderCol = userChannels.Find(uch => uch.ChannelID == sch.ChannelID).OrderCol;
                });
            }
        }

        

                
        private async void MainForm_Load(object sender, EventArgs e)
        {
            TVEnvironment.systemChannelList = await TvProgController.GetSystemChannelAsyncList(1);
            if (TVEnvironment.currentUser != null)
            {
                List<UserChannel> userChannels = Controllers.TvProgController
                    .GetUserChannelList(TVEnvironment.currentUser.UserID,
                    TVEnvironment.systemChannelList.First().TVProgViewerID).ToList();
                _chList = TVEnvironment.systemChannelList.ToList<SystemChannel>();
                _chList.ForEach(sch =>
                {
                    sch.Visible = userChannels.Find(uch => uch.ChannelID == sch.ChannelID) != null;
                });
            }
            SetStylesAndSettings();
            ShowData();
            UpdateDataGridNow(sender, e, DateTimeOffset.Now);
        }


        
        /// <summary>
        /// Установка стилей и настроек
        /// </summary>
        private void SetStylesAndSettings()
        {
            colNumChannel1.Width = colNumChannel2.Width = 150;
            colIconChannel.Visible = colNextImage.Visible = Settings.Default.FlagLogoTable;
            colNumChannel1.Visible = colNumChannel2.Visible = Settings.Default.FlagNumChan;
            dgNow.Style(DataGridViewExtentions.Styles.TVGridView);
            //dgNow.FitColumns(false, true, true);
            dgNow.RowsDefaultCellStyle.ForeColor = Settings.Default.CurTelecastColor;
            dgNext.Style(DataGridViewExtentions.Styles.TVGridView);
            //dgNext.FitColumns(false, true, true);
            dgNext.RowsDefaultCellStyle.ForeColor = Settings.Default.FutTelecastColor;
            dgChannels.Style(DataGridViewExtentions.Styles.TVGridView);
            //dgChannels.FitColumns(false, true, true);
            dgProgs.Style(DataGridViewExtentions.Styles.TVGridView);
           // dgProgs.FitColumns(false, true, true);
            lblTime.BackColor = lblTime2.BackColor = cbWeekNow.BackColor = cbWeekNext.BackColor = Preferences.CellsColor2;
            tvWeeks.BackColor = tvChannels.BackColor = rtbAnons.BackColor = Preferences.CellsColor2;
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
            tsmMainMenu.Checked = menuStrip.Visible = Settings.Default.FlagMainMenu;
            tsmToolStrip.Checked = toolStrip.Visible = Settings.Default.FlagToolStrip;
            tsmStatusStrip.Checked = statusStrip.Visible = Settings.Default.FlagStatusStrip;
            Preferences.showAnons =
                (Preferences.ShowAnons) Enum.Parse(typeof (Preferences.ShowAnons), Settings.Default.AnonsMode);
            switch (Preferences.showAnons)
            {
                case Preferences.ShowAnons.Never:
                    tsmiAlways.Checked = false;
                    tsmiNever.Checked = true;
                    tsmiIfitis.Checked = false;
                    break;
                case Preferences.ShowAnons.Always:
                    tsmiAlways.Checked = true;
                    tsmiNever.Checked = false;
                    tsmiIfitis.Checked = false;
                    break;
                case Preferences.ShowAnons.Ifitis:
                    tsmiAlways.Checked = false;
                    tsmiNever.Checked = false;
                    tsmiIfitis.Checked = true;
                    break;
            }
            
            rtbAnons.Modified = false;
        }

        

        /// <summary>
        /// Установка настроек видео и аудиоустройства
        /// </summary>
        private void SetVideoAudioSettings()
        {
            // Если есть настройки устройств:
            if (TunerSettings.Default.VideoDevice != String.Empty || TunerSettings.Default.AudioDevice != String.Empty)
            {
                try
                {
                    _capture =
                        new Capture(
                            TunerSettings.Default.VideoDevice != String.Empty
                                 ? new Filter(TunerSettings.Default.VideoDevice)
                                 : null,
                            TunerSettings.Default.AudioDevice != String.Empty
                                 ? new Filter(TunerSettings.Default.AudioDevice)
                                 : null,
                            TunerSettings.Default.AudioViaPCI);
                    if (_capture != null)
                    {
                        if (TunerSettings.Default.VideoCompressor != String.Empty)
                        {
                            _capture.VideoCompressor = new Filter(TunerSettings.Default.VideoCompressor);
                        }
                        if (TunerSettings.Default.AudioCompressor != String.Empty)
                        {
                            _capture.AudioCompressor = new Filter(TunerSettings.Default.AudioCompressor);
                        }
                        _capture.UseVMR9 = TunerSettings.Default.VMR9;
                        _capture.AudioSource = (TunerSettings.Default.AudioSource > 0)
                                                              ? _capture.AudioSources[
                                                                  TunerSettings.Default.AudioSource - 1]
                                                              : _capture.AudioSource;
                        _capture.VideoSource = (TunerSettings.Default.VideoSource > 0)
                                                              ? _capture.VideoSources[
                                                                  TunerSettings.Default.VideoSource - 1]
                                                              : _capture.VideoSource;
                        _capture.FrameSize = (TunerSettings.Default.FrameSize != new Size(0, 0))
                                                            ? TunerSettings.Default.FrameSize
                                                            : _capture.FrameSize;
                        _capture.FrameRate = (!TunerSettings.Default.FrameRate.Equals(0))
                                                            ? TunerSettings.Default.FrameRate
                                                            : _capture.FrameRate;
                        _capture.AudioChannels = (TunerSettings.Default.AudioChannel != 0)
                                                                ? TunerSettings.Default.AudioChannel
                                                                : _capture.AudioChannels;
                        _capture.AudioSamplingRate = (TunerSettings.Default.AudioRate != 0)
                                                                    ? TunerSettings.Default.AudioRate
                                                                    : _capture.AudioSamplingRate;
                        _capture.AudioSampleSize = (TunerSettings.Default.AudioSize != 0)
                                                                  ? TunerSettings.Default.AudioSize
                                                                  : _capture.AudioSampleSize;
                        DxUtils.ColorSpaceEnum c = (TunerSettings.Default.ColorSpace != String.Empty) ?
                            (DxUtils.ColorSpaceEnum)
                            Enum.Parse(typeof(DxUtils.ColorSpaceEnum), TunerSettings.Default.ColorSpace) : _capture.ColorSpace;
                        _capture.ColorSpace = c;
                        _capture.CaptureComplete += new EventHandler(capture_CaptureComplete);
                        if (_capture.dxUtils != null)
                        { // Установка видеостандарта
                            if (TunerSettings.Default.VideoStandard != String.Empty)
                            {
                                AnalogVideoStandard a =
                                    (AnalogVideoStandard)
                                    Enum.Parse(typeof(AnalogVideoStandard), TunerSettings.Default.VideoStandard);
                                _capture.dxUtils.VideoStandard = a;
                            }
                        }
                        if (_capture.Tuner != null)
                        {
                            _capture.Tuner.InputType = TunerSettings.Default.TuningSpace == 0
                                                                      ? TunerInputType.Cable
                                                                      : TunerInputType.Antenna;
                            _capture.Tuner.CountryCode = TunerSettings.Default.CountryCode;
                            _capture.Tuner.TuningSpace = TunerSettings.Default.TuningSpace;

                        }
                        _capture.PreviewFrameSize = (TunerSettings.Default.PreviewSize != new Size(0, 0))
                                                           ? TunerSettings.Default.PreviewSize
                                                           : _capture.PreviewFrameSize;

                        switch (TunerSettings.Default.RecordMode)
                        {
                            case "Wmv":
                                _capture.RecFileMode = DirectX.Capture.Capture.RecFileModeType.Wmv;
                                break;
                            case "Wma":
                                _capture.RecFileMode = DirectX.Capture.Capture.RecFileModeType.Wma;
                                break;
                            case "Avi":
                                _capture.RecFileMode = DirectX.Capture.Capture.RecFileModeType.Avi;
                                break;
                        }
                        // Расширить возможности приложения:
                        AddExtentionsTuner(true);
                    }
                }
                catch (Exception ex)
                { Statics.EL.LogException(ex);}
            }
        }
        /// <summary>
        /// Добавить/исключить расширяющие возможности:
        /// </summary>
        private void AddExtentionsTuner(bool flag)
        {
            if (_capture != null)
            {
                btnPropTvTuner.Enabled = lblQtyRecords.Visible = lblImageRecord.Visible = flag;
                colRec.Visible = colNextRec.Visible =
                    colChanRec.Visible = colChannelRec.Visible = flag;
                if (flag)
                {
                    colRec.Width = colNextRec.Width = colChanRec.Width = colChannelRec.Width = 100;
                    if (csmiViewChannel == null)
                    {
                        csmiViewChannel = new ToolStripMenuItem(Resources.ShowChannelText, null,
                                                                                new EventHandler(viewChannel_Click));
                        ToolStripSeparator tss = new ToolStripSeparator();
                        contextMenuTables.Items.Add(tss);
                        contextMenuTables.Items.Add(csmiViewChannel);
                    }
                    if (tsmiCurrentRecords == null)
                    {
                        contextMenuIcon.Items.Insert(0, new ToolStripSeparator());
                        tsmiCurrentRecords = new ToolStripMenuItem(Resources.RecorderText, null,
                                                                   new EventHandler(tsmiCurrentRecords_Click));
                        contextMenuIcon.Items.Insert(0, tsmiCurrentRecords);
                        tsmiCurrentRecords = new ToolStripMenuItem(Resources.CurrentRecsText, null,
                                                                   new EventHandler(tsmiCurrentRecords_Click));
                        tsmiFilters.DropDownItems.Add(tsmiCurrentRecords);
                    }
                }
               // dgNow.FitColumns(false, true, true);    
            }
        }

        /// <summary>
        /// Обработчик события правильной инициализации объекта для видеозахвата
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capture_CaptureComplete(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new EventHandler(capture_CaptureComplete), sender, e);
            }
            else
            {
                Process.Start(TunerSettings.Default.CaptureDir);
            }
        }

        private void capture_CaptureReady(object sender, CaptureMessage e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new EventHandler<CaptureMessage>(capture_CaptureReady), sender, e.Complete);
            }
            else
            {
                AddExtentionsTuner(e.Complete);
            }
        }
        public void SetRemindStatus()
        {
            if (Preferences.statusReminder)
            {
                lblImageRemind.Enabled = lblImageVolume.Enabled = lblImageMessageDlg.Enabled =
                                          csmiVolume.Enabled = csmiMessage.Enabled = true;
            }
            else
            {
                lblImageRemind.Enabled = lblImageVolume.Enabled =
                    lblImageMessageDlg.Enabled = csmiVolume.Enabled = csmiMessage.Enabled = false;
            }
        }

        public void SetRemindVolume()
        {
            lblImageVolume.Image = Reminder.Default.OnSound ? Resources.volume_high : Resources.volume_muted;
            lblImageVolume.ToolTipText = Reminder.Default.OnSound
                                             ? Resources.SoundOnText
                                             : Resources.SoundOffText;
            csmiVolume.Checked = Reminder.Default.OnSound;
        }

        public void SetRemindMessage()
        {
            lblImageMessageDlg.Image = Reminder.Default.Message
                                     ? Resources.window_dialog
                                     : Resources.window_dialog_none;
            lblImageMessageDlg.ToolTipText = Reminder.Default.Message
                ? Resources.ReminderWindowOn : Resources.ReminderWindowOff;

            csmiMessage.Checked = Reminder.Default.Message;
        }
        /// <summary>
        /// Фильтрация по жанру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterGenre_CheckedChanged(object sender, EventArgs e)
        {
            if (!_bCheckedAll)
            {
                List<string> lstGenre = new List<string>();
                foreach (ToolStripItem tsi in toolStripFiltGenre.Items)
                {
                    if (tsi is ToolStripButton)
                    {
                        if ((tsi as ToolStripButton).Checked)
                        {
                            lstGenre.Add((tsi as ToolStripButton).ToolTipText);
                        }
                    }
                }
                _tvProg.SetGlobGenreFilter(lstGenre);
                UpdateDataGridNow(sender, e, datetime_for_now);
                UpdateDataGridNext(sender, e);
                UpdateDataGridChannels(sender, e);
                UpdateDataGridProgs(sender, e);
            }
        }
        /// <summary>
        /// Фильтрация по рейтингу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterRating_CheckedChanged(object sender, EventArgs e)
        {
            if (!_bCheckedAll)
            {
                List<string> lstCat = new List<string>();
                foreach (ToolStripItem tsi in toolStripFiltRating.Items)
                {
                    if (tsi is ToolStripButton)
                    {
                        if ((tsi as ToolStripButton).Checked)
                        {
                            lstCat.Add((tsi as ToolStripButton).ToolTipText);
                        }
                    }
                }
                _tvProg.SetGlobRatingFilter(lstCat);
                UpdateDataGridNow(sender, e, datetime_for_now);
                UpdateDataGridNext(sender, e);
                UpdateDataGridChannels(sender, e);
                UpdateDataGridProgs(sender, e);
            }
        }

        /// <summary>
        /// Отметить все рейтинги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckAllRating_CheckedChanged(object sender, EventArgs e)
        {

            _bCheckedAll = true;
            List<string> lstCat = new List<string>();
            foreach (ToolStripItem tsi in toolStripFiltRating.Items)
            {
                if (tsi is ToolStripButton)
                {
                    (tsi as ToolStripButton).Checked = btnCheckAllRating.Checked;
                    lstCat.Add((tsi as ToolStripButton).ToolTipText);
                }
            }
            _tvProg.SetGlobRatingFilter(lstCat);
            UpdateDataGridNow(sender, e, datetime_for_now);
            UpdateDataGridNext(sender, e);
            UpdateDataGridChannels(sender, e);
            UpdateDataGridProgs(sender, e);
            _bCheckedAll = false;
        }

        /// <summary>
        /// Отметить все жанры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckAllGenre_CheckedChanged(object sender, EventArgs e)
        {
            _bCheckedAll = true;
            List<string> lstCat = new List<string>();
            foreach (ToolStripItem tsi in toolStripFiltGenre.Items)
            {
                if (tsi is ToolStripButton)
                {
                    (tsi as ToolStripButton).Checked = btnCheckAllGenre.Checked;
                    lstCat.Add((tsi as ToolStripButton).ToolTipText);
                }
            }
            _bCheckedAll = false;
            _tvProg.SetGlobGenreFilter(lstCat);
            UpdateDataGridNow(sender, e, datetime_for_now);
            UpdateDataGridNext(sender, e);
            UpdateDataGridChannels(sender, e);
            UpdateDataGridProgs(sender, e);
        }

        /// <summary>
        /// Инициализация панели фильтров для жанров и рейтигов
        /// </summary>
        private void InitToolStrip()
        {
            toolStripFiltGenre.ShowItemToolTips =
                toolStripFiltRating.ShowItemToolTips = true;
            foreach (DataRow drGenre in _dsTvProg.Tables["Genres"].Rows)
            {
                // Создание кнопок и пунктов меню для жанров:
                if ((bool)drGenre["visible"])
                {
                    ToolStripButton tsbFilt = new ToolStripButton(drGenre["imagename"].ToString(),
                                                                  (Image)drGenre["image"])
                                                  {
                                                      Name = "btn" + drGenre["imagename"],
                                                      ToolTipText = drGenre["genrename"].ToString(),
                                                      DisplayStyle = ToolStripItemDisplayStyle.Image,
                                                      CheckOnClick = true
                                                  };
                    tsbFilt.CheckedChanged += new EventHandler(FilterGenre_CheckedChanged);
                    if (!toolStripFiltGenre.Items.Contains(tsbFilt))
                    {
                        toolStripFiltGenre.Items.Add(tsbFilt);
                    }
                    ToolStripMenuItem tsmiGenre = new ToolStripMenuItem(
                        drGenre["genrename"].ToString(), (Image)drGenre["image"], new EventHandler(SetGenre_Click));
                    if (!csmiChangeType.DropDownItems.Contains(tsmiGenre))
                    {
                        csmiChangeType.DropDownItems.Add(tsmiGenre);
                    }
                }
            }
            foreach (DataRow drRating in _dsTvProg.Tables["Favorites"].Rows)
            {
                // Создание кнопок и пунктов меню для рейтингов:
                if ((bool)drRating["visible"])
                {
                    ToolStripButton tsbFiltRating = new ToolStripButton(drRating["imagename"].ToString(),
                                                                        (Image)drRating["image"])
                                                        {
                                                            Name = String.Format("btn{0}", drRating["imagename"]),
                                                            ToolTipText = drRating["favname"].ToString(),
                                                            DisplayStyle = ToolStripItemDisplayStyle.Image,
                                                            CheckOnClick = true
                                                        };
                    tsbFiltRating.CheckedChanged += new EventHandler(FilterRating_CheckedChanged);
                    if (!toolStripFiltRating.Items.Contains(tsbFiltRating))
                    {
                        toolStripFiltRating.Items.Add(tsbFiltRating);
                    }
                    ToolStripMenuItem tsmiRating = new ToolStripMenuItem(
                        drRating["favname"].ToString(),
                        (Image)drRating["image"],
                        new EventHandler(SetRating_Click));
                    if (!csmiChangeRating.DropDownItems.Contains(tsmiRating))
                    {
                        csmiChangeRating.DropDownItems.Add(tsmiRating);
                    }
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
            switch (tabMain.SelectedIndex)
            {
                case 0:
                    if (dgNow.CurrentRow != null)
                    {
                        idChan = dgNow.CurrentRow.Cells["colCID"].Value.ToString();
                        title = dgNow.CurrentRow.Cells["colTelecastTitle"].Value.ToString();
                        from = (DateTime)dgNow.CurrentRow.Cells["colFrom"].Value;
                        to = (DateTime)dgNow.CurrentRow.Cells["colTo"].Value;
                        foundRow = _dsTvProg.Tables["AllProgrammes"].Rows.Find(
                                    long.Parse(dgNow.CurrentRow.Cells["colID"].Value.ToString()));
                    }
                    break;
                case 1:
                    if (dgNext.CurrentRow != null)
                    {
                        idChan = dgNext.CurrentRow.Cells["colNextChannel"].Value.ToString();
                        title = dgNext.CurrentRow.Cells["colNextTitle"].Value.ToString();
                        from = (DateTime)dgNext.CurrentRow.Cells["colNextFrom"].Value;
                        to = (DateTime)dgNext.CurrentRow.Cells["colNextTo"].Value;
                        foundRow = _dsTvProg.Tables["AllProgrammes"].Rows.Find(
                                    long.Parse(dgNext.CurrentRow.Cells["colNextID"].Value.ToString()));
                    }
                    break;
                case 2:
                    if (dgChannels.CurrentRow != null)
                    {
                        idChan = dgChannels.CurrentRow.Cells["colChanId"].Value.ToString();
                        title = dgChannels.CurrentRow.Cells["colChanProg"].Value.ToString();
                        from = (DateTime)dgChannels.CurrentRow.Cells["colChanFrom"].Value;
                        to = (DateTime)dgChannels.CurrentRow.Cells["colChanTo"].Value;
                        foundRow = _dsTvProg.Tables["AllProgrammes"].Rows.Find(
                                    long.Parse(dgChannels.CurrentRow.Cells["colProgID"].Value.ToString()));
                    }
                    break;
                case 3:
                    if (dgProgs.CurrentRow != null)
                    {
                        idChan = dgProgs.CurrentRow.Cells["colChannelId"].Value.ToString();
                        title = dgProgs.CurrentRow.Cells["colChannelTitle"].Value.ToString();
                        from = (DateTime)dgProgs.CurrentRow.Cells["colChannelFrom"].Value;
                        to = (DateTime)dgProgs.CurrentRow.Cells["colChannelTo"].Value;
                        foundRow = _dsTvProg.Tables["AllProgrammes"].Rows.Find(
                            long.Parse(dgProgs.CurrentRow.Cells["colProgramId"].Value.ToString()));
                    }
                    break;
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
                    }
                    switch (tabMain.SelectedIndex)
                    {
                        case 0: UpdateDataGridNow(sender, e, datetime_for_now); break;
                        case 1: UpdateDataGridNext(sender, e); break;
                        case 2: UpdateDataGridChannels(sender, e); break;
                        case 3: UpdateDataGridProgs(sender, e); break;
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
                    switch (tabMain.SelectedIndex)
                    {
                        case 0: UpdateDataGridNow(sender, e, datetime_for_now); break;
                        case 1: UpdateDataGridNext(sender, e); break;
                        case 2: UpdateDataGridChannels(sender, e); break;
                        case 3: UpdateDataGridProgs(sender, e); break;
                    }
                }
            }
        }

        /// <summary>
        /// Инициализация таблицы для любимых передач
        /// </summary>
        /// <returns></returns>
        private DataTable InitFavorites()
        {
            string binPath = Path.Combine(Application.StartupPath, Preferences.binData);
            string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlFavorites);

            DataTable dtFavorites = new DataTable("Favorites");
            dtFavorites.Columns.Add("id", typeof(int));
            dtFavorites.Columns["id"].AutoIncrement = true;
            dtFavorites.Columns["id"].AutoIncrementSeed = 1;
            dtFavorites.Columns["id"].AutoIncrementStep = 1;
            dtFavorites.Columns.Add("visible", typeof(bool));
            dtFavorites.Columns.Add("image", typeof(Image));
            dtFavorites.Columns.Add("imagename", typeof(string));
            dtFavorites.Columns.Add("favname", typeof(string));

            if (!File.Exists(xmlPath))
            {
                dtFavorites.Rows.Add(null, true, imageListFavs.Images["favempty.png"], "favempty.png", "Без рейтинга");
                dtFavorites.Rows.Add(null, true, imageListFavs.Images["favblue.png"], "favblue.png", "Можно посмотреть");
                dtFavorites.Rows.Add(null, true, imageListFavs.Images["favpurple.png"], "favpurple.png", "Приличные");
                dtFavorites.Rows.Add(null, true, imageListFavs.Images["favgreen.png"], "favgreen.png", "Нормальные");
                dtFavorites.Rows.Add(null, true, imageListFavs.Images["favyellow.png"], "favyellow.png", "Хорошие");
                dtFavorites.Rows.Add(null, true, imageListFavs.Images["favpink.png"], "favpink.png", "Отличные");

                DataTable dtWriteToXml = dtFavorites.Copy();
                dtWriteToXml.Columns.Remove("image");
                dtWriteToXml.WriteXml(xmlPath);
            }
            else
            {
                if (File.Exists(binPath))
                {
                    if (_serialData == null)
                    {
                        Stream dataStream = File.OpenRead(binPath);
                        BinaryFormatter deserializer = new BinaryFormatter();
                        _serialData = (SerializableDataClass)deserializer.Deserialize(dataStream);
                        dataStream.Close();
                    }
                    if (_serialData.FavsImageList != null)
                    {
                        foreach (Tuple<string, Image> tsi in _serialData.FavsImageList)
                        {
                            imageListFavs.Images.Add(tsi.Item1, tsi.Item2);
                        }
                    }
                    else
                    {
                        _serialData = new SerializableDataClass();
                    }
                }
                DataSet dsFavorites = new DataSet();
                dsFavorites.ReadXml(xmlPath);
                if (dsFavorites.Tables.Count > 0)
                {
                    if (dsFavorites.Tables[0] != null)
                    {
                        foreach (DataRow drFavorite in dsFavorites.Tables[0].Rows)
                        {
                            dtFavorites.Rows.Add(null, drFavorite["visible"].ToString().ToLower() == "true" ? true : false,
                                imageListFavs.Images[drFavorite["imagename"].ToString()],
                                              drFavorite["imagename"].ToString(), drFavorite["favname"].ToString());
                        }
                    }
                }
            }
            return dtFavorites;
        }

        /// <summary>
        /// Инициализация жанров
        /// </summary>
        /// <returns></returns>
        private DataTable InitGenres()
        {
            string binPath = Path.Combine(Application.StartupPath, Preferences.binData);
            string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlGenres);

            DataTable dtGenres = new DataTable("Genres");
            dtGenres.Columns.Add("id", typeof(int));
            dtGenres.Columns["id"].AutoIncrement = true;
            dtGenres.Columns["id"].AutoIncrementSeed = 1;
            dtGenres.Columns["id"].AutoIncrementStep = 1;
            dtGenres.Columns.Add("visible", typeof(bool));
            dtGenres.Columns.Add("image", typeof(Image));
            dtGenres.Columns.Add("imagename", typeof(string));
            dtGenres.Columns.Add("genrename", typeof(string));

            if (!File.Exists(xmlPath))
            {
                dtGenres.Rows.Add(null, true, imageListGenres.Images["hudFilm.gif"], "hudFilm.gif", "Художественный фильм");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["teleserial.png"], "teleserial.png", "Сериал");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["balloons.gif"], "balloons.gif", "Детям");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["sport.png"], "sport.png", "Спорт");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["docs.png"], "docs.png", "Документальный фильм");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["info.png"], "info.png", "Информ. программа");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["humor.png"], "humor.png", "Юмор");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["teleshow.png"], "teleshow.png", "Теле-шоу");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["telegames.png"], "telegames.png", "Теле-игра");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["theater.png"], "theater.png", "Театр и кино");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["multfilm.png"], "multfilm.png", "Мультфильм");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["criminal.png"], "criminal.png", "Криминал");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["music.png"], "music.png", "Музыка");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["web.png"], "web.png", "Интернет");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["animals.png"], "animals.png", "О животных");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["weather.png"], "weather.png", "Погода");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["art.png"], "art.png", "Искусство и культура");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["politic.png"], "politic.png", "Политика и социология");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["science.png"], "science.png",
                                  "Научно-популярная передача");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["travel.png"], "travel.png", "Путешествие и краеведение");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["people.png"], "people.png", "Персона");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["tech.png"], "tech.png", "Техника");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["fashion.png"], "fashion.png", "Мода");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["auto.png"], "auto.png", "Авто");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["medicine.png"], "medicine.png", "Здоровье");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["religion.png"], "religion.png", "Религия");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["gardening.png"], "gardening.png", "Садоводство");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["advertize.png"], "advertize.png", "Реклама");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["female.png"], "female.png", "Женские");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["cooking.png"], "cooking.png", "Кулинария");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["extrime.png"], "extrime.png", "Экстрим");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["finance.png"], "finance.png", "Экономика и бизнес");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["erotic.png"], "erotic.png", "Эротика");
                dtGenres.Rows.Add(null, true, imageListGenres.Images["untype.png"], "untype.png", "Без типа");

                DataTable dtWriteToXml = dtGenres.Copy();
                dtWriteToXml.Columns.Remove("image");
                dtWriteToXml.WriteXml(xmlPath);
            }
            else
            {
                if (File.Exists(binPath))
                {
                    if (_serialData == null)
                    {
                        Stream dataStream = File.OpenRead(binPath);
                        BinaryFormatter deserializer = new BinaryFormatter();
                        _serialData = (SerializableDataClass)deserializer.Deserialize(dataStream);
                        dataStream.Close();
                    }
                    if (_serialData.GenreImageList != null)
                    {
                        foreach (Tuple<string, Image> tsi in _serialData.GenreImageList)
                        {
                            imageListGenres.Images.Add(tsi.Item1, tsi.Item2);
                        }
                    }
                    else
                    {
                        _serialData = new SerializableDataClass();
                    }
                }
                DataSet dsGenres = new DataSet();
                dsGenres.ReadXml(xmlPath);
                if (dsGenres.Tables.Count > 0)
                {
                    if (dsGenres.Tables[0] != null)
                    {
                        foreach (DataRow drGenre in dsGenres.Tables[0].Rows)
                        {
                            dtGenres.Rows.Add(null, drGenre["visible"].ToString().ToLower() == "true" ? true : false,
                                imageListGenres.Images[drGenre["imagename"].ToString()],
                                              drGenre["imagename"].ToString(), drGenre["genrename"].ToString());
                        }
                    }
                }
            }
            return dtGenres;
        }

        private void DownloadProg()
        {
            
            string oldInterPath = Path.Combine(Application.StartupPath, Preferences.interOldFileName);
            string newInterPath = Path.Combine(Application.StartupPath, Preferences.interNewFileName);
            string xmlTVFileName1 = Path.Combine(Application.StartupPath, Preferences.xmlFileName1);
            if (!_tvProg.GetWebTVProgramm(Settings.Default.UrlNewProgXMLTV, xmlTVFileName1))
            {
                if (_tvProg.GetWebTVProgramm(Settings.Default.UrlNewProgInterTV, newInterPath))
                {
                    if (File.Exists(newInterPath))
                    {
                        _tvProg.InterToXml(newInterPath, ref xmlTVFileName1);
                    }
                }
            }
            if (File.Exists(xmlTVFileName1))
            {
                _tvProg.MergeXmls(xmlTVFileName1);
            }
            if (!_tvProg.GetWebTVProgramm(Settings.Default.UrlOldProgXMLTV, xmlTVFileName1))
            {
                if (_tvProg.GetWebTVProgramm(Settings.Default.UrlOldProgInterTV, oldInterPath))
                {
                    if (File.Exists(oldInterPath))
                    {
                        _tvProg.InterToXml(oldInterPath, ref xmlTVFileName1);
                    }
                }
            }
            if (File.Exists(xmlTVFileName1))
            {
                _tvProg.MergeXmls(xmlTVFileName1);
            }
        }
        /// <summary>
        /// Получение программы из глобальной сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var downloadThread = new Thread(DownloadProg)
                    {
                        Name = "DownloadProgThread",
                        Priority = ThreadPriority.Normal
                    };
                downloadThread.Start();

                while (downloadThread.IsAlive) { Application.DoEvents(); }

                _tvProg = new TVProgClass();

                LoadDataSetAndProg();

                Statics.ShowDialog("Информация", Resources.LoadSuccess,
                    MessageDialog.MessageIcon.Info,
                                       MessageDialog.MessageButtons.Ok);
            }
            catch (Exception ex)
            {
                Statics.ShowDialog("Информация", Resources.LoadFail,
                MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
            }


            // Создать поток для установки напоминаний и любимых:
            var threadSetter = new Thread(CheckAndSetRemindAndFavorite)
            {
                Name = "ChekAndSetRemindAndFavorite2",
                Priority = ThreadPriority.Lowest
            };
            threadSetter.Start();   // - запустить поток
        }

        private void FillImageList()
        {
            ImgListDays.ColorDepth = ColorDepth.Depth32Bit;
            ImgListDays.ImageSize = new Size(16, 16);
            ImgListDays.Images.Clear();
            ImgListDays.Images.Add("Mon", Resources.Mon);
            ImgListDays.Images.Add("Tue", Resources.Tue);
            ImgListDays.Images.Add("Wen", Resources.Wen);
            ImgListDays.Images.Add("Ths", Resources.Ths);
            ImgListDays.Images.Add("Fri", Resources.Fri);
            ImgListDays.Images.Add("Sat", Resources.Sat);
            ImgListDays.Images.Add("Sun", Resources.Sun);
            imgLst.Images.Clear();
            imgLst.Images.Add("sd2", Resources.satellite_dish2);
            imgLst.Images.Add("ans", Resources.GreenAnons);
            imgLst.Images.Add("gen", Resources.GenreEditor);
            imgLst.Images.Add("fav", Resources.favorites25);
            imgLst.Images.Add("Mon", Resources.Mon);
            imgLst.Images.Add("Mon_exp", Resources.Mon_exp);
            imgLst.Images.Add("Tue", Resources.Tue);
            imgLst.Images.Add("Tue_exp", Resources.Tue_exp);
            imgLst.Images.Add("Wen", Resources.Wen);
            imgLst.Images.Add("Wen_exp", Resources.Wen_exp);
            imgLst.Images.Add("Ths", Resources.Ths);
            imgLst.Images.Add("Ths_exp", Resources.Ths_exp);
            imgLst.Images.Add("Fri", Resources.Fri);
            imgLst.Images.Add("Fri_exp", Resources.Fri_exp);
            imgLst.Images.Add("Sat", Resources.Sat);
            imgLst.Images.Add("Sat_exp", Resources.Sat_exp);
            imgLst.Images.Add("Sun", Resources.Sun);
            imgLst.Images.Add("Sun_exp", Resources.Sun_exp);
            imgLst.Images.Add("All", Resources.All);

            imgLstBig.Images.Clear();
            if (TVEnvironment.systemChannelList == null)
                return;
            if (TVEnvironment.currentUser == null)
            {
                foreach (IChannel ch in TVEnvironment.systemChannelList)
                {
                    if (ch.Emblem25 != null)
                        using (MemoryStream ms25 = new MemoryStream(ch.Emblem25))
                        {
                            imgLst.Images.Add(ch.ChannelID.ToString() + "_25", Image.FromStream(ms25));
                        }
                    if (ch.EmblemOrig != null)
                        using (MemoryStream msOrig = new MemoryStream(ch.EmblemOrig))
                        {
                            imgLstBig.Images.Add(ch.ChannelID + "_full", Image.FromStream(msOrig));
                        }
                }
            }
            else
            {
               

                foreach (IChannel ch in _chList)
                {
                    if (!ch.Visible) continue;
                    if (ch.Emblem25 != null)
                        using (MemoryStream ms25 = new MemoryStream(ch.Emblem25))
                        {
                            imgLst.Images.Add(ch.ChannelID.ToString() + "_25", Image.FromStream(ms25));
                        }
                    if (ch.EmblemOrig != null)
                        using (MemoryStream msOrig = new MemoryStream(ch.EmblemOrig))
                        {
                            imgLstBig.Images.Add(ch.ChannelID + "_full", Image.FromStream(msOrig));
                        }
                }
            }
            
            
        }

        private void CreateChanOpt()
        {
            Statics.ShowDialog(Resources.InformationText, Resources.PleaseSetChannelsText,
                MessageDialog.MessageIcon.Info, MessageDialog.MessageButtons.Ok);
            ChannelsForm chForm = new ChannelsForm();
            chForm.ShowDialog(this);
        }

        

        private void ModifyDiff()
        {
           /* foreach (DataRow drChan in _dsTvProg.Tables["AllChannels"].Rows)
            {
                int hours = 0, mins = 0;
                char ch = ' ';
                if (drChan["diff"].ToString() != String.Empty)
                {
                    bool pr1 = Char.TryParse(drChan["diff"].ToString().Substring(0, 1), out ch);
                    bool pr2 = Int32.TryParse(drChan["diff"].ToString().Substring(1, 2), out hours);
                    bool pr3 = Int32.TryParse(drChan["diff"].ToString().Substring(4, 2), out mins);
                    if ((ch == '+' || ch == '-') && pr1 && pr2 && pr3)
                    {
                        DataRow[] drsProgs = drChan.GetChildRows("FK_ChannelID_ProgChanID");
                        foreach (DataRow drProg in drsProgs)
                        {
                            if (ch == '+')
                            {
                                drProg["start"] =
                                    Convert.ToDateTime(drProg["start_mo"]).AddHours(hours - 4).AddMinutes(mins);
                                drProg["stop"] =
                                    Convert.ToDateTime(drProg["stop_mo"]).AddHours(hours - 4).AddMinutes(mins);
                            }
                            else if (ch == '-')
                            {
                                drProg["start"] =
                                    Convert.ToDateTime(drProg["start_mo"]).AddHours(-hours - 4).AddMinutes(mins);
                                drProg["stop"] =
                                    Convert.ToDateTime(drProg["stop_mo"]).AddHours(-hours - 4).AddMinutes(mins);
                            }
                        }
                    }
                }
            }*/
        }
        /// <summary>
        /// Загрузка данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDataSetAndProg()
        {
            Statics.ShowLogo(Resources.StatusLoadingAppText, 0);
            
            _dsTvProg.Clear();
            _dsTvProg.Relations.Clear();
            if (_dsTvProg.Tables.Count > 0)
            {
                _dsTvProg.Tables["AllProgrammes"].Constraints.Clear();
                _dsTvProg.Tables["AllChannels"].Constraints.Clear();
            }
            _dsTvProg.Tables.Clear();
            _dsTvProg.Tables.Add(InitGenres());     // - инициализация жанров
            _dsTvProg.Tables.Add(InitFavorites());  // - инициализация любимых

            Preferences.genres = new Genres(_dsTvProg.Tables["Genres"]);
            DataTable dtClassifGenres = null, dtClassifFavs = null;
            dtClassifGenres = Preferences.genres.ClassifTable.Rows.Count > 0 ?
              Preferences.genres.ClassifTable.Select("", "prior DESC").CopyToDataTable()
             : new DataTable("KeywordsTable");
            dtClassifGenres.TableName = "KeywordsTable";
            _dsTvProg.Tables.Add(dtClassifGenres);

            Preferences.favorites = new Favorites(_dsTvProg.Tables["Favorites"]);
            dtClassifFavs = Preferences.favorites.ClassifTable.Rows.Count > 0
               ? Preferences.favorites.ClassifTable.Select("", "prior DESC").CopyToDataTable()
               : new DataTable("FavwordsTable");
            dtClassifFavs.TableName = "FavwordsTable";
            _dsTvProg.Tables.Add(dtClassifFavs);
            InitToolStrip();
            Statics.ShowLogo(Resources.StatusLoadingAppText, 100);
            Statics.ShowLogo(Resources.StatusPrepText, 0);
            // Получение каналов:
            
            
            DateTime oldMinDateTime = DateTime.MinValue, 
                     oldMaxDateTime = DateTime.MaxValue,
                     newMinDateTime = DateTime.MinValue,
                     newMaxDateTime = DateTime.MaxValue;
            Statics.ShowLogo(Resources.StatusPrepText, 20);
            // Получение тв-программы:
            _dtProgramme = _tvProg.GetTVProgramm(ref oldMinDateTime, ref oldMaxDateTime);
            Statics.ShowLogo(Resources.StatusPrepText, 40);
            
            // Установка ключей:
            DataColumn pkProgCol = new DataColumn("id", typeof(long));
            pkProgCol.AutoIncrement = true;
            pkProgCol.AutoIncrementSeed = 1;
            pkProgCol.AutoIncrementStep = 1;
            DataTable dtAllProgrammes = new DataTable("AllProgrammes");
            dtAllProgrammes.Columns.Add(pkProgCol);
            DataColumn[] keyProgColumns = new DataColumn[1];
            keyProgColumns[0] = dtAllProgrammes.Columns["id"];
            dtAllProgrammes.PrimaryKey = keyProgColumns;
            DataTableReader dtrAllProg = new DataTableReader(_dtProgramme);
            Statics.ShowLogo(Resources.StatusPrepText, 76);
            dtAllProgrammes.Load(dtrAllProg);

            DataColumn[] keyColumns = new DataColumn[1];
            Statics.ShowLogo(Resources.StatusPrepText, 100);
            Statics.ShowLogo(Resources.StatusFormingDataSetText, 0);
            _dsTvProg.Tables.Add(dtAllProgrammes);
                     
            Statics.ShowLogo(Resources.StatusFormingDataSetText, 50);
            ModifyDiff();
            
            dgNow.AutoGenerateColumns = dgNext.AutoGenerateColumns =
                dgChannels.AutoGenerateColumns = dgProgs.AutoGenerateColumns = false;
            
                       
            // Подсказки для жанров и рейтинга:
            foreach (DataGridViewRow dgvr in dgNow.Rows)
            {
                if (dgvr.Cells["colGenreName"].Value != null)
                {
                    dgvr.Cells["colGenre"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colGenreName"].Value.ToString())
                            ? dgvr.Cells["colGenreName"].Value.ToString()
                            : Resources.WithoutTypeText;
                }
                if (dgvr.Cells["colFavName"].Value != null)
                {
                    dgvr.Cells["colRating"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colFavName"].Value.ToString())
                            ? dgvr.Cells["colFavName"].Value.ToString()
                            : Resources.WithoutRatingText;
                }
            }
            Statics.ShowLogo(Resources.StatusFormingDataSetText, 76);
            foreach (DataGridViewRow dgvr in dgNext.Rows)
            {
                if (dgvr.Cells["colNextCategory"].Value != null)
                {
                    dgvr.Cells["colNextGenre"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colGenreNext"].Value.ToString())
                            ? dgvr.Cells["colGenreNext"].Value.ToString()
                            : Resources.WithoutTypeText;
                }
                if (dgvr.Cells["colNextFavName"].Value != null)
                {
                    dgvr.Cells["colNextRating"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colNextFavName"].Value.ToString())
                            ? dgvr.Cells["colNextFavName"].ToString()
                            : Resources.WithoutRatingText;
                }
            } 
            slblCountTelecast.Text = String.Format("{0} {1}", Properties.Resources.Telecasts, dgNow.Rows.Count);
            Statics.ShowLogo(Resources.StatusCreatingTreesText, 0);
            tvWeeks.Nodes.Clear();
            //CreateTree(_dsTvProg.Tables["AllChannels"],
            //    _dsTvProg.Tables["AllProgrammes"], GroupingBy.Dates);
            Statics.ShowLogo(Resources.StatusCreatingTreesText, 50);
            tvChannels.Nodes.Clear();
            //CreateTree(_dsTvProg.Tables["AllChannels"],
            //    _dsTvProg.Tables["AllProgrammes"], GroupingBy.Channels);
            Statics.ShowLogo(Resources.StatusLastsText, 20);
           
            timerReminder.Enabled = true;
            StatRemindsAndRecords();
            lblTime.Text = lblTime2.Text = DateTime.Now.ToShortTimeString();
            
            Statics.ShowLogo(Resources.StatusLastsText, 100);
            Statics.HideLogo();
        }

        private void BindTaskPriorityCombo(ComboBox priorityCombo)
        {
            priorityCombo.DrawMode = DrawMode.OwnerDrawVariable;
            priorityCombo.DrawItem += new DrawItemEventHandler(priorityCombo_DrawItem);
            priorityCombo.Items.Clear();
            for (DateTimeOffset tsCurDate = _periodMinMax.dtStart; tsCurDate <= _periodMinMax.dtEnd; tsCurDate = tsCurDate.AddDays(1))
            {
                priorityCombo.Items.Add(tsCurDate.DateTime.ToShortDateString());
            }
            priorityCombo.SelectedIndex = 0;
        }

        private void priorityCombo_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                ComboBox cmbPriority = sender as ComboBox;
                string text = cmbPriority.Items[e.Index].ToString();

                if (text.Length > 0)
                {
                    string priority = text;
                    Image img = GetTaskPriorityImage(priority);

                    if (img != null)
                    {
                        e.Graphics.DrawImage(img, e.Bounds.X, e.Bounds.Y, 16, 16);
                    }
                }

                e.Graphics.DrawString(text, cmbPriority.Font, System.Drawing.Brushes.Black,
                                      new RectangleF(e.Bounds.X + 15, e.Bounds.Y,
                                                     e.Bounds.Width, e.Bounds.Height));

                e.DrawFocusRectangle();
            }
        }

        private Image GetTaskPriorityImage(string priority)
        {
            for (DateTimeOffset tsCurDate = _periodMinMax.dtStart; tsCurDate <= _periodMinMax.dtEnd; tsCurDate = tsCurDate.AddDays(1))
            {
                string strKey = Preferences.dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                if (priority == tsCurDate.DateTime.ToShortDateString())
                {
                    return ImgListDays.Images[strKey];
                }
            }
            return null;
        }
        /// <summary>
        /// Создание дерева, с заданной сгруппировкой
        /// <param name="groupby">Условие группировки</param>
        /// </summary>
        private void CreateTree(List<IChannel> chanList, GroupingBy groupby)
        {
            DateTime tsMin = _periodMinMax.dtStart.DateTime;
            tsMin =
                new DateTime(tsMin.Year, tsMin.Month, tsMin.Day, 0, 0, 0).AddTicks(Settings.Default.BeginEndTime.Ticks);
            DateTime tsMiddle = tsMin.AddDays(6);
            DateTime tsMax = _periodMinMax.dtEnd.DateTime;
            tsMax =
                new DateTime(tsMax.AddDays(-1).Year, tsMax.AddDays(-1).Month, tsMax.AddDays(-1).Day, 0, 0, 0).AddTicks(
                    Settings.Default.BeginEndTime.Ticks); 
            chanList.OrderBy(ch => ch.OrderCol);
            switch (groupby)
            {
                case GroupingBy.Dates:
                    tvWeeks.ImageList = Settings.Default.FlagPictTree ? imgLst : null;
                    
                    tvWeeks.Nodes.Add("Root", "");
                    tvWeeks.Nodes[0].Nodes.Add(tsMin.ToShortDateString(), "(" + tsMin.ToShortDateString() + "—" +
                    tsMiddle.ToShortDateString() + ")", "All", "All");
                    for (DateTime tsCurDate = tsMin; tsCurDate <= tsMiddle; tsCurDate = tsCurDate.AddDays(1))
                    {
                        
                        string strKey = Preferences.dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                        if (Settings.Default.FlagPictTree)
                        {
                            tvWeeks.Nodes[0].Nodes[0].Nodes.Add(tsCurDate.ToShortDateString()
                                ,"(" + tsCurDate.ToShortDateString() + ")"
                                ,strKey
                                ,strKey + "_exp");
                        }
                        else
                        {
                            tvWeeks.Nodes[0].Nodes[0].Nodes
                                .Add(tsCurDate.ToShortDateString(), String.Format("({0})", 
                                tsCurDate.ToShortDateString()));
                        }

                        foreach (IChannel drChan in chanList)
                        {
                            if (drChan.Visible)
                            {
                                string keyId = drChan.ChannelID.ToString() + "_25";
                                tvWeeks.Nodes[0].Nodes[0].Nodes[tsCurDate.ToShortDateString()].
                                    Nodes.Add(drChan.ChannelID.ToString(), drChan.UserTitle, keyId, keyId);
                            }
                        }
                    }
                    tvWeeks.Nodes[0].Nodes.Add(tsMiddle.AddDays(1).ToShortDateString(),
                    "(" + tsMiddle.AddDays(1).ToShortDateString() + "—" +
                    tsMax.ToShortDateString() + ")", "All", "All");
                    for (DateTime tsCurDate = tsMiddle.AddDays(1); tsCurDate <= tsMax; tsCurDate = tsCurDate.AddDays(1))
                    {
                        string strKey = Preferences.dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                        if (Settings.Default.FlagPictTree)
                        {
                            tvWeeks.Nodes[0].Nodes[1].Nodes.Add(tsCurDate.ToShortDateString(), "(" +
                                                                                     tsCurDate.
                                                                                         ToShortDateString
                                                                                         () + ")",
                                                      strKey,
                                                      strKey + "_exp");
                        }
                        else
                        {
                            tvWeeks.Nodes[0].Nodes[1].Nodes.Add(tsCurDate.ToShortDateString(), "(" +
                                                                                               tsCurDate.
                                                                                                   ToShortDateString
                                                                                                   () + ")");

                        }
                        foreach (IChannel drChan in chanList)
                        {
                            if (drChan.Visible)
                            {
                                string keyId = drChan.ChannelID.ToString() + "_25";
                                tvWeeks.Nodes[0].Nodes[1].Nodes[tsCurDate.ToShortDateString()].
                                    Nodes.Add(drChan.ChannelID.ToString(), drChan.UserTitle, keyId, keyId);
                            }
                        }
                    }
                    tvWeeks.Nodes[0].Expand();
                    break;
                case GroupingBy.Channels:
                    tvChannels.ImageList = Settings.Default.FlagPictTree ? imgLst : null;
                    
                    tvChannels.Nodes.Add("Root", "");
                    if (Settings.Default.FlagPictTree)
                    {// если нужно показывать значки
                        tvChannels.Nodes[0].Nodes.Add(tsMin.ToShortDateString(), "(" + tsMin.ToShortDateString() + "—" +
                         tsMiddle.ToShortDateString() + ")", "All", "All");
                    }
                    else
                    { // без значков
                        tvChannels.Nodes[0].Nodes.Add(tsMin.ToShortDateString(), "(" + tsMin.ToShortDateString() + "—" +
                                                                                 tsMiddle.ToShortDateString() + ")");
                    }
                    foreach (IChannel drChan in chanList)
                    {
                        if (drChan.Visible)
                        {
                            string keyId = drChan.ChannelID.ToString() + "_25";
                            tvChannels.Nodes[0].Nodes[0].Nodes.Add(drChan.ChannelID.ToString(),
                                                                   drChan.UserTitle, keyId, keyId);
                            for (DateTime tsCurDate = tsMin; tsCurDate <= tsMiddle; tsCurDate = tsCurDate.AddDays(1))
                            {
                                string strKey = Preferences.dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                                if (Settings.Default.FlagPictTree)
                                {
                                    tvChannels.Nodes[0].Nodes[0].Nodes[drChan.ChannelID.ToString()].Nodes
                                        .Add(tsCurDate.ToShortDateString(), "(" + tsCurDate
                                        .ToShortDateString() +")", strKey,strKey + "_exp");
                                }
                                else
                                {
                                    tvChannels.Nodes[0].Nodes[0].Nodes[drChan.ChannelID.ToString()].Nodes.Add(
                                        tsCurDate.ToShortDateString(),
                                          "(" +tsCurDate.ToShortDateString() +")");
                                }
                            }
                        }
                    }
                    tvChannels.Nodes[0].Nodes.Add(tsMiddle.AddDays(1).ToShortDateString(),
                        "(" + tsMiddle.AddDays(1).ToShortDateString() + "—" +
                         tsMax.ToShortDateString() + ")", "All", "All");
                    foreach (IChannel drChan in chanList)
                    {
                        if (drChan.Visible)
                        {
                            string keyId = drChan.ChannelID.ToString() + "_25";
                            tvChannels.Nodes[0].Nodes[1].Nodes.Add(drChan.ChannelID.ToString(),
                                drChan.UserTitle, keyId, keyId);
                            for (DateTime tsCurDate = tsMiddle.AddDays(1); tsCurDate <= tsMax; tsCurDate = tsCurDate.AddDays(1))
                            {
                                string strKey = Preferences.dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                                tvChannels.Nodes[0].Nodes[1].Nodes[drChan.ChannelID.ToString()].Nodes.Add(tsCurDate.ToShortDateString(), "(" +
                                                                                                          tsCurDate.
                                                                                                              ToShortDateString
                                                                                                              () + ")",
                                                                           strKey,
                                                                           strKey + "_exp");
                            }
                        }
                        tvChannels.Nodes[0].Expand();
                    }
                    break;
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

                    int offset = (e.CellBounds.Width - this.imgLst.Images["sd2"].Size.Width)/2;
                    pt.X += offset;
                    pt.Y += (e.CellBounds.Height - this.imgLst.Images["sd2"].Size.Height)/2;
                    this.imgLst.Draw(e.Graphics, pt, imgLst.Images.IndexOfKey("sd2"));
                    e.Handled = true;
                }
                else
                {
                    if (e.Value.ToString() == Resources.NoticeText && e.RowIndex == -1)
                    {
                        e.PaintBackground(e.ClipBounds, false);

                        Point pt = e.CellBounds.Location;
                        int offset = (e.CellBounds.Width - this.imgLst.Images["ans"].Size.Width)/2;
                        pt.X += offset;
                        pt.Y += (e.CellBounds.Height - this.imgLst.Images["ans"].Size.Height)/2;
                        this.imgLst.Draw(e.Graphics, pt, imgLst.Images.IndexOfKey("ans"));
                        e.Handled = true;
                    }
                    else
                    {
                        if (e.Value.ToString() == Resources.GenreText && e.RowIndex == -1)
                        {
                            e.PaintBackground(e.ClipBounds, false);

                            Point pt = e.CellBounds.Location;
                            int offset = (e.CellBounds.Width - this.imgLst.Images["gen"].Size.Width) / 2;
                            pt.X += offset;
                            pt.Y += (e.CellBounds.Height - this.imgLst.Images["gen"].Size.Height) / 2;
                            this.imgLst.Draw(e.Graphics, pt, imgLst.Images.IndexOfKey("gen"));
                            e.Handled = true;
                        }
                        else
                        {
                            if (e.Value.ToString() == Resources.RatingText && e.RowIndex == -1)
                            {
                                e.PaintBackground(e.ClipBounds, false);

                                Point pt = e.CellBounds.Location;
                                int offset = (e.CellBounds.Width - this.imgLst.Images["fav"].Size.Width)/2;
                                pt.X += offset;
                                pt.Y += (e.CellBounds.Height - this.imgLst.Images["fav"].Size.Height)/2;
                                this.imgLst.Draw(e.Graphics, pt, this.imgLst.Images.IndexOfKey("fav"));
                                e.Handled = true;
                            }
                            else
                            {
                                if (e.Value.ToString() == Resources.ToRemindText && e.RowIndex == -1)
                                {
                                    e.PaintBackground(e.ClipBounds, false);

                                    Point pt = e.CellBounds.Location;
                                    int offset = (e.CellBounds.Width -
                                                  this.imageListBell.Images["bellheader.png"].Size.Width)/2;
                                    pt.X += offset;
                                    pt.Y += (e.CellBounds.Height -
                                             this.imageListBell.Images["bellheader.png"].Size.Height)/2;
                                    this.imageListBell.Draw(e.Graphics, pt,
                                                            this.imageListBell.Images.IndexOfKey("bellheader.png"));
                                    e.Handled = true;
                                }
                                else
                                {
                                    if (e.Value.ToString() == Resources.RecText && e.RowIndex == -1)
                                    {
                                        e.PaintBackground(e.ClipBounds, false);

                                        Point pt = e.CellBounds.Location;
                                        int offset = (e.CellBounds.Width -
                                                      this.imageListBell.Images["capture_header.png"].Size.Width)/2;
                                        pt.X += offset;
                                        pt.Y += (e.CellBounds.Height -
                                                 this.imageListBell.Images["capture_header.png"].Size.Height)/2;
                                        this.imageListBell.Draw(e.Graphics, pt,
                                                                this.imageListBell.Images.IndexOfKey("capture_header.png"));
                                        e.Handled = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void dgNext_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            SetSatteliteOnHeaderColumn(sender, e);
        }

        private void dgNow_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            SetSatteliteOnHeaderColumn(sender, e);
        }

        private void dgChannels_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            SetSatteliteOnHeaderColumn(sender, e);
        }

        private void dgProgs_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            SetSatteliteOnHeaderColumn(sender, e);
        }

        private void btnChannels_Click(object sender, EventArgs e)
        {
            ChannelsForm chForm = new ChannelsForm();
            chForm.ShowDialog(this);
        }

        private void dgChannels_Paint(object sender, PaintEventArgs e)
        {
            lock (this)
            {
                foreach (DataGridViewRow dgvr in dgChannels.Rows)
                {
                    if ((DateTimeOffset)dgvr.Cells["colChanTo"].Value < DateTimeOffset.Now)
                    {
                        dgvr.DefaultCellStyle.ForeColor = Settings.Default.LastTelecastColor;
                    }
                    if ((DateTimeOffset)dgvr.Cells["colChanFrom"].Value <= DateTimeOffset.Now &&
                        (DateTimeOffset)dgvr.Cells["colChanTo"].Value > DateTimeOffset.Now)
                    {
                        dgvr.DefaultCellStyle.ForeColor = Settings.Default.CurTelecastColor;
                    }
                    if ((DateTimeOffset)dgvr.Cells["colChanFrom"].Value > DateTimeOffset.Now)
                    {
                        dgvr.DefaultCellStyle.ForeColor = Settings.Default.FutTelecastColor;
                    }
                }
            } 
        }

        private void dgProgs_Paint(object sender, PaintEventArgs e)
        {
            foreach (DataGridViewRow dgvr in dgProgs.Rows)
            {
                if ((DateTimeOffset)dgvr.Cells["colChannelTo"].Value <= DateTimeOffset.Now)
                {
                    dgvr.DefaultCellStyle.ForeColor = Settings.Default.LastTelecastColor;
                }
                if ((DateTimeOffset)dgvr.Cells["colChannelFrom"].Value <= DateTimeOffset.Now &&
                        (DateTimeOffset)dgvr.Cells["colChannelTo"].Value > DateTimeOffset.Now)
                {
                    dgvr.DefaultCellStyle.ForeColor = Settings.Default.CurTelecastColor;
                }
                if ((DateTimeOffset)dgvr.Cells["colChannelFrom"].Value > DateTimeOffset.Now)
                {
                    dgvr.DefaultCellStyle.ForeColor = Settings.Default.FutTelecastColor;
                }
            }
             
        }
        
        /// <summary>
        /// При выборе узла по датам
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void tvWeeks_AfterSelect(object sender, TreeViewEventArgs e)
        {
            User user = TVEnvironment.currentUser;
            colDay.Visible = colChanChannel.Visible = colChanImage.Visible = false;
            int typeProgID = GetTypeProgID();
            if (typeProgID == -1)
                return;
            switch (e.Node.Level)
            {
                case 1:
                    Preferences.AllWeekShown allWeekShown =
                        (Preferences.AllWeekShown)
                        Enum.Parse(typeof (Preferences.AllWeekShown), Settings.Default.AllWeekMode);
                    switch (allWeekShown)
                    {
                        case Preferences.AllWeekShown.Never:
                            break;
                        case Preferences.AllWeekShown.Always:
                            AllWeekShowChannels(e);
                            break;
                        case Preferences.AllWeekShown.Query:

                            if (Statics.ShowDialog(Resources.ConfirmChoiceText, Resources.ShowAllProgramText,
                                                   MessageDialog.MessageIcon.Question,
                                                   MessageDialog.MessageButtons.YesNo)
                                == DialogResult.Yes)
                            {
                                AllWeekShowChannels(e);
                            }
                            break;
                    }
                    break;
                case 2:
                    if (Settings.Default.FlagNodeSelect)
                    {
                        DateTime tsDay = Convert.ToDateTime(e.Node.Name);
                        //dtProg = _tvProg.FiltTVProgram(_dsTvProg, 1, tsDay, String.Empty);
                        lblDate.Text = tsDay.ToLongDateString() + " (" + tsDay.ToString("ddd", new CultureInfo("ru-Ru")) +
                                       ")";
                        lblName.Text = String.Empty;
                        panelChannel.Visible = true;
                        //dgChannels.DataSource = dtProg;
                    }
                    break;
                case 3:
                    if (e.Node.Parent != null)
                    {
                        DateTime tsDate = Convert.ToDateTime(e.Node.Parent.Name);
                        if (user == null)
                        {
                            dgChannels.DataSource = await TvProgController.GetSystemProgrammesOfDayAsyncList(typeProgID, int.Parse(e.Node.Name),
                                tsDate.AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                       Settings.Default.BeginEndTime.Minutes),
                                tsDate.AddDays(1).AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                       Settings.Default.BeginEndTime.Minutes));
                        }
                        else
                        {
                            dgChannels.DataSource = await TvProgController.GetUserProgrammesOfDayAsyncList(user.UserID,typeProgID, int.Parse(e.Node.Name),
                                tsDate.AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                       Settings.Default.BeginEndTime.Minutes),
                                tsDate.AddDays(1).AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                       Settings.Default.BeginEndTime.Minutes));
                        }

                        //dtProg = _tvProg.FiltTVProgram(_dsTvProg, 1, tsDate, e.Node.Name);
                        Image chImage = imgLstBig.Images[string.Concat(e.Node.Name, "_full")];
                        if (chImage != null)
                        {
                            pbChannel.Width = chImage.Width;
                            if (chImage.Height > panelChannel.Height)
                            {
                                pbChannel.Height = panelChannel.Height;
                                pbChannel.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                            else
                            {
                                pbChannel.Height = chImage.Height;
                                pbChannel.SizeMode = PictureBoxSizeMode.CenterImage;
                            }
                            pbChannel.Image = chImage;
                        }
                        else
                        {
                            pbChannel.Image = null;
                        }
                        lblName.Text = e.Node.Text;
                        lblDate.Text = String.Format("{0} ({1})", tsDate.ToLongDateString(), tsDate.ToString("ddd", new CultureInfo("ru-Ru")));
                        panelChannel.Visible = true;
                    }
                    break;
            }
            /*foreach (DataGridViewRow dgvr in dgChannels.Rows)
            {
                dgvr.Cells["colChanGenre"].ToolTipText =
                    !String.IsNullOrEmpty(dgvr.Cells["colProgCategory"].Value.ToString()) ?
                    dgvr.Cells["colProgCategory"].Value.ToString() : Resources.WithoutTypeText;
                dgvr.Cells["сolChanRating"].ToolTipText =
                    !String.IsNullOrEmpty(dgvr.Cells["colChanFavName"].Value.ToString()) ?
                    dgvr.Cells["colChanFavName"].Value.ToString() : Resources.WithoutRatingText;
            } */
            slblCountTelecast.Text = Properties.Resources.Telecasts + dgChannels.Rows.Count;
        }

        private void AllWeekShowChannels(TreeViewEventArgs e)
        {
            DataTable dtProg;
            DateTime tsStart = Convert.ToDateTime(e.Node.Name);
            dtProg = _tvProg.FiltTVProgram(_dsTvProg, 7, tsStart, String.Empty);
            panelChannel.Visible = false;
            colChanChannel.Visible = colDay.Visible =
                                     colChanImage.Visible = true;
            dgChannels.DataSource = dtProg;
            lblName.Text = lblDate.Text = String.Empty;
        }

        /// <summary>
        /// При выборе узла по каналам
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void tvChannels_AfterSelect(object sender, TreeViewEventArgs e)
        {
            User user = TVEnvironment.currentUser;
            colChannelDay.Visible = colChannelName.Visible =
            colChannelImage.Visible = false;
            int typeProgID = GetTypeProgID();
            if (typeProgID == -1)
                return;
            switch (e.Node.Level)
            {
                case 1:
                      Preferences.AllWeekShown allWeekShown =
                        (Preferences.AllWeekShown)
                        Enum.Parse(typeof (Preferences.AllWeekShown), Settings.Default.AllWeekMode);
                      switch (allWeekShown)
                      {
                          case Preferences.AllWeekShown.Never:
                              break;
                          case Preferences.AllWeekShown.Always:
                              AllWeekShowProg(e);
                              break;
                          case Preferences.AllWeekShown.Query:
                              if (Statics.ShowDialog(Resources.ConfirmChoiceText, Resources.ShowAllProgramText,
                                                     MessageDialog.MessageIcon.Question,
                                                     MessageDialog.MessageButtons.YesNo)
                                  == DialogResult.Yes)
                              {
                                  AllWeekShowProg(e);
                              }
                              break;
                      }
                    break;
                case 2:
                    if (Settings.Default.FlagNodeSelect)
                    {
                        if (e.Node.Parent != null)
                        {
                            DateTime tsDate = Convert.ToDateTime(e.Node.Parent.Name);
                            //dtProg = _tvProg.FiltTVProgram(_dsTvProg, 7, tsDate, e.Node.Name);
                            Image chImage = _tvProg.GetChannelImage(e.Node.Name, e.Node.Text);
                            if (chImage != null)
                            {
                                pbChannel2.Width = chImage.Width;
                                if (chImage.Height > panelChannel2.Height)
                                {
                                    pbChannel2.Height = panelChannel2.Height;
                                    pbChannel2.SizeMode = PictureBoxSizeMode.StretchImage;
                                }
                                else
                                {
                                    pbChannel2.Height = chImage.Height;
                                    pbChannel2.SizeMode = PictureBoxSizeMode.CenterImage;
                                }
                                pbChannel2.Image = chImage;
                            }
                            else
                            {
                                pbChannel2.Image = null;
                            }
                            lblName2.Text = e.Node.Text;
                            lblDate2.Text = tsDate.ToLongDateString() + "—" + tsDate.AddDays(6).ToLongDateString();
                            lblDate2.Left = panelChannel2.Width - lblDate2.Width;
                            //dgProgs.DataSource = dtProg;
                            colChannelDay.Visible = panelChannel2.Visible = true;
                        }
                    }
                    break;
                case 3:
                    if (e.Node.Parent != null)
                    {
                        DateTime tsDay = Convert.ToDateTime(e.Node.Name);
                        if (user == null)
                        {
                            dgProgs.DataSource = await TvProgController.GetSystemProgrammesOfDayAsyncList(typeProgID, int.Parse(e.Node.Parent.Name),
                                tsDay.AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                       Settings.Default.BeginEndTime.Minutes),
                                tsDay.AddDays(1).AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                       Settings.Default.BeginEndTime.Minutes));
                        }
                        else
                        {
                            dgProgs.DataSource = await TvProgController.GetUserProgrammesOfDayAsyncList(user.UserID, typeProgID, int.Parse(e.Node.Parent.Name),
                                tsDay.AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                       Settings.Default.BeginEndTime.Minutes),
                                tsDay.AddDays(1).AddHours(Settings.Default.BeginEndTime.Hours).AddMinutes(
                                       Settings.Default.BeginEndTime.Minutes));
                        }
                        
                        lblDate2.Text = tsDay.ToLongDateString() + " (" + tsDay.ToString("ddd", new CultureInfo("ru-Ru")) +
                                       ")";
                        lblDate2.Left = panelChannel2.Width - lblDate2.Width;
                        Image chImage = imgLstBig.Images[string.Concat(e.Node.Parent.Name, "_full")];
                        if (chImage != null)
                        {
                            pbChannel2.Width = chImage.Width;
                            if (chImage.Height > panelChannel2.Height)
                            {
                                pbChannel2.Height = panelChannel2.Height;
                                pbChannel2.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                            else
                            {
                                pbChannel2.Height = chImage.Height;
                                pbChannel2.SizeMode = PictureBoxSizeMode.CenterImage;
                            }
                            pbChannel2.Image = chImage;
                        }
                        lblName2.Text = e.Node.Parent.Text;
                    }
                    break;
            }
            /*foreach (DataGridViewRow dgvr in dgProgs.Rows)
            {
                dgvr.Cells["colChannelGenre"].ToolTipText =
                    !String.IsNullOrEmpty(dgvr.Cells["colChannelCategory"].Value.ToString()) ?
                    dgvr.Cells["colChannelCategory"].Value.ToString() : Resources.WithoutTypeText;
                dgvr.Cells["colProgRating"].ToolTipText =
                    !String.IsNullOrEmpty(dgvr.Cells["colChannelFavName"].Value.ToString()) ?
                    dgvr.Cells["colChannelFavName"].Value.ToString() : Resources.WithoutRatingText;
            } */
            slblCountTelecast.Text = Properties.Resources.Telecasts + dgProgs.Rows.Count;
        }

        private void AllWeekShowProg(TreeViewEventArgs e)
        {
            DataTable dtProg;
            DateTime tsStart = Convert.ToDateTime(e.Node.Name);
            dtProg = _tvProg.FiltTVProgram(_dsTvProg, 7, tsStart, String.Empty);
            panelChannel2.Visible = false;
            colChannelName.Visible = colChannelDay.Visible = colChannelImage.Visible = true;
            dgProgs.DataSource = dtProg;
        }

        /// <summary>
        /// Обновление таблицы "сейчас"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateDataGridNow(object sender, EventArgs e, DateTimeOffset dateTimeOffset)
        {
            this.dgNow.SelectionChanged -= this.dgNow_SelectionChanged;    
            if (chkNow.Checked) dateTimeOffset = DateTimeOffset.Now;
            else dateTimeOffset = DateTimeOffset.Parse(cbWeekNow.Text + " " + DateTime.Now.ToShortTimeString() + " +03:00");
          //  DataTable dtNow = _tvProg.GetShowTVNow(_dsTvProg, datetime);
            // Остановить срабатывание тела SelectionChanged для dgNow
            
            ListSortDirection direction;
            DataGridViewColumn dgColSorted = dgNow.SortedColumn;
            SortOrder sortOrder = dgNow.SortOrder;
            dgNow.AutoGenerateColumns = false;
            int typeProgID = GetTypeProgID();
            if (typeProgID == -1)
                return;
            SystemProgramme[] now;
            if (TVEnvironment.currentUser == null)
            {
                now = await TvProgController.GetSystemProgrammesAtNowAsycList(typeProgID, datetime_for_now);
            }
            else
            {
                now = await TvProgController.GetUserProgrammesAtNowAsyncList(TVEnvironment.currentUser.UserID,
                    typeProgID, datetime_for_now);
            }
            now.ToList<SystemProgramme>().ForEach(p => { p.ChannelContent = imgLst.Images[string.Concat(p.CID, "_25")]; });

            dgNow.DataSource = now;
            
            if (dgColSorted != null)
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Ascending;
                }
                else if (sortOrder == SortOrder.Descending)
                {
                    direction = ListSortDirection.Descending;
                }
                else direction = ListSortDirection.Ascending;
                dgNow.Sort(dgColSorted, direction);
            }
            if (dgNow.Rows.Count > 0)
            {
                try
                {
                    dgNow.Rows[0].Selected = false;
                    dgNow.Rows[_curNowRowIndex].Selected = true;   // - не сбрасывать предыдущее выделение
                    dgNow.CurrentCell = dgNow.Rows[_curNowRowIndex].Cells["colTelecastTitle"]; // - назначить текущей
                    dgNow_SelectionChanged(sender, e);
                }
                catch (Exception ex)
                {
                    Statics.EL.LogException(ex);
                }
            }
            // Подсказки для жанров и рейтинга:
            foreach (DataGridViewRow dgvr in dgNow.Rows)
            {
                if (dgvr.Cells["colGenreName"].Value != null)
                {
                    dgvr.Cells["colGenre"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colGenreName"].Value.ToString())
                            ? dgvr.Cells["colGenreName"].Value.ToString()
                            : Resources.WithoutTypeText;
                }
                if (dgvr.Cells["colFavName"].Value != null)
                {
                    dgvr.Cells["colRating"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colFavName"].Value.ToString())
                            ? dgvr.Cells["colFavName"].Value.ToString()
                            : Resources.WithoutRatingText;
                }
            }

            lblTime.Text = DateTime.Now.ToShortTimeString();
            this.dgNow.SelectionChanged += new System.EventHandler(this.dgNow_SelectionChanged);
        }

        private ListSortDirection _nextDirection;
        private DataGridViewColumn _dgNextColSorted;
        private SortOrder _nextSortOrder;
        /// <summary>
        /// Обновление таблицы "Затем"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateDataGridNext(object sender, EventArgs e)
        {
            //DataTable dtNext = _tvProg.GetShowTVNext(_dsTvProg, datetime_for_next);
            this.dgNext.SelectionChanged -= this.dgNext_SelectionChanged ;
            _dgNextColSorted = dgNext.SortedColumn;
            _nextSortOrder = dgNext.SortOrder;
            dgNext.AutoGenerateColumns = false;
            int typeProgID = GetTypeProgID();
            if (typeProgID == -1)
                return;
            SystemProgramme[] next;
            if (TVEnvironment.currentUser == null)
            {
                next = await TvProgController.GetSystemProgrammesAtNextAsycList(typeProgID, datetime_for_next);
            }
            else
            {
                next = await TvProgController.GetUserProgrammesAtNextAsyncList(TVEnvironment.currentUser.UserID,
                    typeProgID, datetime_for_next);
            }
            next.ToList<SystemProgramme>().ForEach(p => { p.ChannelContent = imgLst.Images[string.Concat(p.CID, "_25")]; });
            dgNext.DataSource = next;

            foreach (DataGridViewRow dgvr in dgNext.Rows)
            {
                if (dgvr.Cells["colNextCategory"].Value != null)
                {
                    dgvr.Cells["colNextGenre"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colGenreNameNext"].Value.ToString())
                            ? dgvr.Cells["colGenreNameNext"].Value.ToString()
                            : Resources.WithoutTypeText;
                }
                if (dgvr.Cells["colNextFavName"].Value != null)
                {
                    dgvr.Cells["colNextRating"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colNextFavName"].Value.ToString())
                            ? dgvr.Cells["colNextFavName"].ToString()
                            : Resources.WithoutRatingText;
                }
            }

            lblTime2.Text = DateTime.Now.ToShortTimeString();
        }

        /// <summary>
        /// Обновление таблицы "По дням недели"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDataGridChannels(object sender, EventArgs e)
        {
            if (tvWeeks.SelectedNode != null)
            {
                tvWeeks_AfterSelect(sender, new TreeViewEventArgs(tvWeeks.SelectedNode));
            }
        }

        /// <summary>
        /// Обновление таблицы "По каналам"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDataGridProgs(object sender, EventArgs e)
        {
            if (tvChannels.SelectedNode != null)
            {
                tvChannels_AfterSelect(sender, new TreeViewEventArgs(tvChannels.SelectedNode));
            }

        }


        /// <summary>
        /// При срабатывании таймера обновления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Second == 1)
            { // Срабатывание в :00(:01) секунд системного времени
                chkNow_CheckedChanged(sender, e);
                UpdateDataGridNow(sender, e, datetime_for_now);
                UpdateDataGridNext(sender, e);

                dgChannels.Refresh();
                dgChannels_SelectionChanged(sender, e);
                dgProgs.Refresh();
                dgProgs_SelectionChanged(sender, e);

                StatRemindsAndRecords();
            }
        }

        /// <summary>
        /// Статистика по напоминаниям и записям
        /// </summary>
        private void StatRemindsAndRecords()
        {
            if (_dsTvProg.Tables["AllProgrammes"].Columns.Contains("remind"))
            {
                int qtyReminds =
                    Convert.ToInt32(_dsTvProg.Tables["AllProgrammes"].Compute("Count(remind)", "remind = true"));
                string qtyText = String.Empty;
                if (qtyReminds > 0)
                {
                    lblsQtyRemind.Visible = true;
                    qtyText = Resources.RemindsText + qtyReminds;
                    lblsQtyRemind.Text = qtyReminds.ToString();
                    lblImageRemind.Image = Resources.bell;
                    lblsQtyRemind.ToolTipText = lblImageRemind.ToolTipText = qtyText;
                }
                else
                {
                    qtyText = Resources.NoRemindsText;
                    lblsQtyRemind.Text = String.Empty;
                    lblImageRemind.Image = Resources.bellempty;
                    lblsQtyRemind.ToolTipText = lblImageRemind.ToolTipText = qtyText;
                }
                string qtyText2 = String.Empty;
                if (_capture != null)
                {
                    int qtyRecords =
                        Convert.ToInt32(_dsTvProg.Tables["AllProgrammes"].Compute("Count(record)", "record = True"));
                    if (qtyRecords > 0)
                    {
                        lblQtyRecords.Visible = true;
                        qtyText2 = Resources.RecordsText + qtyRecords;
                        lblQtyRecords.Text = qtyRecords.ToString();
                        lblImageRecord.Image = Resources.capture;
                        lblQtyRecords.ToolTipText = lblImageRecord.ToolTipText = qtyText;
                    }
                    else
                    {
                        qtyText2 = Resources.NoRecordsText;
                        lblQtyRecords.Text = String.Empty;
                        lblImageRecord.Image = Resources.capturempty;
                        lblQtyRecords.ToolTipText = lblImageRecord.ToolTipText = qtyText;
                    }
                }

                nIcon.Text = "TVProgViewer.TVProgApp: " + Application.ProductVersion.ToString();
                nIcon.Text += "\n" + qtyText;
                if (_capture != null) nIcon.Text += "\n" + qtyText2;
            }
        }

      

        private void tvWeeks_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 2)
            {
                if (e.Node.IsExpanded)
                {
                    e.Node.ImageKey = e.Node.SelectedImageKey;
                }
            }
        }

        private void tvWeeks_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 2)
            {
                if (!e.Node.IsExpanded)
                {
                    if (e.Node.SelectedImageKey.ToString().Length >= 3)
                    {
                        e.Node.ImageKey = e.Node.SelectedImageKey.ToString().Substring(0, 3);
                    }
                }
            }
        }

        /// <summary>
        /// Закрыть панель
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExitDesc_Click(object sender, EventArgs e)
        {
            pAnons.Visible = false;
        }

        /// <summary>
        /// При изменении выделения строки таблицы "Сейчас"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgNow_SelectionChanged(object sender, EventArgs e)
        {
            if (dgNow.CurrentRow != null && tabMain.SelectedIndex == 0)
            {
                _curNowRowIndex = dgNow.CurrentRow.Index;

                if (!rtbAnons.Modified)
                {
                    object descrVal = dgNow.CurrentRow.Cells["colTelecastDescr"].Value;
                    switch (Preferences.showAnons) // Как показывать анонсы:
                    {
                        case Preferences.ShowAnons.Ifitis:
                            if (descrVal != null && !String.IsNullOrEmpty(descrVal.ToString()))
                            {
                                rtbAnons.Text = descrVal.ToString();
                                pAnons.Visible = true;
                            }
                            else
                            {
                                rtbAnons.Text = String.Empty;
                                pAnons.Visible = false;
                            }
                            break;
                        case Preferences.ShowAnons.Always:
                            if (descrVal != null)
                            {
                                rtbAnons.Text = descrVal.ToString();
                            }
                            pAnons.Visible = true;
                            break;
                        case Preferences.ShowAnons.Never:
                            rtbAnons.Text = String.Empty;
                            pAnons.Visible = false;
                            break;
                    }
                }
                rtbAnons.Modified = false;
            }
        }

        /// <summary>
        /// При изменении выделения строки таблицы "Затем"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgNext_SelectionChanged(object sender, EventArgs e)
        {
            if (dgNext.CurrentRow != null && tabMain.SelectedIndex == 1)
            {
                _curNextRowIndex = dgNext.CurrentRow.Index;
                if (!rtbAnons.Modified)
                {
                    object descrVal = dgNext.CurrentRow.Cells["colNextDesc"].Value;
                    switch (Preferences.showAnons)
                    {
                        case Preferences.ShowAnons.Ifitis:
                            if (descrVal != null && !String.IsNullOrEmpty(descrVal.ToString()))
                            {
                                rtbAnons.Text = descrVal.ToString();
                                pAnons.Visible = true;
                            }
                            else
                            {
                                rtbAnons.Text = String.Empty;
                                pAnons.Visible = false;
                            }
                            break;
                        case Preferences.ShowAnons.Always:
                            if (descrVal != null)
                            {
                                rtbAnons.Text = descrVal.ToString();
                            }
                            pAnons.Visible = true;
                            break;
                        case Preferences.ShowAnons.Never:
                            rtbAnons.Text = String.Empty;
                            pAnons.Visible = false;
                            break;
                    }
                }
                rtbAnons.Modified = false;
            }
        }

        /// <summary>
        /// При перемещении выделения строки таблицы "По дням недели"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgChannels_SelectionChanged(object sender, EventArgs e)
        {
            if (dgChannels.CurrentRow != null && tabMain.SelectedIndex == 2)
            {
                _curChannelsRowIndex = dgChannels.CurrentRow.Index;
                object val = dgChannels.Rows[_curChannelsRowIndex].Cells["colChanId"].Value;
                if (val != null)
                {
                    Image chImage = imgLstBig.Images[string.Concat(val.ToString(), "_full")];

                    if (chImage != null)
                    {
                        pbChannel.Width = chImage.Width;
                        if (chImage.Height > panelChannel.Height)
                        {
                            pbChannel.Height = panelChannel.Height;
                            pbChannel.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                        {
                            pbChannel.Height = chImage.Height;
                            pbChannel.SizeMode = PictureBoxSizeMode.CenterImage;
                        }
                        pbChannel.Image = chImage;
                    }
                    else
                    {
                        pbChannel.Image = null;
                    }
                } 
                if (!rtbAnons.Modified)
                {
                    object anonsVal = dgChannels.CurrentRow.Cells["colChanDesc"].Value;
                    switch (Preferences.showAnons)
                    {
                        case Preferences.ShowAnons.Ifitis:
                            if (anonsVal != null && !String.IsNullOrEmpty(anonsVal.ToString()))
                            {
                                rtbAnons.Text = anonsVal.ToString();
                                pAnons.Visible = true;
                            }
                            else
                            {
                                rtbAnons.Text = String.Empty;
                                pAnons.Visible = false;
                            }
                            break;
                        case Preferences.ShowAnons.Always:
                            rtbAnons.Text = anonsVal.ToString();
                            pAnons.Visible = true;
                            break;
                        case Preferences.ShowAnons.Never:
                            rtbAnons.Text = String.Empty;
                            pAnons.Visible = false;
                            break;
                    }
                }
                rtbAnons.Modified = false;
            }     
        }

        /// <summary>
        /// При перемещении выделения строки таблицы "По каналам"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgProgs_SelectionChanged(object sender, EventArgs e)
        {
            if (dgProgs.CurrentRow != null && tabMain.SelectedIndex == 3)
            {
                _curProgsRowIndex = dgProgs.CurrentRow.Index;
                object anonsVal = dgProgs.CurrentRow.Cells["colChannelDesc"].Value;
                switch (Preferences.showAnons)
                {
                    case Preferences.ShowAnons.Ifitis:
                        if (anonsVal != null && !String.IsNullOrEmpty(anonsVal.ToString()))
                        {
                            rtbAnons.Text = anonsVal.ToString();
                            pAnons.Visible = true;
                        }
                        else
                        {
                            rtbAnons.Text = String.Empty;
                            pAnons.Visible = false;
                        }
                        break;
                    case Preferences.ShowAnons.Always:
                        rtbAnons.Text = anonsVal.ToString();
                        pAnons.Visible = true;
                        break;
                    case Preferences.ShowAnons.Never:
                        rtbAnons.Text = String.Empty;
                        pAnons.Visible = false;
                        break;
                }
            }
        }

        /// <summary>
        /// При изменении индекса закладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Preferences.showAnons == Preferences.ShowAnons.Ifitis)
            { pAnons.Visible = false; }
            switch (tabMain.SelectedIndex)
            {
                case 0:
                    dgNow.Font = Settings.Default.TableFont;
                    dgNow_SelectionChanged(sender, e);
                    tsmiNow.Checked = true;
                    tsmiNext.Checked = tsmiDays.Checked = tsmiChannel.Checked = false;
                    slblCountTelecast.Text = Properties.Resources.Telecasts + dgNow.Rows.Count;
                    break;
                case 1:
                    dgNext.Font = Settings.Default.TableFont;
                    dgNext_SelectionChanged(sender, e);
                    tsmiNext.Checked = true;
                    tsmiDays.Checked = tsmiNow.Checked = tsmiChannel.Checked = false;
                    slblCountTelecast.Text = Properties.Resources.Telecasts + dgNext.Rows.Count;
                    break;
                case 2:
                    dgChannels.Font = Settings.Default.TableFont;
                    dgChannels_SelectionChanged(sender, e);
                    tsmiDays.Checked = true;
                    tsmiNow.Checked = tsmiNext.Checked = tsmiChannel.Checked = false;
                    slblCountTelecast.Text = Properties.Resources.Telecasts + dgChannels.Rows.Count;
                    break;
                case 3:
                    dgProgs.Font = Settings.Default.TableFont;
                    dgProgs_SelectionChanged(sender, e);
                    tsmiChannel.Checked = true;
                    tsmiNow.Checked = tsmiNext.Checked = tsmiDays.Checked = false;
                    slblCountTelecast.Text = Properties.Resources.Telecasts + dgProgs.Rows.Count;
                    break;
            }
        }
        /// <summary>
        /// При модификации анонса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Отмена действий для анонса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelDesc_Click(object sender, EventArgs e)
        {
            rtbAnons.Undo();
            rtbAnons.Modified = false;
        }
        /// <summary>
        /// Сохранение анонса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDesc_Click(object sender, EventArgs e)
        {
            // Инициализация:
            string idChan = String.Empty;       // - код канала
            string title = String.Empty;        // - название передачи
            DateTime from = DateTime.MinValue;  // - дата начала передачи
            DateTime to = DateTime.MinValue;    // - дата завершения передачи
            DataRow foundRow = null;
            GetTelecastInfo(ref idChan, ref title, ref from, ref to, ref foundRow);
            if (idChan != String.Empty && title != String.Empty &&
                from != DateTime.MinValue && to != DateTime.MinValue)
            {
                if (_tvProg.SaveDescription(idChan, from, to, title, rtbAnons.Text))
                {
                    if (foundRow != null)
                    {
                        foundRow.BeginEdit();
                        foundRow["desc"] = rtbAnons.Text;
                        foundRow.EndEdit();
                    }
                    switch (tabMain.SelectedIndex)
                    {
                        case 0: // Сейчас:
                            if (dgNow.CurrentRow != null)
                            {
                                dgNow.CurrentRow.Cells["colTelecastDescr"].Value = rtbAnons.Text;
                                UpdateDataGridNow(sender, e, datetime_for_now);
                            }
                            break;
                        case 1: // Затем:
                            if (dgNext.CurrentRow != null)
                            {
                                dgNext.CurrentRow.Cells["colNextDesc"].Value = rtbAnons.Text;
                                UpdateDataGridNext(sender, e);
                            }
                            break;
                        case 2: // По датам:
                            if (dgChannels.CurrentRow != null)
                            {
                                dgChannels.CurrentRow.Cells["colChanDesc"].Value = rtbAnons.Text;
                                UpdateDataGridChannels(sender, e);
                            }
                            break;
                        case 3: // По каналам:
                            if (dgProgs.CurrentRow != null)
                            {
                                dgProgs.CurrentRow.Cells["colChannelDesc"].Value = rtbAnons.Text;
                                UpdateDataGridProgs(sender, e);
                            }
                            break;
                    }
                }
            }
            rtbAnons.Modified = false;
        }
        /// <summary>
        /// Поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (_schForm.ShowDialog(this) == DialogResult.OK) { ;}
        }

        /// <summary>
        /// Классификатор жанров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenre_Click(object sender, EventArgs e)
        {
            Preferences.genres = new Genres(_dsTvProg.Tables["Genres"]);
            _genreForm = new GenreForm(Preferences.genres);
            _genreForm.ShowDialog(this);
            _dsTvProg.Tables.Remove("KeywordsTable");
            DataTable dtClassifGenres = Preferences.genres.ClassifTable.Select("", "prior DESC").CopyToDataTable();
            dtClassifGenres.TableName = "KeywordsTable";
            _dsTvProg.Tables.Add(dtClassifGenres);
        }
        private void OpenTab(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                int tagTab = int.Parse((sender as ToolStripMenuItem).Tag.ToString()) - 1;
                tabMain.SelectTab(tagTab);
                switch (tagTab)
                {
                    case 0:
                        tsmiNow.Checked = true;
                        tsmiNext.Checked = tsmiDays.Checked = tsmiChannel.Checked = false;
                        break;
                    case 1:
                        tsmiNext.Checked = true;
                        tsmiNow.Checked = tsmiDays.Checked = tsmiChannel.Checked = false;
                        break;
                    case 2:
                        tsmiDays.Checked = true;
                        tsmiNow.Checked = tsmiNext.Checked = tsmiChannel.Checked = false;
                        break;
                    case 3:
                        tsmiChannel.Checked = true;
                        tsmiNow.Checked = tsmiNext.Checked = tsmiDays.Checked = false;
                        break;
                }
            }
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bClose = true;
            this.Close();
            Application.Exit();
        }
        /// <summary>
        /// Показать рейтиги (любимые)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRating_Click(object sender, EventArgs e)
        {
            Preferences.favorites = new Favorites(_dsTvProg.Tables["Favorites"]);
            _ratingForm = new RatingForm(Preferences.favorites);
            _ratingForm.ShowDialog(this);

            var thread1 = new Thread(CheckAndSetRemindAndFavorite)
            {
                Name = "ChekAndSetRemindAndFavorite",
                Priority = ThreadPriority.Lowest
            };
            thread1.Start();
        }
        /// <summary>
        /// Проверка передач на любимость и выставление при необходимости напоминаний
        /// </summary>
        private void CheckAndSetRemindAndFavorite()
        {
            // Список передач для напоминания:
            List<Tuple<string, string, DateTime, DateTime, bool>> lstRemind =
                new List<Tuple<string, string, DateTime, DateTime, bool>>();

            _dsTvProg.Tables.Remove("FavwordsTable");
            DataTable dtClassifFavs = Preferences.favorites.ClassifTable.Rows.Count > 0
                                          ? Preferences.favorites.ClassifTable.Select("", "prior DESC").CopyToDataTable()
                                          : new DataTable("FavwordsTable");
            dtClassifFavs.TableName = "FavwordsTable";
            _dsTvProg.Tables.Add(dtClassifFavs);

            foreach (DataRow drClassifRating in _dsTvProg.Tables["FavwordsTable"].Rows)
            {
                DataRow[] drsProgs = _dsTvProg.Tables["AllProgrammes"].Select(
                    "title LIKE '*" + drClassifRating["contain"].ToString() + "*' " + _tvProg.GlobalFiltChan);
                foreach (DataRow curRow in drsProgs)
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
                                    _dsTvProg.Tables["Favorites"].Select("id = " + drClassifRating["fid"]);
                                if (drsRating.Length > 0)
                                {
                                    foreach (DataRow drFavorite in drsRating)
                                    {
                                        if ((bool)drFavorite["visible"])
                                        {
                                            if ((bool)drClassifRating["remind"])
                                            {
                                                lstRemind.Add(
                                                    new Tuple<string, string, DateTime, DateTime, bool>(
                                                        curRow["cid"].ToString(),
                                                        curRow["title"].ToString(),
                                                        (DateTime)curRow["start"],
                                                        (DateTime)curRow["stop"], true));

                                                curRow["remind"] = true;
                                            }
                                            else
                                            {
                                                lstRemind.Add(
                                                    new Tuple<string, string, DateTime, DateTime, bool>(
                                                        curRow["cid"].ToString(),
                                                        curRow["title"].ToString(),
                                                        (DateTime)curRow["start"],
                                                        (DateTime)curRow["stop"], false));

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
                                    lstRemind.Add(
                                        new Tuple<string, string, DateTime, DateTime, bool>(
                                            curRow["cid"].ToString(),
                                            curRow["title"].ToString(),
                                            (DateTime)curRow["start"],
                                            (DateTime)curRow["stop"], false));

                                    curRow["remind"] = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            _tvProg.SetRemind(lstRemind);
        }
        /// <summary>
        /// При щелчке на колокольчике
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgNow_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && (e.ColumnIndex == 2 || e.ColumnIndex == 3))
            {
                string idProg = dgNow.Rows[e.RowIndex].Cells["colID"].Value.ToString();
                string idChan = dgNow.Rows[e.RowIndex].Cells["colCID"].Value.ToString();
                string title = dgNow.Rows[e.RowIndex].Cells["colTelecastTitle"].Value.ToString();
                DateTimeOffset from = (DateTimeOffset) dgNow.Rows[e.RowIndex].Cells["colFrom"].Value;
                DateTimeOffset to = (DateTimeOffset) dgNow.Rows[e.RowIndex].Cells["colTo"].Value;
                switch (e.ColumnIndex)
                {
                    case 2:
                        bool statusRec = false;
                        if (SetStatusRec(idProg, idChan, title, from, to, ref statusRec))
                        {
                            UpdateDataGridNow(sender, e, datetime_for_now);
                            if (_capture != null)
                            {
                                if (statusRec)
                                {
                                    if (!_capture.Capturing)
                                    {
                                        try
                                        {
                                            if (_dsTvProg.Tables["AllChannels"].Columns.Contains("freq"))
                                            {
                                                string strTextChannel =
                                                    dgNow.Rows[e.RowIndex].Cells["colNameChannel"].Value.ToString().
                                                        Contains
                                                        ("'")
                                                        ? dgNow.Rows[e.RowIndex].Cells["colNameChannel"].Value.ToString()
                                                              .
                                                              Replace("'", "''")
                                                        : dgNow.Rows[e.RowIndex].Cells["colNameChannel"].Value.ToString();
                                                long freqChan =
                                                    (long)
                                                    _dsTvProg.Tables["AllChannels"].Select(
                                                        String.Format("[user-name] = '{0}'", strTextChannel))[0]["freq"];
                                                 _capture.Stop();
                                                _capture.Tuner.CountryCode = TunerSettings.Default.CountryCode;
                                                _capture.Tuner.AudioMode = AMTunerModeType.TV;
                                                _capture.Tuner.SetFrequency((int) freqChan);
                                                if (_capture.Stopped && !_capture.Cued)
                                                {
                                                    _capture.Filename = TunerSettings.Default.CaptureDir + @"\" +
                                                                        TransformFileName(
                                                                            TunerSettings.Default.PatternCaptureFileName,
                                                                            dgNow.Rows[e.RowIndex].Cells[
                                                                                "colNameChannel"].
                                                                                Value.ToString(),
                                                                            title);

                                                    _capture.RecFileMode = (DirectX.Capture.Capture.RecFileModeType)
                                                                           Enum.Parse(
                                                                               typeof (
                                                                                   DirectX.Capture.Capture.
                                                                                   RecFileModeType),
                                                                               TunerSettings.Default.RecordMode.ToString());

                                                    _capture.Cue();

                                                    _capture.Start(); // - запуск видеозахвата
                                                    timerCapture = new Timer
                                                                       {
                                                                           Interval =
                                                                               (int)
                                                                               (to - DateTime.Now).TotalMilliseconds
                                                                       };
                                                    timerCapture.Tick += new EventHandler(timer_Tick);
                                                    timerCapture.Start();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Statics.EL.LogException(ex);
                                            MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    if (_capture.Capturing)
                                    {
                                        try
                                        {
                                            _capture.Stop();
                                            timerCapture.Stop();
                                            timerCapture.Tick -= new EventHandler(timer_Tick);
                                        }
                                        catch (Exception ex)
                                        {
                                            Statics.EL.LogException(ex);
                                            MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case 3:
                        bool statusBell = false;
                        if (SetStatusBell(idProg, idChan, title, from, to, ref statusBell))
                        {
                            UpdateDataGridNow(sender, e, datetime_for_now);
                        }
                        break;
                }
                StatRemindsAndRecords();
            }

        }

        

        /// <summary>
        /// Изменение статуса напоминания
        /// </summary>
        /// <param name="idProg">Код передачи</param>
        /// <param name="idChan">Код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="from">Дата начала передачи</param>
        /// <param name="to">Дата окончания передачи</param>
        /// <param name="statusBell">Статус колокольчика</param>
        /// <returns>Результат выполения</returns>
        private bool SetStatusBell(string idProg, string idChan, string title,
            DateTimeOffset from, DateTimeOffset to, ref bool statusBell)
        {
            statusBell = false;
            if (_dsTvProg.Tables["AllProgrammes"].PrimaryKey.Length == 0)
            {
                DataColumn[] keyProgColumns = new DataColumn[1];
                keyProgColumns[0] = _dsTvProg.Tables["AllProgrammes"].Columns["id"];
                _dsTvProg.Tables["AllProgrammes"].PrimaryKey = keyProgColumns;
            }
            DataRow foundRow = _dsTvProg.Tables["AllProgrammes"].Rows.Find(long.Parse(idProg));
            if (foundRow != null)
            {
                statusBell = !String.IsNullOrEmpty(foundRow["remind"].ToString())
                                      ? !(bool)foundRow["remind"]
                                      : true;
                if (_tvProg.SetRemind(idChan, title, from, to, statusBell))
                {
                    foundRow.BeginEdit();
                    foundRow["remind"] = statusBell;
                    foundRow.EndEdit();
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
        private bool SetStatusRec(string idProg, string idChan, string title, DateTimeOffset from, DateTimeOffset to, ref bool statusRec)
        {
            statusRec = false;
            DataRow foundRow = _dsTvProg.Tables["AllProgrammes"].Rows.Find(long.Parse(idProg));
            if (foundRow != null)
            {
                statusRec = !String.IsNullOrEmpty(foundRow["record"].ToString())
                                ? !(bool) foundRow["record"]
                                : true;
                if (statusRec)
                {
                    DataRow[] drsCasts =
                    _dsTvProg.Tables["AllProgrammes"].Select(
                        String.Format("((start >= '{0}' and (start < '{1}' and stop >= '{1}')) or " +
                        "(start <= '{0}' and (stop > '{0}' and stop <= '{1}')) or " +
                        "(start >= '{0}' and stop <= '{1}') or (start <='{0}' and stop >= '{1}')) and record = True", from, to));
                    if (drsCasts.Length > 0)
                    {
                        if (drsCasts.Length == 1)
                        {
                            if (drsCasts[0]["title"].ToString() != title || drsCasts[0]["cid"].ToString() != idChan)
                            {
                                Statics.ShowDialog(Resources.WarningText, Resources.ImpossibleRecText,
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

        private void dgNext_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && (e.ColumnIndex == 2 || e.ColumnIndex == 3))
            {
                string idProg = dgNext.Rows[e.RowIndex].Cells["colNextID"].Value.ToString();
                string idChan = dgNext.Rows[e.RowIndex].Cells["colNextCID"].Value.ToString();
                string title = dgNext.Rows[e.RowIndex].Cells["colNextTitle"].Value.ToString();
                DateTime from = (DateTime)dgNext.Rows[e.RowIndex].Cells["colNextFrom"].Value;
                DateTime to = (DateTime)dgNext.Rows[e.RowIndex].Cells["colNextTo"].Value;
                switch (e.ColumnIndex)
                {
                    case 2:
                        bool statusRec = false;
                        if (SetStatusRec(idProg, idChan, title, from, to, ref statusRec))
                        {
                            UpdateDataGridNext(sender, e);
                        }
                        break;
                    case 3:
                         bool statusBell = false;
                         if (SetStatusBell(idProg, idChan, title, from, to, ref statusBell))
                         {
                             UpdateDataGridNext(sender, e);
                         }
                        break;
                }
                StatRemindsAndRecords();
            }
        }
        private void dgChannels_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && (e.ColumnIndex == 2 || e.ColumnIndex == 3))
            {
                string idProg = dgChannels.Rows[e.RowIndex].Cells["colProgID"].Value.ToString();
                string idChan = dgChannels.Rows[e.RowIndex].Cells["colChanId"].Value.ToString();
                string title = dgChannels.Rows[e.RowIndex].Cells["colChanProg"].Value.ToString();
                DateTime from = (DateTime)dgChannels.Rows[e.RowIndex].Cells["colChanFrom"].Value;
                DateTime to = (DateTime)dgChannels.Rows[e.RowIndex].Cells["colChanTo"].Value;
                switch (e.ColumnIndex)
                {
                    case 2:
                        bool statusRec = false;
                        if (SetStatusRec(idProg, idChan, title, from, to, ref statusRec))
                        {
                            dgChannels.Rows[e.RowIndex].Cells["colChanRec"].Value =
                                statusRec ? Resources.capture : Resources.capturempty;
                        }
                        break;
                    case 3:
                        bool statusBell = false;
                        if (SetStatusBell(idProg, idChan, title, from, to, ref statusBell))
                        {
                            dgChannels.Rows[e.RowIndex].Cells["colChanBell"].Value =
                                statusBell ? Resources.bell : Resources.bellempty;
                        }
                        break;
                }
                StatRemindsAndRecords();
            }
        }
        private void dgProgs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && (e.ColumnIndex == 2 || e.ColumnIndex == 3))
            {
                string idProg = dgProgs.Rows[e.RowIndex].Cells["colProgramId"].Value.ToString();
                string idChan = dgProgs.Rows[e.RowIndex].Cells["colChannelId"].Value.ToString();
                string title = dgProgs.Rows[e.RowIndex].Cells["colChannelTitle"].Value.ToString();
                DateTime from = (DateTime) dgProgs.Rows[e.RowIndex].Cells["colChannelFrom"].Value;
                DateTime to = (DateTime) dgProgs.Rows[e.RowIndex].Cells["colChannelTo"].Value;
                switch (e.ColumnIndex)
                {
                    case 2:
                        bool statusRec = false;
                        if (SetStatusRec(idProg, idChan, title, from, to, ref statusRec))
                        {
                            dgProgs.Rows[e.RowIndex].Cells["colChannelRec"].Value =
                                statusRec ? Resources.capture : Resources.capturempty;
                        }
                        break;
                    case 3:
                        bool statusBell = false;
                        if (SetStatusBell(idProg, idChan, title, from, to, ref statusBell))
                        {
                            dgProgs.Rows[e.RowIndex].Cells["colChannelBell"].Value =
                                statusBell ? Resources.bell : Resources.bellempty;
                        }
                        break;
                }
                StatRemindsAndRecords();
            }
        }
        private string TransformFileName(string pattern, DataRow drName)
        {
            pattern = pattern.Replace(Resources.DateText, DateTime.Today.ToString("ddMMyyyy"));
            pattern = pattern.Replace(Resources.TimeText, DateTime.Now.ToString("HHmm"));
            pattern = pattern.Replace(Resources.ProgramText, drName["display-name"].ToString());
            pattern = pattern.Replace(Resources.TelecastText, drName["title"].ToString());
            pattern = ReplaceFileName(pattern, new char[] { '\\', '/', '*', '?', ':', '<', '>', '|', '\"'}, ' ' );
            return pattern;
        }
        private string TransformFileName(string pattern, string channel, string title)
        {
            pattern = pattern.Replace(Resources.DateText, DateTime.Today.ToString("ddMMyyyy"));
            pattern = pattern.Replace(Resources.TimeText, DateTime.Now.ToString("HHmm"));
            pattern = pattern.Replace(Resources.ProgramText, channel);
            pattern = pattern.Replace(Resources.Telecasts, title);
            pattern = ReplaceFileName(pattern, new char[]
                                                   {
                                                       '\\',
                                                       '/',
                                                       '*',
                                                       '?',
                                                       ':',
                                                       '<',
                                                       '>',
                                                       '|',
                                                       '\"'
                                                   }
                                      , ' ');
            return pattern;
        }

        private string ReplaceFileName(string pattern, char[] charArr, char newChar)
        {
            foreach (char ch in charArr)
            {
                pattern = pattern.Replace(ch, newChar);
            }
            return pattern;
        }

        TimeSpan timeOfRefresh = new TimeSpan();
        /// <summary>
        /// Срабатывание таймера расписания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         private void timerScheduler_Tick(object sender, EventArgs e)
        {
            string dayOfWeek = DateTime.Now.DayOfWeek.ToString().ToLower();
            bool flagEnabled = false;
             
            foreach (SettingsProperty dayOW in Settings.Default.Properties)
            {
                if (dayOW.Name.ToLower() == dayOfWeek && dayOW.PropertyType == typeof(bool))
                {
                    if (flagEnabled = bool.Parse(Settings.Default[dayOW.Name].ToString()))
                    {
                        foreach (SettingsProperty time in Settings.Default.Properties)
                        {
                            if (time.Name.ToLower().Contains(Preferences.dictWeek[dayOfWeek].ToLower()) && time.PropertyType == typeof(TimeSpan))
                            {
                                timeOfRefresh = (TimeSpan) Settings.Default[time.Name];
                            }
                        }
                    }
                }
            }
            
            if (flagEnabled && timeOfRefresh == new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))
            {
                btnDownload_Click(sender, e);
            }
        }
        /// <summary>
        /// Срабатывание таймера напоминаний
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerReminder_Tick(object sender, EventArgs e)
        {
            Win32API.EnumWindowsProc callbackPtr = new Win32API.EnumWindowsProc(Win32API.FullScreenForm);
            Win32API.EnumWindows(callbackPtr, IntPtr.Zero); // - перечислим все окна для поиска полноэкранного окна
            // Если включать напоминатель при полноэкранных приложениях, 
            // тогда даже, если запущено полноэкранное приложение, считаем,
            // что оно не полноэкранное и запускаем напоминатель
            if (!Reminder.Default.FullScreenMain) Win32API.fullMode = false;
            if (Reminder.Default.On && !Win32API.fullMode)
            {
                string dayOfWeek = DateTime.Now.DayOfWeek.ToString().ToLower();
                TimeSpan from = new TimeSpan(), to = new TimeSpan();
                bool constrain = false;
                
                if (DateTime.Now.Second == 0 || _firstin)
                {
                    // Начало минуты:
                    //_firstin = false;
                    foreach (SettingsProperty dayOW in Reminder.Default.Properties)
                    { // Перебор настроек
                        if (dayOW.Name.ToLower() == dayOfWeek && dayOW.PropertyType == typeof(bool))
                        {
                            if (constrain = bool.Parse(Reminder.Default[dayOW.Name].ToString()))
                            { // Есть ограничение по времени
                                foreach (SettingsProperty fromto in Reminder.Default.Properties)
                                {
                                    if (fromto.Name.ToLower().Contains(Preferences.dictWeek[dayOfWeek].ToLower()) &&
                                        fromto.PropertyType == typeof(TimeSpan))
                                    {
                                        if (fromto.Name.ToLower().Contains("from"))
                                            from = (TimeSpan) Reminder.Default[fromto.Name];
                                        if (fromto.Name.ToLower().Contains("to"))
                                            to = (TimeSpan) Reminder.Default[fromto.Name];
                                    }
                                }
                                break;
                            }
                            
                        }
                    }
                    if (constrain && from != new TimeSpan() && to != new TimeSpan())
                    {  // Ограничение есть
                        if (DateTime.Now.TimeOfDay >= from && DateTime.Now.TimeOfDay <= to)
                        { // В промежутке ограничения
                           // Remind();
                        }
                    }
                    else
                    { // Нет никаких ограничений
                       // Remind();
                    }
                }
            }
            if (Preferences.statusCapture == true)
            {
                if (DateTime.Now.Second == 0 || DateTime.Now.Second == 10 || _firstin)
                {
                    _firstin = false;
                   /* DataTable dtCapture = _tvProg.Recorder(_dsTvProg);
                    if (dtCapture != null)
                    {
                        if (dtCapture.Rows.Count == 1)
                        {
                            if (_capture != null)
                            {
                                if (!_capture.Capturing)
                                {
                                    try
                                    {
                                        if (_dsTvProg.Tables["AllChannels"].Columns.Contains("freq"))
                                        {
                                            string strTextChannel =
                                                dtCapture.Rows[0]["display-name"].ToString().Contains
                                                    ("'")
                                                    ? dtCapture.Rows[0]["display-name"].ToString().
                                                          Replace("'", "''")
                                                    : dtCapture.Rows[0]["display-name"].ToString();
                                            long freqChan =
                                                (long)
                                                _dsTvProg.Tables["AllChannels"].Select(
                                                    String.Format("[user-name] = '{0}'",
                                                                  strTextChannel))[0][
                                                                      "freq"];
                                            if (_capture.Cued)
                                            {
                                                _capture.Stop();
                                            }
                                            _capture.Tuner.AudioMode = AMTunerModeType.TV;
                                            _capture.Tuner.CountryCode = TunerSettings.Default.CountryCode;
                                            _capture.Tuner.SetFrequency((int) freqChan);
                                            
                                                _capture.Filename = TunerSettings.Default.CaptureDir + @"\" +
                                                                    TransformFileName(
                                                                        TunerSettings.Default.PatternCaptureFileName,
                                                                        dtCapture.Rows[0]);
                                                _capture.RecFileMode = (DirectX.Capture.Capture.RecFileModeType)
                                                                       Enum.Parse(
                                                                           typeof(
                                                                               DirectX.Capture.Capture.
                                                                               RecFileModeType),
                                                                           TunerSettings.Default.RecordMode.ToString());
                                            
                                            if (!_capture.Cued)
                                            {
                                                _capture.Cue();
                                            }
                                            _capture.Start();    // - запуск видеозахвата
                                        }
                                        timerCapture = new Timer
                                                           {
                                                               Interval =
                                                                   (int)
                                                                   (DateTime.Parse(dtCapture.Rows[0]["stop"].ToString()) -
                                                                    DateTime.Now).TotalMilliseconds
                                                           };
                                        timerCapture.Tick += new EventHandler(timer_Tick);
                                        timerCapture.Start();
                                    }
                                    catch(Exception ex)
                                    {
                                        Statics.EL.LogException(ex);
                                        MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
                                    }
                                }
                            }
                        }
                    }*/
                }
            }
        }

        private void Remind()
        {
            DataTable dtReminder = _tvProg.Reminder(_dsTvProg);
            if (dtReminder != null)
            {
                if (dtReminder.Rows.Count > 0)
                {
                    // Есть напоминания:
                    bool same = false; // - инициализация флага проверки одинаковости списка напоминаний
                    foreach (DataRow drRem in dtReminder.Rows)
                    {
                        // Перебор напоминаний:
                        bool waitrem = false; // - инициализация флага проверки для отображения формы
                        //  напоминаний через указанный интервал
                        foreach (DataRow drWaitRem in _tvProg.WaitRemind.Rows)
                        {
                            // Перебор по ожидающим напоминания программам:
                            if (int.Parse(drRem["id"].ToString()) == int.Parse(drWaitRem["id"].ToString()))
                            {
                                // Передача для напоминания совпала с передачей для ожидания напоминания:
                                waitrem = true; // - отобразить форму напоминания через указанное время
                                break;
                            }
                        }
                        if (!waitrem)
                        {
                            // Не отображается форма напоминаний:
                            DataRow drWaitRem = _tvProg.WaitRemind.NewRow();
                            drWaitRem["id"] = drRem["id"];
                            drWaitRem["title"] = drRem["title"];
                            drWaitRem["start"] = drRem["start"];
                            drWaitRem["waitto"] = DBNull.Value;
                            _tvProg.WaitRemind.Rows.Add(drWaitRem);
                        }
                    }
                    if (_remForm.dtRemind != null)
                    {
                        // Форма напомиания уже была ранее создана:
                        same = true; // - такие же напоминания - не выводить форму повторно
                        if (dtReminder.Rows.Count == _remForm.dtRemind.Rows.Count)
                        {
                            same = true;
                            foreach (DataRow drRem in dtReminder.Rows)
                            {
                                foreach (DataRow drRemform in _remForm.dtRemind.Rows)
                                {
                                    if ((drRem["title"] + drRem["start"].ToString()) ==
                                        (drRemform["title"] + drRemform["start"].ToString()))
                                    {
                                        same = true;
                                        break;
                                    }
                                    else
                                    {
                                        same = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            same = false;
                        }
                    }

                    if (!same)
                    {
                        if (!_remForm.IsDisposed)
                        {
                            _remForm.dtRemind = dtReminder.Copy();
                            if (!_remForm.Visible && Reminder.Default.Message)
                            {
                                _remForm.Show(this);
                            }
                        }
                        else
                        {
                            _remForm = new RemindForm(_tvProg);
                            _remForm.dtRemind = dtReminder.Copy();
                            if (Reminder.Default.Message)
                            {
                                _remForm.Show(this);
                            }
                        }
                        if (Reminder.Default.OnSound)
                        {
                            Preferences.PlaySound playSound =
                                (Preferences.PlaySound)
                                Enum.Parse(typeof (Preferences.PlaySound), Reminder.Default.SoundMode);
                            switch (playSound)
                            {
                                case Preferences.PlaySound.SystemSound:
                                    System.Media.SystemSounds.Exclamation.Play();
                                    break;
                                case Preferences.PlaySound.UserSound:
                                    if (Reminder.Default.FileNameSound.Length > 0)
                                    {
                                        try
                                        {
                                            System.Media.SoundPlayer sp = new SoundPlayer(Reminder.Default.FileNameSound);
                                            sp.Play();
                                        }catch(Exception ex)
                                        {
                                            Statics.EL.LogException(ex);
                                        }
                                    }
                                    break;
                                case Preferences.PlaySound.DynamicSound:
                                    Win32API.Beep(100, 500);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (!_remForm.IsDisposed)
                    {
                        if (_remForm.dtRemind != null)
                            if (_remForm.dtRemind.Rows.Count > 0)
                            {
                                _remForm.dtRemind.Rows.Clear();
                            }
                    }
                    if (_tvProg.WaitRemind.Rows.Count > 0)
                    {
                        _tvProg.WaitRemind.Rows.Clear();
                    }
                }
            }
            foreach (DataRow drWaitRem in _tvProg.WaitRemind.Rows)
            {
                if (drWaitRem["waitto"] != DBNull.Value)
                {
                    if (DateTime.Parse(drWaitRem["waitto"].ToString()) <= DateTime.Now)
                    {
                        drWaitRem.BeginEdit();
                        drWaitRem["waitto"] = DBNull.Value;
                        drWaitRem.EndEdit();
                        if (!_remForm.IsDisposed)
                        {
                            _remForm.ID = (int)drWaitRem["id"];
                            if (!_remForm.Visible && Reminder.Default.Message) _remForm.Show(this);
                        }
                        else
                        {
                            _remForm = new RemindForm(_tvProg);
                            _remForm.ID = (int)drWaitRem["id"];
                            if (Reminder.Default.Message)
                            {_remForm.Show(this);}
                        }
                        break;
                    }
                }
            }
        }

        private Timer timerCapture = null;
        private void timer_Tick(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_capture.Capturing)
                {
                    try
                    {
                        _capture.Stop();
                        timerCapture.Stop();
                        timerCapture.Tick -= new EventHandler(timer_Tick);
                    }
                     catch(Exception ex)
                    {
                         Statics.EL.LogException(ex);
                        MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
                    }
                }
            }
        }

        private void tsmiFiltOnTypeProg_Click(object sender, EventArgs e)
        {
            if (!tsmiFiltOnTypeProg.Checked)
            {
                tsmiFiltOnTypeProg.Checked = true;
                btnFiltGenre.Checked = true;
                toolStripFiltGenre.Visible = true;
            }
            else
            {
                tsmiFiltOnTypeProg.Checked = false;
                btnFiltGenre.Checked = false;
                toolStripFiltGenre.Visible = false;
            }
        }

        private void tsmiFiltOnRating_Click(object sender, EventArgs e)
        {
            if (!tsmiFiltOnRating.Checked)
            {
                tsmiFiltOnRating.Checked = true;
                btnFiltRating.Checked = true;
                toolStripFiltRating.Visible = true;
            }
            else
            {
                tsmiFiltOnRating.Checked = false;
                btnFiltRating.Checked = false;
                toolStripFiltRating.Visible = false;
            }
        }

        private void tsmiFiltOnAnons_Click(object sender, EventArgs e)
        {
            if (!tsmiFiltOnAnons.Checked)
            {
                tsmiFiltOnAnons.Checked = true;
                btnFiltAnons.Checked = true;
                _tvProg.SetGlobalAnonsFilter(true);
            }
            else
            {
                tsmiFiltOnAnons.Checked = false;
                btnFiltAnons.Checked = false;
                _tvProg.SetGlobalAnonsFilter(false);
            }
            UpdateDataGridNow(sender, e, datetime_for_now);
            UpdateDataGridNext(sender, e);
            UpdateDataGridChannels(sender, e);
            UpdateDataGridProgs(sender, e);
        }

        private void tsmiFiltOnRemind_Click(object sender, EventArgs e)
        {
            if (!tsmiFiltOnRemind.Checked)
            {
                tsmiFiltOnRemind.Checked = true;
                btnFiltRemind.Checked = true;
                _tvProg.SetGlobalRemindFilter(true);
            }
            else
            {
                tsmiFiltOnRemind.Checked = false;
                btnFiltRemind.Checked = false;
                _tvProg.SetGlobalRemindFilter(false);
            }
            UpdateDataGridNow(sender, e, datetime_for_now);
            UpdateDataGridNext(sender, e);
            UpdateDataGridChannels(sender, e);
            UpdateDataGridProgs(sender, e);
        }

        private void chkNow_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNow.Checked)
            {
                cbWeekNow.Text = DateTime.Now.ToShortDateString();
                datetime_for_now = DateTimeOffset.Now;
                UpdateDataGridNow(sender, e, datetime_for_now);
            }
            cbWeekNow.Enabled = !chkNow.Checked;
        }

        private void cbWeekNow_TextChanged(object sender, EventArgs e)
        {
            datetime_for_now = DateTimeOffset.Parse(cbWeekNow.Text + " " + DateTime.Now.ToShortTimeString() + " +03:00");
            UpdateDataGridNow(sender, e, datetime_for_now);
        }

        private void trackBarTo_ValueChanged(object sender, EventArgs e)
        {
            if (chkAndTo.Checked)
            {
                TimeSpan tsMinute = new TimeSpan(0, 10 * (144 - trackBarTo.Value), 0);
                _tsFinalTo = _tsTo.Add(tsMinute);
                chkAndTo.Text = Resources.AndToText.Replace("...", " ") +
                                String.Format(String.Format("{0:D2}:{1:D2}", _tsFinalTo.Hours, _tsFinalTo.Minutes));
                double nowValue =
                    (new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0) - new TimeSpan(5, 0, 0)).TotalMinutes / 10;
                if ((144 - trackBarTo.Value) < nowValue) trackBarTo.Value = 144 - (int)nowValue;
                toolTip1.SetToolTip(trackBarTo,
                                    Resources.AndToText.Replace("...", " ") +
                                    String.Format("{0:D2}:{1:D2}", _tsFinalTo.Hours, _tsFinalTo.Minutes));
                datetime_for_next = new DateTimeOffset(Convert.ToDateTime(cbWeekNext.Text) + _tsFinalTo);
                UpdateDataGridNext(sender, e);
            }
        }

        private void chkAndTo_CheckedChanged(object sender, EventArgs e)
        {
            trackBarTo.Visible = chkAndTo.Checked;
            if (!chkAndTo.Checked)
            {
                chkAndTo.Text = Resources.AndToText + " ";
                datetime_for_next = new DateTimeOffset(new DateTime(1800, 1, 1)); ;
                UpdateDataGridNext(sender, e);
            }
        }

        /// <summary>
        /// Если присутствуют анонсы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiIfitis_Click(object sender, EventArgs e)
        {
            Preferences.showAnons = Preferences.ShowAnons.Ifitis;
            Settings.Default.AnonsMode = "Ifitis";
            Settings.Default.Save();
            tsmiAlways.Checked = false;
            tsmiNever.Checked = false;
        }
        /// <summary>
        /// Всегда присутствуют анонсы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAlways_Click(object sender, EventArgs e)
        {
            Preferences.showAnons = Preferences.ShowAnons.Always;
            Settings.Default.AnonsMode = "Always";
            Settings.Default.Save();
            tsmiIfitis.Checked = false;
            tsmiNever.Checked = false;
            pAnons.Visible = true;
        }
        /// <summary>
        /// Анонсы никогда не присутствуют
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiNever_Click(object sender, EventArgs e)
        {
            Preferences.showAnons = Preferences.ShowAnons.Never;
            Settings.Default.AnonsMode = "Never";
            Settings.Default.Save();
            tsmiIfitis.Checked = false;
            tsmiAlways.Checked = false;
            pAnons.Visible = false;
        }

        /// <summary>
        /// Когда все строки добавлены в датагрид
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgNow_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (_dgNextColSorted != null)
            {
                if (_nextSortOrder == SortOrder.Ascending)
                {
                    _nextDirection = ListSortDirection.Ascending;
                }
                else if (_nextSortOrder == SortOrder.Descending)
                {
                    _nextDirection = ListSortDirection.Descending;
                }
                else _nextDirection = ListSortDirection.Ascending;
                dgNext.Sort(_dgNextColSorted, _nextDirection);
            }
            if (dgNext.Rows.Count > 0)
            {
                try
                {
                    dgNext.Rows[0].Selected = false;
                    dgNext.Rows[_curNextRowIndex].Selected = true;
                    dgNext.CurrentCell = dgNext.Rows[_curNextRowIndex].Cells["colNextTitle"];
                    dgNext_SelectionChanged(sender, e);
                }
                catch (Exception ex)
                {
                    Statics.EL.LogException(ex);
                }
            }
            this.dgNext.SelectionChanged += new EventHandler(this.dgNext_SelectionChanged);
            foreach (DataGridViewRow dgvr in dgNext.Rows)
            {
                if (dgvr.Cells["colNextCategory"].Value != null)
                {
                    dgvr.Cells["colNextGenre"].ToolTipText =
                       !String.IsNullOrEmpty(dgvr.Cells["colNextCategory"].Value.ToString()) ?
                       dgvr.Cells["colNextCategory"].Value.ToString() : Resources.WithoutTypeText;
                }
                if (dgvr.Cells["colNextFavName"].Value != null)
                {
                    dgvr.Cells["colNextRating"].ToolTipText =
                       !String.IsNullOrEmpty(dgvr.Cells["colNextFavName"].Value.ToString()) ?
                       dgvr.Cells["colNextFavName"].Value.ToString() : Resources.WithoutRatingText;
                }
            }
            for (int index = e.RowIndex; index < e.RowIndex + e.RowCount; index++)
            {
                (sender as DataGridViewExt).Rows[index].ContextMenuStrip = contextMenuTables;
            } 
        }

        /// <summary>
        /// Отмечать строку и при щелчке на правую кнопку мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgNow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (sender is DataGridViewExt)
                {
                    DataGridViewExt.HitTestInfo hti;
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
        /// КМ строки таблицы: копировать в буфер обмена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiCopyToBuffer_Click(object sender, EventArgs e)
        {
            switch (tabMain.SelectedIndex)
            {
                // Сейчас:
                case 0:
                    if (dgNow.CurrentRow != null)
                    {

                        Clipboard.SetText(dgNow.CurrentRow.Cells["colNameChannel"].Value +
                            String.Format(" {0:t} — {1:t} ", (DateTimeOffset)dgNow.CurrentRow.Cells["colFrom"].Value,
                            (DateTimeOffset)dgNow.CurrentRow.Cells["colTo"].Value) + dgNow.CurrentRow.Cells["colTelecastTitle"].Value);
                    }
                    break;
                case 1:
                    if (dgNext.CurrentRow != null)
                    {
                        Clipboard.SetText(dgNext.CurrentRow.Cells["colNextChannel"].Value.ToString() +
                            string.Format(" {0:t} — {1:t} ", (DateTimeOffset)dgNext.CurrentRow.Cells["colNextFrom"].Value,
                            (DateTimeOffset)dgNext.CurrentRow.Cells["colNextTo"].Value) + dgNext.CurrentRow.Cells["colNextTitle"].Value);
                    }
                    break;
                case 2:
                    if (dgChannels.CurrentRow != null)
                    {
                        Clipboard.SetText(_dsTvProg.Tables["AllChannels"].Select("id = " +
                            dgChannels.CurrentRow.Cells["colChanId"].Value)[0]["display-name"].ToString() +
                            string.Format(" {0} — {1} ", dgChannels.CurrentRow.Cells["colChanFrom"].Value,
                            dgChannels.CurrentRow.Cells["colChanTo"].Value) + dgChannels.CurrentRow.Cells["colChanProg"].Value);
                    }
                    break;
                case 3:
                    if (dgProgs.CurrentRow != null)
                    {
                        Clipboard.SetText(_dsTvProg.Tables["AllChannels"].Select("id = " +
                            dgProgs.CurrentRow.Cells["colChannelId"].Value)[0]["display-name"].ToString() +
                            string.Format(" {0} — {1} ", dgProgs.CurrentRow.Cells["colChannelFrom"].Value,
                            dgProgs.CurrentRow.Cells["colChannelTo"].Value) + dgProgs.CurrentRow.Cells["colChannelTitle"].Value);
                    }
                    break;
            }
        }

        /// <summary>
        /// КМ строки таблицы: открыть диалог поиска по названию передачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiSearch_Click(object sender, EventArgs e)
        {
            switch (tabMain.SelectedIndex)
            {
                case 0:
                    if (dgNow.CurrentRow != null)
                    {
                        _schForm = new SearchDialog(_tvProg, dgNow.CurrentRow.Cells["colTelecastTitle"].Value.ToString());
                        _schForm.ShowDialog(this);
                    }
                    break;
                case 1:
                    if (dgNext.CurrentRow != null)
                    {
                        _schForm = new SearchDialog(_tvProg, dgNext.CurrentRow.Cells["colNextTitle"].Value.ToString());
                        _schForm.ShowDialog(this);
                    }
                    break;
                case 2:
                    if (dgChannels.CurrentRow != null)
                    {
                        _schForm = new SearchDialog(_tvProg, dgChannels.CurrentRow.Cells["colChanProg"].Value.ToString());
                        _schForm.ShowDialog(this);
                    }
                    break;
                case 3:
                    if (dgProgs.CurrentRow != null)
                    {
                        _schForm = new SearchDialog(_tvProg,
                                                    dgProgs.CurrentRow.Cells["colChannelTitle"].Value.ToString());
                        _schForm.ShowDialog(this);
                    }
                    break;
            }
        }

        /// <summary>
        /// КМ строки таблицы: добавить к любимым
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiAddToFavorite_Click(object sender, EventArgs e)
        {
            string txtFavorite = String.Empty;
            switch (tabMain.SelectedIndex)
            {
                case 0:
                    if (dgNow.CurrentRow != null)
                    {
                        txtFavorite = dgNow.CurrentRow.Cells["colTelecastTitle"].Value.ToString();
                    }
                    break;
                case 1:
                    if (dgNext.CurrentRow != null)
                    {
                        txtFavorite = dgNext.CurrentRow.Cells["colNextTitle"].Value.ToString();
                    }
                    break;
                case 2:
                    if (dgChannels.CurrentRow != null)
                    {
                        txtFavorite = dgChannels.CurrentRow.Cells["colChanProg"].Value.ToString();
                    }
                    break;
                case 3:
                    if (dgProgs.CurrentRow != null)
                    {
                        txtFavorite = dgProgs.CurrentRow.Cells["colChannelTitle"].Value.ToString();
                    }
                    break;
            }
            ClassifFavoriteDialog clFavoriteDialog = new ClassifFavoriteDialog(Preferences.favorites, txtFavorite);
            if (clFavoriteDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (!Preferences.favorites.ClassifTable.Columns.Contains("deleteafter"))
                {
                    Preferences.favorites.ClassifTable.Columns.Add("deleteafter", typeof(DateTime));
                }
                if (clFavoriteDialog.Edit)
                { // Изменение любимости передачи
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
                { // Добавление к любимым
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
                var thread1 = new Thread(CheckAndSetRemindAndFavorite)
                {
                    Name = "ChekAndSetRemindAndFavorite",
                    Priority = ThreadPriority.Lowest
                };
                thread1.Start();
            }
        }

        /// <summary>
        /// КМ строки таблицы: добавить в классификатор жанров передач
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiAddToGenres_Click(object sender, EventArgs e)
        {
            string txtGenre = String.Empty;
            switch (tabMain.SelectedIndex)
            {
                case 0:
                    if (dgNow.CurrentRow != null)
                    {
                        txtGenre = dgNow.CurrentRow.Cells["colTelecastTitle"].Value.ToString();
                    }
                    break;
                case 1:
                    if (dgNext.CurrentRow != null)
                    {
                        txtGenre = dgNext.CurrentRow.Cells["colNextTitle"].Value.ToString();
                    }
                    break;
                case 2:
                    if (dgChannels.CurrentRow != null)
                    {
                        txtGenre = dgChannels.CurrentRow.Cells["colChanProg"].Value.ToString();
                    }
                    break;
                case 3:
                    if (dgProgs.CurrentRow != null)
                    {
                        txtGenre = dgProgs.CurrentRow.Cells["colChannelTitle"].Value.ToString();
                    }
                    break;
            }
            ClassifGenreDialog clGenreDialog = new ClassifGenreDialog(Preferences.genres, txtGenre);
            if (clGenreDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (clGenreDialog.Edit)
                { // Редактирование имеющегося класса:
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
                { // Добавление в классификатор:
                    if (!Preferences.genres.ClassifTable.Columns.Contains("deleteafter"))
                    {
                        Preferences.genres.ClassifTable.Columns.Add("deleteafter", typeof(DateTime));
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
                };
                string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifGenres);
                DataTable dtSerial = Preferences.genres.ClassifTable.Copy();
                dtSerial.Columns.Remove("id");
                dtSerial.Columns.Remove("image");
                dtSerial.WriteXml(xmlPath);
                _dsTvProg.Tables.Remove("KeywordsTable");
                DataTable dtClassifGenres = Preferences.genres.ClassifTable.Rows.Count > 0
                                              ? Preferences.genres.ClassifTable.Select("", "prior DESC").CopyToDataTable()
                                              : new DataTable("KeywordsTable");
                dtClassifGenres.TableName = "KeywordsTable";
                _dsTvProg.Tables.Add(dtClassifGenres);
            }
        }

        /// <summary>
        /// КМ строки таблицы: просмотреть свойства канала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiPropChannel_Click(object sender, EventArgs e)
        {
            string strCID = String.Empty;
            switch (tabMain.SelectedIndex)
            {
                case 0:
                    if (dgNow.CurrentRow != null)
                    {
                        strCID = dgNow.CurrentRow.Cells["colCID"].Value.ToString();
                    }
                    break;
                case 1:
                    if (dgNext.CurrentRow != null)
                    {
                        strCID = dgNext.CurrentRow.Cells["colNextCID"].Value.ToString();
                    }
                    break;
                case 2:
                    if (dgChannels.CurrentRow != null)
                    {
                        strCID = dgChannels.CurrentRow.Cells["colChanId"].Value.ToString();
                    }
                    break;
                case 3:
                    if (dgProgs.CurrentRow != null)
                    {
                        strCID = dgProgs.CurrentRow.Cells["colChannelId"].Value.ToString();
                    }
                    break;
            }
            DataRow[] drsChan = _dsTvProg.Tables["AllChannels"].Select("id = " + strCID);
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
                        XPathNodeIterator itr = xNav.Select(String.Format("/Root/channel[@id='{0}']", strCID));
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
                    ShowData();
                }
            }
        }
        /// <summary>
        /// КМ строки таблицы: просмотр канала...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewChannel_Click(object sender, EventArgs e)
        {
            string idChannel = String.Empty;
            switch (tabMain.SelectedIndex)
            {
                case 0:
                   if (dgNow.CurrentRow != null)
                   {
                       idChannel = dgNow.CurrentRow.Cells["colCID"].Value.ToString();
                   }
                    break;
                case 1:
                    if (dgNext.CurrentRow != null)
                    {
                        idChannel = dgNext.CurrentRow.Cells["colNextCID"].Value.ToString();
                    }
                    break;
                case 2:
                    if (dgChannels.CurrentRow != null)
                    {
                        idChannel = dgChannels.CurrentRow.Cells["colChanId"].Value.ToString();
                    }
                    break;
                case 3:
                    if (dgProgs.CurrentRow != null)
                    {
                        idChannel = dgProgs.CurrentRow.Cells["colChannelId"].Value.ToString();
                    }
                    break;
            }
            if (idChannel != String.Empty)
            {
                DataRow drFoundChannel = _dsTvProg.Tables["AllChannels"].Rows.Find(idChannel);
                if (drFoundChannel != null)
                {
                    VideoDialog videoDlg = new VideoDialog(_capture, _dsTvProg.Tables["AllChannels"], int.Parse(drFoundChannel["freq"].ToString()));
                    videoDlg.ShowDialog(this);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_bClose)
            {
                if (e.CloseReason != CloseReason.WindowsShutDown)
                {
                    e.Cancel = true;
                    Hide();
                }
                else e.Cancel = false;
            }
            else e.Cancel = false;
        }

        private void nIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.WindowState = FormWindowState.Normal;
                this.Show();
            }
        }
        
        
        /// <summary>
        /// Показать напоминатель
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiCurrentReminds_Click(object sender, EventArgs e)
        {
            DataTable tblCurRemind = new DataTable("CurRemindTable");
            tblCurRemind = _tvProg.GetEventInFuture(_dsTvProg, "remind");
            if (tblCurRemind.Rows.Count > 0)
            {
                RemindDialog remDlg = new RemindDialog(_tvProg, tblCurRemind, Resources.CurrentRemindsText);
                remDlg.ShowDialog(this);
            }
            else
            {
                Statics.ShowDialog(Resources.InformationText, Resources.NoSettersRemindsText, MessageDialog.MessageIcon.Info,
                                   MessageDialog.MessageButtons.Ok);
            }
        }

        /// <summary>
        /// Показать напоминатель
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiCurrentRecords_Click(object sender, EventArgs e)
        {
            DataTable tblCurRecord = new DataTable("CurRecordTable");
            tblCurRecord = _tvProg.GetEventInFuture(_dsTvProg, "record");
            if (tblCurRecord.Rows.Count > 0)
            {
                RemindDialog remDlg = new RemindDialog(_tvProg, tblCurRecord, Resources.PlannedRecsText);
                remDlg.ShowDialog(this);
            }
            else
            {
                Statics.ShowDialog(Resources.InformationText, Resources.NoSettersRecordsText, MessageDialog.MessageIcon.Info,
                                   MessageDialog.MessageButtons.Ok);
            }
        }
        
        /// <summary>
        /// КМ иконки в трее: развернуть окно с приложением
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiOpenTVProgViewer_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }
        
        /// <summary>
        /// КМ иконки в трее: вкл/откл звук
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiVolume_CheckedChanged(object sender, EventArgs e)
        {
            Reminder.Default.OnSound = csmiVolume.Checked;
            Reminder.Default.Save();
            SetRemindVolume();
        }
      
        /// <summary>
       /// КМ иконки в трее: вкл/откл сообщения
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       private void csmiMessage_CheckedChanged(object sender, EventArgs e)
       {
           Reminder.Default.Message = csmiMessage.Checked;
           Reminder.Default.Save();
           SetRemindMessage();
       }

        /// <summary>
        /// КМ иконки в трее: вкл/откл напоминателя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void csmiCheckRemind_CheckedChanged(object sender, EventArgs e)
       {
           Preferences.statusReminder = csmiCheckRemind.Checked;
           SetRemindStatus();
       }

       private void lblImageVolume_DoubleClick(object sender, EventArgs e)
       {
           Reminder.Default.OnSound = !Reminder.Default.OnSound;
           Reminder.Default.Save();
           SetRemindVolume();
       }

       private void lblImageMessageDlg_DoubleClick(object sender, EventArgs e)
       {
           Reminder.Default.Message = Reminder.Default.Message;
           Reminder.Default.Save();
           SetRemindMessage();
       }
       
       private void btnExportXsl_Click(object sender, EventArgs e)
       {
           string strExp = String.Empty;
           DataSet expDs = new DataSet("ExportDataSet");
           expDs.Tables.Clear();
           switch (tabMain.SelectedIndex)
           {
               case 0:
                   if (dgNow.DataSource != null)
                   {
                       strExp = Resources.NowText;
                       expDs.Tables.Add(dgNow.DataSource as DataTable);
                   }
                   break;
               case 1:
                   if (dgNext.DataSource != null)
                   {
                       strExp = Resources.NextText;
                       expDs.Tables.Add(dgNext.DataSource as DataTable);
                   }
                   break;
               case 2:
                   if (dgChannels.DataSource != null)
                   {
                       strExp = lblName.Text + "_" + lblDate.Text;
                       expDs.Tables.Add(dgChannels.DataSource as DataTable);
                   }
                   break;
               case 3:
                   if (dgProgs.DataSource != null)
                   {
                       strExp = lblName2.Text + "_" + lblDate2.Text;
                       expDs.Tables.Add(dgProgs.DataSource as DataTable);
                   }
                   break;
           }
           ExcelExport exp = new ExcelExport(expDs, _exp_folder + "exp_template1.xls" );
           if (exp != null)
           {
               ExportBuilder.Save(exp.BuildWorkBook(), String.Format(Resources.ExportFileName, strExp));
           }
       }
       
        
        /// <summary>
        /// Общие настройки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOptions_Click(object sender, EventArgs e)
        {
            if (_optDlg.ShowDialog(this) == DialogResult.OK)
            {
                Settings.Default.Save();
                TunerSettings.Default.Save();
                Reminder.Default.Save();
            }else
            {
                Settings.Default.Reload();
                TunerSettings.Default.Reload();
                Reminder.Default.Reload();
            }
            tvWeeks.Nodes.Clear();
            FillImageList();
            tvWeeks.Nodes.Clear();
            if (TVEnvironment.currentUser == null)
                CreateTree(TVEnvironment.systemChannelList.ToList<IChannel>(), GroupingBy.Dates);
            else
                CreateTree(_chList.OrderBy(s => s.OrderCol).ToList<IChannel>(), GroupingBy.Dates);


            tvChannels.Nodes.Clear();
            if (TVEnvironment.currentUser == null)
                CreateTree(TVEnvironment.systemChannelList.ToList<IChannel>(), GroupingBy.Channels);
            else
                CreateTree(_chList.OrderBy(s => s.OrderCol).ToList<IChannel>(), GroupingBy.Channels);
            SetStylesAndSettings();
            RegistryKey hkcuAutorun =
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            try
            {
                string path = Application.StartupPath + @"\TVProgViewer.TVProgApp.exe";
                if (Settings.Default.Autorun)
                {
                    hkcuAutorun.SetValue("TVProgViewer.TVProgApp", path, RegistryValueKind.String);
                }
                else
                {
                    hkcuAutorun.DeleteValue("TVProgViewer.TVProgApp");
                }
            }
            catch (Exception ex)
            {
               Statics.EL.LogException(ex);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_capture != null)) _capture.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void btnPropTvTuner_Click(object sender, EventArgs e)
        {
            VideoDialog vd = new VideoDialog(_capture, _dsTvProg.Tables["AllChannels"]);
            vd.ShowDialog(this);
        }

        private void tsmMainMenu_Click(object sender, EventArgs e)
        {
            Settings.Default.FlagMainMenu = menuStrip.Visible = tsmMainMenu.Checked;
            Settings.Default.Save();
        }
        
        private void tsmToolStrip_Click(object sender, EventArgs e)
        {
            Settings.Default.FlagToolStrip = toolStrip.Visible = tsmToolStrip.Checked;
            Settings.Default.Save();
        }

        private void tsmStatusStrip_Click(object sender, EventArgs e)
        {
            Settings.Default.FlagStatusStrip = statusStrip.Visible = tsmStatusStrip.Checked;
            Settings.Default.Save();
        }
        /// <summary>
        /// Изменение локализации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiRussian_Click(object sender, EventArgs e)
        {
            Settings.Default.LocalizationSetting = "ru-RU";
            MessageBox.Show("Изменения вступят в силу после следующей загрузки", "Инфо", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
            Settings.Default.Save();
        }
   
        private void tsmiEnglish_Click(object sender, EventArgs e)
        {
            Settings.Default.LocalizationSetting = "en-US";
            MessageBox.Show("Changes will take effect after next load application", "Info", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
            Settings.Default.Save();
        }
        /// <summary>
        /// Открыть файл с телепрограммой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            string xmlTVFileName1 = Path.Combine(Application.StartupPath, Preferences.xmlFileName1);
            if (openProgDlg.ShowDialog() == DialogResult.OK)
            {
                if (openProgDlg.FilterIndex == 1) // - Это программа формата Интер-тв
                {
                    _tvProg.InterToXml(openProgDlg.FileName, ref xmlTVFileName1);
                    _tvProg.MergeXmls(xmlTVFileName1);
                    LoadDataSetAndProg();
                    this.Cursor = Cursors.Default;
                    // Создать поток для установки напоминаний и любимых:
                    var threadSetter = new Thread(CheckAndSetRemindAndFavorite)
                    {
                        Name = "ChekAndSetRemindAndFavorite2",
                        Priority = ThreadPriority.Lowest
                    };
                    threadSetter.Start();   // - запустить поток
                }
                if (openProgDlg.FilterIndex == 2) // - это программа формата XMLTV
                {
                    byte[] bytes = File.ReadAllBytes(openProgDlg.FileName);
                    File.WriteAllBytes(xmlTVFileName1, bytes);
                    _tvProg.MergeXmls(xmlTVFileName1);
                    _tvProg = new TVProgClass();
                    LoadDataSetAndProg();
                    this.Cursor = Cursors.Default;
                    // Создать поток для установки напоминаний и любимых:
                    var threadSetter = new Thread(CheckAndSetRemindAndFavorite)
                    {
                        Name = "ChekAndSetRemindAndFavorite2",
                        Priority = ThreadPriority.Lowest
                    };
                    threadSetter.Start();   // - запустить поток
                }
            }
        }

        /// <summary>
        /// Вызов справки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiHelp_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "Help.chm");
            Process.Start(path);
        }

        private void tsmiRegistration_Click(object sender, EventArgs e)
        {
            reg.ShowDialog();
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            aboutForm.ShowDialog(this);
        }
        #endregion

        
    }
}    
             