   M S B A M L   `   `   `  ����  	Prototype'    !Prototype.View.SingleIncidentView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $	��150 �6   �� 5      $��3 �6   �� 5      $��* �6   �� 5      $��3 �6   �� 5      $��2* �6   .+�5   
   �� $��0��6   $��Incident Information��6    $��Stretch=�6>   $��Stretch=�6\   .��5      � $��Stretch=�6   $��Stretch=�61    � ColumnDefinitions 5      �� 5      $
��auto �6'   �� 5      �� 5      $
��auto �6'   �� 5      �� 5      $
��auto �6'   �� 5      �� 5      $
��auto �6'   �� 5      �� 5      $
��auto �6'     5!      �� 5"      �� 5#      �� 5$      �� 5%      �� 5&      .+�5)      �� $��
Date/Time:��6   $��0��6.   $��0��6;   $
��Left=�6K   $��Center=�6f   $��0,0,10,0q�6�   �� 5*      $��0��6   $��1��6'   $��23��6p   $��0,0,10,0q�6|   $��Center=�6�   {�.  ��67   �� *Incident.DateTimeFormatted+ �� Mode$ OneWay=�6�   �� 5+      $��Caller:��6   $��1��6+   $��0��68   $
��Left=�6H   $��Center=�6c   $��0,0,10,0q�6~   �� 5,      $��1��6   $��1��6'   $��24��6}   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *Incident.Caller+ �� UpdateSourceTrigger$ PropertyChanged=�{�6�   �� *ButtonEnabled+�� 5-      $��Address:��6   $��2��6,   $��0��69   $
��Left=�6I   $��Center=�6d   $��0,0,10,0q�6   �� 5.      $��2��6   $��1��6'   $��3��67   $��23��6z   $��0,0,10,0q�6�   $��Center=�6�   {�.  ��6K   �� *Incident.Address+$ OneWay=�6�   �� 50      $��Type:��6   $��0��6)   $��2��66   $
��Left=�6F   $��Center=�6a   $��0,0,10,0q�6|   �� 51      $��0��6   $��3��6(   $��23��68   $��0,0,10,0q�6D   $��Center=�6V   ��6q   �� *IncidentTypes+ �� SelectedItem 6�   �� *IncidentTypeSelectedItem+{�6�   �� *ButtonEnabled+�� 53      $
��OiC:��6   $��0��6(   $��4��65   $
��Left=�6E   $��Center=�6`   $��0,0,10,0q�6{   �� 54      $��0��6   $��5��6'   $��23��6z   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *Incident.OiC+$ PropertyChanged=�{�6�   �� *ButtonEnabled+�� 55      $��	Exchange:��6   $��1��6-   $��2��6:   $
��Left=�6J   $��Center=�6e   $��0,0,10,0q�6�   �� 56      $��1��6   $��3��6'   $��24��6   $��0,0,10,0q�6�   $��Center=�6�   ��67   �� *Incident.Exchange+$ PropertyChanged=�{�6�   �� *ButtonEnabled+�� 57      $��Details:��6   $��1��6,   $��4��69   $
��Left=�6I   $��Center=�6d   $��0,0,10,0q�6   �� 58      $��1��6   $��5��6'   $��2��67   $��0,0,10,0q�6�   $��Stretch=�6�    �� TextWrapping$
 Wrap=�6�    �� AcceptsReturn . ��6H   �� *Incident.Details+$ PropertyChanged=�{�6�   �� *ButtonEnabled+�� 5:      $��	Operator:��6   $��0��6-   $��6��6:   $
��Left=�6J   $��Center=�6e   $��0,0,10,0q�6�   �� 5;      $��0��6   $��7��6'   $��2��67   $��23��6�   $��0,0,10,0q�6�   $��Center=�6�   {�.  ��6K   �� *Incident.Operator+$ PropertyChanged=�$ OneWay=�6�   �� 5<      $��
Stop Time:��6   $��1��6.   $��6��6;   $
��Left=�6K   $��Center=�6f   $��0,0,10,0q�6�   �� 5=      $��1��6   $��7��6'   $��2��67   $��23��6�   $��0,0,10,0q�6�   $��Center=�6�   {�.  ��6K   �� *Incident.StopTimeFormatted+$ OneWay=�$ PropertyChanged=�6�   �� 5>      $��Status:��6   $��2��6+   $��6��68   $
��Left=�6H   $��Center=�6c   $��0,0,10,0q�6~   �� 5?      $��2��6   $��7��6(   $��23��68   $��0,0,10,0q�6D   $��Center=�6V   ��6q   �� *IncidentStates+ 6�   �� *CurrentState+$ PropertyChanged=��� 5@      $��2��6   $��8��6&   $��Center=�66   $��Right=�6Q   $��23��6m   $�� Assign... ��6y   $��0,0,10,0q�6�   ��6�   �� *AssignAppliancesCommand+{�6�   �� *ButtonEnabled+�� 5A      $��Summary:��6   $��3��6,   $��0��69   $
��Left=�6I   $��Center=�6d   $��0,0,10,0q�6   �� 5B      $��3��6   $��1��6'   $��8��67   $��2��6K   $��0,5,10,0q�6�   $��Stretch=�6�   $
 Wrap=�6�    . ��6\   �� *Incident.Summary+$ PropertyChanged=�{�6�   �� *ButtonEnabled+5D      5E      �� 5F   
   $��Stretch=�6   $��1��65   &   System.Windows.Controls.DataGrid  5G   
   $��2��6   $��Stretch=�6    $��Stretch=�6>      AutoGenerateColumns .     
IsReadOnly . 	   SelectionMode$	 Single=�6�   
   SelectionUnit$
 FullRow=�6�   ��6[   �� *Incident.AssignedResources+ 6�   �� *SelectedItem+   Columns 5H      0  *System.Windows.Controls.DataGridTextColumn  5I      ,  &System.Windows.Controls.DataGridColumn   Header$	 OiC��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 62   �� *OiC+  5J      $
 Crew��6%    63   �� *Crew+  5K      $ BA��6%    61   �� *BA+  5L      $ Resource��6%    67   �� *
CallSign+  5M      $ Alerted��6%    66   �� *AlertedFormatted+  5N      $ Mobile��6%    65   �� *MobileFormatted+  5O      $ On Scene��6%    67   �� *InAttendanceFormatted+  5P      $ 	Available��6%    68   �� *AvailableFormatted+  5Q      $ Closed��6%    65   �� *ClosedDownFormatted+ J� InputBindings 5T      d� 5U       d� MouseAction$ LeftDoubleClicke�6    �� Command 6=   �� *ViewApplianceCommand+5W      �� 5X   
   $��Stretch=�6   $��3��65     5Y   
   $��4��6   $��Stretch=�6    $��Stretch=�6>    .   . $	 Single=�6�   $
 FullRow=�6�   ��6[   �� *
Messages+ 5Z        5[      $ 	Date/Time��6%    68   �� *
DateTime+  5\      $ Message��6%      MaxWidth$	 900Q�6V    66   �� *Description+   ElementStyle 5]      �� 5^      .�5_      ��  �� Property � �� �� Value$
 Wrap=�6G   5`      5b        5c      $
 User��6%    63   �� *User+5e      5f      5g      