using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RealtimeViewer.Logger;
using RealtimeViewer.Movie;
using RealtimeViewer.Network.Mqtt;


namespace RealtimeViewer.WMShipView
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource CancellationTokenSource { get; set; } = null;

        private bool IsEventTabBinded { get; set; } = false;

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

        private void GridEventList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView dataGrid && e.ColumnIndex == 0)
            {
                for (var i = 0; i < dataGrid.Rows.Count; i++)
                {
                    if (i != e.RowIndex)
                    {
                        dataGrid.Rows[i].Cells[e.ColumnIndex].Value = false;
                    }
                }
            }
        }

        private void GridEventList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (sender is DataGridView dataGrid &&
                dataGrid.IsCurrentCellDirty)
            {
                dataGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void ComboBoxOffice_SelectedValueChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox &&
                int.TryParse($"{comboBox.SelectedValue}", out var officeId))
            {
                ViewModel.SelectedOfficeId = officeId;
                CancellationTokenSource?.Cancel();
                // 車両リストの更新
                CancellationTokenSource = new CancellationTokenSource();
                DeviceBindingSource.Filter = $"OfficeId = {officeId}";

                // イベントリストの更新
                UnbindEventDataSource();

                Task.Run(async () =>
                {
                    ViewModel.IsUpdatingEventList = true;
                    try
                    {
                        var eventList = await ViewModel.GetEventsAsync(officeId, CancellationTokenSource.Token, (now, total, isCompleted) =>
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
                    finally
                    {
                        ViewModel.IsUpdatingEventList = false;
                    }
                }, CancellationTokenSource.Token);
            }
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            //  閲覧権限あり
            if (ViewModel.IsBrowsable &&
                EventListBindingSource.Current is DataRowView rowView &&
                rowView.Row is WMDataSet.EventListRow row)
            {
                CancellationTokenSource?.Cancel();
                // 車両リストの更新
                CancellationTokenSource = new CancellationTokenSource();

                Task.Run(async () =>
                {
                    ViewModel.IsDownloadingMovie = true;
                    try
                    {
                        var movies = await ViewModel.EventListDownloadAsync(row, CancellationTokenSource.Token);
                        if (movies.Result == 0)  // 正常
                        {
                            var playlist = ViewModel.PlaylistTable;
                            row.ExtractFilePath = movies.UnzippedPath;
                            foreach (var keyValue in movies.chFile)
                            {
                                var playChannel = playlist.NewPlayListRow();
                                playChannel.Timestamp = row.Timestamp;
                                playChannel.DeviceId = row.DeviceId;
                                playChannel.Ch = keyValue.Key;
                                playChannel.FilePath = keyValue.Value;
                                playlist.AddPlayListRow(playChannel);
                            }
                            playlist.AcceptChanges();
                        }
                        else
                        {
                            row.ExtractFilePath = string.Empty;
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
            else
            {
                MessageBox.Show("閲覧する権限がありません", "確認", MessageBoxButtons.OK
                                   , MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            }

        }

        private void buttonEventListUpdate_Click(object sender, EventArgs e)
        {
            CancellationTokenSource?.Cancel();

            // イベントリストの更新
            CancellationTokenSource = new CancellationTokenSource();
            UnbindEventDataSource();

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
                        BindEventDataSource(eventList);
                    }));
                }
                finally
                {
                    ViewModel.IsUpdatingEventList = false;
                }
            }, CancellationTokenSource.Token);
        }

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

        private void buttonCancelG_Click(object sender, EventArgs e)
        {
            CancellationTokenSource?.Cancel();
        }

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

                    var title = $"Event {ViewModel.PlayDeviceId} | {ViewModel.PlayTimestampStr} | Ch{channel}";
                    ViewModel.OperationLogger.Out(OperationLogger.Category.EventData, ViewModel.AuthedUser.Name, $"Play {title}");
                    var fps = 15;
                    if (4 < channel)
                    {
                        fps = 10; // for Analog.
                    }
                    PlayffmpegControl(channel, movie.FilePath, audio.FilePath, title, 480, fps);
                }
            }
        }

        #region Class Handlers
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

        private void OnLocationReceived(object sender, MqttMessageEventArgs<MqttJsonLocation> e)
        {
            try
            {
                var deviceTable = ViewModel.DeviceTable;
                var message = e.DeserializeMessage();
                var row = deviceTable.FirstOrDefault(item => item.DeviceId == message.Device_id);
                if (row is WMDataSet.DeviceTableRow device)
                {
                    Invoke((MethodInvoker)(() => 
                    { 
                        device.Longitude = message.Lon;
                        device.Latitude = message.Lat;
                        device.AcceptChanges();
                        //deviceTable.AcceptChanges();
                    }));

                }
            }
            catch (Exception) 
            {
                // 処理なし
            }
        }

        #endregion
    }
}
