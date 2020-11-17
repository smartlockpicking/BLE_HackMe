using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;


namespace BLE_Hackme.BLEServices
{
    /// <summary>
    /// Implementation of the battery profile
    /// </summary>
    public class BatteryLevelCharacteristic : GenericGattCharacteristic
    {

        public int batteryLevelIndication;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatteryLevelCharacteristic" /> class.
        /// </summary>
        /// <param name="characteristic">The characteristic that this wraps</param>
        public BatteryLevelCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            // generate random value 1-100% 
            Random rnd = new Random();
            batteryLevelIndication = rnd.Next(1, 100);
            Value = BLEServices.Helpers.ToIBuffer(batteryLevelIndication);
        }
    }

}
