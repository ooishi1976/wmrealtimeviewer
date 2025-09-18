using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Assemblies;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using RealtimeViewer.Network;

namespace RealtimeViewer.Network
{
    public enum ServerKind
    {
        /// <summary>
        /// Aws Prod
        /// </summary>
        Tobu,
        /// <summary>
        /// Aws stg
        /// </summary>
        Dev
    }

    public enum ServerIndex
    {
        Tobu = 0,
        Dev1 = 1,
        Meiji = 2,
        Nemuro = 3,
        WeatherMedia = 4,
        Dev2 = 5,
    }

    public enum UserIndex
    {
        Tobu = 0,
        Multiwave = 1,
        Meiji = 2,
        Nemuro = 3,
        Issui = 4,
        WeatherMedia = 5,
    }

    public class ServerInfo
    {
        public ServerKind ServerKind { get; set; }
        public string HttpAddr { get; set; }
        public string MqttAddr { get; set; }
        public string AccessId { get; set; }
        public string AccessPassword { get; set; }
    }


    public class OperationServerInfo
    {
        /// <summary>
        /// 種別(東武、ISSUI、開発、根室、ウェザーメディア)
        /// </summary>
        public ServerIndex Id { get; set; }
        /// <summary>
        /// 本番、開発
        /// </summary>
        public ServerKind Kind { get; set; }

