using DPUruNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace UserEnroll
{
    public partial class Enrollment : Form
    {
        /// <summary>
        /// Holds the main form with many functions common to all of SDK actions.
        /// </summary>
        public Form_Main _sender;

        List<Fmd> preenrollmentFmds;
        int count;
        bool bUserEdit = false;

        private Fmd biometricFmd = null;
        public UserBioDP.User TargetUser { get; set; } // 編集対象のユーザー

        public Enrollment()
        {
            InitializeComponent();

            comboBoxRole.SelectedIndex = 0;

            label4.Text = "登録する指をゆっくり置いて離してください。\r\n約４回ほど繰り返すと完了します。";

            var dr = new UserBioDP.DefaultRoleList();

            comboBoxRole.DataSource = dr.Roles;
            comboBoxRole.DisplayMember = @"Name";
            comboBoxRole.ValueMember = @"Role";
        }

        /// <summary>
        /// Initialize the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enrollment_Load(object sender, EventArgs e)
        {
            preenrollmentFmds = new List<Fmd>();
            count = 0;
            bUserEdit = false;

            if (!_sender.OpenReader())
            {
                Close();
            }

            if (!_sender.StartCaptureAsync(OnCaptured))
            {
                Close();
            }

            if (TargetUser != null)
            {
                textBoxName.Text = TargetUser.Name;
                biometricFmd = Fmd.DeserializeXml(TargetUser.BiometricsFmd);

                var r = UserBioDP.RolePermissonConverter.GuessRole((UserBioDP.Permission)TargetUser.Permission);
                comboBoxRole.SelectedValue = r;

                buttonEnroll.Enabled = true;

                bUserEdit = true;
            }
        }

        /// <summary>
        /// Handler for when a fingerprint is captured.
        /// </summary>
        /// <param name="captureResult">contains info and data on the fingerprint capture</param>
        private void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                // Check capture quality and throw an error if bad.
                if (!_sender.CheckCaptureResult(captureResult)) return;

                count++;

                DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);

                Debug.WriteLine($"A finger was captured. Count:{count}");

                if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    _sender.Reset = true;
                    throw new Exception(resultConversion.ResultCode.ToString());
                }

                preenrollmentFmds.Add(resultConversion.Data);

                SendMessage(Action.UpdateProgress, "");

                if (count >= 4)
                {
                    DataResult<Fmd> resultEnrollment = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.ANSI, preenrollmentFmds);

                    if (resultEnrollment.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {
                        //  登録成功
                        Debug.WriteLine($"An enrollment FMD was successfully created");

                        _sender.CancelCaptureAndCloseReader(OnCaptured);

                        //  登録したい指紋データ
                        biometricFmd = preenrollmentFmds[0];
                        
                        preenrollmentFmds.Clear();
                        count = 0;
                        SendMessage(Action.CompleteEnroll,"");
                        return;
                    }
                    else if (resultEnrollment.ResultCode == Constants.ResultCode.DP_ENROLLMENT_INVALID_SET)
                    {
                        //  登録失敗 ?
                        Debug.WriteLine($"Enrollment was unsuccessful.  Please try again");
                        preenrollmentFmds.Clear();
                        count = 0;
                        return;
                    }
                }

                Debug.WriteLine($"Now place the same finger on the reader");
            }
            catch (Exception ex)
            {
                // Send error message, then close form
                Debug.WriteLine($"Error: {ex.Message}");
            }  
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void btnBack_Click(Object sender, EventArgs e)
        {
            Close();
        }


        private void Enrollment_FormClosed(object sender, FormClosedEventArgs e)
        {
            _sender.CancelCaptureAndCloseReader(OnCaptured);
        }

        private enum Action
        {
            SendMessage,
            UpdateProgress,
            CompleteEnroll
        }

        private delegate void SendMessageCallback(Action action, string payload);
        private void SendMessage(Action action, string payload)
        {
            try
            {
                if (buttonEnroll.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    Invoke(d, new object[] { action, payload });
                }
                else
                {
                    switch (action)
                    {
                        case Action.SendMessage:
                            break;
                        case Action.UpdateProgress:
                            progressBar1.Value += 25;
                            break;
                        case Action.CompleteEnroll:
                            buttonEnroll.Enabled = true;
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ButtonEnroll_Click(object sender, EventArgs e)
        {
            UserBioDP.User u;

            if (bUserEdit == false)
            {
                //  追加
                u = new UserBioDP.User();
            }
            else
            {
                //  編集
                u = TargetUser;
            }

            u.Name = textBoxName.Text;
            u.UpdateAt = DateTime.Now;
            if (comboBoxRole.SelectedItem != null)
            {
                var role = (UserBioDP.RoleItem)comboBoxRole.SelectedItem;
                u.Permission = (int)UserBioDP.RolePermissonConverter.ToPermission(role.Role);
                var r = UserBioDP.RolePermissonConverter.GuessRole((UserBioDP.Permission)u.Permission);
                u.RoleName = UserBioDP.RolePermissonConverter.GetRoleName(r);
            }
            u.BiometricsFmd = Fmd.SerializeXml(biometricFmd);

            if (bUserEdit == false)
            {
                var uid = Form_Main.userDb.AddUser(u);
                u.Id = uid;
                Form_Main.userList.Add(u);
            }
            else
            {
                Form_Main.userDb.UpdateUser(u);
            }

            Close();
        }

    }
}
