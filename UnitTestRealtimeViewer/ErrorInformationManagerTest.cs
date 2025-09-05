using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using RealtimeViewer.Logger;
using RealtimeViewer.Model;
using RealtimeViewer.Network.Mqtt;

namespace UnitTestRealtimeViewer
{
    /// <summary>
    /// UnitTest1 の概要の説明
    /// </summary>
    [TestClass]
    public class ErrorInformationManagerTest
    {
        private readonly int SSDError = 0x00000100;
        private readonly int CameraError1 = 0x00010000;
        private readonly int CameraError2 = 0x00020000;
        private readonly int CameraError3 = 0x00040000;
        private readonly int CameraError4 = 0x00080000;
        private readonly int CameraError5 = 0x00100000;
        private readonly int CameraError6 = 0x00200000;
        private readonly int CameraError7 = 0x00400000;
        private readonly int CameraError8 = 0x00800000;

        private readonly int SDCardError = 0x00000200;
        private readonly int NamePlateError = 0x00000800;
        private readonly int GPSError = 0x00001000;
        private readonly int ETCError = 0x00002000;
        private readonly int PrinterError = 0x00004000;
        private readonly int StatusSwitchError = 0x00008000;

        private ErrorInformationManager manager;
        public ErrorInformationManagerTest()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize()]
        public void MyTestInitialize() {
            manager = new ErrorInformationManager(null);
        }

        [TestMethod]
        public void TestMethodDefault()
        {
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, false);
        }

        [TestMethod]
        public void TestMethodNormal()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = "0",
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, false);
        }

        [TestMethod]
        public void TestMethodError()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = SSDError.ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, true);
            Assert.AreEqual(manager.HasWarn, false);
        }

        [TestMethod]
        public void TestMethodWarn()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = SDCardError.ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, true);
        }

        [TestMethod]
        public void TestMethodWarnAndError()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = (SDCardError | SSDError).ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, true);
            Assert.AreEqual(manager.HasWarn, true);
        }

        [TestMethod]
        public void TestMethodWarnAfterNormal()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = (SDCardError).ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, true);

            error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = "0",
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, false);
        }

        [TestMethod]
        public void TestMethodErrorAfterNormal()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = SSDError.ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, true);
            Assert.AreEqual(manager.HasWarn, false);

            error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = "0",
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, false);

        }

        [TestMethod]
        public void TestMethodErrorAfterWarn()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = SSDError.ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, true);
            Assert.AreEqual(manager.HasWarn, false);

            error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = SDCardError.ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, true);
        }

        [TestMethod]
        public void TestMethodErrorWithWarnAfterError()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = (SSDError | SDCardError).ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, true);
            Assert.AreEqual(manager.HasWarn, true);

            error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = SSDError.ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, true);
            Assert.AreEqual(manager.HasWarn, false);
        }

        [TestMethod]
        public void TestMethodErrorWithWarnAfterWarn()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = (SSDError | SDCardError).ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, true);
            Assert.AreEqual(manager.HasWarn, true);

            error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = SDCardError.ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, true);
        }

        [TestMethod]
        public void TestMethodErrorWithWarnAfterWarnAfterNormal()
        {
            MqttJsonError error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = (SSDError | SDCardError).ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, true);
            Assert.AreEqual(manager.HasWarn, true);

            error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = SDCardError.ToString(),
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, true);

            error = new MqttJsonError()
            {
                DeviceId = "9999",
                Error = "0",
                Ts = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            manager.AddError(error);
            Assert.AreEqual(manager.HasError, false);
            Assert.AreEqual(manager.HasWarn, false);
        }
    }
}
