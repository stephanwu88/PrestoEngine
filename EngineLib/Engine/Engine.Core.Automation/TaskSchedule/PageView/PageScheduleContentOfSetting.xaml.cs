using System.Windows;
using System.Windows.Controls;

namespace Engine.Core.TaskSchedule
{

    /// <summary>
    /// PageScheduleContentOfSetting.xaml 的交互逻辑
    /// </summary>
    public partial class PageScheduleContentOfSetting : UserControl
    {
        private string _authority = string.Empty;
        private string _creator = string.Empty;
        private bool uc_isLoaded = false;
        private ParamSetting _param = new ParamSetting();
        public PageScheduleContentOfSetting(string authoruty)
        {
            InitializeComponent();
            _authority = authoruty;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this._TimeOut_Set.Items.Clear();
            this._TimeOut_Set.Items.Add("1min");
            this._TimeOut_Set.Items.Add("5min");
            this._TimeOut_Set.Items.Add("10min");
            this._TimeOut_Set.Items.Add("30min");
            this._TimeOut_Set.Items.Add("60min");
            this._TimeOut_Set.Items.Add("90min");
            this._TimeOut_Set.Items.Add("120min");
            this._TimeOut_Set.Items.Add("200min");
            this._TimeOut_Set.SelectedIndex = 1;

            if (_authority == "Add" || _authority == "Edit")
            {
                //设定窗口可输入
                this._TimeOut_Set.IsReadOnly = false;
                this._TimeOut_Set.IsEnabled = true;
                _IsEnable.IsEnabled = true;
            }
            if (_authority == "ReadOnly")
            {
                //绑定事件 ucScheduleContent选中数据变化时，只读选项卡需要同步显示
                //this.Tag = ucScheduleContent.xaml
                if (this.Tag != null)
                    (this.Tag as PageScheduleContent).ScheduleContent_SelectionChanged += ScheduleContent_SelectionChanged;
            }
            if (_authority == "Edit")
            {
                //在编辑窗口模式下，窗口加载时，将主窗口点选的信息加载到默认显示
                //this.Tag = ucScheduleContent.xaml
                ParamSetting CardParam = (this.Tag as PageScheduleContent).OptionCard_Setting;
                if (_TimeOut_Set.Items.Contains(CardParam.TimeOut))
                {
                    this._TimeOut_Set.Text = CardParam.TimeOut;
                    this._IsEnable.IsChecked = true;
                }
                else
                    this._IsEnable.IsChecked = false;
            }
        }

        /// <summary>
        /// 主窗口选中项发生变化
        /// </summary>
        /// <param name=""></param>
        private void ScheduleContent_SelectionChanged(PageScheduleContent sender, ScheduleContent arg2,ParamSetting arg3)
        {
            ParamSetting paramGroup = arg3;
            if (_TimeOut_Set.Items.Contains(paramGroup.TimeOut))
            {
                this._TimeOut_Set.Text = paramGroup.TimeOut;
                this._IsEnable.IsChecked = true;
            }
            else
            {
                this._IsEnable.IsChecked = false;
            }
            
        }

        public ParamSetting ParamGroup
        {
            get
            {
                if (_IsEnable.IsChecked == true)
                    _param.TimeOut = this._TimeOut_Set.Text;
                else
                    _param.TimeOut = string.Empty ;
                return _param;
            }
            set
            {
                _param = value;
            }
        }
    }
}
