using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RealtimeViewer.Model;
using RealtimeViewer.Network;
using RealtimeViewer.Network.Http;
using RealtimeViewer.WMShipView;
using static System.Net.Mime.MediaTypeNames;

namespace RealtimeViewer.Network.Http
{
    public partial class RequestSequence
    {
        /// <summary>
        /// 事業所一覧の取得
        /// </summary>
        /// <param name="excludeOfficeList">一覧から除外する事業所IDのリスト</param>
        /// <returns>事業所一覧</returns>
        public async Task<WMDataSet.OfficeDataTable> GetOfficesAsync(List<int> excludeOfficeList)
        {
            var offices = new WMDataSet.OfficeDataTable();
            var server = ServerInfo.GetPhygicalServerInfo();
            var hasAccessToken = false;
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                var login = await HttpRequest.GetLoginToken(httpClient, server);
                if (!string.IsNullOrEmpty(login.access_token))
                {
                    loginResponse = login;
                    hasAccessToken = true;
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                var officeResult = await HttpRequest.GetAllOffices(httpClient, server.HttpAddr, loginResponse.access_token);
                if (officeResult != null && officeResult.data != null)
                {
                    foreach (var jo in officeResult.data)
                    {
                        if (int.TryParse(jo.id, out var officeId) &&
                            int.TryParse(jo.company_id, out var companyId) &&
                            ServerInfo.IsDisplayOffice(officeId))
                        {
                            var row = offices.NewOfficeRow();
                            row.OfficeId = officeId;
                            row.CompanyId = companyId;
                            row.Name = jo.name;

                            row.Visible = !excludeOfficeList.Contains(officeId);

                            if (OfficeLocations.TryGetValue(officeId, out (int x, int y) value))
                            {
                                row.Longitude = value.x;
                                row.Latitude = value.y;
                            }
                            else
                            {
                                row.Longitude = 0;
                                row.Latitude = 0;
                            }
                            offices.AddOfficeRow(row);
                        }
                    }
                    offices.AcceptChanges();
                }
            }
            return offices;
        }

        public async Task<WMDataSet.DeviceDataTable> GetDevicesAsync()
        {
            var devices = new WMDataSet.DeviceDataTable();
            var hasAccessToken = false;
            var result = new JsonDeviceSearchResult();
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                var login = await HttpRequest.GetLoginToken(httpClient, serverInfo);
                if (!string.IsNullOrEmpty(login.access_token))
                {
                    loginResponse = login;
                    hasAccessToken = true;
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                result = await HttpRequest.GetAllDevices(httpClient, serverInfo, loginResponse.access_token);
            }

            if (0 < result.data.Length)
            {
                foreach (var device in result.data)
                {
                    var row = devices.NewDeviceRow();
                    row.DeviceId = device.device_id;
                    row.CarId = device.car_id;
                    row.CarNumber = device.car_number;
                    row.OfficeId = device.office_id;
                    devices.AddDeviceRow(row);
                }
                devices.AcceptChanges();
            }
            return devices;
        }

        public async Task<WMDataSet.EventListDataTable> GetAllEventsAsync(
               EnumerableRowCollection<WMDataSet.DeviceRow> devices, 
               CancellationToken token,
               UpdateEventsProgress progress)
        {
            token.ThrowIfCancellationRequested();
            var events = new WMDataSet.EventListDataTable();
            var hasAccessToken = false;
            var result = new JsonDeviceSearchResult();
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                var login = await HttpRequest.GetLoginToken(httpClient, serverInfo);
                if (!string.IsNullOrEmpty(login.access_token))
                {
                    loginResponse = login;
                    hasAccessToken = true;
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                var baseDate = DateTime.Now;
                var count = 0;
                var total = devices.Count();
                foreach (var device in devices)
                {
                    token.ThrowIfCancellationRequested();
                    var eventList = await GetEventListAsync(device, baseDate, token);
                    if (0 < eventList.Count)
                    {
                        var beforeTimestamp = DateTime.MinValue;
                        var beforeSequence = string.Empty;
                        foreach (var movie in eventList)
                        {
                            if (beforeSequence != movie.sequence ||
                                beforeTimestamp != movie.ts_start) 
                            {
                                var row = events.NewEventListRow();
                                row.Timestamp = movie.ts_start;
                                row.DeviceId = movie.device_id;
                                row.CarNumber = device.CarNumber;
                                row.Sequence = movie.sequence;
                                row.MovieId = movie.id;
                                row.MovieType = movie.movie_type;
                                row.MovieTypeName = ConvertMovieType(movie.movie_type);
                                events.AddEventListRow(row);
                                beforeTimestamp = movie.ts_start;
                                beforeSequence = movie.sequence;
                            }
                        }
                        events.AcceptChanges();
                    }
                    count++;
                    progress?.Invoke(count, total, false);
                }
                progress?.Invoke(count, total, true);
            }
            return events;
        }

