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
    public sealed partial class Scenario_Summary : Page
    {
        private MainPage rootPage = MainPage.Current;
        private int solvedCount;

        public Scenario_Summary()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            solvedCount = 0;

            //generate list of scenarios
            foreach (Scenario sc in rootPage.Scenarios)
            {
                ListScenario(sc);
            }

            //update solved count header
            CurrentStatusText.Text = $"{solvedCount} task";
            if ((solvedCount ==0) || (solvedCount > 1))
            {
                CurrentStatusText.Text += "s";
            }
            CurrentStatusText.Text += $" of {rootPage.numberOfTasks} solved";

            if(solvedCount == rootPage.numberOfTasks)
            {
                allSolved();
            }
        }

        private void ListScenario(Scenario sc)
        {
            string className = sc.ClassType.Name;

            // do not add Start and Summary pages
            if (className.Equals("Scenario_01_Compatibility") || className.Equals("Scenario_Summary"))
            {
                return;
            }

            StackPanel newSp = new StackPanel();
            newSp.Orientation = Orientation.Horizontal;
            newSp.Margin = new Thickness(10, 5, 0, 0);

            FontIcon newIcon = new FontIcon();
            newIcon.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkGray);
            newIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
            newIcon.FontSize = 15;
            newIcon.Glyph = "\uF16B";
            newIcon.Margin = new Thickness(0, 0, 5, 0);

            HyperlinkButton newHl = new HyperlinkButton();
            newHl.Foreground = newIcon.Foreground;
            newHl.Margin = new Thickness(0, 0, 0, 0);
            newHl.Click += scenarioLinkClick;
            newHl.Content = sc.Title;
            newHl.Padding = new Thickness(0, 0, 0, 0);

            if (rootPage.isSolved.ContainsKey(className))
            {
                if (rootPage.isSolved[className] == true)
                {
                    newIcon.Foreground = new SolidColorBrush(Windows.UI.Colors.MediumSpringGreen);
                    newHl.Foreground = newIcon.Foreground;
                    newIcon.Glyph = "\uF16C";
                    solvedCount++;
                }
            }
            newSp.Children.Add(newIcon);
            newSp.Children.Add(newHl);
            ListOfScenarios.Children.Add(newSp);
        }

        void scenarioLinkClick(object sender, RoutedEventArgs e)
        {
            var name = (sender as HyperlinkButton).Content.ToString();
            rootPage.NavigateScenarioByTitle(name);
        }

        void allSolved()
        {
            // show trophy
            Trophy.Visibility = Visibility.Visible;
            CongratulationText.Visibility = Visibility.Visible;
            // play applause
            Applause.AutoPlay = true;
            Applause.Play();

            //manually enforce 100% progress, as partial % may not add to 100
            rootPage.SetProgress(100);
        }
    }
}
