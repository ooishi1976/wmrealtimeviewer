using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using System.ComponentModel;
using RealtimeViewer.Network;
using Newtonsoft.Json;
using System.Reflection;
using System.Diagnostics;

namespace RealtimeViewer.Setting
{
    using WatchType = DeviceWatcher.WatchType;
    using UsbDevice = DeviceWatcher.UsbDevice;

    /// <summary>
    /// Iniファイルクラス
    /// </summary>
    public class SettingIni
    {
        /// <summary>
        /// 画面更新タイマーのinterval初期値
        /// </summary>
        private const int UPDATE_INTERVAL = 15000;
        /// <summary>
        /// 未使用
        /// </summary>
        private const int POSITION_DEFERMENT = 100;
        /// <summary>
        /// 地図縮尺の初期値
        /// </summary>
        private const int MAPPING_SCALE = 25000;
        /// <summary>
        /// 未使用
        /// </summary>
        private const bool OUTPUT_DEBUG_FILE = false;
        /// <summary>
        /// 未使用
        /// </summary>
        private const int LIVE_LIMIT_TIME = 5;
        /// <summary>
        /// イベントリスト取得範囲初期値
        /// </summary>
        private const int EVENT_LIST_GET_RANGE = 6;
        /// <summary>
        /// イベントリスト抽出日数初期値
        /// </summary>
        private const int EVENT_LIST_PERIOD = 60;
        /// <summary>
        /// ストリーミング配信要求リトライ回数初期値
        /// </summary>
        private const int STREAMING_REQUEST_RETRY_COUNT = 2;
        /// <summary>
        /// ストリーミングセッションリトライ回数初期値
        /// </summary>
        private const int STREAMING_SESSION_RETRY_COUNT = 2;
        /// <summary>
        /// ストリーミング配信要求待ち時間(秒)
        /// </summary>
        private const int STREAMING_REQUEST_WAIT = 30;
        /// <summary>
        /// ストリーミングセッション待ち時間(秒)
        /// </summary>
        private const int STREAMING_SESSION_WAIT = 60;
        /// <summary>
        /// ストリーミングセッション待ち時間(秒)(ウェザーメディア用)
        /// </summary>
        private const int STREAMING_SESSION_WAIT_WM = 5;
        /// <summary>
        /// ServerIndex: 0 (東武本番) 1: (開発)
        /// </summary>
        private const int SERVER_INDEX_TOBU_PRODUCTION = 0;
        /// <summary>
        /// REST API Request Timeout (秒)
        /// </summary>
        private const int REST_REQUEST_TIMEOUT = 60;
        /// <summary>
        /// REST API (Download用) Timeout (秒)
        /// </summary>
        private const int REST_DOWNLOAD_REQUEST_TIMEOUT = 120;
        /// <summary>
        /// 営業所数
        /// </summary>
        //private const int OFFICE_NUM = 22;

        //private enum Company
        //{
        //    MainOffice = 1,
        //    FormerNishiKashiwa = 17,
        //    FormerShonan = 18
        //}

