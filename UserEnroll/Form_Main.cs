using DPUruNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;


namespace UserEnroll
{
    public partial class Form_Main : Form
    {
        private List<Fmd> FmdList;
        private Dictionary<Fmd, int> FmdDic;

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

        public static UserBioDP.UserDatabaseDP userDb;
        public static BindingList<UserBioDP.User> userList = new BindingList<UserBioDP.User>();

        private DPCtlUruNet.IdentificationControl identificationControl;
        private const int DPFJ_PROBABILITY_ONE = 0x7fffffff;
        private bool hasUserEditPerm = false;
        private Dictionary<int, UserBioDP.User> userDic = new Dictionary<int, UserBioDP.User>();
        private bool hasAuthed;

        public Form_Main()
        {
            using (Tracer tracer = new Tracer("Form_Main::Form_Main"))
            {
                InitializeComponent();
            }
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = $"ユーザ登録 Ver. {version.Major}.{version.Minor}.{version.Build}";

            userDb = new UserBioDP.UserDatabaseDP();
            userDb.Connect();
            userDb.CreateDefault();

            FmdList = new List<Fmd>();
            FmdDic = new Dictionary<Fmd, int>();

            foreach (var u in userDb.GetAllUsers())
            {
                var r = UserBioDP.RolePermissonConverter.GuessRole((UserBioDP.Permission)u.Permission);
                u.RoleName = UserBioDP.RolePermissonConverter.GetRoleName(r);
                userList.Add(u);

                var f = Fmd.DeserializeXml(u.BiometricsFmd);
                FmdList.Add(f);
                FmdDic.Add(f, (int)u.Id);
                userDic.Add((int)u.Id, u);
                if (u.Can(UserBioDP.Permission.UserEdit) || u.Can(UserBioDP.Permission.Engineer))
                {
                    hasUserEditPerm = true;
                }
            }
            if (userList.Count == 0)
            {
                //  初期登録
                tabControl1.Enabled = true;
                tabControl1.SelectedTab = tabPage2;
            }
            if (hasUserEditPerm)
            {
                // ユーザー編集権限を保持しているユーザーが存在したら、注意書きは隠す。
                richTextBoxImpotant.Visible = false;
            }

            // 実行ユーザがDBファイルへの書き込み権が無く、読み取りのみとなっている場合
            if (userDb.IsReadOnly)
            {
                toolStripStatusLabel1.Text = Properties.Resources.MSG_NO_WRITE_PERMISSION;
                buttonAdd.Enabled = false;
                buttonEdit.Enabled = false;
                buttonDelete.Enabled = false;
            }


            dataGridViewUser.AutoGenerateColumns = false;
            dataGridViewUser.DataSource = userList;
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

                identificationControl = new DPCtlUruNet.IdentificationControl(CurrentReader, FmdList, thresholdScore, 10, Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
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
                    var listIndex = IdentificationResult.Indexes[0][0];

                    var fmd = FmdList[listIndex];
                    if (FmdDic.ContainsKey(fmd))
                    {
                        var key = FmdDic[fmd];
                        SendMessageDelegate(Action.IdentificationComp, key);
                    }
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

        /// <summary>
        /// Open a device and check result for errors.
        /// </summary>
        /// <returns>Returns true if successful; false if unsuccessful</returns>
        public bool OpenReader()
        {
            using (Tracer tracer = new Tracer("Form_Main::OpenReader"))
            {
                reset = false;
                Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

                // Open reader
                result = currentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

                if (result != Constants.ResultCode.DP_SUCCESS)
                {
                    MessageBox.Show("Error:  " + result);
                    reset = true;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Hookup capture handler and start capture.
        /// </summary>
        /// <param name="OnCaptured">Delegate to hookup as handler of the On_Captured event</param>
        /// <returns>Returns true if successful; false if unsuccessful</returns>
        public bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
        {
            using (Tracer tracer = new Tracer("Form_Main::StartCaptureAsync"))
            {
                // Activate capture handler
                currentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);

                // Call capture
                if (!CaptureFingerAsync())
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Cancel the capture and then close the reader.
        /// </summary>
        /// <param name="OnCaptured">Delegate to unhook as handler of the On_Captured event </param>
        public void CancelCaptureAndCloseReader(Reader.CaptureCallback OnCaptured)
        {
            using (Tracer tracer = new Tracer("Form_Main::CancelCaptureAndCloseReader"))
            {
                if (currentReader != null)
                {
                    currentReader.CancelCapture();

                    // Dispose of reader handle and unhook reader events.
                    currentReader.Dispose();

                    if (reset)
                    {
                        CurrentReader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Check the device status before starting capture.
        /// </summary>
        /// <returns></returns>
        public void GetStatus()
        {
            using (Tracer tracer = new Tracer("Form_Main::GetStatus"))
            {
                Constants.ResultCode result = currentReader.GetStatus();

                if ((result != Constants.ResultCode.DP_SUCCESS))
                {
                    reset = true;
                    throw new Exception("" + result);
                }

                if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
                {
                    Thread.Sleep(50);
                }
                else if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
                {
                    currentReader.Calibrate();
                }
                else if ((currentReader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
                {
                    throw new Exception("Reader Status - " + currentReader.Status.Status);
                }
            }
        }

        /// <summary>
        /// Check quality of the resulting capture.
        /// </summary>
        public bool CheckCaptureResult(CaptureResult captureResult)
        {
            using (Tracer tracer = new Tracer("Form_Main::CheckCaptureResult"))
            {
                if (captureResult.Data == null || captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        reset = true;
                        throw new Exception(captureResult.ResultCode.ToString());
                    }

                    // Send message if quality shows fake finger
                    if ((captureResult.Quality != Constants.CaptureQuality.DP_QUALITY_CANCELED))
                    {
                        throw new Exception("Quality - " + captureResult.Quality);
                    }
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Function to capture a finger. Always get status first and calibrate or wait if necessary.  Always check status and capture errors.
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public bool CaptureFingerAsync()
        {
            using (Tracer tracer = new Tracer("Form_Main::CaptureFingerAsync"))
            {
                try
                {
                    GetStatus();

                    Constants.ResultCode captureResult = currentReader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, currentReader.Capabilities.Resolutions[0]);
                    if (captureResult != Constants.ResultCode.DP_SUCCESS)
                    {
                        reset = true;
                        throw new Exception("" + captureResult);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:  " + ex.Message);
                    return false;
                }
            }
        }


        private enum Action
        {
            UpdateReaderState,
            IdentificationComp
        }
        private delegate void SendMessageCallback(Action state, object payload);
        private void SendMessageDelegate(Action state, object payload)
        {
            using (Tracer tracer = new Tracer("Form_Main::SendMessage"))
            {

                if (buttonAdd.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessageDelegate);
                    Invoke(d, new object[] { state, payload });
                }
                else
                {
                    switch (state)
                    {
                        case Action.UpdateReaderState:
                            if ((Reader)payload != null && !userDb.IsReadOnly)
                            {
                                buttonAdd.Enabled = true;
                            }
                            else
                            {
                                //  デバイスがない
                                buttonAdd.Enabled = false;
                            }
                            break;
                        case Action.IdentificationComp:
                            {
                                int userId = (int)payload;
                                if (userDic.ContainsKey(userId))
                                {
                                    Debug.WriteLine($"+++ Auth: uid {userId}, {userDic[userId].Id}, {userDic[userId].Name}");
                                    UpdateUiAtAuthed(AuthStatus.Authed, userDic[userId]);
                                }
                            }
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

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (identificationControl != null)
            {
                identificationControl.StopIdentification();
            }

            var window = new Enrollment();

            window._sender = this;
            window.TargetUser = null;

            window.ShowDialog();
            window.Dispose();

            identificationControl.StartIdentification();
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewUser.CurrentRow == null)
            {
                return;
            }
            DialogResult result;
            var delUser = (UserBioDP.User)dataGridViewUser.CurrentRow.DataBoundItem;

            result = MessageBox.Show($"{delUser.Name} のユーザー情報を削除します。よろしいですか？", @"削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                // MVVM的にはリストを更新したら、DBにも反映されるのだろうけど、
                // 実装方法が分からない。とりあえずリストとDB両方から削除しておく。
                if (userDb.DeleteUser(delUser.Id) <= 0)
                {
                    Debug.WriteLine($"@@@ Error: DB - could not removed ");
                }
                if (!userList.Remove(delUser))
                {
                    Debug.WriteLine($"@@@ Error: LIST - could not removed ");
                }
            }
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewUser.CurrentRow == null)
            {
                return;
            }
            if (identificationControl != null)
            {
                identificationControl.StopIdentification();
            }
            var u = (UserBioDP.User)dataGridViewUser.CurrentRow.DataBoundItem;
            Debug.WriteLine($"Selected {u.Name}, {u.UpdateAt}");


            var window = new Enrollment();

            window._sender = this;
            window.TargetUser = u;

            window.ShowDialog();
            window.Dispose();
            identificationControl.StartIdentification();
        }

        enum AuthStatus
        {
            None = 0,
            Authed,
            DevMode,
        }

        private void UpdateUiAtAuthed(AuthStatus status, UserBioDP.User user = null)
        {
            switch (status)
            {
                case AuthStatus.None:
                    labelAuthed.Visible = false;
                    label1.Visible = true;
                    labelStatus.Text = string.Empty;
                    labelStatus.Visible = false;
                    break;

                case AuthStatus.Authed:
                    if (user != null)
                    {
                        if (((UserBioDP.Permission)user.Permission & UserBioDP.Permission.UserEdit) != 0)
                        {
                            tabControl1.Enabled = true;
                            labelAuthed.Visible = true;
                            label1.Visible = false;
                            labelStatus.Visible = false;
                            hasAuthed = true;
                        }
                        else
                        {
                            labelAuthed.Enabled = true;
                            labelStatus.Text = @"特権ユーザーで認証してください";
                            labelStatus.Visible = true;
                        }
                    }
                    break;

                case AuthStatus.DevMode:
                    tabControl1.Enabled = true;
                    label1.Visible = false;
                    labelStatus.Text = @"エンジニアモード";
                    labelStatus.Visible = true;
                    hasAuthed = true;
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.F9))
            {
                this.Text += @" [エンジニアモード]";
                UpdateUiAtAuthed(AuthStatus.DevMode);
                Debug.WriteLine(@"#### Enter Developer Mode");
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // 未認証の時は、ユーザー一覧タブは開けないようにする。
            if (!hasAuthed && e.TabPageIndex == 1)
            {
                e.Cancel = true;
            }
        }
    }
}