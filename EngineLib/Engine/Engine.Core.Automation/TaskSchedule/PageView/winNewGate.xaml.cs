using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Engine.Data.DBFAC;
using System.Data;
using Engine.Common;
using Engine.Mod;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// winNewGate.xaml 的交互逻辑
    /// </summary>
    public partial class winNewGate : Window
    {
        private bool win_isLoaded = false;
        private string _Authority = string.Empty;
        //门控组所属的计划ID
        private int _schedule_id = 0;
        //门控项ID
        private int _gate_id = 0;
        private IDBFactory<ServerNode> _DB = DbFactory.Task;

        /// <summary>
        /// 编辑窗口自动获取的默认参数
        /// </summary>
        private Gate _GateDefaultContent;
        /// <summary>
        /// 触发器发生变更事件
        /// </summary>
        public event Action<winNewGate, Gate, string> GateChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authority">指定窗口操作权限 Add or Edit</param>
        /// <param name="scheduleID">指定计划ID</param>
        /// <param name="gateID">指定门控项ID</param>
        /// <param name="GateDefaultContent">加载默认界面参数</param>
        public winNewGate(string authority, int scheduleID = 0, int gateID = 0, Gate gateDefaultContent = null)
        {
            InitializeComponent();
            _Authority = authority;
            _schedule_id = scheduleID;
            _gate_id = gateID;
            _GateDefaultContent = gateDefaultContent;
            switch (_Authority)
            {
                case "Add":
                    this.Title = "新建门控条件";
                    break;

                case "Edit":
                    this.Title = "编辑门控条件";
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //添加门控类型
            _GateType.Items.Clear();
            _GateType.Items.Add("StationItem");
            _GateType.Items.Add("ScheduleItem");
            _GateType.SelectedIndex = 0;

            //添加逻辑符号
            _Sign.Items.Clear();
            _Sign.Items.Add("=");
            _Sign.Items.Add("!=");
            _Sign.SelectedIndex = 0;

            //添加目标值可选项
            _ObjectVal.Items.Clear();
            _ObjectVal.Items.Add("NORMAL");
            _ObjectVal.Items.Add("RUN");
            _ObjectVal.Items.Add("ALARM");
            _ObjectVal.Items.Add("WAIT");
            _ObjectVal.Items.Add("DONE");
            _ObjectVal.Items.Add("OFFLINE");
            _ObjectVal.Items.Add("ACCEPT");
            _ObjectVal.Items.Add("ACCEPTED");
            _ObjectVal.Items.Add("LOCK");
            _ObjectVal.SelectedIndex = 0;


            if (_Authority == "Edit" && _GateDefaultContent != null)
            {
                _GateType.SelectedItem = _GateDefaultContent.gate_type;
                _Chk_Enable.IsChecked = _GateDefaultContent.gate_enable == "已启用" ? true : false;
                _Group.SelectedItem = _GateDefaultContent.gate_pos;
                _Item.SelectedItem = _GateDefaultContent.gate_item;
                _Sign.Text = _GateDefaultContent.gate_sign;
                _ObjectVal.Text = _GateDefaultContent.gate_object;
            }
        }

        /// <summary>
        /// 加载工位可选择项
        /// </summary>
        private void Load_Group(string gateType)
        {
            string strSql = string.Empty;
            string key = string.Empty;
            switch (gateType)
            {
                case "StationItem":
                    ModelSystemGroup modelGroup = new ModelSystemGroup()
                    {
                        MarkKey = string.Format(" MarkKey='{0}' order by OrderID asc", SystemGroupType.Symbol.ToString())
                                .MarkExpress().MarkWhere()
                    };
                    strSql = _DB.SqlQuery<ModelSystemGroup>(modelGroup).ToMyString();
                    key = "pos_key";
                    break;

                case "ScheduleItem":
                    strSql = "select id,name from core_schedule_menu order by name asc";
                    key = "name";
                    break;
            }
            if (strSql.Length > 0)
            {
                List<string> PosItems = new List<string>();
                DataTable dt = _DB.ExcuteQuery(strSql).Result.ToMyDataTable();
                foreach (DataRow row in dt.Rows)
                {
                    if (key == "pos_key")
                    {
                        string PosKey = row["Name"].ToMyString();
                        PosItems.Add(PosKey);
                    }
                    else
                    {
                        string PosKey = row["name"].ToMyString();
                        PosItems.Add(PosKey);
                    }
                }
                _Group.ItemsSource = PosItems;
                _Group.SelectedIndex = 0;
            }
        }

        private void _GateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object gateType = _GateType.SelectedItem;
            if (gateType != null)
            {
                Load_Group(gateType.ToString());
                string strGateType = gateType.ToString();
                if (strGateType == "ScheduleItem")
                {
                    List<string> item = new List<string>();
                    item.Add("state");
                    _Item.ItemsSource = item;
                    _Item.SelectedIndex = 0;
                    return;
                }
            }
        }

        private void _Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_GateType.SelectedItem == null || _Group.SelectedItem == null)
                return;
            string strGateType = _GateType.SelectedItem.ToMyString();
            string strPosName = _Group.SelectedItem.ToMyString();
            if (strGateType == "StationItem")
            {
                ModelSystemSymbol modelSymbol = new ModelSystemSymbol()
                {
                    GroupName = string.Format("GroupName='{0}'and DataType='string' order by OrderID asc", strPosName)
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
            else if (strGateType == "ScheduleItem")
            {
                List<string> item = new List<string>();
                item.Add("state");
                _Item.ItemsSource = item;
                _Item.SelectedIndex = 0;
            }
        }

        private void _Cmd_Sure_Click(object sender, RoutedEventArgs e)
        {
            NewGate(_GateType.SelectedItem.ToString(),_Authority);
            Close();
        }

        private void NewGate(string GateType,string authority)
        {
            Gate GateContent = new Gate();
            if (GateType == "ScheduleItem" || GateType == "StationItem")
            {
                GateContent.gate_type = GateType;
                GateContent.gate_enable = _Chk_Enable.IsChecked == true ? "已启用" : "禁用";
                GateContent.gate_pos = _Group.SelectedItem.ToString();
                GateContent.gate_item = _Item.SelectedItem.ToString();
                GateContent.gate_sign = _Sign.SelectedItem.ToString();
                GateContent.gate_object = _ObjectVal.Text.ToString().Trim();

                string detail = string.Format("{0}.{1}={2}", GateContent.gate_pos, GateContent.gate_item, GateContent.gate_object);
                GateContent.gate_detail = detail;
            }

            //向事件端口发送消息
            if (GateChanged != null && (authority == "Add" || authority == "Edit"))
                GateChanged(this, GateContent, authority);
        }

        private void _Cmd_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
        public int GateID
        {
            get => _gate_id;
            set
            {
                _gate_id = value;
            }
        }

        public Gate GateDefaultContent
        {
            get => _GateDefaultContent;
            set
            {
                _GateDefaultContent = value;
            }
        }
    }
}
