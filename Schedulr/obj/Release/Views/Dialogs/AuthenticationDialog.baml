   M S B A M L   `   `   `  ����   Schedulr1    +Schedulr.Views.Dialogs.AuthenticationDialog   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Authenticate with Flickr��$	��600��$	��800��$	��450��$	��600�� � WindowStartupLocation$ CenterOwner=� � 
ResizeMode$ CanResizeWithGrip=�.��� $��10q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��.+��� $��14(�omBefore you continue, you must authenticate with Flickr and authorize this application to access your account.�� $��1��   BorderStyle#���    �� Padding$ 5q�$��0,5,0,5q�._�(  "System.Windows.Controls.WebBrowser  -     Name$ authenticationWebBrowser��� $��2�� � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $��0��.+��� $��Center=��� -    ��Name$ authenticationHyperlink��.�><Alternatively, launch a separate web browser to authenticate�� $��1�� �� Orientation$ 
Horizontal=�.+��� $��Verification code:��$
��Left=�$	��Top=�
���   ��� -   	 ��Name$	 VerifierTextBox��$
��Left=�$	��100���� -   
 ��Name$
 okButton��$��OK��$��0,0,5,0q��� -   $
 cancelButton��$��Cancel��