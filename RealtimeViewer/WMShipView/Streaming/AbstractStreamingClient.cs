using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;
using RealtimeViewer.Movie;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network.Mqtt;
using RealtimeViewer.WMShipView.Streaming;

namespace RealtimeViewer.WMShipView
{
    using TTimer = System.Timers.Timer;

    public class CounterEventArgs : EventArgs
    {
        public TimeSpan Count { get; private set; }

        public CounterEventArgs(TimeSpan count)
        {
            Count = count;
        }
    }

    public delegate void CounterEventHandler(object sender, CounterEventArgs e);

    public abstract class AbstractStreamingClient : IStreamingClient
    {
        public StreamingStatuses Status { get; set; } = StreamingStatuses.None;

        public MqttJsonStreamingStatus LastStreamingStatus { get; set; } = new MqttJsonStreamingStatus();

        public string DeviceId { get; private set; } = string.Empty;

        public int Channel { get; private set; } = 0;

        public int SessionRetryCount { get; private set; } = 0;

        public int RequestCount { get; private set; } = 0;

        //public long PlayTime { get; private set; } = 0;
        protected Stopwatch PlayCounter { get; private set; }

        protected Stopwatch SessionWaitCounter { get; private set; } 

        protected MqttController MqttController { get; set; }

        protected string VlcSkinPath { get; set; } 

        protected Process VlcProcess { get; set; }

        protected TTimer PlayCountTimer { get; set; } 

        protected TTimer SessionWaitTimer { get; set; }

        protected StreamingServerInfo ServerInfo { get; set; }

        protected List<CounterEventHandler> counterHandler = new List<CounterEventHandler>(); 

        protected event CounterEventHandler PlayCounted;

        protected List<EventHandler> closedHandler = new List<EventHandler>(); 

        protected event EventHandler Closed;

        protected List<CounterEventHandler> sessionWaitHandler = new List<CounterEventHandler>(); 

        protected event CounterEventHandler WaitCounted;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mqttController"></param>
        /// <param name="deviceId"></param>
        public AbstractStreamingClient(
            MqttController mqttController, 
            string deviceId) 
        { 
            MqttController = mqttController;
            DeviceId = deviceId;
            VlcSkinPath = Path.Combine(Application.StartupPath, @"vlc", @"skins", @"isdt-nc.vlt");
        }

        public void AddCounterHandler(CounterEventHandler handler)
        {
            if (!counterHandler.Contains(handler))
            {
                counterHandler.Add(handler);
                PlayCounted += handler;
            }
        }

        public void RemoveCounterHandler(CounterEventHandler handler)
        {
            if (counterHandler.Contains(handler))
            {
                counterHandler.Remove(handler);
                PlayCounted -= handler;
            }
        }

        public void AddClosedHandler(EventHandler handler)
        {
            if (!closedHandler.Contains(handler))
            {
                closedHandler.Add(handler);
                Closed += handler;
            }
        }

        public void RemoveClosedHandler(EventHandler handler)
        {
            if (closedHandler.Contains(handler))
            {
                closedHandler.Remove(handler);
                Closed -= handler;
            }
        }

        public void AddSessionWaitHandler(CounterEventHandler handler)
        {
            if (!sessionWaitHandler.Contains(handler))
            {
                sessionWaitHandler.Add(handler);
                WaitCounted += handler;
            }
        }

        public void RemoveSessionWaitHandler(CounterEventHandler handler)
        {
            if (sessionWaitHandler.Contains(handler))
            {
                sessionWaitHandler.Remove(handler);
                WaitCounted -= handler;
            }
        }

        public void RequestStartStreaming()
        {
            SendStartTopic(true);
            if (SessionWaitCounter != null)
            {
                SessionWaitTimer.Stop();
            }
            SessionWaitCounter = Stopwatch.StartNew();

            if (SessionWaitTimer != null)
            {
                SessionWaitTimer.Stop();
                SessionWaitTimer.Dispose();
            }
            SessionWaitTimer = new TTimer(1000);
            SessionWaitTimer.Elapsed += SessionWaitTimer_Elapsed;
            SessionWaitTimer.Start();
            Status = StreamingStatuses.Request;
            RequestCount++;
        }

        public void RequestStopStreaming()
        {
            SendStopTopic();
            Status = StreamingStatuses.None;
            PlayCounter?.Stop();
        }

