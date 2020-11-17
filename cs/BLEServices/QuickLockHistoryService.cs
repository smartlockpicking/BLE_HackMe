using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.ComponentModel;

namespace BLE_Hackme.BLEServices
{
    public class QuickLockHistoryService : GenericGattService
    {

        // 0 - uninitialized, 1 - init, waiting for auth, 2 - authenticated
        public byte currentState = 0;
        // 0 - locked, 1 - unlocked
        public byte locked = 0;

        /// <summary>
        /// Name of the service
        /// </summary>
        public override string Name
        {
            get
            {
                return "Quick Lock History Service";
            }
        }

        private QuickLockHistoryControlCharacteristic quickLockHistoryControl;
        private QuickLockHistoryDataCharacteristic quickLockHistoryData;
        private QuickLockUsernameCharacteristic quickLockUsername;

        /// <summary>
        /// Getters / setters
        /// </summary>

        public QuickLockHistoryControlCharacteristic QuickLockHistoryControl
        {
            get
            {
                return quickLockHistoryControl;
            }
            set
            {
                if (quickLockHistoryControl != value)
                {
                    quickLockHistoryControl = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuickLockHistoryControl"));
                }
            }
        }

        public QuickLockHistoryDataCharacteristic QuickLockHistoryData
        {
            get
            {
                return quickLockHistoryData;
            }
            set
            {
                if (quickLockHistoryData != value)
                {
                    quickLockHistoryData = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuickLockHistory"));
                }
            }
        }

        public QuickLockUsernameCharacteristic QuickLockUsername
        {
            get
            {
                return quickLockUsername;
            }
            set
            {
                if (quickLockUsername != value)
                {
                    quickLockUsername = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuickLockUsername"));
                }
            }
        }


        /// <summary>
        /// Asynchronous initialization
        /// </summary>
        /// <returns>Initialization Task</returns>
        public override async Task Init()
        {
            await CreateServiceProvider(Constants.QuickLockHistoryServiceUuid);


            // history control 

            GattLocalCharacteristicParameters quickLockHistoryControlCharacteristicsParameters = PlainReadWriteParameters;
            // Set the user descriptions
            quickLockHistoryControlCharacteristicsParameters.UserDescription = "control!";

            // Create the characteristic for the service
            GattLocalCharacteristicResult result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.QuickLockHistoryControlCharacteristicUuid,
                    quickLockHistoryControlCharacteristicsParameters);

            // Grab the characteristic object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseQuickLockHistoryControl = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseQuickLockHistoryControl);

            if (baseQuickLockHistoryControl != null)
            {
                QuickLockHistoryControl = new BLEServices.QuickLockHistoryControlCharacteristic(baseQuickLockHistoryControl, this);
            }


            // history data

            GattLocalCharacteristicParameters quickLockHistoryDataCharacteristicsParameters = PlainReadNotifyParameters;
            // Set the user descriptions
            quickLockHistoryDataCharacteristicsParameters.UserDescription = "history!";

            // Create the characteristic for the service
            result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.QuickLockHistoryDataCharacteristicUuid,
                    quickLockHistoryDataCharacteristicsParameters);

            // Grab the characteristic object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseQuickLockHistoryData = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseQuickLockHistoryData);

            if (baseQuickLockHistoryData != null)
            {
                QuickLockHistoryData = new BLEServices.QuickLockHistoryDataCharacteristic(baseQuickLockHistoryData, this);
            }


            // "imei" (username)

            // Preparing the characteristics
            GattLocalCharacteristicParameters quickLockUsernameCharacteristicsParameters = PlainReadWriteParameters;
            // Set the user descriptions
            quickLockUsernameCharacteristicsParameters.UserDescription = "phone id!";

            // Create the characteristic for the service
            result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.QuickLockUsernameCharacteristicUuid,
                    quickLockUsernameCharacteristicsParameters);

            // Grab the characterist object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseQuickLockUsername = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseQuickLockUsername);

            if (baseQuickLockUsername != null)
            {
                QuickLockUsername = new BLEServices.QuickLockUsernameCharacteristic(baseQuickLockUsername, this);
            }

        }
    }

}
