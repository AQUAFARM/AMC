   M S B A M L   `   `   `  ����  Schedulr.    (Schedulr.Views.Controls.RenderingPreview   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � RowDefinitions  5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��0��6   $��0,5,0,0q�6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $
��Auto �6#   �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� -   5         ��Name$ previousButton��5   A   $��1��6   $��Right=�6%      SmallButton#��[  6v   .��5      �� $��/Resources/MoveLeft.png��6   $��12��69   $��12��6E   $��Uniform=�6P   5      �� 5      $��2��6    �� Orientation$ 
Horizontal=�6)   $��5,0,5,0q�6B   .+�5      �� -   5         ��Name$ currentIndexTextBox��5   (   $��40��6   6E   �� 5      -   5         ��Name$ totalCountTextBlock��5   V   $��Center=�6   $��of ...��67   $��5,0,0,0q�6E   6V   5      �� 5      -   5        $ 
nextButton��5   @   $��3��6   $
��Left=�6%   #��[  6m   .��5      �� $��/Resources/MoveRight.png��6   $��12��6:   $��12��6F   $��Uniform=�6Q   5      �� 5       $��4��6   $ 
Horizontal=�6)   $��Right=�6B   .+�5!      �� -   5        $ refreshButton��5!       � ToolTip$ Refresh the preview��60   #��[  6j   .��5"      �� $��/Resources/Refresh.png��6   5#      �� 5$      -   5        $ 
saveButton��5$      $&  Save a copy of the rendered file��6-   #��[  6q   .��5%      �� $��/Resources/Save.png��6   5&      5'      5(      � 5)   
   $��1��6    5*      �� 5+      $
��Auto �6#   �� 5,      $��* �6#   .+�5.      � $��0��6     5/      �� 50      $��* �6$   �� 51      $
��Auto �6$   .+�53      �� -   5         ��Name$ 
zoomSlider��53   �   $��0��6    �� Orientation$ Vertical=�6&   $��0,0,5,0q�6=   	 � Minimum$	 0Q�6N   
 � Maximum$
 4Q�6Z    � Value$
 0.25Q�6f    � LargeChange$
 0.25Q�6s    � SmallChange$
 0.01Q�6�    �� TickPlacement$ BottomRight=�6�    �� TickFrequency$
 0.25Q�6�     SmallSlider#��[ 6�   �� 54      $��1��6   $��10(�6n     InfoFontBrush#��[ 6|   $��0,5,0,0q�6�   ��6)   ��  �� ElementName$ 
zoomSlider�� �� Path$ Value� �� StringFormat$ p0��6�   55      �� 56      $��1��6     BorderStyle#���  6%    �� Padding$ 5q�6K   $��0,5,0,5q�6W   ._�57      � .+�58      �   59      �� 5:      $��* �6,   �� 5;      $
��Auto �6,   .+�5=      �� $��0��6'   $
��Auto=�64   $
��Auto=�6Y   .��5>      � .+�5?   "   � -   5         �Name$ previewImageContainer��5?   '   $y�Hidden=�6F   .+�5@   &   �� -	   5         ��Name$ previewImage��5@   ,   $
��None=�6B    � LayoutTransform 5A   *   V� 5B   .   .~�5C   2   �� -
   5        dpiScaleTransform��5C   A   �� 5D   2    �� ScaleX 6A   �� $ Value�$ 
zoomSlider�� �� ScaleY 6w   �� $ Value�$ 
zoomSlider��5E   /   5G   '   5H   #   � 5I   "   -   5        $ previewVideoContainer��5I   '   $y�	Collapsed=�6F   .+�5J   &   r� -   5         r�Name$ previewVideo��5J   @   $��0��63    r� IsMuted .   r� LoadedBehavior$ Manual=�6�    r� Volume 6f   �� $ volumeSlider��$ Value� 5K   *   �� 5L   .    6=   �� $ Value�$ 
zoomSlider�� 6s   �� $ Value�$ 
zoomSlider��5N   '   5O   #   5P      5Q      �� 5R      -   5         ��Name$ mediaInfoContainer��5R   w   $��1��6%   $��Center=�62   $ Vertical=�6O   $��0,5,0,5q�6f   .+�5S      �� -   5        $ mediaInfoTextBlock��5S   �   $��Center=�6(   $��Center=�6E   #��[ 6`   $
��Wrap=�6�    �� TextAlignment$ Center=�6�   6�   �� 5T      #���  6o   $��5,5,0,0q�6�   $ 5q�6�   y�6%   �� $ previewVideoContainer��$ 
Visibility�._�5U   "   �� $ 
Horizontal=�6-   .+�5V   &   �� -   5        $ videoPlayButton��5V   -   $��0,0,5,0q�6d   #��[  6u   $
 Play��6�   .��5W   *   �� $��/Resources/MediaPlay.png��60   5X   '   �� 5Y   &   -   5        $ videoPauseButton��5Y   -   $��0,0,5,0q�6f   #��[  6w   $ Pause��6�   .��5Z   *   �� $��/Resources/MediaPause.png��60   5[   '   �� 5\   &   -   5        $ videoStopButton��5\   -   $��0,0,5,0q�6d   #��[  6u   $
 Stop��6�   .��5]   *   �� $��/Resources/MediaStop.png��60   5^   '   �� 5_   &   -   5        $ positionSlider��5_   V   $��0,0,5,0q�6-   $	��200��6>   $	 0Q�6J   #��[ 6n   6�   �� 5`   &   -   5        $ positionTextBlock��5`   �   $��Center=�60   $��Center=�6M   #��[ 6h   $��0,0,5,0q�6�   �� 5a   &   $ ��/Resources/MediaVolume.png��6,   �� 5b   &   -   5        $ volumeSlider��5b   \   $��50��6-   $	 0Q�68   $
 1Q�6D   $	 0.5Q�6P   #��[ 6r   5c   #   5d      5e      5f      �� 5g      -   5        $ messageTextBlock��5g   �   $��Center=�6    $��Center=�6=   #��[ 6X   $
��Wrap=�6�   $ Center=�6�   6�   5h      5i      5j      5k      5l      