using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BLE_Hackme
{
    public class Constants
    {
        public static readonly Guid iBeaconUuid = Guid.Parse("6834636b-6d33-6942-3334-63306e553144");

        public static readonly String manufacturerSpecificAdvertisementText = "Please don't hack me ";

        public static readonly String heartRateDescriptorText = "Beats per minute ";

        public static readonly Guid LightBulbServiceUuid = Guid.Parse("6834636b-6d33-4c31-3668-744275314221");

        public static readonly Guid LightBulbSwitchCharacteristicUuid = Guid.Parse("6834636b-6d33-4c31-3668-744275314201");
        public static readonly Guid LightBulbTTSCharacteristicUuid = Guid.Parse("6834636b-6d33-4c31-3668-744275314202");
        public static readonly Guid LightBulbColorCharacteristicUuid = Guid.Parse("6834636b-6d33-4c31-3668-744275314203");
        public static readonly Guid LightBulbProtectedCharacteristicUuid = Guid.Parse("6834636b-6d33-4c31-3668-744275314204");

        //quicklock
        public static readonly Guid QuickLockHistoryServiceUuid = Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb");
        public static readonly Guid QuickLockHistoryControlCharacteristicUuid = Guid.Parse("0000fff1-0000-1000-8000-00805f9b34fb");
        public static readonly Guid QuickLockHistoryDataCharacteristicUuid = Guid.Parse("0000fff2-0000-1000-8000-00805f9b34fb");
        public static readonly Guid QuickLockUsernameCharacteristicUuid = Guid.Parse("0000fff3-0000-1000-8000-00805f9b34fb");

        public static readonly Guid QuickLockMainServiceUuid = Guid.Parse("0000ffd0-0000-1000-8000-00805f9b34fb");
        public static readonly Guid QuickLockAuthCharacteristicUuid = Guid.Parse("0000ffd6-0000-1000-8000-00805f9b34fb");
        public static readonly Guid QuickLockAuthStatusCharacteristicUuid = Guid.Parse("0000ffd7-0000-1000-8000-00805f9b34fb");
        public static readonly Guid QuickLockOpenTimeCharacteristicUuid = Guid.Parse("0000ffd8-0000-1000-8000-00805f9b34fb");
        public static readonly Guid QuickLockCommandCharacteristicUuid = Guid.Parse("0000ffd9-0000-1000-8000-00805f9b34fb");
        public static readonly Guid QuickLockUnlockStatusCharacteristicUuid = Guid.Parse("0000ffda-0000-1000-8000-00805f9b34fb");

    };
}
