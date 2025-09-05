using RealtimeViewer.Network.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeViewer.Model
{
    public class EventInfo : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public const int AUDIO_CHANNEL_BASE = 16;

        public string DeviceId { get; set; }
        public string CarId { get; set; }
        public DateTime Timestamp { get; set; }
        public int MovieId { get; set; }
        public Task taskDownload { get; set; }
        public string unzippedPath { get; set; }
        public Dictionary<int, string> ChannelToMovieFileDic { get; set; }
        //public MovieType MovieType { get; set; }
        public string MovieType { get; set; }
        public GravityRecord GravityRecord { get; set; }

        private bool isDownloadable;
        private bool isDownloading;
        private bool isPlayable;
        private string remarks = string.Empty;

        public EventInfo()
        {
            isDownloadable = true;
        }

        public void Dispose()
        {
        }

        public bool IsDownloadable
        {
            get
            {
                return isDownloadable;
            }

            set
            {
                if (isDownloadable != value)
                {
                    isDownloadable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsDownloading
        {
            get
            {
                return isDownloading;
            }

            set
            {
                if (isDownloading != value)
                {
                    isDownloading = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsPlayable
        {
            get
            {
                return isPlayable;
            }

            set
            {
                if (isPlayable != value)
                {
                    isPlayable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Remarks
        {
            get
            {
                return remarks;
            }

            set
            {
                if (remarks != value)
                {
                    remarks = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string selected;
        public string Selected 
        {
            get {
                return selected;
            }
            set
            {
                selected = value;
                NotifyPropertyChanged();
            } 
        }
    }

    public class EventInfoUtil
    {
        /// <summary>
        /// 映像検索結果を日付で並び替える。新しいリストを作らず、もとのリストを並べ替える。
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="isDescending">trueなら日付の降順(新しい順)に並べる。falseなら昇順。</param>
        public static void SortMovieResults(List<JsonMovie> movies, bool isDescending)
        {
            movies.Sort(delegate (JsonMovie x, JsonMovie y)
            {
                if (x.ts_start == y.ts_start)
                {
                    if (x.device_id == y.device_id)
                    {
                        if (x.camera_id < y.camera_id)
                        {
                            return -1;
                        }
                        else if (x.camera_id > y.camera_id)
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (x.device_id < y.device_id)
                        {
                            return -1;
                        }
                        else if (x.device_id > y.device_id)
                        {
                            return 1;
                        }
                    }
                    return 0;
                }
                else if (x.ts_start < y.ts_start)
                {
                    return isDescending ? 1 : -1;
                }
                else
                {
                    return isDescending ? -1 : 1;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="movies"></param>
        /// <returns></returns>
        public static List<JsonMovie> UniqueMovieResults(List<JsonMovie> movies)
        {
            var newList = new List<JsonMovie>();

            var lastDt = new DateTime(2000, 1, 1);
            var lastSeq = string.Empty;

            foreach (var m in movies)
            {
                if (lastDt != m.ts_start || lastSeq != m.sequence)
                {
                    lastDt = m.ts_start;
                    lastSeq = m.sequence;
                    newList.Add(m);
                }
            }

            return newList;
        }

        public static Dictionary<int, string> SearchMovieFiles(string path)
        {
            // For example: event_movie_id_23_camera_id_0.mpg

            var regex = new Regex(@"event_movie_id_(?<MID>\d+)_camera_id_(?<CH>\d+)\.mpg");
            var dic = new Dictionary<int, string>();

            try
            {
                var en = Directory.EnumerateFiles(path, @"*.mpg", SearchOption.TopDirectoryOnly);
                foreach (var f in en)
                {
                    var match = regex.Match(f);
                    if (match.Success)
                    {
                        var mid = match.Groups["MID"].Value;
                        var ch = match.Groups["CH"].Value;

                        Debug.WriteLine($"SearchMovieFiles: {f}, {mid}, {ch}");
                        dic.Add(int.Parse(ch), f);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Error: SearchMovieFiles {ex.ToString()}, {ex.Message}");
            }

            var audioRegex = new Regex(@"event_movie_id_(?<MID>\d+)_camera_id_(?<CH>\d+)\.aac");

            try
            {
                var en = Directory.EnumerateFiles(path, @"*.aac", SearchOption.TopDirectoryOnly);
                foreach (var f in en)
                {
                    var match = audioRegex.Match(f);
                    if (match.Success)
                    {
                        var mid = match.Groups["MID"].Value;
                        var ch = match.Groups["CH"].Value;

                        Debug.WriteLine($"SearchMovieFiles: {f}, {mid}, {ch}");
                        dic.Add(int.Parse(ch), f);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Error: SearchMovieFiles {ex.ToString()}, {ex.Message}");
            }
            return dic;
        }
    }
    /// <summary>
    /// イベント情報のコンパレータ
    /// </summary>
    class EventInfoComparer : IComparer<EventInfo>, IComparer
    {
        public enum SortMode
        {
            TimeStamp,
            DeviceId,
            EventType,
            Remark
        }

        private SortMode mode = SortMode.TimeStamp;
        private SortOrder order = SortOrder.Ascending;

        public SortMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public SortOrder Order
        {
            get { return order; }
            set { order = value; }
        }

        public int Compare(EventInfo x, EventInfo y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");

            int orderValue = (order == SortOrder.Ascending) ? 1 : -1;

            int result;
            switch (mode)
            {
                case SortMode.TimeStamp:  // Time -> CarId -> EventType -> Remark -> MovieId
                    result = x.Timestamp.CompareTo(y.Timestamp) * orderValue;
                    if (result == 0)
                    {
                        result = x.CarId.CompareTo(y.CarId);
                    }
                    if (result == 0)
                    {
                        result = x.MovieType.CompareTo(y.MovieType);
                    }
                    if (result == 0)
                    {
                        result = x.Remarks.CompareTo(y.Remarks);
                    }
                    if (result == 0)
                    {
                        result = x.MovieId.CompareTo(y.MovieId);
                    }
                    break;
                case SortMode.DeviceId:  // CarId -> Time(Desc) -> EventType -> Remark -> MovieId
                    result = x.CarId.CompareTo(y.CarId) * orderValue;
                    if (result == 0)
                    {
                        result = x.Timestamp.CompareTo(y.Timestamp) * -1;
                    }
                    if (result == 0)
                    {
                        result = x.MovieType.CompareTo(y.MovieType);
                    }
                    if (result == 0)
                    {
                        result = x.Remarks.CompareTo(y.Remarks);
                    }
                    if (result == 0)
                    {
                        result = x.MovieId.CompareTo(y.MovieId);
                    }
                    break;
                case SortMode.EventType:  // EventType -> CarId -> Time(Desc) -> Remark -> MovieId
                    result = x.MovieType.CompareTo(y.MovieType) * orderValue;
                    if (result == 0)
                    {
                        result = x.CarId.CompareTo(y.CarId);
                    }
                    if (result == 0)
                    {
                        result = x.Timestamp.CompareTo(y.Timestamp) * -1;
                    }
                    if (result == 0)
                    {
                        result = x.Remarks.CompareTo(y.Remarks);
                    }
                    if (result == 0)
                    {
                        result = x.MovieId.CompareTo(y.MovieId);
                    }
                    break;
                case SortMode.Remark:  // Remark -> CarId -> Time(Desc) -> EventType -> MovieId
                    // Gセンサーの情報(X: Y: Z:)がCompareToだと
                    // 不思議な並び替えになるので、意図的に文字コード順を指定している
                    result = string.Compare(x.Remarks, y.Remarks, StringComparison.Ordinal) * orderValue;
                    if (result == 0)
                    {
                        result = x.CarId.CompareTo(y.CarId);
                    }
                    if (result == 0)
                    {
                        result = x.Timestamp.CompareTo(y.Timestamp) * -1;
                    }
                    if (result == 0)
                    {
                        result = x.MovieType.CompareTo(y.MovieType);
                    }
                    if (result == 0)
                    {
                        result = x.MovieId.CompareTo(y.MovieId);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid SortMode.");
            }
            return result;
        }

        public int Compare(object x, object y)
        {
            // 引数がnullの場合はArgumentNullExceptionをスローする
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");
            if (x is EventInfo xE && y is EventInfo yE)
            {
                return Compare(xE, yE);
            }
            else
            {
                throw new InvalidOperationException("Target is invalid Target.");
            }
        }
    }
}
