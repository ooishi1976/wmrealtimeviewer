using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeViewer.WMShipView
{
    public partial class MainForm : Form
    {
        private void SetOffices(WMDataSet.OfficeDataTable offices)
        {
            foreach (var office in offices)
            {
                var row = ViewModel.OfficeTable.NewOfficeRow();
                row.OfficeId = office.OfficeId;
                row.CompanyId = office.CompanyId;
                row.Name = office.Name;
                row.Visible = office.Visible;
                row.Latitude = office.Latitude;
                row.Longitude = office.Longitude;
                ViewModel.OfficeTable.AddOfficeRow(row);
            }
            ViewModel.OfficeTable.AcceptChanges();
        }

        private void SetDevices(WMDataSet.DeviceDataTable devices)
        {
            foreach (var device in devices)
            {
                var row = ViewModel.DeviceTable.NewDeviceRow();
                row.OfficeId = device.OfficeId;
                row.DeviceId = device.DeviceId;
                row.CarId = device.CarId;
                row.CarNumber = device.CarNumber;
                ViewModel.DeviceTable.AddDeviceRow(row);
            }
            ViewModel.DeviceTable.AcceptChanges();
        }
    }
}
