using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RealtimeViewer.Logger;
using RealtimeViewer.Model;
using RealtimeViewer.Movie;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network.Mqtt;
using RealtimeViewer.WMShipView;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace RealtimeViewer
{
    public partial class MovieRequestWindow : Form
    {
        enum TopicLabel
        {
            TopicNone,
            TopicStreamingStatus,
            TopicErrorStatus,
            TopicEventAccOn,
            TopicLocation,
            TopicEventDriver,
            TopicEventPrepost,
        };

        public const string MQTT_TOPIC_STREAMING_REQUEST = "car/streaming/{0}/request";
        public const string MQTT_TOPIC_REMOTECONFIG_REQUEST = "car/request/{0}/conf";

        class TopicRegex
        {
            public Regex Regex { get; set; }
            public TopicLabel Label { get; set; }
        }

        /// <summary>
        /// new するな。ウィンドウの呼び出し元(MainFormを想定)が生成したオブジェクトを指定されたし。
        /// </summary>
        public MqttClient MqttClient { get; set; }
        /// <summary>
        /// ViewModel
        /// </summary>
        public MovieRequestWindowViewModel viewModel;
        /// <summary>
        /// 車載器情報
        /// </summary>
        public int OfficeId { get; set; }
        public string CarId { get; set; }
        public string DeviceId { get; set; }

        private List<TopicRegex> topicRegexes = new List<TopicRegex>();

        /// <summary>
        /// イベント情報取得用モデル
        /// </summary>
        private RequestSequence reqSequence;

        /// <summary>
        /// 映像ダウンロード用テンポラリフォルダ
        /// </summary>
        private Dictionary<string, bool> tempPaths = new Dictionary<string, bool>();
        /// <summary>
        /// 映像処理クラス
        /// </summary>
        private FfmpegCtrl ffmpegCtrl = new FfmpegCtrl();
        /// <summary>
        /// イベント映像更新タスクのキャンセル用トークン
        /// </summary>
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private OperationLogger logger;

        public string UserName { get; set; }
        /// <summary>
        /// ドラレコ映像リスト
        /// </summary>
        private SortableBindingList<EventInfo> requestsBindingList;
        /// <summary>
        /// イベント情報のコンパレータ
        /// </summary>
        private EventInfoComparer requestComparer;
        /// <summary>
        /// ドラレコ映像リストのソート列
        /// </summary>
        private DataGridViewColumn sortedColumn;
        /// <summary>
        /// イベント選択中リスト
        /// ソート時の選択状態の復元用に用いる
        /// </summary>
        private List<EventInfo> eventSelectedList = new List<EventInfo>();

        private WMShipView.MainViewModel mainViewModel = null;

        private BindingSource movieListBindingSource;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="requestSequence"></param>
        public MovieRequestWindow(
            RequestSequence requestSequence, 
            WMShipView.MainViewModel mainViewModel = null)
        {
            InitializeComponent();
            viewModel = new MovieRequestWindowViewModel();
            viewModel.Device = new DeviceInfo();
            viewModel.IsDisposing = false;
            logger = OperationLogger.GetInstance();

            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/streaming/(?<device_id>\w+)/status"), Label = TopicLabel.TopicStreamingStatus, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/status/(?<device_id>\w+)"), Label = TopicLabel.TopicLocation, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/error/(?<device_id>\w+)"), Label = TopicLabel.TopicErrorStatus, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/engine"), Label = TopicLabel.TopicEventAccOn, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/driver"), Label = TopicLabel.TopicEventDriver, });
            topicRegexes.Add(new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/prepost"), Label = TopicLabel.TopicEventPrepost, });

            reqSequence = requestSequence;
            requestComparer = new EventInfoComparer()
            {
                Mode = EventInfoComparer.SortMode.TimeStamp,
                Order = SortOrder.Descending
            };
            // RequestSequence.Requestsが本体だが
            // 複数ウィンドウでデータを共有するため、
            // RequestSequence.RequestsをソートしてGrid表示することができない。
            // その為、Shallow Copyを作っている。
            if (mainViewModel is null)
            {
                requestsBindingList = new SortableBindingList<EventInfo>(reqSequence.Requests);
            }
            else
            {
                this.mainViewModel = mainViewModel;
                movieListBindingSource = new BindingSource();
                movieListBindingSource.DataSource = mainViewModel.EventTable;
                //movieListBindingSource.DataMember = "EventList";
                movieListBindingSource.Filter = "MovieType = 2";
                movieListBindingSource.Sort = "Timestamp";

                var movieList = movieListBindingSource.List;
                requestsBindingList = new SortableBindingList<EventInfo>();
                requestsBindingList.AddRange(movieList);
            }
            SortRequestList();
            eventInfoBindingSource.DataSource = requestsBindingList;
            // リクエスト映像更新完了時のタイミングを得る為、イベントを登録
            reqSequence.AddHandlerRequestsUpdateCompleted(RequestsListUpdateCompleted);
        }

        private void MovieRequestWindow_Load(object sender, EventArgs e)
        {
            logger.Out(OperationLogger.Category.MovieRequest, UserName, $"Open MovieRequestWindow {CarId}");
            try
            {
                MqttClient.MqttMsgPublishReceived += ArrivedMqttMessage;
                // RequestSequence.Requestsが変更されるとイベントが発報される
                // このWindowで保持しているShallow Copyの更新用に用いる
                reqSequence.Requests.ListChanged += Requests_ListChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex}, {ex.Message}");
            }
            viewModel.Device.DeviceId = DeviceId;
            viewModel.Device.CarId = CarId;

            label2.Text = viewModel.Device.CarId;
            labelValidateError.Text = string.Empty;

            //  このタイミングでないと日付系更新されない

            DateTime dt = DateTime.Now;
            DateTime dt2 = dt.AddMinutes(-10);

            numericUpDown3.Value = dt.Hour;
            numericUpDown4.Value = dt.Minute;
            numericUpDown1.Value = dt2.Hour;
            numericUpDown2.Value = dt2.Minute;

            numericUpDown1.Width = 46;
            numericUpDown2.Width = 46;
            numericUpDown3.Width = 46;
            numericUpDown4.Width = 46;
            dateTimePicker1.Width = 165;
            dateTimePicker2.Width = 165;

            label12.Visible = false;
            labelDownload.Visible = false;
            label8.Visible = false;
            progressBar1.Visible = false;
            panelPlay.Visible = false;

            //ReadEventRemark();
        }

        /// <summary>
        /// 閉じるボタン処理<br/>
        /// いったん閉じて再度、開いたとき、<br/>
        /// ダウンロードデータを再利用するため、<br/>
        /// Closeではなく、Hideする。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
            //Hide();
        }

        /// <summary>
        /// 営業所切り替えや、終了時にClose処理が発行されるタイミングで、<br/>
        /// このウィンドウも破棄する。<br/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovieRequestWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            logger.Out(OperationLogger.Category.MovieRequest, UserName, $"Closed MovieRequestWindow {CarId}");
            try
            {
                MqttClient.MqttMsgPublishReceived -= ArrivedMqttMessage;
                reqSequence.Requests.ListChanged -= Requests_ListChanged;
                reqSequence.RemoveHandlerRequestsUpdateCompleted(RequestsListUpdateCompleted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex}, {ex.Message}");
            }

            // 非同期処理を止めておきたい。
            // プレイヤーについては放置で良い。リソースを食いつぶしそうなものを止めたい。
            cancellationTokenSource.Cancel();

            ffmpegCtrl.KillAllProcesses();

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

            //  gridviewコメントの保存
            //WriteEventRemark();
        }

        private void ReadEventRemark()
        {
            //  備考欄読み込み
            string stCurrentDir = Environment.CurrentDirectory;
            string folderPath = stCurrentDir + "\\remarkList";
            string filePath = folderPath + "\\drvRecList.dat";
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

        private void WriteEventRemark()
        {
            //  備考欄保存
            string stCurrentDir = Environment.CurrentDirectory;
            string folderPath = stCurrentDir + "\\remarkList";
            string filePath = folderPath + "\\drvRecList.dat";
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

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            //  データ取得
            var btn = (Button)sender;
            labelUploadComplete.Text = string.Empty;

            try
            {
                DateTime sdt = dateTimePicker1.Value;
                DateTime edt = dateTimePicker2.Value;
                var startBase = new DateTime(sdt.Year, sdt.Month, sdt.Day);
                var startSpan = new TimeSpan((int)numericUpDown1.Value, (int)numericUpDown2.Value, 0);
                startBase += startSpan;

                var endBase = new DateTime(edt.Year, edt.Month, edt.Day);
                var endSpan = new TimeSpan((int)numericUpDown3.Value, (int)numericUpDown4.Value, 0);
                endBase += endSpan;

                var duration = endBase - startBase;
                if (duration.TotalMinutes < 0 || duration.TotalMinutes > MovieRequestWindowViewModel.RANGE_MAX_MINUTES)
                {
                    labelValidateError.Text = $"開始できません。範囲は{MovieRequestWindowViewModel.RANGE_MAX_MINUTES}分以内にしてください。";
                    return;
                }
                labelValidateError.Text = string.Empty;

                label12.Visible = true;
                progressBar1.Visible = true;
                buttonUpload.Enabled = false;
                labelProgress.Visible = true;

                labelProgress.Text = $"サーバーへ 1/{duration.TotalMinutes} 個目の送信が始まりました";

                var bs = new byte[8];
                System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
                rng.GetBytes(bs);
                //.NET Framework 4.0以降
                rng.Dispose();

                var req = new MqttJsonRequestMovieDownload();
                req.device_id = viewModel.Device.DeviceId;
                req.command = 0;
                req.req_id = Convert.ToBase64String(bs);
                req.ts_start = startBase.ToString(@"yyyyMMddHHmmss");
                req.ts_end = endBase.ToString(@"yyyyMMddHHmmss");

                viewModel.DownloadStatus[req.req_id] = new DownloadStatus();
                var json = JsonConvert.SerializeObject(req);
                var msg = System.Text.Encoding.UTF8.GetBytes(json);
                var mid = MqttClient.Publish($"car/request/{viewModel.Device.DeviceId}/upload", msg, 2, false);

                logger.Out(OperationLogger.Category.MovieRequest, UserName, $"Request MovieUpload to {CarId}, start:{req.ts_start} end:{req.ts_end}");
                Debug.WriteLine($"Request movie upload. {json}");
            }
            catch (Exception ex)
            {
                labelProgress.Visible = false;
                label12.Visible = false;
                progressBar1.Visible = false;
                buttonUpload.Enabled = true;
                Debug.WriteLine($"@@@Exception: ButtonStart_Clickm {ex}");
            }
        }

        private void UpdateUiAtUploadCompleted(DownloadStatus status)
        {
            bool needHideProgressBar = false;
            if (status == null)
            {
                needHideProgressBar = true;
                labelUploadComplete.Text = string.Empty;
            }
            else
            {
                label12.Visible = false;
                label8.Visible = true;
                labelProgress.Visible = false;

                if (status.StoredCount > 0)
                {
                    labelUploadComplete.Text = @"リストを更新中です...";
                }
                else if (status.SkipCount > 0)
                {
                    needHideProgressBar = true;
                    labelUploadComplete.Text = @"該当期間のデータがありません。";
                }
                else if (status.ErrorCount > 0)
                {
                    needHideProgressBar = true;
                    labelUploadComplete.Text = @"エラー。時間をあけて再実施ください。";
                }
            }

            if (needHideProgressBar)
            {
                progressBar1.Visible = false;
                label8.Visible = false;
                buttonUpload.Enabled = true;
            }
        }

        private void Requests_ListChanged(object sender, ListChangedEventArgs e)
        {
            // ドラレコ映像リストが更新された
            // 追加件数分呼ばれる
            // 削除処理が無いため、削除は現在考慮していない。
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                Invoke((MethodInvoker)(() =>
                {
                    EventInfo eventInfo = reqSequence.Requests.ElementAt(e.NewIndex);
                    requestsBindingList.Add(eventInfo);
                }));
            }
        }

        /// <summary>
        /// RequestSequence.Requestsの更新が完了した時に呼ばれるイベント。
        /// ドラレコ映像リストのソートと画面ボタンの有効/無効制御を行う。
        /// </summary>
        private void RequestsListUpdateCompleted()
        {
            SortRequestList();
            if (sortedColumn != null)
            {
                sortedColumn.HeaderCell.SortGlyphDirection = requestComparer.Order;
            }
            UpdateUiAtUploadCompleted(null);
        }

        void ArrivedMqttMessage(object sender, MqttMsgPublishEventArgs e)
        {
            var msg = System.Text.Encoding.UTF8.GetString(e.Message);
            //Debug.WriteLine($"Received topic:{e.Topic}, msg:{msg}");
            bool isPrepost = $"car/event/{viewModel.Device.DeviceId}/prepost" == e.Topic;
            if (isPrepost)
            {
                var status = JsonConvert.DeserializeObject<MqttJsonPrepostEvent>(msg, new IsoDateTimeConverter { DateTimeFormat = "yyyyMMddHHmmss" });
                if (status.req_id != null
                        && viewModel.DownloadStatus.TryGetValue(status.req_id, out DownloadStatus downloadStatus))
                {
                    Debug.WriteLine($"Update Movie status:{e.Topic}, msg:{msg}");
                    viewModel.ProgressCurrentValue = status.n;
                    viewModel.ProgressMaxValue = status.total;
                    viewModel.DownloadStatus[status.req_id].Total = status.total;

                    switch ((MovieUploadStatus)status.status)
                    {
                        case MovieUploadStatus.Started:
                            viewModel.ProgressText = $"サーバーへ {status.n}/{status.total} 個目の送信が始まりました";
                            break;

                        case MovieUploadStatus.Sended:
                            viewModel.ProgressText = $"サーバーへ {status.n}/{status.total} 個が送信されました";
                            viewModel.DownloadStatus[status.req_id].StoredCount++;
                            break;

                        case MovieUploadStatus.NotFound:
                            viewModel.ProgressText = $"{status.n} 個目のデータが無かったためスキップします";
                            viewModel.DownloadStatus[status.req_id].SkipCount++;
                            break;

                        case MovieUploadStatus.ErrorGeneric:
                            viewModel.ProgressText = $"{status.n} 個目でエラーが発生しましたがスキップします";
                            viewModel.DownloadStatus[status.req_id].ErrorCount++;
                            break;

                        case MovieUploadStatus.ErrorBusy:
                            viewModel.ProgressText = $"車載器がビジーのため送信できません";
                            viewModel.DownloadStatus[status.req_id].ErrorCount++;
                            break;
                    }

                    if (status.n >= status.total)
                    {
                        viewModel.IsUploading = false;
                        if (viewModel.DownloadStatus[status.req_id].StoredCount > 0)
                        {
                            if (viewModel is null)
                            {
                                var w = new TimeSpan(0, 0, 10);
                                var httpSequenceTask = reqSequence.UpdateEvents(
                                        OfficeId, w, cancellationTokenSource.Token);
                                httpSequenceTask.GetAwaiter().OnCompleted(
                                    () =>
                                    {
                                        Invoke((MethodInvoker)(() =>
                                        {
                                            reqSequence.CreateEventBindingList();
                                            reqSequence.UpdateRequestBindingList();
                                            UpdateUiAtUploadCompleted(null);
                                        }));
                                    });
                            }
                            else
                            {
                                var task = mainViewModel.GetEventsAsync(mainViewModel.SelectedOfficeId, cancellationTokenSource.Token, null);
                                var newEvents = task.GetAwaiter().GetResult();

                                // 差分抽出（dtBefore にあって dtAfter にない行）

                            }
                        }

                        Invoke((MethodInvoker)(() =>
                        {
                            UpdateUiAtUploadCompleted(viewModel.DownloadStatus[status.req_id]);
                            viewModel.DownloadStatus.Remove(status.req_id);
                        }));
                    }
                    else
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            labelProgress.Text = viewModel.ProgressText;
                        }));
                    }
                }
            }
        }

        private void gridEventList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
