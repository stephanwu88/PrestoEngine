using Engine.ComDriver;
using Engine.ComDriver.Types;
using Engine.Common;
using Engine.Core.TaskSchedule;
using Engine.Data.DBFAC;
using Engine.Mod;
using Engine.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.Core
{
    /// <summary>
    /// 工位管理
    /// </summary>
    public class POS : Accessor
    {
        public string PosState = string.Empty;
        public string TaskState = string.Empty;
        public string TaskCommand = string.Empty;
        public string ViewSI = string.Empty;
        public string StepKey = string.Empty;
        public string PPKey = string.Empty;

        public void UpdateItem(string PosName)
        {
            PosState = ReadPosItem(PosName, "POS_STATE");
            TaskState = ReadPosItem(PosName, "TASK_STATE");
            TaskCommand = ReadPosItem(PosName, "TASK_CMD");
            ViewSI = ReadPosItem(PosName, "VIEW_SI");
            StepKey = ReadPosItem(PosName, "STEP_KEY");
            PPKey = ReadPosItem(PosName, "PP_KEY");
        }
    }

    /// <summary>
    /// 任务工厂框架
    /// </summary>
    public abstract class TaskFactoryFrame : Accessor
    {
        #region 内部变量
        protected IDBFactory<ServerNode> _DB = DbFactory.Task.CloneInstance("TaskFactoryFrame");
        private POS WorkPos = new POS();
        
        //需要监管的任务队列
        protected List<TaskItem> TaskTakeItems = new List<TaskItem>();
        private static List<string> LstMacroCmd = new List<string>() { "DataMove.=", "Math.++" };
        private string LastError;

        //事件主键
        public static readonly string TaskDriverStarted = SystemDefault.UUID;
        public static readonly string TaskDriverFinished = SystemDefault.UUID;
        #endregion

        /// <summary>
        /// 解析列表中是否包含指定任务工位
        /// </summary>
        /// <param name="TaskPosName"></param>
        /// <returns></returns>
        public bool ContainsTaskPos(string TaskPosName,string TaskCommand)
        {
            lock (TaskTakeItems)
            {
                //TaskItem item = TaskTakeItems.FirstOrDefault(x => x.task_pos == TaskPosName &&
                //            x.task_type == "Driver" && x.task_state == "UPDATE");
                TaskItem item = TaskTakeItems.FirstOrDefault(x => x.task_pos == TaskPosName &&
                                x.task_type == "Driver" && x.task_id== TaskCommand);
                return item != null;
            }
        }

        /// <summary>
        /// 执行任务驱动项
        /// </summary>
        /// <param name="PosName"></param>
        /// <param name="TaskName"></param>
        public virtual void TryTaskDriver(string PosName, string TaskName)
        {
            string strTaskRelatedPos = PosName;
            string TryTaskCommand = TaskName;
            ModelSystemSymbol symbol = ReadSymbolItem(strTaskRelatedPos, TryTaskCommand, "TaskDriver");
            //任务驱动代理 - 转换逻辑
            if (symbol.LogicExpress.Contains("ReGroupName=[") || symbol.LogicExpress.Contains("ReName=["))
            {
                string strFormatedNameValue = symbol.Name.ToMyString();
                if (symbol.LogicExpress.Contains("ReGroupName=["))
                {
                    string strFormatedGroupNameValue = GetExpressField(symbol.LogicExpress, "GroupName");
                    string CalculatedGroupNameValue = TaskDriverExpressCalc(strFormatedGroupNameValue, symbol.LogicExpress, "ReGroupName");
                    strTaskRelatedPos = CalculatedGroupNameValue;
                }
                if (symbol.LogicExpress.Contains("ReName=["))
                {
                    string CalculatedNameValue = TaskDriverExpressCalc(strFormatedNameValue, symbol.LogicExpress, "ReName");
                    TryTaskCommand = CalculatedNameValue;
                }
                if (!string.IsNullOrEmpty(strTaskRelatedPos) && !string.IsNullOrEmpty(TryTaskCommand))
                    symbol = ReadSymbolItem(strTaskRelatedPos, TryTaskCommand, "TaskDriver");
                else
                {
                    //指令转换错误
                    if (LastError.ToMyString() != symbol.Comment.ToMyString())
                    {
                        LastError = symbol.Comment.ToMyString();
                        Logger.Task.Write(LOG_TYPE.ERROR, string.Format("任务指令转换错误:【{0}】", symbol.Comment));
                    }
                }
            }
            WorkPos.UpdateItem(strTaskRelatedPos);
            if (!string.IsNullOrEmpty(symbol.LogicExpress))
            {
                bool TaskInValid = TaskDrvierExpressValidOfIN(symbol.LogicExpress);
                if (TaskInValid && TryTaskCommand.Length > 0)
                {
                    if (!string.IsNullOrEmpty(WorkPos.TaskCommand))
                        WritePosItem(strTaskRelatedPos, "TASK_CMD", "");
                    if (WorkPos.TaskState.ToMyString() != "NORMAL")
                        WritePosItem(strTaskRelatedPos, "TASK_STATE", "NORMAL");
                    //发送任务指令
                    if (!string.IsNullOrEmpty(symbol.Name) && !string.IsNullOrEmpty(symbol.RelatedDriver))
                    {
                        bool CommandOK = true;
                        bool CommandWriten = false;
                        string strSetValue = symbol.SetValue.ToMyString();
                        if (strSetValue.MatchParamCount() > 0)
                            CommandOK = TaskDriverCommandCalc(ref symbol);
                        if (!CommandOK)
                        {
                            //指令转换错误
                            if (LastError.ToMyString() != symbol.Comment.ToMyString())
                            {
                                LastError = symbol.Comment.ToMyString();
                                Logger.Task.Write(LOG_TYPE.ERROR, string.Format("任务指令运算错误:【{0}】", symbol.Comment));
                            }
                        }
                        strSetValue = symbol.SetValue.ToMyString();
                        if (strTaskRelatedPos.ToLower().Contains("robot"))
                        {
                            if (!string.IsNullOrEmpty(strSetValue) && CommandOK)
                            {
                                CommandWriten = WriteCommand(symbol.GroupName, "CmdTaskRouting", strSetValue, true, symbol.Name);
                            }
                        }
                        else
                            CommandWriten = WriteCommandByDataName(symbol.RelatedDriver, symbol.Name, strSetValue, false, symbol.Name);
                        if (CommandWriten)
                        {
                            //清空任务命令返回值
                            WriteTaskDriverValue(symbol.RelatedDriver, symbol.Name, "");
                            Logger.Task.Write(LOG_TYPE.MESS, string.Format("任务下发成功:【手动】【{0}】", symbol.Comment));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 任务驱动项表达式验证 - IN段落
        /// </summary>
        /// <param name="LogicExpress"></param>
        /// <returns></returns>
        protected bool TaskDrvierExpressValidOfIN(string LogicExpress)
        {
            return TaskDrvierExpressValid(LogicExpress, "IN");
        }

        /// <summary>
        /// 任务驱动项表达式验证 - OUT段落
        /// </summary>
        /// <param name="LogicExpress"></param>
        /// <returns></returns>
        protected bool TaskDrvierExpressValidOfOUT(string LogicExpress)
        {
            return TaskDrvierExpressValid(LogicExpress, "OUT");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LogicExpress"></param>
        /// <param name="FieldMark"></param>
        /// <returns></returns>
        protected bool ContainsField(string LogicExpress, string FieldMark)
        {
            return LogicExpress.Contains(FieldMark + "=[") && 
                LogicExpress.Contains("]=" + FieldMark);
        }

        /// <summary>
        /// 任务驱动项表达式验证
        /// </summary>
        /// <param name="LogicExpress"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool TaskDrvierExpressValid(string LogicExpress, string Mark = "IN")
        {
            Mark = Mark.Trim();
            string strLogicInContent = LogicExpress.MidString(Mark + "=[", "]=" + Mark).Trim();
            if (string.IsNullOrEmpty(strLogicInContent))
                return true;
            List<string> LstLogicIn = strLogicInContent.MySplit("|");
            foreach (string exp in LstLogicIn)
            {
                List<string> LstCondition = exp.MySplit("&");
                bool CompareMatch = false;
                foreach (string cond in LstCondition)
                {
                    string strCond = cond;
                    int CmdParamNum = cond.MatchParamCount();
                    if (CmdParamNum > 0)
                        strCond = TaskDriverExpressCalc(cond, LogicExpress, Mark + "Value");
                    if (strCond.Contains("{") || strCond.Contains("}"))
                        break;
                    PosItem pos = strCond.ParsePosItem();
                    if (pos.Value.Length == 0 && !strCond.Contains("="))
                        pos.Value = "1";
                    string strPosItemValue = ReadPosItem(pos.Name, pos.Item);
                    if (strPosItemValue == pos.Value)
                    {
                        if (!CompareMatch)
                            CompareMatch = true;
                    }
                    else
                    {
                        CompareMatch = false;
                        break;
                    }
                }
                if (CompareMatch)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取表达式字段内容
        /// </summary>
        /// <param name="LogicExpress"></param>
        /// <param name="FieldMark"></param>
        /// <returns></returns>
        private string GetExpressField(string LogicExpress, string FieldMark)
        {
            FieldMark = FieldMark.Replace("=[", "").Replace("]=", "");
            return LogicExpress.MidString(FieldMark+"=[", "]="+ FieldMark).Trim();
        }

        /// <summary>
        /// 任务关联表达式计算
        /// </summary>
        /// <param name="FormatedExpress"></param>
        /// <param name="ParamExpress"></param>
        /// <param name="FieldMark"></param>
        /// <returns></returns>
        private string TaskDriverExpressCalc(string FormatedExpress, string ParamExpress, string FieldMark = "CmdValue")
        {
            return TaskDriverExpressCalc(FormatedExpress, ParamExpress, FieldMark, out string Error);
        }

        /// <summary>
        /// 任务关联表达式计算
        /// </summary>
        /// <param name="FormatedExpress"></param>
        /// <param name="ParamExpress"></param>
        /// <param name="FieldMark"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        private string TaskDriverExpressCalc(string FormatedExpress, string ParamExpress, string FieldMark, out string ErrorMessage)
        {
            //CmdValue=[
            // SRC={ ReadTableRow(SRC=[location_group],IF=[GroupKey=RockSieve & PosID=1],RET=[MarkKey]) } RET={SEL=( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 )} --
            // SRC={ ReadTableRow(SRC=[location_group],IF=[GroupKey=RockSieve & PosID=1],RET=[MarkKey]) } RET={SEL=( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 )} --
            // SRC={ ReadTableRow(SRC=[location_group],IF=[GroupKey=RockSieve & PosID=1],RET=[MarkKey]) } RET={SEL=( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 )} --
            // SRC={ ReadTableRow(SRC=[location_group],IF=[GroupKey=RockSieve & PosID=1],RET=[MarkKey]) } RET={SEL=( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 )} --
            // SRC={ ReadTableRow(SRC=[location_group],IF=[GroupKey=RockSieve & PosID=1],RET=[MarkKey]) } RET={SEL=( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 ) | ( 50mm, 01 )}
            // ]=CmdValue

            int CmdParamNum = FormatedExpress.MatchParamCount();
            ErrorMessage = string.Empty;
            if (!string.IsNullOrEmpty(ParamExpress) && CmdParamNum > 0)
            {
                ParamExpress = ParamExpress.MidString(FieldMark + "=[", "]=" + FieldMark).Trim();
                List<string> LstValueExp = ParamExpress.MySplit("@");
                List<string> LstValue = new List<string>();
                if (LstValueExp.Count == CmdParamNum)
                {
                    foreach (string exp in LstValueExp)
                    {
                        string strSRC = exp.MidString("SRC={", "}");
                        string strRET = exp.MidString("RET={", "}");
                        CallResult _res = _DB.ExcuteMacroQuery(strSRC);
                        if (_res.Success)
                        {
                            string strSrcValue = _res.Result.ToMyString();
                            //if (string.IsNullOrEmpty(strSrcValue))
                            //    break;
                            if (!exp.Contains("RET={"))
                            {
                                LstValue.Add(strSrcValue);
                            }
                            else if (strRET.Contains("SEL="))
                            {
                                List<string> LstCase = strRET.MidString("SEL=", "}").MySplit("|");
                                foreach (string KeyPair in LstCase)
                                {
                                    string Key = KeyPair.MidString("(", ",").Trim();
                                    string Value = KeyPair.MidString(",", ")").Trim();
                                    //if (Key == strSrcValue && !string.IsNullOrEmpty(Value))
                                    if (Key == strSrcValue)
                                    {
                                        LstValue.Add(Value);
                                        break;
                                    }
                                }
                            }
                            else if (strRET.Contains("ToHexArray"))
                            {
                                int iNum = strSrcValue.ToMyInt();
                                if (iNum > 0)
                                {
                                    string strExp = strRET.MidString("ToHexArray(", ")").Trim().Replace(" ", "");
                                    int iDest = 0;
                                    if (strExp.Contains("LineCalc"))
                                    {
                                        string strExp2 = strRET.MidString("LineCalc(", ")").Trim().Replace(" ", "");
                                        byte byGain = strExp2.MidString("", ",").ToMyByte();
                                        byte byOff = strExp2.MidString(",", "").Replace("+", "").Replace("-", "").ToMyByte();
                                        if (strExp.Contains("-"))
                                            iDest = (iNum * byGain - byOff);
                                        else
                                            iDest = (iNum * byGain + byOff);
                                        if (iDest > 0)
                                            LstValue.Add(sCommon.ByteArrayToHexString(
                                               Word.ToByteArray((UInt16)iDest, ByteOrder16.BA)));
                                    }
                                    else
                                        LstValue.Add(sCommon.ByteArrayToHexString(
                                            Word.ToByteArray((UInt16)iNum, ByteOrder16.BA)));
                                }
                            }
                            else if (strRET.Contains("ToHEX"))
                            {
                                byte byNum = strSrcValue.ToMyByte();
                                if (byNum > 0)
                                {
                                    string strExp = strRET.MidString("ToHEX(", ")").Trim().Replace(" ", "");
                                    byte byDest = 0;
                                    if (strExp.Contains("LineCalc"))
                                    {
                                        string strExp2 = strRET.MidString("LineCalc(", ")").Trim().Replace(" ", "");
                                        byte byGain = strExp2.MidString("", ",").ToMyByte();
                                        byte byOff = strExp2.MidString(",", "").Replace("+", "").Replace("-", "").ToMyByte();
                                        if (strExp.Contains("-"))
                                            byDest = (byte)(byNum * byGain - byOff);
                                        else
                                            byDest = (byte)(byNum * byGain + byOff);
                                        if (byDest > 0)
                                            LstValue.Add(sCommon.ByteArrayToHexString(new byte[] { byDest }));
                                    }
                                    else
                                        LstValue.Add(sCommon.ByteArrayToHexString(new byte[] { byNum }));
                                }
                            }
                            else if (strRET.Contains("LineCalc"))
                            {
                                byte byNum = strSrcValue.ToMyByte();
                                string strExp = strRET.MidString("LineCalc(", ")").Trim().Replace(" ", "");
                                byte byGain = strExp.MidString("", ",").ToMyByte();
                                byte byOff = strExp.MidString(",", "").Replace("+", "").Replace("-", "").ToMyByte();
                                byte byDest = 0;
                                if (strExp.Contains("-"))
                                    byDest = (byte)(byNum * byGain - byOff);
                                else
                                    byDest = (byte)(byNum * byGain + byOff);
                                if (byDest > 0)
                                    LstValue.Add(sCommon.ByteArrayToHexString(new byte[] { byDest }));
                            }
                            else if (strRET.Contains("ReadTableRow("))
                            {
                                strRET = strRET.Replace("$SRC0", strSrcValue);
                                CallResult _res2 = _DB.ExcuteMacroQuery(strRET);
                                if (_res2.Success)
                                    LstValue.Add(_res2.Result.ToMyString());
                            }
                        }
                    }
                }
                else
                {
                    ErrorMessage = $"表达式[ {FormatedExpress}] 解析错误: {FieldMark}字段参数元数量设定数量和解析数量不一致";
                }
                if (ErrorMessage.IsEmpty())
                {
                    if (LstValue.Count == CmdParamNum)
                    {
                        FormatedExpress = string.Format(FormatedExpress, LstValue.ToArray());
                        return FormatedExpress;
                    }
                    else
                    {
                        string ExpParam = LstValue.ToMyString(",", false, "\"", "\"");
                        ErrorMessage = $"表达式 [{FormatedExpress}] 计算错误: {FieldMark} 字段参数=[{ExpParam}]";
                    }
                }
            }
            return FormatedExpress;
        }

        /// <summary>
        /// 格式化命令输出
        /// 处理TaskDriver SetValue & Comment 逻辑运算
        /// </summary>
        /// <param name="taskDriver"></param>
        /// <returns></returns>
        private bool TaskDriverCommandCalc(ref ModelSystemSymbol taskDriver)
        {
            try
            {
                if (string.IsNullOrEmpty(taskDriver.SetValue))
                    return false;
                //处理TaskDriver SetValue & Comment 逻辑运算
                string strFormatedSetValue = taskDriver.SetValue.ToMyString();
                string strFormatedCommentValue = taskDriver.Comment.ToMyString();
                string strLogicExpress = taskDriver.LogicExpress.ToMyString();
                string CalculatedSetValue = TaskDriverExpressCalc(strFormatedSetValue, strLogicExpress, "CmdValue",out string ErrorCmd);
                string CalculatedCommentValue = TaskDriverExpressCalc(strFormatedCommentValue, strLogicExpress, "CommentValue",out string ErrorComment);
                //ex格式 : $"表达式 [{FormatedExpress}] 计算错误: {FieldMark} 字段参数=[{ExpParam}]"
                if (ErrorCmd.IsNotEmpty())
                    taskDriver.Comment += ErrorCmd;
                if (ErrorComment.IsNotEmpty())
                    taskDriver.Comment += "\r\n" + ErrorComment;
                taskDriver.SetValue = CalculatedSetValue;
                taskDriver.Comment = CalculatedCommentValue;
                return CalculatedSetValue.MatchParamCount() == 0;

            }
            catch (Exception ex)
            {
                taskDriver.Comment = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 启动工作
        /// </summary>
        public void StartWorker()
        {
            if (Project.Current.StartUP["TaskLogger"].IsFunctionOn())
            {
                var SampleLoggerInit = SampleLogger.Default;
            }
            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    #region 内核过程运行
                    try
                    {
                        string strSql = "update sys_symbol as d set d.CurrentValue= (" +
                        "case when" +
                        "(" +
                            "d.GroupName in " +
                            "(" +
                                "select distinct t.task_pos from core_schedule_items as t where length(t.task_pos) > 0 and " +
                                "length(t.task_detail) > 0 and t.task_state not in ('DONE', 'DISABLE') and " +
                                "t.schedule_id in (select m.id from core_schedule_menu as m where m.state in ('已运行'))" +
                            ") " +
                            "or " +
                            "d.GroupName in " +
                            "(" +
                                "select np.GroupName from(select distinct sp.GroupName from sys_symbol as sp " +
                                "where sp.Name = 'PP_KEY' and sp.CurrentValue not in('', 'MAIN')) as np" +
                            ")" +
                        ") " +
                        "then '1' else '0' end " +
                        ") " +
                        "where d.Name = 'InProcess'; ";
                        _DB.ExcuteSQL(strSql);
                        _DB.ExcuteProcedure("Proc_Core");
                        TaskWork(state);
                    }
                    catch (Exception ex)
                    {
                        Logger.Task.Write(LOG_TYPE.ERROR, string.Format("任务框架运行时错误:【{0}】", ex.Message));
                    }
                    #endregion

                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// 任务工作
        /// </summary>
        /// <param name="sender"></param>
        private void TaskWork(object state)
        {
            string strSql = string.Empty;
            #region 生产任务解析           
            try
            {
                string sql_active = string.Empty;
                #region 解析定时任务
                strSql = " select t.id,t.schedule_id,t.trig_type,t.trig_detail,t.trig_state,m.enable,m.name,m.state,m.active_time,m.exe_time,m.exe_result,m.timeout_min,m.msg_stream from core_schedule_items as t " +
               "inner join core_schedule_menu as m on t.schedule_id=m.id " +
               "where t.trig_type = 'ByScheduleTime' and t.domain = 'Lab' and (m.enable is null or m.enable != '已禁用') ";
                DataTable dt = _DB.ExcuteQuery(strSql).Result.ToMyDataTable();
                foreach (DataRow row in dt.Rows)
                {
                    string trig_id = row["id"].ToString();
                    string trig_detail = row["trig_detail"].ToString();
                    string schedule_id = row["schedule_id"].ToString();
                    string schedule_enable = row["enable"].ToString();
                    string schedule_name = row["name"].ToString();
                    string schedule_state = row["state"].ToString();
                    string active_time = row["active_time"].ToString();
                    string exe_time = row["exe_time"].ToString();
                    string exe_result = row["exe_result"].ToString();
                    string strTimeout_min = row["timeout_min"].ToString();
                    string msg_stream = row["msg_stream"].ToString();
                    double timeout_min = 0;
                    if (strTimeout_min.Length == 0)
                        timeout_min = 0;
                    else if (strTimeout_min.Contains("min"))
                    {
                        strTimeout_min = strTimeout_min.Replace("min", "");
                        double.TryParse(strTimeout_min, out timeout_min);
                    }

                    string strFreq = string.Empty;
                    string strDate = string.Empty;
                    string strTime = string.Empty;
                    DateTime t_set;
                    DateTime t_now = DateTime.Now;
                    string[] strTrigDetail = trig_detail.Split(',');
                    if (strTrigDetail.Length >= 3)
                    {
                        strTime = strTrigDetail[1];
                        strFreq = strTrigDetail[2];
                    }
                    //根据情况，设定目标运算时间
                    bool EnActive = false;
                    switch (strFreq)
                    {
                        case "一次":
                            strDate = strTrigDetail[0];
                            EnActive = true;
                            break;

                        case "每天":
                            strDate = DateTime.Now.Date.ToString("yyyy/MM/dd");
                            EnActive = true;
                            break;

                        case "每周":
                            strDate = DateTime.Now.Date.ToString("yyyy/MM/dd");
                            string dayOfWeek = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
                            if (trig_detail.Contains(dayOfWeek))
                                EnActive = true;
                            break;
                    }
                    //根据时间差确定范围
                    bool IsValid = DateTime.TryParse(strDate + " " + strTime, out t_set);
                    bool Match_ActiveTime = false;
                    bool Active_time_OldOrEmpty = false;
                    if (IsValid)
                    {
                        double secSpan = (t_now - t_set).TotalSeconds;
                        //在超过设定时间的 n 分钟以内有效
                        if (secSpan >= 0 && secSpan <= 5)
                            Match_ActiveTime = true;
                        else if (timeout_min > 0 && secSpan >= timeout_min * 60 && schedule_state == "已激活")
                        {
                            sql_active += string.Format(" update m set m.state='已超时', m.exe_result='已超时({0})' from core_schedule_menu as m where m.id = {1} and LEN(m.exe_result)=0", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), int.Parse(row["schedule_id"].ToString()));
                        }

                        if (Match_ActiveTime)
                        {
                            if (exe_time.Length > 0)
                            {
                                DateTime t1 = DateTime.Parse(active_time);
                                //已经在设定时间之后激活
                                if (DateTime.Compare(t_set, t1) > 0)
                                    Active_time_OldOrEmpty = true;
                            }
                            else
                                Active_time_OldOrEmpty = true;
                        }
                    }

                    if (EnActive && IsValid && Active_time_OldOrEmpty && exe_result != "已取消")
                        sql_active += string.Format(" update s set s.trig_state='PASS' from core_schedule_items as s where id={0} and s.trig_state!='PASS' ", trig_id);
                    else
                        sql_active += string.Format(" update s set s.trig_state='NG' from core_schedule_items as s where id={0} and s.trig_state!='NG' ", trig_id);
                }
                if (sql_active.Length > 0)
                    _DB.ExcuteSQL(sql_active);
                #endregion

                //获取并行任务项并解析
                strSql = "select d.id,d.schedule_id,d.task_state,d.task_type,d.task_detail,d.task_pos,d.task_id,d.task_item,d.task_in,d.task_out,d.task_set_val,d.msg_from,d.msg_to,d.msg_mode,d.msg_tag, m.state as schedule_state,m.exe_result,m.msg_stream,m.msg_visual from " +
                        "(" +
                            "select distinct *, (@row_number1:=case when @now_schedule_id = schedule_id then @row_number1 + 1 else 1 end) as num_mm1,(@now_schedule_id:= schedule_id) " +
                                "from(Select  @row_number1:= 0, @now_schedule_id:= 0) as b, core_schedule_items as t " +
                                "where length(t.task_detail) > 0 and t.task_state in('NORMAL', 'UPDATE') and t.schedule_id in(select m.id from core_schedule_menu as m where m.state = '已运行') " +
                                "order by t.schedule_id,t.task_order asc" +
                        ") as d inner join core_schedule_menu as m on d.schedule_id = m.id where d.num_mm1 <= 5; ";
                CallResult _result = _DB.ExcuteQuery(strSql);
                if (_result.Success)
                {
                    dt = _result.Result.ToMyDataTable();
                    List<TaskItem> ListTaskRun = ColumnDef.ToEntityList<TaskItem>(dt);
                    if (ListTaskRun != null)
                    {
                        List<TaskItem> LstTaskHandled = new List<TaskItem>();
                        TaskItem TaskHandled = new TaskItem();
                        List<string> LstHandledScheduleID = new List<string>();
                        foreach (TaskItem item in ListTaskRun)
                        {
                            if (LstHandledScheduleID.Contains(item.schedule_id))
                                continue;
                            if (item.task_state == "NORMAL" && string.IsNullOrEmpty(item.msg_tag))
                            {
                                bool EntryChanged = false;
                                string strMsgVisual = string.Empty;
                                string strMsgStream = string.Empty;
                                MsgStreamMove(item, ref EntryChanged, ref strMsgVisual, ref strMsgStream);
                                if (EntryChanged)
                                {
                                    item.msg_visual = strMsgVisual;
                                    item.msg_stream = strMsgStream;
                                }
                            }
                            if (item.task_type == "Driver")
                            {
                                string CalculatedTaskPos = string.Empty;
                                string CalculatedTaskCmd = string.Empty;
                                TaskHandled = ParseTaskDriver(item, ref CalculatedTaskPos, ref CalculatedTaskCmd);
                                if (!string.IsNullOrEmpty(CalculatedTaskPos)) item.task_pos = CalculatedTaskPos;
                                if (!string.IsNullOrEmpty(CalculatedTaskCmd)) item.task_pos = CalculatedTaskCmd;
                            }
                            else if (LstMacroCmd.Contains(item.task_type))
                                TaskHandled = ParseTaskLogicCalc(item);
                            else if (item.task_type == "Event")
                                TaskHandled = ParseTaskEvent(item);
                            if (string.IsNullOrEmpty(item.msg_tag))
                                TaskHandled.msg_tag = "Moved";
                            if (!string.IsNullOrEmpty(TaskHandled.task_state) || 
                                !string.IsNullOrEmpty(TaskHandled.msg_tag))
                            {
                                _DB.ExcuteUpdate<TaskItem>(TaskHandled);
                                //LstTaskHandled.Add(TaskHandled);
                            }
                            if (!string.IsNullOrEmpty(item.msg_from) || !LstMacroCmd.Contains(item.task_type))
                            {
                                LstHandledScheduleID.AppandList(item.schedule_id);
                            }
                        }
                        //if (LstTaskHandled.Count > 0)
                        //    _DB.ExcuteUpdate<TaskItem>(LstTaskHandled);
                        lock (TaskTakeItems)
                            TaskTakeItems = ListTaskRun;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }

        /// <summary>
        /// 信息流传递
        /// </summary>
        /// <param name="task"></param>
        /// <param name="EntryChanged">任务列表头信息更新</param>
        /// <param name="refMsgVisual"></param>
        /// <param name="refMsgStream"></param>
        private void MsgStreamMove(TaskItem task,ref bool EntryChanged, ref string refMsgVisual,ref string refMsgStream)
        { 
            string msg_visual = string.Empty;
            string msg_stream = string.Empty;
            string strSql = string.Empty;
            if (task.msg_from.Length > 0)
            {
                //明确指定了信息流源--通知内核层更新到流程体
                msg_visual = ReadPosItem(task.msg_from, "VIEW_SI");
                msg_stream = ReadPosItem(task.msg_from, "SAMPLE_MSG");
                refMsgVisual = msg_visual;
                task.msg_visual = msg_visual;
                refMsgStream = msg_stream;
                EntryChanged = true;
                strSql = string.Format(" update core_schedule_menu as m set m.msg_stream='{0}',m.msg_visual='{1}' where m.id = {2} ",
                    msg_stream, msg_visual, task.schedule_id.ToString());
                _DB.ExcuteSQL(strSql);

                if (task.msg_mode.Contains("KillSource"))
                {
                    WriteTaskedPosItem(task.msg_from, "VIEW_SI", "", msg_visual);
                    WriteTaskedPosItem(task.msg_from, "SAMPLE_MSG", "", msg_visual);
                }
            }
            else
            {
                msg_visual = task.msg_visual;
                msg_stream = task.msg_stream;
            }
            //关联工位之间直接传递
            if (task.msg_to.Length > 0 && msg_visual.Length > 0)
            {
                WriteTaskedPosItem(task.msg_to, "VIEW_SI", msg_visual, msg_visual);
                WriteTaskedPosItem(task.msg_to, "SAMPLE_MSG", msg_stream,msg_visual);
                if (task.msg_mode.Contains("KeepCopy") && task.task_pos != task.msg_to)
                {
                    WriteTaskedPosItem(task.task_pos, "VIEW_SI", msg_visual,msg_visual);
                    WriteTaskedPosItem(task.task_pos, "SAMPLE_MSG", msg_stream, msg_visual);
                }
            }
            else
            {
                //无需处理，任务下达时若无样号，指定...
            }
        }

        /// <summary>
        /// 解析驱动项
        /// </summary>
        /// <param name="task"></param>
        /// <param name="LogicPosName">任务驱动使用代理 - 转换逻辑工位名称</param>
        /// <param name="LogicTaskCommand">任务驱动使用代理 - 转换逻辑命令名称</param>
        /// <returns></returns>
        private TaskItem ParseTaskDriver(TaskItem task,ref string LogicPosName,ref string LogicTaskCommand)
        {
            if (task == null) 
                throw new Exception("解析任务项时任务项无效");
            TaskItem TaskHandled = new TaskItem() { id = task.id.MarkWhere() };
            string strTaskType = task.task_type.ToMyString();
            string strTaskRelatedPos = task.task_pos.ToMyString();
            string strMsgVisual = task.msg_visual;
            //string strTaskRelatedItem = task.task_item.ToMyString();
            string TryTaskCommand = task.task_id.ToMyString();
            if (strTaskType != "Driver" || string.IsNullOrEmpty(TryTaskCommand))
                return TaskHandled;
            ModelSystemSymbol symbol = ReadSymbolItem(strTaskRelatedPos, TryTaskCommand, "TaskDriver");
            symbol.LogicExpress = symbol.LogicExpress.MyReplace("$VIEW_SI", strMsgVisual);
            //任务驱动代理 - 转换逻辑
            if (symbol.LogicExpress.Contains("ReGroupName=[") || symbol.LogicExpress.Contains("ReName=["))
            {
                string strFormatedNameValue = symbol.Name.ToMyString();
                if (symbol.LogicExpress.Contains("ReGroupName=["))
                {
                    string strFormatedGroupNameValue = GetExpressField(symbol.LogicExpress, "GroupName");
                    string CalculatedGroupNameValue = TaskDriverExpressCalc(strFormatedGroupNameValue, symbol.LogicExpress, "ReGroupName");
                    strTaskRelatedPos = CalculatedGroupNameValue;
                    LogicPosName = strTaskRelatedPos;
                }
                if (symbol.LogicExpress.Contains("ReName=["))
                {
                    string CalculatedNameValue = TaskDriverExpressCalc(strFormatedNameValue, symbol.LogicExpress, "ReName");
                    TryTaskCommand = CalculatedNameValue;
                    LogicTaskCommand = TryTaskCommand;
                }
                if (!string.IsNullOrEmpty(strTaskRelatedPos) && !string.IsNullOrEmpty(TryTaskCommand))
                    symbol = ReadSymbolItem(strTaskRelatedPos, TryTaskCommand, "TaskDriver");
                else
                {
                    //指令转换错误
                    if (LastError.ToMyString() != symbol.Comment.ToMyString())
                    {
                        LastError = symbol.Comment.ToMyString();
                        Logger.Task.Write(LOG_TYPE.ERROR, string.Format("任务指令转换错误:【{0}】", symbol.Comment));
                    }
                    return TaskHandled;
                }
            }
            symbol.LogicExpress = symbol.LogicExpress.MyReplace("$VIEW_SI", strMsgVisual);
            WorkPos.UpdateItem(strTaskRelatedPos);
            if (task.task_state == "NORMAL" && !string.IsNullOrEmpty(symbol.LogicExpress))
            {
                bool TaskInValid = TaskDrvierExpressValidOfIN(symbol.LogicExpress);
                if (TaskInValid && TryTaskCommand.Length > 0)
                {
                    if (!string.IsNullOrEmpty(WorkPos.TaskCommand))
                        WritePosItem(strTaskRelatedPos, "TASK_CMD", "");
                    if (WorkPos.TaskState.ToMyString() != "NORMAL")
                        WritePosItem(strTaskRelatedPos, "TASK_STATE", "NORMAL");
                    //发送任务指令
                    if (!string.IsNullOrEmpty(symbol.Name) && !string.IsNullOrEmpty(symbol.RelatedDriver))
                    {
                        bool CommandOK = true;
                        bool CommandWriten = false;
                        string strSetValue = symbol.SetValue.ToMyString();
                        if (strSetValue.MatchParamCount() > 0)
                            CommandOK = TaskDriverCommandCalc(ref symbol);
                        if (!CommandOK)
                        {
                            //指令转换错误
                            if (LastError.ToMyString() != symbol.Comment.ToMyString())
                            {
                                LastError = symbol.Comment.ToMyString();
                                Logger.Task.Write(LOG_TYPE.ERROR, $"任务指令【{symbol?.Name}】运算错误:【 {symbol.Comment}】");
                                if (Project.Current.StartUP["TaskLogger"].IsFunctionOn())
                                    TaskLogger.Default.Write(LOG_TYPE.ERROR, strMsgVisual, strTaskRelatedPos, symbol.Comment);
                            }
                            return TaskHandled;
                        }
                        strSetValue = symbol.SetValue.ToMyString();
                        if (strTaskRelatedPos.ToLower().Contains("robot"))
                        {
                            if (!string.IsNullOrEmpty(strSetValue) && CommandOK)
                            {
                                CommandWriten = WriteCommand(symbol.GroupName, "CmdTaskRouting", strSetValue, true, symbol.Name);
                            }
                        }
                        else
                            CommandWriten = WriteCommandByDataName(symbol.RelatedDriver, symbol.Name, strSetValue, false, symbol.Name);
                        if (CommandWriten)
                        {
                            //清空任务命令返回值
                            WriteTaskDriverValue(symbol.RelatedDriver, symbol.Name, "");
                            Logger.Task.Write(LOG_TYPE.MESS, string.Format("任务下发成功:【{0}】", symbol.Comment));
                            if (Project.Current.StartUP["TaskLogger"].IsFunctionOn())
                            {
                                TaskLogger.Default.Write(LOG_TYPE.MESS, strMsgVisual, strTaskRelatedPos, symbol.Comment);
                            }
                            //添加样品任务记录项
                            if (symbol.CustomMark.MyContains("RecAndEndBy", StringMatchMode.IgnoreCase))
                            {
                                var TpTask = new Tuple<TaskItem, ModelSystemSymbol, string>(task, symbol, strMsgVisual);
                                Messenger.Default.Send(TpTask, TaskDriverStarted);
                            }
                            WritePosItem(strTaskRelatedPos, "TASK_CMD", TryTaskCommand);
                            WritePosItem(strTaskRelatedPos, "TASK_STATE", "ACCEPTED");
                            TaskHandled.task_state = "UPDATE";
                        }
                    }
                }
            }
            else if (task.task_state == "UPDATE")
            {
                if (WorkPos.PosState == "RUN" && WorkPos.TaskState != "RUN")
                    WritePosItem(strTaskRelatedPos, "TASK_STATE", "RUN");
 
                bool TaskOutValid = false;
                if (ContainsField(symbol.LogicExpress, "OUT"))
                    TaskOutValid = TaskDrvierExpressValidOfOUT(symbol.LogicExpress);
                else
                    TaskOutValid = !string.IsNullOrEmpty(symbol.CurrentValue);
                if (TaskOutValid || (task.task_out == "RUN" && WorkPos.TaskState == "RUN"))
                {
                    //;结束脚本在此添加  
                    if (symbol.CustomMark.MyContains("RecAndEndByTask", StringMatchMode.IgnoreCase))
                    {
                        var TpTask = new Tuple<ModelSystemSymbol, string>(symbol, strMsgVisual);
                        Messenger.Default.Send(TpTask, TaskDriverFinished);
                    }
                    if (strTaskRelatedPos.ToLower().Contains("robot"))
                    {
                        string strPPKey = ReadPosItem(strTaskRelatedPos, "PP_KEY");
                        if (string.IsNullOrEmpty(strPPKey))
                            WritePosItem(task.task_pos, "VIEW_SI", "");
                    }
                    WritePosItem(task.task_pos, "TASK_STATE", "NORMAL");
                    WritePosItem(strTaskRelatedPos, "TASK_CMD", "");
                    TaskHandled.task_state = "DONE";
                }
            }
            return TaskHandled;
        }

        /// <summary>
        /// 解析逻辑运算
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private TaskItem ParseTaskLogicCalc(TaskItem task)
        {
            if (task == null)
                throw new Exception("解析任务项时任务项无效");
            TaskItem TaskHandled = new TaskItem() { id = task.id.MarkWhere() };
            string strTaskType = task.task_type.ToMyString();
            string strTaskRelatedPos = task.task_pos.ToMyString();
            string strTaskRelatedItem = task.task_item.ToMyString();
            if (!LstMacroCmd.Contains(strTaskType))
                return TaskHandled;
            bool DataMoveSuccess = false;
            if (strTaskType == "DataMove.=" && task.task_state == "NORMAL")
            {
                string set_val = task.task_set_val.ToMyString().Trim();
                string[] strSource = set_val.Split('.');
                string strItemVal = string.Empty;
                if (set_val.StartsWith("\"") && set_val.EndsWith("\""))
                {
                    strItemVal = set_val.MidString("\"", "\"", EndStringSearchMode.FromHead);
                }
                else if (strSource.Length >= 2)
                {
                    //strItemVal = ReadPosItem(strSource[0], strSource[1]);
                    string strPosKey = set_val.MidString("", ".", EndStringSearchMode.FromTail);
                    string strPosItem = set_val.MidString(strPosKey + ".", "");
                    strItemVal = ReadPosItem(strPosKey, strPosItem);
                }
                else
                {
                    strItemVal = set_val;
                }

                if (!strItemVal.Contains(SystemDefault.ReplaceSign))
                {
                    //没有换元符，传递原值
                    DataMoveSuccess = WriteTaskedPosItem(strTaskRelatedPos, strTaskRelatedItem, 
                        strItemVal, task.msg_visual);
                }
                else
                {
                    //参数值右 有换元符，有没有前、后缀
                    string msg_visual = task.msg_visual;
                    if (strItemVal.ToMyString().Trim() == SystemDefault.ReplaceSign)
                    {
                        DataMoveSuccess = WriteTaskedPosItem(strTaskRelatedPos, strTaskRelatedItem,
                            msg_visual, task.msg_visual);
                    }
                    else
                    {
                        msg_visual = msg_visual.MidString("", SystemDefault.LinkSign);
                        strItemVal = strItemVal.Replace(SystemDefault.ReplaceSign, msg_visual);
                        DataMoveSuccess = WriteTaskedPosItem(strTaskRelatedPos, strTaskRelatedItem, strItemVal, strItemVal);
                    }
                    WritePosItem(strTaskRelatedPos, "SAMPLE_MSG", task.msg_stream);
                }
                if (DataMoveSuccess)
                {
                    Logger.Task.Write(LOG_TYPE.MESS, string.Format("逻辑运算: DataMove.=:【{0}.{1}=[{2}]】",
                      strTaskRelatedPos, strTaskRelatedItem, strItemVal));
                    TaskHandled.task_state = "DONE";
                }
            }
            else if (strTaskType == "Math.++" && task.task_state == "NORMAL")
            {
                int val = ReadPosItem(strTaskRelatedPos, strTaskRelatedItem).ToMyInt();
                WritePosItem(strTaskRelatedPos, strTaskRelatedItem, (val + 1).ToString());
                Logger.Task.Write(LOG_TYPE.MESS, string.Format("逻辑运算: Math.++:【{0}.{1}=[{2}]】",
                      strTaskRelatedPos, strTaskRelatedItem, (val + 1).ToString()));
                TaskHandled.task_state = "DONE";
            }
            return TaskHandled;
        }

        /// <summary>
        /// 解析事件
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected virtual TaskItem ParseTaskEvent(TaskItem task)
        {
            TaskItem TaskHandled = new TaskItem() { id = task.id.MarkWhere(), task_state = "DONE" };
            //TaskHandled.task_state = "UPDATE";
            return TaskHandled;
        }

        /// <summary>
        /// 写入工位参数 - 带事件关联
        /// </summary>
        /// <param name="PosName"></param>
        /// <param name="ItemName"></param>
        /// <param name="value"></param>
        /// <param name="RelatedMsgVisual"></param>
        /// <returns></returns>
        public virtual bool WriteTaskedPosItem(string PosName, string ItemName, string value, string RelatedMsgVisual)
        {
            return WritePosItem(PosName, ItemName, value);
        }
    }
}
