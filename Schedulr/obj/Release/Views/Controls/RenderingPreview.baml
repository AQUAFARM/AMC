   M S B A M L   `   `   `  ����   Schedulr.    (Schedulr.Views.Controls.RenderingPreview   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .���   � RowDefinitions  �� $
��Auto ��� $��* ��� $
��Auto �.+�� $��0��$��0,5,0,0q� � ColumnDefinitions �� $��* ��� $��* ��� $
��Auto ��� $��* ��� $��* �.+��� -    ��Name$ previousButton��$��1��$��Right=�   SmallButton#��[  .���� $��/Resources/MoveLeft.png��$��12��$��12��$��Uniform=��� $��2�� �� Orientation$ 
Horizontal=�$��5,0,5,0q�.+��� -    ��Name$ currentIndexTextBox��$��40���� -    ��Name$ totalCountTextBlock��$��Center=�$��of ...��$��5,0,0,0q��� -   $ 
nextButton��$��3��$
��Left=�#��[  .���� $��/Resources/MoveRight.png��$��12��$��12��$��Uniform=��� $��4��$ 
Horizontal=�$��Right=�.+��� -   $ refreshButton�� � ToolTip$ Refresh the preview��#��[  .���� $��/Resources/Refresh.png���� -   $ 
saveButton��$&  Save a copy of the rendered file��#��[  .���� $��/Resources/Save.png��� $��1�� �� $
��Auto ��� $��* �.+�� $��0��  �� $��* ��� $
��Auto �.+��� -    ��Name$ 
zoomSlider��$��0�� �� Orientation$ Vertical=�$��0,0,5,0q�	 � Minimum$	 0Q�
 � Maximum$
 4Q� � Value$
 0.25Q� � LargeChange$
 0.25Q� � SmallChange$
 0.01Q� �� TickPlacement$ BottomRight=� �� TickFrequency$
 0.25Q�  SmallSlider#��[ �� $��1��$��10(�  InfoFontBrush#��[ $��0,5,0,0q�����  �� ElementName$ 
zoomSlider�� �� Path$ Value� �� StringFormat$ p0���� $��1��  BorderStyle#���   �� Padding$ 5q�$��0,5,0,5q�._�� .+��   �� $��* ��� $
��Auto �.+��� $��0��$
��Auto=�$
��Auto=�.��� .+�� -    �Name$ previewImageContainer��$y�Hidden=�.+��� -	    ��Name$ previewImage��$
��None=� � LayoutTransform V� .~��� -
   dpiScaleTransform����  �� ScaleX �� $ Value�$ 
zoomSlider�� �� ScaleY �� $ Value�$ 
zoomSlider��� -   $ previewVideoContainer��$y�	Collapsed=�.+�r� -    r�Name$ previewVideo��$��0�� r� IsMuted .   r� LoadedBehavior$ Manual=� r� Volume �� $ volumeSlider��$ Value� ��  �� $ Value�$ 
zoomSlider�� �� $ Value�$ 
zoomSlider���� -    ��Name$ mediaInfoContainer��$��1��$��Center=�$ Vertical=�$��0,5,0,5q�.+��� -   $ mediaInfoTextBlock��$��Center=�$��Center=�#��[ $
��Wrap=� �� TextAlignment$ Center=��� #���  $��5,5,0,0q�$ 5q�y��� $ previewVideoContainer��$ 
Visibility�._��� $ 
Horizontal=�.+��� -   $ videoPlayButton��$��0,0,5,0q�#��[  $
 Play��.���� $��/Resources/MediaPlay.png���� -   $ videoPauseButton��$��0,0,5,0q�#��[  $ Pause��.���� $��/Resources/MediaPause.png���� -   $ videoStopButton��$��0,0,5,0q�#��[  $
 Stop��.���� $��/Resources/MediaStop.png���� -   $ positionSlider��$��0,0,5,0q�$	��200��$	 0Q�#��[ �� -   $ positionTextBlock��$��Center=�$��Center=�#��[ $��0,0,5,0q��� $ ��/Resources/MediaVolume.png���� -   $ volumeSlider��$��50��$	 0Q�$
 1Q�$	 0.5Q�#��[ �� -   $ messageTextBlock��$��Center=�$��Center=�#��[ $
��Wrap=�$ Center=