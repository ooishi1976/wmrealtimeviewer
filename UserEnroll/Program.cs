using System;

using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

//! @cond
namespace UserEnroll
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
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
                    //得られなかった場合は、すでに起動していると判断して終了
                    MessageBox.Show("多重起動はできません。");
                    return;
                }

#if (!WindowsCE)
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();
#endif
                Application.Run(new Form_Main());
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
//! @endcond