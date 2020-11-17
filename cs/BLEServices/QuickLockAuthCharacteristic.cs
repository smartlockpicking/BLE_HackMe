using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;

namespace BLE_Hackme.BLEServices
{
    public class QuickLockAuthCharacteristic : GenericGattCharacteristic
    {
        // matching sample pcap dump
        public byte[] password = { 0x66, 0x61, 0x26, 0x66 };

        public QuickLockAuthCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            Value = BLEServices.Helpers.ToIBuffer((byte)0);
        }

        protected override bool WriteRequested(GattSession session, GattWriteRequest request)
        {
            byte[] receivedWrite;
            CryptographicBuffer.CopyToByteArray(request.Value, out receivedWrite);
            var receivedWriteHexString = Helpers.ToHexString(request.Value);

            QuickLockMainService parent = (QuickLockMainService)this.ParentService;

            //expected: 0066612666 (pcap), or 006661266600000000 (also correct)
            if ( (request.Value.Length != 5) && (request.Value.Length != 9))
            {
                Debug.WriteLine($"Smart lock auth WriteRequested, but invalid format: {receivedWriteHexString} ({request.Value.Length})");
                //do not update Value, return
                OnPropertyChanged(new PropertyChangedEventArgs("InvalidRequest"));
                return true;
            }

            //check just the first byte and password, ignore rest if provided
            if ((receivedWrite[0] != 0x00) || (receivedWrite[1] != password[0]) || (receivedWrite[2] != password[1]) || (receivedWrite[3] != password[2]) || (receivedWrite[4] != password[3]))
            {
                    Debug.WriteLine($"Smart lock auth WriteRequested: INVALID PASSWORD: {receivedWriteHexString} ({request.Value.Length})");
                    //do not update Value, return
                    OnPropertyChanged(new PropertyChangedEventArgs("InvalidPin"));
                    return true;
            }

            parent.UpdateAuthenticationState(true);
            OnPropertyChanged(new PropertyChangedEventArgs("AuthenticationState"));

            return true;
        }
    }
}
