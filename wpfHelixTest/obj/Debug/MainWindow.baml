   M S B A M L   `   `   `  ����U  QInteractiveDataDisplay.WPF, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullkLclr-namespace:InteractiveDataDisplay.WPF;assembly=InteractiveDataDisplay.WPFInteractiveDataDisplay.WPF  5   	   U QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf 5   	   G CwpfHelixTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:wpfHelixTestwpfHelixTest 5
   	      wpfHelixTest.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   [d3Lclr-namespace:InteractiveDataDisplay.WPF;assembly=InteractiveDataDisplay.WPF     5   	   Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5   	   W SHelixToolkit.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=52aa3500039caf0d(helixhttp://helix-toolkit.org/wpf  5   	   &localclr-namespace:wpfHelixTest  5
   	   -   5          � Title$  
MainWindow��5   	   $	��400��6   $	��800��6)   $	��400��65   $	��600��6E   .��5      � -   5         �Name$ MainWindow1��5       � RowDefinitions 5   
   �� 5      $	��19* �6   �� 5      �� 5      $
��351* �6   .+�5   
   ]� $��Stretch=�6   $��18��62   ��.  $	��Top=�6T   $��0,0,0.333,0q�6l   .+�5      o� -   5        $��MainMenu��5      $��20��6#   $	��Top=�6/   $	��Top=�6G   .@�5      m� -   5        $��MenuFile��5   )   $
��File��6   .@�5      m� $��Load stl model��6   m� 5      $
��Exit��6   5      m� 5      -   5        $��MenuEdit��5   )   $
��Edit��6   6)   m� 5      -   5        $��MenuView��5   /   $��
Connection��6   .@�5      m� -   5        $��Configuration��5      66   5      5       5!      �� 5#   
   -   5        $
��Left=�5#      $��0,2,0,-0.333q�60   $	��793��6F   $��2��6R   .@�5$      �� $��TabItem��6   .��5%      � $��0,0,-0.667,0.333q�6    � ColumnDefinitions 5&      �� 5'      $	��45* �6+   �� 5(      $
��349* �6+   .+�5,      &   HelixToolkit.Wpf.HelixViewport3D  -	   5          Name$ 
viewPort3d��5,   ,   $��24.333,62,220.001,15q�6A   $	��150��6_   $	��800��6n   $	��212��6}   $��1��6�      Children. 5/      $  HelixToolkit.Wpf.DefaultLights     Camera 51      =� 52       � LookDirection$ 0,0,-15�60   54      56      � 58      $	��324��6   $	��Top=�6(   $��0,0,-0.999,-3q�6@   $
��Left=�6W   $	��789��6r   $��2��6~   .+�59      �� -
   5         ��Name$ loadStl_btn��59   !   $��Load Stl Model��66   $
��Left=�6P   $��	8,0,0,198q�6k   $��bottom=�6~   $��80��6�   $��9(�6�   �� 5:      -   5        $ loadSensors_btn��5:   !   $��Load sensors��6:   $
��Left=�6Q   $��	8,0,0,177q�6l   $��bottom=�6   $��80��6�   	 J� RenderTransformOrigin$	 0.5,0.5&�6�   $��9(�6�   �� 5;      -   5        $ 	start_btn��5;   !   $��Start��64   $
��Left=�6D   $��	8,0,0,156q�6_   $��bottom=�6r   $��80��6�   $	 0.5,0.5&�6�   $��9(�6�   6�   �� 5<      -   5        $ stop_btn��5<   !   $
��Stop��63   $
��Left=�6B   $��	8,0,0,135q�6]   $��bottom=�6p   $��80��6�   $	 0.5,0.5&�6�   $��9(�6�   �� 5=      -   5        $ exportCsv_btn��5=   !   $��
Export csv��68   $
��Left=�6M   $��	8,0,0,114q�6h   $��bottom=�6{   $��80��6�   $	 0.5,0.5&�6�   $��9(�6�   6�   �� 5>      -   5        $ exportTxt_btn��5>   !   $��
Export txt��68   $
��Left=�6M   $��8,0,0,93q�6h   $��bottom=�6z   $��80��6�   $	 0.5,0.5&�6�   $��9(�6�   6�   5?      &   System.Windows.Controls.DataGrid  5@      -   5        
  Name$
 SensorsInfoDataGrid��5@      $��0,62,10.001,20q�6<   $��Right=�6T   $��Stretch=�6p   $
��Auto��6�   $
��Auto��6�   $	 0.5,0.5&�6�   $	��400��6�   $��9(�6�   $��1��6�    � 	Resources 5B      =  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value��%�   '        �� 5C      "  6$   .�5D   "   ��  � ��$ 0,0,1,2q�6D   6V   �� 5E   "    � ��$ Center=�6O   6`   5F      5G         Columns 5I      0  *System.Windows.Controls.DataGridTextColumn  5J      ,  &System.Windows.Controls.DataGridColumn   Header$  ��61      Width5  /System.Windows.Controls.DataGridLengthConverter$ 48 6U      FontSize$ 8Q�6`   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6<   �� *Name+6`     5K      $ X��61   $ 30 6R   $ 8Q�6]    6<   �� *x+6]     5L      $ Y��61   $ 30 6R   $ 8Q�6]    6<   �� *y+6]     5M      $ Z��61   $ 30 6R   $ 8Q�6]    6<   �� *z+6]     5N      $ Delta Z��61   $ 50 6]   $ 8Q�6h    6B   �� *deltaZ+6h   5Q      �� 5S      $
��Left=�6    $
��Auto��6;   
���   �$��Stretch=�6X   $��8��6t   $��7.333,63,0,19q�6~   $��1��6�   ��5T      �� 5U       �� 
StartPoint$	 0,0&�62    �� EndPoint$	 1,1&�6C   .��5V   "   �  � Color$	 Red��6/    � Offset$
 0.40Q�6;   � 5W   "   $ Black��6/   $
 0.50Q�6=   � 5X   "   $ 	LawnGreen��6/   $
 0.60Q�6A   5Y      5[      5\      5]      �� 5^      $��TabItem��6   .��5_      � .+�5a      �� -   5        $
��Play��5a      $
��Left=�6,   $��
10,137,0,0q�6G   $	��Top=�6[   $��75��6s   6~   �� 5b      $��10,188,681.333,99.333q�6   
������ 
������ .��5c      ��  �� Orientation$ 
Horizontal=�6%   $
��Left=�6>   .+�5d      �� $��18��6(   �� 65   $�	Save Data��69   6M   �� 5e      $%��wpfHelixTest/images/saveBtn.png��6$   $��30��6M   $��30��6Y   5f      5g      5i      5k      �� 5l      $��TabItem��6   .��5n      #	  LiveCharts.Wpf.CartesianChart	  &
   LiveCharts.Wpf.Charts.Base.Chart 
  LegendLocation$ Right=�6I   $	��375��6`   $	��300��6l   $��215,22,197.333,-0.667q�6y    
  Series 6%   �� *SeriesCollection+ 
  AxisX 5p        LiveCharts.Wpf.Axis     Title$ Sales��6#      LabelFormatter 61   �� *
YFormatter+6Z   5r      5s      5u      5w      5x      