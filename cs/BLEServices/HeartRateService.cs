using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BLE_Hackme.BLEServices
{
    public class HeartRateService : GenericGattService
    {
        /// <summary>
        /// Name of the service
        /// </summary>
        public override string Name
        {
            get
            {
                return "Heart Rate Service";
            }
        }

        /// <summary>
        /// This characteristic is used to send a heart rate measurement.
        /// </summary>
        private HeartRateMeasurementCharacteristic heartRateMeasurement;

        public String randomForDescriptor { get; private set; }

        /// <summary>
        /// Gets or Sets the heart rate characteristic
        /// </summary>
        public HeartRateMeasurementCharacteristic HeartRateMeasurement
        {
            get
            {
                return heartRateMeasurement;
            }

            set
            {
                if (heartRateMeasurement != value)
                {
                    heartRateMeasurement = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("HeartRateMeasurement"));
                }
            }
        }

        /// <summary>
        /// Starts the Heart rate service
        /// </summary>
        public override async Task Init()
        {
            await CreateServiceProvider(GattServiceUuids.HeartRate);

            // Preparing the heart rate characteristics
            var heartRateCharacteristicsParameters = PlainReadNotifyParameters;

            //random 4 digits added to manufacturer specific advertisement text
            Random rnd = new Random();
            randomForDescriptor = Convert.ToString(rnd.Next(1000, 9999));

            heartRateCharacteristicsParameters.UserDescription = $"{Constants.heartRateDescriptorText}{randomForDescriptor}";

            // Create the heart rate characteristic for the service
            GattLocalCharacteristicResult result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    GattCharacteristicUuids.HeartRateMeasurement,
                    heartRateCharacteristicsParameters);

            // Grab the characteristic object from the service set it to the HeartRate property which is of a specfic Characteristic type
            GattLocalCharacteristic baseHeartRateMeasurement = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseHeartRateMeasurement);

            if (baseHeartRateMeasurement != null)
            {
                HeartRateMeasurement = new BLEServices.HeartRateMeasurementCharacteristic(baseHeartRateMeasurement, this);
            }
        }


    }
}
