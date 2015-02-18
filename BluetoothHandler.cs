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


namespace remote_inspection_unit_control
{
    static class BluetoothHandler
    {
        private static Dictionary<string, BluetoothDeviceInfo> _devicesInfo;
        private static BluetoothDeviceInfo _selectedDevice;
        private static BluetoothClient _client = new BluetoothClient();
        private static NetworkStream _bluetoothStream;

        public static void selectDevice(string device)
        {
            Guid _serviceGuid = new Guid("94f39d29-7d6d-437d-973b-fba39e49d4ee");
            _selectedDevice = _devicesInfo[device];
            try
            {
                BluetoothEndPoint blueEndPoint = new BluetoothEndPoint(_selectedDevice.DeviceAddress, _serviceGuid);

                // connecting
                _client.Connect(blueEndPoint);

                // get stream for send the data
                 _bluetoothStream = _client.GetStream();
            }
            catch(Exception)
            {
                MessageBox.Show("Could not connect to device, please make sure it is in range and switched on.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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

        public static async Task<bool> send(string content)
        { 
            //do not block UI thread
            Task<bool> task = Task.Run(() =>
            {    
                // if all is ok to send
                if(_client.Connected && _bluetoothStream != null)
                {
                    // write the data in the stream
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    _bluetoothStream.Write(buffer, 0, buffer.Length);
                    _bluetoothStream.Flush();
                    //bluetoothStream.Close();
                    return true;
                }
                return false;
            });
            return await task;
        }

        private static void receive()
        {
        }
    }
}
