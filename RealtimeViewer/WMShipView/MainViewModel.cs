using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using MpgCustom;
using MpgMap;
using RealtimeViewer.Logger;
using RealtimeViewer.Model;
using RealtimeViewer.Movie;
using RealtimeViewer.Network;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network.Mqtt;
using RealtimeViewer.Setting;
using RealtimeViewer.WMShipView.Streaming;
using uPLibrary.Networking.M2Mqtt;
using UserBioDP;
using static RealtimeViewer.Network.Http.RequestSequence;

namespace RealtimeViewer.WMShipView
{
    public class MainViewModel : BindableModel
    {
        /// <summary>
        /// リアルタイムビューアの仕向け(東武、開発、明治、others)
        /// </summary>
        private UserIndex UserIndex { get; set; } = UserIndex.Multiwave;

        /// <summary>
        /// リアルタイム視聴方式
        /// </summary>
        //private StreamingTypes StreamingTypes { get; set; } = StreamingTypes.Rtsp;

        public OperationLogger OperationLogger { get; private set; }

        /// <summary>
        /// INIファイル情報
        /// </summary>
        public SettingIni LocalSettings { get; private set; } = new SettingIni();

        public OperationServerInfo OperationServerInfo { get; private set; }

        /// <summary>
        /// マップルマップに対し使用するスケール。
        /// </summary>
        public int[] MapScales { get; private set; }

        public WMDataSet WMDataSet { get; private set; } = new WMDataSet();

        public WMShipView.WMDataSet.OfficeDataTable OfficeTable => WMDataSet.Office;

        public WMShipView.WMDataSet.DeviceDataTable DeviceTable => WMDataSet.Device;

        public WMShipView.WMDataSet.ErrorDataTable ErrorTable => WMDataSet.Error;

        public WMShipView.WMDataSet.EventListDataTable EventTable { get; set; } = new WMDataSet.EventListDataTable();

        public DataView FilteredEventTable { get; set; } 

        public WMShipView.WMDataSet.PlayListDataTable PlaylistTable { get; set; }

        #region ユーザ認証
        /// <summary>
        /// 認証ユーザ
        /// </summary>
        private UserBioDP.User authedUser;
        /// <summary>
        ///  認証ユーザ
        /// </summary>
        public UserBioDP.User AuthedUser 
        { 
            get => authedUser; 
            set 
            {
                SetProperty(ref authedUser, value); 
                NotifyPropertyChanged(nameof(UserName));
                NotifyPropertyChanged(nameof(IsEngineer));
                NotifyPropertyChanged(nameof(IsBrowsable));
                NotifyPropertyChanged(nameof(IsOfficeEditable));
            }
        }

        /// <summary>
        /// 認証メッセージを出すかどうか
        /// </summary>
        private bool isShowAuthMessage = true;
        /// <summary>
        /// 認証メッセージを出すかどうか
        /// </summary>
        public bool IsShowAuthMessage 
        { 
            get => isShowAuthMessage; 
            private set => SetProperty(ref isShowAuthMessage, value); 
        }

        /// <summary>
        /// 認証済み
        /// </summary>
        private bool isUserAuthComplited = false;
        /// <summary>
        /// 認証済み
        /// </summary>
        public bool IsUserAuthCompleted 
        { 
            get => isUserAuthComplited; 
            private set => SetProperty(ref isUserAuthComplited, value); 
        }

        /// <summary>
        /// 認証コンポーネント
        /// </summary>
        private UserAuthDp userAuthDp;

        /// <summary>
        /// 認証ユーザ名
        /// </summary>
        public string UserName
        {
            get 
            {
                string result;
                if (AuthedUser is null)
                {
                    result = "(指紋認証してください)";
                }
                else
                {
                    result = AuthedUser.Name;
                }
                return result;
            } 
        }

        /// <summary>
        /// エンジニアか
        /// </summary>
        public bool IsEngineer => (AuthedUser is null) ? false : AuthedUser.Can(Permission.Engineer);

