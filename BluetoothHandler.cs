using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using System.Windows;


namespace remote_inspection_unit_control
{
    static class BluetoothHandler
    {
        private static Dictionary<string, BluetoothDeviceInfo> _deviceInfo;
        private static readonly String DEFAULT_PIN = "1234";

        public static bool isSupported()
        {
            return BluetoothRadio.IsSupported;
        }

        public static async Task<List<String>> discoverAsync()
        {
            List<String> items = new List<string> { };
            BluetoothClient bc = new BluetoothClient();
            BluetoothDeviceInfo[] devices = await Task.Run(() => bc.DiscoverDevicesInRange());
            _deviceInfo = new Dictionary<string, BluetoothDeviceInfo> { };
            foreach (BluetoothDeviceInfo device in devices)
            {
                items.Add(device.DeviceName);
                _deviceInfo.Add(device.DeviceName, device);
            }
            return items;
        }

        public static async Task<bool> pairAsync(String name)
        {
            bool isPaired = false;
            isPaired = await Task.Run(() => BluetoothSecurity.PairRequest(_deviceInfo[name].DeviceAddress, DEFAULT_PIN));
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
