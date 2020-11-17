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
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Bluetooth;
using System.Diagnostics;

namespace BLE_Hackme
{
    public sealed partial class Scenario_12_Macros : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String scenarioName;
        private bool isSolved = false;


        public Scenario_12_Macros()
        {
            this.InitializeComponent();
            scenarioName = this.GetType().Name;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // set up BLE advertising for this scenario
            var ret = await rootPage.Ble.AdvertisingForScenario(scenarioName);

            if (ret == true && rootPage.Ble.lightBulbService != null)
            {
                rootPage.Ble.lightBulbService.LightBulbSwitch.PropertyChanged += Service_PropertyChanged;
            }
            else
            {
                Debug.WriteLine("Error, lightbulb service is null!");
                rootPage.NotifyUser("Problem starting device simulation", NotifyType.ErrorMessage, NotifyButton.RestartDevice);
            }


            if (rootPage.isSolved.ContainsKey(scenarioName))
            {
                if (rootPage.isSolved[scenarioName] == true)
                {
                    Solved.Visibility = Visibility.Visible;
                }
            }
        }


        private void Service_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName.Equals("InvalidRequest"))
            {
                rootPage.NotifyUser("Invalid request!", NotifyType.ErrorMessage);
            }

            else if (e.PropertyName.Equals("BlinkPassed"))
            {
                rootPage.NotifyCorrect(scenarioName);
                Solved.Visibility = Visibility.Visible;
                LightBulb.Checked(true);
                isSolved = true;
            }

            //new characteristic value received
            else if (e.PropertyName.Equals("Value"))
            {
                String receivedWrite = BLEServices.Helpers.ToHexString(rootPage.Ble.lightBulbService.LightBulbSwitch.Value);

                // notify of every write until solved, otherwise it will overwrite the "congratulations" notification
                if (!isSolved)
                {
                    rootPage.NotifyUser("Write received: " + receivedWrite, NotifyType.StatusMessage);
                }

                if (receivedWrite.Equals("01"))
                {
                    LightBulb.On();
                }
                else if (receivedWrite.Equals("00"))
                {
                    LightBulb.Off();
                }
            }
        }

    }
}
