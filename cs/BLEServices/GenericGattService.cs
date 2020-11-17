using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BLE_Hackme.BLEServices
{
    public abstract class GenericGattService : INotifyPropertyChanged
    {
        #region Generic helper parameters
        /// <summary>
        /// Gatt Local characteristics parameter for Reading Parameters
        /// </summary>
        protected static readonly GattLocalCharacteristicParameters PlainReadParameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = GattCharacteristicProperties.Read,
            WriteProtectionLevel = GattProtectionLevel.Plain,
            ReadProtectionLevel = GattProtectionLevel.Plain
        };

        /// <summary>
        /// Gatt Local characteristics parameter for Writing without response (WRITE CMD) Parameters
        /// </summary>
        protected static readonly GattLocalCharacteristicParameters PlainWriteWithoutResponseParameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = GattCharacteristicProperties.WriteWithoutResponse,
            WriteProtectionLevel = GattProtectionLevel.Plain
        };

        /// <summary>
        /// Gatt Local characteristics parameter for Writing with response
        /// </summary>
        protected static readonly GattLocalCharacteristicParameters PlainWriteParameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = GattCharacteristicProperties.Write,
            WriteProtectionLevel = GattProtectionLevel.Plain
        };


        /// <summary>
        /// Gatt Local characteristics parameter for Writing Parameters
        /// </summary>
        protected static readonly GattLocalCharacteristicParameters PlainWriteOrWriteWithoutResponseParameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = GattCharacteristicProperties.Write | GattCharacteristicProperties.WriteWithoutResponse,
            WriteProtectionLevel = GattProtectionLevel.Plain
        };



        /// <summary>
        /// Gatt Local characteristics parameter for Reading and Notifying Parameters
        /// </summary>
        protected static readonly GattLocalCharacteristicParameters PlainReadNotifyParameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = GattCharacteristicProperties.Read | GattCharacteristicProperties.Notify,
            ReadProtectionLevel = GattProtectionLevel.Plain,
            WriteProtectionLevel = GattProtectionLevel.Plain
        };

        /// <summary>
        /// Gatt Local characteristics parameter for Notifying Parameters
        /// </summary>
        protected static readonly GattLocalCharacteristicParameters PlainNotifyParameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = GattCharacteristicProperties.Notify,
            WriteProtectionLevel = GattProtectionLevel.Plain
        };

        /// <summary>
        /// Gatt Local characteristics parameter for Indicating Parameters
        /// </summary>
        protected static readonly GattLocalCharacteristicParameters PlainIndicateParameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = GattCharacteristicProperties.Indicate,
            WriteProtectionLevel = GattProtectionLevel.Plain
        };

        protected static readonly GattLocalCharacteristicParameters PlainReadWriteParameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = GattCharacteristicProperties.Read | GattCharacteristicProperties.Write,
            WriteProtectionLevel = GattProtectionLevel.Plain
        };



        #endregion

        #region INotifyPropertyChanged requirements
        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property changed method
        /// </summary>
        /// <param name="e">Property that changed</param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
        #endregion

        /// <summary>
        /// Gets the name of the characteristic
        /// </summary>
        public abstract string Name
        {
            get;
        }

        private bool isPublishing = false;
        public bool IsPublishing
        {
            get
            {
                return isPublishing;
            }

            private set
            {
                if (value != isPublishing)
                {
                    isPublishing = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsPublishing"));
                }
            }
        }


        public bool IsConnectable
        {
            get
            {
                return ad.IsConnectable;
            }

            set
            {
                if (value != ad.IsConnectable)
                {
                    ad.IsConnectable = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsConnectable"));
                }
            }
        }

        public bool IsDiscoverable
        {
            get
            {
                return ad.IsDiscoverable;
            }

            set
            {
                if (value != ad.IsDiscoverable)
                {
                    ad.IsDiscoverable = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsDiscoverable"));
                }
            }
        }

        /// <summary>
        /// Internal ServiceProvider
        /// </summary>
        private GattServiceProvider serviceProvider;

        private GattServiceProviderAdvertisingParameters ad = new GattServiceProviderAdvertisingParameters();

        /// <summary>
        /// Gets or sets the Gatt Service Provider
        /// </summary>
        public GattServiceProvider ServiceProvider
        {
            get
            {
                return serviceProvider;
            }

            protected set
            {
                if (serviceProvider != value)
                {
                    serviceProvider = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ServiceProvider"));
                    serviceProvider.AdvertisementStatusChanged += ServiceProvider_AdvertisementStatusChanged;
                }
            }
        }

        private void ServiceProvider_AdvertisementStatusChanged(GattServiceProvider sender, GattServiceProviderAdvertisementStatusChangedEventArgs args)
        {

            Debug.WriteLine($"ServiceProvider ({this.Name}) advertisementstatuschanged : {args.Status}");

            if (args.Status == GattServiceProviderAdvertisementStatus.Started)
            {
                isPublishing = true;
            } else
            {
                Debug.WriteLine($"ServiceProvider ({this.Name}) advertisementstatuschanged - ERROR!");
                IsPublishing = false;
                OnPropertyChanged(new PropertyChangedEventArgs("ServiceStartError"));
            }
        }

        /// <summary>
        /// Abstract method used to initialize this class
        /// </summary>
        /// <returns>Tasks that initializes the class</returns>
        public abstract Task Init();

        /// <summary>
        /// Starts the Gatt Service
        /// </summary>
        public virtual void Start()
        {
            try
            {
                ServiceProvider.StartAdvertising(ad);
                IsPublishing = true;
                OnPropertyChanged(new PropertyChangedEventArgs("IsPublishing"));
            }
            catch (Exception)
            {
                Debug.WriteLine($"Exception while start Advertising {ServiceProvider.Service.Uuid}");
                IsPublishing = false;
                throw;
            }
        }

        /// <summary>
        /// Stops the already running Service
        /// </summary>
        public virtual void Stop()
        {
            try
            {
                ServiceProvider.StopAdvertising();
                IsPublishing = false;
                OnPropertyChanged(new PropertyChangedEventArgs("IsPublishing"));
            }
            catch (Exception)
            {
                Debug.WriteLine($"Exception while Stop Advertising {ServiceProvider.Service.Uuid}");
                IsPublishing = true;
                throw;
            }
        }

        /// <summary>
        /// Creates the Gatt Service provider
        /// </summary>
        /// <param name="uuid">UUID of the Service to create</param>
        protected async Task CreateServiceProvider(Guid uuid)
        {
            // Create Service Provider - similar to RFCOMM APIs
            GattServiceProviderResult result = await GattServiceProvider.CreateAsync(uuid);

            if (result.Error != BluetoothError.Success)
            {
                throw new System.Exception(string.Format($"Error occured while creating the BLE service provider, Error Code:{result.Error}"));
            }

            ServiceProvider = result.ServiceProvider;
        }


    }
}
