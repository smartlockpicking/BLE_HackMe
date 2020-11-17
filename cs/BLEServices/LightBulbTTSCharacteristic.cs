using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;
using Windows.Security.Cryptography;
using Windows.Devices.SerialCommunication;
using System.ComponentModel;

namespace BLE_Hackme.BLEServices
{

    public class LightBulbTTSCharacteristic : GenericGattCharacteristic
    {

        public LightBulbTTSCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            Value = BLEServices.Helpers.ToIBuffer(0);
        }

        protected override bool WriteRequested(GattSession session, GattWriteRequest request)
        {
            //check if this is write command not write request
            if (request.Option == GattWriteOption.WriteWithResponse)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("InvalidRequest"));
                Debug.WriteLine("Write request");
            }
            else
            {
                Debug.WriteLine("Write command");
                Value = request.Value;
            }
            return true;
        }
    }
}
