using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeViewer.Logger
{
    /// <summary>
    /// 操作ログ。
    /// Singletonであることに注意されたい。
    /// </summary>
    public class OperationLogger : IDisposable
    {
        public enum Category
        {
            Application = 0,
            Authentication,
            Streaming,
            Setting,
            EventData,
            RemoteConfig,
            MovieRequest,
        }

        public string LogDirectoryPath { get; set; }
        public string LogFileNamePrefix { get; set; }
        public readonly string LOG_DIRECTORY_NAME = @"Log";

        private static OperationLogger _instance;
        private readonly object lockObj = new object();

        private StreamWriter stream = null;
        private string LogFilePath = null;
        private DateTime lastOutTime;


        private OperationLogger()
        {
#if _WEATHER_MEDIA
            LogDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Multiwave",
                Path.GetFileNameWithoutExtension(Application.ExecutablePath),
                LOG_DIRECTORY_NAME);
            LogFileNamePrefix = string.Empty;

#else
            LogDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ISSUI",
                Path.GetFileNameWithoutExtension(Application.ExecutablePath),
                LOG_DIRECTORY_NAME);
            LogFileNamePrefix = string.Empty;
#endif
        }

        public static OperationLogger GetInstance()
        {
            if (_instance == null)
            {
                _instance = new OperationLogger();
            }

            return _instance;
        }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Close();
            }
        }

        public void Out(Category category, string userName, string message)
        {
            var name = string.IsNullOrEmpty(userName) ? string.Empty : userName;
            // 行頭はISO 8601書式の日付時刻。
            var now = DateTime.Now;
            string fullMsg = string.Format(@"[{0}][{1}][{2}] {3}", now.ToString(@"yyyy-MM-ddTHH:mm:ss.fff"), category.ToString(), name, message);
            fullMsg = Convert.ToBase64String(Encoding.UTF8.GetBytes(fullMsg));
            for (int i = 0; i < (fullMsg.Length % 4); i++) fullMsg += "=";
            lock (lockObj)
            {
                if (RotateLogFile(now) == 0)
                {
                    stream.WriteLine(fullMsg);
                }
            }
        }

        public int CreateLogFile()
        {
            lastOutTime = DateTime.Now;
            return CreateLogFile(lastOutTime);
        }

        private int CreateLogFile(DateTime dt)
        {
            int ret = 0;
            try
            {
                if (!Directory.Exists(LogDirectoryPath))
                {
                    Directory.CreateDirectory(LogDirectoryPath);
                }

                if (stream != null)
                {
                    stream.Close();
                }

                var ts = dt.ToString(@"yyyy-MM-dd");
                var ext = @".log";
                var sep = string.IsNullOrEmpty(LogFileNamePrefix) ? string.Empty : @"_";
                var filename = $"{LogFileNamePrefix}{sep}{ts}{ext}";
                LogFilePath = Path.Combine(LogDirectoryPath, filename);
                stream = new StreamWriter(LogFilePath, true, Encoding.UTF8)
                {
                    AutoFlush = true
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex} {ex.StackTrace}");
                ret = -1;
            }

            return ret;
        }

        private int RotateLogFile(DateTime dt)
        {
            int ret = 0;
            if (lastOutTime.Date != dt.Date)
            {
                lastOutTime = dt;
                ret = CreateLogFile(dt);
            }

            return ret;
        }
    }
}
