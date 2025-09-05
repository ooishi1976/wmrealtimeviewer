using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeViewer.Network.Http
{
    [JsonObject]
    public class StreamingChannel
    {
        public int Channel { get; set; }
    }
}
