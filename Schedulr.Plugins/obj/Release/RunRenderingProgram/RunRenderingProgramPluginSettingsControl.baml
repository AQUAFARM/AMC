   M S B A M L   `   `   `  ����   Schedulr.PluginsS    MSchedulr.Plugins.RunRenderingProgram.RunRenderingProgramPluginSettingsControl   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .���   � RowDefinitions  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $
��Auto ��� $��* �.+��� $��0��$��0��$��Program��$��0,0,5,0q�$��Center=�� $��0��$��1�� �� $��* ��� $
��Auto �.+��� $��0������  �� Path$ FileName��� -    ��Name$ browseFileNameButton��$��1��$	��...��$��5,0,0,0q�   MediumButton#���   �� $��1��$��0��$��Working folder��$��0,5,5,0q�$��Center=�� $��1��$��1��$��0,5,0,0q� �� $��* ��� $
��Auto �.+��� $��0������ $ WorkingDirectory��� -   $" browseWorkingDirectoryButton��$��1��$	��...��$��5,0,0,0q�#���   �� $��2��$��0��$��	Arguments��$��0,5,5,0q�$��Center=�� $��2��$��1��$��0,5,0,0q� �� $��* ��� $
��Auto �.+��� $��0��$��0������ $ 	Arguments��� -   $! editArgumentsTemplateButton��$��0��$��1��$	��...��$��5,0,0,0q�#���   �� -    ��Name$ argumentsInfoTextBlock��$��3��$��1��$��0,5,0,0q�  UserMessageFontBrush#���  $
��Wrap=��� $��4��$��0��$��Run on��$��0,5,5,0q�$��Center=�� $��4��$��1��$��0,5,0,0q�.+���  d� 	IsChecked �� $ RunOnPictures�.���� $��Pictures���� $��5,0,0,0q� �� $ RunOnVideos�.���� $��Videos���� $��5��$��0��$��File extensions��$��0,5,5,0q�$��Center=��� $��5��$��1��$��0,5,0,0q� �� Orientation$ 
Horizontal=�.+��� $��40��$��0,0,5,0q����� $ InputFileExtension��� $"��for the temporary input file��$��Center=��� $��6��$��1��$��0,5,0,0q�$ 
Horizontal=�.+��� $��40��$��0,0,5,0q����� $ OutputFileExtension��� $#��for the temporary output file��$��Center=��� $��7��$��1��$��0,5,0,0q�#���  $
��Wrap=�$����If the program you are using depends on certain file extensions, you can configure these here (if not, you can leave them empty to use the original file extension).��