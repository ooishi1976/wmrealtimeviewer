using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeViewer.WMShipView.Streaming
{

    public interface IStreamingSettings
    {
        /// <summary>
        /// ストリーミング配信要求リトライ回数
        /// (PCから車載器へMQTTで配信要求をする際のリトライ回数)
        /// </summary>
        int StreamingRequestRetryCount { get; set; }
        /// <summary>
        /// ストリーミングセッションリトライ回数
        /// (ストリーミングサーバーにファイルが置かれない場合のリトライ回数。
        ///  配信要求からやり直す)
        /// </summary>
        int StreamingSessionRetryCount { get; set; }
        /// <summary>
        /// ストリーミング配信要求待ち時間(秒)
        /// </summary>
        int StreamingRequestWait { get; set; }
        /// <summary>
        /// ストリーミングセッション待ち時間(秒)
        /// </summary>
        int StreamingSessionWait { get; set; }
    }
}