        /// <summary>
        /// 参照可能か
        /// </summary>
        public bool IsBrowsable => (AuthedUser is null) ? false : (AuthedUser.Can(Permission.StreamingView) || AuthedUser.Can(Permission.Engineer));

        /// <summary>
        /// 営業所変更可能か
        /// </summary>
        public bool IsOfficeEditable => (AuthedUser is null) ? false : (AuthedUser.Can(Permission.SettingEdit) || AuthedUser.Can(Permission.Engineer));

        /// <summary>
        /// エンジニアモードか
        /// </summary>
        private bool isEngineerMode = false;
        /// <summary>
        /// エンジニアモードか
        /// </summary>
        public bool IsEngineerMode
        {
            get => isEngineerMode; 
            set => SetProperty(ref isEngineerMode, value);
        }

        /// <summary>
        /// 緊急通報車両
        /// </summary>
        public string EmergencyDeviceId { get; private set; } = string.Empty;

        /// <summary>
        /// 緊急通報モード
        /// </summary>
        public bool IsEmergencyMode { get; private set; } = false;

        /// <summary>
        /// USB監視
        /// </summary>
        private DeviceWatcher deviceWatcher;
        #endregion

        #region HTTP
        /// <summary>
        /// WebAPI呼び出し用のHTTPクライアント
        /// </summary>
        public HttpClient HttpClient { get; private set; } = new HttpClient();

        public RequestSequence RequestController { get; private set; }

        private MqttController MqttController { get; set; } = new MqttController();

        public MqttClient MqttClient => MqttController.MqttClient;

        private IStreamingController StreamingController { get; set; }

        #endregion

        #region メインパネル
        public CancellationTokenSource CancellationTokenSource { get; private set; } = null;

        private bool isDeviceFocus = false;
        /// <summary>
        /// 選択車両表示/全車両表示
        /// </summary>
        public bool IsDeviceFocus
        {
            get => isDeviceFocus;
            set => SetProperty(ref isDeviceFocus, value);
        }

        private int selectedOfficeId = -1;
        /// <summary>
        /// 選択営業所
        /// </summary>
        public int SelectedOfficeId
        {
            get => selectedOfficeId;
            set => SetProperty(ref selectedOfficeId, value);
        }

        private string selectedDeviceId = string.Empty;
        /// <summary>
        /// 選択車両
        /// </summary>
        public string SelectedDeviceId
        {
            get => selectedDeviceId;
            set 
            {
                SetProperty(ref selectedDeviceId, value);
                StreamingController.CurrentDeviceId = value;
            }
        }

        /// <summary>
        /// 選択車両名(CarNumber)
        /// </summary>
        public string SelectedDeviceName => (SelectedDevice is null) ? string.Empty : SelectedDevice.CarNumber;