        /// <summary>
        /// ファイルバージョン
        /// </summary>
        public int FileVersion { get; set; }
        /// <summary>
        /// 画面更新タイマーのinterval
        /// </summary>
        public int UpdateInterval { get; set; }
        /// <summary>
        /// 未使用
        /// </summary>
        public int PositionDeferment { get; set; }
        /// <summary>
        /// 地図縮尺
        /// </summary>
        public int MappingScale { get; set; }
        /// <summary>
        /// 未使用
        /// </summary>
        public bool OutputDebugFile { get; set; }
        /// <summary>
        /// 未使用
        /// </summary>
        public int LiveLimitTime { get; set; }
        /// <summary>
        /// 選択事業所(前回)
        /// </summary>
        public int SelectOfficeID { get; set; }
        /// <summary>
        /// 事業所リストから除外する事業所ID
        /// </summary>
        public List<int> ExcludeOfficeId { get; set; }
        /// <summary>
        /// 事業所所在地(緯度経度)
        /// </summary>
        public Dictionary<int, (int latitude, int longitude)> OfficeLocations { get; set; }
        /// <summary>
        /// USB監視対象 指紋認証装置情報
        /// </summary>
        public Dictionary<string, UsbDevice> WatchTargets { get; set; }
        /// <summary>
        /// イベントリスト取得範囲
        /// </summary>
        public int EventListGetRange { get; set; }
        /// <summary>
        /// イベントリスト抽出日数
        /// </summary>
        public int EventListPeriod { get; set; }
        /// <summary>
        /// ストリーミング配信要求リトライ回数
        /// (PCから車載器へMQTTで配信要求をする際のリトライ回数)
        /// </summary>
        public int StreamingRequestRetryCount { get; set; }
        /// <summary>
        /// ストリーミングセッションリトライ回数
        /// (ストリーミングサーバーにファイルが置かれない場合のリトライ回数。
        ///  配信要求からやり直す)
        /// </summary>
        public int StreamingSessionRetryCount { get; set; }
        /// <summary>
        /// ストリーミング配信要求待ち時間(秒)
        /// </summary>
        public int StreamingRequestWait { get; set; }
        /// <summary>
        /// ストリーミングセッション待ち時間(秒)
        /// </summary>
        public int StreamingSessionWait { get; set; }
        /// <summary>
        /// 緊急通報ポップアップ使用
        /// </summary>
        public bool UseEmergencyPopUp { get; set; }
        /// <summary>
        /// REST API Request Timeout
        /// </summary>
        public int RestRequestTimeout { get; set; }
        /// <summary>
        /// REST API (Download用) Request Timeout
        /// </summary>
        public int RestDownloadRequestTimeout { get; set; }
        /// <summary>
        /// ログファイルディレクトリ
        /// </summary>
        public string LogFileDirectory { get; set; }
        /// <summary>
        /// イベント映像の時間(秒)
        /// </summary>
        public double PrePostDuration { get; set; }

        #region 接続設定
        /// <summary>
        /// 接続可能サーバーリスト
        /// </summary>
        public BindingList<OperationServerInfo> AllowedServers { get; set; }

        public OperationServerInfo OperationServer { get; private set; }

        /// <summary>
        /// サーバーインデックス
        /// </summary>
        public int ServerIndex { get; set; }
        /// <summary>
        /// サーバー切替後
        /// </summary>
        public bool IsChangedServer { get; set; }
        ///// <summary>
        ///// RestAPIサーバー
        ///// </summary>
        //public string RestServer { get; set; }
        ///// <summary>
        ///// Mqttサーバー
        ///// </summary>
        //public string MqttServer { get; set; }
        ///// <summary>
        ///// アクセスID
        ///// </summary>
        //public string AccessId { get; set; }
        ///// <summary>
        ///// アクセスパスワード
        ///// </summary>
        //public string AccessPassword { get; set; }
        #endregion

