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
        private static Dictionary<String, BluetoothAddress> deviceInfo = new Dictionary<string, BluetoothAddress> { };
        private static readonly String DEFAULT_PIN = "1234";


        public static bool isSupported()
        {
            if(!BluetoothRadio.IsSupported)
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

        public static bool pair(String name)
        {
            bool isPaired;
            isPaired = BluetoothSecurity.PairRequest(deviceInfo[name], DEFAULT_PIN);
            
            if(isPaired)
            {
                return true;
            }
            else
            {
                return false;
            }  
        }

        public static bool pair(String name, String pin)
        {
            bool isPaired;
            isPaired = BluetoothSecurity.PairRequest(deviceInfo[name], pin);
            
            if(isPaired)
            {
                return true;
            }
            else
            {
                return false;
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
