using Mamt;
using Mamt.Args;
using MpgCommon;
using MpgCustom;
using MpgMap;
using MpgMap.Args;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RealtimeViewer.Controls;
using RealtimeViewer.Ipc;
using RealtimeViewer.Logger;
using RealtimeViewer.Map;
using RealtimeViewer.Model;
using RealtimeViewer.Movie;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network;
using RealtimeViewer.Network.Mqtt;
using RealtimeViewer.Setting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UserBioDP;
using System.ComponentModel;
using System.Windows.Threading;
using RealtimeViewer.Converter;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Windows.Interop;

namespace RealtimeViewer
{
    using Assembly = System.Reflection.Assembly;

    public partial class MainForm : Form
    {
        /// <summary>
        ///  サイドパネルサイズ
        /// </summary>
        private const int LEFT_PANEL_WIDTH = 240;

        /// <summary>
        /// コンフィグタブ(TabPage.Name)
        /// </summary>
        private const string TabPageConfigName = "tabPageConfig";

        /// <summary>
        /// コンフィグタブ(TabPage.Text)
        /// </summary>
        private const string tabPageConfigText = "コンフィグ";

        private OperationLogger operationLogger;

        // カスタム情報プロパティ値
        private CustomProperty m_customProperty = new CustomProperty();

        private MqttClient mqttClient;

        private System.Timers.Timer reconnectTimer;

        // INIファイル情報
        private SettingIni localSettings = new SettingIni();

        private int m_SelectOfficeID = 0;  // 選択中の事業所ID

        private MainViewModel viewModel;

        private WaitingForm m_wform;
        private EventAlertWindow m_alertForm;

        private RequestSequence httpSequence;
        private CancellationTokenSource downloadCancelTokenSource;
        private CancellationTokenSource eventListCancelTokenSource;

        private Dictionary<string, bool> tempPaths = new Dictionary<string, bool>();
        private FormMessage formMessage;
        private RtvIpcMessageServer messageServer;
        private FfmpegCtrl ffmpegCtrl = new FfmpegCtrl();
        private CancellationTokenSource gtaskCancelSource;
        private int streamingBeforeWaitSec;
        private Dictionary<string, StreamingStatus> streamingDic = new Dictionary<string, StreamingStatus>();

        private List<TopicRegex> topicRegexes = new List<TopicRegex>();  // topic用正規表現

#if USE_SREX
        private string authResultFile;
        Process userAuthProcess = null;
        public static NotifyChangedItem<UserBio.User> AuthedUser { get; set; }
#else
        public static NotifyChangedItem<UserBioDP.User> AuthedUser { get; set; }
#endif
        private UserBio.UserDatabase userDatabase = new UserBio.UserDatabase();
        private UserBioDP.UserDatabaseDP userDatabaseDP = new UserBioDP.UserDatabaseDP();

        private bool m_UserAuthComplete = false;

        private UserAuthDp userAuthDp;
        private int streamingPreparationSecond = 0;

        private DeviceWatcher deviceWatcher;

        private ConfigPanel configPanel;
        private CarInfo emergencyCar;

        private OperationServerInfo operationServerInfo;

        /// <summary>
        /// イベント情報のコンパレータ
        /// </summary>
        private EventInfoComparer eventComparer;

        /// <summary>
        /// イベントリスト用DataGridのソート列
        /// </summary>
        private DataGridViewColumn eventListSortedColumn;

        /// <summary>
        /// 地図登録情報のコンパレータ
        /// </summary>
        private MapEntryInfoComparer mapEntryInfoComparer;

        /// <summary>
        /// 車両リスト用DataGridのソート列
        /// </summary>
        private DataGridViewColumn carListSortedColumn;

        /// <summary>
        /// WebAPI呼び出し用のHTTPクライアントハンドラ
        /// </summary>
        private HttpClientHandler httpClientHandler;

        /// <summary>
        /// WebAPI呼び出し用のHTTPクライアント
        /// </summary>
        private HttpClient httpClient;

        /// <summary>
        /// リアルタイムビューアの仕向け(東武、開発、明治、others)
        /// </summary>
#if _TOBU
        private UserIndex userIndex = UserIndex.Tobu;
#elif _MEIJI
        private UserIndex userIndex = UserIndex.Meiji;
#elif _NEMURO
        private UserIndex userIndex = UserIndex.Nemuro;
#elif _WEATHER_MEDIA
        private UserIndex userIndex = UserIndex.WeatherMedia;
#elif _ISSUI
        private UserIndex userIndex = UserIndex.Issui;
#else
        private UserIndex userIndex = UserIndex.Multiwave;
#endif

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
            viewModel = new MainViewModel();
            carListBindingSource.DataSource = viewModel.BindingCarList;
            // アセンブリバージョンが変わっても、「設定」を引き継ぐおまじない。
            // see https://stackoverflow.com/questions/534261/how-do-you-keep-user-config-settings-across-different-assembly-versions-in-net/534335#534335
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            var vlcConf = new VlcConfig();
            vlcConf.UpdateRcFile();
            localSettings.AllowedServers = ServerInfoList.GetServers(userIndex);
            localSettings.ReadIniFile();
            operationServerInfo = GetServerInfo();  // localSettings.AllowedServersを設定してから出ないと取れない
            m_SelectOfficeID = localSettings.SelectOfficeID;
            streamingBeforeWaitSec = Properties.Settings.Default.StreamingBeforeWaitInitSec;
            httpClientHandler = new HttpClientHandler();
            httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(localSettings.RestRequestTimeout);
            this.Icon = GetMarkerIcon();
            // 画面DPIを設定する。
            InitMapScale();
            LeftPanelHide();

            // サイドパネル(ストリーミング/ドラレコ映像/遠隔設定)のEnableを設定する
            // ウェザーメディア用の場合、ドラレコ映像/遠隔設定は利用不可
            SetSpecifyLayout(operationServerInfo.Id);

            if (string.IsNullOrEmpty(deviceId))
            {
                if (localSettings.OperationServer.IsShowWaitDialog)
                {
                    // 通常モード
                    AuthedUser = new NotifyChangedItem<UserBioDP.User>();
                    AuthedUser.Item = new UserBioDP.User();
                    StartUserAuth();
                    userDatabaseDP.Connect();
                    StartDeviceWatcher();
                }
                else
                {
                    // 認証無しモード
                    // 指紋認証、USB監視は行わない
                    AuthedUser = new NotifyChangedItem<UserBioDP.User>();
                    AuthedUser.Item = new User()
                    {
                        Name = Properties.Resources.NoAuthMode,
                        Permission = (int)UserBioDP.RolePermissonConverter.ToPermission(Role.Engineer),
                    };
                    m_UserAuthComplete = true;
                    UpdateUiAtUserAuthed();
                }
            }
            else
            {
                // 緊急通報モード
                // 指紋認証、USB監視は行わない
                emergencyCar = new CarInfo();
                emergencyCar.DeviceInfo = new DeviceInfo();
                emergencyCar.DeviceInfo.CarId = deviceId;
                emergencyCar.DeviceInfo.DeviceId = deviceId;

                AuthedUser = new NotifyChangedItem<UserBioDP.User>();
                AuthedUser.Item = new User()
                {
                    Name = Properties.Resources.EmergencyMode,
                    Permission = (int)UserBioDP.RolePermissonConverter.ToPermission(Role.SuperUser),
                };
                m_UserAuthComplete = true;
                UpdateUiAtUserAuthed();
            }
            try
            {
                messageServer = new RtvIpcMessageServer();
                messageServer.Request.PropertyChanged += RtvIpcRequestChangeHandler;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
            }
        }

#region ヘルパー関数(地図操作)

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private PointF GetDPI()
        {
            PointF dpi = new PointF();
            Graphics g = this.CreateGraphics();

            // ★画面DPIの制御を行う。(SMDと同じ処理)
            IntPtr hDC = g.GetHdc();
            int horzSize = GetDeviceCaps(hDC, 4);   //  4:= HORZSIZE物理画面の幅(ミリメートル単位)
            int vertSize = GetDeviceCaps(hDC, 6);   //  6:= VERTSIZE物理画面の高さ(ミリメートル単位)
            int horzS = GetDeviceCaps(hDC, 8);  //  8:= HORZRES画面の幅(ピクセル単位)
            int vertS = GetDeviceCaps(hDC, 10); // 10:= VERTRES画面の高さ(ピクセル単位)
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
                int value = viewModel.GetNearIndexInMapScales(mpgMap.MapScale);
                zoomBar.Value = value;
            }
        }

        /// <summary>
        /// 画面中心の最大縮尺をチェックし、必要なら縮尺を変更する。
        /// </summary>
        private void CheckScale()
        {
            // ピクセル位置を緯度経度に変換する。
            Point[] pt = new Point[] { new Point(mpgMap.Width / 2, mpgMap.Height / 2) };
            PointLL[] ptl = mpgMap.Convert(pt);

            int maxscale = mpgMap.GetMaxScale(ptl[0]);
            int scale = mpgMap.MapScale;

            //  世界の中心セット
            mpgMap.MapCenter = new PointLL(ptl[0]);
            if (maxscale > scale)
            {
                mpgMap.MapScale = maxscale;

                // MPGを再描画する。
                mpgMap.RePaint();

                // ズームバーを同期させる。
                SyncMapScale(true);
            }
        }

        /// <summary>
        /// 地図の初期化<br/>
        /// <ul>
        ///   <li>DPI変更</li>
        ///   <li>ズームバースケール設定</li>
        ///   <li>マウス右ボタンの回転禁止</li>
        /// </ul>
        /// </summary>
        private void InitMapScale()
        {
            PointF dpi = GetDPI();
            mpgMap.SetDPI(dpi.X, dpi.Y);
            //m_customProperty.Icon = new Bitmap(GetType(), "bus.png");
            m_customProperty.Icon = GetMarkerBitmap();
            mpgMap.MouseWheel += new MouseEventHandler(MpgMap_MouseWheel);
            ChangeMapMode(MapMode.Move);

            // ズームバー設定
            zoomBar.Minimum = 0;
            zoomBar.Maximum = viewModel.MapScales.Length - 1;
            zoomBar.Value = viewModel.GetNearIndexInMapScales(mpgMap.MapScale);
            zoomBar.TickFrequency = 1;
            zoomBar.TickStyle = TickStyle.Both;

            // マウスの右ボタンでの回転禁止
            mpgMap.MapRotateButton = MouseButtons.None;
            mpgMap.MapScale = localSettings.MappingScale;
        }

#endregion ヘルパー関数(地図操作)

