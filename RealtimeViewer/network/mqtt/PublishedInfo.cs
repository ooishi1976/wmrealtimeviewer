namespace RealtimeViewer.Network.Mqtt
{
    public class PublishedInfo
    {
        public delegate void InfoHandler(ushort MessageId, PublishedInfo info);

        public object Message { get; set; }
        public InfoHandler PublishedHandler;
    }
}
