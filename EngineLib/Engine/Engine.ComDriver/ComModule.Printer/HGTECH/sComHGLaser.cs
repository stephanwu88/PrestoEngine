using Engine.Common;
using Engine.Core;
using Engine.Data;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;

namespace Engine.ComDriver.HGTECH
{
    /// <summary>
    /// 指令请求枚举
    /// </summary>
    public enum Enum_REQ
    {
        [Description("@GET_MARKER_STATUS")]
        Get_Marker_Status = 1,
        [Description("@REGISTER")]
        Register = 2,
        [Description("@PROCESS_START")]
        Process_Start = 3,
    }

    /// <summary>
    /// 指令反馈枚举
    /// </summary>
    public enum Enum_ACK
    {
        [Description("@MARKER_STATUS")]
        Marker_Status = 1,
        [Description("@REGISTERED")]
        Registered = 2,
        [Description("@PROCESS_STARTED")]
        Process_Started = 3,
        [Description("@PROCESS_ERROR")]
        Process_Error = 5,
    }

    /// <summary>
    /// 请求字段
    /// </summary>
    public class ReqField
    {
        //多字段逗号分割
        public string _Register__Fields__ = string.Empty;
        public string _Register__StartNow__ = string.Empty;
    }

    /// <summary>
    /// 响应字段
    /// </summary>
    public class AckField
    {
        public string _Status_Initializing;
        public string _Status_InitSuccessed;
        public string _Status_Marking;

        public string _Registered_Fields = string.Empty;
        public string _ProcStarted_Fields = string.Empty;
        public string _ProcFinished_Fields = string.Empty;

        public string _Error_Message = string.Empty;
    }

    /// <summary>
    /// 电文字段
    /// </summary>
    public class TelegramField
    {
        /// <summary>
        /// 字段类型（必须 or 可选）MUST OPTION
        /// </summary>
        public string FieldType = "MUST";
        /// <summary>
        /// 字段引用符号
        /// Ref 时字段双引号 ""
        /// </summary>
        public string FiledMark;
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName;
        /// <summary>
        /// 字段值
        /// </summary>
        public string FieldVal;
    }

    /// <summary>
    /// 电文结构
    /// </summary>
    public class Telegram
    {
        public string MsgType;

        public Dictionary<string, TelegramField> Fields = new Dictionary<string, TelegramField>();

        public void Add(TelegramField field)
        {
            if (Fields.ContainsKey(field.FieldName))
                Fields[field.FieldName] = field;
            else
                Fields.Add(field.FieldName, field);
        }

        public void Add(List<TelegramField> LstFields)
        {
            foreach (var field in LstFields)
            {
                if (Fields.ContainsKey(field.FieldName))
                    Fields[field.FieldName] = field;
                else
                    Fields.Add(field.FieldName, field);
            }
        }

        public string GetFieldString(int id)
        {
            TelegramField[] FieldVal = Fields.Values.ToArray();
            if (FieldVal.Length > id)
                return FieldVal[id].FieldVal;
            else
                return string.Empty;
        }

        public int FieldCount => Fields.Count;
    }

    /// <summary>
    /// 会话区
    /// </summary>
    public class SessionMemory : NotifyObject
    {
        //状态字
        public string State_Normal { get; set; } = string.Empty;
        public string State_Run { get; set; } = string.Empty;
        public string State_Alarm { get; set; } = string.Empty;
        public string State_Offline { get; set; } = string.Empty;
        /// <summary>
        /// 初始化完成
        /// </summary>
        public string InitSuccessed { get; set; }
        /// <summary>
        /// 正在标刻
        /// </summary>
        public string Marking { get; set; }
        /// <summary>
        /// 正在初始化
        /// </summary>
        public string Initializing { get; set; }
        /// <summary>
        /// 红光开启
        /// </summary>
        public string EnableRed { get; private set; }
        /// <summary>
        /// 发生报警
        /// </summary>
        public string Error { get; set; } = string.Empty;
        /// <summary>
        /// 报警文本
        /// </summary>
        public string ErrorText { get; set; } = string.Empty;
        /// <summary>
        /// 标刻机未就绪
        /// </summary>
        public string NotReady { get; set; }
    }

