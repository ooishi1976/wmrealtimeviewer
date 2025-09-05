using Mamt;
using Mamt.Args;
using MpgCommon;
using MpgCustom;
using RealtimeViewer.Model;
using RealtimeViewer.Network.Mqtt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RealtimeViewer.Model
{
    /// <summary>
    /// 運行車両情報
    /// </summary>
    public class CarInfo
    {
        /// <summary>
        /// 車載器情報
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }

        /// <summary>
        /// 車載器ID取得
        /// </summary>
        public string DeviceId
        {
            get
            {
                string result = string.Empty;
                if (DeviceInfo != null)
                {
                    result = DeviceInfo.DeviceId;
                }
                return result;
            }
        }

        /// <summary>
        /// 事業所
        /// </summary>
        public OfficeInfo OfficeInfo { get; set; }

        /// <summary>
        /// MQTT - Locationから取得 
        /// 位置情報を配列で保持。取得した順番に格納。
        /// </summary>
        private SynchronizedCollection<MqttJsonLocation> LocationList { get; set; }

        /// <summary>
        /// MQTT - Errorから取得
        /// </summary>
        public MqttJsonError ErrorCode { get; set; }

        /// <summary>
        /// MQTT - Eventから取得
        /// </summary>
        public MqttJsonEventAccOn AccStatus { get; set; }

        /// <summary>
        /// MQTT - PrepostEventから取得
        /// </summary>
        public MqttJsonPrepostEvent EventPrepost { get; set; }

        /// <summary>
        /// 地図上の表示情報
        /// </summary>
        public MapEntryInfo MapEntry { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CarInfo()
        {
            LocationList = new SynchronizedCollection<MqttJsonLocation>();
        }

        /// <summary>
        /// 地図/運行車両リストの表示対象であるか判定する。
        /// 下記の場合に表示対象外と判断される
        /// ・位置情報が無い
        /// ・位置情報がある 且つ ACC情報がある 且つ ACCがOFF
        /// 位置情報は指定時刻よりも以前のデータのうち、最新を用いる。
        /// 採用された位置情報より古いものは削除される。
        /// 指定時刻にnullを指定した場合は、先頭のデータを用いる。
        /// </summary>
        /// <param name="dateTime">指定時刻</param>
        /// <returns>true: 表示対象 false: 表示対象外</returns>
        public bool CanEntryForMap(DateTime? dateTime = null)
        {
            bool result = true;
            if (GetLocation(dateTime) == null)
            {
                // 位置情報が無ければ地図には表示しない。
                result = false;
            } 
            else if (AccStatus != null && !AccStatus.IsAccOn())
            {
                // 位置情報はあるが、ACCがOFFなら地図には表示しない
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 位置情報を取得する。
        /// 位置情報は指定時刻よりも以前のデータのうち、最新を返す。
        /// 返された位置情報より古いものは削除される。
        /// 指定時刻にnullを指定した場合は、先頭のデータを返す。
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>位置情報</returns>
        public MqttJsonLocation GetLocation(DateTime? dateTime = null)
        {
            MqttJsonLocation result = null;
            if (0 < LocationList.Count)
            {
                if (dateTime == null)
                {
                    // 時間指定なしなら先頭を返す
                    result = LocationList.First();
                }
                else
                {
                    // 指定時間以前ならListを操作する
                    int targetIndex = 0;
                    foreach (var location in LocationList)
                    {
                        // 基準時刻より後は取り出さない
                        if (0 < location.Ts.CompareTo(dateTime?.ToString("yyyyMMddHHmmss")))
                        {
                            break;
                        }
                        targetIndex++;
                    }
                    for (int i = 0; i < targetIndex - 1; i++)
                    {
                        LocationList.RemoveAt(0);
                    }
                    result = LocationList.First();
                }
            }
            return result;
        }

        /// <summary>
        /// 位置情報を位置情報リストに追加する。
        /// </summary>
        /// <param name="location">位置情報</param>
        public void AddLocation(MqttJsonLocation location)
        {
            LocationList.Add(location);
        }

        /// <summary>
        /// 位置情報を位置情報リストに設定する。
        /// それまでリストに保持していた位置情報は全てクリアされる。
        /// </summary>
        /// <param name="location">位置情報</param>
        public void SetLocation(MqttJsonLocation location)
        {
            LocationList.Clear();
            AddLocation(location);
        }
    }

    /// <summary>
    /// 地図登録情報
    /// </summary>
    public class MapEntryInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 住所
        /// </summary>
        private string address = string.Empty;

        /// <summary>
        /// ディスパッチャー
        /// </summary>
        private Dispatcher dispatcher;

        /// <summary>
        /// 車番
        /// </summary>
        public string CarId { get; set; }

        /// <summary>
        /// 車載器ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 住所
        /// 住所が更新されるとイベント通知が行われる。
        /// </summary>
        public string Address {
            get {
                return address;
            }
            set {
                address = value;
                NotifyPropertyChanged();
            } 
        }

        /// <summary>
        /// 地図座標
        /// </summary>
        public PointLL MapCoordinate { get; set; }

        /// <summary>
        /// エラーコード(サイドパネルにbindしている)
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// エラーメッセージ(サイドパネルにbindしている)
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 地図のテキストオブジェクト
        /// </summary>
        public TextObject EntryObject { get; set; }

        /// <summary>
        /// 通知イベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// コンストラクタ
        /// 住所が更新されるとイベント通知が行われる。
        /// その際、UI更新を伴う場合は、UIスレッドのディスパッチャーを設定すること。
        /// </summary>
        /// <param name="dispatcher">ディスパッチャー</param>
        public MapEntryInfo(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// 通知イベント
        /// </summary>
        /// <param name="propertyName">変更プロパティ名</param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        /// <summary>
        /// 住所検索を実行する際に使用するコールバック。
        /// 住所検索の結果を受け取り、Addressに設定することで、
        /// イベント通知を発生させ、UI上でbindしている運行情報一覧上の「住所」を更新する。
        /// </summary>
        /// <param name="obj">呼び出し元オブジェクト</param>
        /// <param name="args">イベントオブジェクト</param>
        public void FindAddressFinished(object obj, FindAddressCoordFinishEventArgs args)
        {
            ResultsCoord result = args.CoordResults;
            if (result == null)
            {
                Address = string.Empty;
            } 
            else
            {
                foreach (ResultCoord adr in result.Address)
                {
                    Address = $@"{adr.Shi}{adr.Oaza}{adr.Koaza}";
                }
            }
        }
    }

    /// <summary>
    /// 地図登録情報のコンパレータ
    /// </summary>
    class MapEntryInfoComparer : IComparer<MapEntryInfo>, IComparer
    {
        public enum SortMode
        {
            DeviceId,
            Address
        }

        private SortMode mode = SortMode.DeviceId;
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

        public int Compare(MapEntryInfo x, MapEntryInfo y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");

            int orderValue = (order == SortOrder.Ascending) ? 1 : -1;

            int result;
            switch (mode)
            {
                case SortMode.DeviceId:
                    result = x.DeviceId.CompareTo(y.DeviceId) * ((int)orderValue);
                    if (result == 0)
                    {
                        result = x.Address.CompareTo(y.Address);
                    }
                    break;
                case SortMode.Address:
                    result = x.Address.CompareTo(y.Address) * ((int)orderValue);
                    if (result == 0)
                    {
                        result = x.DeviceId.CompareTo(y.DeviceId) * 1;
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
            if (x is MapEntryInfo xE && y is MapEntryInfo yE)
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
