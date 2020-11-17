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
    /// Class for Light Bulb Services
    /// </summary>
    public class LightBulbService : GenericGattService
    {
        /// <summary>
        /// Name of the service
        /// </summary>
        public override string Name
        {
            get
            {
                return "Light Bulb Service";
            }
        }

        /// <summary>
        /// Light bulb
        /// </summary>
        private LightBulbSwitchCharacteristic lightBulbSwitch;
        private LightBulbTTSCharacteristic lightBulbTTS;
        private LightBulbColorCharacteristic lightBulbColor;

        /// <summary>
        /// Gets or sets the light bulb state 
        /// </summary>
        public LightBulbSwitchCharacteristic LightBulbSwitch
        {
            get
            {
                return lightBulbSwitch;
            }

            set
            {
                if (lightBulbSwitch != value)
                {
                    lightBulbSwitch = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LightBulbSwitch"));
                }
            }
        }
        public LightBulbTTSCharacteristic LightBulbTTS
        {
            get
            {
                return lightBulbTTS;
            }

            set
            {
                if (lightBulbTTS != value)
                {
                    lightBulbTTS = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LightBulbTTS"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the lightbulb color 
        /// </summary>
        public LightBulbColorCharacteristic LightBulbColor
        {
            get
            {
                return lightBulbColor;
            }

            set
            {
                if (lightBulbColor != value)
                {
                    lightBulbColor = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("LightBulbColor"));
                }
            }
        }


        /// <summary>
        /// Asynchronous initialization
        /// </summary>
        /// <returns>Initialization Task</returns>
        public override async Task Init()
        {
            await CreateServiceProvider(Constants.LightBulbServiceUuid);

            //Light Bulb Switch characteristic
            GattLocalCharacteristicParameters lightBulbSwitchCharacteristicsParameters = PlainReadWriteParameters;
            // Set the user descriptions
            lightBulbSwitchCharacteristicsParameters.UserDescription = "Light bulb on/off";
            // Create the characteristic for the service
            GattLocalCharacteristicResult result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.LightBulbSwitchCharacteristicUuid,
                    lightBulbSwitchCharacteristicsParameters);

            // Grab the characterist object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseLightBulbSwitch = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseLightBulbSwitch);

            if (baseLightBulbSwitch != null)
            {
                LightBulbSwitch = new BLEServices.LightBulbSwitchCharacteristic(baseLightBulbSwitch, this);
            }



            //Light Bulb Text To Speech characteristic
            GattLocalCharacteristicParameters lightBulbTTSCharacteristicsParameters = PlainWriteOrWriteWithoutResponseParameters;
            // Set the user descriptions
            lightBulbTTSCharacteristicsParameters.UserDescription = "Light bulb text to speech";
            // Create the characteristic for the service
            result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.LightBulbTTSCharacteristicUuid,
                    lightBulbTTSCharacteristicsParameters);

            // Grab the characterist object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseLightBulbTTS = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseLightBulbTTS);

            if (baseLightBulbTTS != null)
            {
                LightBulbTTS = new BLEServices.LightBulbTTSCharacteristic(baseLightBulbTTS, this);
            }




            //Light Bulb Color characteristic
            GattLocalCharacteristicParameters lightBulbColorCharacteristicsParameters = PlainWriteOrWriteWithoutResponseParameters;
            // Set the user descriptions
            lightBulbColorCharacteristicsParameters.UserDescription = "Light bulb color/dim";
            // Create the characteristic for the service
            result =
                await ServiceProvider.Service.CreateCharacteristicAsync(
                    Constants.LightBulbColorCharacteristicUuid,
                    lightBulbColorCharacteristicsParameters);

            // Grab the characterist object from the service set it to the property which is of a specfic Characteristic type
            GattLocalCharacteristic baseLightBulbColor = null;
            BLEServices.Helpers.GetCharacteristicsFromResult(result, ref baseLightBulbColor);

            if (baseLightBulbColor != null)
            {
                LightBulbColor = new BLEServices.LightBulbColorCharacteristic(baseLightBulbColor, this);
            }

        }
    }
}
