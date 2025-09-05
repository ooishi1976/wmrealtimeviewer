using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace RealtimeViewer.Network.Mqtt
{
    [JsonObject]
    public class MqttJsonStreamingRequest
    {
        public string device_id { get; set; }
        public int startstop { get; set; }
        public int channel { get; set; }
    }

    [JsonObject]
    public class MqttJsonStreamingStatus
    {
        public string device_id { get; set; }
        public string ts { get; set; }
        public int streamingStarted { get; set; }
        public int streamingChannel { get; set; }

        public long ucount { get; set; }
        public long tframes { get; set; }
        public long tbytes { get; set; }
        public double fps { get; set; }
        public double kbps { get; set; }
        public string elapsed { get; set; } // "HH:MM:SS"
        public long dropped { get; set; }
        public double speed { get; set; }
    }

    [JsonObject]
    public class MqttJsonEventAccOn
    {
        private enum AccStatus
        {
            ON = 1,
            OFF = 0
        };

        public string device_id { get; set; }
        public string ts { get; set; }
        public int on { get; set; }
        public bool IsAccOn()
        {
            return (on == (int)AccStatus.ON);
        }
    }

    [JsonObject]
    public class MqttJsonEventError
    {
        public string device_id { get; set; }
        public string ts { get; set; }
        public int error { get; set; }

        [DefaultValue("")]
        public string version { get; set; }
    }

    [JsonObject]
    public class MqttJsonError
    {
        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("sd_free")]
        public virtual string SdFree { get; set; }

        [JsonProperty("ssd_free")]
        public virtual string SsdFree { get; set; }

        [JsonProperty("iccid")]
        public virtual string IccId { get; set; }

        public int GetErrorCode()
        {
            int.TryParse(Error, out int result);
            return result;
        }
    }

    [JsonObject]
    public class MqttJsonLocation
    {
        [JsonProperty("device_id")]
        public string Device_id { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("lat")]
        public string Lat { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; }
    }

    [JsonObject]
    public class MqttJsonRemoteConfigCamera
    {
        public enum FlipMode
        {
            Normal = 0,  // 通常(flip: false, mirror: false)
            FlipHolizontal = 1,  // 水平反転(flip: false, mirror: true)
            FlipVertical = 2,  // 垂直反転(flip: true, mirror: false)
            FlipHolizontalVertical = 3  // 水平垂直反転(flip: true, mirror: true)
        };

        public int is_enabled { get; set; } // 0:Disabled, 1:Enabled
        public string bitrate { get; set; }
        public int framerate { get; set; }
        public string resolution { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(false)]
        public bool flip { get; set; }  // 垂直反転

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(null)]
        public bool? mirror { get; set; }  // 左右反転

        /// <summary>
        /// カメラのEnable設定.
        /// int <-> booleanの変換あり
        /// </summary>
        [JsonIgnore]
        public bool Enabled
        {
            get
            {
                return is_enabled == 1;
            }
            set
            {
                is_enabled = value ? 1 : 0;
            }
        }

        public FlipMode GetFlipMode(bool mirrorDefault = false)
        {
                FlipMode result;
                bool mirror = (this.mirror == null) ? mirrorDefault : this.mirror.Value;
                if (flip && mirror)
                {
                    result = FlipMode.FlipHolizontalVertical;
                }
                else if (flip && !mirror)
                {
                    result = FlipMode.FlipVertical;
                }
                else if (!flip && mirror)
                {
                    result = FlipMode.FlipHolizontal;
                }
                else
                {
                    result = FlipMode.Normal;
                }
                return result;
        }

        public void SetFlipMode(int value)
        {
            FlipMode flipMode = ParseFlipMode(value);
            SetFlipMode(flipMode);
        }

        public void SetFlipMode(FlipMode flipMode)
        {
            switch (flipMode)
            {
                case FlipMode.FlipHolizontalVertical:
                    flip = true;
                    mirror = true;
                    break;
                case FlipMode.FlipVertical:
                    flip = true;
                    mirror = false;
                    break;
                case FlipMode.FlipHolizontal:
                    flip = false;
                    mirror = true;
                    break;
                case FlipMode.Normal:
                default:
                    flip = false;
                    mirror = false;
                    break;
            }
        }

        private static FlipMode ParseFlipMode(int value)
        {
            FlipMode result;
            switch (value)
            {
                case 0:
                    result = FlipMode.Normal;
                    break;
                case 1:
                    result = FlipMode.FlipHolizontal;
                    break;
                case 2:
                    result = FlipMode.FlipVertical;
                    break;
                case 3:
                    result = FlipMode.FlipHolizontalVertical;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }

    /// <summary>
    /// MqttJsonRemoteConfigCamera.FlipModeの拡張メソッド
    /// </summary>
    public static class FlipModeExtentions
    {
        public static int Value(this MqttJsonRemoteConfigCamera.FlipMode flipMode)
        {
            return (int)flipMode;
        }
    }

    [JsonObject]
    public class MqttJsonRemoteConfig
    {
        /// <summary>
        /// I/O用の列挙子。 数値は配列の位置要素を指す。
        /// </summary>
        public enum IoSignalPolarities
        {
            BackGear = 0,
            Brake = 1,
            LeftWinker = 2,
            RightWinker = 3,
            FrontDoor = 4,
            RearDoor = 5,
            ParkingBrake = 6
        }

        /// <summary>
        /// 外部機器用の列挙子。 数値は配列の位置要素を指す。
        /// </summary>
        public enum SerialDevices
        {
            NamePlate = 0,
            Etc = 1,
            Printer = 2,
            StatusSwitch = 3
        }

        /// <summary>
        /// パルス検出方式
        /// </summary>
        public enum PulseDitectType
        {
            CURRENT = 1,
            VOLTAGE = 0
        }

        public string device_id { get; set; }
        public MqttJsonRemoteConfigCamera[] cameras { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(5)]
        public int event_pre_sec { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(5)]
        public int event_post_sec { get; set; }

        /// <summary>
        /// Gセンサー閾値 前方 = 加速G = 急発進
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0.8)]
        public double gsensor_forward { get; set; }

        /// <summary>
        /// Gセンサー閾値 後方 = 減速G = 急ブレーキ
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0.8)]
        public double gsensor_backward { get; set; }

        /// <summary>
        /// Gセンサー閾値 左右G
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0.8)]
        public double gsensor_horizontal { get; set; }

        /// <summary>
        /// Gセンサー閾値 上下G
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0.8)]
        public double gsensor_vertical { get; set; }

        /// <summary>
        /// 設置方向 
        /// /// 1:平置き・後退<br>
        /// 2:平置き・進行<br>
        /// 3:平置き・ドア<br>
        /// 4:縦置き・天井<br>
        /// 5:縦置き・ドア<br>
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public int gsensor_direction { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int speed_level { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(8)]
        public int speed_num_of_gear { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(54)]
        public int speed_threshold { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int tach_level { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1500)]
        public int tach_num_of_gear { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(54)]
        public int tach_threshold { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("256K")]
        public string streaming_bitrate { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(15)]
        public int streaming_framerate { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int speaker_volume { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int odometer_km { get; set; }

        public int[] io_signal_polarities { get; set; }
        public int[] has_serial_devices { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(60)]
        public int speed_normal_load_limit { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int speed_normal_load_decision_time { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(100)]
        public int speed_highway_limit { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int speed_highway_decision_time { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(3000)]
        public int tacho_normal_load_limit { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int tacho_normal_load_decision_time { get; set; }

        public DateTime update_at { get; set; }

        /// <summary>
        /// パルス閾値用のDict.
        /// key: 電圧
        /// value: 閾値
        /// </summary>
        private static readonly Dictionary<int, int> pulseVoltageThreshold = new Dictionary<int, int> {
            { 4, 46 },
            { 5, 54 },
            { 6, 70 },
            { 7, 77 },
            { 8, 89 },
            { 9, 100 },
            { 10, 108 }
        };

        /// <summary>
        /// パルス電圧の取得。
        /// パルス閾値から4～10のパルス電圧を取得する。
        /// </summary>
        /// <param name="threshold">パルス閾値</param>
        /// <returns>パルス電圧</returns>
        private int GetPulseVoltage(int threshold)
        {
            int result = 10;
            foreach (var item in pulseVoltageThreshold)
            {
                if (threshold <= item.Value)
                {
                    result = item.Key;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 車速パルス設定(閾値 <-> パルス電圧の変換あり)
        /// </summary>
        [JsonIgnore]
        public int SpeedPulseThreshold
        {
            get
            {
                return GetPulseVoltage(speed_threshold);
            }
            set
            {
                speed_threshold = pulseVoltageThreshold[value];
            }
        }

        /// <summary>
        /// タコパルス設定(閾値 <-> パルス電圧の変換あり)
        /// </summary>
        [JsonIgnore]
        public int TachPalseThreshold
        {
            get
            {
                return GetPulseVoltage(tach_threshold);
            }
            set
            {
                tach_threshold = pulseVoltageThreshold[value];
            }
        }

        /// <summary>
        /// 速度パルス検出方式
        /// </summary>
        public PulseDitectType SpeedPulseDitectType
        {
            get
            {
                return speed_level == 1 ? PulseDitectType.CURRENT : PulseDitectType.VOLTAGE;
            }
            set
            {
                speed_level = (int)value;
            }
        }

        /// <summary>
        /// タコパルス検出方式
        /// </summary>
        public PulseDitectType TachPulseDitectType
        {
            get
            {
                return tach_level == 1 ? PulseDitectType.CURRENT : PulseDitectType.VOLTAGE;
            }
            set
            {
                tach_level = (int)value;
            }
        }

        /// <summary>
        /// I/Oポートの値を取得する
        /// </summary>
        /// <param name="ioSignal">取得対象のI/Oポート</param>
        /// <returns>値</returns>
        public int GetIoSignalPolarity(IoSignalPolarities ioSignal)
        {
            return io_signal_polarities[(int)ioSignal];
        }

        /// <summary>
        /// I/Oポートの値を設定する
        /// </summary>
        /// <param name="ioSignal">設定対象のI/Oポート</param>
        /// <param name="value">値</param>
        public void SetIoSignalPolarity(IoSignalPolarities ioSignal, int value)
        {
            io_signal_polarities[(int)ioSignal] = value;
        }

        /// <summary>
        /// 外部機器の実装有無を取得する
        /// </summary>
        /// <param name="serialDevice"></param>
        /// <returns>実装あり/なし</returns>
        public bool HasSerialDevice(SerialDevices serialDevice)
        {
            return has_serial_devices[(int)serialDevice] == 1;
        }

        /// <summary>
        /// 外部機器の実装有無を取得する
        /// </summary>
        /// <param name="serialDevice">設定対象の外部機器</param>
        /// <param name="value">true: 実装あり false: 実装なし</param>
        public void SetSerialDevice(SerialDevices serialDevice, bool value)
        {
            has_serial_devices[(int)serialDevice] = value ? 1 : 0;
        }
    }

    [JsonObject]
    public class MqttJsonRequestMovieDownload
    {
        public string device_id { get; set; }
        public int command { get; set; }
        public string req_id { get; set; }
        public string ts_start { get; set; }
        public string ts_end { get; set; }
    }

    /// <summary>
    /// 車載器からサーバーへプリポスト映像を送る/送ったステータス通知。
    /// SSDの1分ファイルも含む。
    /// </summary>
    [JsonObject]
    public class MqttJsonPrepostEvent
    {
        public string device_id { get; set; }
        public DateTime ts { get; set; }
        public int movie_type { get; set; }
        public string g { get; set; }
        public string req_id { get; set; }
        public int status { get; set; }
        public int total { get; set; }
        public int n { get; set; }
    }
}
