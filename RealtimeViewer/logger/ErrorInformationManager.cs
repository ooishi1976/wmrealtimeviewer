using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealtimeViewer.Network.Mqtt;
using RealtimeViewer.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace RealtimeViewer.Logger
{
    /// <summary>
    /// エラー情報表示管理
    /// 実装したものの未使用。
    /// </summary>
    public class ErrorInformationManager : INotifyPropertyChanged
    {
        /// <summary>
        /// エラー/警告情報 リスト
        /// </summary>
        private Dictionary<string, MqttJsonError> errors;
        /// <summary>
        /// UI通知用ディスパッチャー
        /// </summary>
        private Dispatcher dispatcher;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dispatcher">UI通知用のディスパッチャー</param>
        public ErrorInformationManager(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            errors = new Dictionary<string, MqttJsonError>();
            hasError = false;
            hasWarn = false;
        }

        /// <summary>
        /// エラー/警告情報リストにエラーを含んでいるか
        /// </summary>
        private bool hasError;
        public bool HasError
        {
            get
            {
                return hasError;
            }
            set
            {
                hasError = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// エラー/警告情報リストに警告を含んでいるか
        /// </summary>
        private bool hasWarn;
        public bool HasWarn
        {
            get
            {
                return hasWarn;
            }
            set
            {
                hasWarn = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 変更通知イベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 変更通知イベント
        /// </summary>
        /// <param name="propertyName">変更プロパティ名</param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            dispatcher?.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        /// <summary>
        /// エラー/警告情報の登録。
        /// エラー/警告情報リストにエラー/警告データが存在する場合に
        /// 正常データを渡すと管理されているエラーリストから除去される。
        /// </summary>
        /// <param name="value"></param>
        public void AddError(MqttJsonError value)
        {
            if (value.GetErrorCode() == 0)
            {
                // Error -> Normal
                if (errors.TryGetValue(value.DeviceId, out MqttJsonError error))
                {
                    errors.Remove(error.DeviceId);
                    HasError = ExistsError();
                    HasWarn = ExistsWarn();
                }
            }
            else
            {
                // Normal -> Error or Error -> Error
                errors[value.DeviceId] = value;
                HasError = ExistsError();
                HasWarn = ExistsWarn();
            }
        }

        /// <summary>
        /// エラー/警告情報リストにエラーがそんざいするかの判定
        /// </summary>
        /// <returns>true: 存在 false: 未存在</returns>
        private bool ExistsError()
        {
            var error = errors.Values.FirstOrDefault((value) =>
            {
                return DeviceErrorCode.HasError(value.GetErrorCode());
            });
            return (error != null);
        }

        /// <summary>
        /// エラー/警告情報リストに警告が存在するかの判定
        /// </summary>
        /// <returns>true: 存在 false: 未存在</returns>
        private bool ExistsWarn()
        {
            var error = errors.Values.FirstOrDefault((value) =>
            {
                return DeviceErrorCode.HasWarn(value.GetErrorCode());
            });
            return (error != null);
        }
    }

}
