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

//anim
using System.Numerics;
using Windows.Foundation.Metadata;
using Windows.UI.Composition;


namespace BLE_Hackme
{
    public sealed partial class Scenario_14_PasswordBrute : Page
    {
        private MainPage rootPage = MainPage.Current;
        private String scenarioName;
        SolidColorBrush lightBulbBrush = new SolidColorBrush();
        SpeechSynthesizer synth = new SpeechSynthesizer();
        SpeechSynthesisStream speechStream;
        private DispatcherTimer colorTicker = null;
        private Random rnd = new Random();
        private int effectCount = 0;

        //for animation
        Compositor _compositor = Window.Current.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;

        public Scenario_14_PasswordBrute()
        {
            this.InitializeComponent();
            scenarioName = this.GetType().Name;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // set up BLE advertising for this scenario
            var ret = await rootPage.Ble.AdvertisingForScenario(this.GetType().Name);

            if (ret==true && rootPage.Ble.lightBulbService != null)
            {
                rootPage.Ble.lightBulbService.LightBulbColor.PropertyChanged += Service_PropertyChanged;
                var pin = rootPage.Ble.lightBulbService.LightBulbColor.pin;

                ShowValidPin.Text = $"Valid PIN: {pin[0]} {pin[1]} {pin[2]}";

                //initialize speech stream
                speechStream = await synth.SynthesizeTextToStreamAsync($"Congratulations, valid pin is {pin[0]} {pin[1]} {pin[2]}. Enjoy!");

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

            else if (e.PropertyName.Equals("InvalidPin"))
            {
                rootPage.NotifyUser("Invalid password, access denied!", NotifyType.ErrorMessage);
            }

            // process ARGB Value 
            if (e.PropertyName.Equals("Value"))
            {
                byte[] newColor = rootPage.Ble.lightBulbService.LightBulbColor.argb;
                rootPage.NotifyUser($"New color received: {newColor[0]} {newColor[1]} {newColor[2]} {newColor[3]}", NotifyType.StatusMessage);

                //initial check already in the characteristic, assuming [A R G B] here
                lightBulbBrush.Color = Windows.UI.Color.FromArgb(newColor[0], newColor[1], newColor[2], newColor[3]);
                LightBulb.ARGB(lightBulbBrush);
            }

            // password protected special scenario received
            else if (e.PropertyName.Equals("SpecialValue"))
            {
                byte specialEffects = rootPage.Ble.lightBulbService.LightBulbColor.specialEffectsScenario;
                rootPage.NotifyUser($"Correct PIN, new special effects scenario received: {specialEffects}", NotifyType.StatusMessage);

                //initial check already in the characteristic, assuming [A R G B] here
                if (specialEffects == 0x01)
                {
                    //Green
                    lightBulbBrush.Color = Windows.UI.Color.FromArgb(0xFF, 0x00, 0xFF, 0x00);
                    LightBulb.ARGB(lightBulbBrush);

                    //TextToSpeech.AutoPlay = true;
                    VoiceCongrats.SetSource(speechStream, speechStream.ContentType);
                    VoiceCongrats.Play();
                    // play Rick after the voice congrats finishes
                    VoiceCongrats.MediaEnded += RickRoll;

                    rootPage.NotifyCorrect(scenarioName);
                    Solved.Visibility = Visibility.Visible;
                    ShowValidPin.Visibility = Visibility.Visible;
                    LightBulb.Checked(true);
                } 
                else
                {
                    LightBulb.Off();
                    LightBulb.Checked(false);
                    Rick.Stop();
                    Rick.Visibility = Visibility.Collapsed;
                    colorTicker.Stop();
                }
            }
        }

        private void RickRoll(object Sender, RoutedEventArgs e)
        {
            Rick.Visibility = Visibility.Visible;
            Rick.Play();
            LightBulb.Checked(false);

            colorTicker = new DispatcherTimer();
            colorTicker.Tick += UpdateColor; 
            colorTicker.Interval = new TimeSpan(0, 0, 0, 0, 250); // 4 x per second
            colorTicker.Start();
        }

        private void UpdateColor(object sender, object e)
        {
            if (effectCount == 0)
            {
                UpdateSpringAnimation(1.2f);
                StartAnimationIfAPIPresent(LightBulb, _springAnimation);
                effectCount++;

                var red = (byte)rnd.Next(0, 255);
                var green = (byte)rnd.Next(0, 255);
                var blue = (byte)rnd.Next(0, 255);

                lightBulbBrush.Color = Windows.UI.Color.FromArgb(255, red, green, blue);
                LightBulb.ARGB(lightBulbBrush);
            }
            else
            {
                UpdateSpringAnimation(1f);
                StartAnimationIfAPIPresent(LightBulb, _springAnimation);
                effectCount--;
            }
        }


        private void UpdateSpringAnimation(float finalValue)
        {
            if (_springAnimation == null)
            {
                _springAnimation = _compositor.CreateSpringVector3Animation();
                _springAnimation.Target = "Scale";
            }

            _springAnimation.FinalValue = new Vector3(finalValue);
            _springAnimation.DampingRatio = 0.6f;
            _springAnimation.Period = TimeSpan.FromMilliseconds(150);
        }

        private void StartAnimationIfAPIPresent(UIElement sender, Windows.UI.Composition.CompositionAnimation animation)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                (sender as UIElement).StartAnimation(animation);
            }
        }
    }
}
