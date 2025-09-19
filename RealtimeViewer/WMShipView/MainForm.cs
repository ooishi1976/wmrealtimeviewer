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
using Microsoft.VisualBasic.Devices;
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
using RealtimeViewer.WMShipView.Streaming;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UserBioDP;

namespace RealtimeViewer.WMShipView
{
    using Assembly = System.Reflection.Assembly;

    public partial class MainForm : Form
    {
        /// <summary>
        ///  サイドパネルサイズ
        /// </summary>
        private const int LEFT_PANEL_WIDTH = 240;

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
                        WaitFormViewModel.IsShowAuthWarning = false;
                        //if (WaitForm != null && WaitForm.IsDisposed)
                        //{
                        //    //TabControlUpdate(true);
                        //}
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

            // ストリーミングコントローラ作成
            ViewModel.CreateStreamingController(Dispatcher.CurrentDispatcher);

            // MQTTリスナー登録
            ViewModel.AddMqttReceivedHandler<MqttJsonLocation>(OnLocationReceived);
            ViewModel.AddMqttReceivedHandler<MqttJsonError>(OnErrorReceived);
            ViewModel.AddMqttReceivedHandler<MqttJsonEventAccOn>(OnAccOnReceived);
            ViewModel.AddMqttReceivedHandler<MqttJsonPrepostEvent>(OnPrepostReceived);
            //ViewModel.AddMqttReceivedHandler<MqttJsonStreamingStatus>(OnStreamingStatusReceived);

            // MQTTサーバー接続
            ViewModel.ConnectMqttServer();

            // 地図描画
            SyncMapScale(true);
            mpgMap.RePaint();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // ストリーミング中止
                ViewModel.AbortAllStreaming();

                // イベント関連情報取得タスクキャンセル
                if (CancellationTokenSource != null &&
                    !CancellationTokenSource.IsCancellationRequested)
                {
                    CancellationTokenSource.Cancel();
                }

                // 動画再生関連タスクキャンセル
                if (MovieCancellationTokenSource != null &&
                    !MovieCancellationTokenSource.IsCancellationRequested) 
                { 
                    MovieCancellationTokenSource.Cancel();
                }

                // FFMPEG 関連タスクキャンセル
                FfmpegCtrl.KillAllProcesses();

                if (!ViewModel.IsEmergencyMode && 
                    ViewModel.LocalSettings.OperationServer.IsShowWaitDialog)
                {
                    ViewModel.StopUserAuth();
                    StopDeviceWatcher();
                }

                // MQTT停止
                ViewModel.CloseMqttServer();

                // イベント動画のテンポラリ削除
                ViewModel.RemoveAllPlayListFile();

                // 開いた子画面を閉じる
                CloseAllRemoteSettingWindow();
                CloseAllMovieRequestWindow();
            }
            finally 
            {
                // 最終状態保存
                ViewModel.WriteLastState(mpgMap.MapScale);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            var waitTask = Task.Run(async () =>
            {
                var offices = await ViewModel.GetOfficesAsync();
                SetOffices(offices);

                var devices = await ViewModel.GetDevicesAsync();
                SetDevices(devices);

                Invoke((MethodInvoker)(() =>
                {
                    // TODO 拡張パネル追加
                    CreateExtTabPage(this);

                    // 画面項目のバインド
                    BindDeviceDataSource();
                    BindStreamingDataSource();

                    if (ViewModel.IsEmergencyMode)
                    {
                        lock (ViewModel.DeviceTable)
                        {
                            var emergencyDevice = ViewModel.DeviceTable.FindByDeviceId(ViewModel.EmergencyDeviceId);
                            if (emergencyDevice is WMDataSet.DeviceRow device)
                            {
                                // 選択値が変わっていない場合、変更イベントを強制的に発動する
                                if (!SetOfficePostion(device.OfficeId))
                                {
                                    OfficeBindingSource_CurrentChanged(OfficeBindingSource, new EventArgs());
                                }

                                // 選択値が変わっていない場合、変更イベントを強制的に発動する
                                if (!SetDevicePostion(device.DeviceId))
                                {
                                    DeviceBindingSource_CurrentChanged(DeviceBindingSource, new EventArgs());
                                }

                                // 選択船舶表示を選択
                                radioButtonSelect.Checked = true;
                            }
                        }
                    }
                    else
                    {
                        // 選択値が変わっていない場合、変更イベントを強制的に発動する
                        if (!SetOfficePostion(ViewModel.LocalSettings.SelectOfficeID)) 
                        {
                            //OfficeBindingSource.ResetCurrentItem();
                            OfficeBindingSource_CurrentChanged(OfficeBindingSource, new EventArgs());
                        }
                    }
                    DrawMapEntries();
                    if (!ViewModel.IsEmergencyMode)
                    {
                        timerGPSDraw.Start();
                    }
                }));
            });
            waitTask.ContinueWith((t) =>
            {
                ViewModel.DataUpdateDate = DateTime.Now;
            });
            WaitFormViewModel = new WaitFormViewModel(Dispatcher.CurrentDispatcher, waitTask);
            WaitForm = new WaitingForm(WaitFormViewModel);
            WaitForm.ShowDialog();
        }

