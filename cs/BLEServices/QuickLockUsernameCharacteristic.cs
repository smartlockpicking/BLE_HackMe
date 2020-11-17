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
    public class QuickLockUsernameCharacteristic : GenericGattCharacteristic
    {

        public QuickLockUsernameCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            // "smartlockpick" user
            Value = Helpers.HexStringToIBuffer("0d736d6172746c6f636b7069636b00");
        }

        protected override bool WriteRequested(GattSession session, GattWriteRequest request)
        {
            byte[] receivedWrite;
            CryptographicBuffer.CopyToByteArray(request.Value, out receivedWrite);
            var receivedWriteHexString = Helpers.ToHexString(request.Value);

            // valid format: one byte of length, followed by ascii hex username, and padded with 00 up to 15 bytes
            // for now just check the length and accept 15 chars only
            if ( (request.Value.Length != 15) )
                {
                    Debug.WriteLine($"Quicklock username WriteRequested, wrong length: {receivedWriteHexString} ({request.Value.Length})");
                    //do not update Value, return
                    OnPropertyChanged(new PropertyChangedEventArgs("InvalidRequest"));
                    return true;
            }

            // Set the characteristic Value to the value received
            Value = request.Value;
            return true;
        }

    }
}
