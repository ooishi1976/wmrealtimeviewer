using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RealtimeViewer.Movie;

namespace RealtimeViewer.Movie
{
    public partial class FfmpegCtrl
    {
        public delegate void PlayMovieHandler(PlayMovieProgress progress);

        public event PlayMovieHandler MovieProgress;

        /// <summary>
        /// ffmpeg で映像と音声をミックスし、それをVLCで再生する。
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="filepath"></param>
        /// <param name="audioFilePath"></param>
        /// <param name="fps"></param>
        /// <param name="callback">進捗など</param>
        /// <returns></returns>
        public async Task PlayMovieAsync(
            int ch, string filepath, string audioFilePath, int fps, 
            CancellationToken? cancellationToken = null)
        {
            var token = (cancellationToken ?? CancellationToken.None);
            var di = Directory.GetParent(filepath);
            var avfile = MakeContainerFullPath(di.FullName, ch);

            if (!File.Exists(avfile))
            {
                var createContainerCommand = MakeContainerFileCommand(avfile, fps, filepath, audioFilePath);
                var createContainerProcess = Process.Start(createContainerCommand);
                try
                {
                    while (!createContainerProcess.HasExited)
                    {
                        if (token.IsCancellationRequested)
                        {
                            createContainerProcess.Kill(); // 強制終了
                            break;
                        }
                        Thread.Sleep(200); // ポーリング間隔
                    }
                }
                finally
                {
                    createContainerProcess.Dispose();
                }
            }

            // VLC.
            const int width = 720;
            const int height = 480;
            var x = (1920 - width) / 2;
            var y = (1080 - height) / 2;

            string[] options =
            {
                @"--no-qt-video-autoresize", @"--no-video-title-show", @"--no-qt-bgcone",
                @"--no-qt-updates-notif", @"--video-on-top", @"--play-and-pause",
                $"--video-x={x}",$"--video-y={y}", $"--width={width}", $"--height={height}",
                $"--skins2-last=\"{GetVlcEventSkinPath(ch)}\"",
                $" {avfile}",
            };

            MovieProgress?.Invoke(new PlayMovieProgress() { PlayMovieStatus = PlayMovieStatus.Playing });

            var psi = new ProcessStartInfo
            {
                FileName = Application.StartupPath + @"\vlc\vlc.exe",
                Arguments = string.Join(" ", options),
                WorkingDirectory = Application.StartupPath,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            var playVlcProcess = Process.Start(psi);
            processes.TryAdd(playVlcProcess, true);
            try
            {
                await Task.Run(() =>
                {
                    while (!playVlcProcess.HasExited)
                    {
                        if (token.IsCancellationRequested)
                        {
                            playVlcProcess.Kill(); // 強制終了
                            break;
                        }
                        Thread.Sleep(200); // ポーリング間隔
                    }
                }, token);
            }
            finally
            {
                processes.TryRemove(playVlcProcess, out _);
            }
            MovieProgress?.Invoke(new PlayMovieProgress() { PlayMovieStatus = PlayMovieStatus.ProcessCompleted });
        }
    }
}
