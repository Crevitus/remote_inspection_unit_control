using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace remote_inspection_unit_control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _fullScreen = false;
        bool _isMax = false;
        public MainWindow()
        {
            InitializeComponent();
			this.MouseLeftButtonDown += delegate { this.DragMove(); };
			this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            getDevices();
        }

        private async void getDevices()
        {
            try
            {
                List<string> items = new List<string> { };
                List<string> _devicesInfo = await BluetoothHandler.discoverAsync();

                if (_devicesInfo.Count > 0)
                {
                    foreach (string device in _devicesInfo)
                    {
                        cbxDeviceList.Items.Add(device);
                    }
                }
            }
            catch (System.PlatformNotSupportedException)
            {
                MessageBox.Show(Application.Current.MainWindow, "Please make sure that your hardware is supported and bluetooth is switched on.",
                "Bluetooth Search Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExitClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if(BluetoothHandler.Connected)
            {
                if (MessageBox.Show("Drone is still connected, are you sure you want to exit? This will shutdown the drone.", "Exit?", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    BluetoothHandler.send("exit");
                    Application.Current.Shutdown();
                }
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
		
		private void btnMinClick(object sender, System.Windows.RoutedEventArgs e)
        {
    	  WindowState = WindowState.Minimized;
        }
		
		private void btnMaxClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	if(WindowState == WindowState.Normal)
			{
				WindowState = WindowState.Maximized;
                _isMax = true;
				btnMaximize.Content = "2";
				btnMaximize.ToolTip = "Restore";
			}
			else
			{
				WindowState = WindowState.Normal;
                _isMax = false;
				btnMaximize.Content = "1";
				btnMaximize.ToolTip = "Maximize";
			}
			
        }

        private void btnDevices_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.send("m");
        }

        private void cbxDeviceList_DropDownClosed(object sender, EventArgs e)
        {
            if (cbxDeviceList.SelectedItem != null)
            {
                string device = cbxDeviceList.SelectedItem.ToString();
                if (BluetoothHandler.selectDevice(device))
                {
                    lblConStatus.Content = "Connected";
                    lblConStatus.Foreground = new SolidColorBrush(Colors.Green);
                    btnDisconnect.IsEnabled = true;
                }
                else
                {
                    cbxDeviceList.Text = "-- Select Device --";
                }
            }
        }

        private void btnMediaFullScreen_Click(object sender, RoutedEventArgs e)
        {
            fullScreen(gdMediaWrapper);
        }
        private void btnMapFullScreen_Click(object sender, RoutedEventArgs e)
        {
            fullScreen(gdMapWrapper);
        }

        private void fullScreen(Grid control)
        {
            if (!_fullScreen)
            {
                layoutRoot.Children.Remove(control);
                this.Background = new SolidColorBrush(Colors.Black);
                this.Content = control;
                this.MaxHeight = 100000;
                control.Margin = new Thickness(8);
                this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.Content = layoutRoot;
                layoutRoot.Children.Add(control);
                this.Background = new SolidColorBrush(Colors.LightGray);
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                if (control.Name == "gdMapWrapper")
                {
                    control.Margin = new Thickness(4, 0, 8, 0);
                }
                else
                {
                    control.Margin = new Thickness(8, 0, 4, 0);
                }
                if (!_isMax)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    this.WindowState = WindowState.Maximized;
                }
            }
            _fullScreen = !_fullScreen;
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.send("0150");
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.send("1150");
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.send("2150");
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.send("3150");
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.disconnect();
            lblConStatus.Content = "Disconnected";
            lblConStatus.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            getDevices();
        }

    }
}
