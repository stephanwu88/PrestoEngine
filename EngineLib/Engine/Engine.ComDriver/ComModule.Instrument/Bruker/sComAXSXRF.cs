using Engine.Common;
using Engine.Core.TaskSchedule;
using Engine.Data;
using Engine.Data.DBFAC;
using Engine.Mod;
using Engine.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Xml;

namespace Engine.ComDriver.Bruker
{
    /// <summary>
    /// 指令请求枚举
    /// </summary>
    public enum Enum_REQ
    {
        [Description("STATUS")]
        RequestXRFStatus = 1,

        [Description("STATUS")]
        RequestSampleStatus = 2,

        [Description("STATUS")]
        RequestSampleListStatus = 3,

        [Description("STATUS")]
        RequestFreePos = 4,

        [Description("MEASMP")]
        RequestMeasure = 5,

        [Description("READRS")]
        ReadResult = 6,

        //[Description("SMPUNL")]
        //HostRemoveSample = 7,

        [Description("LOADSM")]
        LoadSample = 8,

        [Description("UNLOAD")]
        UnLoadSample = 9,

        [Description("CANCEL")]
        CancelSample = 10,

        //[Description("RESETED")]
        //ResetDriver = 11,

        [Description("START")]
        StartMeasure = 12,

        [Description("STOP")]
        StopMeasure = 13,

        [Description("ResetProc")]
        ResetProc,
    }

    /// <summary>
    /// 指令反馈枚举
    /// </summary>
    public enum Enum_ACK
    {
        [Description("@STATUS_MELDUNG")]
        SpecStatus = 1
    }

    /// <summary>
    /// 请求字段
    /// </summary>
    public class ReqField
    {
        /// <summary>
        /// SampleName: 样品名称 ex:Sample1
        /// </summary>
        public string _Request_SampleName__ { get; set; } = string.Empty;
        /// <summary>
        /// Program:  任务管理插件中配置的任务名称 Clinker:渣样料 Rawmeal:原料 CemIII InstrumentCheck
        /// </summary>
        public string _Request_Program__ { get; set; } = string.Empty;
        /// <summary>
        /// MagzinePos: 注册位置 矩阵盘进样 ex:1A01  自动线进样： 0, 荧光自动分配空闲位置
        /// </summary>
        public string _Request_MagzinePos__ { get; set; } = "0";
        /// <summary>
        /// //Mode: 样品模式  UN: 例行分析  RE: 校准分析
        /// </summary>
        public string _Request_Mode__ { get; set; } = "UN";
        /// <summary>
        /// //PEP:  固定为0
        /// </summary>
        public string _Request_REP__ { get; set; } = "0";
        /// <summary>
        /// //Time: Time Scale时间因数  0.01-10
        /// </summary>
        public string _Request_TimeScale__ { get; set; } = "1.00";
        /// <summary>
        /// Transfer: 传输位置  Z01..Z04
        /// </summary>
        public string _Request_TransferPos__ { get; set; } = "Z01";
        /// <summary>
        /// Format: 结果数据格式 1,2,8,9 or 10,详见 READRS Command
        /// </summary>
        public string _Request_ResultFormat__ { get; set; } = "1";
        /// <summary>
        /// PRIO: 优先级 00-99 最高-最低
        /// </summary>
        public string _Request_PRIO__ { get; set; } = "50";
    }

    /// <summary>
    /// 响应字段
    /// </summary>
    public class AckField
    {
       
    }

    /// <summary>
    /// 会话区
    /// </summary>
    public class SessionMemory : NotifyObject
    {
        //仪器状态代码
        public string StatusCode_Ins { get; set; } = string.Empty;
        //样品状态代码
        public string StatusCode_Sample { get; set; } = string.Empty;
        //仪器错误信息代码
        public string StatusCode_Ins_Error{ get; set; } = string.Empty;
        //样品错误信息代码
        public string StatusCode_Sample_Error { get; set; } = string.Empty;
        //指令错误信息代码
        public string CommandCode_Error { get; set; } = string.Empty;

        //仪器状态文本
        public string StatusText_Ins { get; set; } = string.Empty;
        //样品状态文本
        public string StatusText_Sample { get; set; } = string.Empty;
        //仪器错误信息文本
        public string StatusText_Ins_Error { get; set; } = string.Empty;
        //样品错误信息文本
        public string StatusText_Sample_Error { get; set; } = string.Empty;
        //指令错误信息文本
        public string CommandText_Error { get; set; } = string.Empty;

        public string State_Normal { get; set; } = "0";
        public string State_Run { get; set; } = "0";
        public string State_Alarm { get; set; } = "0";
        public string State_Wait { get; set; } = "0";
        public string State_Done { get; set; } = "0";
        public string Status_Offline { get; set; } = "0";
        public string Status_Online { get; set; } = "0";
        public string ComShake { get; set; } = "0";

        /// <summary>
        /// 任务队列样品数量
        /// </summary>
        public string TaskSampleNumber { get; set; } = "0";
        /// <summary>
        /// 荧光内可供自动化线占用的空余工位数量
        /// </summary>
        public string FreeWorkPosNumber { get; set; } = "0";
        /// <summary>
        /// 荧光内有工位可用
        /// </summary>
        public string WorkPosReady { get; set; } = "0";
        /// <summary>
        /// 测量指令通过
        /// </summary>
        public string Proc_MeasureAllowed { get; set; } = "0";
        public string Proc_Error { get; set; }
        public string Proc_Warn { get; set; }
        //测量流程步骤
        public string Proc_MeasureStep
        {
            get
            {
                if (string.IsNullOrEmpty(_Proc_MeasureStep))
                    _Proc_MeasureStep = "分析任务待命";
                return _Proc_MeasureStep;
            }
            set
            {
                _Proc_MeasureStep = value;
                RaisePropertyChanged("Proc_MeasureStep");
            }
        }
        private string _Proc_MeasureStep;
    }

    /// <summary>
    /// Bruker Spectro Protocol
    /// </summary>
    public class sComAXSXRF : sComAXSXRF<ServerNode>
    {
        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComAXSXRF(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {

        }
    }

