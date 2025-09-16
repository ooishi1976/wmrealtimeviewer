using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network.Mqtt;
using uPLibrary.Networking.M2Mqtt;

namespace RealtimeViewer.WMShipView
{
    public class RtspClient : AbstractStreamingClient
    {
        public RtspClient(
            MqttController mqttController, string deviceId) :base(mqttController, deviceId) { }

        protected override string GetDistributeUrl(StreamingServerInfo serverInfo)
        {
            // e.g. RTMP->RTSP 視聴URL rtsp://176.34.37.239:23468/live/WM00003.1748310655.v7UQEpYxwn
            return StreamingRequest.MakeStreamingServerRtspUri(serverInfo);
        }
    }
}
