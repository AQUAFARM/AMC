   M S B A M L   `   `   `  ����  Schedulr.PluginsA    ;Schedulr.Plugins.RunProgram.RunProgramPluginSettingsControl   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � RowDefinitions  5	   
   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6   $��Program��6-   $��0,0,5,0q�6?   $��Center=�6P   � 5   
   $��0��6   $��1��6    5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5      �� $��0��6   ��6&   ��  �� Path$ FileName��� 5      -   5         ��Name$ browseFileNameButton��5   4   $��1��6   $	��...��6&   $��5,0,0,0q�6u      MediumButton#���   6�   5      �� 5   
   $��1��6   $��0��6   $��Working folder��6-   $��0,5,5,0q�6F   $��Center=�6W   � 5   
   $��1��6   $��1��6   $��0,5,0,0q�6,    5       �� 5!      $��* �6#   �� 5"      $
��Auto �6#   .+�5$      �� $��0��6   ��6&   �� $ WorkingDirectory��� 5%      -   5        $" browseWorkingDirectoryButton��5%   4   $��1��6   $	��...��6&   $��5,0,0,0q�6�   #���   6�   5&      �� 5(   
   $��2��6   $��0��6   $��	Arguments��6-   $��0,5,5,0q�6A   $��Center=�6R   � 5)   
   $��2��6   $��1��6   $��0,5,0,0q�6,    5*      �� 5+      $��* �6#   �� 5,      $
��Auto �6#   .+�5.      �� $��0��6   ��6&   �� $ 	Arguments��� 5/      -   5        $! editArgumentsTemplateButton��5/   4   $��1��6   $	��...��6&   $��5,0,0,0q�6�   #���   6�   50      � 52   
    � Orientation$ 
Horizontal=�6   $��3��6-   $��2��6:   $��0,5,0,0q�6N   .+�53      �� $��Center=�6    d� 	IsChecked 62   �� $ WaitForExit�.��6Z   �� $)��#Wait for the program to exit within��6`   6�   �� 54      $��40��6F   $��5,0,0,0q�6T   ��6   �� $ WaitForExitTimeoutSeconds�6T   �� 55      $)��#seconds (or 0 to wait indefinitely)��6   $��Center=�6C   $��5,0,0,0q�6^   56      57      58      