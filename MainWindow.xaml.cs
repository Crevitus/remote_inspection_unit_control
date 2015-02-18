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
        bool fullScreen = false;
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
            if (MessageBox.Show("Drone is still connected, are you sure you want to exit?", "Exit?", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
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
				btnMaximize.Content = "2";
				btnMaximize.ToolTip = "Restore";
			}
			else
			{
				WindowState = WindowState.Normal;
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await BluetoothHandler.send("shutdown");
        }

        private void cbxDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string device = cbxDeviceList.SelectedItem.ToString();
            BluetoothHandler.selectDevice(device);
        }

        private void btnMediaFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if(!fullScreen)
            {
                layoutRoot.Children.Remove(gdMediaWrapper);
                this.Background = new SolidColorBrush(Colors.Black);
                this.Content = gdMediaWrapper;
                this.MaxHeight = SystemParameters.MaximumWindowTrackHeight;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.Content = layoutRoot;
                layoutRoot.Children.Add(gdMediaWrapper);
                this.Background = new SolidColorBrush(Colors.LightGray);
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.WindowState = WindowState.Normal;
            }
            fullScreen = !fullScreen;
        }
        private void btnMapFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (!fullScreen)
            {
                layoutRoot.Children.Remove(gdMapWrapper);
                this.Background = new SolidColorBrush(Colors.Black);
                this.Content = gdMapWrapper;
                this.MaxHeight = SystemParameters.MaximumWindowTrackHeight;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.Content = layoutRoot;
                layoutRoot.Children.Add(gdMapWrapper);
                this.Background = new SolidColorBrush(Colors.LightGray);
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.WindowState = WindowState.Normal;
            }
            fullScreen = !fullScreen;
        }
    }
}
