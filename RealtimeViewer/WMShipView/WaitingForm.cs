using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeViewer.WMShipView
{
    public partial class WaitingForm : Form
    {
        private WaitFormViewModel ViewModel { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="viewModel"></param>
        public WaitingForm(WaitFormViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;

            labelAuthWarning.DataBindings.Add("Visible", ViewModel, nameof(ViewModel.IsShowAuthWarning));
            ViewModel.WaitTask.ContinueWith((t) =>
            {
                Invoke((MethodInvoker)(() => { Close(); } ));
            });
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WaitingForm_Shown(object sender, EventArgs e)
        {
            if (ViewModel.IsEndWaitTask)
            {
                Close();
            }
        }
    }
}
