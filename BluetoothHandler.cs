﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using InTheHand.Net;



namespace remote_inspection_unit_control
{
    static class BluetoothHandler
    {
        private static Dictionary<String, BluetoothAddress> deviceInfo = new Dictionary<string, BluetoothAddress> { };
        private const String DEFAULT_PIN = "1234";


        public static bool isSupported()
        {
            if (!BluetoothRadio.IsSupported)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static async Task<List<String>> discoverAsync()
        {
            BluetoothClient bc = new BluetoothClient();
            List<String> items = new List<string> { };


            BluetoothDeviceInfo[] devices = await Task.Run(() => bc.DiscoverDevicesInRange());
            foreach (BluetoothDeviceInfo device in devices)
            {
                items.Add(device.DeviceName);
                deviceInfo.Add(device.DeviceName, device.DeviceAddress);
            }
            return items;
        }

        public static async Task<bool> pairAsync(String name, String pin = DEFAULT_PIN)
        {
            bool isPaired;
            isPaired = await Task.Run(() => BluetoothSecurity.PairRequest(deviceInfo[name], pin));
            return isPaired;
        }

        public static void send(String command)
        {
        }

        private static void receive()
        {
        }

    }
}
