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
        private void SetOffices(WMDataSet.OfficeTableDataTable offices)
        {
            foreach (var office in offices)
            {
                var row = ViewModel.OfficeTable.NewOfficeTableRow();
                row.OfficeId = office.OfficeId;
                row.CompanyId = office.CompanyId;
                row.Name = office.Name;
                row.Visible = office.Visible;
                row.Latitude = office.Latitude;
                row.Longitude = office.Longitude;
                ViewModel.OfficeTable.AddOfficeTableRow(row);
            }
            ViewModel.OfficeTable.AcceptChanges();
        }

        private void SetDevices(WMDataSet.DeviceTableDataTable devices)
        {
            foreach (var device in devices)
            {
                var row = ViewModel.DeviceTable.NewDeviceTableRow();
                row.OfficeId = device.OfficeId;
                row.DeviceId = device.DeviceId;
                row.CarId = device.CarId;
                row.CarNumber = device.CarNumber;
                ViewModel.DeviceTable.AddDeviceTableRow(row);
            }
            ViewModel.DeviceTable.AcceptChanges();
        }
    }
}
