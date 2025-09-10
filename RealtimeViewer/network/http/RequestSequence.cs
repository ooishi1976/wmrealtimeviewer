using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RealtimeViewer.Model;
using RealtimeViewer.Setting;
using System.Windows.Threading;
using System.Net.Http;

namespace RealtimeViewer.Network.Http
{
    /// <summary>
    /// リクエスト映像リスト更新完了時のデリゲート
    /// </summary>
    public delegate void UpdateEventListHandler();

    /// <summary>
    /// 各種REST APIを実行するクラス
    /// HTTP通信自体はHttpRequestクラスが行う。
    /// APIを呼び出すメソッドのうち、どれかを呼び出すとアクセストークンを内部に保持する。
    /// 以降、トークンは使いまわさされる。
    /// TODO: 有効期限切れやログイン出来ない場合の処理が無く、ログイン可能の前提で作られている。
    /// </summary>
    public partial class RequestSequence
    {
//        public const int SEARCH_MOVIE_DAYS = 60;
        public OperationServerInfo ServerInfo { get; set; }

        /// <summary>
        /// イベント情報
        /// </summary>
        public SortableBindingList<EventInfo> Events { get; set; }

        /// <summary>
        /// イベントリクエスト情報
        /// </summary>
        public SortableBindingList<EventInfo> Requests { get; set; }

        /// <summary>
        /// 事業所情報(表示用)
        /// </summary>
        public BindingList<OfficeInfo> Offices { get; set; }

        /// <summary>
        /// 全車両情報(全車載器情報)
        /// </summary>
        public Dictionary<string, CarInfo> AllCars;

        /// <summary>
        /// 事業所毎の運行車両情報(車載器情報)
        /// 保持しているオブジェクトは全車両情報と同じオブジェクトのため、
        /// オブジェクトの変更はどちらにも影響を与える。
        /// </summary>
        public Dictionary<int, Dictionary<string, CarInfo>> GroupCars;

        /// <summary>
        /// 事業所位置情報
        /// </summary>
        public Dictionary<int, (int latitude, int longitude)> OfficeLocations { get; set; }

        /// <summary>
        /// ログイントークン
        /// </summary>
        private JsonLoginResponse loginResponse = null;

        /// <summary>
        /// 情報取得時の内部保持用のイベントリスト
        /// </summary>
        private List<EventInfo> eventInfoList = new List<EventInfo>();

        /// <summary>
        /// 情報取得時の内部保持用のリクエストリスト
        /// </summary>
        private List<EventInfo> requestEventInfoList = new List<EventInfo>();

        private SettingIni localSettings;

        /// <summary>
        /// HTTPクライアント
        /// </summary>
        private HttpClient httpClient;

        /// <summary>
        /// リクエスト映像リストの更新完了時イベント
        /// </summary>
        private event UpdateEventListHandler requestUpdateCompleted;

        /// <summary>
        /// 車載器情報比較用のコンパレータ
        /// </summary>
        private class OrderedDeviceList : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                var xx = (DeviceInfo)x;
                var yy = (DeviceInfo)y;
                if (xx.OfficeId < yy.OfficeId)
                {
                    return -1;
                }
                else if (xx.OfficeId > yy.OfficeId)
                {
                    return 1;
                }

                int ret = 0;
                try
                {
                    int CarIdX = int.Parse(xx.CarId);
                    int CarIdY = int.Parse(yy.CarId);
                    if (CarIdX < CarIdY)
                    {
                        ret = -1;
                    }
                    else if (CarIdX > CarIdY)
                    {
                        ret = 1;
                    }
                }
                catch
                {
                    ret = xx.CarId.CompareTo(yy.CarId);
                }

                return ret;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RequestSequence(SettingIni localSettings, HttpClient httpClient, OperationServerInfo serverInfo)
        {
            this.httpClient = httpClient;
            ServerInfo = serverInfo;
            Events = new SortableBindingList<EventInfo>();
            Requests = new SortableBindingList<EventInfo>();
            Requests.RaiseListChangedEvents = true;

            AllCars = new Dictionary<string, CarInfo>();
            GroupCars = new Dictionary<int, Dictionary<string, CarInfo>>();
            this.localSettings = localSettings;
        }

