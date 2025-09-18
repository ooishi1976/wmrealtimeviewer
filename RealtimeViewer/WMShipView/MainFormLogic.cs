using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MpgCommon;
using MpgCustom;
using MpgMap;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RealtimeViewer.Controls;
using RealtimeViewer.Logger;
using RealtimeViewer.Map;
using RealtimeViewer.Model;
using RealtimeViewer.Movie;
using RealtimeViewer.Network;
using RealtimeViewer.Network.Mqtt;
using RealtimeViewer.Setting;


namespace RealtimeViewer.WMShipView
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// コンフィグタブ(TabPage.Name)
        /// </summary>
        private const string TabPageConfigName = "tabPageConfig";

        /// <summary>
        /// コンフィグタブ(TabPage.Text)
        /// </summary>
        private const string tabPageConfigText = "コンフィグ";

        private OperationLogger OperationLogger => ViewModel.OperationLogger;

        /// <summary>
        /// INIファイル情報
        /// </summary>
        private SettingIni LocalSettings => ViewModel.LocalSettings;

        private OperationServerInfo OperationServerInfo => ViewModel.OperationServerInfo;

        private MainViewModel ViewModel { get; set; } = new MainViewModel();

        private CancellationTokenSource MovieCancellationTokenSource { get; set; } = null;

        /// <summary>
        /// イベント関連のキャンセルトークンソース
        /// </summary>
        private CancellationTokenSource CancellationTokenSource { get; set; } = null;

        private FfmpegCtrl FfmpegCtrl { get; set; } = new FfmpegCtrl();

        private WaitingForm　WaitForm { get; set; }

        private WaitFormViewModel WaitFormViewModel { get; set; }

        private EventAlertWindow AlertForm { get; set; }

        /// <summary>
        /// USB監視
        /// </summary>
        private DeviceWatcher deviceWatcher;

        private ConfigPanel configPanel;

        private BindingSource DeviceBindingSource { get; set; } = new BindingSource();

        private BindingSource OfficeBindingSource { get; set; } = new BindingSource();

        private BindingSource EventListBindingSource { get; set; } = new BindingSource();

        private List<TextObject> MapSymbolList { get; set; } = new List<TextObject>();

        private Dictionary<string, MovieRequestWindow> MovieRequestWindows { get; set; } 

        private Dictionary<string, RemoteSetting> RemoteSettingWindows { get; set; }

        /// <summary>
        /// イベントタブ選択済み(項目バインドのためのフラグ)
        /// </summary>
        private bool IsEventTabBinded { get; set; } = false;

        private delegate void WaitFormCloseDelegate();

        /// <summary>
        /// 最初に表示されている画面項目のバインド
        /// </summary>
        private void BindViewModel()
        {
            // 指紋認証済み
            // タブパネルと車両リストの操作を許可する
            tabControlMain.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.IsUserAuthCompleted));
            gridCarList.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.IsUserAuthCompleted));
            // ユーザが営業所操作権があれば、操作を許可する
            comboBoxOffice.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.IsOfficeEditable));
            // ユーザ名
            labelUserName.DataBindings.Add("Text", ViewModel, nameof(ViewModel.UserName), true, DataSourceUpdateMode.OnPropertyChanged);
            // 初期データ取得中・・・
            labelUpdateDate.DataBindings.Add("Text", ViewModel, nameof(ViewModel.DataUpdateDate));
            // サイドパネル
            labelRtCarId.DataBindings.Add("Text", ViewModel, nameof(ViewModel.SelectedDeviceName));
            labelRtCarStatus.DataBindings.Add("Text", ViewModel, nameof(ViewModel.SelectedDeviceErrorStr));
            tableLayoutPanelCarInfo.DataBindings.Add("BackColor", ViewModel, nameof(ViewModel.SelectedDeviceBackColor));
            tabPageRT.DataBindings.Add("BackColor", ViewModel, nameof(ViewModel.SelectedDeviceBackColor));
            tabPageEvent.DataBindings.Add("BackColor", ViewModel, nameof(ViewModel.SelectedDeviceBackColor));
            tabPageRemoteConfig.DataBindings.Add("BackColor", ViewModel, nameof(ViewModel.SelectedDeviceBackColor));
        }

        private void BindStreamingDataSource()
        {
            var streamingStatus = ViewModel.StreamingStatus;
            progressBarRtStart.DataBindings.Add("Visible", streamingStatus, nameof(streamingStatus.IsWaitToReady));
            labelCarStreamElapsed.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.Elapsed));
            labelCarStreamFrames.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.Frames));
            labelCarStreamBytes.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.Bytes));
            labelCarStreamFps.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.Fps));
            labelCarStreamKbps.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.Kbps));
            labelCarStreamDrops.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.Dropped));
            labelCarStreamSpeed.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.Speed));
            tableLayoutPanelStreamingOnCar.DataBindings.Add("Visible", streamingStatus, nameof(streamingStatus.IsWaitOrPlaying));
            labelRtStatus.DataBindings.Add("Visible", streamingStatus, nameof(streamingStatus.IsDisplayMessage));
            labelRtRetryStatus.DataBindings.Add("Visible", streamingStatus, nameof(streamingStatus.IsDisplayMessage));
            labelRtStatus.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.StatusText1));
            labelRtRetryStatus.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.StatusText2));
            labelRtElapsed.DataBindings.Add("Text", streamingStatus, nameof(streamingStatus.PlayCounter));

            radioButtonRtCh1.DataBindings.Add("Enabled", streamingStatus, nameof(streamingStatus.IsPlaying));
            radioButtonRtCh2.DataBindings.Add("Enabled", streamingStatus, nameof(streamingStatus.IsPlaying));
            radioButtonRtCh3.DataBindings.Add("Enabled", streamingStatus, nameof(streamingStatus.IsPlaying));
            radioButtonRtCh4.DataBindings.Add("Enabled", streamingStatus, nameof(streamingStatus.IsPlaying));
            radioButtonRtCh5.DataBindings.Add("Enabled", streamingStatus, nameof(streamingStatus.IsPlaying));
            radioButtonRtCh6.DataBindings.Add("Enabled", streamingStatus, nameof(streamingStatus.IsPlaying));
            radioButtonRtCh7.DataBindings.Add("Enabled", streamingStatus, nameof(streamingStatus.IsPlaying));
            radioButtonRtCh8.DataBindings.Add("Enabled", streamingStatus, nameof(streamingStatus.IsPlaying));
            streamingStatus.PropertyChanged += StreamingStatus_PropertyChanged;
        }

        /// <summary>
        /// イベントパネルの画面項目のバインド
        /// </summary>
        private void BindEventTab()
        {
            // イベントリストテーブルレイアウト
            tableLayoutPanelEventList.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.IsEnableEventTable));

            // ダウンロード
            progressBarDownload.DataBindings.Add("Visible", ViewModel, nameof(ViewModel.IsDownloadingMovie));
            labelDownloadStatus.DataBindings.Add("Visible", ViewModel, nameof(ViewModel.IsDownloadingMovie));
            
            // イベント更新
            progressBarEventListUpdate.DataBindings.Add("Visible", ViewModel, nameof(ViewModel.IsUpdatingEventList));
            buttonEventListUpdate.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanUpdateEvent));

            // Gダウンロード/キャンセル
            labelGstatus.DataBindings.Add("Visible", ViewModel, nameof(ViewModel.IsDownloadingG));
            progressBarG.DataBindings.Add("Visible", ViewModel, nameof(ViewModel.IsDownloadingG));
            buttonGetG.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanDownloadG));
            buttonCancelG.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.IsDownloadingG));

            // 動画再生
            panelPlay.DataBindings.Add("Visible", ViewModel, nameof(ViewModel.CanPlayMovie));
            labelPanelPlayCarId.DataBindings.Add("Text", ViewModel, nameof(ViewModel.PlayDeviceName));
            labelPanelPlayDate.DataBindings.Add("Text", ViewModel, nameof(ViewModel.PlayTimestampStr));
            labelEventProc.DataBindings.Add("Text", ViewModel, nameof(ViewModel.PlayMessage));
            buttonPlay1.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanPlayCh1));
            buttonPlay2.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanPlayCh2));
            buttonPlay3.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanPlayCh3));
            buttonPlay4.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanPlayCh4));
            buttonPlay5.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanPlayCh5));
            buttonPlay6.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanPlayCh6));
            buttonPlay7.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanPlayCh7));
            buttonPlay8.DataBindings.Add("Enabled", ViewModel, nameof(ViewModel.CanPlayCh8));
            FfmpegCtrl.MovieProgress += FfmpegCtrl_MovieProgress;
        }

        /// <summary>
        /// 営業所選択、車両リスト取得後のComboBox, DataGridViewへのバインド
        /// </summary>
        private void BindDeviceDataSource()
        {
            // 車両リスト
            if (0 < ViewModel.DeviceTable.Rows.Count) 
            {
                var currentOffice = OfficeBindingSource.Current;

                DeviceBindingSource.DataSource = ViewModel.WMDataSet;
                DeviceBindingSource.DataMember = "Device";
                DeviceBindingSource.Filter = $"OfficeId = -1";
                DeviceBindingSource.Sort = "DeviceId";
                DeviceBindingSource.CurrentChanged += DeviceBindingSource_CurrentChanged;
                gridCarList.Enabled = true;
                gridCarList.AutoGenerateColumns = false;
                gridCarList.DataSource = DeviceBindingSource;
            }

            // 営業所リスト
            if (0 < ViewModel.OfficeTable.Rows.Count) 
            {
                OfficeBindingSource.DataSource = ViewModel.WMDataSet;
                OfficeBindingSource.DataMember = "Office";
                OfficeBindingSource.Filter = "Visible = true";
                OfficeBindingSource.Sort = "OfficeId";
                //OfficeBindingSource.CurrentChanged += ComboBoxOffice_SelectedValueChanged;
                comboBoxOffice.DisplayMember = "Name";
                comboBoxOffice.ValueMember = "OfficeId";
                comboBoxOffice.SelectedValueChanged += ComboBoxOffice_SelectedValueChanged;
                comboBoxOffice.DataSource = OfficeBindingSource;
            }
        }

        /// <summary>
        /// イベントリスト取得後のDataGridViewへのバインド
        /// </summary>
        /// <param name="eventList"></param>
        private void BindEventDataSource(WMDataSet.EventListDataTable eventList)
        {
            ViewModel.EventTable = eventList;
            ViewModel.PlaylistTable = new WMDataSet.PlayListDataTable();
            EventListBindingSource.DataSource = eventList;
            EventListBindingSource.Sort = "Timestamp desc, DeviceId";
            EventListBindingSource.Filter = "MovieType <> 2";
            gridEventList.AutoGenerateColumns = false;
            gridEventList.DataSource = EventListBindingSource;
            if (EventListBindingSource.List is DataView dataView)
            {
                ViewModel.FilteredEventTable = dataView;
            }
        }

        private void UnbindEventDataSource()
        {
            gridEventList.DataSource = null;
            ViewModel.EventTable = null;
            ViewModel.PlaylistTable = null;
        }

        private void SetOffices(WMDataSet.OfficeDataTable offices)
        {
            foreach (var office in offices)
            {
                var row = ViewModel.OfficeTable.NewOfficeRow();
                row.OfficeId = office.OfficeId;
                row.CompanyId = office.CompanyId;
                row.Name = office.Name;
                row.Visible = office.Visible;
                row.Latitude = office.Latitude;
                row.Longitude = office.Longitude;
                ViewModel.OfficeTable.AddOfficeRow(row);
            }
            ViewModel.OfficeTable.AcceptChanges();
        }

        private void SetDevices(WMDataSet.DeviceDataTable devices)
        {
            foreach (var device in devices)
            {
                var row = ViewModel.DeviceTable.NewDeviceRow();
                row.OfficeId = device.OfficeId;
                row.DeviceId = device.DeviceId;
                row.CarId = device.CarId;
                row.CarNumber = device.CarNumber;
                ViewModel.DeviceTable.AddDeviceRow(row);
            }
            ViewModel.DeviceTable.AcceptChanges();
        }

        #region 地図
        /// <summary>
        /// 地図の初期化<br/>
        /// <ul>
        ///   <li>DPI変更</li>
        ///   <li>ズームバースケール設定</li>
        ///   <li>マウス右ボタンの回転禁止</li>
        /// </ul>
        /// </summary>
        private void InitMapScale(int mappingScale)
        {
            PointF dpi = GetDPI();
            mpgMap.SetDPI(dpi.X, dpi.Y);
            //m_customProperty.Icon = new Bitmap(GetType(), "bus.png");
            // m_customProperty.Icon = GetMarkerBitmap();  
            mpgMap.MouseWheel += new MouseEventHandler(MpgMap_MouseWheel);
            ChangeMapMode(MapMode.Move);

            // ズームバー設定
            zoomBar.Minimum = 0;
            zoomBar.Maximum = ViewModel.MapScales.Length - 1;
            zoomBar.Value = ViewModel.GetNearIndexInMapScales(mpgMap.MapScale);
            zoomBar.TickFrequency = 1;
            zoomBar.TickStyle = TickStyle.Both;

            // マウスの右ボタンでの回転禁止
            mpgMap.MapRotateButton = MouseButtons.None;
            mpgMap.MapScale = mappingScale;
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private PointF GetDPI()
        {
            PointF dpi = new PointF();
            Graphics g = this.CreateGraphics();

            // ★画面DPIの制御を行う。(SMDと同じ処理)
            var hDC = g.GetHdc();
            var horzSize = GetDeviceCaps(hDC, 4);   //  4:= HORZSIZE物理画面の幅(ミリメートル単位)
            var vertSize = GetDeviceCaps(hDC, 6);   //  6:= VERTSIZE物理画面の高さ(ミリメートル単位)
            var horzS = GetDeviceCaps(hDC, 8);  //  8:= HORZRES画面の幅(ピクセル単位)
            var vertS = GetDeviceCaps(hDC, 10); // 10:= VERTRES画面の高さ(ピクセル単位)
            g.ReleaseHdc(hDC);

            dpi.X = (float)horzS * 25.4f / (float)horzSize; // (float)g.DpiX, 120.0f, 96.0f, 72.0fなど ※72dpi にするとSMDとサイズが近い。
            dpi.Y = (float)vertS * 25.4f / (float)vertSize; // (float)g.DpiY, 120.0f, 96.0f, 72.0fなど
            return dpi;
        }

        /// <summary>
        /// 地図操作モードの変更を行う。
        /// </summary>
        /// <param name="mode"></param>
        private void ChangeMapMode(MapMode mode)
        {
            // 地図モードを設定する。
            mpgMap.MapMode = mode;

            // 縮尺表示を同期する。
            SyncMapScale(true);
        }

        /// <summary>
        /// 現在の地図縮尺に応じてUI表示を同期させる。
        /// </summary>
        /// <param name="mode"></param>
        private void SyncMapScale(bool syncZoombar)
        {
            if (syncZoombar)
            {
                zoomBar.Value = ViewModel.GetNearIndexInMapScales(mpgMap.MapScale);
            }
        }

        private void MoveToOfficeLocation()
        {
            var officeRow = ViewModel.OfficeTable.FirstOrDefault(item => item.OfficeId == ViewModel.SelectedOfficeId);
            if (officeRow is WMDataSet.OfficeRow office &&
                office.TryGetLocation(out var location))
            {
                // なんか緯度経度が逆な気がする
                mpgMap.MapCenter = location;
                mpgMap.RePaint();
            }
        }

        private void MoveToDeviceLocation()
        {
            MoveToDeviceLocation(ViewModel.SelectedDeviceId);
        }

        private void MoveToDeviceLocation(string deviceId)
        {
            var deviceRow = ViewModel.DeviceTable.FirstOrDefault(item => item.DeviceId == deviceId);
            if (deviceRow is WMDataSet.DeviceRow device &&
                device.TryGetLocation(out var location))
            {
                mpgMap.MapCenter = location;
                mpgMap.RePaint();
            }
        }

        private void DrawMapEntries()
        {
            var arrayList = new ArrayList();
            //List<TextObject> list;
            if (ViewModel.IsDeviceFocus)
            {
                MapSymbolList = CreateMapEntry();
            }
            else
            {
                MapSymbolList = CreateMapEntries();
            }
            arrayList.AddRange(MapSymbolList);
            mpgMap.SetCustomData(arrayList);

            if (ViewModel.IsDeviceFocus)
            {
                MoveToDeviceLocation();
            }
            else
            {
                mpgMap.RePaint();
            }
        }

        /// <summary>
        /// 運行車両情報の地図オブジェクトを作成する。
        /// </summary>
        /// <param name="carInfo">車両情報</param>
        /// <returns></returns>
        private List<TextObject> CreateMapEntry()
        {
            // 位置が取れない場合は対象外
            var result = new List<TextObject>();
            var target = ViewModel.DeviceTable.FirstOrDefault(item => item.DeviceId == ViewModel.SelectedDeviceId);
            if (target is WMDataSet.DeviceRow device &&
                device.TryGetLocation(out var _))
            {
                result.Add(CreateEntryObject(device));
            }
            return result;
        }

        /// <summary>
        /// 運行車両情報の地図オブジェクトを作成する。
        /// </summary>
        /// <param name="carInfo">車両情報</param>
        /// <returns></returns>
        private List<TextObject> CreateMapEntries()
        {
            var result = new List<TextObject>();
            var target = ViewModel.DeviceTable.Where(item => item.OfficeId == ViewModel.SelectedOfficeId);
            if (target.Any())
            {
                foreach (var device in target)
                {
                    if (string.IsNullOrEmpty(device.Latitude) || string.IsNullOrEmpty(device.Longitude))
                    {
                        continue;
                    }
                    result.Add(CreateEntryObject(device));
                }

            }
            return result;
        }

        private TextObject CreateEntryObject(WMDataSet.DeviceRow device)
        {
            var entryObject = new TextObject
            {
                Title = $"{device.CarNumber}",
                TitleFont = new Font("Meiryo UI", 9, FontStyle.Bold),
                CommentFont = new Font("Meiryo UI", 9),
                pen = new Pen(Color.Orange, 1),
                brush = new SolidBrush(Color.White),
                TextColor = Color.Black,
                ID = device.DeviceId  //  車両ID情報追加
            };

            var comments = new List<string>();

            // 位置取得
            if (device.TryGetLocation(out var position))
            {
                //entryObject.Title = $"{device.CarNumber}\nLon: {device.LongitudeDisp}\nLat: {device.LatitudeDisp}";
                // 座標を設定する
                entryObject.Point = position;
                // 原点の描画を設定する
                entryObject.OriginStyle = MpgCustomEnum.OriginStyleEnum.Icon;

                //  テキスト表示
                entryObject.BallonStyle = MpgCustomEnum.BallonStyleEnum.TitleAndComment;
                entryObject.TextZoom = 2.0f;

                // アイコン
                entryObject.Icon = Properties.Resources.shipBmp;
                entryObject.IconZoom = MpgCustomEnum.IconZoomLevelEnum.Level2; // アイコン拡大率

                // 予め決めた円周上に配置する
                //entryObject.Offset = ViewModel.GetRandomBalloonOffset();
                entryObject.Offset = new Point(0, -50);

                // 再度パネル表示中のエラー更新
                if (device.DeviceId == ViewModel.SelectedDeviceId)
                {
                    ViewModel.SelectedDevice = device;
                }

                comments.Add($"{device.LongitudeDMS}");
                comments.Add($"{device.LatitudeDMS}");
            }

            // エラー
            WMDataSet.ErrorRow errorRow = null;
            lock (ViewModel.ErrorTable)
            {
                errorRow = ViewModel.ErrorTable.FirstOrDefault(item => item.DeviceId == device.DeviceId);
            }
            if (errorRow is WMDataSet.ErrorRow error)
            {
                var code = error.GetCode();
                if (code != 0)
                {
                    entryObject.brush = new SolidBrush(error.GetColor());
                    //entryObject.Comment = error.ErrorStr;
                    comments.Add(error.ErrorStr);
                }

                // 再度パネル表示中のエラー更新
                if (error.DeviceId == ViewModel.SelectedDeviceId)
                {
                    ViewModel.SelectedDeviceError = error;
                }
            }

            if (0 < comments.Count)
            {
                entryObject.Comment = string.Join("\n", comments);
            }
            return entryObject;
        }
        #endregion

        private void LeftPanelShow()
        {
            panelLeft.Enabled = true;
            panelLeft.Width = LEFT_PANEL_WIDTH;
            //tableLayoutPanelStreamingOnCar.Visible = false;
        }

        private void LeftPanelHide()
        {
            panelLeft.Enabled = false;
            panelLeft.Width = 0;
            //tableLayoutPanelStreamingOnCar.Visible = false;
        }

        /// <summary>
        /// 車載器用機能パネルの有効/無効<br/>
        /// 左パネルのタブ選択、ボタン選択。<br/>
        /// tabPageにEnabledが無いので、<br/>
        /// ボタンのEnabledとタブの選択イベントで対処する<br/>
        /// WPFならば。。。。OTL
        /// </summary>
        /// <param name="index"></param>
        private void SetSpecifyLayout(ServerIndex index)
        {
            if (ServerIndex.WeatherMedia == index)
            {
                buttonShowDrivingMovie.Enabled = false;
                buttonShowRemoteSetting.Enabled = false;
                //gridCarList.Columns[0].MinimumWidth = 100;
                //gridCarList.Columns[0].Width = 100;
                //tabControlRtSelect.Selecting += TabControlRtSelect_Selecting;
            }
        }

        private void ReadyToPlay(WMDataSet.EventListRow row)
        {
            var playlist = ViewModel.PlaylistTable.Where(
                content => (content.DeviceId == row.DeviceId && content.Timestamp == row.Timestamp));
            if (playlist.Any()) 
            {
                ViewModel.PlayDeviceId = row.DeviceId;
                ViewModel.PlayDeviceName = row.CarNumber;
                ViewModel.PlayTimestamp = row.Timestamp;
                for (var i = 0; i < 8; i++)
                {
                    var canPlay = true;
                    if (playlist.FirstOrDefault(item => item.Ch == i) is null) 
                    {
                        canPlay = false;
                    }
                    ViewModel.SetCanPlayMovies(i, canPlay);
                }
            }
            else
            {
                ViewModel.PlayDeviceName = string.Empty;
                ViewModel.PlayTimestamp = DateTime.MinValue;
                for (var i = 0; i < 8; i++)
                {
                    ViewModel.SetCanPlayMovies(i, false);
                }
            }
        }

        private void PlayffmpegControl(
            int ch, string filepath, string audioFilePath, string windowTitle, int windowHeight, int fps)
        {
            MovieCancellationTokenSource?.Cancel();
            MovieCancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () => 
            {
                await FfmpegCtrl.PlayMovieAsync(ch, filepath, audioFilePath, fps, MovieCancellationTokenSource.Token);
            });
        }

        #region Prepost Alert
        private void WaitFormClose()
        {
            WaitForm.Close();
            WaitForm.Dispose();
        }

        private void ShowAlertDialogUiThread(string DeviceId, int MovieType)
        {
            if (WaitForm.Visible == true)
            {
                Invoke(new WaitFormCloseDelegate(WaitFormClose));
            }

            if (AlertForm == null)
            {
                Invoke((MethodInvoker)(() =>
                {
                    this.Visible = true;
                    WindowState = FormWindowState.Normal;
                    Activate();
                }));

                Invoke((MethodInvoker)(() =>
                {
                    AlertForm = new EventAlertWindow(DeviceId, MovieType);
                    AlertForm.FormClosed += AlertForm_FormClosed;
                    AlertForm.Show();
                }));
            }
        }

        private void AlertForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AlertForm.Dispose();
            AlertForm = null;
        }

        //  アラート表示
        private void ShowAlertDialog(string msg)
        {
            //  アラート
            var status = JsonConvert.DeserializeObject<MqttJsonPrepostEvent>(msg,
                                  new IsoDateTimeConverter { DateTimeFormat = "yyyyMMddHHmmss" });
            Debug.WriteLine($"TopicEventPrepost: {status.device_id}, {status.req_id}, status:{status.status}, mt:{status.movie_type}, {status.n}/{status.total}");

            // 1: 強い衝撃, 3: 緊急スイッチ
            if ((status.movie_type == 1) || (status.movie_type == 3))
            {
                // ここはMQTTスレッドの可能性がある。
                // UIスレッドから子ウィンドウを表示する。
                WMDataSet.DeviceRow device = null;
                lock(ViewModel.DeviceTable)
                {
                    device = ViewModel.DeviceTable.FirstOrDefault(item => item.DeviceId == status.device_id);
                }
                var notifyDevice = (device is null) ? status.device_id : device.CarNumber;
                ShowAlertDialogUiThread(notifyDevice, status.movie_type);
            }
        }
        #endregion

        #region サーバ情報取得
        /// <summary>
        /// サーバー情報取得<br/>
        /// Iniファイルにサーバー情報の定義がある場合、<br/>
        /// そちらを優先する。
        /// ・ServerIndex -> 内部リストからサーバ情報を取得する。<br/>
        /// ・RestServer, MqttServer, AccessId, AccessPassword<br/>
        ///     -> ４つの情報が存在する場合、これらを返す
        /// ・未設定 -> ユーザプロパティのServerIndexから取得する
        /// </summary>
        /// <returns>サーバー情報</returns>
        private OperationServerInfo GetServerInfo()
        {
            OperationServerInfo result;
            try
            {
                ServerIndex serverIndex = (ServerIndex)Enum.ToObject(typeof(ServerIndex), LocalSettings.ServerIndex);
                result = LocalSettings.AllowedServers.FirstOrDefault(x => x.Id == serverIndex);
                if (result == null)
                {
                    result = LocalSettings.AllowedServers[0];
                }
            }
            catch (Exception)
            {
                result = LocalSettings.AllowedServers[0];
            }
            return result;
        }
        #endregion サーバ情報取得

        private void CloseAllRemoteSettingWindow()
        {
            if (RemoteSettingWindows != null)
            {
                // window.Close時にウィンドウ管理ディクショナリに変更が加わるため
                // ウィンドウ管理ディクショナリを直接操作しない
                var windowList = RemoteSettingWindows.Values.ToList();
                foreach (var window in windowList)
                {
                    window.Close();
                }
            }
        }

        private void CloseAllMovieRequestWindow()
        {
            if (MovieRequestWindows != null)
            {
                // window.Close時にウィンドウ管理ディクショナリに変更が加わるため
                // ウィンドウ管理ディクショナリを直接操作しない
                var windowList = MovieRequestWindows.Values.ToList();
                foreach (var window in windowList)
                {
                    window.viewModel.IsDisposing = true;
                    window.Close();
                }
                MovieRequestWindows.Clear();
            }
        }
    }
}
