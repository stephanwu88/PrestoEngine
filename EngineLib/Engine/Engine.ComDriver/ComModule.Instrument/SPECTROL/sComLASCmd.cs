using Engine.ComDriver.Siemens;
using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;
using Engine.Mod;
using Engine.MVVM.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Engine.ComDriver.SPECTROL
{
    /// <summary>
    /// 会话区
    /// </summary>
    public class SessionMemory : NotifyObject
    {
        public string State_Normal { get; set; } = "0";
        public string State_Run { get; set; } = "0";
        public string State_Alarm { get; set; } = "0";
        public string State_Wait { get; set; } = "0";
        public string State_Done { get; set; } = "0";
        public string State_Offline { get; set; } = "0";
        public string State_Online { get; set; } = "0";
        public string ComShake { get; set; } = "0";
        public string IsSparking { get; set; } = "0";
        //注册通过
        public string SampleAllowed { get; set; } = "0";
        //移点信号
        public string InvokePotMove { get; set; } = "0";
        //分析结束信号
        public string InvokeAnaFinish { get; set; } = "0";
        //光室温度
        public string Data_CurrentTemp { get; set; }
        public string Proc_Error { get; set; }
        public string Proc_Warn { get; set; }
        public string Proc_MeasureStep { get; set; }
    }

    /// <summary>
    /// Spectrol LAS01 Protocol
    /// </summary>
    public class sComLASCmd : sComLASCmd<ServerNode>
    {
        public sComLASCmd(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {

        }
    }

    /// <summary>
    /// 接口方法
    /// </summary>
    /// <typeparam name="TConNode"></typeparam>
    public partial class sComLASCmd<TConNode> 
        where TConNode : new()
    {
        public readonly AsyncLock asyncLock = new AsyncLock();
        private ThreadSignalGate _SignalGate = new ThreadSignalGate();
        public readonly string AnaReceivedDataParsed = SystemDefault.UUID;
        public readonly string SessionValueChanged = SystemDefault.UUID;
        private sComS7PLC ComSparkHeader;

        /// <summary>
        /// 生产样注册 - 牌号控制分析
        /// </summary>
        /// <param name="SampleID">样品编码</param>
        /// <param name="Matrix">基体</param>
        /// <param name="AnaProgram">分析方法</param>
        public async Task<bool> AsyncReservePS(string SampleID, string Matrix, string AnaProgram)
        {
            return await Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    string ErrorMessage = string.Empty;

                    MessageStruct mes = new MessageStruct("", new List<object>() { CurrentData, RoundingCurrentData, RoundingAvgData });
                    Messenger.Default.Send(mes, AnaReceivedDataParsed);

                    //Step1. SelectMethod
                    string CommandName = "SelectMethod";
                    WriteCommandParam(CommandName, "ExcitationType", "Spark");
                    WriteCommandParam(CommandName, "BaseElement", Matrix);
                    WriteCommandParam(CommandName, "MethodName", AnaProgram);
                    WriteCommandParam(CommandName, "AutoAcceptValid", "False");
                    WriteCommandParam(CommandName, "AutoRejectInvalid", "False");
                    bool SendSucess = Request(CommandName, out string Telegram);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }
                    SignalData res = _SignalGate.WaitSignal(CommandName, 10000);
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }

                    //Step2. PrepareAnalysis
                    CommandName = "PrepareAnalysis@PS";
                    WriteCommandParam(CommandName, "AnalyticalMode", "UnknownMeas");
                    WriteCommandParam(CommandName, "炉机号", SampleID);
                    WriteCommandParam(CommandName, "炉次", SampleID.MidString(0, 1));
                    WriteCommandParam(CommandName, "操作员", "Heao");
                    WriteCommandParam(CommandName, "序号", "1");
                    //WriteCommandParam(CommandName, "StandardName", string.IsNullOrEmpty(ToleranceMethod) ? "" : "TypeStd");

                    SendSucess = Request(CommandName, out Telegram);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }
                    CommandName = CommandName.MidString("", "@");
                    res = _SignalGate.WaitSignal(CommandName, 2000);
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }

                    ////Step3. SelectTypeStandard
                    //if (!string.IsNullOrEmpty(ToleranceMethod))
                    //{
                    //    CommandName = "SelectTypeStandard";
                    //    WriteCommandParam(CommandName, "TypeStandardName", ToleranceMethod);
                    //    SendSucess = Request(CommandName, out Telegram);
                    //}

                    //清空数据集
                    AnaDataSourceDic.Clear();
                    AnaStatisticDic.Clear();
                    SampleAnaDataState = string.Empty;
                    Session.InvokePotMove = "0";
                    Session.InvokeAnaFinish = "0";

                    return true;
                }
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// 启动激发
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AsyncStartExciter()
        {
            return await Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    string ErrorMessage = string.Empty;

                    Session.InvokePotMove = "0";
                    Session.InvokeAnaFinish = "0";

                    //Step1. HeaderDn
                    string CommandName = "HeaderDn";
                    if (ComSparkHeader == null)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]通讯未加载";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }
                    bool SendSucess = ComSparkHeader.WriteByDataName("CmdHeaderDn", 1);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        Logger.Error.Write( LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }
                    SignalData res = _SignalGate.WaitSignal("SS_SparkDn", 10000);
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }

                    //Step2. StartExciter
                    CommandName = "Measure";
                    for (int i = 0; i < 8; i++)
                    {
                        SendSucess = Request(CommandName, out string Telegram1);
                        if (!SendSucess)
                        {
                            ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                            Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                            return false;
                        }
                        Session.IsSparking = "1";
                        Messenger.Default.Send("IsSparking=1", SessionValueChanged);
                        TryParseBinder();
                        res = _SignalGate.WaitSignal("MeasurementCompleted", 50000);
                        if (res.Error)
                        {
                            ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                            Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                            if (i == 7)
                                return false;
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            break;
                        }
                    }

                    //Step3. 激发头上升
                    if (ComSparkHeader != null)
                    {
                        ComSparkHeader.WriteByDataName("CmdHeaderUp", 1);
                    }
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }

                    //Step4. GetAnalysisResults
                    CommandName = "GetAnalysisResults";
                    WriteCommandParam(CommandName, "SampleResultsType", "Complete");
                    WriteCommandParam(CommandName, "SampleResultsValuesType", "Elements");
                    SendSucess = Request(CommandName, out string Telegram);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }
                    res = _SignalGate.WaitSignal(CommandName, 4000);
                    Session.IsSparking = "0";
                    Messenger.Default.Send("IsSparking=0", SessionValueChanged);
                    TryParseBinder();
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        Logger.Error.Write(LOG_TYPE.ERROR, ErrorMessage);
                        return false;
                    }
                    else
                    {
                        if (res.Message == "MovePot")
                        {
                            Session.InvokePotMove = "1";
                            Session.InvokeAnaFinish = "0";
                        }
                        else if (res.Message == "AnaFinish")
                        {
                            Session.InvokePotMove = "0";
                            Session.InvokeAnaFinish = "1";
                        }
                        MessageStruct mes = new MessageStruct(res.Message, new List<object>() { CurrentData, RoundingCurrentData, RoundingAvgData });
                        Messenger.Default.Send(mes, AnaReceivedDataParsed);
                    }
                    TryParseBinder();
                    return true;
                }
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// 启动激发 - 测试
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AsyncStartExciterTest()
        {
            return await Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    string ErrorMessage = string.Empty;

                    Session.InvokePotMove = "0";
                    Session.InvokeAnaFinish = "0";

                    //Step4. GetAnalysisResults
                    string CommandName = "GetAnalysisResults";
                    WriteCommandParam(CommandName, "SampleResultsType", "Complete");
                    WriteCommandParam(CommandName, "SampleResultsValuesType", "Elements");
                    bool SendSucess = Request(CommandName, out string Telegram);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        return false;
                    }
                    SignalData res = _SignalGate.WaitSignal(CommandName, 1000000);
                    Session.IsSparking = "0";
                    Messenger.Default.Send("IsSparking=0", SessionValueChanged);
                    TryParseBinder();
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        return false;
                    }
                    else
                    {
                        if (res.Message == "MovePot")
                        {
                            Session.InvokePotMove = "1";
                            Session.InvokeAnaFinish = "0";
                        }
                        else if (res.Message == "AnaFinish")
                        {
                            Session.InvokePotMove = "0";
                            Session.InvokeAnaFinish = "1";
                        }
                        MessageStruct mes = new MessageStruct(res.Message, new List<object>() { CurrentData, RoundingCurrentData, RoundingAvgData });
                        Messenger.Default.Send(mes, AnaReceivedDataParsed);
                    }
                    TryParseBinder();
                    return true;
                }
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// 刷电极
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AsyncCleanPole()
        {
            return await Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    string ErrorMessage = string.Empty;
                    string CommandName = "Clean";
                    if (ComSparkHeader == null)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]通讯未加载";
                        return false;
                    }
                    bool SendSucess = ComSparkHeader.WriteByDataName("CmdStartBrush", 1);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        return false;
                    }
                    SignalData res = _SignalGate.WaitSignal("SS_CleanRun", 5000);
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        return false;
                    }
                    else
                    {
                        res = _SignalGate.WaitSignal("SS_SparkUp", 20000);
                    }
                    return true;
                }
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// 复位流程
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AsyncResetProc()
        {
            return await Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    string ErrorMessage = string.Empty;
                    //清除数据集
                    AnaDataSourceDic.Clear();
                    AnaStatisticDic.Clear();
                    SampleAnaDataState = string.Empty;
                    Session.InvokeAnaFinish = "0";
                    Session.InvokePotMove = "0";
                    
                    //Step1. HeaderUp
                    string CommandName = "HeaderUp";
                    if (ComSparkHeader == null)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]通讯未加载";
                    }
                    bool SendSucess = ComSparkHeader.WriteByDataName("CmdHeaderUp", 1);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        return false;
                    }
                    SignalData res = _SignalGate.WaitSignal("SS_SparkUp", 2000);
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        return false;
                    }

                    //Step2. DiscardAnalysis
                    CommandName = "DiscardAnalysis";
                    SendSucess = Request(CommandName, out string Telegram);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        return false;
                    }
                    return true;
                }
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// 放样准备
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AsyncReservePrepare()
        {
            return await Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    string ErrorMessage = string.Empty;
                    
                    //Step1. HeaderUp
                    string CommandName = "HeaderUp";
                    if (ComSparkHeader == null)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]通讯未加载";
                    }
                    bool SendSucess = ComSparkHeader.WriteByDataName("CmdHeaderUp", 1);
                    if (!SendSucess)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]发送失败";
                        return false;
                    }
                    SignalData res = _SignalGate.WaitSignal("SS_SparkUp", 2000);
                    if (res.Error)
                    {
                        ErrorMessage = $"[{DriverItem?.DriverName}][{CommandName}]时遇到错误:{res.Message}";
                        return false;
                    }
                    return true;
                }
            }).ConfigureAwait(true); 
        }
    }

    /// <summary>
    /// Spectrol LAS01 Protocol - ActionCommmand
    /// 斯派克LAS01系列光谱仪 - 控制指令协议
    /// </summary>
    public partial class sComLASCmd<TConNode> : sComRuleAsc
        where TConNode : new()
    {
        public static readonly string AnaDataReceived = SystemDefault.UUID;
        public static readonly string AnaDataParseError = SystemDefault.UUID;
        public static readonly string SampleListUpdated = SystemDefault.UUID;
        private IDBFactory<TConNode> _DB = DbFactory.Current.GetConn<TConNode>("nd_work");
        public JDictionary ReqField { get; private set; } = new JDictionary();
        //通讯会话区
        public SessionMemory Session { get; } = new SessionMemory();
        /// <summary>
        /// 协议变量列表
        /// </summary>
        List<ModelComInstr> ListComSymbol;
        List<string> LastRcvStatus = new List<string>();
        public static XmlDocument FieldMapDoc;
        public XmlDocument CommandReqDoc;
        public XmlDocument EventDoc;
        //@字段@格式检索
        public readonly string RegexFieldText = "@([^<>]*?)@";
        List<string> FaultErrorList = new List<string>();
        List<string> StatusErrorList = new List<string>();
        List<string> CommandErrorList = new List<string>();
        protected Dictionary<string, ModelSheetColumn> DicResultConfig
            = new Dictionary<string, ModelSheetColumn>();

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComLASCmd(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            STX = "<?xml";
            EDX = "</AutomationCommand>";
            QueueWithEDX = true;
            SendWithSTX = false;
            SendWithEDX = false;
            TextEncoding = UnicodeEncoding.UTF8;
            Max_Buffer_Size = 1 * 1024 * 1024;  //1M
            BulidMessageList();
            if (_DB != null)
                LoadProtocolTable();
            if (ComMain._ComNetPLC.ContainsKey("SparkHeader"))
            {
                ComSparkHeader = ComMain._ComNetPLC["SparkHeader"] as sComS7PLC;
            }
            else
            {
                ComMain.ComDriverAdded += (s, e) =>
                  {
                      if (e?.Name == "SparkHeader")
                      {
                          ComSparkHeader = ComMain._ComNetPLC["SparkHeader"] as sComS7PLC;
                      }
                  };
            }
            StartSparkHeaderReceive();
            StartParseEvent();
        }

        /// <summary>
        /// 创建信息列表
        /// </summary>
        private void BulidMessageList()
        {
            FieldMapDoc = sCommon.LoadResourceXml("Engine.ComDriver/ComModule.Instrument/SPECTROL/ComField.xml", out bool Success);
            CommandReqDoc = sCommon.LoadResourceXml("Engine.ComDriver/ComModule.Instrument/SPECTROL/AutomationCommand.xml", out Success);
            EventDoc = sCommon.LoadResourceXml("Engine.ComDriver/ComModule.Instrument/SPECTROL/AutomationEvent.xml", out Success);
            //string strXml = CommandReqDoc?.OuterXml.ToMyString();
            ReqField = InitCommandFieldDict();
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
                DriverName = DriverItem.DriverName.ToMyString().MarkWhere(),
                ComLink = "TCP/IP".MarkWhere(),
                LinkState = State == SocketState.Connected ? "1" : "0"
            };
            _DB.ExcuteUpdate<ModelComLink>(comLink);
        }

        /// <summary>
        /// 协议指令发送
        /// </summary>
        /// <param name="eReq"></param>
        /// <param name="Telegram"></param>
        /// <param name="LogSend"></param>
        /// <returns></returns>
        public bool Request(Enum_REQ eReq, out string Telegram, bool LogSend = true)
        {
            string CommandName = eReq.FetchDescription();
            return Request(CommandName, out Telegram, LogSend);
        }

        /// <summary>
        /// 协议指令发送
        /// </summary>
        /// <param name="eReq"></param>
        /// <param name="strTelegram"></param>
        /// <param name="LogSend"></param>
        /// <returns></returns>
        public bool Request(string CommandName, out string strTelegram, bool LogSend = true)
        {
            string strCommandXml = GetCommandXml(CommandName);
            List<string> lstField = strCommandXml.RegexMatchedList(RegexFieldText, true);
            foreach (string item in lstField)
            {
                strCommandXml = strCommandXml.MyReplace(item, ReqField[CommandName].GetString(item));
            }
            //AutomationCommand command = strCommandXml.XmlDeserialize<AutomationCommand>();
            //strTelegram = command.XmlSerialize();
            strTelegram = strCommandXml;
            return Send(strTelegram, LogSend);
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
                            Session.State_Online = "1";
                            string strMessage = _QueueRcv.Dequeue();
                            if (!strMessage.Contains("InstrumentStateChanged") && !strMessage.Contains("GetInstrumentState") &&
                                !strMessage.Contains("NotifyInstrumentState"))
                                Messenger.Default.Send<string>(strMessage, $"{DriverItem?.DriverPtl.ToMyString()}:{ComIP?.ToMyString()}");
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
                            Session.State_Online = "0";
                        if (ComState != LastComState)
                        {
                            NetStateChanged = true;
                            dtStart = DateTime.Now;
                            LastComState = ComState;
                        }
                        if (Session.State_Online == "0")
                        {
                            DateTime dtNow = DateTime.Now;
                            double iTimeSpan = (dtNow - dtStart).TotalSeconds;
                            if (iTimeSpan > 5 || NetStateChanged)
                            {
                                dtStart = DateTime.Now;
                                TryParseBinder();
                                Request("GetInstrumentState", out string Telegram, NetStateChanged);
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

                                //解析函数名称
                                string FuncName = modelQueryCmd.RelatedVariable.MidString("", "(").Trim();
                                CallResult result = this.ParseMethodInvoke(modelQueryCmd.RelatedVariable, modelQueryCmd.DataValue);
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
            if (string.IsNullOrEmpty(content) || content.Contains("GetInstrumentState"))
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
            if (strData.MyContains("GetInstrumentState|NotifyInstrumentState","|"))
                return;
            if (strData.EndsWith("\r\n"))
            {
                strData = strData.MidString("", "\r\n", EndStringSearchMode.FromTailAndToEndWhenUnMatch);
            }
            string strContent = string.Format("【{0}】 【{1}】  \r\n{2}", DataType, DriverItem.DriverName.ToMyString(), strData);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }

        /// <summary>
        /// 解析接收报文
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        protected virtual TelegramParsedResult ParseRecievedContent(string strMessage)
        {
            TelegramParsedResult parsedRes = new TelegramParsedResult();
            try
            {
                AutomationCommand _command = sCommon.XmlDeserialize<AutomationCommand>(strMessage);
                parsedRes = ParseRecievedContent(_command);
            }
            catch (Exception ex)
            {
                Logger.Error.Write(LOG_TYPE.ERROR, $"控制器[{DriverItem?.DriverName}]接收电文解析错误," +
                    $"{ex.Message},\r\n{strMessage}");
            }
            return parsedRes;
        }

        /// <summary>
        /// 解析接收指令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual TelegramParsedResult ParseRecievedContent(AutomationCommand _command)
        {
            TelegramParsedResult parsedRes = new TelegramParsedResult()
            {

            };
            SignalData signal = new SignalData();
            bool SignalSendAllowed = true;
            try
            {
                parsedRes.CommandName = _command?.Name;
                signal.Token = parsedRes.CommandName;
                switch (_command?.Name)
                {
                    case "GetAnalysisResults":                  //获取分析数据
                        string ParseResult = string.Empty;
                        bool ParseSuccess = ParseResultDataPoint(_command, ref _AnaDataSourceDic, out _AnaStatisticDic,
                            out List<string[]> CurrentPotElem,
                            out List<string[]> RoundingSinglePotElem,
                            out List<string[]> RoundingAvgPotElem, 
                            out ParseResult);
                        signal.Message = ParseResult;
                        break;

                    case "GetMethodNames":                      //获取分析方法列表

                        break;
                    case "GetControlStandardNames":             //GetControlStandardNames
                        break;
                    case "GetICalStandardNames":                //获取标样列表
                        break;
                    case "GetRemoteControlState":               //查询远程控制权
                        break;
                    case "LogIn":                               //远程登录
                        break;
                    case "SelectMethod":                        //选定分析方法
                        break;
                    case "PrepareAnalysis":                     //准备样品
                        break;
                    case "Measure":                             //启动测量分析
                        var MeasureCompleteState = _command?.Return?.MeasurementCompletionDescriptor?.MeasurementCompletionState;
                        if(MeasureCompleteState == "StartError_ClampUp")
                        {
                            signal.Token = "MeasurementCompleted";
                            signal.Error = true;
                            signal.Message = MeasureCompleteState;
                        }
                        break;
                    case "IsTypeStandardValid":                 //检查类标有效性
                        break;
                    case "SelectTypeStandard":                  //带类标
                        break;
                    case "FinishAnalysis":                      //结束测量分析
                        break;

                    #region 附加协议指令
                    case "NotifyReset":                         //SparkWin-IPS 通知复位样品
                        Session.IsSparking = "0";
                        Session.InvokePotMove = "0";
                        Session.InvokeAnaFinish = "0";
                        break;

                    case "NotifySparking":                      //SparkWin-IPS 通知开始激发
                        Session.IsSparking = "1";
                        Session.InvokePotMove = "0";
                        Session.InvokeAnaFinish = "0";
                        break;

                    case "NotifySparked":                       //SparkWin-IPS 通知激发完毕
                        Session.IsSparking = "0";
                        break;

                    case "NotifyMovePot":                       //SparkWin-IPS 通知移点
                        Session.InvokePotMove = "1";
                        Session.InvokeAnaFinish = "0";
                        break;

                    case "NotifySparkFinish":                   //SparkWin-IPS 通知分析结束
                        Session.InvokePotMove = "0";
                        Session.InvokeAnaFinish = "1";
                        break;

                    case "NotifyInstrumentState":               //SparkWin->IPS 通知仪器状态
                        AutomationCommandParaInstrumentStateDescriptor InstrStateContent = _command?.Para?.InstrumentStateDescriptor;
                        WriteCommandParam(_command?.Name, InstrStateContent);
                        Session.State_Online = InstrStateContent?.InstrumentReady == "1" && InstrStateContent?.SparkHeaderReady == "1" ? "1" : "0";
                        break;
                    #endregion

                }
            }
            catch (Exception ex)
            {
                signal.Error = true;
                signal.Message = ex.Message;
            }
            if (SignalSendAllowed)
            {
                _SignalGate.SendSignal(signal);
            }
            return parsedRes;
        }

        /// <summary>
        /// 会话整理
        /// </summary>
        /// <param name="parsedRes"></param>
        private void SessionMake(TelegramParsedResult parsedRes = null)
        {
            Session.State_Offline = Session.State_Online == "0" ? "1" : "0";
            Session.State_Alarm = ((!string.IsNullOrEmpty(Session.Proc_Error) || !string.IsNullOrEmpty(Session.Proc_Warn)) &&
                Session.State_Offline == "0") ? "1" : "0";
            if (Session.State_Offline == "1" || Session.State_Alarm == "1")
            {
                Session.State_Normal = "0";
                Session.State_Run = "0";
                Session.State_Done = "0";
            }
            else
            {
                Session.State_Done = Session.InvokeAnaFinish == "1" && Session.IsSparking == "0" ? "1" : "0";
                Session.State_Run = Session.IsSparking == "1" && Session.InvokeAnaFinish == "0" ? "1" : "0";
                Session.State_Normal = Session.State_Done == "0" && Session.State_Run == "0" ? "1" : "0";
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
            SessionMake();
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
                if(_result.Fail)
                    _result = _DB.ExcuteUpdate<ModelComInstr>(LstModelUpdate);
                if (_result.Success && Session.InvokePotMove == "1")
                    Session.InvokePotMove = "0";
                if (_result.Success && Session.InvokeAnaFinish == "1")
                    Session.InvokeAnaFinish = "0";

            }
            return true;
        }
    }

    /// <summary>
    /// Spectrol LAS01 Protocol - AutomationEvent
    /// 斯派克LAS01系列光谱仪 - 自动化事件协议
    /// </summary>
    /// <typeparam name="TConNode"></typeparam>
    public partial class sComLASCmd<TConNode> 
    {
        /// <summary>
        /// 启动事件报文解析
        /// </summary>
        public void StartParseEvent()
        {
            //获取目标机的Spectrol LAS AutomationEvent协议电文
            Messenger.Default.Register<string>(this, $"Spectrol LAS AutomationEvent:{ComIP.ToMyString()}", (message) =>
            {
                ParseRecievedAutomationEvent(message);
            });
        }
        protected virtual TelegramParsedResult ParseRecievedAutomationEvent(string strMessage)
        {
            TelegramParsedResult parsedRes = new TelegramParsedResult()
            {

            };
            try
            {
                AutomationEvent _event = sCommon.XmlDeserialize<AutomationEvent>(strMessage);
                parsedRes.CommandName = _event?.Name;
                SignalData signal = new SignalData() { Token = parsedRes.CommandName };
                switch (_event?.Name)
                {
                    case "AnalysisFinished":                    //分析结束
                        break;
                    case "AnalysisIDChanged":                   //注册信息变更
                        break;
                    case "AnalysisStarted":                     //分析开始
                        break;
                    case "AnalyticalModeChanged":               //分析模式变化
                        break;
                    case "BaseDataChanged":                     //基础数据变化
                        break;
                    case "InstrumentStateChanged":              //仪器状态变化
                        AutomationEventParaInstrumentStateDescriptor InstrStateContent = _event?.Para?.InstrumentStateDescriptor;
                        WriteCommandParam(_event?.Name, InstrStateContent);
                        break;
                    case "LogInChanged":                        //登录状态变化
                        break;
                    case "MaintenanceReminder":                 //MaintenanceReminder
                        break;
                    case "MaintenanceReminderResetted":         //MaintenanceReminderResetted
                        break;
                    case "MeasurementBroken":                   //测量中断
                        break;
                    case "MeasurementCompleted":                //测量完成
                        break;
                    case "MeasurementFinished":                 //测量结束
                        break;
                    case "MeasurementReplicateChanged":         //MeasurementReplicateChanged
                        break;
                    case "MeasurementStarted":                  //测量已开始
                        break;

                    case "MethodLoaded":                        //方法加载
                        break;

                    case "OperatorInteractionFinished":         //OperatorInteractionFinished
                        break;

                    case "OperatorInteractionNeeded":           //OperatorInteractionNeeded
                        break;

                    case "OpticsDriftCorrectionDataCalculated": //OpticsDriftCorrectionDataCalculated
                        break;

                    case "ProcessError":                        //流程错误
                        break;
                    case "ProcessInformation":                  //流程信息
                        break;
                    case "ProfileLoaded":                       //ProfileLoaded
                        break;
                    case "RemoteControlChanged":                //远程控制状态变化
                        AutomationEventParaRemoteControlDescriptor RemoteControl = _event?.Para?.RemoteControlDescriptor;
                        WriteCommandParam(_event?.Name, RemoteControl);
                        break;
                }
                _SignalGate.SendSignal(signal);
            }
            catch (Exception ex)
            {

            }
            return parsedRes;
        }
    }

    /// <summary>
    /// 激发头状态收集
    /// </summary>
    /// <typeparam name="TConNode"></typeparam>
    public partial class sComLASCmd<TConNode>
    {
        public void StartSparkHeaderReceive()
        {
            Messenger.Default.SingleRegister<ObservableCollection<ModelComPLC>>(this, "SparkHeader", (CommList) =>
            {
                try
                {
                    JDictionary Dic = CommList.ToMyJDictionary(x => x.DataName, x => x.DataValue);
                    if (Dic["SS_SparkUp"].IsTrue() && Dic["State_Free"].IsTrue())
                    {
                        _SignalGate.SendSignal(new SignalData() { Token = "SS_SparkUp" });
                    }
                    if (Dic["SS_SparkDn"].IsTrue() && Dic["State_Free"].IsTrue() && Dic["SS_Sample"].IsTrue())
                    {
                        _SignalGate.SendSignal(new SignalData() { Token = "SS_SparkDn" });
                    }
                    if (Dic["State_Run"].IsTrue())
                    {
                        _SignalGate.SendSignal(new SignalData() { Token = "SS_CleanRun" });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error.Write(LOG_TYPE.ERROR, ex.Message);
                }
            });
        }
    }

    /// <summary>
    /// 内部方法
    /// </summary>
    /// <typeparam name="TConNode"></typeparam>
    public partial class sComLASCmd<TConNode>
    {
        /// <summary>
        /// 获取指令XML模版
        /// </summary>
        /// <param name="CommandName"></param>
        /// <returns></returns>
        public string GetCommandXml(string CommandName)
        {
            XmlNode nd = CommandReqDoc.SelectSingleNode($"/AutomationCommandList/AutomationCommand[@Name='{CommandName}']");
            XmlDocument doc = nd.ImportToNewDoc();
            nd = doc.SelectSingleNode("//AutomationCommand");
            nd.Attributes["Name"].Value = nd.Attributes["Name"].Value.MidString("", "@");
            return doc.ToMyString();
        }

        /// <summary>
        /// 获取事件XML模板
        /// </summary>
        /// <param name="EventName"></param>
        /// <returns></returns>
        public string GetEventXml(string EventName)
        {
            XmlNode nd = EventDoc.SelectSingleNode($"/AutomationEventList/AutomationEvent[@Name='{EventName}']");
            XmlDocument doc = nd.ImportToNewDoc();
            nd = doc.SelectSingleNode("//AutomationEvent");
            nd.Attributes["Name"].Value = nd.Attributes["Name"].Value.MidString("", "@");
            return doc.ToMyString();
        }

        /// <summary>
        /// 获取指令名称列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetCommandList()
        {
            List<string> LstCmdName = new List<string>();
            XmlNodeList CmdList = CommandReqDoc.SelectNodes("//AutomationCommand");
            foreach (XmlNode node in CmdList)
            {
                LstCmdName.Add(node.Attributes["Name"].Value);
            }
            return LstCmdName;
        }

        /// <summary>
        /// 获取指令对象
        /// </summary>
        /// <param name="CommandName"></param>
        /// <returns></returns>
        public AutomationCommand GetCommandObject(string CommandName)
        {
            try
            {
                string strCommandModel = GetCommandXml(CommandName);
                AutomationCommand cmd = strCommandModel.XmlDeserialize<AutomationCommand>();
                return cmd;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 初始化参数字典
        /// </summary>
        /// <returns></returns>
        private JDictionary InitCommandFieldDict()
        {
            JDictionary MergedDictionaries = new JDictionary();
            List<string> LstCmdName = new List<string>();
            XmlNodeList nodeList = CommandReqDoc.SelectNodes("//AutomationCommand");
            foreach (XmlNode node in nodeList)
            {
                JDictionary Dic = new JDictionary();
                string strCmdName = node.Attributes["Name"].Value;
                string strInnerText = node.OuterXml.ToMyString();
                List<string> LstFields = strInnerText.RegexMatchedList(RegexFieldText, true);
                foreach (string item in LstFields)
                {
                    MergedDictionaries[strCmdName][item] = string.Empty;
                }
            }
            nodeList = EventDoc.SelectNodes("//AutomationEvent");
            foreach (XmlNode node in nodeList)
            {
                JDictionary Dic = new JDictionary();
                string strEventName = node.Attributes["Name"].Value;
                string strInnerText = node.OuterXml.ToMyString();
                List<string> LstFields = strInnerText.RegexMatchedList(RegexFieldText, true);
                foreach (string item in LstFields)
                {
                    MergedDictionaries[strEventName][item] = string.Empty;
                }
            }
            return MergedDictionaries;
        }

        /// <summary>
        /// 写参数
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="ParamName"></param>
        /// <param name="ParamValue"></param>
        public void WriteCommandParam(string CommandName, string ParamName, string ParamValue)
        {
            if (!ParamName.StartsWith("@") || !ParamName.EndsWith("@"))
            {
                ParamName = ParamName.Replace("@", "");
                ParamName = $"@{ParamName}@";
            }
            ReqField[CommandName].SetValue(ParamName, ParamValue);
        }

        /// <summary>
        /// 参数自动对应到实体写值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name"></param>
        /// <param name="Model"></param>
        public void WriteCommandParam<T>(string Name, T Model)
        {
            if (Model == null || Name.IsEmpty()) return;
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReqField[Name].ToMyString());
            foreach (KeyValuePair<string, string> item in dic)
            {
                string strItem = item.Key.Replace("@", "");
                string ParamValue = Model?.GetPropValue(strItem).ToMyString();
                ReqField[Name].SetValue(item.Key, ParamValue);
            }
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
    }
}
