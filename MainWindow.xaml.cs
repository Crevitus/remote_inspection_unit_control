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
using System.Timers;

namespace remote_inspection_unit_control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IDataHandler
    {
        private bool _fullScreen = false;
        ImageConverter imgCon = new ImageConverter();
        Map mMap;
        Bitmap mImage;
        private System.Drawing.Point mMapPos = new System.Drawing.Point(0, 0);
        private readonly string FORWARD = "150", LEFT = "250", RIGHT = "350", BACKWARD = "450", STOP = "0";
        private bool mBlocking = false;
        private bool _manual = true;
        private bool _init = false;

        public MainWindow()
        {
            InitializeComponent();
            getDevices();
            ConnectionHandler.ConnectionChanged += ConnectionHandler_ConnectionChanged;
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(path + "\\Remote Inspection Unit"))
            {
                System.IO.Directory.CreateDirectory(path + "\\Remote Inspection Unit");
                System.IO.Directory.CreateDirectory(path + "\\Remote Inspection Unit\\Logs");
                System.IO.Directory.CreateDirectory(path + "\\Remote Inspection Unit\\Screenshots");
            }
        }

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

        void ConnectionHandler_ConnectionChanged(object sender, EventArgs e)
        {
            if(ConnectionHandler.Connected)
            {
                lstLogList.Items.Clear();
                addLogItem("System", "Connected to device");
                lblConStatus.Content = "Connected";
                lblConStatus.Foreground = new SolidColorBrush(Colors.Green);
                btnDisconnect.IsEnabled = true;
                btnSwitch.IsEnabled = true;
            }
            else
            {
                addLogItem("System", "Disconnected from device");
                btnSwitch.Content = "AI Control";
                _manual = true;
                cbxDeviceList.Text = "-- Select Device --";
                lblConStatus.Content = "Disconnected";
                lblConStatus.Foreground = new SolidColorBrush(Colors.DarkOrange);
                btnDisconnect.IsEnabled = false;
                btnSwitch.IsEnabled = false;
                mMap.refresh();
                refresh();
            }
        }

        //get available remote inspection devices
        private async void getDevices()
        {
            btnSearch.IsEnabled = false;
            try
            {
                cbxDeviceList.Items.Clear();
                cbxDeviceList.Text = "-- Searching --";
                List<string> items = new List<string> { };
                List<string> _devicesInfo = await ConnectionHandler.discoverAsync();

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
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Please make sure that WiFi is switched on.",
                "Device Search Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                cbxDeviceList.Text = "-- No Devices --";
            }
            btnSearch.IsEnabled = true;
        }

        //
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (ConnectionHandler.Connected)
            {
                if (MessageBox.Show("Drone is still connected, are you sure you want to exit? This will shutdown the drone.", "Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)
                    == MessageBoxResult.Yes)
                {
                    ConnectionHandler.send("exit");
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private async void cbxDeviceList_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (cbxDeviceList.SelectedItem != null)
                {
                    string device = cbxDeviceList.SelectedItem.ToString();
                    cbxDeviceList.Text = "-- Connecting --";
                    if (ConnectionHandler.Connected)
                    {
                        ConnectionHandler.disconnect();
                    }
                    if ((await ConnectionHandler.selectDevice(device)))
                    {
                        cbxDeviceList.Text = device;
                        ConnectionHandler.receive(this);
                        initialiseMap();
                    }
                    else
                    {
                        MessageBox.Show("Could not connect to device.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        cbxDeviceList.Text = "-- Select Device --";
                    }
                }
            }
            catch (NullReferenceException)
            {
                cbxDeviceList.Items.Clear();
                MessageBox.Show("Please make sure the device is switched on and in range.",
                "Device not found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void send(string data)
        {
            if (ConnectionHandler.Connected)
            {
                if (!ConnectionHandler.send(data))
                {
                    MessageBox.Show("Failed to send message to device.", "Transmission Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectionHandler.disconnect();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            getDevices();
        }

        public void dataHandler(byte[] data)
        {
            byte[] tempData = new byte[data.Length - 1];
            Buffer.BlockCopy(data, 1, tempData, 0, tempData.Length);
            char[] tempChar = Encoding.UTF8.GetString(data).ToCharArray();
            switch(tempChar[0])
            {
                case '1':
                    switch(Encoding.UTF8.GetString(tempData))
                    {
                        case "FORWARD":
                            mMap.Forward = true;
                            break;
                        case "nFORWARD":
                            mMap.Forward = false;
                            break;
                        case "REVERSE":
                            mMap.Back = true;
                            break;
                        case "nREVERSE":
                            mMap.Back = false;
                            break;
                        case "LEFT":
                            mMap.Left = true;
                            break;
                        case "nLEFT":
                            mMap.Left = false;
                            break;
                        case "RIGHT":
                            mMap.Right = true;
                            break;
                        case "nRIGHT":
                            mMap.Right = false;
                            break;
                    }
                    refresh();
                    break;
                case '2':
                    Bitmap frame = (Bitmap)imgCon.ConvertFrom(tempData);
                    refreshCamera(frame);
                    break;
                default:
                    if(lstLogList.Items.Count >= 400)
                    {
                        lstLogList.Items.Clear();
                    }
                    addLogItem("Remote Device", Encoding.UTF8.GetString(tempData));
                    break;
            }
        }
        public void refresh()
        {
            mMap.draw();
            imgSensors.Source = bitmapToImageSource(mImage);
            imgSensors.InvalidateVisual();
        }
        public void refreshCamera(Bitmap frame)
        {
            //refresh image
            imgPlayer.Source = bitmapToImageSource(frame);

            imgPlayer.InvalidateVisual();
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

        private void btnUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!mBlocking)
            {
                mBlocking = true;
                send(FORWARD);
            }
        }

        private void btnUp_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mBlocking = false;
            send(STOP);
        }

        private void btnRight_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!mBlocking)
            {
                mBlocking = true;
                send(RIGHT);
            }
        }

        private void btnRight_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mBlocking = false;
            send(STOP);
        }

        private void btnDown_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!mBlocking)
            {
                mBlocking = true;
                send(BACKWARD);
            }
        }

        private void btnDown_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mBlocking = false;
            send(STOP);
        }

        private void btnLeft_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!mBlocking)
            {
                mBlocking = true;
                send(LEFT);
            }
        }

        private void btnLeft_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mBlocking = false;
            send(STOP);
        }

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!mBlocking)
            {
                switch (e.Key)
                {
                    case Key.Up:
                        mBlocking = true;
                        send(FORWARD);
                        break;
                    case Key.Down:
                        mBlocking = true;
                        send(BACKWARD);
                        break;
                    case Key.Left:
                        mBlocking = true;
                        send(LEFT);
                        break;
                    case Key.Right:
                        mBlocking = true;
                        send(RIGHT);
                        break;
                    default:
                        break;
                }
            }
        }

        private void window_PreviewKeyUp(object sender, KeyEventArgs e)
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
            mBlocking = false;
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (_manual)
            {
                send("51");
                _manual = false;
                addLogItem("System", "Automatic path finding enabled");
                btnSwitch.Content = "Manual";
            }
            else
            {
                send("50");
                _manual = true;
                addLogItem("System", "Manual control enabled");
                btnSwitch.Content = "AIControl";
            }
        }

        private void addLogItem(string type, object item)
        {
            lstLogList.Items.Insert(0, ">> " + DateTime.Now.ToLongTimeString() + " [" + type + "] "+ item);
        }

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (imgPlayer.Source != null)
            {
                cnvsFlash.Background = new SolidColorBrush(Colors.White);
                await Task.Delay(100);
                cnvsFlash.Background = null;
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgPlayer.Source));
                string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (FileStream stream = new FileStream(path + "\\Remote Inspection Unit\\Screenshots\\" + DateTime.Now.ToString("yyyy MM dd HH mm ss") + ".jpg", FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            lstLogList.Items.Clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (lstLogList.Items.Count != 0)
            {
                string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (StreamWriter SaveFile = new StreamWriter(path + "\\Remote Inspection Unit\\Logs\\Log " + DateTime.Now.ToString("yyyy MM dd HH mm ss") + ".txt"))
                {
                    foreach (var item in lstLogList.Items)
                    {
                        SaveFile.WriteLine(item.ToString());
                    }
                }
                addLogItem("System", "Log saved");
            }
        }
    }
}