        public async Task GetGravity(
            WMDataSet.EventListDataTable eventTable, 
            CancellationToken token,
            UpdateEventsProgress progress)
        {
            var count = 0;
            var total = eventTable.Count;
            foreach (var eventData in eventTable)
            {
                token.ThrowIfCancellationRequested();
                if (string.IsNullOrEmpty(eventData.Remarks))
                {
                    var dig = await GetDig(eventData.DeviceId, eventData.Timestamp, token);
                    if (dig.GravityRecord is GravityRecord g)
                    {
                        eventData.GX = g.X;
                        eventData.GY = g.Y;
                        eventData.GZ = g.Z;
                        eventData.Remarks = $"X:{g.X:0.00}, Y:{g.Y:0.00}, Z:{g.Z:0.00}";
                    }
                }
                progress.Invoke(count, total, false);
                count++;
            }
            eventTable.AcceptChanges();
            progress.Invoke(count, total, false);
        }

        private async Task<List<JsonMovie>> GetEventListAsync(
                WMDataSet.DeviceRow device, DateTime baseDate, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            var serverInfo = ServerInfo.GetPhygicalServerInfo();
            var resultList = new List<JsonMovie>();
            var startDate = baseDate.AddDays(-localSettings.EventListPeriod);
            var endDate = baseDate;
            // イベント映像リストを取得する
            try
            {
                var result = await HttpRequest.SearchMovies(
                        httpClient,
                        serverInfo.HttpAddr, 
                        loginResponse.access_token, 
                        device.DeviceId, 
                        startDate, 
                        endDate);
                if (result != null && result.data != null)
                {
                    foreach (var ev in result.data)
                    {
                        resultList.Add(ev);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // 再トライ(60日分のイベントを複数回に分けて取得してみる)
                resultList = await GetEventListByRangeAsync(device, baseDate, cancelToken);
            }
            return resultList;
        }

        private async Task<List<JsonMovie>> GetEventListByRangeAsync(
            WMDataSet.DeviceRow device, DateTime baseDate, CancellationToken cancelToken)
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
                    var result = await HttpRequest.SearchMovies(
                            httpClient,
                            serverInfo.HttpAddr, 
                            loginResponse.access_token, 
                            device.DeviceId, 
                            startDate, 
                            endDate);
                    if (result != null && result.data != null)
                    {
                        foreach (var ev in result.data)
                        {
                            resultList.Add(ev);
                        }
                    }
                    startDate = endDate;
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"SearchMovies ServerCancel Occurred. ID:{device.DeviceId}, start: {startDate}, end: {endDate}");
                }
            }
            return resultList;
        }

        private async Task<DigResult> GetDig(
            string deviceId, 
            DateTime timestamp, 
            CancellationToken token,
            bool isDeleteWorkFile = true)
        {
            var result = new DigResult();
            result.Result = -1;
            var hasAccessToken = false;
            var serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                token.ThrowIfCancellationRequested();
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
                result = await GetDigFromMu(deviceId, timestamp, isDeleteWorkFile, token);
                if (result.Result != 0)
                {
                    token.ThrowIfCancellationRequested();
                    result = await GetDigFromMtx(deviceId, timestamp, isDeleteWorkFile, token);
                }
            }
            return result;
        }

        /// <summary>
        /// サーバー上の動画をダウンロードする。関連付けられているチャンネル全てが対象となる。
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<DownloadResult> DownloadEventDataAsync(int movieId, CancellationToken token)
        {
            var result = new DownloadResult();
            result.Result = -1;
            var hasAccessToken = false;
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.access_token))
            {
                token.ThrowIfCancellationRequested();
                var login = await HttpRequest.GetLoginToken(httpClient, serverInfo);
                if (string.IsNullOrEmpty(login.access_token))
                {
                    result.Result = -2;
                }
                else
                {
                    loginResponse = login;
                    hasAccessToken = true;
                }
            }
            else
            {
                hasAccessToken = true;
            }

            if (hasAccessToken)
            {
                token.ThrowIfCancellationRequested();
                var zipfile = await HttpRequest.DownloadMovie(
                    httpClient, serverInfo.HttpAddr, loginResponse.access_token, movieId, token);
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
            return result;
        }

        public async Task<StreamingServerInfo> GetStreamingServerAsync(string deviceId)
        {
            var serverInfo = ServerInfo.GetPhygicalServerInfo();
            return await StreamingRequest.GetStreamingInfo(httpClient, serverInfo.HttpAddr, deviceId);
        }
    }
}
