using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RealtimeViewer.WMShipView
{
    public class BindableModel: INotifyPropertyChanged
    {
        public Dispatcher Dispatcher { get; set; }

        /// <summary>
        /// 変更通知イベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 変更通知イベント
        /// </summary>
        /// <param name="propertyName">変更プロパティ名</param>
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            Dispatcher?.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] String propertyName = "")
        {
            field = value;
            NotifyPropertyChanged(propertyName);
        }
    }
}
