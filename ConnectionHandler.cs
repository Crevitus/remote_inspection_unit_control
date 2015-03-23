using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.Net;
using NativeWifi;


namespace remote_inspection_unit_control
{
    static class ConnectionHandler
    {
        private static Dictionary<string, Wlan.WlanAvailableNetwork> _devicesInfo;
        private static Wlan.WlanAvailableNetwork _selectedDevice;
        private static TcpClient _tcpClient = new TcpClient();
        private static NetworkStream _dataStream;
        private static WlanClient _wifiClient;
        private static bool _connected = false;
        private static bool _receive = false;
        private static readonly string IP = "192.168.42.1";
        private static readonly int PORT = 6756;
        private static readonly string KEY = "raspberry";
        private static readonly int RETRYS = 10;

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
            _dataStream.Dispose();
            _tcpClient.Close();
            _receive = false;
            _connected = false;
        }

        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        public static async Task<List<String>> discoverAsync()
        {
            List<String> items = new List<string> { };
            _devicesInfo = new Dictionary<string, Wlan.WlanAvailableNetwork> { };
            _wifiClient = new WlanClient();
            foreach (WlanClient.WlanInterface wlanIface in _wifiClient.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = await Task.Run(() => wlanIface.GetAvailableNetworkList(0));
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    if (GetStringForSSID(network.dot11Ssid).StartsWith("RaspAP") && !_devicesInfo.ContainsKey(GetStringForSSID(network.dot11Ssid)))
                    {
                        items.Add(GetStringForSSID(network.dot11Ssid));
                        _devicesInfo.Add(GetStringForSSID(network.dot11Ssid), network);

                    }
                }
            }

            return items;
        }

        public static async Task<bool> selectDevice(string device)
        {
            if (_dataStream != null)
            {
                _dataStream.Close();
                _tcpClient.Close();
            }

            _selectedDevice = _devicesInfo[device];
            foreach (WlanClient.WlanInterface wlanIface in _wifiClient.Interfaces)
            {
                _tcpClient = new TcpClient();
                // connecting
                _selectedDevice = _devicesInfo[device];
                string profileXml = string.Format("<?xml version=\"1.0\" encoding=\"US-ASCII\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>WPA2PSK</authentication><encryption>AES</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>passPhrase</keyType><protected>false</protected><keyMaterial>{1}</keyMaterial></sharedKey></security></MSM></WLANProfile>", _selectedDevice.profileName, KEY);
                wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, _selectedDevice.profileName);
                for (int i = 0; i < RETRYS; i++)
                {
                    try
                    {
                        await _tcpClient.ConnectAsync(IP, PORT);
                        _connected = true;
                        break;
                    }
                    catch (SocketException)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                if (_connected)
                {
                    _dataStream = _tcpClient.GetStream();
                }
                else
                {
                    MessageBox.Show("Could not connect to device.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _connected = false;
                }
            }

            return _connected;
        }

        public static bool send(string content)
        {
            if (_tcpClient.Connected && _dataStream != null)
            {
                // write the data in the stream
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                _dataStream.Write(buffer, 0, buffer.Length);
                _dataStream.Flush();
                return true;
            }
            else
            {
                MessageBox.Show("Failed to send message to device.", "Transmission Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                            if ((bytesRead = await _dataStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
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
