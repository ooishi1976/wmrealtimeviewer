using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RealtimeViewer.Model
{
    /// <summary>
    /// 営業所ViewModel
    /// </summary>
    public class OfficeInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public OfficeInfo()
        {
            _Name = string.Empty;
        }

        public OfficeInfo(OfficeInfo src)
        {
            Name = src.Name;
            Id = src.Id;
            CompanyId = src.CompanyId;
            Visible = src.Visible;
            if (src.Location != null)
                Location = (src.Location.Value.latitude, src.Location.Value.longitude);            
        }

        private int _Id;
        public int Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _CompanyId;
        public int CompanyId
        {
            get { return _CompanyId; }
            set
            {
                if (_CompanyId != value)
                {
                    _CompanyId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public (int latitude, int longitude)? Location { get; set; }
        public bool Visible { get; set; }
    }
}
