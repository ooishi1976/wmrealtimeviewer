using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace RealtimeViewer
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex app_mutex = new Mutex(false, @"ISDT20DvrRealtimeViewer");

            bool hasHandle = false;
            try
            {
                try
                {
                    //ミューテックスの所有権を要求する
                    hasHandle = app_mutex.WaitOne(0, false);
                }
                //.NET Framework 2.0以降の場合
                catch (System.Threading.AbandonedMutexException)
                {
                    //別のアプリケーションがミューテックスを解放しないで終了した時
                    hasHandle = true;
                }
                //ミューテックスを得られたか調べる
                if (hasHandle == false)
                {
#if HIDE_IN_TASKBAR

                    // 基本的に、この手のヤツはタイミングがダメなときはダメ。高負荷時など。あきらめてくれ。
                    try
                    {
                        // おそらく最初であろうインスタンスに向けて「アクティブになれ」と送り付ける。あとは知らん。
                        var c = new RtvIPcMessageClient();
                        c.Request(RtvIpcRequest.ActivateWindow);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"@@@ Exception: {ex}, {ex.StackTrace}");
                        // メッセージでお茶を濁しておく。どうなろうと知らん。
                        MessageBox.Show("多重起動はできません。");
                    }
#else
                    MessageBox.Show("多重起動はできません。");
#endif
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                // 緊急通報モード判定
                string deviceId = string.Empty;
                string[] args = Environment.GetCommandLineArgs();
                if (3 <= args.Length)
                {
                    if (args[1] == "/e" && !string.IsNullOrEmpty(args[2]))
                    {
                        deviceId = args[2];
                    }
                }
                if (string.IsNullOrEmpty(deviceId))
                {
                    // 通常モード
                    Application.Run(new MainForm());
                }
                else
                {
                    // 緊急通報モード
                    Application.Run(new MainForm(deviceId));
                }
            }
            finally
            {
                if (hasHandle)
                {
                    //ミューテックスを解放する
                    app_mutex.ReleaseMutex();
                }
                app_mutex.Close();
            }
        }
    }
}
