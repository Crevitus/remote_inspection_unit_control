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
        private static bool _receive = false;
        private static Guid _serviceGuid = new Guid("6d617474-6865-7769-7361-676179626f79");

        public static bool Connected
        {
            get { return _connected; }
        }

        public static bool Receive
        {
            set { _receive = value; }
        }

        public static void disconnect()
        {
            _bluetoothStream.Dispose();
            _connected = false;
        }

        public static async Task<bool> selectDevice(string device)
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
                await Task.Run(() => _client.Connect(blueEndPoint));

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
                _bluetoothStream.Flush();
                return true;
            }
            else
            {
                MessageBox.Show("Failed to send command to device.", "Transmission Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static async void receive(IDataHandler reference)
        {
            while (_receive)
            {
                try
                {
                    if (_connected)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            byte[] buffer = new byte[2048]; // read in chunks of 2KB
                            int bytesRead;
                            if ((bytesRead = await _bluetoothStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                stream.Write(buffer, 0, bytesRead);
                                byte[] result = stream.ToArray();
                                reference.dataHandler(result);
                            }
                        } // end using
                    } //end connection check
                } //end try
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (IOException)
                {
                    break;
                }
            }
        }
    }
}
