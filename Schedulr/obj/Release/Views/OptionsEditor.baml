   M S B A M L   `   `   `  ���� V  RSchedulr, Version=3.2.5290.19321, Culture=neutral, PublicKeyToken=7c0c6187eb889c22A%clr-namespace:Schedulr.Views.ControlsSchedulr.Views.Controls  V RSchedulr.Common, Version=2.0.1.0, Culture=neutral, PublicKeyToken=7c0c6187eb889c22J6clr-namespace:Schedulr.Common;assembly=Schedulr.CommonSchedulr.Common "    Schedulr.Views.OptionsEditor   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5svc%clr-namespace:Schedulr.Views.Controls     Z VJelleDruyts.Windows, Version=1.1.0.0, Culture=neutral, PublicKeyToken=7c0c6187eb889c22>w6http://schemas.jelle.druyts.net/2010/xaml/presentation  Ccommon6clr-namespace:Schedulr.Common;assembly=Schedulr.Common    � DataContext  ��  �� Path$ OptionsViewModel�   Locator �� Source# [  .����   BorderStyle#���    DiagramGradientBrush#���   �� Padding$ 5q�._��  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* �.+��� $��0��$��Display Settings��$��0,0,0,5q�$��5q�.���� .+�� $��0,5,0,0q� � ColumnDefinitions �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$ ��Display mode for the queue��$��0,0,5,0q�$��Center=��� $��0��$��1��$��Center=�$��Center=�.@��� $��Vertical�� �� 
IsSelected �� $- 'AccountSettings.PictureQueueDisplayMode� �� Mode$ TwoWay=�  enumConverter �� 	Converter# [ 	 �� ConverterParameter$	 Vertical���� $
��Flow�� �� $- 'AccountSettings.PictureQueueDisplayMode�$ TwoWay=�# [ $	 HorizontalFlow���� $��1��$��0��$$��Display mode for file previews��$��0,0,5,0q�$��Center=��� $��1��$��1��$��Center=�$��Center=�.@��� $��
Thumbnails�� �� $/ )AccountSettings.PicturePreviewDisplayMode�$ TwoWay=�# [ $	 	Thumbnail���� $��	Text Only�� �� $/ )AccountSettings.PicturePreviewDisplayMode�$ TwoWay=�# [ $	 TextOnly���� $��2��$��0��$-��'Display mode for Flickr Sets and Groups��$��0,0,5,0q�$��Center=��� $��2��$��1��$��Center=�$��Center=�.@��� $��Text And Icon�� �� $2 ,AccountSettings.PictureCollectionDisplayMode�$ TwoWay=�# [ $	 TextAndIcon���� $��	Text Only�� �� $2 ,AccountSettings.PictureCollectionDisplayMode�$ TwoWay=�# [ $	 TextOnly���� $��Upload Settings��$
��Left=�$��1��$	��Top=�$��0,0,0,5q�$��5q�.���� .+�� $��0,5,0,0q� �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$(��"Number of upload attempts per file��$��0,0,5,0q�$��Center=��� $��0��$��1��$��18��$
��Left=�$��40��$	��Top=�.������ $) #AccountSettings.UploadRetryAttempts�
 �� StringFormat$
 {0:##}�� �� ValidationRules )  #Schedulr.Common.ShortValidationRule  
   Min$ 1��
   Max$ 20��   	FieldName$) #AccountSettings.UploadRetryAttempts��   CustomMessage$ Must be between 1 and 20!��.���� $��2��$"��Default values for new files��.��,   &Schedulr.Views.Controls.PictureDetails    �� *PictureDefaults+