   M S B A M L   `   `   `  ����M  IEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1859617a688202375clr-namespace:Engine.WpfControlEngine.WpfControl  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 &  "S.Storyboard.RenderTransform.Shake  W� From	 W� To g� Duration �� TargetProperty g� RepeatBehavior W� EasingFunction0   *System.Windows.Media.Animation.ElasticEase ! S.Storyboard.FontSize.Rebound/  )System.Windows.Media.Animation.BounceEase  S.Storyboard.Color.Flash �� From	 �� To g� AutoReverse	 �� EasingFunction.  (System.Windows.Media.Animation.CubicEase / +S.Storyboard.MessageLoad.OpacityRightToLeft
 g� 	BeginTime O� KeyTime O� Value / +S.Storyboard.MessageHide.OpacityLeftToRight R� KeyTime R� Value  System.Windows.Visibility�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    )localclr-namespace:Engine.WpfControl  5      .sys&clr-namespace:System;assembly=mscorlib  5      hQQ:178876839   5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      %Z  &	        &	 �     &	 �    &	 \    &	     �� 5      .��5	   
   W� $	  -10X�6   $ 0X�6%   $ 0:0:0.2D�6-   $V P(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)�6@   $ Forever��6�    5
         5      5      5      �� 5      .��5   
   W� $  5X�6   $ 50X�6#   $ 0:0:2D�6+   $ FontSize�6<   $ Forever��6b    5        5      5      5      �� 5      .��5   
   �� $2 ,(Control.Foreground).(SolidColorBrush.Color)�6   $ Forever��6c   $ GreenX�6}   $ #ED4646X�6�   $ 0:0:2D�6�    . 	 5        5      5      5       �� 5#      .��5$   
   U� $
 00:00:00X�6(   $ (UIElement.Opacity)�6=   .\�5%      �� $ 00:00:00��6#   $ 0Q�66   �� 5&      $ 
00:00:00.5��6#   $ 1Q�68   5'      U� 5)   
   $
 00:00:00X�6(   $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�6=   .\�5*      �� $ 00:00:00��6#   $	 200Q�66   �� 5+      $ 
00:00:00.5��6#   $ 0Q�68   5,      5-      �� 50      .��51   
   U� $
 00:00:00X�6(   $ (UIElement.Opacity)�6=   .\�52      �� $ 00:00:00��6#   $ 0Q�66   �� 53      $ 
00:00:00.5��6#   $ 1Q�68   �� 54      $ 
00:00:04.7��6#   $ 1Q�68   �� 55      $ 00:00:05��6#   $ 0Q�66   56      U� 58   
   $
 00:00:00X�6(   $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�6=   .\�59      �� $ 00:00:00��6#   $	 200Q�66   �� 5:      $ 
00:00:00.5��6#   $ 0Q�68   �� 5;      $ 
00:00:04.7��6#   $ 0Q�68   �� 5<      $ 00:00:05��6#   $	 200Q�66   5=      T� 5?   
   $
 00:00:00X�6(   $ (UIElement.Visibility)�6=   ..�5@      i� $ 0:0:5��6%    5A       5B      	Collapsed=�6,   5D      5E      5F      5f     