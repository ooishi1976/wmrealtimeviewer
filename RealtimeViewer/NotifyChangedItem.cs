using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealtimeViewer
{
    public class NotifyChangedItem<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private T _item;
        public T Item
        {
            get
            {
                return _item;
            }

            set
            {
                _item = value;
                NotifyPropertyChanged();
            }
        }
    }
}
