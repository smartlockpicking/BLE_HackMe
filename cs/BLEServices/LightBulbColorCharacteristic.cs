using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;
using System.ComponentModel;
using Windows.Security.Cryptography;

namespace BLE_Hackme.BLEServices
{
    public class LightBulbColorCharacteristic : GenericGattCharacteristic
    {
        public byte[] pin = { 0, 0, 0 };
        public byte[] argb = { 0, 0, 0, 0 };
        public byte specialEffectsScenario = 0;

        public LightBulbColorCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            // argb
            Value = BLEServices.Helpers.ToIBuffer(argb);

            //generate random PIN
            Random rnd = new Random();
            pin[0] = (byte)rnd.Next(0, 10);
            pin[1] = (byte)rnd.Next(0, 10);
            pin[2] = (byte)rnd.Next(0, 10);
        }

        protected override bool WriteRequested(GattSession session, GattWriteRequest request)
        {
            byte[] receivedWrite;
            CryptographicBuffer.CopyToByteArray(request.Value, out receivedWrite);
            var receivedWriteHexString = Helpers.ToHexString(request.Value);

            if ((request.Value.Length != 6) || (receivedWrite[5] != 0xFF))
            {
                Debug.WriteLine($"Light Bulb color Write requested, INVALID INPUT: {receivedWriteHexString} ({request.Value.Length})");
                //send notification to user
                OnPropertyChanged(new PropertyChangedEventArgs("InvalidRequest"));

                //do not update Value, return
                return true;
            }

            // ARGB request
            if (receivedWrite[0] == 0xAA)
            {
                ProcessARGB(receivedWrite);
            }

            // password protected request
            else if (receivedWrite[0] == 0xBE)
            {
                ProcessPassProtected(receivedWrite);
            }

            //send to scenario
            return true;
        }

        private bool ProcessARGB(byte[] req)
        {
            // Set the characteristic Value - strip beginning 0xAA and ending 0xFF, leave 4 bytes ARGB
            argb = req.Skip(1).Take(4).ToArray();
            Value = Helpers.ToIBuffer(argb);
            return true;
        }

        private bool ProcessPassProtected(byte[] req)
        {
            //check PIN
            if ((req[1] != pin[0]) || (req[2] != pin[1]) || (req[3] != pin[2]))
            {
                Debug.WriteLine($"Light Bulb protected INVALID PIN: {req[1]}{req[2]}{req[3]},  expected {pin[0]}{pin[1]}{pin[2]}");
                OnPropertyChanged(new PropertyChangedEventArgs("InvalidPin"));
                return false;
            }

            //do not store in the Value, only in the local var
            specialEffectsScenario = req[4];
            //notify scenario
            OnPropertyChanged(new PropertyChangedEventArgs("SpecialValue"));
            return true;
        }
    }
}
