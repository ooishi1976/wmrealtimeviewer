using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Threading;
using Mamt;
using Mamt.Args;
using Microsoft.Extensions.FileSystemGlobbing;
using MpgCommon;
using MpgCustom;
using MpgMap;
using MpgMap.Args;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RealtimeViewer.Controls;
using RealtimeViewer.Converter;
using RealtimeViewer.Ipc;
using RealtimeViewer.Logger;
using RealtimeViewer.Map;
using RealtimeViewer.Model;
using RealtimeViewer.Movie;
using RealtimeViewer.Network;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network.Mqtt;
using RealtimeViewer.Setting;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UserBioDP;

namespace RealtimeViewer.WMShipView
{
    using Assembly = System.Reflection.Assembly;

    public partial class MainForm : Form
    {
        ///// <summary>
        /////  サイドパネルサイズ
        ///// </summary>
        //private const int LEFT_PANEL_WIDTH = 240;

        /// <summary>
        /// コンフィグタブ(TabPage.Name)
        /// </summary>
        private const string TabPageConfigName = "tabPageConfig";

        /// <summary>
        /// コンフィグタブ(TabPage.Text)
        /// </summary>
        private const string tabPageConfigText = "コンフィグ";

        private OperationLogger OperationLogger => ViewModel.OperationLogger;

        //// カスタム情報プロパティ値
        //private CustomProperty m_customProperty = new CustomProperty();

        //private MqttClient mqttClient;

        //private System.Timers.Timer reconnectTimer;

        /// <summary>
        /// INIファイル情報
        /// </summary>
        private SettingIni LocalSettings => ViewModel.LocalSettings;

        private OperationServerInfo OperationServerInfo => ViewModel.OperationServerInfo;

        //private int m_SelectOfficeID = 0;  // 選択中の事業所ID

        private MainViewModel ViewModel { get; set; } = new MainViewModel();

        private WaitingForm m_wform;
        //private EventAlertWindow m_alertForm;

        //private RequestSequence httpSequence;
        //private CancellationTokenSource downloadCancelTokenSource;
        //private CancellationTokenSource eventListCancelTokenSource;

        //private Dictionary<string, bool> tempPaths = new Dictionary<string, bool>();
        //private FormMessage formMessage;
        //private RtvIpcMessageServer messageServer;
        private CancellationTokenSource MovieCancellationTokenSource { get; set; } = new CancellationTokenSource();

        private FfmpegCtrl FfmpegCtrl { get; set; } = new FfmpegCtrl();
        //private CancellationTokenSource gtaskCancelSource;
        //private int streamingBeforeWaitSec;
        //private Dictionary<string, StreamingStatus> streamingDic = new Dictionary<string, StreamingStatus>();

        //private List<TopicRegex> topicRegexes = new List<TopicRegex>();  // topic用正規表現

        //private int streamingPreparationSecond = 0;

        /// <summary>
        /// USB監視
        /// </summary>
        private DeviceWatcher deviceWatcher;

        private ConfigPanel configPanel;
        //private CarInfo emergencyCar;


        ///// <summary>
        ///// イベント情報のコンパレータ
        ///// </summary>
        //private EventInfoComparer eventComparer;

        ///// <summary>
        ///// イベントリスト用DataGridのソート列
        ///// </summary>
        //private DataGridViewColumn eventListSortedColumn;

        ///// <summary>
        ///// 地図登録情報のコンパレータ
        ///// </summary>
        //private MapEntryInfoComparer mapEntryInfoComparer;

        ///// <summary>
        ///// 車両リスト用DataGridのソート列
        ///// </summary>
        //private DataGridViewColumn carListSortedColumn;

        ///// <summary>
        ///// WebAPI呼び出し用のHTTPクライアントハンドラ
        ///// </summary>
        //private HttpClientHandler httpClientHandler;

        ///// <summary>
        ///// WebAPI呼び出し用のHTTPクライアント
        ///// </summary>
        //private HttpClient httpClient;

        private Bitmap markerBitmap;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm() : this(string.Empty)
        {
        }

