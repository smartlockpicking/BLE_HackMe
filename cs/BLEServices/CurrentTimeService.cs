using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.ComponentModel;

namespace BLE_Hackme.BLEServices
{
    /// <summary>
    /// Class for Current time service
    /// </summary>
    public class CurrentTimeService : GenericGattService
    {
        /// <summary>
        /// Name of the service
        /// </summary>
        public override string Name
        {
            get
            {
                return "Current Time Service (Quicklock)";
            }
        }

        /// <summary>
        /// Current time characteristic
        /// </summary>
        private GenericGattCharacteristic currentTime;

        /// <summary>
        /// Gets or sets the currentTime
        /// </summary>
        public GenericGattCharacteristic CurrentTime
        {
            get
            {
                return currentTime;
            }

            set
            {
                if (currentTime != value)
                {
                    currentTime = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentTime"));
                }
            }
        }

        /// <summary>
        /// Asynchronous initialization
        /// </summary>
        /// <returns>Initialization Task</returns>
        public override async Task Init()
        {
            await CreateServiceProvider(GattServiceUuids.CurrentTime);

            GattLocalCharacteristicParameters currentTimeCharacteristicsParameters = PlainReadNotifyParameters;
            // Set the user descriptions
            currentTimeCharacteristicsParameters.UserDescription = "Current Time!";


            GattLocalCharacteristicResult result = await ServiceProvider.Service.CreateCharacteristicAsync(
                GattCharacteristicUuids.CurrentTime,
                PlainReadNotifyParameters);

            GattLocalCharacteristic currentTimeCharacterisitic = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref currentTimeCharacterisitic);
            if (currentTimeCharacterisitic != null)
            {
                CurrentTime = new BLEServices.CurrentTimeCharacteristic(currentTimeCharacterisitic, this);
            }
        }
    }
}
