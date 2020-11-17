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
using System.Diagnostics;


namespace BLE_Hackme
{
    public sealed partial class Scenario_04_Beacons : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String scenarioName;


        public Scenario_04_Beacons()
        {
            this.InitializeComponent();
            scenarioName = this.GetType().Name;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // set up BLE advertising for this scenario
            var ret = await rootPage.Ble.AdvertisingForScenario(scenarioName);

            if (ret == false)
            {
                Debug.WriteLine("Error starting custom advertisement!");
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

        private void ValueSubmitButton_Click()
        {
            var Major = Convert.ToUInt16(MajorNumber.Text);
            var Minor = Convert.ToUInt16(MinorNumber.Text);

            if ( (Major == rootPage.Ble.iBeaconMajor) && (Minor == rootPage.Ble.iBeaconMinor))
            {
                rootPage.NotifyCorrect(scenarioName);
                Solved.Visibility = Visibility.Visible;
            }
            else
            {
                rootPage.NotifyUser("Sorry, incorrect, try again!", NotifyType.ErrorMessage);
            }
        }

    }
}
