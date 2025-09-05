using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Diagnostics;

namespace RealtimeViewer
{
    /// <summary>
    /// デバイス除去通知イベントを監視して、ハンドラを実行するクラス。
    /// VID_XXXX&PID_XXXXを含むデバイスIDで監視する。
    /// 注意: 
    /// PnPEntityでの検索を選んだ場合、
    /// Entityが複数存在するデバイスではEntity毎にイベントが通知されるため、
    /// ハンドラが複数回実行されることになる。
    /// </summary>
    public class DeviceWatcher
    {
        // 参照WMIテーブルの種類
        public enum WatchType
        {
            USB,  // Win32_USBHub
            PNP,  // Win32_PnPEntity
        }

        public class UsbDevice
        {
            public WatchType WatchType { get; set; }
            public string VendorId { get; set; }
            public string ProductId { get; set; }
        }

        /// <summary>
        /// デバイス監視オブジェクト(Create)
        /// </summary>
        private ManagementEventWatcher createWatcher;
        /// <summary>
        /// デバイス監視オブジェクト(Remove)
        /// </summary>
        private ManagementEventWatcher removeWatcher;
        /// <summary>
        /// 監視対象が接続された時に呼ばれるアクション
        /// </summary>
        private Action createHandler;
        /// <summary>
        /// 監視対象が切断された時に呼ばれるアクション
        /// </summary>
        private Action removeHandler;


        /// <summary>
        /// 監視対象デバイスを監視オブジェクトに設定する。
        /// </summary>
        /// <param name="deviceIds">VID, PIDを持つオブジェクトのディクショナリ</param>
        /// <param name="removeHandler">removeイベント通知時に呼ばれるアクション。</param>
        public void SetWatchTarget(
                Dictionary<string, UsbDevice> deviceIds,
                Action createHandler,
                Action removeHandler)
        {
            createWatcher = GetCreationWatcher(deviceIds);
            this.createHandler = createHandler;
            removeWatcher = GetDeletionWatcher(deviceIds);
            this.removeHandler = removeHandler;
        }

        /// <summary>
        /// USBデバイスの追加イベント通知のQueryを取得する。
        /// </summary>
        /// <param name="deviceIds">VID, PIDを持つオブジェクトのディクショナリ</param>
        /// <returns>WMIObjcetQueryString</returns>
        private string GetWMIQueryCreationEvent(Dictionary<string, UsbDevice> deviceIds)
        {
            return GetWMIQuery("__InstanceCreationEvent", deviceIds);
        }

        /// <summary>
        /// USBデバイスの除去イベント通知のQueryを取得する。
        /// </summary>
        /// <param name="deviceIds">VID, PIDを持つオブジェクトのディクショナリ</param>
        /// <returns>WMIObjcetQueryString</returns>
        private string GetWMIQueryDeletionEvent(Dictionary<string, UsbDevice> deviceIds)
        {
            return GetWMIQuery("__InstanceDeletionEvent", deviceIds);
        }

        /// <summary>
        /// USBデバイスの除去イベント通知のQueryを取得する。
        /// </summary>
        /// <param name="deviceIds">VID, PIDを持つオブジェクトのディクショナリ</param>
        /// <returns>WMIObjcetQueryString</returns>
        private string GetWMIQuery(string tableName, Dictionary<string, UsbDevice> deviceIds)
        {
            string result = $@"select * from {tableName} within 1";
            Dictionary<WatchType, string> conditions = new Dictionary<WatchType, string>();
            foreach (var item in deviceIds.Values)
            {
                if (conditions.TryGetValue(item.WatchType, out string condition))
                {
                    condition += $@" OR TargetInstance.DeviceID like '%VID_{item.VendorId}&PID_{item.ProductId}%'";
                }
                else
                {
                    condition = $@"TargetInstance.DeviceID like '%VID_{item.VendorId}&PID_{item.ProductId}%'";
                }
                conditions[item.WatchType] = condition;
            }
            foreach (var (index, item) in conditions.Select((item, index) => (index, item)))
            {
                if (0 == index)
                {
                    result += " where ";
                }
                else if (0 < index)
                {
                    result += " OR ";
                }
                switch (item.Key)
                {
                    case WatchType.USB:
                        result += $@"(TargetInstance ISA 'Win32_USBHub' AND ({item.Value}))";
                        break;
                    case WatchType.PNP:
                        result += $@"(TargetInstance ISA 'Win32_PnPEntity' AND ({item.Value}))";
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// ManagementEventWatcher。
        /// DeviceIDにVID, PIDを含むをDeviceのイベントを監視対象とする。
        /// </summary>
        /// <param name="deviceIds">VID, PIDのタプルを持つディクショナリ</param>
        /// <returns>監視オブジェクト</returns>
        private ManagementEventWatcher GetCreationWatcher(Dictionary<string, UsbDevice> deviceIds)
        {
            string query = GetWMIQueryCreationEvent(deviceIds);
            var watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += OnWatcherDeviceCreated;
            return watcher;
        }

        /// <summary>
        /// ManagementEventWatcher。
        /// DeviceIDにVID, PIDを含むをDeviceのイベントを監視対象とする。
        /// </summary>
        /// <param name="deviceIds">VID, PIDのタプルを持つディクショナリ</param>
        /// <returns>監視オブジェクト</returns>
        private ManagementEventWatcher GetDeletionWatcher(Dictionary<string, UsbDevice> deviceIds)
        {
            string query = GetWMIQueryDeletionEvent(deviceIds);
            var watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += OnWatcherDeviceRemoved;
            return watcher;
        }

        /// <summary>
        /// 監視開始
        /// </summary>
        public void Start()
        {
            if (removeWatcher != null)
            {
                Debug.WriteLine("ManagementEventWatcher: start.");
                createWatcher.Start();
                removeWatcher.Start();
            }
        }

        /// <summary>
        /// 監視終了
        /// </summary>
        public void Stop()
        {
            if (removeWatcher != null)
            {
                Debug.WriteLine("ManagementEventWatcher: end.");
                createWatcher.Stop();
                removeWatcher.Stop();
            }
        }

        /// <summary>
        /// デバイスが除去された際のイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInfoUpdate"></param>
        private void OnWatcherDeviceCreated(object sender, EventArrivedEventArgs args)
        {
            var device = args.NewEvent.GetPropertyValue("TargetInstance") as ManagementBaseObject;
            Debug.WriteLine($@"ManagementEventWatcher: ""{device.GetPropertyValue("PnPDeviceID")}"" created.");
            createHandler?.Invoke();
        }

        /// <summary>
        /// デバイスが除去された際のイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInfoUpdate"></param>
        private void OnWatcherDeviceRemoved(object sender, EventArrivedEventArgs args)
        {
            var device = args.NewEvent.GetPropertyValue("TargetInstance") as ManagementBaseObject;
            Debug.WriteLine($@"ManagementEventWatcher: ""{device.GetPropertyValue("PnPDeviceID")}"" removed.");
            removeHandler?.Invoke();
        }
    }
}