    /// <summary>
    /// 武汉华工激光
    /// </summary>
    public class sComHGLaser : sComHGLaser<ServerNode>
    {
        public sComHGLaser() : base()
        {

        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComHGLaser(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {

        }
    }

    /// <summary>
    /// 武汉华工激光
    /// </summary>
    public partial class sComHGLaser<TConNode> : sComRuleAsc where TConNode:new()
    {
        private IDBFactory<TConNode> _DB = DbFactory.Current.GetConn<TConNode>(DbConKey.Task);
        public ReqField ReqField = new ReqField();
        public AckField AckField = new AckField();
        public SessionMemory Session { get; } = new SessionMemory();
        /// <summary>
        /// 协议变量列表
        /// </summary>
        private List<ModelComHG> ListComSymbol;

        /// <summary>
        /// 报文字典
        /// </summary>
        public Dictionary<Enum_REQ, Telegram> _REQTelegrams = new Dictionary<Enum_REQ, Telegram>();
        public Dictionary<Enum_ACK, Telegram> _ACKTelegrams = new Dictionary<Enum_ACK, Telegram>();

        private string LastRcvStatusContent = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        public sComHGLaser()
        {
            EDX = "@ENDE";
            foreach (Enum_REQ req in Enum.GetValues(typeof(Enum_REQ)))
                _REQTelegrams.Add(req, new Telegram() { MsgType = req.FetchDescription() });
            foreach (Enum_ACK ack in Enum.GetValues(typeof(Enum_ACK)))
                _ACKTelegrams.Add(ack, new Telegram() { MsgType = ack.FetchDescription() });
        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComHGLaser(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            EDX = "@ENDE";
            foreach (Enum_REQ req in Enum.GetValues(typeof(Enum_REQ)))
                _REQTelegrams.Add(req, new Telegram() { MsgType = req.FetchDescription() });
            foreach (Enum_ACK ack in Enum.GetValues(typeof(Enum_ACK)))
                _ACKTelegrams.Add(ack, new Telegram() { MsgType = ack.FetchDescription() });
            if (_DB != null)
                LoadProtocolTable();
        }

        /// <summary>
        /// 加载协议列表
        /// </summary>
        /// <param name="ListComData"></param>
        public void LoadProtocolTable(List<ModelComHG> ListComData)
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
            if (ListComSymbol != null)
            {
                ListComSymbol = new List<ModelComHG>();
                return;
            }
            ModelComHG modelComHeao = new ModelComHG()
            {
                DriverToken = DriverItem.DriverToken,
                DataUnitType = "DataUnitType in ('Status','Alarm')".MarkExpress().MarkWhere()
            };
            DataTable dt = _DB.ExcuteQuery<ModelComHG>(modelComHeao).Result.ToMyDataTable();
            ListComSymbol = ColumnDef.ToEntityList<ModelComHG>(dt);
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
            //_REQTelegrams[Enum_REQ.Get_Marker_Status].Add(new List<TelegramField>() { });

            _REQTelegrams[Enum_REQ.Register].Add(new List<TelegramField>() {
                 new TelegramField() { FieldName = "Fields", FieldType = "MUST", FiledMark = "", FieldVal = ReqField._Register__Fields__ },
                 new TelegramField() { FieldName = "StartNow", FieldType = "OPTION", FiledMark = "", FieldVal = ReqField._Register__StartNow__ },
            });

            //_REQTelegrams[Enum_REQ.Process_Start].Add(new List<TelegramField>() {   });
            #endregion

            #region 响应报文
            _ACKTelegrams[Enum_ACK.Marker_Status].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Initializing", FieldType = "MUST", FiledMark = "", FieldVal = AckField._Status_Initializing },
                new TelegramField() { FieldName = "InitSuccessed", FieldType = "MUST", FiledMark = "", FieldVal = AckField._Status_InitSuccessed },
                new TelegramField() { FieldName = "Marking", FieldType = "MUST", FiledMark = "", FieldVal = AckField._Status_Marking }
            });

            _ACKTelegrams[Enum_ACK.Process_Started].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Fields", FieldType = "OPTION", FiledMark = "", FieldVal = AckField._ProcStarted_Fields },
            });

