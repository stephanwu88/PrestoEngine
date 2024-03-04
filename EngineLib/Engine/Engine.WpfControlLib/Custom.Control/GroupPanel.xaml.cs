using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Engine.WpfControl
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupPanel : Expander
    {
        static GroupPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupPanel), new FrameworkPropertyMetadata(typeof(GroupPanel)));
        }

        public string HeaderIcon
        {
            get { return (string)GetValue(HeaderIconProperty); }
            set { SetValue(HeaderIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderIconProperty =
            DependencyProperty.Register("HeaderIcon", typeof(string), typeof(GroupPanel), new PropertyMetadata(""));

        public string  HeaderComment
        {
            get { return (string )GetValue(HeaderCommentProperty); }
            set { SetValue(HeaderCommentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderComment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderCommentProperty =
            DependencyProperty.Register("HeaderComment", typeof(string ), typeof(GroupPanel), new PropertyMetadata(""));


        public GridLength HeaderHeight
        {
            get { return (GridLength)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register("HeaderHeight", typeof(GridLength), typeof(GroupPanel), new PropertyMetadata(GridLength.Auto));



        /// <summary>
        /// 背景色
        /// </summary>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(GroupPanel), new PropertyMetadata(Brushes.Transparent));





        /// <summary>
        /// 是否可展开
        /// </summary>
        public bool Expandable
        {
            get { return (bool)GetValue(ExpandableProperty); }
            set { SetValue(ExpandableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Expandable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandableProperty =
            DependencyProperty.Register("Expandable", typeof(bool), typeof(GroupPanel), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public bool Contractable
        {
            get { return (bool)GetValue(ContractableProperty); }
            set { SetValue(ContractableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Contractable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContractableProperty =
            DependencyProperty.Register("Contractable", typeof(bool), typeof(GroupPanel), new PropertyMetadata(true));
    }
}
