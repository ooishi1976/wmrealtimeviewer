using System.Net.NetworkInformation;
using System.Text;

namespace RealtimeViewer.Network
{
    public class NetworkUtils
    {
        public static string GetPhysicalAddress()
        {
            string address = string.Empty;
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in interfaces)
            {
                if (OperationalStatus.Up == adapter.OperationalStatus)
                {
                    if (NetworkInterfaceType.Unknown != adapter.NetworkInterfaceType &&
                        NetworkInterfaceType.Loopback != adapter.NetworkInterfaceType)
                    {
                        var pa = adapter.GetPhysicalAddress();
                        if (pa.GetAddressBytes().Length > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var b in pa.GetAddressBytes())
                            {
                                sb.Append(string.Format(@"{0:X2}", b));
                            }
                            address = sb.ToString();
                            break;
                        }
                    }
                }
            }
            return address;
        }
    }
}