        public void ReadIniFile()
        {
            var sessionWaitDefault = STREAMING_SESSION_WAIT;
            // .exeを.iniに
            string sAppExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string sIniFilePath = sAppExePath.Replace(".exe", ".ini");
            UtilIniFile.UtilSetIniFilePath(sIniFilePath);
            if (File.Exists(sIniFilePath))
            {
                // 接続設定読込
                ServerIndex = UtilIniFile.getValueInt("ServerInfo", "ServerIndex", -1);
                IsChangedServer = UtilIniFile.getValueBoolean("ServerInfo", "ChangedServer", true);
                try
                {
                    ServerIndex serverIndex = (ServerIndex)Enum.ToObject(typeof(ServerIndex), ServerIndex);
                    if (serverIndex == Network.ServerIndex.WeatherMedia)
                    {
                        sessionWaitDefault = STREAMING_SESSION_WAIT_WM;
                    }
                    OperationServer = AllowedServers.FirstOrDefault(x => x.Id == serverIndex);
                    if (OperationServer == null)
                    {
                        OperationServer = AllowedServers[0];
                        ServerIndex = (int)OperationServer.Id;
                        IsChangedServer = true;
                    }
                }
                catch (Exception)
                {
                    OperationServer = AllowedServers[0];
                    ServerIndex = (int)OperationServer.Id;
                    IsChangedServer = true;
                }

                FileVersion = UtilIniFile.getValueInt("General", "fileVersion", 0);
                UpdateInterval = UtilIniFile.getValueInt("Timer", "updateInterval", UPDATE_INTERVAL);
                PositionDeferment = UtilIniFile.getValueInt("GPS", "positionDefer", POSITION_DEFERMENT);
                MappingScale = UtilIniFile.getValueInt("GPS", "mappingScale", MAPPING_SCALE);
                OutputDebugFile = UtilIniFile.getValueBoolean("Debug", "bOutputFile", OUTPUT_DEBUG_FILE);
                LiveLimitTime = UtilIniFile.getValueInt("Timer", "liveLimitTIme", LIVE_LIMIT_TIME);
                SelectOfficeID = UtilIniFile.getValueInt("Office", "selectOffice", 0);
                EventListGetRange = UtilIniFile.getValueInt("EventList", "eventListGetRange", EVENT_LIST_GET_RANGE);
                EventListPeriod = UtilIniFile.getValueInt("EventList", "eventListPeriod", EVENT_LIST_PERIOD);
                UseEmergencyPopUp = UtilIniFile.getValueBoolean("EventList", "UseEmergencyPopUp", true);
                PrePostDuration = UtilIniFile.getValueDouble("EventList", "PrePostDuration", 10D);
                StreamingRequestRetryCount = UtilIniFile.getValueInt("Streaming", "StreamingRequestRetryCount", STREAMING_REQUEST_RETRY_COUNT);
                StreamingSessionRetryCount = UtilIniFile.getValueInt("Streaming", "StreamingSessionRetryCount", STREAMING_SESSION_RETRY_COUNT);
                StreamingRequestWait = UtilIniFile.getValueInt("Streaming", "StreamingRequestWait", STREAMING_REQUEST_WAIT);
                StreamingSessionWait = UtilIniFile.getValueInt("Streaming", "StreamingSessionWait", sessionWaitDefault);
                RestRequestTimeout = UtilIniFile.getValueInt("Timer", "RestRequestTimeout", REST_REQUEST_TIMEOUT);
                RestDownloadRequestTimeout = UtilIniFile.getValueInt("Timer", "RestDownloadTimeout", REST_DOWNLOAD_REQUEST_TIMEOUT);
                LogFileDirectory = UtilIniFile.getValueString("General", "logFileDirectory", string.Empty);

                // 監視するUSB機器のハードウェアID
                Dictionary<string, string> watchTargets = UtilIniFile.getValuesString("WatchTargetDevice");
                if (0 == watchTargets.Count)
                {
                    // 未定義の場合はハードコードの値を使う
                    WatchTargets = GetWatchTargets();
                }
                else
                {
                    WatchTargets = ConvertWatchTargets(watchTargets);
                }


                // 事業所の位置情報
                Dictionary<string, string> locations = UtilIniFile.getValuesString("OfficeLocations");
                Dictionary<int, (int latitude, int longitude)> defaultLocations = GetOfficeLocations();
                if (IsChangedServer || locations.Count < defaultLocations.Count)
                {
                    // Iniに設定がない場合、
                    // 営業所位置情報が定義より少ない場合、ハードコードの値を使う
                    OfficeLocations = defaultLocations;
                    ExcludeOfficeId = GetExcludeOfficeIds();
                    SelectOfficeID = 0;
                }
                else
                {
                    OfficeLocations = ConvertLocations(locations);
                    // 営業所除外設定読込
                    ExcludeOfficeId = UtilIniFile.getValueIntList(
                            "Office", "excludeOfficeId", ',', GetExcludeOfficeIdsString());
                }

                // サーバー切替フラグを落とす
                IsChangedServer = false;

                // Migration.
                if (0 == FileVersion)
                {
                    // 東武バスイースト -> 東武バスセントラル移行対応
                    FileVersion = 1;
                    UpdateInterval = UPDATE_INTERVAL;
                    ExcludeOfficeId = GetExcludeOfficeIds();
                    OfficeLocations = GetOfficeLocations();
                }
                if (1 == FileVersion)
                {
                    FileVersion = 2;
                    StreamingRequestRetryCount = STREAMING_REQUEST_RETRY_COUNT;
                    StreamingSessionRetryCount = STREAMING_SESSION_RETRY_COUNT;
                    StreamingRequestWait = STREAMING_REQUEST_WAIT;
                    StreamingSessionWait = sessionWaitDefault;
                    UseEmergencyPopUp = true;
                    LogFileDirectory = string.Empty;
                    PrePostDuration = 10D;
                }
            }
            else
            {
                OperationServer = AllowedServers[0];
                ServerIndex = (int)OperationServer.Id;

                FileVersion = 2;
                UpdateInterval = UPDATE_INTERVAL;
                PositionDeferment = POSITION_DEFERMENT;
                MappingScale = MAPPING_SCALE;
                OutputDebugFile = OUTPUT_DEBUG_FILE;
                LiveLimitTime = LIVE_LIMIT_TIME;
                SelectOfficeID = 0;

                WatchTargets = GetWatchTargets();
                EventListGetRange = EVENT_LIST_GET_RANGE;
                EventListPeriod = EVENT_LIST_PERIOD;
                StreamingRequestRetryCount = STREAMING_REQUEST_RETRY_COUNT;
                StreamingSessionRetryCount = STREAMING_SESSION_RETRY_COUNT;
                StreamingRequestWait = STREAMING_REQUEST_WAIT;
                StreamingSessionWait = (ServerIndex == Network.ServerIndex.WeatherMedia.Value()) ? STREAMING_SESSION_WAIT_WM : STREAMING_SESSION_WAIT;
                UseEmergencyPopUp = true;
                
                ExcludeOfficeId = GetExcludeOfficeIds();
                OfficeLocations = GetOfficeLocations();

                RestRequestTimeout = REST_REQUEST_TIMEOUT;
                RestDownloadRequestTimeout = REST_DOWNLOAD_REQUEST_TIMEOUT;
                LogFileDirectory = string.Empty;
                PrePostDuration = 10D;
                IsChangedServer = false;
            }
        }

