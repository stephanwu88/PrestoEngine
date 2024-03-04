using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Engine.WpfControl
{
    /// <summary>
    /// ParEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ParamEditor :  UserControl   
    {   
        public event Action<object, string> ParamVal_Changed;
        public ParamEditor()
        {
            InitializeComponent();
            _ParVal.DataContext = this;
            //注意，这个事件的注册必须在LIKE_textBox获得焦点之前
            _ParVal.PreviewMouseDown += new MouseButtonEventHandler(_ParVal_PreviewMouseDown);
        }

        /// <summary>
        /// 数据地址
        /// </summary>
        private string _DataAddress;
        public string DataAddress
        {
            get => _DataAddress;
            set
            {
                if (_DataAddress != value)
                {
                    _DataAddress = value;
                }
            }
        }
        

        /// <summary>
        /// 数据类型
        /// </summary>
        private string _DataType;
        public string DataType
        {
            get => _DataType;
            set
            {
                if (_DataType != value)
                {
                    _DataType = value;
                }
            }
        }

        /// <summary>
        /// 参数标题
        /// </summary>
        private string _Title;
        public string Title
        {
            get => _Title;
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    _ParView.Content = _Title.Trim();
                }
            }
        }

        /// <summary>
        /// 参数值
        /// </summary>
        private string _ParamVal;
        public string ParamVal
        {
            get
            {
                return _ParamVal;
            } 
            set
            {
                if (_ParamVal != value)
                {
                    _ParamVal = value;
                    if (ParamVal_Changed != null)
                        ParamVal_Changed(this, value);
                }

            }
        }

        private string _ParamValTemp;
        public string ParamValTemp
        {
            get
            {
                return (string)GetValue(ParamValTempProperty);
            }
            set
            {
                SetValue(ParamValTempProperty, value);
                if (_ParamValTemp != value)
                {
                    if (ParamVal_Changed != null)
                        ParamVal_Changed(this, value);
                    _ParamValTemp = value;
                }
            }
        }

        public static readonly DependencyProperty ParamValTempProperty =
            DependencyProperty.Register("ParamValTemp", typeof(string), typeof(ParamEditor), new PropertyMetadata(""));

        public void _ParVal_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _ParVal.Focus();
            e.Handled = true;
        }

        private void _ParVal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //ParamVal = this._ParVal.Text.Trim();
                ParamValTemp = this._ParVal.Text.Trim();
            }
        }

        private void _ParVal_LostFocus(object sender, RoutedEventArgs e)
        {
            //ParamVal = this._ParVal.Text.Trim();
            //ParamValTemp = this._ParVal.Text.Trim();
            this._ParVal.Background = Brushes.White;
            _ParVal.PreviewMouseDown += new MouseButtonEventHandler(_ParVal_PreviewMouseDown);
        }

        private void _ParVal_GotFocus(object sender, RoutedEventArgs e)
        {
            this._ParVal.Background = Brushes.Orange;
            this._ParVal.SelectAll();
            _ParVal.PreviewMouseDown -= new MouseButtonEventHandler(_ParVal_PreviewMouseDown);
        }
    }
}
 