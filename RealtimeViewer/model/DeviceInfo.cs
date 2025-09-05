using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealtimeViewer.Model
{
    public class DeviceInfo : INotifyPropertyChanged
    {
        public DeviceInfo()
        {
            is_alive = false;
        }

        public string DeviceId { get; set; }
        public string CarId { get; set; }
        public int OfficeId { get; set; }

        private string status = string.Empty;
        private string error_text = string.Empty;

        private bool is_alive = false;

        // INotifyPropertyChanged を実装しておくと、オブザーバーに変更通知が行く。
        // このアプリでは、オブザーバー(バインド先)はバス一覧のDataGridであり、それが届くと画面が更新される。
        // see https://docs.microsoft.com/ja-jp/dotnet/api/system.componentmodel.inotifypropertychanged?redirectedfrom=MSDN&view=netframework-4.7.2
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                if (status != value)
                {
                    status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ErrorText
        {
            get
            {
                return error_text;
            }

            set
            {
                if (error_text != value)
                {
                    error_text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsAlive
        {
            get
            {
                return is_alive;
            }

            set
            {
                if (is_alive != value)
                {
                    is_alive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isMovieDownloading;
        public bool IsMovieDownloading
        {
            get
            {
                return _isMovieDownloading;
            }

            set
            {
                if (_isMovieDownloading != value)
                {
                    _isMovieDownloading = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _version = string.Empty;
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
