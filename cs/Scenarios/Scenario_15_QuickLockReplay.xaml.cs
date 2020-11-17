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
using System.Threading.Tasks;



namespace BLE_Hackme
{
    public sealed partial class Scenario_15_QuickLockReplay : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String scenarioName;
        SpeechSynthesizer synth = new SpeechSynthesizer();
        SpeechSynthesisStream speechStream;

        public Scenario_15_QuickLockReplay()
        {
            this.InitializeComponent();
            scenarioName = this.GetType().Name;
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // set up BLE advertising for this scenario
            var ret = await rootPage.Ble.AdvertisingForScenario(this.GetType().Name);

            if (ret == true && rootPage.Ble.quickLockMainService != null)
            {
                rootPage.Ble.quickLockMainService.QuickLockAuth.PropertyChanged += Service_PropertyChanged;
                rootPage.Ble.quickLockMainService.QuickLockCommand.PropertyChanged += Service_PropertyChanged;

                //initialize speech stream
                speechStream = await synth.SynthesizeTextToStreamAsync($"Unlocking! Locking again in 3,,,,, 2,,,,, 1,,,,, ");

                VoiceUnlocking.SetSource(speechStream, speechStream.ContentType);
                VoiceUnlocking.AutoPlay = false;
                // lock it again after playing
                VoiceUnlocking.MediaEnded += LockAgain;

            }
            else 
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

        private void Service_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("InvalidRequest"))
            {
                rootPage.NotifyUser("Invalid request!", NotifyType.ErrorMessage);
            }

            else if (e.PropertyName.Equals("InvalidPin"))
            {
                rootPage.NotifyUser("Invalid password, access denied!", NotifyType.ErrorMessage);
            }

            else if (e.PropertyName.Equals("Unauthenticated"))
            {
                rootPage.NotifyUser("Unauthenticated, access denied!", NotifyType.ErrorMessage);
            }


            // new command value received only after successful authentication
            if (e.PropertyName.Equals("LockState"))
            {

                Debug.WriteLine("Received LockState update");

                if (rootPage.Ble.quickLockMainService.isUnlocked)
                {
                    rootPage.NotifyCorrect(scenarioName);
                    Solved.Visibility = Visibility.Visible;
                    //tbd next time (if isSolved)
                    //                rootPage.NotifyUser($"Correct password, unlocking!", NotifyType.StatusMessage);
                    SmartLock.Unlock();

                    //VoiceUnlocking.AutoPlay = true;
                    VoiceUnlocking.Play();
                }
                //received "lock" command, but it should be locked anyway ;)
                else
                {
                    rootPage.NotifyUser("Locking...", NotifyType.StatusMessage);
                    SmartLock.Lock();
                }

            }
        }

        private void LockAgain(object Sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Locking again");
            SmartLock.Lock();

            rootPage.Ble.quickLockMainService.UpdateAuthenticationState(false);
            rootPage.Ble.quickLockMainService.UpdateUnlockState(false);

        }

    }
}