        public MainForm(string deviceId)
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = $"リアルタイム配信システム Ver. {version.Major}.{version.Minor}.{version.Build}";
            ViewModel = new MainViewModel() 
            {
                Dispatcher = Dispatcher.CurrentDispatcher
            };

            // アセンブリバージョンが変わっても、「設定」を引き継ぐおまじない。
            // see https://stackoverflow.com/questions/534261/how-do-you-keep-user-config-settings-across-different-assembly-versions-in-net/534335#534335
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            // ViewModelのバインド設定
            BindViewModel();

            this.Icon = ViewModel.GetMarkerIcon();

            if (!string.IsNullOrEmpty(deviceId))
            {
                ViewModel.SetEmergencyMode(deviceId);
            }
        }

        private BindingSource DeviceBindingSource { get; set; } = new BindingSource();

        private BindingSource OfficeBindingSource { get; set; } = new BindingSource();

        private BindingSource EventListBindingSource { get; set; } = new BindingSource();


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
        }

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

        private void BindDeviceDataSource()
        {
            // 車両リスト
            if (0 < ViewModel.DeviceTable.Rows.Count) 
            {
                var currentOffice = OfficeBindingSource.Current;

                DeviceBindingSource.DataSource = ViewModel.WMDataSet;
                DeviceBindingSource.DataMember = "DeviceTable";
                DeviceBindingSource.Filter = $"OfficeId = -1";
                DeviceBindingSource.Sort = "DeviceId";
                gridCarList.Enabled = true;
                gridCarList.AutoGenerateColumns = false;
                gridCarList.DataSource = DeviceBindingSource;
            }

            // 営業所リスト
            if (0 < ViewModel.OfficeTable.Rows.Count) 
            {
                OfficeBindingSource.DataSource = ViewModel.WMDataSet;
                OfficeBindingSource.DataMember = "OfficeTable";
                OfficeBindingSource.Filter = "Visible = true";
                OfficeBindingSource.Sort = "OfficeId";
                //OfficeBindingSource.CurrentChanged += ComboBoxOffice_SelectedValueChanged;
                comboBoxOffice.DisplayMember = "Name";
                comboBoxOffice.ValueMember = "OfficeId";
                comboBoxOffice.SelectedValueChanged += ComboBoxOffice_SelectedValueChanged;
                comboBoxOffice.DataSource = OfficeBindingSource;
            }
        }

        private void BindEventDataSource(WMDataSet.EventListDataTable eventList)
        {
            ViewModel.EventTable = eventList;
            ViewModel.PlaylistTable = new WMDataSet.PlayListDataTable();
            EventListBindingSource.DataSource = eventList;
            EventListBindingSource.Sort = "Timestamp desc, DeviceId";
            EventListBindingSource.Filter = "MovieType <> 2";
            gridEventList.AutoGenerateColumns = false;
            gridEventList.DataSource = EventListBindingSource;
        }

        private void UnbindEventDataSource()
        {
            gridEventList.DataSource = null;
            ViewModel.EventTable = null;
            ViewModel.PlaylistTable = null;
        }

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

        private void LeftPanelHide()
        {
            panelLeft.Enabled = false;
            panelLeft.Width = 0;
            tableLayoutPanelStreamingOnCar.Visible = false;
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

        #region 指紋認証
        private void OnIdentifiedResult(object sender, IdentifiedResult e)
        {
            // 認証結果を得る。
            if (e.ResultCode == 0)
            {
                // 権限を確認する
                if (e.User.Can(Permission.StreamingView) || e.User.Can(Permission.Engineer))
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        if (m_wform != null && m_wform.IsDisposed)
                        {
                            //TabControlUpdate(true);
                        }
                        // TODO データ取得中ダイアログの文言
                        //UpdateUiAtUserAuthed();
                    }));
                }
            }
        }
        #endregion

        #region フォームイベント処理

        /// <summary>
        /// フォームロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Iniファイルを読み込んで、ロガーの作成
            ViewModel.ReadIniFile();
            OperationLogger.Out(OperationLogger.Category.Application, string.Empty, @"RealtimeViewer Start");

            // VLCコンフィグの更新
            var vlcConf = new VlcConfig();
            vlcConf.UpdateRcFile();

            // 画面DPIを設定する。
            InitMapScale(ViewModel.LocalSettings.MappingScale);
            LeftPanelHide();

            // サイドパネル(ストリーミング/ドラレコ映像/遠隔設定)のEnableを設定する
            // ウェザーメディア用の場合、ドラレコ映像/遠隔設定は利用不可
            SetSpecifyLayout(ViewModel.OperationServerInfo.Id);

            // 認証開始
            if (!ViewModel.IsEmergencyMode)
            {
                if (LocalSettings.OperationServer.IsShowWaitDialog)
                {
                    // 通常モード
                    ViewModel.StartUserAuth(OnIdentifiedResult);
                    StartDeviceWatcher();
                }
                else
                {
                    // 認証無しモード
                    // 指紋認証、USB監視は行わない
                    ViewModel.SetNoUserMode();
                    //UpdateUiAtUserAuthed();
                }
            }
            // イベント再生タブの項目初期化
            PrepareEventListUpdate();

            // HTTP通信用コントローラ作成
            ViewModel.CreateRequestController();

            // MQTTサーバー接続
            ViewModel.AddLocationHandler(OnLocationReceived);
            ViewModel.ConnectMqttServer();

            // 地図描画
            SyncMapScale(true);
            mpgMap.RePaint();

            // TODO 拡張パネル追加
            // CreateExtTabPage(this);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ViewModel.CloseMqttServer();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            var waitTask = Task.Run(async () =>
            {
                var offices = await ViewModel.GetOfficesAsync();
                SetOffices(offices);

                var devices = await ViewModel.GetDevicesAsync();
                SetDevices(devices);

                //var events = await ViewModel.GetEventsAsync(3);

                Invoke((MethodInvoker)(() =>
                {
                    BindDeviceDataSource();
                    timerGPSDraw.Start();
                }));
            });
            waitTask.ContinueWith((t) =>
            {
                ViewModel.DataUpdateDate = DateTime.Now;
            });
            var waitFormViewModel = new WaitFormViewModel(waitTask);
            var waitForm = new WaitingForm(waitFormViewModel);
            waitForm.ShowDialog();
        }

        /// <summary>
        /// 地図描画用タイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerGPSDraw_Tick(object sender, EventArgs e)
        {
            var arrayList = new ArrayList();
            var list = CreateMapEntries();
            arrayList.AddRange(list);
            mpgMap.SetCustomData(arrayList);
        }

        /// <summary>
        /// 車両リスト選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CarListBindingSource_CurrentChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 車両リストクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCarList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        /// <summary>
        /// 車両リストヘッダクリックイベント<br/>
        /// ソート処理<br/>
        /// 住所でソートした場合、住所検索が非同期の関係上、<br/>
        /// 正しくソートされない場合がある。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCarList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        /// <summary>
        /// 選択車両表示変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonSelect_CheckedChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ズームバーにおけるマウスアップイベントを処理する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomBar_MouseUp(object sender, MouseEventArgs e)
        {
        }

