using Engine.Common;
using Engine.Data.DBFAC;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Core.TaskSchedule
{
    
    /// <summary>
    /// PageScheduleContentOfGate.xaml 的交互逻辑
    /// </summary>
    public partial class PageScheduleContentOfGate : UserControl
    {
        private bool uc_isLoaded = false;
        private string _Authority;
        private IDBFactory<ServerNode> _DB = DbFactory.Task.CloneInstance("TaskSchedule");
        private int schedule_id = 0;
        private int GateContentRowID = 0;  //当前被编辑的触发器选中行
        private int EditGateID = 0;        //当前被编辑的触发器ID
        private ObservableCollection<Gate> Schedule_Gate = new ObservableCollection<Gate>();
        //ObservableCollection<Gate> Schedule_Ga1te { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authority">ReadOnly Add Edit</param>
        public PageScheduleContentOfGate(string authority)
        {
            InitializeComponent();
            _Authority = authority;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_Authority == "ReadOnly")
            {
                this._CmdNewGate.Visibility = Visibility.Hidden;
                this._CmdEditGate.Visibility = Visibility.Hidden;
                this._CmdDelGate.Visibility = Visibility.Hidden;
                this._CmdMoveUp.Visibility = Visibility.Hidden;
                this._CmdMoveDown.Visibility = Visibility.Hidden;
                //绑定事件 ucScheduleContent选中数据变化时，只读选项卡需要同步显示
                //this.Tag = ucScheduleContent.xaml
                if (this.Tag != null)
                    (this.Tag as PageScheduleContent).ScheduleContent_SelectionChanged += ScheduleContent_SelectionChanged;
            }
            else if(_Authority == "Edit" || _Authority == "Add")
            {
                this._CmdNewGate.Visibility = Visibility.Visible;
                this._CmdEditGate.Visibility = Visibility.Visible;
                this._CmdDelGate.Visibility = Visibility.Visible;
                this._CmdMoveUp.Visibility = Visibility.Visible;
                this._CmdMoveDown.Visibility = Visibility.Visible;
            }
            if (_Authority == "Edit" && !uc_isLoaded)
            {
                if (schedule_id > 0)
                    Update_Gate_View(schedule_id);
                uc_isLoaded = true;
            }
        }

        /// <summary>
        /// 显示（只读）模式下绑定页面更新事件，由用户操作引发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg2"></param>
        private void ScheduleContent_SelectionChanged(PageScheduleContent sender, ScheduleContent arg2, ParamSetting arg3)
        {
            if (arg2 == null)
                return;
            ScheduleContent content = arg2 as ScheduleContent;
            int id = content.ID.ToMyInt();
            if (id > 0)
            {
                Update_Gate_View(id);
            }
        }

        /// <summary>
        /// 窗口加载时，只读直接绑定数据库，编辑绑定到泛型（后续可编辑）
        /// </summary>
        /// <param name="schedule_index"></param>
        private void Update_Gate_View(int schedule_index)
        {
            Schedule_Gate.Clear();
            string strSql = string.Format("select * from core_schedule_items where schedule_id={0} and Length(gate_detail)>0 order by gate_order asc", schedule_index);
            DataTable dt = _DB.ExcuteQuery(strSql).Result.ToMyDataTable();
            if (_Authority == "Edit")
            {
                foreach (DataRow Row in dt.Rows)
                {
                    Gate gate = new Gate();
                    gate.id = Row["id"].ToString();
                    gate.gate_enable = Row["gate_enable"].ToString();
                    gate.gate_type = Row["gate_type"].ToString();
                    gate.gate_detail = Row["gate_detail"].ToString();
                    gate.gate_state = Row["gate_state"].ToString();
                    gate.gate_pos = Row["gate_pos"].ToString();
                    gate.gate_item = Row["gate_item"].ToString();
                    gate.gate_object = Row["gate_object"].ToString();
                    gate.gate_sign = Row["gate_sign"].ToString();
                    gate.gate_order = Row["gate_order"].ToString();
                    gate.gate_val = Row["gate_val"].ToString();
                    Schedule_Gate.Add(gate);
                }
            }
            if (_Authority == "Edit")
                _dgContentOfGate.ItemsSource = Schedule_Gate;
            else
                _dgContentOfGate.ItemsSource = dt.DefaultView;

        }

        #region  test
        /// <summary>
        /// 操纵泛型，改变触发器变更后的显示，但不进入数据库，由最后确定后一并调整数据库
        /// </summary>
        private void Change_Gate_View(winNewGate sender, Gate arg2, string oper_mode)
        {
            //解析事件端口发送过来的数据
            Gate GateContent = arg2;

            //添加门控项
            if (GateContent != null && _Authority == "Edit")
            {
                if (oper_mode == "Add")
                {
                    GateContent.id = "0";  //id为0表示添加项
                    Schedule_Gate.Add(GateContent);
                }
                else if (oper_mode == "Edit")
                {
                    //若是数据库原始记录，恢复其索引号
                    GateContent.id = EditGateID.ToString();    
                    //恢复当时进入编辑窗口时加载的的选中行，修改项
                    int gateRowId = GateContentRowID;
                    Schedule_Gate[gateRowId] = GateContent;
                    Schedule_Gate[gateRowId].tag = "update";
                }
            }
            this._dgContentOfGate.ItemsSource = Schedule_Gate.Where(x => x.tag != "del");
        }
        #endregion

        /// <summary>
        /// 新建门控条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdNewGate_Click(object sender, RoutedEventArgs e)
        {
            winNewGate win = new winNewGate("Add");
            if ((this.Tag as Window).OwnedWindows.Count == 0)
            {
                win.Owner = this.Tag as Window;
                win.ScheduleID = schedule_id;
                win.GateChanged += Change_Gate_View;
                win.Show();
            }
        }

        /// <summary>
        /// 编辑门控条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdEditGate_Click(object sender, RoutedEventArgs e)
        {
            Gate row = _dgContentOfGate.SelectedItem as Gate;
            if (row != null)
            {
                winNewGate win = new winNewGate("Edit", schedule_id);
                if ((this.Tag as Window).OwnedWindows.Count == 0)
                {
                    win.Owner = this.Tag as Window;
                    win.ScheduleID = schedule_id;
                    //记录门控项ID
                    EditGateID = row.id.ToMyInt();
                    //将表格的选中行作为更新时的操作索引
                    GateContentRowID = _dgContentOfGate.SelectedIndex;
                    win.GateID = GateContentRowID;
                    win.GateDefaultContent = row;
                    win.GateChanged += Change_Gate_View;
                    win.Show();
                }
            }
        }

        /// <summary>
        /// 删除门控条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdDelGate_Click(object sender, RoutedEventArgs e)
        {
            Gate row = _dgContentOfGate.SelectedItem as Gate;
            if (row != null)
            {
                int gateRowId = _dgContentOfGate.SelectedIndex;
                Schedule_Gate[gateRowId].tag = "del";
                this._dgContentOfGate.ItemsSource = Schedule_Gate.Where(x => x.tag != "del");
            }
        }

        /// <summary>
        /// 记录上移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdMoveUp_Click(object sender, RoutedEventArgs e)
        {
            Gate row = _dgContentOfGate.SelectedItem as Gate;
            Gate tempRow = new Gate();
            int rowId = _dgContentOfGate.SelectedIndex;
            if (row != null)
            {
                if (rowId > 0)
                {
                    if (Schedule_Gate[rowId + 1].tag != "del")
                        Schedule_Gate[rowId + 1].tag = "update";
                    if (Schedule_Gate[rowId].tag != "del")
                        Schedule_Gate[rowId].tag = "update";
                    tempRow = Schedule_Gate[rowId - 1];
                    Schedule_Gate[rowId - 1] = Schedule_Gate[rowId];
                    Schedule_Gate[rowId] = tempRow;
                }
            }
            this._dgContentOfGate.ItemsSource = Schedule_Gate.Where(x => x.tag != "del");
        }

        private void _CmdMoveDown_Click(object sender, RoutedEventArgs e)
        {
            Gate row = _dgContentOfGate.SelectedItem as Gate;
            Gate tempRow = new Gate();
            int rowId = _dgContentOfGate.SelectedIndex;
            if (row != null)
            {
                if (rowId <= _dgContentOfGate.Items.Count - 2)
                {
                    if (Schedule_Gate[rowId + 1].tag != "del")
                        Schedule_Gate[rowId + 1].tag = "update";
                    if (Schedule_Gate[rowId].tag != "del")
                        Schedule_Gate[rowId].tag = "update";
                    tempRow = Schedule_Gate[rowId + 1];
                    Schedule_Gate[rowId + 1] = Schedule_Gate[rowId];
                    Schedule_Gate[rowId] = tempRow;
                }
            }
            this._dgContentOfGate.ItemsSource = Schedule_Gate.Where(x => x.tag != "del");
        }

        /// <summary>
        /// 编辑时需要的计划ID号
        /// </summary>
        public int Schedule_ID
        {
            get => schedule_id;
            set
            {
                schedule_id = value;
            }
        }

        /// <summary>
        /// 发送到父窗口消息
        /// </summary>
        public ObservableCollection<Gate> ScheduleOfGate
        {
            get => Schedule_Gate;
        }
    }
}
