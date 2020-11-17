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
using Windows.Media.SpeechSynthesis;
using Windows.System;
using System.Threading;


namespace BLE_Hackme
{
    public sealed partial class Scenario_16_QuickLockLogs : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String scenarioName;

        public Scenario_16_QuickLockLogs()
        {
            this.InitializeComponent();
            scenarioName = this.GetType().Name;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // set up BLE advertising for this scenario
            var ret = await rootPage.Ble.AdvertisingForScenario(this.GetType().Name);

            if (ret == false || rootPage.Ble.quickLockMainService == null)
            {
                    Debug.WriteLine("Error, quicklock main service is null!");
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
            var correctDate = new DateTime(2020, 11, 7);

            // the date provided by CalendarDatePicker (UnlockDate) is with current time, in order to compare it we need to clean it first
            if (UnlockDate.Date.HasValue)
            {
                var dateVal = UnlockDate.Date.Value;
                var enteredDate = new DateTime(dateVal.Year, dateVal.Month, dateVal.Day);

                Debug.WriteLine($"Entered username: {ValueToEnter.Text}; entered date: {enteredDate}; expected date: {correctDate}");
                if (ValueToEnter.Text.Equals("hackmeuser", StringComparison.CurrentCultureIgnoreCase) && enteredDate.Equals(correctDate))
                {
                    rootPage.NotifyCorrect(scenarioName);

                    Solved.Visibility = Visibility.Visible;
                }
                else
                {
                    rootPage.NotifyUser("Sorry, incorrect, try again!", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Please enter the date!", NotifyType.ErrorMessage);
            }
        }
    }
}