#endregion フォームイベント処理

#region 地図コントロールイベント処理
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

                    PointLL center = MapUtils.ConvertJdgToTky(device.Latitude, device.Longitude);

                    TextObject entryObject = new TextObject
                    {
                        Title = device.CarNumber,
                        TitleFont = new Font("Meiryo UI", 9),
                        CommentFont = new Font("Meiryo UI", 9),
                        pen = new Pen(Color.Orange, 1),
                        brush = new SolidBrush(Color.White),
                        TextColor = Color.Black
                    };

                    // TODO: エラー
                    //result.ErrorCode = 0;
                    //result.ErrorMessage = DeviceErrorCode.MakeErrorMessage(result.ErrorCode);  // 「正常」で初期化
                    //if (carInfo.ErrorCode != null)
                    //{
                    //    result.ErrorCode = carInfo.ErrorCode.GetErrorCode();
                    //    result.ErrorMessage = DeviceErrorCode.MakeErrorMessage(carInfo.ErrorCode.GetErrorCode());
                    //    if (result.ErrorCode != 0)
                    //    {
                    //        entryObject.brush = new SolidBrush(DeviceErrorCode.GetBackgroundColor(carInfo.ErrorCode.GetErrorCode()));
                    //        entryObject.Comment = result.ErrorMessage;
                    //    }
                    //}

                    // 座標を設定する
                    entryObject.Point = center;
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


                    //  車両ID情報追加
                    entryObject.ID = device.DeviceId;
                    result.Add(entryObject);
                }

            }
            return result;
        }



        /// <summary>
        /// カスタム情報のヒットテスト結果取得イベント
        /// 吹出しを右クリックした時に呼ばれる。
        /// 該当する運行車情報を選択肢サイドパネルを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpgMap_CustomObjectHit(object sender, CustomObjectHitArgs e)
        {
        }

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpgMap_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 標準的なマウスダウンイベント
        /// 右クリックのときカスタム情報選択判定を行う。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpgMap_MouseDown(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 標準的なマウスホイール操作イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpgMap_MouseWheel(object sender, EventArgs e)
        {
        }

