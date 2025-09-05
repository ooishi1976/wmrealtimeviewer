using System.Text;

namespace RealtimeViewer.Network.Http
{
    internal class StringContent
    {
        private string json;
        private Encoding uTF8;
        private string v;

        public StringContent(string json, Encoding uTF8, string v)
        {
            this.json = json;
            this.uTF8 = uTF8;
            this.v = v;
        }
    }
}