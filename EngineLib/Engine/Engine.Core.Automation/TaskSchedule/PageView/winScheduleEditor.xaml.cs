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
    /// winScheduleEditor.xaml 的交互逻辑
    /// </summary>
    public partial class winScheduleEditor : Window
    {
        #region 内部变量
        private string _ControlBits = string.Empty;
        private string _Creator = string.Empty;
        private string _Authority = string.Empty;
        private IDBFactory<ServerNode> _DB = DbFactory.Task;
        /// <summary>
        /// 当时选中计划的ID
        /// </summary>
        private int Schedule_ID = 0;
        /// <summary>
        /// 分组名称
        /// </summary>
        private string _TreeGroup = string.Empty;
        /// <summary>
        /// 记录可选工位 pos_id , pos_key, remark
        /// </summary>
        private List<string[]> _PosGroup;
        /// <summary>
        /// 可选工位
        /// </summary>
        private List<string> _PosItems;
        //定义事件通知主窗口发生数据变化
        public event Action<winScheduleEditor> DataChanged;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Creator">编辑页面： 创建者</param>
        /// <param name="ControlBits">"11111": 常规,触发器,门控条件,控制流程,设置</param>
        /// <param name="title">窗口标题： 创建计划  计划编辑器</param>
        /// <param name="authority">权限： "ReadOnly" or "Add" or "Edit"</param>
        /// <param name="itemIndex">数据库中该项的ID值</param>
        public winScheduleEditor(string Creator,string ControlBits,string title,string authority,int schedule_id=0)
        {
            InitializeComponent();
            _ControlBits = ControlBits;
            _Creator = Creator;
            _Authority = authority;
            Schedule_ID = schedule_id;
            this.Title = title;
        }

        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _TabScheduleContentList.Items.Clear();
            string[,] TabTitle = new string[,] {{ "常规", "Content_Normal" },{ "触发器","Content_Trigger"},{ "门控条件","Content_Gate"},{ "控制流程","Content_Process"},{ "设置","Content_Setting"}};
            for (int i = 0; i < _ControlBits.Length; i++)
            {
                string bit = _ControlBits.Substring(i, 1);
                if (bit == "1")
                {
                    AddTab(TabTitle[i, 0], TabTitle[i, 1]);
                }
            }
            _TabScheduleContentList.SelectedIndex = 0;

            if (_Authority == "Edit")
            {
                Load_PosKey();
            }
        }

        /// <summary>
        /// 加载工位表
        /// </summary>
        private void Load_PosKey()
        {
            _PosItems = new List<string>();
            ModelSystemGroup modelGroup = new ModelSystemGroup()
            {
                MarkKey = string.Format(" MarkKey='{0}' order by OrderID asc", SystemGroupType.Symbol.ToString())
                                 .MarkExpress().MarkWhere()
            };
            List<string> PosItems = new List<string>();
            DataTable dt = _DB.ExcuteQuery<ModelSystemGroup>(modelGroup).Result.ToMyDataTable();
            _PosGroup = new List<string[]>();
            foreach (DataRow row in dt.Rows)
            {
                string sn_id = row["id"].ToString();
                string pos_key = row["Name"].ToString();
                string remark = row["Comment"].ToString();
                _PosGroup.Add(new string[] { sn_id, pos_key });
                _PosItems.Add(pos_key);
            }
        }

        /// <summary>
        /// 加载Tab窗口
        /// </summary>
        /// <param name="tabTitle"></param>
        /// <param name="tag"></param>
        public void AddTab(string tabTitle, string tag)
        {
            UserControl pageCard = null;
            switch (tag)
            {
                case "Content_Normal":  //常规选项卡
                    PageScheduleContentOfNormal uc_normal = new PageScheduleContentOfNormal(_Creator, _Authority);
                    //this.Tag = ucScheduleContent.xaml
                    uc_normal.Tag = this.Tag;
                    pageCard = uc_normal;
                    break;

                case "Content_Trigger": //触发器选项卡
                    PageScheduleContentOfTrigger uc_trig = new PageScheduleContentOfTrigger(_Authority);
                    uc_trig.Schedule_ID = Schedule_ID;
                    uc_trig.Tag = this;
                    pageCard = uc_trig;
                    break;

                case "Content_Process": //流程选项卡
                    PageScheduleContentOfProcess uc_process = new PageScheduleContentOfProcess(_Authority);
                    uc_process.Schedule_ID = Schedule_ID;
                    uc_process.Tag = this;
                    pageCard = uc_process;
                    break;

                case "Content_Gate":   //门控条件选项卡
                    PageScheduleContentOfGate uc_gate = new PageScheduleContentOfGate(_Authority);
                    uc_gate.Schedule_ID = Schedule_ID;
                    uc_gate.Tag = this;
                    pageCard = uc_gate;
                    break;

                case "Content_Setting": //设置选项卡
                    PageScheduleContentOfSetting uc_param = new PageScheduleContentOfSetting(_Authority);
                    uc_param.Tag = this.Tag;
                    pageCard = uc_param;
                    break;
            }
            _TabScheduleContentList.AddTab(tabTitle, pageCard);
        }

        private void _CmdSure_Click(object sender, RoutedEventArgs e)
        {
            PageScheduleContentOfNormal OptionCardNormal = ((TabItem)_TabScheduleContentList.Items[0]).Content as PageScheduleContentOfNormal;
            NormalCard content = OptionCardNormal.NormalContent;
            string Normal_name = content.Name ;
            string Normal_Creator = content.Creator;
            string Normal_Desc = content.Comment;
            string Normal_Authority = OptionCardNormal.Authority;
            string sql = string.Empty;
            if (Normal_Authority == "Add")
                sql = string.Format("insert into core_schedule_menu(domain,enable,tree_group,name,creator,create_time,description,timeout_min) values ('{0}','已启用','{1}','{2}','{3}','{4}','{5}','{6}');", "Lab", _TreeGroup, Normal_name, Normal_Creator, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Normal_Desc, "");
            else if (Normal_Authority == "Edit" && Schedule_ID > 0)
            {
                PageScheduleContentOfSetting OptionCardSetting = ((TabItem)_TabScheduleContentList.Items[4]).Content as PageScheduleContentOfSetting;
                ParamSetting Setting_Param = OptionCardSetting.ParamGroup;

                sql = string.Format("update core_schedule_menu set name='{0}',creator='{1}',description='{2}',timeout_min='{3}' where id={4};", Normal_name, Normal_Creator, Normal_Desc, Setting_Param.TimeOut, Schedule_ID);

                PageScheduleContentOfTrigger OptionCardOfTrigger = ((TabItem)_TabScheduleContentList.Items[1]).Content as PageScheduleContentOfTrigger;
                System.Collections.ObjectModel.ObservableCollection<Trigger> Triggers = OptionCardOfTrigger.ScheduleOfTrigger;
                int i = 0;
                foreach (Trigger trig in Triggers)
                {
                    if (trig.id.ToMyInt() == 0)
                    {
                        //新增
                        sql += string.Format("insert into core_schedule_items(schedule_id,domain,trig_enable, trig_type, trig_detail, trig_state, trig_pos, trig_item, trig_object,trig_sign, trig_order) values({0}, '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10});",
                            Schedule_ID, "Lab", trig.trig_enable, trig.trig_type, trig.trig_detail, trig.trig_state, trig.trig_pos, trig.trig_item, trig.trig_object, trig.trig_sign, i);
                    }
                    else if (trig.id.ToMyInt() > 0 && trig.tag == "update")
                    {
                        //更新
                        sql += string.Format("update core_schedule_items set trig_enable='{0}',trig_type='{1}',trig_detail='{2}',trig_state='{3}',trig_pos='{4}',trig_item='{5}',trig_object='{6}',trig_sign='{7}',trig_order={8} where id={9};",
                            trig.trig_enable, trig.trig_type, trig.trig_detail, trig.trig_state, trig.trig_pos, trig.trig_item, trig.trig_object, trig.trig_sign, i, trig.id);
                    }
                    else if (trig.id.ToMyInt() > 0 && trig.tag == "del")
                    {
                        //删除项目
                        sql += string.Format("delete from core_schedule_items where id = {0};", trig.id);
                    }
                    i++;
                }

                PageScheduleContentOfGate OptionCardOfGate = ((TabItem)_TabScheduleContentList.Items[2]).Content as PageScheduleContentOfGate;
                System.Collections.ObjectModel.ObservableCollection<Gate> Gates = OptionCardOfGate.ScheduleOfGate;
                i = 0;
                foreach (Gate gate in Gates)
                {
                    if (gate.id.ToMyInt() == 0)
                    {
                        //新增
                        sql += string.Format("insert into core_schedule_items(schedule_id,domain,gate_enable, gate_type, gate_detail, gate_state, gate_pos, gate_item, gate_object,gate_sign, gate_order) values({0}, '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10});",
                            Schedule_ID, "Lab", gate.gate_enable, gate.gate_type, gate.gate_detail, gate.gate_state, gate.gate_pos, gate.gate_item, gate.gate_object, gate.gate_sign, i);
                    }
                    else if (gate.id.ToMyInt() > 0 && gate.tag == "update")
                    {
                        //更新
                        sql += string.Format("update core_schedule_items set gate_enable='{0}',gate_type='{1}',gate_detail='{2}',gate_state='{3}',gate_pos='{4}',gate_item='{5}',gate_object='{6}',gate_sign='{7}',gate_order={8} where id={9};",
                            gate.gate_enable, gate.gate_type, gate.gate_detail, gate.gate_state, gate.gate_pos, gate.gate_item, gate.gate_object, gate.gate_sign, i, gate.id);
                    }
                    else if (gate.id.ToMyInt() > 0 && gate.tag == "del")
                    {
                        //删除项目
                        sql += string.Format("delete from core_schedule_items where id = {0};", gate.id);
                    }
                    i++;
                }

                PageScheduleContentOfProcess OptionCardOfProcess = ((TabItem)_TabScheduleContentList.Items[3]).Content as PageScheduleContentOfProcess;
                System.Collections.ObjectModel.ObservableCollection<Process> Processes = OptionCardOfProcess.ScheduleOfProcess;
                i = 0;
                foreach (Process proc in Processes)
                {
                    if (proc.id.ToMyInt() == 0)
                    {
                        //新增
                        sql += string.Format("insert into core_schedule_items(schedule_id,domain,task_enable, task_type, task_detail, task_state, task_pos, task_id, task_in,task_out,task_item, task_set_val,task_des,task_order,msg_from,msg_to,msg_mode) values({0}, '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}','{16}');",
                            Schedule_ID, "Lab", proc.task_enable, proc.task_type, proc.task_detail, proc.task_state, proc.task_pos, proc.task_id, proc.task_in, proc.task_out, proc.task_item, proc.task_set_val, proc.task_des, i, proc.msg_from, proc.msg_to, proc.msg_mode);
                    }
                    else if (proc.id.ToMyInt() > 0 && proc.tag == "update")
                    {
                        //更新
                        sql += string.Format("update core_schedule_items set task_enable='{0}',task_type='{1}',task_detail='{2}',task_state='{3}',task_pos='{4}',task_id='{5}',task_in='{6}',task_out='{7}',task_item='{8}',task_set_val='{9}',task_des='{10}',task_order={11},msg_from='{12}',msg_to='{13}',msg_mode='{14}' where id={15};",
                            proc.task_enable, proc.task_type, proc.task_detail, proc.task_state, proc.task_pos, proc.task_id, proc.task_in, proc.task_out, proc.task_item, proc.task_set_val, proc.task_des, i, proc.msg_from, proc.msg_to, proc.msg_mode, proc.id);
                    }
                    else if (proc.id.ToMyInt() > 0 && proc.tag == "del")
                    {
                        //删除项目
                        sql += string.Format("delete from core_schedule_items where id = {0};", proc.id);
                    }
                    i++;
                }
            }

            if (sql.Length > 0)
            {
                CallResult _result = _DB.ExcuteSQL(sql);
            }

            //更新事件
            if (DataChanged != null)
                DataChanged(this);
            this.Close();
        }

        private void _CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public List<string[]> PosGroup
        {
            get => _PosGroup;
        }

        public List<string> PosItems
        {
            get => _PosItems;
        }

        public string TreeGroup
        {
            get => _TreeGroup;
            set
            {
                _TreeGroup = value;
            }
        }
    }
}
