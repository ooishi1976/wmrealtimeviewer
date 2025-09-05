using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using RealtimeViewer.Model;
using RealtimeViewer.Setting;
using System.Text.RegularExpressions;

namespace RealtimeViewer.Logger
{
    /// <summary>
    /// 車載器から受信したエラー情報をファイル出力する。
    /// 実装したものの未使用。
    /// </summary>
    public class ErrorListWriter
    {
        /// <summary>
        /// 出力先ディレクトリ(親ディレクトリ)
        /// </summary>
        private string baseDirPath;
        /// <summary>
        /// ローテーション数
        /// </summary>
        private int rotationNum;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rotationNum">ローテーション数</param>
        public ErrorListWriter(int rotationNum)
        {
            this.rotationNum = rotationNum;
            baseDirPath = GetBaseDirPath();
        }

        /// <summary>
        /// エラー情報のファイル書き込みを行う。
        /// [ユーザディレクトリ]/ドキュメント/アプリケーション名/RecieveErrorMessage/出力日時/営業所名/車番.txtに追加書き込みを行う。
        /// ディレクトリが未存在の場合は作成したのち、ファイル出力を行う。
        /// また、ディレクトリ数がローテーション数を超えた場合はディレクトリ名が古いものから削除する。
        /// </summary>
        /// <param name="carInfo">運行車両情報(エラー情報含む)</param>
        /// <param name="dateTime">出力日時</param>
        public void Write(CarInfo carInfo, DateTime dateTime)
        {
            string path = GetTargetDirPath(carInfo, dateTime);
            string fileName = $"{carInfo.DeviceInfo.DeviceId}.txt";
            string log = GetLogText(carInfo, dateTime);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (0 < rotationNum) 
            {
                RotateDirectory(baseDirPath);
            }
            path = Path.Combine(path, fileName);
            File.AppendAllText(path, log);
        }

        /// <summary>
        /// ディレクトリのローテーションを行う。
        /// 指定パスの配下にyyyyMMdd形式のディレクトリをカウントし、古い日付のディレクトリから削除する
        /// </summary>
        /// <param name="path"></param>
        private void RotateDirectory(string path)
        {
            var regex = new Regex(@"\\(\d{4})(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])$");
            List<string> children = Directory.GetDirectories(path).Where(x => regex.IsMatch(x)).ToList();
            if (rotationNum < children.Count)
            {
                int removeItemNum = (children.Count - rotationNum);
                children.Sort((x, y) => x.CompareTo(y));
                foreach (var child in children.Select((value, index) => (value, index)))
                {
                    if (removeItemNum <= child.index)
                    {
                        break;
                    }
                    RemoveDirectory(child.value);
                }
            }
        }

        /// <summary>
        /// 指定したパスのディレクトリを削除する。
        /// ディレクトリ内にファイルが存在しても削除する。
        /// </summary>
        /// <param name="path">削除するディレクトリのパス</param>
        private void RemoveDirectory(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            try
            {
                dirInfo.Delete(true);
            } catch {}
        }

        /// <summary>
        /// 出力先ディレクトリ(親ディレクトリ)のパスを取得する。
        /// 対象は[ユーザディレクトリ]/ドキュメント/アプリケーション名/RecieveErrorMessage
        /// </summary>
        /// <returns></returns>
        private string GetBaseDirPath()
        {
            string[] pathPhrase = {
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Application.ProductName,
                "RecieveErrorMessage",
            };
            return Path.Combine(pathPhrase);
        }

        /// <summary>
        /// 出力対象ディレクトリのパスを取得する。
        /// [ユーザディレクトリ]/ドキュメント/アプリケーション名/RecieveErrorMessage/出力日時/事業所名
        /// </summary>
        /// <param name="carInfo"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string GetTargetDirPath(CarInfo carInfo, DateTime dateTime)
        {
            string officeName = (carInfo.OfficeInfo == null) ? carInfo.DeviceInfo.OfficeId.ToString() : carInfo.OfficeInfo.Name;
            string[] pathPhrase = {
                baseDirPath,
                dateTime.ToString("yyyyMMdd"),
                officeName,
            };
            return Path.Combine(pathPhrase);
        }

        /// <summary>
        /// ファイルに書き込む文字列を取得する。
        /// 出力フォーマットは以下の通り。
        /// yyyyMMdd-HH:mm:ss.fff: Ts:{エラー情報のTs}, ErrorCode:{エラー情報のエラーコード}, {エラーメッセージ}
        /// </summary>
        /// <param name="carInfo">車載器情報</param>
        /// <param name="dateTime">出力日時</param>
        /// <returns>出力文字列</returns>
        private string GetLogText(CarInfo carInfo, DateTime dateTime)
        {
            return $"{dateTime.ToString("yyyyMMdd-HH:mm:ss.fff")}: Ts:{carInfo.ErrorCode.Ts}, ErrorCode:{carInfo.ErrorCode.Error}, {DeviceErrorCode.MakeErrorMessage(carInfo.ErrorCode.GetErrorCode(), Separator: ", ")}\n";
        }
    }
}
