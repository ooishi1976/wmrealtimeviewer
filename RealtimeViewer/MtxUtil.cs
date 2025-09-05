using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace RealtimeViewer
{
    public class MtxUtil
    {
        /// <summary>
        /// 指定パスに存在する全MTXファイルから、最大のG値を得る。
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static GravityRecord GetBiggestGravity(string targetDir, DateTime timestamp)
        {
            GravityRecord result = null;
            try
            {
                var baseTs = new TimeSpan(0, 0, 30); // timestampからの前後期間
                double biggestG = 0.0;

                foreach (var f in Directory.GetFileSystemEntries(targetDir, @"*.mtx", SearchOption.AllDirectories))
                {
                    Debug.WriteLine($"SearchDig - {f}");
                    var glist = GetGravityRecords(f);
                    foreach (var g in glist)
                    {
                        var s = timestamp - g.Timestamp;
                        var d = s.Duration();
                        if (baseTs > d)
                        {
                            var v = Math.Sqrt((g.X * g.X) + (g.Y * g.Y) + (g.Z * g.Z));
                            if (biggestG < v)
                            {
                                biggestG = v;
                                result = g;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception : SearchDig {ex}, {ex.StackTrace}");
            }

            return result;
        }

        /// <summary>
        /// 指定ディレクトリ内のMTXファイルにイベント発生分のmtxが存在する場合、<br/>
        /// イベント発生秒のGセンサー値を取得する。<br/>
        /// 同一秒、ミリ秒違いのデータがある場合はその最大値をとる。<br/>
        /// </summary>
        /// <param name="targetDir">MTX展開ディレクトリ</param>
        /// <param name="occurDateTime">イベント発生秒</param>
        /// <returns></returns>
        public static GravityRecord GetGravity(string targetDir, DateTime occurDateTime, double prepostDuration=10D)
        {
            GravityRecord result = null;
            double maxG = 0D;

            foreach (string file in Directory.GetFileSystemEntries(targetDir, $"*.mtx", SearchOption.AllDirectories))
            {
                foreach (GravityRecord gravity in GetGravityRecords(file))
                {
                    if (occurDateTime <= gravity.Timestamp)
                    {
                        TimeSpan duration = (occurDateTime - gravity.Timestamp).Duration();
                        if (prepostDuration <= duration.TotalSeconds)
                        {
                            break;
                        }
                        double gValue = Math.Sqrt((gravity.X * gravity.X) + (gravity.Y * gravity.Y) + (gravity.Z * gravity.Z));
                        if (maxG < gValue)
                        {
                            maxG = gValue;
                            result = gravity;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gセンサー値を取得する。<br/>
        /// MUイベントから発生時刻以降のDIG値のログを読み取る。<br/>
        /// </summary>
        /// <param name="targetDir">MUイベントファイル展開ディレクトリ</param>
        /// <param name="occurDateTime">イベント発生時刻</param>
        /// <returns>Gセンサー値。該当なしの場合はNULL</returns>
        public static GravityRecord Gravity(string targetDir, DateTime occurDateTime)
        {
            GravityRecord result = null;

            foreach (string file in Directory.GetFileSystemEntries(targetDir, $"*.mu", SearchOption.AllDirectories))
            {
                result = GetGravityRecords(file, occurDateTime);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// MTXファイルからGのレコードを抽出する。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<GravityRecord> GetGravityRecords(string filePath)
        {
            var result = new List<GravityRecord>();
            try
            {
                const int RECORD_LENGTH = 12;
                using (var f = new FileStream(filePath, FileMode.Open))
                using (var r = new BinaryReader(f))
                {
                    if (f.Length < RECORD_LENGTH * 2)
                        throw new FileFormatException(@"Too short length.");

                    byte[] record = r.ReadBytes(RECORD_LENGTH);
                    if (record[0] != 'M' || record[1] != 'T' || record[2] != 'X')
                        throw new FileFormatException(@"Does not MTX.");
                    record = r.ReadBytes(RECORD_LENGTH);
                    var baseTime = new DateTime(2000 + record[1], record[2], record[3], record[4], 0, 0);

                    while ((record = r.ReadBytes(RECORD_LENGTH)) != null)
                    {
                        if (record.Length < RECORD_LENGTH)
                        {
                            break;
                        }

                        if (record[0] != 0x10)
                        {
                            continue;
                        }

                        var g = GravityRecord.Parse(baseTime.Year, baseTime.Month, baseTime.Day, baseTime.Hour, record);
                        result.Add(g);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception : GetGravityRecords {ex}, {ex.StackTrace}");
            }

            return result;
        }

        /// <summary>
        /// Gセンサー値を取得する。<br/>
        /// 入力ファイルはMUイベントで、DIGの行を探してデータを取得する。<br/>
        /// </summary>
        /// <param name="filePath">MUイベントファイルパス</param>
        /// <param name="occurTime">発生時刻</param>
        /// <returns>Gセンサー値</returns>
        private static GravityRecord GetGravityRecords(string filePath, DateTime occurTime)
        {
            GravityRecord result = null;
            if (File.Exists(filePath))
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    Match match = Regex.Match(line, @"(?<occurDate>\d{12}).DIG(?<gx>\d{3})(?<gy>\d{3})(?<gz>\d{3})");
                    if (match.Success)
                    {
                        string occurDateStr = $"20{match.Groups["occurDate"].Value}";
                        DateTime.TryParseExact(occurDateStr, "yyyyMMddHHmmss", 
                            CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime digOccurDate);
                        if (occurTime.CompareTo(digOccurDate) <= 0)
                        {
                            int gx = int.Parse(match.Groups["gx"].Value);
                            int gy = int.Parse(match.Groups["gy"].Value);
                            int gz = int.Parse(match.Groups["gz"].Value);
                            result = new GravityRecord(digOccurDate, gx, gy, gz);
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// G値。
    /// </summary>
    public class GravityRecord
    {
        public DateTime Timestamp { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <param name="hour">時</param>
        /// <param name="record">分秒ミリ秒XYZ</param>
        public GravityRecord(int year, int month, int day, int hour, byte[] record)
        {
            if (record == null)
            {
                throw new ArgumentNullException();
            }

            if (record.Length < 12)
            {
                throw new ArgumentException(@"too short input data.");
            }

            Timestamp = new DateTime(year, month, day, hour, record[1], record[2], (int)record[3] * 10);
            X = ConvertGValue(record[4]);
            Y = ConvertGValue(record[5]);
            Z = ConvertGValue(record[6]);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dateTime">発生日時</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        public GravityRecord(DateTime dateTime, int x, int y, int z)
        {
            Timestamp = dateTime;
            X = ConvertGValue(x);
            Y = ConvertGValue(y);
            Z = ConvertGValue(z);
        }

        /// <summary>
        /// MTXのGセンサー値をG表記に変換する。<br/>
        /// 100が0Gで、1刻みが0.02Gとのこと。<br/>
        /// </summary>
        /// <param name="gValue"></param>
        /// <returns></returns>
        private static double ConvertGValue(int gValue)
        {
            return ((double)gValue - 100.0) * 0.02;
        }

        public override string ToString()
        {
            return $"{Timestamp} : X{X:0.00}, Y{Y:0.00}, Z{Z:0.00}";
        }

        /// <summary>
        /// MTXのレコードから、G値オブジェクトを生成する。
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static GravityRecord Parse(int year, int month, int day, int hour, byte[] record)
        {
            //if (record == null)
            //{
            //    throw new ArgumentNullException();
            //}

            //if (record.Length < 12)
            //{
            //    throw new ArgumentException(@"too short input data.");
            //}

            //// MTXは100が0Gで、1刻みが0.02Gとのこと。
            //double[] g = new double[3];
            //for (int i = 0; i < 3; i++)
            //{
            //    double v = record[4 + i];
            //    v = (v - 100.0) * 2.0 / 100.0;
            //    g[i] = v;
            //}

            //var result = new GravityRecord()
            //{
            //    Timestamp = new DateTime(year, month, day, hour, record[1], record[2], (int)record[3] * 10),
            //    X = g[0],
            //    Y = g[1],
            //    Z = g[2],
            //};
            //return result;
            return new GravityRecord(year, month, day, hour, record);
        }
    }
}
