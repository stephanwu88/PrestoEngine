using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Engine.WpfControl
{
    /// <summary>
    /// 自定义控件 - 图片按钮
    /// </summary>
    public class ImageButton : Button
    {
        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.MouseOverBackground == null)
            {
                this.MouseOverBackground = Background;
            }
            if (this.MouseDownBackground == null)
            {
                if (this.MouseOverBackground == null)
                {
                    this.MouseDownBackground = Background;
                }
                else
                {
                    this.MouseDownBackground = MouseOverBackground;
                }
            }
            if (this.MouseOverBorderBrush == null)
            {
                this.MouseOverBorderBrush = BorderBrush;
            }
            if (this.MouseDownBorderBrush == null)
            {
                if (this.MouseOverBorderBrush == null)
                {
                    this.MouseDownBorderBrush = BorderBrush;
                }
                else
                {
                    this.MouseDownBorderBrush = MouseOverBorderBrush;
                }
            }
            if (this.MouseOverForeground == null)
            {
                this.MouseOverForeground = Foreground;
            }
            if (this.MouseDownForeground == null)
            {
                if (this.MouseOverForeground == null)
                {
                    this.MouseDownForeground = Foreground;
                }
                else
                {
                    this.MouseDownForeground = this.MouseOverForeground;
                }
            }
        }

        /// <summary>
        /// 鼠标移上去的背景颜色
        /// </summary>
        public Brush MouseOverBackground
        {
            get
            {
                return (Brush)GetValue(MouseOverBackgroundProperty);
            }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseOverBackgroundProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBackgroundProperty
            = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ImageButton));

        /// <summary>
        /// 鼠标按下去的背景颜色
        /// </summary>
        public Brush MouseDownBackground
        {
            get
            {
                return (Brush)GetValue(MouseDownBackgroundProperty);
            }
            set { SetValue(MouseDownBackgroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseDownBackgroundProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseDownBackgroundProperty
            = DependencyProperty.Register("MouseDownBackground", typeof(Brush), typeof(ImageButton));

        /// <summary>
        /// 鼠标移上去的字体颜色
        /// </summary>
        public Brush MouseOverForeground
        {
            get
            {
                return (Brush)GetValue(MouseOverForegroundProperty);
            }
            set { SetValue(MouseOverForegroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseOverForegroundProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverForegroundProperty
            = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null, null));

        /// <summary>
        /// 鼠标按下去的字体颜色
        /// </summary>
        public Brush MouseDownForeground
        {
            get
            {
                return (Brush)GetValue(MouseDownForegroundProperty);
            }
            set { SetValue(MouseDownForegroundProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseDownForegroundProperty.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty MouseDownForegroundProperty
            = DependencyProperty.Register("MouseDownForeground", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null, null));

        /// <summary>
        /// 鼠标移上去的边框颜色
        /// </summary>
        public Brush MouseOverBorderBrush
        {
            get { return (Brush)GetValue(MouseOverBorderBrushProperty); }
            set { SetValue(MouseOverBorderBrushProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseOverBorderBrushProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBorderBrushProperty
            = DependencyProperty.Register("MouseOverBorderBrush", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null, null));

        /// <summary>
        /// 鼠标按下去的边框颜色
        /// </summary>
        public Brush MouseDownBorderBrush
        {
            get { return (Brush)GetValue(MouseDownBorderBrushProperty); }
            set { SetValue(MouseDownBorderBrushProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MouseDownBorderBrushProperty.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty MouseDownBorderBrushProperty
            = DependencyProperty.Register("MouseDownBorderBrush", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null, null));

        /// <summary>
        /// 圆角
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CornerRadiusProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ImageButton), null);

        /// <summary>
        /// 图标
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ImageButton), null);

        /// <summary>
        /// 鼠标移上去的图标图标
        /// </summary>
        public ImageSource IconMouseOver
        {
            get { return (ImageSource)GetValue(IconMouseOverProperty); }
            set { SetValue(IconMouseOverProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconMouseOverProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconMouseOverProperty
            = DependencyProperty.Register("IconMouseOver", typeof(ImageSource), typeof(ImageButton), null);

        /// <summary>
        /// 鼠标按下去的图标图标
        /// </summary>
        public ImageSource IconPress
        {
            get { return (ImageSource)GetValue(IconPressProperty); }
            set { SetValue(IconPressProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconPressProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconPressProperty
            = DependencyProperty.Register("IconPress", typeof(ImageSource), typeof(ImageButton), null);

        /// <summary>
        /// 图标高度
        /// </summary>
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconHeightProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty
            = DependencyProperty.Register("IconHeight", typeof(double), typeof(ImageButton), new PropertyMetadata(24.0, null));

        /// <summary>
        /// 图标宽度
        /// </summary>
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconWidthProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty
            = DependencyProperty.Register("IconWidth", typeof(double), typeof(ImageButton), new PropertyMetadata(24.0, null));

        /// <summary>
        /// 图标和内容的对齐方式  
        /// </summary>
        public Orientation IconContentOrientation
        {
            get { return (Orientation)GetValue(IconContentOrientationProperty); }
            set { SetValue(IconContentOrientationProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconContentOrientationProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconContentOrientationProperty
            = DependencyProperty.Register("IconContentOrientation", typeof(Orientation), typeof(ImageButton), new PropertyMetadata(Orientation.Horizontal, null));

        /// <summary>
        /// 图标和内容的距离  
        /// </summary>
        public Thickness IconContentMargin
        {
            get { return (Thickness)GetValue(IconContentMarginProperty); }
            set { SetValue(IconContentMarginProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IconContentMarginProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconContentMarginProperty
            = DependencyProperty.Register("IconContentMargin", typeof(Thickness), typeof(ImageButton), new PropertyMetadata(new Thickness(0, 0, 0, 0), null));
    }
}
