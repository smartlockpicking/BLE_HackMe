using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.ComponentModel;

namespace BLE_Hackme.BLEServices
{
    /// <summary>
    /// Class for Battery Services
    /// </summary>
    public class BatteryService : GenericGattService
    {
        /// <summary>
        /// Name of the service
        /// </summary>
        public override string Name
        {
            get
            {
                return "Battery Service";
            }
        }

        /// <summary>
        /// Battery level
        /// </summary>
        private BatteryLevelCharacteristic batteryLevel;

        /// <summary>
        /// Gets or sets the battery level 
        /// </summary>
        public BatteryLevelCharacteristic BatteryLevel
        {
            get
            {
                return batteryLevel;
            }

            set
            {
                if (batteryLevel != value)
                {
                    batteryLevel = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("BatteryLevel"));
                }
            }
        }

        /// <summary>
        /// Asynchronous initialization
        /// </summary>
        /// <returns>Initialization Task</returns>
        public override async Task Init()
        {
            await CreateServiceProvider(GattServiceUuids.Battery);

            // Preparing the Battery Level characteristics
            GattLocalCharacteristicParameters batteryCharacteristicsParameters = PlainReadParameters;

            // Set the user descriptions
//            batteryCharacteristicsParameters.UserDescription = "Battery Level percentage remaining";

            // Create the characteristic for the service
            GattLocalCharacteristicResult result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    GattCharacteristicUuids.BatteryLevel,
                    batteryCharacteristicsParameters);

            // Grab the characterist object from the service set it to the BatteryLevel property which is of a specfic Characteristic type
            GattLocalCharacteristic baseBatteryLevel = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseBatteryLevel);

            if (baseBatteryLevel != null)
            {
                BatteryLevel = new BLEServices.BatteryLevelCharacteristic(baseBatteryLevel, this);
            }
        }
    }
}