#endregion 地図コントロールイベント処理

#region イベントタブ イベント
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

        private void GridEventList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        /// <summary>
        /// イベントリストのフォーマット指定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridEventList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        /// <summary>
        /// イベントリストの列クリックイベント<br/>
        /// ソート処理を実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridEventList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

#endregion イベントタブ イベント


        private void Button1_Click(object sender, EventArgs e)
        {
        }


        private void TimerStartMQTT_Tick(object sender, EventArgs e)
        {
        }

        private void buttonLeftPanelClose_Click(object sender, EventArgs e)
        {
        }

#region "リアルタイム再生関係"
        /// <summary>
        /// リアルタイム開始ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStreamStart_Click(object sender, EventArgs e)
        {
        }

        private void buttonStreamStop_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ストリーミングチャンネルボタン押下処理<br/>
        /// ラジオボタンをトルグボタン化して利用している。<br/>
        /// ストリーミングを開始するまでは押せない。<br/>
        /// チャンネル番号はラジオボタンのTagプロパティに埋め込んでいる。<br/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonRtCh1_CheckedChanged(object sender, EventArgs e)
        {
        }
#endregion "リアルタイム再生関係"

#region "車載器の映像データ"


        private void buttonShowDrivingMovie_Click(object sender, EventArgs e)
        {
        }
#endregion "車載器の映像データ"

#region "リモート設定"

        private void buttonShowRemoteSetting_Click(object sender, EventArgs e)
        {
        }

        private void RemoteSettingWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
#endregion "リモート設定"
        /// <summary>
        /// イベント再生タブの項目初期化
        /// </summary>
        private void PrepareEventListUpdate()
        {
            //progressBarEventListUpdate.Maximum = 100;
            //progressBarEventListUpdate.Value = 0;
            //progressBarEventListUpdate.Visible = true;
            //buttonEventListUpdate.Enabled = false;
            //buttonDownload.Enabled = false;
            //buttonGetG.Enabled = false;
            //buttonCancelG.Enabled = false;
            //labelGstatus.Text = string.Empty;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
#if HIDE_IN_TASKBAR
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = false;
            }
