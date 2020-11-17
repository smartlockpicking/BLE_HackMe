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
using System.Text.RegularExpressions;


namespace BLE_Hackme
{
    public sealed partial class Scenario_11_WriteCommand : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String scenarioName;
        private bool isSolved;

        private SpeechSynthesizer synth;
        private SpeechSynthesisStream speechStream;

        public Scenario_11_WriteCommand()
        {
            this.InitializeComponent();
            scenarioName = this.GetType().Name;

            synth = new SpeechSynthesizer();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // set up BLE advertising for this scenario
            var ret = await rootPage.Ble.AdvertisingForScenario(scenarioName);
            if (ret == true && rootPage.Ble.lightBulbService != null)
            {
                rootPage.Ble.lightBulbService.LightBulbTTS.PropertyChanged += ServiceTTS_PropertyChanged;
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

        private async void GenerateTTS(String whatToSay)
        {
            speechStream = await synth.SynthesizeTextToStreamAsync(whatToSay);
            TextToSpeech.AutoPlay = true;
            TextToSpeech.SetSource(speechStream, speechStream.ContentType);
            TextToSpeech.Play();
        }

        private void ServiceTTS_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("InvalidRequest"))
            {
                rootPage.NotifyUser("Invalid request! Did you send write command?", NotifyType.ErrorMessage);
            }

            //new characteristic value received
            if (e.PropertyName.Equals("Value"))
            {
                Debug.WriteLine($"Lightbulb service received from : {sender}");

                byte[] receivedWrite;
                CryptographicBuffer.CopyToByteArray(rootPage.Ble.lightBulbService.LightBulbTTS.Value, out receivedWrite);
                String receivedText = System.Text.Encoding.UTF8.GetString(receivedWrite);

                Debug.WriteLine($"TTS received value: {receivedText}");
                //remove excessive characters
                receivedText = Regex.Replace(receivedText, @"[^a-zA-Z0-9 -]", "");
                Debug.WriteLine($"TTS received value after removing special characters: {receivedText}");

                if (receivedText.Length == 0)
                {
                    rootPage.NotifyUser("Received TTS request, but no readable text value", NotifyType.ErrorMessage);
                }
                else
                {
                    var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => GenerateTTS(receivedText));
                    if (receivedText.StartsWith("Hello", StringComparison.OrdinalIgnoreCase))
                    {
                        if (isSolved == false)
                        {
                            isSolved = true;
                            rootPage.NotifyCorrect(scenarioName);
                            Solved.Visibility = Visibility.Visible;
                            LightBulb.Checked(true);
                        }
                    }
                    else
                    {
                        rootPage.NotifyUser($"Received TTS text: {receivedText}. How about saying hello?", NotifyType.StatusMessage);
                    }
                }
            }
        }
    }
}
