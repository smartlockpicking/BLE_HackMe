
![BLE_HackMe](https://raw.githubusercontent.com/smartlockpicking/BLE_HackMe/master/logo.png)


# Bluetooth Low Energy hardware-less HackMe

The project aims to introduce the BLE protocol and its security basics.
A standard Windows 10 computer with Bluetooth interface will simulate various BLE devices - on the radio layer working exactly as real ones.
In a series of tasks with increasing level of complexity, allows to get familiar with BLE advertisements, beacons, connections, take control over talking BLE smart bulb, reverse-engineer the communication, brute force passwords, and even hack real smart lock.


## System requirements

 * Windows 10 Anniversary Edition
 * Bluetooth adapter - should work with most adapters built in standard laptops, not necessarily with external dongle

The tasks are designed to be solved using free Android mobile application, connecting to the simulated devices via BLE (iOS has limited low level BLE features). 

It is also possible to use other BLE tools, for example running on Linux or Mac, however details are not covered in the HackMe instructions.


## Installation

Binary version is available in Microsoft Store:

[https://www.microsoft.com/en-us/p/ble-hackme/9n7pnvs9j1b7](https://www.microsoft.com/en-us/p/ble-hackme/9n7pnvs9j1b7)

## Building from source

### Build in Visual Studio

For building from source, Microsoft Visual Studio is required (free, Community edition will work).

1. Start Microsoft Visual Studio and select **File** \> **Open** \> **Project/Solution**.
2. Double-click the Visual Studio Solution (.sln) file.
3. Press Ctrl+Shift+B, or select **Build** \> **Build Solution**.

### Run the debug session

To debug the application and then run it, press F5 or select Debug >  Start Debugging. To run without debugging, press Ctrl+F5 or selectDebug > Start Without Debugging. 

Some debug information available in the "Output" section in Visual Studio.


## FAQ, more information

Frequently Asked Questions, list of incompatible hardware: [https://github.com/smartlockpicking/BLE_HackMe/wiki/FAQ](https://github.com/smartlockpicking/BLE_HackMe/wiki/FAQ)

More information: [www.smartlockpicking.com/ble_hackme](www.smartlockpicking.com/ble_hackme)


## License

This application was developed by Slawomir Jasek slawomir.jasek@smartlockpicking.com.

It is free software licensed under the MIT License.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)


The code is partially based on Microsoft [Bluetooth LE Explorer](https://github.com/microsoft/BluetoothLEExplorer) and [Bluetooth LE Sample](https://github.com/microsoft/Windows-universal-samples/tree/master/Samples/BluetoothLE).