    /// <summary>
    /// Bruker Spectro Protocol
    /// 布鲁克S8系列光谱仪
    /// </summary>
    public partial class sComAXSXRF<TConNode> : sComRuleAsc
        where TConNode : new()
    {
        public static readonly string AnaDataReceived = SystemDefault.UUID;
        public static readonly string AnaDataParseError = SystemDefault.UUID;
        public static readonly string SampleListUpdated = SystemDefault.UUID;
        private IDBFactory<TConNode> _DB = DbFactory.Current.GetConn<TConNode>("nd_work");
        public ReqField ReqField = new ReqField();
        public AckField AckField = new AckField();
        public SessionMemory Session { get; } = new SessionMemory();
        /// <summary>
        /// 协议变量列表
        /// </summary>
        List<ModelComInstr> ListComSymbol;
        List<string> LastRcvStatus = new List<string>();
        static XmlDocument FieldMapDoc;
        List<string> FaultErrorList = new List<string>();
        List<string> StatusErrorList = new List<string>();
        List<string> CommandErrorList = new List<string>();
        protected Dictionary<string, ModelSheetColumn> DicResultConfig
            = new Dictionary<string, ModelSheetColumn>();
        /// <summary>
        /// 报文字典
        /// </summary>
        public Dictionary<Enum_REQ, Telegram> _REQTelegrams = new Dictionary<Enum_REQ, Telegram>();
        public Dictionary<Enum_ACK, Telegram> _ACKTelegrams = new Dictionary<Enum_ACK, Telegram>();
        public Dictionary<string, ComTask> DicComTask = new Dictionary<string, ComTask>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public sComAXSXRF()
        {
            STX = "";
            //默认为Cr, 也可设置修改为Crlf，与仪器设置同步
            EDX = SystemDefault.Cr;
            //EDX = SystemDefault.Crlf;
            foreach (Enum_REQ req in Enum.GetValues(typeof(Enum_REQ)))
                _REQTelegrams.Add(req, new Telegram() { MsgType = req.FetchDescription() });
            foreach (Enum_ACK ack in Enum.GetValues(typeof(Enum_ACK)))
                _ACKTelegrams.Add(ack, new Telegram() { MsgType = ack.FetchDescription() });
            UpdateTelegram();
            BulidMessageList();
        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComAXSXRF(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            STX = "";
            //默认为Cr, 也可设置修改为Crlf，与仪器设置同步
            EDX = SystemDefault.Cr;
            //EDX = SystemDefault.Crlf;
            foreach (Enum_REQ req in Enum.GetValues(typeof(Enum_REQ)))
                _REQTelegrams.Add(req, new Telegram() { MsgType = req.FetchDescription() });
            foreach (Enum_ACK ack in Enum.GetValues(typeof(Enum_ACK)))
                _ACKTelegrams.Add(ack, new Telegram() { MsgType = ack.FetchDescription() });
            UpdateTelegram();
            BulidMessageList();
            if (_DB != null)
                LoadProtocolTable();
        }

        /// <summary>
        /// 创建信息列表
        /// </summary>
        private void BulidMessageList()
        {
            FieldMapDoc = sCommon.LoadResourceXml("Engine.ComDriver/ComModule.Instrument/Bruker.ComField.xml", out bool Success);
        }

        /// <summary>
        /// 加载协议列表
        /// </summary>
        /// <param name="ListComData"></param>
        public void LoadProtocolTable(List<ModelComInstr> ListComData)
        {
            if (ListComData != null && ListComSymbol == null)
                return;
            ListComSymbol = ListComData;
        }

        /// <summary>
        /// 加载数据库协议表
        /// </summary>
        private void LoadProtocolTable()
        {
            if (ListComSymbol != null) return;
            ModelComInstr modelCom = new ModelComInstr()
            {
                DriverToken = DriverItem.DriverToken,
                DataUnitType = "DataUnitType in ('Status','Alarm')".MarkExpress().MarkWhere()
            };
            DataTable dt = _DB.ExcuteQuery<ModelComInstr>(modelCom).Result.ToMyDataTable();
            ListComSymbol = ColumnDef.ToEntityList<ModelComInstr>(dt);
            LoadToSessionMemory();
        }

        /// <summary>
        /// 加载到会话内存
        /// </summary>
        private void LoadToSessionMemory()
        {
            if (ListComSymbol == null) return;
            foreach (ModelComInstr mod in ListComSymbol)
            {
                if (string.IsNullOrEmpty(mod.DataValue))
                    continue;
                Session.SetPropValue(mod.DataName, mod.DataValue);
            }
        }

        /// <summary>
        /// 网络状态变化
        /// </summary>
        /// <param name="obj"></param>
        protected sealed override void _Com_StateChanged(sSocket sender, SocketState State)
        {
            if (_DB == null)
                return;
            ModelComLink comLink = new ModelComLink()
            {
                AppName = SystemDefault.AppName.MarkWhere(),
                DriverName = DriverItem.DriverName.MarkWhere(),
                ComLink = "TCP/IP".MarkWhere(),
                LinkState = State == SocketState.Connected ? "1" : "0"
            };
            _DB.ExcuteUpdate<ModelComLink>(comLink);
        }

        /// <summary>
        /// 更新发送报文内容
        /// </summary>
        private void UpdateTelegram()
        {
            #region 问询报文
            //RequestXRFStatus 无参
            //_REQTelegrams[Enum_REQ.RequestXRFStatus].Add(new List<TelegramField>() {

            //});

            _REQTelegrams[Enum_REQ.RequestSampleStatus].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ },
            });

