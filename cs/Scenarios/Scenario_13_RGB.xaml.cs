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
using Windows.Security.Cryptography;

namespace BLE_Hackme
{
    public sealed partial class Scenario_13_RGB : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String scenarioName;
        SolidColorBrush lightBulbBrush = new SolidColorBrush();

        public Scenario_13_RGB()
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
                rootPage.Ble.lightBulbService.LightBulbColor.PropertyChanged += Service_PropertyChanged;
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

            //new characteristic value received
            if (e.PropertyName.Equals("Value"))
            {
                byte[] newColor = rootPage.Ble.lightBulbService.LightBulbColor.argb;
                rootPage.NotifyUser($"New color received: {newColor[0]} {newColor[1]} {newColor[2]} {newColor[3]}", NotifyType.StatusMessage);

                //initial check already in the characteristic, assuming [A R G B] here
                lightBulbBrush.Color = Windows.UI.Color.FromArgb(newColor[0], newColor[1], newColor[2], newColor[3]);
                LightBulb.ARGB(lightBulbBrush);

                //check if half dim red (FF0000) received
                if ((newColor[0] > 110) && (newColor[0] < 150) && (newColor[1] == 255) && (newColor[1] == 255) && (newColor[2]==0) && (newColor[3] == 0))
                {
                    rootPage.NotifyCorrect(scenarioName);
                    Solved.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
