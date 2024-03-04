using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Engine.WpfControl
{
    /// <summary>
    /// 运动形式
    /// </summary>
    public enum Motion
    {
        Stop,   //默认值
        Forword,
        Backword
    }

    /// <summary>
    /// 值转换器
    /// </summary>
    public class MotionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value==null)
                return Visibility.Hidden;
            Motion mo = (Motion)value;
            if (mo == Motion.Stop)
                return Visibility.Hidden;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Pipe.xaml 的交互逻辑
    /// </summary>
    public partial class PipeLine : UserControl
    {
        public PipeLine()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 流水方向
        /// </summary>
        public Motion Motion
        {
            get { return (Motion)GetValue(MotionProperty); }
            set { SetValue(MotionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MotionProperty =
            DependencyProperty.Register("Motion", typeof(Motion), typeof(PipeLine), new PropertyMetadata(default(Motion), new PropertyChangedCallback(OnDirectionChanged)));

        private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Motion value = (Motion)e.NewValue;
            string strStateName = string.Empty;
            if (value == Motion.Forword)
                strStateName = "Forword";
            else if (value == Motion.Backword)
                strStateName = "Backword";
            else if (value == Motion.Stop)
                strStateName = "Stop";
            VisualStateManager.GoToState(d as PipeLine, strStateName, false);
        }

        /// <summary>
        /// 颜色
        /// </summary>
        public Brush LiquidColor
        {
            get { return (Brush)GetValue(LiquidColorProperty); }
            set { SetValue(LiquidColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LiquidColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LiquidColorProperty =
            DependencyProperty.Register("LiquidColor", typeof(Brush), typeof(PipeLine), new PropertyMetadata(Brushes.Orange));

        public int CapRadius
        {
            get { return (int)GetValue(CapRadiusProperty); }
            set { SetValue(CapRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CapRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CapRadiusProperty =
            DependencyProperty.Register("CapRadius", typeof(int), typeof(PipeLine), new PropertyMetadata(0));
    }
}