        public void WriteIniFile()
        {
            // .exeを.iniに
            string sAppExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string sIniFilePath = sAppExePath.Replace(".exe", ".ini");
            UtilIniFile.UtilSetIniFilePath(sIniFilePath);

            UtilIniFile.setValue("General", "fileVersion", FileVersion);
            UtilIniFile.setValue("General", "logFileDirectory", LogFileDirectory);
            UtilIniFile.setValue("Timer", "updateInterval", UpdateInterval);
            UtilIniFile.setValue("Timer", "RestRequestTimeout", RestRequestTimeout);
            UtilIniFile.setValue("Timer", "RestDownloadRequestTimeout", RestDownloadRequestTimeout);
            UtilIniFile.setValue("GPS", "positionDefer", PositionDeferment);
            UtilIniFile.setValue("GPS", "mappingScale", MappingScale);
            UtilIniFile.setValue("Debug", "bOutputFile", OutputDebugFile);
            UtilIniFile.setValue("Timer", "liveLimitTime", LiveLimitTime);
            UtilIniFile.setValue("Office", "selectOffice", SelectOfficeID);
            UtilIniFile.setValue("Office", "excludeOfficeId", ExcludeOfficeId, ",");
            UtilIniFile.setValue("Streaming", "StreamingRequestRetryCount", StreamingRequestRetryCount);
            UtilIniFile.setValue("Streaming", "StreamingSessionRetryCount", StreamingSessionRetryCount);
            UtilIniFile.setValue("Streaming", "StreamingRequestWait", StreamingRequestWait);
            UtilIniFile.setValue("Streaming", "StreamingSessionWait", StreamingSessionWait);
            UtilIniFile.setValue("EventList", "UseEmergencyPopUp", UseEmergencyPopUp);
            UtilIniFile.setValue("EventList", "PrePostDuration", PrePostDuration);

            
            UtilIniFile.setValue("ServerInfo", "ServerIndex", ServerIndex);
            UtilIniFile.setValue("ServerInfo", "ChangedServer", IsChangedServer);

            WriteLocations();
            WriteWatchTargets();
            WriteServerInfo();
        }

