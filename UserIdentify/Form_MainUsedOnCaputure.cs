using DPUruNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
        private Fmd anyFinger;

        public Form_Main()
        {
            using (Tracer tracer = new Tracer("Form_Main::Form_Main"))
            {

                InitializeComponent();
            }

            label1.Text = "認証を開始します\r\n認証装置に指を置いてください";

            userDb = new UserBioDP.UserDatabaseDP();
            userDb.Connect();
            userDb.CreateDefault();

            Fmds.Clear();

            AddUserforFMD();

        }


        private void Form_Main_Load(object sender, EventArgs e)
        {
            anyFinger = null;

            GetReaderDevice();

            if (!OpenReader())
            {
                Close();
            }

            if (!StartCaptureAsync(OnCaptured))
            {
                Close();
            }
        }

        private void Form_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            CancelCaptureAndCloseReader(OnCaptured);
        }

        private void AddUserforFMD()
        {

            foreach (var u in userDb.GetAllUsers())
            {
                Fmds.Add((int)u.Id, Fmd.DeserializeXml(u.BiometricsFmd));

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
                if (!CheckCaptureResult(captureResult)) return;


                DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);
                if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    if (resultConversion.ResultCode != Constants.ResultCode.DP_TOO_SMALL_AREA)
                    {
                        Reset = true;
                    }
                    throw new Exception(resultConversion.ResultCode.ToString());
                }

                anyFinger = resultConversion.Data;

                // See the SDK documentation for an explanation on threshold scores.
                //  threshHoldが大きいほど甘い
                int tmpScore = 2500;    //  <- こいつを小さくするほど甘くなる default=100000
                int thresholdScore = DPFJ_PROBABILITY_ONE * 1 / tmpScore;

                IdentifyResult identifyResult = Comparison.Identify(anyFinger, 0, Fmds.Values, thresholdScore, 10);
                if (identifyResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    Reset = true;
                    throw new Exception(identifyResult.ResultCode.ToString());
                }

                if (identifyResult.Indexes.Length > 0)
                {
                    SendMessageDelegate(Action.IdentificationComp, "");
                }

                Debug.WriteLine($"Identification resulted in the following number of matches: {identifyResult.Indexes.Length}" );             
            }
            catch (Exception ex)
            {
                // Send error message, then close form
                Debug.WriteLine($"@@@ Error: " + ex.Message);
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
                    Debug.WriteLine($"@@@ Error: " + result);
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
                    Debug.WriteLine($"@@@ Error: " + ex.Message);
                    return false;
                }
            }
        }

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
                            label1.Text = "認証できました";
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

        private void OutputResult(string outputPath, int resultCode, long userId = 0, string name = "")
        {
            if (outputPath.Length > 0)
            {
                string msg;
                if (resultCode == 0)
                {
                    msg = String.Format("{0}\t{1}\t{2}{3}", resultCode, userId, name, Environment.NewLine);
                }
                else
                {
                    msg = String.Format("{0}\t\t{1}", resultCode, Environment.NewLine);
                }

                try
                {
                    File.AppendAllText(outputPath, msg);
                }
                catch
                {
                    resultCode = 11;
                }
            }

            Environment.ExitCode = resultCode;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
//
        }
    }
}