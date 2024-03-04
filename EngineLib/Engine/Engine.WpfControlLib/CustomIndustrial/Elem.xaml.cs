using Engine.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.WpfControl
{
    /// <summary>
    /// 控件权限属性
    /// </summary>
    public enum Authority
    {
        READ,
        READWRITE
    }

    /// <summary>
    /// Elem.xaml 的交互逻辑
    /// </summary>
    public partial class Elem : UserControl
    {
        private string strElemName = string.Empty;
        private string strElemView = string.Empty;
        private string strElemVal = string.Empty;
        public Elem()
        {
            InitializeComponent();
        }

        public Elem(string elemName,string elemView,string elemVal)
        {
            InitializeComponent();
            ElemName = elemName;
            ElemView = elemView;
            ElemVal = elemVal;
        }

        /// <summary>
        /// 元素名称
        /// </summary>
        public string ElemName
        {
            get => strElemName;
            set
            {
                strElemName = value;
            }
        }

        /// <summary>
        /// 元素显示名称
        /// </summary>
        public string ElemView
        {
            get => strElemView;
            set
            {
                strElemView = value;
                _ElemView.Content = strElemView;
            }
        }

        /// <summary>
        /// 元素值
        /// </summary>
        public string ElemVal
        {
            get => _ElemVal.Text.Trim().ToMyString();
            set
            {
                strElemVal = value;
                _ElemVal.Text = strElemVal;
            }
        }

        /// <summary>
        /// 可操作属性
        /// </summary>
        public Authority Authority
        {
            get => _ElemVal.IsReadOnly ? Authority.READ : Authority.READWRITE;
            set
            {
                _ElemVal.IsReadOnly = value == Authority.READ;
            }
        }

        private void _ElemVal_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    _ElemVal.Text = _ElemVal.Text.Trim().ToMyString();
                    break;
            }
        }

        private void _ElemVal_LostFocus(object sender, RoutedEventArgs e)
        {
            _ElemVal.Text = _ElemVal.Text.Trim().ToMyString();
        }
    }
}
