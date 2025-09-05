using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using RealtimeViewer;

namespace WindowsFormsDeviceWatcher
{
    using WatchType = DeviceWatcher.WatchType;
    using UsbDevice = DeviceWatcher.UsbDevice;

    public partial class Form1 : Form
    {
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVTYP_PORT = 0x3;
        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_HDR
        {
            internal Int32 dbch_size;
            internal Int32 dbch_devicetype;
            internal Int32 dbch_reserved;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_PORT_A
        {
            internal Int32 dbcp_size;
            internal Int32 dbcp_devicetype;
            internal Int32 dbcp_reserved;
            internal char[] dbcp_name;
        }

        Binder binder = new Binder();

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher(
            //        @"Select * From Win32_PnPEntity where (DeviceID like '%VID_05BA&PID_000A%' or DeviceId like '%VID_0416&PID_511D%')");
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPDevice");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From CIM_BinarySensor");

            foreach (var device in searcher.Get())
            {
                var deviceId = device["DeviceID"] as string;
                if (deviceId.Contains("VID_05BA&PID_000A") || deviceId.Contains("VID_0416&PID_511D"))
                {
                    //Debug.WriteLine($"{device["DeviceID"]}: {device["PNPClass"]}: {device["ClassGuid"]}\n");
                    Debug.WriteLine($"{device["name"]}: {device["PNPDeviceID"]}\n");
                }
            }
            Debug.WriteLine(searcher.Get().Count);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    //                    test(m);
                    break;
            }
        }

        private void test(Message m) {
            Debug.WriteLine(m.WParam.ToInt32());
            switch (m.WParam.ToInt32()) {
                case 0x8000:  // arrive
                case 0x8004:  // remove
                    this.timer1.Enabled = true;
                    break;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                $@"Select * From Win32_USBHub where DeviceID like '%VID_05BA&PID_000A%'");
            var queryResult = searcher.Get();
            if (0 < queryResult.Count)
            {
                foreach (var target in searcher.Get())
                {
                    Debug.WriteLine($"{target.ToString()} arrived!");
                }
            }
            else
            {
                Debug.WriteLine("target is removed!");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            //            string query = @"select * from __InstanceCreationEvent within 1 WHERE TargetInstance ISA 'Win32_USBHub' and TargetInstance.DeviceID like '%VID_0416&PID_511D%'";
            //            string query = @"select *, TargetInstance.DeviceID from __InstanceDeletionEvent within 1 WHERE TargetInstance ISA 'Win32_USBHub' and TargetInstance.DeviceID like '%VID_0416&PID_511D%'";
            //string query = @"select *, TargetInstance.DeviceID from __InstanceDeletionEvent within 1 WHERE TargetInstance ISA 'Win32_PnPEntity' and TargetInstance.DeviceID like '%VID_05BA&PID_000A%'";
            string query = @"select *, TargetInstance.DeviceID from __InstanceDeletionEvent within 1 WHERE (TargetInstance ISA 'Win32_USBHub' and TargetInstance.DeviceID like '%VID_0416&PID_511D%') or (TargetInstance ISA 'Win32_PnPEntity' and TargetInstance.DeviceID like '%VID_05BA&PID_000A%')";
            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += ((s, ev) => {
                var target = ev.NewEvent.Properties["TargetInstance"].Value as ManagementBaseObject;
                Debug.WriteLine("到着");
            });
            watcher.Start();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            string query = @"select * from Win32_DeviceChangeEvent WHERE EventType = 2";
            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += ((s, ev) => {
                Debug.WriteLine($"到着: {ev.NewEvent.Properties["SECURITY_DESCRIPTOR"]}");
            });
            watcher.Start();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            DeviceWatcher watcher = new DeviceWatcher();
            var targetList = new Dictionary<string, UsbDevice>()
            {
                //{ "testA", new UsbDevice() { WatchType = WatchType.USB, VendorId = "0416", ProductId = "511D" } },
                //{ "testB", new UsbDevice() { WatchType = WatchType.PNP, VendorId = "0174", ProductId = "55AA" } },
                { "DP4500", new UsbDevice() { WatchType = WatchType.PNP, VendorId = "05BA", ProductId = "000A"} },
            };
            watcher.SetWatchTarget(targetList, () => { Debug.WriteLine("aaaa"); }, () => { Debug.WriteLine("bbbbb"); });
            watcher.Start();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            binder.Aa = "uma";
            binder.Bb = true;
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            binderBindingSource.DataSource = binder;
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            binder.Aa = "shika";
            binder.Bb = false;
        }
    }

    public class Binder : INotifyPropertyChanged
    {
        private string _aa;
        public string Aa {
            get { return _aa;  }
            set { 
                _aa = value;
                NotifyPropertyChanged();
            } 
        }
        private bool _bb;
        public bool Bb { 
            get {
                return _bb;
            }
            set {
                _bb = value;
                NotifyPropertyChanged();
            } }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
