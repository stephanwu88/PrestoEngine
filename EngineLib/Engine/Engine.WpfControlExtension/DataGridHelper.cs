using Engine.Common;
using Engine.WpfBase;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Engine.WpfControl
{
    /// <summary>
    /// DataGrid控件相关方法
    /// </summary>
    public static partial class DataGridHelper
    {
        /// <summary>
        /// 根据单元格获取选中行
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <returns></returns>
        public static object SelectedGridRow(this System.Windows.Controls.DataGrid dataGrid)
        {
            try
            {
                if (dataGrid == null)
                    return null;
                if (dataGrid.SelectionUnit == System.Windows.Controls.DataGridSelectionUnit.Cell)
                {
                    if (dataGrid != null && dataGrid.SelectedCells.Count > 0)
                    {
                        var selectedCell = dataGrid.SelectedCells[0];
                        var selectedRow = selectedCell.Item;
                        return selectedRow;
                    }
                }
                return dataGrid.SelectedItem;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// DataGrid控件附加依赖属性及回调封装
    /// </summary>
    public static partial class DataGridHelper
    {
        #region AutoFocus 单元格鼠标左键点击自动聚焦 
        public static bool GetAutoFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoFocusProperty);
        }

        public static void SetAutoFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoFocusProperty, value);
        }

        public static readonly DependencyProperty AutoFocusProperty =
            DependencyProperty.RegisterAttached("AutoFocus", typeof(bool), typeof(DataGridHelper), new PropertyMetadata(false, AutoFocusPropertyChanged));

        private static void AutoFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.DataGridCell cell && (bool)e.NewValue)
            {
                cell.PreviewMouseLeftButtonUp += (sender, args) =>
                {
                    try
                    {
                        System.Windows.Controls.DataGridCell clickedCell = sender as System.Windows.Controls.DataGridCell;
                        if (clickedCell != null && !clickedCell.IsEditing)
                        {
                            clickedCell.Focus();
                            var dataGrid = sCommon.GetVisualParent<System.Windows.Controls.DataGrid>(clickedCell);
                            if (dataGrid.SelectedCells.Count > 1)
                                return;
                            if (dataGrid != null)
                            {
                                dataGrid.CurrentCell = new DataGridCellInfo(clickedCell);
                                dataGrid.BeginEdit();
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }                    
                };
            }
        }
        #endregion

        #region CellMoveFocus 上下左右 F2 移动单元格并且聚焦 --  SelectionUnit= Cell 模式下才可以
        public static bool GetCellMoveFocus(this DependencyObject obj)
        {
            return (bool)obj.GetValue(CellMoveFocusProperty);
        }

        public static void SetCellMoveFocus(this DependencyObject obj, bool value)
        {
            obj.SetValue(CellMoveFocusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CellMoveFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellMoveFocusProperty =
            DependencyProperty.RegisterAttached("CellMoveFocus", typeof(bool), typeof(DataGridHelper), new PropertyMetadata(false, CellMoveFocusPropertyChanged));

        private static void CellMoveFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is System.Windows.Controls.DataGridCell cell && (bool)e.NewValue)
                {
                    var dataGrid = sCommon.GetVisualParent<System.Windows.Controls.DataGrid>(cell);
                    if (dataGrid == null)
                        return;
                    dataGrid.PreviewKeyDown += (sender, args) =>
                    {
                        if (dataGrid.SelectionUnit != DataGridSelectionUnit.Cell)
                            return;
                        if (args.Key == Key.Up)
                        {
                            // 向上移动焦点
                            MoveFocusToCell(dataGrid, -1, 0);
                            args.Handled = true;
                        }
                        else if (args.Key == Key.Down)
                        {
                            // 向下移动焦点
                            MoveFocusToCell(dataGrid, 1, 0);
                            args.Handled = true;
                        }
                        else if (args.Key == Key.Left)
                        {
                            // 向左移动焦点
                            MoveFocusToCell(dataGrid, 0, -1);
                            args.Handled = true;
                        }
                        else if (args.Key == Key.Right)
                        {
                            // 向右移动焦点
                            MoveFocusToCell(dataGrid, 0, 1);
                            args.Handled = true;
                        }
                        else if (args.Key == Key.F2)
                        {
                            // 向右移动焦点
                            MoveFocusToCell(dataGrid, 0, 0);
                            args.Handled = true;
                        }

                        //if (args.Key == System.Windows.Input.Key.Enter)
                        //{
                        //    var dataGrid = (System.Windows.Controls.DataGrid)sender;
                        //    var cell = GetNextCell(dataGrid, dataGrid.CurrentCell);
                        //    if (cell != null)
                        //    {
                        //        dataGrid.CurrentCell = cell;
                        //        dataGrid.ScrollIntoView(cell.Item);
                        //        dataGrid.BeginEdit();
                        //    }
                        //    e.Handled = true; 
                        //}
                    };
                }
            }
            catch (Exception)
            {

            }            
        }

        /// <summary>
        /// 移动单元格聚焦
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="rowOffset"></param>
        /// <param name="columnOffset"></param>
        private static void MoveFocusToCell(System.Windows.Controls.DataGrid dataGrid, int rowOffset, int columnOffset)
        {
            if (dataGrid == null) return;
            if (dataGrid.SelectedCells.Count > 0)
            {
                DataGridCellInfo currentCell = dataGrid.SelectedCells[0];
                int columnIndex = dataGrid.CurrentCell.Column.DisplayIndex + columnOffset;
                int rowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentCell.Item) + rowOffset;

                if (columnIndex >= 0 && columnIndex < dataGrid.Columns.Count && rowIndex >= 0 && rowIndex < dataGrid.Items.Count)
                {
                    DataGridRow newSelectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
                    if (newSelectedRow != null)
                    {
                        DataGridCellsPresenter presenter = sCommon.GetVisualChild<DataGridCellsPresenter>(newSelectedRow);
                        if (presenter != null)
                        {
                            System.Windows.Controls.DataGridCell newSelectedCell = (System.Windows.Controls.DataGridCell)
                                presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                            if (newSelectedCell != null)
                            {
                                newSelectedCell.Focus();
                                dataGrid.SelectedCells.Clear();
                                newSelectedCell.IsSelected = true;
                                dataGrid.BeginEdit();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 单元格内容修改 绑定实体属性
        public static string GetCellModifiedBinding(this DependencyObject obj)
        {
            return (string)obj.GetValue(CellModifiedBindingProperty);
        }

        public static void SetCellModifiedBinding(this DependencyObject obj, string value)
        {
            obj.SetValue(CellModifiedBindingProperty, value);
        }

        // Using a DependencyProperty as the backing store for CellModifiedBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellModifiedBindingProperty =
            DependencyProperty.RegisterAttached("CellModifiedBinding", typeof(string), typeof(DataGridHelper), new PropertyMetadata("",CellModifiedBindingPropertyChanged));

        private static void CellModifiedBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string CurrentCellValue = string.Empty;
            if (d is DataGrid dg && e.NewValue != null)
            {
                string strBindingName = e.NewValue.ToMyString();
                if (strBindingName.IsEmpty())
                    return;
                dg.CurrentCellChanged += (sd, ev) =>
                  {
                      try
                      {
                          DataGrid dataGrid = (DataGrid)sd;

                          // 获取当前选中单元格的行索引和列索引
                          int rowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
                          int columnIndex = dataGrid.CurrentColumn.DisplayIndex;

                          // 获取当前选中单元格对应的数据项
                          DataGridCellInfo cellInfo = new DataGridCellInfo(dataGrid.Items[rowIndex], dataGrid.Columns[columnIndex]);

                          if (cellInfo.IsValid && cellInfo.Column is DataGridBoundColumn)
                          {
                              // 获取绑定到该列的属性名称
                              string propertyName = cellInfo.Column.SortMemberPath;
                              // 获取当前单元格的内容
                              object cellValue = dataGrid.CurrentItem.GetType().GetProperty(propertyName).GetValue(dataGrid.CurrentItem);
                              CurrentCellValue = cellValue.ToMyString();
                          }
                      }
                      catch (Exception)
                      {
                          CurrentCellValue = SystemDefault.StringEmpty;
                      }
                  };
                dg.CellEditEnding += (sd, ev) =>
                {
                    try
                    {
                        // 获取编辑后的单元格内容
                        //DataGridCell cell = ev.EditingElement as DataGridCell;
                        //string newValue = (cell.Content as TextBlock).Text;
                        string newValue = (ev.EditingElement as TextBox).Text;
                        // 获取原始的单元格内容
                        var item = ev.Row.Item; // 获取当前行的数据项
                        var propertyName = ev.Column.SortMemberPath; // 获取当前列绑定的属性名称
                        var propertyValue = item.GetType().GetProperty(propertyName).GetValue(item, null); // 获取原始值

                        // 判断单元格内容是否发生改变
                        //if (!newValue.Equals(propertyValue.ToMyString()))
                        if (!newValue.Equals(CurrentCellValue) && CurrentCellValue!=SystemDefault.StringEmpty)
                        {
                            item.GetType().GetProperty(strBindingName).SetValue(item, true);
                        }
                        else
                        {
                            // 单元格内容没有变化
                        }
                    }
                    catch (Exception)
                    {

                    }                   
                };
            }
        }


        #endregion

        #region TextColumnCellStyle 指定TextColumn对象单元格显示样式
        public static string GetTextColumnCellStyle(this DependencyObject obj)
        {
            return (string)obj.GetValue(TextColumnCellStyleProperty);
        }

        public static void SetTextColumnCellStyle(this DependencyObject obj, string value)
        {
            obj.SetValue(TextColumnCellStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextColumnCellStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextColumnCellStyleProperty =
            DependencyProperty.RegisterAttached("TextColumnCellStyle", typeof(string), typeof(DataGridHelper), new PropertyMetadata("", TextColumnCellStyleChanged));

        private static void TextColumnCellStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridTextColumn col && e.NewValue != null)
            {
                try
                {
                    col.ElementStyle = ResouceManager.Default.FindResource(e.NewValue.ToMyString()) as Style;
                    //Style style = new Style(typeof(TextBlock));
                    ////style.TargetType = typeof(TextBlock);
                    //Setter setter = new Setter();
                    //setter.Property = TextBlock.TextAlignmentProperty;
                    //setter.Value = TextAlignment.Center;
                    //style.Setters.Add(setter);
                    //setter = new Setter();
                    //setter.Property = TextBox.FontWeightProperty;
                    //setter.Value = FontWeights.Bold;
                    //style.Setters.Add(setter);
                    //col.ElementStyle = style;
                }
                catch (Exception)
                {

                }
            }
        }
        #endregion

        #region TextColumnCellEditStyle 指定TextColumn对象单元格编辑样式
        public static string GetTextColumnCellEditStyle(this DependencyObject obj)
        {
            return (string)obj.GetValue(TextColumnCellEditStyleProperty);
        }

        public static void SetTextColumnCellEditStyle(this DependencyObject obj, string value)
        {
            obj.SetValue(TextColumnCellEditStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextColumnCellEditStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextColumnCellEditStyleProperty =
            DependencyProperty.RegisterAttached("TextColumnCellEditStyle", typeof(string), typeof(DataGridHelper), new PropertyMetadata("", TextColumnCellEditStyleChanged));

        private static void TextColumnCellEditStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridTextColumn col && e.NewValue != null)
            {
                try
                {
                    col.EditingElementStyle = ResouceManager.Default.FindResource(e.NewValue.ToMyString()) as Style;
                    //style = new Style(typeof(TextBox));
                    //setter = new Setter();
                    //setter.Property = TextBox.BackgroundProperty;
                    //setter.Value = System.Windows.Media.Brushes.Yellow;
                    //style.Setters.Add(setter);
                    //setter = new Setter();
                    //setter.Property = TextBox.SelectionBrushProperty;
                    //setter.Value = System.Windows.Media.Brushes.LightGray;
                    //style.Setters.Add(setter);
                    //setter = new Setter();
                    //setter.Property = TextBox.FontWeightProperty;
                    //setter.Value = FontWeights.Bold;
                    //style.Setters.Add(setter);
                    //setter = new Setter();
                    //setter.Property = TextBox.TextAlignmentProperty;
                    //setter.Value = TextAlignment.Center;
                    //style.Setters.Add(setter);
                    //col.EditingElementStyle = style;
                }
                catch (Exception)
                {

                }
                
            }
        }
        #endregion

        #region RowAlterColor0 RowAlterColor1 隔行变色属性
        public static Brush GetRowAlterColor0(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(RowAlterColor0Property);
        }

        public static void SetRowAlterColor0(this DependencyObject obj, Brush value)
        {
            obj.SetValue(RowAlterColor0Property, value);
        }

        // Using a DependencyProperty as the backing store for RowAlterColor0.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowAlterColor0Property =
            DependencyProperty.RegisterAttached("RowAlterColor0", typeof(Brush), typeof(DataGridHelper), new PropertyMetadata(Brushes.Transparent));



        public static Brush GetRowAlterColor1(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(RowAlterColor1Property);
        }

        public static void SetRowAlterColor1(this DependencyObject obj, Brush value)
        {
            obj.SetValue(RowAlterColor1Property, value);
        }

        // Using a DependencyProperty as the backing store for RowAlterColor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowAlterColor1Property =
            DependencyProperty.RegisterAttached("RowAlterColor1", typeof(Brush), typeof(DataGridHelper), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region RowMouseOverColor 鼠标悬浮时的行颜色
        public static Brush GetRowMouseOverColor(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(RowMouseOverColorProperty);
        }

        public static void SetRowMouseOverColor(this DependencyObject obj, Brush value)
        {
            obj.SetValue(RowMouseOverColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowMouseOverColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowMouseOverColorProperty =
            DependencyProperty.RegisterAttached("RowMouseOverColor", typeof(Brush), typeof(DataGridHelper), new PropertyMetadata(Brushes.Transparent));

        //private static void RowMouseOverColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (d is System.Windows.Controls.DataGrid dGrid && e.NewValue != null)
        //        {
        //            Trigger trigger = new Trigger();
        //            trigger.Property = System.Windows.Controls.DataGridRow.IsMouseOverProperty;
        //            trigger.Value = true;
        //            Setter setter = new Setter(System.Windows.Controls.DataGridRow.BackgroundProperty, e.NewValue as Brush);
        //            trigger.Setters.Add(setter);
        //            if (dGrid.RowStyle == null)
        //                dGrid.RowStyle = new Style(typeof(System.Windows.Controls.DataGridRow));
        //            dGrid.RowStyle.Triggers.Add(trigger);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        #endregion

        #region RowSelectedBackColor 行选中时的背景颜色
        public static Brush GetRowSelectedBackColor(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(RowSelectedBackColorProperty);
        }

        public static void SetRowSelectedBackColor(this DependencyObject obj, Brush value)
        {
            obj.SetValue(RowSelectedBackColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowSelectedBackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowSelectedBackColorProperty =
            DependencyProperty.RegisterAttached("RowSelectedBackColor", typeof(Brush), typeof(DataGridHelper), new PropertyMetadata(Brushes.Transparent));

        //private static void RowSelectedColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (d is System.Windows.Controls.DataGrid dGrid && e.NewValue != null)
        //        {
        //            Trigger trigger = new Trigger();
        //            trigger.Property = System.Windows.Controls.DataGridRow.IsSelectedProperty;
        //            trigger.Value = true;
        //            Setter setter = new Setter(System.Windows.Controls.DataGridRow.BackgroundProperty, e.NewValue as Brush);
        //            trigger.Setters.Add(setter);
        //            if (dGrid.RowStyle == null)
        //                dGrid.RowStyle = new Style(typeof(System.Windows.Controls.DataGridRow));
        //            dGrid.RowStyle.Triggers.Add(trigger);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}
        #endregion

        #region RowSelectedForeColor 行选中时的前景颜色
        public static Brush GetRowSelectedForeColor(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(RowSelectedForeColorProperty);
        }

        public static void SetRowSelectedForeColor(this DependencyObject obj, Brush value)
        {
            obj.SetValue(RowSelectedForeColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowSelectedForeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowSelectedForeColorProperty =
            DependencyProperty.RegisterAttached("RowSelectedForeColor", typeof(Brush), typeof(DataGridHelper), new PropertyMetadata(Brushes.Transparent));
        
        #endregion
    }
}