#region フォームイベント処理

        /// <summary>
        /// フォームロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            operationLogger = OperationLogger.GetInstance();
            if (!string.IsNullOrEmpty(localSettings.LogFileDirectory))
            {
                operationLogger.LogDirectoryPath = localSettings.LogFileDirectory;
            }
            operationLogger.LogFileNamePrefix = @"RealtimeViewer";
            operationLogger.CreateLogFile();
            operationLogger.Out(OperationLogger.Category.Application, string.Empty, @"RealtimeViewer Start");
            if (userAuthDp != null)
            {
                operationLogger.Out(OperationLogger.Category.Application, string.Empty, $"UserDB Permission (HasChangePermission = {userAuthDp.HasChangePermission}, IsReadOnly = {userAuthDp.IsReadOnly})");
            }

            // イベント情報タブのコントロールの初期化
            prepareEventListUpdate();

            // 事業所情報の取得
            var dispatcher = Dispatcher.CurrentDispatcher;
            httpSequence = new RequestSequence(localSettings, httpClient, operationServerInfo);
            httpSequence.OfficeLocations = localSettings.OfficeLocations;

            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/streaming/(?<device_id>\w+)/status"), Label = TopicLabel.TopicStreamingStatus, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/status/(?<device_id>\w+)"), Label = TopicLabel.TopicLocation, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/error/(?<device_id>\w+)"), Label = TopicLabel.TopicErrorStatus, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/engine"), Label = TopicLabel.TopicEventAccOn, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/driver"), Label = TopicLabel.TopicEventDriver, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/prepost"), Label = TopicLabel.TopicEventPrepost, });

            var officeRequestTask = httpSequence.GetOfficeList(localSettings.ExcludeOfficeId);
            officeRequestTask.GetAwaiter().OnCompleted((() =>
            {
                if (httpSequence.Offices == null)
                {
                    MessageBox.Show("クラウドサーバに接続できませんでした。\n終了してください。", "通信エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Environment.Exit(-1);
                    return;
                }
                else if (httpSequence.Offices.Count == 0)
                {
                    MessageBox.Show("営業所情報が取得できません。\n終了してください。", "タイムアウト", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Environment.Exit(-1);
                    return;
                }

                var dataSource = new BindingList<OfficeInfo>(httpSequence.Offices.Where(x => x.Visible).ToList());
                if (dataSource.Count == 0)
                {
                    MessageBox.Show("選択可能な営業所がありません。\n設定を見直してください。", "設定ファイルエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Environment.Exit(-1);
                }
                officeInfoBindingSource.DataSource = dataSource;
                comboBoxOffice.DataSource = officeInfoBindingSource;
                comboBoxOffice.DisplayMember = "Name";
                comboBoxOffice.ValueMember = "Id";

                // 事業所コンボボックスの選択と地図の描画
                var selectedOffice = dataSource.FirstOrDefault(x => x.Id == m_SelectOfficeID);
                if (selectedOffice == null)
                {
                    officeInfoBindingSource.Position = 0;
                    selectedOffice = officeInfoBindingSource.Current as OfficeInfo;
                    if (selectedOffice == null)
                    {
                        m_SelectOfficeID = 0;
                    }
                    else
                    {
                        m_SelectOfficeID = selectedOffice.Id;
                    }
                }
                else
                {
                    officeInfoBindingSource.Position = dataSource.IndexOf(selectedOffice);
                }
                // 選択中事業所を地図上に描画
                MoveToOfficeLocation();
                SyncMapScale(true);
                mpgMap.RePaint();

                mapEntryInfoComparer = new MapEntryInfoComparer()
                {
                    Mode = MapEntryInfoComparer.SortMode.DeviceId,
                    Order = SortOrder.Ascending
                };
                eventComparer = new EventInfoComparer()
                {
                    Mode = EventInfoComparer.SortMode.TimeStamp,
                    Order = SortOrder.Descending
                };

                Form owner = this;
                eventListCancelTokenSource = new CancellationTokenSource();
                if (emergencyCar == null)
                {
                    // 通常モード
                    _ = Task.Run(async () =>
                    {
                        await httpSequence.GetDeviceList(m_SelectOfficeID, false, eventListCancelTokenSource.Token, eventListUpdateProgress);

                        // 拡張機能パネル設定
                        dispatcher.Invoke(() =>
                        {
                            CreateExtTabPage(owner);
                        });

                        //  営業所振り分けのため ALL device取得後に起動する
                        ConnectMQTTServer();

                        // イベント・リクエスト映像リスト取得
                        var w = new TimeSpan(0, 0, 0);
                        await httpSequence.UpdateEvents(
                            m_SelectOfficeID, w, eventListCancelTokenSource.Token, eventListUpdateProgress);
                        dispatcher.Invoke(() =>
                        {
                            // bindされたリストをUIスレッド以外から触るとエラーとなるので
                            // UIスレッドにリスト更新を任せる
                            httpSequence.CreateEventBindingList();
                            httpSequence.Events.Sort(eventComparer);
                            httpSequence.CreateRequestBindingList();
                        });
                    });
                }
                else  // 緊急モード
                {
                    _ = Task.Run(async () =>
                    {
                        await httpSequence.GetDevice(emergencyCar.DeviceId, eventListCancelTokenSource.Token);
                        if (!httpSequence.AllCars.TryGetValue(emergencyCar.DeviceId, out emergencyCar))
                        {
                            MessageBox.Show("車両情報が取得できませんでした。\n終了します。", "車両情報取得失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(-1);
                        }

                        // 運用上、営業所選択は変更されていない想定だが
                        // 指定車両と選択営業所が異なっている場合は、営業所選択を変更する。
                        // ただし、プルダウンに表示されていない営業所に所属している場合は
                        // その限りではない。
                        if (m_SelectOfficeID != emergencyCar.OfficeInfo.Id)
                        {
                            int index = dataSource.IndexOf(emergencyCar.OfficeInfo);
                            if (index != -1)
                            {
                                Invoke((MethodInvoker)(() =>
                                {
                                    officeInfoBindingSource.Position = index;
                                }));
                                selectedOffice = emergencyCar.OfficeInfo;
                                m_SelectOfficeID = index;
                            }
                        }

                        // 車両リスト強制表示用にダミーの位置情報を設定する。
                        MqttJsonLocation location = new MqttJsonLocation()
                        {
                            Device_id = emergencyCar.DeviceId,
                            Ts = DateTime.Now.AddSeconds(-60).ToString("yyyyMMddHHmmss"),
                            Lon = string.Empty,
                            Lat = string.Empty
                        };
                        emergencyCar.AddLocation(location);

                        dispatcher.Invoke(() =>
                        {
                            drawMapCancelToken = new CancellationTokenSource();
                            DrawObjectOnMap(dispatcher, drawMapCancelToken.Token);
                            radioButtonSelect.Checked = true;
                            tabControlMain.Enabled = true;
                            // 拡張機能パネル設定
                            CreateExtTabPage(owner);
                        });

                        //  営業所振り分けのため ALL device取得後に起動する
                        ConnectMQTTServer();

                        // イベント・リクエスト映像リスト取得
                        var w = new TimeSpan(0, 0, 0);
                        await httpSequence.UpdateEvents(
                            m_SelectOfficeID, w, eventListCancelTokenSource.Token, eventListUpdateProgress);
                        dispatcher.Invoke(() =>
                        {
                            // bindされたリストをUIスレッド以外から触るとエラーとなるので
                            // UIスレッドにリスト更新を任せる
                            httpSequence.CreateEventBindingList();
                            httpSequence.Events.Sort(eventComparer);
                            httpSequence.CreateRequestBindingList();
                        });
                    });
                }
                officeInfoBindingSource.CurrentChanged += OfficeInfoBindingSource_CurrentChanged;
            }));

            // TODO: m_UserAuthComplete は削除したい。もしくは代替措置。
            // ひとまず認証済みということにしておく。認証ダイアログを削除したものの、その周辺が未修整のため。
            //m_UserAuthComplete = true;
            gridEventList.DataSource = httpSequence.Events;
        }

        private string GetMqttClientId()
        {
            //  ユニークなIDをmacAddress[0]+ProcessIDで作成
            List<PhysicalAddress> macAddress = GetPhysicalAddress();
            string tmpAddress = macAddress[0].ToString();
            Process hProcess = Process.GetCurrentProcess();

            string result = "ex:" + tmpAddress + hProcess.Id;

            hProcess.Close();
            hProcess.Dispose();

            return result;
        }

        private void ConnectMQTTServer()
        {
            //  MQTTサーバ設定
            var topic_list = new List<string>();
            var qos_list = new List<byte>();
            ServerInfo serverInfo = operationServerInfo.GetPhygicalServerInfo();
            try
            {
                mqttClient = new MqttClient(serverInfo.MqttAddr);
                mqttClient.MqttMsgPublishReceived += ArrivedMqttMessage;
                mqttClient.MqttMsgPublished += Client_MqttMsgPublished;
                mqttClient.ConnectionClosed += Client_ConnectionClosed;
                mqttClient.Connect(GetMqttClientId());     //  MAC address + PIDとかにする

                if (emergencyCar == null)
                {
                    topic_list.Add($"car/status/#");
                    qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                    topic_list.Add($"car/error/#");
                    qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                    topic_list.Add($"car/event/#");
                    qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                    topic_list.Add($"car/streaming/+/status");
                    qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                }
                else
                {
                    // 緊急モードの場合は、指定車両の情報のみを受信する
                    topic_list.Add($"car/status/{emergencyCar.DeviceId}/#");
                    qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                    topic_list.Add($"car/error/{emergencyCar.DeviceId}/#");
                    qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                    topic_list.Add($"car/event/{emergencyCar.DeviceId}/#");
                    qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                    topic_list.Add($"car/streaming/{emergencyCar.DeviceId}/status");
                    qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                }
                mqttClient.Subscribe(topic_list.ToArray(), qos_list.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex.ToString()}, {ex.Message}");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (reconnectTimer != null)
                {
                    reconnectTimer.Dispose();
                    reconnectTimer = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception {ex}, {ex.StackTrace}");
            }

            if (isStreaming)
            {
                StopStreaming();
                for (int i = 0; i < 5; i++)
                {
                    if (0 == pubRecord.Count)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }

            CloseAllRemoteSettingWindow();
            CloseAllMovieRequestWindow();
            if (downloadCancelTokenSource != null && downloadCancelTokenSource.Token.CanBeCanceled)
            {
                downloadCancelTokenSource.Cancel();
            }
            buttonCancelG_Click(sender, e);

            if (eventListCancelTokenSource != null && eventListCancelTokenSource.Token.CanBeCanceled)
            {
                eventListCancelTokenSource.Cancel();
            }

            try
            {
                if (mqttClient != null)
                {
                    // MQTTクライアントからイベントハンドラを取り除いておく。
                    mqttClient.ConnectionClosed -= Client_ConnectionClosed;

                    if (mqttClient.IsConnected)
                    {
                        // Disconnect するとMQTTスレッドが停止する。
                        mqttClient.Disconnect();
                    }
                }

                if (gtaskCancelSource != null)
                {
                    gtaskCancelSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception {ex}, {ex.StackTrace}");
            }

            ffmpegCtrl.KillAllProcesses();

            if (httpClient != null)
            {
                httpClient.Dispose();
            }

            if (emergencyCar == null && localSettings.OperationServer.IsShowWaitDialog)
            {
                TerminateUserAuth();
                StopDeviceWatcher();
            }

            localSettings.MappingScale = mpgMap.MapScale;
            localSettings.SelectOfficeID = m_SelectOfficeID;
            localSettings.WriteIniFile();

            var userName = string.IsNullOrEmpty(AuthedUser.Item.Name) ? string.Empty : AuthedUser.Item.Name;
            operationLogger.Out(OperationLogger.Category.Application, userName, @"RealtimeViewer End");

            // 一時ファイル、ディレクトリを全部削除する。
            foreach (var path in tempPaths?.Keys)
            {
                try
                {
                    Directory.Delete(path, true);
                    Debug.WriteLine($"Deleted {path}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"@@@ Exception: Directory.Delete {path}, {ex}");
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            timerGPSDraw.Interval = localSettings.UpdateInterval;  //  初期30-40秒
            timerGPSDraw.Enabled = true;
            if (emergencyCar == null && localSettings.OperationServer.IsShowWaitDialog)
            {
                // 通常モード 認証プログレスウィンドウあり
                ShowWaitingForm();
            }
        }

        private void ShowWaitingForm()
        {
            if (m_wform != null && !m_wform.IsDisposed)
            {
                m_wform.Close();
                m_wform.Dispose();
            }
            m_wform = new WaitingForm();
            if (AuthedUser.Item?.Permission > 0)
            {
                // すでに認証済み。「認証してくれ」メッセージは隠す。
                m_wform.SetVisibleForAuthWarning(false);
            }
            m_wform.Show();
        }

        /// <summary>
        /// タブ選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControlMain_Selected(object sender, TabControlEventArgs e)
        {
            switch (e.TabPageIndex)
            {
                case 1:  // イベントタブ
                    panelPlay.Visible = false;
                    InitEventListCheckBox();
                    break;

                default:
                    // 処理なし
                    break;
            }
        }

        private void ReadEventRemark()
        {
            //  備考欄読み込み
            string stCurrentDir = Environment.CurrentDirectory;
            string folderPath = stCurrentDir + "\\remarkList";
            string filePath = folderPath + "\\eventRemarkList.dat";
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            DataGridView dgv = gridEventList;

            try
            {
                string[] lines = File.ReadAllLines(filePath, enc);

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fieldChk = lines[i].Split(',');
                    for (int j = 0; j < dgv.Rows.Count; j++)
                    {
                        if (dgv.Rows[j].Cells[5].Value.ToString() == fieldChk[1])
                        {
                            dgv.Rows[j].Cells[4].Value = fieldChk[0];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@Exception: readEventRemark {ex}");
            }
        }

        private void InitEventListCheckBox()
        {
            gridEventList.Columns[0].ReadOnly = false;
            foreach (DataGridViewRow row in gridEventList.Rows)
            {
                row.Cells[0].Value = false;
            }
        }

        private void WriteEventRemark()
        {
            //  備考欄保存
            string stCurrentDir = Environment.CurrentDirectory;
            string folderPath = stCurrentDir + "\\remarkList";
            string filePath = folderPath + "\\eventRemarkList.dat";
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            DataGridView dgv = gridEventList;
            string writeData = null;
            int writeCount = 0;

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    if (dgv.Rows[i].Cells[4].Value != null)
                    {
                        writeData += dgv.Rows[i].Cells[4].Value.ToString() + "," + dgv.Rows[i].Cells[5].Value.ToString() + "\r\n";
                        writeCount++;
                    }
                }

                if (writeCount > 0)
                {
                    File.WriteAllText(filePath, writeData, enc);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@Exception: writeEventRemark {ex}");
            }
        }

        //  ユニークな接続ID作成のため適当なMACアドレス取得
        public List<PhysicalAddress> GetPhysicalAddress()
        {
            var list = new List<PhysicalAddress>();
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in interfaces)
            {
                if (OperationalStatus.Up == adapter.OperationalStatus)
                {
                    if ((NetworkInterfaceType.Unknown != adapter.NetworkInterfaceType) &&
                        (NetworkInterfaceType.Loopback != adapter.NetworkInterfaceType))
                    {
                        list.Add(adapter.GetPhysicalAddress());
                    }
                }
            }
            return list;
        }

        private void ArrivedMqttMessage(object sender, MqttMsgPublishEventArgs e)
        {
            //  MQTT取得
            var msg = System.Text.Encoding.UTF8.GetString(e.Message);
            //            Debug.WriteLine($"Received topic:{e.Topic}, msg:{msg}");

            //  トピック検索 (error/status/event...)
            TopicLabel label = TopicLabel.TopicNone;
            foreach (var tr in topicRegexes)
            {
                Match match = tr.Regex.Match(e.Topic);
                if (match.Success)
                {
                    label = tr.Label;

                    string received_device_id;

                    switch (label)
                    {
                        case TopicLabel.TopicNone:
                            break;

                        case TopicLabel.TopicErrorStatus:
                            break;

                        case TopicLabel.TopicEventAccOn:
                            break;

                        case TopicLabel.TopicLocation:
                            break;

                        case TopicLabel.TopicEventDriver:
                            break;

                        case TopicLabel.TopicEventPrepost:
                            break;

                        case TopicLabel.TopicStreamingStatus:
                            received_device_id = match.Groups["device_id"].Value;
                            if (!streamingDic.ContainsKey(received_device_id))
                            {
                                // 他者が実行しているストリーミング情報のため、無視。
                                break;
                            }
                            else if (streamingDic[received_device_id].State == StreamingState.None)
                            {
                                // すでに終了したつもりだが、車載器の状態とすれ違ったと考えられる。無視する。
                                break;
                            }
                            else
                            {
                                if (ServerIndex.WeatherMedia == operationServerInfo.Id)
                                {
                                    StreamingRtmpTopicReceived(received_device_id, msg);
                                }
                                else
                                {
                                    StreamingTopicReceived(received_device_id, msg);
                                }
                            }
                            break;
                    }
                }
            }
            //  MQTT保存
            AppendMQTTData(label, msg);
        }

        /// <summary>
        /// MQTTのStreamingトピックを受信した時の処理<br/>
        /// ISDT用(UDP -> MPEG-TS形式, MEGA-DASH形式視聴用)
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="msg"></param>
        private void StreamingTopicReceived(string deviceId, string msg)
        {
            if (streamingCommandTimer != null)
            {
                // タイマーは破棄しておく。
                streamingCommandTimer.Dispose();
                streamingCommandTimer = null;
            }

            try
            {
                var ss = JsonConvert.DeserializeObject<MqttJsonStreamingStatus>(msg);
                if (isStreaming && 
                    ss.streamingStarted == 1 && 
                    (taskStreamingRequest == null || taskStreamingRequest.IsCompleted) && 
                    ffmpeg == null)
                {
                    if (string.IsNullOrEmpty(ss.elapsed))
                    {
                        // ffmpeg の進捗なし。開始まで単純に時間で待つ。
                        if (streamingDeviceId == deviceId)
                        {
                            taskStreamingRequest = PlayStreaming(true);
                        }
                    }
                    else
                    {
                        // ffmpeg の進捗あり。送信経過時間が1分以上で開始する。
                        var d = TimeSpan.Parse(ss.elapsed);
                        if (streamingDeviceId == deviceId && d.TotalSeconds >= localSettings.StreamingSessionWait)
                        {
                            taskStreamingRequest = PlayStreaming(false);
                        }
                    }
                }

                // ストリーミング状況を表示更新。主体は車載器であることに注意されたい。
                if (streamingDic.TryGetValue(deviceId, out StreamingStatus status))
                {
                    if (status.State != StreamingState.None && !string.IsNullOrEmpty(ss.elapsed))
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            tableLayoutPanelStreamingOnCar.Visible = true;
                            UpdateLeftPanelStreamingStatusOnCar(ss);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
            }
        }

        /// <summary>
        /// MQTTのStreamingトピックを受信した時の処理<br/>
        /// ウェザーメディア用(RTMP -> FLV形式(RTSP視聴用))
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="msg"></param>
        private void StreamingRtmpTopicReceived(string deviceId, string msg)
        {
            if (streamingCommandTimer != null)
            {
                // タイマーは破棄しておく。
                streamingCommandTimer.Dispose();
                streamingCommandTimer = null;
            }

            try
            {
                var ss = JsonConvert.DeserializeObject<MqttJsonStreamingStatus>(msg);
                if (isStreaming && 
                    ss.streamingStarted == 1 && 
                    (taskStreamingRequest == null || taskStreamingRequest.IsCompleted) && 
                    ffmpeg == null)
                {
                    if (string.IsNullOrEmpty(ss.elapsed))
                    {
                        // ffmpeg の進捗なし。開始まで単純に時間で待つ。
                        if (streamingDeviceId == deviceId)
                        {
                            taskStreamingRequest = PlayRtspStreaming(true);
                        }
                    }
                    else
                    {
                        // ffmpeg の進捗あり。送信経過時間が1分以上で開始する。
                        var d = TimeSpan.Parse(ss.elapsed);
                        if (streamingDeviceId == deviceId && d.TotalSeconds >= localSettings.StreamingSessionWait)
                        {
                            taskStreamingRequest = PlayRtspStreaming(false);
                        }
                    }
                }

                // ストリーミング状況を表示更新。主体は車載器であることに注意されたい。
                if (streamingDic.TryGetValue(deviceId, out StreamingStatus status))
                {
                    if (status.State != StreamingState.None && !string.IsNullOrEmpty(ss.elapsed))
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            tableLayoutPanelStreamingOnCar.Visible = true;
                            UpdateLeftPanelStreamingStatusOnCar(ss);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
            }
        }

        private delegate void WaitFormCloseDelegate();

        private void WaitFormClose()
        {
            m_wform.Close();
            m_wform.Dispose();
        }

        //  描画処理で使うMQTTデータ登録
        private void AppendMQTTData(TopicLabel topic, string msg)
        {
            try
            {
                //  選択中営業所のみ処理
                MqttJsonLocation fieldData = new MqttJsonLocation();
                var jsonData = msg;
                fieldData = JsonConvert.DeserializeObject<MqttJsonLocation>(jsonData);

                if (httpSequence.AllCars.TryGetValue(fieldData.Device_id, out CarInfo carInfo))
                {
                    // staging 環境なら全車両が対象となる。
                    // production 環境なら該当営業所のみに絞り込む。
                    if (carInfo.DeviceInfo.OfficeId == m_SelectOfficeID)
                    {
                        if (topic == TopicLabel.TopicEventPrepost)
                        {
                            //  アラート表示
                            if (localSettings.UseEmergencyPopUp)
                            {
                                ShowAlertDialog(msg);
                            }
                        }
                    }
                    // 事業所に関わらず受信したものを保持
                    SetTopicInfomation(topic, msg);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void ShowAlertDialogUiThread(string DeviceId, int MovieType)
        {
            if (m_wform.Visible == true)
            {
                Invoke(new WaitFormCloseDelegate(WaitFormClose));
            }

            if (m_alertForm == null)
            {
                RestoreMainForm();

                Invoke((MethodInvoker)(() =>
                {
                    m_alertForm = new EventAlertWindow(DeviceId, MovieType);
                    m_alertForm.FormClosed += AlertForm_FormClosed;
                    m_alertForm.Show();
                }));
            }
        }

        private void AlertForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_alertForm.Dispose();
            m_alertForm = null;
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
                ShowAlertDialogUiThread(status.device_id, status.movie_type);
            }
        }

        /// <summary>
        /// 受信情報を保持する
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="msg"></param>
        private void SetTopicInfomation(TopicLabel topic, string msg)
        {
            if (TopicLabel.TopicLocation == topic)
            {
                MqttJsonLocation fieldData = JsonConvert.DeserializeObject<MqttJsonLocation>(msg);
                CarInfo carInfo = httpSequence.GetCarInfoFromGroups(m_SelectOfficeID, fieldData.Device_id);
                if (carInfo != null)
                {
                    if (emergencyCar == null)
                    {
                        if (operationServerInfo.Id == ServerIndex.WeatherMedia)
                        {
                            carInfo.SetLocation(fieldData);
                        }
                        else
                        {
                            carInfo.AddLocation(fieldData);
                        }
                    }
                    else
                    {
                        // 緊急モード
                        // 先頭の位置情報がダミーの場合は、受信したものに置き換える
                        var location = carInfo.GetLocation();
                        if (string.IsNullOrEmpty(location.Lat) && string.IsNullOrEmpty(location.Lon))
                        {
                            carInfo.SetLocation(fieldData);
                        }
                        else
                        {
                            carInfo.AddLocation(fieldData);
                        }
                    }
                }
                else if (httpSequence.AllCars.TryGetValue(fieldData.Device_id, out carInfo))
                {
                    carInfo.SetLocation(fieldData);
                }
            }
            else if (TopicLabel.TopicErrorStatus == topic)
            {
                MqttJsonError fieldData = JsonConvert.DeserializeObject<MqttJsonError>(msg);
                if (httpSequence.AllCars.TryGetValue(fieldData.DeviceId, out CarInfo carInfo))
                {
                    carInfo.ErrorCode = fieldData;
                }
            }
            else if (TopicLabel.TopicEventPrepost == topic)
            {
                MqttJsonPrepostEvent fieldData = JsonConvert.DeserializeObject<MqttJsonPrepostEvent>(
                    msg, new IsoDateTimeConverter { DateTimeFormat = "yyyyMMddHHmmss" });
                if (httpSequence.AllCars.TryGetValue(fieldData.device_id, out CarInfo carInfo))
                {
                    carInfo.EventPrepost = fieldData;
                }
            }
            else if (TopicLabel.TopicEventAccOn == topic)
            {
                MqttJsonEventAccOn fieldData = JsonConvert.DeserializeObject<MqttJsonEventAccOn>(msg);
                if (httpSequence.AllCars.TryGetValue(fieldData.device_id, out CarInfo carInfo))
                {
                    carInfo.AccStatus = fieldData;
                    // ストリーミング対象の車載器がACC OFFになったら、ストリーミングをOFF
                    if (!fieldData.IsAccOn() && IsStreaming(fieldData.device_id))
                    {
                        StopStreaming();
                    }
                }
            }
            else
            {
                // 処理なし
            }
        }

        private CancellationTokenSource drawMapCancelToken;

        /// <summary>
        /// 地図描画用タイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerGPSDraw_Tick(object sender, EventArgs e)
        {
            //  ここで主要コントロール有効
            if (m_UserAuthComplete == true)
            {
                TabControlUpdate(true);
            }

            //  位置描画
            if (drawMapCancelToken != null && drawMapCancelToken.Token.CanBeCanceled)
            {
                drawMapCancelToken.Cancel();
            }
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            _ = Task.Run(() =>
            {
                drawMapCancelToken = new CancellationTokenSource();
                try
                {
                    DrawObjectOnMap(dispatcher, drawMapCancelToken.Token);
                    dispatcher.Invoke(() =>
                    {
                        if (m_UserAuthComplete && m_wform != null)
                        {
                            m_wform.FinishWaiting();
                        }
                    });
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("[OnGPSTimerTick]地図描画処理が中断されました。");
                }
            });
        }

        /// <summary>
        /// 地図に運行車両情報を描画する
        /// </summary>
        private void DrawObjectOnMap(Dispatcher dispatcher, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            bool drawAllCar = true;
            var nowTime = DateTime.Now;
            var borderTime = nowTime.AddSeconds(-20);
            dispatcher.Invoke(() =>
            {
                labelUpdateDate.Text = nowTime.ToString();
            });

            // リストクリア
            viewModel.MapEntries.Clear();
            viewModel.MapEntriesForMpgMap.Clear();
            if (httpSequence.GroupCars.TryGetValue(m_SelectOfficeID, out Dictionary<string, CarInfo> carInfoList))
            {
                // 新リスト作成
                foreach (var keyValuePair in carInfoList)
                {
                    cancelToken.ThrowIfCancellationRequested();

                    var carInfo = keyValuePair.Value;
                    carInfo.MapEntry = null;
                    if (carInfo.CanEntryForMap(borderTime))
                    {
                        MapEntryInfo newMapEntry = CreateMapEntry(carInfo, dispatcher);
                        carInfo.MapEntry = newMapEntry;
                        viewModel.MapEntries[carInfo.DeviceInfo.DeviceId] = newMapEntry;
                        if (newMapEntry.EntryObject != null)
                        {
                            viewModel.MapEntriesForMpgMap.Add(newMapEntry.EntryObject);
                        }
                    }
                }

                // 運行車両リストの選択行を取得
                cancelToken.ThrowIfCancellationRequested();
                MapEntryInfo selectedEntry = carListBindingSource.Current as MapEntryInfo;
                dispatcher.Invoke(() =>
                {
                    gridCarList.Enabled = false;
                    radioButtonALL.AutoCheck = false;
                    radioButtonSelect.AutoCheck = false;
                    drawAllCar = radioButtonALL.Checked;
                });

                // 運行車両リスト更新
                cancelToken.ThrowIfCancellationRequested();
                var carList = viewModel.MapEntries.Values.ToList();
                carList.Sort((a, b) => string.Compare(a.DeviceId, b.DeviceId));
                if (selectedEntry != null)
                {
                    selectedEntry = carList.FirstOrDefault(x => x.DeviceId == selectedEntry.DeviceId);
                }

                cancelToken.ThrowIfCancellationRequested();
                dispatcher.Invoke(() =>
                {
                    // BindingListを一旦クリアしているせいか、
                    // CurrentChangedイベントが2度走り、地図の描画がうまくいかない場合があるので
                    // リスト更新前にイベントを外し、更新後に再度イベントを付加するようにした。
                    carListBindingSource.SuspendBinding();
                    carListBindingSource.CurrentChanged -= CarListBindingSource_CurrentChanged;
                    viewModel.BindingCarList.Clear();
                    foreach (MapEntryInfo info in carList)
                    {
                        viewModel.BindingCarList.Add(info);
                    }
                    if (carListSortedColumn != null)
                    {
                        viewModel.BindingCarList.Sort(mapEntryInfoComparer);
                        carListSortedColumn.HeaderCell.SortGlyphDirection = mapEntryInfoComparer.Order;
                    }
                    carListBindingSource.ResumeBinding();
                    int selectedIndex = carListBindingSource.IndexOf(selectedEntry);
                    if (selectedIndex != -1)
                    {
                        carListBindingSource.Position = selectedIndex;
                    }
                    else
                    {
                        carListBindingSource.Position = 0;
                    }

                    // 地図描画
                    cancelToken.ThrowIfCancellationRequested();
                    if (drawAllCar)
                    {
                        mpgMap.MapCenter = mpgMap.GetMapCenter();
                    }
                    else
                    {
                        // 選択車両がなし(AccOff、営業所切り替え時)
                        if (selectedEntry == null ||
                                !viewModel.MapEntries.TryGetValue(selectedEntry.DeviceId, out MapEntryInfo centerEntry))
                        {
                            centerEntry = carListBindingSource.Current as MapEntryInfo;
                        }
                        viewModel.MapEntriesForMpgMap.Clear();
                        if (centerEntry != null && centerEntry.EntryObject != null)
                        {
                            MoveToMapCenter(centerEntry);
                            viewModel.MapEntriesForMpgMap.Add(centerEntry.EntryObject);
                        }
                    }
                    mpgMap.SetCustomData(viewModel.MapEntriesForMpgMap);
                    mpgMap.RePaint();

                    carListBindingSource.CurrentChanged += CarListBindingSource_CurrentChanged;
                    radioButtonALL.AutoCheck = true;
                    radioButtonSelect.AutoCheck = true;
                    gridCarList.Enabled = true;
                });
            }
            else
            {
                // 所属車両無し
                dispatcher.Invoke(() =>
                {
                    mpgMap.SetCustomData(viewModel.MapEntriesForMpgMap);
                    mpgMap.RePaint();
                    carListBindingSource.SuspendBinding();
                    carListBindingSource.Clear();
                    carListBindingSource.ResumeBinding();
                    radioButtonALL.AutoCheck = true;
                    radioButtonSelect.AutoCheck = true;
                    gridCarList.Enabled = true;
                });
            }
        }

        private void SetMapEntryForMpgMap(MapEntryInfo mapEntry)
        {
            viewModel.MapEntriesForMpgMap.Clear();
            if (mapEntry != null && mapEntry.EntryObject != null)
            {
                viewModel.MapEntriesForMpgMap.Add(mapEntry.EntryObject);
                mpgMap.SetCustomData(viewModel.MapEntriesForMpgMap);
            }
        }

        private void SetMapEntryForMpgMap()
        {
            viewModel.MapEntriesForMpgMap.Clear();
            foreach (var mapEntry in viewModel.MapEntries.Values)
            {
                if (mapEntry.EntryObject != null)
                {
                    viewModel.MapEntriesForMpgMap.Add(mapEntry.EntryObject);
                }
            }
            mpgMap.SetCustomData(viewModel.MapEntriesForMpgMap);
        }

        /// <summary>
        /// 運行車両情報の地図オブジェクトを作成する。
        /// </summary>
        /// <param name="carInfo">車両情報</param>
        /// <returns></returns>
        private MapEntryInfo CreateMapEntry(CarInfo carInfo, Dispatcher dispatcher)
        {
            MapEntryInfo result = new MapEntryInfo(dispatcher)
            {
                DeviceId = carInfo.DeviceInfo.DeviceId,
                CarId = carInfo.DeviceInfo.CarId
            };

            //  地図上の座標・住所取得
            MqttJsonLocation location = carInfo.GetLocation();
            if (!string.IsNullOrEmpty(location.Lat) && !string.IsNullOrEmpty(location.Lon))
            {
                PointLL center = MapUtils.ConvertJdgToTky(location.Lat, location.Lon);
                result.MapCoordinate = center;

                //MapUtils.FindAddress(result.MapCoordinate, result.FindAddressFinished);
                MapUtils.FindAddress(location.Lat, location.Lon, result.FindAddressFinished);

                //  基本
                TextObject entryObject = new TextObject
                {
                    Title = carInfo.DeviceInfo.DeviceId,
                    TitleFont = new Font("Meiryo UI", 9),
                    CommentFont = new Font("Meiryo UI", 9),
                    pen = new Pen(Color.Orange, 1),
                    brush = new SolidBrush(Color.White),
                    TextColor = Color.Black
                };

                //  エラーステータス
                result.ErrorCode = 0;
                result.ErrorMessage = DeviceErrorCode.MakeErrorMessage(result.ErrorCode);  // 「正常」で初期化
                if (carInfo.ErrorCode != null)
                {
                    result.ErrorCode = carInfo.ErrorCode.GetErrorCode();
                    result.ErrorMessage = DeviceErrorCode.MakeErrorMessage(carInfo.ErrorCode.GetErrorCode());
                    if (result.ErrorCode != 0)
                    {
                        entryObject.brush = new SolidBrush(DeviceErrorCode.GetBackgroundColor(carInfo.ErrorCode.GetErrorCode()));
                        entryObject.Comment = result.ErrorMessage;
                    }
                }
                // 座標を設定する
                entryObject.Point = center;
                // 原点の描画を設定する
                entryObject.OriginStyle = MpgCustomEnum.OriginStyleEnum.Icon;

                //  テキスト表示
                entryObject.BallonStyle = MpgCustomEnum.BallonStyleEnum.TitleAndComment;
                entryObject.TextZoom = 2.0f;

                // アイコン
                entryObject.Icon = m_customProperty.Icon;
                entryObject.IconZoom = MpgCustomEnum.IconZoomLevelEnum.Level2; // アイコン拡大率

                // 予め決めた円周上に配置する
                entryObject.Offset = viewModel.GetRandomBalloonOffset();

                //  車両ID情報追加
                entryObject.ID = carInfo.DeviceInfo.DeviceId;
                result.EntryObject = entryObject;
            }
            return result;
        }

        /// <summary>
        /// 地図オブジェクトを地図の中心に据える
        /// </summary>
        /// <param name="mapEntry"></param>
        private void MoveToMapCenter(MapEntryInfo mapEntry)
        {
            if (mapEntry != null && mapEntry.MapCoordinate != null)
            {
                mpgMap.MapCenter = mapEntry.MapCoordinate;
                mpgMap.RePaint();
            }
        }

        /// <summary>
        /// 車両リスト選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CarListBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            var item = carListBindingSource.Current as MapEntryInfo;
            if (radioButtonSelect.Checked && item != null)
            {
                var dispatcher = Dispatcher.CurrentDispatcher;
                // 地図描画リスト再作成
                Task.Run(async () =>
                {
                    await Task.Delay(10);
                    dispatcher.Invoke(() =>
                    {
                        MoveToMapCenter(item);
                        SetMapEntryForMpgMap(item);
                    });
                });
            }
        }

        /// <summary>
        /// 車両リストクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCarList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                MapEntryInfo selectedItem = carListBindingSource.Current as MapEntryInfo;
                if (selectedItem != null)
                {
                    if (httpSequence.GroupCars.TryGetValue(m_SelectOfficeID, out Dictionary<string, CarInfo> carInfoList))
                    {
                        CarInfo carInfo = carInfoList[selectedItem.DeviceId];
                        int errorCode = (carInfo.ErrorCode == null) ? 0 : carInfo.ErrorCode.GetErrorCode();
                        LeftPanelShow(selectedItem.DeviceId, errorCode);
                    }
                }
            }
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
            DataGridView grid = sender as DataGridView;
            if (grid.Columns[e.ColumnIndex].SortMode != DataGridViewColumnSortMode.NotSortable)
            {
                var current = carListBindingSource.Current;
                MapEntryInfoComparer.SortMode sortMode = MapEntryInfoComparer.SortMode.DeviceId;
                switch (e.ColumnIndex)
                {
                    case 0:
                        sortMode = MapEntryInfoComparer.SortMode.DeviceId;
                        break;

                    case 1:
                        sortMode = MapEntryInfoComparer.SortMode.Address;
                        break;
                }
                mapEntryInfoComparer.Mode = sortMode;
                if (carListSortedColumn == null || carListSortedColumn != grid.Columns[e.ColumnIndex])
                {
                    if (carListSortedColumn != null)
                    {
                        carListSortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }
                    mapEntryInfoComparer.Order = SortOrder.Descending;
                    carListSortedColumn = grid.Columns[e.ColumnIndex];
                }
                else
                {
                    if (mapEntryInfoComparer.Order == SortOrder.Descending)
                    {
                        mapEntryInfoComparer.Order = SortOrder.Ascending;
                    }
                    else
                    {
                        mapEntryInfoComparer.Order = SortOrder.Descending;
                    }
                }
                viewModel.BindingCarList.Sort(mapEntryInfoComparer);
                carListSortedColumn.HeaderCell.SortGlyphDirection = mapEntryInfoComparer.Order;
                int index = carListBindingSource.IndexOf(current);
                carListBindingSource.Position = (index == -1) ? 0 : index;
            }
        }

        /// <summary>
        /// 選択車両表示変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonSelect_CheckedChanged(object sender, EventArgs e)
        {
            var item = carListBindingSource.Current as MapEntryInfo;
            if (item != null)
            {
                MoveToMapCenter(item);
                if ((sender as RadioButton).Checked)
                {
                    // 選択車両表示
                    SetMapEntryForMpgMap(item);
                }
                else
                {
                    // 全車両表示
                    SetMapEntryForMpgMap();
                }
            }
        }

        /// <summary>
        /// ズームバーにおけるマウスアップイベントを処理する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomBar_MouseUp(object sender, MouseEventArgs e)
        {
            // ズームバーの値と地図のスケールが一致する場合は何もしない。
            if (mpgMap.MapScale != viewModel.MapScales[zoomBar.Value])
            {
                // 地図のスケールを変更する。
                mpgMap.MapScale = viewModel.MapScales[zoomBar.Value];
                SyncMapScale(false);
                mpgMap.RePaint();  // 地図を再描画する。
            }
        }

#endregion フォームイベント処理

#region 地図コントロールイベント処理

        /// <summary>
        /// カスタム情報のヒットテスト結果取得イベント
        /// 吹出しを右クリックした時に呼ばれる。
        /// 該当する運行車情報を選択肢サイドパネルを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpgMap_CustomObjectHit(object sender, CustomObjectHitArgs e)
        {
            if (viewModel.MapEntries.TryGetValue(e.ID, out MapEntryInfo mapEntry))
            {
                int index = viewModel.BindingCarList.IndexOf(mapEntry);
                carListBindingSource.Position = index;
                if (httpSequence.AllCars.TryGetValue(e.ID, out CarInfo carInfo))
                {
                    //m_SelectCarID = carInfo.DeviceInfo.DeviceId;
                    int errorCode = (carInfo.ErrorCode == null) ? 0 : carInfo.ErrorCode.GetErrorCode();
                    LeftPanelShow(mapEntry.DeviceId, errorCode);
                }
            }
            ChangeMapMode(MapMode.Write);
        }

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpgMap_MouseMove(object sender, MouseEventArgs e)
        {
            // 地図操作モードに応じて、マウスカーソルを変更する。
            if (mpgMap.MapMode != MapMode.Write)
            {
                // 移動モードの場合
                mpgMap.Cursor = Cursors.Hand;
            }
        }

        /// <summary>
        /// 標準的なマウスダウンイベント
        /// 右クリックのときカスタム情報選択判定を行う。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpgMap_MouseDown(object sender, MouseEventArgs e)
        {
            ChangeMapMode(MapMode.Move);

            // ピクセル位置を緯度経度に変換する。
            Point[] pt = new Point[] { mpgMap.PointToClient(Cursor.Position) };
            PointLL[] ptl = mpgMap.Convert(pt);

            // ヒットテストを実施する。（結果はイベントで取得する）
            if (e.Button == MouseButtons.Right)
            {
                mpgMap.CustomObjectHitTest(ptl[0]);
            }
        }

        /// <summary>
        /// 標準的なマウスホイール操作イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpgMap_MouseWheel(object sender, EventArgs e)
        {
            // 縮尺変更を行う。
            SyncMapScale(true);
        }

#endregion 地図コントロールイベント処理

#region イベントタブ イベント

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            labelGstatus.Text = string.Empty;

            //  閲覧権限あり
            if (AuthedUser.Item.Can(Permission.StreamingView) || AuthedUser.Item.Can(Permission.Engineer))
            {
                if (progressBarDownload.Visible == false)
                {
                    //  チェックは1個所限定
                    for (int i = 0; i < gridEventList.Rows.Count; i++)
                    {
                        if (gridEventList.Rows[i].Cells[0].Value != null)
                        {
                            if (gridEventList.Rows[i].Cells[0].Value.ToString() == "1")
                            {
                                var x = (EventInfo)gridEventList.Rows[i].DataBoundItem;
                                EventListDownload(gridEventList, x);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("閲覧する権限がありません", "確認", MessageBoxButtons.OK
                                   , MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            }
        }

        //  イベントダウンロード
        private async void EventListDownload(DataGridView dgv, EventInfo ei)
        {
            try
            {
                operationLogger.Out(OperationLogger.Category.EventData, AuthedUser.Item.Name, $"Download event {ei.CarId}, {ei.Timestamp:F}");

                Debug.WriteLine($"Download MovieId: {ei.MovieId}");

                if (ei.IsDownloadable)
                {
                    progressBarDownload.Visible = true;
                    labelDownloadStatus.Visible = true;
                    panelPlay.Visible = false;
                    buttonDownload.Enabled = false;

                    downloadCancelTokenSource = new CancellationTokenSource();
                    Task task = httpSequence.DownloadEventData(ei.MovieId, (result) =>
                    {
                        ei.IsDownloading = false;
                        if (result.Result == 0)
                        {
                            ei.IsDownloadable = false;
                            ei.IsPlayable = true;
                            ei.unzippedPath = result.UnzippedPath;
                            ei.ChannelToMovieFileDic = result.chFile;

                            tempPaths.Add(result.UnzippedPath, true);
                        }
                        else
                        {
                            ei.IsDownloadable = true;
                        }
                    }, downloadCancelTokenSource.Token);

                    await task;
                }

                PrePostPlayEx(ei);

                progressBarDownload.Visible = false;
                labelDownloadStatus.Visible = false;
                labelPanelPlayCarId.Text = ei.CarId;
                labelPanelPlayDate.Text = ei.Timestamp.ToString();
                panelPlay.Visible = true;
                buttonDownload.Enabled = true;

                Debug.WriteLine($"DownloadEvent end ");
            }
            catch (OperationCanceledException)
            {
                /// ダウンロード強制キャンセル
                /// キャンセルボタンは無いので、営業所変更イベントの切っ掛けと判断する。
                /// その為、プログレスバー、文言、ボタンのVisible, Enableをfalseとする。
                Debug.WriteLine("EvnetDownload force cancel.");
                progressBarDownload.Visible = false;
                labelDownloadStatus.Visible = false;
                panelPlay.Visible = false;
                buttonDownload.Enabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ERROR: EventDownload_Click {ex}");
                progressBarDownload.Visible = false;
                labelDownloadStatus.Visible = false;
                panelPlay.Visible = false;
                buttonDownload.Enabled = true;
            }
        }

        /// <summary>
        /// イベントリストのフォーマット指定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridEventList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (e.Value != null && e.Value is DateTime time)
                {
                    e.Value = time.ToString("yyyy/MM/dd HH:mm:ss");
                }
            }
        }

        /// <summary>
        /// イベントリストの列クリックイベント<br/>
        /// ソート処理を実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridEventList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (grid.Columns[e.ColumnIndex].SortMode != DataGridViewColumnSortMode.NotSortable)
            {
                EventInfoComparer.SortMode sortMode = EventInfoComparer.SortMode.TimeStamp;
                switch (e.ColumnIndex)
                {
                    case 1:
                        sortMode = EventInfoComparer.SortMode.TimeStamp;
                        break;

                    case 2:
                        sortMode = EventInfoComparer.SortMode.DeviceId;
                        break;

                    case 3:
                        sortMode = EventInfoComparer.SortMode.EventType;
                        break;

                    case 4:
                        sortMode = EventInfoComparer.SortMode.Remark;
                        break;
                }

                eventComparer.Mode = sortMode;
                if (eventListSortedColumn == null || eventListSortedColumn != grid.Columns[e.ColumnIndex])
                {
                    if (eventListSortedColumn != null)
                    {
                        eventListSortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }
                    if (sortMode == EventInfoComparer.SortMode.TimeStamp)
                    {
                        // 日時はデフォルトが降順なので昇順へ
                        eventComparer.Order = SortOrder.Ascending;
                    }
                    else
                    {
                        eventComparer.Order = SortOrder.Descending;
                    }
                    eventListSortedColumn = grid.Columns[e.ColumnIndex];
                }
                else
                {
                    if (eventComparer.Order == SortOrder.Ascending)
                    {
                        eventComparer.Order = SortOrder.Descending;
                    }
                    else
                    {
                        eventComparer.Order = SortOrder.Ascending;
                    }
                }
                httpSequence.Events.Sort(eventComparer);
                eventListSortedColumn.HeaderCell.SortGlyphDirection = eventComparer.Order;
            }
        }

#endregion イベントタブ イベント

        private Dictionary<string, object> dc = new Dictionary<string, object>();
        private EventInfo ei;
        private List<Button> buttonPlays = new List<Button>();
        private Dictionary<Button, int> buttonToChannel = new Dictionary<Button, int>();

        public void PrePostPlayEx(EventInfo ei)
        {
            this.ei = ei;

            //dc.Add("PrePostEvent", ei);

            //DataContext = this.ei;
            buttonPlays.Clear();
            buttonPlays.Add(buttonPlay1);
            buttonPlays.Add(buttonPlay2);
            buttonPlays.Add(buttonPlay3);
            buttonPlays.Add(buttonPlay4);
            buttonPlays.Add(buttonPlay5);
            buttonPlays.Add(buttonPlay6);
            buttonPlays.Add(buttonPlay7);
            buttonPlays.Add(buttonPlay8);
            int ch = 0;
            buttonToChannel.Clear();
            foreach (var b in buttonPlays)
            {
                buttonToChannel.Add(b, ch);
                ch++;
            }

            foreach (var key in ei.ChannelToMovieFileDic.Keys)
            {
                if (key < buttonPlays.Count)
                {
                    buttonPlays[key].Enabled = true;
                }
            }
        }

        private void buttonPlay1_Click(object sender, EventArgs e)
        {
            if (buttonToChannel.ContainsKey((Button)sender))
            {
                string audioFile = null;
                if (ei.ChannelToMovieFileDic.ContainsKey(EventInfo.AUDIO_CHANNEL_BASE))
                {
                    audioFile = ei.ChannelToMovieFileDic[EventInfo.AUDIO_CHANNEL_BASE];
                }
                var ch = buttonToChannel[(Button)sender];
                if (ei.ChannelToMovieFileDic.ContainsKey(ch))
                {
                    var title = $"Event {ei.CarId} | {ei.Timestamp} | Ch{ch + 1}";
                    operationLogger.Out(OperationLogger.Category.EventData, AuthedUser.Item.Name, $"Play {title}");
                    int fps = 15;
                    if (ch > 0)
                    {
                        fps = 10; // for Analog.
                    }
                    PlayffmpegControl(ch, ei.ChannelToMovieFileDic[ch], audioFile, title, 480, fps);
                }
            }
        }

        /// <summary>
        /// ffmpeg で映像と音声をミックスし、それをVLCで再生する。
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="filepath"></param>
        /// <param name="audioFilePath"></param>
        /// <param name="windowTitle"></param>
        /// <param name="windowHeight"></param>
        /// <param name="fps"></param>
        private void PlayffmpegControl(int ch, string filepath, string audioFilePath, string windowTitle, int windowHeight, int fps)
        {
            Debug.WriteLine($"PlayffmpegControl {filepath}, {audioFilePath}");
            var task = ffmpegCtrl.PlayMovie(ch, filepath, audioFilePath, fps, new Action<PlayMovieProgress>((args) =>
            {
                var msg = string.Empty;
                if (PlayMovieStatus.PreProcess == args.PlayMovieStatus)
                {
                    msg = @"準備中です...";
                }
                Invoke((MethodInvoker)(() =>
                {
                    labelEventProc.Text = msg;
                }));
            }));
        }

        public string GetUserAuthPath()
        {
#if USE_SREX
            return Path.Combine(new string[] {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                @"ISSUI",
                @"UserAuth",
                @"UserAuth.exe"
            });
#else
            string result = @"UserIdentify.exe";
            return result;
#endif
        }

        private void StartUserAuth()
        {
#if USE_SREX
            try
            {
                userAuthProcess = new Process();
                // Hide console window. Show player window.
                //                userAuthProcess.StartInfo.UseShellExecute = false;
                //                userAuthProcess.StartInfo.CreateNoWindow = true;

                userAuthProcess.StartInfo.FileName = GetUserAuthPath();
                if (File.Exists(userAuthProcess.StartInfo.FileName))
                {
                    int perm = (int)UserBio.Permission.StreamingView;
                    authResultFile = Path.GetTempPath() + Path.GetRandomFileName();
                    userAuthProcess.StartInfo.Arguments = $"--perm {perm} --out {authResultFile}";
                    userAuthProcess.Exited += new EventHandler(ExitedUserAuthHandler);
                    userAuthProcess.EnableRaisingEvents = true;
                    userAuthProcess.Start();
                }
                else
                {
                    Debug.WriteLine(@"Not exists UserAuth.exe");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"@@@ Error: StartUserAuth {e}");
                DeleteAuthResultFile();
            }
#else
            userAuthDp = new UserAuthDp();
            userAuthDp.IdentifiedResultEvent += UserAuthDp_IdentifiedResultEvent;
            userAuthDp.StartAuth();
#endif
        }

        private void UserAuthDp_IdentifiedResultEvent(object sender, IdentifiedResult e)
        {
            // 認証結果を得る。
            if (e.ResultCode == 0)
            {
                // 権限を確認する
                if (e.User.Can(Permission.StreamingView) || e.User.Can(Permission.Engineer))
                {
                    AuthedUser.Item = e.User;
                    AuthedUser.Item.Name = string.IsNullOrEmpty(e.User.Name) ? string.Empty : e.User.Name;
                    Debug.WriteLine($"---- User: {e.User.Name}, 0x{e.User.Permission:X8}");
                    m_UserAuthComplete = true;

                    operationLogger.Out(OperationLogger.Category.Authentication, e.User.Name, @"Authenticated");
                    Invoke((MethodInvoker)(() =>
                    {
                        if (m_wform != null && m_wform.IsDisposed)
                        {
                            TabControlUpdate(true);
                        }
                        UpdateUiAtUserAuthed();
                    }));
                }
                else
                {
                    // 権限なし。
                    // 北畠殿が作ったformsアプリで登録したユーザーは全て、なにも権限を持っていない。権限をDBに格納していなかったから。
                    // 何かしらメッセージを出す必要がある。
                    Debug.WriteLine($"A User \"${e.User.Name}\" does not have the necessary permissions.");
                    if (formMessage == null)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            formMessage = new FormMessage();
                            formMessage.FormClosed += FormMessage_FormClosed;
                            formMessage.Title = @"権限不足";
                            formMessage.Message = "登録済みユーザーではありますが、権限が不足しています。";
                            formMessage.MessageColor = Color.Red;
                            formMessage.AutoCloseMillisec = 5000; // 時間が来たら閉じる。
                            formMessage.Show();
                        }));
                    }
                }
                if (tabControlMain.TabPages.ContainsKey(TabPageConfigName))
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        tabControlMain.TabPages.RemoveByKey(TabPageConfigName);
                    }));
                }
            }
        }

        private void FormMessage_FormClosed(object sender, FormClosedEventArgs e)
        {
            formMessage.Dispose();
            formMessage = null;
        }

#if USE_SREX
        private void DeleteAuthResultFile()
        {
            if (authResultFile != null && authResultFile.Length > 0)
            {
                try
                {
                    File.Delete(authResultFile);
                }
                catch { }
                finally
                {
                    authResultFile = null;
                }
            }
        }
#endif

        private delegate void UpdateTabcontrolDelegate();

        private void TabControlUpdate(bool enable)
        {
            tabControlMain.Enabled = enable;
            gridCarList.Enabled = enable;
        }

        private UserBio.User ReadAuthedUser(string filepath)
        {
            long id = -1;
            using (var file = new StreamReader(filepath))
            {
                string lastLine = string.Empty;
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    lastLine = line;
                }

                if (lastLine.Length > 0)
                {
                    var words = lastLine.Split('\t');
                    try
                    {
                        id = Int64.Parse(words[1]);
                    }
                    catch
                    { }
                }
            }

            if (id >= 0)
            {
                return userDatabase.GetUser(id);
            }
            return null;
        }

        private void TerminateUserAuth()
        {
#if USE_SREX
            if (userAuthProcess != null)
            {
                try
                {
                    if (!userAuthProcess.HasExited)
                    {
                        userAuthProcess.CloseMainWindow();
                        userAuthProcess.Close();
                    }
                    userAuthProcess.Kill();
                }
                catch { }
            }
            userAuthProcess = null;
#else
            if (userAuthDp != null)
            {
                userAuthDp.Dispose();
            }
#endif
        }

        private void SkipUserAuth(object parameter)
        {
#if USE_SREX
            Debug.WriteLine($"SkipUserAuth {parameter}");
            AuthedUser.Item = new UserBio.User()
            {
                Id = -1,
                Name = @"----",
                Permission = 0xFFFF,
            };
#endif
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //            var httpSequenceTask = httpSequence.UpdateDeviceList(m_SelectOfficeID, false);
            MapEntryInfo current = carListBindingSource.Current as MapEntryInfo;
            this.buttonUpdateDeviceListUseDebugOnly.Text = current.DeviceId;
        }

        private void MoveToOfficeLocation()
        {
            var office = officeInfoBindingSource.Current as OfficeInfo;
            if (office != null && office.Location != null)
            {
                mpgMap.MapCenter = new PointLL(office.Location.Value.latitude, office.Location.Value.longitude);
                mpgMap.RePaint();
            }
        }

        private void TimerStartMQTT_Tick(object sender, EventArgs e)
        {
            //  Deviceリスト 取得後から起動するのが理想
            timerStartMQTT.Enabled = false;
            ConnectMQTTServer();
        }

        private void LeftPanelShow(string CarId, int ErrorCode)
        {
            panelLeft.Enabled = true;
            panelLeft.Width = LEFT_PANEL_WIDTH;
            labelRtRetryStatus.Text = string.Empty;
            if (AuthedUser.Item.Can(Permission.StreamingView))
            {
                tabControlRtSelect.Enabled = true;
                labelRtStatus.Text = string.Empty;
            }
            else
            {
                tabControlRtSelect.Enabled = false;
                labelRtStatus.Text = @"権限がありません";
            }

            var msg = @"正常";
            if (ErrorCode > 0)
            {
                msg = DeviceErrorCode.MakeErrorMessage(ErrorCode);
            }
            labelRtCarStatus.Text = msg;
            labelRtCarId.Text = CarId;

            tableLayoutPanelStreamingOnCar.Visible = false;
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
                gridCarList.Columns[0].MinimumWidth = 100;
                gridCarList.Columns[0].Width = 100;
                //tabControlRtSelect.Selecting += TabControlRtSelect_Selecting;
            }
        }

        //private void TabControlRtSelect_Selecting(object sender, TabControlCancelEventArgs e)
        //{
        //    if (e.TabPage == tabPageRemoteConfig || 
        //        e.TabPage == tabPageEvent)
        //    {
        //        e.Cancel = true;
        //    }
        //}

        private void buttonLeftPanelClose_Click(object sender, EventArgs e)
        {
            if (!isStreaming)
            {
                LeftPanelHide();
            }
        }

        private Dictionary<ushort, PublishedInfo> pubRecord = new Dictionary<ushort, PublishedInfo>();

        private void Client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (pubRecord.TryGetValue(e.MessageId, out PublishedInfo publishedInfo))
            {
                if (publishedInfo != null)
                {
                    Debug.WriteLine($"*** Client_MqttMsgPublished: {e.MessageId}, {pubRecord[e.MessageId]}");
                    pubRecord[e.MessageId].PublishedHandler(e.MessageId, pubRecord[e.MessageId]);
                }
                pubRecord.Remove(e.MessageId);
            }
        }

        private void Client_ConnectionClosed(object sender, EventArgs e)
        {
            mqttClient = null;

            // 再接続する。Form Closed 時、再接続しないよう、先にこのイベントハンドラを取り除くこと！
            if (reconnectTimer != null)
            {
                reconnectTimer.Dispose();
                reconnectTimer = null;
            }
            reconnectTimer = new System.Timers.Timer(10 * 1000);
            reconnectTimer.Elapsed += ReconnectTimer_Elapsed;
            reconnectTimer.AutoReset = true; // true:繰り返す。
            reconnectTimer.Enabled = true;
        }

        private void ReconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine($"MQTT try reconnect {e.SignalTime}");
            if (mqttClient == null)
            {
                ConnectMQTTServer();
            }
            else
            {
                if (mqttClient.IsConnected)
                {
                    Debug.WriteLine($"MQTT reconnect succeeded");
                    reconnectTimer.AutoReset = false;
                    reconnectTimer.Enabled = false;
                }
                else
                {
                    // 切断してからリトライ。
                    try
                    {
                        mqttClient.Disconnect();
                        mqttClient = null;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"@@@ Exception : {ex}, {ex.StackTrace}");
                    }
                }
            }
        }