            _REQTelegrams[Enum_REQ.RequestSampleListStatus].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "系统固定字段值", FieldType = "MUST", FiledMark = "", FieldVal = "$ALL" },
            });

            _REQTelegrams[Enum_REQ.RequestFreePos].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "系统固定字段值", FieldType = "MUST", FiledMark = "", FieldVal = "$FREE" },
            });

            _REQTelegrams[Enum_REQ.RequestMeasure].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ },
                new TelegramField() { FieldName = "Program", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_Program__ },
                new TelegramField() { FieldName = "MagzinePos", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_MagzinePos__ },
                new TelegramField() { FieldName = "Mode", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_Mode__ },
                new TelegramField() { FieldName = "Rep", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_REP__ },
                new TelegramField() { FieldName = "TimeScale", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_TimeScale__ },
                new TelegramField() { FieldName = "TransferPos", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_TransferPos__  },
                new TelegramField() { FieldName = "ResultFormat", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_ResultFormat__  },
                new TelegramField() { FieldName = "PRIO", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_PRIO__  },
            });

            _REQTelegrams[Enum_REQ.ReadResult].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ },
            });

            //_REQTelegrams[Enum_REQ.HostRemoveSample].Add(new List<TelegramField>() {
            //    new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ },
            //});

            _REQTelegrams[Enum_REQ.LoadSample].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ },
                new TelegramField() { FieldName = "TransferPos", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_TransferPos__ },
                new TelegramField() { FieldName = "MagzinePos", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_MagzinePos__ },
            });

            _REQTelegrams[Enum_REQ.UnLoadSample].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ }
            });

            _REQTelegrams[Enum_REQ.CancelSample].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ },
            });

            _REQTelegrams[Enum_REQ.StartMeasure].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ },
            });

            _REQTelegrams[Enum_REQ.StopMeasure].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "SampleName", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Request_SampleName__ },
            });

            #endregion

            #region 响应报文
            //_ACKTelegrams[Enum_ACK.SpecStatus].Add(new List<TelegramField>() {
            //    new TelegramField() { FieldName = "GERAET", FieldType = "OPTION", FiledMark = "", FieldVal = Session.StatusCode_Ins },
            //});
            #endregion
        }

        /// <summary>
        /// 请求仪器状态
        /// </summary>
        /// <returns></returns>
        public bool RequestStatus()
        {
            return RequestStatus(true);
        }

        /// <summary>
        /// 请求仪器状态
        /// </summary>
        /// <returns></returns>
        public bool RequestStatus(bool LogSend)
        {
            string str = string.Empty;
            if (LogSend) LastRcvStatus.Clear();
            Request(Enum_REQ.RequestXRFStatus, out str, LogSend);
            Request(Enum_REQ.RequestSampleListStatus, out str, LogSend);
            Request(Enum_REQ.RequestFreePos, out str, LogSend);
            return true;
        }

        /// <summary>
        /// 复位样品任务
        /// </summary>
        /// <returns></returns>
        public bool ResetSampleTask()
        {
            ClearProcStatus();
            return true;
        }

        /// <summary>
        /// 清除流程状态
        /// </summary>
        private void ClearProcStatus()
        {
            Session.Proc_Error = string.Empty;
            Session.Proc_MeasureStep = "分析任务待命";
            FaultErrorList.Clear();
            CommandErrorList.Clear();
            StatusErrorList.Clear();
            RequestStatus(false);
        }

        public void RequestXRFStatus()
        {
            LastRcvStatus.Clear();
            Request(Enum_REQ.RequestXRFStatus, out string str, true);
        }

        public void RequestFreePos()
        {
            LastRcvStatus.Clear();
            Request(Enum_REQ.RequestFreePos, out string str, true);
        }

        /// <summary>
        /// 启动测量任务
        /// </summary>
        /// <param name="SampleName"></param>
        /// <param name="AnaProgram"></param>
        public async void StartSampleTask(string SampleName, string AnaProgram)
        {
            ComTask task = new ComTask();
            task.Put(new ComItem() { RelatedGroup = DriverItem.DriverName.ToMyString(), Command = "RequestXRFStatus", OverTime = 5 });
            task.Put(new ComItem() { RelatedGroup = DriverItem.DriverName.ToMyString(), Command = "RequestFreePos", OverTime = 5 });
            task.Put(new ComItem() { RelatedGroup = DriverItem.DriverName.ToMyString(), Command = "ReserveSample", Param = new string[] { SampleName, AnaProgram }, OverTime = 8 });
            DicComTask.AppandDict("SampleMeasure", task);
            CallResult res = await task.StartTask(this);
            if (res.Success)
                Logger.Task.Write(LOG_TYPE.MESS, $"荧光启动测量任务成功:样品【{SampleName}】，分析方法【{AnaProgram}】");
            else
                Logger.Task.Write(LOG_TYPE.ERROR, $"荧光启动测量任务失败:原因【{res.Result.ToMyString()}】");
        }

        /// <summary>
        /// 注册样品信息
        /// </summary>
        /// <param name="SampleName">样品名称</param>
        /// <param name="AnaProgram">分析程序</param>
        /// <returns></returns>
        public bool ReserveSample(string SampleName, string AnaProgram)
        {
            ReqField._Request_SampleName__ = SampleName;
            ReqField._Request_Program__ = AnaProgram;
            ReqField._Request_MagzinePos__ = "Z01";
            ReqField._Request_TransferPos__ = "0";
            ReqField._Request_PRIO__ = "50";
            ReqField._Request_TimeScale__ = "1.00";
            Session.Proc_MeasureAllowed = string.Empty;
            return Request(Enum_REQ.RequestMeasure, out string Telegram);
        }

        /// <summary>
        /// 卸载样品
        /// </summary>
        /// <param name="SampleName"></param>
        /// <returns></returns>
        public bool UnloadSample(string SampleName)
        {
            ReqField._Request_SampleName__ = SampleName;
            return Request(Enum_REQ.UnLoadSample, out string Telegram);
        }


        /// <summary>
        /// 取消样品
        /// </summary>
        /// <param name="SampleName"></param>
        /// <returns></returns>
        public bool CancelSample(string SampleName)
        {
            ReqField._Request_SampleName__ = SampleName;
            return Request(Enum_REQ.CancelSample, out string Telegram);
        }

        ///// <summary>
        ///// 创建样品任务
        ///// </summary>
        ///// <param name="SampleName"></param>
        ///// <param name="AnaProgram"></param>
        ///// <returns></returns>
        //public bool StartSampleTask(string SampleName, string AnaProgram)
        //{
        //    Session.Proc_MeasureStep = "样品流程启动";
        //    Task.Factory.StartNew(() =>
        //    {
        //        while (Session.Proc_MeasureStep.ToMyString() != "分析任务待命")
        //        {
        //            switch (Session.Proc_MeasureStep)
        //            {
        //                case "样品流程启动":
        //                    RequestStatus();
        //                    Session.Proc_MeasureStep = "进样前确认状态";
        //                    break;

        //                case "进样前确认状态":
        //                    if (Session._StatusCode_Ins__ == "01" && Session._StatusCode_Ins_Error__ == "00")
        //                    {
        //                        //注册样品
        //                        ReserveSample(SampleName, AnaProgram);
        //                        Session.Proc_MeasureStep = "样品进样请求";
        //                    }
        //                    else if ((!string.IsNullOrEmpty(Session._StatusCode_Ins__) && Session._StatusCode_Ins__ != "01") ||
        //                        (!string.IsNullOrEmpty(Session._StatusCode_Ins_Error__) && Session._StatusCode_Ins_Error__ != "00"))
        //                    {
        //                        Session.Proc_MeasureStep = "分析任务待命";
        //                        Session.Proc_Error = "荧光及进样装置未就绪";
        //                    }
        //                    break;

        //                case "样品进样":
        //                    if (Session.Proc_ReservationResponseCode == "0")
        //                    {
        //                        ClearProcStatus();
        //                        Session.Proc_MeasureStep = "允许样品放置";
        //                    }
        //                    else if (!string.IsNullOrEmpty(Session.Proc_ReservationResponseCode) && Session.Proc_ReservationResponseCode != "0")
        //                    {
        //                        Session.Proc_MeasureStep = "分析任务待命";
        //                        Session.Proc_Error = "拒绝进样请求";
        //                    }
        //                    break;
        //            }
        //            Thread.Sleep(100);
        //        }
        //    });
        //    return true;
        //}

        /// <summary>
        /// 协议请求
        /// </summary>
        /// <param name="eREQ"></param>
        /// <param name="strTelegram"></param>
        public bool Request(Enum_REQ eREQ, out string strTelegram, bool LogSend = true)
        {
            string strMsg = string.Empty;
            if (eREQ == Enum_REQ.ResetProc)
            {
                ClearProcStatus();
                strTelegram = string.Empty;
                return true;
            }
            UpdateTelegram();
            strMsg = _REQTelegrams[eREQ].MsgType;
            switch (eREQ)
            {
                case Enum_REQ.RequestXRFStatus:
                    Session.StatusCode_Ins = string.Empty;
                    Session.StatusCode_Ins_Error = string.Empty;
                    if (LogSend) LastRcvStatus.Clear();
                    break;
                case Enum_REQ.RequestSampleStatus:
                case Enum_REQ.RequestSampleListStatus:
                case Enum_REQ.RequestFreePos:
                    if(LogSend) LastRcvStatus.Clear();
                    break;
            }
            foreach (var fld in _REQTelegrams[eREQ].Fields.Values)
            {
                if (fld.FieldType == "OPTION" && string.IsNullOrEmpty(fld.FieldVal.ToMyString()))
                    continue;
                string strFldMarkLeft = fld.FiledMark == "Ref" ? "\"" : "";
                string strFldMarkRight = fld.FiledMark == "Ref" ? "\"" : "";
                if (string.IsNullOrEmpty(strFldMarkLeft) && string.IsNullOrEmpty(strFldMarkRight))
                {
                    if (fld.FieldVal.Trim().Contains(SystemDefault.Space))
                    {
                        strFldMarkLeft = "\"";
                        strFldMarkRight = "\"";
                    }
                }
                strMsg += $"{SystemDefault.Space}{strFldMarkLeft}{fld.FieldVal}{strFldMarkRight}";
            }
            strTelegram = strMsg;
            return base.Send(strMsg, LogSend);
        }

        /// <summary>
        /// 协议回应
        /// </summary>
        public bool ACK(Enum_ACK eAck, out string strTelegram, bool LogSend = true)
        {
            string strMsg = string.Empty;
            UpdateTelegram();
            strMsg = _ACKTelegrams[eAck].MsgType;
            foreach (var fld in _ACKTelegrams[eAck].Fields.Values)
            {
                if (fld.FieldType == "OPTION" && string.IsNullOrEmpty(fld.FieldVal.ToMyString()))
                    continue;
                string strFldMarkLeft = fld.FiledMark == "Ref" ? "\"" : "";
                string strFldMarkRight = fld.FiledMark == "Ref" ? "\"" : "";
                if (string.IsNullOrEmpty(strFldMarkLeft) && string.IsNullOrEmpty(strFldMarkRight))
                {
                    if (fld.FieldVal.Trim().Contains(SystemDefault.Space))
                    {
                        strFldMarkLeft = "\"";
                        strFldMarkRight = "\"";
                    }
                }
                strMsg += $"{SystemDefault.Space}{strFldMarkLeft}{fld.FieldVal}{strFldMarkRight}";
            }
            strTelegram = strMsg;
            return base.Send(strMsg, LogSend);
        }

        /// <summary>
        /// 数据消费者
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueRcvWorker(object state)
        {
            DateTime dtStart = DateTime.Now;
            SocketState LastComState = SocketState.Nothing;
            while (ComWorking)
            {
                try
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > 0)
                        {
                            dtStart = DateTime.Now;
                            Session.Status_Online = "1";
                            string strMessage = _QueueRcv.Dequeue();
                            TelegramParsedResult parseRes = ParseRecievedContent(strMessage);
                            AddRcvCommLog(parseRes?.CommandText.ToMyString());
                            AddRcvCommLog(parseRes?.StatusText.ToMyString());
                            AddRcvCommLog(parseRes?.ErrorText.ToMyString());
                            if (parseRes?.CommandName == "STATUS")
                            {
                                LastRcvStatus.AppandList(parseRes?.CommandText.ToMyString());
                                LastRcvStatus.AppandList(parseRes?.StatusText.ToMyString());
                                LastRcvStatus.AppandList(parseRes?.ErrorText.ToMyString());
                            }
                            SessionMake(parseRes);
                            TryParseBinder();
                        }
                        bool NetStateChanged = false;
                        if (ComState != SocketState.Connected)
                            Session.StatusCode_Ins = "OFFLINE";
                        if (ComState != LastComState)
                        {
                            NetStateChanged = true;
                            dtStart = DateTime.Now;
                            LastComState = ComState;
                        }
                        //if (Session.StatusCode_Ins == "" || Session.StatusCode_Ins == "OFFLINE")
                        {
                            DateTime dtNow = DateTime.Now;
                            double iTimeSpan = (dtNow - dtStart).TotalSeconds;
                            if (iTimeSpan > 5 || NetStateChanged)
                            {
                                dtStart = DateTime.Now;
                                RequestStatus(NetStateChanged);
                                SessionMake();
                                TryParseBinder();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                Thread.Sleep(DriverItem.ComParam.CycleTime);
            }
        }

        /// <summary>
        /// 数据发送队列
        /// </summary>
        /// <param name="state"></param>
        protected override void QueueSendWorker(object state)
        {
            while (ComWorking)
            {
                #region 发送队列解析
                try
                {
                    if (ComState == SocketState.Connected && _DB != null)
                    {
                        ModelComInstr modelQueryCmd = new ModelComInstr()
                        {
                            AwaitState = string.Format(" DriverToken = '{0}' and DataUnitType in('Command') and " +
                                "AwaitState in ('Sending','TaskSending') order by AwaitTime asc", DriverItem.DriverToken)
                                .MarkExpress().MarkWhere()
                        };
                        CallResult _result = _DB.ExcuteQuery<ModelComInstr>(modelQueryCmd);
                        if (_result.Success)
                        {
                            DataTable dt = _result.Result.ToMyDataTable();
                            foreach (DataRow row in dt.Rows)
                            {
                                modelQueryCmd = ColumnDef.ToEntity<ModelComInstr>(row);
                                string strID = modelQueryCmd.ID.ToMyString();
                                string strCommandValue = modelQueryCmd.DataValue;
                                string strCmdComment = modelQueryCmd.Comment.ToMyString();
                                if (string.IsNullOrEmpty(strCmdComment))
                                    strCmdComment = strCommandValue;

                                string Message = string.Format("【Command】【{0}】{1}",
                                       modelQueryCmd.RelatedGroup.ToMyString(), strCmdComment);
                                Logger.CommBody.Write(LOG_TYPE.MESS, Message);

                                //bool SendSuccess = Send(modelQueryCmd.DataValue);
                                CallResult result = ParseCommandInvoke(modelQueryCmd.RelatedVariable,modelQueryCmd.DataValue);
                                if (result.Fail)
                                {
                                    string Error = string.Format("【Command发送错误】【{0}】{1}:{2}",
                                       modelQueryCmd.RelatedGroup.ToMyString(), strCmdComment,
                                       result.Result.ToMyString());
                                    Logger.CommBody.Write(LOG_TYPE.ERROR, Error);
                                }
                                modelQueryCmd = new ModelComInstr()
                                {
                                    ID = strID.MarkWhere(),
                                    AwaitState = "Sended",
                                    AwaitTime = SystemDefault.StringTimeNow
                                };

                                _DB.ExcuteUpdate<ModelComInstr>(modelQueryCmd);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string ErrMsg = string.Format("【QueueWorker_SND】【{0}】【{1}:{2}】\r\n\t{3}",
                     DriverItem.DriverName, ComIP, ComPort, ex.Message);
                    Logger.Error.Write(LOG_TYPE.ERROR, ErrMsg);
                }
                #endregion
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 添加接收日志
        /// </summary>
        /// <param name="content"></param>
        private void AddRcvCommLog(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
            AddCommLog(content, "RCV");
        }

        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="DataType">SND or RCV</param>
        protected override void AddCommLog(string strData, string DataType)
        {
            if (DataType == "RCV" && LastRcvStatus.Contains(strData))
                return;
            if (strData.EndsWith("\r\n"))
            {
                strData = strData.MidString("", "\r\n", EndStringSearchMode.FromTailAndToEndWhenUnMatch);
            }
            string strContent = string.Format("【{0}】 【{1}】  {2}", DataType, DriverItem.DriverName.ToMyString(), strData);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }

        /// <summary>
        /// 解析接收报文
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        protected virtual TelegramParsedResult ParseRecievedContent(string strMessage)
        {
            List<string> LstResField = strMessage.MySplit(SystemDefault.Space);
            string strMessKey = LstResField.MySelectAny(0);
            string strResCmdCode = LstResField.MySelectAny(1);
            TelegramParsedResult parsedRes = new TelegramParsedResult()
            {
                CommandName = strMessKey,
                CommandCode = strResCmdCode
            };
            ComTask com = DicComTask.DictFieldValue("SampleMeasure");
            switch (strMessKey)
            {
                case "STATUS":             //状态回复
                    ConvertMessage("STATUS_COMMAND", strResCmdCode, ref parsedRes);
                    if (strResCmdCode == "00")
                    {
                        //仪器状态
                        ConvertMessage("STATUS_INS", LstResField.MySelectAny(11), ref parsedRes);
                        ConvertMessage("STATUS_INS_ERROR", LstResField.MySelectAny(12), ref parsedRes);
                        Session.StatusCode_Ins = LstResField.MySelectAny(11);
                        Session.StatusCode_Ins_Error = LstResField.MySelectAny(12);
                        Session.StatusText_Ins = parsedRes.StatusText;
                        Session.StatusText_Ins_Error = parsedRes.ErrorText;
                        if (com != null)
                        {
                            if (parsedRes.ErrorOccurs || parsedRes.StatusError)
                            {
                                com.AppandComResult("RequestXRFStatus", "Fail", parsedRes.ErrorText + "\r\n" + parsedRes.StatusText);
                            }
                            else
                            {
                                com.AppandComResult("RequestXRFStatus", "Success", parsedRes.StatusText);
                            }
                        }
                    }
                    else if (strResCmdCode == "01")
                    {
                        //样品状态
                        ConvertMessage("STATUS_SAMPLE", LstResField.MySelectAny(11), ref parsedRes);
                        parsedRes.ErrorText = ConvertMessage("STATUS_SAMPLE_ERROR", LstResField.MySelectAny(12),
                            LstResField.MySelectAny(12), ref parsedRes);
                        Session.StatusCode_Sample = LstResField.MySelectAny(11);
                        Session.StatusText_Sample = parsedRes.StatusText;
                        Session.StatusCode_Sample_Error = LstResField.MySelectAny(12);
                        Session.StatusText_Sample_Error = parsedRes.ErrorText;
                    }
                    else if (strResCmdCode == "02")
                    {
                        //样品列表状态
                        List<ModelSubPos> LstSample = new List<ModelSubPos>();
                        int iRcvFieldCount = LstResField.MyCount();
                        string strSampleListMsg = string.Empty;
                        //获取样品队列状态
                        int iNum = 1;
                        for (int i = 2; i < iRcvFieldCount; i = i + 3)
                        {
                            if (i + 2 > iRcvFieldCount - 1)
                                break;
                            ModelSubPos modSam = new ModelSubPos()
                            {
                                SampleLabel = LstResField.MySelectAny(i),
                                ProcKey = ConvertMessage("STATUS_SAMPLE", LstResField.MySelectAny(i + 1), ref parsedRes),
                                MarkKey = LstResField.MySelectAny(i + 2)
                            };
                            LstSample.Add(modSam);
                            strSampleListMsg += $"\r\nNo.{iNum++}: 样品[{modSam.SampleLabel.GoOriginal()}]\t" +
                                $"位置[{modSam.MarkKey.GoOriginal()}]\t状态[{modSam.ProcKey}]";
                        }
                        if (!string.IsNullOrEmpty(strSampleListMsg))
                        {
                            AddRcvCommLog(strSampleListMsg);
                            LastRcvStatus.AppandList(strSampleListMsg);
                        }
                        Session.TaskSampleNumber = LstSample.MyCount().ToString();
                        Messenger.Default.Send(LstSample, SampleListUpdated);
                    }
                    else if (strResCmdCode == "03")
                    {
                        //空闲工位数量
                        Session.FreeWorkPosNumber = LstResField.MySelectAny(2).ToMyInt().ToMyString();
                        if (com != null)
                        {
                            if (Session.FreeWorkPosNumber.ToMyInt() < 1)
                            {
                                com.AppandComResult("RequestFreePos", "Fail", "仪器内没有空闲工位可使用");
                            }
                            else
                            {
                                com.AppandComResult("RequestFreePos", "Success", $"空闲工位{Session.FreeWorkPosNumber}个");
                            }
                        }
                    }
                    LastRcvStatus.AppandList(strMessage);
                    if (LastRcvStatus.Count > 30)
                        LastRcvStatus.RemoveAt(0);
                    break;

                case "MEASMP":
                    ConvertMessage("MEASMP_COMMAND", strResCmdCode, ref parsedRes);
                    if (com != null)
                    {
                        if (parsedRes.CommandError)
                        {
                            com.AppandComResult("ReserveSample", "Fail", parsedRes.CommandText);
                        }
                        else
                        {
                            com.AppandComResult("ReserveSample", "Success", parsedRes.CommandText);
                        }
                    }
                    break;

                case "LOADSM":
                    ConvertMessage("LOADSM_COMMAND", strResCmdCode, ref parsedRes);
                    break;

                case "UNLOAD":
                    ConvertMessage("UNLOAD_COMMAND", strResCmdCode, ref parsedRes);
                    break;

                case "CANCEL":
                    ConvertMessage("CANCEL_COMMAND", strResCmdCode, ref parsedRes);
                    break;

                case "READRS":
                    ConvertMessage("READRS_COMMAND", strResCmdCode, ref parsedRes);
                    if (parsedRes.CommandCode == "00")
                        ParseResultData(strMessage);
                    break;
            }
            return parsedRes;
        }

        /// <summary>
        /// 转换信息
        /// </summary>
        /// <param name="MesKey"></param>
        /// <param name="Code"></param>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        private string ConvertMessage(string MesKey, string Code, ref TelegramParsedResult parsedResult)
        {
            if (FieldMapDoc == null)
                return string.Empty;
            XmlNodeList nodes = FieldMapDoc.SelectNodes($"FieldMappings/FieldMapping[@Name='{MesKey}']/Field");
            foreach (XmlNode node in nodes)
            {
                string strCode = node.Attributes["Code"]?.Value;
                string strText = node.Attributes["Text"]?.Value;
                string strResCodeType = node.Attributes["Type"]?.Value;
                if (strCode == Code)
                {
                    FillParsedResult(node, ref parsedResult);
                    return strText;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 转换信息
        /// </summary>
        /// <param name="MesKey"></param>
        /// <param name="Code1"></param>
        /// <param name="Code2"></param>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        private string ConvertMessage(string MesKey, string Code1, string Code2, ref TelegramParsedResult parsedResult)
        {
            if (FieldMapDoc == null)
                return string.Empty;
            XmlNodeList nodes = FieldMapDoc.SelectNodes($"FieldMappings/FieldMapping[@Name='{MesKey}']/Field");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["Code1"]?.Value == Code1 && node.Attributes["Code"]?.Value == Code2)
                {
                    FillParsedResult(node, ref parsedResult);
                    return node.Attributes["Text"]?.Value;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 将解析目标XmlNode信息填充到解析结果实体
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parsedResult"></param>
        private void FillParsedResult(XmlNode node, ref TelegramParsedResult parsedResult)
        {
            if (parsedResult == null)
                return;
            string strCode = node.Attributes["Code"]?.Value;
            string strText = node.Attributes["Text"]?.Value;
            string strResCodeType = node.Attributes["Type"]?.Value;
            XmlNode pNode = node.ParentNode;
            string strFieldType = pNode.Attributes["Type"]?.Value;
            if (strFieldType == "COMMAND")
            {
                parsedResult.CommandCode = $"{parsedResult?.CommandName.ToMyString()}:{strCode}";
                parsedResult.CommandText = strText;
                parsedResult.CommandError = IsErrorOrWarning(strResCodeType);
                parsedResult.ErrorLevel = strResCodeType;
            }
            else if (strFieldType == "STATUS")
            {
                parsedResult.StatusCode = strCode;
                parsedResult.StatusText = strText;
                parsedResult.StatusError = IsErrorOrWarning(strResCodeType);
                parsedResult.ErrorLevel = strResCodeType;
            }
            else if (IsErrorOrWarning(strFieldType))
            {
                parsedResult.ErrorCode = strCode;
                parsedResult.ErrorText = strText;
                parsedResult.ErrorOccurs = IsErrorOrWarning(strResCodeType);
                parsedResult.ErrorLevel = strResCodeType;
            }
        }

        /// <summary>
        /// 信息类型
        /// </summary>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        public bool IsErrorOrWarning(string MessageType)
        {
            return MessageType.MyContains("ERROR,WARN", ",");
        }

        /// <summary>
        /// 解析执行指令
        /// </summary>
        /// <param name="CmdExp"></param>
        /// <param name="DataValue"></param>
        /// <returns></returns>
        private CallResult ParseCommandInvoke(string CmdExp, string DataValue)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            try
            {
                if (string.IsNullOrEmpty(CmdExp))
                {
                    ret.Result = "未关联到任务指令";
                    return ret;
                }
                //解析函数名称
                string FuncName = CmdExp.MidString("", "(").Trim();
                //解析函数参数
                string FuncParamField = CmdExp.MidString("(", ")", EndStringSearchMode.FromTailAndToEndWhenUnMatch);
                //参数元
                string[] ArrayDataValue = DataValue.ToMyString().MySplit(",", null, null, false, true).ToArray();
                //参数换元
                FuncParamField = string.Format(FuncParamField, ArrayDataValue);
                //参数字符串按，分割给参
                List<string> FuncParams = FuncParamField.MySplit(",", null, null, false, true);
                return this.SyncInvokeMethod(FuncName, FuncParams.ToArray());
            }
            catch (Exception ex)
            {
                ret.Result = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 会话整理
        /// </summary>
        /// <param name="parsedRes"></param>
        private void SessionMake(TelegramParsedResult parsedRes = null)
        {
            if (parsedRes?.ErrorOccurs == true)
                FaultErrorList.AppandList(parsedRes?.ErrorText.ToMyString());
            else
                FaultErrorList.Clear();
            if (parsedRes?.CommandError == true)
                CommandErrorList.AppandList(parsedRes?.CommandText.ToMyString());
            else
                CommandErrorList.Clear();
            if (parsedRes?.StatusError == true)
                StatusErrorList.AppandList(parsedRes?.StatusText.ToMyString());
            else
                StatusErrorList.Clear();
            Session.Proc_Error = FaultErrorList.ToMyString("|") + CommandErrorList.ToMyString("|") + StatusErrorList.ToMyString("|");
            Session.WorkPosReady = Session.FreeWorkPosNumber.ToMyInt() > 0 ? "1" : "0";

            if (!string.IsNullOrEmpty(parsedRes?.StatusCode)) Session.StatusCode_Ins = parsedRes?.StatusCode;
            if (!string.IsNullOrEmpty(parsedRes?.StatusText)) Session.StatusText_Ins = parsedRes?.StatusText;
            if (!string.IsNullOrEmpty(parsedRes?.ErrorCode)) Session.StatusCode_Ins_Error = parsedRes?.ErrorCode;
            if (!string.IsNullOrEmpty(parsedRes?.ErrorLevel)) Session.StatusText_Ins_Error = parsedRes?.ErrorText;
            if (!string.IsNullOrEmpty(parsedRes?.CommandCode)) Session.CommandText_Error = parsedRes?.CommandCode;
            if (!string.IsNullOrEmpty(parsedRes?.CommandText)) Session.CommandText_Error = parsedRes?.CommandText;

            Session.Status_Offline = (Session.StatusCode_Ins == "OFFLINE" || Session.StatusCode_Ins == "00" ||
                Session.StatusCode_Ins == "") ? "1" : "0";
            if (Session.Status_Offline == "1")
                Session.Status_Online = "0";
            else if (Session.StatusCode_Ins != "OFFLINE" && !string.IsNullOrEmpty(Session.StatusCode_Ins))
                Session.Status_Online = "1";
            Session.State_Alarm = ((!string.IsNullOrEmpty(Session.Proc_Error) || !string.IsNullOrEmpty(Session.Proc_Warn)) &&
                Session.Status_Offline == "0") ? "1" : "0";
            if (Session.Status_Offline == "1" || Session.State_Alarm == "1")
            {
                Session.State_Normal = "0";
                Session.State_Run = "0";
                Session.State_Done = "0";
            }
            else
            {
                //Session.State_Done = Session.InvokeAnaFinish;
                //Session.State_Run = Session.IsSparking == "1" && Session.InvokeAnaFinish == "0" ? "1" : "0";
                Session.State_Normal = Session.StatusCode_Ins.MyContains("01,02",",") && Session.StatusCode_Ins_Error == "00" ? "1" : "0";
            }
            Session.ComShake = ComState == SocketState.Connected ? "1" : "0";
            Session.RaisePropertyNotify();
        }

        /// <summary>
        /// 解析绑定
        /// </summary>
        /// <param name="DataName"></param>
        /// <param name="strDataValue"></param>
        /// <returns></returns>
        protected bool TryParseBinder()
        {
            if (DriverItem.ServerUpdEn != "1") return true;
            List<ModelComInstr> LstModel = ListComSymbol.Where(x => x.DataUnitType != "Command").ToList();
            List<string> LstRelatedExpress = new List<string>();
            List<ModelComInstr> LstModelUpdate = new List<ModelComInstr>();
            foreach (ModelComInstr com in LstModel)
            {
                string strRelatedVariable = com.RelatedVariable;
                string strDataValue = Session.GetPropValue(com.DataName).ToMyString();
                //添加通讯值
                ModelComInstr modelUpdate = new ModelComInstr()
                {
                    ID = com.ID.MarkWhere(),
                    DriverToken = com.DriverToken.MarkWhere(),
                    DataValue = strDataValue.ValueAttachMark()
                };
                LstModelUpdate.Add(modelUpdate);
                //解析关联变量
                if (!string.IsNullOrEmpty(strRelatedVariable))
                {
                    List<string> LstRelatedVariable = strRelatedVariable.MySplit("|");
                    foreach (string itmRelated in LstRelatedVariable)
                    {
                        if (itmRelated.Contains("WriteTableRow("))
                        {
                            if (itmRelated.MatchParamCount() > 0)
                            {
                                string strCalcRelted = string.Format(itmRelated, strDataValue);
                                LstRelatedExpress.Add(strCalcRelted);
                            }
                            else if ((strDataValue.ToMyInt() == 1 || strDataValue.ToUpper() == "TRUE") &&
                                com.AddrType.ToMyString().ToUpper() == "BOOL")
                            {
                                LstRelatedExpress.Add(itmRelated);
                            }
                        }
                        else
                        {
                            if (!itmRelated.Contains("="))
                            {
                                string strCalcRelted = string.Format(itmRelated + "={0}", strDataValue);
                                LstRelatedExpress.Add(strCalcRelted);
                            }
                            else if ((strDataValue.ToMyInt() == 1 || strDataValue.ToUpper() == "TRUE") &&
                             com.AddrType.ToMyString().ToUpper() == "BOOL")
                            {
                                LstRelatedExpress.Add(itmRelated);
                            }
                        }
                    }
                }
            }
            if (LstModelUpdate.Count > 0 && _DB != null)
            {
                if (LstRelatedExpress.Count > 0)
                    _DB.ExcuteMacroCommand(LstRelatedExpress);
                CallResult _result = _DB.ExcuteUpdate<ModelComInstr>(LstModelUpdate);
            }
            return true;
        }
    }

    /// <summary>
    /// 布鲁克分析结果解析
    /// </summary>
    public partial class sComAXSXRF<TConNode>
    {
        /// <summary>
        /// 1、解析仪器通讯发送的结果数据
        /// </summary>
        /// <param name="strResultContent"></param>
        /// <returns></returns>
        public virtual CallResult ParseResultData(string strResultContent)
        {
            #region 数据模板
            //READRS 00 601-0918-TEST02 PHL-603 "18-Sep-2023 15:31:48" SiO2 1.513 % Al2O3 0.000 % Fe2O3 0.388 % 
            //CaO 0.000 % MgO 0.000 % K2O 0.321 % Na2O 0.214 % TiO2 0.119 % SO3 0.270 % C 0.399 % H2O 100.109 %
            #endregion

            CallResult res = new CallResult() { Success = false };
            List<string> LstResultFields = strResultContent.MySplit(SystemDefault.Space);
            //验证分析数据报文标记
            int FieldsCount = LstResultFields.MyCount();
            if (!LstResultFields.Contains("READRS") || FieldsCount < 7)
            {
                res.Result = "非数据报文无法继续解析！";
                Messenger.Default.Send(res.Result, AnaDataParseError);
                return res;
            }
            //解析报文
            ModelXRFData ResultData = new ModelXRFData()
            {
                SampleName = LstResultFields[2],
                AnaProgram = LstResultFields[3],
                AnaEndTime = LstResultFields[4].ToMyDateTimeStr(SystemDefault.DateTimeFormat)
            };
            Dictionary<string, string> DicElemItem = new Dictionary<string, string>();
            Dictionary<string, string> DicElemVal = new Dictionary<string, string>();
            //将不明确的字段以【FIELD_x】格式打包
            for (int i = 0; i < 5; i++)
            {
                string strElemVal = LstResultFields[i + 1];
                ResultData.DicSrcField.AppandDict($"FIELD_{i.ToString()}", strElemVal);
            }
            for (int i = 5; i < FieldsCount; i = i + 3)
            {
                if (i + 1 > FieldsCount - 1)
                    break;
                string strElemName = $"ELEM_{LstResultFields[i].ToUpper()}";
                string strElemVal = LstResultFields[i + 1];
                if (!strElemVal.IsNumeric())
                    continue;
                ResultData.DicSrcField.AppandDict(strElemName, strElemVal);
            }
            res.Result = ResultData;
            res.Success = true;
            Messenger.Default.Send(ResultData, AnaDataReceived);
            return res;
        }

        /// <summary>
        /// 2、解析布鲁克仪器分析结果文件
        /// 布鲁克仪器 C:\SPECplus\Temp_C.dat文件
        /// </summary>
        /// <param name="fileName">dat文本文件</param>
        /// <returns></returns>
        public static CallResult ParseReultDataFile(string fileName)
        {
            #region 文件格式 - 数据行尾部没有【'】结束
            //PHL - E8  

            //603 - 0919 - 4
            //PRN
            //00
            //00
            //SiO2                                 0.53771     186.340   30.0           53.771 %
            //Al2O3                                0.17993      92.177   10.0           17.993 %
            //Fe2O3                                0.00422      70.329    6.0            0.422 %
            //CaO                                  0.05670      41.445   20.0             5.67 %
            //MgO                                  0.10447     107.891   30.0           10.447 %
            //K2O                                  0.00354      12.380    6.0            0.354 %
            //Na2O                                 0.00101       0.707   30.0            0.101 %
            //TiO2                                 0.00434      13.937    6.0            0.434 %
            //SO3                                  0.00459       8.112   10.0            0.459 %
            //C                                    0.00832       0.386   30.0            0.832 %
            //H2O                                  0.09515 - 900.000    0.0            9.515 %

            //END
            //19 - Sep - 2023 09:57:48
            //45188.41514
            //TOTAL TIME: 271; MODE: UN
            //OPERATOR: Admin
            #endregion

            #region 文件格式 - 数据行尾部有【'】结束
            //-----------分析异常数据文件
            //QE - Check - Vac34

            //q1
            //PRN
            //00
            //00
            //SiO2                                 0.46018     559.763  0.050             46.0 % '
            //Na2O                                 0.20793     167.588  0.050             20.8 % '
            //CaO                                  0.11514     292.073  0.050             11.5 % '
            //MgO                                  0.07357     112.803  0.050             7.36 % '
            //K2O                                  0.05968     186.566  0.050             5.97 % '
            //Al2O3                                0.01100      11.663  0.050             1.10 % '
            //BaO                                  0.01027      10.663  0.050             1.03 % '
            //PbO                                  0.00936     351.827  0.050            0.936 % '
            //SrO                                  0.00935     242.899  0.050            0.935 % '
            //Sb2O3                                0.00837     265.046  0.050            0.837 % '
            //SO3                                  0.00590      12.536  0.050            0.590 % '
            //CdO                                  0.00522      45.246  0.050            0.522 % '
            //ZnO                                  0.00510     236.751  0.050            0.510 % '
            //MnO                                  0.00483      58.105  0.050            0.483 % '
            //P2O5                                 0.00420       5.152  0.050            0.420 % '
            //Cl                                   0.00253       8.108  0.050            0.253 % '

            //END
            //18 - Oct - 2019 12:56:52
            //43756.53949
            //TOTAL TIME: 474; MODE: UN
            //OPERATOR: Admin
            #endregion

            //设置默认文件
            if (string.IsNullOrEmpty(fileName))
                fileName = @"C:\SPECplus\Temp_C.dat";
            CallResult res = new CallResult() { Success = false };
            List<string> FileLines = sCommon.ReadFile(fileName);
            //第一行：分析程序  第3行：样品名称  
            int iLineCount = FileLines.MyCount();
            if (iLineCount < 4)
            {
                res.Result = "分析数据文件无效！";
                return res;
            }
            try
            {
                ModelXRFData ResultData = new ModelXRFData()
                {
                    SampleName = FileLines[2].Replace("\"", ""),
                    AnaProgram = FileLines[0].Replace("\"", ""),
                };
                string strAnaTime = FileLines[iLineCount - 4];
                if (strAnaTime.IsTimeMate())
                    ResultData.AnaEndTime = strAnaTime.ToMyDateTimeStr();
                List<string> LstRemove = new List<string>() { "\"" };
                foreach (string line in FileLines)
                {
                    if (line.Contains("%") || line.Contains("'"))
                    {
                        List<string> LstElemPart = line.MySplit(" ", null, LstRemove);
                        if (LstElemPart.MyCount() >= 5)
                        {
                            string strElemName = $"ELEM_{LstElemPart[0].ToUpper()}";
                            string strElemValue = LstElemPart[4];
                            ResultData.DicSrcField.AppandDict(strElemName, strElemValue);
                        }
                    }
                    else
                    {
                        if (line.StartsWith("OPERATOR"))
                            ResultData.DicSrcField.AppandDict("OPERATOR", line.MidString(":", "").Trim());
                        else if (line.StartsWith("TOTAL TIME"))
                        {
                            ResultData.TotalAnaTime = line.MidString("TOTAL TIME:", ";").Trim();
                            ResultData.AnaMode = line.MidString("MODE:", ";").Trim();
                        }
                    }
                }
                res.Result = ResultData;
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.Result = $"荧光分析结果文件解析出错！\r\n{ex.Message}";
            }
            return res;
        }
    }
}
