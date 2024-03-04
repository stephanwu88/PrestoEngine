using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Engine.Util.Controls
{
    /// <summary>
    /// 样品状态
    /// </summary>
    public enum SampleStatus
    {
        NORMAL,     //正常
        WARNING,    //样品异常
        WAITBACK,   //样品离开，等待返回
    }

    /// <summary>
    /// 激发点显示模式
    /// </summary>
    public enum ExciterShowMode
    {
        BeforeExciter,  //激发前显示，绿色
        AfterExciter,   //激发后显示，红色
    }

    /// <summary>
    /// ucExciterArea.xaml 的交互逻辑
    /// </summary>
    public partial class ExciterArea : UserControl
    {
        private bool _IsAvailable = true;
        private string _ExciterCode = string.Empty;
        private SampleStatus _SampleStatus;
        private List<Ellipse> _SparkGroup = new List<Ellipse>();
        private ExciterShowMode _ExciterShowMode = ExciterShowMode.AfterExciter;
        public ExciterArea()
        {
            InitializeComponent();

            _SparkGroup.Clear();
            _SparkGroup.Add(_Spark1);
            _SparkGroup.Add(_Spark2);
            _SparkGroup.Add(_Spark3);
            _SparkGroup.Add(_Spark4);
            _SparkGroup.Add(_Spark5);
            _SparkGroup.Add(_Spark6);
            _SparkGroup.Add(_Spark7);
            _SparkGroup.Add(_Spark8);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        
        }

        private void _PosEN_Checked(object sender, RoutedEventArgs e)
        {
            this._SampleGraph.Visibility = Visibility.Visible;
        }

        private void _PosEN_Unchecked(object sender, RoutedEventArgs e)
        {
            this._SampleGraph.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 托盘是否有效
        /// </summary>
        public bool IsAvailable
        {
            get => _IsAvailable;
            set
            {
                _IsAvailable = value;
                if (_IsAvailable)
                {
                    this._Panel.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xBB, 0xBB, 0xBB));
                    this._PanelDesc.Content = "";
                }
                else
                {
                    this._Panel.Fill = new SolidColorBrush(Colors.Yellow);
                    this._PanelDesc.Content = "NA";
                }
            }
        }

        /// <summary>
        /// 托盘有无配置样品
        /// </summary>
        public bool HasSample
        {
            get => this._PosEN.IsChecked == true ? true : false;
            set
            {
                this._PosEN.IsChecked = value;
            }
        }

        /// <summary>
        /// 样品位置标识
        /// </summary>
        public string PosID
        {
            get => this._PosID.Content.ToString();
            set
            {
                this._PosID.Content = value;
            }
        }

        /// <summary>
        /// 样品名称
        /// </summary>
        public string SampleName
        {
            get => this._SampleID.Text.ToString();
            set
            {
                this._SampleID.Text = value;
            }
        }

        /// <summary>
        /// 样品类型
        /// </summary>
        public string SampleType
        {
            get => this._SampleType.Content.ToString();
            set
            {
                this._SampleType.Content = value;
            }
        }

        /// <summary>
        /// 样品状态
        /// </summary>
        public SampleStatus SampleStatus
        {
            get => _SampleStatus;
            set
            {
                _SampleStatus = value;
                this._StatusWarning.Visibility = _SampleStatus == SampleStatus.WARNING ? Visibility.Visible : Visibility.Hidden;
                this._StatusWaitBack.Visibility = _SampleStatus == SampleStatus.WAITBACK ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /// <summary>
        /// 激发点显示模式，激发前
        /// </summary>
        public ExciterShowMode ExciterShowMode
        {
            get => _ExciterShowMode;
            set
            {
                _ExciterShowMode = value;
                if (_ExciterShowMode == ExciterShowMode.BeforeExciter)
                    this._SampleEdge.Fill = new SolidColorBrush(Colors.White);
                else
                    this._SampleEdge.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x89, 0x89, 0x89));
            }
        }

        /// <summary>
        /// 激发点编码
        /// </summary>
        public string ExcitePoint
        {
            get => _ExciterCode;
            set
            {
                //if (_PosEN.IsChecked == true && _IsAvailable)
                //{
                _ExciterCode = value.Trim();
                _ExciterCode = _ExciterCode.PadRight(8, '0');
                //}
                //else
                //{
                //    _ExciterCode = "00000000";
                //}
                for (int i = 0; i < 8; i++)
                {
                    string strEN = _ExciterCode.Substring(i, 1);
                    if (strEN == "1")
                        this._SparkGroup[i].Fill = _ExciterShowMode == ExciterShowMode.BeforeExciter ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.White);
                    else
                        this._SparkGroup[i].Fill = _ExciterShowMode == ExciterShowMode.BeforeExciter ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
                }
            }
        }
    }
}
