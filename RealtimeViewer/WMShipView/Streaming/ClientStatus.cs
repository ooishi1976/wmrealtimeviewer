using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using RealtimeViewer.Network.Mqtt;

namespace RealtimeViewer.WMShipView.Streaming
{
    public class ClientStatus : BindableModel
    {
        private string deviceId = string.Empty;
        public string DeviceId 
        { 
            get => deviceId; 
            set => SetProperty(ref deviceId, value); 
        }

        #region FFMPEG Status
        private string timestamp = string.Empty;
        public string Timestamp 
        { 
            get => timestamp; 
            set => SetProperty(ref timestamp, value); 
        }

        private bool isStarted = false;
        public bool IsStarted 
        { 
            get => isStarted; 
            set => SetProperty(ref isStarted, value); 
        }

        private int channel = 0;
        public int Channel 
        { 
            get => channel; 
            set => SetProperty(ref channel, value); 
        }

        private long count = 0L;
        public long Count 
        { 
            get => count; 
            set => SetProperty(ref count, value); 
        }

        private long frames = 0L;
        public long Frames 
        { 
            get => frames; 
            set => SetProperty(ref frames, value); 
        }

        private long bytes = 0L;
        public long Bytes 
        { 
            get => bytes; 
            set => SetProperty(ref bytes, value); 
        }

        private double fps = 0d;
        public double Fps 
        { 
            get => fps; 
            set => SetProperty(ref fps, value); 
        }

        private double kbps = 0d;
        public double Kbps 
        { 
            get => kbps; 
            set => SetProperty(ref kbps, value); 
        }

        private string elapsed = string.Empty;
        public string Elapsed // "HH:MM:SS"
        { 
            get => elapsed; 
            set => SetProperty(ref elapsed, value); 
        } 

        private long dropped = 0L;
        public long Dropped 
        { 
            get => dropped; 
            set => SetProperty(ref dropped, value); 
        }

        private double speed = 0d;
        public double Speed 
        { 
            get => speed; 
            set => SetProperty(ref speed, value); 
        }
        #endregion

        #region Application Status
        private StreamingStatuses status = StreamingStatuses.None;
        public StreamingStatuses Status
        {
            get => status;
            set 
            {
                SetProperty(ref status, value);
                NotifyPropertyChanged(nameof(IsWaitToReady));
                NotifyPropertyChanged(nameof(IsWaitOrPlaying));
                NotifyPropertyChanged(nameof(IsDisplayMessage));
                NotifyPropertyChanged(nameof(StatusText1));
                NotifyPropertyChanged(nameof(StatusText2));
            }
        }

        /// <summary>
        /// リクエスト～再生待ち(プログレスバー)
        /// </summary>
        public bool IsWaitToReady => (Status == StreamingStatuses.WaitToPlay || Status == StreamingStatuses.Request);

        /// <summary>
        /// 再生待ち～再生中(通信状態)
        /// </summary>
        public bool IsWaitOrPlaying => (Status == StreamingStatuses.WaitToPlay || Status == StreamingStatuses.Playing);

        /// <summary>
        /// ストリーミングメッセージを見せるか
        /// </summary>
        public bool IsDisplayMessage => (Status != StreamingStatuses.None);

        /// <summary>
        /// 視聴中
        /// </summary>
        public bool IsPlaying => (Status == StreamingStatuses.Playing);

        private string playCounter = string.Empty;
        /// <summary>
        /// 再生中カウンター
        /// </summary>
        public string PlayCounter
        {
            get => playCounter;
            set => SetProperty(ref playCounter, value);
        }

        private string waitCounter = string.Empty;
        /// <summary>
        /// 再生待ちカウンター
        /// </summary>
        public string WaitCounter
        {
            get => waitCounter;
            set 
            {
                SetProperty(ref waitCounter, value);
                NotifyPropertyChanged(nameof(StatusText1));
                NotifyPropertyChanged(nameof(StatusText2));
            }
        }

        public string StatusText1
        {
            get
            {
                var result = string.Empty;
                if (status == StreamingStatuses.Request || status == StreamingStatuses.WaitToPlay)
                {
                    if (string.IsNullOrEmpty(WaitCounter))
                    {
                        result = $"配信情報取得中";
                    }
                    else
                    {
                        result = $"配信情報取得中({WaitCounter})";
                    }
                }
                else if (status == StreamingStatuses.Playing)
                {
                    result = "再生中";
                }
                else if (status == StreamingStatuses.PlayerError) 
                {
                    result = "[異常]再生できません";
                }
                else if (status == StreamingStatuses.RequestFailure || 
                         status == StreamingStatuses.PlayingFailure)
                {
                    result = "通信状況が悪い為、";
                }
                return result;
            }
        }

        public string StatusText2
        {
            get
            {
                var result = string.Empty;
                if (status == StreamingStatuses.Request || status == StreamingStatuses.WaitToPlay)
                {
                    var retryCount = (requestCount - 1);
                    if (0 < retryCount)
                    {
                        result = $"配信要求リトライ({retryCount})";
                    }
                }
                else if (status == StreamingStatuses.RequestFailure ||
                         status == StreamingStatuses.PlayingFailure)
                {
                    result = "再生できません";
                }
                return result;
            }
        }

        private int requestCount = 0;
        public int RequestCount
        {
            get => requestCount;
            set 
            {
                SetProperty(ref requestCount, value);
                NotifyPropertyChanged(nameof(StatusText1));
                NotifyPropertyChanged(nameof(StatusText2));
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dispatcher"></param>
        public ClientStatus(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        public void SetValues(MqttJsonStreamingStatus jsonStreamingStatus)
        {
            DeviceId = jsonStreamingStatus.device_id;
            Timestamp = jsonStreamingStatus.ts;
            IsStarted = (jsonStreamingStatus.streamingStarted == 1) ? true : false;
            Channel = jsonStreamingStatus.streamingChannel;

            Count = jsonStreamingStatus.ucount;
            Frames = jsonStreamingStatus.tframes;
            Bytes = jsonStreamingStatus.tbytes;
            Fps = jsonStreamingStatus.fps;
            Kbps = jsonStreamingStatus.kbps;
            Elapsed = jsonStreamingStatus.elapsed;  // "HH:MM:SS"
            Dropped = jsonStreamingStatus.dropped;
            Speed = jsonStreamingStatus.speed;
        }

        public void SetValues(ClientStatus status) 
        {
            Status = status.Status;
            PlayCounter = status.PlayCounter;
            WaitCounter = status.WaitCounter;
            RequestCount = status.RequestCount;

            DeviceId = status.DeviceId;
            Timestamp = status.Timestamp;
            IsStarted = status.IsStarted;
            Channel = status.Channel;

            Count = status.Count;
            Frames = status.Frames;
            Bytes = status.Bytes;
            Fps = status.Fps;
            Kbps = status.Kbps;
            Elapsed = status.Elapsed;
            Dropped = status.Dropped;
            Speed = status.Speed;

        }
    }
}