            _ACKTelegrams[Enum_ACK.Process_Error].Add(new List<TelegramField>() {
                new TelegramField() { FieldName = "Message", FieldType = "OPTION", FiledMark = "", FieldVal = AckField._Error_Message },
            });
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
            if (LogSend) LastRcvStatusContent = string.Empty;
            return Request(Enum_REQ.Get_Marker_Status, out str, LogSend);
        }

        /// <summary>
        /// 复位流程
        /// </summary>
        public void ResetProc()
        {
            Session.Error = "0";
            Session.ErrorText = "";
            TryParseBinder();
        }

        /// <summary>
        /// 协议请求
        /// </summary>
        /// <param name="eREQ"></param>
        /// <param name="strTelegram"></param>
        public bool Request(Enum_REQ eREQ, out string strTelegram, bool LogSend = true)
        {
            string strMsg = string.Empty;
            UpdateTelegram();
            strMsg = _REQTelegrams[eREQ].MsgType;
            foreach (var fld in _REQTelegrams[eREQ].Fields.Values)
            {
                if (fld.FieldType == "OPTION" && string.IsNullOrEmpty(fld.FieldVal.ToMyString()))
                    continue;
                string strFldMarkLeft = fld.FiledMark == "Ref" ? "\"" : "";
                string strFldMarkRight = fld.FiledMark == "Ref" ? "\"" : "";
                strMsg += string.Format("@{0}={1}{2}{3}", fld.FieldName, strFldMarkLeft, fld.FieldVal, strFldMarkRight);
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
                strMsg += string.Format("@{0}={1}{2}{3}", fld.FieldName, strFldMarkLeft, fld.FieldVal, strFldMarkRight);
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
                            string strMessage = _QueueRcv.Dequeue();
                            string strErrorMessage = ParseRecievedContent(strMessage);
                            TryParseBinder();
                        }
                        else
                        {
                            bool NetStateChanged = false;
                            if (ComState != SocketState.Connected)
                                AckField._Status_InitSuccessed = "0";
                            if (ComState != LastComState)
                            {
                                NetStateChanged = true;
                                dtStart = DateTime.Now;
                                LastComState = ComState;
                            }
                            DateTime dtNow = DateTime.Now;
                            double iTimeSpan = (dtNow - dtStart).TotalSeconds;
                            if (iTimeSpan > 5 || NetStateChanged)
                            {
                                dtStart = DateTime.Now;
                                RequestStatus(false);
                                TryParseBinder();
                            }
                        }
                    }
                }
                catch (Exception )
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
                        //lock (CommandQueue)
                        {
                            ModelComHG modelQueryCmd = new ModelComHG()
                            {
                                AwaitState = string.Format(" DriverToken = '{0}' and DataUnitType in('Command') and " +
                                    "AwaitState in ('Sending','TaskSending') order by AwaitTime asc", DriverItem.DriverToken)
                                    .MarkExpress().MarkWhere()
                            };
                            CallResult _result = _DB.ExcuteQuery<ModelComHG>(modelQueryCmd);
                            if (_result.Success)
                            {
                                DataTable dt = _result.Result.ToMyDataTable();
                                foreach (DataRow row in dt.Rows)
                                {
                                    modelQueryCmd = ColumnDef.ToEntity<ModelComHG>(row);
                                    string strID = modelQueryCmd.ID.ToMyString();
                                    string strCommandValue = modelQueryCmd.DataValue;
                                    string strCmdComment = modelQueryCmd.Comment.ToMyString();
                                    if (string.IsNullOrEmpty(strCmdComment))
                                        strCmdComment = strCommandValue;

                                    string Message = string.Format("【Command】【{0}】{1}",
                                           modelQueryCmd.RelatedGroup.ToMyString(), strCmdComment);
                                    Logger.CommBody.Write(LOG_TYPE.MESS, Message);

                                    //bool SendSuccess = Send(modelQueryCmd.DataValue);
                                    CallResult result = ParseCommandInvoke(modelQueryCmd.RelatedVariable,
                                        modelQueryCmd.DataValue);
                                    if (result.Fail)
                                    {
                                        string Error = string.Format("【Command发送错误】【{0}】{1}:{2}",
                                           modelQueryCmd.RelatedGroup.ToMyString(), strCmdComment,
                                           result.Result.ToMyString());
                                        Logger.CommBody.Write(LOG_TYPE.ERROR, Error);
                                    }
                                    modelQueryCmd = new ModelComHG()
                                    {
                                        ID = strID.MarkWhere(),
                                        AwaitState = "Sended",
                                        AwaitTime = SystemDefault.StringTimeNow
                                    };

                                    _DB.ExcuteUpdate<ModelComHG>(modelQueryCmd);
                                }
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
        /// 添加日志记录
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="DataType">SND or RCV</param>
        protected override void AddCommLog(string strData, string DataType)
        {
            if (strData.Contains("@MARKER_STATUS"))
            {
                if (strData == LastRcvStatusContent)
                    return;
                LastRcvStatusContent = strData;
            }
            string strContent = string.Format("【{0}】 【{1}】  {2}", DataType, DriverItem.DriverName.ToMyString(), strData);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }

        /// <summary>
        /// 解析接收报文
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        private string ParseRecievedContent(string strMessage)
        {
            string ErrorMessage = string.Empty;
            string strMessKey = strMessage.MidString("@", "@");
            string AckContent = string.Empty;
            switch (strMessKey)
            {
                case "MARKER_STATUS":
                    AckField._Status_Initializing = strMessage.MidString("@Initializing=", "@");
                    AckField._Status_InitSuccessed = strMessage.MidString("@InitSuccessed=", "@");
                    AckField._Status_Marking = strMessage.MidString("@Marking=", "@");
                    break;
                case "REGISTERED":
                    //@REGISTER@Fields=打印内容1,打印内容2@StartNow=Y@ENDE
                    break;
                case "PROCESS_STARTED":
                    break;
                case "PROCESS_ERROR":
                    Session.Error = "1";
                    Session.ErrorText = strMessage.MidString("@Message=", "@");
                    break;
            }
            return ErrorMessage;
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
                string[] ArrayDataValue = DataValue.ToMyString().MySplit(",").ToArray();
                //参数换元
                FuncParamField = string.Format(FuncParamField, ArrayDataValue);
                //参数字符串按，分割给参
                List<string> FuncParams = FuncParamField.MySplit(",");
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
        private void SessionMake()
        {
            Session.Initializing = AckField._Status_Initializing;
            Session.InitSuccessed = AckField._Status_InitSuccessed;
            Session.Marking = AckField._Status_Marking;
            Session.NotReady = AckField._Status_InitSuccessed == "1" ? "0" : "1";

            Session.State_Offline = ComState == SocketState.Connected ? "0" : "1";
            Session.State_Alarm = (Session.Error == "1" || Session.NotReady == "1") ? "1" : "0";
            if (Session.State_Offline == "1" || Session.State_Alarm == "1")
            {
                Session.State_Normal = "0";
                Session.State_Run = "0";
            }
            else
            {
                Session.State_Run = Session.Marking == "1" ? "1" : "0";
                Session.State_Normal = Session.Marking == "1" ? "0" : "1";
            }
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
            List<ModelComHG> LstModel = ListComSymbol.Where(x => x.DataUnitType != "Command").ToList();
            List<string> LstRelatedExpress = new List<string>();
            List<ModelComHG> LstModelUpdate = new List<ModelComHG>();
            foreach (ModelComHG com in LstModel)
            {
                string strRelatedVariable = com.RelatedVariable;
                string strDataValue = Session.GetPropValue(com.DataName).ToMyString();
                //添加通讯值
                ModelComHG modelUpdate = new ModelComHG()
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
                CallResult _result = _DB.ExcuteUpdate<ModelComHG>(LstModelUpdate);
            }
            return true;
        }
    }

    /// <summary>
    /// 接口实现
    /// </summary>
    public partial class sComHGLaser<TConNode> : IComPrinter
    {
        /// <summary>
        /// 发送打印内容
        /// </summary>
        /// <param name="PrintText"></param>
        public void Print(string PrintText)
        {
            string ReqContent = string.Empty;
            ReqField._Register__Fields__ = PrintText;
            ReqField._Register__StartNow__ = "Y";
            Request(Enum_REQ.Register, out ReqContent);
        }


        /// <summary>
        /// 发送打印内容
        /// </summary>
        /// <param name="PrintText1"></param>
        /// <param name="PrintText2"></param>
        public void Printf(string PrintText1, string PrintText2)
        {
            string ReqContent = string.Empty;
            ReqField._Register__Fields__ = PrintText1 + "," + PrintText2;
            ReqField._Register__StartNow__ = "Y";
            Request(Enum_REQ.Register, out ReqContent);
        }

        /// <summary>
        /// 打印标签 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <returns></returns>
        public bool PrintLabel(string PrnFile, string[] FieldContent)
        {
            return PrintLabel(PrnFile, FieldContent, 1);
        }

        /// <summary>
        /// 打印标签 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <param name="CopyCount">打印次数</param>
        /// <returns></returns>
        public bool PrintLabel(string PrnFile, string[] FieldContent, int CopyCount)
        {
            return true;
        }
    }
}
