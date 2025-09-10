using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeViewer.Movie
{
    public partial class FfmpegCtrl
    {
        private string strFFMPEG = Application.StartupPath + @"\ffmpeg.exe";
        private string strFFPLAY = Application.StartupPath + @"\ffplay.exe";
        private string strFFPROBE = Application.StartupPath + @"\ffprobe.exe";
        private readonly string vlcSkinDir = Path.Combine(Application.StartupPath, "vlc", "skins");
        public string VlcSkinPath;
        public string VlcSkinNoControlsPath;

        private ConcurrentDictionary<Process, bool> processes = new ConcurrentDictionary<Process, bool>();

        public FfmpegCtrl()
        {
            VlcSkinPath = Path.Combine(vlcSkinDir, @"isdt.vlt");
            VlcSkinNoControlsPath = Path.Combine(Application.StartupPath,
                @"vlc", @"skins", @"isdt-nc.vlt");
        }

        /// <summary>
        /// コンテナファイル(.mkv)の絶対パスを生成する。
        /// </summary>
        /// <param name="parentPath">親ディレクトリのパス</param>
        /// <param name="dt">タイムスタンプ</param>
        /// <param name="ch">映像チャンネル(0始まりのインデックス値)</param>
        /// <returns>絶対パス</returns>
        public static string MakeContainerFullPath(string parentPath, DateTime dt, int ch)
        {
            // ".mkv" 形式については https://ja.wikipedia.org/wiki/Matroska あたりを参考に。
            // ".mkv" は、書き込みが途中で終わっていても再生可能。例えば、エンコーダーが途中で死んだケースでも何とか見れる。

            var datetime = dt.ToString(@"yyyy-MM-dd_HHmm");
            return Path.Combine(parentPath, $"{datetime}_ch{ch + 1}.mkv");
        }

        /// <summary>
        /// コンテナファイル(.mkv)の絶対パスを生成する。
        /// </summary>
        /// <param name="parentPath">親ディレクトリのパス</param>
        /// <param name="ch">映像チャンネル(0始まりのインデックス値)</param>
        /// <returns>絶対パス</returns>
        public static string MakeContainerFullPath(string parentPath, int ch)
        {
            return Path.Combine(parentPath, $"ch{ch + 1}.mkv");
        }

        /// <summary>
        /// 映像と音声ファイルを結合し、コンテナファイルとするコマンドを生成する。
        /// </summary>
        /// <param name="output"></param>
        /// <param name="fps"></param>
        /// <param name="movieFilePath"></param>
        /// <param name="audioFilePath"></param>
        /// <returns>ProcessStartInfo インスタンス</returns>
        /// <remarks>ProcessStartInfoを生成するのみである。呼び出し元側で実行されたい。</remarks>
        public static ProcessStartInfo MakeContainerFileCommand(string output, int fps, string movieFilePath, string audioFilePath)
        {
            ProcessStartInfo psi;

            // ffmpeg のオプションは順番にも意味がある。注意されたい。
            var opts = new StringBuilder();
            opts.Append($" -framerate {fps}");
            opts.Append($" -i {movieFilePath}");
            if (!string.IsNullOrEmpty(audioFilePath))
                opts.Append($" -i {audioFilePath}");
            opts.Append(@" -c copy");
            opts.Append(@" -shortest");
            opts.Append($" {output}");

            psi = new ProcessStartInfo();
            psi.WorkingDirectory = Application.StartupPath;
            psi.FileName = Application.StartupPath + @"\ffmpeg.exe";
            psi.Arguments = opts.ToString();
            psi.CreateNoWindow = true; // コンソール・ウィンドウを開かない
            psi.UseShellExecute = false; // シェル機能を使用しない

            return psi;
        }

        /// <summary>
        /// ffmpeg で映像と音声をミックスし、それをVLCで再生する。
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="filepath"></param>
        /// <param name="audioFilePath"></param>
        /// <param name="fps"></param>
        /// <param name="callback">進捗など</param>
        /// <returns></returns>
        public async Task PlayMovie(int ch, string filepath, string audioFilePath, int fps, Action<PlayMovieProgress> callback)
        {
            var di = Directory.GetParent(filepath);
            var avfile = MakeContainerFullPath(di.FullName, ch);

            if (!File.Exists(avfile))
            {
                try
                {
                    var psi = MakeContainerFileCommand(avfile, fps, filepath, audioFilePath);

                    await Task.Run(() =>
                    {
                        callback(new PlayMovieProgress() { PlayMovieStatus = PlayMovieStatus.PreProcess });

                        Process p0 = Process.Start(psi);
                        p0.WaitForExit(); // 終了を待つ。
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"@@@ ffmpeg Exception {ex}");
                }
            }

            try
            {
                // VLC.
                const int width = 720;
                const int height = 480;
                int x = (1920 - width) / 2;
                int y = (1080 - height) / 2;

                string[] options =
                {
                    @"--no-qt-video-autoresize", @"--no-video-title-show", @"--no-qt-bgcone",
                    @"--no-qt-updates-notif", @"--video-on-top", @"--play-and-pause",
                    $"--video-x={x}",$"--video-y={y}", $"--width={width}", $"--height={height}",
                    $"--skins2-last=\"{GetVlcEventSkinPath(ch)}\"",
                    $" {avfile}",
                };
                await Task.Run(() =>
                {
                    callback(new PlayMovieProgress() { PlayMovieStatus = PlayMovieStatus.Playing });
                    var psi1 = new ProcessStartInfo();
                    psi1.WorkingDirectory = Application.StartupPath;
                    psi1.FileName = Application.StartupPath + @"\vlc\vlc.exe";
                    psi1.Arguments = string.Join(@" ", options);
                    psi1.CreateNoWindow = true; // コンソール・ウィンドウを開かない
                    psi1.UseShellExecute = false; // シェル機能を使用しない

                    Process p = Process.Start(psi1);
                    processes.AddOrUpdate(p, true, (k, v) => true);
                    p.WaitForExit();
                    bool value;
                    processes.TryRemove(p, out value);
                    callback(new PlayMovieProgress() { PlayMovieStatus = PlayMovieStatus.ProcessCompleted });
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ vlc Exception {ex}");
            }
        }

        public async Task PlayList(int channel, string[] fileList, Action<PlayMovieProgress> callback)
        {
            var fileArg = string.Join(@" ", fileList);
            if (string.IsNullOrEmpty(fileArg))
            {
                return;
            }

            try
            {
                // VLC.
                const int width = 720;
                const int height = 480;
                int x = (1920 - width) / 2;
                int y = (1080 - height) / 2;

                string[] options =
                {
                    @"--no-qt-video-autoresize", @"--no-video-title-show", @"--no-qt-bgcone",
                    @"--no-qt-updates-notif", @"--video-on-top",
                    $"--video-x={x}",$"--video-y={y}", $"--width={width}", $"--height={height}",
                    $"--skins2-last=\"{GetVlcEventSkinPath(channel)}\"",
                    $" {fileArg}",
                };
                Debug.WriteLine($"vlc {string.Join(@" ", options)}");

                await Task.Run(() =>
                {
                    if (callback != null)
                        callback(new PlayMovieProgress() { PlayMovieStatus = PlayMovieStatus.Playing });
                    var psi = new ProcessStartInfo();
                    psi.WorkingDirectory = Application.StartupPath;
                    psi.FileName = Application.StartupPath + @"\vlc\vlc.exe";
                    psi.Arguments = string.Join(@" ", options);
                    psi.CreateNoWindow = true; // コンソール・ウィンドウを開かない
                    psi.UseShellExecute = false; // シェル機能を使用しない

                    Process p = Process.Start(psi);
                    processes.AddOrUpdate(p, true, (k, v) => true);
                    p.WaitForExit();
                    bool value;
                    processes.TryRemove(p, out value);
                    if (callback != null)
                        callback(new PlayMovieProgress() { PlayMovieStatus = PlayMovieStatus.ProcessCompleted });
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ vlc Exception {ex}");
            }
        }

        public bool MakeContainerFileSync(string output, int fps, string movieFilePath, string audioFilePath)
        {
            bool isSuccessed = true;
            if (!File.Exists(output))
            {
                try
                {
                    var psi = MakeContainerFileCommand(output, fps, movieFilePath, audioFilePath);

                    Process p = Process.Start(psi);
                    processes.AddOrUpdate(p, true, (k, v) => true);
                    p.WaitForExit(); // 終了を待つ。
                    bool value;
                    processes.TryRemove(p, out value);
                    Debug.WriteLine($"exit {psi.FileName}, status:{p.ExitCode}");
                    if (p.ExitCode != 0)
                    {
                        isSuccessed = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"@@@ ffmpeg Exception {ex}");
                    isSuccessed = false;
                }
            }
            return isSuccessed;
        }

        /// <summary>
        /// 起動したプロセスを強制終了する。
        /// </summary>
        public void KillAllProcesses()
        {
            foreach (var p in processes.Keys)
            {
                try
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"@@@ Exception: KillAllProcesses {ex}");
                }
            }
        }

        private string GetVlcEventSkinPath(int channel)
        {
            return Path.Combine(vlcSkinDir, $"isdt-ch{channel + 1}.vlt");
        }
    }

    public enum PlayMovieStatus
    {
        None = 0,
        PreProcess,
        Playing,
        ProcessCompleted,
    }

    public class PlayMovieProgress
    {
        public PlayMovieStatus PlayMovieStatus { get; set; }
    }
}
