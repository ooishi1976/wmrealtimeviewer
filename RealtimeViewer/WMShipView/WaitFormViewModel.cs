using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RealtimeViewer.WMShipView
{
    public class WaitFormViewModel : BindableModel
    {
        private bool isShowAuthWarning = true;

        public bool IsShowAuthWarning 
        { 
            get => isShowAuthWarning; 
            set => SetProperty(ref isShowAuthWarning, value); 
        }

        private Task waitTask;
        public Task WaitTask
        {
            get => waitTask;
            set => SetProperty(ref waitTask, value);
        }

        public bool IsEndWaitTask => (WaitTask is null | WaitTask.IsCompleted | WaitTask.IsFaulted | WaitTask.IsCanceled);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="waitTask"></param>
        public WaitFormViewModel(Dispatcher dispatcher, Task waitTask)
        {
            Dispatcher = dispatcher;
            this.waitTask = waitTask;
        }
    }
}
