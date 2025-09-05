using Microsoft.VisualStudio.TestTools.UnitTesting;
using RealtimeViewer.Logger;
using RealtimeViewer.Model;
using System;

namespace UnitTestRealtimeViewer
{
    [TestClass]
    public class DeviceErrorCodeTest
    {
        int SSDError = 0x00000100;
        int CameraError1 = 0x00010000;
        int CameraError2 = 0x00020000;
        int CameraError3 = 0x00040000;
        int CameraError4 = 0x00080000;
        int CameraError5 = 0x00100000;
        int CameraError6 = 0x00200000;
        int CameraError7 = 0x00400000;
        int CameraError8 = 0x00800000;

        int SDCardError = 0x00000200;
        int NamePlateError = 0x00000800;
        int GPSError = 0x00001000;
        int ETCError = 0x00002000;
        int PrinterError = 0x00004000;
        int StatusSwitchError = 0x00008000;

        [TestMethod]
        public void TestMethodSSD()
        {
            var result = DeviceErrorCode.HasError(SSDError);
            Assert.AreEqual(result, true);
        }
        [TestMethod]
        public void TestMethodCamera()
        {
            var result = DeviceErrorCode.HasError(CameraError1);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(CameraError2);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(CameraError3);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(CameraError4);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(CameraError5);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(CameraError6);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(CameraError7);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(CameraError8);
            Assert.AreEqual(result, true);
        }
        [TestMethod]
        public void TestMethodSD()
        {
            var result = DeviceErrorCode.HasWarn(SDCardError);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(SDCardError);
            Assert.AreEqual(result, false);
        }
        [TestMethod]
        public void TestMethodName()
        {
            var result = DeviceErrorCode.HasWarn(NamePlateError);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(NamePlateError);
            Assert.AreEqual(result, false);
        }
        [TestMethod]
        public void TestMethodGPS()
        {
            var result = DeviceErrorCode.HasWarn(GPSError);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(GPSError);
            Assert.AreEqual(result, false);
        }
        [TestMethod]
        public void TestMethodETC()
        {
            var result = DeviceErrorCode.HasWarn(ETCError);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(ETCError);
            Assert.AreEqual(result, false);
        }
        [TestMethod]
        public void TestMethodPrinter()
        {
            var result = DeviceErrorCode.HasWarn(PrinterError);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(PrinterError);
            Assert.AreEqual(result, false);
        }
        [TestMethod]
        public void TestMethodSw()
        {
            var result = DeviceErrorCode.HasWarn(StatusSwitchError);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasError(StatusSwitchError);
            Assert.AreEqual(result, false);
        }
        [TestMethod]
        public void TestMethodErrWarn()
        {
            var result = DeviceErrorCode.HasError(StatusSwitchError | SSDError);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasWarn(StatusSwitchError | SSDError);
            Assert.AreEqual(result, true);
        }
        [TestMethod]
        public void TestMethodNormal()
        {
            var result = DeviceErrorCode.HasError(0);
            Assert.AreEqual(result, false);
            result = DeviceErrorCode.HasWarn(0);
            Assert.AreEqual(result, false);
        }
        [TestMethod]
        public void TestMethodNormalIgnore()
        {
            var result = DeviceErrorCode.HasError(-1);
            Assert.AreEqual(result, true);
            result = DeviceErrorCode.HasWarn(-1);
            Assert.AreEqual(result, true);
        }
    }
}
