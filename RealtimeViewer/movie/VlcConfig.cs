using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace RealtimeViewer.Movie
{
    /// <summary>
    /// VLC の設定を良い感じにする。
    /// </summary>
    public class VlcConfig
    {
        private const string RC_FILE = @"vlcrc";
        private string vlcConfigRoot;
        public VlcConfig()
        {
            vlcConfigRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"vlc"
            );
        }

        /// <summary>
        /// VLCのrcファイルを作る。
        /// </summary>
        /// <returns></returns>
        public bool CreateRcFile()
        {
            bool result = true;
            var filepath = Path.Combine(vlcConfigRoot, RC_FILE);
            const string content = "[core]\nintf=skins2,any\nskins2-systray=0";
            try
            {
                if (!Directory.Exists(vlcConfigRoot))
                {
                    Directory.CreateDirectory(vlcConfigRoot);
                }

                using (FileStream fs = new FileStream(filepath, FileMode.CreateNew))
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    sw.Write(content);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Debug.WriteLine($"@@@ Exception : {ex}, {ex.StackTrace}");
            }
            return result;
        }

        /// <summary>
        /// VLCのRCファイルを変更する。
        /// 所定のファイルが存在しない場合は、CreateRcFileを呼び出す。
        /// 前提としてVLCが出力した妥当なファイルであること。
        /// </summary>
        /// <returns></returns>
        public bool UpdateRcFile()
        {
            bool result = true;
            var filepath = Path.Combine(vlcConfigRoot, RC_FILE);
            char[] EOL = "\r\n".ToCharArray();

            string content;
            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Open))
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8))
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    content = sr.ReadToEnd();
                    fs.Seek(0, SeekOrigin.Begin);
                    foreach (var x in content?.Split('\n'))
                    {
                        bool doWriteLine = true;
                        var line = x.Trim(EOL);
                        if (line.StartsWith(@"#intf="))
                        {
                            // スキンが適用できる設定にする。
                            doWriteLine = false;
                            sw.Write("intf=skins2,any\n");
                        }
                        
                        if (line.StartsWith("#skins2-systray"))
                        {
                            // ウィンドウ毎にアイコンが増えていくので
                            // タスクトレイのアイコン非表示設定とする
                            doWriteLine = false;
                            sw.Write("skins2-systray=0\n");
                        }

                        if (doWriteLine)
                        {
                            // 改行はLFのみ(Unix)
                            sw.Write(line + "\n");
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException dnfex)
            {
                Debug.WriteLine($"{dnfex} 発生。{RC_FILE} がない。ないなら適当にでっちあげる。");
                result = CreateRcFile();
            }
            catch (FileNotFoundException fnfex)
            {
                Debug.WriteLine($"{fnfex} 発生。{RC_FILE} がない。ないなら適当にでっちあげる。");
                result = CreateRcFile();
            }
            catch (Exception ex)
            {
                result = false;
                Debug.WriteLine($"@@@ Exception : {ex}, {ex.StackTrace}");
            }

            return result;
        }
    }
}
