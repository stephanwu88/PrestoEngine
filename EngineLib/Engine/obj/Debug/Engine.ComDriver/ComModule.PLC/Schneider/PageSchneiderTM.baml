   M S B A M L   `   `   `  ����M  IEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1859617a68820237G(clr-namespace:Engine.ComDriver.SchneiderEngine.ComDriver.Schneider  5      5clr-namespace:Engine.WpfControlEngine.WpfControl  5      0    *Engine.ComDriver.Schneider.PageSchneiderTM   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      :local(clr-namespace:Engine.ComDriver.Schneider     5      4controlsclr-namespace:Engine.WpfControl     5      -   5          � 	Resources  5
         Menu_VarList
 � Tag �� MergedDictionaries �� Source�� 5   
    5      �� 5      $A ;/Engine;component/Engine.WpfTheme/CustomControl/Button.xaml?�6%   %=  &	        �� 5      .@�5      m� -   5        $ AddOneVariable��5      $��新增变量��60   6?   m� 5      -   5        $ EditOneVariable��5      $��修改变量��61   6?   m� 5      -   5        $ VariableValueWrite��5      $��	修改值��64   6A   m� 5      -   5        $ ListNodeMoveUp��5      $��上移��60   6<   m� 5      -   5        $ ListNodeMoveDown��5      $��下移��62   6>   �� 5      6   m� 5      -   5        $ ImportVariableList��5      $��导入��64   6@   m� 5      -   5        $ ExportVariableList��5      $��导出��64   6@   �� 5      6   m� 5      -	   5        $ VariableNodeCut��5      $��剪切��61   6=   m� 5      -
   5        $ VariableNodeCopy��5      $��复制��62   6>   m� 5      -   5        $ VariableNodePaste��5      $��粘贴��63   6?   m� 5      -   5        $ VariableNodeDelete��5      $��删除��64   6@   m� 5      -   5        $ SelectListAll��5      $��全选��6/   6;   5      5      .��5!      � .+�5"   
   &   System.Windows.Controls.DataGrid  -   5          Name$ 
_dgVarList��5"       � ContextMenu# [  6Z      
IsReadOnly . $��15(�6�      	RowHeight$ 25Q�6�      VerticalGridLinesBrush �	#FFF0F0F0	   HorizontalGridLinesBrush	 �	#FFF0F0F0
   AutoGenerateColumns
 .     CanUserAddRows .  $��0,62,0,0q�6G  ��6'   �� *DataSet.EditVariableNodes+��6�   Z�    Columns 5#      0  *System.Windows.Controls.DataGridTextColumn  5$      ,  &System.Windows.Controls.DataGridColumn   Header$ 序号��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 61      
Visibility$ Hidden=�6=   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6Q   �� *ID+  5%      $ 	排序号��6%   $	 100 62   $ Hidden=�6>    6R   �� *
ORDER_ID+  5&      $ 控制器序号��6%   $	 100 64   $ Hidden=�6@    6T   �� *	Driver_ID+  5'      $ 	数据组��6%   $	 100 62   $ Hidden=�6>    6R   �� *	V_GROUP+  5(      $ 名称��6%   $	 200 61    6=   �� *V_NAME+  5)      $ 地址��6%   $	 100 61    6=   �� *V_ADDR+  5*      $ 数据类型��6%   $	 100 63    6?   �� *V_TYPE+  5+      $ 访问权限��6%   $	 100 63    6?   �� *
V_ACCESS+4  .System.Windows.Controls.DataGridTemplateColumn  53      $	 150 6)      HeaderTemplate 54      �� 55      .R�56      �� $��0,0,0,0q�6(    �� TextAlignment$ Center=�69   $��	监控值��6P   $	��120��6[   57         CellTemplate 59      �� 5:      .R�5@      �� $��CharacterEllipsis=�6@   $ Center=�6a   $��0,0,0,0q�6x   ��6(   �� *V_VIEW+��5A   0   �� *VarViewBackColor+5B      5D        5G      $	 100 6)    5H      �� 5I      .R�5J      �� $��0,0,0,0q�6(   $ Center=�69   $��	修改值��6P   $	��100��6[   5K       5M      �� 5N      .R�5O      ��  �� TextAlignment$ Center=�6>   $��0,0,0,0q�6U   ��6&   �� *V_EDIT+6U   5P      5R      4  .System.Windows.Controls.DataGridCheckBoxColumn  5T      $	 HMI��6)   $ 50 66    6A   �� *IsHmiVisible+  5U      $ 报警��6)   $ 50 65    6@   �� *	IsAlarm+  5V      $ 注释��6%   $	 300 61    6=   �� *
V_REMARK+5X      �� 5Z   
   $��32��6   ���	#FFF0F0F0$	��Top=�68    �� Orientation$ 
Horizontal=�6P   $��0,30,0,0q�6i   .+�5[      #   Engine.WpfControl.MediaButton  #	   Engine.WpfControl.MediaButton 	  Click$ Menu_VarList_Click��6�   $ AddOneVariable��6#      
ButtonType$
 Icon=�68      Icon$, &/Engine;component/Assets/Image/New.ico��6J   $��新增变量��6x   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�     5\      -   5          Name$ _CmdVarEdit��5\   9   $ Menu_VarList_Click��6�   $ EditOneVariable��6#   $
 Icon=�6N   $0 */Engine;component/Assets/Image/EditDoc.png��6`   $��修改变量��6�   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�   �� 5]      #��["�6   $��4q�6V   
