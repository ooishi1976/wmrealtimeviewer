using RealtimeViewer.Model;
using RealtimeViewer.Setting;
using RealtimeViewer.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mamt.Args;
using Mamt;
using System.Threading;
using RealtimeViewer.Network;
using RealtimeViewer.WMShipView;

namespace RealtimeViewer.Controls
{
    public class ConfigPanelModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Iniファイル情報
        /// </summary>
        public SettingIni Settings { get; set; }

        /// <summary>
        /// 営業所リスト
        /// </summary>
        public IList<OfficeInfo> OfficeList { get; set; }

        /// <summary>
        /// ConfigPanelのOwner
        /// </summary>
        public Form Owner { get; set; }

        /// <summary>
        /// 適用ボタンが押せるか否か
        /// </summary>
        private bool isApplyConfig = true;
        public bool IsApplyConfig
        {
            get
            {
                return isApplyConfig;
            }
            set
            {
                isApplyConfig = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 配信要求リトライ回数
        /// </summary>
        private decimal streamingRequestRetryCount = 0;
        public decimal StreamingRequestRetryCount {
            get { 
                return streamingRequestRetryCount; 
            }
            set
            {
                streamingRequestRetryCount = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 配信セッションリトライ回数
        /// </summary>
        private decimal streamingSessionRetryCount = 0;
        public decimal StreamingSessionRetryCount
        {
            get
            {
                return streamingSessionRetryCount;
            }
            set
            {
                streamingSessionRetryCount = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 配信要求待ち時間(秒)
        /// </summary>
        private decimal streamingRequestWait = 0;
        public decimal StreamingRequestWait
        {
            get
            {
                return streamingRequestWait;
            }
            set
            {
                streamingRequestWait = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 配信セッション待ち時間(秒)
        /// </summary>
        private decimal streamingSessionWait = 0;
        public decimal StreamingSessionWait {
            get
            {
                return streamingSessionWait;
            }
            set
            {
                streamingSessionWait = value;
                NotifyPropertyChanged();
            }
        }

        public int StreamingSessionWaitMin
        {
            get; set;
        } = 30;

        /// <summary>
        /// 緊急通報ポップアップする/しない
        /// </summary>
        private bool useEmergencyPopUp = false;
        public bool UseEmergencyPopUp
        {
            get 
            {
                return useEmergencyPopUp;
            }
            set
            {
                useEmergencyPopUp = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// ログファイル出力先
        /// </summary>
        private string logFileDirectory = string.Empty;
        public string LogFileDirectory
        {
            get
            {
                return logFileDirectory;
            }
            set
            {
                logFileDirectory = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 営業所リスト表示設定リスト
        /// </summary>
        private BindingList<OfficeInfo> officeInfoDataSource = new BindingList<OfficeInfo>();
        public BindingList<OfficeInfo> OfficeInfoDataSource
        {
            get
            {
                return officeInfoDataSource;
            }
            set 
            {
                officeInfoDataSource = value;
                NotifyPropertyChanged();
            }
        }

        public int GetVisibleOfficeCount()
        {
            return officeInfoDataSource.Count(
                (x) =>
                {
                    return x.Visible;
                });
        }

        /// <summary>
        /// Gセンサー検索範囲(時間)
        /// </summary>
        private decimal prepostDuration = 0;
        public decimal PrepostDuration
        {
            get
            {
                return prepostDuration;
            }
            set
            {
                prepostDuration = value;
                NotifyPropertyChanged();
            }
        }

        private ServerIndex operationServer;
        public ServerIndex OperationServer
        {
            get
            {
                return operationServer;
            }
            set
            {
                operationServer = value;
                NotifyPropertyChanged();
            }
        }

        private BindingList<OperationServerInfo> serverInfoDataSource = new BindingList<OperationServerInfo>();
        public BindingList<OperationServerInfo> ServerInfoDataSource
        {
            get
            {
                return serverInfoDataSource;
            }
            set
            {
                serverInfoDataSource = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 通知イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 通知イベント
        /// </summary>
        /// <param name="propertyName">変更プロパティ名</param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Iniファイル情報から値を読み込む
        /// </summary>
        public void LoadConfigData()
        {
            if (Settings != null)
            {
                StreamingRequestRetryCount = Convert.ToDecimal(Settings.StreamingRequestRetryCount);
                StreamingSessionRetryCount = Convert.ToDecimal(Settings.StreamingSessionRetryCount);
                StreamingRequestWait = Convert.ToDecimal(Settings.StreamingRequestWait);
                StreamingSessionWait = Convert.ToDecimal(Settings.StreamingSessionWait);
                UseEmergencyPopUp = Settings.UseEmergencyPopUp;
                LogFileDirectory = Settings.LogFileDirectory;
                PrepostDuration = Convert.ToDecimal(Settings.PrePostDuration);
                
                officeInfoDataSource.Clear();
                if (OfficeList != null)
                {
                    foreach (OfficeInfo office in OfficeList)
                    {
                        officeInfoDataSource.Add(new OfficeInfo(office));
                    }
                }

                serverInfoDataSource.Clear();
                foreach (OperationServerInfo server in Settings.AllowedServers)
                {
                    serverInfoDataSource.Add(server);
                }
                OperationServer = Settings.OperationServer.Id;
            }
        }

        /// <summary>
        /// 各プロパティ値をIniファイル情報へ設定する。
        /// </summary>
        public void ApplyConfigData()
        {
            if (Settings != null)
            {
                Settings.StreamingRequestRetryCount = Convert.ToInt32(streamingRequestRetryCount);
                Settings.StreamingRequestWait = Convert.ToInt32(streamingRequestWait);
                Settings.StreamingSessionRetryCount = Convert.ToInt32(streamingSessionRetryCount);
                Settings.StreamingSessionWait = Convert.ToInt32(streamingSessionWait);
                Settings.UseEmergencyPopUp = useEmergencyPopUp;
                Settings.LogFileDirectory = logFileDirectory;
                Settings.PrePostDuration = Convert.ToDouble(prepostDuration);

                List<int> excludeOfficeList = new List<int>();
                //Dictionary<int, (int latitude, int longitude)> officeLocations = new Dictionary<int, (int latitude, int longitude)>();
                foreach (OfficeInfo office in OfficeInfoDataSource)
                {
                    if (!office.Visible)
                    {
                        excludeOfficeList.Add(office.Id);
                    }
                    if (office.Location != null)
                    {
                        Settings.OfficeLocations[office.Id] = (office.Location.Value.latitude, office.Location.Value.longitude);
                    }
                }
                Settings.ExcludeOfficeId = excludeOfficeList;
                //Settings.OfficeLocations = officeLocations;
                var selectServer = ServerInfoDataSource.FirstOrDefault(x => x.Id == OperationServer);

                if (Settings.OperationServer.Id != selectServer.Id)
                {
                    Settings.ServerIndex = (int)selectServer.Id;
                    Settings.IsChangedServer = true;
                }
            }
        }

        /// <summary>
        /// 住所検索用CountdownEvent
        /// (検索 -> 結果取得まで他の処理を止める為に使用)
        /// </summary>
        private CountdownEvent condition;

        /// <summary>
        /// 住所検索結果(緯度経度)
        /// </summary>
        private (int latiude, int logitude)? location;
        public (int latiude, int logitude)? FindedLocation { 
            get { 
                return location; 
            } 
        }

        /// <summary>
        /// 住所から緯度経度を取得する.
        /// 検索結果イベントが発生するまではCountdownEventにより処理が止まる。
        /// </summary>
        /// <param name="address">住所文字列</param>
        /// <returns>(緯度, 経度)</returns>
        public (int latiude, int logitude)? FindLocation(string address)
        {
            location = null;
            MapUtils.FindAddress(address, FindAddressAnalyFinish);
            condition = new CountdownEvent(1);
            condition.Wait();
            return location;
        }

        /// <summary>
        /// 住所検索結果イベント
        /// 検索結果から緯度経度を取得してlocationに設定する。
        /// FindLocationで止めていたCountdownEventをリリースして、処理を再開する。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void FindAddressAnalyFinish(object obj, FindAddressAnalyFinishEventArgs args)
        {
            ResultsAnaly result = args.AnalyResults;
            if (result != null)
            {
                // 住所解析が完了した。
                foreach (ResultAnaly adr in result.Address)
                {
                    location = ((int)adr.X, (int)adr.Y);
                    break;
                }
            }
            condition.Signal();
            Thread.Sleep(1);
        }
    }
}
