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
            try
            {
                List<String> devices = BluetoothHandler.discover();
                foreach (object device in devices)
                {
                    deviceView.Items.Add(device);
                }
            }
            catch(System.PlatformNotSupportedException)
            {
                MessageBox.Show("Please make sure that your hardware is supported and bluetooth is switched on",
                "Bluetooth Search Failed.");
            }
		}
		
		 private void btnExitClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	Window.Close();
        }

         private void nextBtn_Click(object sender, RoutedEventArgs e)
         {
             BluetoothHandler.pair(deviceView.SelectedValue.ToString());
         }

	}
}