�������$��1q�6y     5^      -   5        $ VariableNodeCopy��5^   #   $ Menu_VarList_Click��6�   $
 Icon=�6=   $- '/Engine;component/Assets/Image/Copy.ico��6O   $��复制变量��6~   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�     5_      -   5        $ VariableNodePaste��5_   #   $ Menu_VarList_Click��6�   $
 Icon=�6>   $. (/Engine;component/Assets/Image/Paste.png��6P   $��粘贴变量��6�   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�     5`      -   5        $ VariableNodeDelete��5`   #   $ Menu_VarList_Click��6�   $
 Icon=�6?   $0 */Engine;component/Assets/Image/Del_Doc.ico��6Q   $��删除变量��6�   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�   �� 5a      #��["�6   $��4q�6V   
�������$��1q�6y   �� 5b      -   5         ��Name$ 
_DataGroup��5b      $
��Left=�6^   $��	0,3.5,0,0q�6y   $	��139��6�   $��25��6�   $	��Top=�6�   ��6+   �� *DataSet.ListDataGroupNames+6�     5c      -   5        $ VariableValueWrite��5c   #   $ 
_Cmd_Click��6�   $
 Icon=�6?   $. (/Engine;component/Assets/Image/Write.ico��6Q   $��	写入值��6�   $��10,0,0,0q�6�   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�     5d      -   5        $ MonitorOnce��5d   #   $ 
_Cmd_Click��6�   $
 Icon=�68   $0 */Engine;component/Assets/Image/MonOnce.ico��6J   $��监视一次��6|   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�     5e      -   5        $ MonitorContinue��5e   #   $ 
_Cmd_Click��6�   $
 Icon=�6<   $/ )/Engine;component/Assets/Image/CycMon.ico��6N   $��全部监视��6   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�     5f      -   5        $ ExportVariableList��5f   #   $ Menu_VarList_Click��6�   $
 Icon=�6?   $/ )/Engine;component/Assets/Image/CycMon.ico��6Q   $��导出��6�   $
��Left=�6�   $	��Top=�6�   $��30��6�   $��30��6�   6�   5g      �� 5h   
   
���   �
��� ���$��0q�69   $��30��6M   $	��Top=�6Y   $ Backgroud:BrushTitleBack��5k      ._�5l      �� $��Schneider PLC ▶ TM241��6   
������� �� Padding$ 10,0,0,0q�6H   $��CharacterEllipsis=�6[   $��Center=�6|   $��15(�6�   $G AForeground="{Binding BrushTitleFore}" Text="{Binding TitleString}��5p   !   5q      5r      5s      5t      