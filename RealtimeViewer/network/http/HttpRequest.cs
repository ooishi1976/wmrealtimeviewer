using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RealtimeViewer.Network.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeViewer.Network.Http
{
    class HttpRequest
    {
        const string TIMESTAMP_FORMAT = "yyyyMMddHHmmss";

        /// <summary>
        /// ログイントークンの取得
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverInfo">接続先サーバー情報</param>
        /// <param name="clientTimeout">タイムアウト値</param>
        /// <returns></returns>
        public static async Task<JsonLoginResponse> GetLoginToken(HttpClient client, ServerInfo serverInfo)
        {
            Debug.WriteLine("start GetLoginResponse");

            JsonLoginResponse result = null;
            var loginObj = new JsonLogin()
            {
                id = serverInfo.AccessId,
                password = serverInfo.AccessPassword
            };
            var json = JsonConvert.SerializeObject(loginObj);
            System.Net.Http.StringContent content = new System.Net.Http.StringContent(json, Encoding.UTF8, @"application/json");

            const string api = @"api/v1/login";
            try
            {
                var uri = new Uri($"http://{serverInfo.HttpAddr}/{api}");
                Debug.WriteLine($"request url: {uri}");
                using (var response = await client.PostAsync(uri, content))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Debug.WriteLine("status OK");
                        // status == 200
                        result = JsonConvert.DeserializeObject<JsonLoginResponse>(await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: GetLoginToken {ex}, {ex.InnerException}, {ex.StackTrace}");
            }

            if (result == null)
            {
                result = new JsonLoginResponse();
                result.error = "Unexpected Error";
            }

            Debug.WriteLine("end GetLoginResponse");
            return result;
        }

        /// <summary>
        /// ログイントークンの取得
        /// </summary>
        /// <param name="serverInfo">接続先サーバー情報</param>
        /// <param name="clientTimeout">タイムアウト値</param>
        /// <returns></returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<JsonLoginResponse> GetLoginToken(ServerInfo serverInfo, int clientTimeout)
        {
            var result = new JsonLoginResponse();
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                result = await GetLoginToken(client, serverInfo);
            }
            return result;
        }

        /// <summary>
        /// イベント映像の情報を得る。
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverAddr">接続先アドレス</param>
        /// <param name="accessToken">認証トークン</param>
        /// <param name="deviceId">DeviceId</param>
        /// <param name="startDate">検索開始</param>
        /// <param name="endDate">検索終了</param>
        /// <returns></returns>
        public static async Task<JsonMovieSearchResult> SearchMovies(
            HttpClient client, string serverAddr, string accessToken, string deviceId, DateTime startDate, DateTime endDate)
        {
            Debug.WriteLine("start SearchMovies");

            JsonMovieSearchResult seachResult = null;
            string api = $"api/v1/ua/devices/{deviceId}/movie";
            HttpStatusCode statusCode = HttpStatusCode.OK;
            try
            {
                var sd = startDate.ToString(TIMESTAMP_FORMAT);
                var ed = endDate.ToString(TIMESTAMP_FORMAT);
                var uri = new Uri($"http://{serverAddr}/{api}/?device_id={deviceId}&ts_start={sd}&ts_end={ed}");
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Debug.WriteLine("status OK");
                            // status == 200
                            seachResult = JsonConvert.DeserializeObject<JsonMovieSearchResult>(await response.Content.ReadAsStringAsync(),
                                new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                        }
                        statusCode = response.StatusCode;
                    }
                }
            }
            catch (TaskCanceledException tce)
            {
                // timeoutでtaskキャンセルが走った場合
                // (おそらく、抽出件数が多いため、時間内にresponseが返されなかった)
                Debug.WriteLine($"@@@ Exception: SearchMovies {tce}, {tce.InnerException}, {tce.StackTrace}");
                throw tce;
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine($"@@@ Exception: SearchMovies {hre}, {hre.InnerException}, {hre.StackTrace}");
                throw hre;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: SearchMovies {ex}, {ex.InnerException}, {ex.StackTrace}");
            }

            if (seachResult == null)
            {
                seachResult = new JsonMovieSearchResult();
                if (HttpStatusCode.InternalServerError <= statusCode)
                {
                    throw new TaskCanceledException(statusCode.ToString());
                }
            }

            Debug.WriteLine("end SearchMovies");
            return seachResult;
        }

        /// <summary>
        /// イベント映像の情報を得る。
        /// </summary>
        /// <param name="serverAddr">接続先アドレス</param>
        /// <param name="accessToken">認証トークン</param>
        /// <param name="deviceId">DeviceId</param>
        /// <param name="startDate">検索開始</param>
        /// <param name="endDate">検索終了</param>
        /// <returns></returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<JsonMovieSearchResult> SearchMovies(
            string serverAddr, string accessToken, string deviceId, DateTime startDate, DateTime endDate,
            int clientTimeout)
        {
            JsonMovieSearchResult result = null;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                result = await SearchMovies(client, serverAddr, accessToken, deviceId, startDate, endDate);
            }
            return result;
        }

        /// <summary>
        /// イベント映像をダウンロードする。
        /// ダウンロード先はテンポラリファイルで、そのファイル名を返却する。
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverAddr">接続先アドレス</param>
        /// <param name="accessToken">認証トークン</param>
        /// <param name="movieId">映像ID</param>
        /// <returns>ダウンロードファイルのファイル名。Emptyの場合はダウンロード失敗である。</returns>
        public static async Task<string> DownloadMovie(HttpClient client, string serverAddr, string accessToken, int movieId)
        {
            string filepath = string.Empty;
            try
            {
                var uri = new Uri($"http://{serverAddr}/api/v1/ua/event_movies/{movieId}/zip");
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            filepath = Path.GetTempFileName();
                            using (var content = response.Content)
                            using (var stream = await content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                try
                                {
                                    Task task = stream.CopyToAsync(fileStream);
                                    await task;
                                    Debug.WriteLine($"Succeeded download: {filepath}");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"@@@ Failed Download: {ex.ToString()}, {ex.Message}");
                                    filepath = string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: DownloadMovie {ex}, {ex.InnerException}, {ex.StackTrace}");
            }
            return filepath;
        }

        /// <summary>
        /// イベント映像をダウンロードする。
        /// ダウンロード先はテンポラリファイルで、そのファイル名を返却する。
        /// </summary>
        /// <param name="serverAddr"></param>
        /// <param name="accessToken"></param>
        /// <param name="movieId"></param>
        /// <returns>ダウンロードファイルのファイル名。Emptyの場合はダウンロード失敗である。</returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<string> DownloadMovie(
            string serverAddr, string accessToken, int movieId, int clientTimeout)
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                result = await DownloadMovie(client, serverAddr, accessToken, movieId);
            }
            return result;
        }

        /// <summary>
        /// 全車載器情報を取得する
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverInfo">接続先サーバ情報</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns></returns>
        public static async Task<JsonDeviceSearchResult> GetAllDevices(
            HttpClient client, ServerInfo serverInfo, string accessToken)
        {
            JsonDeviceSearchResult result = null;
            string api = $"api/v1/ua/devices";
            try
            {
                var uri = new Uri($"http://{serverInfo.HttpAddr}/{api}/");
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Debug.WriteLine("status OK");
                            // status == 200
                            result = JsonConvert.DeserializeObject<JsonDeviceSearchResult>(await response.Content.ReadAsStringAsync(),
                                new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: GetAllDevices {ex}, {ex.InnerException}, {ex.StackTrace}");
            }

            if (result == null)
            {
                result = new JsonDeviceSearchResult();
            }

            return result;
        }

        /// <summary>
        /// 全車載器情報を取得する
        /// </summary>
        /// <param name="serverInfo">接続先サーバ情報</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns></returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<JsonDeviceSearchResult> GetAllDevices(
            ServerInfo serverInfo, string accessToken, int clientTimeout)
        {
            JsonDeviceSearchResult result = null;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                result = await GetAllDevices(client, serverInfo, accessToken);
            }
            return result;
        }

        /// <summary>
        /// 全車載器情報を取得する
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverInfo">接続先サーバ情報</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="deviceId">車載器ID</param>
        /// <returns></returns>
        public static async Task<JsonDeviceSearchResult> GetDevice(
            HttpClient client, ServerInfo serverInfo, string accessToken, string deviceId)
        {
            JsonDeviceSearchResult result = null;
            string api = $"api/v1/ua/devices?device_id={deviceId}";
            try
            {
                var uri = new Uri($"http://{serverInfo.HttpAddr}/{api}");
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Debug.WriteLine("status OK");
                            // status == 200
                            string content = response.Content.ReadAsStringAsync().Result;
                            result = JsonConvert.DeserializeObject<JsonDeviceSearchResult>(content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: GetDevice {ex}, {ex.InnerException}, {ex.StackTrace}");
            }

            if (result == null)
            {
                result = new JsonDeviceSearchResult();
            }

            return result;
        }

        /// <summary>
        /// 全車載器情報を取得する
        /// </summary>
        /// <param name="serverInfo">接続先サーバ情報</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="deviceId">車載器ID</param>
        /// <returns></returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<JsonDeviceSearchResult> GetDevice(
            ServerInfo serverInfo, string accessToken, string deviceId, int clientTimeout)
        {
            JsonDeviceSearchResult result = null;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                result = await GetDevice(client, serverInfo, accessToken, deviceId);
            }
            return result;
        }

        /// <summary>
        /// 全事業所情報を取得する。
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverAddr">接続先サーバ情報</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns></returns>
        public static async Task<JsonOfficeSearchResult> GetAllOffices(
            HttpClient client, string serverAddr, string accessToken)
        {
            JsonOfficeSearchResult result = null;
            string api = $"api/v1/ua/offices";
            try
            {
                var uri = new Uri($"http://{serverAddr}/{api}/");
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Debug.WriteLine("status OK");
                            // status == 200
                            result = JsonConvert.DeserializeObject<JsonOfficeSearchResult>(await response.Content.ReadAsStringAsync(),
                                new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                        }
                        else
                        {
                            throw new WebException(response.StatusCode.ToString());
                        }
                    }
                }
            }
            catch (WebException httpException)
            {
                throw httpException;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: GetAllOffices {ex}, {ex.InnerException}, {ex.StackTrace}");
            }

            if (result == null)
            {
                result = new JsonOfficeSearchResult();
            }

            return result;
        }

        /// <summary>
        /// 全事業所情報を取得する。
        /// </summary>
        /// <param name="serverAddr">接続先サーバ情報</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns></returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<JsonOfficeSearchResult> GetAllOffices(
            string serverAddr, string accessToken, int clientTimeout)
        {
            JsonOfficeSearchResult result = null;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                result = await GetAllOffices(client, serverAddr, accessToken);
            }
            return result;
        }

        /// <summary>
        /// 遠隔設定を取得する。
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverAddr">接続先サーバ情報</param>
        /// <param name="deviceId">車載器情報</param>
        /// <returns></returns>
        public static async Task<JsonRemoteConfig> GetRemoteConfig(
            HttpClient client, string serverAddr, string deviceId)
        {
            JsonRemoteConfig result = null;
            string api = $"api/v1/da/devices_settings?device_id={deviceId}";

            try
            {
                var uri = new Uri($"http://{serverAddr}/{api}");
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Debug.WriteLine("status OK");
                            // status == 200
                            result = JsonConvert.DeserializeObject<JsonRemoteConfig>(await response.Content.ReadAsStringAsync(),
                                new JsonSerializerSettings
                                {
                                    Converters = { new IsoDateTimeConverter() { DateTimeFormat = @"yyyyMMddHHmmss", }, },
                                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                                });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: GetRemoteConfig {ex}, {ex.InnerException}, {ex.StackTrace}");
            }

            return result;
        }

        /// <summary>
        /// 遠隔設定を取得する。
        /// </summary>
        /// <param name="serverAddr">接続先サーバ情報</param>
        /// <param name="deviceId">車載器情報</param>
        /// <returns></returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<JsonRemoteConfig> GetRemoteConfig(
            string serverAddr, string deviceId, int clientTimeout)
        {
            JsonRemoteConfig result = null;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(clientTimeout);
                result = await GetRemoteConfig(client, serverAddr, deviceId);
            }
            return result;
        }

        /// <summary>
        /// 圧縮MTXファイルをテンポラリディレクトリにダウンロードする。
        /// </summary>
        /// <param name="client">HTTPクライアント</param>
        /// <param name="serverAddr">接続先サーバ情報</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="deviceId">車載器ID</param>
        /// <param name="begin">検索日時Begin</param>
        /// <param name="end">検索日時End</param>
        /// <param name="end">クライアントタイムアウト</param>
        /// <param name="end">データ種別(1: MTX(デフォルト) 2: MU</param>
        /// <returns>ダウンロードファイルのパス</returns>
        public static async Task<string> DownloadMuZip(
            HttpClient client, string serverAddr, string accessToken, string deviceId, DateTime begin, DateTime end, int dataType = 1)
        {
            // 1:mtx, 2:mu
            int data_type = dataType;

            string filepath = string.Empty;

            try
            {
                var dtFormat = @"yyyyMMddHHmmss";
                var tsStart = begin.ToString(dtFormat);
                var tsEnd = end.ToString(dtFormat);

                var uri = new Uri($"http://{serverAddr}/api/v1/ua/mtx/zip?device_id={deviceId}&ts_start={tsStart}&ts_end={tsEnd}&data_type={data_type}");
                Debug.WriteLine($"request {uri}");
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        Debug.WriteLine($"http status {response.StatusCode}");
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            filepath = Path.GetTempFileName();
                            using (var content = response.Content)
                            using (var stream = await content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                try
                                {
                                    Task task = stream.CopyToAsync(fileStream);
                                    await task;
                                    Debug.WriteLine($"Succeeded mu download: {filepath}");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"@@@ Failed mu Download: {ex}, {ex.StackTrace}");
                                    filepath = string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: DownloadMovie {ex}, {ex.InnerException}, {ex.StackTrace}");
            }

            return filepath;
        }

        /// <summary>
        /// 圧縮MTXファイルをテンポラリディレクトリにダウンロードする。
        /// </summary>
        /// <param name="serverAddr">接続先サーバ情報</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="deviceId">車載器ID</param>
        /// <param name="begin">検索日時Begin</param>
        /// <param name="end">検索日時End</param>
        /// <param name="end">クライアントタイムアウト</param>
        /// <param name="end">データ種別(1: MTX(デフォルト) 2: MU</param>
        /// <returns>ダウンロードファイルのパス</returns>
        [Obsolete("このメソッドの使用は非推奨です")]
        public static async Task<string> DownloadMuZip(
            string serverAddr, string accessToken, string deviceId, DateTime begin, DateTime end, int clientTimeout, int dataType = 1)
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(clientTimeout);
                result = await DownloadMuZip(client, serverAddr, accessToken, deviceId, begin, end, dataType);
            }
            return result;
        }
    }
}
