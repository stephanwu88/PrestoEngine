using Engine.Common;
using Engine.Data.DBFAC;
using System;
using System.Diagnostics;
using System.Threading;
using Engine.Mod;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Engine.ComDriver.HGTECH
{
    /// <summary>
    /// 通过Win32与EzCad2软件交互控制
    /// </summary>
    public class sComEzCad2 : sComEzCad2<ServerNode>
    {
        public sComEzCad2() : base()
        {

        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="DrvItem"></param>
        public sComEzCad2(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {

        }
    }

    /// <summary>
    /// 通过Win32与EzCad2软件交互控制
    /// </summary>
    public partial class sComEzCad2<TConNode> : INotifyPropertyChanged
    {
        Process WorkProc = null;
        public IntPtr hwdEzcad2 = IntPtr.Zero;
        string AppExe = sCommon.GetStartUpPath() + @"\EzCad2\EzCad2.exe";
        string AppProccessName = "EzCad2";
        string ProjectFileName = sCommon.GetStartUpPath() + $@"\Model\Model.ezd";
        string ExCadVersion = "2.14.10";
        string PrintTextFileName = sCommon.GetStartUpPath() + $@"\Locales\PrintModel.txt";
        public event Action<object, string> MarkStateChanged;

        #region 属性
        /// 初始化完成
        /// </summary>
        public bool InitSuccessed { get; set; }

        /// <summary>
        /// 正在标刻
        /// </summary>
        public bool Marking { get; set; }

        /// <summary>
        /// 正在初始化
        /// </summary>
        public bool Initializing { get; set; }

        /// <summary>
        /// 打印内容
        /// </summary>
        public string PrintContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性发生改变时调用该方法发出通知
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// 反射所有属性并通知
        /// </summary>
        /// <returns></returns>
        public void RaisePropertyNotify()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                // 属性Error和Item来自接口IDataErrorInfo，无需进行验证
                if (pi.Name == "Error" || pi.Name == "Item")
                    continue;
                RaisePropertyChanged(pi.Name);
            }
        }
        #endregion

        /// <summary>
        /// 创建EzCad句柄
        /// </summary>
        /// <param name="ModelFileName">sCommon.GetStartUpPath() + $@"\Data\{strPgjName}"</param>
        /// <param name="PrjName">Model.ezd</param>
        /// <param name="ProcessName"></param>
        /// <param name="EzCadVersion"></param>
        /// <returns></returns>
        public IntPtr CreateEzCadHandle()
        {
            if (Initializing)
                return IntPtr.Zero;
            Initializing = true;
            this.RaisePropertyNotify();
            WorkProc = sCommon.GetProcessWithStartFile(AppProccessName, AppExe, ProjectFileName);
            if (WorkProc == null)
                return IntPtr.Zero;
            //WorkProc.WaitForInputIdle(); //没有图形界面的不能用这个方法，会报错
            WorkProc.MainWindowHandle.HideWindow();
            IntPtr hwdMainWindow = IntPtr.Zero;
            while (hwdMainWindow == IntPtr.Zero)
            {
                WorkProc = sCommon.GetProcess(AppProccessName);
                //自动消灭弹窗，主要针对EzCad没有安装驱动时打开软件弹窗
                sCommon.SearchPopWindow(out IntPtr posHwd, "确定");
                if (WorkProc == null)
                {
                    Initializing = false;
                    InitSuccessed = false;
                    return IntPtr.Zero;
                }
                string PrjName = ProjectFileName.GetShortFileName(true);
                hwdMainWindow = sCommon.FindWindow(null, $"EzCad{ExCadVersion} - {PrjName}");
                if (hwdMainWindow != IntPtr.Zero)
                    hwdMainWindow.ShowWindow(0);
                Thread.Sleep(500);
            }
            Initializing = false;
            InitSuccessed = true;
            hwdEzcad2 = hwdMainWindow;
            hwdMainWindow.SetForegroundWindow();
            this.RaisePropertyNotify();
            return hwdMainWindow;
        }

        /// <summary>
        /// 将窗口挂载到父句柄显示
        /// </summary>
        /// <param name="hwd"></param>
        /// <param name="Parent"></param>
        public void SetWindowAndMountToParent(IntPtr Parent)
        {
            try
            {
                if (hwdEzcad2 == IntPtr.Zero)
                    return;
                //禁用关闭按钮
                hwdEzcad2.DisableCloseButton();
                //除去窗体边框，包括标题栏
                //hwd.RemoveWindowBorder();
                //窗体嵌入
                hwdEzcad2.MountToParent(Parent);
                hwdEzcad2.WindowMaxmized();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 启动标刻
        /// </summary>
        public bool StartMarker(out string ErrorMessage)
        {
            Marking = true;
            MarkStateChanged?.Invoke(this, "Marking"); AckStatus();
            this.RaisePropertyNotify();
            ErrorMessage = string.Empty;
            if (hwdEzcad2 == IntPtr.Zero)
            {
                ErrorMessage = "未加载EzCad2句柄";
                return false;
            }
            IntPtr MarkerButton = hwdEzcad2.SearchObjectHandle("13.2");
            MarkerButton.SendMessage_Click();
            this.RaisePropertyNotify();
            Marking = false;
            MarkStateChanged?.Invoke(this, "MarkFinish"); AckStatus();
            return true;
        }
    }

    /// <summary>
    /// 通信协议实现
    /// </summary>
    public partial class sComEzCad2<TConNode> : sComRuleAsc where TConNode : new()
    {
        private IDBFactory<TConNode> _DB = DbFactory.Current.GetConn<TConNode>(DbConKey.Task);
        public ReqField ReqField = new ReqField();
        public AckField AckField = new AckField();
        public SessionMemory Session { get; } = new SessionMemory();
        public string ModelFile { get; set; } = string.Empty;
        private string LastRcvStatusContent = string.Empty;
        /// <summary>
        /// 协议变量列表
        /// </summary>
        private List<ModelComHG> ListComSymbol;
        /// <summary>
        /// 报文字典
        /// </summary>
        public Dictionary<Enum_REQ, Telegram> _REQTelegrams = new Dictionary<Enum_REQ, Telegram>();
        public Dictionary<Enum_ACK, Telegram> _ACKTelegrams = new Dictionary<Enum_ACK, Telegram>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public sComEzCad2()
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
        public sComEzCad2(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
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
        /// 发送数据
        /// </summary>
        /// <param name="PrintText"></param>
        public void SendData(string PrintText)
        {
            string strTemp = string.Empty;
            List<string> LstPrint = PrintText.MySplit(",");
            foreach (string item in LstPrint)
            {
                if (!string.IsNullOrEmpty(strTemp))
                    strTemp += "\r\n";
                strTemp += item;
            }
            PrintContent = strTemp;
            sCommon.WriteFile(PrintTextFileName, LstPrint);
            Logger.Default["MES"].Write(LOG_TYPE.MESS, string.Format("[{0}]收到数据 ...", PrintText));
        }

        /// <summary>
        /// 启动打印
        /// </summary>
        public void StartPrint()
        {
            string PrintText =  sCommon.ReadFile(PrintTextFileName).ToMyString(",");
            Logger.Default["MES"].Write(LOG_TYPE.MESS, $"[{PrintText}]正在标刻...");
            StartMarker(out string Error);
            Logger.Default["MES"].Write(LOG_TYPE.MESS, $"[{PrintText}]标刻完成...");
        }

        /// <summary>
        /// PrintText=打印内容1,打印内容2
        /// </summary>
        /// <param name="PrintText"></param>
        public void Print(string ModelEzdFile, string PrintText)
        {
            Task.Factory.StartNew(() =>
            {
                string strContent = PrintText;
                Logger.Default["MES"].Write(LOG_TYPE.MESS, string.Format("[{0}]正在标刻...", strContent));
                List<string> LstPrint = PrintText.MySplit(",");
                string strTemp = string.Empty;
                foreach (string item in LstPrint)
                {
                    if (!string.IsNullOrEmpty(strTemp))
                        strTemp += "\r\n";
                    strTemp += item;
                }
                PrintContent = strTemp;
                AckStatus();
                sCommon.WriteFile(PrintTextFileName, LstPrint);
                StartMarker(out string Error);
                Logger.Default["MES"].Write(LOG_TYPE.MESS, string.Format("[{0}]标刻完成", strContent));
                AckStatus();
            });
        }

        private void Marker_StateChanged(object arg1, string arg2)
        {
            AckStatus();
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
            while (ComWorking)
            {
                try
                {
                    lock (_QueueRcv)
                    {
                        if (_QueueRcv.Count > 0)
                        {
                            string strMessage = _QueueRcv.Dequeue();
                            string strErrorMessage = ParseRecievedContent(strMessage);
                            if (DriverItem.ServerUpdEn == "1") TryParseBinder();
                        }
                        else
                        {
                            DateTime dtNow = DateTime.Now;
                            double iTimeSpan = (dtNow - dtStart).TotalSeconds;
                            if (iTimeSpan > 5)
                            {
                                TryParseBinder();
                                dtStart = DateTime.Now;
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                Thread.Sleep(DriverItem.ComParam.CycleTime);
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
        protected virtual string ParseRecievedContent(string strMessage)
        {
            string ErrorMessage = string.Empty;
            string strMessKey = strMessage.MidString("@", "@");
            string AckContent = string.Empty;
            switch (strMessKey)
            {
                case "GET_MARKER_STATUS":
                    AckStatus();
                    break;
                case "REGISTER":
                    //@REGISTER@Fields=打印内容1,打印内容2@StartNow=Y@ENDE
                    string strFields = strMessage.MidString("@Fields=", "@");
                    string strStartNow = strMessage.MidString("@StartNow=", "@");
                    if (strStartNow == "Y")
                    {
                        AckField._Registered_Fields = strFields;
                        ACK(Enum_ACK.Registered, out AckContent);
                        Print(ProjectFileName, strFields);
                    }
                    else if (strStartNow == "N")
                    {
                        SendData(strFields);
                    }
                    break;
                case "PROCESS_START":
                    StartPrint();
                    break;
            }
            return ErrorMessage;
        }

        private void AckStatus()
        {
            string AckContent = string.Empty;
            AckField._Status_Initializing = Initializing ? "1" : "0";
            AckField._Status_InitSuccessed = InitSuccessed ? "1" : "0";
            AckField._Status_Marking = Marking ? "1" : "0";
            ACK(Enum_ACK.Marker_Status, out AckContent);
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
                List<string> FuncParams = FuncParamField.MySplit(",", null, null, false, true);
                return this.InvokeMethod(FuncName, FuncParams.ToArray());
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
            //Session.State_Alarm = (Session.Error_MeetOverTime == "1" || Session.Error_PrintError == "1") ? "1" : "0";
            //Session.State_Offline = ComState == SocketState.Connected ? "1" : "0";
            //if (Session.State_Offline == "1" || Session.State_Alarm == "1")
            //{
            //    Session.State_Normal = "0";
            //    Session.State_Run = "0";
            //    Session.State_Done = "0";
            //}
            //else
            //{
            //    if (Session.ProcStep == "打印就绪")
            //    {
            //        Session.State_Run = "0";
            //        Session.State_Normal = "1";
            //    }
            //    else if (!string.IsNullOrEmpty(Session.ProcStep))
            //    {
            //        Session.State_Run = "1";
            //        Session.State_Normal = "0";
            //    }
            //}
            //Session.RaisePropertyNotify();
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
    /// 操作接口实现
    /// </summary>
    public partial class sComEzCad2<TConNode> : IComPrinter
    {
        /// <summary>
        /// 打印标签 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <returns></returns>
        public bool PrintLabel(string PrnFile, string[] FieldContent)
        {
            return true;
        }
        /// <summary>
        /// 打印标签 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <param name="CopyCount">打印份数</param>
        /// <returns></returns>
        public bool PrintLabel(string PrnFile, string[] FieldContent, int CopyCount)
        {
            return true;
        }
    }
}
