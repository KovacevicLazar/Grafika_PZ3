   M S B A M L   `   `   `  ����F  Bpz3_Grafika, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:pz3_Grafikapz3_Grafika  5   	       pz3_Grafika.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:pz3_Grafika     5   	     � WindowState$  	Maximized=�5   	    � Title$ 	PowerGrid��5	   	   .��5      � -   5         �Name$
 grid��5      .+�5   
   '� -   5         '�Name$ ViewPort��5       '� Camera 5      =� 5      -   5        
CAMERA�� � Position$ 
1 -650 8501�5   4    � LookDirection$ 0,3.4,-55�6L    =� FieldOfView$ 65Q�6g    �� 	Transform 5      Y� 5      .�5      S� -   5        translacija��	 S� OffsetX$	 0Q�5   H   
 S� OffsetY$
 0Q�6T    S� OffsetZ$ 0Q�6`   5      5      .x�5      g� -   5        myLight�� g� Content 5      t� 5      -   5        dirLightMain�� t� 	Direction$ 0,2,-105�5   @   5!      g� 5#       5$      h� 5%      -   5        model3dGroup��.��5'      �  � Geometry 5(      k� 5)   "    k� 	Positions5 �    v�� 8�� v������# 8����#�� k� TriangleIndices �     k� TextureCoordinates �   6�    � Material 5,      u� 5-   "    u� Brush 5.   &   �� 5/   *    �� ImageSource$ /Images/PZ3_map.jpg��65   51   #    j� 	Transform 54      �� 55   "   -   5        
Rotate�� �� Rotation 56   &   �� 57   *   -	   5        myAngleRotation�� �� Axis$ 0,0,15�57   W    �� Angle$ 0Q�6d   59   #   5;      5=      5?      5E      �� 5I   
   
���   �$��1q�6%   $
��Left=�69   $	��136��6T   $��	10,10,0,0q�6a   $	��Top=�6t   $	��162��6�   ��5J      �� 5K       �� Opacity$
 0.85Q�6"    �� Color$ #c4d7ff��61   ._�5N      �� $
��Left=�6   $	��136��64   $��-1q�6A   $	��Top=�6M   $	��162��6e   .+�5O      �� $
��Bold"�6   $��16(�6*   .��68   Select Entity:6H   �� 5P      -
   5        $��Nodes��5P      $��Center=�6+   $
��Bold"�6M   $��20(�6_   $��5,0,0,0q�6m   $
��Left=�6   $	��Top=�6�    d� 	IsChecked$
 TrueY�6�   $	��147��6�   6  �� 5Q      -   5        $��Switchs��5Q      $��Center=�6-   $
��Bold"�6O   $��20(�6a   $��5,5,0,0q�6o   $
��Left=�6�   $	��Top=�6�   $
 TrueY�6�   $	��147��6�   �� 5R      -   5        $��Substations��5R      $��Center=�61   $
��Bold"�6S   $��20(�6e   $��5,5,0,0q�6s   $
��Left=�6�   $	��Top=�6�   $
 TrueY�6�   $	��147��6  5S      5U      5\      5]      