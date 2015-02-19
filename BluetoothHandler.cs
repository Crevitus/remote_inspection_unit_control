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
using System.Net.Sockets;
using System.IO;


namespace remote_inspection_unit_control
{
    static class BluetoothHandler
    {
        private static Dictionary<string, BluetoothDeviceInfo> _devicesInfo;
        private static BluetoothDeviceInfo _selectedDevice;
        private static BluetoothClient _client = new BluetoothClient();
        private static NetworkStream _bluetoothStream;
        private static bool _connected = false;

        public static bool Connected
        {
            get { return _connected; }
        }

        public static void disconnect()
        {
            _bluetoothStream.Dispose();
        }

        public static bool selectDevice(string device)
        {
            if (_bluetoothStream != null)
            {
                _bluetoothStream.Close();
                _client = new BluetoothClient();
            }
            Guid _serviceGuid = new Guid("42656e20-6c6f-7665-7320-636f636b7321");
            _selectedDevice = _devicesInfo[device];
            try
            {
                BluetoothEndPoint blueEndPoint = new BluetoothEndPoint(_selectedDevice.DeviceAddress, _serviceGuid);

                // connecting
                _client.Connect(blueEndPoint);

                // get stream for send the data
                 _bluetoothStream = _client.GetStream();
                 _connected = true;
            }
            catch(SocketException)
            {
                MessageBox.Show("Could not connect to device, please make sure it is in range and switched on.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return _connected;
        }

        public static async Task<List<String>> discoverAsync()
        {
            List<String> items = new List<string> { };
            BluetoothDeviceInfo[] devices = await Task.Run(() => _client.DiscoverDevicesInRange());
            _devicesInfo = new Dictionary<string, BluetoothDeviceInfo> { };
            foreach (BluetoothDeviceInfo device in devices)
            {
                items.Add(device.DeviceName);
                _devicesInfo.Add(device.DeviceName, device);
            }
            return items;
        }

        public static void send(string content)
        {
            try
            {
                // if all is ok to send
                if (_client.Connected && _bluetoothStream != null)
                {
                    // write the data in the stream
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    _bluetoothStream.Write(buffer, 0, buffer.Length);
                    _bluetoothStream.Flush();
                    //bluetoothStream.Close();

                }
                else
                {
                    throw new IOException();
                }
            }
            catch(IOException)
            {
                MessageBox.Show("Failed to send message to device.", "Transmission Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void receive()
        {
        }
    }
}