#endif
        }

        private void UpdateUiAtUserLostAuthority()
        {
            if (ViewModel.IsUserAuthCompleted && !ViewModel.IsEngineerMode)
            {
                ViewModel.AuthedUser = null;
                // ユーザ名の戻し
                labelUserName.Text = Properties.Resources.BeforeAuthoricationUserName;

                // TODO ダウンロードの中止
                //if (downloadCancelTokenSource != null && downloadCancelTokenSource.Token.CanBeCanceled)
                //{
                //    downloadCancelTokenSource.Cancel();
                //}

                // サイドパネルの戻し
                LeftPanelHide();
                tabControlRtSelect.SelectedIndex = 0;

                // メインタブの戻し
                tabControlMain.SelectedIndex = 0;

                // TODO ストリーミングの中止
                //if (isStreaming)
                //{
                //    StopStreaming();
                //}

                // TODO イベント再生強制終了
                // ffmpegCtrl.KillAllProcesses();

                // TODO MovieRequestWindowの終了
                //CloseAllMovieRequestWindow();
                //// RemoteSettingWindowの終了
                //CloseAllRemoteSettingWindow();

                // TODO 事業所の戻し
                // comboBoxOffice.Enabled = false;
                //if (m_SelectOfficeID != localSettings.SelectOfficeID)
                //{
                //    // イベント更新の中止
                //    if (eventListCancelTokenSource != null && eventListCancelTokenSource.Token.CanBeCanceled)
                //    {
                //        eventListCancelTokenSource.Cancel();
                //    }

                //    // Gキャンセル
                //    buttonCancelG_Click(null, null);

                //    m_SelectOfficeID = localSettings.SelectOfficeID;
                //    var dataSource = officeInfoBindingSource.DataSource as BindingList<OfficeInfo>;
                //    if (dataSource != null)
                //    {
                //        var selectedOffice = dataSource.FirstOrDefault(x => x.Id == m_SelectOfficeID);
                //        if (selectedOffice == null)
                //        {
                //            officeInfoBindingSource.Position = 0;
                //            m_SelectOfficeID = 0;
                //        }
                //        else
                //        {
                //            officeInfoBindingSource.Position = dataSource.IndexOf(selectedOffice);
                //        }
                //    }
                //}
            }
        }

        private void UpdateUiAtUserAuthed()
        {
            //labelUserName.Text = AuthedUser.Item.Name;
            //if (AuthedUser.Item.Can(Permission.OfficeEdit) || AuthedUser.Item.Can(Permission.Engineer))
            //{
            //    comboBoxOffice.Enabled = true;
            //}
            //else
            //{
            //    comboBoxOffice.Enabled = false;
            //}
            // TODO データ取得中ダイアログの文言
            //if (m_wform != null)
            //{
            //    m_wform.SetVisibleForAuthWarning(false);
            //}
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (keyData == (Keys.Control | Keys.Shift | Keys.F9) && configPanel != null)
            if (keyData == (Keys.Control | Keys.Shift | Keys.F9))
            {
                Debug.WriteLine(@"Detected Ctrl+Shift+F9.");
                ViewModel.SetEngineerMode();
                if (!tabControlMain.TabPages.ContainsKey(TabPageConfigName))
                {
                    TabPage tabPage = new TabPage();
                    tabPage.Name = TabPageConfigName;
                    tabPage.Text = tabPageConfigText;
                    tabPage.Controls.Add(configPanel);
                    tabControlMain.Enabled = true;
                    tabControlMain.TabPages.Add(tabPage);
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TimerStreamingPreparation_Tick(object sender, EventArgs e)
        {
        }

        private void OfficeInfoBindingSource_CurrentChanged(object sender, EventArgs e)
        {
        }

#region USB機器監視

        private void StartDeviceWatcher()
        {
            deviceWatcher = new DeviceWatcher();
            deviceWatcher.SetWatchTarget(LocalSettings.WatchTargets, OnUsbDeviceCreated, OnUsbDeviceRemoved);
            deviceWatcher.Start();
        }

        private void StopDeviceWatcher()
        {
            deviceWatcher.Stop();
        }

        private void OnUsbDeviceCreated()
        {
            ViewModel.StartUserAuth(OnIdentifiedResult);
        }

        private void OnUsbDeviceRemoved()
        {
            if (ViewModel.IsUserAuthCompleted)
            {
                Debug.WriteLine("Lost Authentication.");
                Invoke((Action)(() =>
                {
                    UpdateUiAtUserLostAuthority();
                }));
            }
        }
#endregion USB機器監視


#region 拡張設定

        private void CreateExtTabPage(Form owner)
        {
            var sessionWaitMin = (OperationServerInfo.Id == ServerIndex.WeatherMedia) ? 5 : 30;
            // コンフィグパネル設定
            configPanel = new ConfigPanel();
            configPanel.Dock = DockStyle.Fill;
            configPanel.DataModel = new ConfigPanelModel()
            {
                Settings = LocalSettings,
//                OfficeList = httpSequence.Offices,
                Owner = owner,
                StreamingSessionWaitMin = sessionWaitMin
            };
            configPanel.DataModel.LoadConfigData();
        }

        #endregion 拡張設定

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
    }
}