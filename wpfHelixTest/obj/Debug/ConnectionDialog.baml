   M S B A M L   `   `   `  ����G  CwpfHelixTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:wpfHelixTestwpfHelixTest  5   	   #    wpfHelixTest.ConnectionDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:wpfHelixTest     5   	     � Title$  ConnectionDialog��5   	   $	��190��6"   $	��300��6/   .��5
      � $	��160��6   $	��290��6   .+�5   
   �� $��Hostname��6   $
��Left=�6#   $��30��6>   $��	20,15,0,0q�6J   $	��Top=�6]   �� 5   
   $
��Port��6   $
��Left=�6   $��30��6:   $��	20,40,0,0q�6F   $	��Top=�6Y   �� 5   
   $��Username��6   $
��Left=�6#   $��30��6>   $��	20,66,0,0q�6J   $	��Top=�6]   �� 5   
   $��Password��6   $
��Left=�6#   $��30��6>   $��	20,92,0,0q�6J   $	��Top=�6]   �� 5   
   -   5         ��Name$ HostnameBox��5      $
��Left=�6'    �� TextWrapping$
 Wrap=�6C   $	��Top=�6v   $��20��6�   $	��123��6�   $��
109,21,0,0q�6�   ��6W   ��  �� Path$ HostName�6�   �� 5   
   -   5        $ PortBox��5      $
��Left=�6#   $
 Wrap=�6?   $	��Top=�6n   $��20��6�   $	��123��6�   $��
109,46,0,0q�6�   ��6S   �� $
 Port�6�   �� 5   
   -   5        $ UsernameBox��5      $
��Left=�6'   $
 Wrap=�6C   $	��Top=�6v   $��20��6�   $	��123��6�   $��
109,71,0,0q�6�   ��6W   �� $ Username�6�   G� 5   
   -   5         G�Name$ PasswordBox��5      $��20��6+   $	��123��67   $��109,96,58,44q�6C   �� 5   
   -   5         ��Name$ OkConnectionBtn��5      $��Ok��6*   $
��Left=�67   $��109,128,0,0q�6R   $	��Top=�6g   $��58��6    J� RenderTransformOrigin$ 0.606,0.525&�6�    �� 	IsDefault . 6�   �� 5   
   -   5        $ CancelConnectionBtn��5      $��Cancel��6.   $
��Left=�6?   $��174,128,0,0q�6Z   $	��Top=�6o   $��58��6�    �� IsCancel . 6�   5      5      