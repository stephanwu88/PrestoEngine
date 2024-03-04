using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Engine.WpfControl
{
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum ButtonType
    {
        Normal,
        Icon,
        Text,
        IconText
    }
    /// <summary>
    /// 按钮功能
    /// </summary>
    public enum ButtonFunction
    {
        SetOn,          //设On
        SetOff,         //设off
        HoldByHand,     //保持型
        Alternate,      //交替型
    }

    /// <summary>
    /// 多媒体按钮
    /// </summary>
    public class MediaButton : Button
    {
        static MediaButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaButton), new FrameworkPropertyMetadata(typeof(MediaButton)));
        }

        /// <summary>
        /// 按钮类型
        /// </summary>
        public ButtonType ButtonType
        {
            get { return (ButtonType)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ButtonTypeProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(ButtonType), typeof(MediaButton), new PropertyMetadata(ButtonType.Normal));


        /// <summary>
        /// 按钮功能
        /// </summary>
        public ButtonFunction ButtonFunction
        {
            get { return (ButtonFunction)GetValue(ButtonFunctionProperty); }
            set { SetValue(ButtonFunctionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ButtonFunctionProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonFunctionProperty =
            DependencyProperty.Register("ButtonFunction", typeof(ButtonFunction), typeof(MediaButton), new PropertyMetadata(ButtonFunction.HoldByHand));

        /// <summary>
        /// 按钮元素排布方向
        /// </summary>
        public Orientation ContentOrientaiton
        {
            get { return (Orientation)GetValue(ContentOrientaitonProperty); }
            set { SetValue(ContentOrientaitonProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ContentOrientaiton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentOrientaitonProperty =
            DependencyProperty.Register("ContentOrientaiton", typeof(Orientation), typeof(MediaButton), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// 图片
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(MediaButton), new PropertyMetadata(null));

        /// <summary>
        /// 图标高度
        /// </summary>
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconHeightProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(MediaButton), new PropertyMetadata(16.0));

        /// <summary>
        /// 图标宽度
        /// </summary>
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconWidthProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(MediaButton), new PropertyMetadata(16.0));

        /// <summary>
        /// 图片位置
        /// </summary>
        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconMarginProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(MediaButton), new PropertyMetadata(new Thickness(0,0,0,0)));

        /// <summary>
        /// 文字位置
        /// </summary>
        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TextMarginProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(MediaButton), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// 文字位置
        /// </summary>
        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TextPaddingProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextPaddingProperty =
            DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(MediaButton), new PropertyMetadata(new Thickness(0,0,0,0)));

        /// <summary>
        /// 按钮外框圆角
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CornerRadiusProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MediaButton), new PropertyMetadata(new CornerRadius(0)));


        /// <summary>
        /// 鼠标滑过颜色
        /// </summary>
        public Brush MouseOverForeground
        {
            get { return (Brush)GetValue(MouseOverForegroundProperty); }
            set { SetValue(MouseOverForegroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseOverForegroundProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(MediaButton), new PropertyMetadata());

        /// <summary>
        /// 鼠标按下颜色
        /// </summary>
        public Brush MousePressedForeground
        {
            get { return (Brush)GetValue(MousePressedForegroundProperty); }
            set { SetValue(MousePressedForegroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MousePressedForegroundProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePressedForegroundProperty =
            DependencyProperty.Register("MousePressedForeground", typeof(Brush), typeof(MediaButton), new PropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        public Brush MouseOverBorderbrush
        {
            get { return (Brush)GetValue(MouseOverBorderbrushProperty); }
            set { SetValue(MouseOverBorderbrushProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseOverBorderbrushProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBorderbrushProperty =
            DependencyProperty.Register("MouseOverBorderbrush", typeof(Brush), typeof(MediaButton), new PropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseOverBackgroundProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(MediaButton), new PropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        public Brush MousePressedBackground
        {
            get { return (Brush)GetValue(MousePressedBackgroundProperty); }
            set { SetValue(MousePressedBackgroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MousePressedBackgroundProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePressedBackgroundProperty =
            DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(MediaButton), new PropertyMetadata());
    }
}
