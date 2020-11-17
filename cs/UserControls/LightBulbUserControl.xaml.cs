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
    public sealed partial class LightBulbUserControl : UserControl
    {
        private SolidColorBrush colorBrush;
        public LightBulbUserControl()
        {
            this.InitializeComponent();
            colorBrush = new SolidColorBrush();
        }

        public void ARGB(SolidColorBrush brush)
        {
            LightBulbIn.Foreground = brush;
  //          LightBulbOut.Foreground = brush;
        }

        public void On()
        {
            //colorBrush.Color = Windows.UI.Colors.Yellow;
            colorBrush.Color = Windows.UI.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xA0);
            LightBulbIn.Foreground = colorBrush;
        }

        public void Off()
        {
            colorBrush.Color = Windows.UI.Colors.Black;
            LightBulbIn.Foreground = colorBrush;
        }

        public void Checked(bool on)
        {
            if (on)
            {
                LightBulbChecked.Visibility = Visibility.Visible;
                LightBulbOut.Visibility = Visibility.Collapsed;
            }
            else
            {
                LightBulbChecked.Visibility = Visibility.Collapsed;
                LightBulbOut.Visibility = Visibility.Visible;
            }
        }

    }
}
