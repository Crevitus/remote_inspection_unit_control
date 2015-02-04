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
			// Insert code required on object creation below this point.
		}
		
		 private void btnExitClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	Window.Close();
        }
	}
}