#region "リアルタイム再生関係"

        // ################################################################
        // ソースが長い要因となってごめん。
        // * ダイアログだと処理を阻害する。ウィンドウだと裏に隠れていしまい、クレームがうるさい。
        // * Forms では MVVM がものすごく面倒で、拡張入れれば何とかというレベル。
        // これらの理由から、MainForm埋め込みとなっている。
        //
        // もっとちゃんと管理したいなら、StreamingStatus クラスへ配信関係のメンバ変数を移動し、管理をクラス単位とすると良いかも。
        // そうすれば、複数台の車載器のストリーミングを見れるかと思う。Taskへぶん投げてしまえば管理もTask単位になって楽だろうし。

        private const string MQTT_TOPIC_STREAMING_REQUEST = "car/streaming/{0}/request";
        private const string MQTT_TOPIC_REMOTECONFIG_REQUEST = "car/request/{0}/conf";

        private int streamingRequestRetryCount;  // ストリーミング配信要求のリトライ回数
        private int streamingSessionRetryCount;  // ストリーミングセッションのリトライ回数(配信要求からやり直し)
        private bool isStreaming;
        private Process ffmpeg = null;
        private Task taskStreamingRequest;
        private int streamingChannel;
        private string streamingDeviceId;
        private System.Timers.Timer streamingCommandTimer;
        private System.Timers.Timer streamingWatcingTimer;
        private DateTime streamingWatchingStartTime;

        /// <summary>
        /// リアルタイム開始ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStreamStart_Click(object sender, EventArgs e)
        {
            var mapEntry = carListBindingSource.Current as MapEntryInfo;
            if (!isStreaming && mapEntry != null)
            {
                operationLogger.Out(OperationLogger.Category.Streaming, AuthedUser.Item.Name, $"Start Streaming {mapEntry.DeviceId}");
                streamingChannel = 0;
                StartStreaming(mapEntry.DeviceId);
                streamingRequestRetryCount = 0;
                streamingSessionRetryCount = 0;
                UpdateStreamingUiStatus(StreamingUiStatus.Start);
                UpdateStreamingWatchElapsed(TimeSpan.Zero);
            }
        }

        private void buttonStreamStop_Click(object sender, EventArgs e)
        {
            if (isStreaming)
            {
                StopStreaming();
            }
        }

        private bool IsStreaming(string deviceId)
        {
            bool result = false;
            if (isStreaming && streamingDic.ContainsKey(deviceId))
            {
                result = true;
            }
            return result;
        }

        private void StopStreaming()
        {
            progressBarRtStart.Visible = false;
            StopStreaming(streamingDeviceId);
            streamingRequestRetryCount = 0;
            streamingSessionRetryCount = 0;
            operationLogger.Out(OperationLogger.Category.Streaming, AuthedUser.Item.Name, $"End Streaming {streamingDeviceId}");
            gridCarList.Enabled = true;
        }

        private async Task PlayStreaming(bool needDelay)
        {
            Debug.WriteLine(@"Start PlayStreaming");
            ServerInfo serverInfo = operationServerInfo.GetPhygicalServerInfo();
            if (needDelay)
            {
                // あまり早くても、サーバーが.mpdファイルを作ってくれないから少し待つ。10秒だと早い。
                // .mpd ファイルへ何か(条件不明)よりも早くアクセスしてしまうと、その配信セッション間はずっと.mpdは作られない。
                // もし、その状況に陥った場合は、配信終了後、再び開始すると回復する可能性がある。
                // 車載器としては、インデックスサーバーへアクセスしなおして、新しいセッション情報で再開することになる。
                Debug.WriteLine($"PlayStreaming waiting {streamingBeforeWaitSec}sec.");
                await Task.Delay(streamingBeforeWaitSec * 1000);

                // 待機中に「終了」ボタンを押されているかもしれない。そのときは何もしないで終わる。
                if (string.IsNullOrEmpty(streamingDeviceId)
                    || (streamingDic.ContainsKey(streamingDeviceId) && streamingDic[streamingDeviceId].State == StreamingState.None))
                {
                    Debug.WriteLine(@"Abort PlayStreaming");
                    return;
                }
            }

            bool needRestart = true;
            var ssi1 = await StreamingRequest.GetStreamingInfo(httpClient, serverInfo.HttpAddr, streamingDeviceId);
            if (ssi1 != null && ssi1.hasServerInfo)
            {
                var ssi2 = await StreamingRequest.ExistsStreamingMpdFile(httpClient, ssi1);
                if (ssi2 != null && ssi2.existsMpd)
                {
                    StartStreamingFfmpeg(ssi2);
                    needRestart = false;
                }
                else
                {
                    streamingBeforeWaitSec += Properties.Settings.Default.StreamingBeforeWaitAddSec;
                    if (streamingBeforeWaitSec > Properties.Settings.Default.StreamingBeforeWaitMaxSec)
                    {
                        streamingBeforeWaitSec = Properties.Settings.Default.StreamingBeforeWaitMaxSec;
                    }
                }
            }

            if (string.IsNullOrEmpty(streamingDeviceId)
                || (streamingDic.ContainsKey(streamingDeviceId) && streamingDic[streamingDeviceId].State == StreamingState.None))
            {
                Debug.WriteLine(@"Abort PlayStreaming");
                return;
            }

            if (needRestart)
            {
                //streamingRequestRetryCount++;
                //Debug.WriteLine($"*** PlayStreaming retry: {streamingRequestRetryCount}");
                streamingSessionRetryCount++;
                Debug.WriteLine($"*** PlayStreaming retry: {streamingSessionRetryCount}");

                // StopStreamingを発行するとstreamingDeviceIdがnullになるのでDeviceIdを再開用に保持してから
                // ストリーミングを一旦、終了する。
                // ストリーミングセッションのリトライを行う場合は、再度、ストリーミングを再開する
                // リトライ回数を超えたら終了する。
                if (streamingSessionRetryCount <= localSettings.StreamingSessionRetryCount)
                {
                    // UIスレッドの更新。
                    // ストリーミングを終了してから、再度開始する。
                    // 車載器としては、インデックスサーバーから情報を再度貰う。セッションのやり直し。
                    RestartStreaming(streamingDeviceId);
                    Invoke((MethodInvoker)(() =>
                    {
                        UpdateStreamingUiStatus(StreamingUiStatus.Start);
                        UpdateStreamingWatchElapsed(TimeSpan.Zero);
                        tableLayoutPanelStreamingOnCar.Visible = false;
                        labelStreamingSessionRetry.Text = $"配信セッションリトライ({streamingSessionRetryCount})";
                    }));
                }
                else
                {
                    // 通信状態不良と判断して中止。
                    StopStreaming(streamingDeviceId);
                    Invoke((MethodInvoker)(() =>
                    {
                        UpdateStreamingUiStatus(StreamingUiStatus.RetryOver);
                    }));
                }
            }
            Debug.WriteLine(@"End PlayStreaming");
        }

        private async Task PlayRtspStreaming(bool needDelay)
        {
            Debug.WriteLine(@"Start PlayStreaming");
            var serverInfo = operationServerInfo.GetPhygicalServerInfo();
            if (needDelay)
            {
                Debug.WriteLine($"PlayStreaming waiting {streamingBeforeWaitSec}sec.");

                // 待機中に「終了」ボタンを押されているかもしれない。そのときは何もしないで終わる。
                if (string.IsNullOrEmpty(streamingDeviceId)
                    || (streamingDic.ContainsKey(streamingDeviceId) && streamingDic[streamingDeviceId].State == StreamingState.None))
                {
                    Debug.WriteLine(@"Abort PlayStreaming");
                    return;
                }
            }

            var needRestart = true;
            var liveStream = await StreamingRequest.GetStreamingInfo(httpClient, serverInfo.HttpAddr, streamingDeviceId);
            if (liveStream != null && liveStream.hasServerInfo)
            {
                // RTSPで視聴
                ViewRtspStreamingAsync(liveStream);
                needRestart = false;
            }

            if (needRestart)
            {
                streamingSessionRetryCount++;
                Debug.WriteLine($"*** PlayStreaming retry: {streamingSessionRetryCount}");

                // StopStreamingを発行するとstreamingDeviceIdがnullになるのでDeviceIdを再開用に保持してから
                // ストリーミングを一旦、終了する。
                // ストリーミングセッションのリトライを行う場合は、再度、ストリーミングを再開する
                // リトライ回数を超えたら終了する。
                if (streamingSessionRetryCount <= localSettings.StreamingSessionRetryCount)
                {
                    // UIスレッドの更新。
                    // ストリーミングを終了してから、再度開始する。
                    // 車載器としては、インデックスサーバーから情報を再度貰う。セッションのやり直し。
                    RestartStreaming(streamingDeviceId);
                    Invoke((MethodInvoker)(() =>
                    {
                        UpdateStreamingUiStatus(StreamingUiStatus.Start);
                        UpdateStreamingWatchElapsed(TimeSpan.Zero);
                        tableLayoutPanelStreamingOnCar.Visible = false;
                        labelStreamingSessionRetry.Text = $"配信セッションリトライ({streamingSessionRetryCount})";
                    }));
                }
                else
                {
                    // 通信状態不良と判断して中止。
                    StopStreaming(streamingDeviceId);
                    Invoke((MethodInvoker)(() =>
                    {
                        UpdateStreamingUiStatus(StreamingUiStatus.RetryOver);
                    }));
                }
            }
            Debug.WriteLine(@"End PlayStreaming");
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
            var radio = sender as RadioButton;

            if (radio.Checked)
            {
                if (!isStreaming)
                {
                    radio.Checked = false;
                    return;
                }
                // Sunken
                radio.FlatStyle = FlatStyle.Popup;
                radio.BackColor = SystemColors.ControlDark;
                radio.UseVisualStyleBackColor = false;
                try
                {
                    if (int.TryParse(radio.Tag.ToString(), out int channel))
                    {
                        Debug.WriteLine($"MQTT Request a changes streaming channel: {channel}");
                        MqttStreamingCmd(streamingDeviceId, true, channel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
                }
            }
            else
            {
                radio.FlatStyle = FlatStyle.Standard;
                radio.BackColor = Color.CornflowerBlue;
                radio.UseVisualStyleBackColor = true;
            }
        }

        /// <summary>
        /// ストリーミングチャンネルのトルグボタンON/OFFを制御する<br/>
        /// </summary>
        /// <param name="isChecked"></param>
        private void CheckedStreamingChannel(bool isChecked, bool isFocus = false)
        {
            RadioButton radio;
            switch (streamingChannel)
            {
                case 1:
                    radio = radioButtonRtCh2;
                    break;

                case 2:
                    radio = radioButtonRtCh3;
                    break;

                case 3:
                    radio = radioButtonRtCh4;
                    break;

                case 4:
                    radio = radioButtonRtCh5;
                    break;

                case 5:
                    radio = radioButtonRtCh6;
                    break;

                case 6:
                    radio = radioButtonRtCh7;
                    break;

                case 7:
                    radio = radioButtonRtCh8;
                    break;

                case 0:
                default:
                    radio = radioButtonRtCh1;
                    break;
            }
            radio.Checked = isChecked;
            if (isFocus)
            {
                radio.Focus();
            }
        }

        /// <summary>
        /// ストリーミングチャンネルのトルグボタンを全てOFFにする<br/>
        /// </summary>
        private void UncheckedStreamingAllChannel()
        {
            List<RadioButton> radioButtons = new List<RadioButton>()
            {
                radioButtonRtCh1,
                radioButtonRtCh2,
                radioButtonRtCh3,
                radioButtonRtCh4,
                radioButtonRtCh5,
                radioButtonRtCh6,
                radioButtonRtCh7,
                radioButtonRtCh8,
            };
            foreach (RadioButton radio in radioButtons)
            {
                radio.Checked = false;
                radio.FlatStyle = FlatStyle.Standard;
                radio.BackColor = Color.CornflowerBlue;
                radio.UseVisualStyleBackColor = true;
            }
        }

        /// <summary>
        /// ストリーミングチャンネルのトルグボタンのEnable制御を行う。
        /// </summary>
        /// <param name="enabled"></param>
        private void EnabledStreamingAllChannel(bool enabled)
        {
            radioButtonRtCh1.Enabled = enabled;
            radioButtonRtCh2.Enabled = enabled;
            radioButtonRtCh3.Enabled = enabled;
            radioButtonRtCh4.Enabled = enabled;
            radioButtonRtCh5.Enabled = enabled;
            radioButtonRtCh6.Enabled = enabled;
            radioButtonRtCh7.Enabled = enabled;
            radioButtonRtCh8.Enabled = enabled;
        }

        private enum StreamingUiStatus
        {
            Start,
            Streaming,
            EndPlaying,
            ErrorPlaying,
            Retry,
            RetryOver  // 配信要求, 配信セッションのリトライ共用
        }

        private void UpdateStreamingUiStatus(StreamingUiStatus status)
        {
            switch (status)
            {
                case StreamingUiStatus.Start:
                    progressBarRtStart.Visible = true;
                    streamingPreparationSecond = 0;
                    timerStreamingPreparation.Enabled = true;
                    labelRtRetryStatus.Text = string.Empty;
                    labelRtStatus.Text = string.Empty;
                    break;

                case StreamingUiStatus.Retry:
                    labelRtRetryStatus.Text = $"配信要求リトライ({streamingRequestRetryCount})";
                    break;

                case StreamingUiStatus.RetryOver:
                    progressBarRtStart.Visible = false;
                    timerStreamingPreparation.Enabled = false;
                    labelRtStatus.Text = @"通信状況が悪い為、";
                    labelRtRetryStatus.Text = @"再生できません";
                    UpdateStreamingWatchElapsed(new TimeSpan(0, 0, 0));
                    tableLayoutPanelStreamingOnCar.Visible = false;
                    labelStreamingSessionRetry.Text = string.Empty;
                    break;

                case StreamingUiStatus.Streaming:
                    labelRtStatus.Text = @"再生中";
                    progressBarRtStart.Visible = false;
                    timerStreamingPreparation.Enabled = false;
                    labelRtRetryStatus.Text = string.Empty;
                    labelStreamingSessionRetry.Text = string.Empty;
                    EnabledStreamingAllChannel(true);
                    CheckedStreamingChannel(true);
                    break;

                case StreamingUiStatus.EndPlaying:
                    labelRtStatus.Text = string.Empty;
                    progressBarRtStart.Visible = false;
                    timerStreamingPreparation.Enabled = false;
                    UpdateStreamingWatchElapsed(new TimeSpan(0, 0, 0));
                    tableLayoutPanelStreamingOnCar.Visible = false;
                    labelRtRetryStatus.Text = string.Empty;
                    labelStreamingSessionRetry.Text = string.Empty;
                    UncheckedStreamingAllChannel();
                    EnabledStreamingAllChannel(false);
                    break;

                case StreamingUiStatus.ErrorPlaying:
                    labelRtStatus.Text = @"[異常]再生できません";
                    progressBarRtStart.Visible = false;
                    timerStreamingPreparation.Enabled = false;
                    labelRtRetryStatus.Text = string.Empty;
                    labelStreamingSessionRetry.Text = string.Empty;
                    break;
            }
        }

        private void UpdateStreamingWatchElapsed(TimeSpan span)
        {
            labelRtElapsed.Text = $"{span.ToString(@"hh\:mm\:ss")}";
        }

        private void StartStreamingFfmpeg(StreamingServerInfo ssi)
        {
            if (ssi == null || !ssi.existsMpd)
            {
                isStreaming = false;
                return;
            }
            // e.g. http://176.34.37.239:23468/live/0000000003.1605168010.RKhmUjEtAd.stream/manifest.mpd
            string distributeServer = StreamingRequest.MakeStreamingServerUri(ssi);
            Debug.WriteLine($"stream server: {distributeServer}");

            //            m_LiveBrowsingTime = 0;

            Invoke((MethodInvoker)(() =>
            {
                UpdateStreamingUiStatus(StreamingUiStatus.Streaming);
            }));

            const int width = 720;
            const int height = 480;
            int x = (1920 - width) / 2;
            int y = (1080 - height) / 2;

            string[] options =
            {
                    @"--no-qt-video-autoresize", @"--no-video-title-show", @"--no-qt-bgcone",
                    @"--no-qt-updates-notif", @"--video-on-top",
                    $"--video-x={x}",$"--video-y={y}", $"--width={width}", $"--height={height}",
                    $"--skins2-last=\"{ffmpegCtrl.VlcSkinNoControlsPath}\"",
                    $" {distributeServer}",
            };

            try
            {
                if (streamingDic.ContainsKey(streamingDeviceId))
                {
                    streamingDic[streamingDeviceId].State = StreamingState.Playing;
                }

                var title = "社番：" + streamingDeviceId;
                ffmpeg = new Process();
                // Hide console window. Show player window.
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.FileName = @"vlc\vlc.exe";
                ffmpeg.StartInfo.Arguments = string.Join(@" ", options);
                ffmpeg.Exited += new EventHandler(ExitedFfmpegHandler);
                ffmpeg.EnableRaisingEvents = true;
                ffmpeg.Start();

                streamingWatchingStartTime = DateTime.Now;
                DisposeStreamingWatchingTimer();
                streamingWatcingTimer = new System.Timers.Timer();
                streamingWatcingTimer.Elapsed += StreamingWatchPeriodic;
                streamingWatcingTimer.Interval = 1000;
                streamingWatcingTimer.AutoReset = true;
                streamingWatcingTimer.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"@@@ StartFfmpeg Exception {e}");
                isStreaming = false;
                if (streamingDic.ContainsKey(streamingDeviceId))
                {
                    streamingDic[streamingDeviceId].State = StreamingState.None;
                }

                Invoke((MethodInvoker)(() =>
                {
                    UpdateStreamingUiStatus(StreamingUiStatus.ErrorPlaying);
                }));
            }
        }

        private void ViewRtspStreamingAsync(StreamingServerInfo liveStream)
        {
            if (liveStream is null)
            {
                isStreaming = false;
            }
            else
            {
                var liveInfo = liveStream.data;
                // e.g. RTMP->RTSP 視聴URL rtsp://176.34.37.239:23468/live/WM00003.1748310655.v7UQEpYxwn
                var streamUrl = StreamingRequest.MakeStreamingServerRtspUri(liveStream);

                if (IsExistsRtspStreamingFile(streamUrl))
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        UpdateStreamingUiStatus(StreamingUiStatus.Streaming);
                    }));

                    const int width = 720;
                    const int height = 480;
                    var x = (1920 - width) / 2;
                    var y = (1080 - height) / 2;

                    string[] options =
                    {
                        @"--no-qt-video-autoresize", @"--no-video-title-show", @"--no-qt-bgcone",
                        @"--no-qt-updates-notif", @"--video-on-top",
                        $"--video-x={x}",$"--video-y={y}", $"--width={width}", $"--height={height}",
                        $"--skins2-last=\"{ffmpegCtrl.VlcSkinNoControlsPath}\"",
                        $" {streamUrl}",
                    };

                    try
                    {
                        if (streamingDic.ContainsKey(streamingDeviceId))
                        {
                            streamingDic[streamingDeviceId].State = StreamingState.Playing;
                        }

                        var title = "社番：" + streamingDeviceId;
                        ffmpeg = new Process();
                        // Hide console window. Show player window.
                        ffmpeg.StartInfo.UseShellExecute = false;
                        ffmpeg.StartInfo.CreateNoWindow = true;
                    
                        ffmpeg.StartInfo.FileName = @"vlc\vlc.exe";
                        ffmpeg.StartInfo.Arguments = string.Join(@" ", options);
                        ffmpeg.Exited += new EventHandler(ExitedFfmpegHandler);
                        ffmpeg.EnableRaisingEvents = true;
                        ffmpeg.Start();

                        streamingWatchingStartTime = DateTime.Now;
                        DisposeStreamingWatchingTimer();
                        streamingWatcingTimer = new System.Timers.Timer();
                        streamingWatcingTimer.Elapsed += StreamingWatchPeriodic;
                        streamingWatcingTimer.Interval = 1000;
                        streamingWatcingTimer.AutoReset = true;
                        streamingWatcingTimer.Start();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"@@@ StartFfmpeg Exception {e}");
                        isStreaming = false;
                        if (streamingDic.ContainsKey(streamingDeviceId))
                        {
                            streamingDic[streamingDeviceId].State = StreamingState.None;
                        }

                        Invoke((MethodInvoker)(() =>
                        {
                            UpdateStreamingUiStatus(StreamingUiStatus.ErrorPlaying);
                        }));
                    }
                }
                else
                {
                    isStreaming = false;
                    if (streamingDic.ContainsKey(streamingDeviceId))
                    {
                        streamingDic[streamingDeviceId].State = StreamingState.None;
                    }

                    Invoke((MethodInvoker)(() =>
                    {
                        UpdateStreamingUiStatus(StreamingUiStatus.ErrorPlaying);
                    }));
                }
            }
        }

        /// <summary>
        /// VLCウィンドウが終了された時に呼ばれる。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitedFfmpegHandler(object sender, EventArgs e)
        {
            Debug.WriteLine($"called ExitedFfmpegHandler.");
            // VLCウィンドウの閉じるボタンを押せば、HasExitedがTrueになる。
            if (ffmpeg != null && ffmpeg.HasExited)
            {
                Debug.WriteLine($"exit status {ffmpeg.ExitCode}, {ffmpeg.ExitTime}");
                ffmpeg.Dispose();
                ffmpeg = null;
            }
            StopStreaming(streamingDeviceId);
        }

        private void StartStreaming(string device_id)
        {
            if (string.IsNullOrEmpty(device_id))
            {
                Debug.WriteLine($"::: StartStreaming abort. device_id is empty.");
                return;
            }
            isStreaming = true;
            Debug.WriteLine($"StartStreaming {device_id}");
            if (!streamingDic.ContainsKey(device_id))
            {
                var obj = new StreamingStatus();
                streamingDic.Add(device_id, obj);
            }
            streamingDic[device_id].State = StreamingState.Request;
            MqttStreamingCmd(device_id, true, streamingChannel);
        }

        private void StopStreaming(string device_id, StreamingUiStatus status)
        {
            Debug.WriteLine($"StopStreaming {device_id}");
            if (streamingDic.ContainsKey(device_id))
            {
                streamingDic[device_id].State = StreamingState.None;
            }

            // タイマーは破棄しておく。
            DisposeStreamingCommandTimer();
            DisposeStreamingWatchingTimer();
            DisposeFfmpeg();

            ushort mid = SendStreamingRequestTopic(device_id);
            pubRecord.Add(mid, null);

            streamingDeviceId = "";
            streamingChannel = 0;
            isStreaming = false;

            Invoke((MethodInvoker)(() =>
            {
                UpdateStreamingUiStatus(status);
            }));
        }

        private void DisposeStreamingCommandTimer()
        {
            if (streamingCommandTimer != null)
            {
                streamingCommandTimer.Dispose();
                streamingCommandTimer = null;
            }
        }

        private void DisposeStreamingWatchingTimer()
        {
            if (streamingWatcingTimer != null)
            {
                streamingWatcingTimer.Dispose();
                streamingWatcingTimer = null;
            }
        }

        private void DisposeFfmpeg()
        {
            try
            {
                if (ffmpeg != null && !ffmpeg.HasExited)
                {
                    Debug.WriteLine("Kill ffmpeg");
                    ffmpeg.EnableRaisingEvents = false;
                    ffmpeg.Kill();
                    ffmpeg.WaitForExit(5000);
                    ffmpeg = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"@@@ StopStreaming Exception ffmpeg-process. {e}");
            }
        }

        /// <summary>
        /// Streaming停止要求
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private ushort SendStreamingRequestTopic(string deviceId)
        {
            var topic = String.Format(MQTT_TOPIC_STREAMING_REQUEST, deviceId);
            var req = new MqttJsonStreamingRequest();
            req.startstop = 2;
            req.channel = 0;
            var json = JsonConvert.SerializeObject(req);
            var msg = System.Text.Encoding.UTF8.GetBytes(json);
            return mqttClient.Publish(topic, msg, 2, false);
        }

        private void RestartStreaming(string deviceId)
        {
            Debug.WriteLine($"StopStreaming {deviceId}");
            if (streamingDic.ContainsKey(deviceId))
            {
                streamingDic[deviceId].State = StreamingState.None;
            }

            // タイマーは破棄しておく。
            DisposeStreamingCommandTimer();
            DisposeStreamingWatchingTimer();
            DisposeFfmpeg();
            // ストリーミング停止要求
            SendStreamingRequestTopic(deviceId);
            // wait 5sec.
            Thread.Sleep(5000);
            // ストリーミング開始
            StartStreaming(deviceId);
        }

        private void StopStreaming(string device_id)
        {
            StopStreaming(device_id, StreamingUiStatus.EndPlaying);
        }

        private void MqttStreamingCmd(string device_id, bool needStart, int channel)
        {
            Debug.WriteLine($"MqttStreamingCmd: {device_id}, {needStart}, {channel}");
            var topic = String.Format(MQTT_TOPIC_STREAMING_REQUEST, device_id);

            var req = new MqttJsonStreamingRequest();
            req.device_id = device_id;
            req.startstop = needStart ? 1 : 0;
            req.channel = channel;
            var json = JsonConvert.SerializeObject(req);
            var msg = System.Text.Encoding.UTF8.GetBytes(json);
            var mid = mqttClient.Publish(topic, msg, 2, false);

            DisposeStreamingCommandTimer();
            streamingCommandTimer = new System.Timers.Timer();
            streamingCommandTimer.Elapsed += TimeoutStreamingCmd;
            streamingCommandTimer.AutoReset = false;
            //streamingCommandTimer.Interval = 30 * 1000;
            streamingCommandTimer.Interval = localSettings.StreamingRequestWait * 1000;
            streamingCommandTimer.Start();
            var pi = new PublishedInfo()
            {
                Message = $"MqttStreamingCmd {needStart}, {channel}",
                PublishedHandler = delegate (ushort id, PublishedInfo info)
                {
                    Debug.WriteLine(info.Message);
                }
            };
            pubRecord.Add(mid, pi);

            streamingDeviceId = device_id;
            streamingChannel = channel;
        }

        public void TimeoutStreamingCmd(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("TimeoutStreamingCmd - retry request...");

            if (string.IsNullOrEmpty(streamingDeviceId))
            {
                return;
            }
            else
            {
                if (streamingDic.ContainsKey(streamingDeviceId))
                {
                    if (streamingDic[streamingDeviceId].State == StreamingState.None)
                    {
                        // リトライタイマーのすれ違い対策。
                        // すでにストリーミングが終わっていたなら、リトライタイマーも終わらせる。
                        return;
                    }
                }
            }

            streamingRequestRetryCount++;
            if (streamingRequestRetryCount <= localSettings.StreamingRequestRetryCount)
            {
                // リトライ
                Invoke((MethodInvoker)(() =>
                {
                    UpdateStreamingUiStatus(StreamingUiStatus.Retry);
                }));
                StartStreaming(streamingDeviceId);
            }
            else
            {
                // 通信状態不良と判断して中止
                StopStreaming(streamingDeviceId, StreamingUiStatus.RetryOver);
            }
        }

        public void StreamingWatchPeriodic(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invoke((MethodInvoker)(() =>
            {
                UpdateStreamingWatchElapsed(DateTime.Now - streamingWatchingStartTime);
            }));
        }

        /// <summary>
        /// リアルタイム配信中の車載器側状況を表示更新する。ffmpegによる送信の元気の良さが分かる。
        /// </summary>
        /// <remarks>
        /// あくまで車載器からサーバー間であることに注意されたい。
        /// この情報はffmpegへ -progress オプションを渡すと、1秒に1回程度で状況を貰える。
        /// そのうちMQTTで飛ばしてくるのは、おおよそ10秒に1回程度の頻度のはず。
        /// </remarks>
        /// <param name="obj"></param>
        private void UpdateLeftPanelStreamingStatusOnCar(MqttJsonStreamingStatus obj)
        {
            // MVVM できないから、個別に更新する。
            if (obj == null)
            {
                labelCarStreamElapsed.Text = string.Empty;
                labelCarStreamFrames.Text = string.Empty;
                labelCarStreamBytes.Text = string.Empty;
                labelCarStreamFps.Text = string.Empty;
                labelCarStreamKbps.Text = string.Empty;
                labelCarStreamDrops.Text = string.Empty;
                labelCarStreamSpeed.Text = string.Empty;
            }
            else
            {
                // ストリーミング要求チャンネルと
                // 車載器が返してきたストリーミングチャンネルが異なる場合、
                // 車載器が返したチャンネルを正とする。
                // ※disableとなっているチャンネルを指定すると
                // 車載器側で自動的に有効なチャンネルを返してくるので、
                // 画面で指定したチャンネルと異なるチャンネルが返ってくることがある。
                if (streamingChannel != obj.streamingChannel)
                {
                    streamingChannel = obj.streamingChannel;
                    CheckedStreamingChannel(true, true);
                }
                // ストリーミング情報の表示
                labelCarStreamElapsed.Text = obj.elapsed;
                labelCarStreamFrames.Text = obj.tframes.ToString();
                labelCarStreamBytes.Text = $"{obj.tbytes / 1024}"; // kilo-bytes
                labelCarStreamFps.Text = $"{obj.fps:F1}";
                labelCarStreamKbps.Text = $"{obj.kbps:F1}";
                labelCarStreamDrops.Text = obj.dropped.ToString();
                labelCarStreamSpeed.Text = $"{obj.speed:F3}";
            }
        }

        /// <summary>
        /// FFMPEGを使ったRTSP接続時のStreamingFile存在確認
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool IsExistsRtspStreamingFile(string url, int waitForExitSec = 10000)
        {
            var result = false;
            using (var process = new Process()) 
            {
                var info = process.StartInfo;
                info.FileName = Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe");
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
                var argsList = new List<string>()
                {
                    "-rtsp_transport tcp",
                    $"-i {url}",
                    " -t 1 -f null -"
                };
                info.Arguments = string.Join(" ", argsList);
                process.Start();
                process.WaitForExit(waitForExitSec);
                result = (process.ExitCode == 0);
            }
            return result;
        }