        /// <summary>
        /// 選択車両エラー表示
        /// </summary>
        public string SelectedDeviceErrorStr 
        {
            get
            {
                var result = string.Empty;
                if (SelectedDevice != null)
                {
                    if (SelectedDeviceError != null)
                    {
                        result = SelectedDeviceError.ErrorStr;
                    }
                    else if (SelectedDevice.TryGetLocation(out var _))
                    {
                        result = "正常";
                    }
                    else
                    {
                        result = string.Empty;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 選択車両サイドパネル色
        /// </summary>
        public Color SelectedDeviceBackColor
        {
            get
            {
                var result = Color.Gray;
                if (HasRealtimeErrorSelectedDevice) 
                {
                    result = Color.Gold;
                }
                else if (IsAliveSelectedDevice) 
                {
                    result = Color.CornflowerBlue;
                }
                return result;
            }
        }

        /// <summary>
        /// 選択車両が生きてるか
        /// </summary>
        public bool IsAliveSelectedDevice
        {
            get
            {
                var result = false;
                if (SelectedDevice != null && 
                    SelectedDevice.TryGetLastNotificationTime(out var lastNotificationTime) &&
                    (DateTime.Now - lastNotificationTime).TotalSeconds < 120)
                {
                    result = true;
                }
                return result;
            }
        }

        /// <summary>
        /// 選択車両が生きてるか
        /// </summary>
        public bool HasRealtimeErrorSelectedDevice
        {
            get
            {
                var result = false;
                if (SelectedDevice != null && 
                    StreamingController.HasError(SelectedDevice.DeviceId))
                {
                    result = true;
                }
                return result;
            }
        }

        private WMDataSet.DeviceRow selectedDevice;
        /// <summary>
        /// 選択車両データ
        /// </summary>
        public WMDataSet.DeviceRow SelectedDevice
        {
            get => selectedDevice;
            set
            {
                SetProperty(ref selectedDevice, value);
                NotifyPropertyChanged(nameof(SelectedDeviceName));
                NotifyPropertyChanged(nameof(SelectedDeviceBackColor));
            }
        }

        private WMDataSet.ErrorRow selectedDeviceError;
        /// <summary>
        /// 選択車両エラーデータ
        /// </summary>
        public WMDataSet.ErrorRow SelectedDeviceError
        {
            get => selectedDeviceError;
            set
            {
                SetProperty(ref selectedDeviceError, value);
                NotifyPropertyChanged(nameof(SelectedDeviceErrorStr));
            }
        }

        private DateTime dataUpdateDate = DateTime.Now;
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime DataUpdateDate
        {
            get => dataUpdateDate;
            set => SetProperty(ref dataUpdateDate, value);
        }
        #endregion

        /// <summary>
        /// 選択車両のストリーミングの状態
        /// </summary>
        public ClientStatus StreamingStatus { get; set; }

        #region イベントパネル
        private bool isUpdatingEventList = false;
        public bool IsUpdatingEventList
        {
            get => isUpdatingEventList;
            set 
            {
                SetProperty(ref isUpdatingEventList, value);
                NotifyPropertyChanged(nameof(IsEnableEventTable));
            }
        }

        private bool isDownloadingMovie = false;
        public bool IsDownloadingMovie
        {
            get => isDownloadingMovie;
            set
            {
                SetProperty(ref isDownloadingMovie, value);
                NotifyPropertyChanged(nameof(IsEnableEventTable));
            }
        }

        public bool CanDownloadMovie => !isDownloadingMovie;

        private bool isDownloadingG = false;
        public bool IsDownloadingG
        {
            get => isDownloadingG;
            set
            {
                SetProperty(ref isDownloadingG, value);
                NotifyPropertyChanged(nameof(CanDownloadG));
                NotifyPropertyChanged(nameof(IsEnableEventTable));
                NotifyPropertyChanged(nameof(CanUpdateEvent));
            }
        }

        public bool CanDownloadG => !IsDownloadingG;

        public bool CanUpdateEvent => !IsDownloadingG;

        public bool IsEnableEventTable => !IsUpdatingEventList && !IsDownloadingMovie;
        #endregion

        #region Event Playlist
        private FfmpegCtrl FfmpegCtrl { get; set; } = new FfmpegCtrl();

        public bool CanPlayMovie => canPlayMovies.ContainsValue(true);

        private readonly Dictionary<int, bool> canPlayMovies = new Dictionary<int, bool>();

        public bool GetCanPlayMovies(int ch)
        {
            var result = false;
            if (canPlayMovies.TryGetValue(ch, out var canPlay))
            {
                result = canPlay;
            }
            return result;
        }

        public void SetCanPlayMovies(int ch, bool value)
        {
            canPlayMovies[ch] = value;
            NotifyPropertyChanged($"CanPlayCh{ch + 1}");
            NotifyPropertyChanged(nameof(CanPlayMovie));
        }

        private string playDeviceId = string.Empty;
        public string PlayDeviceId
        {
            get => playDeviceId;
            set => SetProperty(ref playDeviceId, value);
        }

        private DateTime playTimestamp = DateTime.MinValue;
        public DateTime PlayTimestamp
        {
            get => playTimestamp;
            set
            {
                SetProperty(ref playTimestamp, value);
                NotifyPropertyChanged(nameof(PlayTimestampStr));
            }
        }

        private string playDeviceName = string.Empty;
        public string PlayDeviceName
        {
            get => playDeviceName;
            set => SetProperty(ref playDeviceName, value);
        }

        public string PlayTimestampStr => (PlayTimestamp == DateTime.MinValue) ? string.Empty : $"{PlayTimestamp:yyyy/MM/dd HH:mm:ss}";

        private string playMessage = string.Empty;
        public string PlayMessage
        {
            get => playMessage;
            set => SetProperty(ref playMessage, value);
        }

        public bool CanPlayCh1 => GetCanPlayMovies(0);

        public bool CanPlayCh2 => GetCanPlayMovies(1);

        public bool CanPlayCh3 => GetCanPlayMovies(2);

        public bool CanPlayCh4 => GetCanPlayMovies(3);

        public bool CanPlayCh5 => GetCanPlayMovies(4);

        public bool CanPlayCh6 => GetCanPlayMovies(5);

        public bool CanPlayCh7 => GetCanPlayMovies(6);

        public bool CanPlayCh8 => GetCanPlayMovies(7);
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            OperationLogger = OperationLogger.GetInstance();

            // マップルのスケールは1ステップが異様に細かい。
            // 適度に間引いたスケールを右側のバーに対して使う。
            // マップルにおけるスケールの範囲は1000から5百万。
            MapScales = new int[]
            {
                1000, 2500, 5000,
                10000, 25000, 50000,
                100000, 250000, 500000,
                1000000, 2500000, 5000000,
            };
        }

        /// <summary>
        /// Iniファイル読み込み<br/>
        /// ついでにロガーの作成も行う
        /// </summary>
        /// <param name="userIndex"></param>
        public void ReadIniFile()
        {
            LocalSettings.AllowedServers = ServerInfoList.GetServers(UserIndex);
            LocalSettings.ReadIniFile();
            // 接続先サーバーの選択, 設定ファイルの読み込み
            OperationServerInfo = GetServerInfo();  // localSettings.AllowedServersを設定してから出ないと取れない

            // ロガーの作成
            if (!string.IsNullOrEmpty(LocalSettings.LogFileDirectory))
            {
                OperationLogger.LogDirectoryPath = LocalSettings.LogFileDirectory;
            }
            OperationLogger.LogFileNamePrefix = @"RealtimeViewer";
            OperationLogger.CreateLogFile();
        }

        public void CreateRequestController()
        {
            RequestController = new RequestSequence(LocalSettings, HttpClient, OperationServerInfo);
            RequestController.OfficeLocations = LocalSettings.OfficeLocations;
        }

        public void CreateStreamingController(Dispatcher dispatcher)
        {
            if (OperationServerInfo.StreamingType == StreamingTypes.Udp)
            {
                StreamingController = new UdpController(
                        MqttController, RequestController, dispatcher, LocalSettings);
            }
            else
            {
                StreamingController = new RtspController(
                        MqttController, RequestController, dispatcher, LocalSettings);
            }
            StreamingStatus = StreamingController.ClientStatus;
        }

        #region MQTT
        public void ConnectMqttServer()
        {
            if (IsEmergencyMode) 
            {
                MqttController.ConnectMQTTServer(OperationServerInfo.GetPhygicalServerInfo(), EmergencyDeviceId);
            }
            else
            {
                MqttController.ConnectMQTTServer(OperationServerInfo.GetPhygicalServerInfo());
            }
        }

        public void CloseMqttServer()
        {
            MqttController.DisposeMQTTServer();
        }

        public void AddMqttReceivedHandler<T>(MqttMessageHandler<T> handler)
        {
            MqttController.AddReceivedHandler(handler);
        }

        public void RemoveLocationHandler<T>(MqttMessageHandler<T> handler)
        {
            MqttController.AddReceivedHandler(handler);
        }
        #endregion

        public async Task<WMDataSet.OfficeDataTable> GetOfficesAsync()
        {
            return await RequestController.GetOfficesAsync(LocalSettings.ExcludeOfficeId);
        }

        public IList<OfficeInfo> GetOfficeInfos()
        {
            var result = new BindingList<OfficeInfo>();
            lock (OfficeTable)
            {
                foreach (var row in OfficeTable)
                {
                    var office = new OfficeInfo()
                    {
                        Id = row.OfficeId,
                        CompanyId = row.CompanyId,
                        Location = (row.Longitude, row.Latitude),
                        Name = row.Name,
                        Visible = row.Visible
                    };
                    result.Add(office);
                }
            }
            return result;
        }

        public async Task<WMDataSet.DeviceDataTable> GetDevicesAsync()
        {
            return await RequestController.GetDevicesAsync();
        }

        public async Task<WMDataSet.EventListDataTable> GetEventsAsync(
            int officeId, CancellationToken token, UpdateEventsProgress progress)
        {
            var result = new WMDataSet.EventListDataTable();
            DeviceTable.DefaultView.RowFilter = $"OfficeId = {officeId}";
            var allDevices = DeviceTable.DefaultView.ToTable();
            DeviceTable.DefaultView.RowFilter = string.Empty;

            var devices = DeviceTable.Where(x => x.OfficeId == officeId);
            result = await RequestController.GetAllEventsAsync(devices, token, progress);
            return result;
        }

        public async Task GetEventsAsync(
            int officeId, WMDataSet.EventListDataTable events, CancellationToken token, UpdateEventsProgress progress)
        {
            var result = new WMDataSet.EventListDataTable();
            DeviceTable.DefaultView.RowFilter = $"OfficeId = {officeId}";
            var allDevices = DeviceTable.DefaultView.ToTable();
            DeviceTable.DefaultView.RowFilter = string.Empty;

            var devices = DeviceTable.Where(x => x.OfficeId == officeId);
            await RequestController.GetAllEventsAsync(events, devices, token, progress);
        }


        public async Task GetGravityAsync(
            CancellationToken token,
            UpdateEventsProgress progress)
        {
            await RequestController.GetGravity(FilteredEventTable, token, progress);
            //await RequestController.GetGravity(EventTable, token, progress);
        }

        public async Task<DownloadResult> EventListDownloadAsync(WMDataSet.EventListRow row, CancellationToken token)
        {
            OperationLogger.Out(OperationLogger.Category.EventData, AuthedUser.Name, $"Download event {row.DeviceId}, {row.Timestamp:F}");
            return await RequestController.DownloadEventDataAsync(row.MovieId, token);
        }

        public EnumerableRowCollection<WMDataSet.PlayListRow> GetPlaylist()
        {
            return GetPlaylist(PlayDeviceId, PlayTimestamp);
        }

       
        public EnumerableRowCollection<WMDataSet.PlayListRow> GetPlaylist(string deviceId, DateTime timestamp)
        {
            return PlaylistTable.Where(item => (item.DeviceId == deviceId && item.Timestamp == timestamp));
        }

        public void RemoveAllPlayListFile()
        {
            if (PlaylistTable != null)
            {
                foreach (var playList in PlaylistTable)
                {
                    try
                    {
                        var dir = Path.GetDirectoryName(playList.FilePath);
                        if (Directory.Exists(dir))
                        {
                            Directory.Delete(dir, true);
                        }
                    }
                    catch (Exception) {}
                }
            }
        }

        public void StartStreaming()
        {
            var deviceId = SelectedDeviceId;
            if (!StreamingController.IsStarted(deviceId))
            {
                StreamingController.Start(deviceId);
            }
        }

        public void StopStreaming()
        {
            var deviceId = SelectedDeviceId;
            if (StreamingController.IsStarted(deviceId))
            {
                StreamingController.Stop(deviceId);
            }
        }

        public void StopStreamingForce(string deviceId)
        {
            StreamingController.Stop(deviceId);
        }

        public void AbortAllStreaming()
        {
            StreamingController.AbortAll();
        }

        public void SetForeground(string deviceId)
        {
            StreamingController.SetForeground(deviceId);
        }

        public void NotifyUpdateBackgroundColor()
        {
            NotifyPropertyChanged(nameof(SelectedDeviceBackColor));
        }

        public void NotifyStreamingStatus()
        {
            StreamingController.NotifyStatus(SelectedDeviceId);
        }

        public void ChangeStreamingChannel(int channel)
        {
            StreamingController.ChangeChannel(SelectedDeviceId, channel);
        }

        public bool CanChangeStreamingChannel()
        {
            return StreamingController.CanChangeChannel(SelectedDeviceId);
        }

        public int GetNearIndexInMapScales(int x)
        {
            var result = MapScales.Length - 1;
            for (var index = 1; index < MapScales.Length; index++)
            {
                if (x <= MapScales[index])
                {
                    var c = (MapScales[index - 1] + MapScales[index]) / 2;
                    if (x <= c)
                    {
                        result = index - 1;
                    }
                    else
                    {
                        result = index;
                    }
                    break;
                }
            }
            return result;
        }

        //public Point GetRandomBalloonOffset()
        //{
        //    return BalloonOffsets[Random.Next(BalloonOffsets.Length)];
        //}

        public Icon GetMarkerIcon()
        {
            return Properties.Resources.ship;
        }

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

        /// <summary>
        /// 認証なしモード
        /// </summary>
        public void SetNoUserMode()
        {
            AuthedUser = new User() 
            {
                Name = Properties.Resources.NoAuthMode,
                Permission = (int)UserBioDP.RolePermissonConverter.ToPermission(Role.Engineer),
            };
            IsUserAuthCompleted = true;
        }

        /// <summary>
        /// エンジニアモード
        /// </summary>
        public void SetEngineerMode()
        {
            AuthedUser = new User() 
            {
                Name = Properties.Resources.EngineerMode,
                Permission = (int)UserBioDP.RolePermissonConverter.ToPermission(Role.Engineer),
            };
            IsUserAuthCompleted = true;
        }

        /// <summary>
        /// 緊急モード
        /// </summary>
        /// <param name="deviceId"></param>
        public void SetEmergencyMode(string deviceId)
        {
            EmergencyDeviceId = deviceId;
            IsEmergencyMode = true;
        }

        /// <summary>
        /// 指紋認証開始
        /// </summary>
        /// <param name="eventHandler"></param>
        public void StartUserAuth(EventHandler<IdentifiedResult> eventHandler)
        {
            userAuthDp?.Dispose();
            userAuthDp = new UserAuthDp();
            userAuthDp.IdentifiedResultEvent += UserAuthDp_IdentifiedResultEvent;
            userAuthDp.IdentifiedResultEvent += eventHandler;
            userAuthDp.StartAuth();
        }

        /// <summary>
        /// 指紋認証終了
        /// </summary>
        /// <param name="eventHandler"></param>
        public void StopUserAuth()
        {
            userAuthDp?.Dispose();
        }

        /// <summary>
        /// 最終状態保存
        /// </summary>
        /// <param name="mapScale"></param>
        public void WriteLastState(int mapScale)
        {
            LocalSettings.MappingScale = mapScale;
            LocalSettings.SelectOfficeID = SelectedOfficeId;
            LocalSettings.WriteIniFile();
            WriteEndLog();
        }

        /// <summary>
        /// 終了ログ出力
        /// </summary>
        private void WriteEndLog()
        {
            var userName = string.Empty;
            if (AuthedUser != null && !string.IsNullOrEmpty(AuthedUser.Name))
            {
                userName = AuthedUser.Name;
            }
            OperationLogger.Out(OperationLogger.Category.Application, userName, @"RealtimeViewer End");
        }

        /// <summary>
        /// 指紋認証ハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserAuthDp_IdentifiedResultEvent(object sender, IdentifiedResult e)
        {
            if (e.ResultCode == 0)
            {
                // 権限を確認する
                if (e.User.Can(Permission.StreamingView) || e.User.Can(Permission.Engineer))
                {
                    AuthedUser = e.User;
                    IsUserAuthCompleted = true;
                    IsShowAuthMessage = false;
                    OperationLogger.Out(OperationLogger.Category.Authentication, e.User.Name, @"Authenticated");
                }
            }
        }
    }
}