        /// <summary>
        /// 事業所の位置情報を取得する(デフォルト値)
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, (int latitude, int longitude)> GetOfficeLocations()
        {
            Dictionary<int, (int latitude, int longitude)> dict = null;
            var resourceName = "RealtimeViewer.resources.offices.json";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                if (JsonConvert.DeserializeObject<Dictionary<ServerKind, Dictionary<int, Dictionary<string, int>>>>(json) is Dictionary<ServerKind, Dictionary<int, Dictionary<string, int>>> officeInfos) 
                {
                    if (officeInfos.ContainsKey(OperationServer.Kind))
                    {
                        dict = new Dictionary<int, (int latitude, int longitude)>();
                        foreach (var keyValue in officeInfos[OperationServer.Kind])
                        {
                            // 緯度経度の変数名と値が逆になって定義されているので、踏襲
                            // Jsonファイルは緯度経度の値と意味は一致させている。
                            dict[keyValue.Key] = (keyValue.Value["longitude"], keyValue.Value["latitude"]);
                        }
                    }
                }
            }
            return dict;
        }

        ///// <summary>
        ///// 事業所の位置情報を取得する(デフォルト値)
        ///// </summary>
        ///// <returns></returns>
        //private Dictionary<int, (int latitude, int longitude)> GetOfficeLocations()
        //{
        //    Dictionary<int, (int latitude, int longitude)> dict = null;
        //    switch (OperationServer.Kind)
        //    {
        //        case ServerKind.Tobu:
        //            // 移動経度の変数名と値が逆になってる。。。。
        //            dict = new Dictionary<int, (int latitude, int longitude)>() {
        //                {1, (503329796, 128544617)}, // 本社
        //                {2, (503260015, 128862377)}, // 足立
        //                {3, (503217954, 128804060)}, // 西新井
        //                {4, (503497589, 128806274)}, // 葛飾
        //                {5, (503399598, 128839972)}, // 花畑
        //                {6, (503298994, 129039428)}, // 草加
        //                {7, (503445981, 128917387)}, // 八潮
        //                {8, (503507389, 129069301)}, // 三郷
        //                {9, (503464055, 129177134)}, // 吉川
        //                {10, (502594981, 129449996)}, // 大宮
        //                {11, (502968001, 129368939)}, // 岩槻
        //                {12, (502709522, 129237426)}, // 天沼
        //                {13, (502407926, 129470397)}, // 上尾
        //                {14, (502136817, 129347445)}, // 川越
        //                {15, (501929729, 129448175)}, // 坂戸
        //                {16, (502413153, 128927405)}, // 新座
        //                {17, (503811379, 129154729)}, // 西柏(旧東武バスイースト)
        //                {18, (504077915, 128962378)}, // 沼南(旧東武バスイースト)
        //                {19, (502659558, 132291221)}, // 日光
        //                {20, (503811379, 129154729)}, // 西柏(東武バスセントラル)
        //                {21, (504077915, 128962378)}, // 沼南(東武バスセントラル)
        //            };
        //            break;
        //        case ServerKind.Dev:
        //            dict = new Dictionary<int, (int latitude, int longitude)>() {
        //                {1, (503214235, 128487665)}, // 東京都千代田区岩本町2-16-2
        //                {2, (507329654, 135573847)}, // 福島県南相馬市原町区深野庚塚346-1
        //                {3, (524139000, 155912000)}, // 北海道根室市宝林町５丁目２
        //                {4, (503214235, 128487665)}, // 東京都千代田区岩本町2-16-2
        //            };
        //            break;
        //    }
        //    return dict;
        //}

        /// <summary>
        /// 事業所リストから除外する事業所ID
        /// </summary>
        /// <returns></returns>
        private List<int> GetExcludeOfficeIds()
        {
            return OperationServer.InvisibleOfficeIds;
        }

        /// <summary>
        /// 事業所リストから除外する事業所IDの文字列
        /// </summary>
        /// <returns></returns>
        private string GetExcludeOfficeIdsString()
        {
            string result = "";
            foreach (var (id, index) in GetExcludeOfficeIds().Select((id, index) => (id, index)))
            {
                if (0 < index)
                {
                    result += $",{id}";
                }
                else
                {
                    result = $"{id}";
                }
            }
            return result;
        }