#endregion "リアルタイム再生関係"

#region "車載器の映像データ"

        private Dictionary<string, MovieRequestWindow> movieRequestWindows;

        private void buttonShowDrivingMovie_Click(object sender, EventArgs e)
        {
            var mapEntry = carListBindingSource.Current as MapEntryInfo;
            if (movieRequestWindows == null)
            {
                movieRequestWindows = new Dictionary<string, MovieRequestWindow>();
            }
            if (mapEntry == null)
            {
                // 選択中に消えた。なにもしない。
            }
            else if (movieRequestWindows.ContainsKey(mapEntry.DeviceId))
            {
                var window = movieRequestWindows[mapEntry.DeviceId];
                window.Show();
                window.Activate();
            }
            else
            {
                var window = new MovieRequestWindow(httpSequence);
                window.MqttClient = mqttClient;
                window.CarId = mapEntry.CarId;
                window.DeviceId = mapEntry.DeviceId;
                window.OfficeId = m_SelectOfficeID;
                movieRequestWindows[mapEntry.DeviceId] = window;
                if (AuthedUser.Item != null)
                {
                    window.UserName = AuthedUser.Item.Name;
                }
                window.Show();
            }
        }

        private void MovieRequestWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                var window = (MovieRequestWindow)sender;
                var did = window.viewModel.Device.DeviceId;
                if (movieRequestWindows.ContainsKey(did))
                {
                    movieRequestWindows.Remove(did);
                    Debug.WriteLine($"Removed {did} from movieRequestWindows");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
            }
        }

        private void CloseAllMovieRequestWindow()
        {
            if (movieRequestWindows != null)
            {
                // window.Close時にウィンドウ管理ディクショナリに変更が加わるため
                // ウィンドウ管理ディクショナリを直接操作しない
                var windowList = movieRequestWindows.Values.ToList();
                foreach (var window in windowList)
                {
                    window.viewModel.IsDisposing = true;
                    window.Close();
                }
                movieRequestWindows.Clear();
            }
        }

