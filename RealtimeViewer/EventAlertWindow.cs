using System;
using System.Windows.Forms;

namespace RealtimeViewer
{
    public partial class EventAlertWindow : Form
    {
        public EventAlertWindow(string DeviceId, int MovieType)
        {
            InitializeComponent();

            if (MovieType == 1)
            {
                label1.Text = "強い衝撃";
            }
            else if (MovieType == 3)
            {
                label1.Text = "緊急スイッチ";
            }
            label2.Text = DeviceId;
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
