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
    public class QuickLockHistoryDataCharacteristic : GenericGattCharacteristic
    {

        private static String LOG_END_DELIMITER = "E0E0E0E0E0E0E0E0000000000000000000000000";

        // for now just a few static log history values
        private static String[] SAMPLE_LOGS =
        {
            // user "smartlockpick" 2020-11-06 10:56:38
            "0d736d6172746c6f636b7069636b0066e8372700",
            // user "smartlockpick" 2020-11-06 13:40:00
            "0d736d6172746c6f636b7069636b00b00e382700",
            // user "hackmeuser" 2020-11-07 13:30
            "0a6861636b6d657573657200000000fc5d392700",
        };

        public QuickLockHistoryDataCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            Value = Helpers.HexStringToIBuffer(LOG_END_DELIMITER);
        }

        public async void SendLogs()
        {
            // send all the SAMPLE_LOGS
            foreach (String log in SAMPLE_LOGS)
            {
                Debug.WriteLine($"Sending log notification: {log}");
                Value = BLEServices.Helpers.HexStringToIBuffer(log);
                // short delay to imitate real device, and also allow to notice in nRF that the value is changing
                await Task.Delay(200);
                base.NotifyValue();
            }

            Debug.WriteLine($"Sending log notification end delimiter: {LOG_END_DELIMITER}");
            Value = BLEServices.Helpers.HexStringToIBuffer(LOG_END_DELIMITER);
            base.NotifyValue();
        }
    }
}
