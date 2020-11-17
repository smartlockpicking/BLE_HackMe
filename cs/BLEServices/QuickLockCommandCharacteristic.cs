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
    public class QuickLockCommandCharacteristic : GenericGattCharacteristic
    {
        public QuickLockCommandCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            Value = BLEServices.Helpers.ToIBuffer((byte)0);
        }

        protected override bool WriteRequested(GattSession session, GattWriteRequest request)
        {
            byte[] receivedWrite;
            CryptographicBuffer.CopyToByteArray(request.Value, out receivedWrite);
            var receivedWriteHexString = Helpers.ToHexString(request.Value);

            QuickLockMainService parent = (QuickLockMainService)this.ParentService;

            if (!parent.isAuthenticated)
            {
                Debug.WriteLine($"Quicklock cmd WriteRequested, but not authenticated {receivedWriteHexString} ({request.Value.Length})");
                //do not update Value, return
                OnPropertyChanged(new PropertyChangedEventArgs("Unauthenticated"));
                return true;
            }

            if ((request.Value.Length != 1) ||  ( (receivedWrite[0] != 0x00) && (receivedWrite[0] !=0x01)) ) 
            {
                Debug.WriteLine($"Quicklock cmd WriteRequested, but invalid format {receivedWriteHexString} ({request.Value.Length})");
                //do not update Value, return
                OnPropertyChanged(new PropertyChangedEventArgs("InvalidRequest"));
                return true;
            }

            Debug.WriteLine($"Quicklock cmd received: {receivedWriteHexString} ({request.Value.Length})");

            if (receivedWrite[0] == 00) // lock
            {
                Debug.WriteLine($"Locking!");
                parent.UpdateUnlockState(false);
            }
            else if (receivedWrite[0] == 01) // unlock
            {
                Debug.WriteLine($"Unlocking!");
                parent.UpdateUnlockState(true);
            }

            OnPropertyChanged(new PropertyChangedEventArgs("LockState"));

            return true;
        }
    }
}
