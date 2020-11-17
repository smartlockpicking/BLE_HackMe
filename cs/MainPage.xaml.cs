using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.ApplicationModel;

namespace BLE_Hackme
{
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public int currentProgress = 0;
        public BLE Ble;
        // scenario name, <true | false>
        public IDictionary<string, bool> isSolved = new Dictionary<string, bool>();

        // used to show current progress
        // calculated from number of scenarios
        public int numberOfTasks;

        public MainPage()
        {
            this.InitializeComponent();

            // This is a static public property that allows downstream pages to get a handle to the MainPage instance
            // in order to call methods that are in this class.
            Current = this;

            // first and scenario don't have task, so "-2"
            numberOfTasks = scenarios.Count - 2;
            Debug.WriteLine($"Number of tasks: {numberOfTasks}");

            Ble = new BLE();

            // navigation pane initially disabled, enabled after confirming hardware compatibility
            ScenariosDisable();

            // populate build version
            Copyright.Text += $", build {GetAppVersion()}";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate the scenario list from the ScenariosConfiguration.cs file
            var itemCollection = new List<Scenario>();
            int i = 1;
            foreach (Scenario s in scenarios)
            {
                itemCollection.Add(new Scenario { Title = $"{i++}) {s.Title}", ClassType = s.ClassType });
            }
            ScenarioControl.ItemsSource = itemCollection;

            if (Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Called whenever the user changes selection in the scenarios list.  This method will navigate to the respective
        /// sample scenario page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the status block when navigating scenarios.
            NotifyUser(String.Empty, NotifyType.StatusMessage);

            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;
            if (s != null)
            {
                ScenarioFrame.Navigate(s.ClassType);
                if (Window.Current.Bounds.Width < 640)
                {
                    Splitter.IsPaneOpen = false;
                }
            }
        }

        /// <summary>
        /// Left pane navigation invoked from code (links)
        /// </summary>
        /// <param name="index"></param>
        public void NavigateScenario(int index)
        {
            ScenarioControl.SelectedIndex = index;

        }

        public void NavigateScenarioByTitle(string title)
        {
            int ind = Scenarios.FindIndex(
                delegate (Scenario sc)
                {
                    return sc.Title.Equals(title, StringComparison.OrdinalIgnoreCase);
                });

            Debug.WriteLine($"Navigating scenario by title {title}: index {ind}");
            ScenarioControl.SelectedIndex = ind;
        }

        public void NavigateNextScenario()
        {
            ScenarioControl.SelectedIndex++;
        }

        public void ScenariosEnable()
        {
            ScenarioControl.IsEnabled = true;
        }

        public void ScenariosDisable()
        {
            ScenarioControl.IsEnabled = false;
        }

        public void AdvanceProgress()
        {
            Progress.Value += 100 / numberOfTasks;
            Debug.WriteLine($"Advancing progress to {Progress.Value}% ");
        }

        public void SetProgress(int percentage)
        {
            Progress.Value = percentage;

            if (percentage == 100)
            {
                var brush = new SolidColorBrush();
                brush.Color = Windows.UI.Color.FromArgb(255, 20, 220, 100); // light green
                Progress.Foreground = brush;
            }
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }


        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }

        public void NotifyCorrect(String scenario) 
        {
            NotifyUser("Congratulations!", NotifyType.StatusMessage, NotifyButton.ContinueToNextTask);

            if (! isSolved.ContainsKey(scenario))
            {
                isSolved[scenario] = true;
                AdvanceProgress();
            }
        }

        /// <summary>
        /// Display a message to the user.
        /// This method may be called from any thread.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        public int NotifyUser(string strMessage, NotifyType type, NotifyButton button = NotifyButton.None)
        {
            if (button == NotifyButton.ContinueToNextTask)
            {
                ProceedButton.Visibility = Visibility.Visible;
            }
            else
            {
                ProceedButton.Visibility = Visibility.Collapsed;
            }

            if (button == NotifyButton.RestartDevice)
            {
                RestartDeviceButton.Visibility = Visibility.Visible;
            } else
            {
                RestartDeviceButton.Visibility = Visibility.Collapsed;
            }

            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
            return 1;
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }

            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }

			// Raise an event if necessary to enable a screen reader to announce the status update.
			var peer = FrameworkElementAutomationPeer.FromElement(StatusBlock);
			if (peer != null)
			{
				peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
			}
		}

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        public void NextTaskButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateNextScenario();
        }

        public async void RestartDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = ScenarioControl.SelectedIndex;
            string scenarioClass = Scenarios[currentIndex].ClassType.ToString();
            Debug.WriteLine($"Restarting device simulation for: {scenarioClass}");
            var ret = await Ble.AdvertisingForScenario(scenarioClass);
            if (ret == true)
            {
                NotifyUser("Device simulation restarted!", NotifyType.StatusMessage);
            }
        }


    }
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public enum NotifyButton
    {
        None,
        ContinueToNextTask,
        RestartDevice
    }
}
