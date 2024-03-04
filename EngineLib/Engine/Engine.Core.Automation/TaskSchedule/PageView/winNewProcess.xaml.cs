using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Engine.Data.DBFAC;
using System.IO;
using Engine.Common;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// winNewProcess.xaml 的交互逻辑
    /// </summary>
    public partial class winNewProcess : Window
    {
        private string _Authority;
        private int _schedule_id = 0;
        private int _process_id = 0;
        /// <summary>
        /// 编辑窗口自动获取的默认参数
        /// </summary>
        private Process _ProcessDefaultContent;

        /// <summary>
        /// 记录可选工位 pos_id , pos_key, remark
        /// </summary>
        private List<string[]> _PosGroup;

        /// <summary>
        /// 保存编辑后的任务节点
        /// </summary>
        private List<string[]> TaskNodes;
        /// <summary>
        /// 保存编辑后的事件节点
        /// </summary>
        private List<string[]> EventNodes;
        /// <summary>
        /// 触发器发生变更事件
        /// </summary>
        public event Action<winNewProcess, Process, string> ProcessChanged;

        private IDBFactory<ServerNode> _DB = DbFactory.Task;
        public winNewProcess(string authority, int scheduleID = 0, int processID = 0, Process processDefaultContent = null)
        {
            InitializeComponent();
            _Authority = authority;
            _schedule_id = scheduleID;
            _process_id = processID;
            _ProcessDefaultContent = processDefaultContent;
            switch (_Authority)
            {
                case "Add":
                    this.Title = "新建流程";
                    break;

                case "Edit":
                    this.Title = "编辑流程";
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTaskID();
            LoadEvent();

            this._sourceType.IsChecked = false;
            this._PosSource.Visibility = Visibility.Hidden;
            this._ItemSource.Visibility = Visibility.Hidden;
            this._g1_msg_sourceStation.Visibility = Visibility.Hidden;
            this._g1_msg_sourceItem.Visibility = Visibility.Hidden;

            this._CmdGroup.Items.Clear();
            this._CmdGroup.Items.Add("Driver");
            this._CmdGroup.Items.Add("Event");
            this._CmdGroup.Items.Add("DataMove.=");
            this._CmdGroup.Items.Add("Math.++");
            this._CmdGroup.SelectedIndex = 0;

            this._Task_IN.Items.Clear();
            this._Task_IN.Items.Add("NORMAL");
            this._Task_IN.Items.Add("WAIT");
            this._Task_IN.Items.Add("DONE");
            this._Task_IN.SelectedIndex = 0;

            this._Task_OUT.Items.Clear();
            this._Task_OUT.Items.Add("DONE");
            this._Task_OUT.Items.Add("RUN");
            this._Task_OUT.SelectedIndex = 0;

            this._Pos_Driver.Items.Clear();
            this._Pos_Macro.Items.Clear();
            this._Pos_Driver.ItemsSource = (this.Owner as winScheduleEditor).PosItems;
            this._Pos_Macro.ItemsSource = (this.Owner as winScheduleEditor).PosItems;
            this._PosSource.ItemsSource = (this.Owner as winScheduleEditor).PosItems;
            this._MsgFromStation.ItemsSource = (this.Owner as winScheduleEditor).PosItems;
            this._MsgToStation.ItemsSource = (this.Owner as winScheduleEditor).PosItems;
            this._Pos_Driver.SelectedIndex = 0;
            this._Pos_Macro.SelectedIndex = 0;
            this._PosSource.SelectedIndex = 0;
            //this._MsgFromStation.SelectedIndex = 0;
            //this._MsgToStation.SelectedIndex = 0;

            this._Event_Content.Items.Clear();
            if (EventNodes.Count > 0)
            {
                for (int i = 0; i < EventNodes.Count; i++)
                {
                    this._Event_Content.Items.Add(EventNodes[i][1]);
                }
                this._Event_Content.SelectedIndex = 0;
            }

            //从主窗口获取数据集
            _PosGroup = (this.Owner as winScheduleEditor).PosGroup;

            //Driver(0)  已启用(1)  Sieve(2) = 1011(3); in:NORMAL(4);out:DONE(5);振动筛回零(6)
            //DataMove.=(0)  已启用(1)  Sieve(2).SS_LT(3) = 2(4); 振动筛回零(5)
            //Math.++(0)  已启用(1)  Sieve(2).SS_LT(3)
            if (_Authority == "Edit")
            {
                if (_ProcessDefaultContent != null)
                {
                    string strCmdType = _ProcessDefaultContent.task_type;
                    this._CmdGroup.SelectedItem = strCmdType;
                    this._Chk_Enable.IsChecked = _ProcessDefaultContent.task_enable == "已启用" ? true : false;
                    this._Chk_MsgFrom.IsChecked = _ProcessDefaultContent.msg_from.Length > 0 ? true : false;
                    this._Chk_MsgTo.IsChecked = _ProcessDefaultContent.msg_to.Length > 0 ? true : false;
                    _MsgFromStation.Text = _ProcessDefaultContent.msg_from;
                    _MsgToStation.Text = _ProcessDefaultContent.msg_to;
                    this._Chk_KillSoucre.IsChecked = _ProcessDefaultContent.msg_mode.Contains("KillSource") ? true : false;
                    this._Chk_KeepCopy.IsChecked = _ProcessDefaultContent.msg_mode.Contains("KeepCopy") ? true : false;
                    switch (strCmdType)
                    {
                        case "Driver":
                            this._Pos_Driver.Text= _ProcessDefaultContent.task_pos;
                            this._TaskID.Text= _ProcessDefaultContent.task_id;
                            this._Task_IN.Text= _ProcessDefaultContent.task_in;
                            this._Task_OUT.Text= _ProcessDefaultContent.task_out;
                            this._TaskContent.Text= _ProcessDefaultContent.task_detail.Split(';')[_ProcessDefaultContent.task_detail.Split(';').Length-1].Trim();
                            break;

                        case "Event":
                            if (_ProcessDefaultContent.task_detail.Contains(';'))
                            {
                                this._Event_Prog.Text = _ProcessDefaultContent.task_detail.Split(';')[0];
                                this._Event_Content.Text = _ProcessDefaultContent.task_des;
                            }
                            break;

                        case "DataMove.=":
                            this._Pos_Macro.Text = _ProcessDefaultContent.task_pos;
                            this._Pos_Item.Text = _ProcessDefaultContent.task_item;
                            string[] strSource = _ProcessDefaultContent.task_detail.Split('=')[1].Split('.');
                            if (strSource.Length >= 2)
                            {
                                this._sourceType.IsChecked = true;
                                _PosSource.Text = strSource[0];
                                _ItemSource.Text = strSource[1];
                            }
                            else
                            {
                                this._sourceType.IsChecked = false;
                                this._PosSource.Visibility = Visibility.Hidden;
                                this._ItemSource.Visibility = Visibility.Hidden;
                                this._g1_msg_sourceStation.Visibility = Visibility.Hidden;
                                this._g1_msg_sourceItem.Visibility = Visibility.Hidden;
                                this._Content_Set.Text = _ProcessDefaultContent.task_detail.Split('=')[_ProcessDefaultContent.task_detail.Split('=').Length - 1].Trim();
                            }
                            break;

                        case "Math.++":
                            this._Pos_Macro.Text = _ProcessDefaultContent.task_pos;
                            this._Pos_Item.Text = _ProcessDefaultContent.task_item;
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// 指令类型选择发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CmdGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object _CmdSel = this._CmdGroup.SelectedItem;
            if (_CmdSel != null)
            {
                string strCmdSel = _CmdSel.ToString();
                switch (strCmdSel)
                {
                    case "Driver":
                        _Border_Driver.Visibility = Visibility.Visible;
                        _Border_Macro.Visibility = Visibility.Hidden;
                        break;

                    case "Event":
                        _Border_Driver.Visibility = Visibility.Hidden;
                        _Border_Macro.Visibility = Visibility.Visible;

                        _g1_msg_var1.Visibility = Visibility.Hidden;
                        _g1_msg_var2.Visibility = Visibility.Hidden;
                        _g1_msg_var3.Visibility = Visibility.Hidden;
                        _Pos_Macro.Visibility = Visibility.Hidden;
                        _Pos_Item.Visibility = Visibility.Hidden;
                        _Content_Set.Visibility = Visibility.Hidden;

                        _sourceType.Visibility = Visibility.Hidden;
                        this._g1_msg_sourceStation.Visibility = Visibility.Hidden;
                        this._g1_msg_sourceItem.Visibility = Visibility.Hidden;
                        this._PosSource.Visibility = Visibility.Hidden;
                        this._ItemSource.Visibility = Visibility.Hidden;

                        _g2_msg_var1.Visibility = Visibility.Visible;
                        _g2_msg_var2.Visibility = Visibility.Visible;
                        _Event_Content.Visibility = Visibility.Visible;
                        _Event_Prog.Visibility = Visibility.Visible;
                        break;

                    case "DataMove.=":
                        _Border_Driver.Visibility = Visibility.Hidden;
                        _Border_Macro.Visibility = Visibility.Visible;
                        _g1_msg_var1.Visibility = Visibility.Visible;
                        _g1_msg_var2.Visibility = Visibility.Visible;
                        _g1_msg_var3.Visibility = Visibility.Visible;
                        _Pos_Macro.Visibility = Visibility.Visible;
                        _Pos_Item.Visibility = Visibility.Visible;
                        _Content_Set.Visibility = Visibility.Visible;

                        _g2_msg_var1.Visibility = Visibility.Hidden;
                        _g2_msg_var2.Visibility = Visibility.Hidden;
                        _Event_Content.Visibility = Visibility.Hidden;
                        _Event_Prog.Visibility = Visibility.Hidden;

                        _sourceType.Visibility = Visibility.Visible;
                        break;

                    case "Math.++":
                        _Border_Driver.Visibility = Visibility.Hidden;
                        _Border_Macro.Visibility = Visibility.Visible;
                        _g1_msg_var1.Visibility = Visibility.Visible;
                        _g1_msg_var2.Visibility = Visibility.Visible;
                        _g1_msg_var3.Visibility = Visibility.Hidden;
                        _Pos_Macro.Visibility = Visibility.Visible;
                        _Pos_Item.Visibility = Visibility.Visible;
                        _Content_Set.Visibility = Visibility.Hidden;
                        _sourceType.Visibility = Visibility.Hidden;
                        this._g1_msg_sourceStation.Visibility = Visibility.Hidden;
                        this._g1_msg_sourceItem.Visibility = Visibility.Hidden;
                        this._PosSource.Visibility = Visibility.Hidden;
                        this._ItemSource.Visibility = Visibility.Hidden;

                        _g2_msg_var1.Visibility = Visibility.Hidden;
                        _g2_msg_var2.Visibility = Visibility.Hidden;
                        _Event_Content.Visibility = Visibility.Hidden;
                        _Event_Prog.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        private void _Pos_Driver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object _CmdSel = this._CmdGroup.SelectedItem;
            if (_CmdSel != null)
            {
                string strCmdSel = _CmdSel.ToString();
                switch (strCmdSel)
                {
                    case "Driver":
                        if (TaskNodes != null && _Pos_Driver.SelectedItem != null)
                        {
                            this._TaskContent.Items.Clear();
                            for (int i = 0; i < TaskNodes.Count; i++)
                            {
                                if (TaskNodes[i][0] == _Pos_Driver.SelectedItem.ToString())
                                    this._TaskContent.Items.Add(TaskNodes[i][2]);
                            }
                            this._TaskContent.SelectedIndex = 0;
                        }
                        break;
                }
            }
        }

        private void _PosSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object _CmdSel = this._CmdGroup.SelectedItem;
            if (_CmdSel != null)
            {
                string strCmdSel = _CmdSel.ToString();
                switch (strCmdSel)
                {
                    case "DataMove.=":
                        _ItemSource.ItemsSource = Accessor.Current.ReadPosItemList(_PosSource.SelectedItem.ToMyString()); 
                        _ItemSource.SelectedIndex = 0;
                        break;
                }
            }
        }

        private void _Pos_Macro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object _CmdSel = this._CmdGroup.SelectedItem;
            if (_CmdSel != null)
            {
                string strCmdSel = _CmdSel.ToString();
                switch (strCmdSel)
                {
                    case "DataMove.=":
                    case "Math.++":
                        _Pos_Item.ItemsSource = Accessor.Current.ReadPosItemList(_Pos_Macro.SelectedItem.ToMyString());
                        _Pos_Item.SelectedIndex = 0;
                        break;
                }
            }
        }

        private void _TaskContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskNodes != null && _TaskContent.SelectedItem!=null && _Pos_Driver.SelectedItem!=null)
            {
                for (int i = 0; i < TaskNodes.Count; i++)
                {
                    if (TaskNodes[i][0] == _Pos_Driver.SelectedItem.ToString() && TaskNodes[i][2] == _TaskContent.SelectedItem.ToString())
                    {
                        this._TaskID.Text = TaskNodes[i][1];
                        return;
                    }
                }
            }
        }

        private void _Event_Content_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventNodes!=null && _Event_Content.SelectedItem!=null)
            {
                for (int i = 0; i < EventNodes.Count; i++)
                {
                    if (EventNodes[i][1] == _Event_Content.SelectedItem.ToString())
                    {
                        this._Event_Prog.Text = EventNodes[i][0];
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 加载任务节点文件
        /// </summary>
        private void LoadTaskID()
        {
            if (TaskNodes != null)
                return;
            TaskNodes = new List<string[]>();
            List<ModelSystemSymbol> ListSymbols =  Accessor.Current.ReadSymbolList("","", "TaskDriver");
            foreach (ModelSystemSymbol symbol in ListSymbols)
            {
                string[] node = new string[] { symbol.GroupName, symbol.Name, symbol.Comment };
                TaskNodes.Add(node);
            }
        }

        /// <summary>
        /// 加载事件文件
        /// </summary>
        private void LoadEvent()
        {
            if (EventNodes != null)
                return;

            string path = Directory.GetCurrentDirectory() + @"\Core\Event.txt";
            StreamReader sr = new StreamReader(path, Encoding.Default);
            EventNodes = new List<string[]>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.MidString("",";");
                if (line.Split(',').Length >= 2)
                {
                    string[] Ele = line.Split(',');
                    string[] node = new string[] { Ele[0], Ele[1] };
                    EventNodes.Add(node);
                }
            }
        }

        private void _Chk_MsgFrom_Checked(object sender, RoutedEventArgs e)
        {
            if(_Chk_KillSoucre!=null)
                _Chk_KillSoucre.IsEnabled = true;
        }

        private void _Chk_MsgFrom_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_Chk_KillSoucre != null)
            {
                _Chk_KillSoucre.IsEnabled = false;
                _Chk_KillSoucre.IsChecked = null;
            }
        }

        private void _Chk_MsgTo_Checked(object sender, RoutedEventArgs e)
        {
            if(_Chk_KeepCopy!=null)
                _Chk_KeepCopy.IsEnabled = true;
        }

        private void _Chk_MsgTo_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_Chk_KeepCopy != null)
            {
                _Chk_KeepCopy.IsEnabled = false;
                _Chk_KeepCopy.IsChecked = null;
            }
        }

        private void _Cmd_Sure_Click(object sender, RoutedEventArgs e)
        {
            NewProcess(_CmdGroup.SelectedItem.ToString(),_Authority);
            this.Close();
        }

        private void NewProcess(string cmdType, string authority)
        {
            //Driver(0)  已启用(1)  Sieve(2) = 1011(3); in:NORMAL(4);out:DONE(5);振动筛回零(6)
            //DataMove.=(0)  已启用(1)  Sieve(2).SS_LT(3) = 2(4); 振动筛回零(5)
            //Math.++(0)  已启用(1)  Sieve(2).SS_LT(3)
            Process ProcessContent = new Process();
            string strEnable = this._Chk_Enable.IsChecked==true ? "已启用":"禁用";

            //_Border_Driver
            string driver_pos = this._Pos_Driver.Text;
            string driver_taskId = this._TaskID.Text;
            string drvier_taskin = this._Task_IN.Text;
            string driver_taskout = this._Task_OUT.Text;
            string driver_content = this._TaskContent.Text;

            //_Border_Macro 
            //=: Sieve.SS_LT = 2 
            //++: Sieve.SS_LT
            string var1 = this._Pos_Macro.Text;
            string var2 = this._Pos_Item.Text;
            string var3 = string.Empty;
            string strSource = string.Format("{0}.{1}", _PosSource.Text, _ItemSource.Text);
            var3 = _sourceType.IsChecked==true ? strSource : this._Content_Set.Text.Trim();
            string detail = string.Empty;

            //_Border_Macro
            //Event
            string event_content = this._Event_Content.Text;
            string event_prog = this._Event_Prog.Text.Trim();

            ProcessContent.task_type = cmdType;
            ProcessContent.task_enable = strEnable;

            ProcessContent.msg_from = _Chk_MsgFrom.IsChecked == true ? _MsgFromStation.Text : "";
            ProcessContent.msg_to = _Chk_MsgTo.IsChecked == true ? _MsgToStation.Text : "";
            ProcessContent.msg_mode = string.Empty;
            if (this._Chk_KillSoucre.IsChecked == true && this._Chk_KillSoucre.IsEnabled==true && ProcessContent.msg_from.Length>0)
                ProcessContent.msg_mode += "KillSource.";
            if(this._Chk_KeepCopy.IsChecked == true && this._Chk_KeepCopy.IsEnabled==true && ProcessContent.msg_to.Length>0)
                ProcessContent.msg_mode += "KeepCopy";
            switch (cmdType)
            {
                case "Driver":
                    ProcessContent.task_pos = driver_pos;
                    ProcessContent.task_id = driver_taskId;
                    ProcessContent.task_in = drvier_taskin;
                    ProcessContent.task_out = driver_taskout;
                    ProcessContent.task_des = driver_content;
                    //Driver(0)  已启用(1)  Sieve(2) = 1011(3); in:NORMAL(4);out:DONE(5);振动筛回零(6)
                    detail = string.Format("{0}={1}; in:{2}; out:{3}; {4}", driver_pos, driver_taskId, drvier_taskin, driver_taskout, driver_content);
                    ProcessContent.task_detail = detail;
                    break;

                case "Event":
                    ProcessContent.task_des = event_content;
                    //Driver(0)  已启用(1)  Sieve(2) = 1011(3); in:NORMAL(4);out:DONE(5);振动筛回零(6)
                    detail = string.Format("{0};{1}", event_prog, event_content);
                    ProcessContent.task_detail = detail;
                    break;

                case "DataMove.=":
                    ProcessContent.task_pos = var1;
                    ProcessContent.task_item = var2;
                    ProcessContent.task_set_val = var3;
                    //DataMove.=(0)  已启用(1)  Sieve(2).SS_LT(3) = 2(4); 振动筛回零(5)
                    detail = string.Format("{0}.{1}={2}", var1, var2, var3);
                    ProcessContent.task_detail = detail;
                    break;

                case "Math.++":
                    ProcessContent.task_pos = var1;
                    ProcessContent.task_item = var2;
                    //Math.++(0)  已启用(1)  Sieve(2).SS_LT(3)
                    detail = string.Format("{0}.{1}", var1, var2);
                    ProcessContent.task_detail = detail;
                    break;
            }

            //向事件端口发送消息
            if (ProcessChanged != null && (authority == "Add" || authority == "Edit"))
                ProcessChanged(this, ProcessContent, authority);
        }

        private void _sourceType_Checked(object sender, RoutedEventArgs e)
        {
            this._g1_msg_var3.Visibility = Visibility.Hidden;
            this._Content_Set.Visibility = Visibility.Hidden;

            this._g1_msg_sourceStation.Visibility = Visibility.Visible;
            this._g1_msg_sourceItem.Visibility = Visibility.Visible;
            this._PosSource.Visibility = Visibility.Visible;
            this._ItemSource.Visibility = Visibility.Visible;

        }

        private void _sourceType_Unchecked(object sender, RoutedEventArgs e)
        {
            this._g1_msg_var3.Visibility = Visibility.Visible;
            this._Content_Set.Visibility = Visibility.Visible;

            this._g1_msg_sourceStation.Visibility = Visibility.Hidden;
            this._g1_msg_sourceItem.Visibility = Visibility.Hidden;
            this._PosSource.Visibility = Visibility.Hidden;
            this._ItemSource.Visibility = Visibility.Hidden;
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
        /// 编辑时所需的流程ID
        /// </summary>
        public int ProcessID
        {
            get => _process_id;
            set
            {
                _process_id = value;
            }
        }

        /// <summary>
        /// 编辑模式进入窗口时加载的默认参数
        /// </summary>
        public Process ProcessDefaultContent
        {
            get => _ProcessDefaultContent;
            set
            {
                _ProcessDefaultContent = value;
            }
        }
    }
}
