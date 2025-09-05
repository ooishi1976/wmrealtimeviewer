using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeViewer
{
    public partial class FormMessage : Form
    {
        public string Title { get; set; }

        public string Message { get; set; }
        public ContentAlignment MessageAlignment { get; set; }
        public Color MessageColor { get; set; }

        public int AutoCloseMillisec { get; set; }

        public FormMessage()
        {
            InitializeComponent();

            MessageAlignment = ContentAlignment.MiddleCenter;
            MessageColor = Color.Black;
            AutoCloseMillisec = 0;
        }

        private void FormMessage_Load(object sender, EventArgs e)
        {
            Text = Title; // Window title.
            labelMessage.Text = Message;
            labelMessage.TextAlign = MessageAlignment;
            labelMessage.ForeColor = MessageColor;

            if (AutoCloseMillisec > 0)
            {
                timerClose.Interval = AutoCloseMillisec;
                timerClose.Enabled = true;
                timerClose.Start();
            }
        }

        private void RequestFormClose(object sender, EventArgs e)
        {
            timerClose.Stop();
            timerClose.Enabled = false;
            Close();
        }
    }
}
