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
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace BLE_Hackme
{
    public sealed partial class Scenario_06_Connections : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String scenarioName;

        public Scenario_06_Connections()
        {
            this.InitializeComponent();
            scenarioName = this.GetType().Name;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // set up BLE advertising for this scenario
            var ret = await rootPage.Ble.AdvertisingForScenario(scenarioName);

            if (ret == false || rootPage.Ble.lightBulbService == null)
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

        private void ValueSubmitButton_Click()
        {

            String value = ValueToEnter.Text;

            Debug.WriteLine($"Received value: {value}");

            //remove whitespace and "0x"
            value = Regex.Replace(value, @"\s+", "");
            value = Regex.Replace(value, "0x", "");

            Debug.WriteLine($"Received value after regex: {value}");

            if (value.Equals("2a00,2a01,2a04,2aa6", StringComparison.CurrentCultureIgnoreCase))
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