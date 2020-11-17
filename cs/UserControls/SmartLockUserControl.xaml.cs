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
    public sealed partial class SmartLockUserControl : UserControl
    {
        public SmartLockUserControl()
        {
            this.InitializeComponent();
        }

        public void Unlock()
        {
            SmartLockUnlocked.Visibility = Visibility.Visible;
            SmartLockLocked.Visibility = Visibility.Collapsed;
        }

        public void Lock()
        {
            SmartLockLocked.Visibility = Visibility.Visible;
            SmartLockUnlocked.Visibility = Visibility.Collapsed;
        }

    }
}
