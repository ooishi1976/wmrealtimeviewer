using System.Text.RegularExpressions;

namespace RealtimeViewer.Network.Mqtt
{
    class TopicRegex
    {
        public Regex Regex { get; set; }
        public TopicLabel Label { get; set; }
    }
}
