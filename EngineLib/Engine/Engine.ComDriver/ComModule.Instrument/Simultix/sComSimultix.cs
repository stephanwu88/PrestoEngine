using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.ComDriver.Simultix
{
    /// <summary>
    /// 请求功能码
    /// </summary>
    public enum ReqTextNo
    {
        [Description("20")]
        ReqSpecStatus,
        [Description("21")]
        ReqIDReservation,
        [Description("24")]
        NotifyPrepareCompleted,
        [Description("25")]
        NotifySampleUnload
    }

    /// <summary>
    /// 回码功能码
    /// </summary>
    public enum AckTextNo
    {
        [Description("02")]
        AckSampleUnload,
        [Description("04")]
        AckAnaCompleted,
        [Description("05")]
        AckOnlineMessage,
        [Description("05")]
        AckOfflineMessage,
        [Description("06")]
        AckIDReservePossible,
        [Description("09")]
        AckError,
    }

    /// <summary>
    /// 报文类型标志
    /// </summary>
    public enum TypeCodeFlag
    {
        A,  //回复报文
        R,  //请求报文
        I   //信息状态报文
    }

    /// <summary>
    /// 会话区
    /// </summary>
    public class SessionMemory: NotifyObject
    {
        //状态字
        public string State_Normal { get; set; } = string.Empty;
        public string State_Run { get; set; } = string.Empty;
        public string State_Alarm { get; set; } = string.Empty;
        public string State_Wait { get; set; } = string.Empty;
        public string State_Done { get; set; } = string.Empty;

        //自动化样品传输系统在线
        public string Status_Online { get; set; } = "0";            //0,1
        //自动化样品传输系统离线
        public string Status_Offline { get; set; } = "1";           //0,1
        //荧光测量就绪
        public string Status_ReadyForMeasure { get; set; } = "0";   //0,1
        //荧光状态代码  
        public string Status_SpectroCode { get; set; } = string.Empty;
        //荧光状态代码描述
        public string Status_SpectroCodeDesc { get; set; } = string.Empty;
        //传输状态代码
        public string Status_TransportCode { get; set; } = string.Empty;
        //传输状态代码描述
        public string Status_TransportDesc { get; set; } = string.Empty;

        //样品注册的制样工位代号 默认：20
        public string Proc_PreparatorNo { get; set; } = string.Empty;
        //注册结果代码
        public string Proc_ReservationResponseCode { get; set; } = string.Empty;
        //注册结果描述
        public string Proc_ReservationResponseDesc { get; set; } = string.Empty;
        //仪器返回 SampleUnload[02码] 代表允许分析，需要发SampleUnload指令，启动分析
        public string Proc_MeasureAllowed { get; set; } = "0";
        //已经进入测量分析状态
        public string Proc_IsMeasuring { get; set; } = "0";
        //样品已经输出到位
        public string Proc_SampleIsOutput { get; set; } = "0";
        //样品已取走
        public string Proc_SampleIsRemoved { get; set; } = "0";
        //流程错误信息
        public string Proc_Error {
            get => _Proc_Error;
            set
            {
                _Proc_Error = value;
                RaisePropertyChanged("Proc_Error");
            }
        }
        private string _Proc_Error;

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

        public string Error_荧光及进样装置未就绪 { get => Proc_Error == "荧光及进样装置未就绪" ? "1" : "0"; } 
        public string Error_拒绝进样请求 { get => Proc_Error == "拒绝进样请求" ? "1" : "0"; }
        public string Error_拒绝样品启动分析 { get => Proc_Error == "拒绝样品启动分析" ? "1" : "0"; } 
    }

    /// <summary>
    /// Simultix Spectro Protocol
    /// 日本理学荧光协议
    /// </summary>
    public partial class sComSimultix : sComRuleAsc
    {
        //协议结构
        //STX + Header + Messsage + Crlf
        //1       10        0-242    2

        //000:Data Processing System 理学方 001:Preset Panel 集成方
        private string StationAddress = "001";
        //Header结构中的一部分，协议中固定"E"
        private string MultiBlock = "E";
        //Header结构中的保留部分
        private string HeaderUnused = "".PadRight(3, Convert.ToChar(0x20));
        //制样器序号 20:调试测试发现，猜测为集成方制样序号
        //随便注册一个序号，返回的错误码中的序号就是仪器需要的
        private string PreparatorNo = "20";
        //协议定义 - 分割符
        private char SpaceChar = Convert.ToChar(0x20);
        private string SpaceStr = Convert.ToChar(0x20).ToString();
        //通讯会话区
        public SessionMemory Session { get; } = new SessionMemory();
        /// <summary>
        /// 协议变量列表
        /// </summary>
        private List<ModelComInstr> ListComSymbol;

        private string LastRcvStatusContent = string.Empty;

        protected List<string> LstSpecStatusCode;
        protected List<string> LstSampleTransStatusCode;

        private IDBFactory<ServerNode> _DB = DbFactory.CPU.CloneInstance("Com");

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public sComSimultix()
        {
            STX = Convert.ToChar(0x02).ToString();
            EDX = SystemDefault.Crlf;
            BuildDefaultCode();
        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComSimultix(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            STX = Convert.ToChar(0x02).ToString();
            EDX = SystemDefault.Crlf;
            BuildDefaultCode();
            if (_DB != null)
                LoadProtocolTable();
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
        }

        /// <summary>
        /// 预设字段解析释义
        /// </summary>
        private void BuildDefaultCode()
        {
            LstSpecStatusCode = new List<string>() {
                "Initializing","Load Ready","Air->Vacuum","Vacuum->Air","He->Air",
                "Air->He","Loading","Slow Vacuum","Fast Vacuum","Air",
                "He","Shutter Open","Waiting for Vacuum","Waiting","Operation",
                "Shutter Close","Data Out","Unloading","Center Wire Cleaning"
            };
            LstSampleTransStatusCode = new List<string>() {
                "Initializing","Load Ready","Transporting Online Sample","Transporting Holder",
                "Into Holder","Loading","From Holder","Collecting Holder","Waiting for Unloading"
            };
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
        /// 请求仪器状态
        /// </summary>
        /// <returns></returns>
        public bool RequestStatus(bool LogSend = true)
        {
            return Request(ReqTextNo.ReqSpecStatus, TypeCodeFlag.R, "", LogSend);
        }

        /// <summary>
        /// 注册样品信息
        /// </summary>
        /// <param name="SampleName">样品名称 固定13个字符，不足补空</param>
        /// <param name="AnaCode">任务代码 代表分析类型 固定8位,不足补空</param>
        /// <param name="JobName">任务名称 1个字符</param>
        /// <returns></returns>
        public bool ReserveSample(string SampleName, string AnaCode, string JobName = "T")
        {
            string FieldR = "0";                        //代表注册SampleID
            string FieldP = "".PadRight(2, SpaceChar);  //保留字段
            //AnaCode = AnaCode.PadRight(8, SpaceChar);
            AnaCode = AnaCode.PadRight(4, SpaceChar);
            SampleName = SampleName.PadRight(16, SpaceChar);
            string NineSpace = "".PadRight(9, SpaceChar);
            string SampleID = JobName + SpaceStr + AnaCode + SpaceStr + SampleName;
            string strMessage = PreparatorNo + FieldR + FieldP + SampleID + NineSpace;
            return Request(ReqTextNo.ReqIDReservation, TypeCodeFlag.R, strMessage);
        }

        /// <summary>
        /// 复位样品任务
        /// </summary>
        /// <returns></returns>
        public bool ResetSampleTask()
        {
            Session.Proc_MeasureStep = "分析任务待命";
            ClearProcStatus();
            return true;
        }

        /// <summary>
        /// 清除流程状态
        /// </summary>
        private void ClearProcStatus()
        {
            Session.Status_SpectroCode = string.Empty;
            Session.Status_SpectroCodeDesc = string.Empty;
            Session.Status_TransportCode = string.Empty;
            Session.Status_TransportDesc = string.Empty;

            Session.Proc_ReservationResponseCode = string.Empty;
            Session.Proc_ReservationResponseDesc = string.Empty;

            Session.Proc_MeasureAllowed = string.Empty;
            Session.Proc_IsMeasuring = string.Empty;
            Session.Proc_SampleIsOutput = string.Empty;
            Session.Proc_SampleIsRemoved = string.Empty;

            Session.Proc_Error = string.Empty;
            RequestStatus(false);
        }

        /// <summary>
        /// 启动样品任务
        /// </summary>
        /// <param name="SampleName">样品编码</param>
        /// <param name="AnaCode">分析代码</param>
        /// <param name="strJobName">任务名称</param>
        /// <returns></returns>
        public bool StartSampleTask(string SampleName, string AnaCode, string strJobName = "T")
        {
            Session.Proc_MeasureStep = "样品流程启动";
            Task.Factory.StartNew(() =>
            {
                while (Session.Proc_MeasureStep.ToMyString() != "分析任务待命")
                {
                    switch (Session.Proc_MeasureStep)
                    {
                        case "样品流程启动":
                            RequestStatus();
                            Session.Proc_MeasureStep = "进样前确认状态";
                            break;

                        case "进样前确认状态":
                            if (Session.Status_SpectroCode == "01" && Session.Status_TransportCode == "01")
                            {
                                //注册样品
                                ReserveSample(SampleName, AnaCode, strJobName);
                                Session.Proc_MeasureStep = "样品进样请求";
                            }
                            else if ((!string.IsNullOrEmpty(Session.Status_SpectroCode) && Session.Status_SpectroCode != "01") ||
                                (!string.IsNullOrEmpty(Session.Status_TransportCode) && Session.Status_TransportCode != "01"))
                            {
                                Session.Proc_MeasureStep = "分析任务待命";
                                Session.Proc_Error = "荧光及进样装置未就绪";
                            }
                            break;

                        case "样品进样请求":
                            if (Session.Proc_ReservationResponseCode == "0")
                            {
                                ClearProcStatus();
                                NotifySamplePrepareCompleted(); //通知仪器样品已放置号
                                Session.Proc_MeasureStep = "允许样品放置";
                            }
                            else if (!string.IsNullOrEmpty(Session.Proc_ReservationResponseCode) && Session.Proc_ReservationResponseCode != "0")
                            {
                                Session.Proc_MeasureStep = "分析任务待命";
                                Session.Proc_Error = "拒绝进样请求";
                            }
                            break;

                        case "允许样品放置":
                            if (Session.Proc_MeasureAllowed == "1")
                            {
                                NotifySampleCanBeAnalyzed();       //通知仪器进样到分析器
                                Session.Proc_MeasureStep = "允许样品分析";
                            }
                            else if (!string.IsNullOrEmpty(Session.Proc_MeasureAllowed) && Session.Proc_MeasureAllowed != "1")
                            {
                                Session.Proc_MeasureStep = "分析任务待命";
                                Session.Proc_Error = "拒绝进样请求";
                            }
                            break;

                        case "允许样品分析":
                            if (Session.Proc_IsMeasuring == "1")
                                Session.Proc_MeasureStep = "样品正在分析";
                            else if (!string.IsNullOrEmpty(Session.Proc_IsMeasuring) && Session.Proc_IsMeasuring != "1")
                            {
                                Session.Proc_MeasureStep = "分析任务待命";
                                Session.Proc_Error = "拒绝样品启动分析";
                            }
                            break;

                        case "样品正在分析":
                            if (Session.Proc_SampleIsOutput == "1")
                                Session.Proc_MeasureStep = "样品已输出至皮带";
                            break;

                        case "样品已输出至皮带":
                            if (Session.Proc_SampleIsRemoved == "1")
                                Session.Proc_MeasureStep = "分析任务待命";
                            break;
                    }
                    Thread.Sleep(100);
                }
            });
            return true;
        }

        /// <summary>
        /// 通知仪器进样系统样品已放置好,仪器将启动皮带进样
        /// </summary>
        /// <returns></returns>
        public bool NotifySamplePrepareCompleted()
        {
            return Request(ReqTextNo.NotifyPrepareCompleted, TypeCodeFlag.I, PreparatorNo);
        }

        /// <summary>
        /// 通知仪器可以启动分析
        /// </summary>
        /// <returns></returns>
        public bool NotifySampleCanBeAnalyzed()
        {
            //0:允许仪器出样  1：有情况，不允许仪器出样，但是仪器会清除该样品
            //这里始终允许仪器出样
            string strResCode = "0";
            string strMessage = PreparatorNo + strResCode;
            return Request(ReqTextNo.NotifySampleUnload, TypeCodeFlag.I, strMessage);
        }

        /// <summary>
        /// 封装发送
        /// </summary>
        /// <param name="TextNo">报文序号-功能码  2位数字</param>
        /// <param name="TypeCode">A：回复 I：信息 R：请求</param>
        /// <param name="strMessage"></param>
        public bool Request(ReqTextNo TextNo, TypeCodeFlag TypeCode, string strMessage,bool LogSend = true)
        {
            //Header结构  
            //StationAdress + TextNo + Flag(TypeCode  Multi-Block Unused)
            //     3           2                1          1         3
            string strHeader = StationAddress + TextNo.FetchDescription() + TypeCode.ToString() + MultiBlock + HeaderUnused;
            string strSend = strHeader + strMessage;
            if (TextNo == ReqTextNo.ReqSpecStatus)
            {
                Session.Status_SpectroCode = string.Empty;
                Session.Status_SpectroCodeDesc = string.Empty;
                Session.Status_TransportCode = string.Empty;
                Session.Status_TransportDesc = string.Empty;
                if (LogSend) LastRcvStatusContent = string.Empty;
            }
            else if (TextNo == ReqTextNo.ReqIDReservation)
            {
                Session.Proc_ReservationResponseCode = string.Empty;
                Session.Proc_ReservationResponseDesc = string.Empty;
            }
            else if (TextNo == ReqTextNo.NotifyPrepareCompleted)
            {
                Session.Proc_MeasureAllowed = string.Empty;
                Session.Proc_IsMeasuring = string.Empty;
                Session.Proc_SampleIsOutput = string.Empty;
                Session.Proc_SampleIsRemoved = string.Empty;
            }
            else if (TextNo == ReqTextNo.NotifySampleUnload)
            {
                Session.Proc_IsMeasuring = string.Empty;
                Session.Proc_SampleIsOutput = string.Empty;
            }
            return base.Send(strSend, LogSend);
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
                            Session.Status_Online = "1";
                            Session.Status_Offline = "0";
                            string strErrorMessage = ParseRecievedContent(strMessage);
                            TryParseBinder();
                        }
                        else
                        {
                            bool NetStateChanged = false;
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
                                    CallResult result = ParseCommandInvoke(modelQueryCmd.RelatedVariable,
                                        modelQueryCmd.DataValue);
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
            if (DataType == "RCV" && !string.IsNullOrEmpty(LastRcvStatusContent) && 
                strData.StartsWith(LastRcvStatusContent)) return;
            if (strData.EndsWith("\r\n")) strData = strData.MidString("", "\r\n");
            string strContent = string.Format("【{0}】 【{1}】  {2}", DataType, DriverItem.DriverName.ToMyString(), strData);
            Logger.CommBody.Write(LOG_TYPE.MESS, strContent);
        }

        /// <summary>
        /// 解析接收报文
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns>Error Message</returns>
        protected string ParseRecievedContent(string strMessage)
        {
            string strSTX = strMessage.MidString(0, 1);
            string strHeader = strMessage.MidString(1, 10);
            string strStation = strHeader.MidString(0, 3);
            string strTextNo = strHeader.MidString(3, 2);
            string strTypeCode = strHeader.MidString(5, 1);
            string strContent = strMessage.MidString(11);
            if (strSTX != STX)
                return string.Format("错误报文,不正确的起始符");
            switch (strTextNo)
            {
                case "02":     //SampleUnload[02] 可以分析，SampleUnload[25]启动命令
                    Session.Proc_MeasureAllowed = "1";
                    break;

                case "04":     //出样皮带传输完成
                    Session.Proc_SampleIsOutput = "1";
                    break;

                case "05":     //OnLine OffLine
                    if (strContent == "1")
                    {
                        Session.Status_Online = "1";
                        Session.Status_Offline = "0";
                    }
                    else if (strContent == "0")
                    {
                        Session.Status_Online = "0";
                        Session.Status_Offline = "1";
                        Session.Status_SpectroCode = "";
                        Session.Status_TransportCode = "";
                        Session.Status_SpectroCodeDesc = "";
                        Session.Status_TransportDesc = "";
                    }
                    break;

                case "06":    //AckIDReservePossible  相当于仪器已执行分析
                    Session.Proc_IsMeasuring = "1";
                    break;

                case "09":    //错误代码
                    break;

                case "20":    //仪器状态
                    LastRcvStatusContent = strMessage;
                    Session.Status_SpectroCode = strContent.MidString(0, 2);
                    Session.Status_TransportCode = strContent.MidString(2, 2);
                    if (string.IsNullOrEmpty(Session.Status_SpectroCode) ||
                        Session.Status_SpectroCode.ToMyInt() > LstSpecStatusCode.Count)
                        Session.Status_SpectroCodeDesc = string.Empty;
                    else
                        Session.Status_SpectroCodeDesc = LstSpecStatusCode[Session.Status_SpectroCode.ToMyInt()];
                    if (string.IsNullOrEmpty(Session.Status_TransportCode) ||
                        Session.Status_TransportCode.ToMyInt() > LstSampleTransStatusCode.Count)
                        Session.Status_TransportDesc = string.Empty;
                    else
                        Session.Status_TransportDesc = LstSampleTransStatusCode[Session.Status_TransportCode.ToMyInt()];
                    break;

                case "21":    //样品注册
                    Session.Proc_PreparatorNo = strContent.MidString(0, 2);
                    Session.Proc_ReservationResponseCode = strContent.MidString(2, 1);
                    if (Session.Proc_ReservationResponseCode == "0")
                        Session.Proc_ReservationResponseDesc = "Normal end(正常完成)";
                    else if (Session.Proc_ReservationResponseCode == "1")
                        Session.Proc_ReservationResponseDesc = "KeyWord Error(指令错误)";
                    else if (Session.Proc_ReservationResponseCode == "2")
                        Session.Proc_ReservationResponseDesc = "Duplicate Register(重复注册)";
                    else if (Session.Proc_ReservationResponseCode == "6")
                        Session.Proc_ReservationResponseDesc = "Reservation Fail(注册失败)";
                    else
                        Session.Proc_ReservationResponseDesc = "Unknown";
                    break;
            }

            Session.State_Alarm = (Session.Status_Offline == "0" && !string.IsNullOrEmpty(Session.Proc_Error)) ? "1" : "0";
            if (Session.Status_Offline == "1" || Session.State_Alarm == "1")
            {
                Session.State_Normal = "0";
                Session.State_Run = "0";
                Session.State_Done = "0";
                Session.Status_ReadyForMeasure = "0";
            }
            else
            {
                Session.State_Done = Session.Proc_MeasureStep == "样品已输出至皮带" ? "1" : "0";
                Session.State_Run = Session.Proc_MeasureStep == "样品正在分析" ? "1" : "0";
                Session.Status_ReadyForMeasure = Session.Status_SpectroCode == "01" && Session.Status_TransportCode == "01"
                   && Session.State_Done == "0" ? "1" : "0";
                Session.State_Normal = Session.Status_ReadyForMeasure == "1" && Session.State_Run == "0" ? "1" : "0";
                Session.State_Wait = Session.State_Done == "0" && Session.State_Run == "0" && Session.State_Normal == "0" ? "1" : "0";
            }
            return string.Empty;
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
        private void SessionMake()
        {

        }

        /// <summary>
        /// 解析绑定
        /// </summary>
        /// <param name="DataName"></param>
        /// <param name="strDataValue"></param>
        /// <returns></returns>
        private bool TryParseBinder()
        {
            SessionMake();
            if (DriverItem.ServerUpdEn != "1") return true;
            List<ModelComInstr> LstModel = ListComSymbol.Where(x => x.DataUnitType!="Command").ToList();
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
    /// 理学仪器分析结果文件解析
    /// </summary>
    public partial class sComSimultix
    {
        /// <summary>
        /// 解析理学仪器分析结果文件
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <returns></returns>
        public static CallResult ParseReultDataFile(string fileName)
        {
            //-----------分析正常数据文件
            //"7","常规分析","ST","ST","    SAM6","1","","缺省","No"
            //,,,,"Si",,"Mn",,"P",,"S",,"Ti",,"V",,"Nb"
            //,,,,"mass%",,"mass%",,"mass%",,"mass%",,"mass%",,"mass%",,"mass%"
            //"ST","2023- 1- 8 18:22",,,25.082,"",0.090,"",0.012,"",1.395,"",0.321,"",0.003,"",0.062,""

            //-----------分析异常数据文件
            //"1","检查分析","ST","ST","KY","","","缺省","No"
            //"0","测量出错 -- 样品盘或ASC上没有样品."
            CallResult res = new CallResult() { Success = false };
            List<string> FileLines = sCommon.ReadFile(fileName);
            int iLineCount = FileLines.MyCount();
            if (iLineCount  != 4 && iLineCount != 2)
            {
                res.Result = "分析数据文件行数错误！";
                return res;
            }
            ModelXRFData ResultData = new ModelXRFData();
            List<string> LstIgnore = new List<string>() { "\"\"", "\" \"" };
            List<string> LstRemove = new List<string>() { "\"" };
            List<string> line1 = FileLines[0].MySplit(",", null, LstRemove);
            List<string> line2 = FileLines[1].MySplit(",", LstIgnore, LstRemove);
            if (line1.Count >= 9)
            {
                ResultData.Pos = line1[0];
                ResultData.JobName = line1[1];
                ResultData.AnaProgram = line1[2];
                ResultData.AnaCode = line1[3];
                ResultData.SampleName = line1[4];
                ResultData.AnaCountNum = line1[5];
                ResultData.Calculate = line1[6];
                ResultData.Print = line1[7];
                ResultData.Send = line1[8];
                if (FileLines.Count == 4)
                {
                    List<string> line3 = FileLines[2].MySplit(",", LstIgnore, LstRemove);
                    List<string> line4 = FileLines[3].MySplit(",", LstIgnore, LstRemove);
                    if (line2.Count == line3.Count && line2.Count + 2 == line4.Count)
                    {
                        //正常分析数据文件
                        ResultData.ResultState = "分析完成";
                        //2023-1-8 18:22  修正时间格式
                        ResultData.AnaEndTime = line4[1].Replace("- ", "-").Replace("/ ", "/").ToMyDateTimeStr();
                        for (int i = 0; i < line2.Count; i++)
                        {
                            //注: 理学仪器 铁元素结果字段在分析不同样品时返回不一样 高炉样：T.FE 精炼样：FE
                            //这里都和谐为FE
                            string strElemNameCode = line2[i].ToMyString().ToUpper().Replace("T.", "");
                            string strElemName = string.Format("ELEM_{0}", strElemNameCode);
                            string strElemValue = line4[i + 2];
                            if (!ResultData.DicSrcField.MyContains(strElemName))
                                ResultData.DicSrcField.AppandDict(strElemName, strElemValue);
                        }
                    }
                }
                else if (FileLines.Count == 4 && line2.Count >= 2)
                {
                    //分析出错文件
                    ResultData.ResultState = "分析出错";
                    ResultData.ResultErrorText = line2[1].MidString(0, 40);
                }
                res.Result = ResultData;
                res.Success = true;
            }
            else
            {
                res.Result = "分析数据文件内容解析错误！";
            }
            return res;
        }
    }
}
