﻿using System;
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
        public MainWindow()
        {
            InitializeComponent();
			this.MouseLeftButtonDown += delegate { this.DragMove(); };
			this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnExitClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	Application.Current.Shutdown();
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
				Maximize.Content = "2";
				Maximize.ToolTip = "Restore";
			}
			else
			{
				WindowState = WindowState.Normal;
				Maximize.Content = "1";
				Maximize.ToolTip = "Maximize";
			}
			
        }

        private void devices_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        } 
    }
}
