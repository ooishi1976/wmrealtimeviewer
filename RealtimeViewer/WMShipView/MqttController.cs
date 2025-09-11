using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using Newtonsoft.Json;
using RealtimeViewer.Model;
using RealtimeViewer.Network;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network.Mqtt;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace RealtimeViewer.WMShipView
{
    public delegate void MqttMessageHandler<T>(object sender, MqttMessageEventArgs<T> e);

    public class MqttMessageEventArgs<T> : EventArgs
    {
        public TopicLabel Topic { get; private set; } = TopicLabel.TopicNone;

        public string Message { get; private set; } = string.Empty;

        public MqttMessageEventArgs(TopicLabel topic, string message)
        {
            Topic = topic;
            Message = message;
        }

        public T DeserializeMessage()
        {
            return JsonConvert.DeserializeObject<T>(Message);
        }
    }

    public class MqttController
    {
        /// <summary>
        /// MQTTクライアント
        /// </summary>
        private MqttClient MqttClient { get; set; }

        /// <summary>
        /// 配信イベント
        /// </summary>
        protected event MqttClient.MqttMsgPublishedEventHandler Published;

        /// <summary>
        /// 受信イベント
        /// </summary>
        protected event MqttClient.MqttMsgPublishEventHandler PublishReceived;

        /// <summary>
        /// 切断イベント
        /// </summary>
        protected event MqttClient.ConnectionClosedEventHandler Closed;

        /// <summary>
        /// 位置情報
        /// </summary>
        protected event MqttMessageHandler<MqttJsonLocation> LocationReceived;

        /// <summary>
        /// エラー
        /// </summary>
        protected event MqttMessageHandler<MqttJsonError> ErrorReceived;

        /// <summary>
        /// ACC ON/OFF
        /// </summary>
        protected event MqttMessageHandler<MqttJsonEventAccOn> AccOnReceived;

        /// <summary>
        /// プリポスト
        /// </summary>
        protected event MqttMessageHandler<MqttJsonPrepostEvent> PrepostReceived;
        
        /// <summary>
        /// 位置情報ハンドラリスト
        /// </summary>
        private readonly List<MqttMessageHandler<MqttJsonLocation>> loactionHandlers = new List<MqttMessageHandler<MqttJsonLocation>>();

        /// <summary>
        /// エラーハンドラリスト
        /// </summary>
        private readonly List<MqttMessageHandler<MqttJsonError>> errorHandlers = new List<MqttMessageHandler<MqttJsonError>>();

        /// <summary>
        /// AccOnハンドラリスト
        /// </summary>
        private readonly List<MqttMessageHandler<MqttJsonEventAccOn>> accOnHandlers = new List<MqttMessageHandler<MqttJsonEventAccOn>>();

        /// <summary>
        /// PrePostハンドラリスト
        /// </summary>
        private readonly List<MqttMessageHandler<MqttJsonPrepostEvent>> prepostHandlers = new List<MqttMessageHandler<MqttJsonPrepostEvent>>();

        /// <summary>
        /// 接続済みか
        /// </summary>
        public bool IsConnected
        {
            get
            {
                var result = false;
                if (MqttClient != null) 
                {
                    result = MqttClient.IsConnected;
                }
                return result;
            }
        }

        /// <summary>
        /// topic用正規表現
        /// </summary>
        private List<TopicRegex> TopicRegexes { get; set; } = new List<TopicRegex>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MqttController()
        {
            TopicRegexes = new List<TopicRegex>()
            {
                new TopicRegex() { Regex = new Regex(@"car/streaming/(?<device_id>\w+)/status"), Label = TopicLabel.TopicStreamingStatus, },
                new TopicRegex() { Regex = new Regex(@"car/status/(?<device_id>\w+)"), Label = TopicLabel.TopicLocation, },
                new TopicRegex() { Regex = new Regex(@"car/error/(?<device_id>\w+)"), Label = TopicLabel.TopicErrorStatus, },
                new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/engine"), Label = TopicLabel.TopicEventAccOn, },
                new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/driver"), Label = TopicLabel.TopicEventDriver, },
                new TopicRegex() { Regex = new Regex(@"car/event/(?<device_id>\w+)/prepost"), Label = TopicLabel.TopicEventPrepost, }
            };
        }

        public void ConnectMQTTServer(ServerInfo phygicalServerInfo, string deviceId = null)
        {
            // 繋がっていたら、一旦閉じる
            if (MqttClient != null && MqttClient.IsConnected)
            {
                DisposeMQTTServer();
            }

            //  MQTTサーバ設定
            var topic_list = new List<string>();
            var qos_list = new List<byte>();
            var serverInfo = phygicalServerInfo;

            MqttClient = new MqttClient(serverInfo.MqttAddr);
            MqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived; ;
            MqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
            MqttClient.ConnectionClosed += MqttClient_ConnectionClosed;
            MqttClient.Connect(GetMqttClientId());     //  MAC address + PIDとかにする

            if (deviceId == null)
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
                topic_list.Add($"car/status/{deviceId}/#");
                qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                topic_list.Add($"car/error/{deviceId}/#");
                qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                topic_list.Add($"car/event/{deviceId}/#");
                qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
                topic_list.Add($"car/streaming/{deviceId}/status");
                qos_list.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
            }
            MqttClient.Subscribe(topic_list.ToArray(), qos_list.ToArray());
        }

        public void DisposeMQTTServer()
        {
            if (MqttClient != null)
            {
                // MQTTクライアントからイベントハンドラを取り除いておく。
                MqttClient.ConnectionClosed -= MqttClient_ConnectionClosed;

                if (MqttClient.IsConnected)
                {
                    MqttClient.Disconnect();
                }
            }
        }

        public void AddReceivedHandler<T>(MqttMessageHandler<T> handler)
        {
            if (handler is MqttMessageHandler<MqttJsonLocation> locationHandler &&
                !loactionHandlers.Contains(locationHandler))
            {
                LocationReceived += locationHandler;
            }
            else if (handler is MqttMessageHandler<MqttJsonError> errorHandler &&
                     !errorHandlers.Contains(errorHandler))
            {
                ErrorReceived += errorHandler;
            }
            else if (handler is MqttMessageHandler<MqttJsonEventAccOn> accOnHandler &&
                     !accOnHandlers.Contains(accOnHandler))
            {
                AccOnReceived += accOnHandler;
            }
            else if (handler is MqttMessageHandler<MqttJsonPrepostEvent> prepostHandler &&
                     !prepostHandlers.Contains(prepostHandler))
            {
                PrepostReceived += prepostHandler;
            }
        }

        public void RemoveReceivedHandler<T>(MqttMessageHandler<T> handler)
        {
            if (handler is MqttMessageHandler<MqttJsonLocation> locationHandler &&
                loactionHandlers.Contains(locationHandler))
            {
                LocationReceived -= locationHandler;
            }
            else if (handler is MqttMessageHandler<MqttJsonError> errorHandler &&
                     errorHandlers.Contains(errorHandler))
            {
                ErrorReceived -= errorHandler;
            }
            else if (handler is MqttMessageHandler<MqttJsonEventAccOn> accOnHandler &&
                     accOnHandlers.Contains(accOnHandler))
            {
                AccOnReceived -= accOnHandler;
            }
            else if (handler is MqttMessageHandler<MqttJsonPrepostEvent> prepostHandler &&
                     prepostHandlers.Contains(prepostHandler))
            {
                PrepostReceived -= prepostHandler;
            }
        }

        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            Closed?.Invoke(sender, e);
        }

        private void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Published?.Invoke(sender, e);
        }

        /// <summary>
        /// MQTT受信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //  MQTT取得
            var msg = Encoding.UTF8.GetString(e.Message);
            //  トピック検索 (error/status/event...)
            var label = TopicLabel.TopicNone;
            foreach (var pattern in TopicRegexes)
            {
                var match = pattern.Regex.Match(e.Topic);
                if (match.Success)
                {
                    label = pattern.Label;
                    switch (label)
                    {
                        case TopicLabel.TopicNone:
                            break;

                        case TopicLabel.TopicErrorStatus:
                            ErrorReceived?.Invoke(this, new MqttMessageEventArgs<MqttJsonError>(label, msg));
                            break;

                        case TopicLabel.TopicEventAccOn:
                            AccOnReceived?.Invoke(this, new MqttMessageEventArgs<MqttJsonEventAccOn>(label, msg));
                            break;

                        case TopicLabel.TopicLocation:
                            LocationReceived?.Invoke(this, new MqttMessageEventArgs<MqttJsonLocation>(label, msg));
                            break;

                        case TopicLabel.TopicEventDriver:
                            break;

                        case TopicLabel.TopicEventPrepost:
                            PrepostReceived?.Invoke(this, new MqttMessageEventArgs<MqttJsonPrepostEvent>(label, msg));
                            break;

                        case TopicLabel.TopicStreamingStatus:
                            break;
                    }
                }
            }
        }

        private List<PhysicalAddress> GetPhysicalAddress()
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

        private string GetMqttClientId()
        {
            //  ユニークなIDをmacAddress[0]+ProcessIDで作成
            var macAddress = GetPhysicalAddress();
            var tmpAddress = macAddress[0].ToString();
            var hProcess = Process.GetCurrentProcess();

            var result = "ex:" + tmpAddress + hProcess.Id;

            hProcess.Close();
            hProcess.Dispose();

            return result;
        }
    }
}
