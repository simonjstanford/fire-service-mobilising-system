   M S B A M L   `   `   `  ����  	Prototype#    Prototype.View.AppliancesView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5	      � .+�5
   
   &   System.Windows.Controls.DataGrid  $��Stretch=�6   $��Stretch=�61       AutoGenerateColumns  .     
IsReadOnly .    SelectionMode$ Single=�6�      SelectionUnit$ FullRow=�6�   ��6N   �� *
Appliances+ �� SelectedItem 6�   �� *SelectedItem+   	CellStyle 5      �� 5       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6    �� Triggers 5      �� 5       �� Binding 6&   �� *Status+	 �� Value	 Alerted6A   .a�5      �� 
 �� Property
 � �� �� Value
 ����6;   �� 5      
 � ��
 �  ��6;   5      �� 5       6&   �� *Status+	 Mobile to Incident6A   .a�5      �� 
 � ��
 � ���6;   �� 5      
 � ��
 �   �6;   5      �� 5       6&   �� *Status+	 On Scene6A   .a�5      �� 
 � ��
 �  ��6;   �� 5      
 � ��
 �����6;   5      �� 5       6&   �� *Status+	 	Available6A   .a�5      �� 
 � ��
 ��ح�6;   �� 5      
 � ��
 �   �6;   5      �� 5       6&   �� *Status+	 Unavailable6A   .a�5      �� 
 � ��
 �   �6;   �� 5       
 � ��
 �����6;   5!      5#         Columns 5&      0  *System.Windows.Controls.DataGridTextColumn  5'      ,  &System.Windows.Controls.DataGridColumn   Header$ CallSign��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *
CallSign+  5(      $
 Name��6%    63   �� *Name+  5)      $	 OiC��6%    62   �� *OiC+  5*      $
 Crew��6%    63   �� *Crew+  5+      $ BA��6%    61   �� *BA+  5,      $
 Type��6%    63   �� *Type+  5-      $ Location��6%    67   �� *	Address+  5.      $ Status��6%    65   �� *Status+  5/      $ Incident��6%    67   �� *IncidentFormatted+ J� InputBindings 53      d� 54       d� MouseAction$ LeftDoubleClicke�6    �� Command 6=   �� *ViewApplianceCommand+56      57      58      