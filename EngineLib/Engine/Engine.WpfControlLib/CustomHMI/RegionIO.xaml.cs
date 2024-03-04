using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Engine.WpfControl
{
    /// <summary>
    /// RegionIO.xaml 的交互逻辑
    /// </summary>
    public partial class RegionIO : UserControl
    {
        /// <summary>
        /// 默认显示颜色
        /// </summary>
        public Brush DefaultViewColor
        {
            get { return (Brush)GetValue(DefaultViewColorProperty); }
            set { SetValue(DefaultViewColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultViewColorProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultViewColorProperty =
            DependencyProperty.Register("DefaultViewColor", typeof(Brush), typeof(RegionIO), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// 编辑状态显示颜色
        /// </summary>
        public Brush EditViewColor
        {
            get { return (Brush)GetValue(EditViewColorProperty); }
            set { SetValue(EditViewColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditViewColorProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditViewColorProperty =
            DependencyProperty.Register("EditViewColor", typeof(Brush), typeof(RegionIO), new PropertyMetadata(Brushes.Orange));

        /// <summary>
        /// 外框显示颜色
        /// </summary>
        public Brush BorderViewColor
        {
            get { return (Brush)GetValue(BorderViewColorProperty); }
            set { SetValue(BorderViewColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderViewColorProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderViewColorProperty =
            DependencyProperty.Register("BorderViewColor", typeof(Brush), typeof(RegionIO), new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// 外框显示颜色
        /// </summary>
        public Thickness BorderViewThickness
        {
            get { return (Thickness)GetValue(BorderViewThicknessProperty); }
            set { SetValue(BorderViewThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderViewThicknessProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderViewThicknessProperty =
            DependencyProperty.Register("BorderViewThickness", typeof(Thickness), typeof(RegionIO), new PropertyMetadata(new Thickness(1, 1, 1, 1)));

        /// <summary>
        /// 输入文本
        /// </summary>
        public string InputValue
        {
            get { return (string)GetValue(InputValueProperty); }
            set { SetValue(InputValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for InputValueProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputValueProperty = 
            DependencyProperty.Register("InputValue", typeof(string), typeof(RegionIO), new PropertyMetadata("TextBox", new PropertyChangedCallback(OnInputTextChanged)));

        /// <summary>
        /// 写值通道
        /// </summary>
        public string WriteChannel
        {
            get { return (string)GetValue(WriteChannelProperty); }
            set { SetValue(WriteChannelProperty, value); }
        }
        // Using a DependencyProperty as the backing store for WriteChannelProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WriteChannelProperty =
            DependencyProperty.Register("WriteChannel", typeof(string), typeof(RegionIO), new PropertyMetadata(""));

        /// <summary>
        /// 显示值通道
        /// </summary>
        public string ReadChannel
        {
            get { return (string)GetValue(ReadChannelProperty); }
            set { SetValue(ReadChannelProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ReadChannelProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadChannelProperty = 
            DependencyProperty.Register("ReadChannel", typeof(string), typeof(RegionIO), new PropertyMetadata("", new PropertyChangedCallback(OnOutputTextChanged)));

        static void OnInputTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((RegionIO)sender).OnInputValueChanged(args);
        }
        static void OnOutputTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((RegionIO)sender).OnOutputValueChanged(args);
        }


        protected void OnInputValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            InputTextBox.Text = e.NewValue.ToString();
        }
        protected void OnOutputValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            OutputTextBox.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RegionIO()
        {
            InitializeComponent();
            InputTextBox.Visibility = Visibility.Collapsed;
            OutputTextBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 输出框获得焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Visibility = Visibility.Collapsed; //隐藏控件
            InputTextBox.Visibility = Visibility.Visible; //显示控件
            InputTextBox.Focus();//获得焦点
            if (ReadChannel == null) InputValue = "";
            InputValue = ReadChannel; //输出值传给输入框
            InputTextBox.SelectAll();//选择全部内容

        }

        /// <summary>
        /// 输入框和输出框任意一个失去焦点事件(隐藏输入框，显示输出框)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InputTextBox.Visibility = Visibility.Collapsed;
            OutputTextBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 输入框按下键盘(隐藏输入框，显示输出框)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            InputValue = InputTextBox.Text;
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                InputTextBox.Visibility = Visibility.Collapsed;
                OutputTextBox.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 输入框内容改变时更新属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputValue = InputTextBox.Text;
        }
    }

}
