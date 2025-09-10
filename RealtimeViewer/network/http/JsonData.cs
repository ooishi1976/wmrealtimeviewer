using Newtonsoft.Json;
using RealtimeViewer.Network.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeViewer.Network.Http
{
    /// <summary>
    /// 
    /// curl -s -X GET 13.113.139.112/api/v1/da/live_streams/0000000003 -H x-api-key:hoge
    /// 
    /// {
    /// "status": "OK",
    /// "data": {
    /// "id": 125,
    /// "device_id": "0000000003",
    /// "streaming_server_host": "176.34.37.239",
    /// "streaming_server_port": 23468,
    /// "publish_port": 48003,
    /// "stream_name": "0000000003.1605508353.Ca4kVcIjSH.stream"
    ///   }
    /// }
    /// 
    /// </summary>
    [JsonObject]
    public class StreamingServerDataInfo
    {
        public string id { get; set; }
        public string device_id { get; set; }
        public string streaming_server_host { get; set; }
        public int streaming_server_port { get; set; }
        public int publish_port { get; set; }
        public string stream_name { get; set; }
    }

    [JsonObject]
    public class StreamingServerInfo
    {
        public string status { get; set; }
        public StreamingServerDataInfo data { get; set; }

        [JsonIgnore]
        public bool hasServerInfo;

        [JsonIgnore]
        public bool existsMpd;
    }

    [JsonObject]
    public class JsonLogin
    {
        public string id { get; set; }
        public string password { get; set; }
    }

    [JsonObject]
    public class JsonLoginResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string error { get; set; }
    }

    [JsonObject]
    public class JsonMovie
    {
        public int camera_id { get; set; }
        public string device_id { get; set; }
        public int id { get; set; }
        public string sequence { get; set; }
        public string name { get; set; }
        public DateTime ts_start { get; set; }
        public DateTime ts_end { get; set; }
        public int movie_type { get; set; }
    }

    public enum MovieType
    {
        Gsensor = 0,
        Gsensor1G, // 仮
        Request,
        Emergency, // 仮
    }

    [JsonObject]
    public class JsonMovieSearchResult
    {
        public JsonMovie[] data { get; set; }
        public string status { get; set; }
    }

    [JsonObject]
    public class JsonDevice
    {
        public string device_id { get; set; }
        public string car_id { get; set; }
        public string car_number { get; set; }
        public string use_type { get; set; }
        public int office_id { get; set; }
    }

    [JsonObject]
    public class JsonDeviceSearchResult
    {
        public JsonDevice[] data { get; set; }
        public string status { get; set; }
        public int result { get; set; }
    }

    [JsonObject]
    public class JsonOffice
    {
        public string id { get; set; }
        public string company_id { get; set; }
        public string name { get; set; }
    }

    [JsonObject]
    public class JsonOfficeSearchResult
    {
        public JsonOffice[] data { get; set; }
        public string status { get; set; }
        public int result { get; set; }
    }

    [JsonObject]
    public class JsonRemoteConfig
    {
        public int result { get; set; }
        public string status { get; set; }
        public MqttJsonRemoteConfig data { get; set; }
    }

}
