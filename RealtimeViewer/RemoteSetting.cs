using Newtonsoft.Json;
using RealtimeViewer.Converter;
using RealtimeViewer.Logger;
using RealtimeViewer.Network.Http;
using RealtimeViewer.Network;
using RealtimeViewer.Setting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using RealtimeViewer.Network.Mqtt;
using System.Drawing;
using System.ComponentModel;

namespace RealtimeViewer
{
    using SerialDevices = MqttJsonRemoteConfig.SerialDevices;
    using SignalPolarities = MqttJsonRemoteConfig.IoSignalPolarities;
    using PulseDitectType = MqttJsonRemoteConfig.PulseDitectType;

    public partial class RemoteSetting : Form
    {
        private const string MQTT_TOPIC_REMOTECONFIG_REQUEST = "car/request/{0}/conf";
        private bool mqttRetain = false;

        private enum RemoteSettingPersmission
        {
            None,
            Operator,
            SuperUser,
        };

        public MqttClient MqttClient { get; set; }
        public string DeviceId { get; set; }
        public string CarId { get; set; }
        public HttpClient HttpClient { get; set; }

        private MqttJsonRemoteConfig SendingData { get; set; }

        private WindowRemoteConfigViewModel viewModel;

        public WindowRemoteConfigViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                viewModel = value;
                if (viewModel.DeviceStatus != null)
                {
                    bindingSourceDeviceInfo.DataSource = viewModel.DeviceStatus;
                }
                InitComponent();
            }
        }

        private string UserName { get; set; }
        private RemoteSettingPersmission perm = RemoteSettingPersmission.None;
        private OperationLogger logger;

        /// <summary>
        /// クラウドサーバ接続情報
        /// </summary>
        public OperationServerInfo ServerInfo { get; set; }

        public SettingIni LocalSettings { get; set; }

        public RemoteSetting()
        {
            InitializeComponent();
            logger = OperationLogger.GetInstance();
        }

        /// <summary>
        /// コンボボックスのITEMの設定を行う。
        /// </summary>
        private void InitComponent()
        {
            // Camera Bitrate
            SetDictionaryForDataSource(comboBoxRecBitrate1ch, viewModel.BitRateItems);
            SetDictionaryForDataSource(comboBoxRecBitrate2ch, viewModel.BitRateItems);
            SetDictionaryForDataSource(comboBoxRecBitrate3ch, viewModel.BitRateItems);
            SetDictionaryForDataSource(comboBoxRecBitrate4ch, viewModel.BitRateItems);
            SetDictionaryForDataSource(comboBoxRecBitrate5ch, viewModel.BitRateItems);
            SetDictionaryForDataSource(comboBoxRecBitrate6ch, viewModel.BitRateItems);
            SetDictionaryForDataSource(comboBoxRecBitrate7ch, viewModel.BitRateItems);
            SetDictionaryForDataSource(comboBoxRecBitrate8ch, viewModel.BitRateItems);
            // 取付位置
            SetDictionaryForDataSource(comboBoxDevicePos, viewModel.DeviceDirectionItems);
            // Pulse
            SetDictionaryForDataSource(comboBoxSpeedVoltageThreshold, viewModel.VoltageThresholdItems);
            SetDictionaryForDataSource(comboBoxSpeedPulseCoefficient, viewModel.PulseCoefficientItems);
            SetDictionaryForDataSource(comboBoxTachVoltageThreshold, viewModel.VoltageThresholdItems);
            // Realtime
            SetDictionaryForDataSource(comboBoxRealtimeBitrate, viewModel.BitRateItems);
            // I/O
            SetDictionaryForDataSource(comboBoxParkingBrake, viewModel.IoItems);
            SetDictionaryForDataSource(comboBoxFrontDoor, viewModel.IoItems);
            SetDictionaryForDataSource(comboBoxRearDoor, viewModel.IoItems);
            SetDictionaryForDataSource(comboBoxBackGear, viewModel.IoItems);
            SetDictionaryForDataSource(comboBoxBrake, viewModel.IoItems);
            SetDictionaryForDataSource(comboBoxLeftWinker, viewModel.IoItems);
            SetDictionaryForDataSource(comboBoxRightWinker, viewModel.IoItems);

            // Camera Flip Mode
            SetImageListForDropDown(comboBoxCamFlip1ch);
            SetImageListForDropDown(comboBoxCamFlip2ch);
            SetImageListForDropDown(comboBoxCamFlip3ch);
            SetImageListForDropDown(comboBoxCamFlip4ch);
            SetImageListForDropDown(comboBoxCamFlip5ch);
            SetImageListForDropDown(comboBoxCamFlip6ch);
            SetImageListForDropDown(comboBoxCamFlip7ch);
            SetImageListForDropDown(comboBoxCamFlip8ch);

            // retain setting
            mqttRetain = ServerInfo.IsUseRetainMessage(viewModel.OfficeId);
        }

        /// <summary>
        /// NumericUpDownのバリデート後のイベントで
        /// Textプロパティが空白の場合にValueの値をTextプロパティに設定する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumUpDown_Validated(object sender, EventArgs e)
        {
            var numUpDown = sender as NumericUpDown;
            if (numUpDown.Text == string.Empty)
            {
                numUpDown.Text = numUpDown.Value.ToString();
            }
        }

        /// <summary>
        /// コンボボックスのDataSourceを設定する
        /// </summary>
        /// <param name="comboBox">対象のコンボボックス</param>
        /// <param name="items">プルダウン表示用のDictionary</param>
        private void SetDictionaryForDataSource(ComboBox comboBox, Dictionary<int, string> items)
        {
            comboBox.DataSource = new BindingSource(items, null);
            comboBox.DisplayMember = "Value";
            comboBox.ValueMember = "Key";
        }

        /// <summary>
        /// コンボボックスのDataSourceを設定する
        /// </summary>
        /// <param name="comboBox">対象のコンボボックス</param>
        /// <param name="items">プルダウン表示用のDictionary</param>
        private void SetDictionaryForDataSource(ComboBox comboBox, Dictionary<string, string> items)
        {
            comboBox.DataSource = new BindingSource(items, null);
            comboBox.DisplayMember = "Value";
            comboBox.ValueMember = "Key";
        }

        /// <summary>
        /// コンボボックスのリストにImageを設定する。
        /// </summary>
        /// <param name="comboBox">対象のコンボボックス</param>
        private void SetImageListForDropDown(ComboBox comboBox)
        {
            foreach (Image image in imageListCamFlipMode.Images)
            {
                comboBox.Items.Add(image);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.F9))
            {
                ChangePerm(RemoteSettingPersmission.SuperUser);
                Debug.WriteLine(@"Get SuperUserPermission");
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChangePermHelper(Control.ControlCollection controls, RemoteSettingPersmission perm, bool basicValue)
        {
            foreach (Control o in controls)
            {
                if (perm == RemoteSettingPersmission.Operator && string.Compare((string)o.Tag, "operator") == 0)
                {
                    // コントロールのTagに"operator"とあれば、一般編集権限のみとする。
                    o.Enabled = true;
                    o.Visible = true;
                }
                else
                {
                    o.Enabled = basicValue;
                    o.Visible = basicValue;
                }
            }
        }

        private void ChangePerm(RemoteSettingPersmission perm)
        {
            // 基本値。
            bool basicValue = false;

            switch (perm)
            {
                case RemoteSettingPersmission.None:
                    basicValue = false;
                    break;

                case RemoteSettingPersmission.Operator:
                    basicValue = false;
                    break;

                case RemoteSettingPersmission.SuperUser:
                    // 全部有効。
                    basicValue = true;
                    break;
            }

            // 親コントロールを対象に Enabled の値を変更する。
            ChangePermHelper(this.Controls, perm, basicValue);
            buttonCancel.Enabled = true;

            this.perm = perm;
        }

        private async void RemoteSetting_Load(object sender, EventArgs e)
        {
            logger.Out(OperationLogger.Category.RemoteConfig, UserName, $"Open RemoteSetting {DeviceId}");
            ServerInfo serverInfo = ServerInfo.GetPhygicalServerInfo();
            if (MainForm.AuthedUser.Item.Can(UserBioDP.Permission.Engineer))
            {
                // 「エンジニア」のみGセンサー以外も編集可能。
                ChangePerm(RemoteSettingPersmission.SuperUser);
            }
            else if (MainForm.AuthedUser.Item.Can(UserBioDP.Permission.StreamingView))
            {
                // 一般の人(おそらく運行管理者)は、Gセンサーのみ編集可能。
                ChangePerm(RemoteSettingPersmission.Operator);
            }
            else
            {
                ChangePerm(RemoteSettingPersmission.Operator);
            }
            labelDeviceId.Text = CarId;

            JsonRemoteConfig result = await HttpRequest.GetRemoteConfig(HttpClient, serverInfo.HttpAddr, DeviceId);
            if (result != null && result.status == "OK")
            {
                viewModel.Config = result.data;
                string remoteStatusText;
                if (result.data.cameras == null)
                {
                    // サーバーデータ未登録時のConfigは配列がnullのため、初期化を行う。
                    var cameras = new MqttJsonRemoteConfigCamera[8];
                    for (int i = 0; i < 8; i++)
                    {
                        cameras[i] = new MqttJsonRemoteConfigCamera();
                        cameras[i].bitrate = (i == 0) ? "5M" : "384K";
                        cameras[i].Enabled = (i != 6) ? true : false;
                        cameras[i].framerate = (i == 0) ? 15 : 10;
                        cameras[i].flip = false;
                        cameras[i].mirror = (i == 7) ? true : false;
                    }
                    result.data.cameras = cameras;
                    result.data.io_signal_polarities = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
                    result.data.has_serial_devices = new int[4] { 1, 0, 0, 0 };
                    viewModel.Config.streaming_bitrate = "256K";

                    remoteStatusText = @"サーバーにまだ設定が格納されていません";
                    numUpDownOdometer.Text = "";
                }
                else
                {
                    remoteStatusText = $"サーバーから設定を取得しました。\r\n{result.data.update_at:f} のデータです。";
                    if (perm != RemoteSettingPersmission.None)
                    {
                        buttonSend.Enabled = true;
                    }
                }
                ConfigToUI(viewModel.Config);
                labelRemoteStatus.Text = remoteStatusText;
            }
            else
            {
                viewModel.Config = new MqttJsonRemoteConfig();
                var cameras = new MqttJsonRemoteConfigCamera[8];
                for (int i = 0; i < 8; i++)
                {
                    cameras[i] = new MqttJsonRemoteConfigCamera();
                    cameras[i].bitrate = (i == 0) ? "5M" : "384K";
                    cameras[i].Enabled = (i != 6) ? true : false;
                    cameras[i].framerate = (i == 0) ? 15 : 10;
                    cameras[i].flip = false;
                    cameras[i].mirror = (i == 7) ? true : false;
                }
                viewModel.Config.cameras = cameras;
                viewModel.Config.io_signal_polarities = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
                viewModel.Config.has_serial_devices = new int[4] { 1, 0, 0, 0 };
                viewModel.Config.streaming_bitrate = "256K";
                viewModel.Config.gsensor_direction = 4;
                viewModel.Config.tach_num_of_gear = 1500;
                viewModel.Config.speed_num_of_gear = 8;
                viewModel.Config.speed_highway_decision_time = 0;
                viewModel.Config.speed_highway_limit = 100;
                viewModel.Config.speed_normal_load_decision_time = 0;
                viewModel.Config.speed_normal_load_limit = 60;
                viewModel.Config.tacho_normal_load_limit = 3000;
                viewModel.Config.tacho_normal_load_decision_time = 0;
                viewModel.Config.update_at = DateTime.Now;
                ConfigToUI(viewModel.Config);
                numUpDownOdometer.Text = "";
                labelRemoteStatus.Text = @"サーバーからの設定取得に失敗しました";
            }
        }

        private void ConfigToUI(MqttJsonRemoteConfig config)
        {
            checkBoxRecBitrate1ch.Checked = config.cameras[0].Enabled;
            checkBoxRecBitrate2ch.Checked = config.cameras[1].Enabled;
            checkBoxRecBitrate3ch.Checked = config.cameras[2].Enabled;
            checkBoxRecBitrate4ch.Checked = config.cameras[3].Enabled;
            checkBoxRecBitrate5ch.Checked = config.cameras[4].Enabled;
            checkBoxRecBitrate6ch.Checked = config.cameras[5].Enabled;
            checkBoxRecBitrate7ch.Checked = config.cameras[6].Enabled;
            checkBoxRecBitrate8ch.Checked = config.cameras[7].Enabled;

            comboBoxRecBitrate1ch.SelectedValue = config.cameras[0].bitrate;
            comboBoxRecBitrate2ch.SelectedValue = config.cameras[1].bitrate;
            comboBoxRecBitrate3ch.SelectedValue = config.cameras[2].bitrate;
            comboBoxRecBitrate4ch.SelectedValue = config.cameras[3].bitrate;
            comboBoxRecBitrate5ch.SelectedValue = config.cameras[4].bitrate;
            comboBoxRecBitrate6ch.SelectedValue = config.cameras[5].bitrate;
            comboBoxRecBitrate7ch.SelectedValue = config.cameras[6].bitrate;
            comboBoxRecBitrate8ch.SelectedValue = config.cameras[7].bitrate;

            comboBoxCamFlip1ch.SelectedIndex = config.cameras[0].GetFlipMode().Value();
            comboBoxCamFlip2ch.SelectedIndex = config.cameras[1].GetFlipMode().Value();
            comboBoxCamFlip3ch.SelectedIndex = config.cameras[2].GetFlipMode().Value();
            comboBoxCamFlip4ch.SelectedIndex = config.cameras[3].GetFlipMode().Value();
            comboBoxCamFlip5ch.SelectedIndex = config.cameras[4].GetFlipMode().Value();
            comboBoxCamFlip6ch.SelectedIndex = config.cameras[5].GetFlipMode().Value();
            comboBoxCamFlip7ch.SelectedIndex = config.cameras[6].GetFlipMode().Value();
            comboBoxCamFlip8ch.SelectedIndex = config.cameras[7].GetFlipMode(true).Value();

            numUpDownGSensorFront.Value = (decimal)config.gsensor_forward;
            numUpDownGSensorRear.Value = (decimal)config.gsensor_backward;
            numUpDownGSensorYAxis.Value = (decimal)config.gsensor_horizontal;
            numUpDownGSensorZAxis.Value = (decimal)config.gsensor_vertical;

            comboBoxDevicePos.SelectedValue = config.gsensor_direction;

            numUpDownPreEvent.Value = config.event_pre_sec;
            numUpDownPostEvent.Value = config.event_post_sec;
            numUpDownSpeakerVolume.Value = config.speaker_volume;

            radioButtonSpeedCurrent.Checked = (PulseDitectType.CURRENT == config.SpeedPulseDitectType);
            comboBoxSpeedVoltageThreshold.SelectedValue = config.SpeedPulseThreshold;
            comboBoxSpeedPulseCoefficient.SelectedValue = config.speed_num_of_gear;

            radioButtonTachCurrent.Checked = (PulseDitectType.CURRENT == config.TachPulseDitectType);
            comboBoxTachVoltageThreshold.SelectedValue = config.TachPalseThreshold;
            numUpDownTachCoefficient.Value = config.tach_num_of_gear;

            comboBoxRealtimeBitrate.SelectedValue = config.streaming_bitrate;
            numUpDownPreEvent.Value = config.event_pre_sec;
            numUpDownPostEvent.Value = config.event_post_sec;
            numUpDownSpeakerVolume.Value = config.speaker_volume;
            numUpDownOdometer.Value = config.odometer_km;
            if (config.odometer_km == 0)
            {
                // オドメーターの値が0の場合、車載器に走行距離を反映しない、という事なので
                // 0の場合はTextプロパティに空白を設定。
                // Textを設定してもValueは変わらないので、触らなければ送信時に0が設定できる。
                // 走行距離を更新したい場合に数値を入力する
                numUpDownOdometer.Text = "";
            }

            comboBoxParkingBrake.SelectedValue = config.GetIoSignalPolarity(SignalPolarities.ParkingBrake);
            comboBoxFrontDoor.SelectedValue = config.GetIoSignalPolarity(SignalPolarities.FrontDoor);
            comboBoxRearDoor.SelectedValue = config.GetIoSignalPolarity(SignalPolarities.RearDoor);
            comboBoxBrake.SelectedValue = config.GetIoSignalPolarity(SignalPolarities.Brake);
            comboBoxBackGear.SelectedValue = config.GetIoSignalPolarity(SignalPolarities.BackGear);
            comboBoxLeftWinker.SelectedValue = config.GetIoSignalPolarity(SignalPolarities.LeftWinker);
            comboBoxRightWinker.SelectedValue = config.GetIoSignalPolarity(SignalPolarities.RightWinker);

            checkBoxNamePlate.Checked = config.HasSerialDevice(SerialDevices.NamePlate);
            checkBoxEtc.Checked = config.HasSerialDevice(SerialDevices.Etc);
            checkBoxPrinter.Checked = config.HasSerialDevice(SerialDevices.Printer);
            checkBoxStatusSwitch.Checked = config.HasSerialDevice(SerialDevices.StatusSwitch);

            // 速度超過(一般)
            numericUpDownNormalRoadSpeedLimit.Value = config.speed_normal_load_limit;
            numericUpDownNormalRoadDecisionTime.Value = config.speed_normal_load_decision_time;
            // 速度超過(高速)
            numericUpDownHighwaySpeedLimit.Value = config.speed_highway_limit;
            numericUpDownHighwayDecisionTime.Value = config.speed_highway_decision_time;
            // 回転数超過
            numericUpDownEngineRPMLimit.Value = config.tacho_normal_load_limit;
            numericUpDownEngineRPMDecisionTime.Value = config.tacho_normal_load_decision_time;
        }

        private void UItoConfig(MqttJsonRemoteConfig config)
        {
            // サーバーデータ未登録時のConfigは配列がnullのため、初期化を行う。
            if (config.cameras == null)
            {
                config.cameras = new MqttJsonRemoteConfigCamera[8];
                for (int i = 0; i < 8; i++)
                {
                    config.cameras[i] = new MqttJsonRemoteConfigCamera();
                }
            }
            if (config.io_signal_polarities == null)
            {
                config.io_signal_polarities = new int[7];
            }
            if (config.has_serial_devices == null)
            {
                config.has_serial_devices = new int[4];
            }

            config.cameras[0].bitrate = (string)comboBoxRecBitrate1ch.SelectedValue;
            config.cameras[1].bitrate = (string)comboBoxRecBitrate2ch.SelectedValue;
            config.cameras[2].bitrate = (string)comboBoxRecBitrate3ch.SelectedValue;
            config.cameras[3].bitrate = (string)comboBoxRecBitrate4ch.SelectedValue;
            config.cameras[4].bitrate = (string)comboBoxRecBitrate5ch.SelectedValue;
            config.cameras[5].bitrate = (string)comboBoxRecBitrate6ch.SelectedValue;
            config.cameras[6].bitrate = (string)comboBoxRecBitrate7ch.SelectedValue;
            config.cameras[7].bitrate = (string)comboBoxRecBitrate8ch.SelectedValue;

            config.SetIoSignalPolarity(SignalPolarities.ParkingBrake, (int)comboBoxParkingBrake.SelectedValue);
            config.SetIoSignalPolarity(SignalPolarities.FrontDoor, (int)comboBoxFrontDoor.SelectedValue);
            config.SetIoSignalPolarity(SignalPolarities.RearDoor, (int)comboBoxRearDoor.SelectedValue);
            config.SetIoSignalPolarity(SignalPolarities.BackGear, (int)comboBoxBackGear.SelectedValue);
            config.SetIoSignalPolarity(SignalPolarities.Brake, (int)comboBoxBrake.SelectedValue);
            config.SetIoSignalPolarity(SignalPolarities.LeftWinker, (int)comboBoxLeftWinker.SelectedValue);
            config.SetIoSignalPolarity(SignalPolarities.RightWinker, (int)comboBoxRightWinker.SelectedValue);

            config.cameras[0].Enabled = checkBoxRecBitrate1ch.Checked;
            config.cameras[1].Enabled = checkBoxRecBitrate2ch.Checked;
            config.cameras[2].Enabled = checkBoxRecBitrate3ch.Checked;
            config.cameras[3].Enabled = checkBoxRecBitrate4ch.Checked;
            config.cameras[4].Enabled = checkBoxRecBitrate5ch.Checked;
            config.cameras[5].Enabled = checkBoxRecBitrate6ch.Checked;
            config.cameras[6].Enabled = checkBoxRecBitrate7ch.Checked;
            config.cameras[7].Enabled = checkBoxRecBitrate8ch.Checked;

            config.cameras[0].SetFlipMode(comboBoxCamFlip1ch.SelectedIndex);
            config.cameras[1].SetFlipMode(comboBoxCamFlip2ch.SelectedIndex);
            config.cameras[2].SetFlipMode(comboBoxCamFlip3ch.SelectedIndex);
            config.cameras[3].SetFlipMode(comboBoxCamFlip4ch.SelectedIndex);
            config.cameras[4].SetFlipMode(comboBoxCamFlip5ch.SelectedIndex);
            config.cameras[5].SetFlipMode(comboBoxCamFlip6ch.SelectedIndex);
            config.cameras[6].SetFlipMode(comboBoxCamFlip7ch.SelectedIndex);
            config.cameras[7].SetFlipMode(comboBoxCamFlip8ch.SelectedIndex);

            config.SpeedPulseThreshold = (int)comboBoxSpeedVoltageThreshold.SelectedValue;
            config.TachPalseThreshold = (int)comboBoxTachVoltageThreshold.SelectedValue;

            config.gsensor_forward = (double)numUpDownGSensorFront.Value;
            config.gsensor_backward = (double)numUpDownGSensorRear.Value;
            config.gsensor_horizontal = (double)numUpDownGSensorYAxis.Value;
            config.gsensor_vertical = (double)numUpDownGSensorZAxis.Value;
            config.gsensor_direction = (int)comboBoxDevicePos.SelectedValue;
            config.event_pre_sec = (int)numUpDownPreEvent.Value;
            config.event_post_sec = (int)numUpDownPostEvent.Value;
            config.speaker_volume = (int)numUpDownSpeakerVolume.Value;

            config.speed_num_of_gear = (int)comboBoxSpeedPulseCoefficient.SelectedValue;
            config.tach_num_of_gear = (int)numUpDownTachCoefficient.Value;

            config.SpeedPulseDitectType = radioButtonSpeedCurrent.Checked ? PulseDitectType.CURRENT : PulseDitectType.VOLTAGE;
            config.TachPulseDitectType = radioButtonTachCurrent.Checked ? PulseDitectType.CURRENT : PulseDitectType.VOLTAGE;

            config.streaming_bitrate = (string)comboBoxRealtimeBitrate.SelectedValue;

            config.SetSerialDevice(SerialDevices.NamePlate, checkBoxNamePlate.Checked);
            config.SetSerialDevice(SerialDevices.Etc, checkBoxEtc.Checked);
            config.SetSerialDevice(SerialDevices.Printer, checkBoxPrinter.Checked);
            config.SetSerialDevice(SerialDevices.StatusSwitch, checkBoxStatusSwitch.Checked);

            config.odometer_km = (numUpDownOdometer.Text == string.Empty) ? 0 : (int)numUpDownOdometer.Value;

            // 速度超過(一般)
            config.speed_normal_load_limit = (int)numericUpDownNormalRoadSpeedLimit.Value;
            config.speed_normal_load_decision_time = (int)numericUpDownNormalRoadDecisionTime.Value;
            // 速度超過(高速)
            config.speed_highway_limit = (int)numericUpDownHighwaySpeedLimit.Value;
            config.speed_highway_decision_time = (int)numericUpDownHighwayDecisionTime.Value;
            // 回転数超過
            config.tacho_normal_load_limit = (int)numericUpDownEngineRPMLimit.Value;
            config.tacho_normal_load_decision_time = (int)numericUpDownEngineRPMDecisionTime.Value;
            config.update_at = DateTime.Now;
        }

        private void RemoteSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            logger.Out(OperationLogger.Category.RemoteConfig, UserName, $"Closed RemoteSetting {DeviceId}");
            MqttClient.MqttMsgPublished -= MqttClient_MqttMsgPublished;
            MqttClient.MqttMsgPublishReceived -= MqttClient_MqttMsgPublishReceived;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private MqttJsonRemoteConfig CloneConfig()
        {
            return JsonConvert.DeserializeObject<MqttJsonRemoteConfig>(JsonConvert.SerializeObject(viewModel.Config));
        }

        private void buttonSetDefault_Click(object sender, EventArgs e)
        {
            //            viewModel.Config = PermData.GetDefaultConfig();
            viewModel.Config.device_id = DeviceId;
            ConfigToUI(viewModel.Config);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("サーバーへ設定を送信します", "確認", MessageBoxButtons.YesNo
                    , MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                buttonSend.Enabled = false;
                labelRemoteStatus.Text = @"サーバーへ設定を送信中です...";

                Debug.WriteLine($"Config json: {JsonConvert.SerializeObject(viewModel.Config)}");

                //SendingData = CloneConfig();
                SendSettingData();
                //Close();      //  ここで即閉じするとうまく送れないことある
            }
        }

        private ushort msgId = 0;

        private void SendSettingData()
        {
            try
            {
                UItoConfig(viewModel.Config);
                SendingData = CloneConfig();
                var data = SendingData;
                var json = JsonConvert.SerializeObject(data);
                Debug.WriteLine($"Remote config: {json}");
                logger.Out(OperationLogger.Category.RemoteConfig, UserName, $"Send RemoteSetting {DeviceId}, {json}");

                var topic = String.Format(MQTT_TOPIC_REMOTECONFIG_REQUEST, this.DeviceId);
                var msg = System.Text.Encoding.UTF8.GetBytes(json);
                msgId = MqttClient.Publish(topic, msg, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, mqttRetain);
                Debug.WriteLine($"RemoteSetting: Publish msgId = {msgId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ERROR: RemoteConfig_Click {ex}");
            }
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var msg = System.Text.Encoding.UTF8.GetString(e.Message);
            Match match = Regex.Match(e.Topic, $@"car/error/{DeviceId}");
            if (match.Success)
            {
                MqttJsonError deviceStatus = JsonConvert.DeserializeObject<MqttJsonError>(msg);
                if (viewModel != null && viewModel.DeviceStatus != null)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        viewModel.DeviceStatus.SetStatus(deviceStatus);
                    }));
                }
            }
        }

        private void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (msgId == e.MessageId)
            {
                Invoke((MethodInvoker)(() =>
                {
                    labelRemoteStatus.Text = "サーバーへ送信しました。\r\n次回起動時に反映されます。";
                    buttonSend.Enabled = true;
                }));
            }
        }

        private void ComboBoxCamFlip_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && -1 < e.Index)
            {
                Image image = imageListCamFlipMode.Images[e.Index];
                e.Graphics.DrawImage(image, e.Bounds.X, e.Bounds.Y);
            }

            // フォーカス時の囲い線を表示する
            e.DrawFocusRectangle();
        }

        private void RemoteSetting_Shown(object sender, EventArgs e)
        {
            MqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
            MqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
        }
    }
}