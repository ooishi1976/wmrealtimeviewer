using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using RealtimeViewer.Network;
using RealtimeViewer.Network.Http;

namespace RealtimeViewer.WMShipView.Streaming
{
    public class RtspController : AbstractStreamingController
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mqttController"></param>
        /// <param name="requestController"></param>
        /// <param name="dispatcher"></param>
        /// <param name="requestWaitTime"></param>
        /// <param name="sessionWaitTime"></param>
        public RtspController(
            MqttController mqttController,
            RequestSequence requestController,
            Dispatcher dispatcher,
            IStreamingSettings streamingSettings) : base(mqttController, requestController, dispatcher, streamingSettings) { }

        /// <inheritdoc/>
        protected override IStreamingClient CreateStreamingClient(string deviceId)
        {
            return new RtspClient(MqttController, deviceId);
        }
    }
}
