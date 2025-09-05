using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeViewer.Converter
{
    public class BitrateList
    {
        private Bitrate[] bitrates = new Bitrate[]
        {
            new Bitrate() { Id = 0, Name = "20M" },
            new Bitrate() { Id = 1, Name = "15M" },
            new Bitrate() { Id = 2, Name = "10M" },
            new Bitrate() { Id = 3, Name = "5M" },
            new Bitrate() { Id = 4, Name = "2M" },
            new Bitrate() { Id = 5, Name = "1M" },
            new Bitrate() { Id = 6, Name = "512K" },
            new Bitrate() { Id = 7, Name = "384K" },
            new Bitrate() { Id = 8, Name = "256K" },
        };

        private Dictionary<string, int> bitrateTextToId = new Dictionary<string, int>();

        public BitrateList()
        {
            foreach (var b in bitrates)
            {
                bitrateTextToId.Add(b.Name, b.Id);
            }
        }

        public Bitrate[] Bitrates
        {
            get
            {
                return bitrates;
            }
        }

        public Dictionary<string, int> BitrateTextToId
        {
            get
            {
                return bitrateTextToId;
            }
        }
    }
}
