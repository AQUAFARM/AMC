   M S B A M L   `   `   `  ����  Schedulr1    +Schedulr.Views.Dialogs.AuthenticationDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Authenticate with Flickr��5   	   $	��600��5   	   $	��800��6   $	��450��6"   $	��600��62    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ CanResizeWithGrip=�5   	   .��5      � $��10q�6    � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   .+�5      �� $��14(�6   omBefore you continue, you must authenticate with Flickr and authorize this application to access your account.6�   5      �� 5   
   $��1��6      BorderStyle#���   6    �� Padding$ 5q�6D   $��0,5,0,5q�6P   ._�5      (  "System.Windows.Controls.WebBrowser  -   5          Name$ authenticationWebBrowser��5      5      � 5   
   $��2��6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5      �� $��0��6   .+�5      �� $��Center=�6   �� 68   -   5         ��Name$ authenticationHyperlink��5   B   .�6�   ><Alternatively, launch a separate web browser to authenticate6�   6�   5      �� 5      $��1��6    �� Orientation$ 
Horizontal=�6)   .+�5      �� $��Verification code:��6   $
��Left=�65   $	��Top=�6P   
���   �6h   �� 5      -   5        	 ��Name$	 VerifierTextBox��5      $
��Left=�63   $	��100��6N   �� 5      -   5        
 ��Name$
 okButton��5   7   $��OK��6   $��0,0,5,0q�6&   6I   �� 5       -   5        $
 cancelButton��5    *   $��Cancel��6   6@   5!      5"      5#      5$      