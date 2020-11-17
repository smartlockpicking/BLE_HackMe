
using BLE_Hackme.UserControls;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace BLE_Hackme
{
    public partial class MainPage : Page
    {
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Start", ClassType=typeof(Scenario_01_Compatibility) },
            new Scenario() { Title="First steps", ClassType=typeof(Scenario_02_Intro) },
            new Scenario() { Title="BLE Advertisements", ClassType=typeof(Scenario_03_Advertisements) },
            new Scenario() { Title="Beacons", ClassType=typeof(Scenario_04_Beacons) },
            new Scenario() { Title="Manufacturer Specific Advertisements", ClassType=typeof(Scenario_05_ManufacturerAdvertisements) },
            new Scenario() { Title="Connections, services, characteristics", ClassType=typeof(Scenario_06_Connections) },
            new Scenario() { Title="Characteristic read", ClassType=typeof(Scenario_07_Read) },
            new Scenario() { Title="Notifications", ClassType=typeof(Scenario_08_Notifications) },
            new Scenario() { Title="Descriptors", ClassType=typeof(Scenario_09_Descriptors) },
            new Scenario() { Title="Characteristic write", ClassType=typeof(Scenario_10_Write) },
            new Scenario() { Title="Various writes", ClassType=typeof(Scenario_11_WriteCommand) },
            new Scenario() { Title="Write automation", ClassType=typeof(Scenario_12_Macros) },
            new Scenario() { Title="Protocol reverse-engineering", ClassType=typeof(Scenario_13_RGB) },
            new Scenario() { Title="Password brute force", ClassType=typeof(Scenario_14_PasswordBrute) },
            new Scenario() { Title="Smart lock replay", ClassType=typeof(Scenario_15_QuickLockReplay) },
            new Scenario() { Title="Smart lock information leak", ClassType=typeof(Scenario_16_QuickLockLogs) },
            new Scenario() { Title="Summary", ClassType=typeof(Scenario_Summary) },
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
