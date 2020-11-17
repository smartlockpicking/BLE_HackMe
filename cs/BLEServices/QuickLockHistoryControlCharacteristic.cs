using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using System.ComponentModel;

namespace BLE_Hackme.BLEServices
{
    public class QuickLockHistoryControlCharacteristic : GenericGattCharacteristic
    {
        public QuickLockHistoryControlCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            Value = Helpers.ToIBuffer((byte)1);
        }

        // if received "01", send logs via notification from HistoryData characteristic
        protected override bool WriteRequested(GattSession session, GattWriteRequest request)
        {
            byte[] receivedWrite;
            CryptographicBuffer.CopyToByteArray(request.Value, out receivedWrite);
            var receivedWriteHexString = Helpers.ToHexString(request.Value);

            QuickLockHistoryService parent = (QuickLockHistoryService)this.ParentService;

            if ((request.Value.Length != 1) || (receivedWrite[0] != 0x01))
            {
                Debug.WriteLine($"Quicklock history cmd WriteRequested, but invalid format: {receivedWriteHexString} ({request.Value.Length})");
                //do not update Value, return
                OnPropertyChanged(new PropertyChangedEventArgs("InvalidRequest"));
                return true;
            }

            Debug.WriteLine($"Quicklock history cmd received, sending out logs");

            //    OnPropertyChanged(new PropertyChangedEventArgs("Logs"));

            // set the status characteristic and send notification
            parent.QuickLockHistoryData.SendLogs();

            return true;
        }

    }
}