        /// <summary>
        /// 地図描画用タイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerGPSDraw_Tick(object sender, EventArgs e)
        {
            DrawMapEntries();
            ViewModel.DataUpdateDate = DateTime.Now;
        }

        /// <summary>
        /// 車両リスト選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (DeviceBindingSource.Current is DataRowView rowView &&
                rowView.Row is WMDataSet.DeviceRow device)
            {
                ViewModel.SelectedDeviceId = device.DeviceId;
                ViewModel.SelectedDevice = device;
                lock (ViewModel.ErrorTable)
                {
                    ViewModel.SelectedDeviceError = ViewModel.ErrorTable.FirstOrDefault(item => item.DeviceId == device.DeviceId);
                }

                ViewModel.SetForeground(device.DeviceId);
            }
            else
            {
                ViewModel.SelectedDeviceId = string.Empty;
                ViewModel.SelectedDevice = default;
                ViewModel.SelectedDeviceError = default;
            }
            ViewModel.NotifyStreamingStatus();

            if (ViewModel.IsDeviceFocus)
            {
                timerGPSDraw.Stop();
                MoveToDeviceLocation();
                DrawMapEntries();
                timerGPSDraw.Start();
            }
        }

        /// <summary>
        /// 車両リストクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCarList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            LeftPanelShow();
        }

        /// <summary>
        /// 車両リストヘッダクリックイベント<br/>
        /// ソート処理<br/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridCarList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 処理なし
        }

        /// <summary>
        /// 選択車両表示変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton radio) 
            {
                ViewModel.IsDeviceFocus = radio.Checked;
            }

            if (ViewModel.IsDeviceFocus)
            {
                MoveToDeviceLocation();
            }
            DrawMapEntries();
        }

        /// <summary>
        /// ズームバーにおけるマウスアップイベントを処理する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomBar_MouseUp(object sender, MouseEventArgs e)
        {
            // ズームバーの値と地図のスケールが一致する場合は何もしない。
            if (mpgMap.MapScale != ViewModel.MapScales[zoomBar.Value])
            {
                // 地図のスケールを変更する。
                mpgMap.MapScale = ViewModel.MapScales[zoomBar.Value];
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
            // e.IDがDeviceId
            if (DeviceBindingSource.List is DataView dataView)
            {
                var index = 0;
                foreach (var data in dataView)
                {
                    if (data is DataRowView rowView && 
                        rowView.Row is WMDataSet.DeviceRow deviceData &&
                        deviceData.DeviceId == e.ID)
                    {
                        DeviceBindingSource.Position = index;
                        LeftPanelShow();
                        break;
                    }
                    index++;
                }
                ChangeMapMode(MapMode.Write);
            }
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
        private void GridEventList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 選択列の場合
            if (e.ColumnIndex == 0)
            {
                // 最初にすべてチェック解除
                for (var rowIndex = 0; rowIndex < gridEventList.Rows.Count; rowIndex++)
                {
                    if (rowIndex != e.RowIndex && 
                        gridEventList[0, rowIndex].Value is bool oldValue &&
                        oldValue == true)
                    {
                        gridEventList[0, rowIndex].Value = false;
                    }
                }

                // 前回未チェック
                if (gridEventList[0, e.RowIndex].Value is bool newValue &&
                    newValue == false)
                {
                    gridEventList[0, e.RowIndex].Value = true;
                }
            }
        }

#endregion イベントタブ イベント
        private void TimerStartMQTT_Tick(object sender, EventArgs e)
        {
            // 処理なし
        }

        private void buttonLeftPanelClose_Click(object sender, EventArgs e)
        {
            LeftPanelHide();
        }

#region "リアルタイム再生関係"
        /// <summary>
        /// リアルタイム開始ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStreamStart_Click(object sender, EventArgs e)
        {
            if (ViewModel.IsAliveSelectedDevice)
            {
                ViewModel.StartStreaming();
            }
        }

        private void buttonStreamStop_Click(object sender, EventArgs e)
        {
            if (ViewModel.IsAliveSelectedDevice)
            {
                ViewModel.StopStreaming();
            }
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
            if (sender is RadioButton radio)
            {
                if (radio.Checked)
                {
                    if (ViewModel.CanChangeStreamingChannel())
                    {
                        // Sunken
                        radio.FlatStyle = FlatStyle.Popup;
                        radio.BackColor = SystemColors.ControlDark;
                        radio.UseVisualStyleBackColor = false;
                        try
                        {
                            if (int.TryParse(radio.Tag.ToString(), out var channel))
                            {
                                Debug.WriteLine($"MQTT Request a changes streaming channel: {channel}");
                                ViewModel.ChangeStreamingChannel(channel);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
                        }
                    }
                    else
                    {
                        radio.Checked = false;
                    }
                }
                else
                {
                    radio.FlatStyle = FlatStyle.Standard;
                    radio.BackColor = Color.CornflowerBlue;
                    radio.UseVisualStyleBackColor = true;
                }
            }
        }
#endregion "リアルタイム再生関係"

#region "車載器の映像データ"

        /// <summary>
        /// ドラレコ映像ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonShowDrivingMovie_Click(object sender, EventArgs e)
        {
            var device = ViewModel.SelectedDevice;
            if (device.IsAlive())
            {
                if (MovieRequestWindows == null)
                {
                    MovieRequestWindows = new Dictionary<string, MovieRequestWindow>();
                }

                if (string.IsNullOrEmpty(device.DeviceId))
                {
                    // 選択中に消えた。なにもしない。
                }
                else if (MovieRequestWindows.ContainsKey(device.DeviceId))
                {
                    var window = MovieRequestWindows[device.DeviceId];
                    window.Show();
                    window.Activate();
                }
                else
                {
                    var window = new MovieRequestWindow(ViewModel.RequestController, ViewModel);
                    window.MqttClient = ViewModel.MqttClient;
                    window.CarId = device.CarId;
                    window.DeviceId = device.DeviceId;
                    window.OfficeId = device.OfficeId;
                    MovieRequestWindows[device.DeviceId] = window;
                    if (ViewModel.AuthedUser != null)
                    {
                        window.UserName = ViewModel.AuthedUser.Name;
                    }
                    window.Show();
                }
            }
        }
#endregion "車載器の映像データ"

#region "リモート設定"

        private void buttonShowRemoteSetting_Click(object sender, EventArgs e)
        {
            var device = ViewModel.SelectedDevice;
            if (device.IsAlive())
            {
                if (RemoteSettingWindows == null)
                {
                    RemoteSettingWindows = new Dictionary<string, RemoteSetting>();
                }

                if (string.IsNullOrEmpty(device.DeviceId))
                {
                    // 選択中に消えた。なにもしない。
                }
                else if (RemoteSettingWindows.ContainsKey(device.DeviceId))
                {
                    var window = RemoteSettingWindows[device.DeviceId];
                    window.Show();
                    window.Activate();
                }
                else
                {
                    WMDataSet.ErrorRow error = null;
                    lock (ViewModel.ErrorTable)
                    {
                        error = ViewModel.ErrorTable.FirstOrDefault(item => item.DeviceId == device.DeviceId);
                    }

                    var window = new RemoteSetting();
                    window.ServerInfo = ViewModel.OperationServerInfo;
                    window.LocalSettings = ViewModel.LocalSettings;
                    window.FormClosed += RemoteSettingWindow_FormClosed;
                    window.MqttClient = ViewModel.MqttClient;
                    window.HttpClient = ViewModel.HttpClient;
                    window.DeviceId = device.DeviceId;
                    window.CarId = device.CarId;
                    RemoteSettingWindows[device.DeviceId] = window;
                    var viewModel = new WindowRemoteConfigViewModel();
                    viewModel.AuthedUser = ViewModel.AuthedUser;
                    viewModel.DeviceStatus = new DeviceStatus(error.GetErrorJson());
                    viewModel.BrList = new BitrateList();
                    viewModel.OfficeId = device.OfficeId;
                    window.ViewModel = viewModel;
                    window.Show();
                }
            }
        }

        private void RemoteSettingWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                var window = (RemoteSetting)sender;
                var did = window.DeviceId;
                if (RemoteSettingWindows.ContainsKey(did))
                {
                    RemoteSettingWindows.Remove(did);
                    Debug.WriteLine($"Removed {did} from remoteSettingWindows");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
            }
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
                if (CancellationTokenSource != null &&
                    !CancellationTokenSource.IsCancellationRequested)
                {
                    CancellationTokenSource.Cancel();
                }

                // サイドパネルの戻し
                LeftPanelHide();
                tabControlRtSelect.SelectedIndex = 0;

                // メインタブの戻し
                tabControlMain.SelectedIndex = 0;

                // ストリーミング中止
                ViewModel.AbortAllStreaming();

                // FFMPEG 関連タスクキャンセル
                FfmpegCtrl.KillAllProcesses();

                // 開いた子画面を閉じる
                CloseAllRemoteSettingWindow();
                CloseAllMovieRequestWindow();

                // 営業所の戻し
                SetOfficePostion(ViewModel.LocalSettings.SelectOfficeID);

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

        /// <summary>
        /// Officeコンボボックスの選択値を変更する
        /// </summary>
        /// <param name="officeId"></param>
        /// <returns>true: 選択値が変わった</returns>
        private bool SetOfficePostion(int? officeId = null)
        {
            var id = (officeId is null) ? ViewModel.SelectedOfficeId : officeId;
            var result = false;
            if (OfficeBindingSource.List is DataView dataView)
            {
                var index = 0;
                foreach (var data in dataView)
                {
                    if (data is DataRowView rowView &&
                        rowView.Row is WMDataSet.OfficeRow office)
                    {
                        if (office.OfficeId == id)
                        {
                            break;
                        }
                    }
                    index++;
                }

                if (dataView.Count <= index)
                {
                    index = 0;
                }
                result = (OfficeBindingSource.Position == index) ? false : true;
                OfficeBindingSource.Position = index;
            }
            return result;
        }

        private bool SetDevicePostion(string deviceId)
        {
            var result = false;
            if (DeviceBindingSource.List is DataView dataView)
            {
                var index = 0;
                foreach (var data in dataView)
                {
                    if (data is DataRowView rowView &&
                        rowView.Row is WMDataSet.DeviceRow device)
                    {
                        if (device.DeviceId == deviceId)
                        {
                            break;
                        }
                    }
                    index++;
                }

                if (dataView.Count <= index)
                {
                    index = 0;
                }
                result = (DeviceBindingSource.Position == index) ? false : true;
                DeviceBindingSource.Position = index;
            }
            return result;
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

            var configModel = new ConfigPanelModel()
            {
                Settings = LocalSettings,
                OfficeList = ViewModel.GetOfficeInfos(),
                Owner = owner,
                StreamingSessionWaitMin = sessionWaitMin
            };
            configModel.LoadConfigData();

            // コンフィグパネル設定
            configPanel = new ConfigPanel();
            configPanel.Dock = DockStyle.Fill;
            configPanel.DataModel = configModel;
        }

#endregion 拡張設定


        /// <summary>
        /// タブ選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControlMain_Selected(object sender, TabControlEventArgs e)
        {
            if (!IsEventTabBinded) 
            {
                BindEventTab();
                IsEventTabBinded = true;
            }
        }

        /// <summary>
        /// 営業所選択変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ComboBoxOffice_SelectedValueChanged(object sender, EventArgs e)
        //{
        //    if (sender is ComboBox comboBox &&
        //        int.TryParse($"{comboBox.SelectedValue}", out var officeId))
        private async void OfficeBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (sender is BindingSource bindingSource &&
                bindingSource.Current is DataRowView rowView &&
                rowView.Row is WMDataSet.OfficeRow office)
            {
                ViewModel.SelectedOfficeId = office.OfficeId;
                CancellationTokenSource?.Cancel();

                while (ViewModel.IsUpdatingEventList)
                {
                    await Task.Delay(1000);
                }

                // 車両リストの更新
                CancellationTokenSource = new CancellationTokenSource();
                if (ViewModel.IsEmergencyMode)
                {
                    DeviceBindingSource.Filter = $"DeviceId = '{ViewModel.EmergencyDeviceId}'";
                }
                else
                {
                    DeviceBindingSource.Filter = $"OfficeId = {office.OfficeId}";
                }

                MoveToOfficeLocation();
                if (ViewModel.IsDeviceFocus)
                {
                    MoveToDeviceLocation();
                }

                // 地図更新
                timerGPSDraw.Stop();
                DrawMapEntries();
                timerGPSDraw.Start();

                // イベントリストの更新
                UnbindEventDataSource();

                await Task.Run(async () =>
                {
                    ViewModel.IsUpdatingEventList = true;
                    try
                    {
                        var eventList = await ViewModel.GetEventsAsync(office.OfficeId, CancellationTokenSource.Token, (now, total, isCompleted) =>
                        {
                            this.Invoke((MethodInvoker)(() => {
                                if (isCompleted)
                                {
                                    progressBarEventListUpdate.Visible = false;
                                } 
                                else
                                {
                                    progressBarEventListUpdate.Value = now;
                                    progressBarEventListUpdate.Maximum = total;
                                    progressBarEventListUpdate.Visible = true;
                                }
                            }));
                        });

                        this.Invoke((MethodInvoker)(() => 
                        { 
                            BindEventDataSource(eventList);
                        }));
                    }
                    catch (OperationCanceledException) { }
                    finally
                    {
                        ViewModel.IsUpdatingEventList = false;
                    }
                }, CancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// ダウンロードボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            //  閲覧権限あり
            if (ViewModel.IsBrowsable)
            {

                WMDataSet.EventListRow selectedRow = null;
                lock (ViewModel.EventTable)
                {
                    selectedRow = ViewModel.EventTable.FirstOrDefault(item => item.Selected);
                }
                //if (EventListBindingSource.Current is DataRowView rowView &&
                //    rowView.Row is WMDataSet.EventListRow row &&
                //    row.Selected)
                if (selectedRow is WMDataSet.EventListRow row)
                {
                    CancellationTokenSource?.Cancel();
                    // 車両リストの更新
                    CancellationTokenSource = new CancellationTokenSource();

                    Task.Run(async () =>
                    {
                        ViewModel.IsDownloadingMovie = true;
                        try
                        {
                            var existMovie = ViewModel.PlaylistTable.Any(
                                    item => (item.Timestamp == row.Timestamp &&
                                             item.DeviceId == row.DeviceId &&
                                             item.Sequence == row.Sequence &&
                                             item.MovieType == row.MovieType));
                            if (!existMovie)
                            {
                                var movies = await ViewModel.EventListDownloadAsync(row, CancellationTokenSource.Token);
                                if (movies.Result == 0)  // 正常
                                {
                                    var playlist = ViewModel.PlaylistTable;
                                    row.ExtractFilePath = movies.UnzippedPath;
                                    foreach (var keyValue in movies.chFile)
                                    {
                                        var trackRow = playlist.FindByTimestampDeviceIdSequenceMovieTypeCh(
                                            row.Timestamp, row.DeviceId, row.Sequence, row.MovieType, keyValue.Key);
                                        if (trackRow is WMDataSet.PlayListRow channelRow)
                                        {
                                            // 更新
                                            trackRow.FilePath = keyValue.Value;
                                        }
                                        else
                                        {
                                            // 追加
                                            var playChannel = playlist.NewPlayListRow();
                                            playChannel.Timestamp = row.Timestamp;
                                            playChannel.DeviceId = row.DeviceId;
                                            playChannel.Sequence = row.Sequence;
                                            playChannel.Ch = keyValue.Key;
                                            playChannel.MovieType = row.MovieType;
                                            playChannel.FilePath = keyValue.Value;
                                            playlist.AddPlayListRow(playChannel);
                                        }
                                    }
                                    playlist.AcceptChanges();
                                }
                                else
                                {
                                    row.ExtractFilePath = string.Empty;
                                }
                            }

                            // 画面に反映
                            Invoke((MethodInvoker)(() => 
                            { 
                                ReadyToPlay(row);
                            }));
                        }
                        finally
                        {
                            ViewModel.IsDownloadingMovie = false;
                        }
                    }, CancellationTokenSource.Token);
                }
            }
            else
            {
                MessageBox.Show("閲覧する権限がありません", "確認", MessageBoxButtons.OK
                                , MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            }
        }

        /// <summary>
        /// イベント更新ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEventListUpdate_Click(object sender, EventArgs e)
        {
            CancellationTokenSource?.Cancel();

            // イベントリストの更新
            CancellationTokenSource = new CancellationTokenSource();
            UnbindEventDataSource(false);

            Task.Run(async () =>
            {
                ViewModel.IsUpdatingEventList = true;
                try
                {
                    var eventList = await ViewModel.GetEventsAsync(ViewModel.SelectedOfficeId, CancellationTokenSource.Token, (now, total, isCompleted) =>
                    {
                        Invoke((MethodInvoker)(() => {
                            if (isCompleted)
                            {
                                progressBarEventListUpdate.Visible = false;
                            } 
                            else
                            {
                                progressBarEventListUpdate.Value = now;
                                progressBarEventListUpdate.Maximum = total;
                                progressBarEventListUpdate.Visible = true;
                            }
                        }));
                    });

                    Invoke((MethodInvoker)(() => 
                    { 
                        BindEventDataSource(eventList, false);
                    }));
                }
                finally
                {
                    ViewModel.IsUpdatingEventList = false;
                }
            }, CancellationTokenSource.Token);
        }

        /// <summary>
        /// G取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGetG_Click(object sender, EventArgs e)
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                ViewModel.IsDownloadingG = true;
                try
                {
                    
                    await ViewModel.GetGravityAsync(CancellationTokenSource.Token, (now, total, isCompleted) =>
                    {
                        Invoke((MethodInvoker)(() => {
                            if (isCompleted)
                            {
                                //progressBarG.Visible = false;
                            } 
                            else
                            {
                                progressBarG.Value = now;
                                progressBarG.Maximum = total;
                                //progressBarG.Visible = true;
                                labelGstatus.Text = $"取得 {now} / {total} 件";
                            }
                        }));

                    });
                }
                finally
                {
                    ViewModel.IsDownloadingG = false;
                }
            });
        }

        /// <summary>
        /// Gキャンセル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancelG_Click(object sender, EventArgs e)
        {
            CancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// 再生ボタン1～8
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlay_Click(object sender, EventArgs e)
        {
            var pattern = @"^buttonPlay(?<ch>\d)$";
            if (sender is Button button && Regex.IsMatch(button.Name, pattern))
            { 
                var match = Regex.Match(button.Name, pattern);
                if (match.Success && int.TryParse(match.Groups["ch"].Value, out var channel)) 
                {
                    channel -= 1;
                    var playlist = ViewModel.GetPlaylist();
                    var audio = playlist.FirstOrDefault(item => item.Ch == RealtimeViewer.Model.EventInfo.AUDIO_CHANNEL_BASE);
                    var movie = playlist.FirstOrDefault(item => item.Ch == channel);
                    var title = $"Event {ViewModel.PlayDeviceId} | {ViewModel.PlayTimestampStr} | Ch{channel + 1}";
                    ViewModel.OperationLogger.Out(OperationLogger.Category.EventData, ViewModel.AuthedUser.Name, $"Play {title}");
                    var fps = 15;
                    if (ServerIndex.WeatherMedia == ViewModel.OperationServerInfo.Id ||
                        ServerIndex.Dev2 == ViewModel.OperationServerInfo.Id)
                    {
                        if (3 < channel)
                        {
                            fps = 10; // for Analog.
                        }
                    }
                    else
                    {
                        if (0 < channel)
                        {
                            fps = 10; // for Analog.
                        }
                    }

                    if (movie != null)
                    {
                        var audioFile = (audio != null) ? audio.FilePath : null;
                        PlayffmpegControl(channel, movie.FilePath, audioFile, title, 480, fps);
                    }
                }
            }
        }

        #region Class Handlers
        /// <summary>
        /// プリポスト動画再生準備進捗
        /// </summary>
        /// <param name="progress"></param>
        private void FfmpegCtrl_MovieProgress(PlayMovieProgress progress)
        {
            var msg = string.Empty;
            if (PlayMovieStatus.PreProcess == progress.PlayMovieStatus)
            {
                msg = @"準備中です...";
            }

            Invoke((MethodInvoker)(() =>
            {
                ViewModel.PlayMessage = msg;
            }));
        }

        /// <summary>
        /// MQTT位置情報受信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLocationReceived(object sender, MqttMessageEventArgs<MqttJsonLocation> e)
        {
            try
            {
                var deviceTable = ViewModel.DeviceTable;
                var message = e.DeserializeMessage();
                var row = deviceTable.FindByDeviceId(message.Device_id);
                //var row = deviceTable.FirstOrDefault(item => item.DeviceId == message.Device_id);
                if (row is WMDataSet.DeviceRow device)
                {
                    var lonMS = UtilGPSConvert.ToGPSMS(int.Parse(message.Lon));
                    var latMS = UtilGPSConvert.ToGPSMS(int.Parse(message.Lat));
                    var lonDeg10 = Convert.ToDouble(lonMS);
                    var latDeg10 = Convert.ToDouble(latMS);
                    UtilGPSConvert.Convert_Millisecond2Dgree10(ref lonDeg10, ref latDeg10);

                    var lonDMS = MapUtils.ConvertMsToDMS(lonMS);
                    var latDMS = MapUtils.ConvertMsToDMS(latMS);

                    Invoke((MethodInvoker)(() => 
                    { 
                        device.Longitude = message.Lon;
                        device.Latitude = message.Lat;
                        device.LongitudeDeg10 = $"{lonDeg10}";
                        device.LatitudeDeg10 = $"{latDeg10}";
                        device.LongitudeDMS = $"{lonDMS.d}°{lonDMS.m}'{lonDMS.s}\"E";
                        device.LatitudeDMS = $"{latDMS.d}°{latDMS.m}'{latDMS.s}\"N";
                        device.LastNotificationTime = message.Ts;
                        device.AcceptChanges();
                        //deviceTable.AcceptChanges();

                        if (ViewModel.IsEmergencyMode || 
                            (ViewModel.IsDeviceFocus && ViewModel.SelectedDeviceId == message.Device_id)) 
                        {
                            DrawMapEntries();
                            MoveToDeviceLocation();
                            ViewModel.DataUpdateDate = DateTime.Now;
                        }
                    }));
                    
                }
            }
            catch (Exception) 
            {
                // 処理なし
            }
        }

        private void OnErrorReceived(object sender, MqttMessageEventArgs<MqttJsonError> e)
        {
            try
            {
                var message = e.DeserializeMessage();
                var errors = ViewModel.ErrorTable;
                lock (errors) 
                {
                    var row = errors.FirstOrDefault(item => item.DeviceId == message.DeviceId);
                    WMDataSet.ErrorRow error;
                    if (row is WMDataSet.ErrorRow)
                    {
                        // 更新
                        error = (WMDataSet.ErrorRow)row;
                        //error.AcceptChanges();
                    }
                    else
                    {
                        error = errors.NewErrorRow();
                        error.DeviceId = message.DeviceId;
                        errors.AddErrorRow(error);
                    }
                    error.Timestamp = string.IsNullOrEmpty(message.Ts) ? string.Empty : message.Ts;
                    error.SdFree = string.IsNullOrEmpty(message.SdFree) ? string.Empty : message.SdFree;
                    error.SsdFree = string.IsNullOrEmpty(message.SsdFree) ? string.Empty : message.SsdFree;
                    error.IccId = string.IsNullOrEmpty(message.IccId) ? string.Empty : message.IccId;
                    error.Error = string.IsNullOrEmpty(message.Error) ? string.Empty : message.Error;
                    error.ErrorStr = error.GetMessage();
                    error.Version = string.Empty;
                    error.AcceptChanges();
                    errors.AcceptChanges();
                }

                var devices = ViewModel.DeviceTable;
                var deviceRow = devices.FirstOrDefault(item => item.DeviceId == message.DeviceId);
                if (deviceRow is WMDataSet.DeviceRow device) 
                {
                    device.LastNotificationTime = message.Ts;
                }
            }
            catch (Exception) { }
        }

        private void OnAccOnReceived(object sender, MqttMessageEventArgs<MqttJsonEventAccOn> e)
        {
            var message = e.DeserializeMessage();
            if (!message.IsAccOn())  // AccOff
            {
                // ストリーミング中止
                ViewModel.StopStreamingForce(message.device_id);
                // 位置情報削除
                var deviceTable = ViewModel.DeviceTable;
                var row = deviceTable.FirstOrDefault(item => item.DeviceId == message.device_id);
                if (row is WMDataSet.DeviceRow device)
                {
                    Invoke((MethodInvoker)(() => 
                    { 
                        device.Longitude = string.Empty;
                        device.Latitude = string.Empty;
                        device.LastNotificationTime = string.Empty;
                        device.AcceptChanges();
                        //deviceTable.AcceptChanges();
                    }));
                }
            }
        }

        private void OnPrepostReceived(object sender, MqttMessageEventArgs<MqttJsonPrepostEvent> e)
        {
            if (ViewModel.LocalSettings.UseEmergencyPopUp)
            {
                ShowAlertDialog(e.Message);
            }
        }

        private void StreamingStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ClientStatus clientStatus)
            {
                if (e.PropertyName == nameof(clientStatus.Channel))
                {
                    RadioButton radio;
                    switch (clientStatus.Channel)
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
                    radio.Checked = true;
                    radio.Focus();
                }
                else if (e.PropertyName == nameof(clientStatus.Status))
                {
                    if (clientStatus.Status != StreamingStatuses.Playing)
                    {
                        radioButtonRtCh1.Checked = false;
                        radioButtonRtCh2.Checked = false;
                        radioButtonRtCh3.Checked = false;
                        radioButtonRtCh4.Checked = false;
                        radioButtonRtCh5.Checked = false;
                        radioButtonRtCh6.Checked = false;
                        radioButtonRtCh7.Checked = false;
                        radioButtonRtCh8.Checked = false;
                    }
                    ViewModel.NotifyUpdateBackgroundColor();
                }
            }
        }
        #endregion


    }
}