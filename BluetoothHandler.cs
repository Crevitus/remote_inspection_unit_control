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



namespace remote_inspection_unit_control
{
    static class BluetoothHandler
    {
        private static BluetoothDeviceInfo[] _devices;
        private const String DEFAULT_PIN = "1234";
        private static String _selectedDevice;


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

        public static async Task<BluetoothDeviceInfo[]> discoverAsync()
        {
            BluetoothClient bc = new BluetoothClient();
            List<String> items = new List<string> { };


            _devices = await Task.Run(() => bc.DiscoverDevicesInRange());
            return _devices;
        }

        public static async Task<bool> pairAsync(int pos, String pin = DEFAULT_PIN)
        {
            bool pairStatus;
            if (!_devices[pos].Authenticated)
            {
                pairStatus = await Task.Run(() => BluetoothSecurity.PairRequest(_devices[pos].DeviceAddress, pin));
                return pairStatus;
            }
            else
            {
                _selectedDevice = _devices[pos].DeviceAddress.ToString();
                pairStatus = true;
                return pairStatus;
            }

        }

        public static void send(String command)
        {
        }

        private static void receive()
        {
        }

    }
}
