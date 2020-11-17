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

namespace BLE_Hackme.UserControls
{
    public sealed partial class ScenarioSubHeaderHints : UserControl
    {
        private MainPage rootPage = MainPage.Current;
        private int currentHint = 0;

        public ScenarioSubHeaderHints()
        {
            this.InitializeComponent();
        }

        private void HintButton_Click(object sender, RoutedEventArgs e)
        {
            currentHint++;
            String hintName = $"Hint{currentHint}";

            // scrollviewer in Page
            var sv = (ScrollViewer)((FrameworkElement)this.Parent).Parent;
//            var page = (Page) sv.Parent;

            StackPanel hintPanel = sv.FindName(hintName) as StackPanel;
            if (hintPanel != null)
            {
                hintPanel.Visibility = Visibility.Visible;
            }
            else
            {
                rootPage.NotifyUser("Sorry, no more hints...", NotifyType.StatusMessage);
            }
        }
    }
}
