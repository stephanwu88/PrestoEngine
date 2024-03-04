using Engine.Common;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// winNewTrigger.xaml 的交互逻辑
    /// </summary>
    public partial class winNewTrigger : Window
    {
        private bool win_isLoaded = false;
        private string _Authority = string.Empty;
        //触发器所属的计划ID
        private int _schedule_id = 0;
        //触发器ID
        private int _trig_id = 0;
        private IDBFactory<ServerNode> _DB = DbFactory.Task;

        /// <summary>
        /// 编辑窗口自动获取的默认参数
        /// </summary>
        private Trigger _TriggerDefaultContent;
        /// <summary>
        /// 触发器发生变更事件
        /// </summary>
        public event Action<winNewTrigger, Trigger, string> TriggerChanged;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authority">指定窗口操作权限 Add or Edit</param>
        /// <param name="scheduleID">指定计划ID号</param>
        /// <param name="trigID">指定触发器ID号</param>
        /// <param name="TriggerDefaultContent">加载的默认参数</param>
        public winNewTrigger(string authority,int scheduleID = 0,int trigID = 0, Trigger triggerDefaultContent = null)
        {
            InitializeComponent();
            _Authority = authority;
            _schedule_id = scheduleID;
            _trig_id = trigID;
            _TriggerDefaultContent = triggerDefaultContent;
            switch (_Authority)
            {
                case "Add":
                    this.Title = "新建触发器";
                    break;

                case "Edit":
                    this.Title = "编辑触发器";
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string strTime = string.Empty;

            //添加任务开始依据
            this._TaskStartBy.Items.Clear();
            this._TaskStartBy.Items.Add("ByScheduleTime");
            this._TaskStartBy.Items.Add("ByStationState");
            this._TaskStartBy.SelectedIndex = 0;
            this._ModeEveryDay.IsChecked = true;

            //更新时间
            this._BaseDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            strTime = DateTime.Now.ToString("HH:mm:ss");
            _BaseTime.Time = strTime;

            //添加逻辑工位
            this._Pos.Items.Clear();
            Load_PosKey();
            this._Pos.SelectedIndex = 0;

            //添加逻辑符号
            this._Sign.Items.Clear();
            this._Sign.Items.Add("=");
            this._Sign.Items.Add("!=");
            this._Sign.SelectedIndex = 0;

            //添加目标值可选项
            this._ObjectVal.Items.Clear();
            this._ObjectVal.Items.Add("NORMAL");
            this._ObjectVal.Items.Add("RUN");
            this._ObjectVal.Items.Add("ALARM");
            this._ObjectVal.Items.Add("WAIT");
            this._ObjectVal.Items.Add("DONE");
            this._ObjectVal.Items.Add("OFFLINE");
            this._ObjectVal.Items.Add("ACCEPT");
            this._ObjectVal.Items.Add("ACCEPTED");
            this._ObjectVal.Items.Add("LOCK");
            this._ObjectVal.SelectedIndex = 0;

            //设置默认已启用状态
            this._Chk_Enable.IsChecked = true;

            if (_Authority == "Edit" && _TriggerDefaultContent != null)
            {
                this._TaskStartBy.SelectedItem = _TriggerDefaultContent.trig_type;
                this._Chk_Enable.IsChecked = _TriggerDefaultContent.trig_enable == "已启用" ? true : false;

                switch (_TriggerDefaultContent.trig_type)
                {
                    case "ByScheduleTime":
                        string strDetail = _TriggerDefaultContent.trig_detail;
                        if (strDetail.Split(',').Length >= 3)
                        {
                            string[] detail = strDetail.Split(',');
                            string baseDate = detail[0];
                            string baseTime = detail[1];
                            string timeSpan = detail[2];

                            _ModeOnce.IsChecked = timeSpan == "一次" ? true : false;
                            _ModeEveryDay.IsChecked = timeSpan == "每天" ? true : false;
                            _ModeEveryWeek.IsChecked = timeSpan == "每周" ? true : false;

                            _BaseDate.Text = baseDate;
                            _BaseTime.Time = baseTime;

                            foreach (var c in _Grid_Week.Children)
                            {
                                if (c is CheckBox)
                                {
                                    CheckBox cBox = c as CheckBox;
                                    cBox.IsChecked = strDetail.Contains(cBox.Content.ToString()) ? true : false;
                                }
                            }
                        }
                        break;

                    case "ByStationState":
                        this._Pos.Text = _TriggerDefaultContent.trig_pos;
                        this._Item.Text = _TriggerDefaultContent.trig_item;
                        this._Sign.Text = _TriggerDefaultContent.trig_sign;
                        this._ObjectVal.Text = _TriggerDefaultContent.trig_object;
                        break;

                }
            }
        }

        private void _TaskStartBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object selectObj = _TaskStartBy.SelectedItem;
            if (selectObj != null)
            {
                string item = selectObj.ToString();
                switch (item)
                {
                    case "ByScheduleTime":
                        _Border_ByScheduleTime.Visibility = Visibility.Visible;
                        _Border_ByStationState.Visibility = Visibility.Hidden;
                        this._ModeEveryDay.IsChecked = true;
                        break;

                    case "ByStationState":
                        _Border_ByScheduleTime.Visibility = Visibility.Hidden;
                        _Border_ByStationState.Visibility = Visibility.Visible;
                        this._Pos.SelectedIndex = 0;
                        break;
                }
            }
        }

        private void _ModeOnce_Checked(object sender, RoutedEventArgs e)
        {
            SetttingByTimeProperty(Visibility.Hidden, Visibility.Hidden);
        }

        private void _ModeEveryDay_Checked(object sender, RoutedEventArgs e)
        {
            SetttingByTimeProperty(Visibility.Visible, Visibility.Hidden);
        }

        private void _ModeEveryWeek_Checked(object sender, RoutedEventArgs e)
        {
            SetttingByTimeProperty(Visibility.Hidden, Visibility.Visible);
        }

        /// <summary>
        /// 根据ByTime设置可见
        /// </summary>
        /// <param name="MsgSetting">设置时间提示文字可见</param>
        /// <param name="Week">设置星期可选可见</param>
        private void SetttingByTimeProperty(Visibility MsgSetting,Visibility Week)
        {
            _MsgSetting.Visibility = MsgSetting;
            _ChkMon.Visibility = Week;
            _ChkTues.Visibility = Week;
            _ChkWed.Visibility = Week;
            _ChkThur.Visibility = Week;
            _ChkFir.Visibility = Week;
            _ChkSat.Visibility = Week;
            _ChkSun.Visibility = Week;
        }

        /// <summary>
        /// 加载工位可选择项
        /// </summary>
        private void Load_PosKey()
        {
            ModelSystemGroup modelGroup = new ModelSystemGroup()
            {
                MarkKey = string.Format(" MarkKey='{0}' order by OrderID asc", SystemGroupType.Symbol.ToString())
                                   .MarkExpress().MarkWhere()
            };
            List<string> PosItems = new List<string>();
            DataTable dt = _DB.ExcuteQuery<ModelSystemGroup>(modelGroup).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                string pos_key = row["Name"].ToString();
                PosItems.Add(pos_key);
            }
            _Pos.ItemsSource = PosItems;
        }

        private void _Pos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_Pos.SelectedItem == null)
                return;
            string strPosName = _Pos.SelectedItem.ToMyString();
            {
                ModelSystemSymbol modelSymbol = new ModelSystemSymbol()
                {
                    GroupName = string.Format("GroupName='{0}' and DataType='string' order by OrderID asc", strPosName)
                                        .MarkExpress().MarkWhere()
                };
                List<string> PosItems = new List<string>();
                DataTable dt = _DB.ExcuteQuery<ModelSystemSymbol>(modelSymbol).Result.ToMyDataTable();
                foreach (DataRow row in dt.Rows)
                {
                    string item_key = row["Name"].ToString();
                    PosItems.Add(item_key);
                }
                _Item.ItemsSource = PosItems;
                _Item.SelectedIndex = 0;
            }
        }

        private void _Cmd_Sure_Click(object sender, RoutedEventArgs e)
        {
            NewTrigger(_TaskStartBy.SelectedItem.ToString(),_Authority);
            this.Close();
        }

        private void NewTrigger(string StartBy,string authority)
        {
            Trigger TriggerContent = new Trigger();
            TriggerContent.trig_type = StartBy;
            TriggerContent.trig_enable = this._Chk_Enable.IsChecked == true ? "已启用" : "禁用";
            switch (StartBy)
            {
                case "ByScheduleTime":
                    string par1 = this._BaseDate.Text;
                    string par2 = this._BaseTime.Time;
                    string par3 = string.Empty;
                    if (this._ModeOnce.IsChecked == true)
                    {
                        par3 = _ModeOnce.Content.ToString();
                    }
                    else if (this._ModeEveryDay.IsChecked == true)
                    {
                        par3 = _ModeEveryDay.Content.ToString();
                    }
                    else if (this._ModeEveryWeek.IsChecked == true)
                    {
                        par3 = _ModeEveryWeek.Content.ToString();
                    }
                    TriggerContent.trig_detail = string.Format("{0},{1},{2}", par1, par2, par3);

                    if (this._ChkMon.IsChecked == true && this._ModeEveryWeek.IsChecked == true)
                        TriggerContent.trig_detail += "," + this._ChkMon.Content.ToString();
                    if (this._ChkTues.IsChecked == true && this._ModeEveryWeek.IsChecked == true)
                        TriggerContent.trig_detail += "," + this._ChkTues.Content.ToString();
                    if (this._ChkWed.IsChecked == true && this._ModeEveryWeek.IsChecked == true)
                        TriggerContent.trig_detail += "," + this._ChkWed.Content.ToString();
                    if (this._ChkThur.IsChecked == true && this._ModeEveryWeek.IsChecked == true)
                        TriggerContent.trig_detail += "," + this._ChkThur.Content.ToString();
                    if (this._ChkFir.IsChecked == true && this._ModeEveryWeek.IsChecked == true)
                        TriggerContent.trig_detail += "," + this._ChkFir.Content.ToString();
                    if (this._ChkSat.IsChecked == true && this._ModeEveryWeek.IsChecked == true)
                        TriggerContent.trig_detail += "," + this._ChkSat.Content.ToString();
                    if (this._ChkSun.IsChecked == true && this._ModeEveryWeek.IsChecked == true)
                        TriggerContent.trig_detail += "," + this._ChkSun.Content.ToString();
                    break;

                case "ByStationState":
                    TriggerContent.trig_pos = this._Pos.SelectedItem.ToString();
                    TriggerContent.trig_item = this._Item.SelectedItem.ToString();
                    TriggerContent.trig_sign = this._Sign.SelectedItem.ToString();
                    TriggerContent.trig_object = this._ObjectVal.Text.Trim();
                    TriggerContent.trig_detail = string.Format("{0}.{1}{2}{3}", TriggerContent.trig_pos, TriggerContent.trig_item, TriggerContent.trig_sign, TriggerContent.trig_object);
                    break;

            }
            //向事件端口发送消息
            if (TriggerChanged != null && (authority=="Add" || authority == "Edit"))
                TriggerChanged(this, TriggerContent, authority);
        }

        private void _Cmd_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 编辑时所需的计划ID
        /// </summary>
        public int ScheduleID
        {
            get => _schedule_id;
            set
            {
                _schedule_id = value;
            }
        }

        /// <summary>
        /// 编辑时所需的触发器ID
        /// </summary>
        public int TrigID
        {
            get => _trig_id;
            set
            {
                _trig_id = value;
            }
        }

        public Trigger TriggerDefaultContent
        {
            get => _TriggerDefaultContent;
            set
            {
                _TriggerDefaultContent = value;
            }
        }
    }
}