        /// <summary>
        /// リクエスト映像リスト更新完了イベントハンドラ追加
        /// </summary>
        /// <param name="handler"></param>
        public void AddHandlerRequestsUpdateCompleted(UpdateEventListHandler handler)
        {
            requestUpdateCompleted += handler;
        }

        /// <summary>
        /// リクエスト映像リスト更新完了イベントハンドラ削除
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveHandlerRequestsUpdateCompleted(UpdateEventListHandler handler)
        {
            requestUpdateCompleted -= handler;
        }

        /// <summary>
        /// 事業所一覧の取得
        /// </summary>
        /// <param name="excludeOfficeList">一覧から除外する事業所IDのリスト</param>
        /// <returns>事業所一覧</returns>
        public async Task GetOfficeList(List<int> excludeOfficeList)
        {
            var result = new JsonDeviceSearchResult();
            ServerInfo server = ServerInfo.GetPhygicalServerInfo();
            bool hasAccessToken = false;
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                var login = await HttpRequest.GetLoginToken(httpClient, server);
                if (!string.IsNullOrEmpty(login.access_token))
                {
                    loginResponse = login;
                    hasAccessToken = true;
                    Debug.WriteLine("Succeeded Login.");
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                Debug.WriteLine(@"======== request GetAllOffices");
                Offices = new BindingList<OfficeInfo>();
                
                var officeResult = await HttpRequest.GetAllOffices(httpClient, server.HttpAddr, loginResponse.access_token);
                if (officeResult != null && officeResult.data != null)
                {

                    foreach (var jo in officeResult.data)
                    {
                        if (int.TryParse(jo.id, out int officeId) && int.TryParse(jo.company_id, out int companyId)
                            && ServerInfo.IsDisplayOffice(officeId))
                        {
                            var office = new OfficeInfo()
                            {
                                Id = officeId,
                                CompanyId = companyId,
                                Name = jo.name,
                            };

                            office.Visible = !excludeOfficeList.Contains(officeId);

                            if (OfficeLocations.TryGetValue(office.Id, out (int x, int y) value))
                            {
                                office.Location = value;
                            }
                            else
                            {
                                office.Location = null;
                            }
                            Debug.WriteLine($"-- office name: {office.Name}");
                            
                            if (!ServerInfo.ForbiddunOfficeIds.Contains(officeId)) 
                            {
                                Offices.Add(office);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 車載器情報を取得する
        /// プロパティ Devicesの車載器リストを更新する。
        /// 車載器情報に加えて事業所情報も取得する。(けど、使ってない？読み捨て？)
        /// 条件によってはイベント情報の取得も行う。(イベント情報の取得は60日分)
        /// </summary>
        /// <param name="targetOfficeId">対象事業所ID</param>
        /// <param name="needEventUpdate">イベント情報を更新するか</param>
        /// <param name="evprog">プログレスバー更新用のデリゲートメソッド</param>
        /// <returns></returns>
        public async Task GetDeviceList(
            int targetOfficeId, bool needEventUpdate, CancellationToken cancelToken, UpdateEventsProgress evprog = null)
        {
            var result = new JsonDeviceSearchResult();
            bool hasAccessToken = false;
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                OperateCancellation(cancelToken);
                var login = await HttpRequest.GetLoginToken(httpClient, serverInfo);
                if (!string.IsNullOrEmpty(login.access_token))
                {
                    loginResponse = login;
                    hasAccessToken = true;
                    Debug.WriteLine("Succeeded Login.");
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                OperateCancellation(cancelToken);
                result = await HttpRequest.GetAllDevices(httpClient, serverInfo, loginResponse.access_token);
            }

            AllCars = new Dictionary<string, CarInfo>();
            GroupCars = new Dictionary<int, Dictionary<string, CarInfo>>();
            if (result.data.Length > 0)
            {
                var numberRegex = new Regex("[^0-9]+");
                foreach (var dev in result.data)
                {
                    // FirmwareManagerなどと合わせて、整数以外も表示できるように変更
                    //// 車載器IDが整数以外であれば、スキップする。
                    //if (numberRegex.IsMatch(dev.device_id))
                    //{
                    //    continue;
                    //}
                    var office = Offices.FirstOrDefault(x => x.Id == dev.office_id);
                    if (office == null)
                    {
                        continue;
                    }

                    var deviceInfo = new DeviceInfo()
                    {
                        CarId = dev.car_id,
                        DeviceId = dev.device_id,
                        OfficeId = dev.office_id,
                    };
                    var carInfo = new CarInfo();
                    carInfo.DeviceInfo = deviceInfo;
                    carInfo.OfficeInfo = office;

                    // バスの一覧
                    //                    Devices.Add(deviceInfo);
                    AllCars[carInfo.DeviceInfo.DeviceId] = carInfo;
                    if (GroupCars.TryGetValue(deviceInfo.OfficeId, out Dictionary<string, CarInfo> group))
                    {
                        group[carInfo.DeviceInfo.DeviceId] = carInfo;
                    }
                    else
                    {
                        var devices = new Dictionary<string, CarInfo>();
                        devices[deviceInfo.DeviceId] = carInfo;
                        GroupCars[deviceInfo.OfficeId] = devices;
                    }
                    Debug.WriteLine($"-- carID: {dev.car_id} deviceID: {dev.device_id} office id: {dev.office_id}");
                }
                //                ArrayList.Adapter(Devices).Sort(new OrderedDeviceList());
            }
            else
            {
                Debug.WriteLine(@"@@@ Error: デバイスの一覧が取得できなかった。");
            }

            if (needEventUpdate)
            {
                var w = new TimeSpan(0, 0, 0);
                await UpdateEvents(targetOfficeId, w, cancelToken, evprog);
            }
        }

        /// <summary>
        /// 車載器情報を取得する
        /// プロパティ Devicesの車載器リストを更新する。
        /// 車載器IDが数字のもののみリストに加える。
        /// 車載器情報に加えて事業所情報も取得する。(けど、使ってない？読み捨て？)
        /// 条件によってはイベント情報の取得も行う。(イベント情報の取得は60日分)
        /// </summary>
        /// <param name="targetOfficeId">対象事業所ID</param>
        /// <param name="needEventUpdate">イベント情報を更新するか</param>
        /// <param name="evprog">プログレスバー更新用のデリゲートメソッド</param>
        /// <returns></returns>
        public async Task GetDevice(string deviceId, CancellationToken cancelToken)
        {
            var result = new JsonDeviceSearchResult();
            bool hasAccessToken = false;
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                OperateCancellation(cancelToken);
                var login = await HttpRequest.GetLoginToken(httpClient, serverInfo);
                if (!string.IsNullOrEmpty(login.access_token))
                {
                    loginResponse = login;
                    hasAccessToken = true;
                    Debug.WriteLine("Succeeded Login.");
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                OperateCancellation(cancelToken);
                result = await HttpRequest.GetDevice(
                    httpClient, serverInfo, loginResponse.access_token, deviceId);
            }

            AllCars = new Dictionary<string, CarInfo>();
            GroupCars = new Dictionary<int, Dictionary<string, CarInfo>>();
            if (result.data.Length > 0)
            {
                var numberRegex = new Regex("[^0-9]+");
                foreach (var dev in result.data)
                {
                    // 車載器IDが整数以外であれば、スキップする。
                    if (numberRegex.IsMatch(dev.device_id))
                    {
                        continue;
                    }

                    var deviceInfo = new DeviceInfo()
                    {
                        CarId = dev.car_id,
                        DeviceId = dev.device_id,
                        OfficeId = dev.office_id,
                    };
                    var carInfo = new CarInfo();
                    carInfo.DeviceInfo = deviceInfo;
                    carInfo.OfficeInfo = Offices.FirstOrDefault(x => x.Id == deviceInfo.OfficeId);

                    // バスの一覧
                    //                    Devices.Add(deviceInfo);
                    AllCars[carInfo.DeviceInfo.DeviceId] = carInfo;
                    if (GroupCars.TryGetValue(deviceInfo.OfficeId, out Dictionary<string, CarInfo> group))
                    {
                        group[carInfo.DeviceInfo.DeviceId] = carInfo;
                    }
                    else
                    {
                        var devices = new Dictionary<string, CarInfo>();
                        devices[deviceInfo.DeviceId] = carInfo;
                        GroupCars[deviceInfo.OfficeId] = devices;
                    }
                    Debug.WriteLine($"-- carID: {dev.car_id} deviceID: {dev.device_id} office id: {dev.office_id}");
                }
            }
            else
            {
                Debug.WriteLine(@"@@@ Error: デバイスの一覧が取得できなかった。");
            }
        }

        /// <summary>
        /// プログレスバー更新用のデリゲート
        /// </summary>
        /// <param name="deviceCount"></param>
        /// <param name="deviceTotal"></param>
        /// <param name="isCompleted"></param>
        public delegate void UpdateEventsProgress(int deviceCount, int deviceTotal, bool isCompleted);

        /// <summary>
        /// イベント情報の取得を行う。
        /// startWaitの分だけ実行が遅れる
        /// </summary>
        /// <param name="days">過去何日分のイベント情報を得るか</param>
        /// <param name="startWait">呼び出し待ち時間</param>
        /// <param name="cancelToken">cancelToken</param>
        /// <param name="prog">コールバック</param>
        /// <returns></returns>
        public async Task UpdateEvents(
            int targetOfficeId, TimeSpan startWait, CancellationToken cancelToken, UpdateEventsProgress prog = null)
        {
            eventInfoList.Clear();
            requestEventInfoList.Clear();
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (startWait != null && startWait.TotalSeconds > 0)
            {
                OperateCancellation(cancelToken);
                await Task.Delay(startWait);
            }

            bool hasAccessToken = false;
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                OperateCancellation(cancelToken);
                var login = await HttpRequest.GetLoginToken(httpClient, serverInfo);
                if (string.IsNullOrEmpty(login.access_token))
                {
                    return;
                }
                else
                {
                    loginResponse = login;
                    hasAccessToken = true;
                    Debug.WriteLine("Succeeded Login.");
                }
            }
            else
            {
                hasAccessToken = true;
            }

            int deviceCount = 0;
            //            int deviceTotal = Devices.Count;
            if (GroupCars.TryGetValue(targetOfficeId, out var groupCars)) {
                //var groupCars = GroupCars[targetOfficeId];
                int deviceTotal = groupCars.Count;
                if (hasAccessToken)
                {
                    var baseDate = DateTime.Now;
                    var resultList = new List<JsonMovie>();
                    foreach (CarInfo car in groupCars.Values)
                    {
                        var eventList = GetEventList(car, baseDate, cancelToken);
                        resultList.AddRange(eventList);

                        deviceCount++;
                        if (prog != null)
                        {
                            prog(deviceCount, deviceTotal, false);
                        }
                    }
                    Debug.WriteLine(DateTime.Now);

                    if (prog != null)
                    {
                        prog(deviceCount, deviceTotal, false);
                    }

                    try
                    {
                        // 検索結果には同じイベント(sequence)でカメラ毎の映像を持っているから
                        // 同じsequenseの情報を省いて異なるsequenceだけのリストを作る
                        var uniqueList = EventInfoUtil.UniqueMovieResults(resultList);
                        foreach (var ev in uniqueList)
                        {
                            string cid = ev.device_id.ToString();
                            var car = groupCars.Values.FirstOrDefault(x => x.DeviceInfo.DeviceId == cid);

                            // JSON情報からアプリ用の情報に変換
                            // イベント映像、リクエスト映像のリストに振り分けて格納
                            var ei = new EventInfo()
                            {
                                CarId = cid,
                                DeviceId = $"{ev.device_id}",
                                MovieId = ev.id,
                                Timestamp = ev.ts_start,
                                IsDownloadable = true,
                                IsPlayable = false,
                                //MovieType = (MovieType)ev.movie_type,
                            };
                            ei.MovieType = ConvertMovieType(ev.movie_type);

                            if (ev.movie_type == 2)
                            {
                                //  リクエスト
                                requestEventInfoList.Add(ei);
                                Debug.WriteLine($"ADDRequest carID: {ei.CarId} movie id: {ei.MovieId}, {ei.Timestamp}");
                            }
                            else
                            {
                                //  イベント系
                                eventInfoList.Add(ei);
                                Debug.WriteLine($"ADDEvent carID: {ei.CarId} movie id: {ei.MovieId}, {ei.Timestamp}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"@@@ Error {ex}, {ex.Message}");
                    }

                    if (prog != null)
                    {
                        prog(deviceCount, deviceTotal, true);
                    }
                }
            }
        }

        private List<JsonMovie> GetEventList(CarInfo car, DateTime baseDate, CancellationToken cancelToken)
        {
            OperateCancellation(cancelToken);
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            List<JsonMovie> resultList = new List<JsonMovie>();
            var startDate = baseDate.AddDays(-localSettings.EventListPeriod);
            var endDate = baseDate;
            // イベント映像リストを取得する
            try
            {
                JsonMovieSearchResult result =
                    HttpRequest.SearchMovies(
                        httpClient,
                        serverInfo.HttpAddr, 
                        loginResponse.access_token, 
                        car.DeviceInfo.DeviceId, 
                        startDate, 
                        endDate).GetAwaiter().GetResult();
                if (result == null || result.data == null)
                {
                    Debug.WriteLine($"SearchMovies result is NULL. ID:{car.DeviceInfo.DeviceId}");
                }
                else
                {
                    Debug.WriteLine($"SearchMovies result.data length = {result.data.Length} ID:{car.DeviceInfo.DeviceId}");
                    foreach (var ev in result.data)
                    {
                        resultList.Add(ev);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // 再トライ(60日分のイベントを複数回に分けて取得してみる)
                resultList = GetEventListByRange(car, baseDate, cancelToken);
            }
            return resultList;
        }

        private List<JsonMovie> GetEventListByRange(CarInfo car, DateTime baseDate, CancellationToken cancelToken)
        {
            List<JsonMovie> resultList = new List<JsonMovie>();
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            var loopMax = (localSettings.EventListPeriod / localSettings.EventListGetRange) + 1;
            var startDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day).AddDays(-localSettings.EventListPeriod);
            for (int count = 0; count < loopMax; count++)
            {
                var endDate = startDate.AddDays(localSettings.EventListGetRange);
                OperateCancellation(cancelToken);
                try
                {
                    // イベント映像リストを取得する
                    JsonMovieSearchResult result =
                        HttpRequest.SearchMovies(
                            httpClient,
                            serverInfo.HttpAddr, 
                            loginResponse.access_token, 
                            car.DeviceInfo.DeviceId, 
                            startDate, 
                            endDate).GetAwaiter().GetResult();
                    if (result == null || result.data == null)
                    {
                        Debug.WriteLine($"SearchMovies result is NULL. ID:{car.DeviceInfo.DeviceId}");
                    }
                    else
                    {
                        Debug.WriteLine($"SearchMovies result.data length = {result.data.Length} ID:{car.DeviceInfo.DeviceId}");
                        foreach (var ev in result.data)
                        {
                            resultList.Add(ev);
                        }
                    }
                    startDate = endDate;
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"SearchMovies ServerCancel Occurred. ID:{car.DeviceInfo.DeviceId}, start: {startDate}, end: {endDate}");
                }
            }
            return resultList;
        }


        /// <summary>
        /// 内部保持用のリストからUIで使用するイベントリストを作成する
        /// </summary>
        public void CreateEventBindingList()
        {
            CreateBindingList(eventInfoList, Events);
        }

        /// <summary>
        /// 内部保持用のリストからUIで使用するリクエストイベントリストを作成する
        /// </summary>
        public void CreateRequestBindingList()
        {
            CreateBindingList(requestEventInfoList, Requests);
            requestUpdateCompleted?.Invoke();
        }

        /// <summary>
        /// 内部保持用のリストからUIで使用するリクエストイベントリストを更新する
        /// </summary>
        public void UpdateEventBindingList()
        {
            UpdateBindingList(eventInfoList, Events);
        }

        /// <summary>
        /// 内部保持用のリストからUIで使用するリクエストイベントリストを更新する
        /// </summary>
        public void UpdateRequestBindingList()
        {
            UpdateBindingList(requestEventInfoList, Requests);
            requestUpdateCompleted?.Invoke();
        }

        /// <summary>
        /// 事業所ごとの車両情報から指定の車両情報を取得する。
        /// </summary>
        /// <param name="officeId">事業所ID</param>
        /// <param name="deviceId">車載器ID</param>
        /// <returns>車両情報</returns>
        public CarInfo GetCarInfoFromGroups(int officeId, string deviceId)
        {
            CarInfo result = null;
            if (GroupCars != null && GroupCars.TryGetValue(officeId, out var carInfos))
            {
                if (carInfos.TryGetValue(deviceId, out var carInfo))
                {
                    result = carInfo;
                }
            }
            return result;
        }

        /// <summary>
        /// 内部保持用のリストからUIで使用するイベントリストを作成する
        /// </summary>
        /// <param name="srcList">元リスト</param>
        /// <param name="destList">生成リスト</param>
        private void CreateBindingList(List<EventInfo> srcList, BindingList<EventInfo> destList)
        {
            destList.Clear();
            foreach (var eventInfo in srcList)
            {
                destList.Add(eventInfo);
            }
        }

        /// <summary>
        /// 内部保持用のリストからUIで使用するイベントリストを作成する
        /// </summary>
        /// <param name="srcList">元リスト</param>
        /// <param name="destList">生成リスト</param>
        private void UpdateBindingList(List<EventInfo> srcList, BindingList<EventInfo> destList)
        {
            foreach (var eventInfo in srcList)
            {
                var exist = destList.FirstOrDefault((x) => x.MovieId == eventInfo.MovieId);
                if (exist == null)
                {
                    destList.Add(eventInfo);
                }
            }
        }

        /// <summary>
        /// サーバー上の動画をダウンロードする。関連付けられているチャンネル全てが対象となる。
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task DownloadEventData(int movieId, Action<DownloadResult> callback, CancellationToken cancelToken)
        {
            var result = new DownloadResult();
            result.Result = -1;
            bool hasAccessToken = false;
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                OperateCancellation(cancelToken);
                var login = await HttpRequest.GetLoginToken(httpClient, serverInfo);
                if (string.IsNullOrEmpty(login.access_token))
                {
                    result.Result = -2;
                }
                else
                {
                    loginResponse = login;
                    hasAccessToken = true;
                    Debug.WriteLine("Succeeded Login.");
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                OperateCancellation(cancelToken);
                var zipfile = await HttpRequest.DownloadMovie(httpClient, serverInfo.HttpAddr, loginResponse.access_token, movieId);
                if (string.IsNullOrEmpty(zipfile))
                {
                    result.Result = -3;
                }
                else
                {
                    result.ZipFile = zipfile;
                    var unzipPath = Path.GetTempPath() + Path.GetRandomFileName();
                    try
                    {
                        // unzip.
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipfile, unzipPath);
                        Debug.WriteLine($"Scceeded unzip file {unzipPath}");

                        var filedic = EventInfoUtil.SearchMovieFiles(Path.Combine(unzipPath, @"movies"));
                        result.UnzippedPath = unzipPath;
                        result.chFile = filedic;
                        result.Result = 0;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"@@@ Error: {ex.ToString()}, {ex.Message}");
                        result.Result = -4;
                    }

                    try
                    {
                        Debug.WriteLine($"Delete download file: {zipfile}");
                        File.Delete(zipfile);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"@@@ Error: {ex.ToString()}, {ex.Message}");
                    }
                }
            }

            OperateCancellation(cancelToken);
            callback(result);
        }

        /// <summary>
        /// Gセンサー値を取得する。<br/>
        /// MUイベントのプリポストイベントを検索し、Gセンサー値を取得する。<br/>
        /// MUイベントからデータが取得できない場合は、MTXからGセンサー値を取得する。<br/>
        /// </summary>
        /// <param name="deviceId">車載器ID</param>
        /// <param name="timestamp">発生時刻</param>
        /// <param name="callback">データ取得時のコールバック</param>
        /// <param name="isDeleteWorkFile">true: 展開ファイルを削除する false: 展開ファイルを削除しない</param>
        /// <returns>なし</returns>
        public async Task GetDig(string deviceId, DateTime timestamp, Action<DigResult> callback, bool isDeleteWorkFile = true)
        {
            var result = new DigResult();
            result.Result = -1;
            bool hasAccessToken = false;
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                var login = await HttpRequest.GetLoginToken(httpClient, serverInfo);
                if (string.IsNullOrEmpty(login.access_token))
                {
                    result.Result = -2;
                }
                else
                {
                    loginResponse = login;
                    hasAccessToken = true;
                    Debug.WriteLine("Succeeded Login.");
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                // MUイベントから取得して、取得できなければMTXを読む
                result = await GetDigFromMu(deviceId, timestamp, isDeleteWorkFile, CancellationToken.None);
                if (result.Result != 0)
                {
                    result = await GetDigFromMtx(deviceId, timestamp, isDeleteWorkFile, CancellationToken.None);
                }
            }
            callback(result);
        }

        /// <summary>
        /// MUイベントからGセンサー値を取得する。
        /// </summary>
        /// <param name="deviceId">車載器ID</param>
        /// <param name="occurTime">発生時刻</param>
        /// <param name="isDeleteWorkFile">true: 展開ファイルを削除する。 false: 展開ファイルを削除しない</param>
        /// <returns>Gセンサー値</returns>
        private async Task<DigResult> GetDigFromMu(
            string deviceId, DateTime occurTime, bool isDeleteWorkFile, CancellationToken token)
        {
            DigResult result = new DigResult();
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            result.Result = -1;
            DateTime tsBegin = new DateTime(occurTime.Year, occurTime.Month, occurTime.Day, occurTime.Hour, 0, 0);
            DateTime tsEnd = tsBegin.AddHours(1);
            int dataType = 2;

            token.ThrowIfCancellationRequested();
            var zipfile = await HttpRequest.DownloadMuZip(
                httpClient, serverInfo.HttpAddr, loginResponse.access_token, deviceId, tsBegin, tsEnd, dataType);
            if (string.IsNullOrEmpty(zipfile))
            {
                result.Result = -3;
            }
            else
            {
                var unzipPath = Path.GetTempPath() + Path.GetRandomFileName();
                try
                {
                    // unzip.
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipfile, unzipPath);
                    Debug.WriteLine($"Scceeded unzip file {unzipPath}");
                    try
                    {
                        var dig = MtxUtil.Gravity(unzipPath, occurTime, token);
                        if (dig != null)
                        {
                            Debug.WriteLine($"Found a DIG - {dig}");
                            result.Result = 0;
                            result.GravityRecord = dig;
                        }
                    }
                    finally
                    {
                        if (isDeleteWorkFile)
                        {
                            Debug.WriteLine($"Delete extract file: {unzipPath}");
                            Directory.Delete(unzipPath, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"@@@ Error: {ex}, {ex.StackTrace}");
                    result.Result = -4;
                }
                finally
                {
                    if (isDeleteWorkFile)
                    {
                        Debug.WriteLine($"Delete download file: {zipfile}");
                        File.Delete(zipfile);
                    }
                }
            }

            return result;
        }

        private async Task<DigResult> GetDigFromMtx(
            string deviceId, DateTime occurTime, bool isDeleteWorkFile, CancellationToken token)
        {
            DigResult result = new DigResult();
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            // 前後1分(全部で3分間)のファイルがあればひっかかるだろう。
            var tsBegin = occurTime.AddMinutes(-1).AddSeconds(-30);
            var tsEnd = occurTime.AddMinutes(1).AddSeconds(30);

            var zipfile = await HttpRequest.DownloadMuZip(
                httpClient, serverInfo.HttpAddr, loginResponse.access_token, deviceId, tsBegin, tsEnd, 1, token);
            if (string.IsNullOrEmpty(zipfile))
            {
                result.Result = -3;
            }
            else
            {
                var unzipPath = Path.GetTempPath() + Path.GetRandomFileName();
                try
                {
                    // unzip.
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipfile, unzipPath);
                    Debug.WriteLine($"Scceeded unzip file {unzipPath}");
                    var dig = MtxUtil.GetGravity(unzipPath, occurTime, localSettings.PrePostDuration, token);
                    if (dig != null)
                    {
                        Debug.WriteLine($"Found a DIG - {dig}");
                        result.Result = 0;
                        result.GravityRecord = dig;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"@@@ Error: {ex}, {ex.StackTrace}");
                    result.Result = -4;
                }

                if (isDeleteWorkFile)
                {
                    try
                    {
                        Debug.WriteLine($"Delete download file: {zipfile}");
                        File.Delete(zipfile);
                        Directory.Delete(unzipPath, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"@@@ Error: {ex}, {ex.StackTrace}");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// イベントの並べ替えに使う比較判定クラス。
        /// </summary>
        public class OrderedEventInfoList : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                var xx = (EventInfo)x;
                var yy = (EventInfo)y;
                if (xx.Timestamp == yy.Timestamp)
                {
                    if (xx.MovieId == yy.MovieId)
                    {
                        // 同一Eventということになる。ここを通ったなら、そもそもおかしい。
                        return 0;
                    }
                    else if (xx.MovieId < yy.MovieId)
                    {
                        return 1;
                    }
                    return -1;
                }
                else if (xx.Timestamp < yy.Timestamp)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class DownloadResult
        {
            public int Result { get; set; }
            public Dictionary<int, string> chFile { get; set; }
            public string ZipFile { get; set; }
            public string UnzippedPath { get; set; }
        }

        public class DigResult
        {
            public int Result { get; set; }
            public GravityRecord GravityRecord { get; set; }
        }

        public string ConvertMovieType(int value)
        {
            var s = string.Empty;
            try
            {
                switch (value)
                {
                    case 0: // "Gsensor":
                        s = Properties.Resources.MovieType0;
                        break;
                    case 1: // "Gsensor1G":
                        s = Properties.Resources.MovieType1;
                        break;
                    case 2: // "Request":
                        s = Properties.Resources.MovieType2;
                        break;
                    case 3: // "Emergency":
                        s = Properties.Resources.MovieType3;
                        break;
                    default:
                        s = "その他";
                        break;
                }
            }
            catch { }

            return s;
        }

        /// <summary>
        /// 実行中の非同期処理が中断された場合に呼び出されるキャンセル処理。
        /// </summary>
        /// <param name="cancelToken">キャンセル情報</param>
        private void OperateCancellation(CancellationToken cancelToken)
        {
            if (cancelToken != null && cancelToken.IsCancellationRequested)
            {
                cancelToken.ThrowIfCancellationRequested();
            }
        }
    }
}
