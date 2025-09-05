using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeViewer.Network.Http
{
    public class StreamingStatus
    {
        public StreamingStatus()
        {

        }

        public StreamingState State { get; set; }
    }

    public enum StreamingState
    {
        None = 0,
        Request,
        Playing,
    }
}
