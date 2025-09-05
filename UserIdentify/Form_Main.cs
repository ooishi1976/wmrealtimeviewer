using DPUruNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;


namespace UserIdentify
{
    public partial class Form_Main : Form
    {
        /// <summary>
        /// Holds fmds enrolled by the enrollment GUI.
        /// </summary>
        public Dictionary<int, Fmd> Fmds
        {
            get { return fmds; }
            set { fmds = value; }
        }
        private Dictionary<int, Fmd> fmds = new Dictionary<int, Fmd>();
        
        /// <summary>
        /// Reset the UI causing the user to reselect a reader.
        /// </summary>
        public bool Reset
        {
            get { return reset; }
            set { reset = value; }
        }
        private bool reset;

        private ReaderCollection _readers;

        public static  UserBioDP.UserDatabaseDP userDb;

        private const int DPFJ_PROBABILITY_ONE = 0x7fffffff;
        private DPCtlUruNet.IdentificationControl identificationControl;

        Dictionary<string, List<string>> m_UserTable = new Dictionary<string, List<string>>();

        public Form_Main()
        {
            using (Tracer tracer = new Tracer("Form_Main::Form_Main"))
            {
                InitializeComponent();
            }
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = $"ユーザ認証 Ver. {version.Major}.{version.Minor}.{version.Build}";


            label1.Text = "認証を開始します\r\n認証装置に指を置いてください";

            userDb = new UserBioDP.UserDatabaseDP();
            userDb.Connect();
            userDb.CreateDefault();

            Fmds.Clear();

            AddUserforFMD();
        }


        private void Form_Main_Load(object sender, EventArgs e)
        {
 
            GetReaderDevice();

            if (identificationControl != null)
            {
                identificationControl.Reader = CurrentReader;
            }
            else
            {
                // See the SDK documentation for an explanation on threshold scores.
                //  threshHoldが大きいほど甘い
                int tmpScore = 2500;    //  <- こいつを小さくするほど甘くなる default=100000
                int thresholdScore = DPFJ_PROBABILITY_ONE * 1 / tmpScore;

                identificationControl = new DPCtlUruNet.IdentificationControl(CurrentReader, Fmds.Values, thresholdScore, 1, Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
                identificationControl.Location = new System.Drawing.Point(3, 3);
                identificationControl.Name = "identificationControl";
                identificationControl.Size = new System.Drawing.Size(397, 128);
                identificationControl.TabIndex = 0;
                identificationControl.OnIdentify += new DPCtlUruNet.IdentificationControl.FinishIdentification(this.identificationControl_OnIdentify);

                // Be sure to set the maximum number of matches you want returned.
                identificationControl.MaximumResult = 10;

                //  謎GUI
                //Controls.Add(identificationControl);

            }

            identificationControl.StartIdentification();
        }

        private void Form_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (identificationControl != null)
            {
                identificationControl.StopIdentification();
            }
        }

        private void AddUserforFMD()
        {
            int id = 0;
            foreach (var u in userDb.GetAllUsers())
            {
                Fmds.Add((int)u.Id, Fmd.DeserializeXml(u.BiometricsFmd));
                m_UserTable.Add(id.ToString(), new List<string>() { u.Name, u.Memo });
                id++;
            }
        }

        private void identificationControl_OnIdentify(DPCtlUruNet.IdentificationControl IdentificationControl, IdentifyResult IdentificationResult)
        {
            if (IdentificationResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
            {
                if (IdentificationResult.Indexes == null)
                {
                    if (IdentificationResult.ResultCode == Constants.ResultCode.DP_INVALID_PARAMETER)
                    {
                        Debug.WriteLine($"Warning: Fake finger was detected");
                    }
                    else if (IdentificationResult.ResultCode == Constants.ResultCode.DP_NO_DATA)
                    {
                        Debug.WriteLine($"Warning: No finger was detected");
                    }
                    else
                    {
                        if (CurrentReader != null)
                        {
                            CurrentReader.Dispose();
                            CurrentReader = null;
                        }
                    }
                }
                else
                {
                    if (CurrentReader != null)
                    {
                        CurrentReader.Dispose();
                        CurrentReader = null;
                    }

                    Debug.WriteLine($"@@@ Error: {IdentificationResult.ResultCode} ");
                }
            }
            else
            {
                CurrentReader = IdentificationControl.Reader;
                Debug.WriteLine($"OnIdentify: { IdentificationResult.Indexes.Length } ");

                if (IdentificationResult.Indexes.Length > 0)
                {
                    string idkey = IdentificationResult.Indexes[0][0].ToString();

                    SendMessageDelegate(Action.IdentificationComp, m_UserTable[idkey][0]);
                   
                }
            }

        }

        // When set by child forms, shows s/n and enables buttons.
        public Reader CurrentReader
        {
            get { return currentReader; }
            set
            {
                currentReader = value;
                SendMessageDelegate(Action.UpdateReaderState, value);
            }
        }
        private Reader currentReader;


        private enum Action
        {
           UpdateReaderState ,
           IdentificationComp
        }
        private delegate void SendMessageCallback(Action state, object payload);
        private void SendMessageDelegate(Action state, object payload)
        {
            using (Tracer tracer = new Tracer("Form_Main::SendMessage"))
            {
                if (buttonCancel.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessageDelegate);
                    Invoke(d, new object[] { state, payload });
                }
                else
                {
                    switch (state)
                    {
                        case Action.UpdateReaderState:
                           
                            break;
                        case Action.IdentificationComp:
                            label1.Text = " 認証できました\r\nしばらくお待ちください";
                            buttonCancel.Enabled = false;
                            OutputResult(0, 0, payload.ToString());
                            identificationControl.StopIdentification();
                            timer1.Enabled = true ;
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        private void GetReaderDevice()
        {
            try
            {
                _readers = ReaderCollection.GetReaders();

                foreach (Reader Reader in _readers)
                {
                    Debug.WriteLine($"ReaderName: {Reader.Description.Name}");
                }
                currentReader = _readers[0];

                SendMessageDelegate(Action.UpdateReaderState, currentReader);
            }
            catch (Exception ex)
            {
                //message box:
                String text = ex.Message;
                text += "\r\n\r\n指紋認証デバイス向けサービスが起動していません";
                String caption = "Cannot access readers";
                MessageBox.Show(text, caption);
            }
        }

        private void OutputResult(int resultCode, long userId = 0, string name = "")
        {
            string stCurrentDir = Environment.CurrentDirectory;
            string folderPath = stCurrentDir;
            string outputPath = folderPath + "\\tempIdentifyResult.dat";

            string msg;
            if (resultCode == 0)
            {
                msg = String.Format("{0},{1},{2}{3}", resultCode, userId, name, Environment.NewLine);
            }
            else
            {
                msg = String.Format("{0},{1}", resultCode, Environment.NewLine);
            }

            try
            {
                File.AppendAllText(outputPath, msg);
            }
            catch
            {
                resultCode = 11;
            }

            Environment.ExitCode = resultCode;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            OutputResult(1, 0, "");
            Close();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}