#if false // 2021-05-12 現在、「ダウンロード」項目が無く使われていない。ColumnIndex がずれると面倒なため残しておく。
            if (progressBar1.Visible == false)
            {
                DataGridView dgv = (DataGridView)sender;

                if (dgv.Columns[e.ColumnIndex].Name == "ColumnDownload")
                {
                    Debug.WriteLine($"===>  Clicked {e}, X{e.ColumnIndex}, Y{e.RowIndex}");

                    //  選択行の色変える
                    if (m_EventListSelectRow >= 0)
                    {
                        dgv.Rows[m_EventListSelectRow].DefaultCellStyle.BackColor = Color.Empty;
                    }
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    m_EventListSelectRow = e.RowIndex;
                    EventDownload(dgv, 0);
                }
            }
#endif
        }

        private void GridEventList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gridEventList.CurrentCellAddress.X == 0 &&
                    gridEventList.IsCurrentCellDirty)
            {
                gridEventList.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Gridセル変更時イベント<br/>
        /// チェックボックスのOn/Offを検知して、その値を保持する。<br/>
        /// ソート時にチェック状態は追従しない為、チェックをつけなおす際に用いる。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridEventList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                buttonDownload.Enabled = true;
                panelPlay.Visible = false;
                gridEventList.EndEdit();

                // 選択チェック復帰用
                var cellValue = gridEventList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                var eventInfo = gridEventList.Rows[e.RowIndex].DataBoundItem as EventInfo;
                if (cellValue != null && cellValue.ToString() == "1")
                {
                    eventSelectedList.Add(eventInfo);
                }
                else
                {
                    eventSelectedList.Remove(eventInfo);
                }
            }
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            DataGridView dgv = gridEventList;

            if (progressBar1.Visible == false)
            {
                int count = 0;
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    if (dgv.Rows[i].Cells[0].Value != null)
                    {
                        if (dgv.Rows[i].Cells[0].Value.ToString() == "1")
                        {
                            count++;
                        }
                    }
                }

                if (count > 0)
                {
                    Task.Run(() =>
                    {
                        EventDownload(dgv, count, cancellationTokenSource.Token);
                    });
                }
            }
        }

        private async void EventDownload(DataGridView dgv, int targetCount, CancellationToken ct)
        {
            try
            {
                Invoke((MethodInvoker)(() =>
                {
                    dgv.Enabled = false;
                    panelPlay.Visible = false;
                    labelDownload.Visible = true;
                    progressBar1.Visible = true;
                    buttonUpload.Enabled = false;
                    buttonDownload.Enabled = false;
                }));

                int count = 0;

                //  ダウンロード映像選択
                for (int i = (dgv.Rows.Count - 1); i >= 0; i--)
                {
                    if (ct.IsCancellationRequested)
                        break;
                    //  check = "1"  no check = null
                    if (dgv.Rows[i].Cells[0].Value != null)
                    {
                        if (dgv.Rows[i].Cells[0].Value.ToString() == "1")
                        {
                            if (ct.IsCancellationRequested)
                                break;
                            var ei = (EventInfo)dgv.Rows[i].DataBoundItem;
                            if (ei.IsDownloadable)
                            {
                                ei.IsDownloading = true;

                                Debug.WriteLine($"Download MovieId: {ei.MovieId}");
                                logger.Out(OperationLogger.Category.MovieRequest, UserName, $"Download movies on {CarId} {ei.Timestamp} (MovieId:{ei.MovieId})");

                                count++;
                                Invoke((MethodInvoker)(() =>
                                {
                                    progressBar1.Visible = true;
                                    labelDownload.Text = $"サーバーからダウンロード中です";

                                    labelProgress.Visible = true;
                                    labelProgress.Text = $"ダウンロード中 {count}/{targetCount}個目";
                                }));

                                Task task = reqSequence.DownloadEventData(ei.MovieId, (result) =>
                                {
                                    if (ct.IsCancellationRequested)
                                    {
                                        try
                                        {
                                            Directory.Delete(result.UnzippedPath, true);
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine($"@@@ Exception delete unzipped path. {ex}");
                                        }
                                    }
                                    else
                                    {
                                        ei.IsDownloading = false;
                                        if (result.Result == 0)
                                        {
                                            ei.IsDownloadable = false;
                                            ei.IsPlayable = true;
                                            ei.unzippedPath = result.UnzippedPath;
                                            ei.ChannelToMovieFileDic = result.chFile;

                                            tempPaths.Add(result.UnzippedPath, true);

                                            MakeContainerFiles(ei);
                                        }
                                        else
                                        {
                                            ei.IsDownloadable = true;
                                        }
                                    }
                                }, cancellationTokenSource.Token);

                                await task;
                            }

                            if (!ct.IsCancellationRequested)
                            {
                                PrePostPlayEx(ei);
                            }
                        }
                    }
                }

                if (!ct.IsCancellationRequested)
                {

                    Invoke((MethodInvoker)(() =>
                    {
                        panelPlay.Visible = true;
                        labelDownload.Visible = false;
                        labelProgress.Visible = false;
                        progressBar1.Visible = false;
                        buttonUpload.Enabled = true;
                        buttonDownload.Enabled = true;
                        dgv.Enabled = true;
                    }));
                }

                Debug.WriteLine($"DownloadEvent end ");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ERROR: EventDownload_Click {ex}");
                if (!ct.IsCancellationRequested)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        panelPlay.Visible = false;
                        labelDownload.Visible = false;
                        labelProgress.Visible = false;
                        progressBar1.Visible = false;
                        buttonUpload.Enabled = true;
                        buttonDownload.Enabled = true;
                        dgv.Enabled = true;
                    }));
                }
            }

        }

        private void MakeContainerFiles(EventInfo ei)
        {
            string audioFile = null;
            if (ei.ChannelToMovieFileDic.ContainsKey(EventInfo.AUDIO_CHANNEL_BASE))
            {
                audioFile = ei.ChannelToMovieFileDic[EventInfo.AUDIO_CHANNEL_BASE];
            }

            for (int ch = 0; ch < 8; ch++)
            {
                if (ei.ChannelToMovieFileDic.ContainsKey(ch))
                {
                    var movieFile = ei.ChannelToMovieFileDic[ch];
                    var di = Directory.GetParent(movieFile);
                    var avfile = FfmpegCtrl.MakeContainerFullPath(di.FullName, ei.Timestamp, ch);

                    int fps = 15;
                    if (ch > 0)
                    {
                        fps = 10; // for Analog.
                    }
                    ffmpegCtrl.MakeContainerFileSync(avfile, fps, movieFile, audioFile);
                }
            }
        }

        private Dictionary<string, object> dc = new Dictionary<string, object>();
        private EventInfo ei;
        List<Button> buttonPlays = new List<Button>();
        Dictionary<Button, int> buttonToChannel = new Dictionary<Button, int>();

        public void PrePostPlayEx(EventInfo ei)
        {
            //  再生ボタン生成
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
                var ch = buttonToChannel[(Button)sender];
                if (ei.ChannelToMovieFileDic.ContainsKey(ch))
                {
                    var title = $"Event {ei.CarId} | {ei.Timestamp} | Ch{ch + 1}";
                    Debug.WriteLine($"play {title}");
                    logger.Out(OperationLogger.Category.MovieRequest, UserName, $"Play {ei.CarId} | {ei.Timestamp} | Ch{ch + 1}");

                    var list = makePlayList(ch);
                    Task t = ffmpegCtrl.PlayList(ch, list, null);
                }
            }
        }

        /// <summary>
        /// 再生対象とするファイルリストを作る。
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private string[] makePlayList(int ch)
        {
            List<EventInfo> eventList = new List<EventInfo>();
            foreach (DataGridViewRow row in gridEventList.Rows)
            {
                var cellValue = row.Cells[0].Value;
                if (cellValue != null && cellValue.ToString() == "1")
                {
                    eventList.Add(row.DataBoundItem as EventInfo);
                }
            }

            List<string> result = new List<string>();
            var comparer = new EventInfoComparer();
            eventList.Sort(comparer);
            foreach (EventInfo eventInfo in eventList)
            {
                viewModel.DownloadedEvent = eventInfo;
                if (eventInfo.ChannelToMovieFileDic.ContainsKey(ch))
                {
                    var di = Directory.GetParent(eventInfo.ChannelToMovieFileDic[ch]);
                    var fullpath = FfmpegCtrl.MakeContainerFullPath(di.FullName, eventInfo.Timestamp, ch);
                    result.Add(fullpath);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// ×ボタン押下処理をCloseからHideにする。
        /// viewModelのIsDisposingがtrueの時はCloseが行われる。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovieRequestWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !viewModel.IsDisposing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        /// <summary>
        /// ドラレコ映像リストのフォーマットイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridEventList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (e.Value != null && e.Value is DateTime time)
                {
                    e.Value = time.ToString("yyyy/MM/dd HH:mm");
                }
            }
        }

        /// <summary>
        /// ドラレコ映像リストをソートする。</br>
        /// BindingListのソートではチェックボックスは追従しない為、<br/>
        /// 別途、チェック状態を再設定する。<br/>
        /// </summary>
        private void SortRequestList()
        {
            var tempList = new List<EventInfo>(eventSelectedList);
            foreach (var eventInfo in tempList)
            {
                int index = requestsBindingList.IndexOf(eventInfo);
                gridEventList.Rows[index].Cells[0].Value = 0;
            }
            requestsBindingList.Sort(requestComparer);
            foreach (var eventInfo in tempList)
            {
                int index = requestsBindingList.IndexOf(eventInfo);
                gridEventList.Rows[index].Cells[0].Value = 1;
            }
        }

        /// <summary>
        /// ドラレコ映像リスト列のクリックイベント<br/>
        /// ソート処理を行う。
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
                        sortMode = EventInfoComparer.SortMode.Remark;
                        break;
                }
                requestComparer.Mode = sortMode;
                if (sortedColumn == null || sortedColumn != grid.Columns[e.ColumnIndex])
                {
                    if (sortedColumn != null)
                    {
                        sortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }
                    if (sortMode == EventInfoComparer.SortMode.TimeStamp)
                    {
                        // 日時はデフォルトが降順なので昇順へ
                        requestComparer.Order = SortOrder.Ascending;
                    }
                    else
                    {
                        requestComparer.Order = SortOrder.Descending;
                    }
                    sortedColumn = grid.Columns[e.ColumnIndex];
                }
                else
                {
                    if (requestComparer.Order == SortOrder.Descending)
                    {
                        requestComparer.Order = SortOrder.Ascending;
                    }
                    else
                    {
                        requestComparer.Order = SortOrder.Descending;
                    }
                }
                SortRequestList();
                sortedColumn.HeaderCell.SortGlyphDirection = requestComparer.Order;
            }
        }
    }
}
