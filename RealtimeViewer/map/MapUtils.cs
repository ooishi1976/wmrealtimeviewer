using Mamt;
using Mamt.Args;
using MpgCommon;
using RealtimeViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Mamt.MamtBase;

namespace RealtimeViewer.Map
{
    public class MapUtils
    {
        public static Dictionary<int, Dictionary<int, string>> cache = new Dictionary<int, Dictionary<int, string>>();

        /// <summary>
        /// 住所検索を行う
        /// </summary>
        /// <param name="point">対象の地図座標</param>
        /// <param name="findFinishedHandler">検索結果を受け取るコールバック</param>
        public static void FindAddress(PointLL point, FindAddressCoordFinishEventHandler findFinishedHandler)
        {
            // オブジェクトを生成する
            // 座標検索結果を取得するイベントを定義する
            Mamt.Mamt mamtObj = MamtFactory.GetInstance("latest") as Mamt.Mamt;
            mamtObj.FindAddressCoordFinish += findFinishedHandler;
            ParamCoord param = new ParamCoord
            {
                X = point.X,
                Y = point.Y,
                InputCoord = MamtEnum.CoordTypeEnum.TKY,
            };
            mamtObj.FindAddressCoord(param);
        }

        /// <summary>
        /// 住所検索を行う
        /// </summary>
        /// <param name="point">対象の地図座標</param>
        /// <param name="findFinishedHandler">検索結果を受け取るコールバック</param>
        public static void FindAddress(string latitude, string longitude, FindAddressCoordFinishEventHandler findFinishedHandler)
        {
            // オブジェクトを生成する
            // 座標検索結果を取得するイベントを定義する
            Mamt.Mamt mamtObj = MamtFactory.GetInstance("latest") as Mamt.Mamt;
            mamtObj.FindAddressCoordFinish += findFinishedHandler;

            ParamCoord param = new ParamCoord
            {
                X = UtilGPSConvert.ToGPSMS(int.Parse(longitude)),
                Y = UtilGPSConvert.ToGPSMS(int.Parse(latitude)),
                InputCoord = MamtEnum.CoordTypeEnum.JGD
            };
            mamtObj.FindAddressCoord(param);
        }

        /// <summary>
        /// 世界測地系(JDG)から日本測地系(TKY)に変換する。
        /// </summary>
        /// <param name="latitude">JDG緯度</param>
        /// <param name="longitude">JDG経度</param>
        /// <returns>TKY座標</returns>
        public static PointLL ConvertJdgToTky(string latitude, string longitude)
        {
            // 緯度経度(世界測地系)を日本測地系に変換する
            long lLatitudeMS = UtilGPSConvert.ToGPSMS(int.Parse(latitude));
            long lLongitudeMS = UtilGPSConvert.ToGPSMS(int.Parse(longitude));
            PointLL pos = new PointLL((int)lLongitudeMS, (int)lLatitudeMS);

            MpgTransform.Tky2Jgd mapTky2Jgd = MpgTransform.Tky2Jgd.GetInstance("latest");
            return mapTky2Jgd.ToTky(pos);
        }

        /// <summary>
        /// 住所文字列から住所を検索する
        /// </summary>
        /// <param name="address">住所文字列</param>
        /// <param name="findFinishedHandler">住所検索イベントハンドラ</param>
        public static void FindAddress(string address, FindAddressAnalyFinishEventHandler findFinishedHandler)
        {
            Mamt.Mamt mamtObj = MamtFactory.GetInstance("latest") as Mamt.Mamt;
            mamtObj.FindAddressAnalyFinish += findFinishedHandler;
            mamtObj.FindAddressAnaly(new ParamAnaly() { 
                Key = address
            });
        }
    }
}
