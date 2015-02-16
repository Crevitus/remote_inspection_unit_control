using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.Threading.Tasks;


namespace remote_inspection_unit_control
{
	/// <summary>
	/// Interaction logic for bluetooth_search.xaml
	/// </summary>
	public partial class bluetooth_search : Window
	{
		public bluetooth_search()
		{
			this.InitializeComponent();
			this.MouseLeftButtonDown += delegate { this.DragMove(); };
            getDevices();
		}

        private async void getDevices()
        {
            try
            {
                List<string> items = new List<string> { };
                List<string> _devicesInfo = await BluetoothHandler.discoverAsync();
                
                if (_devicesInfo.Count> 0)
                {
                    foreach (string device in _devicesInfo)
                    {
                        lstDeviceView.Items.Add(device);
                    }
                    btnSelect.IsEnabled = true;
                    lstDeviceView.Focus();
                    lstDeviceView.SelectedIndex = 0;
                }
            }
            catch (System.PlatformNotSupportedException)
            {
                MessageBox.Show(Application.Current.MainWindow, "Please make sure that your hardware is supported and bluetooth is switched on.",
                "Bluetooth Search Failed");
            }
        }

         private async void btnSelect_Click(object sender, RoutedEventArgs e)
         {
             if(await BluetoothHandler.pairAsync(lstDeviceView.SelectedValue.ToString()))
             {
                 MessageBox.Show(Application.Current.MainWindow, "Device has been successfully connected!",
                "Device Connected!");
             }
             else
             {
                 MessageBox.Show(Application.Current.MainWindow, "Pairing error, please try again.",
                                 "Pairing Error!");
             }
         }

         private void btnSearch_Click(object sender, RoutedEventArgs e)
         {
             getDevices();
         }

         private void btnExitClick(object sender, System.Windows.RoutedEventArgs e)
         {
             Window.Close();
         }
	}
}