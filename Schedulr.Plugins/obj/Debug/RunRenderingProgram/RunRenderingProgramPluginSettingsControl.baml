   M S B A M L   `   `   `  ����  Schedulr.PluginsS    MSchedulr.Plugins.RunRenderingProgram.RunRenderingProgramPluginSettingsControl   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � RowDefinitions  5	   
   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6   $��Program��6-   $��0,0,5,0q�6?   $��Center=�6P   � 5   
   $��0��6   $��1��6    5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5      �� $��0��6   ��6&   ��  �� Path$ FileName��� 5      -   5         ��Name$ browseFileNameButton��5   4   $��1��6   $	��...��6&   $��5,0,0,0q�6u      MediumButton#���   6�   5       �� 5"   
   $��1��6   $��0��6   $��Working folder��6-   $��0,5,5,0q�6F   $��Center=�6W   � 5#   
   $��1��6   $��1��6   $��0,5,0,0q�6,    5$      �� 5%      $��* �6#   �� 5&      $
��Auto �6#   .+�5(      �� $��0��6   ��6&   �� $ WorkingDirectory��� 5)      -   5        $" browseWorkingDirectoryButton��5)   4   $��1��6   $	��...��6&   $��5,0,0,0q�6�   #���   6�   5*      �� 5,   
   $��2��6   $��0��6   $��	Arguments��6-   $��0,5,5,0q�6A   $��Center=�6R   � 5-   
   $��2��6   $��1��6   $��0,5,0,0q�6,    5.      �� 5/      $��* �6#   �� 50      $
��Auto �6#   .+�52      �� $��0��6   $��0��6#   ��63   �� $ 	Arguments��� 53      -   5        $! editArgumentsTemplateButton��53   A   $��0��6   $��1��6"   $	��...��63   $��5,0,0,0q�6�   #���   6�   54      �� 56   
   -   5         ��Name$ argumentsInfoTextBlock��56   v   $��3��6   $��1��6!   $��0,5,0,0q�61     UserMessageFontBrush#���  6B   $
��Wrap=�6�   �� 58   
   $��4��6   $��0��6   $��Run on��6-   $��0,5,5,0q�6>   $��Center=�6O   � 59   
   $��4��6   $��1��6!   $��0,5,0,0q�61   .+�5:      ��  d� 	IsChecked 6   �� $ RunOnPictures�.��6A   �� $��Pictures��6G   6^   �� 5;      $��5,0,0,0q�6>    6   �� $ RunOnVideos�.��6P   �� $��Videos��6V   6k   5<      �� 5>   
   $��5��6   $��0��6   $��File extensions��6-   $��0,5,5,0q�6G   $��Center=�6X   �� 5?   
   $��5��6   $��1��6"   $��0,5,0,0q�62    �� Orientation$ 
Horizontal=�6C   .+�5@      �� $��40��6?   $��0,0,5,0q�6M   ��6   �� $ InputFileExtension�6M   �� 5A      $"��for the temporary input file��6   $��Center=�6;   5B      �� 5D   
   $��6��6   $��1��6"   $��0,5,0,0q�62   $ 
Horizontal=�6C   .+�5E      �� $��40��6@   $��0,0,5,0q�6N   ��6   �� $ OutputFileExtension�6N   �� 5F      $#��for the temporary output file��6   $��Center=�6<   5G      �� 5I   
   $��7��6   $��1��6!   $��0,5,0,0q�61   #���  6B   $
��Wrap=�6v   $����If the program you are using depends on certain file extensions, you can configure these here (if not, you can leave them empty to use the original file extension).��6�   5J      5K      