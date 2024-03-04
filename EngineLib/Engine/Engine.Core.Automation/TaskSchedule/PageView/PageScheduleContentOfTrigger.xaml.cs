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
    /// PageScheduleContentOfTrigger.xaml 的交互逻辑
    /// </summary>
    public partial class PageScheduleContentOfTrigger : UserControl
    {
        private bool uc_isLoaded = false;
        private string _Authority = string.Empty;
        private IDBFactory<ServerNode> _DB = DbFactory.Task.CloneInstance("TaskSchedule");
        private int schedule_id = 0;
        private int TriggerContentRowID = 0;  //当前被编辑的触发器选中行
        private int EditTriggerID = 0;        //当前被编辑的触发器ID
        private ObservableCollection<Trigger> Schedule_Trigger = new ObservableCollection<Trigger>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authority">ReadOnly Add Edit</param>
        public PageScheduleContentOfTrigger(string authority = "ReadOnly")
        {
            _Authority = authority;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_Authority == "ReadOnly")
            {
                this._CmdNewTrigger.Visibility = Visibility.Hidden;
                this._CmdEditTrigger.Visibility = Visibility.Hidden;
                this._CmdDelTrigger.Visibility = Visibility.Hidden;
                this._CmdMoveUp.Visibility = Visibility.Hidden;
                this._CmdMoveDown.Visibility = Visibility.Hidden;
                //绑定事件 ucScheduleContent选中数据变化时，只读选项卡需要同步显示
                //this.Tag = ucScheduleContent.xaml
                if (this.Tag != null)
                    (this.Tag as PageScheduleContent).ScheduleContent_SelectionChanged += ScheduleContent_SelectionChanged;
            }
            else if (_Authority == "Edit" || _Authority == "Add")  
            {
                this._CmdNewTrigger.Visibility = Visibility.Visible;
                this._CmdEditTrigger.Visibility = Visibility.Visible;
                this._CmdDelTrigger.Visibility = Visibility.Visible;
                this._CmdMoveUp.Visibility = Visibility.Visible;
                this._CmdMoveDown.Visibility = Visibility.Visible;
            }
            if (_Authority == "Edit" && !uc_isLoaded)
            {
                if (schedule_id > 0)
                    Update_Trigger_View(schedule_id);
                uc_isLoaded = true;
            }
        }

        /// <summary>
        /// 显示（只读）模式下绑定页面更新事件，由用户操作引发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg2"></param>
        private void ScheduleContent_SelectionChanged(PageScheduleContent sender, ScheduleContent arg2,ParamSetting arg3)
        {
            if (arg2 == null)
                return;
            ScheduleContent content = arg2 as ScheduleContent;
            int id = content.ID.ToMyInt();
            if (id > 0)
            {
                Update_Trigger_View(id);
            }
        }

        /// <summary>
        /// 窗口加载时，只读直接绑定数据库，编辑绑定到泛型（后续可编辑）
        /// </summary>
        /// <param name="schedule_index"></param>
        private void Update_Trigger_View(int schedule_index)
        {
            Schedule_Trigger.Clear();
            string strSql = string.Format("select * from core_schedule_items where schedule_id={0} and Length(trig_detail)>0 order by trig_order asc", schedule_index);
            DataTable dt = _DB.ExcuteQuery(strSql).Result.ToMyDataTable();
            if (_Authority == "Edit")
            {
                foreach (DataRow row in dt.Rows)
                {
                    Trigger trig = new Trigger();
                    trig.id = row["id"].ToString();
                    trig.trig_enable = row["trig_enable"].ToString();
                    trig.trig_type = row["trig_type"].ToString();
                    trig.trig_detail = row["trig_detail"].ToString();
                    trig.trig_state = row["trig_state"].ToString();
                    trig.trig_pos = row["trig_pos"].ToString();
                    trig.trig_item = row["trig_item"].ToString();
                    trig.trig_object = row["trig_object"].ToString();
                    trig.trig_val = row["trig_val"].ToString();
                    trig.trig_sign = row["trig_sign"].ToString();
                    trig.trig_order = row["trig_order"].ToString();
                    Schedule_Trigger.Add(trig);
                }
            }
            if (_Authority == "Edit")
                this._dgContentOfTrigger.ItemsSource = Schedule_Trigger;
            else
                this._dgContentOfTrigger.ItemsSource = dt.DefaultView;

        }

        /// <summary>
        /// 操纵泛型，改变触发器变更后的显示，但不进入数据库，由最后确定后一并调整数据库
        /// </summary>
        private void Change_Trigger_View(winNewTrigger sender, Trigger arg2,string oper_mode)
        {
            //接受事件端口发送过来的数据
            Trigger TriggerContent = arg2;

            //操作数据项
            if (TriggerContent != null && _Authority == "Edit")
            {
                if (oper_mode == "Add")
                {
                    TriggerContent.id = "0";  //id为0表示添加项
                    Schedule_Trigger.Add(TriggerContent);
                }
                else if (oper_mode == "Edit")
                {
                    //若是数据库原始记录，恢复其索引号
                    TriggerContent.id = EditTriggerID.ToString();
                    //恢复当时进入编辑窗口时加载的的选中行，修改项
                    int trigRowId = TriggerContentRowID;
                    Schedule_Trigger[trigRowId] = TriggerContent;
                    Schedule_Trigger[trigRowId].tag = "update";
                }
            }
            this._dgContentOfTrigger.ItemsSource = Schedule_Trigger.Where(x => x.tag != "del");
        }

        /// <summary>
        /// 新建触发器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdNewTrigger_Click(object sender, RoutedEventArgs e)
        {
            winNewTrigger win = new winNewTrigger("Add");
            if ((this.Tag as Window).OwnedWindows.Count == 0)
            {
                win.Owner = this.Tag as Window;
                win.ScheduleID = schedule_id;
                win.TriggerChanged += Change_Trigger_View;
                win.Show();
            }
        }

        /// <summary>
        /// 编辑触发器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdEditTrigger_Click(object sender, RoutedEventArgs e)
        {
            Trigger row = _dgContentOfTrigger.SelectedItem as Trigger;
            if (row != null)
            {
                winNewTrigger win = new winNewTrigger("Edit", schedule_id);
                if ((this.Tag as Window).OwnedWindows.Count == 0)
                {
                    win.Owner = this.Tag as Window;
                    win.ScheduleID = schedule_id;
                    //记录触发器ID
                    EditTriggerID = row.id.ToMyInt();
                    //将表格的选中行作为更新时的操作索引
                    TriggerContentRowID = _dgContentOfTrigger.SelectedIndex;
                    win.TrigID = TriggerContentRowID;
                    win.TriggerDefaultContent = row;
                    win.TriggerChanged += Change_Trigger_View;
                    win.Show();
                }
            }
        }

        /// <summary>
        /// 删除触发器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdDelTrigger_Click(object sender, RoutedEventArgs e)
        {
            Trigger row = _dgContentOfTrigger.SelectedItem as Trigger;
            if (row != null)
            {
                int trigRowId = _dgContentOfTrigger.SelectedIndex;
                Schedule_Trigger[trigRowId].tag = "del";
                this._dgContentOfTrigger.ItemsSource = Schedule_Trigger.Where(x => x.tag != "del");
            }
        }

        /// <summary>
        /// 记录上移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdMoveUp_Click(object sender, RoutedEventArgs e)
        {
            Trigger row = _dgContentOfTrigger.SelectedItem as Trigger;
            Trigger tempRow = new Trigger();
            int rowId = _dgContentOfTrigger.SelectedIndex;
            if (row != null)
            {
                if (rowId > 0)
                {
                    if (Schedule_Trigger[rowId - 1].tag != "del")
                        Schedule_Trigger[rowId - 1].tag = "update";
                    if (Schedule_Trigger[rowId].tag != "del")
                        Schedule_Trigger[rowId].tag = "update";
                    tempRow = Schedule_Trigger[rowId - 1];
                    Schedule_Trigger[rowId - 1] = Schedule_Trigger[rowId];
                    Schedule_Trigger[rowId] = tempRow;
                }
            }
            this._dgContentOfTrigger.ItemsSource = Schedule_Trigger.Where(x => x.tag != "del");
        }

        /// <summary>
        /// 记录下移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdMoveDown_Click(object sender, RoutedEventArgs e)
        {
            Trigger row = _dgContentOfTrigger.SelectedItem as Trigger;
            Trigger tempRow = new Trigger();
            int rowId = _dgContentOfTrigger.SelectedIndex;
            if (row != null)
            {
                if (rowId <= _dgContentOfTrigger.Items.Count - 2)
                {
                    if (Schedule_Trigger[rowId + 1].tag != "del")
                        Schedule_Trigger[rowId + 1].tag = "update";
                    if (Schedule_Trigger[rowId].tag != "del")
                        Schedule_Trigger[rowId].tag = "update";
                    tempRow = Schedule_Trigger[rowId + 1];
                    Schedule_Trigger[rowId + 1] = Schedule_Trigger[rowId];
                    Schedule_Trigger[rowId] = tempRow;
                }
            }
            this._dgContentOfTrigger.ItemsSource = Schedule_Trigger.Where(x => x.tag != "del");
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
        public ObservableCollection<Trigger> ScheduleOfTrigger
        {
            get => Schedule_Trigger;
        }
    }
}
