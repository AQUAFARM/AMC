   M S B A M L   `   `   `  ����   Schedulr.Plugins=    7Schedulr.Plugins.SaveCopy.SaveCopyPluginSettingsControl   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .���   � RowDefinitions  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $
��Auto ��� $��* �.+��� $��0��$��0��$��Destination folder��$��Center=�$��0,0,5,0q�� $��0��$��1�� �� $��* ��� $
��Auto �.+��� $��0������  �� Path$ DestinationFolder� �� Mode$ TwoWay=��� -    ��Name$# browseDestinationFolderButton��$��1��$	��...��$��5,0,0,0q�   MediumButton#���   �� $��1��$��0��$��Run on��$��0,5,5,0q�$��Center=�� $��1��$��1��$��0,5,0,0q�.+���  d� 	IsChecked �� $ RunOnPictures�.���� $��Pictures���� $��5,0,0,0q� �� $ RunOnVideos�.���� $��Videos���� $��2��$��0��$��File extensions��$��0,5,5,0q�$��Center=��� $��2��$��1��$��0,5,0,0q� �� Orientation$ 
Horizontal=�.+��� $��40��$��0,0,5,0q����� $ PictureFileExtension��� $��for pictures��$��Center=��� $��3��$��1��$��0,5,0,0q�$ 
Horizontal=�.+��� $��40��$��0,0,5,0q����� $ VideoFileExtension��� $��
for videos��$��Center=��� $��4��$��1��$��0,5,0,0q�  UserMessageFontBrush#���  $
��Wrap=�$����If any previous manipulations change the file type (e.g. most of them output pictures as JPEG), you can set the output file extension here (if not, you can leave them empty to use the original file extension).��