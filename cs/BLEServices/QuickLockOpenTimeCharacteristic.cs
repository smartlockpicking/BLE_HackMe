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
    // this characteristic stores number of seconds the lock stays unlocked (before auto-lock again)
    public class QuickLockOpenTimeCharacteristic : GenericGattCharacteristic
    {
        public QuickLockOpenTimeCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            // for now just static 5 seconds, setting the value not yet implemented in HackMe
            Value = BLEServices.Helpers.ToIBuffer((byte)5);
        }
        
    }
}