#endregion "車載器の映像データ"

#region "リモート設定"

        private Dictionary<string, RemoteSetting> remoteSettingWindows;

        private void buttonShowRemoteSetting_Click(object sender, EventArgs e)
        {
            if (remoteSettingWindows == null)
            {
                remoteSettingWindows = new Dictionary<string, RemoteSetting>();
            }

            var mapEntry = carListBindingSource.Current as MapEntryInfo;
            if (mapEntry == null)
            {
                // 選択中に消えた。 なにもしない
            }
            else if (remoteSettingWindows.ContainsKey(mapEntry.DeviceId))
            {
                var window = remoteSettingWindows[mapEntry.DeviceId];
                window.Activate();
            }
            else
            {
                CarInfo carInfo = httpSequence.GetCarInfoFromGroups(m_SelectOfficeID, mapEntry.DeviceId);

                var window = new RemoteSetting();
                window.ServerInfo = operationServerInfo;
                window.LocalSettings = localSettings;
                window.FormClosed += RemoteSettingWindow_FormClosed;
                window.MqttClient = mqttClient;
                window.HttpClient = httpClient;
                window.DeviceId = mapEntry.DeviceId;
                window.CarId = mapEntry.CarId;
                remoteSettingWindows[mapEntry.DeviceId] = window;
                var viewModel = new WindowRemoteConfigViewModel();
                viewModel.AuthedUser = AuthedUser.Item;
                viewModel.DeviceStatus = new DeviceStatus(carInfo.ErrorCode);
                viewModel.BrList = new BitrateList();
                viewModel.OfficeId = m_SelectOfficeID;
                window.ViewModel = viewModel;
                window.Show();
            }
        }

        private void RemoteSettingWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                var window = (RemoteSetting)sender;
                var did = window.DeviceId;
                if (remoteSettingWindows.ContainsKey(did))
                {
                    remoteSettingWindows.Remove(did);
                    Debug.WriteLine($"Removed {did} from remoteSettingWindows");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
            }
        }

        private void CloseAllRemoteSettingWindow()
        {
            if (remoteSettingWindows != null)
            {
                // window.Close時にウィンドウ管理ディクショナリに変更が加わるため
                // ウィンドウ管理ディクショナリを直接操作しない
                var windowList = remoteSettingWindows.Values.ToList();
                foreach (var window in windowList)
                {
                    window.Close();
                }
            }
        }

