   M S B A M L   `   `   `  ����  Schedulr.Plugins=    7Schedulr.Plugins.SaveCopy.SaveCopyPluginSettingsControl   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � RowDefinitions  5	   
   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6   $��Destination folder��6-   $��Center=�6J   $��0,0,5,0q�6e   � 5   
   $��0��6   $��1��6    5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5      �� $��0��6   ��6&   ��  �� Path$ DestinationFolder� �� Mode$ TwoWay=��� 5      -   5         ��Name$# browseDestinationFolderButton��5   3   $��1��6   $	��...��6%   $��5,0,0,0q�6�      MediumButton#���   6�   5      �� 5    
   $��1��6   $��0��6   $��Run on��6-   $��0,5,5,0q�6>   $��Center=�6O   � 5!   
   $��1��6   $��1��6!   $��0,5,0,0q�61   .+�5"      ��  d� 	IsChecked 6   �� $ RunOnPictures�.��5#      �� $��Pictures��6   5$      �� 5%      $��5,0,0,0q�6>    6   �� $ RunOnVideos�.��5&      �� $��Videos��6   5'      5(      �� 5*   
   $��2��6   $��0��6   $��File extensions��6-   $��0,5,5,0q�6G   $��Center=�6X   �� 5+   
   $��2��6   $��1��6"   $��0,5,0,0q�62    �� Orientation$ 
Horizontal=�6C   .+�5,      �� $��40��6A   $��0,0,5,0q�6O   ��6   �� $ PictureFileExtension�6O   �� 5-      $��for pictures��6   $��Center=�6+   5.      �� 50   
   $��3��6   $��1��6"   $��0,5,0,0q�62   $ 
Horizontal=�6C   .+�51      �� $��40��6?   $��0,0,5,0q�6M   ��6   �� $ VideoFileExtension�6M   �� 52      $��
for videos��6   $��Center=�6)   53      �� 55   
   $��4��6   $��1��6!   $��0,5,0,0q�61     UserMessageFontBrush#���  6B   $
��Wrap=�6v   $����If any previous manipulations change the file type (e.g. most of them output pictures as JPEG), you can set the output file extension here (if not, you can leave them empty to use the original file extension).��6�   56      57      