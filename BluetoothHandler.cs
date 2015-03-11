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
        private static Guid _serviceGuid = new Guid("42656e20-6c6f-7665-7320-636f636b7321");

        public static bool Connected
        {
            get { return _connected; }
        }
        public static void disconnect()
        {
            _bluetoothStream.Dispose();
            _connected = false;
        }

        public static bool selectDevice(string device)
        {
            if (_bluetoothStream != null)
            {
                _bluetoothStream.Close();
                _client = new BluetoothClient();
            }
     
            _selectedDevice = _devicesInfo[device];
            try
            {
                BluetoothEndPoint blueEndPoint = new BluetoothEndPoint(_selectedDevice.DeviceAddress, _serviceGuid);

                // connecting
                _client.Connect(blueEndPoint);

                 _bluetoothStream = _client.GetStream();
                 _connected = true;
            }
            catch(SocketException)
            {
                _connected = false;
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

        public static bool send(string content)
        {
            if (_client.Connected && _bluetoothStream != null)
            {
                // write the data in the stream
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                _bluetoothStream.Write(buffer, 0, buffer.Length);
                //System.Threading.Thread.Sleep(5);
                _bluetoothStream.Flush();
                return true;
            }
            else
            {
                MessageBox.Show("Failed to send message to device.", "Transmission Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static async Task<string> receive()
        {
            string data = "";
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] buffer = new byte[2048]; // read in chunks of 2KB
                int bytesRead;
                if((bytesRead = await _bluetoothStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                    byte[] result = stream.ToArray();
                    data = System.Text.Encoding.Default.GetString(result);
                }
            }
            return data;
        }
    }
}
