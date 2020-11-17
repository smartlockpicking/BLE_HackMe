using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;


namespace BLE_Hackme
{
    public sealed partial class Scenario_01_Compatibility : Page
    {

        private MainPage rootPage = MainPage.Current;

        public Scenario_01_Compatibility()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var supported = await rootPage.Ble.CheckPeripheralRoleSupportAsync();

            if (supported)
            {
                var started = await rootPage.Ble.AdvertisingForScenario(this.GetType().Name);

                if (started)
                {
                    Checking.Visibility = Visibility.Collapsed;
                    Success.Visibility = Visibility.Visible;
                    rootPage.NotifyUser("BLE device simulation successfully started!", NotifyType.StatusMessage);
                    rootPage.ScenariosEnable();
                }
                else // supported but starting error
                {
                    Checking.Visibility = Visibility.Collapsed;
                    ErrorStarting.Visibility = Visibility.Visible;
                    rootPage.NotifyUser("Error starting BLE device simulation...", NotifyType.ErrorMessage);
                }
            }
            else //not supported, display error message
            {
                Checking.Visibility = Visibility.Collapsed;
                ErrorStarting.Visibility = Visibility.Visible;
                rootPage.NotifyUser("Bluetooth device unsupported...", NotifyType.ErrorMessage);

            }
        }

    }
}