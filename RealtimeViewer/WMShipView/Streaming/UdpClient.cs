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
    public class UdpClient : AbstractStreamingClient
    {
        public UdpClient(
            MqttController mqttController, string deviceId) :base(mqttController, deviceId) { }

        protected override string GetDistributeUrl(StreamingServerInfo serverInfo)
        {
            // e.g. http://176.34.37.239:23468/live/0000000003.1605168010.RKhmUjEtAd.stream/manifest.mpd
            return StreamingRequest.MakeStreamingServerUri(serverInfo);
        }
    }
}
