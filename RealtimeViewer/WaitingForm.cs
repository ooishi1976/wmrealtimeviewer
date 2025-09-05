using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace RealtimeViewer
{
    public partial class WaitingForm : Form
    {
        public bool IsShowAuthWarning { get; set; }
        int m_timeCounter = 0;

        public WaitingForm()
        {
            InitializeComponent();

            IsShowAuthWarning = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_timeCounter < 10)
            {
                progressBar1.Value += 10;
            }
            else
            {
                timer1.Enabled = false;
                Close();
            }
            m_timeCounter++;
        }

        public void FinishWaiting()
        {
            timer1.Enabled = false;
            while (m_timeCounter < 10)
            {
                progressBar1.Value += 10;
                m_timeCounter++;
                Thread.Sleep(50);
            }
            Thread.Sleep(50);
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Close();
        }

        public void SetVisibleForAuthWarning(bool isVisible)
        {
            labelAuthWarning.Visible = isVisible;
            IsShowAuthWarning = isVisible;
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            labelAuthWarning.Visible = IsShowAuthWarning;
            Debug.WriteLine($"WaitingForm_Load : labelAuthWarning.Visible {labelAuthWarning.Visible}");
        }
    }
}
