using RealtimeViewer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealtimeViewer
{
    public class MovieRequestWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public const int RANGE_MAX_MINUTES = 10;
        public MovieRequestWindowViewModel()
        {
            // 日付時刻の範囲として、10分前から現在時刻までとしておく。
            // 何か値を入れておきたい。
            _rangeEnd = DateTime.Now;
            _rangeStart = _rangeEnd.AddMinutes(-RANGE_MAX_MINUTES);
            _startHour = _rangeStart.Hour;
            _startMinute = _rangeStart.Minute;
            _endHour = _rangeEnd.Hour;
            _endMinute = _rangeEnd.Minute;

            _progressCurrentValue = 0;
            _progressMaxValue = RANGE_MAX_MINUTES;
            _progressText = string.Empty;
            _validateErrorText = string.Empty;

            // Downloadという名前だがサーバアップロードのステータスで使っている
            DownloadStatus = new Dictionary<string, DownloadStatus>();
            IsDisposing = false;
        }

        private DateTime _rangeStart;
        public DateTime RangeStart
        {
            get
            {
                return _rangeStart;
            }
            set
            {
                if (_rangeStart != value)
                {
                    _rangeStart = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime _rangeEnd;
        public DateTime RangeEnd
        {
            get
            {
                return _rangeEnd;
            }
            set
            {
                if (_rangeEnd != value)
                {
                    _rangeEnd = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _startHour;
        public int StartHour
        {
            get => _startHour;
            set
            {
                if (_startHour != value)
                {
                    _startHour = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _startMinute;
        public int StartMinute
        {
            get => _startMinute;
            set
            {
                if (_startMinute != value)
                {
                    _startMinute = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _endHour;
        public int EndHour
        {
            get => _endHour;
            set
            {
                if (_endHour != value)
                {
                    _endHour = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _endMinute;
        public int EndMinute
        {
            get => _endMinute;
            set
            {
                if (_endMinute != value)
                {
                    _endMinute = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DeviceInfo Device { get; set; }

        private double _progressCurrentValue;
        public double ProgressCurrentValue
        {
            get
            {
                return _progressCurrentValue;
            }

            set
            {
                if (Math.Abs(_progressCurrentValue - value) >= 0.01)
                {
                    _progressCurrentValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _progressMaxValue;
        public double ProgressMaxValue
        {
            get
            {
                return _progressMaxValue;
            }

            set
            {
                if (Math.Abs(_progressMaxValue - value) >= 0.01)
                {
                    _progressMaxValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _progressText = string.Empty;
        public string ProgressText
        {
            get
            { 
                return _progressText;
            }

            set
            {
                if (_progressText != value)
                {
                    _progressText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _validateErrorText;
        public string ValidateErrorText
        {
            get
            {
                return _validateErrorText;
            }

            set
            {
                if (_validateErrorText != value)
                {
                    _validateErrorText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isUploading;
        public bool IsUploading
        {
            get
            {
                return _isUploading;
            }

            set
            {
                if (_isUploading != value)
                {
                    _isUploading = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public EventInfo DownloadedEvent
        {
            get;
            set;
        }
        public Dictionary<string, DownloadStatus> DownloadStatus { get; set; }

        /// <summary>
        /// ウィンドウを破棄しようとしているか否か
        /// </summary>
        public bool IsDisposing 
        {
            get; set;
        }
    }

    public enum MovieUploadStatus
    {
        Started = 0,
        Sended,
        NotFound,

        ErrorGeneric = 10,
        ErrorBusy,
    }

    public class DownloadStatus
    {
        public int Total; // 全部の数量(期間:分と同じ意味)
        public int StoredCount; // サーバーに格納された個数
        public int SkipCount; // 車載器に無かった数
        public int ErrorCount; // エラーが発生した数
    }
}