        /// <summary>
        /// ストリーミングタイプ
        /// </summary>
        public StreamingTypes StreamingType { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 見せるオフィス
        /// </summary>
        public List<int> ValidOfficeIds { get; set; }
        /// <summary>
        /// 無かったことにするオフィス(見せるオフィスの中から)
        /// </summary>
        public List<int> ForbiddunOfficeIds { get; set; }
        /// <summary>
        /// 隠すオフィス(東武の廃止営業所)
        /// </summary>
        public List<int> InvisibleOfficeIds { get; set; }
        /// <summary>
        /// 認証待ちダイアログの表示
        /// </summary>
        public bool IsShowWaitDialog { get; set; }
        /// <summary>
        /// リモート設定でRetainMessageを使うか
        /// </summary>
        public bool UseRetainMessageForRemoteSettings { get; set; }

        /// <summary>
        /// リモート設定でRetainMessageを利用しないOfficeIds
        /// </summary>
        public List<int> ExcludeOfficesForRetain { get; set; }

        /// <summary>
        /// 接続先サーバー情報
        /// </summary>
        /// <returns></returns>
        public ServerInfo GetPhygicalServerInfo()
        {
            return ServerInfoList.ServerTypes[Kind];
        }

        /// <summary>
        /// リモート設定でRetainMessageを使うOfficeか
        /// </summary>
        /// <param name="officeId"></param>
        /// <returns></returns>
        public bool IsUseRetainMessage(int officeId)
        {
            bool result = false;
            if (UseRetainMessageForRemoteSettings)
            {
                if (ExcludeOfficesForRetain == null)
                {
                    result = true;
                }
                else if (!ExcludeOfficesForRetain.Contains(officeId))
                {
                    result = true;
                }
            }
            return result;
        }

        public bool IsDisplayOffice(int officeId)
        {
            var result = true;
            if (0 < ValidOfficeIds.Count)
            {
                if (ValidOfficeIds.Contains(officeId))
                {
                    if (ForbiddunOfficeIds.Contains(officeId))
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else if (ForbiddunOfficeIds.Contains(officeId))
            {
                result = true;
            }
            return result;
        }

        public void WriteJson(string filePath)
        {
            var data = new Dictionary<ServerIndex, List<OperationServerInfo>>();
            data[ServerIndex.Tobu] = new List<OperationServerInfo>() { this };

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
        }
    }

    public class ServerInfoList
    {
        /// <summary>
        /// 物理サーバー情報
        /// </summary>
        public static Dictionary<ServerKind, ServerInfo> ServerTypes = new Dictionary<ServerKind, ServerInfo>() {
            { 
                ServerKind.Tobu, new ServerInfo() {
                    ServerKind = ServerKind.Tobu, HttpAddr = @"issui-dvr.com", MqttAddr = @"mqtt.issui-dvr.com",
                    AccessId = @"99999", AccessPassword = @"Aicei6Lufee8aeV8"}
            },
            {
                ServerKind.Dev, new ServerInfo() {
                    ServerKind = ServerKind.Tobu, HttpAddr = @"stg.issui-dvr.com", MqttAddr = @"stg_mqtt.issui-dvr.com",
                    AccessId = @"3", AccessPassword = @"password"}
            }
        };

        public static bool IsServerChanging = false;


        //public static BindingList<OperationServerInfo> Servers = new BindingList<OperationServerInfo>()
        //{
        //    new OperationServerInfo() { 
        //        Id = ServerIndex.Tobu, Name = @"本番運用", Kind = ServerKind.Tobu},
        //    new OperationServerInfo() {
        //        Id = ServerIndex.Dev, Name = @"開発", Kind = ServerKind.Dev },
        //    new OperationServerInfo() {
        //        Id = ServerIndex.Meiji, Name = @"明治", Kind = ServerKind.Dev },
        //    new OperationServerInfo() {
        //        Id = ServerIndex.Nemuro, Name = @"根室(試行)", Kind = ServerKind.Dev },
        //    new OperationServerInfo() {
        //        Id = ServerIndex.WeatherMedia, Name = @"ウェザーメディア", Kind = ServerKind.Dev },
        //};

        /// <summary>
        /// 設定を埋め込みリソースから取得<br/>
        /// /resources/server.json
        /// </summary>
        /// <param name="userIndex"></param>
        /// <returns></returns>
        public static BindingList<OperationServerInfo> GetServers(UserIndex userIndex)
        {
            BindingList<OperationServerInfo> result = new BindingList<OperationServerInfo>();

            var resourceName = "RealtimeViewer.resources.server.json";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                if (JsonConvert.DeserializeObject<Dictionary<UserIndex, List<OperationServerInfo>>>(json) is Dictionary<UserIndex, List<OperationServerInfo>> serverInfos) 
                {
                    if (serverInfos.ContainsKey(userIndex))
                    {
                        foreach (var server in serverInfos[userIndex])
                        {
                            result.Add(server);
                        }
                    }
                }
            }
            return result;
        }

        //public static BindingList<OperationServerInfo> GetServers(UserIndex userIndex)
        //{
        //    BindingList<OperationServerInfo> result = new BindingList<OperationServerInfo>();
        //    switch(userIndex)
        //    {
        //        case UserIndex.Tobu:
        //            result.Add(new OperationServerInfo()
        //            {
        //                Id = ServerIndex.Tobu,
        //                Name = @"東武バス",
        //                Kind = ServerKind.Tobu,
        //                ValidOfficeIds = new List<int>(),
        //                ForbiddunOfficeIds = new List<int>(),
        //                InvisibleOfficeIds = new List<int>() { 1, 17, 18 },
        //                IsShowWaitDialog = true,
        //                UseRetainMessageForRemoteSettings = true,
        //                ExcludeOfficesForRetain = new List<int>()
        //            });
        //            break;
        //        case UserIndex.Multiwave:
        //            result.Add(new OperationServerInfo()
        //            {
        //                Id = ServerIndex.Tobu,
        //                Name = @"東武バス",
        //                Kind = ServerKind.Tobu,
        //                ValidOfficeIds = new List<int>(),
        //                ForbiddunOfficeIds = new List<int>(),
        //                InvisibleOfficeIds = new List<int>() { 1, 17, 18 },
        //                IsShowWaitDialog = true,
        //                UseRetainMessageForRemoteSettings = true,
        //                ExcludeOfficesForRetain = new List<int>()
        //            });
        //            result.Add(new OperationServerInfo()
        //            {
        //                Id = ServerIndex.Dev,
        //                Name = @"開発",
        //                Kind = ServerKind.Dev,
        //                ValidOfficeIds = new List<int>(),
        //                ForbiddunOfficeIds = new List<int>(),
        //                InvisibleOfficeIds = new List<int>(),
        //                IsShowWaitDialog = true,
        //                UseRetainMessageForRemoteSettings = true,
        //                ExcludeOfficesForRetain = new List<int>() { 2 }
        //            });
        //            break;
        //        case UserIndex.Meiji:
        //            result.Add(new OperationServerInfo()
        //            {
        //                Id = ServerIndex.Meiji,
        //                Name = @"明治大学",
        //                Kind = ServerKind.Dev,
        //                ValidOfficeIds = new List<int>() { 2 },
        //                ForbiddunOfficeIds = new List<int>(),
        //                //ForbiddunOfficeIds = new List<int>() { 1, 3, 4 },
        //                InvisibleOfficeIds = new List<int>(),
        //                IsShowWaitDialog = false,
        //                UseRetainMessageForRemoteSettings = false,
        //                ExcludeOfficesForRetain = new List<int>()
        //            });
        //            break;
        //        case UserIndex.Nemuro:
        //            result.Add(new OperationServerInfo()
        //            {
        //                Id = ServerIndex.Nemuro,
        //                Name = @"根室観光交通",
        //                Kind = ServerKind.Dev,
        //                ValidOfficeIds = new List<int>() { 3 },
        //                ForbiddunOfficeIds = new List<int>(),
        //                //ForbiddunOfficeIds = new List<int>() { 1, 2, 4 },
        //                InvisibleOfficeIds = new List<int>(),
        //                IsShowWaitDialog = false,
        //                UseRetainMessageForRemoteSettings = true,
        //                ExcludeOfficesForRetain = new List<int>()
        //            });
        //            break;
        //        case UserIndex.Issui:
        //            result.Add(new OperationServerInfo()
        //            {
        //                Id = ServerIndex.Tobu,
        //                Name = @"東武バス",
        //                Kind = ServerKind.Tobu,
        //                ValidOfficeIds = new List<int>(),
        //                ForbiddunOfficeIds = new List<int>(),
        //                InvisibleOfficeIds = new List<int>() { 1, 17, 18 },
        //                IsShowWaitDialog = false,
        //                UseRetainMessageForRemoteSettings = true,
        //                ExcludeOfficesForRetain = new List<int>()
        //            });
        //            result.Add(new OperationServerInfo()
        //            {
        //                Id = ServerIndex.Nemuro,
        //                Name = @"根室観光交通",
        //                Kind = ServerKind.Dev,
        //                ValidOfficeIds = new List<int>() { 3 },
        //                ForbiddunOfficeIds = new List<int>(),
        //                InvisibleOfficeIds = new List<int>(),
        //                IsShowWaitDialog = false,
        //                UseRetainMessageForRemoteSettings = true,
        //                ExcludeOfficesForRetain = new List<int>()
        //            });
        //            break;
        //        case UserIndex.WeatherMedia:
        //            result.Add(new OperationServerInfo()
        //            {
        //                Id = ServerIndex.WeatherMedia,
        //                Name = @"ウェザーメディア",
        //                Kind = ServerKind.Dev,
        //                ValidOfficeIds = new List<int>() { 4 },
        //                ForbiddunOfficeIds = new List<int>(),
        //                InvisibleOfficeIds = new List<int>(),
        //                IsShowWaitDialog = false,
        //                UseRetainMessageForRemoteSettings = true,
        //                ExcludeOfficesForRetain = new List<int>()
        //            });
        //            break;
        //    }
        //    return result;
        //}
    }
}

public static class ServerInfoExtentions
{
    public static int Value(this ServerIndex serverIndex)
    {
        return (int)serverIndex;
    }
}
