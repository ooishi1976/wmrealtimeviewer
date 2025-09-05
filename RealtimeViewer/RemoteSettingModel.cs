using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeViewer
{
    /// <summary>
    /// deprecated. use WindowRemoteConfigViewModel.
    /// </summary>
    [Obsolete("このクラスの使用は非推奨です")]
    public class RemoteSettingModel
    {
        public static DevicePlace[] PlaceAndDirections = new DevicePlace[] {
            new DevicePlace() { SettingValue = 1, Name = @"1:平置き・後退", },
            new DevicePlace() { SettingValue = 2, Name = @"2:平置き・進行", },
            new DevicePlace() { SettingValue = 3, Name = @"3:平置き・ドア", },
            new DevicePlace() { SettingValue = 4, Name = @"4:縦置き・天井", },
            new DevicePlace() { SettingValue = 5, Name = @"5:縦置き・ドア", },
        };
    }

    public class DevicePlace
    {
        public int SettingValue { get; set; }
        public string Name { get; set; }
    }
}
