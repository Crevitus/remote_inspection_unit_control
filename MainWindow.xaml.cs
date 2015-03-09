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
using MahApps.Metro.Controls;
using System.Drawing;
using System.IO;

namespace remote_inspection_unit_control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool _fullScreen = false;
        private bool _init = false;
        private Map mMap;
        private bool mDown = false;
        private int mPosX = 0, mPosY = 0;
        private System.Drawing.Point mMapPos = new System.Drawing.Point(0, 0);
        private int mPrev = -1;
        private Bitmap mImage;
        private readonly string FORWARD = "450", LEFT = "350", RIGHT = "250", BACKWARD = "150", STOP = "0";

        public MainWindow()
        {
            InitializeComponent();
            layoutRoot.Focus();
            getDevices();
        }

        //sets map dimensions after layout pass
        private void initialiseMap()
        {
            if (!_init)
            {
                mImage = new Bitmap((int)gdMapWrapper.ActualWidth, (int)gdMapWrapper.ActualHeight);
                mMap = new Map(mImage);
                refresh();
                _init = true;
            }
        }

        //get available bluetooth devices
        private async void getDevices()
        {
            btnSearch.IsEnabled = false;
            try
            {
                cbxDeviceList.Items.Clear();
                cbxDeviceList.Text = "-- Searching --";
                List<string> items = new List<string> { };
                List<string> _devicesInfo = await BluetoothHandler.discoverAsync();

                if (_devicesInfo.Count > 0)
                {
                    foreach (string device in _devicesInfo)
                    {
                        cbxDeviceList.Items.Add(device);
                    }
                    cbxDeviceList.Text = "-- Select Device --";
                }
                else
                {
                    cbxDeviceList.Text = "-- No Devices --";
                }
            }
            catch (System.PlatformNotSupportedException)
            {
                MessageBox.Show("Please make sure that your hardware is supported and bluetooth is switched on.",
                "Bluetooth Search Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            btnSearch.IsEnabled = true;
        }

        //
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
 	         base.OnClosing(e);
             if(BluetoothHandler.Connected)
            {
                if (MessageBox.Show("Drone is still connected, are you sure you want to exit? This will shutdown the drone.", "Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)
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

        private void btnNorth_Click(object sender, RoutedEventArgs e)
        {
            initialiseMap();
            bool[] b = new bool[] { false, true, true, true };
            if (mPrev >= 0) b[mPrev] = false;
            mMap.add(mMapPos, b);
            mMapPos.Y -= 1;
            refresh();
            mPrev = 1;
        }
        private void btnEast_Click(object sender, RoutedEventArgs e)
        {
            initialiseMap();
            bool[] b = new bool[] { true, true, true, false };
            if (mPrev >= 0) b[mPrev] = false;
            mMap.add(mMapPos, b);
            mMapPos.X += 1;
            refresh();
            mPrev = 2;
        }
        private void btnSouth_Click(object sender, RoutedEventArgs e)
        {
            initialiseMap();
            bool[] b = new bool[] { true, false, true, true };
            if (mPrev >= 0) b[mPrev] = false;
            mMap.add(mMapPos, b);
            mMapPos.Y += 1;
            refresh();
            mPrev = 0;
        }
        private void btnWest_Click(object sender, RoutedEventArgs e)
        {
            initialiseMap();
            bool[] b = new bool[] { true, true, false, true };
            if (mPrev >= 0) b[mPrev] = false;
            mMap.add(mMapPos, b);
            mMapPos.X -= 1;
            refresh();
            mPrev = 3;
        }

          public void refresh()
        {
            mMap.draw();
            map.Source = bitmapToImageSource(mImage);
            map.InvalidateVisual();
        }

        BitmapImage bitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        
        private void map_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mDown = true;
            mPosX = (int)e.GetPosition(this).X;
            mPosY = (int)e.GetPosition(this).Y;
        }

        private void map_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (mDown)
            {
                int tol = 30;
                int xDif = (int)(mPosX - e.GetPosition(this).X);
                int yDif = (int)(mPosY - e.GetPosition(this).Y);
                if (xDif < tol && xDif > -tol &&
                    yDif < tol && yDif > -tol) return;

                if (xDif > yDif)
                {
                    if (xDif > 0) yDif = 0;
                    else xDif = 0;
                }
                if (yDif > xDif)
                {
                    if (yDif > 0) xDif = 0;
                    else yDif = 0;
                }

                if (xDif < 0) xDif = -1;
                if (xDif > 0) xDif = 1;
                if (yDif > 0) yDif = 1;
                if (yDif < 0) yDif = -1;
                mMap.StartLoc = new System.Drawing.Point(mMap.StartLoc.X + xDif, mMap.StartLoc.Y + yDif);
                refresh();
                mPosX = (int)e.GetPosition(this).X;
                mPosY = (int)e.GetPosition(this).Y;
            }
        }

        private void map_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mDown = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mMap.StartLoc = new System.Drawing.Point(0, 0);
            refresh();
        }

        private void map_MouseLeave(object sender, MouseEventArgs e)
        {
            mDown = false;
        }

        private void cbxDeviceList_DropDownClosed(object sender, EventArgs e)
        {
            if (cbxDeviceList.SelectedItem != null)
            {
                string device = cbxDeviceList.SelectedItem.ToString();
                if (BluetoothHandler.selectDevice(device))
                {
                    getData();
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
                this.Content = control;
                control.Margin = new Thickness(0);
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.Content = layoutRoot;
                layoutRoot.Children.Add(control);
                if (control.Name == "gdMapWrapper")
                {
                    control.Margin = new Thickness(4, 0, 8, 0);
                }
                else
                {
                    control.Margin = new Thickness(8, 0, 4, 0);
                }

                this.WindowState = WindowState.Normal;

            }
            _fullScreen = !_fullScreen;
        }

        private void send(string data)
        {
            if(!BluetoothHandler.send(data))
            {
                lblConStatus.Content = "Disconnected";
                lblConStatus.Foreground = new SolidColorBrush(Colors.DarkOrange);
                btnDisconnect.IsEnabled = false;
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.disconnect();
            cbxDeviceList.Text = "-- Select Device --";
            lblConStatus.Content = "Disconnected";
            btnDisconnect.IsEnabled = false;
            lblConStatus.Foreground = new SolidColorBrush(Colors.DarkOrange);
        }

        private async void getData()
        {
            string data;
                while (true)
                {
                    try
                    {
                        if (BluetoothHandler.Connected)
                        {
                            data = await BluetoothHandler.receive();
                            lstLogList.Items.Add(data);
                        }
                    }
                    catch(ObjectDisposedException)
                    {
                        break;
                    }
                }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            getDevices();
        }

 
        private void btnUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            send(FORWARD);
        }

        private void btnUp_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            send(STOP);
        }

        private void btnRight_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            send(RIGHT);
        }

        private void btnRight_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            send(STOP);
        }

        private void btnDown_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            send(BACKWARD);
        }

        private void btnDown_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            send(STOP);
        }

        private void btnLeft_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            send(LEFT);
        }

        private void btnLeft_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            send(STOP);
        }

        private void layoutRoot_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    send(FORWARD);
                    break;
                case Key.Down:
                    send(BACKWARD);
                    break;
                case Key.Left:
                    send(LEFT);
                    break;
                case Key.Right:
                    send(RIGHT);
                    break;
                default:
                    break;
            }
        }

        private void layoutRoot_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    send(STOP);
                    break;
                case Key.Down:
                    send(STOP);
                    break;
                case Key.Left:
                    send(STOP);
                    break;
                case Key.Right:
                    send(STOP);
                    break;
                default:
                    break;
            }
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            mMap.Size -= 1;
            refresh();
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            mMap.Size += 1;
            refresh();
        }

    }
}
