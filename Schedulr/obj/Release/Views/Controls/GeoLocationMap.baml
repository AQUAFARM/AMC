   M S B A M L   `   `   `  ���� c  _GMap.NET.WindowsPresentation, Version=1.7.0.0, Culture=neutral, PublicKeyToken=b85b9027b614afefqPclr-namespace:GMap.NET.WindowsPresentation;assembly=GMap.NET.WindowsPresentationGMap.NET.WindowsPresentation   Schedulr,   &Schedulr.Views.Controls.GeoLocationMap   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ^mPclr-namespace:GMap.NET.WindowsPresentation;assembly=GMap.NET.WindowsPresentation     .���   � RowDefinitions  �� $
��Auto ��� $��* �.+��� -    ��Name$ mapTypesComboBox��$��0�� �� DisplayMemberPath$
 Name��$��0,0,0,5q�� $��1�� � ColumnDefinitions �� $
��Auto ��� $��* �.+�� -    �Name$ zoomStackPanel��$��0��$��0,0,10,0q�.+���  �� Orientation$ Vertical=� �� TickPlacement$
 Both=� � SmallChange$ 1Q� � LargeChange$ 2Q�	 � Minimum	 �� 
 �� ElementName$
 geoGMapControl�� �� Path$ MinZoom� �� Mode$ TwoWay=� � Maximum �� $
 geoGMapControl��$ MaxZoom�$ TwoWay=� � Value �� $
 geoGMapControl��$
 Zoom�$ TwoWay=�.   (GMap.NET.WindowsPresentation.GMapControl  -     Name$ geoGMapControl��$��1��
�������$��1q