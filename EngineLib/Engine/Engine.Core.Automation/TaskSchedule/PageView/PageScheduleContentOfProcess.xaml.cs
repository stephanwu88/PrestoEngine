using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// PageScheduleContentOfProcess.xaml 的交互逻辑
    /// </summary>
    public partial class PageScheduleContentOfProcess : UserControl
    {
        private bool uc_isLoaded = false;
        private string _Authority;
        private IDBFactory<ServerNode> _DB = DbFactory.Task.CloneInstance("TaskSchedule");
        private int schedule_id=0;
        private int ProcessContentRowID = 0;  //当前被编辑的流程选中行
        private int EditProcessID = 0;        //当前被编辑的流程ID
        private ObservableCollection<Process> Schedule_Process = new ObservableCollection<Process>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authority">ReadOnly Add Edit</param>
        public PageScheduleContentOfProcess(string authority)
        {
            InitializeComponent();
            _Authority = authority;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_Authority == "ReadOnly")
            {
                this._CmdNewProcess.Visibility = Visibility.Hidden;
                this._CmdEditProcess.Visibility = Visibility.Hidden;
                this._CmdDelProcess.Visibility = Visibility.Hidden;
                this._CmdMoveUp.Visibility = Visibility.Hidden;
                this._CmdMoveDown.Visibility = Visibility.Hidden;
                //绑定事件 ucScheduleContent选中数据变化时，只读选项卡需要同步显示
                //this.Tag = ucScheduleContent.xaml
                if (this.Tag != null)
                    (this.Tag as PageScheduleContent).ScheduleContent_SelectionChanged += ScheduleContent_SelectionChanged;
            }
            else
            {
                this._CmdNewProcess.Visibility = Visibility.Visible;
                this._CmdEditProcess.Visibility = Visibility.Visible;
                this._CmdDelProcess.Visibility = Visibility.Visible;
                this._CmdMoveUp.Visibility = Visibility.Visible;
                this._CmdMoveDown.Visibility = Visibility.Visible;
            }
            if (_Authority == "Edit" && !uc_isLoaded)
            {
                if (schedule_id > 0)
                    Update_Process_View(schedule_id);
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
                Update_Process_View(id);
            }
        }

        /// <summary>
        /// 窗口加载时，只读直接绑定数据库，编辑绑定到泛型（后续可编辑）
        /// </summary>
        /// <param name="schedule_index"></param>
        private void Update_Process_View(int schedule_index)
        {
            Schedule_Process.Clear();
            string strSql = string.Format("select * from core_schedule_items where schedule_id={0} and Length(task_detail)>0 order by task_order asc", schedule_index);
            DataTable dt = _DB.ExcuteQuery(strSql).Result.ToMyDataTable();
            if (_Authority == "Edit")
            {
                Schedule_Process = sCommon.ToEntityObserverCollection<Process>(dt);
                this._dgContentOfProcess.ItemsSource = Schedule_Process;
            }
            else
            {
                this._dgContentOfProcess.ItemsSource = dt.DefaultView;
            }
        }

        /// <summary>
        /// 操纵泛型，改变触发器变更后的显示，但不进入数据库，由最后确定后一并调整数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg2">[Driver,DataMove.=,Math.++] [Sieve;0501;振动筛回零]</param>
        /// <param name="oper_mode">Add Edit</param>
        private void Change_Process_View(winNewProcess sender,Process arg2,string oper_mode)
        {
            //接受事件端口发送过来的数据
            Process proc = arg2;
            string CmdType = string.Empty;

            //添加流程项
            if (proc != null && _Authority == "Edit")
            {
                if (oper_mode == "Add")
                {
                    proc.id = "0";  //id为0表示添加项
                    Schedule_Process.Add(proc);
                }
                else if (oper_mode == "Edit")
                {
                    //若是数据库原始记录，恢复其索引号
                    proc.id = EditProcessID.ToString();
                    //恢复当时进入编辑窗口时加载的的选中行，修改项
                    int processRowId = ProcessContentRowID;
                    Schedule_Process[processRowId] = proc;
                    Schedule_Process[processRowId].tag = "update";
                }
            }
            this._dgContentOfProcess.ItemsSource = Schedule_Process.Where(x => x.tag != "del");
        }

        /// <summary>
        /// 添加流程控制项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdNewProcess_Click(object sender, RoutedEventArgs e)
        {
            winNewProcess win = new winNewProcess("Add");
            if ((this.Tag as Window).OwnedWindows.Count == 0)
            {
                win.Owner = this.Tag as Window;
                win.ScheduleID = schedule_id;
                win.ProcessChanged += Change_Process_View;
                win.Show();
            }
        }

        /// <summary>
        /// 编辑流程控制项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdEditProcess_Click(object sender, RoutedEventArgs e)
        {
            Process row = _dgContentOfProcess.SelectedItem as Process;
            if (row != null)
            {
                winNewProcess win = new winNewProcess("Edit", schedule_id);
                if ((this.Tag as Window).OwnedWindows.Count == 0)
                {
                    win.Owner = this.Tag as Window;
                    win.ScheduleID = schedule_id;
                    //记录触发器ID
                    EditProcessID = row.id.ToMyInt();
                    //将表格的选中行作为更新时的操作索引
                    ProcessContentRowID = _dgContentOfProcess.SelectedIndex;
                    win.ProcessID = ProcessContentRowID;
                    win.ProcessDefaultContent = row;
                    win.ProcessChanged += Change_Process_View;
                    win.Show();
                }
            }
        }

        /// <summary>
        /// 删除流程项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdDelProcess_Click(object sender, RoutedEventArgs e)
        {
            Process row = _dgContentOfProcess.SelectedItem as Process;
            if (row != null)
            {
                int processRowId = _dgContentOfProcess.SelectedIndex;
                Schedule_Process[processRowId].tag = "del";
                this._dgContentOfProcess.ItemsSource = Schedule_Process.Where(x => x.tag != "del");
            }
        }

        /// <summary>
        /// 记录上移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdMoveUp_Click(object sender, RoutedEventArgs e)
        {
            Process row = _dgContentOfProcess.SelectedItem as Process;
            Process tempRow = new Process();
            int rowId = _dgContentOfProcess.SelectedIndex;
            if (row != null)
            {
                if (rowId > 0)
                {
                    if (Schedule_Process[rowId - 1].tag != "del")
                        Schedule_Process[rowId - 1].tag = "update";
                    if (Schedule_Process[rowId].tag != "del")
                        Schedule_Process[rowId].tag = "update";
                    tempRow = Schedule_Process[rowId - 1];
                    Schedule_Process[rowId - 1] = Schedule_Process[rowId];
                    Schedule_Process[rowId] = tempRow;
                }
            }
            this._dgContentOfProcess.ItemsSource = Schedule_Process.Where(x => x.tag != "del");
        }

        /// <summary>
        /// 记录下移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdMoveDown_Click(object sender, RoutedEventArgs e)
        {
            Process row = _dgContentOfProcess.SelectedItem as Process;
            Process tempRow = new Process();
            int rowId = _dgContentOfProcess.SelectedIndex;
            if (row != null)
            {
                if (rowId <= _dgContentOfProcess.Items.Count - 2)
                {
                    if (Schedule_Process[rowId + 1].tag != "del")
                        Schedule_Process[rowId + 1].tag = "update";
                    if (Schedule_Process[rowId].tag != "del")
                        Schedule_Process[rowId].tag = "update";
                    tempRow = Schedule_Process[rowId + 1];
                    Schedule_Process[rowId + 1] = Schedule_Process[rowId];
                    Schedule_Process[rowId] = tempRow;
                }
            }
            this._dgContentOfProcess.ItemsSource = Schedule_Process.Where(x => x.tag != "del");
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
        public ObservableCollection<Process> ScheduleOfProcess
        {
            get => Schedule_Process;
        }
    }
}
