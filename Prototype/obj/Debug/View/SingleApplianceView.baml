   M S B A M L   `   `   `  ����  	Prototype(    "Prototype.View.SingleApplianceView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $	��100 �6   �� 5      $��3 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��Resource Information��6    .��5      � $��Stretch=�6   $��Stretch=�61    � ColumnDefinitions 5      �� 5      $
��auto �6'   �� 5      �� 5      $
��auto �6'   �� 5      �� 5      $
��auto �6'   �� 5      �� 5      $
��auto �6'   �� 5        5      �� 5      �� 5       �� 5!      .+�5$      �� $��Name:��6   $��0��6)   $��0��66   $
��Left=�6F   $��Center=�6a   $��0,0,10,0q�6|   �� 5%      $��0��6   $��1��6'   $��23��6|   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *Appliance.Name+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5&      $��Mobile:��6   $��1��6+   $��0��68   $
��Left=�6H   $��Center=�6c   $��0,0,10,0q�6~   �� 5'      $��1��6   $��1��6'   $��23��6�   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *Appliance.MobilePhoneNumber+$ PropertyChanged=�6�   �� 5(      $��	Location:��6   $��2��6-   $��0��6:   $
��Left=�6J   $��Center=�6e   $��0,0,10,0q�6�   �� 5)      $��2��6   $��1��6(   $��3��68   $��23��6L   $��0,0,10,0q�6X   $��Center=�6j   ��6�   �� *	Addresses+ �� SelectedItem 6�   �� *CurrentAddressSelectedItem+�� 5+      $��Status:��6   $��0��6+   $��2��68   $
��Left=�6H   $��Center=�6c   $��0,0,10,0q�6~   �� 5,      $��0��6   $��3��6(   $��23��68   $��0,0,10,0q�6D   $��Center=�6V   ��6q   �� *StatusTypes+ 6�   �� *StatusTypeSelectedItem+�� 5.      $
��OiC:��6   $��0��6(   $��4��65   $
��Left=�6E   $��Center=�6`   $��0,0,10,0q�6{   �� 5/      $��0��6   $��5��6'   $��23��6{   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *Appliance.OiC+$ PropertyChanged=�6�   �� 50      $��Crew:��6   $��1��6)   $��2��66   $
��Left=�6F   $��Center=�6a   $��0,0,10,0q�6|   �� 51      $��1��6   $��3��6'   $��23��6�   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *Appliance.NumberOfCrew+$ PropertyChanged=�6�   �� 52      $	��BA:��6   $��1��6'   $��4��64   $
��Left=�6D   $��Center=�6_   $��0,0,10,0q�6z   �� 53      $��1��6   $��5��6'   $��23��6�   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *Appliance.NumberOfBA+$ PropertyChanged=�6�   �� 55      $��Type:��6   $��0��6)   $��6��66   $
��Left=�6F   $��Center=�6a   $��0,0,10,0q�6|   �� 56      $��0��6   $��7��6(   $��23��68   $��0,0,10,0q�6D   $��Center=�6V   ��6q   �� *ApplianceTypes+ 6�   �� *ApplianceTypeSelectedItem+�� 57      $��Office:��6   $��1��6+   $��6��68   $
��Left=�6H   $��Center=�6c   $��0,0,10,0q�6~   �� 58      $��1��6   $��7��6'   $��23��6�   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *" Appliance.Base.OfficePhoneNumber+$ PropertyChanged=�6�   �� 59      $��Base:��6   $��2��6)   $��4��66   $
��Left=�6F   $��Center=�6a   $��0,0,10,0q�6|   �� 5:      $��3��6   $��5��6(   $��3��68   $��23��6L   $��0,0,10,0q�6X   $��Center=�6j   ��6�   �� *Bases+ 6�   �� *BaseSelectedItem+5;      5<      �� 5=   
   $��Stretch=�6   $��1��65   &   System.Windows.Controls.DataGrid  5>   
   $��2��6   $��Stretch=�6    $��Stretch=�6>      AutoGenerateColumns .     
IsReadOnly .    SelectionMode$ Single=�6�      SelectionUnit$ FullRow=�6�   ��6[   �� *	History+ 6�   �� *SelectedItem+   Columns 5?      0  *System.Windows.Controls.DataGridTextColumn  5@      ,  &System.Windows.Controls.DataGridColumn	   Header$	 	Date/Time��6%   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 68   �� *
DateTime+  5A      $	 Description��6%   
 6:   �� *Description+  5B      $
	 User��6%   
 63   �� *User+5D      5E      5F      