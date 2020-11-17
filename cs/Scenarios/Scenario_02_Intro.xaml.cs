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
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Networking;
using Windows.Networking.Connectivity;
using System.Diagnostics;

namespace BLE_Hackme
{
    public sealed partial class Scenario_02_Intro : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String hostName;
        private String scenarioName;

        public Scenario_02_Intro()
        {
            this.InitializeComponent();
            hostName = GetHostName();
            scenarioName = this.GetType().Name;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine($"Hostname: {hostName}");
            DeviceName.Text = hostName;

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
        private String GetHostName()
        {
            var hostNames = NetworkInformation.GetHostNames();
            var hostName = hostNames.FirstOrDefault(name => name.Type == HostNameType.DomainName)?.DisplayName ?? "???";
            hostName = hostName.ToUpper();

            // remove everything after "." (optional domain name)
            int index = hostName.IndexOf(".");
            if (index > 0)
                hostName = hostName.Substring(0, index);

            if (hostName.Length > 15)
            {
                hostName = hostName.Substring(0, 15);
            }
            return hostName;
        }


        private void ValueSubmitButton_Click()
        {
            if ( ValueToEnter.Text.Equals(hostName, StringComparison.CurrentCultureIgnoreCase))
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
