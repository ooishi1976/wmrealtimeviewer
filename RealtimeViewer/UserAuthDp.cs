using DPUruNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RealtimeViewer
{
    public class UserAuthDp : IDisposable
    {
        public Dictionary<Fmd, int> FmdDic;
        public List<Fmd> FmdList;

        private ReaderCollection _readers;
        private Reader currentReader;

        public static UserBioDP.UserDatabaseDP userDb;

        private const int DPFJ_PROBABILITY_ONE = 0x7fffffff;
        private DPCtlUruNet.IdentificationControl identificationControl;

        Dictionary<int, UserBioDP.User> m_UserTable = new Dictionary<int, UserBioDP.User>();

        public delegate void IdentifiedResultEventHandler(object sender, IdentifiedResult args);
        public event EventHandler<IdentifiedResult> IdentifiedResultEvent;

        public bool HasChangePermission => userDb.HasChangePermission;

        public bool IsReadOnly => userDb.IsReadOnly;

        public UserAuthDp()
        {
            FmdDic = new Dictionary<Fmd, int>();
            FmdList = new List<Fmd>();

            userDb = new UserBioDP.UserDatabaseDP();
            userDb.Connect();
            userDb.CreateDefault();

            AddUserforFMD();
        }


        public void Dispose()
        {
            if (userDb != null)
            {
                userDb.Dispose();
            }

            if (identificationControl != null)
            {
                identificationControl.StopIdentification();
            }
        }

        public int StartAuth()
        {
            if (GetReaderDevice() < 0)
            {
                return -1;
            }

            if (identificationControl != null)
            {
                identificationControl.Reader = currentReader;
            }
            else
            {
                // See the SDK documentation for an explanation on threshold scores.
                //  threshHoldが大きいほど甘い
                int tmpScore = 2500;    //  <- こいつを小さくするほど甘くなる default=100000
                int thresholdScore = DPFJ_PROBABILITY_ONE * 1 / tmpScore;

                identificationControl = new DPCtlUruNet.IdentificationControl(currentReader, FmdList, thresholdScore, 1, Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
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

            return 0;
        }

        private void AddUserforFMD()
        {
            foreach (var u in userDb.GetAllUsers())
            {
                var f = Fmd.DeserializeXml(u.BiometricsFmd);
                FmdList.Add(f);
                FmdDic.Add(f, (int)u.Id);
                m_UserTable.Add((int)u.Id, u);
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
                        if (currentReader != null)
                        {
                            currentReader.Dispose();
                            currentReader = null;
                        }
                    }
                }
                else
                {
                    if (currentReader != null)
                    {
                        currentReader.Dispose();
                        currentReader = null;
                    }

                    Debug.WriteLine($"@@@ Error: {IdentificationResult.ResultCode} ");
                }
            }
            else
            {
                currentReader = IdentificationControl.Reader;
                Debug.WriteLine($"OnIdentify: { IdentificationResult.Indexes.Length } ");

                if (IdentificationResult.Indexes.Length > 0)
                {
                    var index = IdentificationResult.Indexes[0][0];
                    var f = FmdList[index];
                    if (FmdDic.ContainsKey(f))
                    {
                        var userId = FmdDic[f];
                        var result = new IdentifiedResult() { ResultCode = 0, User = m_UserTable[userId], };
                        OnRaiseIdentifiedResultEvent(result);
                    }
                }
            }

        }

        private int GetReaderDevice()
        {
            int ret = 0;
            try
            {
                _readers = ReaderCollection.GetReaders();

                foreach (Reader Reader in _readers)
                {
                    Debug.WriteLine($"ReaderName: {Reader.Description.Name}");
                }
                currentReader = _readers[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"@@@ Exception: {ex} {ex.StackTrace}");
                ret = -1;
            }

            return ret;
        }

        protected virtual void OnRaiseIdentifiedResultEvent(IdentifiedResult e)
        {
            IdentifiedResultEvent?.Invoke(this, e);
        }
    }

    public class IdentifiedResult
    {
        /// <summary>
        /// 0 is found a user. Or Error code.
        /// </summary>
        public int ResultCode { get; set; }

        public UserBioDP.User User { get; set; }
    }
}