        public void RequestChangeChannel(int channel)
        {
            Channel = channel;
            SendStartTopic(true);
        }

        public void Play(StreamingServerInfo serverInfo)
        {
            SessionWaitCounter?.Stop();
            SessionWaitTimer?.Stop();

            ServerInfo = serverInfo;
            const int width = 720;
            const int height = 480;
            int x = (1920 - width) / 2;
            int y = (1080 - height) / 2;

            string[] options =
            {
                    @"--no-qt-video-autoresize", @"--no-video-title-show", @"--no-qt-bgcone",
                    @"--no-qt-updates-notif", @"--video-on-top",
                    $"--video-x={x}",$"--video-y={y}", $"--width={width}", $"--height={height}",
                    $"--skins2-last=\"{VlcSkinPath}\"",
                    $" {GetDistributeUrl(serverInfo)}",
            };

            //var title = $"社番: {DeviceId}";
            VlcProcess = new Process();
            VlcProcess.StartInfo.UseShellExecute = false;
            VlcProcess.StartInfo.CreateNoWindow = true;
            VlcProcess.StartInfo.FileName = @"vlc\vlc.exe";
            VlcProcess.StartInfo.Arguments = string.Join(@" ", options);
            VlcProcess.Exited += VlcProcess_Exited;
            VlcProcess.EnableRaisingEvents = true;
            VlcProcess.Start();
            
            PlayCounter = Stopwatch.StartNew();
            PlayCountTimer = new TTimer(1000);
            PlayCountTimer.Elapsed += PlayCountTimer_Elapsed;
            PlayCountTimer.Start();
            Status = StreamingStatuses.Playing;
        }

        public ClientStatus GetClientStatus()
        {
            var result = new ClientStatus(null);
            result.SetValues(LastStreamingStatus);
            result.Status = Status;
            result.RequestCount = RequestCount;
            result.PlayCounter = (PlayCounter is null) ? string.Empty : $@"{PlayCounter.Elapsed:hh\:mm\:ss}";
            result.WaitCounter = (SessionWaitCounter is null) ? string.Empty : $@"{SessionWaitCounter.Elapsed:mm\:ss}";
            return result;
        }

        private void PlayCountTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PlayCounted.Invoke(this, new CounterEventArgs(PlayCounter.Elapsed));
        }

        private void VlcProcess_Exited(object sender, EventArgs e)
        {
            Closed?.Invoke(this, new EventArgs());
        }

        private void SessionWaitTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WaitCounted?.Invoke(this, new CounterEventArgs(SessionWaitCounter.Elapsed));
        }

        public void Stop()
        {
            Stop(StreamingStatuses.None);
        }

        public void Stop(StreamingStatuses status)
        {
            try
            {
                if (VlcProcess != null && !VlcProcess.HasExited)
                {
                    VlcProcess.EnableRaisingEvents = false;
                    VlcProcess.Kill();
                    //VlcProcess.WaitForExit(5000);

                    VlcProcess.Dispose();
                    VlcProcess = null;
                }
            }
            catch (InvalidOperationException)
            {
                // プロセスが起動失敗している時に、プロパティを触ると例外が発生する
                VlcProcess.Dispose();
                VlcProcess = null;
            }
            Status = status;
            PlayCountTimer?.Stop();
            PlayCountTimer?.Dispose();
            PlayCounter?.Stop();

            SessionWaitTimer?.Stop();
            SessionWaitTimer?.Dispose();
            SessionWaitCounter?.Stop();
        }


        protected abstract string GetDistributeUrl(StreamingServerInfo serverInfo);

        private void SendStartTopic(bool isStart)
        {
            var topic = $"car/streaming/{DeviceId}/request";

            var request = new MqttJsonStreamingRequest()
            {
                device_id = DeviceId,
                startstop = isStart ? 1 : 0,
                channel = Channel,
            };
            var json = JsonConvert.SerializeObject(request);
            var message = Encoding.UTF8.GetBytes(json);
            MqttController.SendMessage(topic, message);
        }

        private void SendStopTopic()
        {
            var topic = $"car/streaming/{DeviceId}/request";
            var request = new MqttJsonStreamingRequest()
            {
                device_id = DeviceId,
                startstop = 2,
                channel = 0,
            };
            var json = JsonConvert.SerializeObject(request);
            var message = Encoding.UTF8.GetBytes(json);
            MqttController.SendMessage(topic, message);
        }

    }
}
