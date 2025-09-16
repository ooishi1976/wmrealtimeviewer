using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpgCommon;
using RealtimeViewer.Map;
using RealtimeViewer.Model;

namespace RealtimeViewer.WMShipView
{
    public static class WMTableRowExtentions
    {
        public static bool TryGetLocation(this WMDataSet.OfficeRow office, out PointLL point)
        {
            var result = false;
            point = new PointLL();
            if (office != null)
            {
                // なんか逆な気がする
                point = new PointLL(office.Longitude, office.Latitude);
                result = true;
            }
            return result;
        }

        public static bool TryGetLocation(this WMDataSet.DeviceRow device, out PointLL point)
        {
            var result = false;
            point = new PointLL();
            if (device != null && 
                int.TryParse(device.Latitude, out var _) && 
                int.TryParse(device.Longitude, out var _))
            {
                point = MapUtils.ConvertJdgToTky(device.Latitude, device.Longitude);
                result = true;
            }
            return result;
        }

        public static bool TryGetLastNotificationTime(this WMDataSet.DeviceRow device, out DateTime timestamp)
        {
            var result = false;
            timestamp = DateTime.MinValue;
            if (device != null && !string.IsNullOrEmpty(device.LastNotificationTime))
            {
                if (DateTime.TryParseExact(
                    device.LastNotificationTime, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out timestamp))
                {
                    result = true;
                }
            }
            return result;
        }

        public static int GetCode(this WMDataSet.ErrorRow error)
        {
            var result = 0;
            if (int.TryParse(error.Error, out var code))
            {
                result = code;
            }
            return result;
        }

        public static string GetMessage(this WMDataSet.ErrorRow error)
        {
            return DeviceErrorCode.MakeErrorMessage(error.GetCode());
        }

        public static Color GetColor(this WMDataSet.ErrorRow error)
        {
            return DeviceErrorCode.GetBackgroundColor(error.GetCode());
        }

    }
}
