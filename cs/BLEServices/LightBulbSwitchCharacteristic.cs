using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;
using System.ComponentModel;


namespace BLE_Hackme.BLEServices
{
    /// <summary>
    /// Implementation of the light bulb switch
    /// </summary>
    public class LightBulbSwitchCharacteristic : GenericGattCharacteristic
    {

        private DateTime previousTime, currentTime;
        private TimeSpan span;
        private int switchCounter;


        public LightBulbSwitchCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            Value = BLEServices.Helpers.ToIBuffer((byte)0);
            switchCounter = 0;

        }

        protected override bool WriteRequested(GattSession session, GattWriteRequest request)
        {

            var receivedWrite = Helpers.ToHexString(request.Value);

            // invalid input, discard
            if ( !receivedWrite.Equals("00") && !receivedWrite.Equals("01") )
            {
                Debug.WriteLine($"Light Bulb cmd WriteRequested: INVALID INPUT: {receivedWrite} ({request.Value.Length})");
                //send notification to user
                OnPropertyChanged(new PropertyChangedEventArgs("InvalidRequest"));
                //do not update Value, return
                return true;
            }

            //check for macros blink challenge
            currentTime = DateTime.Now;
            span = currentTime.Subtract(previousTime);
            Debug.WriteLine($"Time span from previoius write: {span.TotalMilliseconds} ms");
            previousTime = currentTime;
            // previus write less than 0.5s ago
            if (span.TotalMilliseconds < 500)
            {
                Debug.WriteLine("Time span less than 0.5s!");
                //check if previous value != current (light bulb swith)
                if (Value != request.Value)
                {
                    switchCounter++;
                    Debug.WriteLine($"Value switch - OK {switchCounter}");
                } else
                {
                    switchCounter = 0;
                }
            }

            if (switchCounter > 10)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("BlinkPassed"));
            }

            // Set the characteristic Value
            Value = request.Value;

            return true;
        }

    }
}
