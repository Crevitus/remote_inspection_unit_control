   M S B A M L   `   `   `  ���� T  PMahApps.Metro, Version=1.1.2.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  " remote_inspection_unit_control/   )remote_inspection_unit_control.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     -   (   "MahApps.Metro.Controls.MetroWindow   Name$  
mainWindow�� � Title$ Remote Inspection Unit��$	��700��$	��900��$	��700��$	��900�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ CanResizeWithGrip=�   AccentColorBrush   	GlowBrush# �   ���	#FF242628   RightWindowCommands +   %MahApps.Metro.Controls.WindowCommands  .@���  �� Orientation$ 
Horizontal=�$��Stretch=�$
��auto��$
��auto��$��Right=�$��1��.+��� -    ��Name$ btnDisconnect��$��
Disconnect��$��Right=�$��Stretch=�$��95��{�.   � Cursor$
 Hand���� -   	 ��Name$	 lblConStatus��$��	10,0,10,0q�$��Disconnected��$
��Left=�$��Stretch=�$��Segoe UI)�$��12(�
���   ��� -   
 ��Name$
 cbxDeviceList�� �� 
IsEditable . |�.   �� 
IsReadOnly .  �� Text$ -- No Devices --��$��Segoe UI)�$��14(�$	��195��$��	0,0,0,0.4q�$��Center=�$��Center=����	#FFF1EEEE��Z� �� -   $ 	btnSearch��$��Search��$
��Left=�$��Stretch=�$��75��$��4,0,0,0q�$
 Hand��.��� -    �Name$ 
layoutRoot��|�.  � RowDefinitions �� $��8* ��� $	��51* ��� $��8* ��� $	��34* � � ColumnDefinitions �� $	��50* ��� $	��50* �.+��� $��Camera View��  
basicLabel#��[ $��Center=�$��0��$��Center=�$
��auto��$
��auto���� $��Map View��#��[ $��Center=�$��0��$��Center=�$
��auto��$
��auto��$��1��� -   $ gdMapWrapper��$��1�����	#FF2F5A7F$��1��$��Stretch=�$��Stretch=�$
��auto��$
��auto��$��4,0,8,0q�.+��� -    ��Name$	 map��$��Stretch=�$
��auto��$��Stretch=�$
��auto���� -	   $ btnMapFullScreen��  basicButton#��[ $��Full Screen��$��75��$��Right=�$��Bottom=� � UseLayoutRounding .  $��0,0,2,1q�� -
   $ gdMediaWrapper��$��1��$��0��$��Stretch=�$��Stretch=�$
��auto��$
��auto��$��8,0,4,0q�.+�r� -    r�Name$ 	mdePlayer��$��1��$��Stretch=�$��Stretch=�$
��auto��$
��auto���� -    ��Name$ cnvsDeleteMe��$��Stretch=�$��Stretch=�$
��auto��$
��auto�����	#FF313C44 .  �� -   $ btnMediaFullScreen��#��[ $��Right=�$��Bottom=�$��Full Screen��$��75��$��0,0,2,1q��� $��
System Log��#��[ $��1��$��Center=�$��2��$��Center=��� $��Capture��#��[ $��2��$��0��$
��Left=�$	��Top=�$��75��$��22��$��10,7,0,0q��� -    ��Name$ 
lstLogList��$��Stretch=�$��Stretch=�$
��auto��$
��auto��$��3�����	#FF5C6D7E$��1��$��2��$��4,0,8,15q�� $��Stretch=�$��Stretch=�$
��auto��$
��auto��$��0��$��3��.+��� -   $ btnUp��$
��Left=�$��
108,10,0,0q�$	��Top=�$��75��  controlButton#���  ���	#00531717.���� .+��� $��icon/uparrow.png��  controlImage#���   J� RenderTransformOrigin$ 0.5,0.5&�z�V� .~��� �� ��  �� Angle$	 -90Q�T� �� -   $ btnDown��$
��Left=�$��108,114,0,0q�$	��Top=�$��75��#���  ���	#00531717.���� .+��� $��icon/uparrow.png��#���  $ 0.5,0.5&�$��0q�z�V� .~���  �� ScaleY$ 0.99999999999999989Q� �� ScaleX$ 0.99999999999999989Q��� �� $ 90Q�T�  T� Y$ 5Q��� -   $ btnLeft��$
��Left=�$��	50,59,0,0q�$	��Top=�$��75��#���  ���	#00531717.���� .+��� $��icon/uparrow.png��#���  $ 0.5,0.5&�$��0q�z�V� .~��� $ 0.99999999999999989Q�$ 0.99999999999999989Q��� �� $	 180Q�T� $ 5Q��� -   $ btnRight��$
��Left=�$��
167,59,0,0q�$	��Top=�$��75��#���  ���	#00531717.���� $ 0.5,0.5&�.+��� $��icon/uparrow.png��#���  $ 0.5,0.5&�$��0q�z�V� .~��� $ 0.99999999999999989Q�$ 0.99999999999999989Q��� �� $
 -360Q�T� $ 5Q��� -   $ btnNorth��$��North��#��[ $��Right=�$��0,42,111.4,0q�$	��Top=�$��75��$��22���� -   $ btnEast��$
��East��#��[ $��Right=�$��0,112,111.4,0q�$	��Top=�$��75��$��22���� -   $ btnSouth��$��South��#��[ $��Right=�$��0,75,111.4,0q�$	��Top=�$��75��$��22���� -   $ btnWest��$
��West��#��[ $��Right=�$��0,149,111.4,0q�$	��Top=�$��75��$��22��$ -0.587,5.291&��� -   $ 	btnManual��$��Manual��#��[ $��Right=�$��0,75,20.4,0q�$	��Top=�$��75��$��22���� -   $ 	btnZoomIn��$��Zoom In��#��[ $��Right=�$��0,112,20.4,0q�$	��Top=�$��75��$��22���� -   $ 
btnZoomOut��$��Zoom Out��#��[ $��Right=�$��0,149,20.4,0q�$	��Top=�$��75��$��22��