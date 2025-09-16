using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RealtimeViewer.WMShipView.Streaming
{
    public interface IStreamingController
    {
        /// <summary>
        /// 開始中？
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        bool IsStarted(string deviceId);

        /// <summary>
        /// 開始
        /// </summary>
        /// <param name="deviceId"></param>
        void Start(string deviceId);

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="deviceId"></param>
        void Stop(string deviceId);

        /// <summary>
        /// チャンネル変更
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channel"></param>
        void ChangeChannel(string deviceId, int channel);

        void NotifyStatus(string deviceId);

        bool CanChangeChannel(string deviceId);

        /// <summary>
        /// 参照しているデバイスID
        /// </summary>
        string CurrentDeviceId { get; set; }

        /// <summary>
        /// 参照しているデバイスの配信情報
        /// </summary>
        ClientStatus ClientStatus { get; }
    }
}
