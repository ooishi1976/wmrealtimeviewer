using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;

namespace RealtimeViewer.Network.Http
{

    public class StreamingRequest
    {
        // ユーザーへの状況表示について更新頻度を高めたいため、タイムアウトは短めにしておく。
        // 通常、数百ミリ秒程度でレスポンスは返ってくる。
        const double REQUEST_TIMEOUT_SEC = 15.0;

        public StreamingRequest()
        {
        }

        public static string MakeStreamingServerUri(StreamingServerInfo ssi)
        {
            return $"http://{ssi.data.streaming_server_host}:{ssi.data.streaming_server_port}/live/{ssi.data.stream_name}/manifest.mpd";
        }

        /// <summary>
        /// RTSP用
        /// </summary>
        /// <param name="ssi"></param>
        /// <returns></returns>
        public static string MakeStreamingServerRtspUri(StreamingServerInfo ssi)
        {
            return $"rtsp://{ssi.data.streaming_server_host}:{ssi.data.streaming_server_port}/live/{ssi.data.stream_name}";
        }

        /// <summary>
        /// リアルタイムビュー用の配信サーバー情報を取得する
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverAddr">接続先サーバーアドレス</param>
        /// <param name="device_id">DeviceId</param>
        /// <returns>StreamingInfo. If timeouted, return null.</returns>
        public static async Task<StreamingServerInfo> GetStreamingInfo(HttpClient client, string serverAddr, string device_id)
        {
            if (string.IsNullOrEmpty(serverAddr) || string.IsNullOrEmpty(device_id))
            {
                return null;
            }

            Debug.WriteLine("start GetStreamingInfoSync");

            StreamingServerInfo si = null;
            const string api = @"api/v1/da/live_streams";
            {
                var uri = new Uri($"http://{serverAddr}/{api}/{device_id}");
                Debug.WriteLine($"request url: {uri}");
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Debug.WriteLine("status OK");
                            // status == 200
                            var obj = JsonConvert.DeserializeObject<StreamingServerInfo>(await response.Content.ReadAsStringAsync());
                            if (obj != null && obj.data != null)
                            {
                                si = obj;
                                si.hasServerInfo = true;
                                si.existsMpd = false;
                            }
                        }
                    }
                }
            }

            if (si == null)
            {
                si = new StreamingServerInfo();
                si.hasServerInfo = false;
                si.existsMpd = false;
            }

            Debug.WriteLine("end GetStreamingInfoSync");
            return si;
        }

        /// <summary>
        /// リアルタイムビュー用の配信サーバー情報を取得する
        /// </summary>
        /// <param name="serverAddr">接続先サーバーアドレス</param>
        /// <param name="device_id">DeviceId</param>
        /// <returns>StreamingInfo. If timeouted, return null.</returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<StreamingServerInfo> GetStreamingInfo(string serverAddr, string device_id)
        {
            StreamingServerInfo result = null;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT_SEC);
                result = await GetStreamingInfo(client, serverAddr, device_id);
            }
            return result;
        }

        /// <summary>
        /// リアルタイム配信の開始通知から、数十秒の時間経過ののち、.mpdファイルが作られる。
        /// .mpd ファイルが存在しない場合、FFPLAY の起動に失敗してしまう。
        /// 起動の失敗と、ユーザー操作による終了が区別できないため、事前に .mpd ファイルの存在を確認するよう、本メソッドを使用されたい。
        /// </summary>
        /// <param name="ssi"></param>
        /// <returns>true:存在する, false:存在しない</returns>
        public static async Task<StreamingServerInfo> ExistsStreamingMpdFile(HttpClient client, StreamingServerInfo ssi)
        {
            if (!ssi.hasServerInfo)
            {
                return ssi;
            }

            var uri = new Uri(MakeStreamingServerUri(ssi));
            Debug.WriteLine($"request url: {uri}");
            using (var response = await client.GetAsync(uri))
            {
                Debug.WriteLine($"ExistsStreamingMpdFile http status: {response.StatusCode}, {response}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ssi.existsMpd = true;
                }
            }

            if (ssi.existsMpd)
            {
                Debug.WriteLine("Exists .mpd file.");
            }
            else
            {
                Debug.WriteLine("*Not* exists .mpd file.");
            }

            return ssi;
        }

        /// <summary>
        /// リアルタイム配信の開始通知から、数十秒の時間経過ののち、.mpdファイルが作られる。
        /// .mpd ファイルが存在しない場合、FFPLAY の起動に失敗してしまう。
        /// 起動の失敗と、ユーザー操作による終了が区別できないため、事前に .mpd ファイルの存在を確認するよう、本メソッドを使用されたい。
        /// </summary>
        /// <param name="ssi"></param>
        /// <returns>true:存在する, false:存在しない</returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<StreamingServerInfo> ExistsStreamingMpdFile(StreamingServerInfo ssi)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT_SEC);
                ssi = await ExistsStreamingMpdFile(client, ssi);
            }
            return ssi;
        }
    }
}

