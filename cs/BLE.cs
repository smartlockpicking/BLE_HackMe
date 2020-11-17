using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth.Advertisement;
using System.Diagnostics;


namespace BLE_Hackme
{
    public class BLE
    {
        BluetoothLEAdvertisementPublisher publisher;

        public BLEServices.HeartRateService heartRateService = null;
        public BLEServices.BatteryService batteryService = null;
        public BLEServices.CurrentTimeService currentTimeService = null;
        public BLEServices.LightBulbService lightBulbService = null;
        public BLEServices.QuickLockMainService quickLockMainService = null;
        public BLEServices.QuickLockHistoryService quickLockHistoryService = null;

        public bool peripheralSupported;
        public bool isCurrentlyStarting = false;

        public bleSimulatedDevice currentSimulatedDevice = bleSimulatedDevice.Unknown;
        public enum bleSimulatedDevice
        {
            LightBulb,
            iBeacon,
            ManufacturerSpecific,
            QuickLock,
            Unknown
        }

        public ushort iBeaconMajor { get; private set; }
        public ushort iBeaconMinor { get; private set; }
        public String randomForManufacturerSpecific { get; private set; }

        public BLE()
        {
            publisher = new BluetoothLEAdvertisementPublisher();
            publisher.StatusChanged += OnPublisherStatusChanged;

            //generate new random iBeacon numbers
            Random rnd = new Random();
            iBeaconMajor = (ushort) rnd.Next(0, 65535);
            iBeaconMinor = (ushort) rnd.Next(0, 65535);

            //random 4 digits added to manufacturer specific advertisement text
            randomForManufacturerSpecific = Convert.ToString(rnd.Next(1000, 9999));
        }


        public async Task<bool> AdvertisingForScenario(String currentScenario)
        {

            Debug.WriteLine($"Setting up device for scenario: {currentScenario}");
            var ret = true;
            switch (currentScenario)
            {
                case "Scenario_04_Beacons":
                    ret = await startBleDevice(bleSimulatedDevice.iBeacon);
                    break;
                case "Scenario_05_ManufacturerAdvertisements":
                    ret = await startBleDevice(bleSimulatedDevice.ManufacturerSpecific);
                    break;
                case "Scenario_15_QuickLockReplay":
                case "Scenario_16_QuickLockLogs":
                    ret = await startBleDevice(bleSimulatedDevice.QuickLock);
                    break;
                default:
                    ret = await startBleDevice(bleSimulatedDevice.LightBulb);
                    break;
            }
            return ret;
        }

        private async Task<bool> startBleDevice(bleSimulatedDevice deviceToStart)
        {
            var ret = true;
            if (currentSimulatedDevice != deviceToStart)
            {
                // check if previous start finished
                if (isCurrentlyStarting)
                {
                    Debug.WriteLine("Currently starting simulation, aborting this request...");
                    ret = false;
                }
                else
                {
                    StopAdvertisingServices();
                    StopAdvertisingCustom();

                    switch (deviceToStart)
                    {
                        case bleSimulatedDevice.LightBulb:
                            ret = await StartAdvertisingLightbulb();
                            break;
                        case bleSimulatedDevice.iBeacon:
                            ret = StartAdvertisingIBeacon();
                            break;
                        case bleSimulatedDevice.ManufacturerSpecific:
                            ret = StartAdvertisingFF();
                            break;
                        case bleSimulatedDevice.QuickLock:
                            ret = await StartAdvertisingQuickLock();
                            break;
                        default:
                            ret = false;
                            break;
                    }

                }

                if (ret)
                {
                    currentSimulatedDevice = deviceToStart;
                    Debug.WriteLine($"Setting up {deviceToStart} device!");
                } else
                {
                    // something happened during device initialization, probably does not advertise at all
                    currentSimulatedDevice = bleSimulatedDevice.Unknown;
                    Debug.WriteLine($"Error setting up {deviceToStart} device!");
                }
            } else
            {
                Debug.WriteLine($"Already advertising device {deviceToStart}");
            }
            return ret;
        }

        public async Task<bool> CheckPeripheralRoleSupportAsync()
        {
            try
            {
                // BT_Code: New for Creator's Update - Bluetooth adapter has properties of the local BT radio.
                var localAdapter = await BluetoothAdapter.GetDefaultAsync();
                if (localAdapter != null)
                {
                    peripheralSupported = localAdapter.IsPeripheralRoleSupported;
                }
                else
                {
                    // unsupported
                    peripheralSupported = false;
                }
            }
            catch 
            {
                peripheralSupported = false;
            }
            return peripheralSupported;
        }

        public async Task<bool> StartAdvertisingLightbulb()
        {
            isCurrentlyStarting = true;

            batteryService = new BLEServices.BatteryService();
            //check if we can start service
            try
            {
                await batteryService.Init();
            }
            catch  // e.g. when Bluetooth adapter is off
            {
                Debug.WriteLine("Service start exception, probably Bluetooth adapter is off!");
                return false;
            }
            batteryService.IsConnectable = true;
            batteryService.IsDiscoverable = true;
            batteryService.Start();

            // we could start first one, so the others should also go (no need for try/catch ?)
            heartRateService = new BLEServices.HeartRateService();
            await heartRateService.Init();
            heartRateService.IsConnectable = false;
            heartRateService.IsDiscoverable = false;
            heartRateService.Start();

            lightBulbService = new BLEServices.LightBulbService();
            await lightBulbService.Init();
            lightBulbService.IsConnectable = false;
            lightBulbService.IsDiscoverable = false;
            lightBulbService.Start();

            // wait 0.5s for the service publishing callbacks to kick in
            await Task.Delay(500);

            // the service publishing callback can hit back with error a bit later
            // checking actual state
            if ( (!batteryService.IsPublishing) || (!heartRateService.IsPublishing) || (!lightBulbService.IsPublishing))
            {
                Debug.WriteLine("Service start exception, isPublishing = false (from service callback?), restart Bluetooth adapter?");
                isCurrentlyStarting = false;
                return false;
            }
            else 
            {
                isCurrentlyStarting = false;
                return true;
            }
        }


