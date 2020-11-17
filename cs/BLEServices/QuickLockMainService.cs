using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.ComponentModel;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace BLE_Hackme.BLEServices
{
    public class QuickLockMainService : GenericGattService
    {
        public bool isAuthenticated = false;
        public bool isUnlocked = false;

        /// <summary>
        /// Name of the service
        /// </summary>
        public override string Name
        {
            get
            {
                return "Quick Lock Main Service";
            }
        }

        /// <summary>
        /// true - authenticated, false - unauthenticated
        /// </summary>
        /// <param name="param"></param>
        public void UpdateAuthenticationState(bool param)
        {

            Debug.WriteLine($"Update authentication state: {param}");           

            isAuthenticated = param;
            IBuffer Value;

            if (param == true)
            {
                Value = Helpers.ToIBuffer((byte)0x01);
            } else
            {
                Value = Helpers.ToIBuffer((byte)0x00); ;
            }

            QuickLockAuth.Value = Value;
            QuickLockAuthStatus.Value = Value;
            QuickLockAuthStatus.NotifyValue();

        }

        /// <summary>
        /// true - unlocked, false - locked
        /// </summary>
        /// <param name="param"></param>
        public void UpdateUnlockState(bool param)
        {
            Debug.WriteLine($"Update unlock state: {param}");

            isUnlocked = param;
            IBuffer Value;

            if (param == true)
            {
                Value = Helpers.ToIBuffer((byte)0x01);
            }
            else
            {
                Value = Helpers.ToIBuffer((byte)0x00); ;
            }

            QuickLockCommand.Value = Value;
            QuickLockUnlockStatus.Value = Value;
            QuickLockUnlockStatus.NotifyValue();
        }


        private QuickLockAuthCharacteristic quickLockAuth;
        private QuickLockAuthStatusCharacteristic quickLockAuthStatus;
        private QuickLockOpenTimeCharacteristic quickLockOpenTime;
        private QuickLockCommandCharacteristic quickLockCommand;
        private QuickLockUnlockStatusCharacteristic quickLockUnlockStatus;


        /// <summary>
        /// Getters / setters
        /// </summary>

        public QuickLockAuthCharacteristic QuickLockAuth
        {
            get
            {
                return quickLockAuth;
            }
            set
            {
                if (quickLockAuth != value)
                {
                    quickLockAuth = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuickLockAuth"));
                }
            }
        }

        public QuickLockAuthStatusCharacteristic QuickLockAuthStatus
        {
            get
            {
                return quickLockAuthStatus;
            }
            set
            {
                if (quickLockAuthStatus != value)
                {
                    quickLockAuthStatus = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuickLockAuthStatus"));
                }
            }
        }
        public QuickLockOpenTimeCharacteristic QuickLockOpenTime
        {
            get
            {
                return quickLockOpenTime;
            }
            set
            {
                if (quickLockOpenTime != value)
                {
                    quickLockOpenTime = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuickLockOpenTime"));
                }
            }
        }

        public QuickLockCommandCharacteristic QuickLockCommand
        {
            get
            {
                return quickLockCommand;
            }
            set
            {
                if (quickLockCommand != value)
                {
                    quickLockCommand = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuickLockCommand"));
                }
            }
        }
        public QuickLockUnlockStatusCharacteristic QuickLockUnlockStatus
        {
            get
            {
                return quickLockUnlockStatus;
            }
            set
            {
                if (quickLockUnlockStatus != value)
                {
                    quickLockUnlockStatus = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("QuckLockUnlockStatus"));
                }
            }
        }



        /// <summary>
        /// Asynchronous initialization
        /// </summary>
        /// <returns>Initialization Task</returns>
        public override async Task Init()
        {
            await CreateServiceProvider(Constants.QuickLockMainServiceUuid);


            // Quicklock auth

            GattLocalCharacteristicParameters quickLockAuthCharacteristicsParameters = PlainWriteParameters;
            // Set the user descriptions
            quickLockAuthCharacteristicsParameters.UserDescription = "Password!";

            // Create the characteristic for the service
            GattLocalCharacteristicResult result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.QuickLockAuthCharacteristicUuid,
                    quickLockAuthCharacteristicsParameters);

            // Grab the characteristic object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseQuickLockAuth = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseQuickLockAuth);

            if (baseQuickLockAuth != null)
            {
                QuickLockAuth = new BLEServices.QuickLockAuthCharacteristic(baseQuickLockAuth, this);
            }


            // Quicklock auth status

            GattLocalCharacteristicParameters quickLockAuthStatusCharacteristicsParameters = PlainReadNotifyParameters;
            // Set the user descriptions
            quickLockAuthStatusCharacteristicsParameters.UserDescription = "Password Result!";

            // Create the characteristic for the service
            result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.QuickLockAuthStatusCharacteristicUuid,
                    quickLockAuthStatusCharacteristicsParameters);

            // Grab the characteristic object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseQuickLockAuthStatus = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseQuickLockAuthStatus);

            if (baseQuickLockAuthStatus != null)
            {
                QuickLockAuthStatus = new BLEServices.QuickLockAuthStatusCharacteristic(baseQuickLockAuthStatus, this);
            }


            // quick lock open time

            GattLocalCharacteristicParameters quickLockOpenTimeCharacteristicsParameters = PlainReadWriteParameters;
            // Set the user descriptions
            quickLockOpenTimeCharacteristicsParameters.UserDescription = "Open Time!";

            // Create the characteristic for the service
            result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.QuickLockOpenTimeCharacteristicUuid,
                    quickLockOpenTimeCharacteristicsParameters);

            // Grab the characteristic object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseQuickLockOpenTime = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseQuickLockOpenTime);

            if (baseQuickLockOpenTime != null)
            {
                QuickLockOpenTime = new BLEServices.QuickLockOpenTimeCharacteristic(baseQuickLockOpenTime, this);
            }


            // quick lock command

            GattLocalCharacteristicParameters quickLockCommandCharacteristicsParameters = PlainWriteParameters;
            // Set the user descriptions
            quickLockCommandCharacteristicsParameters.UserDescription = "Lock Control!";

            // Create the characteristic for the service
            result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.QuickLockCommandCharacteristicUuid,
                    quickLockCommandCharacteristicsParameters);

            // Grab the characterist object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseQuickLockCommand = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseQuickLockCommand);

            if (baseQuickLockCommand != null)
            {
                QuickLockCommand = new BLEServices.QuickLockCommandCharacteristic(baseQuickLockCommand, this);
            }


            // quick lock unlock status

            GattLocalCharacteristicParameters quickLockUnlockStatusCharacteristicsParameters = PlainReadNotifyParameters;
            // Set the user descriptions
            quickLockUnlockStatusCharacteristicsParameters.UserDescription = "Lock Status!";

            // Create the characteristic for the service
            result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.QuickLockUnlockStatusCharacteristicUuid,
                    quickLockUnlockStatusCharacteristicsParameters);

            // Grab the characterist object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseQuickLockUnlockStatus = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseQuickLockUnlockStatus);

            if (baseQuickLockUnlockStatus != null)
            {
                QuickLockUnlockStatus = new BLEServices.QuickLockUnlockStatusCharacteristic(baseQuickLockUnlockStatus, this);
            }


        }
    }

}
