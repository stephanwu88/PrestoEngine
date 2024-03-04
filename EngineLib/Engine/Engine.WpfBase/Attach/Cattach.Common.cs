using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Engine.WpfBase
{
    /// <summary>
    /// 公共附加属性
    /// </summary>
    public static partial class Cattach
    {
        /************************************ Attach Property **************************************/

        #region FocusBackground 获得焦点背景色，

        public static readonly DependencyProperty FocusBackgroundProperty = DependencyProperty.RegisterAttached(
            "FocusBackground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static void SetFocusBackground(this DependencyObject element, Brush value)
        {
            element.SetValue(FocusBackgroundProperty, value);
        }

        public static Brush GetFocusBackground(this DependencyObject element)
        {
            return (Brush)element.GetValue(FocusBackgroundProperty);
        }

        #endregion

        #region FocusForeground 获得焦点前景色，

        public static readonly DependencyProperty FocusForegroundProperty = DependencyProperty.RegisterAttached(
            "FocusForeground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static void SetFocusForeground(this DependencyObject element, Brush value)
        {
            element.SetValue(FocusForegroundProperty, value);
        }

        public static Brush GetFocusForeground(this DependencyObject element)
        {
            return (Brush)element.GetValue(FocusForegroundProperty);
        }

        #endregion

        #region MouseOverBackgroundProperty 鼠标悬浮背景色

        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.RegisterAttached(
            "MouseOverBackground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static void SetMouseOverBackground(this DependencyObject element, Brush value)
        {
            element.SetValue(MouseOverBackgroundProperty, value);
        }

        public static Brush MouseOverBackground(this DependencyObject element)
        {
            return (Brush)element.GetValue(MouseOverBackgroundProperty);
        }

        #endregion

        #region MouseOverForegroundProperty 鼠标悬浮前景色

        public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.RegisterAttached(
            "MouseOverForeground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static void SetMouseOverForeground(this DependencyObject element, Brush value)
        {
            element.SetValue(MouseOverForegroundProperty, value);
        }

        public static Brush MouseOverForeground(this DependencyObject element)
        {
            return (Brush)element.GetValue(MouseOverForegroundProperty);
        }

        #endregion

        #region SelectBackgroundProperty 选中背景色

        public static readonly DependencyProperty SelectBackgroundProperty = DependencyProperty.RegisterAttached(
            "SelectBackground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static void SetSelectBackground(this DependencyObject element, Brush value)
        {
            element.SetValue(SelectBackgroundProperty, value);
        }

        public static Brush SelectBackground(this DependencyObject element)
        {
            return (Brush)element.GetValue(SelectBackgroundProperty);
        }

        #endregion

        #region SelectFontSize 选中后字体大小
        /// <summary>
        /// SelectFontSize 选中后字体大小
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetSelectFontSize(this DependencyObject obj)
        {
            return (double)obj.GetValue(SelectFontSizeProperty);
        }

        public static void SetSelectFontSize(this DependencyObject obj, double value)
        {
            obj.SetValue(SelectFontSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectFontSizeProperty =
            DependencyProperty.RegisterAttached("SelectFontSize", typeof(double), typeof(Cattach), new PropertyMetadata(16.0));
        #endregion

        #region MouseOverForegroundProperty 选中前景色

        public static readonly DependencyProperty SelectForegroundProperty = DependencyProperty.RegisterAttached(
            "SelectForeground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static void SetSelectForeground(this DependencyObject element, Brush value)
        {
            element.SetValue(MouseOverForegroundProperty, value);
        }

        public static Brush SelectForeground(this DependencyObject element)
        {
            return (Brush)element.GetValue(SelectForegroundProperty);
        }

        #endregion

        #region MousePressed 鼠标点击时的背景颜色
        /// <summary>
        /// 鼠标点击时的背景颜色
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMousePressedBackground(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(MousePressedBackgroundProperty);
        }

        public static void SetMousePressedBackground(this DependencyObject obj, Brush value)
        {
            obj.SetValue(MousePressedBackgroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for MousePressedBackGround.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePressedBackgroundProperty =
            DependencyProperty.RegisterAttached("MousePressedBackground", typeof(Brush), typeof(Cattach), new PropertyMetadata(null));
        #endregion

        #region PressBackgroundProperty 按下背景色

        public static readonly DependencyProperty PressBackgroundProperty = DependencyProperty.RegisterAttached(
            "PressBackground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static void SetPressBackground(this DependencyObject element, Brush value)
        {
            element.SetValue(PressBackgroundProperty, value);
        }

        public static Brush PressBackground(this DependencyObject element)
        {
            return (Brush)element.GetValue(PressBackgroundProperty);
        }

        #endregion

        #region  PressForegroundProperty 按下前景色

        public static readonly DependencyProperty PressForegroundProperty = DependencyProperty.RegisterAttached(
            "PressForeground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static void SetPressForeground(this DependencyObject element, Brush value)
        {
            element.SetValue(PressForegroundProperty, value);
        }

        public static Brush PressForeground(this DependencyObject element)
        {
            return (Brush)element.GetValue(PressForegroundProperty);
        }

        #endregion

        #region - PressBorderBrushProperty 按下边框颜色 -

        /// <summary>
        /// 按下边框颜色
        /// </summary>
        public static readonly DependencyProperty PressBorderBrushProperty = DependencyProperty.RegisterAttached(
            "PressBorderBrush", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPressBorderBrushChanged));

        public static Brush GetPressBorderBrush(this DependencyObject d)
        {
            return (Brush)d.GetValue(PressBorderBrushProperty);
        }

        public static void SetPressBorderBrush(this DependencyObject obj, Brush value)
        {
            obj.SetValue(PressBorderBrushProperty, value);
        }

        static void OnPressBorderBrushChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        #endregion

        #region FocusBorderBrush 焦点边框色，输入控件

        public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached(
            "FocusBorderBrush", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(null));
        public static void SetFocusBorderBrush(this DependencyObject element, Brush value)
        {
            element.SetValue(FocusBorderBrushProperty, value);
        }
        public static Brush GetFocusBorderBrush(this DependencyObject element)
        {
            return (Brush)element.GetValue(FocusBorderBrushProperty);
        }

        #endregion

        #region SelectedForegroundBrush 选中前景色

        public static readonly DependencyProperty SelectedForegroundBrushProperty = DependencyProperty.RegisterAttached(
            "SelectedForegroundBrush", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(Brushes.Transparent));
        public static void SetSelectedForegroundBrush(this DependencyObject element, Brush value)
        {
            element.SetValue(SelectedForegroundBrushProperty, value);
        }
        public static Brush GetSelectedForegroundBrush(this DependencyObject element)
        {
            return (Brush)element.GetValue(SelectedForegroundBrushProperty);
        }

        #endregion

        #region SelectedBackgroundBrush 选中背景色

        public static readonly DependencyProperty SelectedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
            "SelectedBackgroundBrush", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(Brushes.Transparent));
        public static void SetSelectedBackgroundBrush(this DependencyObject element, Brush value)
        {
            element.SetValue(SelectedBackgroundBrushProperty, value);
        }
        public static Brush GetSelectedBackgroundBrush(this DependencyObject element)
        {
            return (Brush)element.GetValue(SelectedBackgroundBrushProperty);
        }

        #endregion

        #region CheckedForegroundBrush 选中前景色

        public static readonly DependencyProperty CheckedForegroundBrushProperty = DependencyProperty.RegisterAttached(
            "CheckedForegroundBrush", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static void SetCheckedForegroundBrush(this DependencyObject element, Brush value)
        {
            element.SetValue(CheckedForegroundBrushProperty, value);
        }

        public static Brush GetCheckedForegroundBrush(this DependencyObject element)
        {
            return (Brush)element.GetValue(CheckedForegroundBrushProperty);
        }

        #endregion

        #region CheckedBackgroundBrush 选中背景色

        public static readonly DependencyProperty CheckedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
            "CheckedBackgroundBrush", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static void SetCheckedBackgroundBrush(this DependencyObject element, Brush value)
        {
            element.SetValue(CheckedBackgroundBrushProperty, value);
        }

        public static Brush GetCheckedBackgroundBrush(this DependencyObject element)
        {
            return (Brush)element.GetValue(CheckedBackgroundBrushProperty);
        }

        #endregion

        #region MouseOverBorderBrush 鼠标进入边框色，输入控件

        public static readonly DependencyProperty MouseOverBorderBrushProperty =
            DependencyProperty.RegisterAttached("MouseOverBorderBrush", typeof(Brush), typeof(Cattach),
                new FrameworkPropertyMetadata(Brushes.Transparent,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Sets the brush used to draw the mouse over brush.
        /// </summary>
        public static void SetMouseOverBorderBrush(this DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBorderBrushProperty, value);
        }

        /// <summary>
        /// Gets the brush used to draw the mouse over brush.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(RichTextBox))]
        public static Brush GetMouseOverBorderBrush(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBorderBrushProperty);
        }

        #endregion

        #region MouseOverBorderThickness 鼠标进入后边框厚度
        /// <summary>
        /// MouseOverBorderThickness
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Thickness GetMouseOverBorderThickness(this DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MouseOverBorderThicknessProperty);
        }

        public static void SetMouseOverBorderThickness(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(MouseOverBorderThicknessProperty, value);
        }

        // Using a DependencyProperty as the backing store for MouseOverBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBorderThicknessProperty =
            DependencyProperty.RegisterAttached("MouseOverBorderThickness", typeof(Thickness), typeof(Cattach), new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        #endregion

        #region MouseOverFontSize 鼠标进入后字体大小
        /// <summary>
        /// MouseOverFontSize
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetMouseOverFontSize(this DependencyObject obj)
        {
            return (double)obj.GetValue(MouseOverFontSizeProperty);
        }

        public static void SetMouseOverFontSize(this DependencyObject obj, double value)
        {
            obj.SetValue(MouseOverFontSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for MouseOverFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverFontSizeProperty =
            DependencyProperty.RegisterAttached("MouseOverFontSize", typeof(double), typeof(Cattach), new PropertyMetadata(15.0));
        #endregion

        #region MouseOverOpacity 鼠标进入后 透明度
        /// <summary>
        /// MouseOverOpacity 鼠标进入后 透明度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetMouseOverOpacity(this DependencyObject obj)
        {
            return (double)obj.GetValue(MouseOverOpacityProperty);
        }

        public static void SetMouseOverOpacity(this DependencyObject obj, double value)
        {
            obj.SetValue(MouseOverOpacityProperty, value);
        }

        // Using a DependencyProperty as the backing store for MouseOverOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverOpacityProperty =
            DependencyProperty.RegisterAttached("MouseOverOpacity", typeof(double), typeof(Cattach), new PropertyMetadata(1.0));
        #endregion

        #region BorderBrush 边框颜色
        /// <summary>
        /// BorderBrush 边框颜色
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetBorderBrush(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(BorderBrushProperty);
        }

        public static void SetBorderBrush(this DependencyObject obj, Brush value)
        {
            obj.SetValue(BorderBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(Cattach), new PropertyMetadata(Brushes.Transparent));
        #endregion


        #region ShowMarker 是否显示标记
        /// <summary>
        /// ShowMarker 是否显示标记
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetShowMarker(this DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowMarkerProperty);
        }

        public static void SetShowMarker(this DependencyObject obj, bool value)
        {
            obj.SetValue(ShowMarkerProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowMarker.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMarkerProperty =
            DependencyProperty.RegisterAttached("ShowMarker", typeof(bool), typeof(Cattach), new PropertyMetadata(false));
        #endregion

        #region MarkerMargin 标记框 位置
        /// <summary>
        /// MarkerMargin
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Thickness GetMarkerMargin(this DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MarkerMarginProperty);
        }

        public static void SetMarkerMargin(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarkerMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerMarginProperty =
            DependencyProperty.RegisterAttached("MarkerMargin", typeof(Thickness), typeof(Cattach), new PropertyMetadata(new Thickness(0,0,0,0)));
        #endregion

        #region MarkerThickness 标记框 边框
        /// <summary>
        /// MarkerThickness
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Thickness GetMarkerThickness(this DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MarkerThicknessProperty);
        }

        public static void SetMarkerThickness(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarkerThicknessProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerThicknessProperty =
            DependencyProperty.RegisterAttached("MarkerThickness", typeof(Thickness), typeof(Cattach), new PropertyMetadata(new Thickness(0,0,0,0)));
        #endregion

        #region MarkerWidth 标记框 宽度
        /// <summary>
        /// MarkerWidth
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetMarkerWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(MarkerWidthProperty);
        }

        public static void SetMarkerWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(MarkerWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerWidthProperty =
            DependencyProperty.RegisterAttached("MarkerWidth", typeof(double), typeof(Cattach), new PropertyMetadata(double.NaN));
        #endregion

        #region MarkerHeight 标记框 高度
        /// <summary>
        /// MarkerHeight
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetMarkerHeight(this DependencyObject obj)
        {
            return (double)obj.GetValue(MarkerHeightProperty);
        }

        public static void SetMarkerHeight(this DependencyObject obj, double value)
        {
            obj.SetValue(MarkerHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerHeightProperty =
            DependencyProperty.RegisterAttached("MarkerHeight", typeof(double), typeof(Cattach), new PropertyMetadata(double.NaN));
        #endregion

        #region MarkerMaxWidth 标记框最大宽度
        /// <summary>
        /// MarkerMaxWidth 标记框最大宽度
        /// 默认值 double.NaN 类似于Auto
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetMarkerMaxWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(MarkerMaxWidthProperty);
        }

        public static void SetMarkerMaxWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(MarkerMaxWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerMaxWidthProperty =
            DependencyProperty.RegisterAttached("MarkerMaxWidth", typeof(double), typeof(Cattach), new PropertyMetadata(double.NaN));
        #endregion


        #region MarkerBorderBrush 标记框 边框颜色
        /// <summary>
        /// MarkerBrush
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMarkerBorderBrush(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(MarkerBorderBrushProperty);
        }

        public static void SetMarkerBorderBrush(this DependencyObject obj, Brush value)
        {
            obj.SetValue(MarkerBorderBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerBorderBrushProperty =
            DependencyProperty.RegisterAttached("MarkerBorderBrush", typeof(Brush), typeof(Cattach), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region MarkerBackground 标记框 背景色/填充色
        /// <summary>
        /// MarkerBackground
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMarkerBackground(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(MarkerBackgroundProperty);
        }

        public static void SetMarkerBackground(this DependencyObject obj, Brush value)
        {
            obj.SetValue(MarkerBackgroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerBackgroun.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerBackgroundProperty =
            DependencyProperty.RegisterAttached("MarkerBackground", typeof(Brush), typeof(Cattach), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region MarkerCornerRadius 标记框 圆角
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CornerRadius GetMarkerCornerRadius(this DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(MarkerCornerRadiusProperty);
        }

        public static void SetMarkerCornerRadius(this DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(MarkerCornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerCornerRadiusProperty =
            DependencyProperty.RegisterAttached("MarkerCornerRadius", typeof(CornerRadius), typeof(Cattach), new PropertyMetadata(new CornerRadius(0,0,0,0)));
        #endregion

        #region MarkerPadding 标记框 内部定位
        /// <summary>
        /// MarkerPadding 标记框 内部定位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Thickness GetMarkerPadding(this DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MarkerPaddingProperty);
        }

        public static void SetMarkerPadding(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarkerPaddingProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerPaddingProperty =
            DependencyProperty.RegisterAttached("MarkerPadding", typeof(Thickness), typeof(Cattach), new PropertyMetadata(new Thickness(0,0,0,0)));
        #endregion

        #region MarkerFontSize 标记块 字体
        /// <summary>
        /// 标记块 字体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetMarkerFontSize(this DependencyObject obj)
        {
            return (double)obj.GetValue(MarkerFontSizeProperty);
        }

        public static void SetMarkerFontSize(this DependencyObject obj, double value)
        {
            obj.SetValue(MarkerFontSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for MarkerFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerFontSizeProperty =
            DependencyProperty.RegisterAttached("MarkerFontSize", typeof(double), typeof(Cattach), new PropertyMetadata(0.0));
        #endregion


        #region CutomBorderWidth 自定义边框宽度
        /// <summary>
        /// CutomBorderWidth 自定义边框宽度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetCutomBorderWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(CutomBorderWidthProperty);
        }

        public static void SetCutomBorderWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(CutomBorderWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for CutomBorderWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CutomBorderWidthProperty =
            DependencyProperty.RegisterAttached("CutomBorderWidth", typeof(double), typeof(Cattach), new PropertyMetadata(0.0));
        #endregion

        #region CutomBorderWidth 自定义边框高度
        /// <summary>
        /// CutomBorderWidth 自定义边框高度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetCustomBorderHeight(this DependencyObject obj)
        {
            return (double)obj.GetValue(CustomBorderHeightProperty);
        }

        public static void SetCustomBorderHeight(this DependencyObject obj, double value)
        {
            obj.SetValue(CustomBorderHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for CustomBorderHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomBorderHeightProperty =
            DependencyProperty.RegisterAttached("CustomBorderHeight", typeof(double), typeof(Cattach), new PropertyMetadata(0.0));
        #endregion

        #region CustomBorderCornerRadius 自定义边框圆角
        /// <summary>
        /// CustomBorderCornerRadius 自定义边框圆角
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CornerRadius GetCustomBorderCornerRadius(this DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CustomBorderCornerRadiusProperty);
        }

        public static void SetCustomBorderCornerRadius(this DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CustomBorderCornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CustomBorderCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomBorderCornerRadiusProperty =
            DependencyProperty.RegisterAttached("CustomBorderCornerRadius", typeof(CornerRadius), typeof(Cattach), new PropertyMetadata(new CornerRadius(0,0,0,0)));
        #endregion

        #region CustomBorderBackground 自定义边框背景色
        /// <summary>
        /// CustomBorderBackground 自定义边框背景色
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetCustomBorderBackground(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(CustomBorderBackgroundProperty);
        }

        public static void SetCustomBorderBackground(this DependencyObject obj, Brush value)
        {
            obj.SetValue(CustomBorderBackgroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for CustomBorderBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomBorderBackgroundProperty =
            DependencyProperty.RegisterAttached("CustomBorderBackground", typeof(Brush), typeof(Cattach), new PropertyMetadata(Brushes.Transparent));
        #endregion


        #region AttachContentProperty 附加组件模板
        /// <summary>
        /// 附加组件模板
        /// </summary>
        public static readonly DependencyProperty AttachContentProperty = DependencyProperty.RegisterAttached(
            "AttachContent", typeof(ControlTemplate), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static ControlTemplate GetAttachContent(this DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(AttachContentProperty);
        }

        public static void SetAttachContent(this DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(AttachContentProperty, value);
        }
        #endregion

        #region WatermarkProperty 水印
        /// <summary>
        /// 水印
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
            "Watermark", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(""));

        public static string GetWatermark(this DependencyObject d)
        {
            return (string)d.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(this DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }
        #endregion

        #region ShowImage 显示图片
        /// <summary>
        /// ShowImage 显示图片
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ImageSource GetShowImage(this DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ShowImageProperty);
        }

        public static void SetShowImage(this DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ShowImageProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowImageProperty =
            DependencyProperty.RegisterAttached("ShowImage", typeof(ImageSource), typeof(Cattach), new PropertyMetadata(null));
        #endregion

        #region MouseOverImage 鼠标悬浮图片
        /// <summary>
        /// MouseOverImage 鼠标悬浮图片
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ImageSource GetMouseOverImage(this DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(MouseOverImageProperty);
        }

        public static void SetMouseOverImage(this DependencyObject obj, ImageSource value)
        {
            obj.SetValue(MouseOverImageProperty, value);
        }

        // Using a DependencyProperty as the backing store for MouseOverImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverImageProperty =
            DependencyProperty.RegisterAttached("MouseOverImage", typeof(ImageSource), typeof(Cattach), new PropertyMetadata(null));
        #endregion

        #region MousePressedImage 鼠标按下图片
        /// <summary>
        /// MousePressedImage 鼠标按下图片
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ImageSource GetMousePressedImage(this DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(MousePressedImageProperty);
        }

        public static void SetMousePressedImage(this DependencyObject obj, ImageSource value)
        {
            obj.SetValue(MousePressedImageProperty, value);
        }

        // Using a DependencyProperty as the backing store for MousePressedImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePressedImageProperty =
            DependencyProperty.RegisterAttached("MousePressedImage", typeof(ImageSource), typeof(Cattach), new PropertyMetadata(null));
        #endregion

        #region SelectedImage 选中后图片
        /// <summary>
        /// SelectedImage 选中后图片
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ImageSource GetSelectedImage(this DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(SelectedImageProperty);
        }

        public static void SetSelectedImage(this DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SelectedImageProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedImageProperty =
            DependencyProperty.RegisterAttached("SelectedImage", typeof(ImageSource), typeof(Cattach), new PropertyMetadata(null));
        #endregion

        #region UnSelectedImage 未选中时图片
        /// <summary>
        /// UnSelectedImage 未选中时图片
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ImageSource GetUnSelectedImage(this DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(UnSelectedImageProperty);
        }

        public static void SetUnSelectedImage(this DependencyObject obj, ImageSource value)
        {
            obj.SetValue(UnSelectedImageProperty, value);
        }

        // Using a DependencyProperty as the backing store for UnSelectedImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnSelectedImageProperty =
            DependencyProperty.RegisterAttached("UnSelectedImage", typeof(ImageSource), typeof(Cattach), new PropertyMetadata(null));
        #endregion

        #region ImageWidth 图片宽度
        /// <summary>
        /// ImageWidth 图片宽度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetImageWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(ImageWidthProperty);
        }

        public static void SetImageWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(ImageWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.RegisterAttached("ImageWidth", typeof(double), typeof(Cattach), new PropertyMetadata(double.NaN));
        #endregion

        #region ImageHeight 图片高度
        /// <summary>
        /// ImageHeight 图片高度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetImageHeight(this DependencyObject obj)
        {
            return (double)obj.GetValue(ImageHeightProperty);
        }

        public static void SetImageHeight(this DependencyObject obj, double value)
        {
            obj.SetValue(ImageHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.RegisterAttached("ImageHeight", typeof(double), typeof(Cattach), new PropertyMetadata(double.NaN));
        #endregion

        #region ImageMargin 图片 定位
        /// <summary>
        /// ImageMargin 图片 定位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Thickness GetImageMargin(this DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ImageMarginProperty);
        }

        public static void SetImageMargin(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(ImageMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.RegisterAttached("ImageMargin", typeof(Thickness), typeof(Cattach), new PropertyMetadata(new Thickness(0,0,0,0)));
        #endregion


        #region FIconProperty 字体图标

        /// <summary>
        /// 字体图标
        /// </summary>
        public static readonly DependencyProperty FIconProperty = DependencyProperty.RegisterAttached(
            "FIcon", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(""));

        public static string GetFIcon(this DependencyObject d)
        {
            return (string)d.GetValue(FIconProperty);
        }

        public static void SetFIcon(this DependencyObject obj, string value)
        {
            obj.SetValue(FIconProperty, value);
        }

        /// <summary>
        /// 字体图标
        /// </summary>
        public static readonly DependencyProperty FIconChangedProperty = DependencyProperty.RegisterAttached(
            "FIconChanged", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(""));

        public static string GetFIconChanged(this DependencyObject d)
        {
            return (string)d.GetValue(FIconChangedProperty);
        }

        public static void SetFIconChanged(this DependencyObject obj, string value)
        {
            obj.SetValue(FIconChangedProperty, value);
        }

        #endregion

        #region FIconSizeProperty 字体图标大小
        /// <summary>
        /// 字体图标
        /// </summary>
        public static readonly DependencyProperty FIconSizeProperty = DependencyProperty.RegisterAttached(
            "FIconSize", typeof(double), typeof(Cattach), new FrameworkPropertyMetadata(12D));

        public static double GetFIconSize(this DependencyObject d)
        {
            return (double)d.GetValue(FIconSizeProperty);
        }

        public static void SetFIconSize(this DependencyObject obj, double value)
        {
            obj.SetValue(FIconSizeProperty, value);
        }
        #endregion

        #region FIconMarginProperty 字体图标边距
        /// <summary>
        /// 字体图标
        /// </summary>
        public static readonly DependencyProperty FIconMarginProperty = DependencyProperty.RegisterAttached(
            "FIconMargin", typeof(Thickness), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static Thickness GetFIconMargin(this DependencyObject d)
        {
            return (Thickness)d.GetValue(FIconMarginProperty);
        }

        public static void SetFIconMargin(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(FIconMarginProperty, value);
        }
        #endregion

        #region FIconColor 图标颜色
        /// <summary>
        /// FIconColor 图标颜色
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetFIconColor(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(FIconColorProperty);
        }

        public static void SetFIconColor(this DependencyObject obj, Brush value)
        {
            obj.SetValue(FIconColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for FIconColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconColorProperty =
            DependencyProperty.RegisterAttached("FIconColor", typeof(Brush), typeof(Cattach), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region CommentText 描述文字
        /// <summary>
        /// CommentText 描述文字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetCommentText(this DependencyObject obj)
        {
            return (string)obj.GetValue(CommentTextProperty);
        }

        public static void SetCommentText(this DependencyObject obj, string value)
        {
            obj.SetValue(CommentTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for CommentText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentTextProperty =
            DependencyProperty.RegisterAttached("CommentText", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region UnitText 单位文字
        /// <summary>
        /// UnitText 单位文字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetUnitText(this DependencyObject obj)
        {
            return (string)obj.GetValue(UnitTextProperty);
        }

        public static void SetUnitText(this DependencyObject obj, string value)
        {
            obj.SetValue(UnitTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for UnitText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitTextProperty =
            DependencyProperty.RegisterAttached("UnitText", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region CommentWidth 说明字段宽度
        /// <summary>
        /// CommentWidth 说明字段宽度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetCommentWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(CommentWidthProperty);
        }

        public static void SetCommentWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(CommentWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for CommentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentWidthProperty =
            DependencyProperty.RegisterAttached("CommentWidth", typeof(double), typeof(Cattach), new PropertyMetadata(double.NaN));
        #endregion

        #region UnitWidth 单位宽度
        /// <summary>
        /// UnitWidth 单位宽度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetUnitWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(UnitWidthProperty);
        }

        public static void SetUnitWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(UnitWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for UnitWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitWidthProperty =
            DependencyProperty.RegisterAttached("UnitWidth", typeof(double), typeof(Cattach), new PropertyMetadata(double.NaN));
        #endregion

        #region ContentWidth 内容宽度
        /// <summary>
        /// ContentWidth 内容宽度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetContentWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(ContentWidthProperty);
        }

        public static void SetContentWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(ContentWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.RegisterAttached("ContentWidth", typeof(double), typeof(Cattach), new PropertyMetadata(double.NaN));
        #endregion

        #region ContentMargin 内容外部定位
        /// <summary>
        /// ContentMargin 内容定位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Thickness GetContentMargin(this DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ContentMarginProperty);
        }

        public static void SetContentMargin(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(ContentMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.RegisterAttached("ContentMargin", typeof(Thickness), typeof(Cattach), new PropertyMetadata(new Thickness(0,0,0,0)));
        #endregion

        #region ContentPadding 内容内部定位
        /// <summary>
        /// Content Padding
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Thickness GetContentPadding(this DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ContentPaddingProperty);
        }

        public static void SetContentPadding(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(ContentPaddingProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.RegisterAttached("ContentPadding", typeof(Thickness), typeof(Cattach), new PropertyMetadata(new Thickness(0,0,0,0)));
        #endregion

        #region ContentText 内容文字
        /// <summary>
        /// ContentText 内容文字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetContentText(this DependencyObject obj)
        {
            return (string)obj.GetValue(ContentTextProperty);
        }

        public static void SetContentText(this DependencyObject obj, string value)
        {
            obj.SetValue(ContentTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.RegisterAttached("ContentText", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region ContentTextBrush 内容文字颜色 
        /// <summary>
        /// ContentTextBrush 内容文字颜色
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetContentTextBrush(this DependencyObject obj)
        {
            return (Brush)obj.GetValue(ContentTextBrushProperty);
        }

        public static void SetContentTextBrush(this DependencyObject obj, Brush value)
        {
            obj.SetValue(ContentTextBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentTextBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTextBrushProperty =
            DependencyProperty.RegisterAttached("ContentTextBrush", typeof(Brush), typeof(Cattach), new PropertyMetadata(Brushes.Black));
        #endregion

        #region ContentTextFontSize 内容文字字体大小
        /// <summary>
        /// ContentTextFontSize 内容文字字体大小
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetContentTextFontSize(this DependencyObject obj)
        {
            return (double)obj.GetValue(ContentTextFontSizeProperty);
        }

        public static void SetContentTextFontSize(this DependencyObject obj, double value)
        {
            obj.SetValue(ContentTextFontSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentTextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTextFontSizeProperty =
            DependencyProperty.RegisterAttached("ContentTextFontSize", typeof(double), typeof(Cattach), new PropertyMetadata(12.0));
        #endregion

        #region ContentError 内容有错误
        /// <summary>
        /// ContentError 内容有错误
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetContentError(this DependencyObject obj)
        {
            return (bool)obj.GetValue(ContentErrorProperty);
        }

        public static void SetContentError(this DependencyObject obj, bool value)
        {
            obj.SetValue(ContentErrorProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentErrorProperty =
            DependencyProperty.RegisterAttached("ContentError", typeof(bool), typeof(Cattach), new PropertyMetadata(false));
        #endregion

        #region ContentErrorMessage 内容错误消息
        /// <summary>
        /// ContentErrorMessage 内容错误消息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetContentErrorMessage(this DependencyObject obj)
        {
            return (string)obj.GetValue(ContentErrorMessageProperty);
        }

        public static void SetContentErrorMessage(this DependencyObject obj, string value)
        {
            obj.SetValue(ContentErrorMessageProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentErrorMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentErrorMessageProperty =
            DependencyProperty.RegisterAttached("ContentErrorMessage", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion


        #region AllowsAnimationProperty 启用旋转动画
        /// <summary>
        /// 启用旋转动画
        /// </summary>
        public static readonly DependencyProperty AllowsAnimationProperty = DependencyProperty.RegisterAttached("AllowsAnimation"
            , typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(false, AllowsAnimationChanged));

        public static bool GetAllowsAnimation(this DependencyObject d)
        {
            return (bool)d.GetValue(AllowsAnimationProperty);
        }

        public static void SetAllowsAnimation(this DependencyObject obj, bool value)
        {
            obj.SetValue(AllowsAnimationProperty, value);
        }

        /// <summary>
        /// 旋转动画刻度
        /// </summary>
        private static DoubleAnimation RotateAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(200)));

        /// <summary>
        /// 绑定动画事件
        /// </summary>
        private static void AllowsAnimationChanged(this DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var uc = d as FrameworkElement;
            //if (uc == null) return;
            //if (uc.RenderTransformOrigin == new Point(0, 0))
            //{
            //    uc.RenderTransformOrigin = new Point(0.5, 0.5);
            //    RotateTransform trans = new RotateTransform(0);
            //    uc.RenderTransform = trans;
            //}
            //var value = (bool)e.NewValue;
            //if (value)
            //{
            //    RotateAnimation.To = 180;
            //    uc.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
            //}
            //else
            //{
            //    RotateAnimation.To = 0;
            //    uc.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
            //}
        }
        #endregion

        #region CornerRadiusProperty Border圆角
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
            "CornerRadius", typeof(CornerRadius), typeof(Cattach), new FrameworkPropertyMetadata(null));

        public static CornerRadius GetCornerRadius(this DependencyObject d)
        {
            return (CornerRadius)d.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(this DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region DoubleAttach 类型的附加属性 Tag
        /// <summary>
        /// double 类型的附加属性
        /// </summary>
        public static readonly DependencyProperty DoubleAttachProperty = DependencyProperty.RegisterAttached(
            "DoubleAttach", typeof(double), typeof(Cattach), new FrameworkPropertyMetadata(0.0));

        public static double GetDoubleAttach(this DependencyObject d)
        {
            return (double)d.GetValue(DoubleAttachProperty);
        }

        public static void SetDoubleAttach(this DependencyObject obj, double value)
        {
            obj.SetValue(DoubleAttachProperty, value);
        }
        #endregion

        #region LabelProperty TextBox的头部Label
        /// <summary>
        /// TextBox的头部Label
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached(
            "Label", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static string GetLabel(this DependencyObject d)
        {
            return (string)d.GetValue(LabelProperty);
        }

        public static void SetLabel(this DependencyObject obj, string value)
        {
            obj.SetValue(LabelProperty, value);
        }
        #endregion

        #region LabelTemplateProperty TextBox的头部Label模板
        /// <summary>
        /// TextBox的头部Label模板
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.RegisterAttached(
            "LabelTemplate", typeof(ControlTemplate), typeof(Cattach), new FrameworkPropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static ControlTemplate GetLabelTemplate(this DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(LabelTemplateProperty);
        }

        public static void SetLabelTemplate(this DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(LabelTemplateProperty, value);


        }
        #endregion

        #region GridLength 网格占用大小 - 左
        /// <summary>
        /// 网格占用大小 - 左
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GridLength GetLeftGridLength(this DependencyObject obj)
        {
            return (GridLength)obj.GetValue(LeftGridLengthProperty);
        }

        public static void SetLeftGridLength(this DependencyObject obj, GridLength value)
        {
            obj.SetValue(LeftGridLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for LeftGridLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftGridLengthProperty =
            DependencyProperty.RegisterAttached("LeftGridLength", typeof(GridLength), typeof(Cattach), new PropertyMetadata(GridLength.Auto));
        #endregion

        #region GridLength 网格占用大小 - 右
        /// <summary>
        /// 网格占用大小 - 右
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GridLength GetRightGridLength(this DependencyObject obj)
        {
            return (GridLength)obj.GetValue(RightGridLengthProperty);
        }

        public static void SetRightGridLength(this DependencyObject obj, GridLength value)
        {
            obj.SetValue(RightGridLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for RightGridLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightGridLengthProperty =
            DependencyProperty.RegisterAttached("RightGridLength", typeof(GridLength), typeof(Cattach), new PropertyMetadata(GridLength.Auto));
        #endregion

        #region GridLength 网格占用大小 - 中间
        /// <summary>
        /// 网格占用大小 - 中间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GridLength GetCenterGridLength(this DependencyObject obj)
        {
            return (GridLength)obj.GetValue(CenterGridLengthProperty);
        }

        public static void SetCenterGridLength(this DependencyObject obj, GridLength value)
        {
            obj.SetValue(CenterGridLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for CenterGridLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterGridLengthProperty =
            DependencyProperty.RegisterAttached("CenterGridLength", typeof(GridLength), typeof(Cattach), new PropertyMetadata(GridLength.Auto));
        #endregion

        #region GridLength 网格占用大小 - 上
        /// <summary>
        /// 网格占用大小 - 上
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GridLength GetUpGridLength(this DependencyObject obj)
        {
            return (GridLength)obj.GetValue(UpGridLengthProperty);
        }

        public static void SetUpGridLength(this DependencyObject obj, GridLength value)
        {
            obj.SetValue(UpGridLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for UpGridLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpGridLengthProperty =
            DependencyProperty.RegisterAttached("UpGridLength", typeof(GridLength), typeof(Cattach), new PropertyMetadata(GridLength.Auto));
        #endregion

        #region GridLength 网格占用大小 - 下
        /// <summary>
        /// 网格占用大小 - 下
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GridLength GetDownGridLength(this DependencyObject obj)
        {
            return (GridLength)obj.GetValue(DownGridLengthProperty);
        }

        public static void SetDownGridLength(this DependencyObject obj, GridLength value)
        {
            obj.SetValue(DownGridLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for DownGridLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DownGridLengthProperty =
            DependencyProperty.RegisterAttached("DownGridLength", typeof(GridLength), typeof(Cattach), new PropertyMetadata(GridLength.Auto));
        #endregion

        #region DataTemplate 数据模板
        /// <summary>
        /// DataTemplate 数据模板
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DataTemplate GetDataTemplate(this DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DataTemplateProperty);
        }

        public static void SetDataTemplate(this DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DataTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for DataTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataTemplateProperty =
            DependencyProperty.RegisterAttached("DataTemplate", typeof(DataTemplate), typeof(Cattach), new PropertyMetadata(null));
        #endregion


        #region HorizontalAlignment 水平对齐方式
        /// <summary>
        /// HorizontalAlignment 水平对齐方式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HorizontalAlignment GetHorizontalAlignment(this DependencyObject obj)
        {
            return (HorizontalAlignment)obj.GetValue(HorizontalAlignmentProperty);
        }

        public static void SetHorizontalAlignment(this DependencyObject obj, HorizontalAlignment value)
        {
            obj.SetValue(HorizontalAlignmentProperty, value);
        }

        // Using a DependencyProperty as the backing store for HorizontalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.RegisterAttached("HorizontalAlignment", typeof(HorizontalAlignment), typeof(Cattach), new PropertyMetadata(HorizontalAlignment.Left));
        #endregion

        #region VerticalAlignment 垂直对齐方式
        /// <summary>
        /// VerticalAlignment 垂直对齐方式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static VerticalAlignment GetVerticalAlignment(this DependencyObject obj)
        {
            return (VerticalAlignment)obj.GetValue(VerticalAlignmentProperty);
        }

        public static void SetVerticalAlignment(this DependencyObject obj, VerticalAlignment value)
        {
            obj.SetValue(VerticalAlignmentProperty, value);
        }

        // Using a DependencyProperty as the backing store for VerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.RegisterAttached("VerticalAlignment", typeof(VerticalAlignment), typeof(Cattach), new PropertyMetadata(VerticalAlignment.Top));
        #endregion


        /************************************ RoutedUICommand Behavior enable **************************************/

        #region IsClearTextButtonBehaviorEnabledProperty 清除输入框Text值按钮行为开关（设为ture时才会绑定事件）
        /// <summary>
        /// 清除输入框Text值按钮行为开关
        /// </summary>
        public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsClearTextButtonBehaviorEnabled"
            , typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetIsClearTextButtonBehaviorEnabled(this DependencyObject d)
        {
            return (bool)d.GetValue(IsClearTextButtonBehaviorEnabledProperty);
        }

        public static void SetIsClearTextButtonBehaviorEnabled(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsClearTextButtonBehaviorEnabledProperty, value);
        }

        /// <summary>
        /// 绑定清除Text操作的按钮事件
        /// </summary>
        private static void IsClearTextButtonBehaviorEnabledChanged(this DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (e.OldValue != e.NewValue && button != null)
            {
                button.CommandBindings.Add(ClearTextCommandBinding);
            }
        }

        #endregion

        #region 设置后面的描述信息
        /// <summary>
        /// 详细信息
        /// </summary>
        public static readonly DependencyProperty DetailProperty = DependencyProperty.RegisterAttached(
            "Detail", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(""));

        public static string GetDetail(this DependencyObject d)
        {
            return (string)d.GetValue(DetailProperty);
        }

        public static void SetDetail(this DependencyObject obj, string value)
        {
            obj.SetValue(DetailProperty, value);
        }

        #endregion

        #region IsOpenFileButtonBehaviorEnabledProperty 选择文件命令行为开关
        /// <summary>
        /// 选择文件命令行为开关
        /// </summary>
        public static readonly DependencyProperty IsOpenFileButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsOpenFileButtonBehaviorEnabled"
            , typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(false, IsOpenFileButtonBehaviorEnabledChanged));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetIsOpenFileButtonBehaviorEnabled(this DependencyObject d)
        {
            return (bool)d.GetValue(IsOpenFileButtonBehaviorEnabledProperty);
        }

        public static void SetIsOpenFileButtonBehaviorEnabled(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenFileButtonBehaviorEnabledProperty, value);
        }

        private static void IsOpenFileButtonBehaviorEnabledChanged(this DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (e.OldValue != e.NewValue && button != null)
            {
                button.CommandBindings.Add(OpenFileCommandBinding);
            }
        }

        #endregion

        #region IsOpenFolderButtonBehaviorEnabledProperty 选择文件夹命令行为开关
        /// <summary>
        /// 选择文件夹命令行为开关
        /// </summary>
        public static readonly DependencyProperty IsOpenFolderButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsOpenFolderButtonBehaviorEnabled"
            , typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(false, IsOpenFolderButtonBehaviorEnabledChanged));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetIsOpenFolderButtonBehaviorEnabled(this DependencyObject d)
        {
            return (bool)d.GetValue(IsOpenFolderButtonBehaviorEnabledProperty);
        }

        public static void SetIsOpenFolderButtonBehaviorEnabled(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenFolderButtonBehaviorEnabledProperty, value);
        }

        private static void IsOpenFolderButtonBehaviorEnabledChanged(this DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (e.OldValue != e.NewValue && button != null)
            {
                button.CommandBindings.Add(OpenFolderCommandBinding);
            }
        }

        #endregion

        #region IsSaveFileButtonBehaviorEnabledProperty 选择文件保存路径及名称
        /// <summary>
        /// 选择文件保存路径及名称
        /// </summary>
        public static readonly DependencyProperty IsSaveFileButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsSaveFileButtonBehaviorEnabled"
            , typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(false, IsSaveFileButtonBehaviorEnabledChanged));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetIsSaveFileButtonBehaviorEnabled(this DependencyObject d)
        {
            return (bool)d.GetValue(IsSaveFileButtonBehaviorEnabledProperty);
        }

        public static void SetIsSaveFileButtonBehaviorEnabled(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsSaveFileButtonBehaviorEnabledProperty, value);
        }

        private static void IsSaveFileButtonBehaviorEnabledChanged(this DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (e.OldValue != e.NewValue && button != null)
            {
                button.CommandBindings.Add(SaveFileCommandBinding);
            }
        }

        #endregion

        /************************************ RoutedUICommand **************************************/

        #region ClearTextCommand 清除输入框Text事件命令

        /// <summary>
        /// 清除输入框Text事件命令，需要使用IsClearTextButtonBehaviorEnabledChanged绑定命令
        /// </summary>
        public static RoutedUICommand ClearTextCommand { get; private set; }

        /// <summary>
        /// ClearTextCommand绑定事件
        /// </summary>
        private static readonly CommandBinding ClearTextCommandBinding;

        /// <summary>
        /// 清除输入框文本值
        /// </summary>
        private static void ClearButtonClick(object sender, ExecutedRoutedEventArgs e)
        {
            var tbox = e.Parameter as FrameworkElement;
            if (tbox == null) return;
            if (tbox is TextBox)
            {
                ((TextBox)tbox).Clear();
            }
            if (tbox is PasswordBox)
            {
                ((PasswordBox)tbox).Clear();
            }
            if (tbox is ComboBox)
            {
                var cb = tbox as ComboBox;
                cb.SelectedItem = null;
                cb.Text = string.Empty;
            }

            if (tbox is DatePicker)
            {
                var dp = tbox as DatePicker;
                dp.SelectedDate = null;
                dp.Text = string.Empty;
            }
            tbox.Focus();
        }

        #endregion

        #region OpenFileCommand 选择文件命令

        /// <summary>
        /// 选择文件命令，需要使用IsClearTextButtonBehaviorEnabledChanged绑定命令
        /// </summary>
        public static RoutedUICommand OpenFileCommand { get; private set; }

        /// <summary>
        /// OpenFileCommand绑定事件
        /// </summary>
        private static readonly CommandBinding OpenFileCommandBinding;

        /// <summary>
        /// 执行OpenFileCommand
        /// </summary>
        private static void OpenFileButtonClick(object sender, ExecutedRoutedEventArgs e)
        {
            var tbox = e.Parameter as FrameworkElement;
            var txt = tbox as TextBox;
            string filter = txt.Tag == null ? "所有文件(*.*)|*.*" : txt.Tag.ToString();
            if (filter.Contains(".bin"))
            {
                filter += "|所有文件(*.*)|*.*";
            }
            if (txt == null) return;
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "请选择文件";
            //“图像文件(*.bmp, *.jpg)|*.bmp;*.jpg|所有文件(*.*)|*.*”
            fd.Filter = filter;
            fd.FileName = txt.Text.Trim();
            if (fd.ShowDialog() == true)
            {
                txt.Text = fd.FileName;
            }
            tbox.Focus();
        }

        #endregion

        #region OpenFolderCommand 选择文件夹命令

        /// <summary>
        /// 选择文件夹命令
        /// </summary>
        public static RoutedUICommand OpenFolderCommand { get; private set; }

        /// <summary>
        /// OpenFolderCommand绑定事件
        /// </summary>
        private static readonly CommandBinding OpenFolderCommandBinding;

        /// <summary>
        /// 执行OpenFolderCommand
        /// </summary>
        private static void OpenFolderButtonClick(object sender, ExecutedRoutedEventArgs e)
        {
            //var tbox = e.Parameter as FrameworkElement;
            //var txt = tbox as TextBox;
            //if (txt == null) return;
            //System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            //fd.Description = "请选择文件路径";
            //fd.SelectedPath = txt.Text.Trim();
            //if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    txt.Text = fd.SelectedPath;
            //}
            //tbox.Focus();
        }

        #endregion

        #region SaveFileCommand 选择文件保存路径及名称

        /// <summary>
        /// 选择文件保存路径及名称
        /// </summary>
        public static RoutedUICommand SaveFileCommand { get; private set; }

        /// <summary>
        /// SaveFileCommand绑定事件
        /// </summary>
        private static readonly CommandBinding SaveFileCommandBinding;

        /// <summary>
        /// 执行OpenFileCommand
        /// </summary>
        private static void SaveFileButtonClick(object sender, ExecutedRoutedEventArgs e)
        {
            //var tbox = e.Parameter as FrameworkElement;
            //var txt = tbox as TextBox;
            //if (txt == null) return;
            //SaveFileDialog fd = new SaveFileDialog();
            //fd.Title = "文件保存路径";
            //fd.Filter = "所有文件(*.*)|*.*";
            //fd.FileName = txt.Text.Trim();
            //if (fd.ShowDialog() == DialogResult.OK)
            //{
            //    txt.Text = fd.FileName;
            //}
            //tbox.Focus();
        }

        #endregion

        #region - PassWordBox -

        public static readonly DependencyProperty PasswordProperty =
          DependencyProperty.RegisterAttached("Password",
          typeof(string), typeof(Cattach),
          new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(Cattach), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(Cattach));

        public static void SetAttach(this DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }
        public static bool GetAttach(this DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }
        public static string GetPassword(this DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }
        public static void SetPassword(this DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }
        private static bool GetIsUpdating(this DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }
        private static void SetIsUpdating(this DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }
        private static void OnPasswordPropertyChanged(this DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }
        private static void Attach(this DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;
            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }

        #endregion

        #region - ListBox - 

        public static IList GetSelectedItems(this DependencyObject obj)
        {
            return (IList)obj.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(this DependencyObject obj, IList value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        //Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...

        public static readonly DependencyProperty SelectedItemsProperty =

            DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(Cattach), new PropertyMetadata(OnSelectedItemsChanged));

        static public void OnSelectedItemsChanged(this DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;

            if ((listBox != null) && (listBox.SelectionMode == SelectionMode.Multiple))
            {
                if (e.OldValue != null)
                {
                    listBox.SelectionChanged -= OnlistBoxSelectionChanged;
                }

                IList collection = e.NewValue as IList;

                listBox.SelectedItems.Clear();

                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                    listBox.SelectionChanged += OnlistBoxSelectionChanged;
                }
            }
        }

        static void OnlistBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList dataSource = GetSelectedItems(sender as DependencyObject);

            //添加用户选中的当前项.
            foreach (var item in e.AddedItems)
            {
                dataSource.Add(item);
            }

            //删除用户取消选中的当前项
            foreach (var item in e.RemovedItems)
            {
                dataSource.Remove(item);
            }
        }

        #endregion

        #region bool 类型的附加属性 Tag
        /// <summary>
        /// bool类型附加属性
        /// </summary>
        public static readonly DependencyProperty BoolProperty = DependencyProperty.RegisterAttached(
            "Bool", typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(false));

        public static bool GetBool(this DependencyObject d)
        {
            return (bool)d.GetValue(BoolProperty);
        }

        public static void SetBool(this DependencyObject obj, bool value)
        {
            obj.SetValue(BoolProperty, value);
        }
        #endregion

        #region - 将焦点设置在控件上 -
        /// <summary>
        /// 将焦点设置在控件上
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused", typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(false, (l, k) =>
            {
                if (k.NewValue == k.NewValue) return;

                if (l is UIElement element)
                {
                    element?.Focus();
                }
            }
            ));

        public static bool GetIsFocused(this DependencyObject d)
        {
            return (bool)d.GetValue(DoubleAttachProperty);
        }

        public static void SetIsFocused(this DependencyObject obj, bool value)
        {
            obj.SetValue(DoubleAttachProperty, value);
        }
        #endregion

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static Cattach()
        {
            //ClearTextCommand
            ClearTextCommand = new RoutedUICommand();
            ClearTextCommandBinding = new CommandBinding(ClearTextCommand);
            ClearTextCommandBinding.Executed += ClearButtonClick;
            //OpenFileCommand
            OpenFileCommand = new RoutedUICommand();
            OpenFileCommandBinding = new CommandBinding(OpenFileCommand);
            OpenFileCommandBinding.Executed += OpenFileButtonClick;
            //OpenFolderCommand
            OpenFolderCommand = new RoutedUICommand();
            OpenFolderCommandBinding = new CommandBinding(OpenFolderCommand);
            OpenFolderCommandBinding.Executed += OpenFolderButtonClick;
            //SaveFileCommand
            SaveFileCommand = new RoutedUICommand();
            SaveFileCommandBinding = new CommandBinding(SaveFileCommand);
            SaveFileCommandBinding.Executed += SaveFileButtonClick;
        }
    }



    static partial class Cattach
    {

        #region - 等待效果 -

        public static bool GetIsBuzy(this DependencyObject obj)
        {
            return (bool)obj.GetValue(IsBuzyProperty);
        }

        public static void SetIsBuzy(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsBuzyProperty, value);
        }

        /// <summary> 是否等待 </summary>
        public static readonly DependencyProperty IsBuzyProperty =
            DependencyProperty.RegisterAttached("IsBuzy", typeof(bool), typeof(Cattach), new PropertyMetadata(false));

        public static string GetBuzyText(this DependencyObject obj)
        {
            return (string)obj.GetValue(BuzyTextProperty);
        }

        public static void SetBuzyText(this DependencyObject obj, string value)
        {
            obj.SetValue(BuzyTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for BuzyText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BuzyTextProperty =
            DependencyProperty.RegisterAttached("BuzyText", typeof(string), typeof(Cattach), new PropertyMetadata("请等待"));

        #endregion

        #region - Path -

        public static Geometry GetPath(this DependencyObject obj)
        {
            return (Geometry)obj.GetValue(PathProperty);
        }

        public static void SetPath(this DependencyObject obj, Geometry value)
        {
            obj.SetValue(PathProperty, value);
        }

        /// <summary> 是否等待 </summary>
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.RegisterAttached("Path", typeof(Geometry), typeof(Cattach), new PropertyMetadata(default(Geometry)));

        #endregion
    }

    static partial class Cattach
    {

        #region - 应用动画效果的显示、隐藏 -


        public static bool GetIsClose(this DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCloseProperty);
        }

        public static void SetIsClose(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsCloseProperty, value);
        }

        /// <summary> 应用窗体关闭和显示 </summary>
        public static readonly DependencyProperty IsCloseProperty =
            DependencyProperty.RegisterAttached("IsClose", typeof(bool), typeof(Cattach), new PropertyMetadata(false, OnIsCloseChanged));

        static public void OnIsCloseChanged(this DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;

            var actions = Cattach.GetVisibleActions(element);

            if (actions == null) return;

            bool v = (bool)e.NewValue;

            if (v)
            {
                //  Do ：显示动画
                foreach (var item in actions)
                {
                    item.BeginVisible(element);
                }
            }
            else
            {
                //  Do ：隐藏动画
                foreach (var item in actions)
                {
                    item.BeginHidden(element, () =>
                    {
                        if (element is Window window)
                        {
                            window.Close();
                        }

                        return;
                    });
                }
            }
        }

        public static bool GetIsVisible(this DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVisibleProperty);
        }

        public static void SetIsVisible(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsVisibleProperty, value);
        }

        /// <summary> 应用控件显示和隐藏 </summary>
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(Cattach), new PropertyMetadata(true, OnIsVisibleChanged));

        static public void OnIsVisibleChanged(this DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;

            var actions = Cattach.GetVisibleActions(element);

            if (actions == null) return;

            bool v = (bool)e.NewValue;

            if (v)
            {

                //  Do ：显示动画
                foreach (var item in actions)
                {
                    item.BeginVisible(element);
                }
            }
            else
            {
                //  Do ：隐藏动画
                foreach (var item in actions)
                {
                    item.BeginHidden(element);
                }

            }
        }


        public static ActionCollection GetVisibleActions(this DependencyObject obj)
        {
            return (ActionCollection)obj.GetValue(VisibleActionsProperty);
        }

        public static void SetVisibleActions(this DependencyObject obj, ActionCollection value)
        {
            obj.SetValue(VisibleActionsProperty, value);
        }

        /// <summary> 显示隐藏过度效果 </summary>
        public static readonly DependencyProperty VisibleActionsProperty =
            DependencyProperty.RegisterAttached("VisibleActions", typeof(ActionCollection), typeof(Cattach), new PropertyMetadata(new ActionCollection() { new OpacityAction() }));

        #endregion
    }

    static partial class Cattach
    {
        /// <summary>
        /// 是否启用关闭按钮
        /// </summary>
        public static readonly DependencyProperty IsUseCloseProperty = DependencyProperty.RegisterAttached(
            "IsUseClose", typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsUseCloseChanged));

        public static bool GetIsUseClose(this DependencyObject d)
        {
            return (bool)d.GetValue(IsUseCloseProperty);
        }

        public static void SetIsUseClose(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsUseCloseProperty, value);
        }

        static void OnIsUseCloseChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 是否正在拖动 DragEnter
        /// </summary>
        public static readonly DependencyProperty IsDragEnterProperty = DependencyProperty.RegisterAttached(
            "IsDragEnter", typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDragEnterChanged));

        public static bool GetIsDragEnter(this DependencyObject d)
        {
            return (bool)d.GetValue(IsDragEnterProperty);
        }

        public static void SetIsDragEnter(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragEnterProperty, value);
        }

        static void OnIsDragEnterChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }

    }

    static partial class Cattach
    {
        /// <summary>
        /// 子内容中的间距
        /// </summary>
        public static readonly DependencyProperty CellMarginProperty = DependencyProperty.RegisterAttached(
            "CellMargin", typeof(Thickness), typeof(Cattach), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCellMarginChanged));

        public static Thickness GetCellMargin(this DependencyObject d)
        {
            return (Thickness)d.GetValue(CellMarginProperty);
        }

        public static void SetCellMargin(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(CellMarginProperty, value);
        }

        static void OnCellMarginChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 布局方式
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached(
            "Orientation", typeof(Orientation), typeof(Cattach), new FrameworkPropertyMetadata(default(Orientation), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnOrientationChanged));

        public static Orientation GetOrientation(this DependencyObject d)
        {
            return (Orientation)d.GetValue(OrientationProperty);
        }

        public static void SetOrientation(this DependencyObject obj, Orientation value)
        {
            obj.SetValue(OrientationProperty, value);
        }

        static void OnOrientationChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 是否选中
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached(
            "IsSelected", typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsSelectedChanged));

        public static bool GetIsSelected(this DependencyObject d)
        {
            return (bool)d.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelected(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedProperty, value);
        }

        static void OnIsSelectedChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// 值
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value", typeof(Double), typeof(Cattach), new FrameworkPropertyMetadata(default(Double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static Double GetValue(this DependencyObject d)
        {
            return (Double)d.GetValue(ValueProperty);
        }

        public static void SetValue(this DependencyObject obj, Double value)
        {
            obj.SetValue(ValueProperty, value);
        }

        static void OnValueChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 文本
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public static string GetText(this DependencyObject d)
        {
            return (string)d.GetValue(TextProperty);
        }

        public static void SetText(this DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        static void OnTextChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 是否显示等待
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.RegisterAttached(
            "IsIndeterminate", typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsIndeterminateChanged));

        public static bool GetIsIndeterminate(this DependencyObject d)
        {
            return (bool)d.GetValue(IsIndeterminateProperty);
        }

        public static void SetIsIndeterminate(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsIndeterminateProperty, value);
        }

        static void OnIsIndeterminateChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 是否始终显示
        /// </summary>
        public static readonly DependencyProperty IsStayOpenProperty = DependencyProperty.RegisterAttached(
            "IsStayOpen", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsStayOpenChanged));

        public static string GetIsStayOpen(this DependencyObject d)
        {
            return (string)d.GetValue(IsStayOpenProperty);
        }

        public static void SetIsStayOpen(this DependencyObject obj, string value)
        {
            obj.SetValue(IsStayOpenProperty, value);
        }

        static void OnIsStayOpenChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 选中文本
        /// </summary>
        public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.RegisterAttached(
            "SelectedText", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTextChanged));

        public static string GetSelectedText(this DependencyObject d)
        {
            return (string)d.GetValue(SelectedTextProperty);
        }

        public static void SetSelectedText(this DependencyObject obj, string value)
        {
            obj.SetValue(SelectedTextProperty, value);
        }

        static void OnSelectedTextChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 未选中文本
        /// </summary>
        public static readonly DependencyProperty UnSelectedTextProperty = DependencyProperty.RegisterAttached(
            "UnSelectedText", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnUnSelectedTextChanged));

        public static string GetUnSelectedText(this DependencyObject d)
        {
            return (string)d.GetValue(UnSelectedTextProperty);
        }

        public static void SetUnSelectedText(this DependencyObject obj, string value)
        {
            obj.SetValue(UnSelectedTextProperty, value);
        }

        static void OnUnSelectedTextChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }

        #region - 标题相关 - 


        //public Thickness TitleMargin
        //{
        //    get { return (Thickness)GetValue(TitleMarginProperty); }
        //    set { SetValue(TitleMarginProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TitleMarginProperty =
        //    DependencyProperty.Register("TitleMargin", typeof(Thickness), typeof(Cattach), new PropertyMetadata(default(Thickness), (d, e) =>
        //     {
        //         Cattach control = d as Cattach;

        //         if (control == null) return;

        //         Thickness config = e.NewValue as Thickness;

        //     }));

        /// <summary>
        /// 标题水平布局
        /// </summary>
        public static readonly DependencyProperty TitleHorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
            "TitleHorizontalAlignment", typeof(HorizontalAlignment), typeof(Cattach), new FrameworkPropertyMetadata(default(HorizontalAlignment), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleHorizontalAlignmentChanged));

        public static HorizontalAlignment GetTitleHorizontalAlignment(this DependencyObject d)
        {
            return (HorizontalAlignment)d.GetValue(TitleHorizontalAlignmentProperty);
        }

        public static void SetTitleHorizontalAlignment(this DependencyObject obj, HorizontalAlignment value)
        {
            obj.SetValue(TitleHorizontalAlignmentProperty, value);
        }

        static void OnTitleHorizontalAlignmentChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题垂直布局
        /// </summary>
        public static readonly DependencyProperty TitleVerticalAlignmentProperty = DependencyProperty.RegisterAttached(
            "TitleVerticalAlignment", typeof(VerticalAlignment), typeof(Cattach), new FrameworkPropertyMetadata(default(VerticalAlignment), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleVerticalAlignmentChanged));

        public static VerticalAlignment GetTitleVerticalAlignment(this DependencyObject d)
        {
            return (VerticalAlignment)d.GetValue(TitleVerticalAlignmentProperty);
        }

        public static void SetTitleVerticalAlignment(this DependencyObject obj, VerticalAlignment value)
        {
            obj.SetValue(TitleVerticalAlignmentProperty, value);
        }

        static void OnTitleVerticalAlignmentChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }



        /// <summary>
        /// 标题间距
        /// </summary>
        public static readonly DependencyProperty TitleMarginProperty = DependencyProperty.RegisterAttached(
            "TitleMargin", typeof(Thickness), typeof(Cattach), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleMarginChanged));

        public static Thickness GetTitleMargin(this DependencyObject d)
        {
            return (Thickness)d.GetValue(TitleMarginProperty);
        }

        public static void SetTitleMargin(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(TitleMarginProperty, value);
        }

        static void OnTitleMarginChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题宽度
        /// </summary>
        public static readonly DependencyProperty TitleWidthProperty = DependencyProperty.RegisterAttached(
            "TitleWidth", typeof(double), typeof(Cattach), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleWidthChanged));

        [TypeConverter(typeof(LengthConverter))]
        public static double GetTitleWidth(this DependencyObject d)
        {
            return (double)d.GetValue(TitleWidthProperty);
        }
        [TypeConverter(typeof(LengthConverter))]
        public static void SetTitleWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(TitleWidthProperty, value);
        }

        static void OnTitleWidthChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题高度
        /// </summary>
        public static readonly DependencyProperty TitleHeightProperty = DependencyProperty.RegisterAttached(
            "TitleHeight", typeof(double), typeof(Cattach), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleHeightChanged));

        [TypeConverter(typeof(LengthConverter))]
        public static double GetTitleHeight(this DependencyObject d)
        {
            return (double)d.GetValue(TitleHeightProperty);
        }
        [TypeConverter(typeof(LengthConverter))]
        public static void SetTitleHeight(this DependencyObject obj, double value)
        {
            obj.SetValue(TitleHeightProperty, value);
        }

        static void OnTitleHeightChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached(
            "Title", typeof(object), typeof(Cattach), new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleChanged));

        public static object GetTitle(this DependencyObject d)
        {
            return (object)d.GetValue(TitleProperty);
        }

        public static void SetTitle(this DependencyObject obj, object value)
        {
            obj.SetValue(TitleProperty, value);
        }

        static void OnTitleChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题模板
        /// </summary>
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.RegisterAttached(
            "TitleTemplate", typeof(ControlTemplate), typeof(Cattach), new FrameworkPropertyMetadata(default(ControlTemplate), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleTemplateChanged));

        public static ControlTemplate GetTitleTemplate(this DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(TitleTemplateProperty);
        }

        public static void SetTitleTemplate(this DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(TitleTemplateProperty, value);
        }

        static void OnTitleTemplateChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题背景色
        /// </summary>
        public static readonly DependencyProperty TitleBackgroundProperty = DependencyProperty.RegisterAttached(
            "TitleBackground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleBackgroundChanged));

        public static Brush GetTitleBackground(this DependencyObject d)
        {
            return (Brush)d.GetValue(TitleBackgroundProperty);
        }

        public static void SetTitleBackground(this DependencyObject obj, Brush value)
        {
            obj.SetValue(TitleBackgroundProperty, value);
        }

        static void OnTitleBackgroundChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题前景色
        /// </summary>
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.RegisterAttached(
            "TitleForeground", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleForegroundChanged));

        public static Brush GetTitleForeground(this DependencyObject d)
        {
            return (Brush)d.GetValue(TitleForegroundProperty);
        }

        public static void SetTitleForeground(this DependencyObject obj, Brush value)
        {
            obj.SetValue(TitleForegroundProperty, value);
        }

        static void OnTitleForegroundChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题边框线颜色
        /// </summary>
        public static readonly DependencyProperty TitleBorderBrushProperty = DependencyProperty.RegisterAttached(
            "TitleBorderBrush", typeof(Brush), typeof(Cattach), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleBorderBrushChanged));

        public static Brush GetTitleBorderBrush(this DependencyObject d)
        {
            return (Brush)d.GetValue(TitleBorderBrushProperty);
        }

        public static void SetTitleBorderBrush(this DependencyObject obj, Brush value)
        {
            obj.SetValue(TitleBorderBrushProperty, value);
        }

        static void OnTitleBorderBrushChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 标题边框
        /// </summary>
        public static readonly DependencyProperty TitleBorderThicknessProperty = DependencyProperty.RegisterAttached(
            "TitleBorderThickness", typeof(Thickness), typeof(Cattach), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTitleBorderThicknessChanged));

        public static Thickness GetTitleBorderThickness(this DependencyObject d)
        {
            return (Thickness)d.GetValue(TitleBorderThicknessProperty);
        }

        public static void SetTitleBorderThickness(this DependencyObject obj, Thickness value)
        {
            obj.SetValue(TitleBorderThicknessProperty, value);
        }

        static void OnTitleBorderThicknessChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        #endregion

        #region - 历史记录相关 -

        /// <summary>
        /// 是否启用历史数据
        /// </summary>
        public static readonly DependencyProperty IsUseHistoryProperty = DependencyProperty.RegisterAttached(
            "IsUseHistory", typeof(bool), typeof(Cattach), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsUseHistoryChanged));

        public static bool GetIsUseHistory(this DependencyObject d)
        {
            return (bool)d.GetValue(IsUseHistoryProperty);
        }

        public static void SetIsUseHistory(this DependencyObject obj, bool value)
        {
            obj.SetValue(IsUseHistoryProperty, value);
        }

        static void OnIsUseHistoryChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var textBox = sender as TextBox;

            if (textBox == null) return;

            if (args.NewValue is bool b == true)
            {
                textBox.LostFocus -= TextBox_LostFocus;
                textBox.LostFocus += TextBox_LostFocus;
            }
            else
            {
                textBox.LostFocus -= TextBox_LostFocus;
            }
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            var datas = Cattach.GetHistoryData(textBox);

            var capacity = Cattach.GetCapacity(textBox);

            var regex = Cattach.GetRegex(textBox);

            if (datas.Count > capacity)
            {
                datas = datas.Take(capacity)?.ToObservable();
            }

            if (!Regex.IsMatch(textBox.Text, regex)) return;

            datas.Remove(textBox.Text);
            datas.Insert(0, textBox.Text);

            Cattach.SetHistoryData(textBox, datas);
        }


        /// <summary>
        /// 历史数据
        /// </summary>
        public static readonly DependencyProperty HistoryDataProperty = DependencyProperty.RegisterAttached(
            "HistoryData", typeof(ObservableCollection<string>), typeof(Cattach), new FrameworkPropertyMetadata(new ObservableCollection<string>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHistoryDataChanged));

        public static ObservableCollection<string> GetHistoryData(this DependencyObject d)
        {
            return (ObservableCollection<string>)d.GetValue(HistoryDataProperty);
        }

        public static void SetHistoryData(this DependencyObject obj, ObservableCollection<string> value)
        {
            obj.SetValue(HistoryDataProperty, value);
        }

        static void OnHistoryDataChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 数据容量
        /// </summary>
        public static readonly DependencyProperty CapacityProperty = DependencyProperty.RegisterAttached(
            "Capacity", typeof(int), typeof(Cattach), new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCapacityChanged));

        public static int GetCapacity(this DependencyObject d)
        {
            return (int)d.GetValue(CapacityProperty);
        }

        public static void SetCapacity(this DependencyObject obj, int value)
        {
            obj.SetValue(CapacityProperty, value);
        }

        static void OnCapacityChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 验证数据的正则表达式 默认不等于空
        /// </summary>
        public static readonly DependencyProperty RegexProperty = DependencyProperty.RegisterAttached(
            "Regex", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(@"\S", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRegexChanged));

        public static string GetRegex(this DependencyObject d)
        {
            return (string)d.GetValue(RegexProperty);
        }

        public static void SetRegex(this DependencyObject obj, string value)
        {
            obj.SetValue(RegexProperty, value);
        }

        static void OnRegexChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }


        /// <summary>
        /// 选中历史数据改变
        /// </summary>
        public static readonly DependencyProperty SelectedHistroyItemProperty = DependencyProperty.RegisterAttached(
            "SelectedHistroyItem", typeof(string), typeof(Cattach), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedHistroyItemChanged));

        public static string GetSelectedHistroyItem(this DependencyObject d)
        {
            return (string)d.GetValue(SelectedHistroyItemProperty);
        }

        public static void SetSelectedHistroyItem(this DependencyObject obj, string value)
        {
            obj.SetValue(SelectedHistroyItemProperty, value);
        }

        static void OnSelectedHistroyItemChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var textBox = sender as TextBox;

            string config = args.NewValue as string;

            if (config == null) return;

            var regex = Cattach.GetRegex(textBox);

            if (!Regex.IsMatch(config, regex)) return;

            textBox.Text = config;
        }

        #endregion
    }

    //static partial class Cattach
    //{
    //    /// <summary>
    //    /// 要注册的行为
    //    /// </summary>
    //    public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached(
    //        "Behaviors", typeof(Behaviors), typeof(Cattach), new FrameworkPropertyMetadata(new Behaviors(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBehaviorsChanged));

    //    public static Behaviors GetBehaviors(this DependencyObject d)
    //    {
    //        return (Behaviors)d.GetValue(BehaviorsProperty);
    //    }

    //    public static void SetBehaviors(this DependencyObject obj, Behaviors value)
    //    {
    //        obj.SetValue(BehaviorsProperty, value);
    //    }

    //    static void OnBehaviorsChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
    //    {
    //        Behaviors o = args.OldValue as Behaviors;

    //        Behaviors n = args.NewValue as Behaviors;

    //        BehaviorCollection bc = Interaction.GetBehaviors(sender);

    //        //  Do ：先删除
    //        if (o != null)
    //        {
    //            foreach (var b in o)
    //            {
    //                var behavior = bc.FirstOrDefault(beh => beh.GetType() == b.GetType());

    //                if (behavior != null)
    //                    bc.Remove(behavior);
    //            }
    //        }

    //        //  Do ：再更新
    //        if (n != null)
    //        {
    //            foreach (var b in n)
    //            {
    //                var behavior = bc.FirstOrDefault(beh => beh.GetType() == b.GetType());

    //                var instance = Activator.CreateInstance(b.GetType()) as Behavior;

    //                if (behavior != null)
    //                {
    //                    bc.Remove(behavior);
    //                }


    //                foreach (var property in b.GetType().GetProperties())
    //                {
    //                    if (property.CanRead && property.CanWrite)
    //                    {
    //                        property.SetValue(instance, property.GetValue(b));
    //                    }
    //                }

    //                bc.Add(instance);
    //            }
    //        }

    //    }



    //    /// <summary>
    //    /// 枚举类型数据源
    //    /// </summary>
    //    public static readonly DependencyProperty EnumTypeSourceProperty = DependencyProperty.RegisterAttached(
    //        "EnumTypeSource", typeof(Type), typeof(Cattach), new FrameworkPropertyMetadata(default(Type), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEnumTypeSourceChanged));

    //    public static Type GetEnumTypeSource(this DependencyObject d)
    //    {
    //        return (Type)d.GetValue(EnumTypeSourceProperty);
    //    }

    //    public static void SetEnumTypeSource(this DependencyObject obj, Type value)
    //    {
    //        obj.SetValue(EnumTypeSourceProperty, value);
    //    }

    //    static void OnEnumTypeSourceChanged(this DependencyObject sender, DependencyPropertyChangedEventArgs args)
    //    {
    //        var items = sender as ItemsControl;

    //        Type type = args.NewValue as Type;

    //        if (type != null)
    //        {
    //            Type actualEnumType = Nullable.GetUnderlyingType(type) ?? type;
    //            Array enumVlues = Enum.GetValues(actualEnumType);

    //            if (actualEnumType == type)
    //            {
    //                items.ItemsSource = enumVlues;
    //                return;
    //            }


    //            Array tempArray = Array.CreateInstance(actualEnumType, enumVlues.Length + 1);

    //            enumVlues.CopyTo(tempArray, 1);

    //            items.ItemsSource = tempArray;
    //        }
    //    }

    //}

    //public class Behaviors : ObservableCollection<Behavior>
    //{

    //}
}
