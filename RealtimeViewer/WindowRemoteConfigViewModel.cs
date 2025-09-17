using RealtimeViewer.Converter;
using RealtimeViewer.Network.Mqtt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealtimeViewer
{

    public class WindowRemoteConfigViewModel : INotifyPropertyChanged
    {
        // 取付方向用のSelectItem
        public readonly Dictionary<int, string> DeviceDirectionItems = new Dictionary<int, string>()
        {
            {1, "1: 平置き / 後退方向"},
            {2, "2: 平置き / 前進方向"},
            {3, "3: 平置き / 横向き"},
            {4, "4: 壁掛け / 上向き"},
            {5, "5: 壁掛け / 横向き"},
        };

        // Bitrate用のSelectItem
        public readonly Dictionary<string, string> BitRateItems = new Dictionary<string, string>()
        {
            {"20M", "20M"},
            {"15M", "15M"},
            {"10M", "10M"},
            {"5M", "5M"},
            {"2M", "2M"},
            {"1M", "1M"},
            {"512K", "512K"},
            {"384K", "384K"},
            {"256K", "256K"},
        };

        // パルス閾値用のSelectItem
        public readonly Dictionary<int, string> VoltageThresholdItems = new Dictionary<int, string>()
        {
            {4, "4"},
            {5, "5"},
            {6, "6"},
            {7, "7"},
            {8, "8"},
            {9, "9"},
            {10, "10"},
        };

        // パルス係数用のSelectItem
        public readonly Dictionary<int, string> PulseCoefficientItems = new Dictionary<int, string>()
        {
            {2, "2"},
            {4, "4"},
            {8, "8"},
            {16, "16"},
            {25, "25"},
        };

        // I/O用のSelectItem
        public readonly Dictionary<int, string> IoItems = new Dictionary<int, string>()
        {
            {0, "正論理" },
            {1, "負論理" }
        };

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BitrateList BrList { get; set; }


        private MqttJsonRemoteConfig _config;
        public MqttJsonRemoteConfig Config
        {
            get
            {
                return _config;
            }

            set
            {
                _config = value;
                NotifyPropertyChanged();
            }
        }

        public DeviceStatus DeviceStatus 
        {
            get; set;
        }

        public int OfficeId { get; set; }

        public UserBioDP.User AuthedUser { get; set; }

        public WindowRemoteConfigViewModel()
        {
            DeviceStatus = new DeviceStatus();
        }
    }

    public class DeviceStatus : MqttJsonError, INotifyPropertyChanged
    {
        public DeviceStatus() : base()
        {
        }

        public DeviceStatus(MqttJsonError mqttDeviceStatus) : this()
        {
            SetStatus(mqttDeviceStatus);
        }

        public void SetStatus(MqttJsonError mqttDeviceStatus)
        {
            if (mqttDeviceStatus != null)
            {
                DeviceId = mqttDeviceStatus.DeviceId;
                Ts = mqttDeviceStatus.Ts;
                Error = mqttDeviceStatus.Error;
                SdFree = mqttDeviceStatus.SdFree;
                SsdFree = mqttDeviceStatus.SsdFree;
                IccId = mqttDeviceStatus.IccId;
            }
        }

        public override string SdFree
        {
            get
            {
                return base.SdFree;
            }
            set
            {
                base.SdFree = value;
                NotifyPropertyChanged();
            }
        }

        public override string SsdFree
        {
            get
            {
                return base.SsdFree;
            }
            set
            {
                base.SsdFree = value;
                NotifyPropertyChanged();
            }
        }

        public override string IccId
        {
            get {
                return base.IccId;
            }
            set
            {
                base.IccId = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
