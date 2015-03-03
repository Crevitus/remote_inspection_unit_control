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

namespace remote_inspection_unit_control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool _fullScreen = false;
        private bool _init = false;
        private Compass _orientation;
        private Point currentPos;
        private Line line, line2;
        private Canvas horizontalLine, verticalLine, rightBottom, leftBottom, rightTop, leftTop;
        private Dictionary<Point, drawType> position = new Dictionary<Point, drawType>{};
        private readonly string FORWARD = "450", LEFT = "350", RIGHT = "250", BACKWARD = "150", STOP = "0";

        public MainWindow()
        {
            InitializeComponent();
            layoutRoot.Focus();
            getDevices();
        }

        private enum Compass
        {
            North, East, South, West
        };

        private enum drawType
        {
            horizontalLine, verticalLine, rightBottom, leftBottom, rightTop, leftTop
        };

        private void initialiseMap()
        {
            if (!_init)
            {
                int x, y;
                x = (int)gdMap.ActualWidth;
                y = (int)gdMap.ActualHeight;
                refreshGrid(x, y);
                currentPos.X = (int)5;
                currentPos.Y = (int)5;
                _init = true;
            }
        }

        private void refreshGrid(int x, int y)
        {
            int rows, col;
            ColumnDefinition gridCol;
            RowDefinition gridrow;

            col = x / 30;
            rows = y / 30;

            for (int i = 0; i < col; i++)
            {
                gridCol = new ColumnDefinition();
                gridCol.Name = "col" + i.ToString();
                gridCol.Width = new GridLength(30);
                gdMap.ColumnDefinitions.Add(gridCol);
            }

            for (int i = 0; i < rows; i++)
            {

                gridrow = new RowDefinition();
                gridrow.Name = "row" + i.ToString();
                gridrow.Height = new GridLength(30);
                gdMap.RowDefinitions.Add(gridrow);
            }

        }

        private void refreshHorizontalLine()
        {
            horizontalLine = new Canvas();
            refreshLines();

            line.X1 = 0;
            line.X2 = 30;
            line.Y1 = 0;
            line.Y2 = 0;

            line2.X1 = 0;
            line2.X2 = 30;
            line2.Y1 = 30;
            line2.Y2 = 30;

            horizontalLine.Children.Add(line);
            horizontalLine.Children.Add(line2);
        }

        private void refreshVerticalLine()
        {
            verticalLine = new Canvas();
            refreshLines();
            line.X1 = 0;
            line.X2 = 0;
            line.Y1 = 0;
            line.Y2 = 30;

            line2.X1 = 30;
            line2.X2 = 30;
            line2.Y1 = 0;
            line2.Y2 = 30;

            verticalLine.Children.Add(line);
            verticalLine.Children.Add(line2);
        }

        private void refreshRightBottom()
        {
            rightBottom = new Canvas();
            refreshLines();

            line.X1 = 30;
            line.X2 = 30;
            line.Y1 = 0;
            line.Y2 = 30;

            line2.X1 = 30;
            line2.X2 = 0;
            line2.Y1 = 30;
            line2.Y2 = 30;

            rightBottom.Children.Add(line);
            rightBottom.Children.Add(line2);
        }

        private void refreshLeftBottom()
        {
            leftBottom = new Canvas();
            refreshLines();

            line.X1 = 0;
            line.X2 = 0;
            line.Y1 = 0;
            line.Y2 = 30;

            line2.X1 = 0;
            line2.X2 = 30;
            line2.Y1 = 30;
            line2.Y2 = 30;

            leftBottom.Children.Add(line);
            leftBottom.Children.Add(line2);
        }

        private void refreshRightTop()
        {
            rightTop = new Canvas();
            refreshLines();

            line.X1 = 0;
            line.X2 = 30;
            line.Y1 = 0;
            line.Y2 = 0;

            line2.X1 = 30;
            line2.X2 = 30;
            line2.Y1 = 0;
            line2.Y2 = 30;

            rightTop.Children.Add(line);
            rightTop.Children.Add(line2);
        }

        private void refreshLeftTop()
        {
            leftTop = new Canvas();
            refreshLines();

            line.X1 = 0;
            line.X2 = 30;
            line.Y1 = 0;
            line.Y2 = 0;

            line2.X1 = 0;
            line2.X2 = 0;
            line2.Y1 = 0;
            line2.Y2 = 30;

            leftTop.Children.Add(line);
            leftTop.Children.Add(line2);
        }

        private void refreshLines()
        {
            // Create a red Brush
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.DarkOrange;
            //first line
            line = new Line();
            line.StrokeThickness = 4;
            line.Stroke = redBrush;
            line.StrokeThickness = 4;
            line.Stroke = redBrush;

            //second line
            line2 = new Line();
            line2.StrokeThickness = 4;
            line2.Stroke = redBrush;
            line2.StrokeThickness = 4;
            line2.Stroke = redBrush;
        }

        private void getPos()
        {
            
        }

        private void setPos(int col, int row, drawType type)
        {
            Point tempPos = new Point();
            tempPos.X = col;
            tempPos.Y = row;
            position.Add(tempPos, type);
        }

        private void drawLine(Compass direction)
        {
            initialiseMap();
            if (currentPos.Y == 0)
            {
                gdMap.RowDefinitions.Clear();
                currentPos.Y = currentPos.Y + 8;
                foreach (UIElement child in gdMap.Children)
                {
                    int tempRow = (int)child.GetValue(Grid.RowProperty);
                    child.SetValue(Grid.RowProperty, tempRow + 8);
                }
                gdMap.Height = gdMap.ActualHeight + 240;
                refreshGrid((int)gdMap.ActualWidth, (int)gdMap.Height);
            }
            if (currentPos.X == 0)
            {
                gdMap.ColumnDefinitions.Clear();
                currentPos.X = currentPos.X + 8;
                foreach (UIElement child in gdMap.Children)
                {
                    int tempCol = (int)child.GetValue(Grid.ColumnProperty);
                    child.SetValue(Grid.ColumnProperty, tempCol + 8);
                }
                gdMap.Width = gdMap.ActualWidth + 240;
                refreshGrid((int)gdMap.Width, (int)gdMap.ActualHeight);
            }
            //direction to go
            switch (direction)
            {
                case Compass.North:
                    //direction facing
                    switch (_orientation)
                    {
                        case Compass.North:
                           refreshVerticalLine();
                           currentPos.Y = currentPos.Y - 1;
                           verticalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                           verticalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                           gdMap.Children.Add(verticalLine);
                            break;
                        case Compass.East:
                           refreshRightBottom();
                           currentPos.X = currentPos.X + 1;
                           rightBottom.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                           rightBottom.SetValue(Grid.RowProperty, (int)currentPos.Y);
                           gdMap.Children.Add(rightBottom);
                            break;
                        case Compass.South:
                           refreshVerticalLine();
                           currentPos.Y = currentPos.Y - 1;
                           verticalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                           verticalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                           gdMap.Children.Add(verticalLine);
                            break;
                        case Compass.West:
                           refreshLeftBottom();
                           currentPos.X = currentPos.X - 1;
                           leftBottom.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                           leftBottom.SetValue(Grid.RowProperty, (int)currentPos.Y);
                           gdMap.Children.Add(leftBottom);
                            break;
                    }
                    _orientation = Compass.North;
                    break;
                case Compass.East:
                    //direction facing
                    switch (_orientation)
                    {
                        case Compass.North:
                            refreshLeftTop();
                            currentPos.Y = currentPos.Y - 1;
                            leftTop.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            leftTop.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(leftTop);
                            break;
                        case Compass.East:
                            refreshHorizontalLine();
                            currentPos.X = currentPos.X + 1;
                            horizontalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            horizontalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(horizontalLine);
                            break;
                        case Compass.South:
                            refreshLeftBottom();
                            currentPos.Y = currentPos.Y + 1;
                            leftBottom.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            leftBottom.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(leftBottom);
                            break;
                        case Compass.West:
                            refreshHorizontalLine();
                            currentPos.X = currentPos.X + 1;
                            horizontalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            horizontalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(horizontalLine);
                            break;
                    }
                    _orientation = Compass.East;
                    break;
                case Compass.South:
                    //direction facing
                    switch (_orientation)
                    {
                        case Compass.North:
                            refreshVerticalLine();
                            currentPos.Y = currentPos.Y + 1;
                            verticalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            verticalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(verticalLine);
                            break;
                        case Compass.East:
                            refreshRightTop();
                            currentPos.X = currentPos.X + 1;
                            rightTop.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            rightTop.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(rightTop);
                            break;
                        case Compass.South:
                            refreshVerticalLine();
                            currentPos.Y = currentPos.Y + 1;
                            verticalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            verticalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(verticalLine);
                            break;
                        case Compass.West:
                            refreshLeftTop();
                            currentPos.X = currentPos.X - 1;
                            leftTop.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            leftTop.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(leftTop);
                            break;
                    }
                    _orientation = Compass.South;
                    break;
                case Compass.West:
                    //direction facing
                    switch (_orientation)
                    {
                        case Compass.North:
                            refreshRightTop();
                            currentPos.Y = currentPos.Y - 1;
                            rightTop.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            rightTop.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(rightTop);
                            break;
                        case Compass.East:
                            refreshHorizontalLine();
                            currentPos.X = currentPos.X - 1;
                            horizontalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            horizontalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(horizontalLine);
                            break;
                        case Compass.South:
                            refreshRightBottom();
                            currentPos.Y = currentPos.Y + 1;
                            rightBottom.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            rightBottom.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(rightBottom);
                            break;
                        case Compass.West:
                            refreshHorizontalLine();
                            currentPos.X = currentPos.X - 1;
                            horizontalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            horizontalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMap.Children.Add(horizontalLine);
                            break;
                    }
                    _orientation = Compass.West;
                    break;
            }
        }

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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
 	         base.OnClosing(e);
             if(BluetoothHandler.Connected)
            {
                if (MessageBox.Show("Drone is still connected, are you sure you want to exit? This will shutdown the drone.", "Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning)
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
		
		
        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            

        }
        private void btnNorth_Click(object sender, RoutedEventArgs e)
        {
            drawLine(Compass.North);
        }
        private void btnEast_Click(object sender, RoutedEventArgs e)
        {
            drawLine(Compass.East);
        }
        private void btnSouth_Click(object sender, RoutedEventArgs e)
        {
            drawLine(Compass.South);
        }
        private void btnWest_Click(object sender, RoutedEventArgs e)
        {
            drawLine(Compass.West);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            initialiseMap();
        }

        private void cbxDeviceList_DropDownClosed(object sender, EventArgs e)
        {
            if (cbxDeviceList.SelectedItem != null)
            {
                string device = cbxDeviceList.SelectedItem.ToString();
                if (BluetoothHandler.selectDevice(device))
                {
                    getData();
                    initialiseMap();
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
                lblConStatus.Foreground = new SolidColorBrush(Colors.Red);
                btnDisconnect.IsEnabled = false;
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            BluetoothHandler.disconnect();
            cbxDeviceList.Text = "-- Select Device --";
            lblConStatus.Content = "Disconnected";
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

    }
}
