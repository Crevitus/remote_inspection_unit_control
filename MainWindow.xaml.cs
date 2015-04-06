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
        private bool _init = false;
        private Map mMap;
        private bool mDown = false;
        private int mPosX = 0, mPosY = 0;
        private System.Drawing.Point mMapPos = new System.Drawing.Point(0, 0);
        private int mPrev = -1;
        private Bitmap mImage;
        private readonly string FORWARD = "150", LEFT = "250", RIGHT = "350", BACKWARD = "450", STOP = "0";
        private bool mBlocking = false;
        private bool _manual = true;

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

        void ConnectionHandler_ConnectionChanged(object sender, EventArgs e)
        {
            if(ConnectionHandler.Connected)
            {
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
                        initialiseMap();
                        ConnectionHandler.receive(this);
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
                    switch(Encoding.UTF8.GetString(data))
                    {
                        case "forward":
                            goForward();
                            break;
                        case "back":
                            goBack();
                            break;
                        case "left":
                            goLeft();
                            break;
                        case "right":
                            goRight();
                            break;
                    }
                    break;
                case '2':
                    Bitmap frame = (Bitmap)imgCon.ConvertFrom(tempData);
                    refreshCamera(frame);
                    break;
                default:
                    if(lstLogList.Items.Count >= 200)
                    {
                        lstLogList.Items.Clear();
                    }
                    addLogItem("Remote Device", Encoding.UTF8.GetString(tempData));
                    break;
            }
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
                if (_init)
                {
                    mMap.Size += 6;
                    mMap.PenSize = 2;
                    refresh();
                }
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
                if (_init)
                {
                    mMap.Size -= 6;
                    mMap.PenSize = 5;
                    refresh();
                }

            }
            _fullScreen = !_fullScreen;
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

        private void goForward()
        {
            bool[] b = new bool[] { false, true, true, true };
            if (mPrev >= 0) b[mPrev] = false;
            mMap.add(mMapPos, b);
            mMapPos.Y -= 1;
            refresh();
            mPrev = 1;
        }
        private void goRight()
        {
            bool[] b = new bool[] { true, true, true, false };
            if (mPrev >= 0) b[mPrev] = false;
            mMap.add(mMapPos, b);
            mMapPos.X += 1;
            refresh();
            mPrev = 2;
        }
        private void goBack()
        {
            bool[] b = new bool[] { true, false, true, true };
            if (mPrev >= 0) b[mPrev] = false;
            mMap.add(mMapPos, b);
            mMapPos.Y += 1;
            refresh();
            mPrev = 0;
        }
        private void goLeft()
        {
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

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (_init)
            {
                mMap.Size -= 1;
                refresh();
            }
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (_init)
            {
                mMap.Size += 1;
                refresh();
            }
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
