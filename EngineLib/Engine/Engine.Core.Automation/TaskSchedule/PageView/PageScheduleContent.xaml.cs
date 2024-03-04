using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using Engine.Data.DBFAC;
using Engine.Common;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// PageScheduleContent.xaml 的交互逻辑
    /// </summary>
    public partial class PageScheduleContent : UserControl
    {
        #region 内部变量
        //当前系统登录者作为创建人员
        private string _CreatorOfEditor = string.Empty;
        //加载窗口时指定属性列表的章节分组Tag
        private string _Tree_Group = string.Empty;
        //定义主窗口事件
        public event Action<PageScheduleContent, ScheduleContent, ParamSetting> ScheduleContent_SelectionChanged;
        private IDBFactory<ServerNode> _DB = DbFactory.Task.CloneInstance("TaskSchedule");
        private int _dgContentSelectIndex = -1;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="treeGroup"></param>
        public PageScheduleContent(string treeGroup)
        {
            InitializeComponent();
            _Tree_Group = treeGroup;
        }

        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _CreatorOfEditor = "Elvis";
            UpdateData();
            _TabScheduleContentList.Items.Clear();
            AddTab("常规", "Content_Normal");
            AddTab("触发器", "Content_Trigger");
            AddTab("门控条件", "Content_Gate");
            AddTab("控制流程", "Content_Process");
            AddTab("设置", "Content_Setting");
            _TabScheduleContentList.SelectedIndex = 0;
        }

        /// <summary>
        /// 添加Tab窗口
        /// </summary>
        /// <param name="tabTitle"></param>
        /// <param name="tag"></param>
        public void AddTab(string tabTitle, string tag)
        {
            UserControl page = new UserControl();
            switch (tag)
            {
                case "Content_Normal":  //常规选项卡
                    PageScheduleContentOfNormal uc_normal = new PageScheduleContentOfNormal("", "ReadOnly");
                    uc_normal.Tag = this;
                    page = uc_normal;
                    break;

                case "Content_Trigger": //触发器选项卡
                    PageScheduleContentOfTrigger uc_trig = new PageScheduleContentOfTrigger("ReadOnly");
                    uc_trig.Tag = this;
                    page = uc_trig;
                    break;

                case "Content_Process": //流程选项卡
                    PageScheduleContentOfProcess uc_process = new PageScheduleContentOfProcess("ReadOnly");
                    uc_process.Tag = this;
                    page = uc_process;
                    break;

                case "Content_Gate":   //门控条件选项卡
                    PageScheduleContentOfGate uc_gate = new PageScheduleContentOfGate("ReadOnly");
                    uc_gate.Tag = this;
                    page = uc_gate;
                    break;

                case "Content_Setting": //设置选项卡
                    PageScheduleContentOfSetting uc_param = new PageScheduleContentOfSetting("ReadOnly");
                    uc_param.Tag = this;
                    page = uc_param;
                    break;
            }
            _TabScheduleContentList.AddTab(tabTitle, page);
        }

        /// <summary>
        /// 页面操作指令响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            Window win = strCmd.MatchOpenedWindow();
            if (win != null)
                return;
            var items = _dgScheduleContent.SelectedItems;
            switch (strCmd)
            {
                case "_CmdCreateBaseSchedule":
                    //创建计划
                    if (win == null)
                    {
                        win = new winScheduleEditor(_CreatorOfEditor, "10000", "创建计划", "Add");
                        //绑定事件，用于更新ucScheduleContent.xaml窗口
                        (win as winScheduleEditor).DataChanged += ScheduleEdit_DataChanged;
                        (win as winScheduleEditor).TreeGroup = _Tree_Group;
                        win.Tag = this;
                        win.Owner = this.Tag as Window;
                        win.OpenWindow(strCmd);
                    }
                    else
                    {
                        win.Activate();
                        win.WindowState = WindowState.Normal;
                    }
                    break;

                case "_CmdEditSchedule":
                    //编辑计划
                    if (win == null)
                    {
                        if (this._dgScheduleContent.SelectedItem != null)
                        {
                            DataRowView row = (DataRowView)_dgScheduleContent.SelectedItem;
                            int id = (int)row["id"];
                            string name = row["name"].ToString();
                            string creator = row["creator"].ToString();
                            string description = row["description"].ToString();

                            winScheduleEditor winEditor = new winScheduleEditor(_CreatorOfEditor, "11111", "计划编辑器", "Edit", id);
                            //this.Tag = winTaskSchedule.xaml
                            winEditor.Owner = this.Tag as Window;
                            //绑定事件，用于更新ucScheduleContent.xaml窗口
                            winEditor.DataChanged += ScheduleEdit_DataChanged;
                            winEditor.Tag = this;
                            winEditor.OpenWindow(strCmd);
                        }
                    }
                    else
                    {
                        win.Activate();
                        win.WindowState = WindowState.Normal;
                    }
                    break;

                case "_CmdDelSchedule":
                    //删除计划
                    if (this._dgScheduleContent.SelectedItem != null)
                    {
                        DataRowView row = (DataRowView)_dgScheduleContent.SelectedItem;
                        int id = (int)row["id"];

                        MessageBoxResult ret = sCommon.MyMsgBox("您是否确定删除此项计划", MsgType.Question);
                        if (ret == MessageBoxResult.Yes)
                        {
                            string strSql = string.Format("delete from core_schedule_menu where tree_group='{0}' and id = {1};", _Tree_Group, id);
                            strSql += string.Format("delete from core_schedule_items where schedule_id='{0}';", id);
                            _DB.ExcuteSQL(strSql);
                            UpdateData();
                        }
                    }
                    break;

                case "_CmdUpdate":
                    UpdateData();
                    //刷新任务
                    break;

                case "_CmdActiveSchedule":
                    //激活任务
                    if (items != null)
                    {
                        string Schedule_IDs = string.Empty;
                        foreach (DataRowView row in items)
                        {
                            string scheduleName = row["name"].ToString();
                            string scheduleID = row["id"].ToString();
                            if (Schedule_IDs == string.Empty)
                                Schedule_IDs = row["id"].ToString().Trim();
                            else
                                Schedule_IDs += "," + row["id"].ToString().Trim();
                        }
                        if (Schedule_IDs.Length > 0)
                        {
                            string strSql = string.Format("update core_schedule_menu as d set d.state='已激活',d.active_time='{0}' where d.state!='已激活' and d.id in ({1});", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Schedule_IDs);
                            _DB.ExcuteSQL(strSql);
                        }
                        UpdateData();
                    }
                    break;

                case "_CmdKillSchedule":
                    //取消任务
                    items = this._dgScheduleContent.SelectedItems;
                    if (items != null)
                    {
                        string Schedule_IDs = string.Empty;
                        foreach (DataRowView row in items)
                        {
                            if (Schedule_IDs == string.Empty)
                                Schedule_IDs = row["id"].ToString().Trim();
                            else
                                Schedule_IDs += "," + row["id"].ToString().Trim();
                        }
                        if (Schedule_IDs.Length > 0)
                        {
                            string strSql = string.Format("update core_schedule_menu as d set d.state='' where d.id in ({0});", Schedule_IDs);
                            _DB.ExcuteSQL(strSql);
                            UpdateData();
                        }
                    }
                    break;

                case "_CmdImportSchedule":  //导入任务
                case "_CmdEnableSchedule":  //启用任务
                case "_CmdDisableSchedule": //禁止任务
                    sCommon.MyMsgBox("该功能暂时不可用，谢谢关注!", MsgType.Infomation);
                    break;
            }
        }

        /// <summary>
        /// 编辑器发生数据变化
        /// </summary>
        /// <param name="sender"></param>
        private void ScheduleEdit_DataChanged(winScheduleEditor sender)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            string strSql = string.Format("select * from core_schedule_menu where tree_group='{0}' order by name asc", _Tree_Group);
            _dgScheduleContent.ItemsSource = _DB.ExcuteQuery(strSql).Result.ToMyDataTable().DefaultView;
            if (_dgScheduleContent.Items.Count > _dgContentSelectIndex && _dgContentSelectIndex != -1)
                _dgScheduleContent.SelectedIndex = _dgContentSelectIndex;
        }

        private void _dgScheduleContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dgScheduleContent.SelectedItem != null)
            {
                DataRowView row = (DataRowView)_dgScheduleContent.SelectedItem;
                ScheduleContent ScheduleContent = new ScheduleContent()
                {
                    ID = row["id"].ToMyString(),
                    Name = row["name"].ToMyString(),
                    Creator = row["creator"].ToMyString(),
                    Comment = row["description"].ToMyString(),

                };
                ParamSetting param = new ParamSetting();
                param.TimeOut = row["timeout_min"].ToString();

                if (ScheduleContent_SelectionChanged != null)
                    ScheduleContent_SelectionChanged(this, ScheduleContent, param);
            }
            _dgContentSelectIndex = _dgScheduleContent.SelectedIndex;
        }

        private void _dgScheduleContent_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataRowView row = e.Row.Item as DataRowView;
            if (row != null)
            {
                string schedule_state = row["state"].ToString();
                switch (schedule_state)
                {
                    case "已激活":
                    case "已运行":
                        e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                        break;
                    default:
                        e.Row.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                }
            }
        }

        #region 属性 - 页面板块内容
        /// <summary>
        /// 常规选项卡
        /// </summary>
        public NormalCard OptionCard_Normal
        {
            get
            {
                if (_dgScheduleContent.SelectedItem != null)
                {
                    DataRowView row = (DataRowView)_dgScheduleContent.SelectedItem;
                    int index = (int)row["id"];
                    List<string> Normal_Content = new List<string>();
                    NormalCard content = new NormalCard()
                    {
                        Name = row["name"].ToMyString(),
                        Creator = row["creator"].ToMyString(),
                        Comment = row["description"].ToMyString()
                    };
                    return content;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 设置选项卡
        /// </summary>
        public ParamSetting OptionCard_Setting
        {
            get
            {
                if (_dgScheduleContent.SelectedItem != null)
                {
                    DataRowView row = (DataRowView)_dgScheduleContent.SelectedItem;
                    ParamSetting Par = new ParamSetting();
                    Par.TimeOut = row["timeout_min"].ToString();
                    return Par;
                }
                else
                    return null;
            }
        }
        #endregion

    }
}
