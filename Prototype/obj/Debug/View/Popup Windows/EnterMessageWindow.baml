   M S B A M L   `   `   `  ����  	Prototype5    /Prototype.View.Popup_Windows.EnterMessageWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  EnterMessageWindow��5   	   $	��195��6$   $	��594��61    � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5   
   �� 5      �� 5	      $��30 �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $
��auto �6#   �� 5      $
��auto �6#   �� 5      $	��100 �6#   �� 5       5      �� 5      $
��auto �6    �� 5      $��* �6    .+�5      �� $��0��6   $��0��6$   $
��Left=�61   $	��Top=�6L   $��10q�6d   $��	Resource:��6p   �� 5      -   5        $��ComboBoxResources��5   s   $��1��6   $��0��6'   $
��Left=�64   $	��Top=�6O   $��10q�6g    �� SelectedIndex$ 0��6�   �� 5      $��0��6   $��1��6$   $
��Left=�61   $	��Top=�6L   $��10q�6d   $��Type:��6p   �� 5      -   5        $��ComboBoxMessageType��5   s   $��1��6   $��1��6'   $
��Left=�64   $	��Top=�6O   $��10q�6g   $ 1��6�   �� 5      -   5        $��textBoxMessage��5)      $��0��5      $��2��5      $��2��5       $��2��5!      $��Stretch=�5"      $��Stretch=�5#      $��10q�5$       �� TextWrapping$
 Wrap=�5%       �� AcceptsReturn . $
��Auto=�5'      $
��Auto=�5(      5)      5*      �� 5,   
   -   5        $��1��5,      $��Right=�6   $��Center=�6:   $��OK��6U   $��75��6b   $
��10,0q�6m   6{   5.      5/      