        public async Task<bool> StartAdvertisingQuickLock()
        {
            isCurrentlyStarting = true;

            batteryService = new BLEServices.BatteryService();
            //check if we can start service
            try
            {
                await batteryService.Init();
            }
            catch  // e.g. when Bluetooth adapter is off
            {
                Debug.WriteLine("Service start exception, probably Bluetooth adapter is off!");
                return false;
            }
            batteryService.IsConnectable = false;
            batteryService.IsDiscoverable = false;
            batteryService.Start();

            currentTimeService = new BLEServices.CurrentTimeService();
            await currentTimeService.Init();
            currentTimeService.IsConnectable = false;
            currentTimeService.IsDiscoverable = false;
            currentTimeService.Start();

            quickLockMainService = new BLEServices.QuickLockMainService();
            await quickLockMainService.Init();
            quickLockMainService.IsConnectable = true; // advertise in services list
            quickLockMainService.IsDiscoverable = true;
            quickLockMainService.Start();

            quickLockHistoryService = new BLEServices.QuickLockHistoryService();
            await quickLockHistoryService.Init();
            quickLockHistoryService.IsConnectable = false;
            quickLockHistoryService.IsDiscoverable = false;
            quickLockHistoryService.Start();

            // wait 0.5s for the service publishing callbacks to kick in
            await Task.Delay(500);

            // the service publishing callback can hit back with error a bit later
            // checking actual state 
            if ((!batteryService.IsPublishing) || (!currentTimeService.IsPublishing) || (!quickLockMainService.IsPublishing) || (!quickLockHistoryService.IsPublishing))
            {
                Debug.WriteLine("Service start exception, isPublishing = false (from service callback?), restart Bluetooth adapter?");
                isCurrentlyStarting = false;
                return false;
            } else
            {
                isCurrentlyStarting = false;
                return true;
            }
        }

        public bool StopAdvertisingServices()
        {
            if (heartRateService != null)
            {
                heartRateService.Stop();
                heartRateService = null;
            }

            if (batteryService != null)
            {
                batteryService.Stop();
                batteryService = null;
            }

            if (currentTimeService != null)
            {
                currentTimeService.Stop();
                currentTimeService = null;
            }

            if (lightBulbService != null)
            {
                lightBulbService.Stop();
                lightBulbService = null;
            }

            if (quickLockMainService != null)
            {
                quickLockMainService.Stop();
                quickLockMainService = null;
            }

            if (quickLockHistoryService != null)
            {
                quickLockHistoryService.Stop();
                quickLockHistoryService = null;
            }

            return true;
        }


        public bool StopAdvertisingCustom()
        {
            if (publisher.Status != BluetoothLEAdvertisementPublisherStatus.Stopped)
            {
                publisher.Stop();
                return true;
            }

            return false;
        }


        /// custom advertisements according to:
        /// https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.advertisement.bluetoothleadvertisementpublisher

        public bool StartAdvertisingCustom(ushort companyid, IBuffer data)
        {
            isCurrentlyStarting = true;
            // Add custom data to the advertisement
            var manufacturerData = new BluetoothLEManufacturerData();
            manufacturerData.CompanyId = companyid; // Apple
            manufacturerData.Data = data;

            // Clear previous values just in case
            publisher.Advertisement.ManufacturerData.Clear();
            publisher.Advertisement.DataSections.Clear();

            // Add the manufacturer data to the advertisement publisher:
            publisher.Advertisement.ManufacturerData.Add(manufacturerData);
            try
            {
                publisher.Start();
                isCurrentlyStarting = false;
                return true;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("ADVERTISING PUBLISHER EXCEPTION!" );
                isCurrentlyStarting = false;
                return false;
            }

        }

        public bool StartAdvertisingIBeacon()
        {
            ushort companyid = 0x004c; // Apple

            if (publisher.Status != BluetoothLEAdvertisementPublisherStatus.Stopped)
            {
                 StopAdvertisingCustom();
            }

            BLEServices.iBeacon beacon = new BLEServices.iBeacon(Constants.iBeaconUuid, iBeaconMajor, iBeaconMinor, 200);
            Debug.WriteLine($"Advertising iBeacon - UUID: {Constants.iBeaconUuid}, Major: {iBeaconMajor}, Minor: {iBeaconMinor}");

            IBuffer data = beacon.getBuffer();
            return StartAdvertisingCustom(companyid, data);
        }


        public bool StartAdvertisingFF()
        {
            ushort companyid = 0xCAFE;
            IBuffer data;

            if (publisher.Status != BluetoothLEAdvertisementPublisherStatus.Stopped)
            {
                StopAdvertisingCustom();
            }

            var writer = new DataWriter();
            writer.WriteString($"{Constants.manufacturerSpecificAdvertisementText}{randomForManufacturerSpecific}!");
            data = writer.DetachBuffer();
            Debug.WriteLine($"Advertising Custom 0xFF, text: {Constants.manufacturerSpecificAdvertisementText}{randomForManufacturerSpecific}!");

            return StartAdvertisingCustom(companyid, data);
        }

        private void OnPublisherStatusChanged(BluetoothLEAdvertisementPublisher sender, BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
        {
            Debug.WriteLine("Advertisement publisher status changed: " + sender.Status + ",  Error: " + args.Error);
        }

    }
}
