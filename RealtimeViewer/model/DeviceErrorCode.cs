using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeViewer.Model
{
    /// <summary>
    /// エラーコード
    /// </summary>
    public enum CodeError
    {
        SSDError = 0x00000100,
        CameraError1 = 0x00010000,
        CameraError2 = 0x00020000,
        CameraError3 = 0x00040000,
        CameraError4 = 0x00080000,
        CameraError5 = 0x00100000,
        CameraError6 = 0x00200000,
        CameraError7 = 0x00400000,
        CameraError8 = 0x00800000,
    }

    /// <summary>
    /// 警告コード
    /// </summary>
    public enum CodeWarn
    {
        SDCardError = 0x00000200,
        NamePlateError = 0x00000800,
        GPSError = 0x00001000,
        ETCError = 0x00002000,
        PrinterError = 0x00004000,
        StatusSwitchError = 0x00008000
    }

    /// <summary>
    /// CodeError, CodeWarnの拡張メソッド
    /// </summary>
    public static class ErrorCodeExtentions
    {
        public static int Value(this CodeError errorCodeError)
        {
            return (int)errorCodeError;
        }
        public static int Value(this CodeWarn errorCodeError)
        {
            return (int)errorCodeError;
        }
    }

    public class DeviceErrorCode
    {
        private const int CameraErrorMin = 0x00010000;
        private const int CameraErrorMax = 0x00800000;

        private static readonly Dictionary<int, ErrorItem> ErrorCodeDic = new Dictionary<int, ErrorItem>()
        {
            { CodeError.SSDError.Value(), new ErrorItem() { ErrorCode = CodeError.SSDError.Value(), Name = @"SSDエラー", BackgroundColor = Color.Red }},
            { CodeWarn.SDCardError.Value(), new ErrorItem() { ErrorCode = CodeWarn.SDCardError.Value(), Name = @"SDカードエラー", BackgroundColor = Color.Yellow} },
//            { 0x00000400, new ErrorItem() { ErrorCode = 0x00000400, Name = @"カメラエラー", TextColor = Color.Red } }, 
            { CodeWarn.NamePlateError.Value(), new ErrorItem() { ErrorCode = CodeWarn.NamePlateError.Value(), Name = @"氏名札エラー", BackgroundColor = Color.Yellow, }},
            { CodeWarn.GPSError.Value(), new ErrorItem() { ErrorCode = CodeWarn.GPSError.Value(), Name = @"GPSエラー", BackgroundColor = Color.Yellow, }},
            { CodeWarn.ETCError.Value(), new ErrorItem() { ErrorCode = CodeWarn.ETCError.Value(), Name = @"ETCエラー", BackgroundColor = Color.Yellow } },
            { CodeWarn.PrinterError.Value(), new ErrorItem() { ErrorCode = CodeWarn.PrinterError.Value(), Name = @"プリンターエラー", BackgroundColor = Color.Yellow } },
            { CodeWarn.StatusSwitchError.Value(), new ErrorItem() { ErrorCode = CodeWarn.StatusSwitchError.Value(), Name = @"ステータススイッチエラー", BackgroundColor = Color.Yellow, } },

            { CodeError.CameraError1.Value(), new ErrorItem() { ErrorCode = CodeError.CameraError1.Value(), Name = @"1", BackgroundColor = Color.Red, } },
            { CodeError.CameraError2.Value(), new ErrorItem() { ErrorCode = CodeError.CameraError2.Value(), Name = @"2", BackgroundColor = Color.Red, } },
            { CodeError.CameraError3.Value(), new ErrorItem() { ErrorCode = CodeError.CameraError3.Value(), Name = @"3", BackgroundColor = Color.Red, } },
            { CodeError.CameraError4.Value(), new ErrorItem() { ErrorCode = CodeError.CameraError4.Value(), Name = @"4", BackgroundColor = Color.Red, } },
            { CodeError.CameraError5.Value(), new ErrorItem() { ErrorCode = CodeError.CameraError5.Value(), Name = @"5", BackgroundColor = Color.Red, } },
            { CodeError.CameraError6.Value(), new ErrorItem() { ErrorCode = CodeError.CameraError6.Value(), Name = @"6", BackgroundColor = Color.Red, } },
            { CodeError.CameraError7.Value(), new ErrorItem() { ErrorCode = CodeError.CameraError7.Value(), Name = @"7", BackgroundColor = Color.Red, } },
            { CodeError.CameraError8.Value(), new ErrorItem() { ErrorCode = CodeError.CameraError8.Value(), Name = @"8", BackgroundColor = Color.Red, } },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ErrorCode"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string MakeErrorMessage(int ErrorCode, string Separator = "\r\n")
        {
            string result = "正常";
            if (ErrorCode != 0)
            {
                var cameraErrors = new List<string>();
                var msgs = new List<string>();
                for (int bit = 1; bit < 32; bit++)
                {
                    int bitValue = 1 << bit;
                    if ((ErrorCode & bitValue) > 0)
                    {
                        if (ErrorCodeDic.ContainsKey(bitValue))
                        {
                            if (CameraErrorMin <= bitValue && bitValue <= CameraErrorMax)
                            {
                                cameraErrors.Add(ErrorCodeDic[bitValue].Name);
                            }
                            else
                            {
                                msgs.Add(ErrorCodeDic[bitValue].Name);
                            }
                        }
                    }
                }

                if (cameraErrors.Count > 0)
                {
                    var s = string.Join(@", ", cameraErrors);
                    msgs.Add($"カメラエラー({s})");
                }
                result = string.Join(Separator, msgs);
            }
            return result;
        }

        /// <summary>
        /// 使うべき背景色を返す。
        /// 赤、黄、白の順で重要度が高く、最も重要度の高い色となる。
        /// </summary>
        /// <param name="ErrorCode"></param>
        /// <returns></returns>
        public static Color GetBackgroundColor(int ErrorCode)
        {
            Color c = Color.White;
            for (int bit = 1; bit < 32; bit++)
            {
                int bitValue = 1 << bit;
                if ((ErrorCode & bitValue) > 0)
                {
                    if (ErrorCodeDic.ContainsKey(bitValue))
                    {
                        if (ErrorCodeDic[bitValue].BackgroundColor == Color.White)
                        {
                            // ignore.
                        }
                        else if (ErrorCodeDic[bitValue].BackgroundColor == Color.Yellow)
                        {
                            c = ErrorCodeDic[bitValue].BackgroundColor;
                        }
                        else if (ErrorCodeDic[bitValue].BackgroundColor == Color.Red)
                        {
                            c = ErrorCodeDic[bitValue].BackgroundColor;
                            break;
                        }
                    }
                }
            }

            return c;
        }

        /// <summary>
        /// エラーコードにエラーが含まれているか
        /// </summary>
        /// <param name="ErrorCode">エラーコード</param>
        /// <returns>true: 含む false: 含まない</returns>
        public static bool HasError(int ErrorCode)
        {
            bool hasError = false;
            foreach (int item in Enum.GetValues(typeof(CodeError))) {
                if ((item & ErrorCode) == item)
                {
                    hasError = true;
                    break;
                }
            }
            return hasError;
        }

        /// <summary>
        /// エラーコードに警告が含まれているか
        /// </summary>
        /// <param name="ErrorCode">エラーコード</param>
        /// <returns>true: 含む false: 含まない</returns>
        public static bool HasWarn(int ErrorCode)
        {
            bool hasWarn = false;
            foreach (int item in Enum.GetValues(typeof(CodeWarn)))
            {
                if ((item & ErrorCode) == item)
                {
                    hasWarn = true;
                    break;
                }
            }
            return hasWarn;
        }

        private class ErrorItem
        {
            public int ErrorCode { get; set; }
            public string Name { get; set; }
            public Color BackgroundColor { get; set; }
        }
    }
}
