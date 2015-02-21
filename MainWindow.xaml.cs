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
        private bool _fullScreen = false;
        private bool _isMax = false;
        private Compass _orientation = Compass.North;
        private Point currentPos;
        private Line line, line2;
        private Canvas horizontalLine, verticalLine, rightBottom, leftBottom, rightTop, leftTop;
        private Dictionary<Point, drawType> position = new Dictionary<Point, drawType>{};
        private int expHeight;
        public MainWindow()
        {
            InitializeComponent();
			this.MouseLeftButtonDown += delegate { this.DragMove(); };
			this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            getDevices();
            //for testing
            MessageBox.Show("Press set up map before using to avoid crash", "Map");     
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
            int x, y;
            x = (int) gdMap.ActualWidth;
            y = (int) gdMap.ActualHeight;


            decimal rows, col;

            col = x / 30;
            col = Math.Round(col, 0);
            rows = y / 30;
            rows = Math.Round(rows, 0);

            for (int i = 0; i < col; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Name = "col" + i.ToString();
                gridCol.Width = new GridLength(30);
                gdMapInner.ColumnDefinitions.Add(gridCol);
            }

            for(int i = 0; i <  rows; i++)
            {

                RowDefinition gridrow = new RowDefinition();
                gridrow.Name = "row" + i.ToString();
                gridrow.Height = new GridLength(30);
                gdMapInner.RowDefinitions.Add(gridrow);
            }

            currentPos.X = (int) 0;
            currentPos.Y = (int) rows ;
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
            redBrush.Color = Colors.Red;
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
                           gdMapInner.Children.Add(verticalLine);
                            break;
                        case Compass.East:
                           refreshRightBottom();
                           currentPos.X = currentPos.X + 1;
                           rightBottom.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                           rightBottom.SetValue(Grid.RowProperty, (int)currentPos.Y);
                           gdMapInner.Children.Add(rightBottom);
                            break;
                        case Compass.South:
                           refreshVerticalLine();
                           currentPos.Y = currentPos.Y - 1;
                           verticalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                           verticalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                           gdMapInner.Children.Add(verticalLine);
                            break;
                        case Compass.West:
                           refreshLeftBottom();
                           currentPos.X = currentPos.X - 1;
                           leftBottom.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                           leftBottom.SetValue(Grid.RowProperty, (int)currentPos.Y);
                           gdMapInner.Children.Add(leftBottom);
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
                            gdMapInner.Children.Add(leftTop);
                            break;
                        case Compass.East:
                            refreshHorizontalLine();
                            currentPos.X = currentPos.X + 1;
                            horizontalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            horizontalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(horizontalLine);
                            break;
                        case Compass.South:
                            refreshLeftBottom();
                            currentPos.Y = currentPos.Y + 1;
                            leftBottom.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            leftBottom.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(leftBottom);
                            break;
                        case Compass.West:
                            refreshHorizontalLine();
                            currentPos.X = currentPos.X + 1;
                            horizontalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            horizontalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(horizontalLine);
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
                            gdMapInner.Children.Add(verticalLine);
                            break;
                        case Compass.East:
                            refreshRightTop();
                            currentPos.X = currentPos.X + 1;
                            rightTop.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            rightTop.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(rightTop);
                            break;
                        case Compass.South:
                            refreshVerticalLine();
                            currentPos.Y = currentPos.Y + 1;
                            verticalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            verticalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(verticalLine);
                            break;
                        case Compass.West:
                            refreshLeftTop();
                            currentPos.X = currentPos.X - 1;
                            leftTop.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            leftTop.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(leftTop);
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
                            gdMapInner.Children.Add(rightTop);
                            break;
                        case Compass.East:
                            refreshHorizontalLine();
                            currentPos.X = currentPos.X - 1;
                            horizontalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            horizontalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(horizontalLine);
                            break;
                        case Compass.South:
                            refreshRightBottom();
                            currentPos.Y = currentPos.Y + 1;
                            rightBottom.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            rightBottom.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(rightBottom);
                            break;
                        case Compass.West:
                            refreshHorizontalLine();
                            currentPos.X = currentPos.X - 1;
                            horizontalLine.SetValue(Grid.ColumnProperty, (int)currentPos.X);
                            horizontalLine.SetValue(Grid.RowProperty, (int)currentPos.Y);
                            gdMapInner.Children.Add(horizontalLine);
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

        private void btnExitClick(object sender, System.Windows.RoutedEventArgs e)
        {
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
            send("0150");
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            send("1150");
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            send("2150");
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            send("3150");
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
            lblConStatus.Foreground = new SolidColorBrush(Colors.Red);
        }

        private async void getData()
        {
            string data;
            if (BluetoothHandler.Connected)
            {
                while (true)
                {
                    data = await BluetoothHandler.receive();
                    lstLogList.Items.Add(data);
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            getDevices();
        }
    }
}
