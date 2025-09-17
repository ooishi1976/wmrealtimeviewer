using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network.Mqtt;

namespace RealtimeViewer.WMShipView.Streaming
{
    public enum StreamingStatuses
    {
        /// <summary>
        /// なし
        /// </summary>
        None = 0,
        /// <summary>
        /// 配信要求
        /// </summary>
        Request = 1,
        /// <summary>
        /// 配信待ち
        /// </summary>
        WaitToPlay = 2,
        /// <summary>
        /// 再生中
        /// </summary>
        Playing = 3,
        /// <summary>
        /// 配信状況が来ない
        /// </summary>
        RequestFailure = 4,
        /// <summary>
        /// 配信URL取得失敗
        /// </summary>
        PlayingFailure = 5,
        /// <summary>
        /// VLC起動失敗
        /// </summary>
        PlayerError = 6,
    }

    public interface IStreamingClient
    {
        StreamingStatuses Status { get; set; }

        MqttJsonStreamingStatus LastStreamingStatus { get; set; }

        string DeviceId { get; }

        int Channel { get; }

        int SessionRetryCount { get; }

        int RequestCount { get; }

        void RequestStartStreaming();

        void RequestStopStreaming();

        void RequestChangeChannel(int channel);

        void Play(StreamingServerInfo serverInfo);

        void Stop();

        void Stop(StreamingStatuses status);

        void SetForground();

        ClientStatus GetClientStatus();

        void AddCounterHandler(CounterEventHandler handler);

        void RemoveCounterHandler(CounterEventHandler handler);

        void AddClosedHandler(EventHandler handler);

        void RemoveClosedHandler(EventHandler handler);

        void AddSessionWaitHandler(CounterEventHandler handler);

        void RemoveSessionWaitHandler(CounterEventHandler handler);
    }
}
