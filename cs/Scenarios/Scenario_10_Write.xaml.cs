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
using Windows.Media.SpeechSynthesis;
using Windows.Security.Cryptography;
using BLE_Hackme.UserControls;
using Windows.UI.Core;

namespace BLE_Hackme
{
    public sealed partial class Scenario_10_Write : Page
    {
        private MainPage rootPage = MainPage.Current;
        SolidColorBrush lightBulbBrush = new SolidColorBrush();
        private String scenarioName;
        private bool isSolved = false;


        public Scenario_10_Write()
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

            // turn on if previously set state
            var currentValue = BLEServices.Helpers.ToHexString(rootPage.Ble.lightBulbService.LightBulbSwitch.Value);
            if (currentValue == "01")
            {
                LightBulb.On();
            }

        }

        private void Service_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName.Equals("InvalidRequest"))
            {
                rootPage.NotifyUser("Invalid request!", NotifyType.ErrorMessage);
            }

            //new characteristic value received
            if (e.PropertyName.Equals("Value"))
            {
                // switch (sender) ...
                Debug.WriteLine($"Lightbulb service received value from : {sender}");

                String receivedWrite = BLEServices.Helpers.ToHexString(rootPage.Ble.lightBulbService.LightBulbSwitch.Value);
                rootPage.NotifyUser("WRITE received: " + receivedWrite, NotifyType.StatusMessage);

                //check if value is 01, otherwise error?
                if (receivedWrite.Equals("01"))
                {
                    LightBulb.On();
                    if (isSolved == false)
                    {
                        isSolved = true;
                        rootPage.NotifyCorrect(scenarioName);
                        Solved.Visibility = Visibility.Visible;
                    }
                }
                else if (receivedWrite.Equals("00"))
                {
                    LightBulb.Off();
                }
            }
        }
    }
}
