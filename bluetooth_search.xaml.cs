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
                List<String> devices = await BluetoothHandler.discoverAsync();
                
                foreach (object device in devices)
                {
                    lstDeviceView.Items.Add(device);
                }
                if (devices.Count > 0)
                {
                    btnNext.IsEnabled = true;
                    lstDeviceView.Focus();
                    lstDeviceView.SelectedIndex = 0;
                }
            }
            catch (System.PlatformNotSupportedException)
            {
                MessageBox.Show("Please make sure that your hardware is supported and bluetooth is switched on",
                "Bluetooth Search Failed.");
            }
        }
		
		 private void btnExitClick(object sender, System.Windows.RoutedEventArgs e)
        {

        	Window.Close();
        }

         private void btnNext_Click(object sender, RoutedEventArgs e)
         {
             BluetoothHandler.pair(lstDeviceView.SelectedValue.ToString());
         }

         private void btnSearch_Click(object sender, RoutedEventArgs e)
         {
             getDevices();
         }
	}
}