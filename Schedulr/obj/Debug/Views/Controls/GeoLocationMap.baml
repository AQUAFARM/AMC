   M S B A M L   `   `   `  ����c  _GMap.NET.WindowsPresentation, Version=1.7.0.0, Culture=neutral, PublicKeyToken=b85b9027b614afefqPclr-namespace:GMap.NET.WindowsPresentation;assembly=GMap.NET.WindowsPresentationGMap.NET.WindowsPresentation  5       Schedulr,   &Schedulr.Views.Controls.GeoLocationMap   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ^mPclr-namespace:GMap.NET.WindowsPresentation;assembly=GMap.NET.WindowsPresentation     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ mapTypesComboBox��5       $��0��6    �� DisplayMemberPath$
 Name��6:   $��0,0,0,5q�6S   � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   .+�5      � -   5         �Name$ zoomStackPanel��5   5   $��0��6   $��0,0,10,0q�6#   .+�5      ��  �� Orientation$ Vertical=�6    �� TickPlacement$
 Both=�60    � SmallChange$ 1Q�6E    � LargeChange$ 2Q�6U   	 � Minimum	 6e   �� 
 �� ElementName$
 geoGMapControl�� �� Path$ MinZoom� �� Mode$ TwoWay=� � Maximum 6�   �� $
 geoGMapControl��$ MaxZoom�$ TwoWay=� � Value 6�   �� $
 geoGMapControl��$
 Zoom�$ TwoWay=�5      .   (GMap.NET.WindowsPresentation.GMapControl  5      -   5          Name$ geoGMapControl��5   S   $��1��6   
�������$��1q�6?   6S   5      5      5      