using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using Newtonsoft.Json;
using RealtimeViewer.Network;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network.Mqtt;
using RealtimeViewer.WMShipView.Streaming;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RealtimeViewer.WMShipView
{

    public abstract class AbstractStreamingController : IStreamingController
    {
        /// <inheritdoc/>
        protected Dispatcher Dispatcher { get; private set; } 

        /// <inheritdoc/>
        public ClientStatus ClientStatus { get; private set; }

        /// <inheritdoc/>
        public string CurrentDeviceId { get; set; }

        protected MqttController MqttController { get; set; }

        protected RequestSequence RequestController { get; set; }

        protected Dictionary<string, IStreamingClient> StreamingClients { get; set; } = new Dictionary<string, IStreamingClient>();

        protected IStreamingSettings StreamingSettings { get; set; }

        private object waitLockObject = new object();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mqttController"></param>
        public AbstractStreamingController(
            MqttController mqttController,
            RequestSequence requestController,
            Dispatcher dispatcher,
            IStreamingSettings streamingSettings)
        {
            //RequestWaitTime = requestWaitTime; 
            //SessionWaitTime = sessionWaitTime;
            StreamingSettings = streamingSettings;
            MqttController = mqttController;
            RequestController = requestController;
            Dispatcher = dispatcher;
            ClientStatus = new ClientStatus(dispatcher);
            MqttController.AddReceivedHandler<MqttJsonStreamingStatus>(OnStreamingStatusReceived);
        }

        public bool IsStarted(string deviceId)
        {
            var result = false;
            if (StreamingClients.TryGetValue(deviceId, out var client) && 
                client.Status != StreamingStatuses.RequestFailure &&
                client.Status != StreamingStatuses.PlayingFailure)
            {
                result = true;
            }
            return result;
        }

        /// <inheritdoc/>
        public void Start(string deviceId)
        {
            if (StreamingClients.TryGetValue(deviceId, out var runningClient)) 
            {
                // TODO 破棄してから
                runningClient.Stop();
            }
            //var client = new RtspClient(deviceId, MqttController);
            var client = CreateStreamingClient(deviceId);
            client.AddCounterHandler(OnPlayCounted);
            client.AddSessionWaitHandler(OnWaitCounted);
            client.AddClosedHandler(OnClosed);
            StreamingClients[deviceId] = client;
            client.RequestStartStreaming();
            if (CurrentDeviceId == deviceId)
            {
                ClientStatus.SetValues(client.GetClientStatus());
            }
        }

        /// <inheritdoc/>
        public void Stop(string deviceId) 
        {
            if (StreamingClients.TryGetValue(deviceId, out var client)) 
            {
                // TODO 破棄してから
                client.Stop();
                client.RequestStopStreaming();
                client.RemoveCounterHandler(OnPlayCounted);
                client.RemoveSessionWaitHandler(OnWaitCounted);
                client.RemoveClosedHandler(OnClosed);
                StreamingClients.Remove(deviceId);
                if (CurrentDeviceId == client.DeviceId)
                {
                    ClientStatus.SetValues(new ClientStatus(null));
                }
            }
        }

        /// <summary>
        /// クライアントはリストから消さない
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="status"></param>
        protected void Abort(string deviceId, StreamingStatuses status) 
        {
            if (StreamingClients.TryGetValue(deviceId, out var client)) 
            {
                // TODO 破棄してから
                client.Stop();
                client.RequestStopStreaming();
                client.Status = status;
                client.RemoveCounterHandler(OnPlayCounted);
                client.RemoveSessionWaitHandler(OnWaitCounted);
                client.RemoveClosedHandler(OnClosed);
                //if (CurrentDeviceId == client.DeviceId)
                //{
                //    ClientStatus.SetValues(new ClientStatus(null));
                //}
            }
        }


        /// <inheritdoc/>
        public void ChangeChannel(string deviceId, int channel)
        {
            if (StreamingClients.TryGetValue(deviceId, out var client)) 
            {
                client.RequestChangeChannel(channel);
            }
        }

        public void NotifyStatus(string deviceId)
        {
            if (StreamingClients.TryGetValue(deviceId, out var client))
            {
                ClientStatus.SetValues(client.GetClientStatus());
            }
            else
            {
                ClientStatus.SetValues(new ClientStatus(null));
            }
        }

        public bool CanChangeChannel(string deviceId)
        {
            var result = false;
            if (StreamingClients.TryGetValue(deviceId, out var client) &&
                client.Status == StreamingStatuses.Playing)
            {
                result = true;
            }
            return result;
        }

        protected abstract IStreamingClient CreateStreamingClient(string deviceId);

        protected async virtual Task<StreamingServerInfo> GetStreamingServerInfoAsync(string deviceId)
        {
            return await RequestController.GetStreamingServerAsync(deviceId);
        }

        private void OnStreamingStatusReceived(object sender, MqttMessageEventArgs<MqttJsonStreamingStatus> e)
        {
            try
            {
                var message = e.DeserializeMessage();
                if (StreamingClients.TryGetValue(message.device_id, out var client)) // 実行中の物だけ
                {
                    client.LastStreamingStatus = message;
                    if (message.streamingStarted == 1)
                    {
                        if (TimeSpan.TryParse(message.elapsed, out var elaplsedTime))
                        {
                            if (elaplsedTime.TotalSeconds < StreamingSettings.StreamingSessionWait)
                            {
                                // 待つ
                                client.Status = StreamingStatuses.WaitToPlay;
                            }
                            else
                            {
                                if (client.Status == StreamingStatuses.Request || 
                                    client.Status == StreamingStatuses.WaitToPlay) 
                                {
                                    // 再生
                                    Task.Run(async () =>
                                    {
                                        var count = 0;
                                        StreamingServerInfo serverInfo = null;
                                        do
                                        {
                                            try
                                            {
                                                serverInfo = await GetStreamingServerInfoAsync(message.device_id);
                                                if (serverInfo != null && serverInfo.hasServerInfo)
                                                {
                                                    break;
                                                }
                                            }
                                            catch (Exception) {}
                                            count++;
                                            await Task.Delay(1000);
                                        } 
                                        while (count < StreamingSettings.StreamingSessionRetryCount);

                                        if (serverInfo != null && serverInfo.hasServerInfo)
                                        {
                                            try
                                            {
                                                client.Play(serverInfo);
                                            }
                                            catch (Exception) 
                                            {
                                                // VLCプロセス起動失敗
                                                Abort(client.DeviceId, StreamingStatuses.PlayerError);
                                            }
                                        }
                                        else
                                        {
                                            // 中断
                                            Abort(client.DeviceId, StreamingStatuses.PlayingFailure);
                                        }

                                        if (message.device_id == CurrentDeviceId)
                                        {
                                            ClientStatus.SetValues(client.GetClientStatus());
                                        }
                                    });
                                }
                            }
                        }
                    }
                    if (message.device_id == CurrentDeviceId)
                    {
                        ClientStatus.SetValues(client.GetClientStatus());
                    }
                }
            }
            catch (Exception) { }
        }

        private void OnPlayCounted(object sender, CounterEventArgs args)
        {
            if (sender is IStreamingClient client && client.DeviceId == CurrentDeviceId)
            {
                ClientStatus.PlayCounter = $@"{args.Count:hh\:mm\:ss}";
            }
        }

        private void OnWaitCounted(object sender, CounterEventArgs args)
        {
            if (sender is IStreamingClient client)
            {
                lock (client) 
                {
                    // リトライ判定
                    if (client.Status == StreamingStatuses.Request &&
                        StreamingSettings.StreamingRequestWait < args.Count.TotalSeconds)
                    {
                        // リトライ
                        if (client.RequestCount <= StreamingSettings.StreamingRequestRetryCount) 
                        {
                            client.RequestStartStreaming();
                        }
                        else
                        {
                            // リトライオーバー
                            Abort(client.DeviceId, StreamingStatuses.RequestFailure);
                        }
                    }

                    // 画面描画
                    if (client.DeviceId == CurrentDeviceId)
                    {
                        ClientStatus.SetValues(client.GetClientStatus()); 
                    }
                }
            }
        }


        private void OnClosed(object sender, EventArgs e)
        {
            if (sender is IStreamingClient client && client.DeviceId == CurrentDeviceId) 
            {
                Stop(client.DeviceId);
            }
        }
    }
}