        /// <summary>
        /// ファイルから読み取った位置情報(文字列)を数値に変換する
        /// </summary>
        /// <param name="locationStrings"></param>
        /// <returns></returns>
        private Dictionary<int, (int latitude, int longitude)> ConvertLocations(Dictionary<string, string> locationStrings)
        {
            Dictionary<int, (int latitude, int longitude)> result = new Dictionary<int, (int latitude, int longitude)>();
            foreach (var keyPair in locationStrings)
            {
                Match matches = Regex.Match(keyPair.Value, @"(\d+),(\d+)");
                if (matches.Success)
                {
                    if (int.TryParse(keyPair.Key, out int key))
                    {
                        int.TryParse(matches.Groups[1].Value, out int latitude);
                        int.TryParse(matches.Groups[2].Value, out int longitude);
                        result[key] = (latitude, longitude);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 位置情報(数値)から文字列へ変換してiniファイルに出力する
        /// </summary>
        private void WriteLocations()
        {
            UtilIniFile.deleteSection("OfficeLocations");
            foreach (var keyPair in OfficeLocations)
            {
                UtilIniFile.setValue("OfficeLocations", keyPair.Key.ToString(), $@"{keyPair.Value.latitude},{keyPair.Value.longitude}");
            }
        }

        /// <summary>
        /// USB監視対象の指紋認証装置の情報を取得する。(デフォルト値)
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, UsbDevice> GetWatchTargets()
        {
            Dictionary<string, UsbDevice> watchTargets = new Dictionary<string, UsbDevice>
            {
                {"DP4500", new UsbDevice() { WatchType = WatchType.PNP, VendorId = "05BA", ProductId = "000A" } }
            };
            return watchTargets;
        }

        /// <summary>
        /// USB監視対象の指紋認証装置の情報を取得する。
        /// VID, PIDの文字列からクラス構造に変換する。
        /// </summary>
        /// <param name="watchTagets"></param>
        /// <returns></returns>
        private Dictionary<string, UsbDevice> ConvertWatchTargets(Dictionary<string, string> watchTagets)
        {
            Dictionary<string, UsbDevice> result = new Dictionary<string, UsbDevice>();
            foreach (var keyPair in watchTagets)
            {
                Match matches = Regex.Match(keyPair.Value, @"(USB|PNP),([\da-fA-F]+),([\da-fA-F]+)");
                if (matches.Success)
                {
                    if (Enum.TryParse(matches.Groups[1].Value, out WatchType watchType))
                    {
                        result[keyPair.Key] = new UsbDevice() {
                            WatchType = watchType,
                            VendorId = matches.Groups[2].Value,
                            ProductId = matches.Groups[3].Value
                        };
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// USB監視対象の指紋認証装置の情報をIniファイルに出力する。
        /// </summary>
        private void WriteWatchTargets()
        {
            foreach (var keyPair in WatchTargets)
            {
                UtilIniFile.setValue(
                    "WatchTargetDevice", 
                    keyPair.Key, 
                    $@"{keyPair.Value.WatchType},{keyPair.Value.VendorId},{keyPair.Value.ProductId}");
            }
        }

        private void WriteServerInfo()
        {
            WriteDisableKeyValue("ServerInfo", "ServerIndex", "#ServerIndex", $"{SERVER_INDEX_TOBU_PRODUCTION}");
            WriteDisableKeyValue("ServerInfo", "RestServer", "#RestServer", "");
            WriteDisableKeyValue("ServerInfo", "MqttServer", "#MqttServer", "");
            WriteDisableKeyValue("ServerInfo", "AccessId", "#AccessId", "");
            WriteDisableKeyValue("ServerInfo", "AccessPassword", "#AccessPassword", "");
        }

        private void WriteDisableKeyValue(string section, string enableKey, string disableKey, string value)
        {
            var enableOption = UtilIniFile.getValueString(section, enableKey, null);
            var disableOption = UtilIniFile.getValueString(section, disableKey, null);
            if (string.IsNullOrEmpty(enableOption) && string.IsNullOrEmpty(disableOption))
            {
                UtilIniFile.setValue(section, disableKey, value);
            }
        }
    }
}