#endregion "リモート設定"

        /// <summary>
        /// 接続先となるサーバー環境の選択。
        /// このコンボボックスはDEBUG時のみ表示されているはずで、原則、Production(本番)環境を使う。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxServerEnv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var obj = (ComboBox)sender;
                Properties.Settings.Default.server_index = obj.SelectedIndex;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception : {ex}, {ex.StackTrace}");
            }
        }

        private void eventListUpdateProgress(int count, int total, bool isCompleted)
        {
            // 負荷を下げるためUIの更新頻度を低くしたい。
            // progressBarEventListUpdate.Maximum (100)を分母にしておく。
            int prog = 0;
            if (total > 0)
            {
                prog = (count * progressBarEventListUpdate.Maximum) / total;
            }
            if (isCompleted || progressBarEventListUpdate.Value != prog)
            {
                Invoke((MethodInvoker)(() =>
                {
                    progressBarEventListUpdate.Value = prog;
                    if (isCompleted)
                    {
                        buttonEventListUpdate.Enabled = true;
                        buttonDownload.Enabled = true;
                        buttonGetG.Enabled = true;
                        progressBarEventListUpdate.Visible = false;
                    }
                }));
            }
        }

        /// <summary>
        /// イベント再生タブの項目初期化
        /// </summary>
        private void prepareEventListUpdate()
        {
            progressBarEventListUpdate.Maximum = 100;
            progressBarEventListUpdate.Value = 0;
            progressBarEventListUpdate.Visible = true;
            buttonEventListUpdate.Enabled = false;
            buttonDownload.Enabled = false;
            buttonGetG.Enabled = false;
            buttonCancelG.Enabled = false;
            labelGstatus.Text = string.Empty;
        }

        private void buttonEventListUpdate_Click(object sender, EventArgs e)
        {
            prepareEventListUpdate();
            var w = new TimeSpan(0, 0, 0);
            eventListCancelTokenSource = new CancellationTokenSource();
            SetSortableMode(gridEventList, false);
            _ = Task.Run(() =>
            {
                Task task = httpSequence.UpdateEvents(
                    m_SelectOfficeID, w, eventListCancelTokenSource.Token, eventListUpdateProgress);
                task.GetAwaiter().OnCompleted(() =>
                {
                    // イベント更新完了後
                    Invoke((MethodInvoker)(() =>
                    {
                        if (task.IsFaulted)
                        {
                            // HttpRequestExceptionなどでエラーとなった場合、
                            // 例外中断となるのでボタンを戻しておく
                            buttonEventListUpdate.Enabled = true;
                            buttonDownload.Enabled = true;
                            buttonGetG.Enabled = true;
                            progressBarEventListUpdate.Visible = false;
                        }
                        else
                        {
                            // bindされたリストをUIスレッド以外から触るとエラーとなるので
                            // UIスレッドにリスト更新を任せる
                            httpSequence.UpdateEventBindingList();
                            httpSequence.Events.Sort(eventComparer);
                            httpSequence.UpdateRequestBindingList();
                        }
                        SetSortableMode(gridEventList, true);
                        if (eventListSortedColumn != null)
                        {
                            eventListSortedColumn.HeaderCell.SortGlyphDirection = eventComparer.Order;
                        }
                    }));
                });
            });
        }

        private void CreateEventBindingList()
        {
            prepareEventListUpdate();
            var w = new TimeSpan(0, 0, 0);
            eventListCancelTokenSource = new CancellationTokenSource();
            SetSortableMode(gridEventList, false);
            if (eventListSortedColumn != null)
            {
                eventListSortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                eventListSortedColumn = null;
            }
            httpSequence.Events.Clear();
            httpSequence.Requests.Clear();
            eventComparer.Mode = EventInfoComparer.SortMode.TimeStamp;
            eventComparer.Order = SortOrder.Descending;
            _ = Task.Run(() =>
            {
                Task task = httpSequence.UpdateEvents(
                    m_SelectOfficeID, w, eventListCancelTokenSource.Token, eventListUpdateProgress);
                task.GetAwaiter().OnCompleted(() =>
                {
                    if (task.IsCompleted)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            if (task.IsCompleted)
                            {
                                httpSequence.CreateEventBindingList();
                                eventComparer.Mode = EventInfoComparer.SortMode.TimeStamp;
                                eventComparer.Order = SortOrder.Descending;
                                httpSequence.Events.Sort(eventComparer);
                                httpSequence.CreateRequestBindingList();
                                SetSortableMode(gridEventList, true);
                            }
                        }));
                    }
                });
            });
        }

        /// <summary>
        /// Grid列のAutoSizeモードを一律で設定する
        /// </summary>
        /// <param name="dataGrid">対象のDataGridView</param>
        /// <param name="range">対象列</param>
        /// <param name="mode">設定するモード</param>
        private void SetAutoSizeColumnMode(
            DataGridView dataGrid, IEnumerable<int> range, DataGridViewAutoSizeColumnMode mode)
        {
            foreach (int i in range)
            {
                if (i < dataGrid.Columns.Count)
                {
                    dataGrid.Columns[i].AutoSizeMode = mode;
                }
            }
        }

        /// <summary>
        /// ウィンドウサイズを復元し、アクティブにする。
        /// </summary>
        private void RestoreMainForm()
        {
            Invoke((MethodInvoker)(() =>
            {
                this.Visible = true;
                WindowState = FormWindowState.Normal;
                this.Activate();
            }));
        }

        /// <summary>
        /// 多重起動した場合の処理。
        /// メッセージが飛んでくるはず。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtvIpcRequestChangeHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // なにか変わった。とはいえ現状ひとつだけだから、それを調べる。
            switch (messageServer.Request.Request)
            {
                case RtvIpcRequest.None:
                    // 何もしない。もしやらかすと、延々PropertyChangedイベントが発生する。止めて差し上げろ。
                    break;

                case RtvIpcRequest.ActivateWindow:
                    RestoreMainForm();
                    // IPCのリクエストを消しておく。
                    messageServer.Request.Clear();
                    break;
            }
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
            if (m_UserAuthComplete && AuthedUser.Item.Name != Properties.Resources.EngineerMode)
            {
                AuthedUser.Item = new User();
                // ユーザ名の戻し
                labelUserName.Text = Properties.Resources.BeforeAuthoricationUserName;

                // ダウンロードの中止
                if (downloadCancelTokenSource != null && downloadCancelTokenSource.Token.CanBeCanceled)
                {
                    downloadCancelTokenSource.Cancel();
                }

                // サイドパネルの戻し
                LeftPanelHide();
                tabControlRtSelect.SelectedIndex = 0;

                // メインタブの戻し
                tabControlMain.SelectedIndex = 0;

                // ストリーミングの中止
                if (isStreaming)
                {
                    StopStreaming();
                }

                // イベント再生強制終了
                ffmpegCtrl.KillAllProcesses();

                // MovieRequestWindowの終了
                CloseAllMovieRequestWindow();
                // RemoteSettingWindowの終了
                CloseAllRemoteSettingWindow();

                // 事業所の戻し
                comboBoxOffice.Enabled = false;
                if (m_SelectOfficeID != localSettings.SelectOfficeID)
                {
                    // イベント更新の中止
                    if (eventListCancelTokenSource != null && eventListCancelTokenSource.Token.CanBeCanceled)
                    {
                        eventListCancelTokenSource.Cancel();
                    }

                    // Gキャンセル
                    buttonCancelG_Click(null, null);

                    m_SelectOfficeID = localSettings.SelectOfficeID;
                    var dataSource = officeInfoBindingSource.DataSource as BindingList<OfficeInfo>;
                    if (dataSource != null)
                    {
                        var selectedOffice = dataSource.FirstOrDefault(x => x.Id == m_SelectOfficeID);
                        if (selectedOffice == null)
                        {
                            officeInfoBindingSource.Position = 0;
                            m_SelectOfficeID = 0;
                        }
                        else
                        {
                            officeInfoBindingSource.Position = dataSource.IndexOf(selectedOffice);
                        }
                    }
                }
            }
        }

        private void UpdateUiAtUserAuthed()
        {
            labelUserName.Text = AuthedUser.Item.Name;
            if (AuthedUser.Item.Can(Permission.OfficeEdit) || AuthedUser.Item.Can(Permission.Engineer))
            {
                comboBoxOffice.Enabled = true;
            }
            else
            {
                comboBoxOffice.Enabled = false;
            }
            if (m_wform != null)
            {
                m_wform.SetVisibleForAuthWarning(false);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.F9) && configPanel != null)
            {
                Debug.WriteLine(@"Detected Ctrl+Shift+F9.");
                AuthedUser.Item = new User()
                {
                    Name = Properties.Resources.EngineerMode,
                    Permission = (int)UserBioDP.RolePermissonConverter.ToPermission(Role.Engineer),
                };
                m_UserAuthComplete = true;
                if (!tabControlMain.TabPages.ContainsKey(TabPageConfigName))
                {
                    TabPage tabPage = new TabPage();
                    tabPage.Name = TabPageConfigName;
                    tabPage.Text = tabPageConfigText;
                    tabPage.Controls.Add(configPanel);
                    tabControlMain.Enabled = true;
                    tabControlMain.TabPages.Add(tabPage);
                }
                UpdateUiAtUserAuthed();
            }
            //else if (keyData == (Keys.Control | Keys.Shift | Keys.F3))
            //{
            //    labelServerEnv.Visible = !labelServerEnv.Visible;
            //    comboBoxServerEnv.Visible = !comboBoxServerEnv.Visible;
            //}
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // ★★★ DEBUG ★★★ このメソッドは、あとで消すこと！
            var window = new MovieRequestWindow(this.httpSequence);
            window.MqttClient = mqttClient;
            window.CarId = @"10";
            window.Show();
        }

        private Task taskGetG;

        private void buttonGetG_Click(object sender, EventArgs e)
        {
            Form owner = this;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            if (taskGetG == null || taskGetG.IsCompleted)
            {
                if (httpSequence.Events != null)
                {
                    buttonGetG.Enabled = false;
                    buttonCancelG.Enabled = true;
                    buttonEventListUpdate.Enabled = false;
                    progressBarG.Visible = true;
                    labelGstatus.Visible = true;
                    if (gtaskCancelSource != null)
                    {
                        gtaskCancelSource.Dispose();
                    }
                    gtaskCancelSource = new CancellationTokenSource();
                    taskGetG = UpdateGravityValue(httpSequence, gtaskCancelSource.Token);
                    taskGetG.GetAwaiter().OnCompleted(() =>
                    {
                        if (taskGetG.IsFaulted)
                        {
                            foreach (var exception in taskGetG.Exception.InnerExceptions)
                            {
                                operationLogger.Out(OperationLogger.Category.Application, AuthedUser.Item.Name, exception.StackTrace);
                                MessageBox.Show(owner, $"{exception.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                            dispatcher.Invoke(() =>
                            {
                                labelGstatus.Text = @"エラーにより中断しました。";
                                progressBarG.Visible = false;
                                buttonGetG.Enabled = true;
                                buttonCancelG.Enabled = false;
                                buttonEventListUpdate.Enabled = true;
                                SetSortableMode(gridEventList, true);
                            });
                        }
                    });
                }
            }
        }

        private void buttonCancelG_Click(object sender, EventArgs e)
        {
            if (taskGetG != null && !taskGetG.IsCompleted)
            {
                if (gtaskCancelSource != null)
                {
                    gtaskCancelSource.Cancel();
                    labelGstatus.Text = @"キャンセル中...";
                }
                buttonCancelG.Enabled = false;
            }
        }

        private Task UpdateGravityValue(RequestSequence rs, CancellationToken ct)
        {
            SetSortableMode(gridEventList, false);
            var t = Task.Run(() =>
            {
                bool isCanceled = false;
                Invoke((MethodInvoker)(() =>
                {
                    labelGstatus.Text = @"開始しました";
                    progressBarG.Value = 0;
                    progressBarG.Maximum = rs.Events.Count;
                }));
                foreach (var ei in rs.Events)
                {
                    if (ct.IsCancellationRequested)
                    {
                        isCanceled = true;
                        break;
                    }
                    if (ei.GravityRecord == null)
                    {
                        var digTask = rs.GetDig(ei.DeviceId, ei.Timestamp, (result) =>
                        {
                            string text = @"-";
                            if (result.Result == 0 && result.GravityRecord != null)
                            {
                                text = $"X:{result.GravityRecord.X:0.00}, Y:{result.GravityRecord.Y:0.00}, Z:{result.GravityRecord.Z:0.00}";
                                ei.GravityRecord = result.GravityRecord;
                            }
                            Invoke((MethodInvoker)(() =>
                            {
                                ei.Remarks = text;
                                progressBarG.Value++;
                                labelGstatus.Text = $"取得 {progressBarG.Value} / {progressBarG.Maximum} 件";
                            }));
                        });
                        digTask.Wait();
                    }
                    else
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            progressBarG.Value++;
                            labelGstatus.Text = $"取得 {progressBarG.Value} / {progressBarG.Maximum} 件";
                        }));
                    }
                }
                Invoke((MethodInvoker)(() =>
                {
                    if (isCanceled)
                    {
                        labelGstatus.Text = @"G取得処理をキャンセルしました";
                    }
                    else
                    {
                        labelGstatus.Text = @"G取得処理が終わりました";
                    }
                    progressBarG.Visible = false;
                    buttonGetG.Enabled = true;
                    buttonCancelG.Enabled = false;
                    buttonEventListUpdate.Enabled = true;
                    SetSortableMode(gridEventList, true);
                }));
                Debug.WriteLine($"End of UpdateGravityValue");
            });
            return t;
        }

        /// <summary>
        /// Gridのソート可能/不可能を設定する<br/>
        /// ここではソートモードはprogrammableを用いている
        /// </summary>
        /// <param name="dataGrid">対象のDataGridView</param>
        /// <param name="isSortable">true: ソート可 false: ソート不可</param>
        private void SetSortableMode(DataGridView dataGrid, bool isSortable)
        {
            foreach (DataGridViewColumn column in dataGrid.Columns)
            {
                if (isSortable)
                {
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                    if (column == eventListSortedColumn)
                    {
                        eventListSortedColumn.HeaderCell.SortGlyphDirection = eventComparer.Order;
                    }
                }
                else
                {
                    if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
                    {
                        eventListSortedColumn = column;
                    }
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
        }

        private void TimerStreamingPreparation_Tick(object sender, EventArgs e)
        {
            streamingPreparationSecond++;
            labelRtStatus.Text = $"配信情報取得中({(streamingPreparationSecond / 60):00}:{(streamingPreparationSecond % 60):00})";
        }

        private void OfficeInfoBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            timerGPSDraw.Stop();
            gridCarList.Enabled = false;
            bool drawAllCar = radioButtonALL.Checked;
            var dispatcher = Dispatcher.CurrentDispatcher;
            _ = Task.Run(async () =>
            {
                await Task.Delay(100);
                dispatcher.Invoke(() =>
                {
                    /// 1. 下記をキャンセルする。
                    ///   ストリーミング、遠隔設定、ドラレコ映像、イベントダウンロード、イベントリスト更新、Gの更新、地図の描画
                    /// 2. イベントリスト更新、地図の更新を行う。

                    if (isStreaming)
                    {
                        buttonStreamStop_Click(sender, e);
                    }
                    CloseAllRemoteSettingWindow();

                    var currentOffice = officeInfoBindingSource.Current as OfficeInfo;
                    if (currentOffice != null)
                    {
                        m_SelectOfficeID = currentOffice.Id;
                    }
                    CloseAllMovieRequestWindow();

                    if (downloadCancelTokenSource != null && downloadCancelTokenSource.Token.CanBeCanceled)
                    {
                        downloadCancelTokenSource.Cancel();
                    }

                    buttonCancelG_Click(sender, e);
                    buttonGetG.Enabled = false;
                    buttonCancelG.Enabled = false;
                    buttonEventListUpdate.Enabled = false;
                    progressBarG.Visible = false;
                    labelGstatus.Visible = false;

                    if (eventListCancelTokenSource != null && eventListCancelTokenSource.Token.CanBeCanceled)
                    {
                        eventListCancelTokenSource.Cancel();
                    }
                    CreateEventBindingList();

                    MoveToOfficeLocation();
                    if (drawMapCancelToken != null && drawMapCancelToken.Token.CanBeCanceled)
                    {
                        drawMapCancelToken.Cancel();
                    }
                    drawMapCancelToken = new CancellationTokenSource();
                    try
                    {
                        DrawObjectOnMap(dispatcher, drawMapCancelToken.Token);
                        if (m_UserAuthComplete && m_wform != null)
                        {
                            m_wform.FinishWaiting();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // 処理なし
                        Debug.WriteLine("[OnOfficeChanged]地図描画処理が中断されました。");
                    }
                    timerGPSDraw.Start();
                });
            });
            if (emergencyCar == null)
            {
                ShowWaitingForm();
            }
        }

#region USB機器監視

        private void StartDeviceWatcher()
        {
            deviceWatcher = new DeviceWatcher();
            deviceWatcher.SetWatchTarget(localSettings.WatchTargets, OnUsbDeviceCreated, OnUsbDeviceRemoved);
            deviceWatcher.Start();
        }

        private void StopDeviceWatcher()
        {
            deviceWatcher.Stop();
        }

        private void OnUsbDeviceCreated()
        {
            if (userAuthDp != null)
            {
                Debug.WriteLine("Restart DP Authentication.");
                userAuthDp.Dispose();
                userAuthDp.StartAuth();
            }
        }

        private void OnUsbDeviceRemoved()
        {
            if (AuthedUser.Item.Permission != 0 && !string.IsNullOrEmpty(AuthedUser.Item.Name))
            {
                Debug.WriteLine("Lost Authentication.");
                Invoke((Action)(() =>
                {
                    UpdateUiAtUserLostAuthority();
                }
                ));
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            User user = new User();
            switch (comboBoxDebugUser.SelectedIndex)
            {
                case 0:
                    user.Name = "一般太郎";
                    user.AddPermission(Permission.LocalView);
                    break;

                case 1:
                    user.Name = "管理次郎";
                    user.AddPermission(Permission.StreamingView);
                    user.AddPermission(Permission.OfficeEdit);
                    break;

                case 2:
                    user.Name = "技術三郎";
                    user.AddPermission(Permission.StreamingView);
                    user.AddPermission(Permission.Engineer);
                    break;
            }

            AuthedUser.Item = user;
            AuthedUser.Item.Name = string.IsNullOrEmpty(user.Name) ? string.Empty : user.Name;
            m_UserAuthComplete = true;
            Invoke((MethodInvoker)(() =>
            {
                UpdateUiAtUserAuthed();
            }));
        }

#endregion USB機器監視

#region 拡張設定

        private void CreateExtTabPage(Form owner)
        {
            var sessionWaitMin = (operationServerInfo.Id == ServerIndex.WeatherMedia) ? 5: 30;
            // コンフィグパネル設定
            configPanel = new ConfigPanel();
            configPanel.Dock = DockStyle.Fill;
            configPanel.DataModel = new ConfigPanelModel()
            {
                Settings = localSettings,
                OfficeList = httpSequence.Offices,
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
                ServerIndex serverIndex = (ServerIndex)Enum.ToObject(typeof(ServerIndex), localSettings.ServerIndex);
                result = localSettings.AllowedServers.FirstOrDefault(x => x.Id == serverIndex);
                if (result == null)
                {
                    result = localSettings.AllowedServers[0];
                }
            }
            catch (Exception)
            {
                result = localSettings.AllowedServers[0];
            }
            return result;
        }

        private Icon GetMarkerIcon()
        {
            switch (operationServerInfo.Id)
            {
                case ServerIndex.WeatherMedia:
                    return Properties.Resources.ship;
                default:
                    return Properties.Resources.livegps;
            };
        }

        private Bitmap GetMarkerBitmap()
        {
            switch (operationServerInfo.Id)
            {
                case ServerIndex.WeatherMedia:
                    return new Bitmap(GetType(), "ship.png");
                default:
                    return new Bitmap(GetType(), "bus.png");
            };
        }
#endregion サーバ情報取得
    }
}